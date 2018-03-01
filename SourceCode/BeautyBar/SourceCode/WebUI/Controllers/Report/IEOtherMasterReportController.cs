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
    public class IEOtherMasterReportController : Controller
    {
        //
        // GET: /IEOtherMasterReport/

        public ActionResult ReportViewerPartial(int? IEOtherMasterId)// id để report with
        {
            CreateViewBag(IEOtherMasterId);
            ViewData["Report"] = new HoaDonXuatNhapKhacXtraReport();
            return PartialView();
        }
        public ActionResult CallbackReportViewerPartial(int? IEOtherMasterId)
        {
            CreateViewBag(IEOtherMasterId);
            ViewData["Report"] = CreateDateReport(IEOtherMasterId); // Lấy data từ Store Procedure đưa vào dataset

            return PartialView("ReportViewerPartial");
        }
        public ActionResult ExportReportViewerPartial(int? IEOtherMasterId)
        {
            HoaDonXuatNhapKhacXtraReport quarterReport = CreateDateReport(IEOtherMasterId);
            return DevExpress.Web.Mvc.ReportViewerExtension.ExportTo(quarterReport);
        }

        private HoaDonXuatNhapKhacXtraReport CreateDateReport(int? IEOtherMasterId)
        {
            HoaDonXuatNhapKhacXtraReport report = new HoaDonXuatNhapKhacXtraReport();
            DataSet ds = GetData(IEOtherMasterId);

            report.DataSource = ds;
            report.DataMember = "Detail"; // Lặp lại Detail
            string orderid = IEOtherMasterId == null ? "" : IEOtherMasterId.ToString();
            report.Name = "Phieu xuat - nhap khac -" + IEOtherMasterId; // Export file Name
            return report;
        }
        private static DataSet GetData(int? IEOtherMasterId)
        {
            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandText = "usp_HoaDonXuatNhapKhac";
                    cmd.Parameters.AddWithValue("@IEOtherMasterId", IEOtherMasterId);
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
        private void CreateViewBag(int? IEOtherMasterId = null)
        {
            ViewBag.IEOtherMasterId = IEOtherMasterId;
        }

    }
}
