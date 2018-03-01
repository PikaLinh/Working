using EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class OrderMasterInfoViewModel : OrderMasterModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StatusCode")]
        public string StatusCode { get; set; }

        public int PreOrderId { get; set; }

        public bool Actived { get; set; }

        public string PreOrderCode { get; set; }
        public string EmployeeName { get; set; }
    }
}
