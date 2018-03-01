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
    public class OrderMasterReportController : Controller
    {
        //
        // GET: /OrderMasterReport/

        public ActionResult ReportViewerPartial(int? OrderId)// id để report with
        {
            CreateViewBag(OrderId);
            ViewData["Report"] = new HoaDonBanHangXtraReport1();
            return PartialView();
        }
        public ActionResult CallbackReportViewerPartial(int? OrderId)
        {
            CreateViewBag(OrderId);
            ViewData["Report"] = CreateDateReport(OrderId); // Lấy data từ Store Procedure đưa vào dataset

            return PartialView("ReportViewerPartial");
        }
        public ActionResult ExportReportViewerPartial(int? OrderId)
        {
            HoaDonBanHangXtraReport1 quarterReport = CreateDateReport(OrderId);
            return DevExpress.Web.Mvc.ReportViewerExtension.ExportTo(quarterReport);
        }
        private HoaDonBanHangXtraReport1 CreateDateReport(int? OrderId)
        {
            HoaDonBanHangXtraReport1 report = new HoaDonBanHangXtraReport1();
            DataSet ds = GetData(OrderId);

            report.DataSource = ds;
            report.DataMember = "Detail"; // Lặp lại Detail
            string orderid = OrderId == null ? "" : OrderId.ToString();
            report.Name = "Phieu ban hang -" + OrderId; // Export file Name
            return report;
        }
        private static DataSet GetData(int? OrderId)
        {
            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandText = "usp_HoaDonBanHang";
                    cmd.Parameters.AddWithValue("@OrderId", OrderId);
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
        private void CreateViewBag(int? OrderId = null)
        {
            ViewBag.OrderId = OrderId;
        }
    }
}
