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
    public class MenuController : Controller
    {
        //
        // GET: /Menu/
        private EntityDataContext db = new EntityDataContext();
         
        public ActionResult Index()
        {
            return View(db.MenuModel.OrderBy(p => p.OrderBy).ToList());
 
        }


        //
        // GET: /Role/Menu/
         
        public ActionResult Details(int id = 0)
        {
            MenuModel menumodel = db.MenuModel.Find(id);
            if (menumodel == null)
            {
                return HttpNotFound();
            }
            return View(menumodel);
        }


        //
        // GET: /Menu/Create
         
        public ActionResult Create()
        {
            MenuModel menumodel = new MenuModel();
            return View(menumodel);
           
        }


        //
        // POST: /Menu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MenuModel menumodel)
        {
            if (ModelState.IsValid)
            {
                db.MenuModel.Add(menumodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(menumodel);
        }

        //
        // GET: /Role/Edit/5
         
        public ActionResult Edit(int id = 0)
        {
            MenuModel menumodel = db.MenuModel.Find(id);
            if (menumodel == null)
            {
                return HttpNotFound();
            }
            return View(menumodel);
        }

        //
        // POST: /Role/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MenuModel menumodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(menumodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(menumodel);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        
    }
}
