using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;

using ViewModels;
using System.Transactions;
using System.Data.Entity;
namespace WebUI.Controllers
{
    public class CRMSMSTemplateController : BaseController
    {
        // GET: CRMSMSTemplate
        
        public ActionResult Index()
        {
            return View(_context.CRM_SMSTemplateModel.OrderByDescending(p => p.SMSTemplateId).ToList());
        }

        #region Thêm mới
        
        public ActionResult Create()
        {
            CRM_SMSTemplateModel model = new CRM_SMSTemplateModel() { Actived = true };
            return View(model);
        }

        public ActionResult _CreateList(List<SMSParameterViewModel> detail = null)
        {
            if (detail == null)
            {
                detail = new List<SMSParameterViewModel>();
            }
            return PartialView(detail);
        }

        public ActionResult _CreatelistInner(List<SMSParameterViewModel> detail = null)
        {
            if (detail == null)
            {
                detail = new List<SMSParameterViewModel>();
            }
            SMSParameterViewModel item = new SMSParameterViewModel();
            detail.Add(item);
            return PartialView(detail);
        }

        public ActionResult _DeletelistInner(List<SMSParameterViewModel> detail, int RemoveId)
        {
            if (detail == null)
            {
                detail = new List<SMSParameterViewModel>();
            }
            return PartialView("_CreatelistInner", detail.Where(p => p.STT != RemoveId).ToList());
        }

        //[HttpPost]
        //public ActionResult Create(CRM_SMSTemplateModel model)
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
        public ActionResult Save(CRM_SMSTemplateModel model, List<SMSParameterViewModel> detail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        model.SMSContent = ConvertToUnsign(model.SMSContent);
                        _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();
                        if (detail != null)
                        {
                            CRM_SMSParameterModel SMSParameter;
                            foreach (var item in detail)
                            {
                                SMSParameter = new CRM_SMSParameterModel()
                                {
                                    SMSTemplateId = model.SMSTemplateId,
                                    Name = item.Name,
                                    Description = item.Description
                                };
                                _context.Entry(SMSParameter).State = System.Data.Entity.EntityState.Added;
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
            var model = _context.CRM_SMSTemplateModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            // Kiểm tra xem mẫu Email đã đc tham chiếu bởi bảng khác hay chưa
            int RemiderId = _context.CRM_RemiderModel.Where(p => p.SMSTemplateId == id).Select(p => p.RemiderId).FirstOrDefault();
            ViewBag.CheckReferenced = RemiderId > 0 ? "true" : "false";

            ViewBag.SMsParameterList = model.CRM_SMSParameterModel.Select(p => new SMSParameterViewModel()
            {
                SMSParameterId = p.SMSParameterId,
                Name = p.Name,
                Description = p.Description,
                RemiderId = RemiderId
            }).ToList();
            return View(model);
        }

        public ActionResult Update(CRM_SMSTemplateModel model, List<SMSParameterViewModel> detail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        model.SMSContent = ConvertToUnsign(model.SMSContent);
                        _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();

                        // Kiểm tra xem mẫu Email đã đc tham chiếu bởi bảng khác hay không
                        int RemiderId = _context.CRM_RemiderModel.Where(p => p.EmailTemplateId == model.SMSTemplateId).Select(p => p.RemiderId).FirstOrDefault();

                        #region Nếu không bị tham chiếu : Xoá những para bị xoá trên giao diện
                        if (RemiderId == 0)
                        {
                            if (detail == null)
                            {
                                detail = new List<SMSParameterViewModel>();
                            }
                            List<int> lstSMSPara = detail.Where(p => p.SMSParameterId != 0).Select(p => p.SMSParameterId).ToList();
                            var lstSMSparaToDelete = _context.CRM_SMSParameterModel
                                                               .Where(p => p.SMSTemplateId == model.SMSTemplateId && !lstSMSPara.Contains(p.SMSParameterId))
                                                               .ToList();
                            foreach (var item in lstSMSparaToDelete)
                            {
                                _context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                                _context.SaveChanges();
                            }
                        }
                        #endregion

                        #region // Sửa và thêm
                        CRM_SMSParameterModel SMSParameter;
                        foreach (var item in detail)
                        {
                            if (item.SMSParameterId == 0) //thêm mới
                            {
                                SMSParameter = new CRM_SMSParameterModel()
                                {
                                    SMSTemplateId = model.SMSTemplateId,
                                    Name = item.Name,
                                    Description = item.Description
                                };
                                _context.Entry(SMSParameter).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();
                            }
                            else//sửa
                            {
                                var modeladd = _context.CRM_SMSParameterModel.Where(p => p.SMSParameterId == item.SMSParameterId).FirstOrDefault();
                                modeladd.SMSTemplateId = model.SMSTemplateId;
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

        //[HttpPost]
        //public ActionResult Edit(CRM_SMSTemplateModel model)
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
        #endregion

        #region Xem chi tiết
        
        public ActionResult Details(int id = 0)
        {
            var model = _context.CRM_SMSTemplateModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.SMsParameterList = model.CRM_SMSParameterModel.Select(p => new SMSParameterViewModel()
            {
                Name = p.Name,
                Description = p.Description
            }).ToList();
            return View(model);
        }
        #endregion

        #region Helper
        public static string ConvertToUnsign(string str)
        {
            str = str.Trim();
            const string FindText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";
            const string ReplText = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY";
            int index = -1;
            char[] arrChar = FindText.ToCharArray();
            while ((index = str.IndexOfAny(arrChar)) != -1)
            {
                int index2 = FindText.IndexOf(str[index]);
                str = str.Replace(str[index], ReplText[index2]);
            }

            return str;
        }
        #endregion
    }
}