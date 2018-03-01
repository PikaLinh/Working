using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Report;

namespace WebUI.Controllers
{
    public class ReturnReportGetDataController : Controller
    {
        //
        // GET: /ReturnReportGetData/

        public ActionResult ReportViewerPartial(int? ReturnMasterId)// id để report with
        {
            CreateViewBag(ReturnMasterId);
            ViewData["Report"] = new HoaDonTraHangNhaCungCapXtraReport();
            return PartialView();
        }
        public ActionResult CallbackReportViewerPartial(int? ReturnMasterId)
        {
            CreateViewBag(ReturnMasterId);
            ViewData["Report"] = CreateDateReport(ReturnMasterId); // Lấy data từ Store Procedure đưa vào dataset

            return PartialView("ReportViewerPartial");
        }
        public ActionResult ExportReportViewerPartial(int? ReturnMasterId)
        {
            HoaDonTraHangNhaCungCapXtraReport quarterReport = CreateDateReport(ReturnMasterId);
            return DevExpress.Web.Mvc.ReportViewerExtension.ExportTo(quarterReport);
        }
        private HoaDonTraHangNhaCungCapXtraReport CreateDateReport(int? ReturnMasterId)
        {
            HoaDonTraHangNhaCungCapXtraReport report = new HoaDonTraHangNhaCungCapXtraReport();
            DataSet ds = GetData(ReturnMasterId);

            report.DataSource = ds;
            report.DataMember = "Detail"; // Lặp lại Detail
            string orderid = ReturnMasterId == null ? "" : ReturnMasterId.ToString();
            report.Name = "Phieu tra hang nha cung cap -" + ReturnMasterId; // Export file Name
            return report;
        }
        private static DataSet GetData(int? ReturnMasterId)
        {
            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandText = "usp_HoaDonTraHangNhaCungCap";
                    cmd.Parameters.AddWithValue("@ReturnMasterId", ReturnMasterId);
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
        private void CreateViewBag(int? ReturnMasterId = null)
        {
            ViewBag.ReturnMasterId = ReturnMasterId;
        }
    }
}
