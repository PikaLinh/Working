﻿using Constant;
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
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using ViewModels;


namespace WebUI.Controllers
{
    public class DynamicWarehouseInventoryController : BaseController
    {
        // GET: DynamicWarehouseInventory
        
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public ActionResult _ProductPartial(ProductSearchViewModel model)
        {
            DataTable dt = new DataTable();
            try
            {
                GetData(model, ref dt);
                ViewBag.DataViewModel = dt;
                ViewBag.lstCusName = _context.CustomerLevelModel.Where(P => P.Actived == true).ToList();
            }
            catch
            {
                return PartialView();
            }
            return PartialView();
        }

        private void GetData(ProductSearchViewModel model, ref DataTable dt)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.CommandText = "usp_ListCheckinfoDynamic";
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
                    System.Data.SqlClient.SqlDataAdapter adapter = new System.Data.SqlClient.SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                    conn.Close();
                }
            }
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

        #region Export
        public ActionResult Export(ProductSearchViewModel model)
        {
            DataTable dt = new DataTable();
            GetData(model, ref dt);

            //Tên file
            string strName = string.Format("{0}_{1}_{2}", "Danh_sach_san_pham_kiem_kho", DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss"), ".xlsx");
            Byte[] report = GenerateReport(dt);

            string handle = Guid.NewGuid().ToString();

            Session[handle] = report;
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = strName }
            };
        }

        private Byte[] GenerateReport(DataTable dt)
        {
            using (ExcelPackage p = new ExcelPackage())
            {
                //set the workbook properties and add a default sheet in it
                SetWorkbookProperties(p);
                //Create a sheet
                ExcelWorksheet ws = CreateSheet(p, "Danh sách sản phẩm kiểm kho");


                //Merging cells and create a center heading for out table
                ws.Cells[2, 1].Value = "Danh sách sản phẩm kiểm kho";
                ws.Cells[2, 1, 2, 12].Merge = true;
                ws.Cells[2, 1, 2, 12].Style.Font.Bold = true;
                ws.Cells[2, 1, 2, 12].Style.Font.Size = 22;
                ws.Cells[2, 1, 2, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                #region //lưu ý
                ws.Cells[3, 1].Value = "Lưu ý : Nhập số lượng tồn kho thực tế vào ô màu vàng ";

                //màu chữ
                ws.Cells[3, 1].Style.Font.Color.SetColor(Color.Red);

                //border
                var cell = ws.Cells[3, 4];
                cell.Style.Border.Bottom.Style = cell.Style.Border.Top.Style = cell.Style.Border.Left.Style = cell.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                //BackgroundColor
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
                #endregion
                var customer = _context.CustomerLevelModel
                                .Where(m => m.Actived == true)
                                .ToList();

                string[] columnNames = dt.Columns.Cast<DataColumn>()
                                .Select(x => x.ColumnName)
                                .ToArray();

                int rowIndex = 5;
                int row = 6;
                CreateHeader(ws, ref rowIndex, columnNames, customer);
                CreateData(ws, ref row, dt, columnNames);
                Byte[] bin = p.GetAsByteArray();

                return bin;

                // return File(bin, "application/vnd.ms-excel", log.FileName);
            }
        }

        private static void SetWorkbookProperties(ExcelPackage p)
        {
            //Here setting some document properties
            p.Workbook.Properties.Author = "PhongNguyen";
            p.Workbook.Properties.Title = "Danh sách sản phẩm cần kiểm kho";
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

        private static void CreateHeader(ExcelWorksheet worksheet, ref int rowIndex, string[] columnNames, List<CustomerLevelModel> customer)
        {
            #region Định dạng Excel
            //độ rộng cột
            worksheet.Column(1).Width = 5;
            worksheet.Column(2).Width = 15;
            worksheet.Column(3).Width = 33;
            worksheet.Column(4).Width = 20;
            worksheet.Column(5).Width = 38;
            int j = 6;
            for (j = 6 ; j <= columnNames.Length - 4; j++)
            {
                worksheet.Column(j).Width = 15;
            }
            worksheet.Column(j).Width = 20;
            worksheet.Column(j+1).Width = 15;
            worksheet.Column(j+2).Width = 15;

            //format date cột i

            //tiêu đề cột
            worksheet.Cells[rowIndex, 1].Value = "STT";
            worksheet.Cells[rowIndex, 2].Value = "Mã Sản Phẩm ID";
            worksheet.Cells[rowIndex, 3].Value = "Mã SP Cửa hàng";
            worksheet.Cells[rowIndex, 4].Value = "Mã Sản Phẩm";
            worksheet.Cells[rowIndex, 5].Value = "Tên Sản Phẩm";
            int k = 6;
            foreach (var item in customer)
            {
                worksheet.Cells[rowIndex, k++].Value = item.CustomerLevelName;
            }
            worksheet.Cells[rowIndex, k].Value = "Xuất xứ hàng hoá";
            worksheet.Cells[rowIndex, k+1].Value = "Tồn";
            worksheet.Cells[rowIndex, k+2].Value = "Tồn thực tế";
            // worksheet.Cells[rowIndex, 12].Value = "Khoảng chênh lệch";
            for (int i = 1; i < columnNames.Length; i++)
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

        private static void CreateData(ExcelWorksheet worksheet, ref int rowIndex, DataTable dt, string[] columnNames)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 1; j <= 3; ++j)
                {
                    var cell = worksheet.Cells[rowIndex, j];
                    var fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(Color.Gold);
                }
                worksheet.Cells[rowIndex, 1].Value = rowIndex - 6;
                worksheet.Cells[rowIndex, 2].Value = dt.Rows[i]["ProductId"];
                worksheet.Cells[rowIndex, 3].Value = dt.Rows[i]["ProductStoreCode"];
                worksheet.Cells[rowIndex, 4].Value = dt.Rows[i]["ProductCode"];
                worksheet.Cells[rowIndex, 5].Value = dt.Rows[i]["ProductName"];
                int k;
                for (k = 6; k <= columnNames.Length - 4; k++)
                {
                    worksheet.Cells[rowIndex, k].Value = dt.Rows[i][columnNames[k]];
                }
                worksheet.Cells[rowIndex, k].Value = dt.Rows[i]["OriginOfProduct"];
                worksheet.Cells[rowIndex, k+1].Value = dt.Rows[i]["Inventory"];
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
        public ActionResult Import(HttpPostedFileBase excelfile, int WarehouseId)
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

                            #region  // Bước 1 : insert WarehouseInventoryMasterModel
                            WarehouseInventoryMasterModel master = new WarehouseInventoryMasterModel()
                            {
                                WarehouseInventoryMasterCode = GetWarehouseInventoryMasterCode(),
                                CreatedDate = DateTime.Now,
                                CreatedAccount = currentAccount.UserName,
                                CreatedEmployeeId = currentEmployee.EmployeeId,
                                CreatedIEOther = false,
                                Actived = true // TotalQty : Tổng số sản phẩm kiểm kho update sau.
                            };
                            master.WarehouseId = WarehouseId;
                            _context.Entry(master).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();// Lưu để lấy masterID
                            #endregion

                            #region    // bước 2 : duyệt danh sách insert vào WarehouseInventoryDetailModel
                            WarehouseInventoryDetailModel p;
                            int TotalQty = 0;
                            for (int row = 6; worksheet.Cells[row, col].Value != null; row++)
                            {
                                // Nếu Tồn thực tế có giá trị -> Có kiểm kho -> Insert vào database.
                                if (worksheet.Cells[row, 12].Text != "")
                                {
                                    try
                                    {
                                        TotalQty++;
                                        decimal tonkho;
                                        #region // Kiểm tra cột 2, 10 TH rỗng
                                        if (worksheet.Cells[row, 2].Text == "")
                                        {
                                            return Json("Dòng " + row + " Cột 'Mã sản phẩm' bị rỗng", JsonRequestBehavior.AllowGet);
                                        }
                                        if (worksheet.Cells[row, 11].Text == "")
                                        {
                                            // return Json("Dòng" + row + " Cột 'Tồn' bị rỗng", JsonRequestBehavior.AllowGet);
                                            tonkho = 0;
                                        }
                                        else
                                        {
                                            tonkho = decimal.Parse(worksheet.Cells[row, 11].Value.ToString());
                                        }
                                        #endregion
                                        p = new WarehouseInventoryDetailModel()
                                        {
                                            WarehouseInventoryMasterId = master.WarehouseInventoryMasterId,
                                            ProductId = int.Parse(worksheet.Cells[row, 2].Value.ToString()),
                                            Inventory = tonkho,
                                            ActualInventory = decimal.Parse(worksheet.Cells[row, 12].Value.ToString()),
                                            AmountDifference = decimal.Parse(worksheet.Cells[row, 12].Value.ToString()) - tonkho,
                                            Actived = true
                                        };
                                        _context.Entry(p).State = System.Data.Entity.EntityState.Added;
                                        _context.SaveChanges();
                                    }
                                    catch
                                    {

                                    }

                                }
                            }
                            #endregion

                            // Nếu TotalQty != 0 : Danh sách kiểm kho hợp lệ (có tối thiểu 1 sản phẩm kiểm kho), update TotalQty trong master
                            if (TotalQty != 0)
                            {
                                master.TotalQty = TotalQty;
                                _context.Entry(master).State = System.Data.Entity.EntityState.Modified;
                                _context.SaveChanges();
                                ts.Complete();
                                return Json("success", JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json("Danh sách kiểm kho không hợp lệ ! Vui lòng kiểm tra lại", JsonRequestBehavior.AllowGet);
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

        public ActionResult WarehouseInventoryList()
        {
            //var List = _context.WarehouseInventoryMasterModel
            //                    .Where(p => p.Actived == true)
            //                    .OrderByDescending(p => p.WarehouseInventoryMasterId).ToList();
            var List = (from p in _context.WarehouseInventoryMasterModel
                        join wm in _context.WarehouseModel on p.WarehouseId equals wm.WarehouseId
                        orderby p.WarehouseInventoryMasterId descending
                        where (p.Actived == true && p.TotalQty > 0)
                        select new WarehouseInventoryMasterViewModel()
                        {
                            WarehouseInventoryMasterId = p.WarehouseInventoryMasterId,
                            CreatedIEOther = p.CreatedIEOther,
                            WarehouseInventoryMasterCode = p.WarehouseInventoryMasterCode,
                            WarehouseName = wm.WarehouseName,
                            CreatedDate = p.CreatedDate,
                            CreatedAccount = p.CreatedAccount,
                            TotalQty = p.TotalQty
                        }).ToList();

            if (List == null)
            {
                List.Add(new WarehouseInventoryMasterViewModel());
            }
            return PartialView(List);
        }
        #endregion

        #region Huỷ đơn hàng
        
        public ActionResult Cancel(int id)
        {
            WarehouseInventoryMasterModel model = _context.WarehouseInventoryMasterModel
                                             .Where(p => p.WarehouseInventoryMasterId == id)
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

        #region Tạo xuất nhập kho
        
        public ActionResult Autoimex(int id)
        {
            WarehouseInventoryMasterModel model = _context.WarehouseInventoryMasterModel
                                             .Where(p => p.WarehouseInventoryMasterId == id && p.Actived == true && p.CreatedIEOther == false)
                                             .FirstOrDefault();
            // Bước 1 : insert IEOtherMaster 
            // Bước 2 : insert IEOtherDetail : dựa vào AmountDifference để nhập hay xuất kho

            var Resuilt = "";
            if (model == null)
            {
                Resuilt = "Không tìm thấy đơn hàng yêu cầu !";
            }
            else
            {
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        #region insert IEOtherMaster
                        IEOtherRepository IEOtherRepository = new IEOtherRepository(_context);
                        IEOtherMasterModel IEOtherMasterModel = new IEOtherMasterModel()
                        {
                            IEOtherMasterCode = IEOtherRepository.GetIEOtherCodePKK(),
                            WarehouseId = model.WarehouseId.Value,
                            InventoryTypeId = EnumInventoryType.KK,
                            Note = "Bù trừ kiểm kho khớp với hệ thống ",
                            CreatedDate = DateTime.Now,
                            CreatedAccount = currentAccount.UserName,
                            CreatedEmployeeId = currentEmployee.EmployeeId,
                            Actived = true
                        };
                        _context.Entry(IEOtherMasterModel).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();
                        #endregion

                        #region InventoryMasterModel
                        int idStore = _context.WarehouseModel.Where(p => p.WarehouseId == model.WarehouseId).Select(p => p.StoreId.Value).FirstOrDefault();
                        InventoryMasterModel InvenMaster = new InventoryMasterModel()
                        {
                            StoreId = idStore,
                            WarehouseModelId = model.WarehouseId,
                            InventoryTypeId = EnumInventoryType.KK,
                            InventoryCode = IEOtherMasterModel.IEOtherMasterCode,
                            CreatedDate = IEOtherMasterModel.CreatedDate,
                            CreatedAccount = IEOtherMasterModel.CreatedAccount,
                            CreatedEmployeeId = IEOtherMasterModel.CreatedEmployeeId,
                            Actived = true,
                            BusinessId = IEOtherMasterModel.IEOtherMasterId, // Id nghiệp vụ 
                            BusinessName = "IEOtherMasterModel",// Tên bảng nghiệp vụ
                            ActionUrl = "/IEOtherMaster/Details/"// Đường dẫn ( cộng ID cho truy xuất)
                        };
                        _context.Entry(InvenMaster).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges(); // insert tạm để lấy InvenMasterID
                        #endregion

                        #region Duyệt WarehouseInventoryDetailModel insert vào InventoryDetailModel
                        var LstWarehouseInventoryDetailModel = _context.WarehouseInventoryDetailModel
                                                              .Where(p => p.WarehouseInventoryMasterId == model.WarehouseInventoryMasterId)
                                                              .OrderByDescending(p => p.WarehouseInventoryDetailId)
                                                              .ToList();
                        foreach (var item in LstWarehouseInventoryDetailModel)
                        {
                            if (item.AmountDifference != 0)
                            {
                                #region Kiểm tra tồn # database
                                var temp = (from detal in _context.InventoryDetailModel
                                            join master in _context.InventoryMasterModel on detal.InventoryMasterId equals master.InventoryMasterId
                                            orderby detal.InventoryDetailId descending
                                            where master.Actived == true && detal.ProductId == item.ProductId
                                            select new
                                            {
                                                TonCuoi = detal.EndInventoryQty.Value
                                            }).FirstOrDefault();
                                decimal TonTrongDatabase = temp != null ? temp.TonCuoi : 0;
                                if (item.Inventory != TonTrongDatabase)
                                {
                                    return Json("Số lượng tồn không chính xác, vui lòng nhấn nút 'Xem' để cập nhật lại", JsonRequestBehavior.AllowGet);
                                }
                                #endregion

                                #region IEOtherDetailModel
                                IEOtherDetailModel detailmodel = new IEOtherDetailModel()
                                {
                                    IEOtherMasterId = IEOtherMasterModel.IEOtherMasterId,
                                    ProductId = item.ProductId,
                                    ImportQty = item.AmountDifference > 0 ? item.AmountDifference : 0, // Nhập kho sp thừa , xuất kho sp thiếu
                                    ExportQty = item.AmountDifference < 0 ? Math.Abs(item.AmountDifference.Value) : 0,
                                    Price = 0,
                                    UnitShippingWeight = 0,
                                    UnitPrice = 0
                                };
                                _context.Entry(detailmodel).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();
                                #endregion

                                #region Insert InventoryDetail

                                decimal tondau;
                                tondau = TonTrongDatabase;
                                if (item.AmountDifference > 0) // Nhập
                                {
                                    InventoryDetailModel InvenDetail = new InventoryDetailModel()
                                    {
                                        InventoryMasterId = InvenMaster.InventoryMasterId,
                                        ProductId = item.ProductId,
                                        BeginInventoryQty = tondau,
                                        COGS = 0,// nhập
                                        Price = 0, // => Xuất
                                        ImportQty = item.AmountDifference,
                                        ExportQty = 0,
                                        UnitCOGS = 0, // nhập
                                        UnitPrice = 0, // => Xuất
                                        EndInventoryQty = tondau + item.AmountDifference
                                    };
                                    _context.Entry(InvenDetail).State = System.Data.Entity.EntityState.Added;
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    InventoryDetailModel InvenDetail = new InventoryDetailModel()
                                    {
                                        InventoryMasterId = InvenMaster.InventoryMasterId,
                                        ProductId = item.ProductId,
                                        BeginInventoryQty = tondau,
                                        COGS = 0,// nhập
                                        Price = 0, // => Xuất
                                        ImportQty = 0,
                                        ExportQty = Math.Abs(item.AmountDifference.Value),
                                        UnitCOGS = 0, // nhập
                                        UnitPrice = 0, // => Xuất
                                        EndInventoryQty = tondau - Math.Abs(item.AmountDifference.Value)
                                    };
                                    _context.Entry(InvenDetail).State = System.Data.Entity.EntityState.Added;
                                    _context.SaveChanges();
                                }
                                #endregion

                            }
                        }
                        #endregion

                        model.CreatedIEOther = true;
                        _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                        ts.Complete();
                        Resuilt = "success";
                    }
                }
                catch
                {
                    Resuilt = "Xảy ra lỗi trong quá trình tạo xuất nhập kho!";
                }
            }
            return Json(Resuilt, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Xem phiếu kiểm kho
        //
        public ActionResult WarehouseInventoryProducts(int? id)
        {
            #region //B1: Lấy ra danh sách sản phẩm kiểm kho theo phiếu kiểm kho
            var lsttmp = (from p in _context.WarehouseInventoryDetailModel
                          join master in _context.WarehouseInventoryMasterModel on p.WarehouseInventoryMasterId equals master.WarehouseInventoryMasterId
                          // join pm in _context.ProductModel on p.ProductId equals pm.ProductId
                          // join wm in _context.WarehouseModel on master.WarehouseId equals wm.WarehouseId
                          where (p.Actived == true && master.Actived && (id == null || p.WarehouseInventoryMasterId == id))
                          orderby p.WarehouseInventoryDetailId descending
                          select new WarehouseInventoryDetailViewModel()
                          {
                              MasterCode = master.WarehouseInventoryMasterCode,
                              WarehouseInventoryMasterId = p.WarehouseInventoryMasterId,
                              // WarehouseName = wm.WarehouseName,
                              // ProductName = pm.ProductName,
                              Inventory = p.Inventory,
                              ActualInventory = p.ActualInventory,
                              AmountDifference = p.AmountDifference,
                              WarehouseInventoryDetailId = p.WarehouseInventoryDetailId,
                              ProductId = p.ProductId
                          }).ToList();
            #endregion

            var lst = new List<WarehouseInventoryDetailViewModel>();
            #region // Nếu List có giá trị thì mới lấy tên hiển thị phụ và TonTrongDatabase
            if (lsttmp != null && lsttmp.Count > 0)
            {
                int WarehouseId = _context.WarehouseInventoryMasterModel.Where(p => p.WarehouseInventoryMasterId == id).Select(p => p.WarehouseId.Value).FirstOrDefault();

                SqlParameter paramWarehouseId = new SqlParameter("@WarehouseId", WarehouseId);

                #region Para ProductList
                var ProductIdLst = new DataTable();
                ProductIdLst.Columns.Add("ProductId", typeof(int));
                foreach (var item in lsttmp)
                {
                    ProductIdLst.Rows.Add(item.ProductId);
                }
                var pList = new SqlParameter("@ProductLst", SqlDbType.Structured);
                pList.TypeName = "dbo.ProductList";
                pList.Value = ProductIdLst;
                #endregion

                var lstEndInventoryQty = _context.Database.
                                        SqlQuery<WarehouseInventoryDetailViewModel>("dbo.usp_TonCuoiSPTuongUngVoiKho @WarehouseId, @ProductLst", paramWarehouseId, pList).ToList();
                var WarehouseName = _context.WarehouseModel.Where(p => p.WarehouseId == WarehouseId).Select(p => p.WarehouseCode).FirstOrDefault();
                lst = (from p in lsttmp
                       join pEndtmp in lstEndInventoryQty on p.ProductId equals pEndtmp.ProductId into pEndLst
                       from pEnd in pEndLst.DefaultIfEmpty()
                       join pm in _context.ProductModel on p.ProductId equals pm.ProductId
                       select new WarehouseInventoryDetailViewModel()
                       {
                           MasterCode = p.MasterCode
                           ,
                           WarehouseInventoryMasterId = p.WarehouseInventoryMasterId
                           ,
                           WarehouseName = WarehouseName
                           ,
                           ProductName = pm.ProductName
                           ,
                           Inventory = p.Inventory
                           ,
                           ActualInventory = p.ActualInventory
                           ,
                           AmountDifference = p.AmountDifference
                           ,
                           WarehouseInventoryDetailId = p.WarehouseInventoryDetailId
                           ,
                           ProductId = p.ProductId
                           ,
                           TonTrongDatabase = pEnd == null ? 0 : pEnd.TonTrongDatabase
                       }).ToList();
                ViewBag.isLast = 0;
                #region Disable nút xoá
                var MaxMaster = _context.WarehouseInventoryMasterModel
                                          .OrderByDescending(p => p.WarehouseInventoryMasterId)
                                          .FirstOrDefault();
                if (MaxMaster != null) // Nếu là phiếu kiểm kho mới nhất
                {
                    if (MaxMaster.Actived && MaxMaster.TotalQty > 0 && !MaxMaster.CreatedIEOther && MaxMaster.WarehouseInventoryMasterId == id)
                    {
                        ViewBag.isLast = 1;
                    }
                }
                #endregion

            }
            #endregion
            return PartialView(lst);
        }
        #endregion

        #region Xoá sản phẩm
        
        public ActionResult DeleteProduct(int id)
        {
            WarehouseInventoryDetailModel model = _context.WarehouseInventoryDetailModel
                                             .Where(p => p.WarehouseInventoryDetailId == id)
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
                var master = _context.WarehouseInventoryMasterModel.Where(p => p.WarehouseInventoryMasterId == model.WarehouseInventoryMasterId).FirstOrDefault();
                master.TotalQty--;
                _context.Entry(master).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                Resuilt = "success";
            }
            return Json(Resuilt, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Cập nhật tồn kho
        
        public ActionResult UpdateInventoryProduct(decimal tontrongdatabase, int id)
        {
            try
            {
                var model = _context.WarehouseInventoryDetailModel.Where(p => p.WarehouseInventoryDetailId == id).FirstOrDefault();
                if (model == null)
                {
                    return Json("Sản phẩm kiểm kho cần cập nhật không tồn tại trong hệ thống !", JsonRequestBehavior.AllowGet);
                }
                model.Inventory = tontrongdatabase;
                model.AmountDifference = model.ActualInventory - model.Inventory;
                _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("Xảy ra lỗi trong quá trình cập nhật, vui lòng liên hệ bộ phận kỹ thuật để nhận được sự giúp đỡ !", JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Hàm WarehouseInventoryMasterCode
        public string GetWarehouseInventoryMasterCode()
        {
            // Tìm giá trị STT order code
            string OrderCodeToFind = string.Format("{0}-{1}{2}", "PKK", (DateTime.Now.Year).ToString().Substring(2), (DateTime.Now.Month).ToString().Length == 1 ? "0" + (DateTime.Now.Month).ToString() : (DateTime.Now.Month).ToString());
            var Resuilt = _context.WarehouseInventoryMasterModel.OrderByDescending(p => p.WarehouseInventoryMasterId).Where(p => p.WarehouseInventoryMasterCode.Contains(OrderCodeToFind)).Select(p => p.WarehouseInventoryMasterCode).FirstOrDefault();
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
    }
}