using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using System.Data;
using System.Data.Entity;


namespace WebUI.Controllers
{
    public class ExchangeRateController : BaseController
    {
        //
        // GET: /ExchangeRate/


        #region Danh sách tỷ giá
        
        public ActionResult Index()
        {
            return View(_context.ExchangeRateModel.OrderByDescending(p =>p.ExchangeDate).ToList());
        }
        #endregion

        #region Chi tiết tỷ giá
        
        public ActionResult Details(int id = 0)
        {
            ExchangeRateModel ExchangeRate = _context.ExchangeRateModel.Find(id);
            if (ExchangeRate == null)
            {
                return HttpNotFound();
            }
            return View(ExchangeRate);
        }

        #endregion
        #region Thêm mới tỷ giá
        
        public ActionResult Create()
        {
            ExchangeRateModel ExchangeRate = new ExchangeRateModel();
            CreateViewBag();
            return View(ExchangeRate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ExchangeRateModel model)
        {
            if (ModelState.IsValid)
            {
                _context.ExchangeRateModel.Add(model);
               _context.SaveChanges();
                return RedirectToAction("Index");
            }
            CreateViewBag(model.CurrencyId);
            return View(model);
        }
        #endregion

        #region sửa tỷ giá
        
        public ActionResult Edit(int id = 0)
        {
            ExchangeRateModel ExchangeRate = _context.ExchangeRateModel.Find(id);
            if (ExchangeRate == null)
            {
                return HttpNotFound();
            }
            CreateViewBag(ExchangeRate.CurrencyId);
            return View(ExchangeRate);
        }

        //
        // POST: /Role/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit(ExchangeRateModel ExchangeRate)
        public ActionResult Edit(int exchangeRateId, int? currencyId, decimal? exchangeRate, DateTime? exchangeDate)
        {
            ExchangeRateModel model = new ExchangeRateModel()
            {
                ExchangeRateId = exchangeRateId,
                CurrencyId = currencyId,
                ExchangeRate = Convert.ToDouble(exchangeRate),
                ExchangeDate = exchangeDate

            };
            try
            {
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch //(Exception ex)
            {
                //throw ex;
            }

            CreateViewBag(currencyId);
            return View(model);
        }

        #endregion
        private void CreateViewBag(int? CurrencyId = null)
        {
            var listcurrency = _context.CurrencyModel.OrderBy(p => p.CurrencyName).Where(p => p.CurrencyId != 1).ToList();
            ViewBag.CurrencyId = new SelectList(listcurrency, "CurrencyId", "CurrencyName", CurrencyId);
        }

    }
}
