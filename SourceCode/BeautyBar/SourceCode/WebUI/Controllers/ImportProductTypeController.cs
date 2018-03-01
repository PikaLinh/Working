using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.InteropServices;
using EntityModels;
using System.IO;
using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Repository;



namespace WebUI.Controllers
{
    public class ImportProductTypeController : BaseController
    {
        //
        // GET: /ProductType/
        
        public ActionResult Index()
        {
            return View();

        }
        public ActionResult Template()
        {

            //Bước 1: lấy path
            string path = Server.MapPath("~/Upload/TemplateProductType/Product-type.xlsx");
            //Bước 2: Copy vào MemoryStream
            MemoryStream memoryStream = new MemoryStream();
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            fileStream.CopyTo(memoryStream);
            fileStream.Close();
            //Bước 3: Trả về cho người dùng
            return File(memoryStream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Octet,"Product - type.xlsx");

        }

        #region Export
        
        public ActionResult Export()
        {
            //Tên file
            string strName = string.Format("{0}_{1}_{2}", "Danh_sach_loai_san_pham", DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss"), ".xlsx");
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
                ExcelWorksheet ws = CreateSheet(p, "Danh sách loại sản phẩm");
                //Merging cells and create a center heading for out table
                ws.Cells[2, 1].Value = "Danh sách loại sản phẩm";
                ws.Cells[2, 1, 2, 6].Merge = true;
                ws.Cells[2, 1, 2, 6].Style.Font.Bold = true;
                ws.Cells[2, 1, 2, 6].Style.Font.Size = 22;
                ws.Cells[2, 1, 2, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                int rowIndex = 4;
                int row = 5;
                List<ProductTypeModel> listProductTypeModel = _context.ProductTypeModel.ToList();
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
            p.Workbook.Properties.Title = "Danh sách loại sản phẩm";


        }

        private static void CreateHeader(ExcelWorksheet worksheet, ref int rowIndex)
        {
            #region Định dạng Excel
            //độ rộng cột
            worksheet.Column(1).Width = 5;
            worksheet.Column(2).Width = 15;
            worksheet.Column(3).Width = 20;
            worksheet.Column(4).Width = 60;
            worksheet.Column(5).Width = 15;
            worksheet.Column(6).Width = 15;
            //format date cột i

            //tiêu đề cột
            worksheet.Cells[rowIndex, 1].Value = "STT";
            worksheet.Cells[rowIndex, 2].Value = "ID";
            worksheet.Cells[rowIndex, 3].Value = "Mã loại sản phẩm";
            worksheet.Cells[rowIndex, 4].Value = "Tên loại sản phẩm";
            worksheet.Cells[rowIndex, 5].Value = "Thứ tự";
            worksheet.Cells[rowIndex, 6].Value = "Kích hoạt";

            for (int i = 1; i <= 6; i++)
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
        private static void CreateData(ExcelWorksheet worksheet, ref int rowIndex, List<ProductTypeModel> listProductTypeModel)
        {
            foreach (ProductTypeModel p in listProductTypeModel)
            {
                #region format
                #endregion
                worksheet.Cells[rowIndex, 1].Value = rowIndex - 4;
                worksheet.Cells[rowIndex, 2].Value = p.ProductTypeId;
                worksheet.Cells[rowIndex, 3].Value = p.ProductTypeCode;
                worksheet.Cells[rowIndex, 4].Value = p.ProductTypeName;
                worksheet.Cells[rowIndex, 5].Value = p.OrderBy;
                worksheet.Cells[rowIndex, 6].Value = p.Actived;
                rowIndex++;
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
                    ViewBag.Import = "Bạn vui lòng chọn 1 file excel";
                    return View("Index");
                }
                else
                {
                    using (var package = new ExcelPackage(excelfile.InputStream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                        int col = 1;
                        for (int row = 5; worksheet.Cells[row, col].Value != null; row++)
                        {
                            ProductTypeModel p = new ProductTypeModel();
                            string ProductTypeCode = worksheet.Cells[row, 3].Value.ToString();
                            int OrderBy =Int32.Parse(worksheet.Cells[row, 5].Value.ToString());
                            if (worksheet.Cells[row, 2].Text=="")
                            {
                                #region kiểm tra tồn tại ProductTypeCode
                                if (ExistProductTypeCode(ProductTypeCode))
                                {
                                    ViewBag.Import = "Dòng " + row + " mã loại sản phẩm đã tồn tại !";
                                    return View("Index");
                                }
                                #endregion

                                #region kiểm tra tồn tại OrderBy
                                if (ExistOrderBy(OrderBy))
                                {
                                    ViewBag.Import = "Dòng " + row + " thứ tự đã tồn tại !";
                                    return View("Index");
                                }
                                #endregion

                                p.ProductTypeCode = ProductTypeCode;
                                p.ProductTypeName = worksheet.Cells[row, 4].Value.ToString();
                                p.OrderBy = OrderBy;
                                p.Actived = true;
                                _context.Entry(p).State = System.Data.Entity.EntityState.Added;
                            }
                            else
                            {
                                int ProductTypeId = Int32.Parse(worksheet.Cells[row, 2].Value.ToString());
                                p = _context.ProductTypeModel.Where(pp => pp.ProductTypeId == ProductTypeId).FirstOrDefault();

                                #region kiểm tra tồn tại ProductTypeCode
                                string ProductTypeCodeUpdate = p.ProductTypeCode;
                                if (ProductTypeCodeUpdate != ProductTypeCode && ExistProductTypeCode(ProductTypeCode))
                                {
                                    ViewBag.Import = "Dòng " + row + " mã loại sản phẩm đã tồn tại !";
                                    return View("Index");
                                }
                                #endregion

                                #region kiểm tra tồn tại OrderBy
                                int ? OrderByUpdate = p.OrderBy;
                                if (OrderByUpdate != OrderBy && ExistOrderBy(OrderBy))
                                {
                                    ViewBag.Import = "Dòng " + row + " thứ tự đã tồn tại !";
                                    return View("Index");
                                }
                                #endregion

                                p.ProductTypeCode = ProductTypeCode;
                                p.ProductTypeName = worksheet.Cells[row, 4].Value.ToString();
                                p.OrderBy = OrderBy;
                                bool Actived = bool.Parse(worksheet.Cells[row, 6].Value.ToString());
                                p.Actived = Actived;
                                _context.Entry(p).State = System.Data.Entity.EntityState.Modified;
                            }
                        }
                        _context.SaveChanges();
                    }
                    ViewBag.Import = "Import Thành công !";
                    return View("Index");
                }

            }
            catch //(Exception ex)
            {
                ViewBag.Import = "Lỗi! Vui lòng liên hệ kĩ thuật viên để được giúp đỡ !";
                return View("Index");

            }

        }
        public bool ExistProductTypeCode(string ProductTypeCode)
        {
            var p = _context.ProductTypeModel.FirstOrDefault(pp => pp.ProductTypeCode == ProductTypeCode);
            if (p != null)
                return true;
            else
                return false;
        }
        public bool ExistOrderBy(int OrderBy)
        {
            var p = _context.ProductTypeModel.FirstOrDefault(pp => pp.OrderBy == OrderBy);
            if (p != null)
                return true;
            else
                return false;
        }

        #endregion
   }
}

