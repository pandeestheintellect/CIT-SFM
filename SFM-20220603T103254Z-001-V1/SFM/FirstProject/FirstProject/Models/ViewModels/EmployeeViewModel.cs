using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstProject.Models.ViewModels
{
    public class EmployeeViewModel
    {
        public Nullable<int> empid { get; set; }
        public string EmpName { get; set; }
        public Nullable<int> phoneno { get; set; }
        public string email { get; set; }


    }
}