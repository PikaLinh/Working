using Constant;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;


namespace WebUI.Controllers
{
    public class GrossRevenueReportController : BaseController
    {
        //
        // GET: /GrossRevenueReport/
        
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
            CreateViewBag(currentEmployee.StoreId, currentEmployee.EmployeeId, currentEmployee.EmployeeId, null, 1, CurrentQuater, 1, DateTime.Now.Month);
            return View();
        }

        #region Quý
        public ActionResult _ReportPartialViewQ(ReportSearchViewModel model)
        {
            ViewBag.ViewType = "Q";
            return PartialView("_ReportPartialViewD", GetListQ(model));
        }
        public IEnumerable GetListQ(ReportSearchViewModel model)
        {
            var List = new List<ReportInforViewModel>();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                    {
                        cmd.CommandText = "usp_BaoCaoDoanhThuTheoQuy";
                        cmd.Parameters.AddWithValue("@StoreId", model.StoreId);
                        cmd.Parameters.AddWithValue("@StaffId", model.StaffId);
                        cmd.Parameters.AddWithValue("@CashierUserId", model.CashierUserId);
                        cmd.Parameters.AddWithValue("@CurrentSaleId", currentEmployee.EmployeeId);
                        cmd.Parameters.AddWithValue("@FromQuater", model.FromQuater);
                        cmd.Parameters.AddWithValue("@FromYearQuater", model.FromYearQuater);
                        cmd.Parameters.AddWithValue("@ToQuater", model.ToQuater);
                        cmd.Parameters.AddWithValue("@ToYearQuater", model.ToYearQuater);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var v = ListView(dr);
                                List.Add(v);
                            }
                        }
                        conn.Close();
                    }
                }
                return List;
            }
            catch
            {
                return List;
            }
        }
        #endregion

        #region Tháng
        public ActionResult _ReportPartialViewM(ReportSearchViewModel model)
        {
            ViewBag.ViewType = "M";
            return PartialView("_ReportPartialViewD", GetListM(model));
        }
        public IEnumerable GetListM(ReportSearchViewModel model)
        {
            var List = new List<ReportInforViewModel>();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                    {
                        cmd.CommandText = "usp_BaoCaoDoanhThuTheoThang";
                        cmd.Parameters.AddWithValue("@StoreId", model.StoreId);
                        cmd.Parameters.AddWithValue("@StaffId", model.StaffId);
                        cmd.Parameters.AddWithValue("@CashierUserId", model.CashierUserId);
                        cmd.Parameters.AddWithValue("@CurrentSaleId", currentEmployee.EmployeeId);
                        cmd.Parameters.AddWithValue("@FromMonth", model.FromMonth);
                        cmd.Parameters.AddWithValue("@FromYearMonth", model.FromYearMonth);
                        cmd.Parameters.AddWithValue("@ToMonth", model.ToMonth);
                        cmd.Parameters.AddWithValue("@ToYearMonth", model.ToYearMonth);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var v = ListView(dr);
                                List.Add(v);
                            }
                        }
                        conn.Close();
                    }
                }
                return List;
            }
            catch
            {
                return List;
            }
        }
        #endregion

        #region Ngày
        public ActionResult _ReportPartialViewD(ReportSearchViewModel model)
        {
            ViewBag.ViewType = "D";
            return PartialView("_ReportPartialViewD", GetListD(model));
        }

        public IEnumerable GetListD(ReportSearchViewModel model)
        {
            var List = new List<ReportInforViewModel>();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                    {
                        cmd.CommandText = "usp_BaoCaoDoanhThuTheoNhanVien";
                        cmd.Parameters.AddWithValue("@StoreId", model.StoreId);
                        cmd.Parameters.AddWithValue("@StaffId", model.StaffId);
                        cmd.Parameters.AddWithValue("@CashierUserId", model.CashierUserId);
                        cmd.Parameters.AddWithValue("@CurrentSaleId", currentEmployee.EmployeeId);
                        cmd.Parameters.AddWithValue("@FromDate", model.FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", model.ToDate);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var v = ListView(dr);
                                List.Add(v);
                            }
                        }
                        conn.Close();
                    }
                }
                return List;
            }
            catch
            {
                return List;
            }
        }

        #endregion

        public ActionResult _GetDatailOrder(DateTime Date, int? StoreId, int? CashierUserId, int? StaffId)
        {
            var FromDate = Date.Date;
            var ToDate = Date.Date.AddDays(1);
            ViewBag.Date = string.Format("Chi tiết doanh thu ngày: {0:dd/MM/yyyy}", Date);
           
            var lst = (from o in _context.Daily_ChicCut_OrderModel
                       //Người tạo
                       join staffTmp in _context.EmployeeModel on o.StaffId equals staffTmp.EmployeeId into staffList
                       from staff in staffList.DefaultIfEmpty()
                       //Người tính tiền
                       join su in _context.AccountModel on o.CashierUserId equals su.UserId
                       join sa in _context.EmployeeModel on su.EmployeeId equals sa.EmployeeId
                       //Khách hàng
                       //join cus in _context.CustomerModel on o.CustomerId equals cus.CustomerId
                       where 
                       //Theo ngày
                       (o.CashierDate.Value >= FromDate && o.CashierDate.Value < ToDate) 
                       //Theo thu ngân
                       && (CashierUserId == null || o.CashierUserId == CashierUserId)
                       //Theo nhân viên (thợ phụ)
                       && (StaffId == null || o.StaffId == StaffId)
                       && o.OrderStatusId == Constant.EnumDaily_ChicCut_OrderStatus.DaTinhTien
                      select new ReportInforDetailViewModel()
                       {
                           OrderId = o.OrderId,
                           StaffName = staff.FullName,
                           Cashier = sa.FullName,
                           CustomerName = o.FullName + " - " + o.Phone,
                           SumCOGSOfOrderDetail = (o.SumCOGSOfOrderDetail),
                           SumPriceOfOrderDetail = o.SumPriceOfOrderDetail,
                           TotalBillDiscount = o.TotalBillDiscount,
                           Total = o.Total,
                           Tip = o.Tip,
                           Commission = o.Commission,
                           Profits = (o.Total ?? 0) - (o.SumCOGSOfOrderDetail ?? 0) - (o.Tip ?? 0) - (o.Commission ?? 0)
                       }).ToList();
            if (lst != null)
            {
                ViewBag.RowSum = new ReportInforDetailViewModel()
                {
                    SumPriceOfOrderDetail = lst.Sum(p => p.SumPriceOfOrderDetail),
                    TotalBillDiscount = lst.Sum(p => p.TotalBillDiscount),
                    SumCOGSOfOrderDetail = lst.Sum(p => p.SumCOGSOfOrderDetail),
                    Total = lst.Sum(p => p.Total),
                    Tip = lst.Sum(p => p.Tip),
                    Commission = lst.Sum(p => p.Commission),
                    Profits = lst.Sum(p => p.Profits)
                };
            }
            return PartialView(lst);
        }
        public ReportInforViewModel ListView(SqlDataReader s)
        {
            ReportInforViewModel ret = new ReportInforViewModel();
            try
            {
                if (s["ViewTime"] != null)
                {
                    ret.ViewTime = s["ViewTime"].ToString();
                }

                if (s["TotalPrice"] != null)
                {
                    ret.TotalPrice = decimal.Parse(s["TotalPrice"].ToString());
                }

                if (s["TotalBillDiscount"] != null)
                {
                    ret.TotalBillDiscount = decimal.Parse(s["TotalBillDiscount"].ToString());
                }

                if (s["TotalVAT"] != null)
                {
                    ret.TotalVAT = decimal.Parse(s["TotalVAT"].ToString());
                }

                if (s["Tip"] != null)
                {
                    ret.Tip = decimal.Parse(s["Tip"].ToString());
                }

                if (s["Commission"] != null)
                {
                    ret.Commission = decimal.Parse(s["Commission"].ToString());
                }

                if (s["Revenue"] != null)
                {
                    ret.Revenue = decimal.Parse(s["Revenue"].ToString());
                }

                if (s["COGS"] != null)
                {
                    ret.COGS = decimal.Parse(s["COGS"].ToString());
                }

                if (s["Profit"] != null)
                {
                    ret.Profit = decimal.Parse(s["Profit"].ToString());
                }
            }
            catch { }
            return ret;
        }

        private void CreateViewBag(int? StoreId = null, int? EmployeeId = null, int? CashierUserId = null, int? StaffId = null, int? FromQuater = null, int? ToQuater = null, int? FromMonth = null, int? ToMonth = null)
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
        }
       
    }
}
