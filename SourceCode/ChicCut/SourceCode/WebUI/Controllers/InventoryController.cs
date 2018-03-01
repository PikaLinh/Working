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
    public class InventoryController : BaseController
    {
        //
        // GET: /Inventory/
        
        public ActionResult Index()
        {
            CreateViewSearchBag();
            return View();
        }
        public ActionResult _SearchInventory(InventorySearchViewModel model)
        {
            if (model.ToDate.HasValue)
            {
               model.ToDate.Value.AddDays(1).AddMilliseconds(-1);
            }
            List<InventoryInfoViewModel> list = new List<InventoryInfoViewModel>();
           
            list = (from p in _context.InventoryDetailModel
                    join ivm in _context.InventoryMasterModel on p.InventoryMasterId equals ivm.InventoryMasterId
                    join ivt in _context.InventoryTypeModel on ivm.InventoryTypeId equals ivt.InventoryTypeId
                    join pd in _context.ProductModel on p.ProductId equals pd.ProductId
                    join e in _context.EmployeeModel on ivm.CreatedEmployeeId equals e.EmployeeId
                    //join ip in _context.ImportMasterModel on ivm.BusinessId equals ip.importmasterid
                    //join or in _context.OrderMasterModel on ivm.BusinessIdequals or.orderid
                    where 
                        //(model.WarehouseId == null || ip.WarehouseId == model.WarehouseId || or.WarehouseId == model.WarehouseId) &&
                    (model.WarehouseId == null || ivm.WarehouseModelId == model.WarehouseId) &&
                    (model.InventoryMasterId == null || ivm.InventoryMasterId == model.InventoryMasterId) &&
                    (model.EmployeeId == null || e.EmployeeId == model.EmployeeId) &&
                    (model.ProductId == null || pd.ProductId == model.ProductId) &&
                    (model.FromDate == null || ivm.CreatedDate.Value.CompareTo(model.FromDate.Value) >= 0) &&
                    (model.ToDate == null || ivm.CreatedDate.Value.CompareTo(model.ToDate.Value) <= 0)
                    select new InventoryInfoViewModel()
                    {
                        InventoryDetailId = p.InventoryDetailId,    
                        InventoryTypeCode = ivt.InventoryTypeCode,
                        EmployeeName = e.FullName,
                        CreatedDate = ivm.CreatedDate,
                        ProductName = pd.ProductName,
                        BeginInventoryQty = p.BeginInventoryQty,
                        COGS = p.COGS,
                        Price = p.Price,
                        ImportQty = p.ImportQty,
                        ExportQty = p.ExportQty,
                        UnitCOGS = p.UnitCOGS,
                        UnitPrice = p.UnitPrice,
                        EndInventoryQty = p.EndInventoryQty,
                        ActionUrl = ivm.ActionUrl,
                        BusinessId = ivm.BusinessId,
                        InventoryCode = ivm.InventoryCode,
                        ProductCode = pd.ProductCode,
                        Specifications = pd.Specifications
                    }).Distinct()
                    .OrderByDescending(p=>p.InventoryDetailId)
                    .ToList();
            
            return PartialView(list);

        }

        private bool Max()
        {
            throw new NotImplementedException();
        }

        private void CreateViewSearchBag(int? WarehouseId = null)
        {
            //1. WarehouseId
            var WarehouseList = _context.WarehouseModel.OrderBy(p => p.WarehouseName).ToList();
            ViewBag.WarehouseId = new SelectList(WarehouseList, "WarehouseId", "WarehouseName", WarehouseId);

        }

    }
}
