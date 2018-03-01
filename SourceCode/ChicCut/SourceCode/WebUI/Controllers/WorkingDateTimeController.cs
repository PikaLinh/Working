using EntityModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels.Booking;
using ViewModels.ChicCut;

namespace WebUI.Controllers
{
    public class WorkingDateTimeController : BaseController
    {
        // GET: WorkingDateTime
        public ActionResult Index()
        {
            return View(_context.Daily_ChicCut_WorkingDateModel.ToList());
        }

        #region Thêm ngày nghỉ
        [HttpPost]
        public ActionResult Create(DateTime DayOff)
        {
            if (ModelState.IsValid)
            {
                //Nếu chưa có trong DB thì thêm vào
                var model = _context.Daily_ChicCut_WorkingDateModel.Where(p => p.DayOff == DayOff.Date).FirstOrDefault();
                if (model == null)
                {
                    var modelAdd = new Daily_ChicCut_WorkingDateModel()
                    {
                        DayOff = DayOff.Date
                    };
                    _context.Entry(modelAdd).State = EntityState.Added;
                    _context.SaveChanges();
                }
                return Json(new
                {
                    Result = true
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                Result = false,
                ErrorMessage = "Loi"
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Xóa ngày nghỉ
        [HttpPost]
        public ActionResult Delete(Daily_ChicCut_WorkingDateModel model, DateTime DayOn)
        {
            //Nếu có ngày nghỉ thì xóa
            var deleteDayOff = _context.Daily_ChicCut_WorkingDateModel.Where(p => p.DayOff == DayOn).FirstOrDefault();

            _context.Entry(deleteDayOff).State = EntityState.Deleted;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        #endregion

        #region//Lấy thông tin ngày nghỉ từ Database
        public ActionResult GetDatesArray()
        {
            //Khởi tạo list chứa ngày nghỉ
            List<string> list = new List<string>();
            //Lấy ngày nghỉ từ DB
            List<Daily_ChicCut_WorkingDateModel> dayOffList = _context.Daily_ChicCut_WorkingDateModel.ToList();
            dayOffList.RemoveAll(p => p.DayOff.Date < DateTime.Now.Date);
            foreach (var item in dayOffList)
            {
                list.Add(string.Format("{0:dd/MM/yyyy}", item.DayOff));
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}