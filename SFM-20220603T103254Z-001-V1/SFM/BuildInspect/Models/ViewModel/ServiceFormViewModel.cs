using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class ServiceFormViewModel
    {
        public int ServiceID { get; set; }
        public string ServiceRefNum { get; set; }
        public string CaseRefNum { get; set; }
        public string Service_Datetime { get; set; }
        public string Priority { get; set; }
        public Nullable<int> SchoolID { get; set; }
        public string Contact_Person { get; set; }
        public string Location { get; set; }
        public string RoomNo { get; set; }
        public string ServiceType { get; set; }
        public string Work_Requests { get; set; }
        public string Customer_Rep { get; set; }
        public string Customer_Rep_Signature { get; set; }
        public string Work_Completion_Details { get; set; }
        public string Remarks_TC { get; set; }
        public string Client_Comments { get; set; }
        public string Technician_Name { get; set; }
        public string Technician_Signature { get; set; }
        public string Customer_incharge_Name { get; set; }
        public string Customer_incharge_Signature { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> Status_Flag { get; set; }
        public Nullable<int> AssignedBy { get; set; }
        public string AssignedDate { get; set; }
        public string Technician_Signature_Date { get; set; }

        public string Zone { get; set; }
        public string SchoolName { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string Assigned_Name { get; set; }
        public List<HttpPostedFileBase> files { get; set; }

        public List<ServiceFormFileViewModel> service_form_files { get; set; }

    }


    public class ServiceFormMobileViewModel
    {
        public int ServiceID { get; set; }
        public string ServiceRefNum { get; set; }
        public string CaseRefNum { get; set; }
        public string Service_Datetime { get; set; }
        public string Priority { get; set; }
        public Nullable<int> SchoolID { get; set; }
        public string Contact_Person { get; set; }
        public string Location { get; set; }
        public string RoomNo { get; set; }
        public string ServiceType { get; set; }
        public string Work_Requests { get; set; }
        public string Customer_Rep { get; set; }
        public string Customer_Rep_Signature { get; set; }
        public string Work_Completion_Details { get; set; }
        public string Remarks_TC { get; set; }
        public string Client_Comments { get; set; }
        public string Technician_Name { get; set; }
        public string Technician_Signature { get; set; }
        public string Customer_incharge_Name { get; set; }
        public string Customer_incharge_Signature { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> Status_Flag { get; set; }
        public Nullable<int> AssignedBy { get; set; }
        public string AssignedDate { get; set; }
        public string Technician_Signature_Date { get; set; }

        public string Zone { get; set; }
        public string SchoolName { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string Assigned_Name { get; set; }
        
        public List<FileUploadViewModel> mobileFiles { get; set; }
    }

    public class FileUploadViewModel
    {
        public string filename { get; set; }
        public string data { get; set; }
    }

}