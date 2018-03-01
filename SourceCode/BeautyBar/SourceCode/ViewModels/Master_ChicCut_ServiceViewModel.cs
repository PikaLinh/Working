using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;

namespace ViewModels
{
    public class Master_ChicCut_ServiceViewModel : Master_ChicCut_ServiceModel
    {
        public string HairTypeName { get; set; }

        [Display(Name = "Ghi chú")]
        public string QuantificationName { get; set; }
        public int QuantificationMasterId { get; set; }

        //Đặt hàng trc
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }
}