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
using System.Security.Claims;

namespace WebUI.Controllers
{
    public class AccountController : BaseController
    {
        //
        // GET: /Account/
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Index()
        {
            if (Session["acc"] == null)
            {
                Response.Redirect("/Account/Login");
                return null;
            }

            var accountmodel = _context.AccountModel.Include(a => a.CustomerModel).Include(a => a.RolesModel);
            return View(accountmodel.ToList());
        }

        //
        // GET: /Account/Details/5
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Details(int id = 0)
        {
            var customer = _context.CustomerModel.Include(m => m.AccountModel).Where(p => p.CustomerId == id).FirstOrDefault();
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        //
        // GET: /Account/Create
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Create()
        {
            CustomerViewModel newModel = new CustomerViewModel();
            newModel.Actived = true;
            newModel.Gender = true;
            ViewBag.RolesId = new SelectList(_context.RolesModel.Where(p => p.Actived).OrderBy(p => p.OrderBy), "RolesId", "RolesName");
            return View(newModel);
        }

        //
        // POST: /Account/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CustomerViewModel customermodel)
        {
            if (ModelState.IsValid)
            {
                CustomerModel modelAdd = new CustomerModel();
                Mapper.CreateMap<CustomerViewModel, CustomerModel>();
                modelAdd = Mapper.Map<CustomerModel>(customermodel);
                modelAdd.AccountModel.Add(new AccountModel(){
                    Password = customermodel.Password,
                    UserName = customermodel.UserName,
                    RolesId = customermodel.RolesId,
                    Actived = customermodel.Actived
                });
                _context.Entry(modelAdd).State = EntityState.Added;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RolesId = new SelectList(_context.RolesModel.Where(p => p.Actived).OrderBy(p => p.OrderBy), "RolesId", "RolesName", customermodel.RolesId);
            return View(customermodel);
        }

        //
        // GET: /Account/Edit/5
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Edit(int id = 0)
        {
            CustomerModel customer = _context.CustomerModel.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            CustomerViewModel model = new CustomerViewModel();
            Mapper.CreateMap<CustomerModel, CustomerViewModel>();
            model = Mapper.Map<CustomerViewModel>(customer);

            var account = _context.AccountModel.Where(p => p.CustomerId == customer.CustomerId).FirstOrDefault();
            if (account != null)
            {
                model.UserName = account.UserName;
                model.RolesId = account.RolesId.Value;
                model.Actived = (account.Actived == true);
            }


            ViewBag.RolesId = new SelectList(_context.RolesModel.Where(p => p.Actived).OrderBy(p => p.OrderBy), "RolesId", "RolesName", model.RolesId);
            return View(model);
        }

        //
        // POST: /Account/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                //_context.Entry(accountmodel).State = EntityState.Modified;
                //_context.SaveChanges();
                try
                {
                    CustomerModel updateModel = _context.CustomerModel.Where(p => p.CustomerId == model.CustomerId).FirstOrDefault();
                    if (updateModel != null)
                    {
                        updateModel.FullName = model.FullName;
                        updateModel.Gender = model.Gender;
                        updateModel.BirthDay = model.BirthDay;
                        updateModel.Email = model.Email;
                        updateModel.Phone = model.Phone;

                        _context.Entry(updateModel).State = EntityState.Modified;
                        AccountModel account = _context.AccountModel.Where(p => p.CustomerId == model.CustomerId).FirstOrDefault();
                        account.UserName = model.UserName;
                        account.RolesId = model.RolesId;
                        account.Actived = model.Actived;
                        _context.Entry(account).State = EntityState.Modified;
                    }
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch //(Exception ex)
                {
                    //thông báo lỗi
                    ViewBag.RolesId = new SelectList(_context.RolesModel.Where(p => p.Actived).OrderBy(p => p.OrderBy), "RolesId", "RolesName", model.RolesId);
                    return RedirectToAction("Index");
                }
            }
            ViewBag.RolesId = new SelectList(_context.RolesModel.Where(p => p.Actived).OrderBy(p => p.OrderBy), "RolesId", "RolesName", model.RolesId);
            return View(model);
        }
        #region login logout
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            string CookieName = ConfigUtilities.GetSysConfigAppSetting("CookieName");
            ViewBag.ReturnUrl = returnUrl;
            HttpCookie userInfo = Request.Cookies[CookieName];
            LoginViewModel log = new LoginViewModel() { RememberMe = false, Password = "", UserName = "" };

            if (userInfo != null && userInfo["username"].ToString() != "")
            {
                try
                {
                    log.RememberMe = true;
                    log.UserName = userInfo["username"].ToString();
                    string password = Library.DecodePassword(userInfo["password"].ToString());
                    log.Password = password;
                }
                catch
                {
                }
            }
            return View(log);
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            string CookieName = ConfigUtilities.GetSysConfigAppSetting("CookieName");
            // If we got this far, something failed, redisplay form
            //ModelState.AddModelError("", "The user name or password provided is incorrect.");
            string userName = model.UserName;
            string password = model.Password;
            //Remember Cookies
            if (model.RememberMe == true)
            {
                HttpCookie userInfo = new HttpCookie(CookieName);
                userInfo.HttpOnly = true;
                userInfo["username"] = userName;
                userInfo["password"] = Library.EncodePassword(password);
                userInfo.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Add(userInfo);
                Request.Cookies[CookieName].Expires = DateTime.Now.AddDays(30);
            }
            else
            {
                HttpCookie userInfo = new HttpCookie(CookieName);
                userInfo["username"] = "";
                userInfo["password"] = "";
                Response.Cookies.Add(userInfo);
            }
            //Kiểm tra nếu tài khoản bị khóa thì không cho đăng nhập
            if (_context.AccountModel.Where(p => p.UserName == model.UserName && p.Actived != true).FirstOrDefault() != null)
            {
                string errorMessage = "Tài khoản đã bị khóa xin vui lòng liên hệ quản trị hệ thống để biết thêm chi tiết.";
                ModelState.AddModelError("", errorMessage);
                return View(model);
            }
            //Kiểm tra đăng nhập
            if (CheckLogin(userName, password))
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(GetRedirectUrl(returnUrl));
                }
                else
                {
                    return RedirectToAction("Index", "Home", null);
                }
            }
            else
            {
                string errorMessage = "Tài khoản hoặc mật khẩu của bạn chưa chính xác ! Xin vui lòng thử lại.";
                ModelState.AddModelError("", errorMessage);
                return View(model);
            }
        }

        private bool CheckLogin(string userName, string password)
        {
            var account = _context.AccountModel.Where(p => p.UserName == userName && p.Password == password).FirstOrDefault();
            if (account != null)
            {

                #region Login
                var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, account.UserName),
                    new Claim(ClaimTypes.Role, account.RolesId.Value.ToString()),
                    new Claim(ClaimTypes.PrimarySid, account.EmployeeId.HasValue ? account.EmployeeId.Value.ToString(): "" ),
                    new Claim(ClaimTypes.Sid, account.UserId.ToString()),
                },
                   "ApplicationCookie");

                var ctx = Request.GetOwinContext();
                var authManager = ctx.Authentication;

                authManager.SignIn(identity);

                #endregion
                //Session["acc"] = account;
                //if (account.isCustomer == true)
                //{
                //    var customer = _context.CustomerModel.Where(p => p.CustomerId == account.CustomerId).FirstOrDefault();
                //    Session["customer"] = customer;
                //}
                //else
                //{
                //    var emp = _context.EmployeeModel.Where(p => p.EmployeeId == account.EmployeeId).FirstOrDefault();
                //    Session["emp"] = emp;
                //}

                
              
                return true;
            }
            else
            {
                return false;
            }
        }
        public ActionResult LogOff()
        {
            Session["acc"] = null;
            Session["customer"] = null;
            Session["emp"] = null;
            Session["Menu"] = null;
            Session["PopupQtyAlert"] = null;

            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");

            return View();
        }

        #endregion

        public ActionResult GetMoreInfo(int? CustomerId)
        {
            //AccountRepository _repository = new AccountRepository(_context);
            //var data = _repository.GetCustomberNameBy(CustomerNumber.ToString());
            //if (Session["catchList"] != null)
            //{
            //    List<CustomerViewModel> catchList = (List<CustomerViewModel>)Session["catchList"];
            //    CustomerViewModel ret = catchList.Where(p => p.CustomerId == CustomerId).FirstOrDefault();

            //    return Json(ret, JsonRequestBehavior.AllowGet);
            //}
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetListCustomer()
        {
            var data = "";
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult ChangePassword(int id)
        {
            var account = _context.AccountModel.Where(p => p.CustomerId == id).FirstOrDefault();
            if (account != null)
            {
                ViewBag.UserId = id;
                ViewBag.UserName = account.UserName;
            }
            else
            {
                return HttpNotFound();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(int UserId, ChangePasswordViewModel accModel)
        {
            string errorMessage = string.Empty;
          
            AccountModel acc = (AccountModel)Session["acc"];
            if (acc == null)
            {
                Response.Redirect("/");
                return null;
            }
            AccountRepository _repository = new AccountRepository(_context);
            AccountModel user = _context.AccountModel.Where(p => p.CustomerId == UserId).FirstOrDefault();
            //Nếu nhập đúng password hoặc đăng nhập từ quyền admin
            //Library.GetMd5Sum(
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
        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return Url.Action("index", "home");
            }

            return returnUrl;
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}