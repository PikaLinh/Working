using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class ReportSearchViewModel
    {
        public int StoreId { get; set; }
        public int? StaffId { get; set; }
        public int? CashierUserId { get; set; }
        public int? PaymentMethodId { get; set; }
        public int FromQuater { get; set; }
        public int FromYearQuater { get; set; }
        public int ToQuater { get; set; }
        public int ToYearQuater { get; set; }

        public int FromMonth { get; set; }
        public int FromYearMonth { get; set; }
        public int ToMonth { get; set; }
        public int ToYearMonth { get; set; }

        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }

        public int CustomerId { get; set; }
        public int SupplierId { get; set; }
        public Nullable<System.DateTime> FromDateSup { get; set; }
        public Nullable<System.DateTime> ToDateSup { get; set; }

    }
}
