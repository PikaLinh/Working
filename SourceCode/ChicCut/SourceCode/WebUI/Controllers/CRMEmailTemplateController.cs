using EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using ViewModels;

using System.Data.Entity;
using AutoMapper;
namespace WebUI.Controllers
{
    public class CRMEmailTemplateController : BaseController
    {
        // GET: CRMEmailTemplate
        
        public ActionResult Index()
        {
            return View(_context.CRM_EmailTemplateModel.OrderByDescending(p => p.EmailTemplateId).ToList());
        }

        #region Thêm mới
       
        public ActionResult Create()
        {
            CRM_EmailTemplateModel model = new CRM_EmailTemplateModel() { Actived = true };
            return View(model);
        }

       public ActionResult _CreateList(List<EmailParameterDetailViewModel> detail = null)
       {
           if (detail == null)
           {
               detail = new List<EmailParameterDetailViewModel>();
           }
           return PartialView(detail);
       }

       public ActionResult _CreatelistInner(List<EmailParameterDetailViewModel> detail = null)
       {
           if (detail == null)
           {
               detail = new List<EmailParameterDetailViewModel>();
           }
           EmailParameterDetailViewModel item = new EmailParameterDetailViewModel();
           detail.Add(item);
           return PartialView(detail);
       }

       public ActionResult _DeletelistInner(List<EmailParameterDetailViewModel> detail, int RemoveId)
       {
           if (detail == null)
           {
               detail = new List<EmailParameterDetailViewModel>();
           }
           return PartialView("_CreatelistInner", detail.Where(p => p.STT != RemoveId).ToList());
       }

        //[HttpPost]
        //[ValidateInput(false)]
       //public ActionResult Create(CRM_EmailTemplateModel model)
       //{
       //    try
       //    {
       //        if (ModelState.IsValid)
       //        {
       //            _context.Entry(model).State = System.Data.Entity.EntityState.Added;
       //            _context.SaveChanges();
       //            return RedirectToAction("Index");
       //        }
       //        else
       //        {
       //            return View(model);
       //        }
       //    }
       //    catch
       //    {
       //        return View(model);
       //    }
       //}

        [ValidateInput(false)]
       public ActionResult Save(CRM_EmailTemplateModel model, List<EmailParameterDetailViewModel> detail)
       {
           try
           {
               if (ModelState.IsValid)
               {
                   using (TransactionScope ts = new TransactionScope())
                   {
                       _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();
                        if (detail != null)
                        {
                            CRM_EmailParameterModel EmailParameter;
                            foreach (var item in detail)
                            {
                                EmailParameter = new CRM_EmailParameterModel()
                                {
                                    EmailTemplateId = model.EmailTemplateId,
                                    Name = item.Name,
                                    Description = item.Description
                                };
                                _context.Entry(EmailParameter).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();
                            }
                        }
                        ts.Complete();
                        return Content("success");
                   }
               }
               else
               {
                   return Content("Vui lòng kiểm tra lại thông tin không hợp lệ");
               }

           }
           catch (Exception ex)
           {
               return Content("Xảy ra lỗi trong quá trình thêm mới.");
           }

       }

        #endregion

        #region Sửa
        
        public ActionResult Edit(int id)
        {
            var model = _context.CRM_EmailTemplateModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            // Kiểm tra xem mẫu Email đã đc tham chiếu bởi bảng khác hay chưa
            int RemiderId = _context.CRM_RemiderModel.Where(p => p.EmailTemplateId == id).Select(p => p.RemiderId).FirstOrDefault();
            ViewBag.CheckReferenced = RemiderId > 0 ? "true" : "false";
            ViewBag.EmailParameterDetailList = model.CRM_EmailParameterModel.Select(p => new EmailParameterDetailViewModel()
            {
                EmailParameterId = p.EmailParameterId,
                Name = p.Name,
                Description = p.Description,
                RemiderId = RemiderId
            }).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        //public ActionResult Edit(CRM_EmailTemplateModel model)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
        //            _context.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            return View(model);
        //        }
        //    }
        //    catch
        //    {
        //        return View(model);
        //    }
        //}
        public ActionResult Update(CRM_EmailTemplateModel model, List<EmailParameterDetailViewModel> detail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();

                        // Kiểm tra xem mẫu Email đã đc tham chiếu bởi bảng khác hay không
                        int RemiderId = _context.CRM_RemiderModel.Where(p => p.EmailTemplateId == model.EmailTemplateId).Select(p => p.RemiderId).FirstOrDefault();

                        #region Nếu không bị tham chiếu : Xoá những para bị xoá trên giao diện
                        if (RemiderId == 0)
                        {
                            if (detail == null)
                            {
                               detail =  new List<EmailParameterDetailViewModel>();
                            }
                            List<int> lstEmailPara = detail.Where(p => p.EmailParameterId != 0).Select(p =>p.EmailParameterId).ToList();
                            var lstEmailparaToDelete = _context.CRM_EmailParameterModel
                                                               .Where(p => p.EmailTemplateId == model.EmailTemplateId && !lstEmailPara.Contains(p.EmailParameterId))
                                                               .ToList();
                            foreach (var item in lstEmailparaToDelete)
                            {
                                _context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                                    _context.SaveChanges();
                            }
                        }
                        #endregion

                        #region // Sửa và thêm
                            CRM_EmailParameterModel EmailParameter;
                            foreach (var item in detail)
                            {
                                if (item.EmailParameterId == 0) //thêm mới
                                {
                                    EmailParameter = new CRM_EmailParameterModel()
                                    {
                                        EmailTemplateId = model.EmailTemplateId,
                                        Name = item.Name,
                                        Description = item.Description
                                    };
                                    _context.Entry(EmailParameter).State = System.Data.Entity.EntityState.Added;
                                    _context.SaveChanges();
                                }
                                else//sửa
                                {
                                    var modeladd = _context.CRM_EmailParameterModel.Where(p => p.EmailParameterId == item.EmailParameterId).FirstOrDefault();
                                    modeladd.EmailTemplateId = model.EmailTemplateId;
                                    modeladd.Name = item.Name;
                                    modeladd.Description = item.Description;
                                    _context.Entry(modeladd).State = System.Data.Entity.EntityState.Modified;
                                    _context.SaveChanges();
                                }
                            }
                        #endregion

                       

                        ts.Complete();
                        return Content("success");
                    }
                }
                else
                {
                    return Content("Vui lòng kiểm tra lại thông tin không hợp lệ");
                }
            }
            catch (Exception ex)
            {
                return Content("Lỗi hệ thống" + ex.Message);
            }

        }
        #endregion

        #region Xem chi tiết
        
        public ActionResult Details(int id = 0)
        {
            var model = _context.CRM_EmailTemplateModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmailParameterDetailList = model.CRM_EmailParameterModel.Select(p => new EmailParameterDetailViewModel()
            {
                Name = p.Name,
                Description = p.Description
            }).ToList();
            return View(model);
        }
        #endregion
    }
}