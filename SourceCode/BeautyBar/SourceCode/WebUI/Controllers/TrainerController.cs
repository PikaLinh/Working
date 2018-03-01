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
    public class TrainerController : Controller
    {
        private EntityDataContext db = new EntityDataContext();

        //
        // GET: /Trainer/

        public ActionResult Index()
        {
            return View(db.TrainerModel.ToList());
        }

        //
        // GET: /Trainer/Details/5

        public ActionResult Details(int id = 0)
        {
            TrainerModel trainermodel = db.TrainerModel.Find(id);
            if (trainermodel == null)
            {
                return HttpNotFound();
            }
            return View(trainermodel);
        }

        //
        // GET: /Trainer/Create

        public ActionResult Create()
        {
            TrainerModel trainermodel = new TrainerModel();
            trainermodel.Actived = true;
            return View(trainermodel);
        }

        //
        // POST: /Trainer/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TrainerModel trainermodel)
        {
            if (ModelState.IsValid)
            {
                db.TrainerModel.Add(trainermodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(trainermodel);
        }

        //
        // GET: /Trainer/Edit/5

        public ActionResult Edit(int id = 0)
        {
            TrainerModel trainermodel = db.TrainerModel.Find(id);
            if (trainermodel == null)
            {
                return HttpNotFound();
            }
            return View(trainermodel);
        }

        //
        // POST: /Trainer/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TrainerModel trainermodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trainermodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trainermodel);
        }

        //
        // GET: /Trainer/Delete/5

        public ActionResult Delete(int id = 0)
        {
            TrainerModel trainermodel = db.TrainerModel.Find(id);
            if (trainermodel == null)
            {
                return HttpNotFound();
            }
            return View(trainermodel);
        }

        //
        // POST: /Trainer/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TrainerModel trainermodel = db.TrainerModel.Find(id);
            db.TrainerModel.Remove(trainermodel);
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