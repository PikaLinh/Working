using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using System.Data;
using System.Data.Entity;


namespace WebUI.Controllers
{
    public class CurrencyController : Controller
    {
        private EntityDataContext db = new EntityDataContext();
        //
        // GET: /Currency/

        
        public ActionResult Index()
        {

            return View(db.CurrencyModel.OrderBy(p=>p.CurrencyName).ToList());
        }


        //
        // GET: /Currency/Details/5
        
        public ActionResult Details(int id = 0)
        {
            CurrencyModel Currency = db.CurrencyModel.Find(id);
            if (Currency == null)
            {
                return HttpNotFound();
            }
            return View(Currency);
        }

        //
        // GET: /Currency/Create
        
        public ActionResult Create()
        {
            CurrencyModel Currency = new CurrencyModel();
            Currency.Actived = true;
            return View(Currency);
        }

        //
        // POST: /Currency/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CurrencyModel Currency)
        {
            if (ModelState.IsValid)
            {
                db.CurrencyModel.Add(Currency);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Currency);
        }

        //
        // GET: /Currency/Edit/5
        
        public ActionResult Edit(int id = 0)
        {
            CurrencyModel Currency = db.CurrencyModel.Find(id);
            if (Currency == null)
            {
                return HttpNotFound();
            }
            return View(Currency);
        }

        //
        // POST: /Currency/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CurrencyModel Currency)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Currency).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Currency);
        }


    }
}
