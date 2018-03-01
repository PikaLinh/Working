using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;

namespace WebUI.Controllers
{
    public class SteelMarkController : Controller
    {
        private EntityDataContext db = new EntityDataContext();

        //
        // GET: /SteelMark/
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Index()
        {
            if (Session["acc"] == null)
            {
                Response.Redirect("/Account/Login");
                return null;
            }
            return View(db.SteelMarkModel.ToList());
        }

        //
        // GET: /SteelMark/Details/5
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Details(int id = 0)
        {
            if (Session["acc"] == null)
            {
                Response.Redirect("/Account/Login");
                return null;
            }
            SteelMarkModel steelmarkmodel = db.SteelMarkModel.Find(id);
            if (steelmarkmodel == null)
            {
                return HttpNotFound();
            }
            return View(steelmarkmodel);
        }

        //
        // GET: /SteelMark/Create
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Create()
        {
            if (Session["acc"] == null)
            {
                Response.Redirect("/Account/Login");
                return null;
            }
            return View();
        }

        //
        // POST: /SteelMark/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SteelMarkModel steelmarkmodel)
        {
            if (ModelState.IsValid)
            {
                db.SteelMarkModel.Add(steelmarkmodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(steelmarkmodel);
        }

        //
        // GET: /SteelMark/Edit/5
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Edit(int id = 0)
        {
            if (Session["acc"] == null)
            {
                Response.Redirect("/Account/Login");
                return null;
            }
            SteelMarkModel steelmarkmodel = db.SteelMarkModel.Find(id);
            if (steelmarkmodel == null)
            {
                return HttpNotFound();
            }
            return View(steelmarkmodel);
        }

        //
        // POST: /SteelMark/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SteelMarkModel steelmarkmodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(steelmarkmodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(steelmarkmodel);
        }

        //
        // GET: /SteelMark/Delete/5
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Delete(int id = 0)
        {
            SteelMarkModel steelmarkmodel = db.SteelMarkModel.Find(id);
            if (steelmarkmodel == null)
            {
                return HttpNotFound();
            }
            return View(steelmarkmodel);
        }

        //
        // POST: /SteelMark/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SteelMarkModel steelmarkmodel = db.SteelMarkModel.Find(id);
            db.SteelMarkModel.Remove(steelmarkmodel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}