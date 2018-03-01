using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class PreImportMasterSearchViewModel
    {
        public int? WarehouseId { get; set; }
        public int? SupplierId { get; set; }
        public int? ProductId { get; set; }
        public int? PreImportMasterId { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public decimal? FromTotalPrice { get; set; }
        public decimal? ToTotalPrice { get; set; }

    }
}
