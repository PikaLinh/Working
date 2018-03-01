using EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class WarehouseInventoryMasterViewModel : WarehouseInventoryMasterModel
    {
        public string WarehouseName { get; set; }
    }
}
