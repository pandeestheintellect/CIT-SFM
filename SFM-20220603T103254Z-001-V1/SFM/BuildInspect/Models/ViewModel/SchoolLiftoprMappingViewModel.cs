using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class SchoolLiftoprMappingViewModel
    {

        public int LiftOprID { get; set; }
        public Nullable<int> SchoolID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> LIFT_count { get; set; }

    }

   

}