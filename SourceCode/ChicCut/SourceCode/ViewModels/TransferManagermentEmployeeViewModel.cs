using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntityModels;

namespace ViewModels
{
    public class TransferManagermentEmployeeViewModel
    {
        [Display(Name = "Nhân viên hiện tại")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public int EmployeeCurrentId { get; set; }


        [Display(Name = "Nhân viên mới")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public int EmployeeNewId { get; set; }
       // public int ListCustomerId { get; set; }
    }
}