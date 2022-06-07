using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class SchoolUserMappingViewModel
    {

        public int SUID { get; set; }
        public int SchoolID { get; set; }
        public Nullable<int> UserID { get; set; }
        public string CLType { get; set; }


    }
}