using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Constant;
using ViewModels;

namespace WebUI.Controllers
{
    public class DisplayCourseController : BaseController
    {
        //
        // GET: /DisplayCourse/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Event(int id = (int)ConstantCourseType.HoiThaoSuKien)
        {
            var cat = _context.CategoryModel.Find(id);
            if (cat == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.Title = cat.CategoryName;

                var thisDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var result = from course in _context.CourseModel
                             join category in _context.CategoryModel on course.CategoryId equals category.CategoryId
                             join calendar in _context.CalendarOfEventModel on course.CourseId equals calendar.CourseId
                             where category.ADNCode.StartsWith(cat.ADNCode) && calendar.StartDate >= thisDate
                             orderby calendar.StartDate
                             select calendar;
                return View(result.ToList());
            }
        }

        public ActionResult Test(int? id = null, bool isCourse = false)
        {
            string domain = "/";
            if (isCourse)
            {
                ViewBag.url = string.Format("{0}DisplayCourse/Course/{1}", domain, id);
            }
            else
            {
                ViewBag.url = string.Format("{0}DisplayCourse/Event/{1}", domain, id);
            }
            return View();
        }

        public ActionResult Course(int id = (int)ConstantCourseType.KhoaHoc, int SortType = EnumSortType.KhoaGanNhat)
        {
            var cat = _context.CategoryModel.Find(id);
            if (cat == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.Title = cat.CategoryName;
                ViewBag.Id = id;

                var thisDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var result =    (from course in _context.CourseModel
                                join category in _context.CategoryModel on course.CategoryId equals category.CategoryId
                                join calendar in _context.CalendarModel on course.CourseId equals calendar.CourseId
                                join location in _context.LocationModel on calendar.LocationId equals location.LocationId
                                where category.ADNCode.StartsWith(cat.ADNCode) && 
                                    calendar.StartDate >= thisDate &&
                                    calendar.Actived &&
                                    course.Actived
                                orderby calendar.StartDate
                                select new CalendarViewModel()
                                {
                                    CalendarId = calendar.CalendarId,
                                    TotalOfReg = calendar.TotalOfReg,
                                    StartDate = calendar.StartDate,
                                    Name = calendar.Name,
                                    Time = calendar.Time,
                                    Price = calendar.Price,
                                    NumberOfTrainees = calendar.NumberOfTrainees,
                                    TrainerModel = calendar.TrainerModel,
                                    CourseUrl = course.Url,
                                    CourseName = course.CourseName,
                                    LocationUrl = location.Url,
                                    LocationName = location.LocationName,
                                    DiscountModel = calendar.DiscountModel,
                                    SEOCategory = category.SEOCategoryName
                                }).ToList();
                //Tính new Price
                foreach (var calendar in result)
                {
                    if ((calendar.NumberOfTrainees - (calendar.TotalOfReg??0)) > 0)
                    {
                        var thisDay = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day);

                        var discount = (from d in calendar.DiscountModel
                                        where (d.Curent == null || (d.Qty ?? 0 - d.Curent ?? 0) > 0) &&
                                              calendar.StartDate.AddDays(-d.Days.Value) >= thisDay
                                        orderby d.Discount descending
                                        select new { Curent = d.Curent ?? 0, Discount = d.Discount, Qty = d.Qty??0 })
                                        .FirstOrDefault();
                        if (discount != null && discount.Discount.HasValue)
                        {
                            calendar.Discount = discount.Discount.Value;
                            calendar.NewPrice = calendar.Price.Value - calendar.Discount;
                            calendar.TotalOfDiscount = discount.Qty - discount.Curent;
                        }
                        else
                        {
                            calendar.Discount = 0;
                            calendar.NewPrice = calendar.Price.Value;
                            calendar.TotalOfDiscount = 0;
                        }
                    }
                    else
                    {
                        calendar.Discount = 0;
                        calendar.NewPrice = calendar.Price.Value;
                        calendar.TotalOfDiscount = 0;
                    }
                }
                var data = result.OrderBy(p => p.StartDate).ToList();
                if (Request.IsAjaxRequest())
                {
                    if (SortType == EnumSortType.KhoaCoUuDaiNhat)
                    {
                        data = result.OrderByDescending(p => p.Discount).ToList();
                    }
                    return PartialView("_CourseCalendarPartial", data);
                }
                else
                {
                    return PartialView(data);
                }
            }
        }

        
    }
}
