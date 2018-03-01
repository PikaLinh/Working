using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.BeautyBar
{
    public class TreatmentDetailViewModel
    {
        public int ServiceId { get; set; }
        public int? CustomerId { get; set; }
        public int TreatmentId { get; set; }
        [Display(Name = "Mã liệu trình")]
        public string TreatmentCode { get; set; }
        [Display(Name = "Dịch vụ")]
        public string ServiceName { get; set; }
        [Display(Name = "Số lần sử dụng")]
        public int? ServiceQty { get; set; }
        [Display(Name = "Số lần đã sử dụng")]
        public int? ServiceQtyIsUsed { get; set; }
        [Display(Name = "Ngày thanh toán")]
        public string CashierDate { get; set; }
    }
}
