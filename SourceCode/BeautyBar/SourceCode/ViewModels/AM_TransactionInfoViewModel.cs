using EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ViewModels
{
    public class AM_TransactionInfoViewModel : AM_TransactionModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TransactionTypeName")]
        public string TransactionTypeName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ContactItemTypeName")]
        public string ContactItemTypeName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StoreId")]
        public string StoreName { get; set; }

        public bool IsImport { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedAccount")]
        public string EmployeeName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CustomerId")]
        public string CustomerName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SupplierId")]
        public string SupplierName { get; set; }
    }
}
