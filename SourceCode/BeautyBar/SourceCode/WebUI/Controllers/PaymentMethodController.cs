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
    public class PaymentMethodController : Controller
    {
        private EntityDataContext db = new EntityDataContext();

        //
        // GET: /PaymentMethod/
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index()
        {
            return View(db.PaymentMethodModel.ToList());
        }

        //
        // GET: /PaymentMethod/Details/5
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Details(int id = 0)
        {
            PaymentMethodModel paymentmethodmodel = db.PaymentMethodModel.Find(id);
            if (paymentmethodmodel == null)
            {
                return HttpNotFound();
            }
            return View(paymentmethodmodel);
        }

        //
        // GET: /PaymentMethod/Create
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /PaymentMethod/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PaymentMethodModel paymentmethodmodel)
        {
            if (ModelState.IsValid)
            {
                db.PaymentMethodModel.Add(paymentmethodmodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(paymentmethodmodel);
        }

        //
        // GET: /PaymentMethod/Edit/5
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Edit(int id = 0)
        {
            PaymentMethodModel paymentmethodmodel = db.PaymentMethodModel.Find(id);
            if (paymentmethodmodel == null)
            {
                return HttpNotFound();
            }
            return View(paymentmethodmodel);
        }

        //
        // POST: /PaymentMethod/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PaymentMethodModel paymentmethodmodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(paymentmethodmodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(paymentmethodmodel);
        }

        //
        // GET: /PaymentMethod/Delete/5
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        
        public ActionResult Delete(int id = 0)
        {
            PaymentMethodModel paymentmethodmodel = db.PaymentMethodModel.Find(id);
            if (paymentmethodmodel == null)
            {
                return HttpNotFound();
            }
            return View(paymentmethodmodel);
        }

        //
        // POST: /PaymentMethod/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PaymentMethodModel paymentmethodmodel = db.PaymentMethodModel.Find(id);
            db.PaymentMethodModel.Remove(paymentmethodmodel);
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