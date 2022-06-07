using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class CheckListItemTitleMasterViewModel
    {
        public int chklstitemid { get; set; }
        public int ChklstTypeId { get; set; }
        public string FormName { get; set; }
        public string TitleDescription { get; set; }
        public string CheckListType { get; set; }
        public string Frequency { get; set; }
      
        public Nullable<int> CompanyID { get; set; }
        public Nullable<int> IsActive { get; set; }

    }


}