using EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace WebUI.Controllers
{
    public class FormController : BaseController
    {
        //
        // GET: /Form/Register/id

        public ActionResult Register(int id, bool isCourse)
        {
            ViewBag.isCourse = isCourse;
            var registry = new RegistryModel();
            if (isCourse)
            {
                registry.CalendarId = id;
                var calendar = (from course in _context.CourseModel
                                join category in _context.CategoryModel on course.CategoryId equals category.CategoryId
                                join c in _context.CalendarModel on course.CourseId equals c.CourseId
                                join location in _context.LocationModel on c.LocationId equals location.LocationId
                                where c.CalendarId == id
                                select new CalendarViewModel()
                                {
                                    CourseId = c.CourseId,
                                    CalendarId = c.CalendarId,
                                    TotalOfReg = c.TotalOfReg,
                                    StartDate = c.StartDate,
                                    Name = c.Name,
                                    Time = c.Time,
                                    Price = c.Price,
                                    NumberOfTrainees = c.NumberOfTrainees,
                                    TrainerModel = c.TrainerModel,
                                    CourseUrl = course.Url,
                                    CourseName = course.CourseName,
                                    LocationUrl = location.Url,
                                    LocationName = location.LocationName,
                                    DiscountModel = c.DiscountModel,
                                    SEOCategory = category.SEOCategoryName
                                }).FirstOrDefault();

                //
                if ((calendar.NumberOfTrainees - (calendar.TotalOfReg ?? 0)) > 0)
                {
                    var thisDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                    var discount = (from d in calendar.DiscountModel
                                    where (d.Curent == null || (d.Qty ?? 0 - d.Curent ?? 0) > 0) &&
                                          calendar.StartDate.AddDays(-d.Days.Value) >= thisDay
                                    orderby d.Discount descending
                                    select new { Curent = d.Curent ?? 0, Discount = d.Discount, Qty = d.Qty ?? 0 })
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

                registry.CourseId = calendar.CourseId;
                registry.Price = calendar.NewPrice;
                ViewBag.Title = calendar.Name + " - " + calendar.CourseName;
                ViewBag.StartDate = calendar.StartDate.ToString("dd/MM/yyyy");
                ViewBag.PriceDisplay = calendar.NewPrice.ToString("#,#đ");
                ViewBag.Url = calendar.CourseUrl;
                ViewBag.Time = calendar.Time;

            }
            else
            {
                var model = _context.CalendarOfEventModel.Where(p => p.EventId == id).FirstOrDefault();
                registry.EventId = id;
                registry.CourseId = model.CourseId.Value;

                ViewBag.Title = model.EventCode + " - " + model.CourseModel.CourseName;
                ViewBag.StartDate = model.StartDate.Value.ToString("dd/MM/yyyy");
                ViewBag.Price = 0;
                ViewBag.PriceDisplay = "0đ";
                ViewBag.Url = model.CourseModel.Url;
            }
            
            return View(registry);
        }

        [HttpPost]
        public ActionResult RegisterCourse(RegistryModel model)
        {
            //calendarid
            model.Actived = null;

            #region check
            var calendar = (from course in _context.CourseModel
                            join category in _context.CategoryModel on course.CategoryId equals category.CategoryId
                            join c in _context.CalendarModel on course.CourseId equals c.CourseId
                            join location in _context.LocationModel on c.LocationId equals location.LocationId
                            where c.CalendarId == model.CalendarId
                            select new CalendarViewModel()
                            {
                                CalendarId = c.CalendarId,
                                TotalOfReg = c.TotalOfReg,
                                StartDate = c.StartDate,
                                Name = c.Name,
                                Time = c.Time,
                                Price = c.Price,
                                NumberOfTrainees = c.NumberOfTrainees,
                                TrainerModel = c.TrainerModel,
                                CourseUrl = course.Url,
                                CourseName = course.CourseName,
                                LocationUrl = location.Url,
                                LocationName = location.LocationName,
                                DiscountModel = c.DiscountModel,
                                SEOCategory = category.SEOCategoryName
                            }).FirstOrDefault();

            //Nếu còn khóa học
            if ((calendar.NumberOfTrainees - (calendar.TotalOfReg ?? 0)) > 0)
            {
                var thisDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                var discount = (from d in calendar.DiscountModel
                                where (d.Curent == null || (d.Qty ?? 0 - d.Curent ?? 0) > 0) &&
                                      calendar.StartDate.AddDays(-d.Days.Value) >= thisDay
                                orderby d.Discount descending
                                select d) //new { Curent = d.Curent ?? 0, Discount = d.Discount, Qty = d.Qty ?? 0 })
                                .FirstOrDefault();
                if (discount != null && discount.Discount.HasValue)
                {
                    calendar.Discount = discount.Discount.Value;
                    calendar.NewPrice = calendar.Price.Value - calendar.Discount;
                    calendar.TotalOfDiscount = (discount.Qty??0) - (discount.Curent??0);
                }
                else
                {
                    calendar.Discount = 0;
                    calendar.NewPrice = calendar.Price.Value;
                    calendar.TotalOfDiscount = 0;
                }


                //Nếu còn khuyến mãi
                //Cập nhật thoogn tin khuyến mãi
                if (model.Price == calendar.NewPrice)
                {
                    if (calendar.Discount > 0)
                    {
                        model.DiscountId = discount.DiscountId;
                        discount.Curent = (discount.Curent ?? 0) + 1;
                        _context.Entry(discount).State = System.Data.EntityState.Modified;
                    }
                    //Cập nhật số lượng đăng ký khóa học
                    var calendarModel = _context.CalendarModel.Find(calendar.CalendarId);
                    calendarModel.TotalOfReg = (calendar.TotalOfReg ?? 0) + 1;
                    _context.Entry(calendarModel).State = System.Data.EntityState.Modified;
                    //Thêm mới đăng ký
                    _context.Entry(model).State = System.Data.EntityState.Added;
                    _context.SaveChanges();
                    ViewBag.IsSuccess = true;
                }
                else
                {
                    ViewBag.Message = "Thật sự xin lỗi bạn! Suất ưu đãi này đã hết! Do có người khác đăng ký trước bạn. Bạn vui lòng đăng ký lại với suất ưu đãi khác.";
                }
            }
            else
            {
                ViewBag.Message = "Thật sự xin lỗi bạn! Khóa học đã hết suất để đăng ký! vui lòng liên hệ với chúng tôi để được tư vấn.";
            }
            #endregion
            return View(model);
        }
        [HttpPost]
        public ActionResult RegisterEvent(RegistryModel model)
        {
            //Thêm mới đăng ký
            _context.Entry(model).State = System.Data.EntityState.Added;
            _context.SaveChanges();
            ViewBag.IsSuccess = true;
            return View("RegisterCourse", model);
        }
    }
}
