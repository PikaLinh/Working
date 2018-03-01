using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Report;

namespace WebUI.Controllers
{
    public class ReceiptVoucherGetDataController : Controller
    {
        //
        // GET: /ReceiptVoucherGetData/

        public ActionResult ReportViewerPartial(int? TransactionId)// id để report with
        {
            CreateViewBag(TransactionId);
            ViewData["Report"] = new PhieuThuXtraReport();
            return PartialView();
        }

        public ActionResult CallbackReportViewerPartial(int? TransactionId)
        {
            CreateViewBag(TransactionId);
            ViewData["Report"] = CreateDateReport(TransactionId); // Lấy data từ Store Procedure đưa vào dataset

            return PartialView("ReportViewerPartial");
        }

        public ActionResult ExportReportViewerPartial(int? TransactionId)
        {
            PhieuThuXtraReport quarterReport = CreateDateReport(TransactionId);
            return DevExpress.Web.Mvc.ReportViewerExtension.ExportTo(quarterReport);
        }

        private PhieuThuXtraReport CreateDateReport(int? TransactionId)
        {
            PhieuThuXtraReport report = new PhieuThuXtraReport();
            DataSet ds = GetData(TransactionId);

            report.DataSource = ds;
            report.DataMember = "Detail"; // Lặp lại Detail
            string orderid = TransactionId == null ? "" : TransactionId.ToString();
            report.Name = "Phieu thu - chi tien mat : " + TransactionId; // Export file Name
            return report;
        }

        private static DataSet GetData(int? TransactionId)
        {
            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandText = "usp_ReceiptVoucher";
                    cmd.Parameters.AddWithValue("@TransactionId", TransactionId);
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    adapter.Fill(ds);
                    conn.Close();
                }
            }
            ds.Tables[0].TableName = "HeaderInfomation";
            return ds;
        }

        private void CreateViewBag(int? TransactionId = null)
        {
            ViewBag.TransactionId = TransactionId;
        }

    }
}
