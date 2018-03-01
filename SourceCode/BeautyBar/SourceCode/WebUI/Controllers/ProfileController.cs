using AutoMapper;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using ViewModels;


namespace WebUI.Controllers
{
    public class ProfileController : BaseController
    {
        //
        // GET: /Profile/
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index()
        {
            EmployeeModel emp = _context.EmployeeModel.Find(currentAccount.EmployeeId);
            if (emp == null)
            {
                return HttpNotFound();
            }
            EmployeeInfoViewModel model = new EmployeeInfoViewModel();
            Mapper.CreateMap<EmployeeModel, EmployeeInfoViewModel>();
            model = Mapper.Map<EmployeeInfoViewModel>(emp);

            var account = _context.AccountModel.Find(currentAccount.UserId);
            if (account != null)
            {
                model.UserName = account.UserName;
                model.RolesId = account.RolesId.Value;
                model.RolesName = account.RolesModel.RolesName;
            }
            model.ParentName = _context.EmployeeModel.Where(p => p.EmployeeId == model.ParentId).Select(p => p.FullName).FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPassword(ChangePasswordViewModel accModel)
        {
            string errorMessage = string.Empty;
            if (accModel.Password == accModel.NewPassword)
            {
                if (accModel.Password == null || accModel.NewPassword == null)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    ViewBag.Message = Resources.LanguageResource.UpdateFailed;
                    return View("Index", accModel);
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    ViewBag.Message = Resources.LanguageResource.UpdateFailed;
                    return Content("warningpass");
                }
            }
            else
            {
                AccountModel acc = (AccountModel)Session["acc"];
                if (acc == null)
                {
                    Response.Redirect("/");
                    return null;
                }
                int UserId = acc.UserId;
                AccountRepository _repository = new AccountRepository(_context);
                AccountModel user = _repository.Find(UserId);
                //Nếu nhập đúng password hoặc đăng nhập từ quyền admin
                if (user.Password == accModel.Password || (Session["isAdminLogin"] != null && (bool)Session["isAdminLogin"] == true))
                //Library.GetMd5Sum(
                {
                    if (ModelState.IsValid)
                    {
                        //user.Password = Library.GetMd5Sum(accModel.NewPassword);
                        user.Password = accModel.NewPassword;
                        if (_repository.UpdateNormal(user, out errorMessage) == true)
                        {
                            return Content("success");
                        }
                        else
                        {
                            ViewBag.Message = errorMessage;
                            return View("Index", accModel);
                        }
                    }
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var e in errors)
                    {
                        errorMessage += e.ErrorMessage;
                    }
                    ViewBag.Message = errorMessage;
                    return View("Index", accModel);
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var e in errors)
                    {
                        errorMessage += e.ErrorMessage;
                    }
                    ViewBag.Message = errorMessage;
                    return Content("failpass");
                }

            }

        }

       
        //UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Profile(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
               
                try
                {
                    CustomerModel updateModel = _context.CustomerModel.Where(p => p.CustomerId == model.CustomerId).FirstOrDefault();
                    if (updateModel != null)
                    {
                        updateModel.Gender = model.Gender;
                        updateModel.Phone = model.Phone;
                        updateModel.BirthDay = model.BirthDay;

                        _context.Entry(updateModel).State = EntityState.Modified;
                        //AccountModel account = _context.AccountModel.Where(p => p.CustomerId == model.CustomerId).FirstOrDefault();
                        //account.UserName = model.UserName;
                        //account.RolesId = model.RolesId;
                        //_context.Entry(account).State = EntityState.Modified;
                    }
                    _context.SaveChanges();
                    Session["customer"] = updateModel;
                    ViewBag.Message = "Cập nhật thành công!";
                    return View("Index",model);
                }
                catch (Exception ex)
                {
                    //thông báo lỗi
                    ViewBag.Message = string.Format("Lỗi hệ thống: {0}", ex.Message);
                    return View("Index", model);
                }
            }
            return View("Index", model);
        }

        public ActionResult UpdateProFileInfor(EmployeeModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();
                    return Json("Cập nhật thành công!", JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json("Xảy ra lỗi trong quá trình cập nhật thông tin!", JsonRequestBehavior.AllowGet);
                }

            }
            return Json("Vui lòng kiểm tra lại thông tin chưa hợp lệ!", JsonRequestBehavior.AllowGet);
        }
    }
}
