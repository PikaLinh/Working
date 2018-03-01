using EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class OrderMasterViewModel : OrderMasterModel
    {
        public string StoreName { get; set; }
        public string WarehouseName { get; set; }
        public string CustomerLevel { get; set; }
    }
}
