using EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ServiceCategoryViewModel : Master_ChicCut_ServiceCategoryModel
    {
        [Display(Name = "Danh mục")]
        public string ServiceParentCategoryName { get; set; }
    }
}
