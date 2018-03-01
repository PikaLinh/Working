using EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class CRMPreviewViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "NextDateRemind")]
        public string NextDateRemindPreview { get; set; }

        public string ObjectName { get; set; }

        public string CustomerName { get; set; }
        public string SupplierName { get; set; }
        public string EmployeeName { get; set; }


        [Display( Name = "Tiêu đề")]
        public string Tile { get; set; }


        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Content")]
        public string Content { get; set; }



    }
}
