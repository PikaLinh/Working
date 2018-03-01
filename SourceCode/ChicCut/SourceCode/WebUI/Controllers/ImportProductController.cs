using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using EntityModels;
using System.IO;
using System.Globalization;
using Repository;
using System.Threading;
using ViewModels;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Constant;
using System.Transactions;


namespace WebUI.Controllers
{
    public class ImportProductController : BaseController
    {
        //
        // GET: /ImportProduct/
        private void CreateViewBag(int? StoreId = null)
        {
            // StoreId
            var StoreList = _context.StoreModel.Where(p => p.Actived == true).OrderBy(p => p.StoreName).ToList();
            ViewBag.StoreId = new SelectList(StoreList, "StoreId", "StoreName", StoreId);

        }
        
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        
        public ActionResult Template()
        {

            //Bước 1: lấy path
            string path = Server.MapPath("~/Upload/TemplateProduct/Product.xlsx");
            //Bước 2: Copy vào MemoryStream
            MemoryStream memoryStream = new MemoryStream();
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            fileStream.CopyTo(memoryStream);
            fileStream.Close();
            //Bước 3: Trả về cho người dùng
            return File(memoryStream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Octet, "Product.xlsx");

        }
        #region Export
        
        public ActionResult Export()
        {
            //Tên file
            string strName = string.Format("{0}_{1}_{2}", "Danh_sach_san_pham", DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss"), ".xlsx");
            Byte[] report = GenerateReport();
            return File(report, "application/vnd.ms-excel", strName);

        }
        private Byte[] GenerateReport()
        {
            using (ExcelPackage p = new ExcelPackage())
            {
                //set the workbook properties and add a default sheet in it
                SetWorkbookProperties(p);
                //Create a sheet
                ExcelWorksheet ws = CreateSheet(p, "Danh sách sản phẩm");
                //Merging cells and create a center heading for out table
                ws.Cells[2, 1].Value = "Danh sách sản phẩm";
                ws.Cells[2, 1, 2, 23].Merge = true;
                ws.Cells[2, 1, 2, 23].Style.Font.Bold = true;
                ws.Cells[2, 1, 2, 23].Style.Font.Size = 22;
                ws.Cells[2, 1, 2, 23].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                int rowIndex = 4;
                int row = 5;
                List<ProductModel> listProductTypeModel = _context.ProductModel.Take(10).ToList();
                
                CreateHeader(ws, ref rowIndex);
                CreateData(ws, ref row, listProductTypeModel);
                Byte[] bin = p.GetAsByteArray();

                return bin;

                // return File(bin, "application/vnd.ms-excel", log.FileName);
            }
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

        private static void SetWorkbookProperties(ExcelPackage p)
        {
            //Here setting some document properties
            p.Workbook.Properties.Author = "Zeeshan Umar";
            p.Workbook.Properties.Title = "Danh sách sản phẩm";


        }

        private static void CreateHeader(ExcelWorksheet worksheet, ref int rowIndex)
        {
            #region Định dạng Excel
            //độ rộng cột
            worksheet.Column(1).Width = 5;
            worksheet.Column(2).Width = 15;
            worksheet.Column(3).Width = 15;
            worksheet.Column(4).Width = 50;
            worksheet.Column(5).Width = 20;
            worksheet.Column(6).Width = 15;
            worksheet.Column(7).Width = 20;
            worksheet.Column(8).Width = 22;
            worksheet.Column(9).Width = 22;
            worksheet.Column(10).Width = 10;
            worksheet.Column(11).Width = 15;
            worksheet.Column(12).Width = 15;
            worksheet.Column(13).Width = 15;
            worksheet.Column(14).Width = 15;
            worksheet.Column(15).Width = 15;
            worksheet.Column(16).Width = 15;
            worksheet.Column(17).Width = 17;
            worksheet.Column(18).Width = 15;
            worksheet.Column(19).Width = 15;
            worksheet.Column(20).Width = 15;
            worksheet.Column(21).Width = 17;
            worksheet.Column(22).Width = 15;
            worksheet.Column(23).Width = 15;

            //format date cột i

            //tiêu đề cột
            worksheet.Cells[rowIndex, 1].Value = "STT";
            worksheet.Cells[rowIndex, 2].Value = "ID";
            worksheet.Cells[rowIndex, 3].Value = "Mã sản phẩm";
            worksheet.Cells[rowIndex, 4].Value = "Tên SP";
            worksheet.Cells[rowIndex, 5].Value = "Loại sản phẩm";
            worksheet.Cells[rowIndex, 6].Value = "Danh mục";
            worksheet.Cells[rowIndex, 7].Value = "Xuất xứ hàng hoá";
            worksheet.Cells[rowIndex, 8].Value = "Quy định khi còn hàng";
            worksheet.Cells[rowIndex, 9].Value = "Quy định khi hết hàng";
            worksheet.Cells[rowIndex, 10].Value = "Vị trí";
            worksheet.Cells[rowIndex, 11].Value = "Trạng thái";
            worksheet.Cells[rowIndex, 12].Value = "Trọng lượng";
            worksheet.Cells[rowIndex, 13].Value = "Đơn vị tính";
            worksheet.Cells[rowIndex, 14].Value = "Giá nhập";
            worksheet.Cells[rowIndex, 15].Value = "Tiền tệ";
            worksheet.Cells[rowIndex, 16].Value = "Tỷ giá";
            worksheet.Cells[rowIndex, 17].Value = "Phí vận chuyển";
            worksheet.Cells[rowIndex, 18].Value = "Giá Vốn";
            worksheet.Cells[rowIndex, 19].Value = "Giá VIP-Vàng";
            worksheet.Cells[rowIndex, 20].Value = "Giá VIP-Bạc ";
            worksheet.Cells[rowIndex, 21].Value = "Giá VIP-Bạch Kim";
            worksheet.Cells[rowIndex, 22].Value = "Tồn đầu kỳ";
            worksheet.Cells[rowIndex, 23].Value = "Còn sử dụng";

            for (int i = 1; i <= 23; i++)
            {
                var cell = worksheet.Cells[rowIndex, i];
                var fill = cell.Style.Fill;
                fill.PatternType = ExcelFillStyle.Solid;
                fill.BackgroundColor.SetColor(Color.Gray);

                //Setting Top/left,right/bottom borders.
                var border = cell.Style.Border;
                border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
            }

            #endregion
        }
        private static void CreateData(ExcelWorksheet worksheet, ref int rowIndex, List<ProductModel> listProductTypeModel)
        {
            EntityDataContext _context = new EntityDataContext();
            foreach (ProductModel p in listProductTypeModel)
            {
                worksheet.Cells[rowIndex, 1].Value = rowIndex - 4;
                worksheet.Cells[rowIndex, 2].Value = p.ProductId;
                worksheet.Cells[rowIndex, 3].Value = p.ProductCode;
                worksheet.Cells[rowIndex, 4].Value = p.ProductName;
                worksheet.Cells[rowIndex, 5].Value = p.ProductTypeId;
                worksheet.Cells[rowIndex, 6].Value = p.CategoryId;
                worksheet.Cells[rowIndex, 7].Value = p.OriginOfProductId;
                worksheet.Cells[rowIndex, 8].Value = p.PolicyInStockId;
                worksheet.Cells[rowIndex, 9].Value = p.PolicyOutOfStockId;
                worksheet.Cells[rowIndex, 10].Value = p.LocationOfProductId;
                worksheet.Cells[rowIndex, 11].Value = p.ProductStatusId;
                worksheet.Cells[rowIndex, 12].Value = p.ShippingWeight;
                worksheet.Cells[rowIndex, 13].Value = p.UnitId;
                worksheet.Cells[rowIndex, 14].Value = p.ImportPrice;
                worksheet.Cells[rowIndex, 15].Value = p.CurrencyId;
                worksheet.Cells[rowIndex, 16].Value = p.ExchangeRate;
                worksheet.Cells[rowIndex, 17].Value = p.ShippingFee;
                worksheet.Cells[rowIndex, 18].Value = p.COGS;
                worksheet.Cells[rowIndex, 19].Value = _context.ProductPriceModel.Where(pp => pp.ProductId == p.ProductId && pp.CustomerLevelId == 1).Select(c => c.Price).FirstOrDefault();
                worksheet.Cells[rowIndex, 20].Value = _context.ProductPriceModel.Where(pp => pp.ProductId == p.ProductId && pp.CustomerLevelId == 2).Select(c => c.Price).FirstOrDefault();
                worksheet.Cells[rowIndex, 21].Value = _context.ProductPriceModel.Where(pp => pp.ProductId == p.ProductId && pp.CustomerLevelId == 3).Select(c => c.Price).FirstOrDefault();
                worksheet.Cells[rowIndex, 22].Value = p.BeginInventoryQty;
                worksheet.Cells[rowIndex, 23].Value = p.Actived;
                rowIndex++;
            }
        }
        #endregion

        //public ActionResult Export()
        //{
        //    Excel.Application application = new Excel.Application();
        //    Excel.Workbook workbook = application.Workbooks.Add(System.Reflection.Missing.Value);
        //    Excel.Worksheet worksheet = workbook.ActiveSheet;
        //    string strName = "";
        //    MemoryStream memoryStream = new MemoryStream();
        //    try
        //    {

        //        //màu tiêu đề
        //        Excel.Range formatRange;
        //        formatRange = worksheet.get_Range("a1", "k1");
        //        formatRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Silver);
        //        //độ rộng cột
        //        //formatRange.EntireColumn.ColumnWidth = 15;
        //        worksheet.Columns[2].ColumnWidth = 50;
        //        worksheet.Columns[6].ColumnWidth = 15;
        //        worksheet.Columns[8].ColumnWidth = 15;
        //        worksheet.Columns[9].ColumnWidth = 15;
        //        worksheet.Columns[10].ColumnWidth = 16;
        //        worksheet.Columns[11].ColumnWidth = 15;

        //        worksheet.Cells[1, 1] = "ID";
        //        worksheet.Cells[1, 2] = "Tên SP";
        //        worksheet.Cells[1, 3] = "Loại sản phẩm";
        //        worksheet.Cells[1, 4] = "Giá nhập";
        //        worksheet.Cells[1, 5] = "Tiền tệ";
        //        worksheet.Cells[1, 6] = "Tỷ giá";
        //        worksheet.Cells[1, 7] = "Phí vận chuyển";
        //        worksheet.Cells[1, 8] = "Giá Vốn";
        //        worksheet.Cells[1, 9] = "Giá VIP-Vàng";
        //        worksheet.Cells[1, 10] = "Giá VIP-Bạc ";
        //        worksheet.Cells[1, 11] = "Giá VIP-Bạch Kim ";
        //        worksheet.Cells[1, 12] = "Còn sử dụng";
        //        int row = 2;
        //        List<ProductModel> listProductModel = _context.ProductModel.ToList();
        //        foreach (ProductModel p in listProductModel)
        //        {
        //            worksheet.Cells[row, 1] = p.ProductId;
        //            worksheet.Cells[row, 2] = p.ProductName;
        //            worksheet.Cells[row, 3] = p.ProductTypeId;
        //            worksheet.Cells[row, 4] = p.ImportPrice;
        //            worksheet.Cells[row, 5] = p.CurrencyId;
        //            worksheet.Cells[row, 6] = p.ExchangeRate;
        //            worksheet.Cells[row, 7] = p.ShippingFee;
        //            worksheet.Cells[row, 8] = p.COGS;
        //            worksheet.Cells[row, 9] = _context.ProductPriceModel.Where(pp=>pp.ProductId== p.ProductId && pp.CustomerLevelId==1).Select(c=>c.Price).FirstOrDefault();
        //            worksheet.Cells[row, 10] = _context.ProductPriceModel.Where(pp => pp.ProductId == p.ProductId && pp.CustomerLevelId == 2).Select(c => c.Price).FirstOrDefault();
        //            worksheet.Cells[row, 11] = _context.ProductPriceModel.Where(pp => pp.ProductId == p.ProductId && pp.CustomerLevelId == 3).Select(c => c.Price).FirstOrDefault();
        //            worksheet.Cells[row, 12] = p.Actived;
        //            row++;
        //        }
        //        strName = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss") + "-product.xlsx";
        //        //Bước 1: Lưu file trên server
        //        string path = Server.MapPath("~/Upload/TempProduct/" + strName);
        //        if (System.IO.File.Exists(path))
        //        {
        //            System.IO.File.Delete(path);
        //        }
        //        workbook.SaveAs(path);
        //        workbook.Close();
        //        //Bước 2: Copy vào MemoryStream
        //        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        //        fileStream.CopyTo(memoryStream);
        //        fileStream.Close();
        //        //Bước 3: Xóa file trên server path
        //        System.IO.File.Delete(path);
        //        //Bước 4: Trả về cho người dùng
        //        return File(memoryStream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Octet, strName);
        //    }
        //    catch (Exception ex)
        //    {
        //        //Trả về thông báo lỗi - Nên trả về View Error
        //        return Content("Error - " + ex.Message);
        //    }
        //}
        //public void ChildImport(object excelfile)
        //{
        //    Thread.Sleep(1000);
        //    HttpPostedFileBase Excelfile = (HttpPostedFileBase)excelfile;
        //    //int Store = (int)model.StoreId;

        //    string strName = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss") + Excelfile.FileName;
        //    string path = Server.MapPath("~/Upload/TempProduct/" + strName);
        //    //kiểm tra nếu tồn tại file thì xoá
        //    if (System.IO.File.Exists(path))
        //    {
        //        System.IO.File.Delete(path);
        //    }
        //    //Lưu file
        //    Excelfile.SaveAs(path);
        //    Excel.Application application = new Excel.Application();
        //    Excel.Workbook workbook = application.Workbooks.Open(path);
        //    Excel.Worksheet worksheet = workbook.ActiveSheet;
        //    Excel.Range range = worksheet.UsedRange;
        //    //Tiến hành đọc data từ file excel

        //    //Dictionary<int, string> listProductTitle = new Dictionary<int, string>();
        //    //listProductTitle.Add(2, "Tên SP");
        //    //listProductTitle.Add(3, "Loại sản phẩm");
        //    //listProductTitle.Add(12, "Giá nhập");
        //    //listProductTitle.Add(13, "Tiền tệ");
        //    //listProductTitle.Add(14, "Tỷ giá");
        //    //listProductTitle.Add(15, "Phí vận chuyển");
        //    //listProductTitle.Add(16, "Giá Vốn");
        //    //listProductTitle.Add(17, "Giá VIP-Vàng");
        //    //listProductTitle.Add(18, "Giá VIP-Bạc");
        //    //listProductTitle.Add(19, "Giá VIP-Bạch Kim");
        //    //listProductTitle.Add(20, "Còn sử dụng");
        //    for (int row = 2; row <= range.Rows.Count; row++)
        //    {

        //        if (string.IsNullOrEmpty(((Excel.Range)range.Cells[row, 2]).Text) ||
        //            string.IsNullOrEmpty(((Excel.Range)range.Cells[row, 3]).Text) ||
        //            string.IsNullOrEmpty(((Excel.Range)range.Cells[row, 12]).Text) ||
        //            string.IsNullOrEmpty(((Excel.Range)range.Cells[row, 13]).Text) ||
        //            string.IsNullOrEmpty(((Excel.Range)range.Cells[row, 14]).Text) ||
        //            string.IsNullOrEmpty(((Excel.Range)range.Cells[row, 15]).Text) ||
        //            string.IsNullOrEmpty(((Excel.Range)range.Cells[row, 16]).Text) ||
        //            string.IsNullOrEmpty(((Excel.Range)range.Cells[row, 17]).Text) ||
        //            string.IsNullOrEmpty(((Excel.Range)range.Cells[row, 18]).Text) ||
        //            string.IsNullOrEmpty(((Excel.Range)range.Cells[row, 19]).Text) ||
        //            string.IsNullOrEmpty(((Excel.Range)range.Cells[row, 20]).Text))
        //        {
        //            ViewBag.Import = "Vui lòng nhập các thông tin bắt buộc tại dòng " + row + "!";
        //            break;
        //        }

        //        ////kiểm tra dữ liệu trống
        //        //foreach (var item in listProductTitle)
        //        //{
        //        //    if (((Excel.Range)range.Cells[row, item.Key]).Text == "")
        //        //    {
        //        //        ViewBag.Import = "Dòng " + row + " " + item.Value + " trống !";
        //        //        CreateViewBag();
        //        //        return View("Index");
        //        //    }
        //        //}
        //        ProductModel p;
        //        string ProductName = (((Excel.Range)range.Cells[row, 2]).Text);
        //        int ProductTypeId = int.Parse(((Excel.Range)range.Cells[row, 3]).Text);
        //        decimal ImportPrice = Decimal.Parse(((Excel.Range)range.Cells[row, 12]).Text);
        //        int CurrencyId = int.Parse(((Excel.Range)range.Cells[row, 13]).Text); ;
        //        decimal ExchangeRate = Decimal.Parse(((Excel.Range)range.Cells[row, 14]).Text);
        //        decimal ShippingFee = Decimal.Parse(((Excel.Range)range.Cells[row, 15]).Text);
        //        decimal COGS = Decimal.Parse(((Excel.Range)range.Cells[row, 16]).Text);
        //        decimal pprice1 = Decimal.Parse(((Excel.Range)range.Cells[row, 17]).Text);
        //        decimal pprice2 = Decimal.Parse(((Excel.Range)range.Cells[row, 18]).Text);
        //        decimal pprice3 = Decimal.Parse(((Excel.Range)range.Cells[row, 19]).Text);
        //        bool Actived = bool.Parse(((Excel.Range)range.Cells[row, 20]).Text);
        //        //Cập nhật
        //        if ((((Excel.Range)range.Cells[row, 1]).Text) != "")
        //        {
        //            int ProductId = int.Parse(((Excel.Range)range.Cells[row, 1]).Text);
        //            p = _context.ProductModel.Where(pp => pp.ProductId == ProductId).FirstOrDefault();
        //            p.ProductCode = "";
        //            p.ProductName = ProductName;
        //            p.ProductTypeId = ProductTypeId;
        //            if ((((Excel.Range)range.Cells[row, 4]).Text) != "")
        //            {
        //                p.CategoryId = int.Parse(((Excel.Range)range.Cells[row, 4]).Text);
        //            };
        //            p.SEOProductName = Library.ConvertToNoMarkString(p.ProductName);
        //            p.CreatedDate = DateTime.Now;
        //            p.CreatedAccount = currentAccount.UserName;
        //            if ((((Excel.Range)range.Cells[row, 5]).Text) != "")
        //            {
        //                p.OriginOfProductId = int.Parse(((Excel.Range)range.Cells[row, 5]).Text);
        //            };
        //            if ((((Excel.Range)range.Cells[row, 6]).Text) != "")
        //            {
        //                p.PolicyInStockId = int.Parse(((Excel.Range)range.Cells[row, 6]).Text);
        //            };
        //            if ((((Excel.Range)range.Cells[row, 7]).Text) != "")
        //            {
        //                p.PolicyOutOfStockId = int.Parse(((Excel.Range)range.Cells[row, 7]).Text);
        //            };
        //            if ((((Excel.Range)range.Cells[row, 8]).Text) != "")
        //            {
        //                p.LocationOfProductId = int.Parse(((Excel.Range)range.Cells[row, 8]).Text);
        //            };
        //            if ((((Excel.Range)range.Cells[row, 9]).Text) != "")
        //            {
        //                p.ProductStatusId = int.Parse(((Excel.Range)range.Cells[row, 9]).Text);
        //            };
        //            if ((((Excel.Range)range.Cells[row, 10]).Text) != "")
        //            {
        //                p.ShippingWeight = int.Parse(((Excel.Range)range.Cells[row, 10]).Text);
        //            };
        //            if ((((Excel.Range)range.Cells[row, 11]).Text) != "")
        //            {
        //                p.UnitId = int.Parse(((Excel.Range)range.Cells[row, 11]).Text);
        //            };
        //            p.ImportPrice = ImportPrice;
        //            p.CurrencyId = CurrencyId;
        //            p.ExchangeRate = ExchangeRate;
        //            p.ShippingFee = ShippingFee;
        //            p.COGS = COGS;
        //            //p.StoreId = Store;
        //            ProductRepository ProductRepository = new ProductRepository(_context);
        //            p.ProductStoreCode = ProductRepository.GetProdcutStoreCode(p.StoreId.Value, p.ProductTypeId.Value);
        //            //Product Price 1
        //            ProductPriceModel price1 = _context.ProductPriceModel.Where(pp => pp.ProductId == ProductId && pp.CustomerLevelId == 1).FirstOrDefault();
        //            price1.Price = pprice1;
        //            //Product Price 2
        //            ProductPriceModel price2 = _context.ProductPriceModel.Where(pp => pp.ProductId == ProductId && pp.CustomerLevelId == 2).FirstOrDefault();
        //            price2.Price = pprice2;

        //            //Product Price 3
        //            ProductPriceModel price3 = _context.ProductPriceModel.Where(pp => pp.ProductId == ProductId && pp.CustomerLevelId == 3).FirstOrDefault();
        //            price3.Price = pprice3;
        //            //Kiểm tra Field Kích hoạt
        //            p.Actived = Actived;

        //            _context.Entry(price1).State = System.Data.Entity.EntityState.Modified;
        //            _context.Entry(price2).State = System.Data.Entity.EntityState.Modified;
        //            _context.Entry(price3).State = System.Data.Entity.EntityState.Modified;
        //            _context.Entry(p).State = System.Data.Entity.EntityState.Modified;
        //            _context.SaveChanges();
        //        }
        //        else //Thêm mới
        //        {
        //            p = new ProductModel();
        //            p.ProductName = ProductName;
        //            p.ProductTypeId = ProductTypeId;
        //            if ((((Excel.Range)range.Cells[row, 4]).Text) != "")
        //            {
        //                p.CategoryId = int.Parse(((Excel.Range)range.Cells[row, 4]).Text);
        //            };
        //            p.SEOProductName = Library.ConvertToNoMarkString(p.ProductName);
        //            p.CreatedDate = DateTime.Now;
        //            p.CreatedAccount = currentAccount.UserName;
        //            if ((((Excel.Range)range.Cells[row, 5]).Text) != "")
        //            {
        //                p.OriginOfProductId = int.Parse(((Excel.Range)range.Cells[row, 5]).Text);
        //            };
        //            if ((((Excel.Range)range.Cells[row, 6]).Text) != "")
        //            {
        //                p.PolicyInStockId = int.Parse(((Excel.Range)range.Cells[row, 6]).Text);
        //            };
        //            if ((((Excel.Range)range.Cells[row, 7]).Text) != "")
        //            {
        //                p.PolicyOutOfStockId = int.Parse(((Excel.Range)range.Cells[row, 7]).Text);
        //            };
        //            if ((((Excel.Range)range.Cells[row, 8]).Text) != "")
        //            {
        //                p.LocationOfProductId = int.Parse(((Excel.Range)range.Cells[row, 8]).Text);
        //            };
        //            if ((((Excel.Range)range.Cells[row, 9]).Text) != "")
        //            {
        //                p.ProductStatusId = int.Parse(((Excel.Range)range.Cells[row, 9]).Text);
        //            };
        //            if ((((Excel.Range)range.Cells[row, 10]).Text) != "")
        //            {
        //                p.ShippingWeight = int.Parse(((Excel.Range)range.Cells[row, 10]).Text);
        //            };
        //            if ((((Excel.Range)range.Cells[row, 11]).Text) != "")
        //            {
        //                p.UnitId = int.Parse(((Excel.Range)range.Cells[row, 11]).Text);
        //            };
        //            p.ImportPrice = ImportPrice;
        //            p.CurrencyId = CurrencyId;
        //            p.ExchangeRate = ExchangeRate;
        //            p.ShippingFee = ShippingFee;
        //            p.COGS = COGS;
        //            //p.StoreId = Store;
        //            ProductRepository ProductRepository = new ProductRepository(_context);
        //            p.ProductStoreCode = ProductRepository.GetProdcutStoreCode(p.StoreId.Value, p.ProductTypeId.Value);
        //            //Product Price 1
        //            ProductPriceModel price1 = new ProductPriceModel()
        //            {
        //                Price = pprice1,
        //                CustomerLevelId = 1,
        //                ProductId = p.ProductId
        //            };
        //            // Product Price 2
        //            ProductPriceModel price2 = new ProductPriceModel()
        //            {
        //                Price = pprice2,
        //                CustomerLevelId = 2,
        //                ProductId = p.ProductId
        //            };
        //            // Product Price 3
        //            ProductPriceModel price3 = new ProductPriceModel()
        //            {
        //                Price = pprice3,
        //                CustomerLevelId = 3,
        //                ProductId = p.ProductId
        //            };
        //            //Kiểm tra Field Kích hoạt
        //            p.Actived = Actived;

        //            _context.Entry(price1).State = System.Data.Entity.EntityState.Added;
        //            _context.Entry(price2).State = System.Data.Entity.EntityState.Added;
        //            _context.Entry(price3).State = System.Data.Entity.EntityState.Added;
        //            _context.Entry(p).State = System.Data.Entity.EntityState.Added;
        //            _context.SaveChanges();
        //        }
        //    }

        //}

        //[HttpPost]
        //public ActionResult Import(HttpPostedFileBase excelfile)
        //{
        //    try
        //    {
        //        if (excelfile == null ||excelfile.ContentLength == 0)
        //        {
        //            ViewBag.Import = "Bạn vui lòng chọn 1 file excel";
        //            CreateViewBag();
        //            return View("Index");
        //        }
        //        else
        //        {
        //            if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
        //            {

        //                 //lấy đường dẫn file

        //                Thread Child = new Thread(new ParameterizedThreadStart(ChildImport));
        //                Child.Start(excelfile);                     
        //                ViewBag.Import = "Import thành công !";
        //            }
        //            else
        //            {
        //                ViewBag.Import = "Kiểu file không hợp lệ";
        //            }
        //        }
        //        CreateViewBag();
        //        return View("Index");
        //    }
        //    catch(Exception ex)
        //    {
        //        ViewBag.Import = "Lỗi! Vui lòng liên hệ kĩ thuật viên để được giúp đỡ !";
        //        CreateViewBag();
        //        return View("Index");

        //    }
        //    }

        #region Import
        [HttpPost]
        public ActionResult Import(HttpPostedFileBase excelfile, int? StoreId)
        {
            try
            {
                if (excelfile == null || excelfile.ContentLength == 0)
                {
                    ViewBag.Import = "Bạn vui lòng chọn 1 file excel";
                    CreateViewBag();
                    return View("Index");
                }
                else
                {

                    using (var package = new ExcelPackage(excelfile.InputStream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                        var rowCnt = worksheet.Dimension.End.Row - 4;
                        int col = 1;
                        for (int row = 5; worksheet.Cells[row, col].Value != null; row++)
                        {
                            #region kiểm tra giá trị null
                            if (string.IsNullOrEmpty(worksheet.Cells[row, 3].Text) ||
                                 string.IsNullOrEmpty(worksheet.Cells[row, 4].Text) ||
                                 string.IsNullOrEmpty(worksheet.Cells[row, 5].Text) ||
                                 string.IsNullOrEmpty(worksheet.Cells[row, 14].Text) ||
                                 string.IsNullOrEmpty(worksheet.Cells[row, 15].Text) ||
                                 string.IsNullOrEmpty(worksheet.Cells[row, 16].Text) ||
                                 string.IsNullOrEmpty(worksheet.Cells[row, 17].Text) ||
                                 string.IsNullOrEmpty(worksheet.Cells[row, 18].Text) ||
                                 string.IsNullOrEmpty(worksheet.Cells[row, 19].Text) ||
                                 string.IsNullOrEmpty(worksheet.Cells[row, 20].Text) ||
                                 string.IsNullOrEmpty(worksheet.Cells[row, 21].Text) ||
                                 string.IsNullOrEmpty(worksheet.Cells[row, 22].Text) ||
                                 string.IsNullOrEmpty(worksheet.Cells[row, 23].Text))
                            {
                                ViewBag.Import = "vui lòng nhập các thông tin bắt buộc tại dòng " + row + "!";
                                CreateViewBag();
                                return View("Index");
                            }
                            #endregion
                            #region gán giatri
                            string ProductCode = worksheet.Cells[row, 3].Value.ToString();
                            string ProductName = worksheet.Cells[row, 4].Value.ToString();
                            int ProductTypeId = int.Parse(worksheet.Cells[row, 5].Value.ToString());
                            decimal ImportPrice = Decimal.Parse(worksheet.Cells[row, 14].Value.ToString());
                            int CurrencyId = int.Parse(worksheet.Cells[row, 15].Value.ToString()); ;
                            decimal ExchangeRate = Decimal.Parse(worksheet.Cells[row, 16].Value.ToString());
                            decimal ShippingFee = Decimal.Parse(worksheet.Cells[row, 17].Value.ToString());
                            decimal COGS = Decimal.Parse(worksheet.Cells[row, 18].Value.ToString());
                            decimal pprice1 = Decimal.Parse(worksheet.Cells[row, 19].Value.ToString());
                            decimal pprice2 = Decimal.Parse(worksheet.Cells[row, 20].Value.ToString());
                            decimal pprice3 = Decimal.Parse(worksheet.Cells[row, 21].Value.ToString());
                            decimal BeginInventoryQty = Decimal.Parse(worksheet.Cells[row, 22].Value.ToString());
                            bool Actived = bool.Parse(worksheet.Cells[row, 23].Value.ToString());
                            #endregion
                            #region cập nhật
                            if (worksheet.Cells[row, 2].Text != "")
                            {
                                ProductModel p = new ProductModel();
                                int ProductId = Int32.Parse(worksheet.Cells[row, 2].Value.ToString());
                                p = _context.ProductModel.Where(pp => pp.ProductId == ProductId).FirstOrDefault();
                                if (p != null)
                                {
                                    p.ProductCode = ProductCode;
                                    p.ProductName = ProductName;
                                    p.ProductTypeId = ProductTypeId;
                                    if (worksheet.Cells[row, 6].Text != "")
                                    {
                                        p.CategoryId = int.Parse(worksheet.Cells[row, 6].Value.ToString());
                                    };
                                    p.SEOProductName = Library.ConvertToNoMarkString(p.ProductName);
                                    p.CreatedDate = DateTime.Now;
                                    p.CreatedAccount = currentAccount.UserName;
                                    if (worksheet.Cells[row, 7].Text != "")
                                    {
                                        p.OriginOfProductId = int.Parse(worksheet.Cells[row, 7].Value.ToString());
                                    };
                                    if (worksheet.Cells[row, 8].Text != "")
                                    {
                                        p.PolicyInStockId = int.Parse(worksheet.Cells[row, 8].Value.ToString());
                                    };
                                    if (worksheet.Cells[row, 9].Text != "")
                                    {
                                        p.PolicyOutOfStockId = int.Parse(worksheet.Cells[row, 9].Value.ToString());
                                    };
                                    if (worksheet.Cells[row, 10].Text != "")
                                    {
                                        p.LocationOfProductId = int.Parse(worksheet.Cells[row, 10].Value.ToString());
                                    };
                                    if (worksheet.Cells[row, 11].Text != "")
                                    {
                                        p.ProductStatusId = int.Parse(worksheet.Cells[row, 11].Value.ToString());
                                    };
                                    if (worksheet.Cells[row, 12].Text != "")
                                    {
                                        p.ShippingWeight = int.Parse(worksheet.Cells[row, 12].Value.ToString());
                                    };
                                    if (worksheet.Cells[row, 13].Text != "")
                                    {
                                        p.UnitId = int.Parse(worksheet.Cells[row, 13].Value.ToString());
                                    };
                                    p.ImportPrice = ImportPrice;
                                    p.CurrencyId = CurrencyId;
                                    p.ExchangeRate = ExchangeRate;
                                    p.ShippingFee = ShippingFee;
                                    p.COGS = COGS;
                                    //p.StoreId = Store;
                                    ProductRepository ProductRepository = new ProductRepository(_context);
                                    p.ProductStoreCode = ProductRepository.GetProdcutStoreCode(StoreId.Value, p.ProductTypeId.Value, p.CategoryId.Value);
                                    //Product Price 1
                                    ProductPriceModel price1 = _context.ProductPriceModel.Where(pp => pp.ProductId == ProductId && pp.CustomerLevelId == 1).FirstOrDefault();
                                    price1.Price = pprice1;
                                    //Product Price 2
                                    ProductPriceModel price2 = _context.ProductPriceModel.Where(pp => pp.ProductId == ProductId && pp.CustomerLevelId == 2).FirstOrDefault();
                                    price2.Price = pprice2;

                                    //Product Price 3
                                    ProductPriceModel price3 = _context.ProductPriceModel.Where(pp => pp.ProductId == ProductId && pp.CustomerLevelId == 3).FirstOrDefault();
                                    price3.Price = pprice3;
                                    //Kiểm tra Field Kích hoạt
                                    p.Actived = Actived;
                                    p.BeginInventoryQty = BeginInventoryQty;

                                    _context.Entry(price1).State = System.Data.Entity.EntityState.Modified;
                                    _context.Entry(price2).State = System.Data.Entity.EntityState.Modified;
                                    _context.Entry(price3).State = System.Data.Entity.EntityState.Modified;
                                    _context.Entry(p).State = System.Data.Entity.EntityState.Modified;
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    ViewBag.Import = "Không tìm thấy sản phẩm dòng " + row + " !";
                                    CreateViewBag();
                                    return View("Index");
                                }

                            }
                            #endregion
                            #region Thêm mới
                            else //Thêm mới
                            {
                                ProductModel p = new ProductModel();
                                p.ProductCode = ProductCode;
                                p.ProductName = ProductName;
                                p.ProductTypeId = ProductTypeId;
                                if (worksheet.Cells[row, 6].Text != "")
                                {
                                    p.CategoryId = int.Parse(worksheet.Cells[row, 5].Value.ToString());
                                };
                                p.SEOProductName = Library.ConvertToNoMarkString(p.ProductName);
                                p.CreatedDate = DateTime.Now;
                                p.CreatedAccount = currentAccount.UserName;
                                if (worksheet.Cells[row, 7].Text != "")
                                {
                                    p.OriginOfProductId = int.Parse(worksheet.Cells[row, 7].Value.ToString());
                                };
                                if (worksheet.Cells[row, 8].Text != "")
                                {
                                    p.PolicyInStockId = int.Parse(worksheet.Cells[row, 8].Value.ToString());
                                };
                                if (worksheet.Cells[row, 9].Text != "")
                                {
                                    p.PolicyOutOfStockId = int.Parse(worksheet.Cells[row, 9].Value.ToString());
                                };
                                if (worksheet.Cells[row, 10].Text != "")
                                {
                                    p.LocationOfProductId = int.Parse(worksheet.Cells[row, 10].Value.ToString());
                                };
                                if (worksheet.Cells[row, 11].Text != "")
                                {
                                    p.ProductStatusId = int.Parse(worksheet.Cells[row, 11].Value.ToString());
                                };
                                if (worksheet.Cells[row, 12].Text != "")
                                {
                                    p.ShippingWeight = int.Parse(worksheet.Cells[row, 12].Value.ToString());
                                };
                                if (worksheet.Cells[row, 13].Text != "")
                                {
                                    p.UnitId = int.Parse(worksheet.Cells[row, 13].Value.ToString());
                                };
                                p.ImportPrice = ImportPrice;
                                p.CurrencyId = CurrencyId;
                                p.ExchangeRate = ExchangeRate;
                                p.ShippingFee = ShippingFee;
                                p.COGS = COGS;
                                //p.StoreId = Store;
                                ProductRepository ProductRepository = new ProductRepository(_context);
                                p.ProductStoreCode = ProductRepository.GetProdcutStoreCode(StoreId.Value, p.ProductTypeId.Value, p.CategoryId.Value);
                                p.Actived = Actived;
                                p.CreatedAccount = currentAccount.UserName;
                                p.BeginInventoryQty = BeginInventoryQty;
                                AccountModel Account = _context.AccountModel.Where(pp => pp.UserName == p.CreatedAccount).FirstOrDefault();
                                IEOtherRepository IEOtherRepository = new IEOtherRepository(_context);
                                _context.Entry(p).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();

                                //Product Price 1
                                ProductPriceModel price1 = new ProductPriceModel()
                                {
                                    Price = pprice1,
                                    CustomerLevelId = 1,
                                    ProductId = p.ProductId
                                };
                                _context.Entry(price1).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();

                                // Product Price 2
                                ProductPriceModel price2 = new ProductPriceModel()
                                {
                                    Price = pprice2,
                                    CustomerLevelId = 2,
                                    ProductId = p.ProductId
                                };
                                _context.Entry(price2).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();

                                // Product Price 3
                                ProductPriceModel price3 = new ProductPriceModel()
                                {
                                    Price = pprice3,
                                    CustomerLevelId = 3,
                                    ProductId = p.ProductId
                                };

                                _context.Entry(price3).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();

                                //Kiểm tra Field Kích hoạt
                                if (p.BeginInventoryQty != 0)
                                {
                                    // Lưu vào IEOtherMasterModel
                                    IEOtherMasterModel IEMaster = new IEOtherMasterModel()
                                    {
                                        IEOtherMasterCode = IEOtherRepository.GetIEOtherCode(),
                                        WarehouseId = p.BeginWarehouseId.Value,
                                        InventoryTypeId = EnumInventoryType.ĐK,
                                        Note = "Tồn đầu",
                                        CreatedDate = DateTime.Now,
                                        CreatedAccount = currentAccount.UserName,
                                        Actived = true,
                                        CreatedEmployeeId = Account.EmployeeId,
                                        TotalPrice = p.BeginInventoryQty * p.COGS
                                    };
                                    _context.Entry(IEMaster).State = System.Data.Entity.EntityState.Added;
                                    _context.SaveChanges();

                                    // Lưu vào IEOtherDetailModel
                                    IEOtherDetailModel detailmodel = new IEOtherDetailModel()
                                    {
                                        IEOtherMasterId = IEMaster.IEOtherMasterId,
                                        ProductId = p.ProductId,
                                        ImportQty = p.BeginInventoryQty,
                                        ExportQty = 0,
                                        Price = p.COGS,
                                        UnitShippingWeight = (decimal)(p.ShippingWeight.HasValue ? p.ShippingWeight.Value : 1) * p.BeginInventoryQty,
                                        UnitPrice = p.BeginInventoryQty * p.COGS,
                                        Note = IEMaster.Note
                                    };
                                    _context.Entry(detailmodel).State = System.Data.Entity.EntityState.Added;
                                    _context.SaveChanges();

                                    // Lưu vào InventoyMaster
                                    InventoryMasterModel InvenMaster = new InventoryMasterModel()
                                    {
                                        StoreId = p.StoreId,
                                        InventoryTypeId = EnumInventoryType.ĐK,
                                        WarehouseModelId = p.BeginWarehouseId,
                                        InventoryCode = IEMaster.IEOtherMasterCode,
                                        CreatedDate = IEMaster.CreatedDate,
                                        CreatedAccount = IEMaster.CreatedAccount,
                                        CreatedEmployeeId = IEMaster.CreatedEmployeeId,
                                        Actived = true,
                                        BusinessId = IEMaster.IEOtherMasterId,// Id nghiệp vụ 
                                        BusinessName = "IEOtherMasterModel",// Tên bảng nghiệp vụ
                                        ActionUrl = "/IEOtherMaster/Details/"// Đường dẫn ( cộng ID cho truy xuất)
                                    };
                                    _context.Entry(InvenMaster).State = System.Data.Entity.EntityState.Added;
                                    _context.SaveChanges();

                                    // Lưu vào InventoryDetailModel
                                    InventoryDetailModel InvenDetail = new InventoryDetailModel()
                                    {
                                        InventoryMasterId = InvenMaster.InventoryMasterId,
                                        ProductId = p.ProductId,
                                        BeginInventoryQty = p.BeginInventoryQty,
                                        COGS = p.COGS,// nhập
                                        Price = 0, // => Xuất
                                        ImportQty = p.BeginInventoryQty,
                                        ExportQty = 0,
                                        UnitCOGS = p.COGS * p.BeginInventoryQty, // nhập
                                        UnitPrice = 0, // => Xuất
                                        EndInventoryQty = p.BeginInventoryQty
                                    };
                                    _context.Entry(InvenDetail).State = System.Data.Entity.EntityState.Added;
                                    _context.SaveChanges();
                                }


                            }
                            #endregion
                        }
                    }

                    //Thread thread = new Thread(() => ImportThread(excelfile, StoreId));
                    //thread.Start();
                    //ViewBag.Import = "Đang Import ...";
                    ViewBag.Import = "Import thành công !";
                    CreateViewBag();
                    return View("Index");
                }
            }
            catch //(Exception ex)
            {
                ViewBag.Import = "Lỗi! Vui lòng liên hệ kĩ thuật viên để được giúp đỡ !";
                CreateViewBag();
                return View("Index");

            }

        }


        //private void ImportThread( HttpPostedFileBase excelfile, int? StoreId)
        //{

        //}
        #endregion
    }    
 }

