using EntityModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WebUI.Controllers
{
    public class AM_AccountController : BaseController
    {
        //
        // GET: /AM_Account/

        #region Danh sách
        public ActionResult Index()
        {
            var list = _context.AM_AccountModel.Where(p => p.Actived == true && p.AMAccountTypeCode != "CONGNO").OrderBy(p => p.StoreId).ThenBy(p => p.Code).ToList();
            return View(list);
        }
        #endregion

        #region Xem chi tiết
        public ActionResult Details(int id)
        {
            AM_AccountModel model = _context.AM_AccountModel.Find(id);
            return View(model);
        }
        #endregion

        #region Thêm mới
        public ActionResult Create()
        {
            AM_AccountModel model = new AM_AccountModel();
            model.Actived = true;
            CreateViewBag();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AM_AccountModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            CreateViewBag();
            return View(model);
        }
        #endregion

        #region Cập nhật
        public ActionResult Edit(int id)
        {
            AM_AccountModel model = _context.AM_AccountModel.Find(id);
            CreateViewBag(model.StoreId, model.AMAccountTypeCode);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AM_AccountModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            CreateViewBag();
            return View(model);
        }
        #endregion

        #region CreateViewBag
        private void CreateViewBag(int? StoreId = null, string AMAccountTypeCode = null)
        {
            var listStore = _context.StoreModel.OrderBy(p => p.StoreName).Where(p => p.Actived == true).ToList();
            ViewBag.StoreId = new SelectList(listStore, "StoreId", "StoreName", StoreId);

            var listAM_AccountType = _context.AM_AccountTypeModel.OrderBy(p => p.AMAccountTypeCode).Where(p => p.AMAccountTypeCode != "CONGNO").ToList();
            ViewBag.AMAccountTypeCode = new SelectList(listAM_AccountType, "AMAccountTypeCode", "AMAccountTypeCode", AMAccountTypeCode);

        }
        #endregion

    }
}
