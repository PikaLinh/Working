using EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class Master_ChicCut_HairTypeSeachModel : Master_ChicCut_HairTypeModel
    {
        public Nullable<bool> ActivedHairType { get; set; }
        public bool Checked { get; set; }
    }
}
