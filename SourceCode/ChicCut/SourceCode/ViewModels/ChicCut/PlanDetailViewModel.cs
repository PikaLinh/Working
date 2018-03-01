using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.ChicCut
{
    public class PlanDetailViewModel
    {
        public int PlanDetailId { get; set; }
        public int PlanMasterId { get; set; }
        public string TimeFrameString { get; set; }
        public Nullable<TimeSpan> TimeFrame { get; set; }
        public Nullable<int> AmountOfCus { get; set; }
    }
}
