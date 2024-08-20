using iEPG.Entity;
using iEPG.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
namespace iEPGMVC.Controllers
{
    public class FreeWordConditionController : Controller
    {
        private iEPGDbContext db;

        public FreeWordConditionController()
        {
            this.db = new iEPGDbContext();
        }

        public ActionResult Create()
        {
            return base.View();
        }

        [HttpPost]
        public ActionResult Create(FreeWordConditon freewordconditon)
        {
            if (!base.ModelState.IsValid)
            {
                return base.View(freewordconditon);
            }
            else
            {
                freewordconditon.Id = Guid.NewGuid();
                this.db.FreeWordConditons.Add(freewordconditon);
                this.db.SaveChanges();
                return base.RedirectToAction("Index");
            }
        }

        public ActionResult Delete(Guid id)
        {
            object[] objArray = new object[1];
            objArray[0] = id;
            FreeWordConditon freeWordConditon = this.db.FreeWordConditons.Find(objArray);
            if (freeWordConditon != null)
            {
                return base.View(freeWordConditon);
            }
            else
            {
                return base.HttpNotFound();
            }
        }

        [ActionName("Delete")]
        [HttpPost]
        public ActionResult DeleteConfirmed(Guid id)
        {
            object[] objArray = new object[1];
            objArray[0] = id;
            FreeWordConditon freeWordConditon = this.db.FreeWordConditons.Find(objArray);
            this.db.FreeWordConditons.Remove(freeWordConditon);
            this.db.SaveChanges();
            return base.RedirectToAction("Index");
        }

        public ActionResult Details(Guid id)
        {
            object[] objArray = new object[1];
            objArray[0] = id;
            FreeWordConditon freeWordConditon = this.db.FreeWordConditons.Find(objArray);
            if (freeWordConditon != null)
            {
                return base.View(freeWordConditon);
            }
            else
            {
                return base.HttpNotFound();
            }
        }

        protected override void Dispose(bool disposing)
        {
            this.db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Edit(Guid id)
        {
            object[] objArray = new object[1];
            objArray[0] = id;
            FreeWordConditon freeWordConditon = this.db.FreeWordConditons.Find(objArray);
            if (freeWordConditon != null)
            {
                return base.View(freeWordConditon);
            }
            else
            {
                return base.HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult Edit(FreeWordConditon freewordconditon)
        {
            if (!base.ModelState.IsValid)
            {
                return base.View(freewordconditon);
            }
            else
            {
                this.db.Entry<FreeWordConditon>(freewordconditon).State = EntityState.Modified;
                this.db.SaveChanges();
                return base.RedirectToAction("Index");
            }
        }

        public ActionResult Index()
        {
            return base.View(this.db.FreeWordConditons.ToList<FreeWordConditon>());
        }
    }

}