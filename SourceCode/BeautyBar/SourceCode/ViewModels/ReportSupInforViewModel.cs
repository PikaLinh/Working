using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using EntityModels;
namespace ViewModels
{
    public class ReportSupInforViewModel : SupplierModel
    {
        //[Display(ResourceType = typeof(Resources.LanguageResource), Name = "FullName")]
        //public string FullName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Province")]
        public string ProvinceName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AccountModel_DistrictId")]
        public string DistrictName { get; set; }

        [Display(Name = "Nợ phải trả")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? NoPhaiTra { get; set; }

        [Display(Name = "Số dư nợ đầu kỳ")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? SoDuNoDauKy { get; set; }

        [Display(Name = "Số dư nợ cuối kỳ")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? SoDuNoCuoiKy { get; set; }

        

    }
}
