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
    
    public partial class Website_LocationModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Website_LocationModel()
        {
            this.Website_AdsModel = new HashSet<Website_AdsModel>();
        }
    
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public string Description { get; set; }
        public Nullable<int> LocationWidth { get; set; }
        public Nullable<int> LocationHeight { get; set; }
        public Nullable<bool> Actived { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Website_AdsModel> Website_AdsModel { get; set; }
    }
}