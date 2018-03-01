using Constant;
using EntityModels;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using ViewModels;


namespace WebUI.Controllers
{
    public class PreImportMasterController : BaseController
    {
        //
        // GET: /PreImportMasterModel/
        #region Danh sách đơn yêu cầu đặt hàng NCC
        
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }

        public ActionResult _SearchPreImportMaster(PreImportMasterSearchViewModel model, int PageIndex = 1, int PageSize = 10)
        {
            //Danh sách
            if (model.ToDate.HasValue)
            {
                model.ToDate.Value.AddDays(1);
            }
            //Tim kiếm
            var list = (from p in _context.PreImportMasterModel

                    join sm in _context.SupplierModel on p.SupplierId equals sm.SupplierId into smList
                    from pp in smList.DefaultIfEmpty()

                    join wh in _context.WarehouseModel on p.WarehouseId equals wh.WarehouseId
                    join id in _context.PreImportDetailModel on p.PreImportMasterId equals id.PreImportMasterId
                    join pd in _context.ProductModel on id.ProductId equals pd.ProductId
                    where (model.SupplierId == null || p.SupplierId == model.SupplierId) &&
                    (model.WarehouseId == null || p.WarehouseId == model.WarehouseId) &&
                    (model.ProductId == null || pd.ProductId == model.ProductId) &&
                    (model.PreImportMasterId == null || p.PreImportMasterId == model.PreImportMasterId) &&
                    (model.FromDate == null || p.CreatedDate.Value.CompareTo(model.FromDate.Value) >= 0) &&
                    (model.ToDate == null || p.CreatedDate.Value.CompareTo(model.ToDate.Value) <= 0) &&
                    (model.FromTotalPrice == null || p.TotalPrice >= model.FromTotalPrice) &&
                    (model.ToTotalPrice == null || p.TotalPrice <= model.ToTotalPrice) 
               //     && p.Actived == true
                    select new PreImportMasterInfoViewModel()
                    {
                        CreatedDate = p.CreatedDate,
                        PreImportMasterId = p.PreImportMasterId,
                        PreImportMasterCode = p.PreImportMasterCode,
                        SupplierName = pp.SupplierName,
                        WarehouseName = wh.WarehouseCode,
                        SalemanName = p.SalemanName,
                        TotalQty = p.TotalQty,
                        TotalPrice = p.TotalPrice,
                        StatusCode = p.StatusCode,
                        Actived = p.Actived
                    }).Distinct()
                    .OrderByDescending(p => p.CreatedDate);
            ViewBag.TotalRow = list.Count();
            ViewBag.RowIndex = (PageIndex - 1) * PageSize;
            return PartialView(list.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList());
        }

        public ActionResult Cancel(int id)
        {
            try
            {
                PreImportMasterModel model = _context.PreImportMasterModel
                                                     .Where(p => p.PreImportMasterId == id)
                                                     .FirstOrDefault();
                if (model == null)
                {
                    return Json("Không tìm thấy đơn hàng yêu cầu !", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.Actived = false;
                    model.StatusCode = "HUY";
                    _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(string.Format(Resources.LanguageResource.CancelErrorMessenge, "Yêu cầu đặt đơn hàng nhà cung cấp"), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPreImportMasterId(string q)
        {
            var data2 = _context
                        .PreImportMasterModel
                        .Where(p => q == null || (p.PreImportMasterCode).Contains(q))
                        .Select(p => new
                        {
                            value = p.PreImportMasterId,
                            text = p.PreImportMasterCode
                        })
                        .Take(10)
                        .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Thêm mới phiếu yêu cầu đặt hàng
        
        public ActionResult Create()
        {

            PreImportMasterModel model = new PreImportMasterModel() { Actived = true };

            model.CreatedAccount = currentAccount.UserName;
            model.SalemanName = currentEmployee.FullName;
            ViewBag.SalemanName = model.SalemanName;
            CreateViewBag(null, null, null, null, 1);
            ViewBag.ImportCode = GetPreImportMasterCode();
            return View(model);
        }
        

        #region _CreateList
        public ActionResult _CreateList(List<PreImportDetailViewModel> detail = null)
        {

            if (detail == null)
            {
                detail = new List<PreImportDetailViewModel>();
            }

            return PartialView(detail);
        }
        #endregion

        #region _CreatelistInner
        public ActionResult _CreatelistInner(List<PreImportDetailViewModel> detail = null, string scanBarcode = null, int? ProQty = 1)
        {
            if (detail == null)
            {
                detail = new List<PreImportDetailViewModel>();
            }
            if (string.IsNullOrEmpty(scanBarcode))
            {
                PreImportDetailViewModel item = new PreImportDetailViewModel();
                detail.Add(item);
            }
            else  // Quét mã vạch
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
                                itemm.Qty += ProQty;
                                itemm.UnitPrice += ((itemm.Price + itemm.ShippingFee) * ProQty);
                                return PartialView(detail);
                            }
                        }
                        #endregion

                        PreImportDetailViewModel item = new PreImportDetailViewModel()
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductCode + " | " + product.ProductName,
                            Price = product.ImportPrice,
                            ShippingFee = product.ShippingFee,
                            Qty = ProQty,
                            UnitPrice = (product.ImportPrice + product.ShippingFee) * ProQty
                        };
                        detail.Add(item);
                    }
                }
                #endregion
            }
            return PartialView(detail);
        }

        #endregion

        #region _DeletelistInner
        public ActionResult _DeletelistInner(List<PreImportDetailViewModel> detail, int RemoveId)
        {
            if (detail == null)
            {
                detail = new List<PreImportDetailViewModel>();
            }
            return PartialView("_CreatelistInner", detail.Where(p => p.STT != RemoveId).ToList());
        }
        #endregion


        private void CreateDetailViewBag(int? ProductId = null)
        {
            var ProductList = _context.ProductModel.OrderBy(p => p.ProductName).ToList();
            ViewBag.ProductId = ProductList;
        }

        public ActionResult Save(PreImportMasterModel model, List<PreImportDetailViewModel> detail, decimal TotalShippingWeight, decimal? GuestAmountPaid, DateTime ExchangeDate)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    #region Master
                    model.CreatedDate = DateTime.Now;
                    model.CreatedAccount = currentAccount.UserName;
                    model.CreatedEmployeeId = currentEmployee.EmployeeId;
                    model.InventoryTypeId = EnumInventoryType.NC;
                    model.Paid = GuestAmountPaid.HasValue ? GuestAmountPaid : 0;
                    model.PreImportMasterCode = GetPreImportMasterCode();
                    model.StatusCode = "TAOMOI";

                    //Thêm tổng công nợ cộng dồn = nợ cũ + nợ mới 
                    decimal? SuplierOldDebt = _context.PreImportMasterModel
                                                      .Where(p => p.SupplierId == model.SupplierId)
                                                      .OrderByDescending(p => p.PreImportMasterId)
                                                      .Select(p => p.RemainingAmountAccrued)
                                                      .FirstOrDefault();
                    model.RemainingAmountAccrued = SuplierOldDebt == null ? 0 + model.RemainingAmount : SuplierOldDebt + model.RemainingAmount;
                    _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                    _context.SaveChanges(); // LƯU TẠM ĐỂ LẤY PREIMPORTMASTERID (SẼ BỊ SCROLLBACK KHI XẢY RA LỖI)
                    #endregion

                    #region Update ExchangeRate
                    var Exchangerate = _context.ExchangeRateModel
                                               .OrderByDescending(p => p.ExchangeDate)
                                               .Where(p => p.CurrencyId == model.CurrencyId &&
                                                            p.ExchangeDate.Value.CompareTo(DateTime.Now) <= 0
                                                      )
                                               .FirstOrDefault();
                    string DateDB = string.Format("{0}-{1}-{2}", Exchangerate.ExchangeDate.Value.Year, Exchangerate.ExchangeDate.Value.Month, Exchangerate.ExchangeDate.Value.Day);
                    string DateNow = string.Format("{0}-{1}-{2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                    if (DateDB == DateNow)
                    {
                        // update
                        Exchangerate.ExchangeRate = (float)model.ExchangeRate.Value;
                        Exchangerate.ExchangeDate = DateTime.Now;
                        _context.Entry(Exchangerate).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        ExchangeRateModel Exchangeratemodel = new ExchangeRateModel()
                        {
                            CurrencyId = model.CurrencyId,
                            ExchangeRate = (float)model.ExchangeRate.Value,
                            ExchangeDate = DateTime.Now,
                        };
                        // add
                        _context.Entry(Exchangeratemodel).State = System.Data.Entity.EntityState.Added;
                    }
                    _context.SaveChanges();
                    #endregion

                    #region Detail
                    if (detail != null)
                    {
                        if (detail.GroupBy(p => p.ProductId).ToList().Count < detail.Count)
                        {
                            //khong duoc trung san pham
                            return Content("Vui lòng không chọn thông tin sản phẩm trùng nhau");
                        }
                        foreach (var item in detail)
                        {
                            PreImportDetailModel detailmodel = new PreImportDetailModel()
                            {
                                PreImportMasterId = model.PreImportMasterId,
                                ProductId = item.ProductId,
                                Qty = item.Qty,
                                Price = item.Price,
                                UnitShippingWeight = item.UnitShippingWeight,
                                UnitPrice = item.UnitPrice,
                                ShippingFee = item.ShippingFee,
                                UnitCOGS = item.UnitCOGS,
                                Note = item.Note
                            };
                            _context.Entry(detailmodel).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                        }
                        // Cập nhật lại Tổng giá vốn 
                        model.SumCOGSOfOrderDetail = detail.Sum(p => p.UnitCOGS * p.Qty);
                        _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
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
                    #endregion
                }
            }
            catch
            {
                return Content("Xảy ra lỗi trong quá trình thêm mới yêu cầu sản phẩm từ nhà cung cấp");
            }
        }


        #endregion

        #region CreateViewBag
        private void CreateViewBag(int? WarehouseId = null, int? SupplierId = null, string VATType = "", string ManualDiscountType = "", int? CurrencyId = null, int? StoreId = null)
        {
            //0. StoreId : Load theo EmployeeId
            var StoreList = _context.StoreModel.OrderBy(p => p.StoreName)
                .Where(p =>
                    p.Actived == true &&
                    (currentEmployee.StoreId == null || p.StoreId == currentEmployee.StoreId)
                ).ToList();
            ViewBag.StoreId = new SelectList(StoreList, "StoreId", "StoreName", StoreId);
            //1. WarehouseId : Load theo cửa hàng
            var WarehouseList = _context.WarehouseModel.OrderBy(p => p.WarehouseName)
                .Where(p => p.Actived == true)
                .ToList();
            ViewBag.WarehouseId = new SelectList(WarehouseList, "WarehouseId", "WarehouseName", WarehouseId);

            //2. SupplierId
            var SupplierList = _context.SupplierModel.OrderBy(p => p.SupplierName).ToList();
            ViewBag.SupplierId = new SelectList(SupplierList, "SupplierId", "SupplierName", SupplierId);

            //3. VATType
            var VATTypeList = _context.VATTypeModel.OrderBy(p => p.VATTypeName).ToList();
            ViewBag.VATType = new SelectList(VATTypeList, "VATType", "VATTypeName", VATType);

            //4. ManualDiscountType
            var ManualDiscountList = _context.ManualDiscountTypeModel.OrderBy(p => p.manualDiscountTypeName).ToList();
            ViewBag.ManualDiscountType = new SelectList(ManualDiscountList, "ManualDiscountType", "manualDiscountTypeName", ManualDiscountType);

            //5. CurrencyId
            var CurrencyList = _context.CurrencyModel.OrderBy(p => p.CurrencyName).ToList();
            ViewBag.CurrencyId = new SelectList(CurrencyList, "CurrencyId", "CurrencyName", CurrencyId);
        }
        #endregion

        #region Detail
        
        public ActionResult Details(int id)
        {
            PreImportMasterModel importmastermodel = _context.PreImportMasterModel.Find(id);
            if (importmastermodel == null)
            {
                return HttpNotFound();
            }
            ViewBag.StoreName = _context.StoreModel.Where(p => p.StoreId == importmastermodel.StoreId).Select(p => p.StoreName).FirstOrDefault();
            ViewBag.WarhouseName = _context.WarehouseModel.Where(p => p.WarehouseId == importmastermodel.WarehouseId).Select(p => p.WarehouseName).FirstOrDefault();

            return View(importmastermodel);
        }
        public ActionResult _DetailList(List<PreImportDetailModel> model, string BillDiscountTypeId, decimal BillDiscount, decimal TotalPrice, int BillVAT, decimal RemainingAmount)
        {
            decimal TotalQty = 0, SumPrice = 0, TotalShipingWeight = 0;
            foreach (var item in model)
            {
                TotalQty += item.Qty.HasValue ? item.Qty.Value : 0;
                SumPrice += item.UnitPrice.HasValue ? item.UnitPrice.Value : 0;
                TotalShipingWeight += item.UnitShippingWeight.HasValue ? item.UnitShippingWeight.Value : 0;
            }
            ViewBag.TotalQty = TotalQty;
            ViewBag.TotalShipingWeight = TotalShipingWeight;
            ViewBag.SumPrice = SumPrice;
            ViewBag.BillDiscount = BillDiscountTypeId == "CASH" ? BillDiscount.ToString("n0") + "đ" : BillDiscount.ToString() + "%";
            ViewBag.TotalPrice = TotalPrice.ToString("n0") + "đ";
            ViewBag.BillVAT = BillVAT;
            ViewBag.GuestAmountPaid = TotalPrice - RemainingAmount; // MoneyTransfer
            ViewBag.RemainingAmount = RemainingAmount;

            return PartialView(model);
        }
        #endregion

        #region Confirm
        
        public ActionResult Confirm(int id)
        {
            PreImportMasterModel importmastermodel = _context.PreImportMasterModel.Find(id);
            if (importmastermodel == null)
            {
                return HttpNotFound();
            }
            ViewBag.StoreName = _context.StoreModel.Where(p => p.StoreId == importmastermodel.StoreId).Select(p => p.StoreName).FirstOrDefault();
            ViewBag.WarhouseName = _context.WarehouseModel.Where(p => p.WarehouseId == importmastermodel.WarehouseId).Select(p => p.WarehouseName).FirstOrDefault();

            return View(importmastermodel);
        }

        public ActionResult _ConfirmList(List<PreImportDetailModel> model, string BillDiscountTypeId, decimal BillDiscount, decimal TotalPrice, int BillVAT, decimal RemainingAmount)
        {
            #region
            //decimal TotalQty = 0, SumPrice = 0, TotalShipingWeight = 0;
            //foreach (var item in model)
            //{
            //    TotalQty += item.Qty.HasValue ? item.Qty.Value : 0;
            //    SumPrice += item.UnitPrice.HasValue ? item.UnitPrice.Value : 0;
            //    TotalShipingWeight += item.UnitShippingWeight.HasValue ? item.UnitShippingWeight.Value : 0;
            //}
            //ViewBag.TotalQty = TotalQty;
            //ViewBag.TotalShipingWeight = TotalShipingWeight;
            //ViewBag.SumPrice = SumPrice;
            //ViewBag.BillDiscount = BillDiscountTypeId == "CASH" ? BillDiscount.ToString("n0") + "đ" : BillDiscount.ToString() + "%";
            //ViewBag.TotalPrice = TotalPrice.ToString("n0") + "đ";
            //ViewBag.BillVAT = BillVAT;
            //ViewBag.GuestAmountPaid = TotalPrice - RemainingAmount; // MoneyTransfer
            //ViewBag.RemainingAmount = RemainingAmount;
            #endregion

            return PartialView(model);
        }

        public ActionResult SaveConfirm(ImportMasterModel model, List<PreImportDetailViewModel> detail, decimal TotalShippingWeight, decimal? GuestAmountPaid, DateTime ExchangeDate, int PreImportMasterId, string StatusCode, int CreateReceipt = 1)
        {
            if (StatusCode.Equals("DADUYET") || StatusCode.Equals("HUY"))
            {
                return Json("Yêu cầu đơn đặt hàng này đã được duyệt hoặc bị huỷ", JsonRequestBehavior.AllowGet);
            }
            #region // Lấy danh sách sản phẩm có ComfirmQty > 0
            var Lstdetail = (from p in detail
                             where p.ConfirmQty > 0
                             select new ImportDetailViewModel()
                             {
                                 ProductId = p.ProductId,
                                 Qty = p.ConfirmQty,
                                 Price = p.Price,
                                 UnitShippingWeight = p.UnitShippingWeight,
                                 UnitPrice = p.UnitPrice,
                                 Note = p.Note,
                                 ShippingFee = p.ShippingFee,
                                 UnitCOGS = p.UnitCOGS
                             }).ToList();
            model.TotalQty = Lstdetail.Count;
            #endregion

            // Bước 1 : Tính toán tương tự ImportMaster
            ImportMasterRepository ImportRepo = new ImportMasterRepository(_context);
            string Resuilt = ImportRepo.Save(model, Lstdetail, TotalShippingWeight, GuestAmountPaid, ExchangeDate, CreateReceipt, currentAccount.UserName, currentEmployee.EmployeeId);
           
            // Bước 2 : Cập nhật lại các field bên PreImportMaster : StatusCode , ImportMasterId
            if (Resuilt.Equals("success"))
            {
                var PreImportModel = _context.PreImportMasterModel.Where(p => p.PreImportMasterId == PreImportMasterId).FirstOrDefault();
                PreImportModel.ImportMasterId = model.ImportMasterId;
                PreImportModel.StatusCode = "DADUYET";
                _context.Entry(PreImportModel).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
            }
            return Json(Resuilt, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Edit
        
        public ActionResult Edit(int id)
        {
            PreImportMasterModel model = _context.PreImportMasterModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            
            CreateViewBag(model.WarehouseId, null, null, null,model.CurrencyId, model.StoreId);
            return View(model);
        }
        #endregion

        #region _EditList
        public ActionResult _EditList(decimal? ManualDiscount, string ManualDiscountType, decimal? VATValue, decimal? GuestAmountPaid, decimal? TotalShippingWeight, List<PreImportDetailModel> detail = null)
        {
            var Lst = (from p in detail
                       join pm in _context.ProductModel on p.ProductId equals pm.ProductId
                       select new PreImportDetailViewModel()
                       {
                           ProductId = p.ProductId,
                           Qty = p.Qty,
                           Price = p.Price,
                           UnitShippingWeight = p.UnitShippingWeight,
                           UnitPrice = p.UnitPrice,
                           Note = p.Note,
                           ShippingFee = p.ShippingFee,
                           UnitCOGS = p.UnitCOGS,
                           ProductName = pm.ProductName // Get Name to set default value Product
                       }).ToList();
            ViewBag.ManualDiscount = ManualDiscount;
            ViewBag.ManualDiscountType = ManualDiscountType;
            ViewBag.VATValue = VATValue;
            ViewBag.GuestAmountPaid = GuestAmountPaid;
            ViewBag.TotalShippingWeight = TotalShippingWeight;
            return PartialView(Lst);
        }
        #endregion

        public ActionResult Update(PreImportMasterModel model, List<PreImportDetailViewModel> detail, decimal TotalShippingWeight, decimal? GuestAmountPaid, DateTime ExchangeDate, int CreateReceipt = 1)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    #region  // Bước 1 : Modified Master
                    model.LastModifiedDate = DateTime.Now;
                    model.LastModifiedAccount = currentAccount.UserName;
                    model.LastModifiedEmployeeId = currentEmployee.EmployeeId;
                    model.Paid = GuestAmountPaid.HasValue ? GuestAmountPaid : 0;
                    //Thêm tổng công nợ cộng dồn = nợ cũ + nợ mới 
                    decimal? SuplierOldDebt = _context.PreImportMasterModel
                                                      .Where(p => p.SupplierId == model.SupplierId && p.PreImportMasterId < model.PreImportMasterId)
                                                      .OrderByDescending(p => p.PreImportMasterId)
                                                      .Select(p => p.RemainingAmountAccrued)
                                                      .FirstOrDefault();
                    model.RemainingAmountAccrued = SuplierOldDebt == null ? 0 + model.RemainingAmount : SuplierOldDebt + model.RemainingAmount;
                   
                    #region  //Cập nhật lại số nợ RemainingAmountAccrued của các đơn yêu hàng cầu sau
                    var lstModelNext = _context.PreImportMasterModel
                                               .Where(p => p.Actived == true && p.SupplierId == model.SupplierId && p.PreImportMasterId > model.PreImportMasterId)
                                               .OrderBy(p => p.PreImportMasterId)
                                               .ToList();
                    if (lstModelNext != null)
                    {
                        decimal? SaveRemainingPre = model.RemainingAmountAccrued.Value; // lưu Nợ cộng dồn của record trc đó
                        foreach (var item in lstModelNext)
                        {
                             decimal? SuplierOldDebtNext = _context.PreImportMasterModel
                                                      .Where(p => p.SupplierId == model.SupplierId && p.PreImportMasterId == item.PreImportMasterId)
                                                      .OrderByDescending(p => p.PreImportMasterId)
                                                      .Select(p => p.RemainingAmountAccrued)
                                                      .FirstOrDefault();
                             item.RemainingAmountAccrued = item.RemainingAmount + SaveRemainingPre;
                             SaveRemainingPre = SuplierOldDebtNext == null ? 0 + item.RemainingAmount : SuplierOldDebtNext + item.RemainingAmount;
                             _context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                             _context.SaveChanges();
                        }
                    }
                    #endregion

                    _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();
                    #endregion

                    #region // Bước 2 : Xoá những sản phẩm cũ , insert sản phẩm mới
                    var lstPreImportDetai = _context.PreImportDetailModel.Where(p => p.PreImportMasterId == model.PreImportMasterId).ToList();
                    foreach (var item in lstPreImportDetai)
                    {
                        _context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                        _context.SaveChanges();
                    }
                    #region Detail insert sản phẩm mới
                    if (detail != null)
                    {
                        if (detail.GroupBy(p => p.ProductId).ToList().Count < detail.Count)
                        {
                            //khong duoc trung san pham
                            return Content("Vui lòng không chọn thông tin sản phẩm trùng nhau");
                        }
                        foreach (var item in detail)
                        {
                            PreImportDetailModel detailmodel = new PreImportDetailModel()
                            {
                                PreImportMasterId = model.PreImportMasterId,
                                ProductId = item.ProductId,
                                Qty = item.Qty,
                                Price = item.Price,
                                UnitShippingWeight = item.UnitShippingWeight,
                                UnitPrice = item.UnitPrice,
                                ShippingFee = item.ShippingFee,
                                UnitCOGS = item.UnitCOGS,
                                Note = item.Note
                            };
                            _context.Entry(detailmodel).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                        }
                        // đánh dấu Transaction hoàn tất
                        ts.Complete();
                        return Content("success");
                    }
                    else
                    {
                        //chua nhap tt san pham
                        return Content("Vui lòng chọn thông tin sản phẩm");
                    }
                    #endregion

                    #endregion
                }
            }
            catch
            {
                return Content("Xảy ra lỗi trong quá trình Chỉnh sửa yêu cầu sản phẩm từ nhà cung cấp");
            }
        }
        private string GetPreImportMasterCode()
        {
            // Tìm giá trị STT order code
            string kq = "";
            string ImportCodeToFind = string.Format("{0}-{1}{2}", "PYC", (DateTime.Now.Year).ToString().Substring(2), (DateTime.Now.Month).ToString().Length == 1 ? "0" + (DateTime.Now.Month).ToString() : (DateTime.Now.Month).ToString());
            var Resuilt = _context.PreImportMasterModel.OrderByDescending(p => p.PreImportMasterId).Where(p => p.PreImportMasterCode.Contains(ImportCodeToFind)).Select(p => p.PreImportMasterCode).FirstOrDefault();
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
               kq = string.Format("{0}{1}", Resuilt.Substring(0, 9), STT); //ViewBag.ImportCode 
            }
            else
            {
                kq = string.Format("{0}-{1}", ImportCodeToFind, "0001");
            }
            return kq;
        }
    }
}
