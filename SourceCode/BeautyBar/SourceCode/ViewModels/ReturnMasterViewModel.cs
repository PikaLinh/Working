using EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class ReturnMasterViewModel : ReturnMasterModel
    {

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StoreId")]
        public string StoreName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WarehouseName")]
        public string WarehouseName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SupplierName")]
        public string SupplierName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ImportMasterId")]
        public string ImportMasterCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Currency")]
        public string CurrencyName { get; set; }

        [DisplayFormat(DataFormatString = "{0:n0}")]
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SumPrice")]
        public decimal? SumPrice { get; set; }


    }
}
