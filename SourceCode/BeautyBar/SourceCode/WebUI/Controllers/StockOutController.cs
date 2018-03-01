using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;


namespace WebUI.Controllers
{
    public class StockOutController : BaseController
    {
        //
        // GET: /StockOut/

        
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public ActionResult Report(int StoreId, DateTime? FromDate, DateTime? ToDate, int?CustomerId, int? EmployeeId)
        {
            ViewBag.StoreId = StoreId;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.CustomerId = CustomerId;
            ViewBag.EmployeeId = EmployeeId;
            return PartialView();
        }


        private void CreateViewBag(int? StoreId = null, int? CustomerId = null, int? EmployeeId = null)
        {
            //0. StoreId
            var StoreList = _context.StoreModel.OrderBy(p => p.StoreName).Where(p =>
                p.Actived == true &&
                currentEmployee.StoreId == null ||
                p.StoreId == currentEmployee.StoreId
                )
                .ToList();
            ViewBag.StoreId = new SelectList(StoreList, "StoreId", "StoreName", StoreId);
            //1. CustomerId
            var CustomerList = _context.CustomerModel.OrderBy(p => p.FullName).Where(p => p.Actived == true)
                .ToList();
            ViewBag.CustomerId = new SelectList(CustomerList, "CustomerId", "FullName", CustomerId);

            //2.
            var EmployeeList = _context.EmployeeModel.OrderBy(p => p.FullName).Where(p => p.Actived == true)
                .ToList();
            ViewBag.EmployeeId = new SelectList(EmployeeList, "EmployeeId", "FullName", EmployeeId);


        }

    }
}
