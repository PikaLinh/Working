using EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace ViewModels
{
    public class ProductViewModel : ProductModel
    {
        [Display(Name="Giá VIP")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public decimal? Price1 { get; set; }

        [Display(Name = "Giá VIP-Bạc")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public decimal? Price2 { get; set; }

        [Display(Name = "Giá VIP-Vàng")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public decimal? Price3 { get; set; }

        [Display(Name = "Giá VIP-Bạch Kim")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public decimal? Price4 { get; set; }

        public string ProductStoreCodeMark { get; set; }

        [Display(Name = "SL tồn hiện tại")]
        public string EninventoryQtyNow { get; set; }


        [Display(Name = "Số lượng cảnh báo")]
        public string QtyAlert { get; set; }

        [Display(Name = "Tồn hiện tại")]
        public Nullable<decimal> EndInventoryQty { get; set; }

    }
}
