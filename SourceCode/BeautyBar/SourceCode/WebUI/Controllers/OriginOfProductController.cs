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
    public class OriginOfProductController : Controller
    {
        private EntityDataContext db = new EntityDataContext();
        //
        // GET: /OriginOfProduct/
        
        public ActionResult Index()
        {
            return View(db.OriginOfProductModel.OrderBy(p =>p.OriginOfProductName).ToList());
        }

        //
        // GET: /OriginOfProduct/Details
        
        public ActionResult Details(int id = 0)
        {
            OriginOfProductModel OriginOfProduct = db.OriginOfProductModel.Find(id);
            if (OriginOfProduct == null)
            {
                return HttpNotFound();
            }
            return View(OriginOfProduct);
        }
        //
        // GET: /OriginOfProduct/Create


        
        public ActionResult Create()
        {
            OriginOfProductModel OriginOfProduct = new OriginOfProductModel();
            OriginOfProduct.Actived = true;
            return View(OriginOfProduct);
        }

        //
        // POST: /OriginOfProduct/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OriginOfProductModel OriginOfProduct)
        {
            if (ModelState.IsValid)
            {
                db.OriginOfProductModel.Add(OriginOfProduct);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(OriginOfProduct);
        }


        //
        // GET: /OriginOfProduct/Edit/5
        
        public ActionResult Edit(int id = 0)
        {
            OriginOfProductModel OriginOfProduct = db.OriginOfProductModel.Find(id);
            if (OriginOfProduct == null)
            {
                return HttpNotFound();
            }
            return View(OriginOfProduct);
        }

        //
        // POST: /OriginOfProduct/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OriginOfProductModel OriginOfProduct)
        {
            if (ModelState.IsValid)
            {
                db.Entry(OriginOfProduct).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(OriginOfProduct);
        }
    }
}
