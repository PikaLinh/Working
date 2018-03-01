using EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class CalendarViewModel : CalendarModel
    {
        public string CourseUrl { get; set; }
        public string CourseName { get; set; }
        public string LocationUrl { get; set; }
        public string LocationName { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,#}đ")]
        public decimal Discount { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,#}đ")]
        public decimal NewPrice { get; set; }
        public int TotalOfDiscount { get; set; }

        public string SEOCategory { get; set; }

    }
}
