using BuildInspect.Models.Repository.Imp;
using BuildInspect.Models.Service.Imp;
using BuildInspect.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.Service.Interface
{
    public class ERPServices : IERPServices
    {
        private readonly IERPRepository erpRepository;
        public ERPServices(IERPRepository _erpRepository)
        {
            erpRepository = _erpRepository;
        }

        public List<BuildingMasterViewModel> GetAllSchools()
        {
            return erpRepository.GetAllSchools();
        }
        public bool CheckSchool(string name, string zone)
        {
            return erpRepository.CheckSchool(name, zone);
        }
        public int CreateSchool(BuildingMasterViewModel school)
        {
            return erpRepository.CreateSchool(school);
        }

        public int SaveSchool(BuildingMasterViewModel school)
        {
            return erpRepository.SaveSchool(school);
        }
        public int DeleteSchool(int sID)
        {
            return erpRepository.DeleteSchool(sID);
        }



        public List<CheckListItemMasterViewModel> GetAllCheckListItems()
        {
            return erpRepository.GetAllCheckListItems();
        }
        public ProjectMasterViewModel GetFrequency(DateTime startDate, DateTime endDate)
        {
            return erpRepository.GetFrequency(startDate, endDate);
        }
        public BuildingMasterViewModel GetSchool(int id)
        {
            return erpRepository.GetSchool(id);
        }
        public List<CheckListTransMasterViewModel> GetAllSubmittedCheckList()
        {
            return erpRepository.GetAllSubmittedCheckList();
        }
        public int SubmitChecklist(CheckListTransMasterViewModel cltm, List<CheckListTransDetailViewModel> Detail, string path, string halfPath)
        {
            return erpRepository.SubmitChecklist(cltm, Detail, path, halfPath);
        }
        public CheckListTransMasterViewModel GetCheckListItems(int id)
        {
            return erpRepository.GetCheckListItems(id);
        }
        public List<ProjectMasterViewModel> GetAllProjects()
        {
            return erpRepository.GetAllProjects();
        }
        public List<ServiceFormViewModel> GetAllServiceForms()
        {
            return erpRepository.GetAllServiceForms();
        }
        public int EditChecklist(CheckListTransMasterViewModel cltm, List<CheckListTransDetailViewModel> Detail)
        {
            return erpRepository.EditChecklist(cltm, Detail);
        }
        public int SubmitSignatureCheckList(CheckListTransMasterViewModel cltm)
        {
            return erpRepository.SubmitSignatureCheckList(cltm);
        }
        public int AssignService(ServiceFormViewModel sfm)
        {
            return erpRepository.AssignService(sfm);
        }
        public ServiceFormViewModel GetServiceForm(int id)
        {
            return erpRepository.GetServiceForm(id);
        }
        public int ServiceFormSave(ServiceFormViewModel sfm, string path, string halfPath)
        {
            return erpRepository.ServiceFormSave(sfm,path,halfPath);
        }
        public CheckListTransMasterMobileViewModel GetCheckListItemsforMobile(int id)
        {
            return erpRepository.GetCheckListItemsforMobile(id);
        }
        public int ServiceFormMobileSave(ServiceFormMobileViewModel sfm, string path, string halfPath)
        {
            return erpRepository.ServiceFormMobileSave(sfm, path, halfPath);
        }
        public List<CheckListTransMasterViewModel> GetAllSubmittedCheckList_School(List<int> ids)
        {
            return erpRepository.GetAllSubmittedCheckList_School(ids);
        }
        public List<CheckListTransMasterViewModel> GetAllSubmittedCheckList_User(List<int> ids)
        {
            return erpRepository.GetAllSubmittedCheckList_User(ids);
        }
        public List<CheckListTransMasterViewModel> GetAllSubmittedCheckList_Project(int id)
        {
            return erpRepository.GetAllSubmittedCheckList_Project(id);
        }
        public int SubmitSignatureCheckListForMobile(CheckListTransMasterViewModel cltm)
        {
            return erpRepository.SubmitSignatureCheckListForMobile(cltm);
        }
        public int SubmitMASignatureCheckListForMobile(CheckListTransMasterViewModel cltm)
        {
            return erpRepository.SubmitMASignatureCheckListForMobile(cltm);
        }
        public int SubmitChecklistForMobile(CheckListTransMasterViewModel cltm, List<CheckListTransDetailViewModel> Detail, string path, string halfPath)
        {
            return erpRepository.SubmitChecklistForMobile(cltm, Detail, path, halfPath);
        }
        public List<int?> GetApplicableSchoolIDs(List<int> ids, string splType, int userid)
        {
            return erpRepository.GetApplicableSchoolIDs(ids,splType, userid);
        }
        public int GetLiftCount(int schoolid, int userid, DateTime monstdate)
        {
            return erpRepository.GetLiftCount(schoolid, userid, monstdate);
        }
        public List<ChecklistSummaryMatrixViewModel> GetCurCLSummaryMatrix(int userid)
        {
            return erpRepository.GetCurCLSummaryMatrix(userid);
        }
        public bool CheckCLAlreadySubmitted(int PrjMasID, int SchoolID)
        {
            return erpRepository.CheckCLAlreadySubmitted(PrjMasID, SchoolID);
        }
        public ProjectMasterViewModel GetProject(int id)
        {
            return erpRepository.GetProject(id);
        }
        //public List<int?> GetLiftSchoolIDs(int userid)
        //{
        //    return erpRepository.GetLiftSchoolIDs(userid);
        //}
        public List<ProjectMasterViewModel> GetAllProject_SchoolID(List<int> ids, string splType)
        {
            return erpRepository.GetAllProject_SchoolID(ids, splType);
        }
        public List<DashboardSummaryViewModel> GetDashboardSummary(int userid, int groupid, int companyid, DateTime startdt, DateTime enddt)
        {
            return erpRepository.GetDashboardSummary(userid,groupid,companyid, startdt, enddt);
        }
        public List<ProjectMasterViewModel> GetLiftProjects(List<int> ids, string oprname)
        {
            return erpRepository.GetLiftProjects(ids,oprname);
        }
        public List<ProjectMasterViewModel> GetAllProjectsOtherthanLift(List<int> ids, string cltype)
        {
            return erpRepository.GetAllProjectsOtherthanLift(ids, cltype);
        }
        public List<CheckListTransMasterViewModel> GetAllSubmittedCheckList_CLType(List<int> ids, string cltype)
        {
            return erpRepository.GetAllSubmittedCheckList_CLType(ids, cltype);
        }
        public int SubmitChecklistForMobileOffline(CheckListTransMasterViewModel cltm, List<CheckListTransDetailViewModel> Detail, string path, string halfPath)
        {
            return erpRepository.SubmitChecklistForMobileOffline(cltm, Detail, path, halfPath);
        }
        public int ServiceFormMobileOffline(ServiceFormMobileViewModel sfm, string path, string halfPath)
        {
            return erpRepository.ServiceFormMobileOffline(sfm, path, halfPath);
        }
        public List<ChecklistSummaryMatrixViewModel> GetFilterCLSummaryMatrix(FilterViewModel fvm)
        {
            return erpRepository.GetFilterCLSummaryMatrix(fvm);
        }
        public int ServiceFormMobileOfflineSave(ServiceFormMobileViewModel sfm, string path, string halfPath)
        {
            return erpRepository.ServiceFormMobileOfflineSave(sfm, path, halfPath);
        }
        public ProjectMasterViewModel GetProjectFromDate(DateTime firstdate)
        {
            return erpRepository.GetProjectFromDate(firstdate);
        }
        public List<ProjectMasterViewModel> GetProjectsForApplicableCLs(int schoolid, DateTime monstdate)
        {
            return erpRepository.GetProjectsForApplicableCLs(schoolid, monstdate);
        }
        public int AddPdfFiles(ChecklistFileViewModel cfvm)
        {
            return erpRepository.AddPdfFiles(cfvm);
        }
        public List<ChecklistFileViewModel> GetFilterCLDownload(FilterViewModel fvm)
        {
            return erpRepository.GetFilterCLDownload(fvm);
        }
        public List<SchoolSpecialistMappingViewModel> GetAllSchoolsFromSchoolUSerMappings(int userid, string cltype)
        {
            return erpRepository.GetAllSchoolsFromSchoolUSerMappings(userid, cltype);
        }
        public List<ChecklistSummaryMatrixViewModel> GetReminderCLSummaryMatrix(int userid, string zone)
        {
            return erpRepository.GetReminderCLSummaryMatrix(userid, zone);
        }
        public List<ProjectMasterViewModel> GetAllProjectsBasedSchoolIDs(List<int> ids, DateTime lastdate)
        {
            return erpRepository.GetAllProjectsBasedSchoolIDs(ids, lastdate);
        }
        public List<CheckListTransMasterViewModel> GetCLTMIDsfromTransMaster(List<int> ids, int usrid)
        {
            return erpRepository.GetCLTMIDsfromTransMaster(ids, usrid);
        }
        public List<ChecklistFileViewModel> GetFilterCLDownloadLift(FilterViewModel fvm)
        {
            return erpRepository.GetFilterCLDownloadLift(fvm);
        }
        public List<ChecklistFileViewModel> GetFilterCLDownload_ServiceType(FilterViewModel fvm)
        {
            return erpRepository.GetFilterCLDownload_ServiceType(fvm);
        }
        public List<ProjectMasterViewModel> GetCLLiftMatrix(FilterViewModel fvm)
        {
            return erpRepository.GetCLLiftMatrix(fvm);
        }
        public List<CheckListIndexViewModel> GetMASearchChecklistIndex(int compid, string zone, int groupid)
        {
            return erpRepository.GetMASearchChecklistIndex(compid, zone, groupid);
        }
        public List<CheckListIndexViewModel> GetAdminChecklistIndex(int compid, DateTime firstdate, DateTime lastdate)
        {
            return erpRepository.GetAdminChecklistIndex(compid, firstdate, lastdate);
        }
        public List<CheckListIndexViewModel> GetUserChecklistIndex(int compid, int userid, DateTime firstdate, DateTime lastdate)
        {
            return erpRepository.GetUserChecklistIndex(compid, userid, firstdate, lastdate);
        }
        public List<CheckListIndexViewModel> GetAdminChecklistIndexNew(string zone, int schoolID, string cltype, string frequency, int clstatus, DateTime fromdate, DateTime todate)
        {
            return erpRepository.GetAdminChecklistIndexNew(zone, schoolID, cltype, frequency, clstatus, fromdate, todate);
        }
            public List<CheckListIndexViewModel> GetUserChecklistIndex_Future(int compid, int userid, DateTime firstdate, DateTime lastdate)
        {
            return erpRepository.GetUserChecklistIndex_Future(compid, userid, firstdate, lastdate);
        }
        public List<CheckListIndexMobileViewModel> GetUserChecklistIndex_Mobile(int compid, int userid, DateTime firstdate, DateTime lastdate)
        {
            return erpRepository.GetUserChecklistIndex_Mobile(compid, userid, firstdate, lastdate);
        }
        public List<CheckListIndexMobileViewModel> GetUserChecklistIndex_Future_Mobile(int compid, int userid, DateTime firstdate, DateTime lastdate)
        {
            return erpRepository.GetUserChecklistIndex_Future_Mobile(compid, userid, firstdate, lastdate);
        }
        public List<CheckListIndexMobileViewModel> GetMASearchChecklistIndex_Mobile(int compid, string zone, int groupid)
        {
            return erpRepository.GetMASearchChecklistIndex_Mobile(compid, zone, groupid);
        }

        public int DeleteChecklistFile(int cfID)
        {
            return erpRepository.DeleteChecklistFile(cfID);
        }
        public List<CheckListTypeMasterViewModel> GetAllCheckListType()
        {
            return erpRepository.GetAllCheckListType();
        }
        public int CreateCheckListType(CheckListTypeMasterViewModel CheckListType)
        {
            return erpRepository.CreateCheckListType(CheckListType);
        }
        public bool CheckCLType(string cltype, int? cpyid)
        {
            return erpRepository.CheckCLType(cltype, cpyid);
        }
        public int DeleteCheckListType(int ID)
        {
            return erpRepository.DeleteCheckListType(ID);
        }
        public int SaveCheckListType(CheckListTypeMasterViewModel checklisttype)
        {
            return erpRepository.SaveCheckListType(checklisttype);
        }
        public CheckListTypeMasterViewModel GetCheckListType(int id)
        {
            return erpRepository.GetCheckListType(id);
        }
        public List<ProjectMasterViewModel> GetAllNewAsset(int userid)
        {
            return erpRepository.GetAllNewAsset(userid);
        }
        public int CreateAssetMaster(ProjectMasterViewModel asset)
        {
            return erpRepository.CreateAsset(asset);
        }
        public int SaveAsset(ProjectMasterViewModel asset)
        {
            return erpRepository.SaveAsset(asset);
        }
        public bool CheckAsset(int? sclid, string cltype, string mnth,int? cpyid)
        {
            return erpRepository.CheckAsset(sclid, cltype, mnth,cpyid);
        }
        public ProjectMasterViewModel GetAsset(int PrjMasID)
        {
            return erpRepository.GetAsset(PrjMasID);
        }
        public int DeleteAsset(int ID)
        {
            return erpRepository.DeleteAsset(ID);
        }
        public List<CheckListItemTitleMasterViewModel> GetAllCheckListItem()
        {
            return erpRepository.GetAllCheckListItem();
        }
        public int CreateCheckListItem(CheckListItemTitleMasterViewModel clitem)
        {
            return erpRepository.CreateCheckListItem(clitem);
        }
        public bool CheckCLItem(string cltype, string freq)
        {
            return erpRepository.CheckCLItem(cltype, freq);
        }
        public int SaveCheckListItem(CheckListItemTitleMasterViewModel clItem)
        {
            return erpRepository.SaveCheckListItem(clItem);
        }
        public int DeleteCheckListItem(int ID)
        {
            return erpRepository.DeleteCheckListItem(ID);
        }
        public CheckListItemTitleMasterViewModel GetCheckListItem(int id)
        {
            return erpRepository.GetCheckListItem(id);
        }
        public CheckListTypeMasterViewModel GetCheckListtype(int id)
        {
            return erpRepository.GetCheckListtype(id);
        }
    }
}