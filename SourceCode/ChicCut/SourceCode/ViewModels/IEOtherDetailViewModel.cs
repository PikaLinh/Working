using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntityModels;

namespace ViewModels
{
    public class IEOtherDetailViewModel : IEOtherDetailModel
    {
        public int STT { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal Qty { get; set; }

    }
}