using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers.Report
{
    public class BaoCaoXuatNhapTonController : BaseController
    {
        // GET: BaoCaoXuatNhapTon
        public ActionResult Index()
        {
            ViewBag.FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd");
            ViewBag.ToDate = DateTime.Now.ToString("yyyy-MM-dd");
            CreateSearchViewBag();
            return View();
        }

        private void CreateSearchViewBag(int? StoreId = null, int? WarehouseId = null, int? CategoryId = null)
        {
            //Load cửa hàng
            var store = _context.StoreModel.Where(p => p.Actived == true).ToList();
            ViewBag.StoreId = new SelectList(store, "StoreId", "StoreName", StoreId);

            //Load kho
            var warehouse = _context.WarehouseModel.Where(p => p.Actived == true).ToList();
            ViewBag.WarehouseId = new SelectList(warehouse, "WarehouseId", "WarehouseName", WarehouseId);

            //Load danh mục sản phẩm
            CategoryRepository repository = new CategoryRepository(_context);
            var CategoryList = repository.GetCategoryByParentWithFormat(2).Select(p => new { CategoryId = p.CategoryId, CategoryName = p.CategoryName.Substring(4) }).ToList();
            CategoryList.RemoveAt(0);
            CategoryList.Insert(0, new { CategoryId = 2, CategoryName = "Tất cả sản phẩm" });
            ViewBag.CategoryId = new SelectList(CategoryList, "CategoryId", "CategoryName", CategoryId);
        }

        //1. Sau khi nhấn nút xem thì vào action Report trả về PartialView chưa có dữ liệu
        public ActionResult Report(int StoreId, int? WarehouseId, DateTime? FromDate, DateTime? ToDate, int? CategoryId)
        {
            ViewBag.StoreId = StoreId;
            ViewBag.WarehouseId = WarehouseId;
            ViewBag.CategoryId = CategoryId;
            ViewBag.FromDate = FromDate;
            if (ToDate.HasValue)
            {
                ViewBag.ToDate = ToDate.Value.AddDays(1).AddSeconds(-1);
            }
            return PartialView();
        }
        //2. PartialView Report lại trả về action ReportViewerPartial và khởi tạo mới Report
        public ActionResult ReportViewerPartial(int StoreId, int? WarehouseId, DateTime? FromDate, DateTime? ToDate, int? CategoryId)// id để report with
        {
            ViewBag.StoreId = StoreId;
            ViewBag.WarehouseId = WarehouseId;
            ViewBag.CategoryId = CategoryId;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewData["Report"] = new BaoCaoXuatNhapTonXtraReport();
            return PartialView();
        }
        //3. Sau đó callback lại để lấy dữ liệu
        public ActionResult CallbackReportViewerPartial(int StoreId, int? WarehouseId, DateTime? FromDate, DateTime? ToDate, int? CategoryId)
        {
            ViewBag.StoreId = StoreId;
            ViewBag.WarehouseId = WarehouseId;
            ViewBag.CategoryId = CategoryId;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewData["Report"] = CreateDateReport(StoreId, WarehouseId, FromDate, ToDate, CategoryId); // Lấy data từ Store Procedure đưa vào dataset
            return PartialView("ReportViewerPartial");
        }

        public ActionResult ExportReportViewerPartial(int StoreId, int? WarehouseId, DateTime? FromDate, DateTime? ToDate, int? CategoryId)
        {
            BaoCaoXuatNhapTonXtraReport quarterReport = CreateDateReport(StoreId, WarehouseId, FromDate, ToDate, CategoryId);
            return DevExpress.Web.Mvc.ReportViewerExtension.ExportTo(quarterReport);
        }
        private BaoCaoXuatNhapTonXtraReport CreateDateReport(int StoreId, int? WarehouseId, DateTime? FromDate, DateTime? ToDate, int? CategoryId)
        {
            BaoCaoXuatNhapTonXtraReport report = new BaoCaoXuatNhapTonXtraReport();
            DataSet ds = GetData(StoreId, WarehouseId, FromDate, ToDate, CategoryId);

            report.DataSource = ds;
            // Lặp lại Detail
            report.DataMember = "Detail";
            // Export file Name
            report.Name = "Báo cáo xuất nhập tồn";
            return report;
        }
        private static DataSet GetData(int StoreId, int? WarehouseId, DateTime? FromDate, DateTime? ToDate, int? CategoryId)
        {
            if (FromDate.HasValue)
            {
                FromDate = FromDate.Value.Date;
            }
            if (ToDate.HasValue)
            {
                ToDate = ToDate.Value.Date.AddDays(1).AddSeconds(-1);
            }
            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandText = "usp_BaoCaoXNT";
                    cmd.Parameters.AddWithValue("@FromDate", FromDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDate", ToDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@StoreId", StoreId);
                    cmd.Parameters.AddWithValue("@WarehouseId", WarehouseId);
                    cmd.Parameters.AddWithValue("@CategoryId", CategoryId);
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