using EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class WarehouseInventoryDetailViewModel : WarehouseInventoryDetailModel
    {
        public int STT { get; set; }
        public string WarehouseName { get; set; }

        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public string MasterCode { get; set; }

        public decimal TonTrongDatabase { get; set; }

        //public string Specifications { get; set; }


        [DisplayFormat(DataFormatString = "{0:n2}")]
        public Nullable<decimal> EndInventoryQty { get; set; }


    }
}
