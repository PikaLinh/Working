using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class SellSearchViewModel
    {
        public int? OrderId { get; set; }
        public int? CustomerId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public decimal? FromTotalPrice { get; set; }
        public decimal? ToTotalPrice { get; set; }
        public int? ProductId { get; set; }
    }
}
