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
	public class InventoryMasterController : BaseController
	{
		//
		// GET: /InventoryMaster/
        
		public ActionResult Index()
		{
			CreateViewSearchBag();
			return View();
		}
		public ActionResult _SearchInventoryMaster(InventoryMasterSearchViewModel model)
		{
			List<InventoryMasterInfoViewModel> list = new List<InventoryMasterInfoViewModel>();
			list = (from p in _context.InventoryMasterModel
					join ivt in _context.InventoryTypeModel on p.InventoryTypeId equals ivt.InventoryTypeId
					join ivd in _context.InventoryDetailModel on p.InventoryMasterId equals ivd.InventoryMasterId
					join e in _context.EmployeeModel on p.CreatedEmployeeId equals e.EmployeeId
					join pd in _context.ProductModel on ivd.ProductId equals pd.ProductId
					join wh in _context.WarehouseModel on p.WarehouseModelId equals wh.WarehouseId
					where
					(model.WarehouseId == null || p.WarehouseModelId == model.WarehouseId) &&
					(model.ProductId == null || pd.ProductId == model.ProductId) &&
					(model.InventoryTypeId == null || p.InventoryTypeId == model.InventoryTypeId) &&
					(model.InventoryMasterId == null || p.InventoryMasterId == model.InventoryMasterId) &&
					(model.FromDate == null || p.CreatedDate.Value.CompareTo(model.FromDate.Value) >= 0) &&
					(model.ToDate == null || p.CreatedDate.Value.CompareTo(model.ToDate.Value) <= 0)

					select new InventoryMasterInfoViewModel()
					{
						InventoryCode = p.InventoryCode,
						WarehouseName = wh.WarehouseName,
						CreatedDate = p.CreatedDate,
						InventoryTypeCode = ivt.InventoryTypeCode,
						EmployeeName = e.FullName,
						ActionUrl = p.ActionUrl,
						BusinessId = p.BusinessId,
						isImport = ivt.isImport
					}).Distinct()
					.OrderByDescending(p => p.CreatedDate)
					.ToList();
			return PartialView(list);
		}

		private void CreateViewSearchBag(int? WarehouseId = null, int? SupplierId = null)
		{
			//1. WarehouseId
			var WarehouseList = _context.WarehouseModel.OrderBy(p => p.WarehouseName).ToList();
			ViewBag.WarehouseId = new SelectList(WarehouseList, "WarehouseId", "WarehouseName", WarehouseId);
		}


		public ActionResult GetInventoryTypeId(string q)
		{
			var data2 = _context
						.InventoryTypeModel
						.Where(p => q == null || (p.InventoryTypeCode + " " + p.InventoryTypeName).Contains(q))
						.Select(p => new
						{
							value = p.InventoryTypeId,
							text = (p.InventoryTypeCode + " | " + p.InventoryTypeName)
						})
						.Take(10)
						.ToList();
			return Json(data2, JsonRequestBehavior.AllowGet);
		}
		public ActionResult GetInventoryMasterId(string q)
		{
			var data2 = _context
						.InventoryMasterModel
						.Where(p => q == null || (p.InventoryCode).Contains(q))
						.Select(p => new
						{
							value = p.InventoryMasterId,
							text = p.InventoryCode
						})
						.Take(10)
						.ToList();
			return Json(data2, JsonRequestBehavior.AllowGet);
		}



	}
}
