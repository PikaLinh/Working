using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using ViewModels;
using AutoMapper;

using Constant;

namespace WebUI.Controllers
{
    public class EmployeeController : BaseController
    {
        //
        // GET: /Employee/
        #region Danh sách nhân viên
        
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }

        public ActionResult _SearchEmployee(EmployeeInfoViewModel model)
        {
            var myRoleOrderBy = _context.RolesModel.Where(p => p.RolesId == model.RolesId).Select(p => p.OrderBy).FirstOrDefault();
            var currentRole = _context.RolesModel.Find(currentAccount.RolesId);
            var list = from emp in _context.EmployeeModel
                       join acc in _context.AccountModel on emp.EmployeeId equals acc.EmployeeId
                       join rol in _context.RolesModel on acc.RolesId equals rol.RolesId

                       join empPaTmp in _context.EmployeeModel on emp.ParentId equals empPaTmp.EmployeeId into empPaLst
                       from empPa in empPaLst.DefaultIfEmpty()

                       where (model.EmployeeId == 0 || model.EmployeeId == emp.EmployeeId)
                             && (model.Phone == null || emp.Phone.Contains(model.Phone))
                             // tìm theo thông tin role
                             // tất cả
                             && ((model.RolesId == 0 && 
                                 (rol.OrderBy >= currentRole.OrderBy || currentRole.Code == "DEV")
                                 ) 
                             //Tìm theo role được chọn
                             || (rol.OrderBy == myRoleOrderBy))
                             && emp.Actived == model.Actived
                             && rol.Actived == true
                       select new EmployeeInfoViewModel()
                       {
                           FullName = emp.FullName,
                           Gender = emp.Gender,
                           BirthDay = emp.BirthDay,
                           RolesName = rol.RolesName,
                           Actived = emp.Actived,
                           Phone = emp.Phone,
                           ParentName = empPa.FullName,
                           EmployeeId = emp.EmployeeId
                       };
          
            return PartialView(list.ToList());
        }
        #endregion

        #region GetCustomerId
        public ActionResult GetEmpoyeeId(string q)
        {
            var data2 = _context
                       .EmployeeModel
                       .Where(p => q == null || (p.FullName + " - " + p.Phone).Contains(q))
                       .Select(p => new
                       {
                           value = p.EmployeeId,
                           text = (p.FullName + " - " + p.Phone)
                       })
                       .Take(10)
                       .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Tạo mới nhân viên
        
        public ActionResult Create()
        {
            EmployeeInfoViewModel model = new EmployeeInfoViewModel() { Actived = true };
            CreateViewBag();
            return View(model);
        }

        private void CreateViewBag(int? RolesId = null, int? StoreId = null, int? ParentId = null, int? EmployeeId=null)
        {
            // ParentIdCreate
            if (CurrentUser.isAdmin)
            {
                var ParentIdCreateList = _context.EmployeeModel.Where(p => p.Actived == true).OrderBy(p => p.FullName).ToList();
                ViewBag.ParentIdCreate = new SelectList(ParentIdCreateList, "EmployeeId", "FullName", ParentId);
            }
            else
            {
                var ParentIdCreateList = (  from e in _context.EmployeeModel
                                            join a in _context.AccountModel on e.EmployeeId equals a.EmployeeId
                                            where e.Actived == true &&
                                                    a.RolesId != EnumRoles.DEV
                                            orderby e.FullName
                                            select new { e.EmployeeId, e.FullName}).ToList();
                                        //.Where(p => p.Actived == true && p.AccountModel.First().RolesId != EnumRoles.DEV).OrderBy(p => p.FullName).ToList();
                ViewBag.ParentIdCreate = new SelectList(ParentIdCreateList, "EmployeeId", "FullName", ParentId);
            }


            // ParentIdEdit
            var ParentIdList = _context.EmployeeModel.Where(p => p.Actived == true && p.EmployeeId != EmployeeId).OrderBy(p => p.FullName).ToList();
            ViewBag.ParentId = new SelectList(ParentIdList, "EmployeeId", "FullName", ParentId);

            // StoreId
            var StoreList = _context.StoreModel.Where(p => p.Actived == true).OrderBy(p => p.StoreName).ToList();
            ViewBag.StoreId = new SelectList(StoreList, "StoreId", "StoreName", StoreId);
            //RolesId
            var currentRole = _context.RolesModel.Find(currentAccount.RolesId);
            ViewBag.RolesId = new SelectList(_context.RolesModel.Where(p =>
                (
                    p.OrderBy >= currentRole.OrderBy || //(Lấy các role dưới role hiện tại <nếu là dev thì thấy cả dev>)
                    currentRole.Code == "DEV"
                )
                &&
                p.Code != "KH").ToList(), "RolesId", "RolesName", RolesId);
        }
        [HttpPost]
        public ActionResult Create(EmployeeInfoViewModel model, bool isRedirectToIndex = true)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    EmployeeModel modelAdd = new EmployeeModel();

                    Mapper.CreateMap<EmployeeInfoViewModel, EmployeeModel>();
                  
                    modelAdd = Mapper.Map<EmployeeModel>(model);

                    _context.Entry(modelAdd).State = System.Data.Entity.EntityState.Added;
                    _context.SaveChanges();

                    AccountModel accountmodel = new AccountModel()
                    {
                        UserName = model.UserName,
                        Password = model.Password,
                        RolesId = model.RolesId,
                        CustomerId = null,
                        EmployeeId = modelAdd.EmployeeId,
                        isCustomer = false,
                        Actived = true
                    };
                    _context.Entry(accountmodel).State = System.Data.Entity.EntityState.Added;
                    _context.SaveChanges();

                    if (isRedirectToIndex)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        EmployeeInfoViewModel newmodel = new EmployeeInfoViewModel();
                        CreateViewBag();
                        return View(newmodel);
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }

            CreateViewBag(model.RolesId, model.StoreId, model.ParentId,model.EmployeeId);
            return View(model);
        }
        #endregion

        #region Sửa thông tin nhân viên
        
        public ActionResult Edit(int id = 0)
        {
            //EmployeeModel employee = _context.EmployeeModel
            //                                .Where(p => p.EmployeeId == id)
            //                                .FirstOrDefault();
            //ViewModels.EmployeeInfoViewModel employee2 = _context.EmployeeModel
            //                                                                 .Where(p => p.EmployeeId == id)
            //                                                                 .FirstOrDefault();
            //AccountModel account = _context.AccountModel
            //                                .Where(p => p.EmployeeId == employee.EmployeeId)
            //                                .FirstOrDefault();

            EmployeeModel employeemodel = _context.EmployeeModel
                                                                .Where(p => p.EmployeeId == id)
                                                                .FirstOrDefault();
            if (employeemodel == null)
            {
                return HttpNotFound();
            }
            else
            {
                EmployeeInfoViewModel model = new EmployeeInfoViewModel();
                Mapper.CreateMap<EmployeeModel, EmployeeInfoViewModel>();
                model = Mapper.Map<EmployeeInfoViewModel>(employeemodel);
                AccountModel accountmodel = _context.AccountModel
                                                            .Where(p => p.EmployeeId == employeemodel.EmployeeId)
                                                            .FirstOrDefault();
                if (accountmodel != null)
                {
                    model.UserName = accountmodel.UserName;
                    model.Password = accountmodel.Password;
                    model.retypePassword = accountmodel.Password;
                    model.RolesId = accountmodel.RolesId.Value;
                    model.RolesId = accountmodel.RolesId.Value;
                    model.Actived = (accountmodel.Actived == true);
                }
                CreateViewBag(model.RolesId, model.StoreId, model.ParentId,model.EmployeeId);
                return View(model);
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EmployeeInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                   
                    AccountModel account = _context.AccountModel
                                                           .Where(p => p.EmployeeId == model.EmployeeId)
                                                           .FirstOrDefault();
                    if (account != null)
                    {
                        account.Password = model.Password;
                        account.UserName = model.UserName;
                        account.RolesId = model.RolesId;
                        account.Actived = model.Actived;
                        _context.Entry(account).State = System.Data.Entity.EntityState.Modified;
                    }
                    _context.SaveChanges();
                    EmployeeModel modelAdd = new EmployeeModel();
                    Mapper.CreateMap<EmployeeInfoViewModel, EmployeeModel>();
                    modelAdd = Mapper.Map<EmployeeModel>(model);
                    // update EmployeeModel
                    _context.Entry(modelAdd).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();
                       
                     return RedirectToAction("Index");
                  }
                catch 
                {
                    CreateViewBag(model.RolesId, model.StoreId, model.ParentId, model.EmployeeId);
                    return RedirectToAction("Index");
                }
                   
            }
            CreateViewBag(model.RolesId, model.StoreId, model.ParentId, model.EmployeeId);
            return View(model);
         }
        
        #endregion

        #region Thông tin chi tiết nhân viên
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        
        public ActionResult Details(int id = 0)
        {
            var employee = _context.EmployeeModel
                            .Where(p => p.EmployeeId == id)
                            .FirstOrDefault();
            ViewBag.ParentIdDetails = _context.EmployeeModel
                                        .Where(p => p.EmployeeId == employee.ParentId)
                                        .Select(p=>p.FullName)
                                        .FirstOrDefault();
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }
        #endregion

        public ActionResult GetEmployeeId(string q)
        {
            var data2 = _context
                        .EmployeeModel
                        .Where(p => q == null || (p.FullName).Contains(q))
                        .Select(p => new
                        {
                            value = p.EmployeeId,
                            text = p.FullName
                        })
                        .Take(10)
                        .ToList();
            return Json(data2, JsonRequestBehavior.AllowGet);
        }
    }
}
