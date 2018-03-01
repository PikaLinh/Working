using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;

namespace WebUI.Controllers
{
    public class RegistryListController : Controller
    {
        //
        // GET: /RegistryList/
        private EntityDataContext _context = new EntityDataContext();
        #region Danh sách đăng ký
        public ActionResult Index()
        {
            return View(_context.RegistryModel.OrderBy(p => p.RegistryId).ToList());
        }
        #endregion

        #region Xem thông tin đăng ký
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")] 
        public ActionResult Details(int id = 0)
        {
            var register = _context.RegistryModel
                                   .Where(p => p.RegistryId == id)
                                   .FirstOrDefault();
            if (register == null)
            {
                return HttpNotFound();
            }
            return View(register);
        }
        #endregion

        #region
        public ActionResult Edit(int id = 0)
        {
            RegistryModel regismodel = _context.RegistryModel.Find(id);
            if (regismodel == null)
            {
                return HttpNotFound();
            }
            return View(regismodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RegistryModel rolesmodel)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(rolesmodel).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rolesmodel);
        }
        #endregion
    }
}
