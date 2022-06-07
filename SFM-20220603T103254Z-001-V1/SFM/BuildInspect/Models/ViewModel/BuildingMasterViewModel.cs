using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class BuildingMasterViewModel
    {

        public int BuildingID { get; set; }
        public string Zone { get; set; }
        public string School_Type { get; set; }
        [Required]
        public string Building_Name { get; set; }
        public string Address { get; set; }
        public string Prefix { get; set; }
        [Required]
        public string Op_Manager_Name { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> ME_Coordinator_ID { get; set; }
        public string School_Ref_Num { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }
        public Nullable<int> Is_Project_Created { get; set; }
        public Nullable<int> IsActive { get; set; }
        public Nullable<int> CompanyID { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public Nullable<int> HVLSFan_count { get; set; }
        public Nullable<int> LIFT_count { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public Nullable<int> CHILLER_count { get; set; }
    }
}