using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class CheckListTransDetailViewModel
    {

        public int CLTDID { get; set; }
        public Nullable<int> CLTMID { get; set; }
        public Nullable<int> ChkListID { get; set; }
        public string IsDone { get; set; }
        public string DateDone { get; set; }
        public string Remarks { get; set; }

    }
}