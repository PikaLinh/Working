using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.ChicCut
{
    public class DayOfWeekViewModel
    {
        //Kiểu int của DayOfWeek
        public int? DayOfWeek { get; set; }
        //Kiểu string của DayOfWeek
        public string DayOfWeekString{ get; set; }
        public int PlanMasterId { get; set; }
    }
}
