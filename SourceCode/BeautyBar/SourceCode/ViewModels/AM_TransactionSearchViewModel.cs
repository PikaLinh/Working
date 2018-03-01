using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class AM_TransactionSearchViewModel
    {
        public int? StoreId { get; set; }
        public string TransactionTypeCode { get; set; }
        public string ContactItemTypeCode { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public decimal? FromTotalPrice { get; set; }
        public decimal? ToTotalPrice { get; set; }
        public bool? Isimport { get; set; }
    }
}
