using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using ViewModels;

namespace WebUI.Controllers
{
    public class UpdateSettingController : BaseController
    {
        //
        // GET: /UpdateSetting/

        string Title = "Cập nhật thông tin";

        #region update contact
        public ActionResult UpdateContact(string id = "Contact")
        {
            ViewBag.Title = "Cập nhật thông tin liên hệ";
            Website_SettingModel settingmodel = _context.Website_SettingModel.Find(id);
            Website_SettingModel settingenmodel = _context.Website_SettingModel.Find(id + "En");
            Website_SettingModel email = _context.Website_SettingModel.Find("Email");
            if (settingmodel == null)
            {
                return HttpNotFound();
            }
            ViewBag.Email = email.Details;
            SettingViewModel viewModel = new SettingViewModel() { Details = Server.HtmlDecode(settingmodel.Details), SettingName = id, DetailsEn = Server.HtmlDecode(settingenmodel.Details) };
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult UpdateContact(SettingViewModel viewModel, string Email)
        {
            if (ModelState.IsValid)
            {
                Website_SettingModel model1 = _context.Website_SettingModel.Find(viewModel.SettingName);
                model1.Details = Server.HtmlEncode(viewModel.Details);
                Website_SettingModel model2 = _context.Website_SettingModel.Find(viewModel.SettingNameEn);
                model2.Details = Server.HtmlEncode(viewModel.DetailsEn);

                Website_SettingModel emailmodel = _context.Website_SettingModel.Find("Email");
                emailmodel.Details = Email;

                _context.SaveChanges();
                ViewBag.Message = "Cập nhật thành công!";
            }
            ViewBag.Title = Title;
            return View(viewModel);
        }
        #endregion
        #region update about
        public ActionResult UpdateAbout(string id = "About")
        {
            ViewBag.Title = "Cập nhật thông tin giới thiệu";
            Website_SettingModel settingmodel = _context.Website_SettingModel.Find(id);
            Website_SettingModel settingenmodel = _context.Website_SettingModel.Find(id + "En");
            Website_SettingModel aboutModel = _context.Website_SettingModel.Find("txtAbout");
            Website_SettingModel aboutenModel = _context.Website_SettingModel.Find("txtAboutEn");
            if (settingmodel == null)
            {
                return HttpNotFound();
            }
            ViewBag.txtAbout = aboutModel.Details;
            ViewBag.txtAboutEn = aboutenModel.Details;
            SettingViewModel viewModel = new SettingViewModel() { Details = Server.HtmlDecode(settingmodel.Details), SettingName = id, DetailsEn = Server.HtmlDecode(settingenmodel.Details) };
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult UpdateAbout(SettingViewModel viewModel, string txtAbout, string txtAboutEn)
        {
            if (ModelState.IsValid)
            {
                Website_SettingModel model1 = _context.Website_SettingModel.Find(viewModel.SettingName);
                model1.Details = Server.HtmlEncode(viewModel.Details);
                Website_SettingModel model2 = _context.Website_SettingModel.Find(viewModel.SettingNameEn);
                model2.Details = Server.HtmlEncode(viewModel.DetailsEn);

                Website_SettingModel aboutModel = _context.Website_SettingModel.Find("txtAbout");
                aboutModel.Details = txtAbout;
                Website_SettingModel aboutenModel = _context.Website_SettingModel.Find("txtAboutEn");
                aboutenModel.Details = txtAboutEn;

                _context.SaveChanges();
                ViewBag.Message = "Cập nhật thành công!";
            }
            ViewBag.Title = Title;
            return View(viewModel);
        }
        #endregion
        #region update settings
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
                ViewBag.Message = "Cập nhật thành công!";
            }
            ViewBag.Title = Title;
            return View(viewModel);
        }
        #endregion
    }
}
