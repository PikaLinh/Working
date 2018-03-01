using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class DemoMasterModel
    {
        public int DemoId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class DemoMasterViewModel : DemoMasterModel
    {
         public bool isRI { get; set; }
    }

    public class DemoDetailModel
    {
        public int DemoDetailId { get; set; }
        public int DemoId { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }

    }

}
