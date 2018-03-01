using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class DailyServicesViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ServiceName")]
        public string ServiceName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "HairTypeName")]
        public string HairTypeName { get; set; }
        public string PaymentMethodId { get; set; }
        [Display(Name = "Phương thức")]
        public string PaymentMethodName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Price")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal Price { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Qty")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Qty { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "UnitPrice")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal UnitPrice { get; set; }
    }
}
