using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers.Report
{
    public class BaoCaoThuChiTheoNgayController : BaseController
    {
        // GET: BaoCaoThuChiTheoNgay
        public ActionResult Index()
        {
            ViewBag.FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd");
            ViewBag.ToDate = DateTime.Now.ToString("yyyy-MM-dd");
            CreateSearchViewBag();
            return View();
        }

        private void CreateSearchViewBag(int? StoreId = null)
        {
            //Load cửa hàng
            var store = _context.StoreModel.Where(p => p.Actived == true).ToList();
            ViewBag.StoreId = new SelectList(store, "StoreId", "StoreName", StoreId);
        }

        //1. Sau khi nhấn nút xem thì vào action Report trả về PartialView chưa có dữ liệu
        public ActionResult Report(int StoreId, DateTime? FromDate, DateTime? ToDate)
        {
            ViewBag.StoreId = StoreId;
            ViewBag.FromDate = FromDate;
            if (ToDate.HasValue)
            {
                ViewBag.ToDate = ToDate.Value.AddDays(1).AddSeconds(-1);
            }
            return PartialView();
        }
        //2. PartialView Report lại trả về action ReportViewerPartial và khởi tạo mới Report
        public ActionResult ReportViewerPartial(int StoreId, DateTime? FromDate, DateTime? ToDate)// id để report with
        {
            ViewBag.StoreId = StoreId;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewData["Report"] = new BaoCaoThuChiTheoNgayXtraReport();
            return PartialView();
        }
        //3. Sau đó callback lại để lấy dữ liệu
        public ActionResult CallbackReportViewerPartial(int StoreId, DateTime? FromDate, DateTime? ToDate)
        {
            ViewBag.StoreId = StoreId;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewData["Report"] = CreateDateReport(StoreId, FromDate, ToDate); // Lấy data từ Store Procedure đưa vào dataset
            return PartialView("ReportViewerPartial");
        }

        public ActionResult ExportReportViewerPartial(int StoreId, DateTime? FromDate, DateTime? ToDate)
        {
            BaoCaoThuChiTheoNgayXtraReport quarterReport = CreateDateReport(StoreId, FromDate, ToDate);
            return DevExpress.Web.Mvc.ReportViewerExtension.ExportTo(quarterReport);
        }
        private BaoCaoThuChiTheoNgayXtraReport CreateDateReport(int StoreId, DateTime? FromDate, DateTime? ToDate)
        {
            BaoCaoThuChiTheoNgayXtraReport report = new BaoCaoThuChiTheoNgayXtraReport();
            DataSet ds = GetData(StoreId, FromDate, ToDate);

            report.DataSource = ds;
            // Lặp lại Detail
            report.DataMember = "Detail";
            // Export file Name
            report.Name = "Báo cáo xuất nhập tồn";
            return report;
        }
        private static DataSet GetData(int StoreId, DateTime? FromDate, DateTime? ToDate)
        {
            if (FromDate.HasValue)
            {
                FromDate = FromDate.Value.Date;
            }
            if (ToDate.HasValue)
            {
                ToDate = ToDate.Value.Date.AddDays(1).AddMinutes(-1);
            }
            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandText = "usp_BaoCaoThuChi";
                    cmd.Parameters.AddWithValue("@FromDate", FromDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDate", ToDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@StoreId", StoreId);
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                    conn.Close();
                }
            }
            ds.Tables[0].TableName = "HeaderInfomation";
            ds.Tables[1].TableName = "Detail";
            return ds;
        }
    }
}