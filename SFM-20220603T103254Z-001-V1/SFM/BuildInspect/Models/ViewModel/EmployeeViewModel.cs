using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class EmployeeViewModel
    {

        public int EmpID { get; set; }
        public string Employee_Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DoB { get; set; }
        public string PhotoPath { get; set; }
        public Nullable<int> DesignationID { get; set; }
        public Nullable<int> ScheduleID { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string DoJ { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string DoR { get; set; }
        public Nullable<int> IsActive { get; set; }
        public HttpPostedFileBase photo_path { get; set; }
        public string FullName { get; set; }

    }
}