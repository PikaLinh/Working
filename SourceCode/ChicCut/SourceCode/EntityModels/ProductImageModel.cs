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
    
    public partial class ProductImageModel
    {
        public int ProductImageId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public string ImageUrl { get; set; }
    
        public virtual ProductModel ProductModel { get; set; }
    }
}