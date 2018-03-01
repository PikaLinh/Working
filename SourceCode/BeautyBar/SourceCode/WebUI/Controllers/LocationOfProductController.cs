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
    public class LocationOfProductController : Controller
    {
        //
        // GET: /LocationOfProduct/
        EntityDataContext db = new EntityDataContext();
        #region danh sách vị trí sản phẩm
        
        public ActionResult Index()
        {
            return View(db.LocationOfProductModel.OrderBy(p => p.LocationOfProductId).ToList());
        }
        #endregion

        #region Chi tiết vi trí sản phẩm
         
        public ActionResult Details(int id = 0)
        {
            LocationOfProductModel model = db.LocationOfProductModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        #endregion

        #region Thêm mới vị trí
         
        public ActionResult Create()
        {
            LocationOfProductModel model = new LocationOfProductModel();
            model.Actived = true;
            return View(model);
        }

        //
        // POST: /Role/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LocationOfProductModel model)
        {
            if (ModelState.IsValid)
            {
                db.LocationOfProductModel.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }


        #endregion

        #region Sửa vị trí sản phẩm
         
        public ActionResult Edit(int id = 0)
        {
            LocationOfProductModel model = db.LocationOfProductModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        //
        // POST: /Role/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LocationOfProductModel model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        #endregion
    }
}
