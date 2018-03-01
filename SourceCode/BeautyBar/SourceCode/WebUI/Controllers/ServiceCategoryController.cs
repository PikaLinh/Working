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
    public class ServiceCategoryController : BaseController
    {
        // GET: ServiceCategory
        #region Danh sách loại dịch vụ
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _SearchPartial(ServiceCategoryViewModel model)
        {
            var list = (from sc in _context.Master_ChicCut_ServiceCategoryModel
                        join spc in _context.Master_ChicCut_ServiceParentCategoryModel on sc.ServiceParentCategoryId equals spc.ServiceParentCategoryId into Service
                        from sv in Service.DefaultIfEmpty()
                        orderby sc.ServiceCategoryId
                        select new ServiceCategoryViewModel()
                        {
                            ServiceCategoryId = sc.ServiceCategoryId,
                            ServiceName = sc.ServiceName,
                            ServiceParentCategoryName = sv.ServiceParentCategoryName,
                            OrderBy = sc.OrderBy,
                            Actived = sc.Actived,
                            Type = sc.Type
                        })
                        .ToList();
            return PartialView(list);
        }
        #endregion

        #region Thêm mới loại dịch vụ
        public ActionResult Create()
        {
            Master_ChicCut_ServiceCategoryModel model = new Master_ChicCut_ServiceCategoryModel();
            model.Actived = true;
            CreateViewBag(null);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Master_ChicCut_ServiceCategoryModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.OrderBy <= 0)
                {
                    ModelState.AddModelError("LonHon0", new Exception("Vui lòng nhập thứ tự lớn hơn 0"));
                    return View(model);
                }
                _context.Master_ChicCut_ServiceCategoryModel.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }
        #endregion

        #region Xem chi tiết loại dịch vụ
        public ActionResult Details(int id)
        {
            //Master_ChicCut_ServiceCategoryModel model = _context.Master_ChicCut_ServiceCategoryModel.Find(id);
            var model = (from sc in _context.Master_ChicCut_ServiceCategoryModel
                         join spc in _context.Master_ChicCut_ServiceParentCategoryModel on sc.ServiceParentCategoryId equals spc.ServiceParentCategoryId into Service
                         from sv in Service.DefaultIfEmpty()
                         where sc.ServiceCategoryId == id
                         select new ServiceCategoryViewModel()
                         {
                             ServiceCategoryId = sc.ServiceCategoryId,
                             ServiceName = sc.ServiceName,
                             ServiceParentCategoryName = sv.ServiceParentCategoryName,
                             OrderBy = sc.OrderBy,
                             Actived = sc.Actived,
                         })
                        .FirstOrDefault();
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        #endregion

        #region Sửa loại dịch vụ
        public ActionResult Edit(int id)
        {
            Master_ChicCut_ServiceCategoryModel model = _context.Master_ChicCut_ServiceCategoryModel.Find(id);

            if (model == null)
            {
                return HttpNotFound();
            }
            CreateViewBag(model.ServiceParentCategoryId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Master_ChicCut_ServiceCategoryModel model)
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

        #region Helper
        private void CreateViewBag(int? ServiceParentCategoryId = null)
        {
            //Danh mục dịch vụ
            var ServiceCategoryLst = _context.Master_ChicCut_ServiceParentCategoryModel.OrderBy(p => p.OrderBy).Where(p => p.Actived == true).ToList();
            ViewBag.ServiceParentCategoryId = new SelectList(ServiceCategoryLst, "ServiceParentCategoryId", "ServiceParentCategoryName", ServiceParentCategoryId);
        }
        #endregion
    }
}