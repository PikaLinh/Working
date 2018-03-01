using Constant;
using EntityModels;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using ViewModels;


namespace WebUI.Controllers
{
    public class OrderReturnMasterController : BaseController
    {
        //
        // GET: /OrderReturnMaster/
        #region Danh sách đơn khách hàng trả hàng
        
        public ActionResult Index()
        {
            var list = (from p in _context.OrderReturnModel
                        join pp in _context.OrderMasterModel on p.OrderId equals pp.OrderId
                        where (p.Actived == true && pp.Actived == true)
                        orderby p.OrderReturnMasterId descending
                        select new OrderReturnMasterViewModel()
                        {
                            OrderReturnMasterId = p.OrderReturnMasterId,
                            OrderCode = pp.OrderCode,
                            FullName = pp.FullName,
                            Phone = pp.Phone,
                            SaleName = pp.SaleName,
                            CreatedDate = p.CreatedDate,
                            TotalPrice = p.TotalPrice
                        }).ToList();
            //return View(_context.OrderReturnModel.Where(p => p.Actived == true).OrderByDescending(p => p.OrderReturnMasterId).ToList());
            return View(list);
        }
        public ActionResult _SearchOrderReturnMaster(SellSearchViewModel model, int PageIndex = 1, int PageSize = 10)
        {
            if (model.ToDate.HasValue)
            {
                model.ToDate.Value.AddDays(1).AddMilliseconds(-1);
            }

            var list = (from p in _context.OrderReturnModel
                    join pp in _context.OrderMasterModel on p.OrderId equals pp.OrderId
                    where p.Actived == true && pp.Actived == true &&
                    (model.CustomerId == null || pp.CustomerId == model.CustomerId) &&
                    (model.Phone == null || pp.Phone.Contains(model.Phone)) &&
                    (model.OrderId == null || p.OrderId == model.OrderId) &&
                    (model.FromDate == null || p.CreatedDate.Value.CompareTo(model.FromDate.Value) >= 0) &&
                    (model.ToDate == null || p.CreatedDate.Value.CompareTo(model.ToDate.Value) <= 0) &&
                    (model.FromTotalPrice == null || p.TotalPrice >= model.FromTotalPrice) &&
                    (model.ToTotalPrice == null || p.TotalPrice <= model.ToTotalPrice)
                    select new OrderReturnMasterViewModel()
                    {
                        OrderReturnMasterId = p.OrderReturnMasterId,
                        OrderCode = pp.OrderCode,
                        FullName = pp.FullName,
                        Phone = pp.Phone,
                        SaleName = pp.SaleName,
                        CreatedDate = p.CreatedDate,
                        TotalPrice = p.TotalPrice
                    }).Distinct()
                    .OrderByDescending(p => p.OrderReturnMasterId);
            ViewBag.TotalRow = list.Count();
            ViewBag.RowIndex = (PageIndex - 1) * PageSize;
            return PartialView(list.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList());
        }
        #endregion

        #region Thêm mới OderReturnMaster
        
        public ActionResult Create()
        {
           
            OrderReturnMasterViewModel model = new OrderReturnMasterViewModel() { Actived = true };

            ////model.SalemanName =
            //model.CreatedAccount = currentAccount.UserName;
            //AccountModel Account = _context.AccountModel.Where(p => p.UserName == model.CreatedAccount).FirstOrDefault();
            //model.SalemanName = Account.EmployeeModel.FullName;
            //ViewBag.SalemanName = model.SalemanName;
            CreateViewBag();
            // Tìm giá trị STT ReturnMaster code
            ViewBag.ReturnCode = GetReturnCode();
            return View(model);
        }
        #endregion

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
        
        #region GetOrderId
        public ActionResult GetOrderId(string q, int? StoreId, int? WarehouseId)
        {
            var data2 = _context.OrderMasterModel
                       .Where(p => (q == null || (p.OrderCode.Contains(q))) &&
                       (StoreId == null || p.StoreId == StoreId) &&
                       (WarehouseId == null || p.WarehouseId == WarehouseId) &&
                        p.Actived == true)
                       .Select(p => new
                       {
                           value = p.OrderId,
                           text = p.OrderCode
                       }).Take(10).ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GetListDetailByOrderID
        public ActionResult GetListDetailByOrderID(int? OrderID)
        {
            try
            {
                // Lấy danh sách OrderDetailList theo OrderID
                var idParam = new SqlParameter
                {
                    ParameterName = "OrderID",
                    Value = OrderID,
                    SqlDbType = System.Data.SqlDbType.Int
                };
                var detail = _context.Database.
                                               SqlQuery<OrderReturnDetailViewModel>("dbo.usp_OrderReturnDetailList @OrderID", idParam)
                                               .ToList();
                return PartialView("_CreatelistInner", detail);
            }
            catch
            {
                var detail = new List<ReturnDetailViewModel>();
                return PartialView("_CreatelistInner", detail);
            }

        }
        #endregion

        #region Get3FieldByOrderID
        public ActionResult Get3FieldByOrderID(int? OrderID)
        {
            var data = (from p in  _context.OrderMasterModel
                       join cusle in _context.CustomerLevelModel on p.CustomerLevelId equals cusle.CustomerLevelId

                       join prov in _context.ProvinceModel on p.ProvinceId equals prov.ProvinceId into provTemp
                       from ptempt in provTemp.DefaultIfEmpty()

                        join distr in _context.DistrictModel on p.DistrictId equals distr.DistrictId into DistrictId
                       from ptempt2 in DistrictId.DefaultIfEmpty()

                       where( p.OrderId == OrderID)
                       select  new
                        {
                            StoreId = p.StoreId,
                            WarehouseId = p.WarehouseId,
                            CustomerId = p.FullName,
                            CustomerLevelId = cusle.CustomerLevelName,
                            IdentityCard = p.IdentityCard,
                            Phone = p.Phone,
                            Gender = p.Gender == true?"Nam": "Nữ",
                            Email = p.Email,
                            ProvinceId = string.IsNullOrEmpty(ptempt.ProvinceName) ? "" : ptempt.ProvinceName,
                            DistrictId = string.IsNullOrEmpty(ptempt2.DistrictName) ? "" : ptempt2.DistrictName,
                            Address = p.Address,
                            SaleId = p.SaleName,
                            CompanyName = p.CompanyName,
                            TaxBillCode = p.TaxBillCode,
                            ContractNumber = p.ContractNumber,
                            TaxBillDate = p.TaxBillDate,
                         //   Note = p.Note,
                            DebtDueDate = p.DebtDueDate,
                            BillDiscount = p.BillDiscount,
                            BillDiscountTypeId = p.BillDiscountTypeId,
                            BillVAT = p.BillVAT
                        }).FirstOrDefault();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult _CreateList(List<OrderReturnDetailViewModel> detail = null)
        {

            if (detail == null)
            {
                detail = new List<OrderReturnDetailViewModel>();
            }

            return PartialView(detail);
        }

        public ActionResult _CreatelistInner(List<OrderReturnDetailViewModel> detail = null)
        {
            if (detail == null)
            {
                detail = new List<OrderReturnDetailViewModel>();
            }
            //ReturnDetailViewModel item = new ReturnDetailViewModel();
            ////CreateDetailViewBag();
            return PartialView(detail);
        }

        #region Save
         
        public ActionResult Save(OrderReturnModel model, List<OrderReturnDetailViewModel> detail, decimal? GuestAmountPaid, int CreateReceipt)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        var currentTime = DateTime.Now;
                        #region Thêm vào OrderReturnModel
                        model.CreatedDate = currentTime;
                        model.CreatedAccount = currentAccount.UserName;
                        AccountModel Account = _context.AccountModel.Where(p => p.UserName == model.CreatedAccount).FirstOrDefault();
                        model.CreatedEmployeeId = Account.EmployeeId;
                        //// Tìm giá trị STT order code
                        model.OrderReturnMasterCode = GetReturnCode();
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
                       
                        #endregion

                        var orderModel = _context.OrderMasterModel.Where(p => p.OrderId == model.OrderId).FirstOrDefault();

                        #region Tính số dư còn lại
                        decimal? CustomerOldDebt = _context.AM_DebtModel
                                                        .Where(p => p.CustomerId == orderModel.CustomerId)
                                                        .OrderByDescending(p => p.TimeOfDebt)
                                                        .Select(p => p.RemainingAmountAccrued)
                                                        .FirstOrDefault();
                        CustomerOldDebt = (CustomerOldDebt == null) ? 0 : CustomerOldDebt.Value;
                        model.RemainingAmount = (model.RemainingAmount == null) ? 0 : model.RemainingAmount.Value;
                        model.RemainingAmountAccrued = CustomerOldDebt.Value - model.RemainingAmount.Value;

                        _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges(); // LƯU TẠM ĐỂ LẤY OrderId (SẼ BỊ SCROLLBACK KHI XẢY RA LỖI)
                        #endregion

                        if (CreateReceipt == 1)
                        {
                            #region Thêm vào giao dịch kế toán
                            AM_TransactionModel AMmodel;
                            int MaKH = (_context.OrderMasterModel.Where(p => p.OrderId == model.OrderId).Select(p => p.CustomerId).FirstOrDefault());
                            #region TH1 : Trả đủ

                            if (model.TotalPrice == GuestAmountPaid)
                            {
                                AMmodel = new AM_TransactionModel()
                                {
                                    StoreId = model.StoreId,
                                    AMAccountId = model.PaymentMethodId == EnumPamentMethod.TienMat ?
                                                       (_context.AM_AccountModel.Where(p => p.Code == EnumAccountCode.TM && p.AMAccountTypeCode == EnumAM_AccountType.TIENMAT && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault()
                                                       : (_context.AM_AccountModel.Where(p => p.Code == EnumAccountCode.NH && p.AMAccountTypeCode == EnumAM_AccountType.NGANHANG && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault(),
                                    TransactionTypeCode = EnumTransactionType.BHTRA,
                                    ContactItemTypeCode = EnumContactType.KH,
                                    CustomerId = MaKH,
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
                                    TransactionTypeCode = EnumTransactionType.BHTRA,
                                    ContactItemTypeCode = EnumContactType.KH,
                                    CustomerId = MaKH,
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
                                model.PaymentMethodId = EnumPamentMethod.CongNo;
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
                                    AMAccountId = model.PaymentMethodId == EnumPamentMethod.TienMat ?
                                                       (_context.AM_AccountModel.Where(p => p.Code == EnumAccountCode.TM && p.AMAccountTypeCode == EnumAM_AccountType.TIENMAT && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault()
                                                       : (_context.AM_AccountModel.Where(p => p.Code == EnumAccountCode.NH && p.AMAccountTypeCode == EnumAM_AccountType.NGANHANG && p.StoreId == model.StoreId).Select(p => p.AMAccountId)).FirstOrDefault(),
                                    TransactionTypeCode = EnumTransactionType.BHTRA,
                                    ContactItemTypeCode = EnumContactType.KH,
                                    CustomerId = MaKH,
                                    SupplierId = null,
                                    EmployeeId = currentEmployee.EmployeeId,
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
                                    TransactionTypeCode = EnumTransactionType.BHTRA,
                                    ContactItemTypeCode = EnumContactType.KH,
                                    CustomerId = MaKH,
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
                                CustomerId = orderModel.CustomerId,
                                TimeOfDebt = currentTime,
                                RemainingAmountAccrued = model.RemainingAmountAccrued,
                                OrderReturnId = model.OrderReturnMasterId,
                                TransactionTypeCode = EnumTransactionType.BHTRA
                            };
                            _context.Entry(AMDebModel).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                        }
                        #endregion

                        #region Thêm vào InventoryMaster
                        // Insert InventoryMaster
                        InventoryMasterModel InvenMaster = new InventoryMasterModel();
                        InvenMaster.WarehouseModelId = model.WarehouseId;
                        InvenMaster.InventoryTypeId = EnumInventoryType.NB;// Nhập - Khách trả hàng
                        InvenMaster.InventoryCode = model.OrderReturnMasterCode;
                        InvenMaster.CreatedDate = model.CreatedDate;
                        InvenMaster.CreatedAccount = model.CreatedAccount;
                        InvenMaster.CreatedEmployeeId = model.CreatedEmployeeId;
                        InvenMaster.StoreId = model.StoreId;
                        InvenMaster.Actived = true;
                        InvenMaster.BusinessId = model.OrderId; // Id nghiệp vụ 
                        InvenMaster.BusinessName = "OrderReturnMaster";// Tên bảng nghiệp vụ
                        InvenMaster.ActionUrl = "/OrderReturnMaster/Details/";// Đường dẫn ( cộng ID cho truy xuất)
                        _context.Entry(InvenMaster).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges(); // insert tạm để lấy InvenMasterID
                        #endregion

                        #region duyệt list lưu OrderReturnDetailModel và InvenrotyDetail
                        decimal TotalQty = 0;
                        foreach (var item in detail)
                        {
                            if (item.ReturnQuantity > 0) // Chỉ tính Số lượng trả > 0
                            {
                                TotalQty += item.ReturnQuantity.Value;
                                #region Lưu OrderReturnDetailModel
                                decimal CogsInOd = _context.OrderDetailModel.Where(p => p.OrderId == model.OrderId && p.ProductId == item.ProductId).Select(p => p.COGS.Value).FirstOrDefault();
                                item.COGS = CogsInOd;
                                OrderReturnDetailModel detailmodel = new OrderReturnDetailModel()
                                {
                                    OrderReturnId = model.OrderReturnMasterId,
                                    ProductId = item.ProductId,
                                    SellQuantity = item.SellQuantity,
                                    ReturnedQuantity = item.ReturnQuantity, // cần xem lại
                                    ReturnQuantity = item.ReturnQuantity,
                                    Price = item.Price,
                                    UnitPrice = item.UnitPrice,
                                    Note = item.Note,
                                    ReturnReason = item.ReturnReason,
                                    COGS = item.COGS
                                };
                                _context.Entry(detailmodel).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();
                                #endregion

                                #region Lưu InventoryDetail
                                // tính tồn đầu
                                EndInventoryRepository EndInventoryRepo = new EndInventoryRepository(_context);
                                decimal tondau = EndInventoryRepo.GetQty(item.ProductId.Value);

                                InventoryDetailModel InvenDetail = new InventoryDetailModel()
                                {
                                    InventoryMasterId = InvenMaster.InventoryMasterId,
                                    ProductId = item.ProductId,
                                    BeginInventoryQty = tondau,
                                    COGS = CogsInOd,
                                    //Price = item.Price,
                                    ImportQty = item.ReturnQuantity,
                                    //ExportQty = 0, 
                                    UnitCOGS = item.ReturnQuantity * CogsInOd,
                                    //UnitPrice = item.ReturnQuantity * item.Price, //[ImportQty] *[Price]
                                    EndInventoryQty = tondau + item.ReturnQuantity//Tồn cuối = [BeginInventoryQty] + [ImportQty] -  [ExportQty]
                                };
                                _context.Entry(InvenDetail).State = System.Data.Entity.EntityState.Added;

                                // _context.SaveChanges();

                                #endregion
                            }
                        }
                        #endregion

                        // Cập nhật lại Tổng giá vốn 
                        model.SumCOGSOfOrderDetail = detail.Where(p => p.ReturnQuantity > 0 ).Sum(p => p.COGS * p.ReturnQuantity);
                        model.TotalQty = TotalQty;
                        _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
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
            catch
            {
                return Content("Xảy ra lỗi trong quá trình thêm mới đơn hàng");
            }
        }
        #endregion

        #region Huỷ đơn hàng
        
        public ActionResult Cancel(int id)
        {
            OrderReturnModel model = _context.OrderReturnModel
                                             .Where(p => p.OrderReturnMasterId == id)
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
                        cmd.CommandText = "usp_SellReturnCanceled";
                        cmd.Parameters.AddWithValue("@OrderReturnMasterId", model.OrderReturnMasterId);
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

        #region Detail
        
        public ActionResult Details(int id)
        {
            //ReturnMasterModel model = _context.ReturnMasterModel.Find(id);
            var model = (from p in _context.OrderReturnModel
                         join pp in _context.OrderMasterModel on p.OrderId equals pp.OrderId
                         join sm in _context.StoreModel on p.StoreId equals sm.StoreId
                         join wm in _context.WarehouseModel on p.WarehouseId equals wm.WarehouseId

                         join cusle in _context.CustomerLevelModel on pp.CustomerLevelId equals cusle.CustomerLevelId

                         join prov in _context.ProvinceModel on pp.ProvinceId equals prov.ProvinceId into provTemp
                         from ptempt in provTemp.DefaultIfEmpty()

                         join distr in _context.DistrictModel on pp.DistrictId equals distr.DistrictId into DistrictId
                         from ptempt2 in DistrictId.DefaultIfEmpty()

                         where (p.OrderReturnMasterId == id && p.Actived == true)
                         select new OrderReturnMasterViewModel()
                         {
                            OrderReturnMasterId = p.OrderReturnMasterId,
                            OrderReturnMasterCode = p.OrderReturnMasterCode,
                            StoreName = sm.StoreName,
                            WarehouseName = wm.WarehouseName,
                            OrderCode = pp.OrderCode,
                            CustomerName = pp.FullName,
                            CustomerLevelName = cusle.CustomerLevelName,
                            IdentityCard = pp.IdentityCard,
                            Phone = pp.Phone,
                            Gender2 = pp.Gender == true ? "Nam" : "Nữ",
                            Email = pp.Email,
                            ProvinceName = string.IsNullOrEmpty(ptempt.ProvinceName) ? "" : ptempt.ProvinceName,
                            DistrictName = string.IsNullOrEmpty(ptempt2.DistrictName) ? "" : ptempt2.DistrictName,
                            Address = pp.Address,
                            SaleName = pp.SaleName,
                            CompanyName = pp.CompanyName,
                            TaxBillCode = pp.TaxBillCode,
                            ContractNumber = pp.ContractNumber,
                            TaxBillDate = pp.TaxBillDate,
                            //   Note = p.Note,
                            DebtDueDate = pp.DebtDueDate,
                            Note = p.Note,

                            SumPrice = p.OrderReturnDetailModel.Sum(s => s.UnitPrice),
                            BillDiscount = p.BillDiscount,
                            BillDiscountTypeId = p.BillDiscountTypeId,
                            BillVAT = p.BillVAT,
                            TotalPrice = p.TotalPrice,
                            PaymentMethodId = p.PaymentMethodId,
                            Paid = p.Paid.HasValue ? p.Paid.Value : 0,
                            MoneyTransfer = p.MoneyTransfer.HasValue? p.MoneyTransfer.Value : 0,
                            RemainingAmount = p.RemainingAmount
                         }).FirstOrDefault();
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        public ActionResult _DetailList(int OrderReturnMasterId)
        {
            //List<OrderReturnDetailModel> model = _context.OrderReturnDetailModel.Where(p => p.OrderReturnId == OrderReturnMasterId).ToList();
            var lst = (from p in _context.OrderReturnDetailModel
                       join prod in _context.ProductModel on p.ProductId equals prod.ProductId
                       where (p.OrderReturnId == OrderReturnMasterId)
                       select new OrderReturnDetailViewModel()
                       {
                           ProductId = p.ProductId,
                           ProductName = prod.ProductName,
                           Price = p.Price,
                           SellQuantity = p.SellQuantity,
                           ReturnedQuantity = p.ReturnedQuantity,
                           ReturnQuantity = p.ReturnQuantity,
                           UnitPrice = p.UnitPrice,
                           Note = p.Note,
                           ReturnReason = p.ReturnReason
                       }).ToList();
            if (lst == null)
            {
                lst = new List<OrderReturnDetailViewModel>();
            }
            return PartialView(lst);
        }
        #endregion

        #region Healper
        private string GetReturnCode()
        {
            // Tìm giá trị STT ReturnMaster code
            string ReturnCodeToFind = string.Format("{0}-{1}{2}", "PTH", (DateTime.Now.Year).ToString().Substring(2), (DateTime.Now.Month).ToString().Length == 1 ? "0" + (DateTime.Now.Month).ToString() : (DateTime.Now.Month).ToString());
            var ResuiltFound = _context.OrderReturnModel.OrderByDescending(p => p.OrderReturnMasterId).Where(p => p.OrderReturnMasterCode.Contains(ReturnCodeToFind)).Select(p => p.OrderReturnMasterCode).FirstOrDefault();
            string ResuiltFinal = "";
            if (ResuiltFound != null)
            {
                int LastNumber = Convert.ToInt32(ResuiltFound.Substring(9)) + 1;
                string STT = "";
                switch (LastNumber.ToString().Length)
                {
                    case 1: STT = "000" + LastNumber.ToString(); break;
                    case 2: STT = "00" + LastNumber.ToString(); break;
                    case 3: STT = "0" + LastNumber.ToString(); break;
                    default: STT = LastNumber.ToString(); break;
                }
                //ViewBag.ReturnCode = string.Format("{0}{1}", Resuilt.Substring(0, 9), STT);
                ResuiltFinal = string.Format("{0}{1}", ResuiltFound.Substring(0, 9), STT);
            }
            else
            {
                ResuiltFinal = string.Format("{0}-{1}", ReturnCodeToFind, "0001");
            }

            return ResuiltFinal;
        }
        #endregion

    }
}
