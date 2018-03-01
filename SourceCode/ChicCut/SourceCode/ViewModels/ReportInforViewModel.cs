using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class ReportInforViewModel
    {
        [Display(Name = "Thời gian")]
        public string ViewTime { get; set; }

        public string dateFormat {
            get {
                return string.Format("{0}-{1}-{2}"
                    ,ViewTime.Substring(6,4)
                    , ViewTime.Substring(3, 2)
                    , ViewTime.Substring(0, 2));
            }
        }
        [Display(Name = "Tổng tiền bán hàng")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? TotalPrice { get; set; }
        [Display(Name = "Phụ thu")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? AdditionalPrice { get; set; }

        [Display(Name = "Giảm giá")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? TotalBillDiscount { get; set; }

        [Display(Name = "VAT")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? TotalVAT { get; set; }

        [Display(Name = "Doanh thu")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? Revenue { get; set; }

        [Display(Name = "Vốn")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? COGS { get; set; }

        [Display(Name = "Tiền boa")]
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
        public decimal? Profit { get; set; }

        [Display(Name = "Tổng thu")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? TotalRevenue { get; set; }

        [Display(Name = "Tổng chi")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? TotalExpenditure { get; set; }

        [Display(Name = "Tổng cộng")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? TotalDifference { get; set; }

    }
}
