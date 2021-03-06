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
    
    public partial class CategoryModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CategoryModel()
        {
            this.CourseModel = new HashSet<CourseModel>();
            this.Website_NewsModel = new HashSet<Website_NewsModel>();
        }
    
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryNameEn { get; set; }
        public Nullable<int> OrderBy { get; set; }
        public Nullable<int> Parent { get; set; }
        public string Keywords { get; set; }
        public string KeywordsEn { get; set; }
        public string Description { get; set; }
        public string DescriptionEn { get; set; }
        public string ImageUrl { get; set; }
        public string SEOCategoryName { get; set; }
        public bool isHasChildren { get; set; }
        public bool Actived { get; set; }
        public string ADNCode { get; set; }
        public Nullable<bool> isDisplayOnHomePage { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseModel> CourseModel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Website_NewsModel> Website_NewsModel { get; set; }
    }
}
