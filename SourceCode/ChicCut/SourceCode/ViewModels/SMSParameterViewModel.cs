using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntityModels;

namespace ViewModels
{
    public class SMSParameterViewModel : CRM_SMSParameterModel
    {
        public int STT { get; set; } 
        public int RemiderId { get; set; }
    }
}