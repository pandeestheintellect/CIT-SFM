using BuildInspect.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.Service.Imp
{
    public interface IERPServices
    {

        List<BuildingMasterViewModel> GetAllSchools();
        bool CheckSchool(string name, string zone);
        int CreateSchool(BuildingMasterViewModel school);
        int SaveSchool(BuildingMasterViewModel school);
        int DeleteSchool(int sID);


        List<CheckListItemMasterViewModel> GetAllCheckListItems();
        ProjectMasterViewModel GetFrequency(DateTime startDate, DateTime endDate);
        BuildingMasterViewModel GetSchool(int id);
        List<CheckListTransMasterViewModel> GetAllSubmittedCheckList();
        int SubmitChecklist(CheckListTransMasterViewModel cltm, List<CheckListTransDetailViewModel> Detail, string path, string halfPath);
        CheckListTransMasterViewModel GetCheckListItems(int id);
        List<ProjectMasterViewModel> GetAllProjects();
        List<ServiceFormViewModel> GetAllServiceForms();
        int EditChecklist(CheckListTransMasterViewModel cltm, List<CheckListTransDetailViewModel> Detail);
        int SubmitSignatureCheckList(CheckListTransMasterViewModel cltm);
        int AssignService(ServiceFormViewModel sfm);
        ServiceFormViewModel GetServiceForm(int id);
        int ServiceFormSave(ServiceFormViewModel sfm, string path, string halfPath);
        CheckListTransMasterMobileViewModel GetCheckListItemsforMobile(int id);
        int ServiceFormMobileSave(ServiceFormMobileViewModel sfm, string path, string halfPath);
        List<CheckListTransMasterViewModel> GetAllSubmittedCheckList_School(List<int> ids);
        List<CheckListTransMasterViewModel> GetAllSubmittedCheckList_User(List<int> ids);
        List<CheckListTransMasterViewModel> GetAllSubmittedCheckList_Project(int id);
        int SubmitSignatureCheckListForMobile(CheckListTransMasterViewModel cltm);
        int SubmitMASignatureCheckListForMobile(CheckListTransMasterViewModel cltm);
        int SubmitChecklistForMobile(CheckListTransMasterViewModel cltm, List<CheckListTransDetailViewModel> Detail, string path, string halfPath);
        List<int?> GetApplicableSchoolIDs(List<int> ids, string splType, int userid);
        int GetLiftCount(int schoolid, int userid, DateTime monstdate);
        List<ChecklistSummaryMatrixViewModel> GetCurCLSummaryMatrix(int userid);
        bool CheckCLAlreadySubmitted(int PrjMasID, int SchoolID);
        ProjectMasterViewModel GetProject(int id);
        //List<int?> GetLiftSchoolIDs(int userid);
        List<ProjectMasterViewModel> GetAllProject_SchoolID(List<int> ids, string splType);
        List<DashboardSummaryViewModel> GetDashboardSummary(int userid, int groupid, int companyid, DateTime startdt, DateTime enddt);
        List<ProjectMasterViewModel> GetLiftProjects(List<int> ids, string oprname);
        List<ProjectMasterViewModel> GetAllProjectsOtherthanLift(List<int> ids, string cltype);
        List<CheckListTransMasterViewModel> GetAllSubmittedCheckList_CLType(List<int> ids, string cltype);
        int SubmitChecklistForMobileOffline(CheckListTransMasterViewModel cltm, List<CheckListTransDetailViewModel> Detail, string path, string halfPath);
        int ServiceFormMobileOffline(ServiceFormMobileViewModel sfm, string path, string halfPath);
        List<ChecklistSummaryMatrixViewModel> GetFilterCLSummaryMatrix(FilterViewModel fvm);
        int ServiceFormMobileOfflineSave(ServiceFormMobileViewModel sfm, string path, string halfPath);
        ProjectMasterViewModel GetProjectFromDate(DateTime firstdate);
        List<ProjectMasterViewModel> GetProjectsForApplicableCLs(int schoolid, DateTime monstdate);
        int AddPdfFiles(ChecklistFileViewModel cfvm);
        List<ChecklistFileViewModel> GetFilterCLDownload(FilterViewModel fvm);
        List<SchoolSpecialistMappingViewModel> GetAllSchoolsFromSchoolUSerMappings(int userid, string cltype);
        List<ChecklistSummaryMatrixViewModel> GetReminderCLSummaryMatrix(int userid, string zone);
        List<ProjectMasterViewModel> GetAllProjectsBasedSchoolIDs(List<int> ids, DateTime lastdate);
        List<CheckListTransMasterViewModel> GetCLTMIDsfromTransMaster(List<int> ids, int usrid);
        List<ChecklistFileViewModel> GetFilterCLDownloadLift(FilterViewModel fvm);
        List<ChecklistFileViewModel> GetFilterCLDownload_ServiceType(FilterViewModel fvm);
        List<ProjectMasterViewModel> GetCLLiftMatrix(FilterViewModel fvm);
        List<CheckListIndexViewModel> GetMASearchChecklistIndex(int compid, string zone, int groupid);
        List<CheckListIndexViewModel> GetAdminChecklistIndex(int compid, DateTime firstdate, DateTime lastdate);
        List<CheckListIndexViewModel> GetAdminChecklistIndexNew(string zone, int schoolID, string cltype, string frequency, int clstatus, DateTime fromdate, DateTime todate);

        List<CheckListIndexViewModel> GetUserChecklistIndex(int compid, int userid, DateTime firstdate, DateTime lastdate);
        List<CheckListIndexViewModel> GetUserChecklistIndex_Future(int compid, int userid, DateTime firstdate, DateTime lastdate);
        List<CheckListIndexMobileViewModel> GetUserChecklistIndex_Mobile(int compid, int userid, DateTime firstdate, DateTime lastdate);
        List<CheckListIndexMobileViewModel> GetUserChecklistIndex_Future_Mobile(int compid, int userid, DateTime firstdate, DateTime lastdate);
        List<CheckListIndexMobileViewModel> GetMASearchChecklistIndex_Mobile(int compid, string zone, int groupid);

        int DeleteChecklistFile(int cfID);
        List<CheckListTypeMasterViewModel> GetAllCheckListType();
        int CreateCheckListType(CheckListTypeMasterViewModel CheckListType);
        bool CheckCLType(string cltype,int? cpyid);
        int DeleteCheckListType(int ID);
        int SaveCheckListType(CheckListTypeMasterViewModel checklisttype);
        CheckListTypeMasterViewModel GetCheckListType(int id);

        List<ProjectMasterViewModel> GetAllNewAsset(int userid);
        int CreateAssetMaster(ProjectMasterViewModel asset);
        int SaveAsset(ProjectMasterViewModel asset);
        bool CheckAsset(int? sclid, string cltype, string mnth, int? cpyid);
        ProjectMasterViewModel GetAsset(int PrjMasID);

        int DeleteAsset(int ID);

        List<CheckListItemTitleMasterViewModel> GetAllCheckListItem();
        int CreateCheckListItem(CheckListItemTitleMasterViewModel clitem);
        bool CheckCLItem(string cltype, string freq);
        int SaveCheckListItem(CheckListItemTitleMasterViewModel clItem);
        int DeleteCheckListItem(int ID);
        CheckListItemTitleMasterViewModel GetCheckListItem(int id);
        CheckListTypeMasterViewModel GetCheckListtype(int id);
    }
}