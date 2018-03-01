using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using Repository;

namespace WebUI.Controllers
{
    public class SteelFIController : Controller
    {
        private EntityDataContext db = new EntityDataContext();

        //
        // GET: /SteelFI/
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Index()
        {
            if (Session["acc"] == null)
            {
                Response.Redirect("/Account/Login");
                return null;
            }
            return View(db.SteelFIModel.ToList());
        }

        //
        // GET: /SteelFI/Details/5
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Details(int id = 0)
        {
            if (Session["acc"] == null)
            {
                Response.Redirect("/Account/Login");
                return null;
            }
            SteelFIModel steelfimodel = db.SteelFIModel.Find(id);
            if (steelfimodel == null)
            {
                return HttpNotFound();
            }
            return View(steelfimodel);
        }

        //
        // GET: /SteelFI/Create
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
        // POST: /SteelFI/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SteelFIModel steelfimodel)
        {
            if (ModelState.IsValid)
            {
                db.SteelFIModel.Add(steelfimodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(steelfimodel);
        }

        //
        // GET: /SteelFI/Edit/5
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Edit(int id = 0)
        {
            if (Session["acc"] == null)
            {
                Response.Redirect("/Account/Login");
                return null;
            }
            SteelFIModel steelfimodel = db.SteelFIModel.Find(id);
            if (steelfimodel == null)
            {
                return HttpNotFound();
            }
            return View(steelfimodel);
        }

        //
        // POST: /SteelFI/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SteelFIModel steelfimodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(steelfimodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(steelfimodel);
        }

        public ActionResult GetByMarkId(int id)
        {
            SteelFIReposytory _repository = new SteelFIReposytory(db);
            var result = _repository.GetByMarkId(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}