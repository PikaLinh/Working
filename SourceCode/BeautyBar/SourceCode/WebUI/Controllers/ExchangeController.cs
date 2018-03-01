using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;

namespace WebUI.Controllers
{
    public class ExchangeController : BaseController
    {
        //
        // GET: /Exchange/
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Index()
        {
            if (Session["acc"] == null)
            {
                Response.Redirect("/Account/Login");
                return null;
            }
            var exchangemodel = _context.ExchangeModel.Include(e => e.SteelFIModel).Include(e => e.SteelMarkModel).Include(e => e.UnitModel).Where(p => p.UnitId == 4);
            return View(exchangemodel.ToList());
        }

        //
        // GET: /Exchange/Details/5
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Details(int SteelMarkId, int SteelFIId, int UnitId)
        {
            ExchangeModel exchangemodel = _context.ExchangeModel.Where(p => p.SteelFIId == SteelFIId && p.SteelMarkId == SteelMarkId && p.UnitId == UnitId).FirstOrDefault();
            if (exchangemodel == null)
            {
                return HttpNotFound();
            }
            return View(exchangemodel);
        }

        //
        // GET: /Exchange/Create
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Create()
        {
            ViewBag.SteelFIId = new SelectList(_context.SteelFIModel, "SteelFIId", "Code");
            ViewBag.SteelMarkId = new SelectList(_context.SteelMarkModel, "SteelMarkId", "Code");
            ViewBag.UnitId = new SelectList(_context.UnitModel.Where(p => p.UnitId == 4), "UnitId", "UnitName");
            return View();
        }

        //
        // POST: /Exchange/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ExchangeModel exchangemodel)
        {
            if (ModelState.IsValid)
            {
                ExchangeModel exchangemodelCheck = _context.ExchangeModel.Where(p => p.SteelFIId == exchangemodel.SteelFIId && p.SteelMarkId == exchangemodel.SteelMarkId && p.UnitId == exchangemodel.UnitId).FirstOrDefault();
                if (exchangemodelCheck == null)
                {
                    _context.ExchangeModel.Add(exchangemodel);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }else
                {
                    string errorMessage = "Quy đổi này (Mác thép, Phi Thép, Đơn vị tính) đã tồn tại không thể thêm mới.";
                    ModelState.AddModelError("", errorMessage);
                }
            }

            ViewBag.SteelFIId = new SelectList(_context.SteelFIModel, "SteelFIId", "Code", exchangemodel.SteelFIId);
            ViewBag.SteelMarkId = new SelectList(_context.SteelMarkModel, "SteelMarkId", "Code", exchangemodel.SteelMarkId);
            ViewBag.UnitId = new SelectList(_context.UnitModel.Where(p => p.UnitId == 4), "UnitId", "UnitName", exchangemodel.UnitId);
            return View(exchangemodel);
        }

        //
        // GET: /Exchange/Edit/5
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Edit(int SteelMarkId, int SteelFIId, int UnitId)
        {
            ExchangeModel exchangemodel = _context.ExchangeModel.Where(p => p.SteelFIId == SteelFIId && p.SteelMarkId == SteelMarkId && p.UnitId == UnitId).FirstOrDefault();
            if (exchangemodel == null)
            {
                return HttpNotFound();
            }
            ViewBag.SteelFIId = new SelectList(_context.SteelFIModel, "SteelFIId", "Code", exchangemodel.SteelFIId);
            ViewBag.SteelMarkId = new SelectList(_context.SteelMarkModel, "SteelMarkId", "Code", exchangemodel.SteelMarkId);
            ViewBag.UnitId = new SelectList(_context.UnitModel.Where(p => p.UnitId == 4), "UnitId", "UnitName", exchangemodel.UnitId);
            return View(exchangemodel);
        }

        //
        // POST: /Exchange/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ExchangeModel exchangemodel)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(exchangemodel).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SteelFIId = new SelectList(_context.SteelFIModel, "SteelFIId", "Code", exchangemodel.SteelFIId);
            ViewBag.SteelMarkId = new SelectList(_context.SteelMarkModel, "SteelMarkId", "Code", exchangemodel.SteelMarkId);
            ViewBag.UnitId = new SelectList(_context.UnitModel.Where(p => p.UnitId == 4), "UnitId", "UnitName", exchangemodel.UnitId);
            return View(exchangemodel);
        }

        //
        // GET: /Exchange/Delete/5
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Delete(int SteelMarkId, int SteelFIId, int UnitId)
        {
            ExchangeModel exchangemodel = _context.ExchangeModel.Where(p => p.SteelFIId == SteelFIId && p.SteelMarkId == SteelMarkId && p.UnitId == UnitId).FirstOrDefault();
            if (exchangemodel == null)
            {
                return HttpNotFound();
            }
            return View(exchangemodel);
        }

        //
        // POST: /Exchange/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int SteelMarkId, int SteelFIId, int UnitId)
        {
            ExchangeModel exchangemodel = _context.ExchangeModel.Where(p => p.SteelFIId == SteelFIId && p.SteelMarkId == SteelMarkId && p.UnitId == UnitId).FirstOrDefault();
            _context.ExchangeModel.Remove(exchangemodel);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        #region Quy đổi

        //Đổi cuộn qua tấn
        public ActionResult RollToTON(double value, int SteelMarkId, int SteelFIId)
        {
            //int FromRollId = 2;
            //int ToTONId = 3;

            //var fromRoll = _context.ExchangeModel.Where(p => p.SteelMarkId == SteelMarkId && p.SteelFIId == SteelFIId && p.UnitId == FromRollId).FirstOrDefault();
            //var toTON = _context.ExchangeModel.Where(p => p.SteelMarkId == SteelMarkId && p.SteelFIId == SteelFIId && p.UnitId == ToTONId).FirstOrDefault();

            //double ret = 0;
            //if (fromRoll != null && toTON != null && fromRoll.Value.HasValue && toTON.Value.HasValue)
            //{
            //    ret = (value / (1/fromRoll.Value.Value)) * toTON.Value.Value;
            //}

            //Fix unit = 4

            double ret = 0;
            var fromRolltoTON = _context.ExchangeModel.Where(p => p.SteelMarkId == SteelMarkId && p.SteelFIId == SteelFIId && p.UnitId == 4).FirstOrDefault();
            if (fromRolltoTON != null && fromRolltoTON.Value.HasValue && fromRolltoTON.Value != 0)
            {
                ret = value * fromRolltoTON.Value.Value;
            }

            if (ret == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Math.Round(ret, 3, MidpointRounding.AwayFromZero).ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        //Đổi tấn qua cuộn
        public ActionResult TONToRoll(double value, int SteelMarkId, int SteelFIId)
        {
            //int FromTONId = 3;
            //int ToRollId = 2;

            //var fromTON = _context.ExchangeModel.Where(p => p.SteelMarkId == SteelMarkId && p.SteelFIId == SteelFIId && p.UnitId == FromTONId).FirstOrDefault();
            //var toRoll = _context.ExchangeModel.Where(p => p.SteelMarkId == SteelMarkId && p.SteelFIId == SteelFIId && p.UnitId == ToRollId).FirstOrDefault();

            //double ret = 0;
            //if (fromTON != null && toRoll != null && fromTON.Value.HasValue && toRoll.Value.HasValue)
            //{
            //    ret = (value / fromTON.Value.Value) * (1 / toRoll.Value.Value);
            //    ret = Math.Ceiling(ret);
            //}
            double ret = 0;
            var fromRolltoTON = _context.ExchangeModel.Where(p => p.SteelMarkId == SteelMarkId && p.SteelFIId == SteelFIId && p.UnitId == 4).FirstOrDefault();
            if (fromRolltoTON != null && fromRolltoTON.Value.HasValue && fromRolltoTON.Value != 0)
            {
                ret = value / fromRolltoTON.Value.Value;
                ret = Math.Ceiling(ret);
            }


            if (ret == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(ret.ToString(), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}