using EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class InventoryInfoViewModel : InventoryDetailModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "InventoryTypeCode")]
        public string InventoryTypeCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedDate")]
        [DisplayFormat(DataFormatString = "{0:HH:mm tt dd/MM/yyyy}")]
        public Nullable<System.DateTime> CreatedDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedEmployeeId")]
        public Nullable<int> CreatedEmployeeId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductShortName")]
        public string ProductName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EmployeeName")]
        public string EmployeeName { get; set; }
        public string ActionUrl { get; set; }
        public int?BusinessId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "InventoryCode")]
        public string InventoryCode { get; set; }

        public string ProductCode { get; set; }

        public string Specifications { get; set; }


    }
}
