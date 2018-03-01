using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;

using ViewModels;
using Constant;
using Repository;
using System.Transactions;
using System.Data.Entity;
using AutoMapper;

namespace WebUI.Controllers
{
    public class CRMRemiderController : BaseController
    {   
        // GET: CRMRemider
        #region Danh sách ghi nhớ
        
        public ActionResult Index()
        {
            var lst = (from p in _context.CRM_RemiderModel
                       join obj in _context.CRM_ObjectModel on p.ObjectId equals obj.ObjectId

                       join custmp in _context.CustomerModel on p.CustomerId equals custmp.CustomerId into cuslst
                       from cus in cuslst.DefaultIfEmpty()

                       join suptmp in _context.SupplierModel on p.SupplierId equals suptmp.SupplierId into suplst
                       from sup in suplst.DefaultIfEmpty()

                       join emptmp in _context.EmployeeModel on p.EmployeeId equals emptmp.EmployeeId into emplst
                       from emp in emplst.DefaultIfEmpty()
                       orderby p.RemiderId descending
                       select new CRMRemiderViewModel()
                       {
                           RemiderId = p.RemiderId,
                           ObjectType = obj.ObjectName,
                           ObjectName = cus != null ? (cus.FullName + (string.IsNullOrEmpty(cus.Fax) == true ? "" : " - " + cus.Fax)) : (sup.SupplierName + (string.IsNullOrEmpty(sup.Phone) == true ? "" : " - " + sup.Phone)),
                           EmployeeName = emp.FullName + " - " + emp.Phone,
                           RemiderName = p.RemiderName,
                           //LastDateRemind = p.LastDateRemind,
                           //NextDateRemind = p.NextDateRemind,
                           Actived = p.Actived
                       }).ToList();
            return View(lst);
        }
        #endregion

        #region Thêm mới
        public ActionResult Create()
        {
            CRM_RemiderModel model = new CRM_RemiderModel() { Actived = true };
            CreateViewBag();
            return View();
        }
        [ValidateInput(false)]
        public ActionResult Save(CRM_RemiderModel model, List<CRM_Remider_EmailParameter_Mapping> detail, List<CRM_Remider_SMSParameter_Mapping> SMSPara)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        CRMNextDateReminderRepository repo = new CRMNextDateReminderRepository(_context);
                        model.NextDateRemind = repo.GetNextDateRemind(model);
                        _context.Entry(model).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();

                        if (detail != null)
                        {
                            CRM_Remider_EmailParameter_Mapping RemiderEmailPara;
                            foreach (var item in detail)
                            {
                                RemiderEmailPara = new CRM_Remider_EmailParameter_Mapping()
                                {
                                    RemiderId = model.RemiderId,
                                    EmailTemplateId = model.EmailTemplateId.Value,
                                    EmailParameterId = item.EmailParameterId,
                                    ValueType = item.ValueType,
                                    Value = item.Value
                                };
                                _context.Entry(RemiderEmailPara).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();
                            }
                        }
                        if (SMSPara != null)
                        {
                            CRM_Remider_SMSParameter_Mapping RemiderSMSPara;
                            foreach (var item in SMSPara)
                            {
                                RemiderSMSPara = new CRM_Remider_SMSParameter_Mapping()
                                {
                                    RemiderId = model.RemiderId,
                                    SMSTemplateId = model.SMSTemplateId.Value,
                                    SMSParameterId = item.SMSParameterId,
                                    ValueType = item.ValueType,
                                    Value = item.Value
                                };
                                _context.Entry(RemiderSMSPara).State = System.Data.Entity.EntityState.Added;
                                _context.SaveChanges();
                            }
                        }

                        ts.Complete();
                        return Json("success", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("Vui lòng kiểm tra lại thông tin không hợp lệ", JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(Resources.LanguageResource.AddErrorMessage, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Sửa
        public ActionResult Edit(int id)
        {
            var model = _context.CRM_RemiderModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            var modelCus= _context.CustomerModel.Where(p => p.CustomerId == model.CustomerId).FirstOrDefault();
            if (modelCus != null)
	        {
                ViewBag.IdCustomer = model.CustomerId;
                ViewBag.FullNameCustomer = modelCus.FullName + " - " + modelCus.Phone;
	        }
            var modelSup = _context.SupplierModel.Where(p => p.SupplierId == model.SupplierId).FirstOrDefault();
            if (modelSup != null)
            {
                ViewBag.IdSupplier = model.SupplierId;
                ViewBag.FullNameSupplier = modelSup.SupplierName + " - " + modelSup.Phone;
            }
            ViewBag.ObjectNameTmp = model.ObjectId == 1 ? "KH" : "NCC";
            // Danh sách Email para
            ViewBag.EmailParaList = (from p in _context.CRM_Remider_EmailParameter_Mapping
                                    join emailpara in _context.CRM_EmailParameterModel on p.EmailParameterId equals emailpara.EmailParameterId
                                    where (p.EmailTemplateId == model.EmailTemplateId && p.RemiderId == model.RemiderId)
                                    select new CRMRemiderEmailParameterMappingViewModel()
                                    {
                                         EmailParameterId = p.EmailParameterId,
                                         EmailParameterName = emailpara.Name,
                                         ValueType = p.ValueType,
                                         Value = p.Value
                                    }).ToList();
            // Danh sách SMS para
            ViewBag.SMSParaList = (from p in _context.CRM_Remider_SMSParameter_Mapping
                                   join smspara in _context.CRM_SMSParameterModel on p.SMSParameterId equals smspara.SMSParameterId
                                   where (p.SMSTemplateId == model.SMSTemplateId && p.RemiderId == model.RemiderId)
                                     select new CRMRemiderSMSParameterMappingViewModel()
                                     {
                                         SMSParameterId = p.SMSParameterId,
                                         SMSParameterName = smspara.Name,
                                         ValueType = p.ValueType,
                                         Value = p.Value
                                     }).ToList();
            CreateViewBag(model.PeriodCode, model.EmailTemplateId, model.SMSTemplateId,model.EmployeeId);
            return View(model);
        }
        [ValidateInput(false)]
        public ActionResult Update(CRM_RemiderModel model, List<CRM_Remider_EmailParameter_Mapping> detail, List<CRM_Remider_SMSParameter_Mapping> SMSPara)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    CRMNextDateReminderRepository repo = new CRMNextDateReminderRepository(_context);
                    model.NextDateRemind = repo.GetNextDateRemind(model);
                    _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();

                    #region B1 : Xoá các Para cũ
                    var Remidermodel = _context.CRM_RemiderModel
                                               .Include(p => p.CRM_Remider_EmailParameter_Mapping)
                                               .Include(p => p.CRM_Remider_SMSParameter_Mapping)
                                               .Where(p => p.RemiderId == model.RemiderId)
                                               .FirstOrDefault();
                    //Xoá EmailPara cũ
                    if (Remidermodel.CRM_Remider_EmailParameter_Mapping.Count > 0)
                    {
                        while (Remidermodel.CRM_Remider_EmailParameter_Mapping.Count > 0)
                        {
                            _context.Entry(Remidermodel.CRM_Remider_EmailParameter_Mapping.First()).State = System.Data.Entity.EntityState.Deleted;
                            _context.SaveChanges();
                        }
                    }
                    //Xoá SMSPara cũ
                    if (Remidermodel.CRM_Remider_SMSParameter_Mapping.Count > 0)
                    {
                        while (Remidermodel.CRM_Remider_SMSParameter_Mapping.Count > 0)
                        {
                            _context.Entry(Remidermodel.CRM_Remider_SMSParameter_Mapping.First()).State = System.Data.Entity.EntityState.Deleted;
                            _context.SaveChanges();
                        }
                    }

                    //Mapper.CreateMap<CRM_RemiderModel, CRM_RemiderModel>();
                    //Remidermodel = Mapper.Map<CRM_RemiderModel>(model);

                   
                    #endregion

                    #region Insert Para mới
                    if (detail != null)
                    {
                        CRM_Remider_EmailParameter_Mapping RemiderEmailPara;
                        foreach (var item in detail)
                        {
                            RemiderEmailPara = new CRM_Remider_EmailParameter_Mapping()
                            {
                                RemiderId = model.RemiderId,
                                EmailTemplateId = model.EmailTemplateId.Value,
                                EmailParameterId = item.EmailParameterId,
                                ValueType = item.ValueType,
                                Value = item.Value
                            };
                            _context.Entry(RemiderEmailPara).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                        }
                    }
                    if (SMSPara != null)
                    {
                        CRM_Remider_SMSParameter_Mapping RemiderSMSPara;
                        foreach (var item in SMSPara)
                        {
                            RemiderSMSPara = new CRM_Remider_SMSParameter_Mapping()
                            {
                                RemiderId = model.RemiderId,
                                SMSTemplateId = model.SMSTemplateId.Value,
                                SMSParameterId = item.SMSParameterId,
                                ValueType = item.ValueType,
                                Value = item.Value
                            };
                            _context.Entry(RemiderSMSPara).State = System.Data.Entity.EntityState.Added;
                            _context.SaveChanges();
                        }
                    }
                    #endregion
                    
                    ts.Complete();
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(Resources.LanguageResource.EditErrorMessage, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Xem chi tiết
        
        public ActionResult Details(int id = 0)
        {
            var model = (from p in _context.CRM_RemiderModel

                         join custempt in _context.CustomerModel on p.CustomerId equals custempt.CustomerId into cuslst
                         from cus in cuslst.DefaultIfEmpty()

                         join suptempt in _context.SupplierModel on p.SupplierId equals suptempt.SupplierId into suplst
                         from sup in suplst.DefaultIfEmpty()

                         join PeriodCodetempt in _context.CRM_PeriodModel on p.PeriodCode equals PeriodCodetempt.PeriodCode into PeriodCodelst
                         from periodcode in PeriodCodelst.DefaultIfEmpty()

                         join Emailtemplatetempt in _context.CRM_EmailTemplateModel on p.EmailTemplateId equals Emailtemplatetempt.EmailTemplateId into Emailtemplatelst
                         from emailtemplate in Emailtemplatelst.DefaultIfEmpty()

                         join SMStemplatetempt in _context.CRM_SMSTemplateModel on p.SMSTemplateId equals SMStemplatetempt.SMSTemplateId into SMStemplatelst
                         from smstemplate in SMStemplatelst.DefaultIfEmpty()

                         join Emptempt in _context.EmployeeModel on p.EmployeeId equals Emptempt.EmployeeId into emplst
                         from emp in emplst.DefaultIfEmpty()

                         where p.RemiderId == id
                         select new CRMRemiderViewModel()
                         {
                             RemiderId = p.RemiderId,
                             RemiderName = p.RemiderName,
                             ObjectName = p.ObjectId == 1 ? "Khách hàng" : "Nhà cung cấp",
                             CustomerName = cus.FullName + " - " + cus.Phone,
                             SupplierName = sup.SupplierName + " - " + sup.Phone,
                             PeriodTypeName = p.PeriodType == 1 ? "Một lần" : "Định kỳ",
                             ExpiryDate = p.ExpiryDate,
                             DaysPriorNotice = p.DaysPriorNotice,
                             PeriodCode = periodcode.PeriodName,
                             StartDate = p.StartDate,
                             NextDateRemind = p.NextDateRemind,
                             isEmailNotified = p.isEmailNotified,
                             isSMSNotifred = p.isSMSNotifred,
                             EmailTemplateName = emailtemplate.EmailTemplateName,
                             SMSTemplateName = smstemplate.SMSTemplateName,
                             EmployeeName = emp.FullName,
                             EmailOfEmployee = p.EmailOfEmployee,
                             SMSOfEmployee = p.SMSOfEmployee,
                             Actived = p.Actived,
                             Price = p.Price,
                             ServiceContent = p.ServiceContent,
                             Note = p.Note
                         }
                         ).FirstOrDefault();
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmailParameterList = (from p in _context.CRM_Remider_EmailParameter_Mapping
                                               join emailtempalte in _context.CRM_EmailTemplateModel on p.EmailTemplateId equals emailtempalte.EmailTemplateId
                                               join emailpara in _context.CRM_EmailParameterModel on p.EmailParameterId equals emailpara.EmailParameterId
                                               where p.RemiderId == id
                                                select new CRMRemiderEmailParameterMappingViewModel()
                                                {
                                                    EmailParameterName = emailpara.Name,
                                                    ValueType = p.ValueType,
                                                    Value = p.Value,
                                                    EmailParameterDescription = emailpara.Description
                                                }).ToList();
            ViewBag.SMSParameterList = (from p in _context.CRM_Remider_SMSParameter_Mapping
                                        join smstempalte in _context.CRM_SMSTemplateModel on p.SMSTemplateId equals smstempalte.SMSTemplateId
                                        join SMSpara in _context.CRM_SMSParameterModel on p.SMSParameterId equals  SMSpara.SMSParameterId
                                          where p.RemiderId == id
                                          select new CRMRemiderSMSParameterMappingViewModel()
                                          {
                                              SMSParameterName = SMSpara.Name,
                                              ValueType = p.ValueType,
                                              Value = p.Value,
                                              SMSParameterDescription = SMSpara.Description
                                          }).ToList();
            return View(model);
        }
        #endregion

        #region Xem Email trước khu lưu
        public ActionResult _PreviewEmail(string NextDateRemindPreview, CRM_RemiderModel model, List<CRMRemiderEmailParameterMappingViewModel> detail)
        {
            var ContentEmail = _context.CRM_EmailTemplateModel
                                        .Where(p => p.EmailTemplateId == model.EmailTemplateId)
                                        .Select(p => p.EmailContent)
                                        .FirstOrDefault();
            if (detail != null)
            {
                foreach (var item in detail)
                {
                    switch(item.Value)
                    {
                        case ConstantRemiderAUTOType.ExpDATE :
                            ContentEmail = ContentEmail.Replace(item.EmailParameterName, model.ExpiryDate.Value.ToString("dd/MM/yyyy"));
                            break;
                        case ConstantRemiderAUTOType.SerCONTENT:
                            ContentEmail = ContentEmail.Replace(item.EmailParameterName, model.ServiceContent);
                            break;
                        case ConstantRemiderAUTOType.SerPRICE:
                            ContentEmail = ContentEmail.Replace(item.EmailParameterName, model.Price.Value.ToString("n0"));
                            break;
                        default :
                            ContentEmail = ContentEmail.Replace(item.EmailParameterName, item.Value);
                            break;
                    }
                }
            }
            var modelPreview = new CRMPreviewViewModel()
            {
                Tile = _context.CRM_EmailTemplateModel
                                        .Where(p => p.EmailTemplateId == model.EmailTemplateId)
                                        .Select(p => p.EmailTitle)
                                        .FirstOrDefault(),
                Content = ContentEmail,
                NextDateRemindPreview = NextDateRemindPreview
            };
            return PartialView(modelPreview);
        }
        #endregion

        #region Xem SMS trước khu lưu
        public ActionResult _PreviewSMS(string NextDateRemindPreview, CRM_RemiderModel model, List<CRMRemiderSMSParameterMappingViewModel> SMSPara)
        {
            var ContentSMS = _context.CRM_SMSTemplateModel
                                        .Where(p => p.SMSTemplateId == model.SMSTemplateId)
                                        .Select(p => p.SMSContent)
                                        .FirstOrDefault();
            if (SMSPara != null)
            {
                foreach (var item in SMSPara)
                {
                    switch (item.Value)
                    {
                        case ConstantRemiderAUTOType.ExpDATE:
                            ContentSMS = ContentSMS.Replace(item.SMSParameterName, model.ExpiryDate.Value.ToString("dd/MM/yyyy"));
                            break;
                        case ConstantRemiderAUTOType.SerCONTENT:
                            ContentSMS = ContentSMS.Replace(item.SMSParameterName, model.ServiceContent);
                            break;
                        case ConstantRemiderAUTOType.SerPRICE:
                            ContentSMS = ContentSMS.Replace(item.SMSParameterName, model.Price.Value.ToString("n0"));
                            break;
                        default:
                            ContentSMS = ContentSMS.Replace(item.SMSParameterName, item.Value);
                            break;
                    }
                }
            }
            var modelPreview = new CRMPreviewViewModel()
            {
                Content = ContentSMS,
                NextDateRemindPreview = NextDateRemindPreview
            };
            return PartialView(modelPreview);
        }
        #endregion

        #region Helper
        private void CreateViewBag(string PeriodCode = null, int? EmailTemplateId = null, int? SMSTemplateId = null, int? EmployeeId = null) 
        {
            //1.Kỳ
            var PeriodCodeList = _context.CRM_PeriodModel.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).ToList();
            ViewBag.PeriodCode = new SelectList(PeriodCodeList, "PeriodCode", "PeriodName", PeriodCode);

            //2. Mẫu Email
            var EmailTemplateLst = _context.CRM_EmailTemplateModel
                                           .Where(p => p.Actived == true)
                                           .ToList();
            ViewBag.EmailTemplateId = new SelectList(EmailTemplateLst, "EmailTemplateId", "EmailTemplateName", EmailTemplateId);

            //3. Mẫu SMS
            var SMSTemplateLst = _context.CRM_SMSTemplateModel.Where(p => p.Actived == true).ToList();
            ViewBag.SMSTemplateId = new SelectList(SMSTemplateLst, "SMSTemplateId", "SMSTemplateName", SMSTemplateId);


            //4.Nhân viên quản lý
            var EmployeeList = _context.EmployeeModel.OrderBy(p => p.FullName).Where(p => p.Actived == true).ToList();
            ViewBag.EmployeeId = new SelectList(EmployeeList, "EmployeeId", "FullName", EmployeeId);
        }

        public ActionResult GetEmailOfEmployee(int id)
        {
            string Email = _context.EmployeeModel.Where(p => p.EmployeeId == id).Select(p => p.Email).FirstOrDefault();
            return Json(string.IsNullOrEmpty(Email) ? "" : Email, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSMSOfEmployee(int id)
        {
            string SMS = _context.EmployeeModel.Where(p => p.EmployeeId == id).Select(p => p.Phone).FirstOrDefault();
            return Json(string.IsNullOrEmpty(SMS) ? "" : SMS, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNextDateReminder(CRM_RemiderModel model)
        {
            CRMNextDateReminderRepository repo = new CRMNextDateReminderRepository(_context);
            DateTime NextDate = repo.GetNextDateRemind(model);
            return Json(NextDate.ToString("dd/MM/yyyy"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult _EmailParameter(int EmailTemplateId = 0)
        {
            var lst = _context.CRM_EmailParameterModel
                              .Where(p => p.EmailTemplateId == EmailTemplateId)
                              .Select(p => new CRMRemiderEmailParameterMappingViewModel()
                              {
                                  EmailParameterId = p.EmailParameterId,
                                  EmailParameterName = p.Name,
                                  EmailParameterDescription = p.Description
                              }).ToList();
            return PartialView(lst);
        }

        public ActionResult _SMSParameter(int SMSTemplateId = 0)
        {
            var lst = _context.CRM_SMSParameterModel
                              .Where(p => p.SMSTemplateId == SMSTemplateId)
                              .Select(p => new CRMRemiderSMSParameterMappingViewModel()
                              {
                                  SMSParameterId = p.SMSParameterId,
                                  SMSParameterName = p.Name,
                                  SMSParameterDescription = p.Description
                              }).ToList();
            return PartialView(lst);
        }

        #endregion
    }
}