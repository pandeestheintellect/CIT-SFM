//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BuildInspect.Models.Domain
{
    using System;
    using System.Collections.Generic;
    
    public partial class chklst_item_master
    {
        public int ChkListID { get; set; }
        public string FormName { get; set; }
        public string Title { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string IsSubTitle { get; set; }
        public Nullable<int> OrderBy { get; set; }
        public Nullable<int> PageNum { get; set; }
        public Nullable<int> CF_Flag { get; set; }
        public string ServiceType { get; set; }
        public string Frequency { get; set; }
        public Nullable<int> IsActive { get; set; }
        public Nullable<int> CompanyID { get; set; }
    }
}
