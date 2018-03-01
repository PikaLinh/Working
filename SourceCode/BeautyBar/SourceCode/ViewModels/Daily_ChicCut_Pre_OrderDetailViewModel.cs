using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntityModels;

namespace ViewModels
{
    public class Daily_ChicCut_Pre_OrderDetailViewModel : Daily_ChicCut_Pre_OrderDetailModel
    {
        public int STT { get; set; }
        public string ServiceName { get; set; }
        //public int EndInventoryQty { get; set; }
       // public string ProductCode { get; set; }

    }
}