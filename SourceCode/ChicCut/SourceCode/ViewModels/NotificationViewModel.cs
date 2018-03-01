using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntityModels;

namespace ViewModels
{
    public class NotificationViewModel : NotificationModel
    {
        [Display(Name = "Nhân viên")]
        public string UserName{ get; set; }
        public string Url { get; set; }
    }
}