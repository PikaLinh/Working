using EntityModels;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace WebUI.Controllers
{
    public class InventoryReportChicCutController : BaseController
    {
        // GET: InventoryReportChicCut
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public ActionResult _ProductPartial(InventorySearchViewModel model)
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

        private void GetData(InventorySearchViewModel model, ref  List<ProductInfoViewModel> List)
        {

            SqlParameter paraCategoryId = new SqlParameter("@CategoryId", model.CategoryId);
            SqlParameter paraToDate = new SqlParameter("@ToDate", model.ToDate);

            #region Kiểm tra giá trị tham số null => khởi tạo giá trị mặc định

            if (!model.CategoryId.HasValue)
            {
                paraCategoryId.Value = 2;
            }
            if (!model.ToDate.HasValue)
            {
                paraToDate.Value = DBNull.Value;
            }

            #endregion

            List = _context.Database.
                                       SqlQuery<ProductInfoViewModel>
                                       ("usp_BaoCaoTonKhoChicCut @CategoryId, @ToDate"
                                       , paraCategoryId
                                       ,paraToDate
                                       ).ToList();
        }

        private void CreateViewBag(int? CategoryId = null, int? OriginOfProductId = null, int? CustomerLevelId = null, int? WarehouseId = null)
        {
            //1. CategoryId
            CategoryRepository repository = new CategoryRepository(_context);
            var CategoryList = repository.GetCategoryByParentWithFormat(2).Select(p => new { CategoryId = p.CategoryId, CategoryName = p.CategoryName.Substring(4) }).ToList();
            CategoryList.RemoveAt(0);
            CategoryList.Insert(0, new { CategoryId = 2, CategoryName = "Tất cả sản phẩm" });
            ViewBag.CategoryId = new SelectList(CategoryList, "CategoryId", "CategoryName", CategoryId);
        }

        #region Export
        public ActionResult Export(InventorySearchViewModel model)
        {
            List<ProductInfoViewModel> listProduct = new List<ProductInfoViewModel>();
            GetData(model, ref listProduct);

            //Tên file
            string strName = string.Format("{0}_{1}_{2}", "Danh_sach_san_pham_ton_kho", DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss"), ".xlsx");
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
                ExcelWorksheet ws = CreateSheet(p, "Danh sách sản phẩm tồn kho");


                //Merging cells and create a center heading for out table
                ws.Cells[2, 1].Value = "Danh sách sản phẩm tồn kho";
                ws.Cells[2, 1, 2, 6].Merge = true;
                ws.Cells[2, 1, 2, 6].Style.Font.Bold = true;
                ws.Cells[2, 1, 2, 6].Style.Font.Size = 22;
                ws.Cells[2, 1, 2, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                int rowIndex = 5;
                int row = 6;
                CreateHeader(ws, ref rowIndex);
                CreateData(ws, ref row, listProduct);
                Byte[] bin = p.GetAsByteArray();

                return bin;

            }
        }

        private static void SetWorkbookProperties(ExcelPackage p)
        {
            //Here setting some document properties
            p.Workbook.Properties.Author = "NgaNguyen";
            p.Workbook.Properties.Title = "Danh sách sản phẩm cần tồn kho";
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
            worksheet.Column(3).Width = 60;
            worksheet.Column(4).Width = 15;
            worksheet.Column(5).Width = 15;

            //tiêu đề cột
            worksheet.Cells[rowIndex, 1].Value = "STT";
            worksheet.Cells[rowIndex, 2].Value = "Mã sản phẩm";
            worksheet.Cells[rowIndex, 3].Value = "Tên sản Phẩm";
            worksheet.Cells[rowIndex, 4].Value = "Quy cách";
            worksheet.Cells[rowIndex, 5].Value = "Tồn hệ thống";

            for (int i = 1; i <= 5; i++)
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

        private static void CreateData(ExcelWorksheet worksheet, ref int rowIndex, List<ProductInfoViewModel> listProduct)
        {
            EntityDataContext db = new EntityDataContext();
            int? CategoryId = -1;
            int Index = 1;

            listProduct.OrderBy(p => p.CategoryId);

            foreach (ProductInfoViewModel p in listProduct)
            {

                if (p.CategoryId != CategoryId)
                {
                    #region Tên danh mục sản phẩm
                    for (int i = 1; i <= 5; i++)
                    {
                        var cell2 = worksheet.Cells[rowIndex, i];
                        cell2.Style.Border.Bottom.Style = cell2.Style.Border.Top.Style = cell2.Style.Border.Left.Style = cell2.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                    worksheet.Cells[rowIndex, 2, rowIndex, 5].Value = p.CategoryName;
                    worksheet.Cells[rowIndex, 2, rowIndex, 5].Merge = true;
                    worksheet.Cells[rowIndex, 2, rowIndex, 5].Style.Font.Bold = true;
                    #endregion

                    #region Sản phẩm đầu danh mục
                    for (int i = 1; i <= 5; i++)
                    {
                        var cell2 = worksheet.Cells[rowIndex + 1, i];
                        cell2.Style.Border.Bottom.Style = cell2.Style.Border.Top.Style = cell2.Style.Border.Left.Style = cell2.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                    Index = 1;
                    worksheet.Cells[rowIndex + 1, 1].Value = Index;
                    worksheet.Cells[rowIndex + 1, 2].Value = p.ProductCode;
                    worksheet.Cells[rowIndex + 1, 3].Value = p.ProductName;
                    worksheet.Cells[rowIndex + 1, 4].Value = p.Specifications;
                    worksheet.Cells[rowIndex + 1, 5].Value = p.Inventory;

                    Index++;
                    #endregion

                    rowIndex = rowIndex + 2;
                    CategoryId = p.CategoryId;
                }
                else
                {
                    #region Sản phẩm tiếp theo
                    for (int i = 1; i <= 5; i++)
                    {
                        var cell2 = worksheet.Cells[rowIndex, i];
                        cell2.Style.Border.Bottom.Style = cell2.Style.Border.Top.Style = cell2.Style.Border.Left.Style = cell2.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    worksheet.Cells[rowIndex, 1].Value = Index;
                    worksheet.Cells[rowIndex, 2].Value = p.ProductCode;
                    worksheet.Cells[rowIndex, 3].Value = p.ProductName;
                    worksheet.Cells[rowIndex, 4].Value = p.Specifications;
                    worksheet.Cells[rowIndex, 5].Value = p.Inventory;
                    Index++;
                    #endregion
                    rowIndex++;
                }
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

    }
}