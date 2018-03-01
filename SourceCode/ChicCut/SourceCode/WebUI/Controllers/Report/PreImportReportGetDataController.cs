using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Report;

namespace WebUI.Controllers
{
    public class PreImportReportGetDataController : Controller
    {
        //
        // GET: /PreImportReportGetData/

        public ActionResult ReportViewerPartial(int? PreImportMasterId)// id để report with
        {
            CreateViewBag(PreImportMasterId);
            ViewData["Report"] = new HoaDonYeuCauNhapHangTuNhaCungCapXtraReport();
            return PartialView();
        }

        public ActionResult CallbackReportViewerPartial(int? PreImportMasterId)
        {
            CreateViewBag(PreImportMasterId);
            ViewData["Report"] = CreateDateReport(PreImportMasterId); // Lấy data từ Store Procedure đưa vào dataset

            return PartialView("ReportViewerPartial");
        }

        public ActionResult ExportReportViewerPartial(int? PreImportMasterId)
        {
            HoaDonYeuCauNhapHangTuNhaCungCapXtraReport quarterReport = CreateDateReport(PreImportMasterId);
            return DevExpress.Web.Mvc.ReportViewerExtension.ExportTo(quarterReport);
        }

        private HoaDonYeuCauNhapHangTuNhaCungCapXtraReport CreateDateReport(int? PreImportMasterId)
        {
            HoaDonYeuCauNhapHangTuNhaCungCapXtraReport report = new HoaDonYeuCauNhapHangTuNhaCungCapXtraReport();
            DataSet ds = GetData(PreImportMasterId);

            report.DataSource = ds;
            report.DataMember = "Detail"; // Lặp lại Detail
            string orderid = PreImportMasterId == null ? "" : PreImportMasterId.ToString();
            report.Name = "Phieu yeu cau nhap kho -" + PreImportMasterId; // Export file Name
            return report;
        }

        private static DataSet GetData(int? PreImportMasterId)
        {
            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandText = "usp_HoaDonYeuCauNhapHangTuNhaCungCap";
                    cmd.Parameters.AddWithValue("@PreImportMasterId", PreImportMasterId);
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

        private void CreateViewBag(int? PreImportMasterId = null)
        {
            ViewBag.PreImportMasterId = PreImportMasterId;
        }
    }
}
