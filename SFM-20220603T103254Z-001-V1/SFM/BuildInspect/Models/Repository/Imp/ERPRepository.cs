using BuildInspect.Models.Repository.Imp;
using BuildInspect.Models.ViewModel;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BuildInspect.Models.Domain;
using AutoMapper;
using System.Data.Entity;
using BuildInspect.Models.Utility;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Globalization;

namespace BuildInspect.Models.Repository.Interface
{
  
    public class ERPRepository : IERPRepository
    {
        BuildInspectEntities BInDB = new BuildInspectEntities();
        Logger logger = LogManager.GetCurrentClassLogger();

        #region School
        public List<BuildingMasterViewModel> GetAllSchools()
        {
            var res = BInDB.building_master.OrderBy(a => a.Zone).ToList();
            var lists = Mapper.Map<List<BuildingMasterViewModel>>(res);

            return lists;
        }

        public bool CheckSchool(string name, string zone)
        {
            try
            {
                var res = BInDB.building_master.Where(a => a.Building_Name == name && a.Zone == zone).SingleOrDefault();
                if (res == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public int CreateSchool(BuildingMasterViewModel school)
        {
            var _db_res = Mapper.Map<building_master>(school);
            BInDB.building_master.Add(_db_res);
            return BInDB.SaveChanges();
        }

        public BuildingMasterViewModel GetSchool(int id)
        {
            var res = BInDB.building_master.Where(a => a.BuildingID == id).FirstOrDefault();
            var lists = Mapper.Map<BuildingMasterViewModel>(res);
            return lists;
        }

        public int SaveSchool(BuildingMasterViewModel school)
        {
            var _db_res = Mapper.Map<building_master>(school);
            BInDB.Entry(_db_res).State = EntityState.Modified;
            return BInDB.SaveChanges();
        }

        public int DeleteSchool(int sID)
        {
            var _db_res = BInDB.building_master.First(a => a.BuildingID == sID);
            _db_res.UpdatedBy = AppSession.GetCurrentUserId();
            _db_res.UpdatedDate = DateTime.Now;
            _db_res.IsActive = 0;

            BInDB.Entry(_db_res).State = EntityState.Modified;
            return BInDB.SaveChanges();
        }

        #endregion

        public int DeleteChecklistFile(int cfID)
        {
            try
            {

           
                var _db_res = BInDB.chklst_trn_files.First(a => a.CLFileID == cfID);
                if (_db_res != null)
                {
                    BInDB.chklst_trn_files.Remove(_db_res);
                
                    File.Delete(_db_res.FileCaption);
                }

             return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {

                return BInDB.SaveChanges();
            }
        }

        public List<CheckListItemMasterViewModel> GetAllCheckListItems()
        {
            var res = BInDB.chklst_item_master.OrderBy(a => a.FormName).ToList();
            var lists = Mapper.Map<List<CheckListItemMasterViewModel>>(res);
            return lists;
        }

        public ProjectMasterViewModel GetFrequency(DateTime startDate, DateTime endDate)
        {
            var res = BInDB.project_master.Where(a => a.Month_Start_Date == startDate && a.Month_End_Date==endDate).FirstOrDefault();
            var lists = Mapper.Map<ProjectMasterViewModel>(res);

            return lists;
        }
               

        public List<CheckListTransMasterViewModel> GetAllSubmittedCheckList()
        {
            var res = BInDB.chklst_trn_master.ToList();
            var lists = Mapper.Map<List<CheckListTransMasterViewModel>>(res);            
            return lists;
        }

        public List<CheckListTransMasterViewModel> GetAllSubmittedCheckList_CLType(List<int> ids, string cltype)
        {
            var res = BInDB.chklst_trn_master.Where(a => ids.Contains(a.BuildingID ?? default(int)) && a.CLType==cltype).ToList();
            var lists = Mapper.Map<List<CheckListTransMasterViewModel>>(res);
            return lists;
        }

        public List<CheckListTransMasterViewModel> GetAllSubmittedCheckList_School(List<int> ids)        
        {
            var res = BInDB.chklst_trn_master.Where(a=> ids.Contains(a.BuildingID?? default(int))).ToList();
            var lists = Mapper.Map<List<CheckListTransMasterViewModel>>(res);
            return lists;
        }

        public List<CheckListTransMasterViewModel> GetAllSubmittedCheckList_User(List<int> ids)
        {
            var res = BInDB.chklst_trn_master.Where(a => ids.Contains(a.UserID ?? default(int))).ToList();
            var lists = Mapper.Map<List<CheckListTransMasterViewModel>>(res);
            return lists;
        }

        public List<CheckListTransMasterViewModel> GetAllSubmittedCheckList_Project(int id)
        {
            var res = BInDB.chklst_trn_master.Where(a => a.PrjMasID == id).ToList();
            var lists = Mapper.Map<List<CheckListTransMasterViewModel>>(res);
            return lists;
        }

        public List<ProjectMasterViewModel> GetAllProject_SchoolID(List<int> ids, string splType)
        {
            var newList = ids.Select(i => (int?)i).ToList();
            var res = BInDB.project_master.Where(a=> newList.Contains(a.BuildingID) && a.CLType == splType).ToList();
            var lists = Mapper.Map<List<ProjectMasterViewModel>>(res);
            return lists;
        }

        public List<int?> GetApplicableSchoolIDs(List<int> ids, string splType, int userid)
        {
            var newList = ids.Select(i => (int?)i).ToList();
            var res = BInDB.project_master.Where(a => newList.Contains(a.BuildingID) && a.CLType == splType).Select(a=>a.BuildingID).Distinct().ToList();
            
            return res;
        }

        public List<SchoolSpecialistMappingViewModel> GetAllSchoolsFromSchoolUSerMappings(int userid, string cltype)
        {
            var res = BInDB.school_user_mapping.Where(a => a.UserID == userid && a.CLType == cltype).ToList();
            var lists = Mapper.Map<List<SchoolSpecialistMappingViewModel>>(res);
            return lists;
        }

        //public List<int?> GetLiftSchoolIDs(int userid)
        //{            
        //    var listIds = BInDB.school_liftopr_mapping.Where(a=>a.UserID==userid).Select(a => a.SchoolID).ToList();            
        //    return listIds;
        //}

        public List<ProjectMasterViewModel> GetLiftProjects(List<int> ids, string oprname)
        {
            var newList = ids.Select(i => (int?)i).ToList();
            var res = BInDB.project_master.Where(a => newList.Contains(a.BuildingID) && a.Lift_Opr_Name == oprname).ToList();
            var lists = Mapper.Map<List<ProjectMasterViewModel>>(res);
            return lists;
        }
        
        
        public List<CheckListTransMasterViewModel> GetCLTMIDsfromTransMaster(List<int> ids, int usrid)
        {
            var newList = ids.Select(i => (int?)i).ToList();
            var res = BInDB.chklst_trn_master.Where(a => newList.Contains(a.BuildingID) && a.UserID == usrid).ToList();
            var lists = Mapper.Map<List<CheckListTransMasterViewModel>>(res);
            return lists;
        }

        public List<ProjectMasterViewModel> GetAllProjectsOtherthanLift(List<int> ids, string cltype)
        {
            var newList = ids.Select(i => (int?)i).ToList();
            var res = BInDB.project_master.Where(a => newList.Contains(a.BuildingID) && a.CLType == cltype).ToList();
            var lists = Mapper.Map<List<ProjectMasterViewModel>>(res);
            return lists;
        }

        public List<ProjectMasterViewModel> GetAllProjectsBasedSchoolIDs(List<int> ids, DateTime lastdate)
        {
            var newList = ids.Select(i => (int?)i).ToList();
            var res = BInDB.project_master.Where(a => newList.Contains(a.BuildingID) && a.Month_End_Date <= lastdate).ToList();
            var lists = Mapper.Map<List<ProjectMasterViewModel>>(res);
            return lists;
        }


        public List<ProjectMasterViewModel> GetProjectsForApplicableCLs(int schoolid, DateTime monstdate)
        {
            
            var res = BInDB.project_master.Where(a => a.BuildingID == schoolid && a.Month_Start_Date == monstdate).ToList();
            var lists = Mapper.Map<List<ProjectMasterViewModel>>(res);
            return lists;
        }

        public int GetLiftCount(int schoolid, int userid, DateTime monstdate)
        {
            var oprname = BInDB.users.Find(userid).DisplayName;
            var cnt = BInDB.project_master.Where(a => a.CLType == "SP-LIFTSYS" && a.BuildingID == schoolid && a.Lift_Opr_Name == oprname && a.Month_Start_Date == monstdate).FirstOrDefault().Lift_Count;
            
            return cnt ?? default(int); 
        }

        public int SubmitChecklist(CheckListTransMasterViewModel cltm, List<CheckListTransDetailViewModel> Detail, string path, string halfPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(cltm.CheckedBy_DateTime))
                    cltm.CheckedBy_DateTime = DateTime.ParseExact(cltm.CheckedBy_DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm");
                if (!string.IsNullOrEmpty(cltm.VerifiedBy_DateTime))
                    cltm.VerifiedBy_DateTime = DateTime.ParseExact(cltm.VerifiedBy_DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm");
                if (!string.IsNullOrEmpty(cltm.EndoresBy_DateTime))
                    cltm.EndoresBy_DateTime = DateTime.ParseExact(cltm.EndoresBy_DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm");
                if (string.IsNullOrEmpty(cltm.CheckedBy_Signature))
                    {
                    cltm.Status_Flag = 0;
                    }
                else
                {
                    cltm.Status_Flag = 1;                    
                }

                chklst_trn_master domainCltm = Mapper.Map<chklst_trn_master>(cltm);
                if (cltm.Status_Flag == 1)
                    domainCltm.CheckedBy_DateTime = DateTime.Now;
                BInDB.chklst_trn_master.Add(domainCltm);
              
                foreach (var desc in Detail)
                {
                    if (!string.IsNullOrEmpty(desc.DateDone))
                        desc.DateDone = DateTime.ParseExact(desc.DateDone, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
 
                    chklst_trn_detail domainDetail = Mapper.Map<chklst_trn_detail>(desc);
                    domainDetail.CLTMID = domainCltm.CLTMID;                    
                    BInDB.chklst_trn_detail.Add(domainDetail);
                }

                var result = BInDB.SaveChanges();
                if (result > 0)
                    if (cltm.files != null && cltm.files.Count > 0)
                    {
                        int i = 0;
                        foreach (var file in cltm.files)
                        {
                            if (file != null)
                            {
                                var filename = Path.GetFileName(file.FileName);
                                var rn = DateTime.Now.ToString("yyMMddhhmmss");

                                Directory.CreateDirectory(path);
                                var finalPath = path + "CLFM_" + cltm.CLTMID + "_" + i.ToString() + "_" + rn + "." + getFileExtention(filename);
                                var halfPathEach = halfPath + "CLFM_" + cltm.CLTMID + "_" + i.ToString() + "_" + rn + "." + getFileExtention(filename);
                                file.SaveAs(finalPath);

                                chklst_trn_files rptFiles = new chklst_trn_files();
                                rptFiles.CLTMID = domainCltm.CLTMID;
                                rptFiles.FilePath = halfPathEach;
                                rptFiles.FileCaption = finalPath;
                                rptFiles.FileName = file.FileName;
                                BInDB.chklst_trn_files.Add(rptFiles);
                                i++;
                            }
                        }
                        BInDB.SaveChanges();
                        return result;
                    }
                return result;


            }
            catch (Exception ex)
            {
                logger.Debug("Chk List Submission:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return -1;
            }
        }


        public int SubmitChecklistForMobile(CheckListTransMasterViewModel cltm, List<CheckListTransDetailViewModel> Detail, string path, string halfPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(cltm.CheckedBy_DateTime))
                    cltm.CheckedBy_DateTime = DateTime.ParseExact(cltm.CheckedBy_DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm");
                if (!string.IsNullOrEmpty(cltm.VerifiedBy_DateTime))
                    cltm.VerifiedBy_DateTime = DateTime.ParseExact(cltm.VerifiedBy_DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm");
                if (!string.IsNullOrEmpty(cltm.EndoresBy_DateTime))
                    cltm.EndoresBy_DateTime = DateTime.ParseExact(cltm.EndoresBy_DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm");
                if (string.IsNullOrEmpty(cltm.CheckedBy_Signature))
                {
                    cltm.Status_Flag = 0;
                }
                else
                {
                    cltm.Status_Flag = 1;
                }

                chklst_trn_master domainCltm = Mapper.Map<chklst_trn_master>(cltm);
                if (cltm.Status_Flag == 1)
                    domainCltm.CheckedBy_DateTime = DateTime.Now;
                BInDB.chklst_trn_master.Add(domainCltm);

                foreach (var desc in Detail)
                {
                    if (!string.IsNullOrEmpty(desc.DateDone))
                        desc.DateDone = DateTime.ParseExact(desc.DateDone, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");

                    chklst_trn_detail domainDetail = Mapper.Map<chklst_trn_detail>(desc);
                    domainDetail.CLTMID = domainCltm.CLTMID;
                    BInDB.chklst_trn_detail.Add(domainDetail);
                }

                var result= BInDB.SaveChanges();
                path = path + domainCltm.UserID + "/";
                halfPath = halfPath + domainCltm.UserID + "/";
                if (result > 0)
                    if (cltm.mobileFiles != null && cltm.mobileFiles.Count > 0)
                    {
                        int i = 0;
                        foreach (var file in cltm.mobileFiles)
                        {
                            if (file != null)
                            {
                                var data = Convert.FromBase64String(file.data);
                                var filename = file.filename;

                                var rn = DateTime.Now.ToString("yyMMddhhmmss");

                                Directory.CreateDirectory(path);
                                var finalPath = path + "CLFM_" + cltm.CLTMID + "_" + i.ToString() + "_" + rn + "." + getFileExtention(filename);
                                var halfPathEach = halfPath + "CLFM_" + cltm.CLTMID + "_" + i.ToString() + "_" + rn + "." + getFileExtention(filename);

                                File.WriteAllBytes(finalPath, data);

                                chklst_trn_files rptFiles = new chklst_trn_files();
                                rptFiles.CLTMID = domainCltm.CLTMID;
                                rptFiles.FilePath = halfPathEach;
                                rptFiles.FileCaption = finalPath;
                                rptFiles.FileName = file.filename;
                                BInDB.chklst_trn_files.Add(rptFiles);
                                i++;
                            }
                        }
                        BInDB.SaveChanges();
                        return result;
                    }
                return result;
            }
            catch (Exception ex)
            {
                logger.Debug("Chk List Mobile Submission:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return -1;
            }
        }

        public int SubmitChecklistForMobileOffline(CheckListTransMasterViewModel cltm, List<CheckListTransDetailViewModel> Detail, string path, string halfPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(cltm.CheckedBy_DateTime))
                    cltm.CheckedBy_DateTime = DateTime.ParseExact(cltm.CheckedBy_DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm");
                if (!string.IsNullOrEmpty(cltm.VerifiedBy_DateTime))
                    cltm.VerifiedBy_DateTime = DateTime.ParseExact(cltm.VerifiedBy_DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm");
                if (!string.IsNullOrEmpty(cltm.EndoresBy_DateTime))
                    cltm.EndoresBy_DateTime = DateTime.ParseExact(cltm.EndoresBy_DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy HH:mm");
                if (string.IsNullOrEmpty(cltm.CheckedBy_Signature))
                {
                    cltm.Status_Flag = 0;
                }
                else
                {
                    cltm.Status_Flag = 1;
                }              

                if (!string.IsNullOrEmpty(cltm.EndoresBy_Signature))
                {
                    cltm.Status_Flag = 2;

                }              

                chklst_trn_master domainCltm = Mapper.Map<chklst_trn_master>(cltm);               


                BInDB.chklst_trn_master.Add(domainCltm);

                foreach (var desc in Detail)
                {
                    if (!string.IsNullOrEmpty(desc.DateDone))
                        desc.DateDone = DateTime.ParseExact(desc.DateDone, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");

                    chklst_trn_detail domainDetail = Mapper.Map<chklst_trn_detail>(desc);
                    domainDetail.CLTMID = domainCltm.CLTMID;
                    BInDB.chklst_trn_detail.Add(domainDetail);
                }

                var result = BInDB.SaveChanges();
                path = path + domainCltm.UserID + "/";
                halfPath = halfPath + domainCltm.UserID + "/";
                if (result > 0)
                {
                    if (cltm.mobileFiles != null && cltm.mobileFiles.Count > 0)
                    {
                        int i = 0;
                        foreach (var file in cltm.mobileFiles)
                        {
                            if (file != null)
                            {
                                var data = Convert.FromBase64String(file.data);
                                var filename = file.filename;

                                var rn = DateTime.Now.ToString("yyMMddhhmmss");

                                Directory.CreateDirectory(path);
                                var finalPath = path + "CLFM_" + domainCltm.CLTMID + "_" + i.ToString() + "_" + rn + "." + getFileExtention(filename);
                                var halfPathEach = halfPath + "CLFM_" + domainCltm.CLTMID + "_" + i.ToString() + "_" + rn + "." + getFileExtention(filename);

                                File.WriteAllBytes(finalPath, data);

                                chklst_trn_files rptFiles = new chklst_trn_files();
                                rptFiles.CLTMID = domainCltm.CLTMID;
                                rptFiles.FilePath = halfPathEach;
                                rptFiles.FileCaption = finalPath;
                                rptFiles.FileName = file.filename;
                                BInDB.chklst_trn_files.Add(rptFiles);
                                i++;
                            }
                        }
                        BInDB.SaveChanges();

                    }
                    return domainCltm.CLTMID;
                }
                else
                {
                    return result;
                }
                
            }
            catch (Exception ex)
            {
                logger.Debug("Chk List Mobile Submission offline:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return -1;
            }
        }

        public ProjectMasterViewModel GetProject(int id)
        {
            var res = BInDB.project_master.Find(id);
            var prj = Mapper.Map<ProjectMasterViewModel>(res);
            return prj;
        }

        public CheckListTransMasterViewModel GetCheckListItems(int id)
        {
            var res = BInDB.chklst_trn_master.Where(a => a.CLTMID == id).FirstOrDefault();
            var lists = Mapper.Map<CheckListTransMasterViewModel>(res);
            return lists;
        }

        public CheckListTransMasterMobileViewModel GetCheckListItemsforMobile(int id)
        {
            var res = BInDB.chklst_trn_master.Where(a => a.CLTMID == id).FirstOrDefault();
            var lists = Mapper.Map<CheckListTransMasterMobileViewModel>(res);
            return lists;
        }

        public List<ProjectMasterViewModel> GetAllProjects()
        {
            var res = BInDB.project_master.ToList();
            var lists = Mapper.Map<List<ProjectMasterViewModel>>(res);
            return lists;
        }

        public List<ServiceFormViewModel> GetAllServiceForms()
        {
            var res = BInDB.service_form.ToList();
            var lists = Mapper.Map<List<ServiceFormViewModel>>(res);
            return lists;
        }

        public int EditChecklist(CheckListTransMasterViewModel cltm, List<CheckListTransDetailViewModel> Detail)
        {
            try
            {
                var _db_cltm = BInDB.chklst_trn_master.AsNoTracking().Where(a => a.CLTMID == cltm.CLTMID).FirstOrDefault();
                if (string.IsNullOrEmpty(cltm.EndoresBy_Signature))
                {
                   // _db_cltm.Status_Flag = 1;
                }
                else
                {
                    _db_cltm.Status_Flag = 2;
                    _db_cltm.EndoresBy_DateTime = DateTime.Now;
                }
                _db_cltm.EndoresBy_Signature = cltm.EndoresBy_Signature;
                _db_cltm.EndoresBy_Remarks = cltm.EndoresBy_Remarks;
                _db_cltm.EndoresBy_Name = cltm.EndoresBy_Name;

                if (!string.IsNullOrEmpty(cltm.EndoresBy_Signature) && !string.IsNullOrEmpty(_db_cltm.VerifiedBy_Signature))
                {
                    _db_cltm.Status_Flag = 4;

                }

                BInDB.Entry(_db_cltm).State = EntityState.Modified;

                foreach (var desc in Detail)
                {
                    if (!string.IsNullOrEmpty(desc.DateDone))
                        desc.DateDone = DateTime.ParseExact(desc.DateDone, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");

                    var _det = BInDB.chklst_trn_detail.Find(desc.CLTDID);
                    _det.IsDone = desc.IsDone;
                    _det.DateDone = Convert.ToDateTime(desc.DateDone);
                    _det.Remarks = desc.Remarks;
                }
                

                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Chk List Edit:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return -1;
            }
        }

        public int SubmitSignatureCheckList(CheckListTransMasterViewModel cltm)
        {
            try
            {
                var _db_cltm = BInDB.chklst_trn_master.AsNoTracking().Where(a => a.CLTMID == cltm.CLTMID).FirstOrDefault();

                    
                    _db_cltm.VerifiedBy_Signature = cltm.VerifiedBy_Signature;                
                    _db_cltm.VerifiedBy_Remarks = cltm.VerifiedBy_Remarks;
                    _db_cltm.VerifiedBy_Name = cltm.VerifiedBy_Name;

                if (!string.IsNullOrEmpty(cltm.VerifiedBy_Signature))
                {
                    _db_cltm.Status_Flag = 3;
                    _db_cltm.VerifiedBy_DateTime = DateTime.Now;
                }
                //if (string.IsNullOrEmpty(_db_cltm.EndoresBy_Signature))
                //{
                //    _db_cltm.Status_Flag = 2;
                //}
                if (!string.IsNullOrEmpty(_db_cltm.EndoresBy_Signature) && !string.IsNullOrEmpty(cltm.VerifiedBy_Signature))                
                {                    
                    _db_cltm.Status_Flag = 4;                 
                    
                }                

                BInDB.Entry(_db_cltm).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Signature submission:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return -1;
            }
        }



        public int SubmitSignatureCheckListForMobile(CheckListTransMasterViewModel cltm)
        {
            try
            {
                var _db_cltm = BInDB.chklst_trn_master.AsNoTracking().Where(a => a.CLTMID == cltm.CLTMID).FirstOrDefault();

                _db_cltm.EndoresBy_Signature = cltm.EndoresBy_Signature;
                _db_cltm.EndoresBy_Remarks = cltm.EndoresBy_Remarks;
                _db_cltm.EndoresBy_Name = cltm.EndoresBy_Name;
                                
                if (!string.IsNullOrEmpty(cltm.EndoresBy_Signature))
                {
                    _db_cltm.Status_Flag = 2;
                    _db_cltm.EndoresBy_DateTime = DateTime.Now;                    
                }
                
                if (!string.IsNullOrEmpty(cltm.EndoresBy_Signature) && !string.IsNullOrEmpty(_db_cltm.VerifiedBy_Signature))
                {
                    _db_cltm.Status_Flag = 4;
                }

                BInDB.Entry(_db_cltm).State = EntityState.Modified;

                foreach (var desc in cltm.clTrnDetail)
                {
                    if (!string.IsNullOrEmpty(desc.DateDone))
                        desc.DateDone = DateTime.ParseExact(desc.DateDone, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");

                    if(desc.CLTDID != 0)
                    { 
                    var _det = BInDB.chklst_trn_detail.Find(desc.CLTDID);
                    _det.IsDone = desc.IsDone;
                    _det.DateDone = Convert.ToDateTime(desc.DateDone);
                    _det.Remarks = desc.Remarks;
                    }

                }

                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Signature submission:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return -1;
            }
        }

        public int SubmitMASignatureCheckListForMobile(CheckListTransMasterViewModel cltm)
        {
            try
            {
                var _db_cltm = BInDB.chklst_trn_master.AsNoTracking().Where(a => a.CLTMID == cltm.CLTMID).FirstOrDefault();
                
                _db_cltm.VerifiedBy_Signature = cltm.VerifiedBy_Signature;
                _db_cltm.VerifiedBy_Remarks = cltm.VerifiedBy_Remarks;
                _db_cltm.VerifiedBy_Name = cltm.VerifiedBy_Name;

                if (!string.IsNullOrEmpty(cltm.VerifiedBy_Signature))
                {
                    _db_cltm.Status_Flag = 3;
                    _db_cltm.VerifiedBy_DateTime = DateTime.Now;
                }

                if (!string.IsNullOrEmpty(_db_cltm.EndoresBy_Signature) && !string.IsNullOrEmpty(_db_cltm.VerifiedBy_Signature))
                {
                    _db_cltm.Status_Flag = 4;
                }

                BInDB.Entry(_db_cltm).State = EntityState.Modified;              

                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Signature submission:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return -1;
            }
        }


        public int AssignService(ServiceFormViewModel sfm)
        {
            try
            {                
                service_form domainSfm = Mapper.Map<service_form>(sfm);
                domainSfm.AssignedDate = DateTime.Now;
                //domainSfm.Status_Flag = 0;
                BInDB.service_form.Add(domainSfm);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("Service Form submission:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return -1;
            }
        }

        public ServiceFormViewModel GetServiceForm(int id)
        {
            var res = BInDB.service_form.Where(a => a.ServiceID == id).FirstOrDefault();
            var lists = Mapper.Map<ServiceFormViewModel>(res);
            return lists;
        }

        public int ServiceFormSave(ServiceFormViewModel sfm, string path, string halfPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(sfm.Service_Datetime))
                    sfm.Service_Datetime = DateTime.ParseExact(sfm.Service_Datetime, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
                sfm.Technician_Signature_Date = sfm.Service_Datetime;

                var domainSF = Mapper.Map<service_form>(sfm);
                if (sfm.Customer_incharge_Signature != null && sfm.Technician_Signature != null)
                    domainSF.Status_Flag = 2;

                domainSF.Technician_Signature_Date = DateTime.Now;
                
                BInDB.Entry(domainSF).State = EntityState.Modified;                              

                var result = BInDB.SaveChanges();
                if (result > 0)
                    if (sfm.files != null && sfm.files.Count > 0)
                    {
                        int i = 0;
                        foreach (var file in sfm.files)
                        {
                            if (file != null)
                            {
                                var filename = Path.GetFileName(file.FileName);
                                var rn = DateTime.Now.ToString("yyMMddhhmmss");

                                Directory.CreateDirectory(path);
                                var finalPath = path + "SFM_" + sfm.ServiceID + "_" + i.ToString() + "_" + rn + "." + getFileExtention(filename);
                                var halfPathEach = halfPath + "SFM_" + sfm.ServiceID + "_" + i.ToString() + "_" + rn + "." + getFileExtention(filename);
                                file.SaveAs(finalPath);

                                service_form_files rptFiles = new service_form_files();
                                rptFiles.ServiceID = sfm.ServiceID;
                                rptFiles.FilePath = halfPathEach;
                                rptFiles.FileCaption = finalPath;
                                rptFiles.FileName = file.FileName;                                
                                BInDB.service_form_files.Add(rptFiles);
                                i++;
                            }
                        }
                        BInDB.SaveChanges();
                        return result;
                    }
                return result;

            }
            catch (Exception ex)
            {
                logger.Debug("Service Form Save:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return -1;
            }

        }

        public int ServiceFormMobileSave(ServiceFormMobileViewModel sfm, string path, string halfPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(sfm.Service_Datetime))
                    sfm.Service_Datetime = DateTime.ParseExact(sfm.Service_Datetime, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");

                var _db_sfm = BInDB.service_form.AsNoTracking().Where(a => a.ServiceID == sfm.ServiceID).FirstOrDefault();
                _db_sfm.CaseRefNum = sfm.CaseRefNum;
                _db_sfm.Contact_Person = sfm.Contact_Person;
                _db_sfm.Service_Datetime = Convert.ToDateTime(sfm.Service_Datetime);
                _db_sfm.Location = sfm.Location;
                _db_sfm.RoomNo = sfm.RoomNo;
                _db_sfm.ServiceType = sfm.ServiceType;
                _db_sfm.Work_Requests = sfm.Work_Requests;
                _db_sfm.Customer_Rep = sfm.Customer_Rep;
                _db_sfm.Customer_Rep_Signature = sfm.Customer_Rep_Signature;
                _db_sfm.Work_Completion_Details = sfm.Work_Completion_Details;
                _db_sfm.Remarks_TC = sfm.Remarks_TC;
                _db_sfm.Client_Comments = sfm.Client_Comments;
                _db_sfm.Technician_Name = sfm.Technician_Name;
                _db_sfm.Technician_Signature = sfm.Technician_Signature;
                _db_sfm.Customer_incharge_Name = sfm.Customer_incharge_Name;
                _db_sfm.Customer_incharge_Signature = sfm.Customer_incharge_Signature;
               

                if (sfm.Technician_Signature != null )
                    _db_sfm.Technician_Signature_Date = _db_sfm.Service_Datetime;                
                if (sfm.Customer_incharge_Signature != null && sfm.Technician_Signature != null)
                    _db_sfm.Status_Flag = 2;

                BInDB.Entry(_db_sfm).State = EntityState.Modified;

                var result = BInDB.SaveChanges();
                if (result > 0)
                    if (sfm.mobileFiles != null && sfm.mobileFiles.Count > 0)
                    {
                        int i = 0;
                        foreach (var file in sfm.mobileFiles)
                        {
                            if (file != null)
                            {
                                var data = Convert.FromBase64String(file.data);
                                var filename = file.filename;
                                
                                var rn = DateTime.Now.ToString("yyMMddhhmmss");

                                Directory.CreateDirectory(path);
                                var finalPath = path + "SFM_" + sfm.ServiceID + "_" + i.ToString() + "_" + rn + "." + getFileExtention(filename);
                                var halfPathEach = halfPath + "SFM_" + sfm.ServiceID + "_" + i.ToString() + "_" + rn + "." + getFileExtention(filename);
                                
                                File.WriteAllBytes(finalPath, data);

                                service_form_files rptFiles = new service_form_files();
                                rptFiles.ServiceID = sfm.ServiceID;
                                rptFiles.FilePath = halfPathEach;
                                rptFiles.FileCaption = finalPath;
                                rptFiles.FileName = file.filename;
                                BInDB.service_form_files.Add(rptFiles);
                                i++;
                            }
                        }
                        BInDB.SaveChanges();
                        return result;
                    }
                return result;

            }
            catch (Exception ex)
            {
                logger.Debug("Service Form Save Error:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return -1;
            }

        }

        public List<ChecklistSummaryMatrixViewModel> GetReminderCLSummaryMatrix(int userid, string zone)
        {
            var dCurrentDayofThisMonth = DateTime.Today.ToString("yyyy-MM-dd");
            var dFirstDayOfCurrMonth = DateTime.Today.AddDays(-(DateTime.Today.Day - 1));
            var dFirstDayOfThisMonth = DateTime.Today.AddDays(-(DateTime.Today.Day - 1)).ToString("yyyy-MM-dd");
            var dLastDayOfThisMonth = dFirstDayOfCurrMonth.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

            var sql = "exec GetSummaryMatrix " + userid + ", '" + dFirstDayOfThisMonth + "', '" + dLastDayOfThisMonth + "', '" + zone + "'";

            var obj = BInDB.Database.SqlQuery<ChecklistSummaryMatrixViewModel>(sql).ToList();
            return obj;
        }

        public List<ChecklistSummaryMatrixViewModel> GetCurCLSummaryMatrix(int userid)
        {            
            var dCurrentDayofThisMonth = DateTime.Today.ToString("yyyy-MM-dd");
            var dFirstDayOfCurrMonth = DateTime.Today.AddDays(-(DateTime.Today.Day - 1));
            var dFirstDayOfThisMonth = DateTime.Today.AddDays(-(DateTime.Today.Day - 1)).ToString("yyyy-MM-dd");
            var dLastDayOfThisMonth = dFirstDayOfCurrMonth.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");            

            var sql = "exec GetSummaryMatrix " + userid + ", '" + dFirstDayOfThisMonth + "', '" + dLastDayOfThisMonth + "', 'ALL'";

            var obj = BInDB.Database.SqlQuery<ChecklistSummaryMatrixViewModel>(sql).ToList();
            return obj;           
        }

        public List<ChecklistSummaryMatrixViewModel> GetFilterCLSummaryMatrix(FilterViewModel fvm)
        {
            var fildt = fvm.Month + fvm.Year;
            fildt = DateTime.ParseExact(fildt, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");

            DateTime date = Convert.ToDateTime(fildt);
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            //var dCurrentDayofThisMonth = DateTime.Today.ToString("yyyy-MM-dd");
            //var dFirstDayOfCurrMonth = DateTime.Today.AddDays(-(DateTime.Today.Day - 1));
            var dFirstDayOfThisMonth = firstDayOfMonth.ToString("yyyy-MM-dd");
            var dLastDayOfThisMonth =  lastDayOfMonth.ToString("yyyy-MM-dd");

            var sql = "exec GetSummaryMatrix " + fvm.userid + ", '" + dFirstDayOfThisMonth + "', '" + dLastDayOfThisMonth + "', '" + fvm.Zone + "'";

            var obj = BInDB.Database.SqlQuery<ChecklistSummaryMatrixViewModel>(sql).ToList();
            return obj;
        }

        public List<ProjectMasterViewModel> GetCLLiftMatrix(FilterViewModel fvm)
        {
            var fildt = fvm.Month + fvm.Year;
            fildt = DateTime.ParseExact(fildt, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");

            DateTime date = Convert.ToDateTime(fildt);
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            //var dCurrentDayofThisMonth = DateTime.Today.ToString("yyyy-MM-dd");
            //var dFirstDayOfCurrMonth = DateTime.Today.AddDays(-(DateTime.Today.Day - 1));
            var dFirstDayOfThisMonth = firstDayOfMonth.ToString("yyyy-MM-dd");
            var dLastDayOfThisMonth = lastDayOfMonth.ToString("yyyy-MM-dd");

            var sql = "exec GetLiftCLSummary " + fvm.userid + ", '" + dFirstDayOfThisMonth + "', '" + dLastDayOfThisMonth + "', '" + fvm.Zone + "'";

            var obj = BInDB.Database.SqlQuery<ProjectMasterViewModel>(sql).ToList();
            return obj;
        }

        public List<CheckListIndexViewModel> GetMASearchChecklistIndex(int compid, string zone, int groupid)
        {
            var now = DateTime.Now;
            var firstdate = new DateTime(now.Year, now.Month, 1);
            var premonth_firstdate = firstdate.AddMonths(-1);
            if (groupid == 5)
            {
               // premonth_firstdate = firstdate.AddMonths(-5);
                premonth_firstdate = firstdate.AddMonths(-14);
            }
                var dFirstDayOfThisMonth = premonth_firstdate.ToString("yyyy-MM-dd");
            
            var sql = "exec GetMAChecklist " + compid + ", '" + dFirstDayOfThisMonth + "', '" + zone + "'";
           

            var obj = BInDB.Database.SqlQuery<CheckListIndexViewModel>(sql).ToList();
            return obj;
        }

        public List<CheckListIndexMobileViewModel> GetMASearchChecklistIndex_Mobile(int compid, string zone, int groupid)
        {
            var now = DateTime.Now;
            var firstdate = new DateTime(now.Year, now.Month, 1);
            var premonth_firstdate = firstdate.AddMonths(-1);
            if (groupid == 5)
            {
                premonth_firstdate = firstdate.AddMonths(-5);
            }
            var dFirstDayOfThisMonth = premonth_firstdate.ToString("yyyy-MM-dd");

            var sql = "exec GetMAChecklist " + compid + ", '" + dFirstDayOfThisMonth + "', '" + zone + "'";


            var obj = BInDB.Database.SqlQuery<CheckListIndexMobileViewModel>(sql).ToList();
            return obj;
        }

        public List<CheckListIndexViewModel> GetAdminChecklistIndex(int compid, DateTime firstdate, DateTime lastdate)
        {
                     
            var dFirstDayOfThisMonth = firstdate.ToString("yyyy-MM-dd");
            var dLastDayOfThisMonth = lastdate.ToString("yyyy-MM-dd");

            var sql = "exec GetAdminChecklist " + compid + ", '" + dFirstDayOfThisMonth + "', '" + dLastDayOfThisMonth + "'";


            var obj = BInDB.Database.SqlQuery<CheckListIndexViewModel>(sql).ToList();
            return obj;

        }



        public List<CheckListIndexViewModel> GetAdminChecklistIndexNew(string zone, int schoolID, string cltype, string frequency, int clstatus, DateTime fromdate, DateTime todate)
        {
           // if()
            var dfromdate = fromdate.ToString("yyyy-MM-dd");
            var dtodate = todate.ToString("yyyy-MM-dd");
            var sql = "";

            // var sql = "exec GetAdminChecklistNew " + schoolID + ", '" + dFirstDayOfThisMonth + "', '" + dLastDayOfThisMonth + "', '" + zone + "'";

            //if (zone != "Select" && schoolID != 0 && cltype != "Select" && frequency != "Select" && clstatus != 0 && dfromdate != "01/01/2000" && dtodate != "01/01/2000")
           if (zone != "Select" && schoolID != 0 && cltype != "Select" && frequency != "Select" && clstatus != 0 && dfromdate != "2000-01-01" && dtodate != "2000-01-01")
            {
                sql = "exec GetAdminChecklistNew_6 '" + zone + "'," + schoolID + ",'" + cltype + "','" + frequency + "'," + clstatus + ",'" + dfromdate + "', '" + dtodate + "'";
            }
            else if (zone != "Select" && schoolID != 0 && cltype != "Select" && frequency != "Select" && clstatus != 0)
            {

                sql = "exec GetAdminChecklistNew_5 '" + zone + "'," + schoolID + ",'" + cltype + "','" + frequency + "'," + clstatus;
            }


            if (zone != "Select" && schoolID != 0 && cltype != "Select" && frequency != "Select" && dfromdate != "2000-01-01" && dtodate != "2000-01-01")
            {
                sql = "exec GetAdminChecklistNew_26 '" + zone + "'," + schoolID + ",'" + cltype + "','" + frequency + "','" + dfromdate + "', '" + dtodate + "'";
            }

            else if (zone != "Select" && schoolID != 0 && cltype != "Select" && clstatus != 0 && dfromdate != "2000-01-01" && dtodate != "2000-01-01")
            {
                sql = "exec GetAdminChecklistNew_25 '" + zone + "'," + schoolID + ",'" + cltype + "'," + clstatus + ",'" + dfromdate + "', '" + dtodate + "'";
            }

            else if (zone != "Select" && schoolID != 0  && frequency != "Select" && clstatus != 0 && dfromdate != "2000-01-01" && dtodate != "2000-01-01")
            {
                sql = "exec GetAdminChecklistNew_24 '" + zone + "'," + schoolID + ",'" + frequency + "'," + clstatus + ",'" + dfromdate + "', '" + dtodate + "'";
            }

            else if (zone != "Select"  && cltype != "Select" && frequency != "Select" && clstatus != 0 && dfromdate != "2000-01-01" && dtodate != "2000-01-01")
            {
                sql = "exec GetAdminChecklistNew_23 '" + zone + "','" + cltype + "','" + frequency + "'," + clstatus + ",'" + dfromdate + "', '" + dtodate + "'";
            }

            else if (zone != "Select"  && cltype != "Select"  && clstatus != 0 && dfromdate != "2000-01-01" && dtodate != "2000-01-01")
            {
                sql = "exec GetAdminChecklistNew_22 '" + zone + "','" + cltype + "'," + clstatus + ",'" + dfromdate + "', '" + dtodate + "'";
            }

            else if (zone != "Select"  && cltype != "Select" && frequency != "Select" && clstatus != 0)
            {
                sql = "exec GetAdminChecklistNew_21 '" + zone + "','" + cltype + "','" + frequency + "'," + clstatus ;
            }

            else  if (zone != "Select" && schoolID != 0  && clstatus != 0 && dfromdate != "2000-01-01" && dtodate != "2000-01-01")

            {
                sql = "exec GetAdminChecklistNew_20 '" + zone + "'," + schoolID + "," + clstatus + ",'" + dfromdate + "', '" + dtodate + "'";

            }

            else if (zone != "Select" && schoolID != 0 && cltype != "Select" && frequency != "Select")
            {

                sql = "exec GetAdminChecklistNew_4 '" + zone + "'," + schoolID + ",'" + cltype + "','" + frequency + "'";

            }
            else if (zone != "Select" && schoolID != 0 &&  frequency != "Select" && clstatus != 0)
            {
                sql = "exec GetAdminChecklistNew_19 '" + zone + "'," + schoolID + ",'" + frequency + "'," + clstatus ;
            }


            else if (zone != "Select" && frequency != "Select" && dfromdate != "2000-01-01" && dtodate != "2000-01-01")
            {

                sql = "exec GetAdminChecklistNew_18 '" + zone + "','" + frequency + "','" + dfromdate + "', '" + dtodate + "'";
            }

             else if (zone != "Select" && frequency != "Select" && clstatus != 0)
            {
                sql = "exec GetAdminChecklistNew_17 '" + zone + "','" + frequency + "'," + clstatus ;
            }

            else if (zone != "Select" &&  cltype != "Select"  && dfromdate != "2000-01-01" && dtodate != "2000-01-01")

            {
                sql = "exec GetAdminChecklistNew_16 '" + zone + "','" + cltype + "','" + dfromdate + "', '" + dtodate + "'";
            }

             else if (zone != "Select" && cltype != "Select"  && clstatus != 0)
            {
                sql = "exec GetAdminChecklistNew_15 '" + zone + "','" + cltype + "'," + clstatus ;
            }
            
             else if (zone != "Select" && cltype != "Select" && frequency != "Select")

            {
                sql = "exec GetAdminChecklistNew_14 '" + zone + "','" + cltype + "','" + frequency + "'";
            }

            else if (zone != "Select" && schoolID != 0 && dfromdate != "2000-01-01" && dtodate != "2000-01-01")
            {
                sql = "exec GetAdminChecklistNew_13 '" + zone + "'," + schoolID + ",'" + dfromdate + "', '" + dtodate + "'";
            }

            else if (zone != "Select" && schoolID != 0 && cltype != "Select")
            {
                sql = "exec GetAdminChecklistNew_3 '" + zone + "'," + schoolID + ",'" + cltype + "'";

            }

            else if (zone != "Select" && schoolID != 0  && clstatus != 0)
            {
                sql = "exec GetAdminChecklistNew_12 '" + zone + "'," + schoolID + "," + clstatus ;
            }

             else if (zone != "Select" && schoolID != 0 && frequency != "Select")
            {
                sql = "exec GetAdminChecklistNew_11 '" + zone + "'," + schoolID + ",'" + frequency + "'";
            }

             else if (zone != "Select" && dfromdate != "2000-01-01" && dtodate != "2000-01-01")
            {
                sql = "exec GetAdminChecklistNew_10 '" + zone + "','" + dfromdate + "', '" + dtodate + "'";
            }

            else if (zone != "Select" && clstatus != 0)
            {
                sql = "exec GetAdminChecklistNew_9 '" + zone + "'," + clstatus;
            }

            else if (zone != "Select"  && frequency != "Select")
            {
                sql = "exec GetAdminChecklistNew_8 '" + zone + "','" + frequency + "'";
            }

            else if (zone != "Select" && cltype != "Select")
            {
                sql = "exec GetAdminChecklistNew_7 '" + zone + "','" + cltype + "'";
            }
          
           
            else if (zone != "Select" && schoolID != 0)
            {
                
                sql = "exec GetAdminChecklistNew_2 '" + zone + "'," + schoolID;
            }
            else {
                sql = "exec GetAdminChecklistNew_1 '" + zone + "'";
            }

           //  sql = "exec GetAdminChecklistNew '" + zone + "',"+ schoolID +",'"+ cltype +"','"+ frequency +"',"+ clstatus +",'" + dfromdate + "', '" + dtodate + "'";


            var obj = BInDB.Database.SqlQuery<CheckListIndexViewModel>(sql).ToList();
            return obj;

        }

        public List<CheckListIndexViewModel> GetUserChecklistIndex(int compid, int userid, DateTime firstdate, DateTime lastdate)
        {

            var dFirstDayOfThisMonth = firstdate.ToString("yyyy-MM-dd");
            var dLastDayOfThisMonth = lastdate.ToString("yyyy-MM-dd");

            var sql = "exec GetUserChecklist " + compid +", " + userid +", '" + dFirstDayOfThisMonth + "', '" + dLastDayOfThisMonth + "'";
            
            var obj = BInDB.Database.SqlQuery<CheckListIndexViewModel>(sql).ToList();
            return obj;
        }

        public List<CheckListIndexViewModel> GetUserChecklistIndex_Future(int compid, int userid, DateTime firstdate, DateTime lastdate)
        {

            var dFirstDayOfThisMonth = firstdate.ToString("yyyy-MM-dd");
            var dLastDayOfThisMonth = lastdate.ToString("yyyy-MM-dd");

            var sql = "exec GetUserChecklist_Future " + compid + ", " + userid + ", '" + dFirstDayOfThisMonth + "', '" + dLastDayOfThisMonth + "'";

            var obj = BInDB.Database.SqlQuery<CheckListIndexViewModel>(sql).ToList();
            return obj;
        }

        public List<CheckListIndexMobileViewModel> GetUserChecklistIndex_Mobile(int compid, int userid, DateTime firstdate, DateTime lastdate)
        {

            var dFirstDayOfThisMonth = firstdate.ToString("yyyy-MM-dd");
            var dLastDayOfThisMonth = lastdate.ToString("yyyy-MM-dd");

            var sql = "exec GetUserChecklist " + compid + ", " + userid + ", '" + dFirstDayOfThisMonth + "', '" + dLastDayOfThisMonth + "'";

            var obj = BInDB.Database.SqlQuery<CheckListIndexMobileViewModel>(sql).ToList();
            return obj;
        }

        public List<CheckListIndexMobileViewModel> GetUserChecklistIndex_Future_Mobile(int compid, int userid, DateTime firstdate, DateTime lastdate)
        {

            var dFirstDayOfThisMonth = firstdate.ToString("yyyy-MM-dd");
            var dLastDayOfThisMonth = lastdate.ToString("yyyy-MM-dd");

            var sql = "exec GetUserChecklist_Future " + compid + ", " + userid + ", '" + dFirstDayOfThisMonth + "', '" + dLastDayOfThisMonth + "'";

            var obj = BInDB.Database.SqlQuery<CheckListIndexMobileViewModel>(sql).ToList();
            return obj;
        }


        public bool CheckCLAlreadySubmitted(int PrjMasID, int SchoolID)
        {
            var chk = BInDB.chklst_trn_master.Where(a => a.PrjMasID == PrjMasID && a.BuildingID == SchoolID).ToList();
            if (chk.Count > 0)
                return true;
            else
                return false;
        }

        public List<DashboardSummaryViewModel> GetDashboardSummary(int userid, int groupid, int companyid, DateTime startdt, DateTime enddt)
        {
            var dCurrentDayofThisMonth = DateTime.Today.ToString("yyyy-MM-dd");
            var dFirstDayOfCurrMonth = DateTime.Today.AddDays(-(DateTime.Today.Day - 1));
            var dFirstDayOfThisMonth = startdt.ToString("yyyy-MM-dd");
            var dLastDayOfThisMonth = enddt.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

            var sql = "exec GetDashboardSummary " + userid + ", "+groupid+ ", " + companyid+ ", '" + dFirstDayOfThisMonth + "', '" + dLastDayOfThisMonth + "'";

            var obj = BInDB.Database.SqlQuery<DashboardSummaryViewModel>(sql).ToList();
            return obj;
        }

        public int ServiceFormMobileOffline(ServiceFormMobileViewModel sfm, string path, string halfPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(sfm.Service_Datetime))
                    sfm.Service_Datetime = DateTime.ParseExact(sfm.Service_Datetime, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");

                service_form domainSfm = Mapper.Map<service_form>(sfm);

                BInDB.service_form.Add(domainSfm); 

                var result = BInDB.SaveChanges();
                if (result > 0)
                    if (sfm.mobileFiles != null && sfm.mobileFiles.Count > 0)
                    {
                        int i = 0;
                        foreach (var file in sfm.mobileFiles)
                        {
                            if (file != null)
                            {
                                var data = Convert.FromBase64String(file.data);
                                var filename = file.filename;

                                var rn = DateTime.Now.ToString("yyMMddhhmmss");

                                Directory.CreateDirectory(path);
                                var finalPath = path + "SFM_" + domainSfm.ServiceID + "_" + i.ToString() + "_" + rn + "." + getFileExtention(filename);
                                var halfPathEach = halfPath + "SFM_" + domainSfm.ServiceID + "_" + i.ToString() + "_" + rn + "." + getFileExtention(filename);

                                File.WriteAllBytes(finalPath, data);

                                service_form_files rptFiles = new service_form_files();
                                rptFiles.ServiceID = domainSfm.ServiceID;
                                rptFiles.FilePath = halfPathEach;
                                rptFiles.FileCaption = finalPath;
                                rptFiles.FileName = file.filename;
                                BInDB.service_form_files.Add(rptFiles);
                                i++;
                            }
                        }
                        BInDB.SaveChanges();
                        if (result > 0)
                            return domainSfm.ServiceID;
                        else
                            return result;
                    }
                return result;

            }
            catch (Exception ex)
            {
                logger.Debug("Service Form Offline Create Error:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return -1;
            }

        }


        public int ServiceFormMobileOfflineSave(ServiceFormMobileViewModel sfm, string path, string halfPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(sfm.Service_Datetime))
                    sfm.Service_Datetime = DateTime.ParseExact(sfm.Service_Datetime, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");

                var _db_sfm = BInDB.service_form.AsNoTracking().Where(a => a.ServiceID == sfm.ServiceID).FirstOrDefault();
                _db_sfm.CaseRefNum = sfm.CaseRefNum;
                _db_sfm.Contact_Person = sfm.Contact_Person;
                _db_sfm.Service_Datetime = Convert.ToDateTime(sfm.Service_Datetime);
                _db_sfm.Location = sfm.Location;
                _db_sfm.RoomNo = sfm.RoomNo;
                _db_sfm.ServiceType = sfm.ServiceType;
                _db_sfm.Work_Requests = sfm.Work_Requests;
                _db_sfm.Customer_Rep = sfm.Customer_Rep;
                _db_sfm.Customer_Rep_Signature = sfm.Customer_Rep_Signature;
                _db_sfm.Work_Completion_Details = sfm.Work_Completion_Details;
                _db_sfm.Remarks_TC = sfm.Remarks_TC;
                _db_sfm.Client_Comments = sfm.Client_Comments;
                _db_sfm.Technician_Name = sfm.Technician_Name;
                _db_sfm.Technician_Signature = sfm.Technician_Signature;
                _db_sfm.Customer_incharge_Name = sfm.Customer_incharge_Name;
                _db_sfm.Customer_incharge_Signature = sfm.Customer_incharge_Signature;


                if (sfm.Technician_Signature != null)
                    _db_sfm.Technician_Signature_Date = _db_sfm.Service_Datetime;
                if (sfm.Customer_incharge_Signature != null && sfm.Technician_Signature != null)
                    _db_sfm.Status_Flag = 2;

                BInDB.Entry(_db_sfm).State = EntityState.Modified;

                var result = BInDB.SaveChanges();
              
                if (result > 0)
                    if (sfm.mobileFiles != null && sfm.mobileFiles.Count > 0)
                    {
                        int i = 0;
                        foreach (var file in sfm.mobileFiles)
                        {
                            if (file != null)
                            {
                                var data = Convert.FromBase64String(file.data);
                                var filename = file.filename;

                                var rn = DateTime.Now.ToString("yyMMddhhmmss");

                                Directory.CreateDirectory(path);
                                var finalPath = path + "SFM_" + _db_sfm.ServiceID + "_" + i.ToString() + "_" + rn + "." + getFileExtention(filename);
                                var halfPathEach = halfPath + "SFM_" + _db_sfm.ServiceID + "_" + i.ToString() + "_" + rn + "." + getFileExtention(filename);

                                File.WriteAllBytes(finalPath, data);

                                service_form_files rptFiles = new service_form_files();
                                rptFiles.ServiceID = _db_sfm.ServiceID;
                                rptFiles.FilePath = halfPathEach;
                                rptFiles.FileCaption = finalPath;
                                rptFiles.FileName = file.filename;
                                BInDB.service_form_files.Add(rptFiles);
                                i++;
                            }
                        }
                        BInDB.SaveChanges();
                        if (result > 0)
                            return _db_sfm.ServiceID;
                        else
                            return result;
                    }
                return result;

            }
            catch (Exception ex)
            {
                logger.Debug("Service Form Offline Save Error:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return -1;
            }

        }

        public ProjectMasterViewModel GetProjectFromDate(DateTime firstdate)
        {
            var res = BInDB.project_master.Where(a => a.Month_Start_Date == firstdate).FirstOrDefault();
            var lists = Mapper.Map<ProjectMasterViewModel>(res);
            return lists;
        }

        public int AddPdfFiles(ChecklistFileViewModel cfvm)
        {
            try
            {
                checklist_files domainCfm = Mapper.Map<checklist_files>(cfvm);                
                BInDB.checklist_files.Add(domainCfm);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("CL Pdf Addition:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return -1;
            }
        }


        public List<ChecklistFileViewModel> GetFilterCLDownload(FilterViewModel fvm)
        {
            var fildt = fvm.Month + fvm.Year;
            fildt = DateTime.ParseExact(fildt, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
            DateTime date = Convert.ToDateTime(fildt);
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);

            var res = BInDB.checklist_files.Where(a=>a.BuildingID == fvm.SchoolID && a.Month_Start_Date == firstDayOfMonth).ToList();
            var lists = Mapper.Map<List<ChecklistFileViewModel>>(res);
            return lists;
        }

        public List<ChecklistFileViewModel> GetFilterCLDownloadLift(FilterViewModel fvm)
        {
            var newSchList = fvm.schoolids.Select(i => (int?)i).ToList();
            //var newPdfList = fvm.pdfclids.Select(i => (int?)i).ToList();

            var fildt = fvm.Month + fvm.Year;
            fildt = DateTime.ParseExact(fildt, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
            DateTime date = Convert.ToDateTime(fildt);
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);

            var res = BInDB.checklist_files.Where(a => newSchList.Contains(a.BuildingID) && a.Month_Start_Date == firstDayOfMonth && a.CLType == "SP-LIFTSYS").ToList();
            var lists = Mapper.Map<List<ChecklistFileViewModel>>(res);
            return lists;
        }

        public List<ChecklistFileViewModel> GetFilterCLDownload_ServiceType(FilterViewModel fvm)
        {
            var fildt = fvm.Month + fvm.Year;
            fildt = DateTime.ParseExact(fildt, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
            DateTime date = Convert.ToDateTime(fildt);
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);

            var res = BInDB.checklist_files.Where(a => a.BuildingID == fvm.SchoolID && a.Month_Start_Date == firstDayOfMonth && a.CLType == fvm.serviceType).ToList();
            var lists = Mapper.Map<List<ChecklistFileViewModel>>(res);
            return lists;
        }

        string getFileExtention(string filename)
        {
            var result = filename.Split(new string[] { }, StringSplitOptions.None);
            return result[result.Length - 1];
        }

        #region CHECK LIST TYPE

        public List<CheckListTypeMasterViewModel> GetAllCheckListType()
        {

            var res = BInDB.servicetype_master.ToList();
            var lists = Mapper.Map<List<CheckListTypeMasterViewModel>>(res);
            return lists;
            
        }

        public int CreateCheckListType(CheckListTypeMasterViewModel CheckListType)
        {
            var _db_res = Mapper.Map<servicetype_master>(CheckListType);
            BInDB.servicetype_master.Add(_db_res);
            return BInDB.SaveChanges();
        }



        public bool CheckCLType(string cltype,int? cpyid)
        {
            try
            {
                var res = BInDB.servicetype_master.Where(a => a.ServiceType == cltype && a.CompanyID == cpyid).SingleOrDefault();
                if (res == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public int DeleteCheckListType(int ID)
        {
            var _db_res = BInDB.servicetype_master.First(a => a.Id == ID);
            _db_res.UpdatedBy = AppSession.GetCurrentUserId();
            _db_res.UpdatedDate = DateTime.Now;
            _db_res.IsActive = 0;
            BInDB.Entry(_db_res).State = EntityState.Modified;
            return BInDB.SaveChanges();
        }

        public CheckListTypeMasterViewModel GetCheckListType(int id)
        {
            var res = BInDB.servicetype_master.Where(a => a.Id == id).FirstOrDefault();
            var lists = Mapper.Map<CheckListTypeMasterViewModel>(res);
            return lists;
        }

        public int SaveCheckListType(CheckListTypeMasterViewModel checklisttype)
        {

            var _db_res = Mapper.Map<servicetype_master>(checklisttype);
            BInDB.Entry(_db_res).State = EntityState.Modified;
            return BInDB.SaveChanges();
       }




        #endregion
        #region Asset

        public List<ProjectMasterViewModel> GetAllNewAsset(int userid)
        {

            var sql = "exec GetProjects " + userid + "";

            var obj = BInDB.Database.SqlQuery<ProjectMasterViewModel>(sql).ToList();
            return obj;
        }

        public int CreateAsset(ProjectMasterViewModel asset)
        {
            var _db_res = Mapper.Map<project_master>(asset);
            BInDB.project_master.Add(_db_res);
            return BInDB.SaveChanges();
        }
        public int SaveAsset(ProjectMasterViewModel asset)
        {

            var _db_res = Mapper.Map<project_master>(asset);
            BInDB.Entry(_db_res).State = EntityState.Modified;
            return BInDB.SaveChanges();
        }

        public bool CheckAsset(int? sclid, string cltype, string mnth,int? cpyid)
        {

            try
            {
                var userid = AppSession.GetCurrentUserId();
                
                var sql = "exec GetProjects " + userid + "";

                var obj = BInDB.Database.SqlQuery<ProjectMasterViewModel>(sql).Where(a => a.BuildingID == sclid && a.CLType == cltype && a.MonthName == mnth && a.CompanyID == cpyid).ToList();

                if (obj.Count == 0)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public ProjectMasterViewModel GetAsset(int PrjMasID)
        {
            var userid = AppSession.GetCurrentUserId();
            var sql = "exec GetProjects " + userid + "";

            var obj = BInDB.Database.SqlQuery<ProjectMasterViewModel>(sql).Where(a => a.Id == PrjMasID).FirstOrDefault();

            var lists = Mapper.Map<ProjectMasterViewModel>(obj);
            return lists;
        }
        public int DeleteAsset(int ID)
        {
            var _db_res = BInDB.project_master.First(a => a.PrjMasID == ID);
          
            _db_res.IsActive = 0;
            BInDB.Entry(_db_res).State = EntityState.Modified;
            return BInDB.SaveChanges();
        }
        #endregion

        #region checklist item
        public List<CheckListItemTitleMasterViewModel> GetAllCheckListItem()
        {

            var res = BInDB.chklst_item_title_master.ToList();
            var lists = Mapper.Map<List<CheckListItemTitleMasterViewModel>>(res);
            return lists;

        }

        public int CreateCheckListItem(CheckListItemTitleMasterViewModel clitem)
        {
           
            var _db_res = Mapper.Map<chklst_item_title_master>(clitem);
            //_db_res.ChklstTypeId = clitem.ChklstTypeId ?? default(int); 
            BInDB.chklst_item_title_master.Add(_db_res);
            return BInDB.SaveChanges();
        }

        public bool CheckCLItem(string cltype, string freq)
        {
            try
            {
                var res = BInDB.chklst_item_title_master.Where(a => a.CheckListType == cltype && a.Frequency == freq).FirstOrDefault();
                if (res == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        public int SaveCheckListItem(CheckListItemTitleMasterViewModel clitem)
        {

            var _db_res = Mapper.Map<chklst_item_title_master>(clitem);
            BInDB.Entry(_db_res).State = EntityState.Modified;
            return BInDB.SaveChanges();
        }

        public int DeleteCheckListItem(int ID)
        {
            var _db_res = BInDB.chklst_item_title_master.First(a => a.chklstitemId == ID);
          
            //_db_res.IsActive = 0;
            BInDB.Entry(_db_res).State = EntityState.Modified;
            return BInDB.SaveChanges();
        }
        public CheckListItemTitleMasterViewModel GetCheckListItem(int id)
        {
            var res = BInDB.chklst_item_title_master.Where(a => a.chklstitemId == id).FirstOrDefault();
            var lists = Mapper.Map<CheckListItemTitleMasterViewModel>(res);
            return lists;
        }
        public CheckListTypeMasterViewModel GetCheckListtype(int id)
        {
            var res = BInDB.servicetype_master.Where(a => a.Id == id).FirstOrDefault();
            var lists = Mapper.Map<CheckListTypeMasterViewModel>(res);
            return lists;
        }

        #endregion checklist item

        #region



        #endregion
    }
}

