using EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class SalaryDetailViewModel : SalaryDetailModel
    {
        [Display(Name = "Nhân viên")]
        public string EmployeeName { get; set; }
        [Display(Name = "Ghi chú")]
        public string Note { get; set; }
    }
}
