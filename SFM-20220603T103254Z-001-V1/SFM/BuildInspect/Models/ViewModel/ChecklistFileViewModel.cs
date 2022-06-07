using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class ChecklistFileViewModel
    {

        public int ReportID { get; set; }
        public Nullable<int> SchoolID { get; set; }
        public Nullable<System.DateTime> Month_Start_Date { get; set; }
        public string Frequency { get; set; }
        public string CLType { get; set; }
        public string FilePath { get; set; }

        public string School_Name { get; set; }
        public string Zone { get; set; }

    }
}