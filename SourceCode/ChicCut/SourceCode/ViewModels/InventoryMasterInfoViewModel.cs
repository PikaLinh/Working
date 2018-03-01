using EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class InventoryMasterInfoViewModel:InventoryMasterModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeName")]
        public string EmployeeName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "InventoryTypeCode")]
        public string InventoryTypeCode { get; set; }
        public Nullable<bool> isImport { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WarehouseName")]
        public string WarehouseName { get; set; }
    }
}
