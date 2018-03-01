using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntityModels;

namespace ViewModels
{
    public class CRMRemiderEmailParameterMappingViewModel : CRM_Remider_EmailParameter_Mapping
    {
        public string EmailParameterName { get; set; }
        public string EmailParameterDescription { get; set; }
    }
}