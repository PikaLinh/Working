using EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class ProductInfoViewModel : ProductModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CategoryId")]
        public string CategoryName { get; set; }

        public string ProductTypeName { get; set; }

        public string PolicyInStockName { get; set; }

        public string PolicyOutOfStockName { get; set; }

        public string ProductStatusName { get; set; }

        public string LocationOfProductName { get; set; }

        public string UnitName { get; set; }

        public string CurrencyName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OriginOfProductId")]
        public string OriginOfProductName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Price")]
        [DisplayFormat(DataFormatString="{0:n0}")]
        public decimal? Price { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CustomerLevel")]
        public string CustomerLevel { get; set; }

        [Display(Name = "Tồn")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? Inventory { get; set; }

        [Display(Name = "Tỷ giá")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? ExchangeRate { get; set; }


        [Display(Name = "Vip")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? Price1 { get; set; }

        [Display(Name = "Vip-Bạc")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? Price2 { get; set; }

        [Display(Name = "Vip-Vàng")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? Price3 { get; set; }

        [Display(Name = "Vip-Bạch Kim")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public decimal? Price4 { get; set; }



        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OriginOfProduct")]
        public string OriginOfProduct { get; set; }

        public string WarehouseName { get; set; }
        public decimal EndInventoryQty { get; set; }
        public decimal ExportQty { get; set; }
        public decimal ImportQty { get; set; }
       // public decimal BeginInventoryQty { get; set; }
        public string InventoryCode { get; set; }
        public string ShortName { get; set; } // Tên cửa hàng
        public string CreatedDateString { get; set; }
    }
}
