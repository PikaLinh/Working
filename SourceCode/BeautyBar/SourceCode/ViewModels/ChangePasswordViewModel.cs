using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AccountModel_NewPassword")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Password)]
        [System.Web.Mvc.Compare("NewPassword", ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "PasswordDoNotMatch")]
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AccountModel_RetypeNewPassword")]
        public string retypeNewPassword { get; set; }
    }
}