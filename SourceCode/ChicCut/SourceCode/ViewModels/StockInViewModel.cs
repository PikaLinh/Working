using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class StockInViewModel
    {
        public int STT { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductName")]
        public string ProductName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductCode")]
        public string ProductCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "UnitName")]
        public string UnitName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Qty")]
        public Nullable<decimal> Qty { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Price")]
        public Nullable<decimal> Price { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "UnitPrice")]
        public Nullable<decimal> UnitPrice { get; set; }


    }
}
