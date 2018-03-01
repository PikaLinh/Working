using AutoMapper;
using Constant;
using EntityModels;
using Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using ViewModels;

using Zen.Barcode;

using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace WebUI.Controllers
{
    public class DynamicProductController : BaseController
    {
        protected bool isUseStoreId = false;
        protected bool isUseShortProductName = false;
        protected bool isUseOriginOfProductId = false; // xuất xứ chuyển thành tông màu.
        protected bool isUsePolicyInStockId = false;
        protected bool isUsePolicyOutOfStockId = false;
        protected bool isUseLocationOfProductIdd = false;
        protected bool isUseProductStatusId = false;
        protected bool isUseKeywords = false;
        protected bool isUseBarcode = true;
        protected bool isUseImportDate = false;
        protected bool isUseCurrencyId = false;
        protected bool isUseExchangeRate = false;
        protected bool isUseShippingFee = false;
        protected bool isUseBeginWarehouseId = false;
        protected bool isUseActived = true;


        // GET: DynamicProduct
        #region Danh sách sản phẩm
        
        public ActionResult Index()
        {
            CreateIndexViewBag();
            ProductSearchViewModel model = new ProductSearchViewModel();
            return View(model);
        }
        public ActionResult _SearchPartial(ProductSearchViewModel model, int CategoryId = 2, int PageIndex = 1, int PageSize = 10)
        {
            #region GetImage
            var listFileId = _context.ProductModel.Where(p => p.FileId != null && p.ImageUrl != "noimage.jpg").ToList();
            foreach (var item in listFileId)
            {
                string filepath = Server.MapPath("/Upload/Product/Thum/" + item.ImageUrl);
                if (!System.IO.File.Exists(filepath))
                {
                    GetPath(@item.FileId);
                }
            }
            #endregion

            var category = _context.CategoryModel.Find(CategoryId);
            //Sản phẩm
            var list = (from p in _context.ProductModel
                        //Danh mục 
                        join c in _context.CategoryModel on p.CategoryId equals c.CategoryId
                        //Xuất xứ
                        join o in _context.OriginOfProductModel on p.OriginOfProductId equals o.OriginOfProductId into OriginOfProduct
                        from oo in OriginOfProduct.DefaultIfEmpty()
                        //Lọc
                        where (model.ProductName == null || p.ProductName.Contains(model.ProductName)) &&
                               (c.ADNCode.StartsWith(category.ADNCode)) &&
                               (model.ProductCode == null || p.ProductCode.Contains(model.ProductCode)) &&
                               (model.Specifications == null || p.Specifications.Contains(model.Specifications)) &&
                               (model.OriginOfProductId == null || p.OriginOfProductId == model.OriginOfProductId) &&
                            // ((model.Valid == true && p.ProductStoreCode != null) || (model.Valid == false && p.ProductStoreCode == null)) &&
                               (model.Actived == p.Actived) &&
                               (
                                 (model.ProductStatust == 0) || // load ra tất cả. 
                                 ((model.ProductStatust == 1) && (p.IsProduct == true)) ||
                                 ((model.ProductStatust == 2) && (p.IsMaterial == true))
                               ) &&
                               (
                                  // 1. null => tất cả
                                 (model.isHasBarcode == null) ||
                                 // 2. true => có mã vạch
                                 ((model.isHasBarcode == true) && (p.Barcode != null)) ||
                                 // 3. false => chưa có mã vạch
                                 ((model.isHasBarcode == false) && (p.Barcode == null))
                               )
                               &&
                               (
                            // 1. null => tất cả
                                 (model.isParentProduct == null) ||
                            // 2. true => là sản phẩm cha
                                 ((model.isParentProduct == true) && (p.isParentProduct == true)) ||
                            // 3. false => là sản phẩm con
                                 ((model.isParentProduct == false) && (p.ParentProductId != null))
                               )
                        select new ProductInfoViewModel()
                        {
                            Barcode = p.ProductCode,
                            ProductId = p.ProductId,
                            ProductCode = p.ProductCode,
                            ProductStoreCode = p.ProductStoreCode,
                            ProductName = p.ProductName,
                            CategoryName = c.CategoryName,
                            OriginOfProductName = oo.OriginOfProductName,
                            ImageUrl = p.ImageUrl,
                            Specifications = p.Specifications
                        });

            list = list.OrderByDescending(p => p.ProductId);

            ViewBag.TotalRow = list.Count();
            ViewBag.RowIndex = (PageIndex - 1) * PageSize;
            return PartialView(list.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList());
        }
        public void CreateIndexViewBag(int? CategoryId = null, int? OriginOfProductId = null, int? ProductStatusId = null)
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

            // 3.
            var ProductStatusNameList = _context.ProductStatusModel.OrderBy(p => p.ProductStatusName).ToList();
            ViewBag.ProductStatusId = new SelectList(ProductStatusNameList, "ProductStatusId", "ProductStatusName", ProductStatusId);

        }

        #endregion

        #region Thêm mới

        
        public ActionResult Create()
        {
            return View();
        }


        public ActionResult _CreateList(List<PriceListViewModel> pricelist = null)
        {
            if (pricelist == null)
            {
                pricelist = new List<PriceListViewModel>();
            }
            return PartialView(pricelist);
        }

        #region _CreatelistInner
        public ActionResult _CreatelistInner(List<PriceListViewModel> pricelist = null)
        {
            if (pricelist == null)
            {
                pricelist = new List<PriceListViewModel>();
                pricelist = (from p in _context.CustomerLevelModel
                             select new PriceListViewModel
                             {
                                 CustomerLevelId = p.CustomerLevelId,
                                 CustomerLevelName = p.CustomerLevelName,
                                 Price = 0,
                             }).ToList();
            }
            else
            {
                PriceListViewModel item = new PriceListViewModel();
                pricelist.Add(item);
            }
            return PartialView(pricelist);
        }
        #endregion

        public ActionResult CreateProduct()
        {
            DynamicProductViewModel productmodel = new DynamicProductViewModel() { Actived = true, CurrencyId = 1, ExchangeRate = 1, BeginInventoryQty = 0, ImportDate = DateTime.Now, ShippingFee = 0, ImportPrice = 0 };
            CreateViewBag(null, null, null, null, null, null, null, null, 1);
            ViewBag.COGS = 0;
            List<PriceListViewModel> pricelist = new List<PriceListViewModel>();
            pricelist = (from p in _context.CustomerLevelModel
                         where p.Actived == true
                         select new PriceListViewModel
                         {
                             CustomerLevelId = p.CustomerLevelId,
                             CustomerLevelName = p.CustomerLevelName,
                             Price = 0,
                         }).ToList();


            ViewBag.pricelist = pricelist;
            return PartialView(productmodel);
        }
        [ValidateInput(false)]
        public ActionResult SaveCreateProduct(DynamicProductViewModel model, HttpPostedFileBase file, List<PriceListViewModel> pricelist)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        //ProductModel
                        ProductModel addmodel = new ProductModel();
                        Mapper.CreateMap<DynamicProductViewModel, ProductModel>();
                        addmodel = Mapper.Map<ProductModel>(model);
                        addmodel.COGS = model.ImportPrice;
                        addmodel.Actived = true;
                        addmodel.SEOProductName = Library.ConvertToNoMarkString(addmodel.ProductName);
                        addmodel.CreatedDate = DateTime.Now;
                        addmodel.CreatedAccount = currentAccount.UserName;
                        if (file != null)
                        {
                            string filename = file.FileName;
                            int index = filename.IndexOf(".");
                            string type = filename.Substring(index);

                            int size = file.ContentLength;
                            string ContentType = file.ContentType;

                            byte[] FileContent = PathToByteArray(file.InputStream);
                            addmodel.ImageUrl = Upload(file, "Product");

                            //lưu
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
                        // ProductRepository ProductRepository = new ProductRepository(_context);
                        //addmodel.ProductStoreCode = ProductRepository.GetDynamicProductStoreCode(model.StoreId.Value, model.CategoryId);
                        addmodel.ProductCode = string.IsNullOrEmpty(model.ProductCode) ? "" : model.ProductCode;
                        // addmodel.COGS = model.ImportPrice * model.ExchangeRate + model.ShippingFee;
                        _context.Entry(addmodel).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();

                        // Lưu vào ProductPriceModel
                        foreach (var item in pricelist)
                        {
                            ProductPriceModel price = new ProductPriceModel()
                            {
                                Price = item.Price,
                                CustomerLevelId = item.CustomerLevelId,
                                ProductId = addmodel.ProductId
                            };
                            _context.Entry(price).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                        }

                        model.CreatedAccount = currentAccount.UserName;
                        AccountModel Account = _context.AccountModel.Where(p => p.UserName == model.CreatedAccount).FirstOrDefault();
                        IEOtherRepository IEOtherRepository = new IEOtherRepository(_context);
                        if (model.BeginInventoryQty != null && model.BeginInventoryQty != 0)
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
                        // đánh dấu Transaction hoàn tất
                        ts.Complete();
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

            ViewBag.isUseStoreId = isUseStoreId;
            ViewBag.isUseShortProductName = isUseShortProductName;
            ViewBag.isUseOriginOfProductId = isUseOriginOfProductId;
            ViewBag.isUsePolicyInStockId = isUsePolicyInStockId;
            ViewBag.isUsePolicyOutOfStockId = isUsePolicyOutOfStockId;
            ViewBag.isUseLocationOfProductIdd = isUseLocationOfProductIdd;
            ViewBag.isUseProductStatusId = isUseProductStatusId;
            ViewBag.isUseKeywords = isUseKeywords;
            ViewBag.isUseBarcode = isUseBarcode;
            ViewBag.isUseImportDate = isUseImportDate;
            ViewBag.isUseCurrencyId = isUseCurrencyId;
            ViewBag.isUseExchangeRate = isUseExchangeRate;
            ViewBag.isUseShippingFee = isUseShippingFee;
            ViewBag.isUseBeginWarehouseId = isUseBeginWarehouseId;
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
            //CategoryList.Insert(0, new { CategoryId = 2, CategoryName = "Sản phẩm" });
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
        public ActionResult UpdateProductStoreCode(int StoreId, int? CategoryId)
        {
            ProductRepository ProductRepository = new ProductRepository(_context);
            string ProductStoreCode = ProductRepository.GetDynamicProductStoreCode(StoreId, CategoryId);
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

        #region Sửa
        public ActionResult Edit(int id)
        {
            ProductModel modeladd = _context.ProductModel
                                                         .Where(p => p.ProductId == id)
                                                         .FirstOrDefault();
            DynamicProductViewModel model = new DynamicProductViewModel();
            if (modeladd == null)
            {
                return HttpNotFound();
            }

            else
            {
                Mapper.CreateMap<ProductModel, DynamicProductViewModel>();
                model = Mapper.Map<DynamicProductViewModel>(modeladd);

                //if (model.FileId != null)
                //{
                //    ViewBag.pathImage = GetPath(model.FileId);
                //}
                //else if (System.IO.File.Exists(Server.MapPath("/Upload/Product/Thum/" + model.ImageUrl)))
                //{
                //    if (model.ImageUrl != "noimage.jpg")
                //    {
                //        using (TransactionScope ts = new TransactionScope())
                //        {
                //            string filepath = Server.MapPath("/Upload/Product/Thum/" + model.ImageUrl);
                //            string filename = model.ImageUrl;
                //            int index = filename.IndexOf(".");
                //            string type = filename.Substring(index);
                //            string type2 = filename.Substring(index + 1);
                //            string ContentType = "Image/" + type2;

                //            Image myImg = Image.FromFile(filepath);
                //            //System.IO.FileInfo info = new System.IO.FileInfo(filepath);

                //            byte[] FileContent = imageToByteArray(myImg);
                //            int size = FileContent.Length;

                //            //addmodel.ImageUrl = Upload(file, "Product");

                //            SYS_tblFile FileSave = new SYS_tblFile()
                //            {
                //                FileTitle = filename,
                //                FileName = model.ImageUrl,
                //                Extension = type,
                //                ContentType = ContentType,
                //                FileContent = FileContent,
                //                FolderId = 1,
                //                Size = size,
                //                CreatedByUserId = currentAccount.EmployeeId,
                //                CreatedOnDate = DateTime.Now
                //            };
                //            _context.Entry(FileSave).State = System.Data.Entity.EntityState.Added;
                //            _context.SaveChanges();
                //            model.FileId = FileSave.FileId;

                //            _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                //            _context.SaveChanges();
                //            ts.Complete();
                //        }

                //    }
                //    ViewBag.pathImage = "/Upload/Product/Thum/" + model.ImageUrl;
                //}

                //pricelist
                var pricelist = (from p in _context.ProductPriceModel
                                 join c in _context.CustomerLevelModel on p.CustomerLevelId equals c.CustomerLevelId
                                 where p.ProductId == id
                                 select new PriceListViewModel
                                 {
                                     CustomerLevelId = p.CustomerLevelId,
                                     CustomerLevelName = c.CustomerLevelName,
                                     Price = p.Price,
                                     STT = p.CustomerLevelId
                                 }).ToList();
                ViewBag.pricelist = pricelist;
            }

            return View(model);
        }
        #region Sửa thông tin sản phẩm
        [HttpGet]
        public ActionResult EditProduct(DynamicProductViewModel model, string pathImage, List<PriceListViewModel> pricelist)
        {
            CreateViewBag(model.BeginWarehouseId, model.StoreId, model.CategoryId, model.OriginOfProductId, model.PolicyInStockId, model.PolicyOutOfStockId, model.LocationOfProductId, model.ProductStatusId, model.UnitId, model.CurrencyId);
            ViewBag.ProductStoreCode = model.ProductStoreCode;
            ViewBag.ProductTypeName = _context.ProductTypeModel.Where(p => p.ProductTypeId == model.ProductTypeId).Select(p => p.ProductTypeCode + "|" + p.ProductTypeName).FirstOrDefault();
            ViewBag.COGS = model.COGS.HasValue ? model.COGS : 0;
            ViewBag.BeginWarehouseName = _context.WarehouseModel.Where(p => p.WarehouseId == model.BeginWarehouseId).Select(p => p.WarehouseName).FirstOrDefault();
            model.ProductStoreCodeMark = model.ProductStoreCode;
            ViewBag.pathImage = pathImage;
            ViewBag.pricelist = pricelist;

            decimal? EndInventoryQty = _context.InventoryDetailModel.Where(p => p.ProductId == model.ProductId).Select(p => p.EndInventoryQty).FirstOrDefault();
            model.EndInventoryQty = EndInventoryQty;
            ViewBag.IdParentProduct = model.ParentProductId ?? 0;
            ViewBag.NameParentProduct = _context.ProductModel.Where(p => p.ProductId == (model.ParentProductId ?? 0)).Select(p => ((string.IsNullOrEmpty(p.ProductCode) ? "" : p.ProductCode) + " | " + p.ProductName)).FirstOrDefault();
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


        public ActionResult _EditList(int ProductId)
        {
            var pricelist = (from p in _context.ProductPriceModel
                             join c in _context.CustomerLevelModel on p.CustomerLevelId equals c.CustomerLevelId
                             where p.ProductId == ProductId
                             select new PriceListViewModel
                             {
                                 CustomerLevelId = p.CustomerLevelId,
                                 CustomerLevelName = c.CustomerLevelName,
                                 Price = p.Price,
                                 STT = p.CustomerLevelId
                             }).ToList();
            ViewBag.Count = pricelist.Count();
            return PartialView(pricelist);
        }
        public ActionResult _EditlistInner(List<PriceListViewModel> pricelist = null, int count = 0)
        {
            if (pricelist.Count() != count)
            {
                PriceListViewModel item = new PriceListViewModel();
                pricelist.Add(item);
            }
            return PartialView(pricelist);
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
        public ActionResult SaveEditProduct(DynamicProductViewModel model, HttpPostedFileBase file, List<PriceListViewModel> pricelist)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ProductModel modelUpdate = new ProductModel();
                    Mapper.CreateMap<DynamicProductViewModel, ProductModel>();
                    modelUpdate = Mapper.Map<ProductModel>(model);
                    modelUpdate.COGS = modelUpdate.ImportPrice;
                    modelUpdate.SEOProductName = Library.ConvertToNoMarkString(modelUpdate.ProductName);
                    modelUpdate.LastModifiedAccount = currentAccount.UserName;
                    modelUpdate.LastModifiedDate = DateTime.Now;
                    modelUpdate.ProductCode = string.IsNullOrEmpty(model.ProductCode) ? "" : model.ProductCode;
                    //modelUpdate.COGS = model.ImportPrice * model.ExchangeRate + model.ShippingFee;
                    //int CountProductStoreCode = _context.ProductModel.Count(p => p.ProductStoreCode == model.ProductStoreCode);
                    //if (model.ProductStoreCodeMark != model.ProductStoreCode || model.ProductStoreCode == null || CountProductStoreCode >= 2)
                    //{
                    //    ProductRepository ProductRepository = new ProductRepository(_context);
                    //    if (model.ProductStoreCode == null)
                    //    {
                    //        modelUpdate.ProductStoreCode = ProductRepository.GetProdcutStoreCode(model.StoreId.Value, model.ProductTypeId.Value, model.CategoryId);
                    //    }
                    //    else
                    //    {
                    //        modelUpdate.ProductStoreCode = ProductRepository.GetProdcutStoreCodeDuplicate(model.StoreId.Value, model.ProductTypeId.Value, model.CategoryId, model.ProductStoreCode);
                    //    }
                    //}
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
                    _context.Entry(modelUpdate).State = System.Data.Entity.EntityState.Modified;
                    // Lưu vào ProductPriceModel
                    foreach (var item in pricelist)
                    {
                        // Lấy ProductPriceID
                        var price = _context.ProductPriceModel
                                        .Where(p => p.CustomerLevelId == item.CustomerLevelId && p.ProductId == modelUpdate.ProductId)
                                        .FirstOrDefault();
                        price.Price = item.Price;
                        _context.Entry(price).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                    }

                    return Content("success");
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

        #endregion

        #region _DeletelistInner
        public ActionResult _DeletelistInner(List<PriceListViewModel> pricelist, int RemoveId)
        {
            if (pricelist == null)
            {
                pricelist = new List<PriceListViewModel>();
            }
            return PartialView("_CreatelistInner", pricelist.Where(p => p.STT != RemoveId).ToList());
        }
        #endregion

        #region BarCode
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

        #endregion

        #region Save Image
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

        public void GetPath(int? FileId)
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

                //return pathReturn;
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

                //return pathReturn;

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


        #endregion

        #region GetCustomerLevelId
        public ActionResult GetCustomerLevelId(string q)
        {
            var data2 = _context
                        .CustomerLevelModel
                        .Where(p => (q == null || p.CustomerLevelName != null && p.Actived == true))
                        .Select(p => new
                        {
                            value = p.CustomerLevelId,
                            text = p.CustomerLevelName,
                        })
                        .Take(10)
                        .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }
        }
        #region Export
        public ActionResult Export()
        {
            var category = _context.CategoryModel.Find(2);
            var list = (from p in _context.ProductModel
                        join c in _context.CategoryModel on p.CategoryId equals c.CategoryId
                        join pp in _context.ProductPriceModel on p.ProductId equals pp.ProductId
                        join u in _context.UnitModel on p.UnitId equals u.UnitId
                        join o in _context.OriginOfProductModel on p.OriginOfProductId equals o.OriginOfProductId into OriginOfProduct
                        from oo in OriginOfProduct.DefaultIfEmpty()
                        select new ProductInfoViewModel()
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            Specifications = p.Specifications,
                            CategoryId = p.CategoryId,
                            CategoryName = c.CategoryName,
                            ShippingWeight = p.ShippingWeight,
                            UnitName = u.UnitName,
                            ImportPrice = p.ImportPrice,
                            Price1 = pp.Price
                        }).OrderBy(p => new { p.CategoryId, p.ProductId })
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

                ws.Cells[2, 1].Value = "DANH SÁCH SẢN PHẨM";
                ws.Cells[2, 1, 2, 7].Merge = true;
                ws.Cells[2, 1, 2, 7].Style.Font.Bold = true;
                ws.Cells[2, 1, 2, 7].Style.Font.Size = 22;
                ws.Cells[2, 1, 2, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

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
            worksheet.Column(2).Width = 35;
            worksheet.Column(3).Width = 15;
            worksheet.Column(4).Width = 10;
            worksheet.Column(5).Width = 10;
            worksheet.Column(6).Width = 15;
            worksheet.Column(7).Width = 15;
            //format date cột i

            //tiêu đề cột
            worksheet.Cells[rowIndex, 1].Value = "STT";
            worksheet.Cells[rowIndex, 2].Value = "Tên sản phẩm";
            worksheet.Cells[rowIndex, 3].Value = "Quy cách";
            worksheet.Cells[rowIndex, 4].Value = "Đơn vị tính";
            worksheet.Cells[rowIndex, 4, rowIndex, 5].Merge = true;
            worksheet.Cells[rowIndex, 4, rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[rowIndex, 6].Value = "Giá nhập";
            worksheet.Cells[rowIndex, 7].Value = "Giá bán";

            var cell = worksheet.Cells[rowIndex, 1];
            var fill = cell.Style.Fill;
            var border = cell.Style.Border;

            for (int i = 1; i <= 7; i++)
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
            int? CategoryId = -1;
            int Index = 1;
            foreach (ProductInfoViewModel p in listProduct)
            {
                if (p.CategoryId != CategoryId)
                {
                    #region Tên danh mục sản phẩm
                    for (int i = 1; i <= 7; i++)
                    {
                        var cell2 = worksheet.Cells[rowIndex, i];
                        cell2.Style.Border.Bottom.Style = cell2.Style.Border.Top.Style = cell2.Style.Border.Left.Style = cell2.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                    worksheet.Cells[rowIndex, 2].Value = p.CategoryName;
                    #endregion

                    #region Sản phẩm đầu danh mục
                    for (int i = 1; i <= 7; i++)
                    {
                        var cell2 = worksheet.Cells[rowIndex + 1, i];
                        cell2.Style.Border.Bottom.Style = cell2.Style.Border.Top.Style = cell2.Style.Border.Left.Style = cell2.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }
                    Index = 1;
                    worksheet.Cells[rowIndex + 1, 1].Value = Index;
                    worksheet.Cells[rowIndex + 1, 2].Value = p.ProductName;
                    worksheet.Cells[rowIndex + 1, 2].Style.WrapText = true;
                    worksheet.Cells[rowIndex + 1, 3].Value = p.Specifications;
                    worksheet.Cells[rowIndex + 1, 4].Value = p.UnitName;
                    worksheet.Cells[rowIndex + 1, 5].Value = p.ShippingWeight;
                    worksheet.Cells[rowIndex + 1, 6].Value = String.Format("{0:n0}", p.ImportPrice);
                    worksheet.Cells[rowIndex + 1, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[rowIndex + 1, 7].Value = String.Format("{0:n0}", p.Price1);
                    worksheet.Cells[rowIndex + 1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    Index++;
                    #endregion

                    rowIndex = rowIndex + 2;
                    CategoryId = p.CategoryId;
                }
                else
                {
                    #region Sản phẩm tiếp theo

                    for (int i = 1; i <= 7; i++)
                    {
                        var cell2 = worksheet.Cells[rowIndex, i];
                        cell2.Style.Border.Bottom.Style = cell2.Style.Border.Top.Style = cell2.Style.Border.Left.Style = cell2.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    worksheet.Cells[rowIndex, 1].Value = Index;

                    worksheet.Cells[rowIndex, 2].Value = p.ProductName;
                    worksheet.Cells[rowIndex, 2].Style.WrapText = true;

                    worksheet.Cells[rowIndex, 3].Value = p.Specifications;
                    worksheet.Cells[rowIndex, 4].Value = p.UnitName;
                    worksheet.Cells[rowIndex, 5].Value = p.ShippingWeight;
                    worksheet.Cells[rowIndex, 6].Value = String.Format("{0:n0}", p.ImportPrice);
                    worksheet.Cells[rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells[rowIndex, 7].Value = String.Format("{0:n0}", p.Price1);
                    worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
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

        //Check barcode already exist
        public JsonResult BarCodeExist(string barCode, int id)
        {
            if (id == 0) // its a new object
            {
                return Json(!_context.ProductModel.Any(x => x.Barcode == barCode), JsonRequestBehavior.AllowGet);
            }
            else // its an existing object so exclude existing objects with the id
            {
                return Json(!_context.ProductModel.Any(x => x.Barcode == barCode && x.ProductId != id), JsonRequestBehavior.AllowGet);
            }
        }
    }
}