using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntityModels;

namespace ViewModels
{
    public class OrderViewModel : OrderMasterModel
    {
        [Display(Name = "Trạng thái")]
        public string StatusName { get; set; }

        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethodName { get; set; }

    }
}