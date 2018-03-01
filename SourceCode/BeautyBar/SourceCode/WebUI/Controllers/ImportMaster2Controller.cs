using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using ViewModels;

namespace WebUI.Controllers
{
    public class ImportMaster2Controller : BaseController
    {
        //
        // GET: /ImportMaster2/

        public ActionResult Index()
        {
            return View();
        }
        #region Create
        public ActionResult Create()
        {
            CreateViewBag();
            return View();
        }
        #endregion
        #region CreateViewBag
        private void CreateViewBag(int? WarehouseId = null, int? SupplierId = null, string VATType = "", string ManualDiscountType = "", int? CurrencyId = null)
        {
            //1. WarehouseId
            var WarehouseList = _context.WarehouseModel.OrderBy(p => p.WarehouseName).ToList();
            ViewBag.WarehouseId = new SelectList(WarehouseList, "WarehouseId", "WarehouseName", WarehouseId);

            //2. SupplierId
            var SupplierList = _context.SupplierModel.OrderBy(p => p.SupplierName).ToList();
            ViewBag.SupplierId = new SelectList(SupplierList, "SupplierId", "SupplierName", SupplierId);

            //3. VATType
            var VATTypeList = _context.VATTypeModel.OrderBy(p => p.VATTypeName).ToList();
            ViewBag.VATType = new SelectList(VATTypeList, "VATType", "VATTypeName", VATType);

            //4. ManualDiscountType
            var ManualDiscountList = _context.ManualDiscountTypeModel.OrderBy(p => p.manualDiscountTypeName).ToList();
            ViewBag.ManualDiscountType = new SelectList(ManualDiscountList, "ManualDiscountType", "manualDiscountTypeName", ManualDiscountType);

            //5. CurrencyId
            var CurrencyList = _context.CurrencyModel.OrderBy(p => p.CurrencyName).ToList();
            ViewBag.CurrencyId = new SelectList(CurrencyList, "CurrencyId", "CurrencyName", CurrencyId);
        }
        #endregion
        #region _CreateList
        public ActionResult _CreateList(List<ImportDetailModel> detail)
        {
            if (detail == null)
                detail = new List<ImportDetailModel>();
            return PartialView(detail);

        }
        #endregion
        #region _CreateListIner
        public ActionResult __CreatelistInner(List<ImportDetailViewModel> detail)
        {
            if (detail == null)
                detail = new List<ImportDetailViewModel>();
            ImportDetailViewModel item = new ImportDetailViewModel();
            detail.Add(item);
            return PartialView(detail);
        }
        #endregion
        #region
        public ActionResult _DeletelistInner(List<ImportDetailViewModel> detail, int RemoveId)
        {
            return PartialView("_CreateListIner", detail.Where(p => p.STT != RemoveId).ToList());
        }
        #endregion
    }
}
