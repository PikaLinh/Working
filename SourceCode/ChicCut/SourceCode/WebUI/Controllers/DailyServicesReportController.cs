using Constant;
using EntityModels;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace WebUI.Controllers
{
    public class DailyServicesReportController : BaseController
    {
        // GET: DailyServicesReport
        public ActionResult Index()
        {
            #region Get CurrentQuater
            int CurrentQuater;
            int CurrentMonth = DateTime.Now.Month;
            if (CurrentMonth <= 3)
            {
                CurrentQuater = 1;
            }
            else if (CurrentMonth <= 6)
            {
                CurrentQuater = 2;
            }
            else if (CurrentMonth <= 9)
            {
                CurrentQuater = 3;
            }
            else
            {
                CurrentQuater = 4;
            }
            #endregion
            CreateViewBag(currentEmployee.StoreId, currentEmployee.EmployeeId, currentEmployee.EmployeeId, null, null, 1, CurrentQuater, 1, DateTime.Now.Month);

            return View();
        }

        public ActionResult _ReportPartial(ReportSearchViewModel model)
        {
            DateTime fromDate = model.FromDate.Value.Date;
            DateTime toDate = model.ToDate.Value.Date.AddDays(1).AddSeconds(-1);
            var result = _context.Database.SqlQuery<DailyServicesViewModel>
                        ("usp_BaoCaoNgay @StoreId, @PaymentMethodId, @FromDate,@ToDate",
                          new SqlParameter("@StoreId", model.StoreId),
                          new SqlParameter("@PaymentMethodId", model.PaymentMethodId ?? (object)DBNull.Value),
                          new SqlParameter("@FromDate", fromDate),
                          new SqlParameter("@ToDate", toDate)
                          )
                        .ToList();

            return PartialView(result);
        }
        public ActionResult SendSMS(DateTime FromDate, DateTime ToDate, string Total, string TotalCash, string TotalCard)
        {
            try
            {
                SendSMSRepository _repository = new SendSMSRepository();
                string message = "", messageCash = "", messageCard = "";
                if (FromDate == ToDate)
                {
                    message = string.Format("Doanh thu ngay {0} la: {1}d. ", ToDate.ToString("dd/MM/yyyy"), Total);
                    messageCash = string.Format("Tien mat la: {0}d. ", TotalCash);
                    messageCard = string.Format("So tien ca the la: {0}d. ", TotalCard);    
                }
                else
                {
                    message = string.Format("Doanh thu tu ngay {0} den ngay {1} la: {2}d. ", FromDate.ToString("dd/MM/yyyy"), ToDate.ToString("dd/MM/yyyy"), Total);
                    messageCash = string.Format("Tien mat la: {0}d. ", TotalCash);
                    messageCard = string.Format("So tien ca the la: {0}d. ", TotalCard); 
                }
                
                AY_SMSCalendar smsModel = new AY_SMSCalendar()
                {
                    EndDate = DateTime.Now,
                    isSent = true,
                    NumberOfFailed = 0,
                    SMSContent = message + messageCash + messageCard,
                    SMSTo = "01229992778"
                };
                _context.Entry(smsModel).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                _repository.SendSMSModel(smsModel);

                return Json(new
                {
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
          

        }
        public ActionResult Print(ReportSearchViewModel model)
        {
            DateTime fromDate = model.FromDate.Value.Date;
            DateTime toDate = model.ToDate.Value.Date.AddDays(1).AddSeconds(-1);
            var result = _context.Database.SqlQuery<DailyServicesViewModel>
                        ("usp_BaoCaoNgay @StoreId,@PaymentMethodId,@FromDate,@ToDate",
                          new SqlParameter("@StoreId", model.StoreId),
                          new SqlParameter("@PaymentMethodId", model.PaymentMethodId ?? (object)DBNull.Value),
                          new SqlParameter("@FromDate", fromDate),
                          new SqlParameter("@ToDate", toDate)
                          )
                        .ToList();
            DailyServicesReportViewModel data = new DailyServicesReportViewModel();
            data.FromDate = fromDate;
            data.ToDate = toDate;
            data.DailyServices = result;
            if (result != null && result.Count > 0)
            {
                data.Total = result.Sum(p => p.UnitPrice);
            }
            return Json(new
            {
                Code = System.Net.HttpStatusCode.Created,
                Success = true,
                Data = data
            });
        }


        private void CreateViewBag(int? StoreId = null, int? EmployeeId = null, int? CashierUserId = null, int? StaffId = null, int? PaymentMethodId = null, int? FromQuater = null, int? ToQuater = null, int? FromMonth = null, int? ToMonth = null)
        {
            //0. StoreId
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

            ////1.2: nv bán hàng: chính nó và n.v nó quản lý
            //var EmployeeSaleList = _context.EmployeeModel.OrderBy(p => p.FullName).Where(p => p.Actived == true && (p.ParentId == currentEmployee.EmployeeId || p.EmployeeId == currentEmployee.EmployeeId)).ToList();
            //ViewBag.SaleId = new SelectList(EmployeeSaleList, "EmployeeId", "FullName", null);

            //1.2: load toàn bộ trừ admin
            //var EmployeeSaleList = _context.EmployeeModel.OrderBy(p => p.FullName).Where(p => p.Actived == true && (p.ParentId == currentEmployee.EmployeeId || p.EmployeeId == currentEmployee.EmployeeId)).ToList();
            //ViewBag.SaleId = new SelectList(EmployeeSaleList, "EmployeeId", "FullName", null);
            // ParentIdCreate
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


            // 2. FromQuater
            var QuaterList = new List<DropdowlistViewBag>()
              {
                  new DropdowlistViewBag () { value = 1, text = "Quý 1"},
                  new DropdowlistViewBag () { value = 2, text = "Quý 2"},
                  new DropdowlistViewBag () { value = 3, text = "Quý 3"},
                  new DropdowlistViewBag () { value = 4, text = "Quý 4"}
                };
            ViewBag.FromQuater = new SelectList(QuaterList, "value", "text", FromQuater);
            // 3. FromQuater
            ViewBag.ToQuater = new SelectList(QuaterList, "value", "text", ToQuater);

            // 3. FromMonth
            var MonthList = new List<DropdowlistViewBag>() 
                 {
                  new DropdowlistViewBag () { value = 1, text = "Tháng 1"},
                  new DropdowlistViewBag () { value = 2, text = "Tháng 2"},
                  new DropdowlistViewBag () { value = 3, text = "Tháng 3"},
                  new DropdowlistViewBag () { value = 4, text = "Tháng 4"},
                  new DropdowlistViewBag () { value = 5, text = "Tháng 5"},
                  new DropdowlistViewBag () { value = 6, text = "Tháng 6"},
                  new DropdowlistViewBag () { value = 7, text = "Tháng 7"},
                  new DropdowlistViewBag () { value = 8, text = "Tháng 8"},
                  new DropdowlistViewBag () { value = 9, text = "Tháng 9"},
                  new DropdowlistViewBag () { value = 10, text = "Tháng 10"},
                  new DropdowlistViewBag () { value = 11, text = "Tháng 11"},
                  new DropdowlistViewBag () { value = 12, text = "Tháng 12"}
                   
                };
            ViewBag.FromMonth = new SelectList(MonthList, "value", "text", FromMonth);

            // 4. ToMonth
            ViewBag.ToMonth = new SelectList(MonthList, "value", "text", ToMonth);

            //5. PaymentMethod
            var PaymentMethodLst = _context.PaymentMethodModel.Where(p => p.Actived == true).ToList();
            ViewBag.PaymentMethodId = new SelectList(PaymentMethodLst, "PaymentMethodId", "PaymentMethodName", PaymentMethodId);
        }
       
    }
}