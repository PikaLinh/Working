using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class MenuViewModel
    {
        [Key]
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MenuModel_MenuId")]
        public int MenuId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MenuModel_MenuName")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string MenuName { get; set; }

        public string MenuNameVi { get; set; }


        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderBy")]
        public Nullable<int> OrderBy { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Icon")]
        public string Icon { get; set; }

        public List<PageSelectViewModel> Pages { get; set; }
    }
}
