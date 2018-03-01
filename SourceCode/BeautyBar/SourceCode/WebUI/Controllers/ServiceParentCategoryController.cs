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
    public class ServiceParentCategoryController : BaseController
    {
        // GET: ServiceCategory
        #region Danh sách danh mục loại dịch vụ
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _SearchPartial(Master_ChicCut_ServiceParentCategoryModel model)
        {
            var list = _context.Master_ChicCut_ServiceParentCategoryModel
                        .OrderBy(p => p.ServiceParentCategoryId)
                        .ToList();
            //var list = (from m in _context.Master_ChicCut_ServiceParentCategoryModel
            //            where (model.ServiceParentCategoryName == null || m.ServiceParentCategoryName.Contains(model.ServiceParentCategoryName)) &&
            //                       (model.OrderBy == null || m.OrderBy == model.OrderBy) &&
            //                       (model.Actived == null || m.Actived == model.Actived)
            //            select new ServiceParentCategoryViewModel
            //            {
            //                ServiceParentCategoryName = m.ServiceParentCategoryName,
            //                OrderBy = m.OrderBy,
            //                Actived = m.Actived
            //            })
            //            .ToList();
            return PartialView(list);
        }
        #endregion

        #region Thêm mới danh mục loại dịch vụ
        public ActionResult Create()
        {
            Master_ChicCut_ServiceParentCategoryModel model = new Master_ChicCut_ServiceParentCategoryModel();
            model.Actived = true;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Master_ChicCut_ServiceParentCategoryModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.OrderBy <= 0)
                {
                    ModelState.AddModelError("LonHon0", new Exception("Vui lòng nhập thứ tự lớn hơn 0"));
                    return View(model);
                }
                _context.Master_ChicCut_ServiceParentCategoryModel.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }
        #endregion

        #region Xem chi tiết danh mục loại dịch vụ
        public ActionResult Details(int id = 0)
        {
            Master_ChicCut_ServiceParentCategoryModel model = _context.Master_ChicCut_ServiceParentCategoryModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        #endregion

        #region Sửa danh mục loại dịch vụ
        public ActionResult Edit(int id = 0)
        {
            Master_ChicCut_ServiceParentCategoryModel model = _context.Master_ChicCut_ServiceParentCategoryModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Master_ChicCut_ServiceParentCategoryModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.OrderBy <= 0)
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
        #endregion
    }
}