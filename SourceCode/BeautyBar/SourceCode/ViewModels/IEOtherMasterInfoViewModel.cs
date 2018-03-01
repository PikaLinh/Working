using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModels;
using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class IEOtherMasterInfoViewModel : IEOtherMasterModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WarehouseName")]
        public string WarehouseName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SupplierName")]
        public string SupplierName { get; set; }
    }
}
