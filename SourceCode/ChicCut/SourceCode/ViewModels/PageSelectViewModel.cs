using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class PageSelectViewModel
    {
        public int PageId { get; set; }
        public int MenuId { get; set; }
        public string PageName { get; set; }
        public bool isSelected { get; set; }
    }


    public class RolesSelectViewModel
    {
        public int RolesId { get; set; }
        public string RolesName { get; set; }
        public bool isSelected { get; set; }
    }
}
