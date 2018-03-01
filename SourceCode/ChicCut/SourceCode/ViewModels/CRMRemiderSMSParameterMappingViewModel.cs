using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntityModels;

namespace ViewModels
{
    public class CRMRemiderSMSParameterMappingViewModel : CRM_Remider_SMSParameter_Mapping
    {
        public string SMSParameterName { get; set; }
        public string SMSParameterDescription { get; set; }
    }
}