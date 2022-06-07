using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class CheckListIndexViewModel
    {

        public int CLTMID { get; set; }
        public int PrjMasID { get; set; }
        public int SchoolID { get; set; }
        public string Frequency { get; set; }
        public string CLType { get; set; }
        public string Status { get; set; }
        public string SchoolName { get; set; }
        public string Zone { get; set; }
        public string Month_Name { get; set; }
    }

    public class CheckListIndexMobileViewModel
    {

        public int CLTMID { get; set; }
        public int PrjMasID { get; set; }
        public int SchoolID { get; set; }
        public string Frequency { get; set; }
        public string CLType { get; set; }
        public string Status { get; set; }
        public string SchoolName { get; set; }
        public string Zone { get; set; }
        public string Month_Name { get; set; }
    }
}