using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;


namespace WebUI.Controllers
{
    public class StockInController : BaseController
    {
        //
        // GET: /StockIn/

        
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public ActionResult Report(int? StoreId, DateTime? ToDate, DateTime? FromDate, int? SupplierId, int? EmployeeId)
        {
            ViewBag.StoreId = StoreId;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.SupplierId = SupplierId;
            ViewBag.EmployeeId = EmployeeId;

            return PartialView();
        }


        //public ActionResult _SearchPartial(StockInSearchViewModel model)
        //{
        //    List<StockInViewModel> ListStockIn = new List<StockInViewModel>();
        //    DataSet ds = new DataSet();
        //    using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
        //    {
        //        conn.Open();
        //        using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
        //        {
        //            cmd.CommandText = "usp_PhieuNhapKho";
        //            cmd.Parameters.AddWithValue("@StoreId", model.StoreId);
        //            cmd.Parameters.AddWithValue("@FromDate", model.FromDate);
        //            cmd.Parameters.AddWithValue("@ToDate", model.ToDate);
        //            cmd.Connection = conn;
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
        //            adapter.Fill(ds);
                   
        //        }
        //        conn.Close();
        //    }
        //    ds.Tables[1].TableName = "Detail";
        //    ListStockIn = ds.Tables[1]
        //                    .AsEnumerable()
        //                    .Select(dataRow => new StockInViewModel
        //                    {
        //                        STT = dataRow.Field<int>("STT"),
        //                        ProductName = dataRow.Field<string>("ProductName"),
        //                        ProductCode = dataRow.Field<string>("ProductCode"),
        //                        UnitName = dataRow.Field<string>("UnitName"),
        //                        Qty = dataRow.Field<decimal>("Qty"),
        //                        Price = dataRow.Field<decimal>("Price"),
        //                        UnitPrice = dataRow.Field<decimal>("UnitPrice")
        //                    }).ToList();
        //    return PartialView(ListStockIn);
        //}


        private void CreateViewBag(int? StoreId = null, int? SupplierId = null, int? EmployeeId = null)
        {
            //0. StoreId
            var StoreList = _context.StoreModel.OrderBy(p => p.StoreName).Where(p =>
                p.Actived == true &&
                currentEmployee.StoreId == null ||
                p.StoreId == currentEmployee.StoreId
                )
                .ToList();
            ViewBag.StoreId = new SelectList(StoreList, "StoreId", "StoreName", StoreId);

            //1. SupplierId
            var SupplierList = _context.SupplierModel.OrderBy(p => p.SupplierName).Where(p => p.Actived == true)
                .ToList();
            ViewBag.SupplierId = new SelectList(SupplierList, "SupplierId", "SupplierName", SupplierId);

            //2.
            var EmployeeList = _context.EmployeeModel.OrderBy(p => p.FullName).Where(p => p.Actived == true)
                .ToList();
            ViewBag.EmployeeId = new SelectList(EmployeeList, "EmployeeId", "FullName", EmployeeId);

        }

    }
}
