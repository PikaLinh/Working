using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WebUI.Controllers
{
    public class InventoryProductReportController : BaseController
    {
        //
        // GET: /InventoryProductReport/
        
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public ActionResult Report(int? StoreId, int? WarehouseId, DateTime? ToDate, DateTime? FromDate, int? SupplierId, int? EmployeeId)
        {
            ViewBag.StoreId = StoreId;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.SupplierId = SupplierId;
            ViewBag.EmployeeId = EmployeeId;
            ViewBag.WarehouseId = WarehouseId;
            return PartialView();
        }

        private void CreateViewBag(int? StoreId = null, int? SupplierId = null, int? EmployeeId = null, int? WarehouseId=null)
        {
            //0. StoreId
            var StoreList = _context.StoreModel.OrderBy(p => p.StoreName).Where(p =>
                p.Actived == true &&
                currentEmployee.StoreId == null ||
                p.StoreId == currentEmployee.StoreId
                )
                .ToList();
            ViewBag.StoreId = new SelectList(StoreList, "StoreId", "StoreName", StoreId);

            //1. SupplierId
            var SupplierList = _context.SupplierModel.OrderBy(p => p.SupplierName).Where(p => p.Actived == true)
                .ToList();
            ViewBag.SupplierId = new SelectList(SupplierList, "SupplierId", "SupplierName", SupplierId);

            //2.
            var EmployeeList = _context.EmployeeModel.OrderBy(p => p.FullName).Where(p => p.Actived == true)
                .ToList();
            ViewBag.EmployeeId = new SelectList(EmployeeList, "EmployeeId", "FullName", EmployeeId);

            //3. WarehouseId
            var WarehouseList = _context.WarehouseModel.OrderBy(p => p.WarehouseName).ToList();
            ViewBag.WarehouseId = new SelectList(WarehouseList, "WarehouseId", "WarehouseName", WarehouseId);


        }

    }
}
