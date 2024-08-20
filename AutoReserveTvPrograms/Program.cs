using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iEPG;
using iEPG.Entity;
using iEPG.Models;
using System.Diagnostics;
using log4net;
using log4net.Config;
using System.Net;

namespace AutoReserveTvPrograms
{
	class Program
	{
		static void Main(string[] args)
		{
			XmlConfigurator.Configure();
			try
			{
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
				var iEPGController = new iEPGController();
				var tvPrograms = iEPGController.GetTvPrograms(iEPGController.GetFreeWordConditions());
				//tvPrograms.ToList().ForEach(tvProgram =>
				//{
				//    Console.WriteLine(tvProgram.StationName + " " + tvProgram.ProgramTitle);
				//});
				using (var dbContext = new iEPGDbContext())
				{
					var alreadyReservedTvPrograms = dbContext.TvPrograms.ToList();
					var stillNotReservedTvPrograms =
						(from tvProgram in tvPrograms
						 where !alreadyReservedTvPrograms.Exists(alreadyReservedTvProgram =>
							 alreadyReservedTvProgram.ProgramId == tvProgram.ProgramId
							 && alreadyReservedTvProgram.Station == tvProgram.Station
							 && alreadyReservedTvProgram.StartDateTime == tvProgram.StartDateTime)
						 select tvProgram).ToList();
					EventLog.WriteEntry("AutoReserveTvPrograms", "正常終了");
					Console.WriteLine("正常終了");
					iEPGController.ReserveTargetTvPrograms(stillNotReservedTvPrograms);
					iEPGController.SaveTvPrograms(dbContext, stillNotReservedTvPrograms);
					iEPGController.MovePastTvPrograms(dbContext);
					dbContext.SaveChanges();

				}
			}
			catch (Exception ex)
			{
				EventLog.WriteEntry("AutoReserveTvPrograms", string.Format("異常終了 {0}", ex));
				Console.WriteLine("異常終了");
				Console.ReadLine();
				throw;
			}

		}
	}
}
