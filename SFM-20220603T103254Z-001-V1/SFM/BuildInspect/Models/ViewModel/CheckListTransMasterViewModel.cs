using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class CheckListTransMasterViewModel
    {

        public int CLTMID { get; set; }
        public Nullable<int> SchoolID { get; set; }
        public Nullable<int> UserID { get; set; }
        public string CheckedBy_Signature { get; set; }
        public string VerifiedBy_Signature { get; set; }
        public string EndoresBy_Signature { get; set; }
        public string Frequency { get; set; }
        public Nullable<int> PrjMasID { get; set; }
        public Nullable<int> Draft_Flag { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<System.DateTime> User_Sign_Date { get; set; }
        public Nullable<System.DateTime> OpMngr_Sign_Date { get; set; }
        public Nullable<System.DateTime> MA_Sign_Date { get; set; }
        public string CheckedBy_Remarks { get; set; }
        public string VerifiedBy_Remarks { get; set; }
        public string EndoresBy_Remarks { get; set; }
        public Nullable<int> Status_Flag { get; set; }
        public string CLType { get; set; }

        public string CheckedBy_DateTime { get; set; }
        public string VerifiedBy_DateTime { get; set; }
        public string EndoresBy_DateTime { get; set; }
        public Nullable<decimal>  CB_Latitude { get; set; }
        public Nullable<decimal> CB_Longitude { get; set; }

        public string CheckedBy_Name { get; set; }
        public string VerifiedBy_Name { get; set; }
        public string EndoresBy_Name { get; set; }

        public Nullable<System.DateTime> CLMonth { get; set; }

        public string School_Name { get; set; }
        public string Zone { get; set; }
        public List<HttpPostedFileBase> files { get; set; }

        public List<CheckListTransDetailViewModel> clTrnDetail { get; set; }


        public List<CheckListTransDetailViewModel> chklst_trn_detail { get; set; }
        public List<CheckListTransFileViewModel> chklst_trn_files { get; set; }

        public List<FileUploadViewModel> mobileFiles { get; set; }
    }

    public class CheckListTransMasterMobileViewModel
    {

        public int CLTMID { get; set; }
        public Nullable<int> SchoolID { get; set; }
        public Nullable<int> UserID { get; set; }
        public string CheckedBy_Signature { get; set; }
        public string VerifiedBy_Signature { get; set; }
        public string EndoresBy_Signature { get; set; }
        public string Frequency { get; set; }
        public Nullable<int> PrjMasID { get; set; }
        public Nullable<int> Draft_Flag { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<System.DateTime> User_Sign_Date { get; set; }
        public Nullable<System.DateTime> OpMngr_Sign_Date { get; set; }
        public Nullable<System.DateTime> MA_Sign_Date { get; set; }
        public string CheckedBy_Remarks { get; set; }
        public string VerifiedBy_Remarks { get; set; }
        public string EndoresBy_Remarks { get; set; }
        public Nullable<int> Status_Flag { get; set; }
        public string CLType { get; set; }

        public string CheckedBy_DateTime { get; set; }
        public string VerifiedBy_DateTime { get; set; }
        public string EndoresBy_DateTime { get; set; }
        public Nullable<decimal> CB_Latitude { get; set; }
        public Nullable<decimal> CB_Longitude { get; set; }

        public string CheckedBy_Name { get; set; }
        public string VerifiedBy_Name { get; set; }
        public string EndoresBy_Name { get; set; }

        public string School_Name { get; set; }
        public string Zone { get; set; }
        public string Title { get; set; }

        public string SchoolName { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string OPMngr { get; set; }
        public string Email { get; set; }
        public int LIFT_count { get; set; }
        public int HVLSFan_count { get; set; }
        public int CHILLER_count { get; set; }
        public List<CheckListTransDetailViewModel> chklst_trn_detail { get; set; }
        public List<CheckListItemMasterViewModel> CheckListItems { get; set; }

    }

}