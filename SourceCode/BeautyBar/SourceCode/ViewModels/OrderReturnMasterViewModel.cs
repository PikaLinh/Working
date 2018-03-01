using EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class OrderReturnMasterViewModel : OrderReturnModel
    {
        public string StoreName { get; set; }
        public string WarehouseName { get; set; }
        public string CustomerLevel { get; set; }
        public string OrderCode { get; set; }
        public string FullName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Customer")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Customer")]
        public int CustomerId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CustomerLevel")]
        public Nullable<int> CustomerLevelId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IdentityCard")]
        public string IdentityCard { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Phone")]
        public string Phone { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Gender")]
        public Nullable<bool> Gender { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Email")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Province")]
        public Nullable<int> ProvinceId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AccountModel_DistrictId")]
        public Nullable<int> DistrictId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Address")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SaleName")]
        public string SaleName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CompanyName")]
        public string CompanyName { get; set; }

        [Display(Name = "Mã số thuế")]
        public string TaxBillCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ContractNumber")]
        public string ContractNumber { get; set; }

        [Display(Name = "Ngày kí hợp đồng")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> TaxBillDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DebtDueDate")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> DebtDueDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:n0}")]
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SumPrice")]
        public decimal? SumPrice { get; set; }

        public string Gender2 { get; set; }
        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerLevelName { get; set; }
    }
}
