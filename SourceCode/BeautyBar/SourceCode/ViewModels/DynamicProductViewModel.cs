using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModels;
using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class DynamicProductViewModel : ProductModel
    {
        public string ProductStoreCodeMark { get; set; }

        [Display( Name = "Tồn hiện tại")]
        public Nullable<decimal> EndInventoryQty { get; set; }
    }
}
