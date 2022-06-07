using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class CheckListItemMasterViewModel
    {

        public int ChkListID { get; set; }
        public string FormName { get; set; }
        public string Title { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string IsSubTitle { get; set; }
        public Nullable<int> OrderBy { get; set; }
        public Nullable<int> PageNum { get; set; }
        public string ServiceType { get; set; }
        public string Frequency { get; set; }
        public Nullable<int> CF_Flag { get; set; }

        public string IsDone { get; set; }
        public string DateDone { get; set; }
        public string Remarks { get; set; }
        public int CLTDID { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public Nullable<int> IsActive { get; set; }

    }


    public class CheckListItemMasterMobileViewModel
    {

        public int ChkListID { get; set; }
        public string FormName { get; set; }
        public string Title { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string IsSubTitle { get; set; }
        public Nullable<int> OrderBy { get; set; }
        public Nullable<int> PageNum { get; set; }
        public string ServiceType { get; set; }
        public string Frequency { get; set; }
        public Nullable<int> CF_Flag { get; set; }

        public string IsDone { get; set; }
        public string DateDone { get; set; }
        public string Remarks { get; set; }
        public int CLTDID { get; set; }

        
    }
}