using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;


namespace WebUI.Controllers
{
    public class RoleController : BaseController
    {
        private EntityDataContext db = new EntityDataContext();

        //
        // GET: /Role/
         
        public ActionResult Index()
        {
            var myRoles = db.RolesModel.Find(currentAccount.RolesId);
            return View(db.RolesModel.Where(p => p.OrderBy >= myRoles.OrderBy).OrderBy(p => p.OrderBy).ToList());
        }

        //
        // GET: /Role/Details/5
         
        public ActionResult Details(int id = 0)
        {
            RolesModel rolesmodel = db.RolesModel.Find(id);
            if (rolesmodel == null)
            {
                return HttpNotFound();
            }
            return View(rolesmodel);
        }

        //
        // GET: /Role/Create
         
        public ActionResult Create()
        {
            RolesModel rolesmodel = new RolesModel();
            rolesmodel.Actived = true;
            return View(rolesmodel);
        }

        //
        // POST: /Role/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RolesModel rolesmodel)
        {
            if (ModelState.IsValid)
            {
                db.RolesModel.Add(rolesmodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rolesmodel);
        }

        //
        // GET: /Role/Edit/5
         
        public ActionResult Edit(int id = 0)
        {
            RolesModel rolesmodel = db.RolesModel.Find(id);
            if (rolesmodel == null)
            {
                return HttpNotFound();
            }
            return View(rolesmodel);
        }

        //
        // POST: /Role/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RolesModel rolesmodel)
        {
            if (ModelState.IsValid)
            {
                if (rolesmodel.OrderBy <= 0)
                {
                    ModelState.AddModelError("LonHon0", new Exception("Vui lòng nhập thứ tự lớn hơn 0"));
                    return View(rolesmodel);
                }

                db.Entry(rolesmodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rolesmodel);
        }

        //
        // GET: /Role/Delete/5
         
        public ActionResult Delete(int id = 0)
        {
            RolesModel rolesmodel = db.RolesModel.Find(id);
            if (rolesmodel == null)
            {
                return HttpNotFound();
            }
            return View(rolesmodel);
        }

        //
        // POST: /Role/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RolesModel rolesmodel = db.RolesModel.Find(id);
            db.RolesModel.Remove(rolesmodel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}