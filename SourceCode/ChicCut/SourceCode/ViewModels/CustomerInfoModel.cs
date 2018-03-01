using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModels;

namespace ViewModels
{
    public class CustomerInfoModel : CustomerModel
    {
        public string EmployeeName { get; set; }
        public string CustomerLevelName { get; set; }
    }
}
