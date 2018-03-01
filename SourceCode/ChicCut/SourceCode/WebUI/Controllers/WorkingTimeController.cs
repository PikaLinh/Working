using EntityModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels.ChicCut;

namespace WebUI.Controllers
{
    public class WorkingTimeController : BaseController
    {
        // GET: WorkingTime
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        //Load giờ làm dựa vào ngày trong tuần
        public ActionResult GetTime(int PlanMasterId)
        {
            var time = _context.PlanModel.Where(p => p.PlanMasterId == PlanMasterId).FirstOrDefault();
            string FromTime = string.Format("{0:HH:mm}", DateTime.Parse(time.FromTime.ToString()));
            string ToTime = string.Format("{0:HH:mm}", DateTime.Parse(time.ToTime.ToString()));
            return Json(new { FromTime = FromTime, ToTime = ToTime }, JsonRequestBehavior.AllowGet);
        }

        #region//Xử lý giờ làm việc
        public ActionResult GetWorkingTime(TimeSpan WorkingFromTime, TimeSpan WorkingToTime, int PlanMasterId)
        {
            //Set 2 hours interval
            TimeSpan Interval1 = new TimeSpan(2, 0, 0);
            TimeSpan Interval2 = new TimeSpan(-2, 0, 0);
            //Set 1 hours interval
            TimeSpan Interval3 = new TimeSpan(-1, 0, 0);
            //Hiển thị danh sách khung giờ theo ngày, giờ bắt đầu và giờ kết thúc
            var detailList = _context.PlanDetailModel.Where(p => p.PlanMasterId == PlanMasterId).ToList();
            List<PlanDetailViewModel> planDetailList = new List<PlanDetailViewModel>();

            var plan = _context.PlanModel.Where(p => p.PlanMasterId == PlanMasterId).FirstOrDefault();

            double wfromTime = WorkingFromTime.TotalHours;
            double wtoTime = WorkingToTime.TotalHours;

            //Có khung giờ trong Db và giờ bắt đầu và giờ kết thúc được chọn giống trong DB
            if (detailList.Count > 0 && WorkingFromTime == plan.FromTime && WorkingToTime == plan.ToTime)
            {
                //Load thông tin khung giờ trong db
                foreach (var item in detailList)
                {
                    PlanDetailViewModel planDetail = new PlanDetailViewModel();
                    planDetail.PlanMasterId = PlanMasterId;
                    planDetail.PlanDetailId = item.PlanDetailId;
                    planDetail.TimeFrame = item.TimeFrame;
                    planDetail.TimeFrameString = string.Format("{0:HH:mm}", DateTime.Parse(item.TimeFrame.ToString()));
                    planDetail.AmountOfCus = item.AmountOfCus;
                    planDetailList.Add(planDetail);
                }
            }

            //Các trường hợp còn lại => Thêm mới khung giờ làm việc
            else
            {
                //Nếu khung giờ có thể chia thành 2 tiếng => Giữ nguyên WorkingToTime
                //Ngược lại => WorkingToTime trừ đi 2 tiếng 
                if ((wtoTime - wfromTime) % 2 != 0)
                {
                    WorkingToTime = WorkingToTime.Add(Interval2);
                }
                WorkingFromTime = WorkingFromTime.Add(Interval2);
                while (WorkingFromTime < WorkingToTime)
                {
                    //Khung giờ
                    WorkingFromTime = WorkingFromTime.Add(Interval1);

                    var modelAdd = new PlanDetailViewModel();
                    //Giờ
                    modelAdd.TimeFrameString = string.Format("{0:HH:mm}", DateTime.Parse(WorkingFromTime.ToString()));
                    modelAdd.TimeFrame = WorkingFromTime;
                    planDetailList.Add(modelAdd);
                }

            }
            return PartialView(planDetailList.OrderBy(p => p.TimeFrame));
        }
        #endregion

        #region//Lưu giờ làm
        [HttpPost]
        public ActionResult Save(PlanViewModel plan, int PlanMasterId, List<PlanDetailViewModel> planDetailList)
        {
            if (ModelState.IsValid)
            {
                #region //B1: Cập nhật Master
                PlanModel planModel = _context.PlanModel.Where(p => p.PlanMasterId == PlanMasterId).FirstOrDefault();
                planModel.FromTime = plan.FromTime;
                planModel.ToTime = plan.ToTime;
                #endregion //B1: Cập nhật Master

                #region //B2: Cập nhật Detail
                //TH1: Cũ không có trong mới -> Delete
                List<PlanDetailModel> detailList = _context.PlanDetailModel.Where(y => y.PlanMasterId == PlanMasterId).ToList();
                foreach (var item in detailList)
                {
                    PlanDetailViewModel pdVM = planDetailList.Where(z => z.PlanDetailId == item.PlanDetailId).FirstOrDefault();

                    if (pdVM == null)
                    {
                        _context.Entry(item).State = EntityState.Deleted;
                    }

                }
                foreach (var item in planDetailList)
                {
                    //TH2: Mới có trong cũ -> Modify
                    PlanDetailModel planDetail = _context.PlanDetailModel.Where(x => x.PlanDetailId == item.PlanDetailId).FirstOrDefault();
                    if (planDetail != null)
                    {

                        planDetail.TimeFrame = item.TimeFrame;
                        planDetail.AmountOfCus = item.AmountOfCus;
                        _context.Entry(planDetail).State = System.Data.Entity.EntityState.Modified;

                    }

                   //TH3: Mới không có trong cũ -> Add
                    else
                    {
                        planDetail = new PlanDetailModel();
                        planDetail.PlanMasterId = PlanMasterId;
                        planDetail.TimeFrame = item.TimeFrame;
                        planDetail.AmountOfCus = item.AmountOfCus;

                        _context.Entry(planDetail).State = System.Data.Entity.EntityState.Added;
                    }
                }
                #endregion //B2: Cập nhật Detail

                #region //B3: Cập nhật Master & Detail
                _context.Entry(planModel).State = EntityState.Modified;
                _context.SaveChanges();
                #endregion //B3: Cập nhật Master & Detail

                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Helper
        private void CreateViewBag(int? PlanMasterId = null)
        {
            #region Thứ
            var DayOfWeekList = _context.PlanModel.ToList();
            List<DayOfWeekViewModel> itemDOW = new List<DayOfWeekViewModel>();
            foreach (var item in DayOfWeekList)
            {
                //Ngày trong tuần
                string dayOfWeek = "";
                switch (item.DayOfWeek)
                {
                    case 0:
                        dayOfWeek = "Chủ nhật";
                        break;
                    case 1:
                        dayOfWeek = "Thứ 2";
                        break;
                    case 2:
                        dayOfWeek = "Thứ 3";
                        break;
                    case 3:
                        dayOfWeek = "Thứ 4";
                        break;
                    case 4:
                        dayOfWeek = "Thứ 5";
                        break;
                    case 5:
                        dayOfWeek = "Thứ 6";
                        break;
                    case 6:
                        dayOfWeek = "Thứ 7";
                        break;
                    default:
                        break;
                }
                itemDOW.Add(new DayOfWeekViewModel() { DayOfWeek = item.DayOfWeek, DayOfWeekString = dayOfWeek, PlanMasterId = item.PlanMasterId });
            }

            ViewBag.PlanMasterId = new SelectList(itemDOW, "PlanMasterId", "DayOfWeekString", PlanMasterId);
            #endregion

            #region Khung giờ làm việc
            // Set the start time (00:00 means 12:00 AM)
            DateTime StartTime = DateTime.ParseExact("08:00", "HH:mm", null);
            // Set the end time (23:55 means 11:55 PM)
            DateTime EndTime = DateTime.ParseExact("22:00", "HH:mm", null);
            //Set 30 minutes interval
            TimeSpan Interval = new TimeSpan(0, 30, 0);
            //To set 1 hour interval
            //TimeSpan Interval = new TimeSpan(1, 0, 0);

            List<string> ddlTimeFrom = new List<string>();
            List<string> ddlTimeTo = new List<string>();
            while (StartTime <= EndTime)
            {
                ddlTimeFrom.Add(string.Format("{0:HH:mm}", StartTime));
                ddlTimeTo.Add(string.Format("{0:HH:mm}", StartTime));
                StartTime = StartTime.Add(Interval);
            }
            ViewBag.FromTime = new SelectList(ddlTimeFrom, "", "");
            ViewBag.ToTime = new SelectList(ddlTimeTo, "", "");

            #endregion
        }

        #endregion
    }
}