using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Report;

namespace WebUI.Controllers.Report
{
    public class StockOutReportController : Controller
    {
        //
        // GET: /StockOutReport/

        public ActionResult ReportViewerPartial(int? StoreId, DateTime? ToDate, DateTime? FromDate, int? CustomerId, int? EmployeeId)// id để report with
        {
            CreateViewBag(StoreId, ToDate, FromDate,CustomerId, EmployeeId);
            ViewData["Report"] = new PhieuXuatKhoXtraReport();
            return PartialView();
        }
        public ActionResult CallbackReportViewerPartial(int? StoreId, DateTime? ToDate, DateTime? FromDate, int? CustomerId, int? EmployeeId)
        {
            CreateViewBag(StoreId, ToDate, FromDate,CustomerId, EmployeeId);
            ViewData["Report"] = CreateDateReport(StoreId, ToDate, FromDate,CustomerId, EmployeeId); // Lấy data từ Store Procedure đưa vào dataset

            return PartialView("ReportViewerPartial");
        }

        private PhieuXuatKhoXtraReport CreateDateReport(int? StoreId, DateTime? ToDate, DateTime? FromDate, int? CustomerId, int? EmployeeId)
        {
            PhieuXuatKhoXtraReport report = new PhieuXuatKhoXtraReport();
            DataSet ds = GetData(StoreId, ToDate, FromDate, CustomerId, EmployeeId);

            report.DataSource = ds;
            report.DataMember = "Detail"; // Lặp lại Detail
            string orderid = StoreId == null ? "" : StoreId.ToString();
            report.Name = "Phieu nhap kho -" + StoreId; // Export file Name
            return report;
        }
        public ActionResult ExportReportViewerPartial(int? StoreId, DateTime? ToDate, DateTime? FromDate, int? CustomerId, int? EmployeeId)
        {
            PhieuXuatKhoXtraReport quarterReport = CreateDateReport(StoreId, ToDate, FromDate, CustomerId, EmployeeId);
            return DevExpress.Web.Mvc.ReportViewerExtension.ExportTo(quarterReport);
        }

        private static DataSet GetData(int? StoreId, DateTime? ToDate, DateTime? FromDate, int? CustomerId, int? EmployeeId)
        {
            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandText = "usp_PhieuXuatKho";
                    cmd.Parameters.AddWithValue("@StoreId", StoreId);
                    cmd.Parameters.AddWithValue("@FromDate", FromDate);
                    cmd.Parameters.AddWithValue("@ToDate", ToDate);
                    cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
                    cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
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

        private void CreateViewBag(int? StoreId = null, DateTime? ToDate = null, DateTime? FromDate = null, int? CustomerId=null, int? EmployeeId=null)
        {
            ViewBag.StoreId = StoreId;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.CustomerId = CustomerId;
            ViewBag.EmployeeId = EmployeeId;

        }
    }
}
