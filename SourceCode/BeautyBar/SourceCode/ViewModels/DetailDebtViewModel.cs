using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntityModels;

namespace ViewModels
{
    public class DetailDebtViewModel 
    {
        [Display(Name = "Mã phiếu")]
        public string OrderCode { get; set; }

        [Display(Name = "Thời gian")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> CreatedDate { get; set; }

        [Display(Name = "Loại")]
        public string Loai { get; set; }

        [Display(Name = "Giá trị")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public Nullable<decimal> TotalPrice { get; set; } // Giá trị

        [Display(Name = "Dư nợ khách hàng")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public Nullable<decimal> RemainingAmountAccrued { get; set; } // Dư nợ khách hàng

        
    }
}