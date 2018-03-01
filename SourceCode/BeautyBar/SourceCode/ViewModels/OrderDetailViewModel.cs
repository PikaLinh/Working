using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntityModels;

namespace ViewModels
{
    public class OrderDetailViewModel : OrderDetailModel
    {
        public int STT { get; set; }
        public string ProductName { get; set; }
        public int EndInventoryQty { get; set; }
       // public string ProductCode { get; set; }

    }
}