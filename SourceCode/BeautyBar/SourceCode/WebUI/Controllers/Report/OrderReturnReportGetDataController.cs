using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Report;

namespace WebUI.Controllers
{
    public class OrderReturnReportGetDataController : Controller
    {
        //
        // GET: /OrderReturnReportGetData/

        public ActionResult ReportViewerPartial(int? OrderReturnMasterId)// id để report with
        {
            CreateViewBag(OrderReturnMasterId);
            ViewData["Report"] = new HoaDonKhachHangTraHangXtraReport1();
            return PartialView();
        }
        public ActionResult CallbackReportViewerPartial(int? OrderReturnMasterId)
        {
            CreateViewBag(OrderReturnMasterId);
            ViewData["Report"] = CreateDateReport(OrderReturnMasterId); // Lấy data từ Store Procedure đưa vào dataset

            return PartialView("ReportViewerPartial");
        }
        public ActionResult ExportReportViewerPartial(int? OrderReturnMasterId)
        {
            HoaDonKhachHangTraHangXtraReport1 quarterReport = CreateDateReport(OrderReturnMasterId);
            return DevExpress.Web.Mvc.ReportViewerExtension.ExportTo(quarterReport);
        }

        private HoaDonKhachHangTraHangXtraReport1 CreateDateReport(int? OrderReturnMasterId)
        {
            HoaDonKhachHangTraHangXtraReport1 report = new HoaDonKhachHangTraHangXtraReport1();
            DataSet ds = GetData(OrderReturnMasterId);

            report.DataSource = ds;
            report.DataMember = "Detail"; // Lặp lại Detail
            string orderid = OrderReturnMasterId == null ? "" : OrderReturnMasterId.ToString();
            report.Name = "Phieu khach hang tra hang -" + OrderReturnMasterId; // Export file Name
            return report;
        }
        private static DataSet GetData(int? OrderReturnMasterId)
        {
            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandText = "usp_HoaDonKhachHangTraHang";
                    cmd.Parameters.AddWithValue("@OrderReturnMasterId", OrderReturnMasterId);
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
        private void CreateViewBag(int? OrderReturnMasterId = null)
        {
            ViewBag.OrderReturnMasterId = OrderReturnMasterId;
        }

    }
}
