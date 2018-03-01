using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Report;
using DevExpress.Web;
using System.Data;

namespace WebUI.Controllers
{
    public class ImportMasterReportController : Controller
    {
        //
        // GET: /ImportMasterReport/

        public ActionResult ReportViewerPartial(int? ImportMasterId)// id để report with
        {
            CreateViewBag(ImportMasterId);
            ViewData["Report"] = new HoaDonNhapNhaCungCapXtraReport();
            return PartialView();
        }
        public ActionResult CallbackReportViewerPartial(int? ImportMasterId)
        {
            CreateViewBag(ImportMasterId);
            ViewData["Report"] = CreateDateReport(ImportMasterId); // Lấy data từ Store Procedure đưa vào dataset

            return PartialView("ReportViewerPartial");
        }
        public ActionResult ExportReportViewerPartial(int? ImportMasterId)
        {
            HoaDonNhapNhaCungCapXtraReport quarterReport = CreateDateReport(ImportMasterId);
            return DevExpress.Web.Mvc.ReportViewerExtension.ExportTo(quarterReport);
        }

        private HoaDonNhapNhaCungCapXtraReport CreateDateReport(int? ImportMasterId)
        {
            HoaDonNhapNhaCungCapXtraReport report = new HoaDonNhapNhaCungCapXtraReport();
            DataSet ds = GetData(ImportMasterId);

            report.DataSource = ds;
            report.DataMember = "Detail"; // Lặp lại Detail
            string orderid = ImportMasterId == null ? "" : ImportMasterId.ToString();
            report.Name = "Phieu nhap kho -" + ImportMasterId; // Export file Name
            return report;
        }
        private static DataSet GetData(int? ImportMasterId)
        {
            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandText = "usp_HoaDonNhapNhaCungCap";
                    cmd.Parameters.AddWithValue("@ImportMasterId", ImportMasterId);
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
        private void CreateViewBag(int? ImportMasterId = null)
        {
            ViewBag.ImportMasterId = ImportMasterId;
        }
    }
}
