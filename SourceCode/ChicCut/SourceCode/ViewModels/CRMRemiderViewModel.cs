using EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class CRMRemiderViewModel : CRM_RemiderModel
    {
        public string CustomerName { get; set; }
        public string SupplierName { get; set; }
        public string EmployeeName { get; set; }
        public string ObjectName { get; set; }

        [Display(Name = "Loại đối tượng")]
        public string ObjectType { get; set; }

        public string PeriodTypeName { get; set; }
        public string EmailTemplateName { get; set; }
        public string SMSTemplateName { get; set; }
    }
}
