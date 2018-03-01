using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class ReportInforDetailViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderId")]
        public int OrderId { get; set; }
        [Display(Name = "Nhân viên (thợ phụ)")]
        public string StaffName { get; set; }

        [Display(Name = "NV thu ngân")]
        public string Cashier { get; set; }

        [Display(Name = "Khách hàng")]
        public string CustomerName { get; set; }

        [Display(Name = "Vốn")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? SumCOGSOfOrderDetail { get; set; }
        [Display(Name = "Tổng tiền")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? SumPriceOfOrderDetail { get; set; }
        [Display(Name = "Phụ thu")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? AdditionalPrice { get; set; }
        [Display(Name = "Giảm giá")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? TotalBillDiscount { get; set; }
        [Display(Name = "Doanh thu")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? Total { get; set; }
        [Display(Name = "Tiền Boa")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? Tip { get; set; }
        [Display(Name = "Hoa hồng dịch vụ")]
        [DisplayFormat(DataFormatString = "{0:n0}")]

        public decimal? Commission { get; set; }
        [Display(Name = "Hoa hồng sản phẩm")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? ProductCommission { get; set; }
        [Display(Name = "Hoa hồng ngày lễ")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? HolidayCommission { get; set; }
        [Display(Name = "Lợi nhuận")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? Profits { get; set; }
    }
}
