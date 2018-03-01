using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ChicCutPreOrderSeachViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedDate")]
        public DateTime? SearchCreatedDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "AppointmentTime")]
        public DateTime? SearchAppointmentTime { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FullName")]

        public string SearchFullName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Phone")]
        public string SearchPhone { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PreOrderCode")]
        public string SearchPreOrderCode { get; set; }

    }
}
