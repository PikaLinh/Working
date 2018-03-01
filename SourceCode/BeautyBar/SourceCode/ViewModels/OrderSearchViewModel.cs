using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class OrderSearchViewModel
    {
        //Quyền Admin thì phải thấy được đơn hàng của 
        //All User ID, được quyền tìm kiếm theo 
        
        //từng khách hàng
        //thời gian đặt hàng
        //trạng thái đơn hàng 
        [Display(Name = "Khách hàng")]
        public int? CustomerId { get; set; }
        [Display(Name = "Từ ngày")]
        public DateTime? FromDate { get; set; }
        [Display(Name = "Đến ngày")]
        public DateTime? ToDate { get; set; }
        [Display(Name = "Trạng thái")]
        public int? StatusId { get; set; }
    }
}