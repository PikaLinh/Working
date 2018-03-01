using EntityModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ViewModels;

namespace WebUI.Controllers
{
    public class Master_ChicCut_HairTypeController : BaseController
    {
        // GET: Master_ChicCut_HairType
        
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _SearchPartial(Master_ChicCut_HairTypeSeachModel model)
        {
            var list = _context.Master_ChicCut_HairTypeModel
                        .Where(p => (model.HairTypeName == null || p.HairTypeName.Contains(model.HairTypeName)) &&
                                  (model.OrderIndex == null|| p.OrderIndex == model.OrderIndex) &&
                                  (model.ActivedHairType == null || p.Actived == model.ActivedHairType))
                        .ToList();
            return PartialView(list);
        }

        
        public ActionResult Create()
        {
            Master_ChicCut_HairTypeModel model = new Master_ChicCut_HairTypeModel();
            model.Actived = true;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Master_ChicCut_HairTypeModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.OrderIndex <= 0)
                {
                    ModelState.AddModelError("LonHon0", new Exception("Vui lòng nhập thứ tự lớn hơn 0"));
                    return View(model);
                }
                _context.Master_ChicCut_HairTypeModel.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        
        public ActionResult Details(int id = 0)
        {
            Master_ChicCut_HairTypeModel model = _context.Master_ChicCut_HairTypeModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        
        public ActionResult Edit(int id = 0)
        {
            Master_ChicCut_HairTypeModel model = _context.Master_ChicCut_HairTypeModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Master_ChicCut_HairTypeModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.OrderIndex <= 0)
                {
                    ModelState.AddModelError("LonHon0", new Exception("Vui lòng nhập thứ tự lớn hơn 0"));
                    return View(model);
                }
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }


    }
}