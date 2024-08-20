using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using iEPG.Entity;
using iEPG.Models;
using iEPG.RecordingReservationManager;
using Microsoft.VisualBasic;
using System.Threading;
using log4net;
using System.Text.RegularExpressions;

namespace iEPG
{
	public class iEPGController
	{
		private NameValueCollection _appSettings = ConfigurationManager.AppSettings;
		private iEPGDbContext dbContext = null;
		private readonly static string dateFormat = "yyyyMMddHHmm";
		private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public iEPGController()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		}

		public IEnumerable<TvProgram> GetTvPrograms(IEnumerable<FreeWordConditon> conditions)
		{
			var tvPrograms = new List<TvProgram>();
			using (var webClient = new WebClient())
			{
				conditions.ToList().ForEach((Action<FreeWordConditon>)(freeWordCondition =>
				{
					tvPrograms.AddRange(this.GetTvPrograms((WebClient)webClient, string.Format(this._appSettings["SearchTvProgramList"], Uri.EscapeDataString((string)freeWordCondition.FreeWord)), freeWordCondition.FreeWord));
				}));
			}
			return tvPrograms;
		}

		public IEnumerable<FreeWordConditon> GetFreeWordConditions()
		{
			using (this.dbContext = new iEPGDbContext())
			{
				return this.dbContext.FreeWordConditons.ToList();
			}
		}

		public void ReserveTargetTvPrograms(IEnumerable<TvProgram> targetTvPrograms)
		{
			var reservationRows = this.GenerateReservationRows(targetTvPrograms);
			var recordingReservationManager = new RecordingReservationManagerClient();
			reservationRows.ToList().ForEach(reservationRow =>
			{
				recordingReservationManager.SetRecordingReservation(reservationRow, false);
			});
		}

		public void SaveTvPrograms(iEPGDbContext dbContext, IEnumerable<TvProgram> tvPrograms)
		{
			foreach (var tvProgram in tvPrograms)
			{
				dbContext.TvPrograms.Add(tvProgram);
			}
		}

		public void MovePastTvPrograms(iEPGDbContext dbContext)
		{
			var pastTvPrograms = from tvProgram in dbContext.TvPrograms
								 where tvProgram.EndDateTime <= DateTime.Now
								 select tvProgram;
			pastTvPrograms.ToList().ForEach(pastTvProgram =>
			{
				dbContext.PastTvPrograms.Add(pastTvProgram.ConvertToPastTvProgram());
				dbContext.TvPrograms.Remove(pastTvProgram);
			});
		}

		private IEnumerable<TvProgram> GetTvPrograms(WebClient webClient, string path, string freeword)
		{
			webClient.Encoding = Encoding.UTF8;
			webClient.Headers.Add("User-Agent", @"Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.51 Safari/537.36");
			webClient.Headers[HttpRequestHeader.Cookie] = string.Format("gtv.stationAreaId={0};gtv.iepgType=0;", this._appSettings["stationAreaId"]);
			var htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(webClient.DownloadString(path));
			var tvPrograms = new List<TvProgram>();
			tvPrograms.AddRange(this.GetTvProgram(htmlDocument, freeword));
			var pageAnkerTags = this.GetPageAnkerTags(htmlDocument);
			pageAnkerTags.Select(pageAnker => HttpUtility.HtmlDecode(pageAnker.Attributes["href"].Value)).Distinct().ToList()
				.ForEach(href =>
				{
					htmlDocument = new HtmlDocument();
					htmlDocument.LoadHtml(webClient.DownloadString(this._appSettings["SearchHomeUrl"] + href));
					tvPrograms.AddRange(this.GetTvProgram(htmlDocument, freeword));
				});
			return tvPrograms;
		}

		private IEnumerable<HtmlNode> GetPageAnkerTags(HtmlDocument htmlDocument)
		{
			var pageAnkerTags = htmlDocument.DocumentNode.SelectNodes(@"//a[@class=""num""]");
			return pageAnkerTags ?? new HtmlNodeCollection(htmlDocument.DocumentNode);
		}

		private IEnumerable<TvProgram> GetTvProgram(HtmlDocument htmlDocument, string freeword)
		{
			if (htmlDocument.DocumentNode.SelectNodes(@"//div[@class=""utileList""]").FirstOrDefault().SelectNodes(@".//a") == null) return Enumerable.Empty<TvProgram>();
			return htmlDocument.DocumentNode.SelectNodes(@"//div[@class=""utileList""]").Union(htmlDocument.DocumentNode.SelectNodes(@"//div[@class=""utileList bl""]") ?? Enumerable.Empty<HtmlNode>())
				.Select(u => GenerateTvProgram(u.SelectNodes(@".//a")?.FirstOrDefault().Attributes["href"].Value.Replace(@"/schedule/", "").Replace(@".action", ""), u.SelectNodes(@".//a")?.FirstOrDefault().InnerText,
					Convert.ToInt32(Regex.Match(u.SelectNodes(@".//p[@class=""utileListProperty""]")?.FirstOrDefault()?.InnerText, @"\([0-9]+分\)").Value.Replace("(", "").Replace(")", "").Replace("分", "")), freeword)) ?? Enumerable.Empty<TvProgram>();
		}

		/// <summary>
		/// iEPGファイルの内容をもとにTvProgram Entityを生成
		/// </summary>
		/// <param name="programDetail">iEPGファイルの内容を</param>
		/// <returns>TvProgram Entity</returns>
		private TvProgram GenerateTvProgram(string scheduleId, string programTitle, int programLengthMinute, string freeword)
		{
			var tvProgram = new TvProgram();
			tvProgram.Id = Guid.NewGuid();
			var startDateAndTime = new DateAndTime();
			tvProgram.Station = scheduleId.Substring(0, 6);
			startDateAndTime.Year = Convert.ToInt32(scheduleId.Substring(6, 4));
			startDateAndTime.Month = Convert.ToInt32(scheduleId.Substring(10, 2));
			startDateAndTime.Day = Convert.ToInt32(scheduleId.Substring(12, 2));
			startDateAndTime.Hour = Convert.ToInt32(scheduleId.Substring(14, 2));
			startDateAndTime.Minute = Convert.ToInt32(scheduleId.Substring(16, 2));
			tvProgram.ProgramTitle = "【" + freeword + "】" + programTitle;
			tvProgram.StartDateTime = startDateAndTime.ConvertDateTime();
			tvProgram.EndDateTime = tvProgram.StartDateTime.AddMinutes(programLengthMinute);
			if (tvProgram.EndDateTime < tvProgram.StartDateTime) tvProgram.EndDateTime = tvProgram.EndDateTime.AddDays(1);
			return tvProgram;
		}

		private class DateAndTime
		{
			public int Year { get; set; }
			public int Month { get; set; }
			public int Day { get; set; }
			public int Hour { get; set; }
			public int Minute { get; set; }
			public DateTime ConvertDateTime()
			{
				return new DateTime(this.Year, this.Month, this.Day).Add(new TimeSpan(this.Hour, this.Minute, 0));
			}
		}

		private IEnumerable<T_RESERVATIONRowSerializer> GenerateReservationRows(IEnumerable<TvProgram> tvPrograms)
		{
			var reservationRowList = new List<T_RESERVATIONRowSerializer>();
			tvPrograms.ToList().ForEach(tvProgram =>
			{
				var reservationRow = new T_RESERVATIONRowSerializer()
				{
					STATION = tvProgram.Station,
					STATION_NAME = tvProgram.StationName,
					START_YYYYMMDDHHMM = tvProgram.StartDateTime.ToString(dateFormat),
					END_YYYYMMDDHHMM = tvProgram.EndDateTime.ToString(dateFormat),
					PROGRAM_TITLE = ReplaceInvalidChars(tvProgram.ProgramTitle),
					PROGRAM_SUBTITLE = ReplaceInvalidChars(tvProgram.ProgramSubTitle),
					PERFORMER = tvProgram.Performer,
					GENRE = Convert.ToInt64(tvProgram.Genre1),
					GENRESpecified = true,
					SUBGENRE = Convert.ToInt64(tvProgram.SubGenre1),
					SUBGENRESpecified = true,
					NOTE = tvProgram.Detail,
					OUTPUT_ERRORLOG = 1L,
					OUTPUT_ERRORLOGSpecified = true,
					EXIT_APPLICATION_AFTER_RECORDING = 1L,
					EXIT_APPLICATION_AFTER_RECORDINGSpecified = true,
					PREVIEW = 0L,
					PREVIEWSpecified = true,
					CLIENT_PC_NAME = System.Environment.MachineName,
				};
				reservationRowList.Add(reservationRow);
			});
			return reservationRowList;
		}

		private string ReplaceInvalidChars(string expression)
		{
			if (string.IsNullOrEmpty(expression)) return "";
			System.IO.Path.GetInvalidFileNameChars().ToList().ForEach(invalidFileNameChar =>
			{
				expression = expression.Replace(invalidFileNameChar.ToString(),
					Strings.StrConv(invalidFileNameChar.ToString(), VbStrConv.Wide));
			});
			return expression;
		}


	}
}
