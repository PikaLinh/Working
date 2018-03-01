using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class ReportCusTransactionViewModel 
    {

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm tt}")]
        [Display(Name = "Ngày giao dịch")]
        public Nullable<System.DateTime> DateTransaction { get; set; }

        [Display(Name = "Mã phiếu")]
        public string TransactionCode { get; set; }

        [Display(Name = "Thay đổi")]
        public string Change { get; set; }

        [Display(Name = "Số tiền")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? Amout { get; set; } 
        
        [Display(Name = "Dư nợ cần thu")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? RemainingAmountAccrued { get; set; } 

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

    }
}
