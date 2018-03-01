using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;
using Constant;

namespace WebUI.Controllers
{
    public class EventController : Controller
    {
        private EntityDataContext db = new EntityDataContext();

        bool isUseUrl = true;
        bool isDescription = true;
        bool isDetails = true;
        int rootCategory = (int)ConstantCourseType.HoiThaoSuKien;
        //
        // GET: /Course/

        public ActionResult Index()
        {
            var root = db.CategoryModel.Find(rootCategory);
            var coursemodel = db.CourseModel.Where(p => p.CategoryModel.ADNCode.StartsWith(root.ADNCode)).Include(c => c.CategoryModel);
            CreateViewBag();
            return View(coursemodel.ToList());
        }

        //
        // GET: /Course/Details/5

        public ActionResult Details(int id = 0)
        {
            CourseModel coursemodel = db.CourseModel.Find(id);
            CreateViewBag(coursemodel.CategoryId);
            if (coursemodel == null)
            {
                return HttpNotFound();
            }
            return View(coursemodel);
        }

        //
        // GET: /Course/Create

        public ActionResult Create()
        {
            CourseModel coursemodel = new CourseModel();
            coursemodel.Actived = true;
            CreateViewBag();
            return View(coursemodel);
        }

        //
        // POST: /Course/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(CourseModel coursemodel)
        {
            if (ModelState.IsValid)
            {
                db.CourseModel.Add(coursemodel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            CreateViewBag(coursemodel.CategoryId);
            return View(coursemodel);
        }

        //
        // GET: /Course/Edit/5

        public ActionResult Edit(int id = 0)
        {
            CourseModel coursemodel = db.CourseModel.Find(id);
            if (coursemodel == null)
            {
                return HttpNotFound();
            }
            CreateViewBag(coursemodel.CategoryId);
            return View(coursemodel);
        }

        //
        // POST: /Course/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(CourseModel coursemodel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(coursemodel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            CreateViewBag(coursemodel.CategoryId);
            return View(coursemodel);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private void CreateViewBag(int? CategoryId = null)
        {
            ViewBag.CategoryId = new SelectList(db.CategoryModel.Where(p => p.Parent == rootCategory && p.Actived).OrderBy(p => p.OrderBy).ToList(), "CategoryId", "CategoryName", CategoryId);
            ViewBag.isUseUrl = isUseUrl;
            ViewBag.isDetails = isDetails;
            ViewBag.isDescription = isDescription;
        }
    }
}