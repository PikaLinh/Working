using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using EntityModels;
using ViewModels;
using Constant;
using System.Transactions;
using System.Data;
using AutoMapper;
using Repository;

namespace WebUI.Controllers
{
    public class SellController : BaseController
    {
        //
        // GET: /Sell/
        #region Danh sách bán hàng
        
        public ActionResult Index()
        {
            return View(_context.OrderMasterModel.Where(p => p.Actived == true).OrderByDescending(p => p.OrderId).ToList());
        }
     
        public ActionResult _SearchSell(SellSearchViewModel model,int PageIndex = 1, int PageSize = 10)
        {
            if (model.ToDate.HasValue)
            {
                model.ToDate.Value.AddDays(1).AddMilliseconds(-1);
            }
            bool isNVKD = _context.RolesModel.Where(p => p.RolesId == currentAccount.RolesId).Select(p => p.Code).FirstOrDefault() == "NVKD" ? true : false;
           var list = (from p in _context.OrderMasterModel
                       join orde in _context.OrderDetailModel on p.OrderId equals orde.OrderId
                       join pro in _context.ProductModel on orde.ProductId equals pro.ProductId
                       join cus in _context.CustomerModel on p.CustomerId equals cus.CustomerId
                      // join emp in _context.EmployeeModel on cus.EmployeeId equals emp.EmployeeId
                    where
                    p.Actived == true &&
                    (model.CustomerId == null || p.CustomerId == model.CustomerId) &&
                    (model.Phone == null || p.Phone.Contains(model.Phone))&&
                    (model.OrderId == null || p.OrderId == model.OrderId) &&
                    (model.FromDate == null || p.CreatedDate.Value.CompareTo(model.FromDate.Value) >= 0) &&
                    (model.ToDate == null || p.CreatedDate.Value.CompareTo(model.ToDate.Value) <= 0) &&
                    (model.FromTotalPrice == null || p.TotalPrice >= model.FromTotalPrice) &&
                    (model.ToTotalPrice == null || p.TotalPrice <= model.ToTotalPrice) &&
                    (model.ProductId == null || pro.ProductId == model.ProductId) &&
                    //nếu là nhóm NVKD:  load n.v bán hàng + n.v quản lý khách hàng là n.v hiện tại đăng nhập
                    ( 
                        (isNVKD == false) 
                        || 
                        ( p.SaleId == currentEmployee.EmployeeId || cus.EmployeeId == currentEmployee.EmployeeId)
                    )
                    select new OrderMasterInfoViewModel()
                    {
                        OrderId = p.OrderId,
                        CreatedDate = p.CreatedDate,
                        FullName = p.FullName,
                        Phone = p.Phone,
                        SaleName = p.SaleName,
                        TotalPrice = p.TotalPrice,
                        OrderCode=p.OrderCode,
                      //  EmployeeName = emp.FullName
                    }).Distinct().OrderByDescending(p=>p.CreatedDate);
           ViewBag.TotalRow = list.Count();
           ViewBag.RowIndex = (PageIndex - 1) * PageSize;
           return PartialView(list.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList());
        }
        public ActionResult GetOrderId(string q)
        {
            var data2 = _context
                        .OrderMasterModel
                        .Where(p => (q == null || (p.OrderCode).Contains(q)) && p.Actived == true)
                        .Select(p => new
                        {
                            value = p.OrderId,
                            text = p.OrderCode
                        })
                        .Take(10)
                        .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Danh sách đơn hàng bị huỷ
        #region Danh sách bán hàng
        
        public ActionResult Canceled()
        {
            return View(_context.OrderMasterModel.Where(p => p.Actived == false).OrderByDescending(p => p.OrderId).ToList());
        }
        [HttpPost]
        public ActionResult Canceled(string FullName = null, string Phone = null, string Email = null)
        {
            var result = _context.OrderMasterModel
                                    .OrderByDescending(p => p.OrderId)
                                    .Where
                                    (
                                        p => p.FullName.Contains(FullName) &&
                                        p.Phone.Contains(Phone) &&
                                        p.Email.Contains(Email) &&
                                        p.Actived == false
                                    )
                                    .ToList();
            return View(result);
        }
        #endregion
        #endregion

        #region Huỷ đơn hàng
        
        public ActionResult Cancel(int id)
        {
            OrderMasterModel model = _context.OrderMasterModel
                                             .Where(p => p.OrderId == id)
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
                        cmd.CommandText = "usp_SellCanceled";
                        cmd.Parameters.AddWithValue("@OrderId", model.OrderId);
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
            return Json(Resuilt, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Khôi phục đơn hàng
        public ActionResult Canceled(int id)
        {
            OrderMasterModel model = _context.OrderMasterModel
                                             .Where(p => p.OrderId == id)
                                             .FirstOrDefault();
            var Resuilt = "";
            if (model == null)
            {
                Resuilt = "Không tìm thấy đơn hàng yêu cầu !";
            }
            else
            {
                model.Actived = true;
                _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                Resuilt = "success";
            }
            return Json(Resuilt, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Thêm mới bán hàng
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        
        public ActionResult Create(int? CustomerId = null, int? PreOrderId = null)
        {
            OrderMasterModel model = new OrderMasterModel()
            {
                OrderStatusId = 1,
                Actived = true
            };
            // Lấy tên nhân viên bán hàng
            model.CreatedAccount = currentAccount.UserName;
            AccountModel Account = _context.AccountModel.Where(p => p.UserName == model.CreatedAccount).FirstOrDefault();
            //model.SaleName = Account.EmployeeModel.FullName;
            //ViewBag.SalemanName = model.SaleName;
            #region Lấy thông tin nhân viên khách hàng (nếu có ReturnUrl)
            if (CustomerId.HasValue)
            {
                GetInfoCustomer(CustomerId.Value, model);
            }
            #endregion

            #region lấy thông tin từ bảng PreOrderMaster (Nếu Xác nhận từ bên PreOrderMaster)
            if (PreOrderId.HasValue)
            {
                var PreOrderModel = _context.PreOrderMasterModel
                                            .Where(p => p.PreOrderId == PreOrderId.Value && p.Actived == true && p.StatusCode != "DADUYET" )
                                            .FirstOrDefault();
                if (PreOrderModel != null)
                {
                    // Thông tin KH
                    GetInfoCustomer(PreOrderModel.CustomerId, model);
                    // Các thông tin còn lại ở header
                    Mapper.CreateMap<PreOrderMasterModel,OrderMasterModel>();
                    model = Mapper.Map(PreOrderModel,model);
                    ViewBag.PreOrderId = PreOrderId;
                   
                }
                else
                {
                    Response.Redirect("/PreOrderMaster/Index");
                }
            }
            #endregion

            CreateViewBag(model.WarehouseId,model.BillDiscountTypeId,model.PaymentMethodId,model.StoreId);
            return View(model);
        }

        private void GetInfoCustomer(int CustomerId, OrderMasterModel model)
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

        private void CreateViewBag(int? WarehouseId = null, int? BillDiscountTypeId = null, int? PaymentMethodId = null, int? StoreId = null)
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

        #region GetCustomerId
        public ActionResult GetCustomerId(string q)
        {
            bool isNVKD = _context.RolesModel.Where(p => p.RolesId == currentAccount.RolesId).Select(p => p.Code).FirstOrDefault() == "NVKD" ? true : false;
            var data2 = _context
                       .CustomerModel
                       .Where(
                               p => q == null || (p.FullName + " - " + p.Phone).Contains(q) &&
                                   //nếu là nhóm NVKD:  load n.v quản lý khách hàng là n.v hiện tại đăng nhập
                                (
                                    (isNVKD == false)
                                    ||
                                    (p.EmployeeId == currentEmployee.EmployeeId)
                                )
                            )
                        .OrderByDescending(p => p.CustomerId)
                       .Select(p => new
                       {
                           value = p.CustomerId,
                           text = (p.FullName + " - " + p.Phone)
                       })
                       
                       .Take(10)
                       .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetSaleId
        public ActionResult GetSaleId(string q)
        {
            var data2 = _context
                       .EmployeeModel
                       .Where(p => q == null || (p.FullName.Contains(q)))
                       .Select(p => new
                       {
                           value = p.EmployeeId,
                           text = p.FullName
                       })
                       .Take(10)
                       .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetProvinceId
        public ActionResult GetProvinceId(string q)
        {
            var data2 = _context
                       .ProvinceModel
                       .Where(p => q == null || (p.ProvinceName.Contains(q)))
                       .Select(p => new
                       {
                           value = p.ProvinceId,
                           text = p.ProvinceName
                       })
                       .Take(10)
                       .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetDistrictByProvinceId
        public ActionResult GetDistrictByProvinceId(string q, int? ProvinceIdSelected = null)
        {
            var data2 = _context
                       .DistrictModel
                       .Where(p => (q == null || (p.Appellation + " " + p.DistrictName).Contains(q)) && p.ProvinceId == ProvinceIdSelected)
                       .Select(p => new
                       {
                           value = p.DistrictId,
                           text = p.Appellation + " " + p.DistrictName
                       })
                       .Take(10)
                       .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetProfileByCustomerId
        public ActionResult GetProfileByCustomerId(int CustomerID)
        {
            var Profile2 = _context.CustomerModel
                               .Where(p => p.CustomerId == CustomerID)
                               .Select(p => new {
                                   FullName = p.FullName,
                                   IdentityCard = p.IdentityCard,
                                   Phone = p.Phone,
                                   Gender = p.Gender,
                                   ProvinceId = p.ProvinceId,
                                   DistrictId = p.DistrictId,
                                   Address = p.Address,
                                   Email = p.Email,
                                   ProvinceName = p.ProvinceModel.ProvinceName,
                                   DistrictName = p.DistrictModel.DistrictName,
                                   CustomerLevelId = p.CustomerLevelId,
                                   EmployeeId = p.EmployeeId,
                               })
                               .FirstOrDefault();
            var FullName = _context.EmployeeModel.Where(p => p.EmployeeId == Profile2.EmployeeId).Select(p => p.FullName).FirstOrDefault();
            var LevelName = _context.CustomerLevelModel.Where(p => p.CustomerLevelId == Profile2.CustomerLevelId).Select(p => p.CustomerLevelName).FirstOrDefault();

            var Profile = new
                        {
                          EmployeeId = Profile2.EmployeeId,
                          CustomerLevelId = Profile2.CustomerLevelId,
                          DistrictName = Profile2.DistrictName,
                          ProvinceName = Profile2.ProvinceName,
                          Email = Profile2.Email,
                          Address = Profile2.Address,
                          DistrictId = Profile2.DistrictId,
                          ProvinceId = Profile2.ProvinceId,
                          Gender = Profile2.Gender,
                          Phone = Profile2.Phone,
                          IdentityCard = Profile2.IdentityCard,
                          FullName = Profile2.FullName,
                          FullNameEmployee = FullName,
                          CustomerLevelName = LevelName
                        };
            return Json(Profile, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetValuePriceAndUnitShippingWeight
        public ActionResult GetValuePriceAndUnitShippingWeight(int SelectedProductid, int CustomerLevelId = 1)
        {
            // ImportPrice, ShippingWeight
            var ValuePriceAndUnitShippingWeight = _context
                       .ProductModel
                       .Where(p => p.ProductId == SelectedProductid)
                       .Select(p => new
                       {
                           Price = p.ImportPrice,
                           ShippingFee = p.ShippingFee,
                           ShippingWeight = p.ShippingWeight
                       })
                       .FirstOrDefault();

            EndInventoryRepository EndInventoryRepo = new EndInventoryRepository(_context);
           decimal tempt = EndInventoryRepo.GetQty(SelectedProductid);

           // Price for CustomerLevelId
           var PriceForCustomerLevelId = _context.ProductPriceModel
                                                 .Where(p => p.ProductId == SelectedProductid && p.CustomerLevelId == CustomerLevelId)
                                                 .Select(p => p.Price)
                                                 .FirstOrDefault();
            var result = new 
            {
                Price = PriceForCustomerLevelId,
                ShippingWeight = ValuePriceAndUnitShippingWeight.ShippingWeight,
                EndInventoryQty = tempt,
                ImportPrice = ValuePriceAndUnitShippingWeight.Price,
                ImportShippingFee = ValuePriceAndUnitShippingWeight.ShippingFee
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region _CreateList
        public ActionResult _CreateList(List<OrderDetailViewModel> detail = null, int? PreOrderId = null)
        {
            if (detail == null)
            {
                detail = new List<OrderDetailViewModel>();
            }
            #region Select PreOrderDetail nếu là xác nhận bên PreOrderMaster
            if (PreOrderId.HasValue)
            {
                detail = (from p in _context.PreOrderDetailModel
                          join pro in _context.ProductModel on p.ProductId equals pro.ProductId
                          where p.PreOrderId == PreOrderId.Value
                            select new OrderDetailViewModel()
                            {
                                ProductId = p.ProductId,
                                Price = p.Price,
                                Quantity = p.Quantity,
                                Note = p.Note,
                                ProductName = pro.ProductCode + " | " + pro.ProductName,
                                UnitPrice = p.UnitPrice
                            }).ToList();
                // Xác định tồn cuối của mỗi sản phẩm
                EndInventoryRepository EndRepo = new EndInventoryRepository(_context);
                foreach (var item in detail)
                {
                    item.EndInventoryQty = Convert.ToInt32(EndRepo.GetQty(item.ProductId.Value));
                }
            }
            #endregion
            return PartialView(detail);
        }
        #endregion

        #region _CreatelistInner
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

        #endregion

        #region GetProductId
        public ActionResult GetProductId(string q)
        {
            var data2 = _context
                        .ProductModel
                        .Where(p => q == null || (p.ProductCode + " " + p.ProductName).Contains(q))
                        .Select(p => new
                        {
                            value = p.ProductId,
                            text = (p.ProductCode + " | " + p.ProductName)
                        })
                        .Take(10)
                        .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductIdIsProduct(string q)
        {
            var data2 = _context
                        .ProductModel
                        .Where(p => 
                            (q == null || (p.ProductCode + " " + p.ProductName).Contains(q))
                            && (p.IsProduct == true)
                        )
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

        #region _DeletelistInner
        public ActionResult _DeletelistInner(List<OrderDetailViewModel> detail, int RemoveId)
        {
            if (detail == null)
            {
                detail = new List<OrderDetailViewModel>();
            }
            return PartialView("_CreatelistInner", detail.Where(p => p.STT != RemoveId).ToList());
        }
        #endregion

        #region _DeleteAll
        public ActionResult _DeleteAll(List<OrderDetailViewModel> detail)
        {
            if (detail == null)
            {
                detail = new List<OrderDetailViewModel>();
            }
            return PartialView("_CreatelistInner", detail.Where(p => p.STT == -1).ToList());
        }
        #endregion

        #region Save
         
        public ActionResult Save(OrderMasterModel model, List<OrderDetailViewModel> detail, decimal? GuestAmountPaid, int CreateReceipt, int? PreOrderId = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        var currentTime = DateTime.Now;

                        model.CreatedDate = currentTime;
                        model.CreatedAccount = currentAccount.UserName;
                        model.OrderStatusId = EnumOrderStatus.KhoiTao;
                        AccountModel Account = _context.AccountModel.Where(p => p.UserName == model.CreatedAccount).FirstOrDefault();
                        model.CreatedEmployeeId = Account.EmployeeId;
                        model.SaleName = _context.EmployeeModel.Where(p => p.EmployeeId == model.SaleId).Select(p => p.FullName).FirstOrDefault();
                        //// Tìm giá trị STT order code
                        model.OrderCode = GetOrderCode();
                        if (model.PaymentMethodId == EnumPaymentMethod.TienMat)
                        {
                            model.Paid = GuestAmountPaid;
                            model.MoneyTransfer = 0;
                        }
                            #region PT thanh toán

                        //else if (model.PaymentMethodId == EnumPamentMethod.ChuyenKhoan)
                        //{
                        //    model.Paid = 0;
                        //    model.MoneyTransfer = GuestAmountPaid;
                        //}
                        //else if (model.PaymentMethodId == EnumPamentMethod.CongNo)
                        //{
                        //    model.Paid = 0;
                        //    model.MoneyTransfer = 0;
                        //    if (model.DebtDueDate.HasValue)
                        //    {
                        //        string infoCus = _context.CustomerModel.Where(p => p.CustomerId == model.CustomerId).Select(p => p.FullName + " - " + p.Phone).FirstOrDefault();
                        //        var Notifi = new NotificationModel()
                        //        {
                        //            NotifiType = "Nợ phải thu khách hàng",
                        //            Note = "Tới ngày hẹn thanh toán công nợ của khách hàng : " + infoCus,
                        //            AccountId = currentEmployee.EmployeeId,
                        //            CreateDate = currentTime,
                        //            EffectDate = model.DebtDueDate,
                        //            Actived = true
                        //        };
                        //        _context.Entry(Notifi).State = System.Data.Entity.EntityState.Added;
                        //        _context.SaveChanges();
                        //    }
                        //}
                            #endregion
                        else
                        {
                            return Content("Phương thức thanh toán không hợp lệ !");
                        }
                        //Thêm tổng công nợ cộng dồn = nợ cũ + nợ mới (lúc 12/8/2016)
                        //decimal? CustomerOldDebt = _context.OrderMasterModel
                        //                                  .Where(p => p.CustomerId == model.CustomerId)
                        //                                  .OrderByDescending(p => p.OrderId)
                        //                                  .Select(p => p.RemainingAmountAccrued)
                        //                                  .FirstOrDefault();
                        decimal? CustomerOldDebt = _context.AM_DebtModel
                                                         .Where(p => p.CustomerId == model.CustomerId)
                                                         .OrderByDescending(p => p.TimeOfDebt)
                                                         .Select(p => p.RemainingAmountAccrued)
                                                         .FirstOrDefault();
                        CustomerOldDebt = (CustomerOldDebt == null) ? 0 : CustomerOldDebt.Value;
                        model.RemainingAmount = (model.RemainingAmount == null) ? 0 : model.RemainingAmount.Value;
                        model.RemainingAmountAccrued = CustomerOldDebt.Value + model.RemainingAmount.Value;

                        _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges(); // LƯU TẠM ĐỂ LẤY OrderId (SẼ BỊ SCROLLBACK KHI XẢY RA LỖI)
                        if (CreateReceipt == 1)
                        {
                            #region Thêm vào giao dịch kế toán
                            AM_TransactionModel AMmodel;

                            #region TH1 : Trả đủ

                            if (model.TotalPrice == GuestAmountPaid)
                            {
                                AMmodel = new AM_TransactionModel()
                                {
                                    StoreId = model.StoreId,
                                    AMAccountId = model.PaymentMethodId == EnumPaymentMethod.TienMat ?
                                                       (_context.AM_AccountModel.Where(p => p.Code == EnumAccountCode.TM &&  p.AMAccountTypeCode == EnumAM_AccountType.TIENMAT && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault()
                                                       : (_context.AM_AccountModel.Where(p => p.Code == EnumAccountCode.NH && p.AMAccountTypeCode == EnumAM_AccountType.NGANHANG && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault(),
                                    TransactionTypeCode = EnumTransactionType.BHBAN,
                                    ContactItemTypeCode = EnumContactType.KH,
                                    CustomerId = model.CustomerId,
                                    SupplierId = null,
                                    EmployeeId = null,
                                    OtherId = null,
                                    Amount = GuestAmountPaid,
                                    OrderId = model.OrderId,
                                    ImportMasterId = null,
                                    IEOtherMasterId = null,
                                    Note = model.Note,
                                    CreateDate = currentTime,
                                    CreateEmpId = currentEmployee.EmployeeId,
                                    RemainingAmountAccrued = model.RemainingAmountAccrued
                                };
                                _context.Entry(AMmodel).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();
                            }
                            #endregion

                            #region TH2 : Không trả lưu vào công nợ
                            else if (GuestAmountPaid == 0 || GuestAmountPaid == null)
                            {
                                AMmodel = new AM_TransactionModel()
                                {
                                    StoreId = model.StoreId,
                                    AMAccountId = (_context.AM_AccountModel.Where(p => p.Code == EnumAccountCode.PTKH && p.AMAccountTypeCode == EnumAM_AccountType.CONGNO && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault(),
                                    TransactionTypeCode = EnumTransactionType.BHBAN,
                                    ContactItemTypeCode = EnumContactType.KH,
                                    CustomerId = model.CustomerId,
                                    SupplierId = null,
                                    EmployeeId = null,
                                    OtherId = null,
                                    Amount = model.TotalPrice,
                                    OrderId = model.OrderId,
                                    ImportMasterId = null,
                                    IEOtherMasterId = null,
                                    Note = model.Note,
                                    CreateDate = currentTime,
                                    CreateEmpId = currentEmployee.EmployeeId,
                                    RemainingAmountAccrued = model.RemainingAmountAccrued
                                };
                                _context.Entry(AMmodel).State = System.Data.Entity.EntityState.Added;
                                // model.PaymentMethodId == EnumPamentMethod.CongNo
                                model.PaymentMethodId = EnumPaymentMethod.CongNo;
                                _context.Entry(model).State = System.Data.Entity.EntityState.Modified;

                                _context.SaveChanges();
                            }
                            #endregion

                            #region TH3 : Trả 1 phần
                            else
                            {
                                #region 1 phần (Tiền mặt hoặc chuyển khoản)
                                AMmodel = new AM_TransactionModel()
                                {
                                    StoreId = model.StoreId,
                                    AMAccountId = model.PaymentMethodId == EnumPaymentMethod.TienMat ?
                                                       (_context.AM_AccountModel.Where(p => p.Code == EnumAccountCode.TM && p.AMAccountTypeCode == EnumAM_AccountType.TIENMAT && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault()
                                                       : (_context.AM_AccountModel.Where(p => p.Code == EnumAccountCode.NH && p.AMAccountTypeCode == EnumAM_AccountType.NGANHANG && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault(),
                                    TransactionTypeCode = EnumTransactionType.BHBAN,
                                    ContactItemTypeCode = EnumContactType.KH,
                                    CustomerId = model.CustomerId,
                                    SupplierId = null,
                                    EmployeeId = null,
                                    OtherId = null,
                                    Amount = GuestAmountPaid,
                                    OrderId = model.OrderId,
                                    ImportMasterId = null,
                                    IEOtherMasterId = null,
                                    Note = model.Note,
                                    CreateDate = currentTime,
                                    CreateEmpId = currentEmployee.EmployeeId,
                                    RemainingAmountAccrued = model.RemainingAmountAccrued
                                };
                                _context.Entry(AMmodel).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();
                                #endregion

                                #region 1 phần đưa vào công nợ
                                AMmodel = new AM_TransactionModel()
                                {
                                    StoreId = model.StoreId,
                                    AMAccountId = (_context.AM_AccountModel.Where(p => p.Code == EnumAccountCode.PTKH && p.AMAccountTypeCode == EnumAM_AccountType.CONGNO && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault(),
                                    TransactionTypeCode = EnumTransactionType.BHBAN,
                                    ContactItemTypeCode = EnumContactType.KH,
                                    CustomerId = model.CustomerId,
                                    SupplierId = null,
                                    EmployeeId = null,
                                    OtherId = null,
                                    Amount = model.TotalPrice - GuestAmountPaid,
                                    OrderId = model.OrderId,
                                    ImportMasterId = null,
                                    IEOtherMasterId = null,
                                    Note = model.Note,
                                    CreateDate = currentTime,
                                    CreateEmpId = currentEmployee.EmployeeId,
                                    RemainingAmountAccrued = model.RemainingAmountAccrued
                                };
                                _context.Entry(AMmodel).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();
                                #endregion
                            }
                            #endregion



                            #endregion
                        }

                        #region Thêm AM_DebtModel (Số nợ còn lại)
                        if (model.RemainingAmount > 0)
                        {
                            var AMDebModel = new AM_DebtModel()
                            {
                                CustomerId = model.CustomerId,
                                TimeOfDebt = currentTime,
                                RemainingAmountAccrued = model.RemainingAmountAccrued,
                                OrderId = model.OrderId,
                                TransactionTypeCode = EnumTransactionType.BHBAN
                            };
                            _context.Entry(AMDebModel).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                        }
                        #endregion

                        // Insert InventoryMaster
                        InventoryMasterModel InvenMaster = new InventoryMasterModel();
                        InvenMaster.WarehouseModelId = model.WarehouseId;
                        InvenMaster.InventoryTypeId = EnumInventoryType.XB; //  bán hàng
                        InvenMaster.InventoryCode = model.OrderCode;
                        InvenMaster.CreatedDate = currentTime;
                        InvenMaster.CreatedAccount = model.CreatedAccount;
                        InvenMaster.CreatedEmployeeId = model.CreatedEmployeeId;
                        InvenMaster.StoreId = model.StoreId;
                        InvenMaster.Actived = true;
                        InvenMaster.BusinessId = model.OrderId; // Id nghiệp vụ 
                        InvenMaster.BusinessName = "OrderMasterModel";// Tên bảng nghiệp vụ
                        InvenMaster.ActionUrl = "/Sell/Details/";// Đường dẫn ( cộng ID cho truy xuất)
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
                                #region Thêm giá vốn
                                decimal? COGS = _context.ProductModel.Where(p => p.ProductId == item.ProductId).Select(p => p.COGS).FirstOrDefault();
                                item.COGS = COGS?? 0;
                                #endregion
                                EndInventoryRepository EndInventoryRepo = new EndInventoryRepository(_context);
                                decimal TonCuoiTrongHeThong = EndInventoryRepo.GetQty(item.ProductId.Value);

                                if (item.Quantity > TonCuoiTrongHeThong)
                                {
                                    return Content(string.Format("Vui lòng chọn số lượng sản phẩm ' {0} ' nhỏ hơn hoặc bằng trong kho", item.ProductName));
                                }
                                OrderDetailModel detailmodel = new OrderDetailModel()
                                {
                                    OrderId = model.OrderId,
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

                                InventoryDetailModel InvenDetail = new InventoryDetailModel()
                                {
                                    InventoryMasterId = InvenMaster.InventoryMasterId,
                                    ProductId = item.ProductId,
                                    BeginInventoryQty = TonCuoiTrongHeThong,
                                    //COGS = 0,
                                    Price = item.Price,
                                    //ImportQty = 0, // số lượng nhập = 0
                                    ExportQty = item.Quantity,
                                    //UnitCOGS = 0, //Tổng giá vốn = [COGS] * [ImportQty] = 0
                                    UnitPrice = item.Quantity * item.Price, //[ExportQty] *[Price]
                                    EndInventoryQty = TonCuoiTrongHeThong + 0 - item.Quantity //Tồn cuối = [BeginInventoryQty] + [ImportQty] -  [ExportQty]
                                };
                                _context.Entry(InvenDetail).State = System.Data.Entity.EntityState.Added;
                                // model.OrderDetailModel.Add(detailmodel);
                            }
                            // Cập nhật lại Tổng giá vốn 
                            model.SumCOGSOfOrderDetail = detail.Sum(p => p.COGS * p.Quantity);
                            _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                            // Nếu là xác nhận yêu cầu đơn đặt hàng của khách hàng
                            if (PreOrderId.HasValue)
                            {
                                var PreOderModel = _context.PreOrderMasterModel.Where(p => p.PreOrderId == PreOrderId).FirstOrDefault();
                                PreOderModel.StatusCode = "DADUYET";
                                PreOderModel.OrderId = model.OrderId;
                                _context.Entry(PreOderModel).State = System.Data.Entity.EntityState.Modified;
                            }
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
            catch (Exception ex)
            {
                return Content("Xảy ra lỗi trong quá trình thêm mới đơn hàng");
            }

        }
        #endregion

        #region Edit OrderMasterModel
        
        public ActionResult Edit(int id)
        {
            OrderMasterModel model = _context.OrderMasterModel
                                    .Include(p => p.OrderDetailModel)
                                    .Where(p => p.OrderId == id).FirstOrDefault();
            if (model.ProvinceId.HasValue)
            {
                ViewBag.ProvinceName = model.ProvinceModel.ProvinceName;
                ViewBag.DistrictName = model.DistrictModel.Appellation + " " + model.DistrictModel.DistrictName;
            }

            ViewBag.IdCustomer = model.CustomerId;
            ViewBag.IdProvince = model.ProvinceId;
            ViewBag.IdDistrict = model.DistrictId;
            ViewBag.OrderDetailList = model.OrderDetailModel.Select(p => new OrderDetailViewModel()
            {
                OrderDetailId = p.OrderDetailId,
                OrderId = p.OrderId,
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
        #endregion

        #region Xử lý btnUpdate
         
        public ActionResult Update(OrderMasterModel model, List<OrderDetailViewModel> detail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //_context.Entry(model).State = System.Data.Entity.EntityState.Modified; Chỉ Modified những filed cần thiết
                    OrderMasterModel OrderMastermodel = _context.OrderMasterModel
                                                                    .Include(p => p.OrderDetailModel)
                                                                    .Where(p => p.OrderId == model.OrderId).FirstOrDefault();
                    // Xoá những Detail cũ
                    if (OrderMastermodel.OrderDetailModel != null && OrderMastermodel.OrderDetailModel.Count > 0)
                    {
                        while (OrderMastermodel.OrderDetailModel.Count > 0)
                        {
                            _context.Entry(OrderMastermodel.OrderDetailModel.First()).State = System.Data.Entity.EntityState.Deleted;
                        }
                    }

                    #region // Update lại những filed cần Modified cho Model Master
                    OrderMastermodel.OrderId = model.OrderId;
                    OrderMastermodel.WarehouseId = model.WarehouseId;
                    OrderMastermodel.BillDiscountTypeId = model.BillDiscountTypeId;
                    OrderMastermodel.BillDiscount = model.BillDiscount;
                    OrderMastermodel.BillVAT = model.BillVAT;
                    OrderMastermodel.SaleName = model.SaleName;
                    OrderMastermodel.DebtDueDate = model.DebtDueDate;
                    OrderMastermodel.PaymentMethodId = model.PaymentMethodId;
                    OrderMastermodel.Paid = model.Paid;
                    OrderMastermodel.MoneyTransfer = model.MoneyTransfer;
                    OrderMastermodel.CompanyName = model.CompanyName;
                    OrderMastermodel.TaxBillCode = model.TaxBillCode;
                    OrderMastermodel.ContractNumber = model.ContractNumber;
                    OrderMastermodel.TaxBillDate = model.TaxBillDate;
                    OrderMastermodel.CustomerId = model.CustomerId;
                    OrderMastermodel.FullName = model.FullName;
                    OrderMastermodel.IdentityCard = model.IdentityCard;
                    OrderMastermodel.FullName = model.FullName;
                    OrderMastermodel.Phone = model.Phone;
                    OrderMastermodel.FullName = model.FullName;
                    OrderMastermodel.Gender = model.Gender;
                    OrderMastermodel.ProvinceId = model.ProvinceId;
                    OrderMastermodel.DistrictId = model.DistrictId;
                    OrderMastermodel.Address = model.Address;
                    OrderMastermodel.Email = model.Email;
                    OrderMastermodel.Note = model.Note;
                    OrderMastermodel.TotalPrice = model.TotalPrice;
                    OrderMastermodel.LastModifiedDate = DateTime.Now;
                    OrderMastermodel.LastModifiedAccount = currentAccount.UserName;
                    OrderMastermodel.OrderStatusId = EnumOrderStatus.KhoiTao;
                    #endregion
                    // đánh dấu sửa OrderMastermodel
                    _context.Entry(OrderMastermodel).State = System.Data.Entity.EntityState.Modified;

                    if (detail != null)
                    {
                        foreach (var item in detail)
                        {
                            OrderDetailModel detailmodel = new OrderDetailModel()
                            {
                                OrderDetailId = item.OrderDetailId,
                                OrderId = model.OrderId,
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
                    return Content("success");
                }
                else
                {
                    return Content("Vui lòng kiểm tra lại thông tin không hợp lệ");
                }
            }
            catch (Exception ex)
            {
                return Content("Lỗi hệ thống" + ex.Message );
            }

        }
        #endregion

        #region Detail
        
        public ActionResult Details(int id)
        {
            OrderMasterModel model = _context.OrderMasterModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.StoreName = _context.StoreModel.Where(p => p.StoreId == model.StoreId).Select(p => p.StoreName).FirstOrDefault();
            ViewBag.WarhouseName = _context.WarehouseModel.Where(p => p.WarehouseId == model.WarehouseId).Select(p => p.WarehouseName).FirstOrDefault();
            ViewBag.CustomerLevelName = _context.CustomerLevelModel.Where(p => p.CustomerLevelId == model.CustomerLevelId).Select(p => p.CustomerLevelName).FirstOrDefault();
            return View(model);
        }
        #endregion

        public ActionResult _DetailList(List<OrderDetailModel> model, decimal BillDiscount, int BillDiscountTypeId, int BillVAT, decimal TotalPrice, string PaymentMethod, decimal RemainingAmount, decimal SumPrice)
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
        #region UpdateOrderCode
        public ActionResult UpdateOrderCode()
        {
            string OrderCode = GetOrderCode();
            return Json(OrderCode, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Hàm GetOrdercode
        public string GetOrderCode()
        {
            // Tìm giá trị STT order code
            string OrderCodeToFind = string.Format("{0}-{1}{2}", "PBH", (DateTime.Now.Year).ToString().Substring(2), (DateTime.Now.Month).ToString().Length == 1 ? "0" + (DateTime.Now.Month).ToString() : (DateTime.Now.Month).ToString());
            var Resuilt = _context.OrderMasterModel.OrderByDescending(p => p.OrderId).Where(p => p.OrderCode.Contains(OrderCodeToFind)).Select(p => p.OrderCode).FirstOrDefault();
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