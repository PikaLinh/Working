using Constant;
using EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace WebUI.Controllers
{
    public class SalaryController : BaseController
    {
        // GET: Salary
        #region Danh sách lương nhân viên
        public ActionResult Index()
        {
            ViewBag.FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd");
            ViewBag.ToDate = DateTime.Now.ToString("yyyy-MM-dd");
            CreateViewSearchBag(currentEmployee.StoreId, currentEmployee.EmployeeId, currentEmployee.EmployeeId, null);
            return View();
        }
        #endregion

        #region CreateViewSearchBag
        private void CreateViewSearchBag(int? StoreId = null, int? EmployeeId = null, int? CashierUserId = null, int? StaffId = null)
        {
            //1. Cửa hàng
            var StoreList = _context.StoreModel.OrderBy(p => p.StoreName).Where(p =>
                p.Actived == true &&
                currentEmployee.StoreId == null ||
                p.StoreId == currentEmployee.StoreId
                ).ToList();
            ViewBag.StoreId = new SelectList(StoreList, "StoreId", "StoreName", StoreId);

            //2. nv thợ phụ
            var StaffLst = (from p in _context.EmployeeModel
                            join ac in _context.AccountModel on p.EmployeeId equals ac.EmployeeId
                            where p.Actived == true && ac.RolesId == EnumRoles.NVPV
                            orderby p.FullName ascending
                            select p
                            ).ToList();

            ViewBag.StaffId = new SelectList(StaffLst, "EmployeeId", "FullName", StaffId);

            if (CurrentUser.isAdmin)
            {
                var CashierUserIdList = (from e in _context.EmployeeModel
                                         join a in _context.AccountModel on e.EmployeeId equals a.EmployeeId
                                         where e.Actived == true
                                         orderby e.FullName
                                         select new { a.UserId, e.FullName }).ToList();
                ViewBag.CashierUserId = new SelectList(CashierUserIdList, "UserId", "FullName", CashierUserId);
            }
            else
            {

                var CashierUserIdList = (from e in _context.EmployeeModel
                                         join a in _context.AccountModel on e.EmployeeId equals a.EmployeeId
                                         where e.Actived == true &&
                                                  a.RolesId != EnumRoles.DEV
                                         orderby e.FullName
                                         select new { a.UserId, e.FullName }).ToList();
                ViewBag.CashierUserId = new SelectList(CashierUserIdList, "UserId", "FullName", CashierUserId);
            }

        }
        #endregion

        #region Thêm mới bảng lương
        public ActionResult Create()
        {
            ViewBag.PayDate = DateTime.Now.ToString("yyyy-MM-dd");
            return View();
        }

        public ActionResult _CreateList(SalaryDetailViewModel salaryVM, DateTime? FromDate, DateTime? ToDate)
        {
            if (FromDate == null || ToDate == null || salaryVM.Note == null)
            {
                return Json(new { Message = "Vui lòng nhập thông tin có dấu (*)!"}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var salary = (from e in _context.EmployeeModel
                              join o in _context.Daily_ChicCut_OrderModel on  e.EmployeeId equals o.StaffId into oList
                              from oL in oList.DefaultIfEmpty()
                              where oL.OrderStatusId == 3
                              select new
                              {
                                  EmployeeName = e.FullName,
                                  Commission = oL.Commission,
                                  Tip = oL.Tip
                              }).ToList();
                return PartialView(salary);
            }
            
        }
        #endregion

        //Thông báo
        public ActionResult Alert(string Content)
        {
            ViewBag.Content = Content;
            return PartialView();
        }
    }
}