using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class ChecklistSummaryMatrixViewModel
    {        
        public string School_Name { get; set; }
        public Nullable<int> Electrical { get; set; }
        public Nullable<int> Mechanical { get; set; }

        public Nullable<int> SP_LIFTSYS { get; set; }
        public Nullable<int> SP_SECUTYSYS { get; set; }
        public Nullable<int> SP_FIREALMSYS { get; set; }
        public Nullable<int> SP_PLCWDWL { get; set; }
        public Nullable<int> SP_HYDPUMP { get; set; }
        public Nullable<int> SP_PASISCM { get; set; }
        public Nullable<int> SP_BASBMSSYS { get; set; }
        public Nullable<int> SP_CHILLERSYS { get; set; }
        public Nullable<int> SP_FUMECESYS { get; set; }
        public Nullable<int> SP_FIRESPNKLR { get; set; }
        public Nullable<int> SP_WETCHSUSYS { get; set; }
        public Nullable<int> SP_HVLSFMPIS { get; set; }
        public Nullable<int> SP_SMKCTLSYS { get; set; }        
        public Nullable<int> SP_OTHLIFTS { get; set; }
        public Nullable<int> SP_GENSET { get; set; }
        public Nullable<int> SP_SIOCDSYS { get; set; }

    }
}