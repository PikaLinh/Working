//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntityModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class WarehouseInventoryMasterModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WarehouseInventoryMasterModel()
        {
            this.WarehouseInventoryDetailModel = new HashSet<WarehouseInventoryDetailModel>();
        }
    
        public int WarehouseInventoryMasterId { get; set; }
        public string WarehouseInventoryMasterCode { get; set; }
        public Nullable<int> WarehouseId { get; set; }
        public Nullable<decimal> TotalQty { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedAccount { get; set; }
        public Nullable<int> CreatedEmployeeId { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
        public string DeletedAccount { get; set; }
        public Nullable<int> DeletedEmployeeId { get; set; }
        public string Note { get; set; }
        public bool CreatedIEOther { get; set; }
        public bool Actived { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WarehouseInventoryDetailModel> WarehouseInventoryDetailModel { get; set; }
    }
}