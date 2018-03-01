using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class ReportCategory1ViewModel
    {
        public int CategoryId { get; set; }

        [Display(Name = "Danh mục")]
        public string CategoryName { get; set; }

        [Display(Name = "Số lượng tồn kho")]
        public decimal EndInventoryQty { get; set; }

        [Display(Name = "Giá trị tồn kho")]
        public decimal TotalCogs { get; set; }
    }
}
