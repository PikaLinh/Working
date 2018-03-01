using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Report;
using DevExpress.Web;

namespace WebUI.Controllers
{
    public class OrderReportController : BaseController
    {
        public ActionResult ReportViewerPartial(int? OrderId)// id để report with
        {
            CreateViewBag(OrderId);
            ViewData["Report"] = new HoaDonXtraReport();
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
            HoaDonXtraReport quarterReport = CreateDateReport(OrderId);
            return DevExpress.Web.Mvc.ReportViewerExtension.ExportTo(quarterReport);
        }

        private HoaDonXtraReport CreateDateReport(int? OrderId)
        {
            HoaDonXtraReport report = new HoaDonXtraReport();
            DataSet ds = GetData(OrderId);
            
            report.DataSource = ds;
            report.DataMember = "Detail"; // Lặp lại Detail
            string orderid = OrderId == null ? "" : OrderId.ToString();
            report.Name = "Phieu Kiem Kho -" + orderid; // Export file Name
            return report;
        }

        #region Helper
        private void CreateViewBag(int? OrderId = null)
        {
            ViewBag.OrderId = OrderId;
        }

        private static DataSet GetData(int? OrderId)
        {
            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandText = "usp_HoaDon";
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
       
        #endregion
    }
}
