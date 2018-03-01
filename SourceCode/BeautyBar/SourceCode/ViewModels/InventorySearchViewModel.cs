using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class InventorySearchViewModel
    {
        public int? CategoryId { get; set; }
        public int? ProductId { get; set; }
        public int? InventoryMasterId { get; set; }
        public int? EmployeeId { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public int? WarehouseId { get; set; }
    }
}
