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
    public class LocationController : Controller
    {
        private EntityDataContext db = new EntityDataContext();

        //
        // GET: /Location/

        
        public ActionResult Index()
        {
            return View(db.LocationModel.ToList());
        }

        //
        // GET: /Location/Details/5
        
        public ActionResult Details(int id = 0)
        {
            LocationModel locationmodel = db.LocationModel.Find(id);
            if (locationmodel == null)
            {
                return HttpNotFound();
            }
            return View(locationmodel);
        }

        //
        // GET: /Location/Create
        
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Location/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LocationModel locationmodel)
        {
            if (ModelState.IsValid)
            {
                db.LocationModel.Add(locationmodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(locationmodel);
        }

        //
        // GET: /Location/Edit/5
        
        public ActionResult Edit(int id = 0)
        {
            LocationModel locationmodel = db.LocationModel.Find(id);
            if (locationmodel == null)
            {
                return HttpNotFound();
            }
            return View(locationmodel);
        }

        //
        // POST: /Location/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LocationModel locationmodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(locationmodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(locationmodel);
        }

        //
        // GET: /Location/Delete/5
        
        public ActionResult Delete(int id = 0)
        {
            LocationModel locationmodel = db.LocationModel.Find(id);
            if (locationmodel == null)
            {
                return HttpNotFound();
            }
            return View(locationmodel);
        }

        //
        // POST: /Location/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LocationModel locationmodel = db.LocationModel.Find(id);
            db.LocationModel.Remove(locationmodel);
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