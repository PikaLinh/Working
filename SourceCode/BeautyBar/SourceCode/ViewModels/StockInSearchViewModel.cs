using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class StockInSearchViewModel
    {
        public int StoreId { get; set; }

        public Nullable<System.DateTime> FromDate { get; set; }

        public Nullable<System.DateTime> ToDate { get; set; }
    }
}
