using Constant;
using EntityModels;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using ViewModels;


namespace WebUI.Controllers
{
    public class PreOrderMasterController : BaseController
    {
        //
        // GET: /PreOrderMaster/

        #region Danh sách yêu cầu đơn hàng
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Search(SellSearchViewModel model, int PageIndex = 1, int PageSize = 10)
        {
            if (model.ToDate.HasValue)
            {
                model.ToDate.Value.AddDays(1).AddMilliseconds(-1);
            }

           var list = (from p in _context.PreOrderMasterModel
                       join orde in _context.PreOrderDetailModel on p.PreOrderId equals orde.PreOrderId
                       join pro in _context.ProductModel on orde.ProductId equals pro.ProductId
                       join cus in _context.CustomerModel on p.CustomerId equals cus.CustomerId
                       join emp in _context.EmployeeModel on cus.EmployeeId equals emp.EmployeeId
                    where
                    (model.Email == null || p.Email.Contains(model.Email)) &&
                    (model.CustomerId == null || p.CustomerId == model.CustomerId) &&
                    (model.Phone == null || p.Phone.Contains(model.Phone)) &&
                    (model.OrderId == null || p.PreOrderId == model.OrderId) &&
                    (model.FromDate == null || p.CreatedDate.Value.CompareTo(model.FromDate.Value) >= 0) &&
                    (model.ToDate == null || p.CreatedDate.Value.CompareTo(model.ToDate.Value) <= 0) &&
                    (model.FromTotalPrice == null || p.TotalPrice >= model.FromTotalPrice) &&
                    (model.ToTotalPrice == null || p.TotalPrice <= model.ToTotalPrice) &&
                    (model.ProductId == null || pro.ProductId == model.ProductId)
                    select new OrderMasterInfoViewModel()
                    {
                        PreOrderId = p.PreOrderId,
                        PreOrderCode = p.PreOrderCode,
                        CreatedDate = p.CreatedDate,
                        FullName = p.FullName,
                        Phone = p.Phone,
                        SaleName = p.SaleName,
                        TotalPrice = p.TotalPrice,
                        OrderCode = p.PreOrderCode,
                        StatusCode = p.StatusCode,
                        Actived = p.Actived,
                        EmployeeName = emp.FullName
                    }).Distinct()
                    .OrderByDescending(p => p.CreatedDate);
           ViewBag.TotalRow = list.Count();
           ViewBag.RowIndex = (PageIndex - 1) * PageSize;
           return PartialView(list.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList());
        }
        #endregion

        #region Thêm mới bán hàng
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        
        public ActionResult Create(int? CustomerId = null)
        {
            PreOrderMasterModel model = new PreOrderMasterModel()
            {
                OrderStatusId = 1,
                Actived = true
            };
            // Lấy tên nhân viên bán hàng
            model.CreatedAccount = currentAccount.UserName;
            AccountModel Account = _context.AccountModel.Where(p => p.UserName == model.CreatedAccount).FirstOrDefault();
            //model.SaleName = Account.EmployeeModel.FullName;
            //ViewBag.SalemanName = model.SaleName;
            if (CustomerId.HasValue)
            {
                var customer = _context.CustomerModel.Where(p => p.CustomerId == CustomerId).FirstOrDefault();
                if (customer != null)
                {
                    model.FullName = customer.FullName;
                    model.IdentityCard = customer.IdentityCard;
                    model.Phone = customer.Phone;
                    model.Gender = customer.Gender;
                    model.Email = customer.Email;
                    model.ProvinceId = customer.ProvinceId;
                    model.DistrictId = customer.DistrictId;
                    model.Address = customer.Address;
                    if (customer.ProvinceId.HasValue)
                    {
                        ViewBag.ProvinceName = customer.ProvinceModel.ProvinceName;
                        if (customer.DistrictId.HasValue)
                        {
                            ViewBag.DistrictName = customer.DistrictModel.Appellation + " " + customer.DistrictModel.DistrictName;
                        }
                    }
                    ViewBag.IdCustomer = customer.CustomerId;
                    ViewBag.IdProvince = customer.ProvinceId;
                    ViewBag.IdDistrict = customer.DistrictId;
                }
            }
            // Tìm giá trị STT order code
            CreateViewBag();
            return View(model);
        }
        public ActionResult _CreateList(List<OrderDetailViewModel> detail = null)
        {

            if (detail == null)
            {
                detail = new List<OrderDetailViewModel>();
            }
            EndInventoryRepository EndRepo = new EndInventoryRepository(_context);
            foreach (var item in detail)
            {
                item.EndInventoryQty = Convert.ToInt32(EndRepo.GetQty(item.ProductId.Value));
            }
            return PartialView(detail);
        }
        public ActionResult _CreatelistInner(List<OrderDetailViewModel> detail = null, string scanBarcode = null, int? ProQty = 1, int? CustomerLevelId = 1)
        {
            if (detail == null)
            {
                detail = new List<OrderDetailViewModel>();
            }
            if (string.IsNullOrEmpty(scanBarcode))
            {
                OrderDetailViewModel item = new OrderDetailViewModel();
                detail.Add(item);
            }
            else // Quét mã vạch
            {
                #region Get thông tin cho product
                int id = 0;
                if (Int32.TryParse(scanBarcode, out id))
                {
                    var product = _context.ProductModel.Where(p => p.ProductId == id).FirstOrDefault();
                    if (product != null)
                    {
                        #region // Nếu Sản phẩm trùng
                        foreach (var itemm in detail)
                        {
                            if (itemm.ProductId == product.ProductId)
                            {
                                itemm.Quantity += ProQty;
                                itemm.UnitPrice += (itemm.Price * ProQty);
                                return PartialView(detail);
                            }
                        }
                        #endregion

                        // EndInventoryQty
                        var tempt = _context.InventoryDetailModel
                                             .OrderByDescending(p => p.InventoryDetailId)
                                             .Where(p => p.ProductId == product.ProductId)
                                             .Select(p => p.EndInventoryQty).FirstOrDefault();

                        var GetInfor = (from p in _context.ProductModel
                                        join price in _context.ProductPriceModel on p.ProductId equals price.ProductId
                                        where (price.CustomerLevelId == CustomerLevelId && p.ProductId == product.ProductId)
                                        select new
                                        {
                                            ShippingWeight = p.ShippingWeight,
                                            PriceForCustomerLevelId = price.Price,
                                            EndInventoryQty = (tempt == null ? 0 : tempt.Value)
                                        }).FirstOrDefault();
                        OrderDetailViewModel item = new OrderDetailViewModel()
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductCode + " | " + product.ProductName,
                            Price = GetInfor.PriceForCustomerLevelId,
                            EndInventoryQty = Convert.ToInt32(GetInfor.EndInventoryQty),
                            Quantity = ProQty,
                            UnitPrice = GetInfor.PriceForCustomerLevelId * ProQty
                        };
                        detail.Add(item);
                    }
                }
                #endregion
            }
            //CreateDetailViewBag();
            return PartialView(detail);
        }
        
        public ActionResult Save(PreOrderMasterModel model, List<OrderDetailViewModel> detail, decimal? GuestAmountPaid,decimal? TotalBillDiscount, decimal ? TotalVAT,  int CreateReceipt = 1)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        model.CreatedDate = DateTime.Now;
                        model.CreatedAccount = currentAccount.UserName;
                        model.OrderStatusId = EnumOrderStatus.KhoiTao;
                        AccountModel Account = _context.AccountModel.Where(p => p.UserName == model.CreatedAccount).FirstOrDefault();
                        model.CreatedEmployeeId = Account.EmployeeId;
                        model.SaleName = Account.EmployeeModel.FullName;
                        model.StatusCode = "TAOMOI";
                        //// Tìm giá trị STT order code
                        model.PreOrderCode = GetOrderCode();
                        if (model.PaymentMethodId == EnumPamentMethod.TienMat)
                        {
                            model.Paid = GuestAmountPaid;
                            model.MoneyTransfer = 0;
                        }
                        else if (model.PaymentMethodId == EnumPamentMethod.ChuyenKhoan)
                        {
                            model.Paid = 0;
                            model.MoneyTransfer = GuestAmountPaid;
                        }
                        else if (model.PaymentMethodId == EnumPamentMethod.CongNo)
                        {
                            model.Paid = 0;
                            model.MoneyTransfer = 0;
                        }
                        else
                        {
                            return Content("Phương thức thanh toán không hợp lệ !");
                        }
                        _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();

                        //_context.SaveChanges();

                        if (detail != null)
                        {
                            if (detail.GroupBy(p => p.ProductId).ToList().Count < detail.Count)
                            {
                                //khong duoc trung san pham
                                return Content("Vui lòng không chọn thông tin sản phẩm trùng nhau");
                            }

                            foreach (var item in detail)
                            {
                                #region Thêm giá vốn
                                decimal? COGS = _context.ProductModel.Where(p => p.ProductId == item.ProductId).Select(p => p.COGS).FirstOrDefault();
                                item.COGS = COGS ?? 0;
                                #endregion
                                EndInventoryRepository EndInventoryRepo = new EndInventoryRepository(_context);
                                decimal TonCuoiTrongHeThong = EndInventoryRepo.GetQty(item.ProductId.Value);

                                if (item.Quantity > TonCuoiTrongHeThong)
                                {
                                    return Content(string.Format("Vui lòng chọn số lượng sản phẩm ' {0} ' nhỏ hơn hoặc bằng trong kho", item.ProductName));
                                }
                                PreOrderDetailModel detailmodel = new PreOrderDetailModel()
                                {
                                    PreOrderId = model.PreOrderId,
                                    ProductId = item.ProductId,
                                    Quantity = item.Quantity,
                                    Price = item.Price,
                                    DiscountTypeId = item.DiscountTypeId,
                                    Discount = item.Discount,
                                    UnitDiscount = item.UnitDiscount,
                                    UnitPrice = item.UnitPrice,
                                    Note = item.Note,
                                    COGS = item.COGS
                                };
                                _context.Entry(detailmodel).State = System.Data.Entity.EntityState.Added;
                            }                            
                            // Cập nhật lại Tổng giá vốn 
                            model.SumCOGSOfOrderDetail = detail.Sum(p => p.COGS);
                            model.TotalDiscount = TotalBillDiscount;
                            model.TotalVAT = TotalVAT;
                            _context.Entry(model).State = System.Data.Entity.EntityState.Modified;

                            _context.SaveChanges();
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
                else
                {
                    return Content("Vui lòng kiểm tra lại thông tin không hợp lệ");
                }

            }
            catch
            {
                return Content("Xảy ra lỗi trong quá trình thêm mới đơn hàng");
            }

        }
        private void CreateViewBag(int? WarehouseId = null, int? BillDiscountTypeId = null, int? PaymentMethodId = null, int? StoreId = null,int? PaymentMethod = null)
        {
            //0. StoreId
            var StoreList = _context.StoreModel.OrderBy(p => p.StoreName)
                .Where(p =>
                    p.Actived == true &&
                    (currentEmployee.StoreId == null || p.StoreId == currentEmployee.StoreId)
                ).ToList();
            ViewBag.StoreId = new SelectList(StoreList, "StoreId", "StoreName", StoreId);
            //1. WarehouseId : Load theo cửa hàng
            var WarehouseList = _context.WarehouseModel.OrderBy(p => p.WarehouseName)
                .Where(p => p.Actived == true && (StoreId == null || p.StoreId == StoreId))
                .ToList();
            ViewBag.WarehouseId = new SelectList(WarehouseList, "WarehouseId", "WarehouseName", WarehouseId);

            //2. BillDiscountType
            var BillDiscountTypeList = _context.DiscountTypeModel.OrderBy(p => p.DiscountName).ToList();
            ViewBag.BillDiscountTypeId = new SelectList(BillDiscountTypeList, "DiscountTypeId", "DiscountName", BillDiscountTypeId);

            //3. PaymentMethodId
            var PaymentMethodList = _context.PaymentMethodModel.OrderBy(p => p.PaymentMethodName).ToList();
            ViewBag.PaymentMethodId = new SelectList(PaymentMethodList, "PaymentMethodId", "PaymentMethodName", PaymentMethodId);

        }
        #endregion

        #region Huỷ đơn hàng
        
        public ActionResult Cancel(int id)
        {
            PreOrderMasterModel model = _context.PreOrderMasterModel
                                             .Where(p => p.PreOrderId == id)
                                             .FirstOrDefault();
            var Resuilt = "";
            if (model == null)
            {
                Resuilt = "Không tìm thấy đơn hàng yêu cầu !";
            }
            else
            {
                model.StatusCode = "HUY";
                model.Actived = false;
               _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
               _context.SaveChanges();
                Resuilt = "success";
            }
            return Json(Resuilt, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Detail
        
        public ActionResult Details(int id)
        {
            PreOrderMasterModel model = _context.PreOrderMasterModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.StoreName = _context.StoreModel.Where(p => p.StoreId == model.StoreId).Select(p => p.StoreName).FirstOrDefault();
            ViewBag.WarhouseName = _context.WarehouseModel.Where(p => p.WarehouseId == model.WarehouseId).Select(p => p.WarehouseName).FirstOrDefault();
            ViewBag.CustomerLevelName = _context.CustomerLevelModel.Where(p => p.CustomerLevelId == model.CustomerLevelId).Select(p => p.CustomerLevelName).FirstOrDefault();
            return View(model);
        }
        public ActionResult _DetailList(List<PreOrderDetailModel> model, decimal BillDiscount, int BillDiscountTypeId, int BillVAT, decimal TotalPrice, string PaymentMethod, decimal RemainingAmount, decimal SumPrice)
        {
            ViewBag.SumPrice = SumPrice;
            ViewBag.BillDiscount = BillDiscountTypeId == 1 ? BillDiscount.ToString("n0") + "đ" : BillDiscount.ToString() + "%";
            ViewBag.TotalPrice = TotalPrice.ToString("n0") + "đ";
            ViewBag.BillVAT = BillVAT;
            ViewBag.PaymentMethod = PaymentMethod;
            ViewBag.GuestAmountPaid = TotalPrice - RemainingAmount;
            ViewBag.RemainingAmount = RemainingAmount;
            return PartialView(model);
        }

        #endregion

        #region
        public ActionResult Edit(int id)
        {
            PreOrderMasterModel model = _context.PreOrderMasterModel.Find(id);
            if (model.ProvinceId.HasValue)
            {
                ViewBag.ProvinceName = model.ProvinceModel.ProvinceName;
                ViewBag.DistrictName = model.DistrictModel.Appellation + " " + model.DistrictModel.DistrictName;
            }

            ViewBag.IdCustomer = model.CustomerId;
            ViewBag.IdProvince = model.ProvinceId;
            ViewBag.IdDistrict = model.DistrictId;
            ViewBag.OrderDetailList = model.PreOrderDetailModel.Select(p => new OrderDetailViewModel()
            {
                OrderDetailId = p.PreOrderDetailId,
                OrderId = p.PreOrderId,
                ProductId = p.ProductId,
                Quantity = p.Quantity,
                Price = p.Price,
                DiscountTypeId = p.DiscountTypeId,
                Discount = p.Discount,
                UnitDiscount = p.UnitDiscount,
                UnitPrice = p.UnitPrice,
                ProductName = p.ProductModel.ProductCode + " | " + p.ProductModel.ProductName,
                Note = p.Note
            }).ToList();


            if (model == null)
            {
                return HttpNotFound();
            }
            CreateViewBag();
            return View(model);
        }
        public ActionResult Update(PreOrderMasterModel model, List<OrderDetailViewModel> detail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        //_context.Entry(model).State = System.Data.Entity.EntityState.Modified; Chỉ Modified những filed cần thiết
                        var PreOrderDetail = _context.PreOrderDetailModel
                                                            .Where(p => p.PreOrderId == model.PreOrderId).ToList();
                        // Xoá những Detail cũ
                        if (PreOrderDetail != null && PreOrderDetail.Count > 0)
                        {
                            foreach (var item in PreOrderDetail)
                            {
                                _context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                            }

                        }

                        #region // Update lại những filed cần Modified cho Model Master
                        //PreOrderMastermodel.PreOrderId = model.PreOrderId;
                        //PreOrderMastermodel.WarehouseId = model.WarehouseId;
                        //PreOrderMastermodel.BillDiscountTypeId = model.BillDiscountTypeId;
                        //PreOrderMastermodel.BillDiscount = model.BillDiscount;
                        //PreOrderMastermodel.BillVAT = model.BillVAT;
                        //PreOrderMastermodel.SaleName = model.SaleName;
                        //PreOrderMastermodel.DebtDueDate = model.DebtDueDate;
                        //PreOrderMastermodel.PaymentMethodId = model.PaymentMethodId;
                        //PreOrderMastermodel.Paid = model.Paid;
                        //PreOrderMastermodel.MoneyTransfer = model.MoneyTransfer;
                        //PreOrderMastermodel.CompanyName = model.CompanyName;
                        //PreOrderMastermodel.TaxBillCode = model.TaxBillCode;
                        //PreOrderMastermodel.ContractNumber = model.ContractNumber;
                        //PreOrderMastermodel.TaxBillDate = model.TaxBillDate;
                        //PreOrderMastermodel.CustomerId = model.CustomerId;
                        //PreOrderMastermodel.FullName = model.FullName;
                        //PreOrderMastermodel.IdentityCard = model.IdentityCard;
                        //PreOrderMastermodel.FullName = model.FullName;
                        //PreOrderMastermodel.Phone = model.Phone;
                        //PreOrderMastermodel.FullName = model.FullName;
                        //PreOrderMastermodel.Gender = model.Gender;
                        //PreOrderMastermodel.ProvinceId = model.ProvinceId;
                        //PreOrderMastermodel.DistrictId = model.DistrictId;
                        //PreOrderMastermodel.Address = model.Address;
                        //PreOrderMastermodel.Email = model.Email;
                        //PreOrderMastermodel.Note = model.Note;
                        //PreOrderMastermodel.TotalPrice = model.TotalPrice;
                        //PreOrderMastermodel.LastModifiedDate = DateTime.Now;
                        //PreOrderMastermodel.LastModifiedAccount = currentAccount.UserName;
                        //PreOrderMastermodel.OrderStatusId = EnumOrderStatus.KhoiTao;
                        #endregion
                        // đánh dấu sửa OrderMastermodel
                        _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();

                        if (detail != null)
                        {
                            foreach (var item in detail)
                            {
                                PreOrderDetailModel detailmodel = new PreOrderDetailModel()
                                {
                                    PreOrderDetailId = item.OrderDetailId,
                                    PreOrderId = model.PreOrderId,
                                    ProductId = item.ProductId,
                                    Quantity = item.Quantity,
                                    Price = item.Price,
                                    DiscountTypeId = item.DiscountTypeId,
                                    Discount = item.Discount,
                                    UnitDiscount = item.UnitDiscount,
                                    UnitPrice = item.UnitPrice,
                                };
                                _context.Entry(detailmodel).State = System.Data.Entity.EntityState.Added;
                            }
                        }
                        _context.SaveChanges();
                        ts.Complete();
                        return Content("success");
                    }
                }
                else
                {
                    return Content("Vui lòng kiểm tra lại thông tin không hợp lệ");
                }
                
            }
            catch (Exception ex)
            {
                return Content("Lỗi hệ thống" + ex.Message);
            }


        }

        #endregion

        #region Hàm GetOrdercode
        public string GetOrderCode()
        {
            // Tìm giá trị STT order code
            string OrderCodeToFind = string.Format("{0}-{1}{2}", "KDT", (DateTime.Now.Year).ToString().Substring(2), (DateTime.Now.Month).ToString().Length == 1 ? "0" + (DateTime.Now.Month).ToString() : (DateTime.Now.Month).ToString());
            var Resuilt = _context.PreOrderMasterModel.OrderByDescending(p => p.OrderId).Where(p => p.PreOrderCode.Contains(OrderCodeToFind)).Select(p => p.PreOrderCode).FirstOrDefault();
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

        public ActionResult GetPreOrderId(string q)
        {
            var data2 = _context
                        .PreOrderMasterModel
                        .Where(p => q == null || (p.PreOrderCode).Contains(q))
                        .Select(p => new
                        {
                            value = p.PreOrderId,
                            text = p.PreOrderCode
                        })
                        .Take(10)
                        .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }

    }
}
