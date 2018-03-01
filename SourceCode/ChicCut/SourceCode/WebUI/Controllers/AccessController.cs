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
    public class AccessController : BaseController
    {
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Index(int? id)
        {
            RolesRepository _rolesRepository = new RolesRepository(_context);
            if (id == null)
            {
                var lst = _rolesRepository.GetAll(currentAccount, LanguageId).ToList();
                if(lst.Count > 0)
                {
                    ViewBag.RolesId = new SelectList(lst, "RolesId", "RolesName");
                    PageSelectData(lst.First());
                    ViewBag.id = lst.First().RolesId;
                }
                return View();
            }
            else
            {
                int RolesId = Convert.ToInt32(id);
                try
                {
                    RolesModel rolesmodel = _rolesRepository.Find(RolesId);
                    if (rolesmodel == null)
                    {
                        return HttpNotFound();
                    }

                    ViewBag.RolesId = new SelectList(_rolesRepository.GetAll(currentAccount, LanguageId), "RolesId", "RolesName", RolesId);
                    ViewBag.id = RolesId;
                    PageSelectData(rolesmodel);

                    return View();
                }
                catch
                {
                    return HttpNotFound();
                }
            }
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateRoles(int id, string[] selectedPages)
        {
            if (ModelState.IsValid)
            {
                RolesRepository _repository = new RolesRepository(_context);
                RolesModel rolesmodel = _repository.Find(id);
                var UpdateRolesModel = _context.RolesModel.Include("PageModel").SingleOrDefault(p => p.RolesId == id);
                //Lấy thông tin cá nhân của mình
                AccountModel accsession = ((AccountModel)Session["acc"]);
                int orderby = (int)_context.AccountModel.SingleOrDefault(a => a.UserId == accsession.UserId).RolesModel.OrderBy;
                //Không cho cập nhật OrderBy nhỏ hơn của mình
                if (orderby > rolesmodel.OrderBy)
                {
                    ViewBag.Message = "Không thể cập nhật \"Thứ tự\" nhỏ hơn " + orderby.ToString();
                    PageSelectData(rolesmodel);
                }
                //Không cho cập nhật quyền quản lý
                else if (CurrentUser.RolesId == 2 && id == 2)
                {
                    ViewBag.Message = "Không thể cập nhật quyền quản lý";
                    PageSelectData(rolesmodel);
                }
                else
                {

                    //Cập nhật lại quyền
                    UpdatePagesForRoles(selectedPages, UpdateRolesModel);

                    _context.Entry(UpdateRolesModel).State = EntityState.Modified;
                    _context.SaveChanges();
                    return RedirectToAction("Index", new { id = id });
                }
            }

            return RedirectToAction("Index", new { id = id });
        }
        private void UpdatePagesForRoles(string[] selectedPages, RolesModel rolesmodel)
        {
            if (selectedPages == null)
            {
                rolesmodel.PageModel = new List<PageModel>();
                return;
            }

            var selectedPagesHS = new HashSet<string>(selectedPages);
            var roles = new HashSet<int>
                (rolesmodel.PageModel.Select(c => c.PageId));

            foreach (var page in _context.PageModel)
            {
                if (selectedPagesHS.Contains(page.PageId.ToString()))
                {
                    if (!roles.Contains(page.PageId))
                    {
                        rolesmodel.PageModel.Add(page);
                    }
                }
                else
                {
                    if (roles.Contains(page.PageId))
                    {
                        rolesmodel.PageModel.Remove(page);
                    }
                }
            }
        }

        private void PageSelectData(RolesModel rolesmodel)
        {
            List<PageModel> allPages = new List<PageModel>();
            // nếu là dev => thấy hết
            if (currentAccount.RolesId == 1)
            {
                allPages = _context.PageModel.Where(p => p.Actived == true).OrderBy(p => p.OrderBy).ToList();
            }
            // nếu không phải chỉ thấy từ quyền admin trở xuống
            else
            {
                allPages = _context.RolesModel.Find(2).PageModel.Where(p => p.Actived == true).ToList();
            }

            var Pages = new HashSet<int>(_context.RolesModel.Find(rolesmodel.RolesId).PageModel.Select(c => c.PageId));
            var viewModel = new List<PageSelectViewModel>();

            if (((System.Globalization.CultureInfo)Session["CurrentLanguage"]).Name == "vi-VN")
            {
                foreach (var page in allPages)
                {
                    viewModel.Add(new PageSelectViewModel
                    {
                        PageId = page.PageId,
                        MenuId = (int)page.MenuId,
                        PageName = page.PageLanguageModel.ToList()[0].PageName,
                        isSelected = Pages.Contains(page.PageId)
                    });
                }
            }
            else
            {
                foreach (var page in allPages)
                {
                    viewModel.Add(new PageSelectViewModel
                    {
                        PageId = page.PageId,
                        MenuId = (int)page.MenuId,
                        PageName = page.PageName,
                        isSelected = Pages.Contains(page.PageId)
                    });
                }
            }
            //ViewBag.Pages = viewModel;
            var allMenus = _context.MenuModel.ToList();
            ViewBag.Menus = (from p in allMenus
                                orderby p.OrderBy
                                select new MenuViewModel() 
                                { 
                                    MenuId = p.MenuId, 
                                    Icon = p.Icon, 
                                    MenuName = p.MenuName,
                                    Pages = viewModel.Where(a => a.MenuId == p.MenuId).ToList()
                                    //MenuName = LanguageId == 10001? p.MenuName : p.MenuLanguageModel.Where(e => e.MenuId == p.MenuId && e.LanguageId == LanguageId).FirstOrDefault().MenuName, OrderBy = p.OrderBy 
                                }).ToList();
        }

    }
}
