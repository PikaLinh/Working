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
        [HttpPost]
        public ActionResult CreateNew(DateTime? FromDate, DateTime? ToDate)
        {
            SalaryMasterModel smModel = new SalaryMasterModel();
            if(FromDate == null && ToDate == null)
            {
                return Json("Vui lòng nhập thông tin có dấu (*)!", JsonRequestBehavior.AllowGet);
            }
            else 
            {
                smModel.FromDate = FromDate;
                ViewBag.FromDate = smModel.FromDate;
                smModel.ToDate = ToDate;
                ViewBag.ToDate = smModel.ToDate;
                return PartialView();
            }
            
        }
        #endregion

        #region _CreateList
        public ActionResult _CreateList(SalaryMasterModel smModel)
        {
            ViewBag.PayDate = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.FromDate = smModel.FromDate;
            ViewBag.ToDate = smModel.ToDate;
            //Get thông tin nhân viên
            //var emp = _context.EmployeeModel.Where(p => p.Actived == true && p.EmployeeId != 10001).ToList();
            //foreach(var item in emp)
            //{
            //    detail.Add(new SalaryDetailViewModel() 
            //    { 
            //        EmployeeId = item.EmployeeId, 
            //        EmployeeName = item.FullName 
            //    });

            //}
            if (smModel.FromDate == null && smModel.ToDate == null)
            {
                var query = (from emp in _context.EmployeeModel
                          join dco in _context.Daily_ChicCut_OrderModel on emp.EmployeeId equals dco.StaffId into ret1
                          from ret2 in ret1.DefaultIfEmpty()
                          where emp.Actived == true && emp.EmployeeId != 10001
                          orderby emp.EmployeeId
                          group ret2 by new { ret2.StaffId, emp.FullName } into g
                          select new SalaryDetailViewModel()
                          {
                              EmployeeName = g.Key.FullName,
                          });
                return PartialView(query.ToList());
            }
            else
            {
            var result = (from emp in _context.EmployeeModel
                          join dco in _context.Daily_ChicCut_OrderModel on emp.EmployeeId equals dco.StaffId into ret1
                          from ret2 in ret1.DefaultIfEmpty()
                          where emp.Actived == true && emp.EmployeeId != 10001
                          && ret2.CashierDate >= smModel.FromDate && ret2.CashierDate <= smModel.ToDate
                          orderby emp.EmployeeId
                          group ret2 by new { ret2.StaffId, emp.FullName } into g
                          select new SalaryDetailViewModel()
                          {
                              EmployeeName = g.Key.FullName,
                              Tip = g.Sum(p => p.Tip),
                              Commission = g.Sum(p => p.Commission),
                          });

            return PartialView(result.ToList());
            }

        }
        #endregion
    }
}