using EntityModels;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
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
    public class ProductUpdates2Controller : BaseController
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
                GetData(model, List);
            }
            catch
            {
                return PartialView(List);
            }
            return PartialView(List);
        }

        private void GetData(ProductSearchViewModel model, List<ProductInfoViewModel> List)
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

        public ProductInfoViewModel ListView(SqlDataReader s)
        {
            ProductInfoViewModel ret = new ProductInfoViewModel();
            try
            {
                if (s["ProductId"] != null)
                {
                    ret.ProductId = (int)s["ProductId"];
                }
                if (s["ProductName"] != null)
                {
                    ret.ProductName = s["ProductName"].ToString();
                }
                if (s["ImportPrice"] != null)
                {
                    ret.ImportPrice = (decimal)s["ImportPrice"];
                }
                if (s["ShippingFee"] != null)
                {
                    ret.ShippingFee = (decimal)s["ShippingFee"];
                }
                if (s["ExchangeRate"] != null)
                {
                    ret.ExchangeRate = (decimal)s["ExchangeRate"];
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
                if (s["ProductStoreCode"] != null)
                {
                    ret.ProductStoreCode = s["ProductStoreCode"].ToString();
                }
                if (s["ProductCode"] != null)
                {
                    ret.ProductCode = s["ProductCode"].ToString();
                }
                if (s["Inventory"] != null)
                {
                    ret.Inventory = (decimal)s["Inventory"];
                }

            }
            catch
            {

            }
            return ret;
        }

        #region Export
        public ActionResult Export(ProductSearchViewModel model)
        {
            List<ProductInfoViewModel> listProduct = new List<ProductInfoViewModel>();
            GetData(model, listProduct);

            //Tên file
            string strName = string.Format("{0}_{1}_{2}", "Danh_sach_san_pham_kiem_kho", DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss"), ".xlsx");
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
            p.Workbook.Properties.Title = "Danh sách sản phẩm";
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
            worksheet.Column(3).Width = 20;
            worksheet.Column(4).Width = 15;
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
                    int? AccountId = currentAccount.EmployeeId;
                    Thread thread = new Thread(() => UpdateProduct(excelfile, AccountId));
                    thread.Start();
                    return Json("success", JsonRequestBehavior.AllowGet);

                }

            }
            catch
            {
                return Json("Lỗi! Vui lòng liên hệ kĩ thuật viên để được giúp đỡ !", JsonRequestBehavior.AllowGet);

            }

        }
        #endregion

        public void UpdateProduct(HttpPostedFileBase excelfile, int? AccountId)
        {

            #region    // Duyệt danh sách insert
            EntityDataContext _context = new EntityDataContext();
            string ErrorEmpty = "";
            string ErrorIsNumber = "";
            string Error = "";
            bool check = true;
            int QuantityProduct = 0;
            try
            {
                using (var package = new ExcelPackage(excelfile.InputStream))
                {
                        using (TransactionScope ts = new TransactionScope())
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                            int col = 1;
                            for (int row = 7; worksheet.Cells[row, col].Value != null; row++)
                            {
                                #region//Gán giá trị tạm
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
                                    check = false;
                                    ErrorEmpty += " Dòng " + row + " Cột 'Mã sản phẩm' ,";
                                }
                                else if (int.TryParse(worksheet.Cells[row, 2].Value.ToString(), out ProductId) == false)
                                {
                                    check = false;
                                    ErrorIsNumber += " Dòng " + row + " Cột 'Mã sản phẩm' ,";
                                }

                                //Mã Sản Phẩm Cửa hàn
                                if (worksheet.Cells[row, 3].Text == "")
                                {
                                    check = false;
                                    ErrorEmpty += " Dòng " + row + " Cột 'Mã Sản Phẩm Cửa hàng' ,";
                                }

                                //Tên Sản Phẩm
                                if (worksheet.Cells[row, 5].Text == "")
                                {
                                    check = false;
                                    ErrorEmpty += " Dòng " + row + " Cột 'Tên Sản Phẩm' ,";
                                }

                                //Giá nhập
                                if (worksheet.Cells[row, 6].Text == "")
                                {
                                    check = false;
                                    ErrorEmpty += " Dòng " + row + " Cột 'Giá nhập' ,";
                                }
                                else if (decimal.TryParse(worksheet.Cells[row, 6].Value.ToString(), out ImportPrice) == false)
                                {
                                    check = false;
                                    ErrorIsNumber += " Dòng " + row + " Cột 'Giá nhập' ,";
                                }

                                //Tỷ giá
                                if (worksheet.Cells[row, 7].Text == "")
                                {
                                    check = false;
                                    ErrorEmpty += " Dòng " + row + " Cột 'Tỷ giá' ,";
                                }
                                else if (decimal.TryParse(worksheet.Cells[row, 7].Value.ToString(), out ExchangeRate) == false)
                                {
                                    check = false;
                                    ErrorIsNumber += " Dòng " + row + " Cột 'Tỷ giá' ,";
                                }

                                //Phí vận chuyển
                                if (worksheet.Cells[row, 8].Text == "")
                                {
                                    check = false;
                                    ErrorEmpty += " Dòng " + row + " Cột 'Phí vận chuyển' ,";
                                }
                                else if (decimal.TryParse(worksheet.Cells[row, 8].Value.ToString(), out ShippingFee) == false)
                                {
                                    check = false;
                                    ErrorIsNumber += " Dòng " + row + " Cột 'Phí vận chuyển' ,";
                                }

                                //Vip
                                if (worksheet.Cells[row, 9].Text == "")
                                {
                                    check = false;
                                    ErrorEmpty += " Dòng " + row + " Cột 'Vip' ,";
                                }
                                else if (decimal.TryParse(worksheet.Cells[row, 9].Value.ToString(), out pprice1) == false)
                                {
                                    check = false;
                                    ErrorIsNumber += " Dòng " + row + " Cột 'Vip' ,";
                                }

                                //Vip-Bạc
                                if (worksheet.Cells[row, 10].Text == "")
                                {
                                    check = false;
                                    ErrorEmpty += " Dòng " + row + " Cột 'Vip-Bạc' ,";
                                }
                                else if (decimal.TryParse(worksheet.Cells[row, 10].Value.ToString(), out pprice2) == false)
                                {
                                    check = false;
                                    ErrorIsNumber += " Dòng " + row + " Cột 'Vip-Bạc' ,";
                                }

                                //Vip-Vàng
                                if (worksheet.Cells[row, 11].Text == "")
                                {
                                    check = false;
                                    ErrorEmpty += " Dòng " + row + " Cột 'Vip-Vàng' ,";
                                }
                                else if (decimal.TryParse(worksheet.Cells[row, 11].Value.ToString(), out pprice3) == false)
                                {
                                    check = false;
                                    ErrorIsNumber += " Dòng " + row + " Cột 'Vip-Vàng' ,";
                                }

                                //Vip-Bạch kim
                                if (worksheet.Cells[row, 12].Text == "")
                                {
                                    check = false;
                                    ErrorEmpty += " Dòng " + row + " Cột 'Vip-Bạch kim' ,";
                                }
                                else if (decimal.TryParse(worksheet.Cells[row, 12].Value.ToString(), out pprice4) == false)
                                {
                                    check = false;
                                    ErrorIsNumber += " Dòng " + row + " Cột 'Vip-Bạch kim' ,";
                                }

                                #endregion

                                if (check)
                                {
                                    #region //Cập nhật Product

                                    ProductModel p = _context.ProductModel.Where(m => m.ProductId == ProductId).FirstOrDefault();
                                    p.ProductStoreCode = worksheet.Cells[row, 3].Value.ToString();
                                    if (worksheet.Cells[row, 4].Text != "")
                                    {
                                        p.ProductCode = worksheet.Cells[row, 4].Value.ToString();
                                    }
                                    p.ProductName = worksheet.Cells[row, 5].Value.ToString();
                                    p.ImportPrice = ImportPrice;
                                    p.ShippingFee = ShippingFee;
                                    p.COGS = p.ImportPrice * ExchangeRate + p.ShippingFee;

                                    #endregion

                                    #region // Cập nhật giá VIP
                                    //Product Price 1
                                    ProductPriceModel price1 = _context.ProductPriceModel.Where(pp => pp.ProductId == ProductId && pp.CustomerLevelId == 1).FirstOrDefault();
                                    price1.Price = pprice1;
                                    //Product Price 2
                                    ProductPriceModel price2 = _context.ProductPriceModel.Where(pp => pp.ProductId == ProductId && pp.CustomerLevelId == 2).FirstOrDefault();
                                    price2.Price = pprice2;

                                    //Product Price 3
                                    ProductPriceModel price3 = _context.ProductPriceModel.Where(pp => pp.ProductId == ProductId && pp.CustomerLevelId == 3).FirstOrDefault();
                                    price3.Price = pprice3;

                                    //Product Price 3
                                    ProductPriceModel price4 = _context.ProductPriceModel.Where(pp => pp.ProductId == ProductId && pp.CustomerLevelId == 4).FirstOrDefault();
                                    price4.Price = pprice4;

                                    #endregion

                                    #region // Update Database
                                    _context.Entry(price1).State = System.Data.Entity.EntityState.Modified;
                                    _context.Entry(price2).State = System.Data.Entity.EntityState.Modified;
                                    _context.Entry(price3).State = System.Data.Entity.EntityState.Modified;
                                    _context.Entry(price4).State = System.Data.Entity.EntityState.Modified;
                                    _context.Entry(p).State = System.Data.Entity.EntityState.Modified;
                                    _context.SaveChanges();
                                    #endregion
                                }
                                QuantityProduct++;
                            }
                            ts.Complete();
                        }
                    }
            }
            catch (EntityException ex)
            {
                Error = "Xảy ra lỗi trong quá trình cập nhật !" +ex;
            }

            #region Note
            NotificationModel n = new NotificationModel();
            if (check == true && Error == "")
            {
                n.Note = "Đã cập nhật thành công giá của " + QuantityProduct + " sản phẩm !";

            }
            else
            {
                n.Note = "Cập nhật không thành công !";
                if (Error != "")
                {
                    n.Note += Error;
                }
                if (ErrorEmpty != "")
                {
                    n.Note += "  Bị rỗng :" + ErrorEmpty;
                }
                if (ErrorIsNumber != "")
                {
                    n.Note += "  Không là số :" + ErrorEmpty;
                }
            }
            n.AccountId = AccountId;
            n.CreateDate = DateTime.Now;
            n.Actived = true;
            _context.Entry(n).State = System.Data.Entity.EntityState.Added;
            _context.SaveChanges();
            #endregion

            _context.Dispose();
            #endregion
        }
    }
}

