using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntityModels;

namespace ViewModels
{
    public class EmailParameterDetailViewModel : CRM_EmailParameterModel
    {
        public int STT { get; set; }
        public int RemiderId { get; set; }
    }
}