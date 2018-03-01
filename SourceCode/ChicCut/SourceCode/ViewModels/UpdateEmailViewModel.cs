using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class UpdateEmailViewModel
    {
        [Display(Name = "Email hiện tại")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string FromEmail { get; set; }

        [Display(Name="Email mới")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Vui lòng nhập chính xác thông tin Email.")]
        public string ToEmail { get; set; }
    }
}