using EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class InventoryMasterSearchViewModel
    {
        public int? WarehouseId { get; set; }
        public int? ProductId { get; set; }
        public int? InventoryMasterId { get; set; }
        public int? InventoryTypeId { get; set; }
		public Nullable<System.DateTime> FromDate { get; set; }
		public Nullable<System.DateTime> ToDate { get; set; }
	}
}
