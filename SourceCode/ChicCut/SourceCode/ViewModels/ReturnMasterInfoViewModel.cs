﻿using EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class ReturnMasterInfoViewModel : ReturnMasterModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ReturnMasterId")]
        public int ReturnMasterId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ImportMasterId")]
        public string ImportMasterCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ReturnMasterId")]
        public string ReturnMasterCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WarehouseName")]
        public string WarehouseName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SupplierName")]
        public string SupplierName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SalemanName")]
        public string SalemanName { get; set; }

        [DisplayFormat(DataFormatString="{0:n0}")]
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalQty")]
        public decimal? TotalQty { get; set; }

        [DisplayFormat(DataFormatString = "{0:n0}")]
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalPrice")]
        public decimal? TotalPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss tt}")]
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedDate")]
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
