using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using System.Data;
using System.Data.Entity;


namespace WebUI.Controllers
{
    public class PolicyController : Controller
    {
        //
        // GET: /Policy/
        EntityDataContext db = new EntityDataContext();
        #region danh sách
        
        public ActionResult Index()
        {
            return View(db.PolicyModel.OrderBy(p =>p.PolicyId).ToList());
        }
        #endregion

        #region chi tiết chính sách
        
        public ActionResult Details(int id = 0)
        {
            PolicyModel model = db.PolicyModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        #endregion

        #region thêm chính sách
        
        public ActionResult Create()
        {
            PolicyModel model = new PolicyModel();
            model.Actived = true;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PolicyModel model)
        {
            if (ModelState.IsValid)
            {
                db.PolicyModel.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }
        #endregion

        #region sửa chính sách
        
        public ActionResult Edit(int id = 0)
        {
            PolicyModel model = db.PolicyModel.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PolicyModel model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View (model);
        }
        

        #endregion
    }
}
