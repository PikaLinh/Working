using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntityModels;

namespace ViewModels
{
    public class Master_ChicCut_QuantificationDetailViewModel : Master_ChicCut_QuantificationDetailModel
    {
        public int STT { get; set; }
        public string ProductName { get; set; }
        public decimal? Price { get; set; }
    }
}