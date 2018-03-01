using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using AutoMapper;
using ViewModels;
using Repository;

namespace WebUI.Controllers
{
    public class Product2Controller : BaseController
    {
        //
        // GET: /Produrt2/
        #region danh sách sản phẩm
        public ActionResult Index()
        {
            return View(_context.ProductModel.OrderByDescending(p =>p.ProductId).ToList());
        }
        #endregion
        #region chi tiết sản phẩm
        public ActionResult Details(int id)
        {
            ProductModel model = _context.ProductModel.Where(p => p.ProductId == id).FirstOrDefault();
            CreateViewBag(model.CategoryId, model.OriginOfProductId, model.PolicyInStockId, model.PolicyOutOfStockId, model.LocationOfProductId, model.ProductStatusId, model.UnitId, model.CurrencyId);
            return View(model);
        }
        #endregion

        #region thêm mới sản phẩm
        public ActionResult Create()
        {
            ProductModel model = new ProductModel();
            model.Actived = true;
            model.ExchangeRate = 1;
            CreateViewBag();
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(ProductModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ////ProductModel
                    //ProductModel addmodel = new ProductModel();
                    //Mapper.CreateMap<ProductViewModel, ProductModel>();
                    //addmodel = Mapper.Map<ProductModel>(model);
                    model.Actived = true;
                    model.SEOProductName = Library.ConvertToNoMarkString(model.ProductName);
                    model.CreatedDate = DateTime.Now;
                    model.CreatedAccount = currentAccount.UserName;
                    model.LastModifiedDate = DateTime.Now;
                    model.LastModifiedAccount = currentAccount.UserName;
                    _context.Entry(model).State = System.Data.Entity.EntityState.Added;


                    //_context.Entry(model).State = System.Data.Entity.EntityState.Added;
                    _context.SaveChanges();
                    return  Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    CreateViewBag(model.CategoryId, model.OriginOfProductId, model.PolicyInStockId, model.PolicyOutOfStockId, model.LocationOfProductId, model.ProductStatusId, model.UnitId, model.CurrencyId); 
                    return View(model);
                }
 
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                CreateViewBag(model.CategoryId, model.OriginOfProductId, model.PolicyInStockId, model.PolicyOutOfStockId, model.LocationOfProductId, model.ProductStatusId, model.UnitId, model.CurrencyId);
                return View(model);
            }
        }
        #endregion

        #region GetExchangeRate
        public ActionResult GetExchangeRateBy(int CurrencyIdSelect)
        {
            var GetExchangeRate = _context.ExchangeRateModel
                                .Where(p => p.CurrencyId == CurrencyIdSelect &&
                                p.ExchangeDate <= DateTime.Now)
                                .OrderByDescending(p => p.ExchangeDate)
                                .FirstOrDefault();

            return Json(GetExchangeRate.ExchangeRate, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region sửa sản phẩm
        public ActionResult Edit(int id)
        {
            ProductModel model = _context.ProductModel.Where(p => p.ProductId == id).FirstOrDefault();
            CreateViewBag(model.CategoryId, model.OriginOfProductId, model.PolicyInStockId, model.PolicyOutOfStockId, model.LocationOfProductId, model.ProductStatusId, model.UnitId, model.CurrencyId);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(ProductModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ////ProductModel
                    //ProductModel addmodel = new ProductModel();
                    //Mapper.CreateMap<ProductViewModel, ProductModel>();
                    ////addmodel = Mapper.Map<ProductModel>(model);
                    //model.ImportPrice = ImportPrice;
                    //model.ShippingFee = ShippingFee;
                    //model.COGS = COGS;
                    //model.ExchangeRate = ExchangeRate;
                    model.Actived = true;
                    model.SEOProductName = Library.ConvertToNoMarkString(model.ProductName);
                    model.CreatedDate = DateTime.Now;
                    model.CreatedAccount = currentAccount.UserName;
                    model.LastModifiedDate = DateTime.Now;
                    model.LastModifiedAccount = currentAccount.UserName;
                    _context.Entry(model).State = System.Data.Entity.EntityState.Modified;


                    //_context.Entry(model).State = System.Data.Entity.EntityState.Added;
                    _context.SaveChanges();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    CreateViewBag(model.CategoryId, model.OriginOfProductId, model.PolicyInStockId, model.PolicyOutOfStockId, model.LocationOfProductId, model.ProductStatusId, model.UnitId, model.CurrencyId);
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                CreateViewBag(model.CategoryId, model.OriginOfProductId, model.PolicyInStockId, model.PolicyOutOfStockId, model.LocationOfProductId, model.ProductStatusId, model.UnitId, model.CurrencyId);
                return View(model);
            }
        }
        #endregion

        #region helper
        private void CreateViewBag(int? CategoryId = null, int? OriginOfProductId = null, int? PolicyInStockId = null, int? PolicyOutOfStockId = null, int? LocationOfProductId = null, int? ProductStatusId = null, int? UnitId = null, int? CurrencyId=null)
        {
            var listCategoty = _context.CategoryModel.OrderBy(p => p.CategoryName).ToList();
            ViewBag.CategoryId = new SelectList(listCategoty, "CategoryId", "CategoryName", CategoryId);

            var listOriginOfProduct = _context.OriginOfProductModel.OrderBy(p => p.OriginOfProductName).ToList();
            ViewBag.OriginOfProductId = new SelectList(listOriginOfProduct, "OriginOfProductId", "OriginOfProductName", OriginOfProductId);

            var listPolicyInStock = _context.PolicyModel.OrderBy(p => p.PolicyName).ToList();
            ViewBag.PolicyInStockId = new SelectList(listPolicyInStock, "PolicyId", "PolicyName", PolicyInStockId);

            var listPolicyOutOfStock = _context.PolicyModel.OrderBy(p => p.PolicyName).ToList();
            ViewBag.PolicyOutOfStockId = new SelectList(listPolicyOutOfStock, "PolicyId", "PolicyName", PolicyOutOfStockId);

            var listLocationOfProduct = _context.LocationOfProductModel.OrderBy(p => p.LocationOfProductName).ToList();
            ViewBag.LocationOfProductId = new SelectList(listLocationOfProduct, "LocationOfProductId", "LocationOfProductName", LocationOfProductId);

            var listProductStatus = _context.ProductStatusModel.OrderBy(p => p.ProductStatusName).ToList();
            ViewBag.ProductStatusId = new SelectList(listProductStatus, "ProductStatusId", "ProductStatusName", ProductStatusId);

            var listUnit = _context.UnitModel.OrderBy(p => p.UnitName).ToList();
            ViewBag.UnitId = new SelectList(listUnit, "UnitId", "UnitName", UnitId);

            var listCurrency = _context.CurrencyModel.OrderBy(p => p.CurrencyId).ToList();
            ViewBag.CurrencyId = new SelectList(listCurrency, "CurrencyId", "CurrencyName", CurrencyId);

        }
        #endregion

        public object GetExchangeRate { get; set; }
    }
}
