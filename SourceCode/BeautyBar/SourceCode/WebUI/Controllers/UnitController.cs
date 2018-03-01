using EntityModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WebUI.Controllers
{
    public class UnitController : BaseController
    {
        // GET: Unit
        
        public ActionResult Index()
        {

            return View(_context.UnitModel.OrderBy(p => p.UnitName).ToList());
        }

        //
        
        public ActionResult Details(int id = 0)
        {
            UnitModel model = _context.UnitModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        //


        
        public ActionResult Create()
        {
            UnitModel model = new UnitModel();
            model.Actived = true;
            return View(model);
        }

        //

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UnitModel model)
        {
            if (ModelState.IsValid)
            {
                _context.UnitModel.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }


        //
        
        public ActionResult Edit(int id = 0)
        {
            UnitModel model = _context.UnitModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        //

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UnitModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}