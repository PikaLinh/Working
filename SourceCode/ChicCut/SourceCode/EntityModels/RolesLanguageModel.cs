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
    
    public partial class RolesLanguageModel
    {
        public int RolesId { get; set; }
        public int LanguageId { get; set; }
        public string RolesName { get; set; }
    
        public virtual LanguageModel LanguageModel { get; set; }
        public virtual RolesModel RolesModel { get; set; }
    }
}