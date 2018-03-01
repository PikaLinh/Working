using Constant;
using EntityModels;
using Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using ViewModels.Booking;
using ViewModels.ChicCut;

namespace WebUI.FrontEnd.Controllers
{
    public class BookingController : Controller
    {
        EntityDataContext _context = new EntityDataContext();
        // GET: Booking
        public ActionResult Index()
        {
            return View();
        }

        #region Lấy ngày
        public ActionResult GetWorkingDay()
        {
            List<WorkingDayViewModel> workingdays = new List<WorkingDayViewModel>();
            WorkingDayViewModel modelAdd1 = GetDay(DateTime.Now.Date);
            //Truyền thông tin qua View
            ViewBag.Today = modelAdd1.Title;
            ViewBag.DayWithFormatToday = modelAdd1.DayWithFormat;
            ViewBag.DayOfWeekToday = modelAdd1.DayOfWeek;
            workingdays.Add(modelAdd1);

            WorkingDayViewModel modelAdd2 = GetDay(DateTime.Now.Date.AddDays(1));
            //Truyền thông tin qua View
            ViewBag.Tomorrow = modelAdd2.Title;
            ViewBag.DayWithFormatTomorrow = modelAdd2.DayWithFormat;
            ViewBag.DayOfWeekTomorrow = modelAdd2.DayOfWeek;
            workingdays.Add(modelAdd2);

            WorkingDayViewModel modelAdd3 = GetDay(DateTime.Now.Date.AddDays(2));
            //Truyền thông tin qua View
            ViewBag.AfterTomorrow = modelAdd3.Title;
            ViewBag.DayWithFormatAfterTomorrow = modelAdd3.DayWithFormat;
            ViewBag.DayOfWeekAfterTomorrow = modelAdd3.DayOfWeek;
            workingdays.Add(modelAdd3);

            return Json(new
            {
                workingdays = workingdays,
                //Today
                Today = ViewBag.Today,
                DayWithFormatToday = ViewBag.DayWithFormatToday,
                DayOfWeekToday = ViewBag.DayOfWeekToday,
                //Tomorrow
                Tomorrow = ViewBag.Tomorrow,
                DayWithFormatTomorrow = ViewBag.DayWithFormatTomorrow,
                DayOfWeekTomorrow = ViewBag.DayOfWeekTomorrow,
                //AfterTomorrow
                AfterTomorrow = ViewBag.AfterTomorrow,
                DayWithFormatAfterTomorrow = ViewBag.DayWithFormatAfterTomorrow,
                DayOfWeekAfterTomorrow = ViewBag.DayOfWeekAfterTomorrow

            }, JsonRequestBehavior.AllowGet);
        }

        private WorkingDayViewModel GetDay(DateTime dateTime)
        {
            //Khởi tạo thông tin từ ngày truyền vào
            WorkingDayViewModel retModel = new WorkingDayViewModel();
            //Tiêu đề
            if (dateTime == DateTime.Now.Date)
            {
                retModel.Title = "Hôm nay";
            }
            else if (dateTime == DateTime.Now.Date.AddDays(1))
            {
                retModel.Title = "Ngày mai";
            }
            else if (dateTime == DateTime.Now.Date.AddDays(2))
            {
                retModel.Title = "Ngày kia";
            }

            //Ngày with Format 06/01/2018 => "06/01"
            retModel.DayWithFormat = string.Format("{0:dd/MM}", dateTime);
            //Ngày Data 06/01/2018 => "2018-01-06"
            retModel.Day = string.Format("{0:yyyy-MM-dd}", dateTime);
            //Ngày trong tuần
            string dayOfWeek = "";
            switch ((int)dateTime.DayOfWeek)
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
            retModel.DayOfWeek = dayOfWeek;
            return retModel;
        }
        #endregion

        #region Lấy khung giờ
        public ActionResult GetWorkingTimeFrame(DateTime? BookingDate = null)
        {
            //Nếu không truyền ngày => set ngày đặt mặc định => ngày hiện tại
            if (!BookingDate.HasValue)
            {
                BookingDate = DateTime.Now.Date;
            }
            //Giờ làm việc:
            //Cấu hình trong bảng: Daily_ChicCut_WorkingTimeModel
            //VD: thời gian giam làm việc từ: 10:30 sáng đến 18h00
            //var fromTime = _context.Daily_ChicCut_WorkingTimeModel.Select(p => p.FromTime).FirstOrDefault();
            //var toTime = _context.Daily_ChicCut_WorkingTimeModel.Select(p => p.ToTime).FirstOrDefault();
            //Convert Time > To DateTime
            //DateTime dt1 = DateTime.Parse(fromTime.ToString());
            //DateTime dt2 = DateTime.Parse(toTime.ToString());
            //Danh sách các khung giờ có thể đặt trong ngày "BookingDate"
            //List<TimeFrameViewModel> timeFrame = new List<TimeFrameViewModel>();
            //while (dt1 < dt2.AddMinutes(-30))
            //{
            //    //Khung giờ
            //    dt1 = dt1.AddMinutes(30);

            //    var modelAdd = new TimeFrameViewModel();
            //    //Giờ
            //    modelAdd.TimeFrame = string.Format("{0:HH:mm}", dt1);
            //    //Màu sắc: RED, GREEN
            //    if (BookingDate != DateTime.Now.Date || dt1.CompareTo(DateTime.Now) > 0)
            //    {
            //        modelAdd.Color = ConstantColor.GREEN;
            //    }
            //    else
            //    {
            //        modelAdd.Color = ConstantColor.RED;
            //    }

            //    timeFrame.Add(modelAdd);
            //}


            var BookingDateInt = (int)BookingDate.Value.DayOfWeek;

            //Lấy dữ liệu từ PreOrderModel từ ngày hiện tại trở đi để đếm lượt khách
            var SlotList = _context.Daily_ChicCut_Pre_OrderModel.Where(p => DbFunctions.TruncateTime(p.AppointmentTime) >= DbFunctions.TruncateTime(DateTime.Now)).ToList();

            //Load khung giờ trong db
            var timeFrame = (from p in _context.PlanModel
                             join pd in _context.PlanDetailModel on p.PlanMasterId equals pd.PlanMasterId
                             where p.DayOfWeek == BookingDateInt
                             orderby pd.TimeFrame
                             select new
                             {
                                 //Ngày trong tuần
                                 dayOfWeek = p.DayOfWeek,
                                 //Khung giờ
                                 timeFrame = pd.TimeFrame,
                                 //Lượt khách
                                 amountOfCus = pd.AmountOfCus
                             }
                            ).AsEnumerable()
                              .Select(p => new TimeFrameViewModel()
                              {
                                  //Khung giờ
                                  TimeFrame = string.Format("{0:HH:mm}", DateTime.Parse(p.timeFrame.ToString())),

                                  //Màu sắc: RED, GREEN
                                  //GREEN:
                                          //1.Ngày khác ngày hiện tại
                                  Color = (BookingDate != DateTime.Now.Date ||
                                          //2.Khung giờ > giờ hiện tại
                                          DateTime.Parse(p.timeFrame.ToString()).CompareTo(DateTime.Now) > 0) &&
                                          //3.Lượt khách đặt hàng nhỏ hơn quy định lượt khách trong PlanModel
                                          SlotList.Where(q => TimeSpan.Compare(q.AppointmentTime.Value.TimeOfDay, p.timeFrame.Value) == 0
                                          && (int)q.AppointmentTime.Value.DayOfWeek == p.dayOfWeek).ToList().Count() < p.amountOfCus ?
                                          ConstantColor.GREEN : ConstantColor.RED
                              }).ToList();




            //Nếu là ngày nghỉ => Cửa hàng không thể phục vụ thời điểm này
            var dayOff = _context.Daily_ChicCut_WorkingDateModel.ToList();
            foreach (var item in dayOff)
            {
                if (BookingDate == item.DayOff)
                {
                    return Json("Cửa hàng không thể phục vụ thời điểm này. Quý khách vui lòng ghé thăm vào thời điểm khác.", JsonRequestBehavior.AllowGet);
                }

            }

            return Json(new { timeFrame = timeFrame }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //Xử lý đặt hàng
        public ActionResult BookingOrder(Daily_ChicCut_Pre_OrderViewModel orderModel, string Phone, DateTime BookingDate, List<int> ServiceNoteList)
        {
            if (ModelState.IsValid)
            {
                if(string.IsNullOrEmpty(Phone) || string.IsNullOrEmpty(orderModel.FullName))
                {
                    return Json(new { Message = "(*) Vui lòng nhập thông tin bắt buộc!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    #region //Tìm khách hàng qua sđt
                    var customer = _context.CustomerModel.Where(p => p.Phone == Phone).FirstOrDefault();
                    //Không có KH trong Db => thêm mới
                    if (customer == null)
                    {
                        CustomerModel cus = new CustomerModel();
                        //Thêm những field bắt buộc vào CustomerModel
                        cus.CustomerLevelId = 1;
                        cus.FullName = orderModel.FullName;
                        cus.ShortName = orderModel.FullName;
                        cus.Phone = orderModel.Phone;
                        cus.Gender = orderModel.Gender;
                        cus.Actived = true;
                        cus.RegDate = DateTime.Now;
                        _context.Entry(cus).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();
                        orderModel.CustomerId = cus.CustomerId;
                    }
                    //Có KH trong Db => cập nhật tên, giới tính dựa vào sđt
                    else
                    {
                        orderModel.CustomerId = customer.CustomerId;
                        customer.ShortName = orderModel.FullName;
                        customer.Gender = orderModel.Gender;
                        _context.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                    }
                    #endregion
                }
               

                //KH chưa chọn khung giờ
                if (orderModel.BookingTime == TimeSpan.Zero)
                {
                    return Json(new { Message1 = "(*) Khung giờ chưa được chọn! Vui lòng chọn khung giờ phục vụ." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //Mỗi sđt chỉ đặt được một lần trong ngày
                    var preOrder = _context.Daily_ChicCut_Pre_OrderModel.Where(p => p.Phone == Phone && DbFunctions.TruncateTime(p.AppointmentTime) == BookingDate.Date).FirstOrDefault();
                    if (preOrder != null)
                    {
                        return Json(new { Message2 = "Số điện thoại này đã được đặt trong ngày!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //KH đặt trước dịch vụ: 
                        //- Cắt
                        //- Uốn
                        //- Duỗi
                        //- Nhuộm
                        List<string> ServiceNoteListString = new List<string>();
                        foreach (var item in ServiceNoteList)
                        {
                            string haircuts = item == 0 ? "Cắt" : (item == 1 ? "Uốn" : (item == 2 ? "Duỗi" : "Nhuộm"));
                            ServiceNoteListString.Add(haircuts);
                        }
                        orderModel.ServiceNote = String.Join(", ", ServiceNoteListString);

                        Daily_ChicCut_Pre_OrderModel order = new Daily_ChicCut_Pre_OrderModel();
                        //Thêm những field bắt buộc vào Daily_ChicCut_Pre_OrderModel
                        order.CustomerId = orderModel.CustomerId;
                        order.FullName = orderModel.FullName;
                        order.Gender = orderModel.Gender;
                        order.Phone = orderModel.Phone;
                        order.ServicesNote = orderModel.ServiceNote;
                        order.Note = orderModel.Note;
                        //Đơn hàng đặt trước => OrderStatusId = 5
                        order.OrderStatusId = 5;
                        //Xử lý ngày giờ hẹn
                        orderModel.AppointmentTime = orderModel.BookingDate + orderModel.BookingTime;
                        order.AppointmentTime = orderModel.AppointmentTime;
                        //Ngày giờ đặt
                        orderModel.CreatedDate = DateTime.Now;
                        order.CreatedDate = orderModel.CreatedDate;
                        //Thêm vào Db
                        _context.Entry(order).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();
                        //Tạo số phiếu đặt từ PreOrderId
                        order.PreOrderCode = (order.PreOrderId % 1000).ToString("D3");

                        #region // Gửi SMS cho khách hàng
                        SendSMSRepository SMSRepo = new SendSMSRepository();
                        string name = order.FullName.LastIndexOf(' ') > 0 ? order.FullName.Substring(order.FullName.LastIndexOf(' ') + 1) : order.FullName;
                        name = Library.ConvertToNoMarkString(name);
                        if (!string.IsNullOrEmpty(name))
                        {
                            name = name.First().ToString().ToUpper() + name.Substring(1).ToLower();
                        }
                        string mes = string.Format(
                                                    "Cam on {0} {1} dat hen tai Chic Cut, thoi gian hen la: {2:HH:mm dd/MM/yyyy}. {0} vui long den dung gio da hen.",
                                                    order.Gender.HasValue && order.Gender.Value ? "anh" : "chi",
                                                    name,
                                                    order.AppointmentTime
                                                    );
                        AY_SMSCalendar smsModel = new AY_SMSCalendar()
                        {
                            EndDate = DateTime.Now,
                            isSent = true,
                            NumberOfFailed = 0,
                            SMSContent = mes,
                            SMSTo = order.Phone
                        };
                        _context.Entry(smsModel).State = System.Data.Entity.EntityState.Added;
                        #endregion
                        _context.SaveChanges();

                        SMSRepo.SendSMSModel(smsModel);

                        _context.SaveChanges();
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult OrderSuccess()
        {
            return View();
        }

        //Thông báo
        public ActionResult Alert(string Content)
        {
            ViewBag.Content = Content;
            return PartialView();
        }
    }
}