using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Report;

namespace WebUI.Controllers.Report
{
    public class BaoCaoTonKhoOnHandController : BaseController
    {
        // GET: BaoCaoTonKhoOnHand
        public ActionResult Index()
        {
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
        public ActionResult Report(int StoreId, int? WarehouseId, DateTime? Date, int? CategoryId)
        {
            ViewBag.StoreId = StoreId;
            ViewBag.WarehouseId = WarehouseId;
            ViewBag.CategoryId = CategoryId;
            ViewBag.Date = Date;
            return PartialView();
        }
        //2. PartialView Report lại trả về action ReportViewerPartial và khởi tạo mới Report
        public ActionResult ReportViewerPartial(int StoreId, int? WarehouseId, DateTime? Date, int? CategoryId)// id để report with
        {
            ViewBag.StoreId = StoreId;
            ViewBag.WarehouseId = WarehouseId;
            ViewBag.CategoryId = CategoryId;
            ViewBag.Date = Date;
            ViewData["Report"] = new BaoCaoTonKhoOnHandXtraReport();
            return PartialView();
        }
        //3. Sau đó callback lại để lấy dữ liệu
        public ActionResult CallbackReportViewerPartial(int StoreId, int? WarehouseId, DateTime? Date, int? CategoryId)
        {
            ViewBag.StoreId = StoreId;
            ViewBag.WarehouseId = WarehouseId;
            ViewBag.CategoryId = CategoryId;
            ViewBag.Date = Date;
            ViewData["Report"] = CreateDateReport(StoreId, WarehouseId, Date, CategoryId); // Lấy data từ Store Procedure đưa vào dataset
            return PartialView("ReportViewerPartial");
        }

        public ActionResult ExportReportViewerPartial(int StoreId, int? WarehouseId, DateTime? Date, int? CategoryId)
        {
            BaoCaoTonKhoOnHandXtraReport quarterReport = CreateDateReport(StoreId, WarehouseId, Date, CategoryId);
            return DevExpress.Web.Mvc.ReportViewerExtension.ExportTo(quarterReport);
        }
        private BaoCaoTonKhoOnHandXtraReport CreateDateReport(int StoreId, int? WarehouseId, DateTime? Date, int? CategoryId)
        {
            BaoCaoTonKhoOnHandXtraReport report = new BaoCaoTonKhoOnHandXtraReport();
            DataSet ds = GetData(StoreId, WarehouseId, Date, CategoryId);

            report.DataSource = ds;
            // Lặp lại Detail
            report.DataMember = "Detail";
            // Export file Name
            report.Name = "Báo cáo tồn kho"; 
            return report;
        }
        private static DataSet GetData(int StoreId, int? WarehouseId, DateTime? Date, int? CategoryId)
        {
            if (Date.HasValue)
            {
                Date = Date.Value.Date.AddDays(1).AddSeconds(-1);
            }

            DataSet ds = new DataSet();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandText = "usp_BaoCaoTonKho";
                    cmd.Parameters.AddWithValue("@Date", Date ?? (object)DBNull.Value);
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