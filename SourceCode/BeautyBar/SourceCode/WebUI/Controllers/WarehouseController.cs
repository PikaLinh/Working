using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;

namespace WebUI.Controllers
{
    public class WarehouseController : Controller
    {
        //
        // GET: /Warehouse/
        EntityDataContext db = new EntityDataContext();
        #region danh sách kho
        
        public ActionResult Index()
        {
            return View(db.WarehouseModel.OrderBy(p => p.WarehouseId).ToList());
        }
        #endregion

        #region chi tiết kho
        
        public ActionResult Details(int id)
        {
            WarehouseModel model = db.WarehouseModel.Find(id);
            if( model==null)
            {
                return HttpNotFound();
            };
            return View (model);
        }
        #endregion

        #region Thêm mới kho
        
        public ActionResult Create()
        {
            WarehouseModel model = new WarehouseModel();
            model.Actived = true;
            CreateViewBag();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WarehouseModel model)
        {
            if(ModelState.IsValid)
            {
                db.WarehouseModel.Add(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                CreateViewBag();
                return RedirectToAction("Index");
            };
            return View(model);
        }
        #endregion

        #region Sửa thông tin kho
        
        public ActionResult Edit(int id)
        {
            WarehouseModel model = db.WarehouseModel.Find(id);
            CreateViewBag(model.ProvinceId,model.DistrictId,model.StoreId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WarehouseModel model)
        {
            if(ModelState.IsValid)
            {
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                CreateViewBag();
                return RedirectToAction("Index");
            };
            return View(model);
        }
        #endregion

        #region helper
        public ActionResult GetDistrictBy(int ProvinceId)
        {
            var districts = db.DistrictModel
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

        private void CreateViewBag(int? ProvinceId = null, int? DistrictId = null, int? StoreId=null)
        {
            //ProvinceId
            var provinces = db.ProvinceModel.OrderBy(p => p.ProvinceName).ToList();
            ViewBag.ProvinceId = new SelectList(provinces, "ProvinceId", "ProvinceName", ProvinceId);

            //DistrictId
            var districts = db.DistrictModel
                                    .Where(p => p.ProvinceId == ProvinceId)
                                    .OrderBy(p => p.DistrictName)
                                    .Select(p => new
                                    {
                                        Id = p.DistrictId,
                                        Name = p.Appellation + " " + p.DistrictName
                                    }).ToList();
            ViewBag.DistrictId = new SelectList(districts, "Id", "Name", DistrictId);

            // StoreId
            var StoreList = db.StoreModel.Where(p=>p.Actived==true).OrderBy(p => p.StoreName).ToList();
            ViewBag.StoreId = new SelectList(StoreList, "StoreId", "StoreName", StoreId);

        }
        #endregion

    }
}
