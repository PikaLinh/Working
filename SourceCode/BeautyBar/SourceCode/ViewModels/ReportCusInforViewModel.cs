using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class ReportCusInforViewModel : CustomerViewModel
    {
        //[Display(ResourceType = typeof(Resources.LanguageResource), Name = "FullName")]
        //public string FullName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Province")]
        public string ProvinceName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AccountModel_DistrictId")]
        public string DistrictName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Gender")]
        public string GenderString { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CustomerLevel")]
        public string CustomerLevelName { get; set; }

        [Display(Name = "Nợ phải thu")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? NoPhaiThu { get; set; }

        [Display(Name = "Số dư nợ đầu kỳ")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? SoDuNoDauKy { get; set; }

        [Display(Name = "Số dư nợ cuối kỳ")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? SoDuNoCuoiKy { get; set; }

        

    }
}
