using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class DailyServicesReportViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<DailyServicesViewModel> DailyServices { get; set; }
        public decimal Total { get; set; }
    }
}
