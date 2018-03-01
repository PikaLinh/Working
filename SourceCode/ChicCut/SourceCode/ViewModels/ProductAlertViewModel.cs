using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntityModels;

namespace ViewModels
{
    public class ProductAlertViewModel : ProductAlertModel
    {
        public int STT { get; set; }
        public string WarehouseName { get; set; }
        public string ProductName { get; set; }
        public decimal EndinventoryQty { get; set; }
    }
}