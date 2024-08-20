using iEPG.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iEPGMVC.Models
{
	public class ReserveTvProgramsModel
	{
		public string SearchWord { get; set; }
		public TvProgram[] SearchedTvPrograms { get; set; }
	}
}