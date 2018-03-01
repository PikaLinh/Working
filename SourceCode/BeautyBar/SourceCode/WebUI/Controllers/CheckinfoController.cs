using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using Repository;
using ViewModels;
using System.Data.SqlClient;
using System.Data;


namespace WebUI.Controllers
{
    public class CheckinfoController :BaseController
    {
        //
        // GET: /Checkinfo/
        
        public ActionResult Index()
        {
           CreateViewBag(null,null,null);
           return View();
        }
        //[HttpPost]
        public ActionResult _ProductPartial(ProductSearchViewModel model)
        {
            var List = new List<ProductInfoViewModel>();
            try
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                    {
                        cmd.CommandText = "usp_ListCheckinfo";
                        cmd.Parameters.AddWithValue("@ProductId", model.ProductId);
                        cmd.Parameters.AddWithValue("@ProductName", model.ProductName);
                        cmd.Parameters.AddWithValue("@CustomerLevelId", model.CustomerLevelId);
                        cmd.Parameters.AddWithValue("@txtkhoanggiaduoi", model.txtkhoanggiaduoi);
                        cmd.Parameters.AddWithValue("@txtkhoanggiatren", model.txtkhoanggiatren);
                        cmd.Parameters.AddWithValue("@CategoryId", model.CategoryId);
                        cmd.Parameters.AddWithValue("@OriginOfProductId", model.OriginOfProductId);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        // do
                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var v = ListView(dr);
                                List.Add(v);
                            }
                        }
                        conn.Close();
                    }
                }
            }
            catch
            {
                return PartialView(List);
            }
            return PartialView(List);
        }

        public ProductInfoViewModel ListView(SqlDataReader s)
        {
            ProductInfoViewModel ret = new ProductInfoViewModel();
            try
            {
                if (s["ProductName"] != null)
                {
                    ret.ProductName = s["ProductName"].ToString();
                }
                
                if (s["Price1"] != null)
                {
                    ret.Price1 = (decimal)s["Price1"];
                }
                if (s["Price2"] != null)
                {
                    ret.Price2 = (decimal)s["Price2"];
                }
                if (s["Price3"] != null)
                {
                    ret.Price3 = (decimal)s["Price3"];
                }
                if (s["Price4"] != null)
                {
                    ret.Price4 = (decimal)s["Price4"];
                }

                if (s["OriginOfProduct"] != null)
                {
                    ret.OriginOfProduct = s["OriginOfProduct"].ToString();
                }
                if (s["Inventory"] != null)
                {
                    ret.Inventory = (decimal)s["Inventory"];
                }
            }
            catch //(Exception ex)
            {
            }
            return ret;
        }

        public ActionResult _CustomerPartial(string dienthoai)
        {
            CustomerModel model = _context.CustomerModel.Where(p =>p.Phone.Contains(dienthoai)).FirstOrDefault();
            return PartialView (model);
        }

        public ActionResult _OrderDetailPartial(int CustomerId)
        {
            var idParam = new SqlParameter
            {
                ParameterName = "CustomerId",
                Value = CustomerId,
                SqlDbType = System.Data.SqlDbType.Int
            };
            var lst = _context.Database.
                                           SqlQuery<DetailDebtViewModel>("dbo.usp_ChiTietDonHang @CustomerId", idParam)
                                           .ToList();
            return PartialView(lst);
        }

        private void CreateViewBag(int? CategoryId = null, int? OriginOfProductId = null, int? CustomerLevelId = null)
        {
            //1. CategoryId
            CategoryRepository repository = new CategoryRepository(_context);
            var CategoryList = repository.GetCategoryByParentWithFormat(2).Select(p => new { CategoryId = p.CategoryId, CategoryName = p.CategoryName.Substring(4) }).ToList();
            CategoryList.RemoveAt(0);
            CategoryList.Insert(0, new { CategoryId = 2, CategoryName = "Tất cả sản phẩm" });
            ViewBag.CategoryId = new SelectList(CategoryList, "CategoryId", "CategoryName", CategoryId);

            // 2. OriginOfProductId
            var OriginOfProductList = _context.OriginOfProductModel.OrderBy(p => p.OriginOfProductName).ToList();
            ViewBag.OriginOfProductId = new SelectList(OriginOfProductList, "OriginOfProductId", "OriginOfProductName", OriginOfProductId);

            // 3.CustomerLevel
            var CustomerLevelList = _context.CustomerLevelModel.OrderBy(p => p.CustomerLevelName).ToList();
            ViewBag.CustomerLevelId = new SelectList(CustomerLevelList, "CustomerLevelId", "CustomerLevelName", CustomerLevelId);
        }

        #region GetProductId
        public ActionResult GetProductId(string q)
        {
            var data2 = _context
                        .ProductModel
                        .Where(p => (q == null || ((string.IsNullOrEmpty(p.ProductCode) ? "" : p.ProductCode) + "  " + p.ProductName).Contains(q)) && p.Actived == true && p.ProductStoreCode != null)
                        .Select(p => new
                        {
                            value = p.ProductId,
                            text = (p.ProductCode + " | " + p.ProductName)
                        })
                        .Take(10)
                        .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
