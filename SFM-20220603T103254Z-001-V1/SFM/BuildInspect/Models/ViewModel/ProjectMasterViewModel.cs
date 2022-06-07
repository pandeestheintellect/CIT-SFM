using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class ProjectMasterViewModel
    {

        public int PrjMasID { get; set; }
        public Nullable<int> BuildingID { get; set; }
        public string MonthName { get; set; }
        public Nullable<System.DateTime> Month_Start_Date { get; set; }
        public Nullable<System.DateTime> Month_End_Date { get; set; }
        public string Frequency { get; set; }
        public Nullable<int> Lift_Count { get; set; }
        public Nullable<int> Is_Completed { get; set; }
        public Nullable<int> Is_Rescheduled { get; set; }
        public Nullable<System.DateTime> Rescheduled_Date { get; set; }
        public string Rescheduled_Remarks { get; set; }
        public int CLTMID { get; set; }
        public bool crntUserChk { get; set; }
        public string status { get; set; }
        public string CLType { get; set; }
        public string Lift_Opr_Name { get; set; }
        public string Building_Name { get; set; }
        public Nullable<int> CLStatus { get; set; }
        public string month_name { get; set; }
        public Nullable<int> CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int Id { get; set; }
        public Nullable<int> Fan_Count { get; set; }
        public string year { get; set; }
        public string ServiceType { get; set; }
        public Nullable<int> IsActive { get; set; }





    }
}