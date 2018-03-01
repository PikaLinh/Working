using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class PriceListViewModel
    {
        public int STT { get; set; }
        public int count { get; set; }
        public Nullable<decimal> Price { get; set; }
        public int CustomerLevelId { get; set; }
        public string CustomerLevelName { get; set; }
    }
}
