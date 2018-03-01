using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ChicCutOrderDetailViewModel
    {
        public Nullable<decimal> COGS { get; set; }
        public Nullable<decimal> OrderId { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> STT { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public Nullable<decimal> UnitCOGS { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }

        #region additonal 4 field for Reservation
        public Nullable<decimal> MinPrice { get; set; }
        public Nullable<decimal> MaxPrice { get; set; }
        public Nullable<decimal> MinUnitPrice { get; set; }
        public Nullable<decimal> MaxUnitPrice { get; set; }
        #endregion

        //Bán sản phẩm
        public int ProductId { get; set; }
    }
}
