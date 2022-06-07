using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class SchoolSpecialistMappingViewModel
    {

        public int SCSPCLID { get; set; }
        public Nullable<int> SchoolID { get; set; }
        public string Specialist_Type { get; set; }        
        
        public List<SpecialistCompanyMappingViewModel>  specialist_company_mapping { get; set; }

    }

   

}