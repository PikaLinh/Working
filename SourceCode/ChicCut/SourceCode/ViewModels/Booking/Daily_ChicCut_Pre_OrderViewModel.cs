using EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Booking
{
    public class Daily_ChicCut_Pre_OrderViewModel
    {
        //Tên khách hàng
        public string FullName { get; set; }
        //Sđt
        public string Phone { get; set; }
        //Giới tính
        public bool Gender { get; set; }
        //Ngày
        public DateTime BookingDate { get; set; }
        //Thời gian
        public TimeSpan BookingTime { get; set; }
        //Id khách hàng
        public int CustomerId { get; set; }
        //Giờ hẹn
        public DateTime AppointmentTime { get; set; }
        //Giờ đặt
        public DateTime CreatedDate { get; set; }
        //Ghi chú
        public string Note { get; set; }
        //KH chọn DV
        public string ServiceNote { get; set; }

    }
}
