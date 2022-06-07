using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class SpecialistCompanyMappingViewModel
    {

        public int SPCMPID { get; set; }
        public Nullable<int> SCSPCLID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> Service_Count { get; set; }


    }   

}