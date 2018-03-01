using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class SettingViewModel
    {
        [Display(Name="Mã cài đặt")]
        public string SettingName { get; set; }
        public string SettingNameEn { get { return SettingName + "En"; } }
        [Display(Name="Chi tiết")]
        public string Details { get; set; }
        [Display(Name = "Chi tiết")]
        public string DetailsEn { get; set; }
    }
}