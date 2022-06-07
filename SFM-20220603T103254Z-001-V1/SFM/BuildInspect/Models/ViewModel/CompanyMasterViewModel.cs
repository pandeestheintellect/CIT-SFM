using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class CompanyMasterViewModel
    {

        public int CompanyID { get; set; }
        [Required]
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Pincode { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string Mob { get; set; }
        public string Email { get; set; }
        public string RegNo { get; set; }
        public string GstRegNo { get; set; }
        public string LogoPath { get; set; }
        public Nullable<decimal> GST { get; set; }
        [Display(Name = "Company Logo")]
        public HttpPostedFileBase profile_Path { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> IsActive { get; set; }
        [Required]
        public string ShortName { get; set; }
    }
}