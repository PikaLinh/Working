using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using ViewModels;


namespace WebUI.Controllers
{
    public class StoreController : BaseController
    {
        //
        // GET: /Store/

        
        public ActionResult Index()
        {
            return View(_context.StoreModel.Where(p=>p.Actived == true).ToList());
        }

        
        public ActionResult Details(int id)
        {
            StoreModel model = _context.StoreModel.Where(p => p.StoreId == id).FirstOrDefault();
            return View(model);
        }
        #region  Thêm mới
        
        public ActionResult Create()
        {
            StoreModel model = new StoreModel() { Actived = true };
            CreateViewBag();
            return View(model);
        }
        [HttpPost]
        public ActionResult Create( StoreModel model)
        {
            if (ModelState.IsValid)
            {
                _context.StoreModel.Add(model);
                _context.SaveChanges();
            }
            else
            {
                CreateViewBag(model.ProvinceId, model.DistrictId);
            }
            return RedirectToAction("Index");
        }
        #endregion

        
        public ActionResult Edit(int id)
        {

            StoreModel model = _context.StoreModel.Where(p => p.StoreId == id).FirstOrDefault();
            CreateViewBag(model.ProvinceId, model.DistrictId);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(StoreModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
            }
            else
            {
                CreateViewBag(model.ProvinceId, model.DistrictId);
            };
            return RedirectToAction("Index");
        }
        #region CreateViewBag
        private void CreateViewBag(int? ProvinceId = null, int? DistrictId = null)
        {
            var provinces = _context.ProvinceModel.OrderBy(p => p.ProvinceName).ToList();
            ViewBag.ProvinceId = new SelectList(provinces, "ProvinceId", "ProvinceName", ProvinceId);

            var districts = _context.DistrictModel
                                    .Where(p => p.ProvinceId == ProvinceId)
                                    .OrderBy(p => p.DistrictName)
                                    .Select(p => new
                                    {
                                        Id = p.DistrictId,
                                        Name = p.Appellation + " " + p.DistrictName
                                    }).ToList();
            ViewBag.DistrictId = new SelectList(districts, "Id", "Name", DistrictId);

        }
        #endregion


        public ActionResult GetDistrictBy(int ProvinceId)
        {
            var districts = _context.DistrictModel
                                    .Where(p => p.ProvinceId == ProvinceId)
                                    .OrderBy(p => p.DistrictName)
                                    .Select(p => new
                                    {
                                        Id = p.DistrictId,
                                        Name = p.Appellation + " " + p.DistrictName
                                    }).ToList();

            //return Content("success");
            return Json(districts, JsonRequestBehavior.AllowGet);
        }

    }
}
