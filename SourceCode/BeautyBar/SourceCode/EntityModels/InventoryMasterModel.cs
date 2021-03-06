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
    
    public partial class InventoryMasterModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InventoryMasterModel()
        {
            this.InventoryDetailModel = new HashSet<InventoryDetailModel>();
        }
    
        public int InventoryMasterId { get; set; }
        public string InventoryCode { get; set; }
        public Nullable<int> WarehouseModelId { get; set; }
        public int InventoryTypeId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedAccount { get; set; }
        public Nullable<int> CreatedEmployeeId { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public string LastModifiedAccount { get; set; }
        public Nullable<int> LastModifiedEmployeeId { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
        public string DeletedAccount { get; set; }
        public Nullable<int> DeletedEmployeeId { get; set; }
        public bool Actived { get; set; }
        public Nullable<int> BusinessId { get; set; }
        public string BusinessName { get; set; }
        public string ActionUrl { get; set; }
        public Nullable<int> StoreId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InventoryDetailModel> InventoryDetailModel { get; set; }
        public virtual InventoryTypeModel InventoryTypeModel { get; set; }
    }
}
