using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class LicenseViewModel
    {

        public int LicenseID { get; set; }
        [Required]
        public int CompanyID { get; set; }

        
        public string License_Start_Date { get; set; }
                
        public string License_End_Date { get; set; }

        public int IsActive { get; set; }
        public string LicenseKey { get; set; }

        [Required]
        public int No_of_Days { get; set; }

        public Nullable<System.DateTime> Creation_Date { get; set; }

        public string CompanyName { get; set; }

        public int StatusFlag { get; set; }

        public string days_to_expire { get; set; }

        public string status { get; set; }



    }

    
}