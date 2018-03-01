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
    public class CalendarOfEventController : BaseController
    {
        private EntityDataContext db = new EntityDataContext();

        //
        // GET: /Calendar/

        int rootCategory = (int)ConstantCourseType.HoiThaoSuKien;
        public ActionResult Index()
        {
            //var root = db.CategoryModel.Find(rootCategory);
            //var coursemodel = db.CourseModel.Where(p => p.CategoryModel.ADNCode.StartsWith(root.ADNCode)).Include(c => c.CategoryModel);

            var list = db.CalendarOfEventModel.Include(c => c.CourseModel).Include(c => c.LocationModel);
            return View(list.ToList());
        }

        //
        // GET: /Calendar/Details/5

        public ActionResult Details(int id = 0)
        {
            CalendarOfEventModel model = db.CalendarOfEventModel
                                           .Include(p => p.LocationModel)
                                           .Include(p => p.TrainerModel)
                                           .Where(p => p.EventId == id).FirstOrDefault();
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        //
        // GET: /Calendar/Create

        public ActionResult Create()
        {
            CreateViewBag();
            ViewBag.Trainer = db.TrainerModel.Where(p => p.Actived).ToList();
            CalendarOfEventModel model = new CalendarOfEventModel() { Actived = true };
            return View(model);
        }

        //
        // POST: /Calendar/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CalendarOfEventModel model, int[] trainers)
        {
            if (ModelState.IsValid)
            {
                if (trainers != null && trainers.Length > 0)
                {
                    foreach (var trainerId in trainers)
                    {
                        var trainer = db.TrainerModel.Find(trainerId);
                        model.TrainerModel.Add(trainer);
                    }
                }
                
                model.CreateDate = DateTime.Now;
                model.UserCreate = currentAccount.UserName;
                db.CalendarOfEventModel.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            CreateViewBag(model.CourseId, model.LocationId);
            return View(model);
        }
      

        //
        // GET: /Calendar/Edit/5

        public ActionResult Edit(int id = 0)
        {
            CalendarOfEventModel model = db.CalendarOfEventModel.Where(p => p.EventId == id).Include(p => p.TrainerModel).FirstOrDefault();
            if (model == null)
            {
                return HttpNotFound();
            }
            CreateViewBag(model.CourseId, model.LocationId);
            ViewBag.Trainer = db.TrainerModel.Where(p => p.Actived).ToList();
            return View(model);
        }

        //
        // POST: /Calendar/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CalendarOfEventModel model, int[] trainers)
        {
            if (ModelState.IsValid)
            {
                //model.TrainerModel == 2 item
                //trainers => Update

                var modelUpdate = db.CalendarOfEventModel.Where(p => p.EventId == model.EventId).FirstOrDefault();
                modelUpdate.CourseId = model.CourseId;
                modelUpdate.EventCode = model.EventCode;
                modelUpdate.StartDate = model.StartDate;
                modelUpdate.StartTime = model.StartTime;
                modelUpdate.EndTime = model.EndTime;
                modelUpdate.LocationId = model.LocationId;
                //modelUpdate.CreateDate = model.CreateDate;
                //modelUpdate.UserCreate = model.UserCreate;
                modelUpdate.Actived = model.Actived;

                modelUpdate.TrainerModel.Clear();
                if (trainers != null && trainers.Length > 0)
                {
                    var newTrainers = db.TrainerModel
                    .Where(r => trainers.Contains(r.TrainerId))
                    .ToList();

                    foreach (var newTrainer in newTrainers)
                    {
                        modelUpdate.TrainerModel.Add(newTrainer);
                    }
                }

                db.Entry(modelUpdate).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            CreateViewBag(model.CourseId, model.LocationId);
            return View(model);
        }

        private void CreateViewBag(int? CourseId = null, int? LocationId = null)
        { 
            var root = db.CategoryModel.Find(rootCategory);
            var courselist = db.CourseModel.Where(p => p.CategoryModel.ADNCode.StartsWith(root.ADNCode)).Select(p => new { p.CourseId, p.CourseName }).ToList();

            ViewBag.CourseId = new SelectList(courselist, "CourseId", "CourseName", CourseId);
            ViewBag.LocationId = new SelectList(db.LocationModel, "LocationId", "LocationName", LocationId);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}