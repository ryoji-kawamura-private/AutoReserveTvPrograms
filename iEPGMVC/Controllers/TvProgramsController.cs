using iEPG;
using iEPG.Entity;
using iEPGMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iEPGMVC.Controllers
{
    public class TvProgramsController : Controller
    {
		[HttpGet]
		public ActionResult Reserve()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Search(ReserveTvProgramsModel model)
		{
			var iEPG = new iEPGController();
			model.SearchedTvPrograms = iEPG.GetTvPrograms(new FreeWordConditon[] { new FreeWordConditon() { FreeWord = model.SearchWord } }).ToArray();
			return View("Reserve", model);
        }

		[HttpPost]
		public ActionResult Reserve(ReserveTvProgramsModel model)
		{
			var iEPG = new iEPGController();
			iEPG.ReserveTargetTvPrograms(model.SearchedTvPrograms.Where(t => t.Selected));
			return View();
		}
	}
}