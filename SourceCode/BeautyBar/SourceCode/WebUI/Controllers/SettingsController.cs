using EntityModels;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace WebUI.Controllers
{
    public class SettingController : BaseController
    {
        string Title = "Quản lý cài đặt";
        //
        // GET: /Setting/

        public ActionResult Index()
        {
            ViewBag.Title = Title;
            return View(_context.Website_SettingModel.ToList());
        }

        //
        // GET: /Setting/Details/5

        public ActionResult Details(string id = null)
        {
            ViewBag.Title = Title;
            Website_SettingModel settingmodel = _context.Website_SettingModel.Find(id);
            if (settingmodel == null)
            {
                return HttpNotFound();
            }
            return View(settingmodel);
        }

        //
        // GET: /Setting/Create

        public ActionResult Create()
        {
            ViewBag.Title = Title;
            return View();
        }

        //
        // POST: /Setting/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Website_SettingModel settingmodel)
        {
            if (ModelState.IsValid)
            {
                _context.Website_SettingModel.Add(settingmodel);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Title = Title;
            return View(settingmodel);
        }

        //
        // GET: /Setting/Edit/5

        public ActionResult Edit(string id = null)
        {
            ViewBag.Title = Title;
            Website_SettingModel settingmodel = _context.Website_SettingModel.Find(id);
            Website_SettingModel settingenmodel = _context.Website_SettingModel.Find(id + "En");
            if (settingmodel == null)
            {
                return HttpNotFound();
            }
            SettingViewModel viewModel = new SettingViewModel() { Details = Server.HtmlDecode(settingmodel.Details), SettingName = id, DetailsEn = Server.HtmlDecode(settingenmodel.Details) };
            return View(viewModel);
        }

        //
        // POST: /Setting/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(SettingViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Website_SettingModel model1 = _context.Website_SettingModel.Find(viewModel.SettingName);
                model1.Details = Server.HtmlEncode(viewModel.Details);
                Website_SettingModel model2 = _context.Website_SettingModel.Find(viewModel.SettingNameEn);
                model2.Details = Server.HtmlEncode(viewModel.DetailsEn);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Title = Title;
            return View(viewModel);
        }

        //
        // GET: /Setting/Delete/5

        public ActionResult Delete(string id = null)
        {
            ViewBag.Title = Title;
            Website_SettingModel settingmodel = _context.Website_SettingModel.Find(id);
            if (settingmodel == null)
            {
                return HttpNotFound();
            }
            return View(settingmodel);
        }

        //
        // POST: /Setting/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ViewBag.Title = Title;
            Website_SettingModel settingmodel = _context.Website_SettingModel.Find(id);
            _context.Website_SettingModel.Remove(settingmodel);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}