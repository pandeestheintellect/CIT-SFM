using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class FilterViewModel
    {

       
        public string Month { get; set; }
        public string Year { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
      
        public string CLType { get; set; }
        public string Frequency { get; set; }
        public int CLStatus { get; set; }

      

        public int userid { get; set; }
        public string Zone { get; set; }

        public int SchoolID { get; set; }

        public List<int> schoolids { get; set; }
        public List<int> pdfclids { get; set; }

        public string Schoollist { get; set; }

        public string serviceType { get; set; }

        
        public List<ChecklistSummaryMatrixViewModel> cls { get; set; }
        public List<ChecklistFileViewModel> clfdownload { get; set; }
        public List<ProjectMasterViewModel> pmvm { get; set; }
    }
}