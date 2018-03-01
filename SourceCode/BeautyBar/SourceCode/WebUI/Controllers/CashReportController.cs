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
    public class CashReportController : BaseController
    {
        //
        // GET: /CashReport/

        
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
            CreateViewBag(currentEmployee.StoreId, currentEmployee.EmployeeId, 1, CurrentQuater, 1, DateTime.Now.Month);
            return View();
        }

        #region Quý
        public ActionResult _ReportPartialViewQ(ReportSearchViewModel model)
        {
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
                        cmd.CommandText = "usp_BaoCaoThuChiTienMatTheoQuy";
                        cmd.Parameters.AddWithValue("@StoreId", model.StoreId);
                        cmd.Parameters.AddWithValue("@StaffId", model.StaffId);
                        cmd.Parameters.AddWithValue("@CurrentEmployeeId", currentEmployee.EmployeeId);
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
                        cmd.CommandText = "usp_BaoCaoThuChiTienMatTheoThang";
                        cmd.Parameters.AddWithValue("@StoreId", model.StoreId);
                        cmd.Parameters.AddWithValue("@StaffId", model.StaffId);
                        cmd.Parameters.AddWithValue("@CurrentEmployeeId", currentEmployee.EmployeeId);
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
                        cmd.CommandText = "usp_BaoCaoThuChiTienMatTheoNgay";
                        cmd.Parameters.AddWithValue("@StoreId", model.StoreId);
                        cmd.Parameters.AddWithValue("@StaffId", model.StaffId);
                        cmd.Parameters.AddWithValue("@CurrentEmployeeId", currentEmployee.EmployeeId);
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

        public ReportInforViewModel ListView(SqlDataReader s)
        {
            ReportInforViewModel ret = new ReportInforViewModel();
            try
            {
                if (s["ViewTime"] != null)
                {
                    ret.ViewTime = s["ViewTime"].ToString();
                }

                if (s["TotalRevenue"] != null)
                {
                    ret.TotalRevenue = decimal.Parse(s["TotalRevenue"].ToString());
                }

                if (s["TotalExpenditure"] != null)
                {
                    ret.TotalExpenditure = decimal.Parse(s["TotalExpenditure"].ToString());
                }

                if (s["TotalDifference"] != null)
                {
                    ret.TotalDifference = decimal.Parse(s["TotalDifference"].ToString());
                }
             
            }
            catch { }
            return ret;
        }

        private void CreateViewBag(int? StoreId = null, int? EmployeeId = null, int? FromQuater = null, int? ToQuater = null, int? FromMonth = null, int? ToMonth = null)
        {
            //0. StoreId
            var StoreList = _context.StoreModel.OrderBy(p => p.StoreName).Where(p =>
                p.Actived == true &&
                currentEmployee.StoreId == null ||
                p.StoreId == currentEmployee.StoreId
                ).ToList();
            ViewBag.StoreId = new SelectList(StoreList, "StoreId", "StoreName", StoreId);

            //1.n.v lập phiếu: chính nó và n.v nó quản lý
            //var EmployeeList = _context.EmployeeModel.OrderBy(p => p.FullName).Where(p => p.Actived == true).ToList();
            var EmployeeSaleList = _context.EmployeeModel.OrderBy(p => p.FullName).Where(p => p.Actived == true && (p.ParentId == currentEmployee.EmployeeId || p.EmployeeId == currentEmployee.EmployeeId)).ToList();

            ViewBag.EmployeeId = new SelectList(EmployeeSaleList, "EmployeeId", "FullName", EmployeeId);

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
      
        class DropdowlistViewBag
        {
            public int value { get; set; }
            public string text { get; set; }
        }
    }
}
