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
    public class CalendarController : BaseController
    {
        //
        // GET: /Calendar/

        int rootCategory = (int)ConstantCourseType.KhoaHoc;
        public ActionResult Index()
        {
            //var root = db.CategoryModel.Find(rootCategory);
            //var coursemodel = db.CourseModel.Where(p => p.CategoryModel.ADNCode.StartsWith(root.ADNCode)).Include(c => c.CategoryModel);

            var calendar = _context.CalendarModel.Include(c => c.CourseModel).Include(c => c.LocationModel);
            return View(calendar.ToList());
        }

        //
        // GET: /Calendar/Details/5

        public ActionResult Details(int id = 0)
        {
            CalendarModel CalendarModel = _context.CalendarModel.Find(id);
            if (CalendarModel == null)
            {
                return HttpNotFound();
            }
            return View(CalendarModel);
        }

        //
        // GET: /Calendar/Create

        public ActionResult Create()
        {
            CreateViewBag();
            ViewBag.Trainer = _context.TrainerModel.Where(p => p.Actived).ToList();
            CalendarModel CalendarModel = new CalendarModel() { Actived = true, TotalOfReg = 0};
            return View(CalendarModel);
        }

        //
        // POST: /Calendar/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(CalendarModel model, int[] trainers)
        {
            if (ModelState.IsValid)
            {
                if (trainers != null)
                {
                    foreach (var trainerId in trainers)
                    {
                        var trainer = _context.TrainerModel.Find(trainerId);
                        model.TrainerModel.Add(trainer);
                    }
                }
              
                model.UserCreated = currentAccount.UserName;
                model.DateCreated = DateTime.Now;

                _context.CalendarModel.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            if (trainers != null && trainers.Length > 0)
            {
                var newTrainers = _context.TrainerModel
                .Where(r => trainers.Contains(r.TrainerId))
                .ToList();

                foreach (var newTrainer in newTrainers)
                {
                    model.TrainerModel.Add(newTrainer);
                }
            }

            CreateViewBag(model.CourseId, model.LocationId);
            ViewBag.Trainer = _context.TrainerModel.Where(p => p.Actived).ToList();
            return View(model);
        }
      

        //
        // GET: /Calendar/Edit/5

        public ActionResult Edit(int id = 0)
        {
            CalendarModel CalendarModel = _context.CalendarModel.Include(p => p.DiscountModel).Where(p => p.CalendarId == id).FirstOrDefault();
            if (CalendarModel == null)
            {
                return HttpNotFound();
            }
            CreateViewBag(CalendarModel.CourseId, CalendarModel.LocationId);
            ViewBag.Trainer = _context.TrainerModel.Where(p => p.Actived).ToList();
            return View(CalendarModel);
        }

        //
        // POST: /Calendar/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(CalendarModel model, int[] trainers)
        {
            if (ModelState.IsValid)
            {
                var modelUpdate = _context.CalendarModel.Include(p => p.DiscountModel).Where(p => p.CalendarId == model.CalendarId).FirstOrDefault();
                modelUpdate.CourseId = model.CourseId;
                modelUpdate.Name = model.Name;
                modelUpdate.LocationId = model.LocationId;
                modelUpdate.StartDate = model.StartDate;
                modelUpdate.Time = model.Time;
                modelUpdate.Price = model.Price;
                modelUpdate.NumberOfTrainees = model.NumberOfTrainees;
                modelUpdate.isHot = model.isHot;
                modelUpdate.HotIndex = model.HotIndex;
                modelUpdate.Actived = model.Actived;
                //modelUpdate.UserCreated = model.UserCreated;
                //modelUpdate.DateCreated = model.DateCreated;
                modelUpdate.UserModified = currentAccount.UserName;
                modelUpdate.DateModified = DateTime.Now;

                modelUpdate.TrainerModel.Clear();
                if (trainers != null && trainers.Length > 0)
                {
                    var newTrainers = _context.TrainerModel
                    .Where(r => trainers.Contains(r.TrainerId))
                    .ToList();

                    foreach (var newTrainer in newTrainers)
                    {
                        modelUpdate.TrainerModel.Add(newTrainer);
                    }
                }
                //update discount
                if (modelUpdate.DiscountModel != null && modelUpdate.DiscountModel.Count > 0)
                {
                    while (modelUpdate.DiscountModel.Count > 0)
                    {
                        _context.Entry(modelUpdate.DiscountModel.First()).State = System.Data.EntityState.Deleted;
                    }
                }
                //add New
                foreach (var item in model.DiscountModel)
                {
                    modelUpdate.DiscountModel.Add(new DiscountModel() { 
                        //DiscountId = 0.
                        CalendarId = modelUpdate.CalendarId,
                        Days = item.Days,
                        Qty = item.Qty,
                        Discount = item.Discount,
                        Curent = item.Curent
                    });
                }
                _context.Entry(modelUpdate).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            if (trainers != null && trainers.Length > 0)
            {
                var newTrainers = _context.TrainerModel
                .Where(r => trainers.Contains(r.TrainerId))
                .ToList();

                foreach (var newTrainer in newTrainers)
                {
                    model.TrainerModel.Add(newTrainer);
                }
            }

            CreateViewBag(model.CourseId, model.LocationId);
            ViewBag.Trainer = _context.TrainerModel.Where(p => p.Actived).ToList();
            return View(model);
        }

        public ActionResult _DiscountList(List<DiscountModel> DiscountModel)
        {
            if (DiscountModel == null)
            {
                DiscountModel = new List<DiscountModel>();
            }
            return PartialView(DiscountModel);
        }


        public ActionResult _AddDiscount(List<DiscountModel> DiscountModel, int? txtDays, int? txtQty, decimal? txtDiscount)
        {
            if (DiscountModel == null)
            {
                DiscountModel = new List<DiscountModel>();
            }
            DiscountModel.Add(new DiscountModel()
            { 
                Days = txtDays,
                Curent = 0,
                Discount = txtDiscount,
                Qty = txtQty
            });
            return PartialView("_DiscountListInner", DiscountModel);
        }
        //_RemoveDiscount
        public ActionResult _RemoveDiscount(List<DiscountModel> DiscountModel, int STT)
        {
            if (DiscountModel == null)
            {
                DiscountModel = new List<DiscountModel>();
            }
            else
            {
                var item = DiscountModel[STT];
                DiscountModel.Remove(item);
            }
            return PartialView("_DiscountListInner", DiscountModel);
        }
        private void CreateViewBag(int? CourseId = null, int? LocationId = null)
        {
            var root = _context.CategoryModel.Find(rootCategory);
            var courselist = _context.CourseModel.Where(p => p.CategoryModel.ADNCode.StartsWith(root.ADNCode)).Select(p => new { p.CourseId, p.CourseName }).ToList();

            ViewBag.CourseId = new SelectList(courselist, "CourseId", "CourseName", CourseId);
            ViewBag.LocationId = new SelectList(_context.LocationModel, "LocationId", "LocationName", LocationId);
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
    }
}