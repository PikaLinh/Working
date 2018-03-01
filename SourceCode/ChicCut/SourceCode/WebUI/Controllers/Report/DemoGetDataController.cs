using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Report;

namespace WebUI.Controllers
{
    public class DemoGetDataController : Controller
    {
        //
        // GET: /DemoGetData/

        public ActionResult ReportViewerPartial()// id để report with
        {
            ViewData["Report"] = new DemoXtraReport1();
            return PartialView();
        }
        public ActionResult CallbackReportViewerPartial()
        {
            ViewData["Report"] = CreateDateReport(); // Lấy data từ Store Procedure đưa vào dataset

            return PartialView("ReportViewerPartial");
        }

        public ActionResult ExportReportViewerPartial()
        {
            DemoXtraReport1 quarterReport = CreateDateReport();
            return DevExpress.Web.Mvc.ReportViewerExtension.ExportTo(quarterReport);
        }

        private DemoXtraReport1 CreateDateReport()
        {
            DemoXtraReport1 report = new DemoXtraReport1();
            DataSet ds = GetData();

            report.DataSource = ds;
            report.DataMember = "Detail"; // Lặp lại Detail
            report.Name = "Phieu thu - chi tien mat : " ; // Export file Name
            return report;
        }

        private static DataSet GetData()
        {
            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandText = "usp_DeMoReport";
                  //  cmd.Parameters.AddWithValue("@TransactionId", TransactionId);
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
