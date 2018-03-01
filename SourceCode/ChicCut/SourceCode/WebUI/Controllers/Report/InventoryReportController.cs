using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Report;
using DevExpress.Web;
using System.Data;
using ViewModels;

namespace WebUI.Controllers
{
    public class InventoryReportController : Controller
    {
        //
        // GET: /IEOtherMasterReport/
        //public ActionResult ReportPost(InventorySearchViewModel model)// id để report with
        //{
        //    model.WarehouseId = 1;
        //    return new JsonResult()
        //    {
        //        Data = new { WarehouseId = model.WarehouseId, ProductId = model.ProductId }
        //    };
        //}

        public ActionResult ReportViewerPartial()// id để report with
        {
            ViewData["Report"] = new DanhSachXuatNhapTonSPXtraReport();
            return PartialView();
        }
        public ActionResult CallbackReportViewerPartial()
        {
            ViewData["Report"] = CreateDateReport(); // Lấy data từ Store Procedure đưa vào dataset

            return PartialView("ReportViewerPartial");
        }
        public ActionResult ExportReportViewerPartial()
        {
            DanhSachXuatNhapTonSPXtraReport quarterReport = CreateDateReport();
            return DevExpress.Web.Mvc.ReportViewerExtension.ExportTo(quarterReport);
        }

        private DanhSachXuatNhapTonSPXtraReport CreateDateReport()
        {
            DanhSachXuatNhapTonSPXtraReport report = new DanhSachXuatNhapTonSPXtraReport();
            DataSet ds = GetData();

            report.DataSource = ds;
            report.DataMember = "Detail"; // Lặp lại Detail
            report.Name = "Danh sách xuất nhập tồn"; // Export file Name
            return report;
        }
        private static DataSet GetData()
        {
            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandText = "usp_DanhSachXuatNhapTonSp2";
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
