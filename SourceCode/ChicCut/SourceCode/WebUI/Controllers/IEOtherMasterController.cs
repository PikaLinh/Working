using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using ViewModels;
using System.Transactions;
using Constant;
using System.Data;
using Repository;


namespace WebUI.Controllers
{
    public class IEOtherMasterController : BaseController
    {
        //
        // GET: /IEOtherMaster/
        
        public ActionResult Index()
        {
            CreateViewSearchBag();
            return View(_context.IEOtherMasterModel.OrderByDescending(p => p.IEOtherMasterId).Where(p => p.Actived == true && p.InventoryTypeId != 7).ToList());
        }

        #region Huỷ đơn hàng
        
        public ActionResult Cancel(int id)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                IEOtherMasterModel model = _context.IEOtherMasterModel
                                                 .Where(p => p.IEOtherMasterId == id)
                                                 .FirstOrDefault();
                var Resuilt = "";
                if (model == null)
                {
                    Resuilt = "Không tìm thấy đơn hàng yêu cầu !";
                }
                else
                {

                    using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                    {
                        using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                        {
                            cmd.CommandText = "usp_IEOtherCanceled";
                            cmd.Parameters.AddWithValue("@IEOtherMasterId", model.IEOtherMasterId);
                            cmd.Parameters.AddWithValue("@DeletedDate", DateTime.Now);
                            AccountModel Account = _context.AccountModel.Where(p => p.UserName == model.CreatedAccount).FirstOrDefault();
                            model.DeletedEmployeeId = Account.EmployeeId;

                            cmd.Parameters.AddWithValue("@DeletedAccount", currentAccount.UserName);
                            cmd.Parameters.AddWithValue("@DeletedEmployeeId", model.DeletedEmployeeId);
                            cmd.Connection = conn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                    //_context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    //_context.SaveChanges();
                    Resuilt = "success";
                }
                ts.Complete();
                return Json(Resuilt, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region CreateViewSearchBag
        private void CreateViewSearchBag(int? WarehouseId = null, int? InventoryTypeId = null)
        {
            //1. WarehouseId
            var WarehouseList = _context.WarehouseModel.OrderBy(p => p.WarehouseName).ToList();
            ViewBag.WarehouseId = new SelectList(WarehouseList, "WarehouseId", "WarehouseName", WarehouseId);

            //2. SupplierId
            var InventoryTypeList = _context.InventoryTypeModel.Where(p => p.InventoryTypeId == 3 || p.InventoryTypeId == 4).ToList();
            ViewBag.InventoryTypeId = new SelectList(InventoryTypeList, "InventoryTypeId", "InventoryTypeName", InventoryTypeId);

        }

        #endregion
        #region _SearchIEOtherMaster
        public ActionResult _SearchIEOtherMaster(IEOtherMasterSearchViewModel model)
        {
            List<IEOtherMasterInfoViewModel> List = new List<ViewModels.IEOtherMasterInfoViewModel>();
            if (model.ToDate.HasValue)
            {
                model.ToDate.Value.AddDays(1).AddMilliseconds(-1);
            }
            List = (from IEOMaster in _context.IEOtherMasterModel
                    join WM in _context.WarehouseModel on IEOMaster.WarehouseId equals WM.WarehouseId
                    join IEODetail in _context.IEOtherDetailModel on IEOMaster.IEOtherMasterId equals IEODetail.IEOtherMasterId
                    //join PM in _context.ProductModel on IEODetail.ProductId equals PM.ProductId
                    where
                         (model.WarehouseId == null || IEOMaster.WarehouseId == model.WarehouseId) &&
                         (model.InventoryTypeId == null || IEOMaster.InventoryTypeId == model.InventoryTypeId)&&
                         (model.ProductId == null || IEODetail.ProductId == model.ProductId) &&
                         (model.IEOtherMasterId == null || IEOMaster.IEOtherMasterId == model.IEOtherMasterId) &&
                         (model.FromDate == null || IEOMaster.CreatedDate.Value.CompareTo(model.FromDate.Value) >= 0) &&
                         (model.ToDate == null || IEOMaster.CreatedDate.Value.CompareTo(model.ToDate.Value) <= 0) &&
                         IEOMaster.Actived == true &&
                         IEOMaster.InventoryTypeId != 7
                   select new IEOtherMasterInfoViewModel()
                   {
                       IEOtherMasterId = IEOMaster.IEOtherMasterId,
                       IEOtherMasterCode = IEOMaster.IEOtherMasterCode,
                       CreatedDate = IEOMaster.CreatedDate,
                       WarehouseName = WM.WarehouseName,
                       TotalPrice = IEOMaster.TotalPrice,
                       Money = IEOMaster.Money
                   }).Distinct()
                   .OrderByDescending(p => p.CreatedDate)
                   .ToList();

            return PartialView(List);
        }
        #endregion

        public ActionResult GetIEOtherMasterId(string q)
        {
            var data2 = _context
                        .IEOtherMasterModel
                        .Where(p => q == null || (p.IEOtherMasterCode).Contains(q))
                        .OrderBy(p => p.IEOtherMasterCode)
                        .Select(p => new
                        {
                            value = p.IEOtherMasterId,
                            text = p.IEOtherMasterCode
                        })
                        .Take(10)
                        .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }
        #region Thêm mới xuất nhập kho khác
        
        public ActionResult Create(int? CustomerId = null)
        {
            IEOtherMasterModel model = new IEOtherMasterModel()
            {
                Actived = true
            };
            if (CustomerId.HasValue)
            {
                var customer = _context.CustomerModel.Where(p => p.CustomerId == CustomerId).FirstOrDefault();
                if (customer != null)
                {
                    model.Phone = customer.Phone;
                }
                ViewBag.IdCustomer = customer.CustomerId;
                ViewBag.FullName = customer.FullName;
            }
            CreateViewBag();
            return View(model);
        }

        private void CreateViewBag(int? WarehouseId = null, int? InventoryTypeId = null)
        {
            //1. WarehouseId
            var WarehouseList = _context.WarehouseModel.OrderBy(p => p.WarehouseName).ToList();
            ViewBag.WarehouseId = new SelectList(WarehouseList, "WarehouseId", "WarehouseName", WarehouseId);

            //2. InventoryTypeId
            var InventoryTypeList = _context.InventoryTypeModel.OrderByDescending(p => p.InventoryTypeId).Where(p => p.InventoryTypeId == 3 || p.InventoryTypeId == 4).ToList();
            ViewBag.InventoryTypeId = new SelectList(InventoryTypeList, "InventoryTypeId", "InventoryTypeName", InventoryTypeId);
        
        }
        #endregion

        #region GetProfileByCustomerId
        public ActionResult GetProfileByCustomerId(int CustomerID)
        {
            var Profile = _context.CustomerModel
                               .Where(p => p.CustomerId == CustomerID)
                               .Select(p => new
                               {
                                   FullName = p.FullName,
                                   IdentityCard = p.IdentityCard,
                                   Phone = p.Phone,
                                   Gender = p.Gender,
                                   ProvinceId = p.ProvinceId,
                                   DistrictId = p.DistrictId,
                                   Address = p.Address,
                                   Email = p.Email,
                                   ProvinceName = p.ProvinceModel.ProvinceName,
                                   DistrictName = p.DistrictModel.DistrictName
                               })
                               .FirstOrDefault();

            return Json(Profile, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region _CreateList
        public ActionResult _CreateList(List<IEOtherDetailViewModel> detail = null)
        {

            if (detail == null)
            {
                detail = new List<IEOtherDetailViewModel>();
            }

            return PartialView(detail);
        }
        #endregion

        //#region _CreatelistInner
        //public ActionResult _CreatelistInner(List<IEOtherDetailViewModel> detail = null)
        //{
        //    if (detail == null)
        //    {
        //        detail = new List<IEOtherDetailViewModel>();
        //    }
        //    IEOtherDetailViewModel item = new IEOtherDetailViewModel();
        //    detail.Add(item);
        //    return PartialView(detail);
        //}
        //#endregion

        #region _CreatelistInner
        public ActionResult _CreatelistInner(List<IEOtherDetailViewModel> detail = null, string scanBarcode = null, decimal ProQty = 1)
        {
            //Nếu detail == null thì khởi tạo List mới
            if (detail == null)
            {
                detail = new List<IEOtherDetailViewModel>();
                //Nếu barcode == null thì thêm mới sản phẩm
                if (string.IsNullOrEmpty(scanBarcode))
                {
                    IEOtherDetailViewModel item = new IEOtherDetailViewModel();
                    detail.Add(item);
                }
                else  // Quét mã vạch
                {
                    #region Get thông tin cho warehouse

                    //Lấy sản phẩm dựa vào barcode
                    var product = _context.ProductModel.Where(p => p.Barcode == scanBarcode).FirstOrDefault();
                    if (product != null)
                    {
                        IEOtherDetailViewModel item = new IEOtherDetailViewModel()
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductCode + " | " + product.ProductName,
                            Qty = ProQty,
                        };
                        detail.Add(item);
                    }
                    else
                    {
                        return Json("Mã vạch này chưa tồn tại!", JsonRequestBehavior.AllowGet);
                    }
                    //}
                    #endregion
                }
            }
            //Nếu detail != null thì add thêm vào không cần khởi tạo List mới
            else
            {
                if (string.IsNullOrEmpty(scanBarcode))
                {
                    IEOtherDetailViewModel item = new IEOtherDetailViewModel();
                    detail.Add(item);
                }
                else  // Quét mã vạch
                {
                    #region Get thông tin cho warehouse

                    //Lấy sản phẩm dựa vào barcode
                    var product = _context.ProductModel.Where(p => p.Barcode == scanBarcode).FirstOrDefault();
                    if (product != null)
                    {
                        #region // Nếu Sản phẩm trùng
                        foreach (var itemm in detail)
                        {
                            if (itemm.ProductId == product.ProductId)
                            {
                                itemm.Qty += ProQty;
                                return PartialView(detail);
                            }
                        }
                        #endregion

                        IEOtherDetailViewModel item = new IEOtherDetailViewModel()
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductCode + " | " + product.ProductName,
                            Qty = ProQty
                        };
                        detail.Add(item);
                    }
                    else
                    {
                        return Json("Mã vạch này chưa tồn tại!", JsonRequestBehavior.AllowGet);
                    }
                    //}
                    #endregion
                }
            }

            return PartialView(detail);
        }
        #endregion

        #region _DeletelistInner
        public ActionResult _DeletelistInner(List<IEOtherDetailViewModel> detail, int RemoveId)
        {
            if (detail == null)
            {
                detail = new List<IEOtherDetailViewModel>();
            }
            return PartialView("_CreatelistInner", detail.Where(p => p.STT != RemoveId).ToList());
        }
        #endregion

        #region ProductModel
        public ActionResult GetProductId(string q)
        {
            var data2 = _context
                        .ProductModel
                        .Where(p => q == null || (p.ProductCode + " " + p.ProductName).Contains(q))
                        .Select(p => new
                        {
                            value = p.ProductId,
                            text = p.ProductCode + " | " + p.ProductName
                        })
                        .Take(10)
                        .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //#region GetUnitShippingWeight
        //public ActionResult GetUnitShippingWeight(int SelectedProductid)
        //{
        //    var UnitShippingWeight = _context
        //               .ProductModel
        //               .Where(p => p.ProductId == SelectedProductid)
        //               .Select(p => new
        //               {
        //                   Price = p.ImportPrice,
        //                   ShippingWeight = p.ShippingWeight,
        //                   ShippingFee = p.ShippingFee
        //               })
        //               .FirstOrDefault();
        //    return Json(UnitShippingWeight, JsonRequestBehavior.AllowGet);
        //}
        //#endregion
        #region Save
        
        public ActionResult Save(IEOtherMasterModel model, List<IEOtherDetailViewModel> detail)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    model.CreatedDate = DateTime.Now;
                    model.CreatedAccount = currentAccount.UserName;
                    AccountModel Account = _context.AccountModel.Where(p => p.UserName == model.CreatedAccount).FirstOrDefault();
                    model.CreatedEmployeeId = Account.EmployeeId;
                    int CustomerId = Convert.ToInt32(model.CustomerName);
                    #region XÁc nhận mã IEOtherMasterCode 1 lần nữa trước khi insert
                    // Insert InventoryMaster
                    InventoryMasterModel InvenMaster = new InventoryMasterModel();
                    //Xác định Nhập hay xuất
                    var IsImport = _context.InventoryTypeModel.Where(p => p.InventoryTypeId == model.InventoryTypeId).FirstOrDefault();
                    IEOtherRepository IEOtherRepository = new IEOtherRepository(_context);
                    model.IEOtherMasterCode = IEOtherRepository.GetIEOtherCode(IsImport.isImport.Value);
                    #endregion
                    model.CustomerName = _context.CustomerModel.Where(p => p.CustomerId == CustomerId).Select(p => p.FullName).FirstOrDefault();
                    _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                    _context.SaveChanges(); // LƯU TẠM ĐỂ LẤY IMPORTMASTERID (SẼ BỊ SCROLLBACK KHI XẢY RA LỖI)
                    if (IsImport.isImport == true)
                    {
                        InvenMaster.InventoryTypeId = EnumInventoryType.NK;
                    }
                    else
                    {
                        InvenMaster.InventoryTypeId = EnumInventoryType.XK;
                    }
                    InvenMaster.WarehouseModelId = model.WarehouseId;
                    InvenMaster.InventoryCode = model.IEOtherMasterCode;
                    InvenMaster.CreatedDate = model.CreatedDate;
                    InvenMaster.CreatedAccount = model.CreatedAccount;
                    InvenMaster.CreatedEmployeeId = model.CreatedEmployeeId;
                    InvenMaster.Actived = true;
                    InvenMaster.BusinessId = model.IEOtherMasterId; // Id nghiệp vụ 
                    InvenMaster.BusinessName = "IEOtherMasterModel";// Tên bảng nghiệp vụ
                    InvenMaster.ActionUrl = "/IEOtherMaster/Details/";// Đường dẫn ( cộng ID cho truy xuất)
                    _context.Entry(InvenMaster).State = System.Data.Entity.EntityState.Added;
                    _context.SaveChanges(); // insert tạm để lấy InvenMasterID
                    if (detail != null)
                    {
                        if (detail.GroupBy(p => p.ProductId).ToList().Count < detail.Count)
                        {
                            //khong duoc trung san pham
                            return Content("Vui lòng không chọn thông tin sản phẩm trùng nhau");
                        }
                        foreach (var item in detail)
                        {
                            IEOtherDetailModel detailmodel = new IEOtherDetailModel()
                            {
                                IEOtherMasterId = model.IEOtherMasterId,
                                ProductId = item.ProductId,
                                //Qty = item.Qty,
                                ImportQty = IsImport.isImport == true? item.Qty : 0,
                                ExportQty = IsImport.isImport == false? item.Qty : 0,
                                Price = item.Price,
                                UnitShippingWeight = item.UnitShippingWeight,
                                UnitPrice = item.UnitPrice,
                                Note = item.Note
                            };
                            //_context.Entry(detailmodel).State = System.Data.Entity.EntityState.Added;
                            _context.Entry(detailmodel).State = System.Data.Entity.EntityState.Added;

                            // Insert InventoryDetail
                            //var temp = _context.InventoryDetailModel.OrderByDescending(p => p.InventoryDetailId).Where(p => p.ProductId == item.ProductId).Select(p => p.EndInventoryQty).FirstOrDefault();
                            var temp = (from detal in _context.InventoryDetailModel
                                        join master in _context.InventoryMasterModel on detal.InventoryMasterId equals master.InventoryMasterId
                                        orderby detal.InventoryDetailId descending
                                        where master.Actived == true && detal.ProductId == item.ProductId
                                        select new
                                        {
                                            TonCuoi = detal.EndInventoryQty.Value
                                        }).FirstOrDefault();
                            decimal tondau;
                            if (temp != null)
                            {
                                tondau = Convert.ToInt32(temp.TonCuoi);
                            }
                            else
                            {
                                tondau = 0;
                            }
                            var tempt2 = _context.ProductModel.Where(p => p.ProductId == item.ProductId).FirstOrDefault();
                            decimal GiaVon = tempt2.COGS.Value;
                            if (IsImport.isImport == true)
                            {
                                InventoryDetailModel InvenDetail = new InventoryDetailModel()
                                {
                                    InventoryMasterId = InvenMaster.InventoryMasterId,
                                    ProductId = item.ProductId,
                                    BeginInventoryQty = tondau,
                                    COGS = GiaVon,// nhập
                                    //Price = 0, // => Xuất
                                    ImportQty =item.Qty,
                                    //ExportQty = 0,
                                    UnitCOGS = GiaVon * item.Qty, // nhập
                                    //UnitPrice = 0, // => Xuất
                                    EndInventoryQty = tondau + item.Qty
                                };
                                _context.Entry(InvenDetail).State = System.Data.Entity.EntityState.Added;
                            }
                            else
                            {
                                InventoryDetailModel InvenDetail = new InventoryDetailModel()
                                {
                                    InventoryMasterId = InvenMaster.InventoryMasterId,
                                    ProductId = item.ProductId,
                                    BeginInventoryQty = tondau,
                                    //COGS = 0,// nhập
                                    Price = item.Price, // => Xuất
                                    //ImportQty = 0,
                                    ExportQty = item.Qty,
                                    //UnitCOGS = 0, // nhập
                                    UnitPrice = item.Price * item.Qty, // => Xuất
                                    EndInventoryQty = tondau - item.Qty
                                };
                                _context.Entry(InvenDetail).State = System.Data.Entity.EntityState.Added;
                            }
                        }
                        _context.SaveChanges();
                        // đánh dấu Transaction hoàn tất
                        ts.Complete();
                        return Content("success");
                    }
                    else
                    {
                        //chua nhap tt san pham
                        return Content("Vui lòng chọn thông tin sản phẩm");
                    }
                }
            }
            catch
            {
                return Content("Xảy ra lỗi trong quá trình thêm mới nhà cung cấp");
            }

        }
        #endregion

        #region Detail
        public ActionResult Details(int id)
        {
            IEOtherMasterModel IEOthermastermodel = _context.IEOtherMasterModel.Find(id);
            if (IEOthermastermodel == null)
            {
                return HttpNotFound();
            }
            return View(IEOthermastermodel);
        }
        public ActionResult _DetailList(List<IEOtherDetailModel> model, int InventoryTypeId, decimal Money)
        {
            decimal TotalQtyImport = 0, TotalQtyExport = 0, TotalPrice = 0, TotalShipingWeight = 0;
            foreach (var item in model)
            {
                TotalQtyImport += item.ImportQty.HasValue ? item.ImportQty.Value : 0;
                TotalQtyExport += item.ExportQty.HasValue ? item.ExportQty.Value : 0;
                TotalPrice += item.UnitPrice.HasValue ? item.UnitPrice.Value : 0;
                TotalShipingWeight += item.UnitShippingWeight.HasValue ? item.UnitShippingWeight.Value : 0;
            }
            ViewBag.TotalQtyImport = TotalQtyImport;
            ViewBag.TotalQtyExport = TotalQtyExport;
            ViewBag.TotalPrice = TotalPrice;
            ViewBag.TotalShipingWeight = TotalShipingWeight;
            ViewBag.Money = Money;
            var IsImport = _context.InventoryTypeModel.Where(p => p.InventoryTypeId == InventoryTypeId).FirstOrDefault();
            if (IsImport.isImport == true)
            {
                ViewBag.isImport = 1;
            }
            else
            {
                ViewBag.isImport = 0;
            }
            return PartialView(model);
        }
        #endregion
        #region UpdateOrderCode
        public ActionResult UpdateOrderCode(string type)
        {
            int TypeId = Convert.ToInt32(type);
            var IsImport = _context.InventoryTypeModel.Where(p => p.InventoryTypeId == TypeId).FirstOrDefault();
            bool dk = IsImport.isImport.Value;
            IEOtherRepository IEOtherRepository = new IEOtherRepository(_context);
            string OrderCode = IEOtherRepository.GetIEOtherCode(dk);
            return Json(OrderCode, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Hàm GetIEOtherCode
        //public string GetIEOtherCode(bool IsImport)
        //{
        //    // Tìm giá trị STT order code
        //    string OrderCodeToFind = "";
        //    if (IsImport)
        //    {
        //       OrderCodeToFind = string.Format("{0}-{1}{2}", "PNO", (DateTime.Now.Year).ToString().Substring(2), (DateTime.Now.Month).ToString().Length == 1 ? "0" + (DateTime.Now.Month).ToString() : (DateTime.Now.Month).ToString());
        //    }
        //    else
        //    {
        //        OrderCodeToFind = string.Format("{0}-{1}{2}", "PXO", (DateTime.Now.Year).ToString().Substring(2), (DateTime.Now.Month).ToString().Length == 1 ? "0" + (DateTime.Now.Month).ToString() : (DateTime.Now.Month).ToString());
        //    }
        //    var Resuilt = _context.IEOtherMasterModel.OrderByDescending(p => p.IEOtherMasterId).Where(p => p.IEOtherMasterCode.Contains(OrderCodeToFind)).Select(p => p.IEOtherMasterCode).FirstOrDefault();
        //    string OrderCode = "";
        //    if (Resuilt != null)
        //    {
        //        int LastNumber = Convert.ToInt32(Resuilt.Substring(9)) + 1;
        //        string STT = "";
        //        switch (LastNumber.ToString().Length)
        //        {
        //            case 1: STT = "000" + LastNumber.ToString(); break;
        //            case 2: STT = "00" + LastNumber.ToString(); break;
        //            case 3: STT = "0" + LastNumber.ToString(); break;
        //            default: STT = LastNumber.ToString(); break;
        //        }
        //        OrderCode = string.Format("{0}{1}", Resuilt.Substring(0, 9), STT);
        //    }
        //    else
        //    {
        //        OrderCode = string.Format("{0}-{1}", OrderCodeToFind, "0001");
        //    }
        //    return OrderCode;
        //}
        #endregion
    }
}
