using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Report;

namespace WebUI.Controllers.Report
{
    public class InventoryProductReportViewController : Controller
    {
        //
        // GET: /InventoryProductReportView/

        public ActionResult ReportViewerPartial(int? StoreId, DateTime? ToDate, DateTime? FromDate, int? SupplierId, int? EmployeeId, int? WarehouseId)// id để report with
        {
            CreateViewBag(StoreId, ToDate, FromDate, SupplierId, EmployeeId, WarehouseId);
            ViewData["Report"] = new BaocaoTonkhoXtraReport();
            return PartialView();
        }
        public ActionResult CallbackReportViewerPartial(int? StoreId, DateTime? ToDate, DateTime? FromDate, int? SupplierId, int? EmployeeId, int? WarehouseId)
        {
            CreateViewBag(StoreId, ToDate, FromDate, SupplierId, EmployeeId, WarehouseId);
            ViewData["Report"] = CreateDateReport(StoreId, ToDate, FromDate, SupplierId, EmployeeId, WarehouseId); // Lấy data từ Store Procedure đưa vào dataset

            return PartialView("ReportViewerPartial");
        }
        public ActionResult ExportReportViewerPartial(int? StoreId, DateTime? ToDate, DateTime? FromDate, int? SupplierId, int? EmployeeId, int? WarehouseId)
        {
            BaocaoTonkhoXtraReport quarterReport = CreateDateReport(StoreId, ToDate, FromDate, SupplierId, EmployeeId, WarehouseId);
            return DevExpress.Web.Mvc.ReportViewerExtension.ExportTo(quarterReport);
        }

        private BaocaoTonkhoXtraReport CreateDateReport(int? StoreId, DateTime? ToDate, DateTime? FromDate, int? SupplierId, int? EmployeeId, int? WarehouseId)
        {
            BaocaoTonkhoXtraReport report = new BaocaoTonkhoXtraReport();
            DataSet ds = GetData(StoreId, ToDate, FromDate, SupplierId, EmployeeId, WarehouseId);

            report.DataSource = ds;
            report.DataMember = "Detail"; // Lặp lại Detail
            string orderid = StoreId == null ? "" : StoreId.ToString();
            report.Name = "Bao cao ton kho -" + StoreId; // Export file Name
            return report;
        }
        private static DataSet GetData(int? StoreId, DateTime? ToDate, DateTime? FromDate, int? SupplierId, int? EmployeeId, int? WarehouseId)
        {
            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandText = "usp_Baocaotonkho";
                    cmd.Parameters.AddWithValue("@StoreId", StoreId);
                    cmd.Parameters.AddWithValue("@FromDate", FromDate);
                    cmd.Parameters.AddWithValue("@ToDate", ToDate);
                    cmd.Parameters.AddWithValue("@SupplierId", SupplierId);
                    cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                    cmd.Parameters.AddWithValue("@WarehouseId", WarehouseId);
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

        private void CreateViewBag(int? StoreId = null, DateTime? ToDate = null, DateTime? FromDate = null, int? SupplierId = null, int? EmployeeId = null, int? WarehouseId=null)
        {
            ViewBag.StoreId = StoreId;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.SupplierId = SupplierId;
            ViewBag.EmployeeId = EmployeeId;
            ViewBag.WarehouseId = WarehouseId;
        }

    }
}
