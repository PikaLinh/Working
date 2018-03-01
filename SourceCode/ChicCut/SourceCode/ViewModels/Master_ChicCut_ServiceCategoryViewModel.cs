using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityModels;

namespace ViewModels
{
    public class Master_ChicCut_ServiceCategoryViewModel : Master_ChicCut_ServiceCategoryModel
    {
        public List<Master_ChicCut_ServiceModel> Services { get; set; }
    }
}