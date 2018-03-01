using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using ViewModels;
using AutoMapper;
using Repository;
using Constant;
using System.Transactions;
using System.IO;
using Zen.Barcode;

using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
namespace WebUI.Controllers
{
    public class ProductController : BaseController
    {

        protected bool isUsePolicyInStockId = true;
        protected bool isUsePolicyOutOfStockId = true;
        protected bool isUseActived = true;


        //
        // GET: /Product/
        //int? CategoryId = null, int? OriginOfProductId = null, string ProductName = null, string ProductCode = null
        #region Danh sách sản phẩm
        
        public ActionResult Index()
        {
            CreateIndexViewBag();
            ProductSearchViewModel model = new ProductSearchViewModel();
            return View(model);
        }
        public ActionResult _SearchPartial(ProductSearchViewModel model, int CategoryId = 2, int PageIndex = 1, int PageSize = 10)
        {

            var category = _context.CategoryModel.Find(CategoryId);
            var list = (from p in _context.ProductModel
                        join c in _context.CategoryModel on p.CategoryId equals c.CategoryId
                        join o in _context.OriginOfProductModel on p.OriginOfProductId equals o.OriginOfProductId into OriginOfProduct
                        from oo in OriginOfProduct.DefaultIfEmpty()
                        where (model.ProductName == null || p.ProductName.Contains(model.ProductName)) &&
                               (c.ADNCode.StartsWith(category.ADNCode)) &&
                               (model.OriginOfProductId == null || p.OriginOfProductId == model.OriginOfProductId) &&
                               (model.ProductCode == null || p.ProductCode.Contains(model.ProductCode)) &&
                               (model.ProductStatusId == null || p.ProductStatusId == model.ProductStatusId) &&
                               ((model.Valid == true && p.ProductStoreCode != null) || (model.Valid == false && p.ProductStoreCode == null)) &&
                               (model.Actived == p.Actived)
                        select new ProductInfoViewModel()
                         {
                             Barcode = p.ProductCode,
                             ProductId = p.ProductId,
                             ProductCode = p.ProductCode,
                             ProductStoreCode = p.ProductStoreCode,
                             ProductName = p.ProductName,
                             CategoryName = c.CategoryName,
                             OriginOfProductName = oo.OriginOfProductName,
                             ImageUrl = p.ImageUrl
                         });

            list = list.OrderByDescending(p => p.ProductId);

            ViewBag.TotalRow = list.Count();
            ViewBag.RowIndex = (PageIndex - 1) * PageSize;
            return PartialView(list.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList());
        }
        public void CreateIndexViewBag(int? CategoryId = null, int? OriginOfProductId = null, int? ProductStatusId = null)
        {

            ViewBag.isUseActived = isUseActived;

            //1. CategoryId
            CategoryRepository repository = new CategoryRepository(_context);
            var CategoryList = repository.GetCategoryByParentWithFormat(2).Select(p => new { CategoryId = p.CategoryId, CategoryName = p.CategoryName.Substring(4) }).ToList();
            CategoryList.RemoveAt(0);
            CategoryList.Insert(0, new { CategoryId = 2, CategoryName = "Tất cả sản phẩm" });
            ViewBag.CategoryId = new SelectList(CategoryList, "CategoryId", "CategoryName", CategoryId);

            // 2. OriginOfProductId
            var OriginOfProductList = _context.OriginOfProductModel.OrderBy(p => p.OriginOfProductName).ToList();
            ViewBag.OriginOfProductId = new SelectList(OriginOfProductList, "OriginOfProductId", "OriginOfProductName", OriginOfProductId);

            // 3.
            var ProductStatusNameList = _context.ProductStatusModel.OrderBy(p => p.ProductStatusName).ToList();
            ViewBag.ProductStatusId = new SelectList(ProductStatusNameList, "ProductStatusId", "ProductStatusName", ProductStatusId);

        }

        #endregion

        #region Thêm mới sản phẩm
        public ActionResult CreateProduct()
        {
            ProductViewModel productmodel = new ProductViewModel() { Actived = true, CurrencyId = 1, ExchangeRate = 1, BeginInventoryQty = 0, ImportDate = DateTime.Now };
            CreateViewBag(null, null, null, null, null, null, null, null, 1);
            ViewBag.COGS = 0;
            return PartialView(productmodel);
        }
        [HttpPost]
        [ValidateInput(false)]
        //ProductViewModel model, List<PriceListViewModel> Price
        public ActionResult CreateProduct(ProductViewModel model, HttpPostedFileBase file, string mydata
            , List<ProductAlertViewModel> QtyAlertList = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        //ProductModel
                        ProductModel addmodel = new ProductModel();
                        Mapper.CreateMap<ProductViewModel, ProductModel>();
                        addmodel = Mapper.Map<ProductModel>(model);
                        addmodel.Actived = true;
                        addmodel.SEOProductName = Library.ConvertToNoMarkString(addmodel.ProductName);
                        addmodel.CreatedDate = DateTime.Now;
                        addmodel.CreatedAccount = currentAccount.UserName;

                        #region lưu file
                        addmodel.ImageUrl = "noimage.jpg";
                        if (file != null)
                        {
                            string filename = file.FileName;
                            int index = filename.IndexOf(".");
                            string type = filename.Substring(index);

                            int size = file.ContentLength;
                            string ContentType = file.ContentType;

                            byte[] FileContent = PathToByteArray(file.InputStream);
                            addmodel.ImageUrl = Upload(file, "Product");

                            SYS_tblFile FileSave = new SYS_tblFile()
                            {
                                FileTitle = filename,
                                FileName = addmodel.ImageUrl,
                                Extension = type,
                                ContentType = ContentType,
                                FileContent = FileContent,
                                FolderId = 1,
                                Size = size,
                                CreatedByUserId = currentAccount.EmployeeId,
                                CreatedOnDate = addmodel.CreatedDate
                            };
                            _context.Entry(FileSave).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                            addmodel.FileId = FileSave.FileId;
                        }
                        #endregion

                        ProductRepository ProductRepository = new ProductRepository(_context);
                        addmodel.ProductStoreCode = ProductRepository.GetProdcutStoreCode(model.StoreId.Value, model.ProductTypeId.Value, model.CategoryId);
                        addmodel.ProductCode = string.IsNullOrEmpty(model.ProductCode) ? "" : model.ProductCode;
                        addmodel.COGS = model.ImportPrice * model.ExchangeRate + model.ShippingFee;
                        _context.Entry(addmodel).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();

                        #region Lưu các loại giá
                        //Product Price 1
                        ProductPriceModel price1 = new ProductPriceModel()
                        {
                            Price = model.Price1,
                            CustomerLevelId = 1,
                            ProductId = addmodel.ProductId
                        };
                        _context.Entry(price1).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();
                        //Product Price 2
                        ProductPriceModel price2 = new ProductPriceModel()
                        {
                            Price = model.Price2,
                            CustomerLevelId = 2,
                            ProductId = addmodel.ProductId
                        };
                        _context.Entry(price2).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();
                        //Product Price 3
                        ProductPriceModel price3 = new ProductPriceModel()
                        {
                            Price = model.Price3,
                            CustomerLevelId = 3,
                            ProductId = addmodel.ProductId
                        };
                        _context.Entry(price3).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();

                        //Product Price 4
                        ProductPriceModel price4 = new ProductPriceModel()
                        {
                            Price = model.Price4,
                            CustomerLevelId = 4,
                            ProductId = addmodel.ProductId
                        };
                        _context.Entry(price4).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();

                        #endregion

                        model.CreatedAccount = currentAccount.UserName;
                        AccountModel Account = _context.AccountModel.Where(p => p.UserName == model.CreatedAccount).FirstOrDefault();

                        #region Tồn đầu kỳ
                        IEOtherRepository IEOtherRepository = new IEOtherRepository(_context);
                        if (model.BeginInventoryQty != 0)
                        {
                            // Lưu vào IEOtherMasterModel
                            IEOtherMasterModel IEMaster = new IEOtherMasterModel()
                            {
                                IEOtherMasterCode = IEOtherRepository.GetIEOtherCode(),
                                WarehouseId = model.BeginWarehouseId.Value,
                                InventoryTypeId = EnumInventoryType.ĐK,
                                Note = "Tồn đầu",
                                CreatedDate = DateTime.Now,
                                CreatedAccount = currentAccount.UserName,
                                Actived = true,
                                CreatedEmployeeId = Account.EmployeeId,
                                TotalPrice = addmodel.BeginInventoryQty * addmodel.COGS
                            };
                            _context.Entry(IEMaster).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                            // Lưu vào IEOtherDetailModel
                            IEOtherDetailModel detailmodel = new IEOtherDetailModel()
                            {
                                IEOtherMasterId = IEMaster.IEOtherMasterId,
                                ProductId = addmodel.ProductId,
                                ImportQty = addmodel.BeginInventoryQty,
                                //ExportQty = 0,
                                Price = addmodel.COGS,
                                UnitShippingWeight = (decimal)(addmodel.ShippingWeight.HasValue ? addmodel.ShippingWeight.Value : 1) * addmodel.BeginInventoryQty,
                                UnitPrice = addmodel.BeginInventoryQty * addmodel.COGS,
                                Note = IEMaster.Note
                            };
                            _context.Entry(detailmodel).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                            // Lưu vào InventoyMaster
                            InventoryMasterModel InvenMaster = new InventoryMasterModel()
                            {
                                StoreId = addmodel.StoreId,
                                InventoryTypeId = EnumInventoryType.ĐK,
                                WarehouseModelId = addmodel.BeginWarehouseId,
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
                            _context.SaveChanges(); // insert tạm để lấy InvenMasterID
                            // Lưu vào InventoryDetailModel
                            InventoryDetailModel InvenDetail = new InventoryDetailModel()
                            {
                                InventoryMasterId = InvenMaster.InventoryMasterId,
                                ProductId = addmodel.ProductId,
                                BeginInventoryQty = 0,
                                COGS = addmodel.COGS,// nhập
                                //Price = 0, // => Xuất
                                ImportQty = addmodel.BeginInventoryQty,
                                //ExportQty = 0,
                                UnitCOGS = addmodel.COGS * addmodel.BeginInventoryQty, // nhập
                                //UnitPrice = 0, // => Xuất
                                EndInventoryQty = addmodel.BeginInventoryQty
                            };
                            _context.Entry(InvenDetail).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                        }
                        #endregion

                        #region Duyệt tồn kho cảnh báo
                        if (QtyAlertList != null)
                        {
                            if (QtyAlertList.GroupBy(p => p.WarehouseId).ToList().Count < QtyAlertList.Count)
                            {
                                return Content("Vui lòng không chọn thông tin 'Kho' trùng nhau khi cài đặt cảnh báo tồn kho");
                            }
                            if (QtyAlertList.Where(p => (p.QtyAlert == 0 || p.QtyAlert == null)).FirstOrDefault() != null)
                            {
                                return Content("Vui lòng nhập thông tin 'Số lượng' lớn hơn 0 khi cài đặt cảnh báo tồn kho");
                            }
                            foreach (var item in QtyAlertList)
                            {
                                ProductAlertModel modelQtyAlert = new ProductAlertModel()
                                {
                                    ProductId = addmodel.ProductId,
                                    WarehouseId = item.WarehouseId,
                                    RolesId = item.RolesId,
                                    QtyAlert = item.QtyAlert
                                };
                                _context.Entry(modelQtyAlert).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();
                            }
                        }

                        #endregion

                        ts.Complete(); // đánh dấu Transaction hoàn tất
                        return Content("success");
                    }
                }
                else
                {
                    CreateViewBag(model.StoreId, model.CategoryId, model.OriginOfProductId, model.PolicyInStockId, model.PolicyOutOfStockId, model.LocationOfProductId, model.ProductStatusId, model.UnitId, model.CurrencyId);
                    // return View(model);
                    return Content(Resources.LanguageResource.AddErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                CreateViewBag(model.CategoryId, model.OriginOfProductId, model.PolicyInStockId, model.PolicyOutOfStockId, model.LocationOfProductId, model.ProductStatusId, model.UnitId, model.CurrencyId);
                //return View(model);
                string errorMessage = string.Format("{0} {1}",
                                        Resources.LanguageResource.AddErrorMessage,
                                        ex.Message
                                    );
                return Content(errorMessage);
            }
        }

        #region CreateViewBag
        private void CreateViewBag(int? WarehouseId = null, int? StoreId = null, int? CategoryId = null, int? OriginOfProductId = null, int? PolicyInStockId = null, int? PolicyOutOfStockId = null, int? LocationOfProductId = null, int? ProductStatusId = null, int? UnitId = null, int? CurrencyId = null)
        {
            ViewBag.isUsePolicyInStockId = isUsePolicyInStockId;
            ViewBag.isUsePolicyOutOfStockId = isUsePolicyOutOfStockId;
            ViewBag.isUseActived = isUseActived;

            //0. StoreId
            var StoreList = _context.StoreModel.OrderBy(p => p.StoreName).Where(p =>
                p.Actived == true &&
                currentEmployee.StoreId == null ||
                p.StoreId == currentEmployee.StoreId
                )
                .ToList();
            ViewBag.StoreId = new SelectList(StoreList, "StoreId", "StoreName", StoreId);
            // 0.1 WareHouse
            var WarehouseList = _context.WarehouseModel.OrderBy(p => p.WarehouseName).ToList();
            ViewBag.BeginWarehouseId = new SelectList(WarehouseList, "WarehouseId", "WarehouseName", WarehouseId);

            //1. CategoryId
            CategoryRepository repository = new CategoryRepository(_context);
            var CategoryList = repository.GetCategoryByParentWithFormat(2).Select(p => new { CategoryId = p.CategoryId, CategoryName = p.CategoryName.Substring(4) }).ToList();
            CategoryList.RemoveAt(0);
            CategoryList.Insert(0, new { CategoryId = 2, CategoryName = "Sản phẩm" });
            ViewBag.CategoryId = new SelectList(CategoryList, "CategoryId", "CategoryName", CategoryId);

            // 2. OriginOfProductId
            var OriginOfProductList = _context.OriginOfProductModel.OrderBy(p => p.OriginOfProductName).ToList();
            ViewBag.OriginOfProductId = new SelectList(OriginOfProductList, "OriginOfProductId", "OriginOfProductName", OriginOfProductId);

            // 3. PolicyInStockId
            var PolicyInStockList = _context.PolicyModel.OrderBy(p => p.PolicyName).ToList();
            ViewBag.PolicyInStockId = new SelectList(PolicyInStockList, "PolicyId", "PolicyName", PolicyInStockId);

            // 4. PolicyOutOfStockId
            var PolicyOutOfStockList = _context.PolicyModel.OrderBy(p => p.PolicyName).ToList();
            ViewBag.PolicyOutOfStockId = new SelectList(PolicyOutOfStockList, "PolicyId", "PolicyName", PolicyOutOfStockId);

            // 5. LocationOfProductId
            var LocationOfProductList = _context.LocationOfProductModel.OrderBy(p => p.LocationOfProductName).ToList();
            ViewBag.LocationOfProductId = new SelectList(LocationOfProductList, "LocationOfProductId", "LocationOfProductName", LocationOfProductId);

            // 6. ProductStatusId
            var ProductStatusList = _context.ProductStatusModel.OrderBy(p => p.ProductStatusName).ToList();
            ViewBag.ProductStatusId = new SelectList(ProductStatusList, "ProductStatusId", "ProductStatusName", ProductStatusId);

            //7 UnitId
            var Unitlist = _context.UnitModel.OrderBy(p => p.UnitName).ToList();
            ViewBag.UnitId = new SelectList(Unitlist, "UnitId", "UnitName", UnitId);

            // 8. CurrencyId
            var CurrencyList = _context.CurrencyModel.ToList();
            ViewBag.CurrencyId = new SelectList(CurrencyList, "CurrencyId", "CurrencyName", CurrencyId);

        }
        #endregion

        #region GetProductType
        public ActionResult GetProductType(string q)
        {
            var data2 = _context
                        .ProductTypeModel
                        .Where(p => q == null || (p.ProductTypeCode + " " + p.ProductTypeName).Contains(q))
                        .Select(p => new
                        {
                            value = p.ProductTypeId,
                            text = (p.ProductTypeCode + " | " + p.ProductTypeName)
                        })
                        .Take(10)
                        .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region UpdateProductStoreCode
        public ActionResult UpdateProductStoreCode(int StoreId, int ProductTypeId, int? CategoryId)
        {
            ProductRepository ProductRepository = new ProductRepository(_context);
            string ProductStoreCode = ProductRepository.GetProdcutStoreCode(StoreId, ProductTypeId, CategoryId);
            return Json(ProductStoreCode, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region UpdateProductStoreCodeDuplicate
        public ActionResult UpdateProductStoreCodeDuplicate(int StoreId, int ProductTypeId, int? CategoryId, string ProductStoreCodeMark)
        {
            ProductRepository ProductRepository = new ProductRepository(_context);
            string ProductStoreCode = ProductRepository.GetProdcutStoreCodeDuplicate(StoreId, ProductTypeId, CategoryId, ProductStoreCodeMark);
            return Json(ProductStoreCode, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region GetExchangeRateBy
        public ActionResult GetExchangeRateBy(int CurrencyIdSelect)
        {
            var GetExchangeRate = _context.ExchangeRateModel
                                .Where(p =>
                                        p.CurrencyId == CurrencyIdSelect &&
                                        p.ExchangeDate <= DateTime.Now)
                                .OrderByDescending(p => p.ExchangeDate)
                                .Select(p => p.ExchangeRate)
                                .FirstOrDefault();
            return Json(GetExchangeRate, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion

        #region Sửa thông tin sản phẩm
        [HttpGet]
        public ActionResult EditProduct(ProductViewModel model, string pathImage)
        {
            CreateViewBag(model.BeginWarehouseId, model.StoreId, model.CategoryId, model.OriginOfProductId, model.PolicyInStockId, model.PolicyOutOfStockId, model.LocationOfProductId, model.ProductStatusId, model.UnitId, model.CurrencyId);
            ViewBag.ProductStoreCode = model.ProductStoreCode;
            ViewBag.ProductTypeName = _context.ProductTypeModel.Where(p => p.ProductTypeId == model.ProductTypeId).Select(p => p.ProductTypeCode + "|" + p.ProductTypeName).FirstOrDefault();
            ViewBag.COGS = model.COGS.HasValue ? model.COGS : 0;
            ViewBag.BeginWarehouseName = _context.WarehouseModel.Where(p => p.WarehouseId == model.BeginWarehouseId).Select(p => p.WarehouseName).FirstOrDefault();
            model.ProductStoreCodeMark = model.ProductStoreCode;
            ViewBag.pathImage = pathImage;
            decimal? EndInventoryQty = _context.InventoryDetailModel.Where(p => p.ProductId == model.ProductId).OrderByDescending(p => p.InventoryDetailId).Select(p => p.EndInventoryQty).FirstOrDefault();
            model.EndInventoryQty = EndInventoryQty;
            return PartialView(model);
        }
        public ActionResult EditProductImage(List<ProductImageModel> ListImage, int ProductId)
        {
            ViewBag.ProductId = ProductId;
            return PartialView(ListImage);
        }
        public ActionResult _EditListImageInner(List<ProductImageModel> ListImage = null)
        {
            if (ListImage == null)
            {
                ListImage = new List<ProductImageModel>();
            }
            return PartialView(ListImage);
        }
        #region -- Thêm ảnh
        private bool isValidd(string path)
        {
            return path.Equals("image/png") || path.Equals("image/jpeg") || path.Equals("image/gif") || path.Equals("image/jpg");
        }
        public ActionResult AddProductImage(int ProductId, List<ProductImageModel> ListImage, List<HttpPostedFileBase> files)
        {
            if (files[0] != null)
            {
                foreach (var file in files)
                {
                    if (isValidd(file.ContentType))
                    {
                        // Bước 1 : thêm ảnh vào folder

                        string folder = "Product/" + DateTime.Now.ToString("yyyy-MM");
                        // Lấy đường dẫn tới folder
                        var physicalpath = Server.MapPath("~/Upload/" + folder);
                        var directory = new DirectoryInfo(physicalpath);
                        string imgUrl = "";
                        // Kiểm tra nếu chưa tồn tại folder Năm - Tháng : tạo Folder mới
                        if (!directory.Exists)
                        {
                            System.IO.Directory.CreateDirectory(physicalpath);// Tạo folder gốc
                            System.IO.Directory.CreateDirectory(physicalpath + "/Thum");// Tạo folder thum
                        }
                        imgUrl = "/Upload/" + folder + "/" + Upload(file, folder);

                        //Bước 2 : Thêm vào database
                        ProductImageModel modeladd = new ProductImageModel()
                        {
                            ProductId = ProductId,
                            ImageUrl = imgUrl
                        };
                        _context.Entry(modeladd).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();

                        // Bước 3 : Thêm ảnh vào List
                        if (ListImage == null)
                        {
                            ListImage = new List<ProductImageModel>();
                        }
                        ListImage.Add(modeladd);
                    }
                }
            }
            return PartialView("_EditListImageInner", ListImage);
        }

        #endregion

        #region -- Xoá ảnh
        public ActionResult DeleteProductImage(List<ProductImageModel> ListImage, int? ProductImageId)
        {
            // b1 : xoá ảnh trong folder
            // b2 : xoá ảnh trong db
            // b3 : xoá trong List
            var List = ListImage.Where(p => p.ProductImageId != ProductImageId).ToList();
            // b2 : xoá ảnh trong db
            var modelDelete = _context.ProductImageModel.Find(ProductImageId);
            if (modelDelete != null)
            {
                _context.Entry(modelDelete).State = System.Data.Entity.EntityState.Deleted;
                _context.SaveChanges();
            }
            return PartialView("_EditListImageInner", List);
        }
        #endregion

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditProduct(ProductViewModel model, HttpPostedFileBase file, List<ProductAlertViewModel> QtyAlertList = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        ProductModel modelUpdate = new ProductModel();
                        Mapper.CreateMap<ProductViewModel, ProductModel>();
                        modelUpdate = Mapper.Map<ProductModel>(model);
                        modelUpdate.SEOProductName = Library.ConvertToNoMarkString(modelUpdate.ProductName);
                        modelUpdate.LastModifiedAccount = currentAccount.UserName;
                        modelUpdate.LastModifiedDate = DateTime.Now;
                        modelUpdate.ProductCode = string.IsNullOrEmpty(model.ProductCode) ? "" : model.ProductCode;
                        modelUpdate.COGS = model.ImportPrice * model.ExchangeRate + model.ShippingFee;
                        int CountProductStoreCode = _context.ProductModel.Count(p => p.ProductStoreCode == model.ProductStoreCode);
                        if (model.ProductStoreCodeMark != model.ProductStoreCode || model.ProductStoreCode == null || CountProductStoreCode >= 2)
                        {
                            ProductRepository ProductRepository = new ProductRepository(_context);
                            if (model.ProductStoreCode == null)
                            {
                                modelUpdate.ProductStoreCode = ProductRepository.GetProdcutStoreCode(model.StoreId.Value, model.ProductTypeId.Value, model.CategoryId);
                            }
                            else
                            {
                                modelUpdate.ProductStoreCode = ProductRepository.GetProdcutStoreCodeDuplicate(model.StoreId.Value, model.ProductTypeId.Value, model.CategoryId, model.ProductStoreCode);
                            }
                        }

                        #region Sửa file ảnh
                        if (file != null)
                        {
                            string filename = file.FileName;
                            int index = filename.IndexOf(".");
                            string type = filename.Substring(index);

                            int size = file.ContentLength;
                            string ContentType = file.ContentType;

                            byte[] FileContent = PathToByteArray(file.InputStream);
                            modelUpdate.ImageUrl = Upload(file, "Product");

                            //lưu
                            SYS_tblFile FileSave = new SYS_tblFile()
                            {
                                FileTitle = filename,
                                FileName = modelUpdate.ImageUrl,
                                Extension = type,
                                ContentType = ContentType,
                                FileContent = FileContent,
                                FolderId = 1,
                                Size = size,
                                CreatedByUserId = currentAccount.EmployeeId,
                                CreatedOnDate = modelUpdate.CreatedDate
                            };
                            _context.Entry(FileSave).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                            modelUpdate.FileId = FileSave.FileId;
                        }
                        #endregion

                        _context.Entry(modelUpdate).State = System.Data.Entity.EntityState.Modified;

                        #region Sửa các loại giá
                        // Lấy ProductPriceID
                        var price1 = _context.ProductPriceModel
                                        .Where(p => p.CustomerLevelId == 1 && p.ProductId == modelUpdate.ProductId)
                                        .FirstOrDefault();
                        price1.Price = model.Price1;

                        _context.Entry(price1).State = System.Data.Entity.EntityState.Modified;
                        //Product Price 2
                        var price2 = _context.ProductPriceModel
                                         .Where(p => p.CustomerLevelId == 2 && p.ProductId == modelUpdate.ProductId)
                                         .FirstOrDefault();
                        price2.Price = model.Price2;
                        _context.Entry(price2).State = System.Data.Entity.EntityState.Modified;
                        //Product Price 3
                        var price3 = _context.ProductPriceModel
                                        .Where(p => p.CustomerLevelId == 3 && p.ProductId == modelUpdate.ProductId)
                                        .FirstOrDefault();
                        price3.Price = model.Price3;
                        _context.Entry(price3).State = System.Data.Entity.EntityState.Modified;

                        //Product Price 4
                        var price4 = _context.ProductPriceModel
                                        .Where(p => p.CustomerLevelId == 4 && p.ProductId == modelUpdate.ProductId)
                                        .FirstOrDefault();
                        price4.Price = model.Price4;
                        _context.Entry(price4).State = System.Data.Entity.EntityState.Modified;

                        #endregion

                        #region Duyệt tồn kho cảnh báo

                        #region Xoá những ProductAlert cũ
                        var ListOldProductAlert = _context.ProductAlertModel.Where(p => p.ProductId == modelUpdate.ProductId).ToList();
                        foreach (var item in ListOldProductAlert)
                        {
                            _context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                            _context.SaveChanges();
                        }
                        #endregion

                        #region Insert ProductAlert mới
                        if (QtyAlertList != null)
                        {
                            if (QtyAlertList.GroupBy(p => p.WarehouseId).ToList().Count < QtyAlertList.Count)
                            {
                                return Content("Vui lòng không chọn thông tin 'Kho' trùng nhau khi cài đặt cảnh báo tồn kho");
                            }
                            if (QtyAlertList.Where(p => (p.QtyAlert == 0 || p.QtyAlert == null)).FirstOrDefault() != null)
                            {
                                return Content("Vui lòng nhập thông tin 'Số lượng' lớn hơn 0 khi cài đặt cảnh báo tồn kho");
                            }
                           

                            foreach (var item in QtyAlertList)
                            {
                                ProductAlertModel modelQtyAlert = new ProductAlertModel()
                                {
                                    ProductId = modelUpdate.ProductId,
                                    WarehouseId = item.WarehouseId,
                                    RolesId = item.RolesId,
                                    QtyAlert = item.QtyAlert
                                };
                                _context.Entry(modelQtyAlert).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();
                            }
                        }
                        #endregion

                        #endregion

                        _context.SaveChanges();
                        ts.Complete(); // đánh dấu Transaction hoàn tất
                        return Content("success");
                    }
                }
                else
                {
                    string ErrorMessage = ": <br />";
                    foreach (ModelState modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            ErrorMessage += error.ErrorMessage + "<br />";
                        }
                    }

                    //ViewBag.Error = ex.Message;
                    CreateViewBag(model.CategoryId, model.OriginOfProductId, model.PolicyInStockId, model.PolicyOutOfStockId, model.LocationOfProductId, model.ProductStatusId, model.UnitId, model.CurrencyId);
                    return Content(Resources.LanguageResource.EditErrorMessage + ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                CreateViewBag(model.CategoryId, model.OriginOfProductId, model.PolicyInStockId, model.PolicyOutOfStockId, model.LocationOfProductId, model.ProductStatusId, model.UnitId, model.CurrencyId);
                string ErrorMessage = string.Format("{0} {1}",
                                                             Resources.LanguageResource.EditErrorMessage,
                                                             ex.Message);
                return Content(Resources.LanguageResource.AddErrorMessage);
            }
        }
        #endregion

        #region Thông tin chi tiết sản phẩm
        
        public ActionResult Details(int id)
        {
            ProductModel modeladd = _context.ProductModel
                                                        .Where(p => p.ProductId == id)
                                                        .FirstOrDefault();
            ProductViewModel model = new ProductViewModel();
            if (modeladd == null)
            {
                return HttpNotFound();
            }
            else
            {
                Mapper.CreateMap<ProductModel, ProductViewModel>();
                model = Mapper.Map<ProductViewModel>(modeladd);

                ProductPriceModel price1 = _context.ProductPriceModel
                                                                      .Where(p => p.ProductId == modeladd.ProductId && p.CustomerLevelId == 1)
                                                                       .FirstOrDefault();
                if (price1 != null && price1.Price.HasValue)
                {
                    model.Price1 = price1.Price.Value;
                }
                ProductPriceModel price2 = _context.ProductPriceModel
                                                                     .Where(p => p.ProductId == modeladd.ProductId && p.CustomerLevelId == 2)
                                                                      .FirstOrDefault();
                if (price2 != null && price2.Price.HasValue)
                {
                    model.Price2 = price2.Price.Value;
                }
                ProductPriceModel price3 = _context.ProductPriceModel
                                                                     .Where(p => p.ProductId == modeladd.ProductId && p.CustomerLevelId == 3)
                                                                      .FirstOrDefault();
                if (price3 != null && price3.Price.HasValue)
                {
                    model.Price3 = price3.Price.Value;
                }

                ProductPriceModel price4 = _context.ProductPriceModel
                                                                     .Where(p => p.ProductId == modeladd.ProductId && p.CustomerLevelId == 4)
                                                                      .FirstOrDefault();
                if (price4 != null && price4.Price.HasValue)
                {
                    model.Price4 = price4.Price.Value;
                }
            }
            CreateViewBag(model.CategoryId, model.OriginOfProductId, model.PolicyInStockId, model.PolicyOutOfStockId, model.LocationOfProductId, model.ProductStatusId, model.UnitId, model.CurrencyId);
            return View(model);
        }
        #endregion

        public ProductModel modelUpdate { get; set; }

        #region GetProductId
        public ActionResult GetProductId(string q)
        {
            var data2 = _context
                        .ProductModel
                        .Where(p => q == null || ((string.IsNullOrEmpty(p.ProductCode) ? "" : p.ProductCode) + "  " + p.ProductName).Contains(q))
                        .Select(p => new
                        {
                            value = p.ProductId,
                            text = ((string.IsNullOrEmpty(p.ProductCode) ? "" : p.ProductCode) + " | " + p.ProductName)
                        })
                        .Take(10)
                        .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }
        #endregion

        
        public ActionResult Create()
        {
            return View();
        }
        
        public ActionResult Edit(int id)
        {
            ProductModel modelUpdate = _context.ProductModel
                                                         .Where(p => p.ProductId == id)
                                                         .FirstOrDefault();
            ProductViewModel model = new ProductViewModel();
            if (modelUpdate == null)
            {
                return HttpNotFound();
            }

            else
            {
                Mapper.CreateMap<ProductModel, ProductViewModel>();
                model = Mapper.Map<ProductViewModel>(modelUpdate);

                ProductPriceModel price1 = _context.ProductPriceModel
                                                                      .Where(p => p.ProductId == modelUpdate.ProductId && p.CustomerLevelId == 1)
                                                                       .FirstOrDefault();
                // Nếu có pricemodel và có giá trị Price thì tiens hành lấy
                if (price1 != null && price1.Price.HasValue)
                {
                    model.Price1 = price1.Price.Value;
                }
                ProductPriceModel price2 = _context.ProductPriceModel
                                                                     .Where(p => p.ProductId == modelUpdate.ProductId && p.CustomerLevelId == 2)
                                                                      .FirstOrDefault();
                if (price2 != null && price2.Price.HasValue)
                {
                    model.Price2 = price2.Price.Value;
                }
                ProductPriceModel price3 = _context.ProductPriceModel
                                                                     .Where(p => p.ProductId == modelUpdate.ProductId && p.CustomerLevelId == 3)
                                                                      .FirstOrDefault();
                if (price3 != null && price3.Price.HasValue)
                {
                    model.Price3 = price3.Price.Value;
                }

                ProductPriceModel price4 = _context.ProductPriceModel
                                                                     .Where(p => p.ProductId == modelUpdate.ProductId && p.CustomerLevelId == 4)
                                                                      .FirstOrDefault();
                if (price4 != null && price4.Price.HasValue)
                {
                    model.Price4 = price4.Price.Value;
                }

                if (model.FileId != null)
                {
                    ViewBag.pathImage = GetPath(model.FileId);
                }
                else if (System.IO.File.Exists(Server.MapPath("/Upload/Product/Thum/" + model.ImageUrl)))
                {
                    if (model.ImageUrl != "noimage.jpg")
                    {
                        using (TransactionScope ts = new TransactionScope())
                        {
                            string filepath = Server.MapPath("/Upload/Product/Thum/" + model.ImageUrl);
                            string filename = model.ImageUrl;
                            int index = filename.IndexOf(".");
                            string type = filename.Substring(index);
                            string type2 = filename.Substring(index + 1);
                            string ContentType = "Image/" + type2;

                            Image myImg = Image.FromFile(filepath);
                            //System.IO.FileInfo info = new System.IO.FileInfo(filepath);

                            byte[] FileContent = imageToByteArray(myImg);
                            int size = FileContent.Length;

                            //addmodel.ImageUrl = Upload(file, "Product");

                            SYS_tblFile FileSave = new SYS_tblFile()
                            {
                                FileTitle = filename,
                                FileName = model.ImageUrl,
                                Extension = type,
                                ContentType = ContentType,
                                FileContent = FileContent,
                                FolderId = 1,
                                Size = size,
                                CreatedByUserId = currentAccount.EmployeeId,
                                CreatedOnDate = DateTime.Now
                            };
                            _context.Entry(FileSave).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                            model.FileId = FileSave.FileId;
                            modelUpdate.FileId = FileSave.FileId;
                            //Code cũ của nga
                            //_context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                            //_context.SaveChanges();
                            _context.Entry(modelUpdate).State = System.Data.Entity.EntityState.Modified;
                            _context.SaveChanges();
                            ts.Complete();
                        }

                    }
                    ViewBag.pathImage = "/Upload/Product/Thum/" + model.ImageUrl;
                }
            }

            return View(model);
        }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }
        }

        public ActionResult PrintBarCode(int ProductId)
        {
            var model = (from p in _context.ProductModel
                         join Or in _context.OriginOfProductModel on p.OriginOfProductId equals Or.OriginOfProductId into tmpt
                         from tmpt2 in tmpt.DefaultIfEmpty()
                         where p.ProductId == ProductId
                         select new ProductInfoViewModel()
                         {
                             ProductName = p.ProductName.Substring(0, 40),
                             ProductId = p.ProductId,
                             ProductCode = p.ProductCode,
                             LastModifiedDate = p.ImportDate,
                             OriginOfProductName = tmpt2.OriginOfProductName
                         }).FirstOrDefault();
            // Nếu chưa có hình ảnh Barcode thì tạo mới
            string path_root = Server.MapPath("~/Upload/BarCode/" + model.ProductId.ToString().PadLeft(8, '0') + ".png");
            if (!System.IO.File.Exists(path_root))
            {
                GetBacode(model.ProductId.ToString().PadLeft(8, '0'));
            }
            ViewBag.BarCodeUrl = "~/Upload/BarCode/" + model.ProductId.ToString().PadLeft(8, '0') + ".png";
            return PartialView(model);
        }

        private void GetBacode(string BarCode)
        {
            BarcodeSymbology s = BarcodeSymbology.Code128;
            BarcodeDraw drawObject = BarcodeDrawFactory.GetSymbology(s);
            var metrics = drawObject.GetDefaultMetrics(26);
            metrics.Scale = 1;
            var barcodeImage = drawObject.Draw(BarCode, metrics);
            string path = Server.MapPath("~/Upload/BarCode/" + BarCode + ".png");
            barcodeImage.Save(path, System.Drawing.Imaging.ImageFormat.Png);
        }

        public byte[] PathToByteArray(Stream inputStream)
        {
            byte[] data;
            MemoryStream memoryStream = inputStream as MemoryStream;
            if (memoryStream == null)
            {
                memoryStream = new MemoryStream();
                inputStream.CopyTo(memoryStream);
            }
            data = memoryStream.ToArray();
            return data;
        }

        public string GetPath(int? FileId)
        {
            var FileSave = _context.SYS_tblFile.Find(FileId);

            string pathFolder = _context.SYS_tblFolder
                        .Where(p => p.FolderId == FileSave.FolderId)
                        .Select(p => p.FolderPath)
                        .FirstOrDefault();

            int index = FileSave.Extension.IndexOf(".") + 1;
            string type = FileSave.Extension.Substring(index);

            string pathFile = Server.MapPath("~" + pathFolder + "/" + FileSave.FileName);

            string pathReturn = pathFolder + "/Thum/" + FileSave.FileName;
            string pathFileThum = Server.MapPath("~" + pathReturn);

            // Nếu Folder ko tồn tại thì tạo Folder
            if (!Directory.Exists(Server.MapPath(pathFolder)))
            {
                //Tạo Folder
                Directory.CreateDirectory(Server.MapPath(pathFolder));

                //Lưu file
                if (!Directory.Exists(Server.MapPath(pathFolder + "/Thum")))
                {
                    Directory.CreateDirectory(Server.MapPath(pathFolder + "/Thum"));
                }

                //pathFile
                MemoryStream ms = new MemoryStream(FileSave.FileContent);
                Image Img = Image.FromStream(ms);
                SaveImage(Img, type, pathFile);

                //pathFileThum
                MemoryStream ms2 = new MemoryStream(FileSave.FileContent);
                Image Img2 = Image.FromStream(ms2);
                SaveImageThum(Img2, type, pathFileThum);

                return pathReturn;
            }
            else
            {

                if (!Directory.Exists(Server.MapPath(pathFolder + "/Thum")))
                {
                    Directory.CreateDirectory(Server.MapPath(pathFolder + "/Thum"));
                }

                if (!System.IO.File.Exists(pathFile))
                {
                    MemoryStream ms = new MemoryStream(FileSave.FileContent);
                    Image Img = Image.FromStream(ms);
                    SaveImage(Img, type, pathFile);
                }

                if (!System.IO.File.Exists(pathFileThum))
                {
                    MemoryStream ms2 = new MemoryStream(FileSave.FileContent);
                    Image Img2 = Image.FromStream(ms2);
                    SaveImageThum(Img2, type, pathFileThum);
                }

                return pathReturn;

            }
        }

        public string GetPathWidth(int? FileId, int Width)
        {
            var FileSave = _context.SYS_tblFile.Find(FileId);

            string pathFolder = _context.SYS_tblFolder
                        .Where(p => p.FolderId == FileSave.FolderId)
                        .Select(p => p.FolderPath)
                        .FirstOrDefault();

            int index = FileSave.Extension.IndexOf(".") + 1;
            string type = FileSave.Extension.Substring(index);

            string pathFile = Server.MapPath("~" + pathFolder + "/" + FileSave.FileName);

            string pathReturn = pathFolder + "/Thum/" + FileSave.FileName;
            string pathFileThum = Server.MapPath("~" + pathReturn);

            // Nếu Folder ko tồn tại thì tạo Folder
            if (!Directory.Exists(pathFolder))
            {
                //Tạo Folder
                Directory.CreateDirectory(Server.MapPath(pathFolder));

                //Lưu file
                if (!Directory.Exists(Server.MapPath(pathFolder + "/Thum")))
                {
                    Directory.CreateDirectory(Server.MapPath(pathFolder + "/Thum"));
                }

                //pathFile
                MemoryStream ms = new MemoryStream(FileSave.FileContent);
                Image Img = Image.FromStream(ms);
                SaveImage(Img, type, pathFile);

                //pathFileThum
                MemoryStream ms2 = new MemoryStream(FileSave.FileContent);
                Image Img2 = Image.FromStream(ms2);
                SaveImageWidth(Img2, type, pathFileThum, Width);

                return pathReturn;
            }
            else
            {

                if (!Directory.Exists(Server.MapPath(pathFolder + "/Thum")))
                {
                    Directory.CreateDirectory(Server.MapPath(pathFolder + "/Thum"));
                }

                if (!System.IO.File.Exists(pathFile))
                {
                    MemoryStream ms = new MemoryStream(FileSave.FileContent);
                    Image Img = Image.FromStream(ms);
                    SaveImage(Img, type, pathFile);
                }

                if (!System.IO.File.Exists(pathFileThum))
                {
                    MemoryStream ms2 = new MemoryStream(FileSave.FileContent);
                    Image Img2 = Image.FromStream(ms2);
                    SaveImageWidth(Img2, type, pathFileThum, Width);
                }

                return pathReturn;

            }
        }

        public string GetPathWidthHeight(int? FileId, int Width, int Height)
        {
            var FileSave = _context.SYS_tblFile.Find(FileId);

            string pathFolder = _context.SYS_tblFolder
                        .Where(p => p.FolderId == FileSave.FolderId)
                        .Select(p => p.FolderPath)
                        .FirstOrDefault();

            int index = FileSave.Extension.IndexOf(".") + 1;
            string type = FileSave.Extension.Substring(index);

            string pathFile = Server.MapPath("~" + pathFolder + "/" + FileSave.FileName);

            string pathReturn = pathFolder + "/Thum/" + FileSave.FileName;
            string pathFileThum = Server.MapPath("~" + pathReturn);

            // Nếu Folder ko tồn tại thì tạo Folder
            if (!Directory.Exists(pathFolder))
            {
                //Tạo Folder
                Directory.CreateDirectory(Server.MapPath(pathFolder));

                //Lưu file
                if (!Directory.Exists(Server.MapPath(pathFolder + "/Thum")))
                {
                    Directory.CreateDirectory(Server.MapPath(pathFolder + "/Thum"));
                }

                //pathFile
                MemoryStream ms = new MemoryStream(FileSave.FileContent);
                Image Img = Image.FromStream(ms);
                SaveImage(Img, type, pathFile);

                //pathFileThum
                MemoryStream ms2 = new MemoryStream(FileSave.FileContent);
                Image Img2 = Image.FromStream(ms2);
                SaveImageWidthHeight(Img2, type, pathFileThum, Width, Height);

                return pathReturn;
            }
            else
            {

                if (!Directory.Exists(Server.MapPath(pathFolder + "/Thum")))
                {
                    Directory.CreateDirectory(Server.MapPath(pathFolder + "/Thum"));
                }

                if (!System.IO.File.Exists(pathFile))
                {
                    MemoryStream ms = new MemoryStream(FileSave.FileContent);
                    Image Img = Image.FromStream(ms);
                    SaveImageWidthHeight(Img, type, pathFile, Width, Height);
                }

                if (!System.IO.File.Exists(pathFileThum))
                {
                    MemoryStream ms2 = new MemoryStream(FileSave.FileContent);
                    Image Img2 = Image.FromStream(ms2);
                    SaveImageWidthHeight(Img2, type, pathFileThum, Width, Height);
                }

                return pathReturn;

            }
        }

        public void SaveImageWidth(Image img, string type, string pathFileThum, int Width)
        {

            //int maxWidth = 1600, maxHeight = 1600;
            int w = img.Width;
            int h = img.Height;

            if (w > Width)
            {
                double ratio = (double)h / w;
                int Height = Convert.ToInt32(ratio * Width);
                var newImage = new Bitmap(Width, Height);

                using (var graphics = Graphics.FromImage(newImage))
                    graphics.DrawImage(img, 0, 0, Width, Height);
                newImage.Save(pathFileThum);
            }
            else
            {
                var newImage = new Bitmap(w, h);

                using (var graphics = Graphics.FromImage(newImage))
                    graphics.DrawImage(img, 0, 0, w, h);

                newImage.Save(pathFileThum);
            }
        }

        public void SaveImageWidthHeight(Image img, string type, string pathFileThum, int Width, int Height)
        {

            //int maxWidth = 1600, maxHeight = 1600;
            int w = img.Width;
            int h = img.Height;

            if (w > Width || h > Height)
            {
                var newImage = new Bitmap(Width, Height);
                using (var graphics = Graphics.FromImage(newImage))
                    graphics.DrawImage(img, 0, 0, Width, Height);
                newImage.Save(pathFileThum);
            }
            else
            {
                var newImage = new Bitmap(w, h);

                using (var graphics = Graphics.FromImage(newImage))
                    graphics.DrawImage(img, 0, 0, w, h);

                newImage.Save(pathFileThum);
            }
        }

        public void SaveImage(Image img, string type, string pathFile)
        {

            int maxWidth = 1600, maxHeight = 1600;
            if (type.ToLower() != "gif" && type.ToLower() != "png")
            {
                int w = img.Width;
                int h = img.Height;

                //save to root folder
                if (w >= maxWidth || h >= maxHeight)
                {
                    double ratio = (double)h / w;
                    int Height = Convert.ToInt32(ratio * maxWidth);
                    var newImage = new Bitmap(maxWidth, Height);

                    using (var graphics = Graphics.FromImage(newImage))
                        graphics.DrawImage(img, 0, 0, maxWidth, Height);
                    newImage.Save(pathFile);
                }
                else
                {
                    var newImage = new Bitmap(w, h);

                    using (var graphics = Graphics.FromImage(newImage))
                        graphics.DrawImage(img, 0, 0, w, h);

                    newImage.Save(pathFile);
                }
            }
            else
            {
                img.Save(pathFile);
            }
        }

        public void SaveImageThum(Image img, string type, string pathFileThum)
        {

            int minWidth = 250;
            int minHeight = 500;
            //int maxWidth = 1600;

            if (type.ToLower() != "gif" && type.ToLower() != "png")
            {
                int w = img.Width;
                int h = img.Height;
                //save to root folder
                if (w >= minWidth || h >= minHeight)
                {
                    double ratio = (double)h / w;
                    int Height = Convert.ToInt32(ratio * minWidth);

                    var newImage = new Bitmap(minWidth, Height);
                    using (var graphics = Graphics.FromImage(newImage))
                        graphics.DrawImage(img, 0, 0, minWidth, Height);
                    newImage.Save(pathFileThum);
                }
                else
                {
                    var newImage = new Bitmap(w, h);

                    using (var graphics = Graphics.FromImage(newImage))
                        graphics.DrawImage(img, 0, 0, w, h);
                    newImage.Save(pathFileThum);
                }
            }
            else
            {
                img.Save(pathFileThum);
            }
        }


        #region Số lượng cảnh báo
        public ActionResult _QtyAlertList(List<ProductAlertViewModel> QtyAlertList = null, int ProductId = 0)
        {
            if (ProductId != 0)
            {
                QtyAlertList = _context.ProductAlertModel.Where(p => p.ProductId == ProductId)
                    .Select(p => new ProductAlertViewModel()
                            {
                                WarehouseId = p.WarehouseId,
                                RolesId = p.RolesId,
                                QtyAlert = p.QtyAlert
                            }).ToList();
            }
            if (QtyAlertList == null)
            {
                QtyAlertList = new List<ProductAlertViewModel>();
            }
            CreateQtyAlertViewBag();
            return PartialView(QtyAlertList);
        }

        public ActionResult _QtyAlertListInner(List<ProductAlertViewModel> QtyAlertList = null)
        {
            if (QtyAlertList == null)
            {
                QtyAlertList = new List<ProductAlertViewModel>();
            }
            ProductAlertViewModel item = new ProductAlertViewModel();
            QtyAlertList.Add(item);
            CreateQtyAlertViewBag();
            return PartialView(QtyAlertList);
        }

        public ActionResult _DeletelistInnerQtyAlertList(List<ProductAlertViewModel> QtyAlertList, int RemoveId)
        {
            if (QtyAlertList == null)
            {
                QtyAlertList = new List<ProductAlertViewModel>();
            }
            CreateQtyAlertViewBag();
            return PartialView("_QtyAlertListInner", QtyAlertList.Where(p => p.STT != RemoveId).ToList());
        }

        private void CreateQtyAlertViewBag()
        {
            ViewBag.WarehouseList = (_context.WarehouseModel.Where(p => p.Actived == true).Select(p => new WareHouseViewModel()
            {
                WarehouseId = p.WarehouseId,
                WarehouseName = p.WarehouseCode
            })).ToList();
            ViewBag.RoleList = (_context.RolesModel.Where(p => p.Actived == true).Select(p => new RoleViewModel()
            {
                RolesId = p.RolesId,
                RolesName = p.RolesName
            })).ToList();
        }
        #endregion

        #region Export
        public ActionResult Export()
        {
            var category = _context.CategoryModel.Find(2);
            var list = (from p in _context.ProductModel
                        join c in _context.CategoryModel on p.CategoryId equals c.CategoryId
                        join o in _context.OriginOfProductModel on p.OriginOfProductId equals o.OriginOfProductId into OriginOfProduct
                        from oo in OriginOfProduct.DefaultIfEmpty()
                        select new ProductInfoViewModel()
                        {
                            Barcode = p.ProductCode,
                            ProductId = p.ProductId,
                            ProductCode = p.ProductCode,
                            ProductStoreCode = p.ProductStoreCode,
                            ProductName = p.ProductName,
                            CategoryName = c.CategoryName,
                            OriginOfProductName = oo.OriginOfProductName,
                            ImageUrl = p.ImageUrl
                        }).OrderByDescending(p => p.ProductId)
                        .ToList();

  
            //Tên file
            string strName = string.Format("{0}_{1}_{2}", "Danh_sach_san_pham", DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss"), ".xlsx");
            Byte[] report = GenerateReport(list);

            string handle = Guid.NewGuid().ToString();

            Session[handle] = report;
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = strName }
            };
        }

        private Byte[] GenerateReport(List<ProductInfoViewModel> list)
        {
            using (ExcelPackage p = new ExcelPackage())
            {
                //set the workbook properties and add a default sheet in it
                SetWorkbookProperties(p);
                //Create a sheet
                ExcelWorksheet ws = CreateSheet(p, "Danh sách sản phẩm");

                ws.Cells[2, 1].Value = "DANH SACH SAN PHAM";
                ws.Cells[2, 1, 2, 6].Merge = true;
                ws.Cells[2, 1, 2, 6].Style.Font.Bold = true;
                ws.Cells[2, 1, 2, 6].Style.Font.Size = 22;
                ws.Cells[2, 1, 2, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                int rowIndex = 4;
                int row = 5;
                CreateHeader(ws, ref rowIndex);
                CreateData(ws, ref row, list);
                Byte[] bin = p.GetAsByteArray();

                return bin;

                // return File(bin, "application/vnd.ms-excel", log.FileName);
            }
        }

        private static void SetWorkbookProperties(ExcelPackage p)
        {
            //Here setting some document properties
            p.Workbook.Properties.Author = "NgaNguyen";
            p.Workbook.Properties.Title = "Danh sách sản phẩm";
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
            worksheet.Column(3).Width = 15;
            worksheet.Column(4).Width = 15;
            worksheet.Column(5).Width = 15;
            worksheet.Column(6).Width = 15;
            //format date cột i

            //tiêu đề cột
            worksheet.Cells[rowIndex, 1].Value = "STT";
            worksheet.Cells[rowIndex, 2].Value = "Tên sản phẩm";
            worksheet.Cells[rowIndex, 3].Value = "Quy cách";
            worksheet.Cells[rowIndex, 4].Value = "Đơn vị tính";
            worksheet.Cells[rowIndex, 5].Value = "Giá nhập";
            worksheet.Cells[rowIndex, 6].Value = "Giá bán";

            var cell = worksheet.Cells[rowIndex, 1];
            var fill = cell.Style.Fill;
            var border = cell.Style.Border;

            for (int i = 1; i <= 6; i++)
            {
                cell = worksheet.Cells[rowIndex, i];
                fill = cell.Style.Fill;
                fill.PatternType = ExcelFillStyle.Solid;
                fill.BackgroundColor.SetColor(Color.Gray);
                border = cell.Style.Border;
                border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
            }
            #endregion
        }

        private static void CreateData(ExcelWorksheet worksheet, ref int rowIndex, List<ProductInfoViewModel> listProduct)
        {
            EntityDataContext db = new EntityDataContext();
            foreach (ProductInfoViewModel p in listProduct)
            {
                for (int i = 1; i <= 6; i++)
                {
                    var cell2 = worksheet.Cells[rowIndex, i];
                    cell2.Style.Border.Bottom.Style = cell2.Style.Border.Top.Style = cell2.Style.Border.Left.Style = cell2.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                }
                worksheet.Cells[rowIndex, 1].Value = rowIndex - 8;
                worksheet.Cells[rowIndex, 2].Value = p.ProductName;
                worksheet.Cells[rowIndex, 3].Value = p.Specifications;
                worksheet.Cells[rowIndex, 4].Value = p.UnitName;
                worksheet.Cells[rowIndex, 5].Value = p.ImportPrice;
                worksheet.Cells[rowIndex, 6].Value = p.Price1;
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

    }
}
