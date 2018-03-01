using EntityModels;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using ViewModels;


namespace WebUI.Controllers
{
    public class ProductUpdatesController : BaseController
    {
        //
        // GET: /ProductUpdates/
        
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }

        private void CreateViewBag(int? CategoryId = null, int? OriginOfProductId = null, int? CustomerLevelId = null, int? WarehouseId = null)
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

            //4. WarehouseId
            var WarehouseList = _context.WarehouseModel.OrderBy(p => p.WarehouseName).ToList();
            ViewBag.WarehouseId = new SelectList(WarehouseList, "WarehouseId", "WarehouseName", WarehouseId);
        }

        public ActionResult _ProductPartial(ProductSearchViewModel model)
        {
            var List = new List<ProductInfoViewModel>();
            try
            {
                GetData(model, ref List);
            }
            catch
            {
                return PartialView(List);
            }
            return PartialView(List);
        }

        private void GetData(ProductSearchViewModel model, ref List<ProductInfoViewModel> List)
        {
            //using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            //{
            //    using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
            //    {
            //        cmd.CommandText = "usp_ListCheckinfo";
            //        cmd.Parameters.AddWithValue("@ProductId", model.ProductId);
            //        cmd.Parameters.AddWithValue("@ProductName", model.ProductName);
            //        cmd.Parameters.AddWithValue("@CustomerLevelId", model.CustomerLevelId);
            //        cmd.Parameters.AddWithValue("@txtkhoanggiaduoi", model.txtkhoanggiaduoi);
            //        cmd.Parameters.AddWithValue("@txtkhoanggiatren", model.txtkhoanggiatren);
            //        cmd.Parameters.AddWithValue("@CategoryId", model.CategoryId);
            //        cmd.Parameters.AddWithValue("@OriginOfProductId", model.OriginOfProductId);
            //        cmd.Connection = conn;
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        conn.Open();
            //        // do
            //        using (var dr = cmd.ExecuteReader())
            //        {
            //            while (dr.Read())
            //            {
            //                var v = ListView(dr);
            //                List.Add(v);
            //            }
            //        }
            //        conn.Close();
            //    }
            //}
            SqlParameter paraProductId = new SqlParameter("@ProductId", model.ProductId);
            SqlParameter paraProductName = new SqlParameter("@ProductName", model.ProductName);
            SqlParameter paraCustomerLevelId = new SqlParameter("@CustomerLevelId", model.CustomerLevelId);
            SqlParameter paratxtkhoanggiaduoi = new SqlParameter("@txtkhoanggiaduoi", model.txtkhoanggiaduoi);
            SqlParameter paratxtkhoanggiatren = new SqlParameter("@txtkhoanggiatren", model.txtkhoanggiatren);
            SqlParameter paraCategoryId = new SqlParameter("@CategoryId", model.CategoryId);
            SqlParameter paraOriginOfProductId = new SqlParameter("@OriginOfProductId", model.OriginOfProductId);

            #region Kiểm tra giá trị tham số null => khởi tạo giá trị mặc định
            if (!model.ProductId.HasValue)
            {
                paraProductId.Value = DBNull.Value;
            }
            if (string.IsNullOrEmpty(model.ProductName))
            {
                paraProductName.Value = DBNull.Value;
            }
            if (!model.txtkhoanggiaduoi.HasValue)
            {
                paratxtkhoanggiaduoi.Value = DBNull.Value;
            }
            if (!model.txtkhoanggiatren.HasValue)
            {
                paratxtkhoanggiatren.Value = DBNull.Value;
            }
            if (!model.CategoryId.HasValue)
            {
                paraCategoryId.Value = 2;
            }
            if (!model.OriginOfProductId.HasValue)
            {
                paraOriginOfProductId.Value = DBNull.Value;
            }
            #endregion

            List = _context.Database.
                                       SqlQuery<ProductInfoViewModel>
                                       ("usp_ListCheckinfo @ProductId, @ProductName, @CustomerLevelId, @txtkhoanggiaduoi, @txtkhoanggiatren, @CategoryId, @OriginOfProductId",
                                       paraProductId
                                       , paraProductName
                                       , paraCustomerLevelId
                                       , paratxtkhoanggiaduoi
                                       , paratxtkhoanggiatren
                                       , paraCategoryId
                                       , paraOriginOfProductId
                                       ).ToList();
        }

        //public ProductInfoViewModel ListView(SqlDataReader s)
        //{
        //    ProductInfoViewModel ret = new ProductInfoViewModel();
        //    try
        //    {
        //        if (s["ProductId"] != null)
        //        {
        //            ret.ProductId = (int)s["ProductId"];
        //        }
        //        if (s["ProductName"] != null)
        //        {
        //            ret.ProductName = s["ProductName"].ToString();
        //        }
        //        if (s["ImportPrice"] != null)
        //        {
        //            ret.ImportPrice = (decimal)s["ImportPrice"];
        //        }
        //        if (s["ShippingFee"] != null)
        //        {
        //            ret.ShippingFee = (decimal)s["ShippingFee"];
        //        }
        //        if (s["ExchangeRate"] != null)
        //        {
        //            ret.ExchangeRate = (decimal)s["ExchangeRate"];
        //        }

        //        if (s["Price1"] != null)
        //        {
        //            ret.Price1 = (decimal)s["Price1"];
        //        }
        //        if (s["Price2"] != null)
        //        {
        //            ret.Price2 = (decimal)s["Price2"];
        //        }
        //        if (s["Price3"] != null)
        //        {
        //            ret.Price3 = (decimal)s["Price3"];
        //        }
        //        if (s["Price4"] != null)
        //        {
        //            ret.Price4 = (decimal)s["Price4"];
        //        }
        //        if (s["OriginOfProduct"] != null)
        //        {
        //            ret.OriginOfProduct = s["OriginOfProduct"].ToString();
        //        }
        //        if (s["ProductStoreCode"] != null)
        //        {
        //            ret.ProductStoreCode = s["ProductStoreCode"].ToString();
        //        }
        //        if (s["ProductCode"] != null)
        //        {
        //            ret.ProductCode = s["ProductCode"].ToString();
        //        }
        //        if (s["Inventory"] != null)
        //        {
        //            ret.Inventory = (decimal)s["Inventory"];
        //        }

        //    }
        //    catch
        //    {

        //    }
        //    return ret;
        //}

        #region Export
        public ActionResult Export(ProductSearchViewModel model)
        {
            List<ProductInfoViewModel> listProduct = new List<ProductInfoViewModel>();
            GetData(model, ref listProduct);

            //Tên file
            string strName = string.Format("{0}_{1}_{2}", "Danh_sach_san_pham_cap_nhat", DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss"), ".xlsx");
            Byte[] report = GenerateReport(listProduct);

            string handle = Guid.NewGuid().ToString();

            Session[handle] = report;
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = strName }
            };
        }

        private Byte[] GenerateReport(List<ProductInfoViewModel> listProduct)
        {
            using (ExcelPackage p = new ExcelPackage())
            {
                //set the workbook properties and add a default sheet in it
                SetWorkbookProperties(p);
                //Create a sheet
                ExcelWorksheet ws = CreateSheet(p, "Cập nhật thông tin sản phẩm");


                //Merging cells and create a center heading for out table
                ws.Cells[2, 1].Value = "Cập nhật thông tin sản phẩm";
                ws.Cells[2, 1, 2, 12].Merge = true;
                ws.Cells[2, 1, 2, 12].Style.Font.Bold = true;
                ws.Cells[2, 1, 2, 12].Style.Font.Size = 22;
                ws.Cells[2, 1, 2, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                #region //lưu ý
                ws.Cells[3, 1].Value = "Lưu ý : Chỉ thay đổi những thông tin ô màu trắng ";
                ws.Cells[4, 1].Value = "               Không thay đổi những thông tin ô màu vàng ";

                //màu chữ
                ws.Cells[3, 1].Style.Font.Color.SetColor(Color.Red);
                ws.Cells[4, 1].Style.Font.Color.SetColor(Color.Red);

                //border
                ws.Cells[3, 4].Style.Border.Bottom.Style = ws.Cells[3, 4].Style.Border.Top.Style = ws.Cells[3, 4].Style.Border.Left.Style = ws.Cells[3, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells[4, 4].Style.Border.Bottom.Style = ws.Cells[4, 4].Style.Border.Top.Style = ws.Cells[4, 4].Style.Border.Left.Style = ws.Cells[4, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //BackgroundColor
                ws.Cells[3, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[4, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[3, 4].Style.Fill.BackgroundColor.SetColor(Color.White);
                ws.Cells[4, 4].Style.Fill.BackgroundColor.SetColor(Color.Gold);
                #endregion

                int rowIndex = 6;
                int row = 7;
                CreateHeader(ws, ref rowIndex);
                CreateData(ws, ref row, listProduct);
                Byte[] bin = p.GetAsByteArray();

                return bin;
            }

            // return File(bin, "application/vnd.ms-excel", log.FileName);
        }

        private static void SetWorkbookProperties(ExcelPackage p)
        {
            //Here setting some document properties
            p.Workbook.Properties.Author = "PhongNguyen";
            p.Workbook.Properties.Title = "Danh sách sản phẩm cập nhật";
        }

        private static ExcelWorksheet CreateSheet(ExcelPackage p, string sheetName)
        {
            p.Workbook.Worksheets.Add(sheetName);
            ExcelWorksheet ws = p.Workbook.Worksheets[1];
            ws.Name = sheetName; //Setting Sheet's name
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet
            return ws;
        }

        private static void CreateHeader(ExcelWorksheet worksheet, ref int rowIndex)
        {
            #region Định dạng Excel
            //độ rộng cột
            worksheet.Column(1).Width = 5;
            worksheet.Column(2).Width = 15;
            worksheet.Column(3).Width = 26;
            worksheet.Column(4).Width = 25;
            worksheet.Column(5).Width = 50;
            worksheet.Column(6).Width = 15;
            worksheet.Column(7).Width = 15;
            worksheet.Column(8).Width = 20;
            worksheet.Column(9).Width = 15;
            worksheet.Column(10).Width = 15;
            worksheet.Column(11).Width = 15;
            worksheet.Column(12).Width = 15;
            //format date cột i

            //tiêu đề cột
            worksheet.Cells[rowIndex, 1].Value = "STT";
            worksheet.Cells[rowIndex, 2].Value = "Mã Sản Phẩm ID";
            worksheet.Cells[rowIndex, 3].Value = "Mã Sản Phẩm Cửa hàng";
            worksheet.Cells[rowIndex, 4].Value = "Mã Sản Phẩm";
            worksheet.Cells[rowIndex, 5].Value = "Tên Sản Phẩm";
            worksheet.Cells[rowIndex, 6].Value = "Giá nhập";
            worksheet.Cells[rowIndex, 7].Value = "Tỷ giá";
            worksheet.Cells[rowIndex, 8].Value = "Phí vận chuyển";
            worksheet.Cells[rowIndex, 9].Value = "Vip";
            worksheet.Cells[rowIndex, 10].Value = "Vip-Bạc";
            worksheet.Cells[rowIndex, 11].Value = "Vip-Vàng";
            worksheet.Cells[rowIndex, 12].Value = "Vip-Bạch kim";
            for (int i = 1; i <= 12; i++)
            {
                var cell = worksheet.Cells[rowIndex, i];
                var fill = cell.Style.Fill;
                fill.PatternType = ExcelFillStyle.Solid;
                fill.BackgroundColor.SetColor(Color.Cyan);

                //Setting Top/left,right/bottom borders.
                var border = cell.Style.Border;
                border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
            }

            #endregion
        }

        private static void CreateData(ExcelWorksheet worksheet, ref int rowIndex, List<ProductInfoViewModel> listProduct)
        {
            foreach (ProductInfoViewModel p in listProduct)
            {
                for (int i = 1; i <= 3; ++i)
                {
                    var cell = worksheet.Cells[rowIndex, i];
                    var fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(Color.Gold);
                }
                worksheet.Cells[rowIndex, 1].Value = rowIndex - 6;
                worksheet.Cells[rowIndex, 2].Value = p.ProductId;
                worksheet.Cells[rowIndex, 3].Value = p.ProductStoreCode;
                worksheet.Cells[rowIndex, 4].Value = p.ProductCode;
                worksheet.Cells[rowIndex, 5].Value = p.ProductName;
                worksheet.Cells[rowIndex, 6].Value = p.ImportPrice;
                worksheet.Cells[rowIndex, 7].Value = p.ExchangeRate;
                worksheet.Cells[rowIndex, 8].Value = p.ShippingFee;
                worksheet.Cells[rowIndex, 9].Value = p.Price1;
                worksheet.Cells[rowIndex, 10].Value = p.Price2;
                worksheet.Cells[rowIndex, 11].Value = p.Price3;
                worksheet.Cells[rowIndex, 12].Value = p.Price4;
                rowIndex++;
            }
        }

        public ActionResult Download(string fileGuid, string fileName)
        {
            if (Session[fileGuid] != null)
            {
                byte[] data = Session[fileGuid] as byte[];
                return File(data, "application/vnd.ms-excel", fileName);
            }
            else
            {
                return new EmptyResult();
            }
        }

        #endregion

        #region Import
        [HttpPost]
        public ActionResult Import(HttpPostedFileBase excelfile)
        {
            try
            {
                if (excelfile == null || excelfile.ContentLength == 0)
                {
                    return Json("Bạn vui lòng chọn 1 file excel", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    using (var package = new ExcelPackage(excelfile.InputStream))
                    {
                        using (TransactionScope ts = new TransactionScope())
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                            int col = 1;
                            int TotalQty = 0;
                            #region  // Bước 1 : insert ProductUpdateMasterModel
                            ProductUpdateMasterModel master = new ProductUpdateMasterModel()
                            {
                                ProductUpdateMasterCode = GetProductUpdateMasterCode(),
                                CreateDate = DateTime.Now,
                                CreateAccount = currentAccount.UserName,
                                CreatedEmployeeId = currentEmployee.EmployeeId,
                                CreatedIEOther = false,
                                Actived = true
                            };
                            _context.Entry(master).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();// Lưu để lấy masterID
                            
                            #endregion

                            #region // Bước 2 : insert ProductUpdateMasterModel


                            var list = (from p in _context.ProductModel
                                        join v1 in _context.ProductPriceModel on p.ProductId equals v1.ProductId
                                        join v2 in _context.ProductPriceModel on p.ProductId equals v2.ProductId
                                        join v3 in _context.ProductPriceModel on p.ProductId equals v3.ProductId
                                        join v4 in _context.ProductPriceModel on p.ProductId equals v4.ProductId
                                        where v1.CustomerLevelId == 1 &&
                                             v2.CustomerLevelId == 2 &&
                                             v3.CustomerLevelId == 3 &&
                                             v4.CustomerLevelId == 4 &&
                                             p.Actived == true
                                        select new
                                        {
                                            ProductId = p.ProductId,
                                            ProductCode = p.ProductCode,
                                            ProductStoreCode = p.ProductStoreCode,
                                            ProductName = p.ProductName,
                                            ImportPrice = p.ImportPrice,
                                            ExchangeRate = p.ExchangeRate,
                                            ShippingFee = p.ShippingFee,
                                            Vip = v1.Price,
                                            VipBac = v2.Price,
                                            VipVang = v3.Price,
                                            VipBachkim = v4.Price
                                        }).ToList();

                            ProductUpdateDetailModel details; 
                            for (int row = 7; worksheet.Cells[row, col].Value != null; row++)
                            {
                                #region//Gán giá trị tạm biến số
                                int ProductId = 0;
                                decimal ImportPrice = 0;
                                decimal ShippingFee = 0;
                                decimal ExchangeRate = 0;
                                decimal pprice1 = 0;
                                decimal pprice2 = 0;
                                decimal pprice3 = 0;
                                decimal pprice4 = 0;
                                #endregion

                                #region // Kiểm tra giá trị cột
                                //Mã sản phẩm
                                if (worksheet.Cells[row, 2].Text == "")
                                {
                                    return Json(" Dòng " + row + " Cột 'Mã Sản Phẩm ID' bị rỗng !", JsonRequestBehavior.AllowGet);

                                }
                                else if (int.TryParse(worksheet.Cells[row, 2].Value.ToString(), out ProductId) == false)
                                {
                                    return Json(" Dòng " + row + " Cột 'Mã Sản Phẩm ID' không phải số !", JsonRequestBehavior.AllowGet);
                                }

                                //Mã Sản Phẩm Cửa hàng
                                if (worksheet.Cells[row, 3].Text == "")
                                {
                                    return Json(" Dòng " + row + " Cột 'Mã Sản Phẩm Cửa hàng' bị rỗng !", JsonRequestBehavior.AllowGet);
                                }

                                //Tên Sản Phẩm
                                if (worksheet.Cells[row, 5].Text == "")
                                {
                                    return Json(" Dòng " + row + " Cột 'Tên Sản Phẩm' bị rỗng !", JsonRequestBehavior.AllowGet);
                                }

                                //Giá nhập
                                if (worksheet.Cells[row, 6].Text == "")
                                {
                                    return Json(" Dòng " + row + " Cột 'Giá nhập' bị rỗng !", JsonRequestBehavior.AllowGet);
                                }
                                else if (decimal.TryParse(worksheet.Cells[row, 6].Value.ToString(), out ImportPrice) == false)
                                {
                                    return Json(" Dòng " + row + " Cột 'Giá nhập' không phải số !", JsonRequestBehavior.AllowGet);
                                }

                                //Tỷ giá
                                if (worksheet.Cells[row, 7].Text == "")
                                {
                                    return Json(" Dòng " + row + " Cột 'Tỷ giá' bị rỗng !", JsonRequestBehavior.AllowGet);
                                }
                                else if (decimal.TryParse(worksheet.Cells[row, 7].Value.ToString(), out ExchangeRate) == false)
                                {
                                    return Json(" Dòng " + row + " Cột 'Tỷ giá' không phải số !", JsonRequestBehavior.AllowGet);
                                }

                                //Phí vận chuyển
                                if (worksheet.Cells[row, 8].Text == "")
                                {
                                    return Json(" Dòng " + row + " Cột 'Phí vận chuyển' bị rỗng !", JsonRequestBehavior.AllowGet);
                                }
                                else if (decimal.TryParse(worksheet.Cells[row, 8].Value.ToString(), out ShippingFee) == false)
                                {
                                    return Json(" Dòng " + row + " Cột 'Phí vận chuyển' không phải số !", JsonRequestBehavior.AllowGet);
                                }

                                //Vip
                                if (worksheet.Cells[row, 9].Text == "")
                                {
                                    return Json(" Dòng " + row + " Cột 'Vip' bị rỗng !", JsonRequestBehavior.AllowGet);
                                }
                                else if (decimal.TryParse(worksheet.Cells[row, 9].Value.ToString(), out pprice1) == false)
                                {
                                    return Json(" Dòng " + row + " Cột 'Vip' không phải số !", JsonRequestBehavior.AllowGet);
                                }

                                //Vip-Bạc
                                if (worksheet.Cells[row, 10].Text == "")
                                {
                                    return Json(" Dòng " + row + " Cột 'Vip-Bạc' bị rỗng !", JsonRequestBehavior.AllowGet);
                                }
                                else if (decimal.TryParse(worksheet.Cells[row, 10].Value.ToString(), out pprice2) == false)
                                {
                                    return Json(" Dòng " + row + " Cột 'Vip-Bạc' không phải số !", JsonRequestBehavior.AllowGet);
                                }

                                //Vip-Vàng
                                if (worksheet.Cells[row, 11].Text == "")
                                {
                                    return Json(" Dòng " + row + " Cột 'Vip-Vàng' bị rỗng !", JsonRequestBehavior.AllowGet);
                                }
                                else if (decimal.TryParse(worksheet.Cells[row, 11].Value.ToString(), out pprice3) == false)
                                {
                                    return Json(" Dòng " + row + " Cột 'Vip-Vàng' không phải số !", JsonRequestBehavior.AllowGet);
                                }

                                //Vip-Bạch kim
                                if (worksheet.Cells[row, 12].Text == "")
                                {
                                    return Json(" Dòng " + row + " Cột 'Vip-Bạch kim' bị rỗng !", JsonRequestBehavior.AllowGet);
                                }
                                else if (decimal.TryParse(worksheet.Cells[row, 12].Value.ToString(), out pprice4) == false)
                                {
                                    return Json(" Dòng " + row + " Cột 'Vip-Bạch kim' không phải số !", JsonRequestBehavior.AllowGet);
                                }

                                #endregion

                                #region//Gán giá trị biến string
                                string ProductStoreCode =worksheet.Cells[row, 3].Value.ToString();
                                string ProductCode = "";
                                if (worksheet.Cells[row, 4].Text != "")
                                {
                                    ProductCode = worksheet.Cells[row, 4].Value.ToString();
                                }
                                string ProductName = worksheet.Cells[row, 5].Value.ToString();
                                #endregion

                                foreach (var item in list)
                                {
                                    if (item.ProductId == ProductId)
                                    {
                                        if(item.ProductCode != ProductCode ||
                                           item.ProductStoreCode != ProductStoreCode ||
                                           item.ProductName != ProductName ||
                                           item.ImportPrice != ImportPrice ||
                                           item.ExchangeRate != ExchangeRate ||
                                           item.ShippingFee != ShippingFee ||
                                           item.Vip != pprice1 ||
                                           item.VipBac != pprice2 ||
                                           item.VipVang != pprice3 ||
                                           item.VipBachkim !=  pprice4)
                                        
                                        {
                                            #region // Update
                                            details = new ProductUpdateDetailModel();
                                            details.ProductUpdateMasterId = master.ProductUpdateMasterId;
                                            details.ProductId = ProductId;
                                            details.ProductStoreCode = ProductStoreCode;
                                            details.ProductCode = ProductCode;
                                            details.ProductName = ProductName;
                                            details.ImportPrice = ImportPrice;
                                            details.ExchangeRate = ExchangeRate;
                                            details.ShippingFee = ShippingFee;
                                            details.Price1 = pprice1;
                                            details.Price2 = pprice2;
                                            details.Price3 = pprice3;
                                            details.Price4 = pprice4;
                                            details.Actived = true;
                                            _context.Entry(details).State = System.Data.Entity.EntityState.Added;
                                            _context.SaveChanges();
                                            #endregion
                                            TotalQty++;

                                        }
                                    }
                                }
                                //ProductModel p = _context.ProductModel.Where(m => m.ProductId == ProductId).FirstOrDefault();
                                //if (p != null)
                                //{
                                //    decimal? Price1 = _context.ProductPriceModel.Where(c => c.ProductId == ProductId && c.CustomerLevelId == 1).Select(c => c.Price).FirstOrDefault();
                                //    decimal? Price2 = _context.ProductPriceModel.Where(c => c.ProductId == ProductId && c.CustomerLevelId == 2).Select(c => c.Price).FirstOrDefault();
                                //    decimal? Price3 = _context.ProductPriceModel.Where(c => c.ProductId == ProductId && c.CustomerLevelId == 3).Select(c => c.Price).FirstOrDefault();
                                //    decimal? Price4 = _context.ProductPriceModel.Where(c => c.ProductId == ProductId && c.CustomerLevelId == 4).Select(c => c.Price).FirstOrDefault();
                                //    if ((p.ProductCode != ProductCode ||
                                //                     p.ProductStoreCode != ProductStoreCode ||
                                //                     p.ProductName != ProductName ||
                                //                     p.ImportPrice != ImportPrice ||
                                //                     p.ExchangeRate != ExchangeRate ||
                                //                     p.ShippingFee != ShippingFee ||
                                //                     Price1 != pprice1 ||
                                //                     Price2 != pprice2 ||
                                //                     Price3 != pprice3 ||
                                //                     Price4 != pprice4))
                                //    {
                                //        #region // Update
                                //        details = new ProductUpdateDetailModel();
                                //        details.ProductUpdateMasterId = master.ProductUpdateMasterId;
                                //        details.ProductId = ProductId;
                                //        details.ProductStoreCode = ProductStoreCode;
                                //        details.ProductCode = ProductCode;
                                //        details.ProductName = ProductName;
                                //        details.ImportPrice = ImportPrice;
                                //        details.ExchangeRate = ExchangeRate;
                                //        details.ShippingFee = ShippingFee;
                                //        details.Price1 = pprice1;
                                //        details.Price2 = pprice2;
                                //        details.Price3 = pprice3;
                                //        details.Price4 = pprice4;
                                //        details.Actived = true;
                                //        _context.Entry(details).State = System.Data.Entity.EntityState.Added;
                                //        _context.SaveChanges();
                                //        #endregion
                                //        TotalQty++;
                                //    }
                                //}
                            }
                            #endregion

                            #region // Bước 3: Cập nhật master
                            master.TotalQty = TotalQty;
                            _context.Entry(master).State = System.Data.Entity.EntityState.Modified;
                            _context.SaveChanges();// Lưu để lấy masterID
                            #endregion
                            ts.Complete();
                            if (TotalQty == 0)
                            {
                                return Json("Danh sách sản phẩm chưa được sửa đổi !", JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json("success", JsonRequestBehavior.AllowGet);

                            }
                        }
                    }
                }

            }
            catch
            {
                return Json("Lỗi! Vui lòng liên hệ kĩ thuật viên để được giúp đỡ !", JsonRequestBehavior.AllowGet);

            }

        }
        #endregion

        #region Hàm GetProductUpdateMasterCode
        public string GetProductUpdateMasterCode()
        {
            // Tìm giá trị STT order code
            string OrderCodeToFind = string.Format("{0}-{1}{2}", "PCN", (DateTime.Now.Year).ToString().Substring(2), (DateTime.Now.Month).ToString().Length == 1 ? "0" + (DateTime.Now.Month).ToString() : (DateTime.Now.Month).ToString());
            var Resuilt = _context.ProductUpdateMasterModel.OrderByDescending(p => p.ProductUpdateMasterId).Where(p => p.ProductUpdateMasterCode.Contains(OrderCodeToFind)).Select(p => p.ProductUpdateMasterCode).FirstOrDefault();
            string OrderCode = "";
            if (Resuilt != null)
            {
                int LastNumber = Convert.ToInt32(Resuilt.Substring(9)) + 1;
                string STT = "";
                switch (LastNumber.ToString().Length)
                {
                    case 1: STT = "000" + LastNumber.ToString(); break;
                    case 2: STT = "00" + LastNumber.ToString(); break;
                    case 3: STT = "0" + LastNumber.ToString(); break;
                    default: STT = LastNumber.ToString(); break;
                }
                OrderCode = string.Format("{0}{1}", Resuilt.Substring(0, 9), STT);
            }
            else
            {
                OrderCode = string.Format("{0}-{1}", OrderCodeToFind, "0001");
            }
            return OrderCode;
        }
        #endregion

        public ActionResult _ProductUpdateList()
        {
            var list = _context.ProductUpdateMasterModel
                                .Where(p => p.Actived == true && p.TotalQty > 0)
                                .OrderByDescending(p => p.ProductUpdateMasterId)
                                .ToList();
            if (list == null)
            {
                list = new List<ProductUpdateMasterModel>();
            }
            return PartialView(list);
        }

        public ActionResult ProductUpdateDetails(int? id)
        {
            var List = _context.ProductUpdateDetailModel
                                .Where(p => p.ProductUpdateMasterId == id && p.Actived == true)
                                .OrderBy(p => p.ProductName)
                                .ToList();

            var MaxMasterId = _context.ProductUpdateMasterModel
                                     .Where(p => p.Actived && p.TotalQty > 0)
                                     .OrderByDescending(p => p.ProductUpdateMasterId)
                                     .FirstOrDefault();
            if (MaxMasterId != null)
            {
                if (!MaxMasterId.CreatedIEOther)
                {
                    ViewBag.isLast = id.HasValue ? (id == MaxMasterId.ProductUpdateMasterId) ? 1 : 0 : 0;
                }
                else
                {
                    ViewBag.isLast = 0;
                }
            }
            else
            {
                ViewBag.isLast = 0;
            }


            if (List == null)
            {
                List = new List<ProductUpdateDetailModel>();
            }
            return PartialView(List);
        }

        #region Huỷ đơn hàng
        
        public ActionResult Cancel(int id)
        {
            ProductUpdateMasterModel model = _context.ProductUpdateMasterModel
                                             .Where(p => p.ProductUpdateMasterId == id)
                                             .FirstOrDefault();
            var Resuilt = "";
            if (model == null)
            {
                Resuilt = "Không tìm thấy đơn hàng yêu cầu !";
            }
            else
            {
                model.Actived = false;
                _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                Resuilt = "success";
            }
            return Json(Resuilt, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Cập nhật sản phẩm
        
        public ActionResult Autoimex(int id)
        {
            ProductUpdateMasterModel model = _context.ProductUpdateMasterModel
                                             .Where(p => p.ProductUpdateMasterId == id && p.Actived == true && p.CreatedIEOther == false)
                                             .FirstOrDefault();
            var Resuilt = "";
            if (model == null)
            {
                Resuilt = "Không tìm thấy phiếu cập nhật yêu cầu !";
            }
            else
            {
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                        {
                            using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                            {
                                cmd.CommandText = "usp_UpdateProduct";
                                cmd.Parameters.AddWithValue("@ProductUpdateMasterId", id);
                                
                                cmd.Connection = conn;
                                cmd.CommandType = CommandType.StoredProcedure;
                                conn.Open();
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                        }
                        ts.Complete();
                        Resuilt = "success";
                    }
                }
                catch
                {
                    Resuilt = "Xảy ra lỗi trong quá trình cập nhật sản phẩm !";
                }
            }
            return Json(Resuilt, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Xoá sản phẩm
        
        public ActionResult DeleteProduct(int id)
        {
            ProductUpdateDetailModel model = _context.ProductUpdateDetailModel
                                             .Where(p => p.ProductUpdateDetailId == id)
                                             .FirstOrDefault();
            var Resuilt = "";
            if (model == null)
            {
                Resuilt = "Không tìm thấy sản phẩm yêu cầu !";
            }
            else
            {
                model.Actived = false;
                _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                var master = _context.ProductUpdateMasterModel.Where(p => p.ProductUpdateMasterId == model.ProductUpdateMasterId).FirstOrDefault();
                master.TotalQty--;
                _context.Entry(master).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                Resuilt = "success";
            }
            return Json(Resuilt, JsonRequestBehavior.AllowGet);
        }
        #endregion



    }
}
