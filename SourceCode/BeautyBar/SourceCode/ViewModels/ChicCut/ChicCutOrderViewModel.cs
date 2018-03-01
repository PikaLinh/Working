using EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ChicCutOrderViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderId")]
        public int OrderId { get; set; }
        public Nullable<int> PaymentMethodId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FullName")]
        public string FullName { get; set; }
        public int IdCustomer { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Note")]
        public string Note { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderCode")]
        public string OrderCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Phone")]
        public string Phone { get; set; }
        public Nullable<decimal> BillDiscount { get; set; }
        public int BillDiscountTypeId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SumCOGSOfOrderDetail")]
        [DisplayFormat(DataFormatString="{0:n0}")]
        public Nullable<decimal> SumCOGSOfOrderDetail { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "SumPriceOfOrderDetail")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public Nullable<decimal> SumPriceOfOrderDetail { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ActualAmount")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public Nullable<decimal> Total { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TotalBillDiscount")]
        [DisplayFormat(DataFormatString = "{0:n0}")]
        public Nullable<decimal> TotalBillDiscount { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CreatedTime")]
        [DisplayFormat(DataFormatString = "{0:hh:mm dd/MM}")]
        public DateTime? CreatedDate { get; set; }

        public List<ChicCutOrderDetailViewModel> details { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Cashier")]
        public int? CashierUserId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CashierTime")]
        [DisplayFormat(DataFormatString = "{0:hh:mm dd/MM}")]
        public DateTime? CashierDate { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Staff")]
        public string StaffName { get; set; }

        #region additional field
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Cashier")]
        public string CashierName { get; set; }
        public bool? Gender { get; set; }
        public string GenderName { get { return Gender == true ? "Nam" : "Nữ"; } }
        public string HairStyle { get; set; }
        public string PaymentMethod { get; set; }
        #endregion

        #region additonal 2 field for Reservation
        public Nullable<decimal> MinSumPriceOfOrderDetail { get; set; }
        public Nullable<decimal> MaxSumPriceOfOrderDetail { get; set; }
        public int PreOrderId { get; set; }
        #endregion
    }
}
