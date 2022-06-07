using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class PermissionViewModel
    {

        public int PermissionID { get; set; }
        public Nullable<int> GroupID { get; set; }
        public Nullable<int> ModuleID { get; set; }
        public Nullable<int> Access { get; set; }

        public  ModuleViewModel rapid_module { get; set; }
        public GroupViewModel rapid_usergroup { get; set; }
    }
}