using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using ViewModels;
using System.Data.Entity;
using Constant;
using System.Transactions;
using System.Data;
using Repository;


namespace WebUI.Controllers
{
    public class ImportMasterController : BaseController
    {
        //
        // GET: /ImportMaster/
        #region Danh sách ImportMaster
        
        public ActionResult Index()
        {
            //checkSession();
            CreateViewSearchBag();
            return View(_context.ImportMasterModel.OrderByDescending(p => p.ImportMasterId).Where(p => p.Actived == true).ToList());
        }
        #endregion

        #region Huỷ đơn hàng
        
        public ActionResult Cancel(int id)
        {
            //checkSession();
            try
            {
                //using(TransactionScope ts = new TransactionScope())
                //{
                ImportMasterModel model = _context.ImportMasterModel
                                                 .Where(p => p.ImportMasterId == id)
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
                            cmd.CommandText = "usp_ImportCanceled";
                            cmd.Parameters.AddWithValue("@ImportMasterId", model.ImportMasterId);
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
                }
                //_context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                //_context.SaveChanges();
                Resuilt = "success";
                //ts.Complete();
                return Json(Resuilt, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(string.Format(Resources.LanguageResource.CancelErrorMessenge, "nhập hàng nhà cung cấp"), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        public ActionResult _SearchImportMaster(ImportMasterSearchViewModel model)
        {
            //Danh sách
            List<ImportMasterInfoViewModel> list = new List<ImportMasterInfoViewModel>();

            if (model.ToDate.HasValue)
            {
                model.ToDate.Value.AddDays(1);
            }
            //Tim kiếm
            list = (from p in _context.ImportMasterModel

                    join sm in _context.SupplierModel on p.SupplierId equals sm.SupplierId into smList
                    from pp in smList.DefaultIfEmpty()

                    join wh in _context.WarehouseModel on p.WarehouseId equals wh.WarehouseId
                    join id in _context.ImportDetailModel on p.ImportMasterId equals id.ImportMasterId
                    join pd in _context.ProductModel on id.ProductId equals pd.ProductId
                    where (model.SupplierId == null || p.SupplierId == model.SupplierId) &&
                    (model.WarehouseId == null || p.WarehouseId == model.WarehouseId) &&
                    (model.ProductId == null || pd.ProductId == model.ProductId) &&
                    (model.ImportMasterId == null || p.ImportMasterId == model.ImportMasterId) &&
                    (model.FromDate == null || p.CreatedDate.Value.CompareTo(model.FromDate.Value) >= 0) &&
                    (model.ToDate == null || p.CreatedDate.Value.CompareTo(model.ToDate.Value) <= 0) &&
                    (model.FromTotalPrice == null || p.TotalPrice >= model.FromTotalPrice) &&
                    (model.ToTotalPrice == null || p.TotalPrice <= model.ToTotalPrice) &&
                    p.Actived == true
                    select new ImportMasterInfoViewModel()
                    {
                        CreatedDate = p.CreatedDate,
                        ImportMasterId = p.ImportMasterId,
                        ImportMasterCode = p.ImportMasterCode,
                        SupplierName = pp.SupplierName,
                        WarehouseName = wh.WarehouseCode,
                        SalemanName = p.SalemanName,
                        TotalQty = p.TotalQty,
                        TotalPrice = p.TotalPrice
                    }).Distinct()
                    .OrderByDescending(p => p.CreatedDate)
                    .ToList();
            return PartialView(list);
        }

        public ActionResult GetImportMasterId(string q)
        {
            var data2 = _context
                        .ImportMasterModel
                        .Where(p => (q == null || (p.ImportMasterCode).Contains(q)) && p.Actived == true)
                        .Select(p => new
                        {
                            value = p.ImportMasterId,
                            text = p.ImportMasterCode
                        })
                        .Take(10)
                        .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }

        #region CreateViewSearchBag
        private void CreateViewSearchBag(int? WarehouseId = null, int? SupplierId = null)
        {
            //1. WarehouseId
            var WarehouseList = _context.WarehouseModel.OrderBy(p => p.WarehouseName).ToList();
            ViewBag.WarehouseId = new SelectList(WarehouseList, "WarehouseId", "WarehouseName", WarehouseId);

            //2. SupplierId
            var SupplierList = _context.SupplierModel.OrderBy(p => p.SupplierName).ToList();
            ViewBag.SupplierId = new SelectList(SupplierList, "SupplierId", "SupplierName", SupplierId);

        }

        #endregion

        #region Thêm mới ImportMaster
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        
        public ActionResult Create()
        {
            ImportMasterModel model = new ImportMasterModel() { Actived = true };

            //model.SalemanName =
            model.CreatedAccount = currentAccount.UserName;
            AccountModel Account = _context.AccountModel.Where(p => p.UserName == model.CreatedAccount).FirstOrDefault();
            model.SalemanName = Account.EmployeeModel.FullName;
            ViewBag.SalemanName = model.SalemanName;
            CreateViewBag(null, null, null, null, 1);
            ViewBag.ImportCode = GetImportMasterCode();
            return View(model);
        }
        #endregion

        #region SupplierModel
        public ActionResult GetSuplierID(string q)
        {
            var data2 = _context
                       .SupplierModel
                       .Where(p => (q == null || (p.SupplierName.Contains(q))) && p.Actived == true)
                       .Select(p => new
                       {
                           value = p.SupplierId,
                           text = p.SupplierName
                       })
                       .Take(10)
                       .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ProductModel
        public ActionResult GetProductId(string q)
        {
            var data2 = _context
                        .ProductModel
                        .Where(p => (q == null || ((string.IsNullOrEmpty(p.ProductCode) ? "" : p.ProductCode) + " " + p.ProductName + (string.IsNullOrEmpty(p.Specifications) ? "" : p.Specifications)).Contains(q))
                            // && p.ProductStoreCode != null
                          && p.Actived == true)
                        .Select(p => new
                        {
                            value = p.ProductId,
                            text = ((string.IsNullOrEmpty(p.ProductCode) ? "" : p.ProductCode) + " | " + p.ProductName + " | " + (string.IsNullOrEmpty(p.Specifications) ? "" : p.Specifications))
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

        #region GetValuePriceAndUnitShippingWeight
        public ActionResult GetValuePriceAndUnitShippingWeight(int SelectedProductid)
        {
            var ValuePriceAndUnitShippingWeight = _context
                       .ProductModel
                       .Where(p => p.ProductId == SelectedProductid)
                       .Select(p => new
                       {
                           Price = p.ImportPrice,
                           ShippingWeight = p.ShippingWeight,
                           ShippingFee = p.ShippingFee
                       })
                       .FirstOrDefault();
            return Json(ValuePriceAndUnitShippingWeight, JsonRequestBehavior.AllowGet);
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
            var CurrencyList = _context.CurrencyModel.Where(p => p.Actived == true).OrderBy(p => p.CurrencyName).ToList();
            ViewBag.CurrencyId = new SelectList(CurrencyList, "CurrencyId", "CurrencyName", CurrencyId);
        }
        #endregion

        #region GetExchangeRateBy
        public ActionResult GetExchangeRateBy(int CurrencyIdSelect)
        {
            var GetExchangeRate2 = _context.ExchangeRateModel
                                .Where(p =>
                                        p.CurrencyId == CurrencyIdSelect &&
                                        p.ExchangeDate <= DateTime.Now)
                                .OrderByDescending(p => p.ExchangeDate)
                                .Select(p => new { ExchangeRate = p.ExchangeRate, ExchangeDate = p.ExchangeDate })
                                .FirstOrDefault();
            //string.Format("{0}-{1}-{2}", p.ExchangeDate.Value.Year, p.ExchangeDate.Value.Month, p.ExchangeDate.Value.Day)
            string Date = string.Format("{0}-{1}-{2}", GetExchangeRate2.ExchangeDate.Value.Year, GetExchangeRate2.ExchangeDate.Value.Month, GetExchangeRate2.ExchangeDate.Value.Day);
            var GetExchangeRate = new
            {
                ExchangeRate = GetExchangeRate2.ExchangeRate,
                ExchangeDate = Date
            };

            return Json(GetExchangeRate, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region _CreateList
        public ActionResult _CreateList(List<ImportDetailViewModel> detail = null)
        {

            if (detail == null)
            {
                detail = new List<ImportDetailViewModel>();
            }

            return PartialView(detail);
        }
        #endregion
        #region _CreatelistInner
        public ActionResult _CreatelistInner(List<ImportDetailViewModel> detail = null, string scanBarcode = null, int? ProQty = 1, string Note = null)
        {
            //Nếu detail == null thì khởi tạo List mới
            if (detail == null)
            {
                detail = new List<ImportDetailViewModel>();
                //Nếu barcode do user nhập không có trong DB thì khởi tạo một Model mới để add vào
                if (string.IsNullOrEmpty(scanBarcode))
                {
                    ImportDetailViewModel item = new ImportDetailViewModel();
                    detail.Add(item);

                }
                else  // Quét mã vạch
                {
                    #region Get thông tin cho product
                    //int id = 0;
                    //if (Int32.TryParse(scanBarcode, out id))
                    //{ 
                    //Lấy sản phẩm dựa vào barcode
                    var product = _context.ProductModel.Where(p => p.Barcode == scanBarcode).FirstOrDefault();
                    if(Note == "KM")
                    {
                        product.ImportPrice = 0;
                    }
                    if (product != null)
                    {
                        ImportDetailViewModel item = new ImportDetailViewModel()
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductCode + " | " + product.ProductName,
                            Price = product.ImportPrice,
                            ShippingFee = product.ShippingFee,
                            Qty = ProQty,
                            UnitPrice = (product.ImportPrice + product.ShippingFee) * ProQty,
                            Note = Note
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
            else
            {
                //Nếu barcode do user nhập không có trong DB thì khởi tạo một Model mới để add vào
                if (string.IsNullOrEmpty(scanBarcode))
                {
                    ImportDetailViewModel item = new ImportDetailViewModel();
                    detail.Add(item);

                }
                else  // Quét mã vạch
                {
                    #region Get thông tin cho product
                    //int id = 0;
                    //if (Int32.TryParse(scanBarcode, out id))
                    //{ 
                    //Lấy sản phẩm dựa vào barcode
                    var product = _context.ProductModel.Where(p => p.Barcode == scanBarcode).FirstOrDefault();
                    if (Note == "KM")
                    {
                        product.ImportPrice = 0;
                    }
                    if (product != null)
                    {

                        // -- 1.1 Thì cộng số lượng lên
                        // -- 1.2 Ngược lại => thêm mới vào danh sách trả về
                        #region // -- 1.1 Thì cộng số lượng lên
                        // Nếu đã có Sản phẩm trong danh sách cũ + và cùng loại (sản phẩm nhập mua || sản phẩm khuyến mãi)
                        foreach (var itemm in detail)
                        {
                            if (itemm.ProductId == product.ProductId && (itemm.Note??"") == (Note??""))
                            {
                                //Có sản phẩm nà và cùng loại trong danh sách cũ
                                // -- 1.1 Thì cộng số lượng lên
                                itemm.Qty += ProQty;
                                itemm.UnitPrice += ((itemm.Price + itemm.ShippingFee) * ProQty);
                                return PartialView(detail);
                            }
                        }
                        #endregion

                        #region // -- 1.2 Ngược lại => thêm mới vào danh sách trả về
                        ImportDetailViewModel item = new ImportDetailViewModel()
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductCode + " | " + product.ProductName,
                            Price = product.ImportPrice,
                            ShippingFee = product.ShippingFee,
                            Qty = ProQty,
                            UnitPrice = (product.ImportPrice + product.ShippingFee) * ProQty,
                            Note = Note
                        };
                        detail.Add(item);
                        //return PartialView(detail);
                        #endregion
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

        private void CreateDetailViewBag(int? ProductId = null)
        {
            var ProductList = _context.ProductModel.OrderBy(p => p.ProductName).ToList();
            ViewBag.ProductId = ProductList;
        }
        #endregion
        #region _DeletelistInner
        public ActionResult _DeletelistInner(List<ImportDetailViewModel> detail, int RemoveId)
        {
            if (detail == null)
            {
                detail = new List<ImportDetailViewModel>();
            }


            return PartialView("_CreatelistInner", detail.Where(p => p.STT != RemoveId).ToList());
        }
        #endregion
        #region Save
        public ActionResult Save(ImportMasterModel model, List<ImportDetailViewModel> detail, decimal TotalShippingWeight, decimal? GuestAmountPaid, DateTime ExchangeDate, int CreateReceipt = 1)
        {
            string resuilt = "";
            ImportMasterRepository ImportMasterRepo = new ImportMasterRepository(_context);
            resuilt = ImportMasterRepo.Save(model, detail, TotalShippingWeight, GuestAmountPaid, ExchangeDate, CreateReceipt, currentAccount.UserName, currentEmployee.EmployeeId);
            return Json(resuilt, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Detail
        
        public ActionResult Details(int id)
        {

            ImportMasterModel importmastermodel = _context.ImportMasterModel.Find(id);
            if (importmastermodel == null)
            {
                return HttpNotFound();
            }
            ViewBag.StoreName = _context.StoreModel.Where(p => p.StoreId == importmastermodel.StoreId).Select(p => p.StoreName).FirstOrDefault();
            ViewBag.WarhouseName = _context.WarehouseModel.Where(p => p.WarehouseId == importmastermodel.WarehouseId).Select(p => p.WarehouseName).FirstOrDefault();

            return View(importmastermodel);
        }
        public ActionResult _DetailList(List<ImportDetailModel> model, string BillDiscountTypeId, decimal BillDiscount, decimal TotalPrice, int BillVAT, decimal RemainingAmount)
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

        #region _DetailList
        //public ActionResult _DetailList(List<ImportDetailModel> model)
        //{
        //    return PartialView(model);
        //}
        #endregion

        #region GetByShippingWeight from ProductModel
        public ActionResult GetByShippingWeight(int producitSelected)
        {
            var jsonData = _context.ProductModel.Where(p => p.ProductId == producitSelected).FirstOrDefault();
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Autocomplete ListProductCode
        public ActionResult ListProductCode(string q)
        {
            var data = _context.ProductModel.Where(p => p.ProductCode.Contains(q)).Select(p => p.ProductCode).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Edit ImportMaster
        public ActionResult Edit(int id)
        {
            ImportMasterModel model = _context.ImportMasterModel
                                    .Include(p => p.ImportDetailModel)
                                    .Where(p => p.ImportMasterId == id).FirstOrDefault();
            model.CreatedAccount = currentAccount.UserName;
            AccountModel Account = _context.AccountModel.Where(p => p.UserName == model.CreatedAccount).FirstOrDefault();
            ViewBag.SalemanName = Account.EmployeeModel.FullName;


            ViewBag.ImportDetailList = model.ImportDetailModel.Select(p => new ImportDetailViewModel()
            {
                //ImportDetailId = p.ImportDetailId,
                ImportMasterId = p.ImportMasterId,
                ProductId = p.ProductId,
                Qty = p.Qty,
                Price = p.Price,
                UnitShippingWeight = p.UnitShippingWeight,
                UnitPrice = p.UnitPrice,
                ProductName = p.ProductModel.ProductName,
            }).ToList();


            if (model == null)
            {
                return HttpNotFound();
            }
            CreateViewBag();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit()
        {
            return View();
        }
        #endregion

        #region Xử lý btnUpdate
        public ActionResult Update(ImportMasterModel model, List<ImportDetailViewModel> detail, decimal TotalShippingWeight)
        {
            try
            {

                //_context.Entry(model).State = System.Data.Entity.EntityState.Modified; Chỉ Modified những filed cần thiết
                ImportMasterModel ImportMastermodel = _context.ImportMasterModel
                                                                .Include(p => p.ImportDetailModel)
                                                                .Where(p => p.ImportMasterId == model.ImportMasterId).FirstOrDefault();
                // Xoá những Detail cũ
                if (ImportMastermodel.ImportDetailModel != null && ImportMastermodel.ImportDetailModel.Count > 0)
                {
                    while (ImportMastermodel.ImportDetailModel.Count > 0)
                    {
                        _context.Entry(ImportMastermodel.ImportDetailModel.First()).State = System.Data.Entity.EntityState.Deleted;
                    }
                }

                #region // Update lại những filed cần Modified cho Model Master
                ImportMastermodel.ImportMasterId = model.ImportMasterId;
                ImportMastermodel.SupplierId = model.SupplierId;
                ImportMastermodel.SalemanName = model.SalemanName;
                ImportMastermodel.SenderName = model.SenderName;
                ImportMastermodel.ReceiverName = model.ReceiverName;
                ImportMastermodel.Note = model.Note;
                ImportMastermodel.CurrencyId = model.CurrencyId;
                ImportMastermodel.ExchangeRate = model.ExchangeRate;
                ImportMastermodel.WarehouseId = model.WarehouseId;
                ImportMastermodel.VATType = model.VATType;
                ImportMastermodel.VATValue = model.VATValue;
                ImportMastermodel.TAXBillCode = model.TAXBillCode;
                ImportMastermodel.TAXBillDate = model.TAXBillDate;
                ImportMastermodel.ManualDiscountType = model.ManualDiscountType;
                ImportMastermodel.ManualDiscount = model.ManualDiscount;
                ImportMastermodel.Paid = model.Paid;
                ImportMastermodel.MoneyTransfer = model.MoneyTransfer;
                ImportMastermodel.DebtDueDate = model.DebtDueDate;
                ImportMastermodel.LastModifiedDate = DateTime.Now;
                ImportMastermodel.TotalQty = model.TotalQty;
                ImportMastermodel.TotalPrice = model.TotalPrice;
                #endregion
                // đánh dấu sửa ImportMasterModel
                _context.Entry(ImportMastermodel).State = System.Data.Entity.EntityState.Modified;

                if (detail != null)
                {
                    foreach (var item in detail)
                    {
                        ImportDetailModel detailmodel = new ImportDetailModel()
                        {
                            ImportMasterId = model.ImportMasterId,
                            ProductId = item.ProductId,
                            Qty = item.Qty,
                            Price = item.Price,
                            UnitShippingWeight = item.UnitShippingWeight,
                            UnitPrice = item.UnitPrice
                        };
                        _context.Entry(detailmodel).State = System.Data.Entity.EntityState.Added;
                    }
                }
                _context.SaveChanges();
                return Content("success");
            }
            catch
            {
                return Content("Xảy ra lỗi trong quá trình thêm mới nhà cung cấp");
            }

        }
        #endregion

        private string GetImportMasterCode()
        {
            // Tìm giá trị STT order code
            string kq = "";
            string ImportCodeToFind = string.Format("{0}-{1}{2}", "PNK", (DateTime.Now.Year).ToString().Substring(2), (DateTime.Now.Month).ToString().Length == 1 ? "0" + (DateTime.Now.Month).ToString() : (DateTime.Now.Month).ToString());
            var Resuilt = _context.ImportMasterModel.OrderByDescending(p => p.ImportMasterId).Where(p => p.ImportMasterCode.Contains(ImportCodeToFind)).Select(p => p.ImportMasterCode).FirstOrDefault();
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
                kq = string.Format("{0}{1}", Resuilt.Substring(0, 9), STT); // ViewBag.ImportCode
            }
            else
            {
                kq = string.Format("{0}-{1}", ImportCodeToFind, "0001");
            }
            return kq;
        }

    }
}
