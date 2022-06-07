using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BuildInspect.Models.Domain;
using BuildInspect.Models.ViewModel;
using AutoMapper;
using BuildInspect.Models.Service.Imp;
using BuildInspect.Models.Utility;
using System.Globalization;
using BuildInspect.Models.Security;
using System.IO;
using NLog;
using ICSharpCode.SharpZipLib.Zip;

namespace BuildInspect.Controllers
{
    [AccessDeniedAuthorize]
    public class ERPController : SuperBaseController
    {
        private BuildInspectEntities db = new BuildInspectEntities();
        Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IERPServices erpService;
        private readonly IUserServices userService;

        public ERPController(IERPServices _erpService, IUserServices _userService)
        {
            erpService = _erpService;
            userService = _userService;

        }

        #region School
        public ActionResult SchoolIndex()
        {
            var cpyid = AppSession.GetCompanyId();
            return View(erpService.GetAllSchools().Where(a => a.CompanyID == cpyid && a.IsActive == 1).ToList());
        }

        public ActionResult SchoolCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SchoolCreate(BuildingMasterViewModel school)
        {
            if (ModelState.IsValid)
            {

                if (erpService.CheckSchool(school.Building_Name, school.Zone) == false)
                {
                    school.CreatedBy = AppSession.GetCurrentUserId();
                    school.CreatedDate = DateTime.Now;
                    school.IsActive = 1;
                    school.Is_Project_Created = 0;
                    school.CompanyID = AppSession.GetCompanyId();

                    var result = erpService.CreateSchool(school);
                    if (result > 0)
                    {
                        return getSuccessfulOperation();
                    }
                    else
                    {
                        return getFailedOperation();
                    }
                }
                else
                {
                    return getFailedOperation("School Name Already exits!");
                }
            }

            return View(school);
        }

        public ActionResult SchoolEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //   eng_users eng_users = db.eng_users.Find(id);
            var school = erpService.GetSchool(id ?? default(int));
            if (school == null)
            {
                return HttpNotFound();
            }

            return View(school);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SchoolEdit(BuildingMasterViewModel school)
        {
            if (ModelState.IsValid)
            {
                school.UpdatedBy = AppSession.GetCurrentUserId();
                school.UpdatedDate = DateTime.Now;

                var result = erpService.SaveSchool(school);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            return View(school);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("DeleteSchool")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSchool(int id)
        {
            var result = erpService.DeleteSchool(id);

            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }

        }
        #endregion




        #region Checklist

        public ActionResult GetSchoolDetail(int id)
        {
            var school = erpService.GetSchool(id);
            return Json(new
            {
                value = "OK",
                result = school
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ChecklistIndex()
        {
            try
            {

                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                var grpid = AppSession.GetCurrentUserGroup();
                List<CheckListIndexViewModel> newCL = new List<CheckListIndexViewModel>();
                //Admin or director group id

                //Team Lead
                //if (grpid == 7)
                //{
                //    var sts = "";
                //    List<int> schoolids = new List<int>();
                //    var schools = erpService.GetAllSchools().ToList();
                //    List<int> userids = userService.getAllUsers().Where(a => a.TeamLead_ID == usrid).Select(a=>a.UserID).ToList();                    
                //    var chkLists = erpService.GetAllSubmittedCheckList_User(userids).Where(a => a.CheckedBy_Signature != null).ToList();
                //    foreach (var chl in chkLists)
                //    {
                //        if (chl.EndoresBy_Signature == null)
                //        {
                //            sts = "Pending User";
                //        }
                //        else if (chl.VerifiedBy_Signature == null)
                //        {
                //            sts = "Pending MA";
                //        }
                //        else
                //        {
                //            sts = "Completed";
                //        }
                //        if (chl.VerifiedBy_Signature == null && chl.EndoresBy_Signature == null)
                //        {
                //            sts = "Pending User & MA";
                //        }
                //        var sch = schools.Where(a => a.SchoolID == chl.SchoolID).FirstOrDefault();
                //        newCL.Add(new CheckListIndexViewModel()
                //        {
                //            CLTMID = chl.CLTMID,
                //            PrjMasID = chl.PrjMasID.Value,
                //            SchoolID = chl.SchoolID.Value,
                //            Status = sts,
                //            Zone = sch.Zone,
                //            SchoolName = sch.School_Name,
                //            Frequency = chl.Frequency,
                //            CLType = chl.CLType
                //        });
                //    }
                //}



                //User - Technician
                if (grpid == 3)
                {
                    //List<int> schoolids = new List<int>();
                    //List<int> pasrs_schoolids = new List<int>();

                    if (user.ServiceType == "Electrical" || user.ServiceType == "Mechanical")
                    {
                        var now = DateTime.Now;
                        var firstdate = new DateTime(now.Year, now.Month, 1);
                        var lastdate = firstdate.AddMonths(1).AddDays(-1);

                        var curmon = lastdate.Month;

                        //if (user.Zone != "ALL")
                        //{
                        //    schoolids = erpService.GetAllSchools().Where(a => a.Zone == user.Zone).Select(a => a.SchoolID).ToList();
                        //}
                        //else
                        //{
                        //    schoolids = erpService.GetAllSchools().Where(a => a.CompanyID == user.CompanyID).Select(a => a.SchoolID).ToList();
                        //}

                        ////    List<int> userids = userService.getAllUsers().Where(a => a.TeamLead_ID == usrid).Select(a=>a.UserID).ToList();                    
                        ////    var chkLists = erpService.GetAllSubmittedCheckList_User(userids).Where(a => a.CheckedBy_Signature != null).ToList();

                        //var liftschoolids = erpService.GetApplicableSchoolIDs(schoolids, user.ServiceType, user.UserID).ToList();



                        //    //var prjs = erpService.GetAllProject_SchoolID(schoolids, user.ServiceType).Where(a => a.Month_End_Date <= lastdate).ToList();
                        //    var prjs = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date <= lastdate).ToList();

                        //    //public List<ProjectMasterViewModel> GetAllProjectsOtherthanLift(List<int> ids, string cltype)
                        //    //var prjs = erpService.GetAllProjects().Where(a => schoolids.Contains(a.SchoolID ?? default(int)) && a.Month_End_Date <= lastdate && a.CLType==user.ServiceType).ToList();
                        //    //var chkLists = erpService.GetAllSubmittedCheckList().ToList();

                        //    List<int> newList = liftschoolids.Where(v => v != null).Select(v => v.Value).ToList();
                        //    var chkLists = erpService.GetAllSubmittedCheckList_CLType(newList, user.ServiceType).ToList();

                        //    var leftObj = (from a in prjs
                        //               join b in chkLists on a.PrjMasID equals b.PrjMasID
                        //                into pcTable
                        //               from c in pcTable.DefaultIfEmpty()
                        //               select new { a.PrjMasID, a.SchoolID, a.Frequency, a.Month_End_Date, c }).ToList();

                        //foreach (var obj in leftObj)
                        //{
                        //    var sch = erpService.GetSchool(obj.SchoolID ?? default(int));
                        //    var sts = "";
                        //    if (obj.c != null)                        {

                        //        if (obj.c.CLTMID != 0)
                        //        {
                        //            if (obj.c.UserID == usrid || user.Designation == "TL")
                        //            {
                        //                if (obj.c.CheckedBy_Signature == null)
                        //                {
                        //                    sts = "Initiated";
                        //                }
                        //                else if (obj.c.EndoresBy_Signature == null)
                        //                {
                        //                    sts = "Pending User";
                        //                }
                        //                else if (obj.c.VerifiedBy_Signature == null)
                        //                {
                        //                    sts = "Pending MA";
                        //                }
                        //                else
                        //                {
                        //                    sts = "Completed";
                        //                }

                        //                if (obj.c.VerifiedBy_Signature == null && obj.c.EndoresBy_Signature == null)
                        //                {
                        //                    sts = "Pending User & MA";
                        //                }
                        //                newCL.Add(new CheckListIndexViewModel()
                        //                {
                        //                    CLTMID = obj.c.CLTMID,
                        //                    PrjMasID = obj.PrjMasID,
                        //                    SchoolID = obj.c.SchoolID.Value,
                        //                    Status = sts,
                        //                    Zone = sch.Zone,
                        //                    SchoolName = sch.School_Name,
                        //                    Frequency = obj.Frequency,
                        //                    CLType = user.ServiceType,
                        //                    Month_Name = obj.Month_End_Date.Value.ToString("MMMM yyyy")
                        //                });
                        //            }

                        //        }

                        //    }
                        //    else
                        //    {
                        //        sts = "New Submit";
                        //        newCL.Add(new CheckListIndexViewModel()
                        //        {
                        //            CLTMID = 0,
                        //            PrjMasID = obj.PrjMasID,
                        //            SchoolID = obj.SchoolID.Value,
                        //            Status = sts,
                        //            Zone = sch.Zone,
                        //            SchoolName = sch.School_Name,
                        //            Frequency = obj.Frequency,
                        //            CLType = user.ServiceType,
                        //            Month_Name = obj.Month_End_Date.Value.ToString("MMMM yyyy")
                        //        });
                        //    }

                        //}

                        newCL = erpService.GetUserChecklistIndex(user.CompanyID ?? default(int), usrid, firstdate, lastdate).Where(a => a.Status != "Completed").ToList();
                        if (user.ServiceType == "Electrical" && user.CompanyID <= 2)
                        {
                            if (curmon != 6)
                            {
                                var nxtyr = AppSettings.GetNextYearly(lastdate);
                                var prjsB = erpService.GetUserChecklistIndex_Future(user.CompanyID ?? default(int), usrid, firstdate, nxtyr).ToList();
                                newCL.AddRange(prjsB);
                            }

                        }
                    }
                    else
                    {
                        var now = DateTime.Now;
                        var firstdate = new DateTime(now.Year, now.Month, 1);
                        var lastdate = firstdate.AddMonths(1).AddDays(-1);

                        var curmon = lastdate.Month;

                        //if (user.Zone != "ALL")
                        //{
                        //    schoolids = erpService.GetAllSchools().Where(a => a.Zone == user.Zone).Select(a => a.SchoolID).ToList();
                        //}
                        //else
                        //{
                        //    schoolids = erpService.GetAllSchools().Where(a => a.CompanyID == user.CompanyID).Select(a => a.SchoolID).ToList();
                        //}

                        //if (user.ServiceType == "SP-CHILLERSYS")
                        //{
                        //    schoolids = erpService.GetAllSchoolsFromSchoolUSerMappings(user.UserID, user.ServiceType).Select(a=>a.SchoolID ?? default(int) ).ToList();
                        //}

                        //if (user.ServiceType == "SP-PASISCM")
                        //{                           
                        //    pasrs_schoolids = erpService.GetAllSchoolsFromSchoolUSerMappings(user.UserID, user.ServiceType).Select(a => a.SchoolID ?? default(int)).ToList();                           
                        //}

                        //var liftschoolids = erpService.GetApplicableSchoolIDs(schoolids, user.ServiceType, user.UserID).ToList();
                        //var liftschoolids = new List<int?>();
                        //if (user.ServiceType == "SP-LIFTSYS")
                        //{
                        //liftschoolids = erpService.GetLiftSchoolIDs(user.UserID).ToList();
                        //public List<ProjectMasterViewModel> GetLiftProjects(List<int> ids, string oprname)
                        //}
                        //else
                        //{
                        //liftschoolids = erpService.GetApplicableSchoolIDs(schoolids, user.ServiceType, user.UserID).ToList();
                        // }
                        //var liftschoolids = erpService.GetLiftSchoolIDs(user.UserID).ToList();



                        //List<int> newList = liftschoolids.Where(v => v != null).Select(v => v.Value).ToList();
                        //var prjs = new List<ProjectMasterViewModel>();
                        //if (user.ServiceType == "SP-LIFTSYS")
                        //{
                        //    //prjs = erpService.GetAllProject_SchoolID(newList, user.ServiceType).Where(a => a.Month_End_Date <= lastdate && a.Lift_Opr_Name == user.DisplayName).ToList();
                        //   // prjs = erpService.GetLiftProjects(schoolids, user.DisplayName).Where(a => a.Month_End_Date <= lastdate).ToList();
                        //}
                        //else if (user.ServiceType == "SP-SECUTYSYS")
                        //{
                        //    //var nxtqtr = AppSettings.GetNextQuartelry(lastdate);
                        //      //// prjs = erpService.GetAllProject_SchoolID(newList, user.ServiceType).Where(a => a.Month_End_Date <= lastdate && a.Lift_Opr_Name == user.DisplayName).ToList();
                        //   //prjs = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date <= nxtqtr).ToList();
                        //}
                        //else if (user.ServiceType == "SP-WETCHSUSYS")
                        //{
                        //   // var nxtyr = AppSettings.GetNextYearly(lastdate);
                        //       ////prjs = erpService.GetAllProject_SchoolID(newList, user.ServiceType).Where(a => a.Month_End_Date <= lastdate && a.Lift_Opr_Name == user.DisplayName).ToList();
                        //  //  prjs = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date <= nxtyr).ToList();
                        //        ////prjs.Union()
                        //}
                        //else if (user.ServiceType == "SP-PASISCM")
                        //{

                        //    //prjs = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date <= lastdate).ToList();

                        //    //List<ProjectMasterViewModel> newprjvm = new List<ProjectMasterViewModel>();
                        //    //foreach (var pr in prjs)
                        //    //{
                        //    //       if (pr.Month_End_Date.Value.Month != 12 && pr.Month_End_Date.Value.Month != 6)
                        //    //       {
                        //    //        if (pasrs_schoolids.Contains(pr.SchoolID ?? default(int)))
                        //    //            newprjvm.Add(
                        //    //                new ProjectMasterViewModel()
                        //    //                {
                        //    //                    PrjMasID = pr.PrjMasID,
                        //    //                    SchoolID = pr.SchoolID,
                        //    //                    MonthName= pr.MonthName,
                        //    //                    CLTMID= pr.CLTMID,
                        //    //                    CLType= pr.CLType,
                        //    //                    Frequency =  pr.Frequency,
                        //    //                    Is_Completed =  pr.Is_Completed,
                        //    //                    Is_Rescheduled =  pr.Is_Rescheduled,
                        //    //                    Lift_Count = pr.Lift_Count,
                        //    //                    Lift_Opr_Name = pr.Lift_Opr_Name,                                               
                        //    //                    Month_End_Date = pr.Month_End_Date,
                        //    //                    Month_Start_Date = pr.Month_Start_Date,
                        //    //                    Rescheduled_Date =  pr.Rescheduled_Date,                                                
                        //    //                    Rescheduled_Remarks = pr.Rescheduled_Remarks
                        //    //                }
                        //    //                );
                        //    //        }
                        //    //    else
                        //    //    {
                        //    //        newprjvm.Add(
                        //    //               new ProjectMasterViewModel()
                        //    //               {
                        //    //                   PrjMasID = pr.PrjMasID,
                        //    //                   SchoolID = pr.SchoolID,
                        //    //                   MonthName = pr.MonthName,
                        //    //                   CLTMID = pr.CLTMID,
                        //    //                   CLType = pr.CLType,
                        //    //                   Frequency = pr.Frequency,
                        //    //                   Is_Completed = pr.Is_Completed,
                        //    //                   Is_Rescheduled = pr.Is_Rescheduled,
                        //    //                   Lift_Count = pr.Lift_Count,
                        //    //                   Lift_Opr_Name = pr.Lift_Opr_Name,
                        //    //                   Month_End_Date = pr.Month_End_Date,
                        //    //                   Month_Start_Date = pr.Month_Start_Date,
                        //    //                   Rescheduled_Date = pr.Rescheduled_Date,
                        //    //                   Rescheduled_Remarks = pr.Rescheduled_Remarks
                        //    //               }
                        //    //               );
                        //    //    }
                        //    //}
                        //    //prjs = newprjvm;                            
                        //    //if (curmon != 12 && curmon != 6)
                        //    //{
                        //    //    var nxthfyr = AppSettings.GetNextHalfYearly(lastdate);
                        //    //    var prjsB = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date == nxthfyr).ToList();
                        //    //    prjs.AddRange(prjsB);                                
                        //    //} 

                        //}
                        //else if (user.ServiceType == "SP-HVLSFMPIS")
                        //{
                        ////    prjs = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date <= lastdate).ToList();

                        ////    if (curmon != 12)
                        ////    {
                        ////        var nxthfyr = AppSettings.GetNextHalfYearly(lastdate);
                        ////        var prjsB = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date == nxthfyr).ToList();
                        ////        prjs.AddRange(prjsB);

                        ////    }
                        //}
                        //else if (user.ServiceType == "SP-FUMECESYS")
                        //{
                        //    //prjs = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date <= lastdate).ToList();

                        //    //if (curmon != 12)
                        //    //{
                        //    //    var nxthfyr = AppSettings.GetNextHalfYearly(lastdate);
                        //    //    var prjsB = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date == nxthfyr).ToList();
                        //    //    prjs.AddRange(prjsB);

                        //    //}
                        //}
                        //else
                        //{
                        //    //prjs = erpService.GetAllProject_SchoolID(newList, user.ServiceType).Where(a => a.Month_End_Date <= lastdate).ToList();
                        //   // prjs = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date <= lastdate).ToList();
                        //}


                        //var chkLists = erpService.GetAllSubmittedCheckList_School(newList).ToList();

                        if (user.ServiceType == "SP-LIFTSYS")
                        {
                            newCL = erpService.GetUserChecklistIndex(user.CompanyID ?? default(int), usrid, firstdate, lastdate).Where(a => a.Status != "Completed").ToList();
                        }
                        else if (user.ServiceType == "SP-SECUTYSYS")
                        {
                            var nxtqtr = AppSettings.GetNextQuartelry(lastdate);
                            newCL = erpService.GetUserChecklistIndex(user.CompanyID ?? default(int), usrid, firstdate, nxtqtr).Where(a => a.Status != "Completed").ToList();
                        }
                        else if (user.ServiceType == "SP-WETCHSUSYS")
                        {
                            var nxtyr = AppSettings.GetNextYearly(lastdate);
                            newCL = erpService.GetUserChecklistIndex(user.CompanyID ?? default(int), usrid, firstdate, nxtyr).ToList();
                        }
                        else if (user.ServiceType == "SP-CHILLERSYS")
                        {
                            newCL = erpService.GetUserChecklistIndex(user.CompanyID ?? default(int), usrid, firstdate, lastdate).ToList();
                        }
                        else if (user.ServiceType == "SP-HVLSFMPIS")
                        {
                            newCL = erpService.GetUserChecklistIndex(user.CompanyID ?? default(int), usrid, firstdate, lastdate).ToList();
                            if (curmon != 12)
                            {
                                var nxthfyr = AppSettings.GetNextHalfYearly(lastdate);
                                var prjsB = erpService.GetUserChecklistIndex_Future(user.CompanyID ?? default(int), usrid, firstdate, nxthfyr).ToList();
                                newCL.AddRange(prjsB);
                            }
                        }
                        else if (user.ServiceType == "SP-FUMECESYS")
                        {
                            newCL = erpService.GetUserChecklistIndex(user.CompanyID ?? default(int), usrid, firstdate, lastdate).ToList();
                            if (curmon != 12)
                            {
                                var nxthfyr = AppSettings.GetNextHalfYearly(lastdate);
                                var prjsB = erpService.GetUserChecklistIndex_Future(user.CompanyID ?? default(int), usrid, firstdate, nxthfyr).ToList();
                                newCL.AddRange(prjsB);
                            }
                        }
                        else if (user.ServiceType == "SP-PASISCM")
                        {
                            newCL = erpService.GetUserChecklistIndex(user.CompanyID ?? default(int), usrid, firstdate, lastdate).ToList();
                            if (curmon != 12 && curmon != 6)
                            {
                                var nxthfyr = AppSettings.GetNextHalfYearly(lastdate);
                                var prjsB = erpService.GetUserChecklistIndex_Future(user.CompanyID ?? default(int), usrid, firstdate, nxthfyr).ToList();
                                newCL.AddRange(prjsB);
                            }


                            //var chkLists = erpService.GetAllSubmittedCheckList_CLType(newList, user.ServiceType).ToList();
                            //var leftObj = (from a in prjs
                            //               join b in chkLists on a.PrjMasID equals b.PrjMasID
                            //                into pcTable
                            //               from c in pcTable.DefaultIfEmpty()
                            //               select new { a.PrjMasID, a.SchoolID, a.Frequency, a.Month_End_Date, c }).ToList();

                            //foreach (var obj in leftObj)
                            //{
                            //    var sch = erpService.GetSchool(obj.SchoolID ?? default(int));
                            //    var sts = "";
                            //    if (obj.c != null)
                            //    {
                            //        if (obj.c.CLTMID != 0)
                            //        {
                            //            if (obj.c.UserID == usrid || user.Designation == "TL")
                            //            {
                            //                if (obj.c.CheckedBy_Signature == null)
                            //                {
                            //                    sts = "Initiated";
                            //                }
                            //                else if (obj.c.EndoresBy_Signature == null)
                            //                {
                            //                    sts = "Pending User";
                            //                }
                            //                else if (obj.c.VerifiedBy_Signature == null)
                            //                {
                            //                    sts = "Pending MA";
                            //                }
                            //                else
                            //                {
                            //                    sts = "Completed";
                            //                }

                            //                if (obj.c.VerifiedBy_Signature == null && obj.c.EndoresBy_Signature == null)
                            //                {
                            //                    sts = "Pending User & MA";
                            //                }
                            //                newCL.Add(new CheckListIndexViewModel()
                            //                {
                            //                    CLTMID = obj.c.CLTMID,
                            //                    PrjMasID = obj.PrjMasID,
                            //                    SchoolID = obj.c.SchoolID.Value,
                            //                    Status = sts,
                            //                    Zone = sch.Zone,
                            //                    SchoolName = sch.School_Name,
                            //                    Frequency = obj.Frequency,
                            //                    CLType = user.ServiceType,
                            //                    Month_Name = obj.Month_End_Date.Value.ToString("MMMM yyyy")

                            //                });
                            //            }

                            //        }

                            //    }
                            //    else
                            //    {
                            //        sts = "New Submit";
                            //        newCL.Add(new CheckListIndexViewModel()
                            //        {
                            //            CLTMID = 0,
                            //            PrjMasID = obj.PrjMasID,
                            //            SchoolID = obj.SchoolID.Value,
                            //            Status = sts,
                            //            Zone = sch.Zone,
                            //            SchoolName = sch.School_Name,
                            //            Frequency = obj.Frequency,
                            //            CLType = user.ServiceType,
                            //            Month_Name = obj.Month_End_Date.Value.ToString("MMMM yyyy")
                            //        });
                            //    }
                            //}

                        }
                        else
                        {
                            newCL = erpService.GetUserChecklistIndex(user.CompanyID ?? default(int), usrid, firstdate, lastdate).Where(a => a.Status != "Completed").ToList();
                        }
                    }

                }

                return View(newCL);
            }
            catch (Exception ex)
            {
                logger.Debug("ChkList Index:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return View();
            }
        }


        public ActionResult ChecklistAdminIndex()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetAdminCheckList()
        {
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                var grpid = AppSession.GetCurrentUserGroup();
                List<CheckListIndexViewModel> newCL = new List<CheckListIndexViewModel>();

                //Admin or director group id
                if (grpid == 2)
                {
                    //List<int> schoolids = new List<int>();
                    //var schools = erpService.GetAllSchools().ToList();
                    //schoolids = schools.Where(a => a.CompanyID == user.CompanyID).Select(a => a.SchoolID).ToList();

                    //var liftschoolids = erpService.GetApplicableSchoolIDs(schoolids, user.ServiceType, user.UserID).ToList();
                    var now = DateTime.Now;
                    var firstdate = new DateTime(now.Year, now.Month, 1);
                    var premonth_firstdate = firstdate.AddMonths(-1);
                    var lastdate = firstdate.AddMonths(1).AddDays(-1);
                    //var prjs = erpService.GetAllProjectsBasedSchoolIDs(schoolids, lastdate).Where(a => a.Month_Start_Date >= premonth_firstdate && a.Month_End_Date <= lastdate).ToList();

                    //List<int> newList = liftschoolids.Where(v => v != null).Select(v => v.Value).ToList();
                    //var chkLists = erpService.GetAllSubmittedCheckList_School(schoolids).Where(a => a.CheckedBy_Signature != null && a.CLMonth >= premonth_firstdate).ToList();

                    //var leftObj = (from a in prjs
                    //               join b in chkLists on a.PrjMasID equals b.PrjMasID
                    //                into pcTable
                    //               from c in pcTable.DefaultIfEmpty()
                    //               select new { a.PrjMasID, a.SchoolID, a.Frequency, a.Month_End_Date, a.CLType, c }).ToList();

                    //foreach (var chl in leftObj)
                    //{
                    //    var sch = schools.Where(a => a.SchoolID == chl.SchoolID).FirstOrDefault();
                    //    var sts = "";
                    //    if (chl.c != null)
                    //    {
                    //        if (chl.c.EndoresBy_Signature == null)
                    //        {
                    //            sts = "Pending User";
                    //        }
                    //        else if (chl.c.VerifiedBy_Signature == null)
                    //        {
                    //            sts = "Pending MA";
                    //        }
                    //        else
                    //        {
                    //            sts = "Completed";
                    //        }

                    //        if (chl.c.VerifiedBy_Signature == null && chl.c.EndoresBy_Signature == null)
                    //        {
                    //            sts = "Pending User & MA";
                    //        }

                    //        //var sch = schools.Where(a => a.SchoolID == chl.SchoolID).FirstOrDefault();
                    //        var projmas = erpService.GetProject(chl.c.PrjMasID ?? default(int)).Month_End_Date;
                    //        newCL.Add(new CheckListIndexViewModel()
                    //        {
                    //            CLTMID = chl.c.CLTMID,
                    //            PrjMasID = chl.c.PrjMasID.Value,
                    //            SchoolID = chl.c.SchoolID.Value,
                    //            Status = sts,
                    //            Zone = sch.Zone,
                    //            SchoolName = sch.School_Name,
                    //            Frequency = chl.c.Frequency,
                    //            CLType = chl.c.CLType,
                    //            Month_Name = projmas.Value.ToString("MMMM yyyy")
                    //        });
                    //    }
                    //    else
                    //    {
                    //        sts = "New Submit";
                    //        newCL.Add(new CheckListIndexViewModel()
                    //        {
                    //            CLTMID = 0,
                    //            PrjMasID = chl.PrjMasID,
                    //            SchoolID = chl.SchoolID.Value,
                    //            Status = sts,
                    //            Zone = sch.Zone,
                    //            SchoolName = sch.School_Name,
                    //            Frequency = chl.Frequency,
                    //            CLType = chl.CLType,
                    //            Month_Name = chl.Month_End_Date.Value.ToString("MMMM yyyy")
                    //        });
                    //    }
                    //}

                    //newCL = newCL.Where(a => a.CLType != "SP-FIREALMSYS" && a.CLType != "SP-PLCWDWL" && a.CLType != "SP-HYDPUMP" && a.CLType != "SP-FIRESPNKLR" && a.CLType != "SP-SMKCTLSYS" && a.CLType != "SP-GENSET" && a.CLType != "SP_SIOCDSYS").ToList();
                    if (user.CompanyID == 1 || user.CompanyID == 2)
                        newCL = erpService.GetAdminChecklistIndex(user.CompanyID ?? default(int), firstdate, lastdate).Where(a => a.CLType != "SP-FIREALMSYS" && a.CLType != "SP-PLCWDWL" && a.CLType != "SP-HYDPUMP" && a.CLType != "SP-FIRESPNKLR" && a.CLType != "SP-SMKCTLSYS" && a.CLType != "SP-GENSET" && a.CLType != "SP_SIOCDSYS").ToList();
                    else
                        newCL = erpService.GetAdminChecklistIndex(user.CompanyID ?? default(int), firstdate, lastdate).ToList();
                }

                //Project Manager or M&E
                if (grpid == 6 || grpid == 8)
                {
                    //var sts = "";
                    //List<int> schoolids = new List<int>();
                    //var now = DateTime.Now;
                    //var firstdate = new DateTime(now.Year, now.Month, 1);
                    //var premonth_firstdate = firstdate.AddMonths(-1);
                    //var lastdate = firstdate.AddMonths(1).AddDays(-1);

                    //var schools = erpService.GetAllSchools().ToList();
                    //schoolids = schools.Where(a => a.Zone == user.Zone && a.CompanyID == user.CompanyID).Select(a => a.SchoolID).ToList();
                    //var chkLists = erpService.GetAllSubmittedCheckList_School(schoolids).Where(a => a.CheckedBy_Signature != null && a.CLMonth >= premonth_firstdate).ToList();
                    //foreach (var chl in chkLists)
                    //{
                    //    if (chl.EndoresBy_Signature == null)
                    //    {
                    //        sts = "Pending User";
                    //    }
                    //    else if (chl.VerifiedBy_Signature == null)
                    //    {
                    //        sts = "Pending MA";
                    //    }
                    //    else
                    //    {
                    //        sts = "Completed";
                    //    }

                    //    if (chl.VerifiedBy_Signature == null && chl.EndoresBy_Signature == null)
                    //    {
                    //        sts = "Pending User & MA";
                    //    }

                    //    var sch = schools.Where(a => a.SchoolID == chl.SchoolID).FirstOrDefault();
                    //    var projmas = erpService.GetProject(chl.PrjMasID ?? default(int)).Month_End_Date;
                    //    newCL.Add(new CheckListIndexViewModel()
                    //    {
                    //        CLTMID = chl.CLTMID,
                    //        PrjMasID = chl.PrjMasID.Value,
                    //        SchoolID = chl.SchoolID.Value,
                    //        Status = sts,
                    //        Zone = sch.Zone,
                    //        SchoolName = sch.School_Name,
                    //        Frequency = chl.Frequency,
                    //        CLType = chl.CLType,
                    //        Month_Name = projmas.Value.ToString("MMMM yyyy")
                    //    });

                    //}

                    newCL = erpService.GetMASearchChecklistIndex(user.CompanyID ?? default(int), user.Zone, user.GroupID ?? default(int));

                }

                //MA
                if (grpid == 5)
                {
                    //var chk = "";
                    //var tst = 0;

                    //var sts = "";
                    //List<int> schoolids = new List<int>();
                    //var now = DateTime.Now;
                    //var firstdate = new DateTime(now.Year, now.Month, 1);
                    //var premonth_firstdate = firstdate.AddMonths(-3);
                    //var lastdate = firstdate.AddMonths(1).AddDays(-1);

                    //var schools = erpService.GetAllSchools().ToList();
                    //schoolids = schools.Where(a => a.Zone == user.Zone && a.CompanyID == user.CompanyID).Select(a => a.SchoolID).ToList();
                    ////var chkLists = erpService.GetAllSubmittedCheckList_School(schoolids).Where(a => a.CheckedBy_Signature != null && a.CLMonth >= premonth_firstdate).ToList();
                    //var chkLists = erpService.GetAllSubmittedCheckList_School(schoolids).Where(a => a.VerifiedBy_Signature == null && a.CLMonth >= premonth_firstdate).OrderBy(a=>a.CLMonth).ToList();

                    ////logger.Info(chkLists.Count);                    

                    //foreach (var chl in chkLists)
                    //{
                    //    //tst++;

                    //    if (chl.EndoresBy_Signature == null)
                    //    {
                    //        sts = "Pending User";
                    //    }
                    //    else if (chl.VerifiedBy_Signature == null)
                    //    {
                    //        sts = "Pending MA";
                    //    }
                    //    else
                    //    {
                    //        sts = "Completed";
                    //    }

                    //    if (chl.VerifiedBy_Signature == null && chl.EndoresBy_Signature == null)
                    //    {
                    //        sts = "Pending User & MA";
                    //    }



                    //    //chk = tst.ToString() + "-" + chl.CLTMID;
                    //    //logger.Info(chk);

                    //    if (sts == "Pending MA" || sts == "Pending User & MA")
                    //    {
                    //        var sch = schools.Where(a => a.SchoolID == chl.SchoolID).FirstOrDefault();
                    //        var projmas = erpService.GetProject(chl.PrjMasID ?? default(int)).Month_End_Date;

                    //        newCL.Add(new CheckListIndexViewModel()
                    //        {
                    //            CLTMID = chl.CLTMID,
                    //            PrjMasID = chl.PrjMasID.Value,
                    //            SchoolID = chl.SchoolID.Value,
                    //            Status = sts,
                    //            Zone = sch.Zone,
                    //            SchoolName = sch.School_Name,
                    //            Frequency = chl.Frequency,
                    //            CLType = chl.CLType,
                    //            Month_Name = projmas.Value.ToString("MMMM yyyy")
                    //        });
                    //    }

                    //}

                    newCL = erpService.GetMASearchChecklistIndex(user.CompanyID ?? default(int), user.Zone, user.GroupID ?? default(int));

                }

                //newCL = newCL.Take(60).ToList();

                return Json(new { value = "OK", data = newCL }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Debug("ChkList Admin Index:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return View();
            }
        }



        public ActionResult SubmitChecklist(int PrjMasID, int SchoolID, string Frequency)
        {
            var usrid = AppSession.GetCurrentUserId();
            var user = userService.GetUser(usrid);
            var servicetype = user.ServiceType;
            var prj = erpService.GetProject(PrjMasID);

            //string[] elecArr = { "Electrical", "SP-FIREALMSYS", "SP-HVLSFMPIS", "SP-SMKCTLSYS", "SP-GENSET", "SP-SIOCDSYS" };
            string[] elecArr = { "Electrical", "SP-FIREALMSYS", "SP-SMKCTLSYS", "SP-GENSET", "SP-SIOCDSYS" };
            string[] mechArr = { "Mechanical", "SP-PLCWDWL", "SP-HYDPUMP", "SP-FUMECESYS", "SP_FIRESPNKLR" };


            var chklst = new List<CheckListItemMasterViewModel>();
            var school = erpService.GetSchool(SchoolID);
            ViewBag.SchoolName = school.Building_Name;
            ViewBag.Address = school.Address;
            ViewBag.Tel = school.Mobile;
            ViewBag.OPMngr = school.Op_Manager_Name;
            ViewBag.Email = school.Email;
            if (user.ServiceType == "SP-LIFTSYS")
            {
                ViewBag.LIFT_count = erpService.GetLiftCount(SchoolID, usrid, prj.Month_Start_Date ?? default(DateTime));
            }
            //ViewBag.LIFT_count = school.LIFT_count;
            ViewBag.HVLSFan_count = school.HVLSFan_count;
            ViewBag.CHILLER_count = school.CHILLER_count;
            ViewBag.Technician_Name = user.DisplayName;

            var formName = "";
            var title = "";
            if (servicetype == "Electrical")
            {
                formName = "Electrical-" + Frequency;
                title = servicetype + " & Domestic";
                var applicCL = erpService.GetProjectsForApplicableCLs(SchoolID, prj.Month_Start_Date ?? default(DateTime)).Where(a => elecArr.Contains(a.CLType)).Select(a => a.CLType).ToList();
                if (user.CompanyID == 3)
                {
                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && a.ServiceType == user.ServiceType).OrderBy(a => a.OrderBy).ToList();
                }
                else
                {
                    //chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && applicCL.Contains(a.ServiceType)).OrderBy(a => a.OrderBy).ToList();
                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName.StartsWith("Electrical") && applicCL.Contains(a.ServiceType) && (a.Frequency == Frequency || a.Frequency == null)).OrderBy(a => a.OrderBy).ToList();
                }
            }
            else if (servicetype == "Mechanical")
            {
                formName = "Mechanical-" + Frequency;
                title = servicetype;
                var applicCL = erpService.GetProjectsForApplicableCLs(SchoolID, prj.Month_Start_Date ?? default(DateTime)).Where(a => mechArr.Contains(a.CLType)).Select(a => a.CLType).ToList();

                if (user.CompanyID == 3)
                {
                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && a.ServiceType == user.ServiceType).OrderBy(a => a.OrderBy).ToList();
                }
                else
                {
                    //chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && applicCL.Contains(a.ServiceType)).OrderBy(a => a.OrderBy).ToList();

                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName.StartsWith("Mechanical") && applicCL.Contains(a.ServiceType) && (a.Frequency == Frequency || a.Frequency == null)).OrderBy(a => a.OrderBy).ToList();
                }
            }
            else
            {
                formName = servicetype;
                title = "Specialist";
                if (user.CompanyID == 3)
                {
                    chklst = erpService.GetAllCheckListItems().Where(a => a.ServiceType == user.ServiceType && (a.Frequency == Frequency || a.Frequency == null)).OrderBy(a => a.OrderBy).ToList();
                }
                else
                {
                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && (a.Frequency == Frequency || a.Frequency == null)).OrderBy(a => a.OrderBy).ToList();
                }

            }

            var Title = Frequency + " Maintenance Check List for " + title + " System [" + prj.MonthName + " " + Convert.ToDateTime(prj.Month_Start_Date).ToString("yyyy") + "]";
            ViewBag.Title = Title;

            ViewBag.CheckListItems = chklst;
            var clm = new CheckListTransMasterViewModel();
            clm.Frequency = Frequency;
            clm.CLType = user.ServiceType;
            clm.UserID = user.UserID;
            clm.SchoolID = SchoolID;
            clm.PrjMasID = PrjMasID;

            return View(clm);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult SubmitCheckList(CheckListTransMasterViewModel inputCLTM)
        {
            string halfPath = "/images/CheckListFiles/" + inputCLTM.UserID + "/";

            inputCLTM.Status_Flag = 0;
            inputCLTM.CreatedDate = DateTime.Now;



            var user = userService.GetUser(inputCLTM.UserID ?? default(int));
            if (user.ServiceType != "SP-LIFTSYS")
            {
                var submitFlag = erpService.CheckCLAlreadySubmitted(inputCLTM.PrjMasID ?? default(int), inputCLTM.SchoolID ?? default(int));
                if (submitFlag)
                {
                    return getFailedOperation("CheckList Already submitted");
                }
            }

            var prj = erpService.GetProject(inputCLTM.PrjMasID ?? default(int));
            inputCLTM.CLMonth = prj.Month_Start_Date;

            //inputCLTM.CheckedBy_Name = user.DisplayName;
            var result = erpService.SubmitChecklist(inputCLTM, inputCLTM.clTrnDetail, Path.Combine(Server.MapPath("~/images/CheckListFiles/" + AppSession.GetCurrentUserId() + "/")), halfPath);
            //var result = 2;
            if (result > 0)
            {
                var schoolname = erpService.GetSchool(inputCLTM.SchoolID ?? default(int)).Building_Name;
                if (user.ServiceType == "Electrical" || user.ServiceType == "Mechanical")
                {
                    var ADdeviceid = userService.GetUser(user.Admin_ID ?? default(int)).DeviceID;
                    var PMdeviceid = userService.GetUser(user.ProjMngr_ID ?? default(int)).DeviceID;
                    var msg = "New checklist has been submitted for " + schoolname + "(" + inputCLTM.Frequency + ")";

                    if (ADdeviceid != "" && ADdeviceid != null)
                    {
                        var resid = AppSettings.SendPushNotification(ADdeviceid, msg);
                    }
                    if (PMdeviceid != "" && PMdeviceid != null)
                    {
                        var resid = AppSettings.SendPushNotification(PMdeviceid, msg);
                    }
                    if (user.DeviceID != "" && user.DeviceID != null)
                    {
                        var usfid = AppSettings.SendPushNotification(user.DeviceID, msg);
                    }
                }


                return getSuccessfulOperation("OK", result.ToString());
            }
            else
            {
                return getFailedOperation();
            }

        }

        public ActionResult EditChecklist(int id)
        {
            //string[] elecArr = { "Electrical", "SP-FIREALMSYS", "SP-HVLSFMPIS", "SP-SMKCTLSYS", "SP-GENSET", "SP-SIOCDSYS" };
            string[] elecArr = { "Electrical", "SP-FIREALMSYS", "SP-SMKCTLSYS", "SP-GENSET", "SP-SIOCDSYS" };
            string[] mechArr = { "Mechanical", "SP-PLCWDWL", "SP-HYDPUMP", "SP-FUMECESYS", "SP_FIRESPNKLR" };
            var chklst = new List<CheckListItemMasterViewModel>();
            //var cpyid = AppSession.GetCompanyId();
            var formName = "";
            var clm = erpService.GetCheckListItems(id);
            var prj = erpService.GetProject(clm.PrjMasID ?? default(int));
            var user = userService.GetUser(clm.UserID ?? default(int));

            if (clm.CLType == "Electrical")
            {
                formName = "Electrical-" + clm.Frequency;
                var applicCL = erpService.GetProjectsForApplicableCLs(clm.SchoolID ?? default(int), prj.Month_Start_Date ?? default(DateTime)).Where(a => elecArr.Contains(a.CLType)).Select(a => a.CLType).ToList();
                //chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && applicCL.Contains(a.ServiceType)).OrderBy(a => a.OrderBy).ToList();
                if (user.CompanyID == 3)
                {
                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && a.ServiceType == user.ServiceType).OrderBy(a => a.OrderBy).ToList();
                }
                else
                {
                    //chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && applicCL.Contains(a.ServiceType)).OrderBy(a => a.OrderBy).ToList();
                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName.StartsWith("Electrical") && applicCL.Contains(a.ServiceType) && (a.Frequency == clm.Frequency || a.Frequency == null)).OrderBy(a => a.OrderBy).ToList();
                }
            }
            else if (clm.CLType == "Mechanical")
            {
                formName = "Mechanical-" + clm.Frequency;
                var applicCL = erpService.GetProjectsForApplicableCLs(clm.SchoolID ?? default(int), prj.Month_Start_Date ?? default(DateTime)).Where(a => mechArr.Contains(a.CLType)).Select(a => a.CLType).ToList();
                //chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && applicCL.Contains(a.ServiceType)).OrderBy(a => a.OrderBy).ToList();
                if (user.CompanyID == 3)
                {
                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && a.ServiceType == user.ServiceType).OrderBy(a => a.OrderBy).ToList();
                }
                else
                {
                    //chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && applicCL.Contains(a.ServiceType)).OrderBy(a => a.OrderBy).ToList();
                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName.StartsWith("Mechanical") && applicCL.Contains(a.ServiceType) && (a.Frequency == clm.Frequency || a.Frequency == null)).OrderBy(a => a.OrderBy).ToList();
                }
            }
            else
            {
                formName = clm.CLType;
                chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName).OrderBy(a => a.OrderBy).ToList();
                if (user.CompanyID == 3)
                {
                    chklst = erpService.GetAllCheckListItems().Where(a => a.ServiceType == user.ServiceType && (a.Frequency == clm.Frequency || a.Frequency == null)).OrderBy(a => a.OrderBy).ToList();
                }
                else
                {
                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && (a.Frequency == clm.Frequency || a.Frequency == null)).OrderBy(a => a.OrderBy).ToList();
                }

            }


            var school = erpService.GetSchool(clm.SchoolID ?? default(int));
            ViewBag.SchoolName = school.Building_Name;
            ViewBag.Address = school.Address;
            ViewBag.Tel = school.Mobile;
            ViewBag.OPMngr = school.Op_Manager_Name;
            ViewBag.Email = school.Email;
            if (clm.CLType == "SP-LIFTSYS")
            {
                ViewBag.LIFT_count = erpService.GetLiftCount(clm.SchoolID ?? default(int), clm.UserID ?? default(int), prj.Month_Start_Date ?? default(DateTime));
            }
            //ViewBag.LIFT_count = school.LIFT_count;
            ViewBag.HVLSFan_count = school.HVLSFan_count;
            ViewBag.CHILLER_count = school.CHILLER_count;
            var title = "";
            if (clm.CLType == "Electrical")
            {
                title = clm.CLType + " & Domestic";
            }
            else if (clm.CLType == "Mechanical")
            {
                title = clm.CLType;
            }
            else
            {
                title = "Specialist";
            }

            var Title = clm.Frequency + " Maintenance Check List for " + title + " System [" + prj.MonthName + " " + Convert.ToDateTime(prj.Month_Start_Date).ToString("yyyy") + "]";
            ViewBag.Title = Title;

            foreach (var cli in clm.chklst_trn_detail)
            {
                var obj = chklst.Where(a => a.ChkListID == cli.ChkListID).SingleOrDefault();
                obj.IsDone = cli.IsDone;
                obj.DateDone = cli.DateDone;
                obj.Remarks = cli.Remarks;
                obj.CLTDID = cli.CLTDID;
            }

            //new SelectList(school, "SchoolID", "School_Name");

            ViewBag.CheckListItems = chklst;

            return View(clm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCheckList(CheckListTransMasterViewModel inputCLTM, List<CheckListTransDetailViewModel> clTrnDetail)
        {

            var result = erpService.EditChecklist(inputCLTM, clTrnDetail);

            var clm = erpService.GetCheckListItems(inputCLTM.CLTMID);
            if (clm.Status_Flag == 4)
            {
                var res = GeneratePDF(inputCLTM.CLTMID, 1);
            }
            if (clm.Status_Flag == 2)
            {
                var res = GeneratePDF(inputCLTM.CLTMID, 2);
            }

            //var result = 2;
            if (result > 0)
            {
                return getSuccessfulOperation("OK", result.ToString());
            }
            else
            {
                return getFailedOperation();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitSignatureCheckList(CheckListTransMasterViewModel inputCLTM)
        {

            var result = erpService.SubmitSignatureCheckList(inputCLTM);
            var clm = erpService.GetCheckListItems(inputCLTM.CLTMID);
            if (clm.Status_Flag == 4)
            {
                var res = GeneratePDF(inputCLTM.CLTMID, 3);
            }
            //var result = 2;
            if (result > 0)
            {
                return getSuccessfulOperation("OK", result.ToString());
            }
            else
            {
                return getFailedOperation();
            }

        }


        public ActionResult PrintCheckList(int id)
        {
            var cpyid = AppSession.GetCompanyId();
            var formName = "";
            var clm = erpService.GetCheckListItems(id);
            var chklst = new List<CheckListItemMasterViewModel>();

            var prjmon = erpService.GetProject(clm.PrjMasID ?? default(int)).Month_Start_Date;
            //string[] elecArr = { "Electrical", "SP-FIREALMSYS", "SP-HVLSFMPIS", "SP-SMKCTLSYS", "SP-GENSET", "SP-SIOCDSYS" };
            string[] elecArr = { "Electrical", "SP-FIREALMSYS", "SP-SMKCTLSYS", "SP-GENSET", "SP-SIOCDSYS" };
            string[] mechArr = { "Mechanical", "SP-PLCWDWL", "SP-HYDPUMP", "SP-FUMECESYS", "SP_FIRESPNKLR" };

            List<string> clArr = new List<string>();

            var title = "";
            var annex = "";
            var TitleH = "";

            if (clm.Frequency == "Monthly")
            {
                annex = "ANNEX D7_1";
            }
            if (clm.Frequency == "Quarterly")
            {
                annex = "ANNEX D7_2";
            }
            if (clm.Frequency == "HalfYearly")
            {
                annex = "ANNEX D7_3";
            }
            if (clm.Frequency == "Yearly")
            {
                annex = "ANNEX D7_4";
            }

            var spl = "";

            if (clm.CLType == "Electrical")
            {
                title = clm.CLType + " & Domestic";
                TitleH = "M & E INSPECTION FORM - " + clm.CLType + "(" + clm.Frequency + ")";
            }
            else if (clm.CLType == "Mechanical")
            {
                title = clm.CLType;
                TitleH = "M & E INSPECTION FORM - " + clm.CLType + "(" + clm.Frequency + ")";
            }
            else
            {
                title = "Specialist";
                annex = "ANNEX D7_5";
                TitleH = "M & E INSPECTION FORM (SPECIALIST)  " + spl;
            }

            if (clm.CLType == "Electrical")
            {
                var applicCL = erpService.GetProjectsForApplicableCLs(clm.SchoolID ?? default(int), prjmon ?? default(DateTime)).Where(a => elecArr.Contains(a.CLType)).ToList();
                foreach (var item in applicCL)
                {
                    clArr.Add(item.CLType);
                }
                formName = "Electrical-" + clm.Frequency;
            }
            else if (clm.CLType == "Mechanical")
            {
                var applicCL = erpService.GetProjectsForApplicableCLs(clm.SchoolID ?? default(int), prjmon ?? default(DateTime)).Where(a => mechArr.Contains(a.CLType)).ToList();
                foreach (var item in applicCL)
                {
                    clArr.Add(item.CLType);
                }
                formName = "Mechanical-" + clm.Frequency;
            }
            else
            {
                formName = clm.CLType;
            }
            var Prj = erpService.GetProject(clm.PrjMasID ?? default(int));
            var mon = Prj.Month_Start_Date.Value.ToString("MMMM yyyy");
            ViewBag.MonthName = mon;
            var user = userService.GetUser(clm.UserID ?? default(int));
            ViewBag.ContractorName = user.DisplayName;
            ViewBag.Annexure = annex;
            if (user.CompanyID == 3)
            {
                chklst = erpService.GetAllCheckListItems().Where(a => a.ServiceType == clm.CLType && (a.Frequency == clm.Frequency || a.Frequency == null)).OrderBy(a => a.OrderBy).ToList();
            }
            else
            {
                if (clm.CLType == "Electrical")
                {
                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName.StartsWith("Electrical") && (a.Frequency == clm.Frequency || a.Frequency == null) && clArr.Contains(a.ServiceType)).OrderBy(a => a.OrderBy).ToList(); //modified order by
                }
                else if (clm.CLType == "Mechanical")
                {
                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName.StartsWith("Mechanical") && (a.Frequency == clm.Frequency || a.Frequency == null) && clArr.Contains(a.ServiceType)).OrderBy(a => a.OrderBy).ToList(); //modified order by
                }
                else
                {
                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && (a.Frequency == clm.Frequency || a.Frequency == null)).OrderBy(a => a.OrderBy).ToList(); //modified order by
                }

            }



            var school = erpService.GetSchool(clm.SchoolID ?? default(int));
            ViewBag.SchoolName = school.Building_Name;
            ViewBag.Address = school.Address;
            ViewBag.Tel = school.Mobile;
            ViewBag.OPMngr = school.Op_Manager_Name;
            ViewBag.Email = school.Email;
            ViewBag.CompanyID = cpyid;

            if (clm.CLType == "SP-LIFTSYS")
            {
                ViewBag.LIFT_count = erpService.GetLiftCount(clm.SchoolID ?? default(int), clm.UserID ?? default(int), Prj.Month_Start_Date ?? default(DateTime));
            }
            //ViewBag.LIFT_count = school.LIFT_count;
            ViewBag.HVLSFan_count = school.HVLSFan_count;
            ViewBag.CHILLER_count = school.CHILLER_count;

            if (clm.CLType == "Electrical")
            {
                title = clm.CLType + " & Domestic";
            }
            else if (clm.CLType == "Mechanical")
            {
                title = clm.CLType;
            }
            else
            {
                title = "Specialist";
            }

            var Title = clm.Frequency + " Maintenance Check List for" + title + " System";
            ViewBag.Title = Title;

            foreach (var cli in clm.chklst_trn_detail)
            {
                var obj = chklst.Where(a => a.ChkListID == cli.ChkListID).SingleOrDefault();
                obj.IsDone = cli.IsDone;
                obj.DateDone = cli.DateDone;
                obj.Remarks = cli.Remarks;
                obj.CLTDID = cli.CLTDID;
            }

            //new SelectList(school, "SchoolID", "School_Name");

            ViewBag.CheckListItems = chklst;

            return View(clm);
        }

        //public ActionResult CLSummaryMatrix()
        //{
        //    var userid = AppSession.GetCurrentUserId();            
        //    var lst = erpService.GetCurCLSummaryMatrix(userid);           
        //    return View(lst);
        //}


        public int GeneratePDF(int id, int mailflag)
        {
            try
            {
                var path = "";
                var clitem = erpService.GetCheckListItems(id);
                var prjmon = erpService.GetProject(clitem.PrjMasID ?? default(int)).Month_Start_Date;
                //string[] elecArr = { "Electrical", "SP-FIREALMSYS", "SP-HVLSFMPIS", "SP-SMKCTLSYS", "SP-GENSET", "SP-SIOCDSYS" };
                string[] elecArr = { "Electrical", "SP-FIREALMSYS", "SP-SMKCTLSYS", "SP-GENSET", "SP-SIOCDSYS" };
                string[] mechArr = { "Mechanical", "SP-PLCWDWL", "SP-HYDPUMP", "SP-FUMECESYS", "SP_FIRESPNKLR" };
                string mailAttach = "";

                var newfile = new ChecklistFileViewModel();
                newfile.Frequency = clitem.Frequency;
                newfile.Month_Start_Date = prjmon;
                newfile.SchoolID = clitem.SchoolID;

                var school = erpService.GetSchool(clitem.SchoolID ?? default(int));
                var project = erpService.GetProject(clitem.PrjMasID ?? default(int));

                if (school.CompanyID != 3)
                {
                    if (clitem.CLType == "Electrical")
                    {
                        var applicCL = erpService.GetProjectsForApplicableCLs(clitem.SchoolID ?? default(int), prjmon ?? default(DateTime)).Where(a => elecArr.Contains(a.CLType)).ToList();
                        //Select(a=>a.CLType)
                        foreach (var item in applicCL)
                        {
                            if (item.CLType == "Electrical")
                            {
                                path = Path.Combine(Server.MapPath("~/images/CLPdfFiles/"), "CL_" + id + ".pdf");
                                mailAttach = mailAttach + path + ";";
                            }
                            else
                            {
                                path = Path.Combine(Server.MapPath("~/images/CLPdfFiles/"), "CL_" + id + "_" + item.CLType + ".pdf");
                                mailAttach = mailAttach + path + ";";
                            }
                            newfile.FilePath = path;
                            newfile.CLType = item.CLType;
                            var actionPDF = new Rotativa.ActionAsPdf("PrintCheckListPdf", new { id = id, cltype = item.CLType })
                            {
                                PageMargins = new Rotativa.Options.Margins(20, 5, 20, 5),
                                PageSize = Rotativa.Options.Size.A4,
                                PageOrientation = Rotativa.Options.Orientation.Portrait
                            };

                            var applicationPDFData = actionPDF.BuildFile(ControllerContext);
                            var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                            fileStream.Write(applicationPDFData, 0, applicationPDFData.Length);
                            fileStream.Close();
                            if (mailflag == 1 || mailflag == 3)
                            {
                                var res = erpService.AddPdfFiles(newfile);
                            }

                        }

                    }
                    else if (clitem.CLType == "Mechanical")
                    {
                        var applicCL = erpService.GetProjectsForApplicableCLs(clitem.SchoolID ?? default(int), prjmon ?? default(DateTime)).Where(a => mechArr.Contains(a.CLType)).ToList();
                        foreach (var item in applicCL)
                        {
                            if (item.CLType == "Mechanical")
                            {
                                path = Path.Combine(Server.MapPath("~/images/CLPdfFiles/"), "CL_" + id + ".pdf");
                                mailAttach = mailAttach + path + ";";
                            }
                            else
                            {
                                path = Path.Combine(Server.MapPath("~/images/CLPdfFiles/"), "CL_" + id + "_" + item.CLType + ".pdf");
                                mailAttach = mailAttach + path + ";";
                            }
                            newfile.FilePath = path;
                            newfile.CLType = item.CLType;
                            var actionPDF = new Rotativa.ActionAsPdf("PrintCheckListPdf", new { id = id, cltype = item.CLType })
                            {
                                PageMargins = new Rotativa.Options.Margins(20, 5, 20, 5),
                                PageSize = Rotativa.Options.Size.A4,
                                PageOrientation = Rotativa.Options.Orientation.Portrait
                            };

                            var applicationPDFData = actionPDF.BuildFile(ControllerContext);
                            var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                            fileStream.Write(applicationPDFData, 0, applicationPDFData.Length);
                            fileStream.Close();
                            if (mailflag == 1 || mailflag == 3)
                            {
                                var res = erpService.AddPdfFiles(newfile);
                            }

                        }
                    }
                    else
                    {
                        path = Path.Combine(Server.MapPath("~/images/CLPdfFiles/"), "CL_" + id + ".pdf");
                        mailAttach = mailAttach + path + ";";
                        var actionPDF = new Rotativa.ActionAsPdf("PrintCheckListPdf", new { id = id, cltype = clitem.CLType })
                        {
                            PageMargins = new Rotativa.Options.Margins(20, 5, 20, 5),
                            PageSize = Rotativa.Options.Size.A4,
                            PageOrientation = Rotativa.Options.Orientation.Portrait
                        };
                        newfile.FilePath = path;
                        newfile.CLType = clitem.CLType;

                        var applicationPDFData = actionPDF.BuildFile(ControllerContext);
                        var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                        fileStream.Write(applicationPDFData, 0, applicationPDFData.Length);
                        fileStream.Close();
                        if (mailflag == 1 || mailflag == 3)
                        {
                            var res = erpService.AddPdfFiles(newfile);
                        }
                    }
                }
                else
                {
                    path = Path.Combine(Server.MapPath("~/images/CLPdfFiles/"), "CL_" + id + ".pdf");
                    mailAttach = mailAttach + path + ";";
                    var actionPDF = new Rotativa.ActionAsPdf("PrintCheckListPdf", new { id = id, cltype = clitem.CLType })
                    {
                        PageMargins = new Rotativa.Options.Margins(20, 5, 20, 5),
                        PageSize = Rotativa.Options.Size.A4,
                        PageOrientation = Rotativa.Options.Orientation.Portrait
                    };
                    newfile.FilePath = path;
                    newfile.CLType = clitem.CLType;

                    var applicationPDFData = actionPDF.BuildFile(ControllerContext);
                    var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                    fileStream.Write(applicationPDFData, 0, applicationPDFData.Length);
                    fileStream.Close();
                    if (mailflag == 1 || mailflag == 3)
                    {
                        var res = erpService.AddPdfFiles(newfile);
                    }
                }

                if (mailflag == 1 || mailflag == 2)
                {
                    var sub = "Sub: ";
                    var cpy = "";
                    var styp = "";
                    var to = "";
                    var cc = "";
                    var pmon = Convert.ToDateTime(prjmon).ToString("MMM yyyy");
                    if (clitem.CLType == "Electrical")
                    {
                        styp = "(E&D)";
                    }
                    else if (clitem.CLType == "Mechanical")
                    {
                        styp = "(Mechanical)";
                    }
                    else
                    {
                        styp = "(Specialist)";
                    }

                    //Sending mail
                    if (school.CompanyID == 1)
                    {
                        sub = sub + "MKV - Maintenance Inspection for the School: " + school.Building_Name + "-" + pmon + " " + styp;
                        cpy = "MKV Engineering & Trading Pte Ltd.";

                        if (school.Email != null && school.Email != "")
                        {
                            to = school.Email;
                        }
                        else
                        {
                            to = "arul@mkv.com.sg";
                        }

                        cc = "sbi@mkv.com.sg";

                    }
                    if (school.CompanyID == 2)
                    {
                        sub = sub + "HEC - Maintenance Inspection for the School: " + school.Building_Name + "-" + pmon + " " + styp;
                        cpy = "HEC Electrical & Construction Pte Ltd.";

                        if (school.Email != null && school.Email != "")
                        {
                            to = school.Email;
                        }
                        else
                        {
                            to = "arul@mkv.com.sg";
                        }
                        cc = "sbi@hec.com.sg";
                    }
                    if (school.CompanyID == 3)
                    {
                        sub = sub + "PPL - Maintenance Inspection for the School: " + school.Building_Name + "-" + pmon + " " + styp;
                        cpy = "PROPELL Integrated Pte Ltd.";

                        if (school.Email != null && school.Email != "")
                        {
                            to = school.Email;
                            //to = "project_moe@propell.com.sg";
                        }
                        else
                        {
                            to = "rskumar@timothy.com.sg";
                        }
                        cc = "project_moe@propell.com.sg";

                    }


                    var body = "Dear Sir/Madam, \n\nPlease find attached the checklist reports for the below detail.\n\n";


                    //sub = sub + " Maintenance Inspection Report for the School:" + school.School_Name;

                    body = body + " Type: " + clitem.CLType + ".\n";
                    body = body + " \n Frequency: " + clitem.Frequency + ".\n";
                    body = body + " Month: " + project.MonthName + " " + Convert.ToDateTime(project.Month_Start_Date).ToString("yyyy") + ".\n";

                    body = body + " \n\nPlease feel free to approach our manager if any clarification/concern.\n\n";
                    body = body + "Thanks & Regards,\n \n";
                    body = body + cpy;

                    body = body + "\n\n **Please don't reply to this email and this is auto generated email from software.** ";


                    //to = school.Email;

                    //to = to + ";" + "pandees@gmail.com";
                    //to = to + ";" + "sm.anandh@gmail.com";

                    //cc = cc + "," + "prabhu.thil@gmail.com";
                    //cc = "sowdambbikainfotechservices@gmail.com";
                    //cc = cc + "," + "rajeowens@gmail.com";

                    AppSettings.sendEmail(sub, body, to, cc, mailAttach, school.CompanyID ?? default(int));

                }

                return 1;
            }
            catch (Exception ex)
            {
                logger.Debug("Pdf Error1:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return 0;
            }
        }

        // GET: CheckList/Print/5
        public ActionResult PrintCheckListPdf(int id, string cltype)
        {

            var formName = "";
            var clm = erpService.GetCheckListItems(id);
            var chklst = new List<CheckListItemMasterViewModel>();

            var title = "";
            var annex = "";
            var TitleH = "";

            if (clm.Frequency == "Monthly")
            {
                annex = "ANNEX D7_1";
            }
            if (clm.Frequency == "Quarterly")
            {
                annex = "ANNEX D7_2";
            }
            if (clm.Frequency == "HalfYearly")
            {
                annex = "ANNEX D7_3";
            }
            if (clm.Frequency == "Yearly")
            {
                annex = "ANNEX D7_4";
            }

            var spl = "";
            if (cltype == "SP-LIFTSYS")
                spl = "LIFTS SYSTEM(PASSENGER, FIREMAN & PLATFORM, HOIST)";
            if (cltype == "SP-SECUTYSYS")
                spl = "SECURITY SYSTEMS (INCLUDING DECAMS)";
            if (cltype == "SP-FIREALMSYS")
                spl = "ALL TYPES OF FIRE ALARM SYSTEMS(INCLUDING DECAMS)";
            if (cltype == "SP-PLCWDWL")
                spl = "PROGRAMMABLE LOGIC CONTROLLER (WIRED & WIRELESS)";
            if (cltype == "SP-HYDPUMP")
                spl = "HYDRANT PUMPING SYSTEM";
            if (cltype == "SP-PASISCM")
                spl = "PUBLIC ADDRESS SYSTEM, ISCM and SOUND REINFORCEMENT SYSTEM";
            if (cltype == "SP-BASBMSSYS")
                spl = "BAS, BMS & MMS";
            if (cltype == "SP-CHILLERSYS")
                spl = "CHILLER SYSTEMS(WATER OR AIR - COOLED TYPES)";
            if (cltype == "SP-FUMECESYS")
                spl = "FUME CUPBOARD EXHAUST SYSTEMS(DUCT OR DUCTLESS)";
            if (cltype == "SP-FIRESPNKLR")
                spl = "FIRE SPRINKLER SYSTEMS";
            if (cltype == "SP-WETCHSUSYS")
                spl = "WET CHEMICAL SUPRESSION SYSTEMS";
            if (cltype == "SP-HVLSFMPIS")
                spl = "HIGH VOLUME LOW SPEED FAN(MPH & ISH)";

            if (cltype == "SP-SMKCTLSYS")
                spl = "SMOKE CONTROL SYSTEMS";
            if (cltype == "SP-OTHLIFTS")
                spl = "OTHER TYPES OF LIFT(CHAIRLIFT & DUMBWAITER)";
            if (cltype == "SP-GENSET")
                spl = "GENERATOR SET(EXCLUDING LIFT, SMOKE AND FIRE FIGHTING SYSTEMS)";
            if (cltype == "SP-SIOCDSYS")
                spl = "SERVICING & INSPECTION OF CIVIL DEFENCE SHELTERS";


            if (cltype == "Electrical")
            {
                title = cltype + " & Domestic";
                TitleH = "M & E INSPECTION FORM - " + cltype + "(" + clm.Frequency + ")";
            }
            else if (cltype == "Mechanical")
            {
                title = cltype;
                TitleH = "M & E INSPECTION FORM - " + cltype + "(" + clm.Frequency + ")";
            }
            else
            {
                title = "Specialist";
                annex = "ANNEX D7_5";
                TitleH = "M & E INSPECTION FORM (SPECIALIST) - " + spl;
            }



            if (clm.CLType == "Electrical")
            {
                formName = "Electrical-" + clm.Frequency;
            }
            else if (clm.CLType == "Mechanical")
            {
                formName = "Mechanical-" + clm.Frequency;
            }
            else
            {
                formName = clm.CLType;
            }

            var Prj = erpService.GetProject(clm.PrjMasID ?? default(int));
            var mon = Prj.Month_Start_Date.Value.ToString("MMMM yyyy");
            ViewBag.MonthName = mon;
            var user = userService.GetUser(clm.UserID ?? default(int));
            ViewBag.ContractorName = user.DisplayName;

            if (user.CompanyID == 3)
            {
                chklst = erpService.GetAllCheckListItems().Where(a => a.ServiceType == cltype && (a.Frequency == clm.Frequency || a.Frequency == null)).OrderBy(a => a.OrderBy).ToList();
            }
            else
            {
                if (clm.CLType == "Electrical")
                {
                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName.StartsWith("Electrical") && (a.Frequency == clm.Frequency || a.Frequency == null) && a.ServiceType == cltype).OrderBy(a => a.OrderBy).ToList(); // modified order  by
                }
                else if (clm.CLType == "Mechanical")
                {
                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName.StartsWith("Mechanical") && (a.Frequency == clm.Frequency || a.Frequency == null) && a.ServiceType == cltype).OrderBy(a => a.OrderBy).ToList(); // modified order  by
                }
                else
                {
                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && (a.Frequency == clm.Frequency || a.Frequency == null) && a.ServiceType == cltype).OrderBy(a => a.OrderBy).ToList(); // modified order  by
                }
                //chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && a.ServiceType == cltype && (a.Frequency == clm.Frequency || a.Frequency == null)).ToList();
            }

            var school = erpService.GetSchool(clm.SchoolID ?? default(int));
            ViewBag.SchoolName = school.Building_Name;
            ViewBag.Address = school.Address;
            ViewBag.Tel = school.Mobile;
            ViewBag.OPMngr = school.Op_Manager_Name;
            ViewBag.Email = school.Email;
            ViewBag.CompanyID = userService.GetUser(clm.UserID ?? default(int)).CompanyID;

            ViewBag.CLTYPE = cltype;

            if (clm.CLType == "SP-LIFTSYS")
            {
                ViewBag.LIFT_count = erpService.GetLiftCount(clm.SchoolID ?? default(int), clm.UserID ?? default(int), Prj.Month_Start_Date ?? default(DateTime));
            }
            //ViewBag.LIFT_count = school.LIFT_count;
            ViewBag.HVLSFan_count = school.HVLSFan_count;
            ViewBag.CHILLER_count = school.CHILLER_count;

            ViewBag.Title = TitleH;
            ViewBag.Annexure = annex;

            foreach (var cli in clm.chklst_trn_detail)
            {
                var obj = chklst.Where(a => a.ChkListID == cli.ChkListID).SingleOrDefault();
                if (obj != null)
                {
                    obj.IsDone = cli.IsDone;
                    obj.DateDone = cli.DateDone;
                    obj.Remarks = cli.Remarks;
                    obj.CLTDID = cli.CLTDID;
                }
            }

            //new SelectList(school, "SchoolID", "School_Name");

            ViewBag.CheckListItems = chklst;

            return View(clm);

        }

        public ActionResult MailChecklist(int id)
        {
            var userid = AppSession.GetCurrentUserId();
            var admin = userService.GetUser(userid);
            var sub = "Sub: Date";
            var body = "Dear Sir/Madam, \n\nPlease find attached the checklist report for the below.\n\n";
            var to = "";
            var cc = "";
            var att = "";

            sub = sub + " Maintenance Inspection Report for the School:";

            body = body + "\nPlease help to look into it and do the necessary. \n\nPlease feel free to approach our manager if any clarification/concern.\n\n";
            body = body + "Thanks\n \n Quality Inspection Team \n Citi Construction Pte Ltd";

            to = "anandh@rapidsystemz.com";

            //cc = "smanandh@gmx.com";
            //cc = cc + "," + "prabhu.thil@gmail.com";
            cc = "sowdambbikainfotechservices@gmail.com";
            //cc = cc + "," + "pandees@gmail.com";
            //cc = cc + "," + "sivatcbm@yahoo.com.sg";


            var result = 1;

            AppSettings.sendEmail(sub, body, to, cc, att, 2);

            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }


        }


        #endregion

        #region Service Form
        public ActionResult ServiceFormIndex()
        {
            var usrid = AppSession.GetCurrentUserId();
            var usergrp = AppSession.GetCurrentUserGroup();

            List<ServiceFormViewModel> svfvm = new List<ServiceFormViewModel>();
            if (usergrp == 3)
            {
                svfvm = erpService.GetAllServiceForms().Where(a => a.UserID == usrid).ToList();
            }
            if (usergrp == 8)
            {
                svfvm = erpService.GetAllServiceForms().Where(a => a.AssignedBy == usrid).ToList();
            }
            var school = erpService.GetAllSchools().ToList();
            var users = userService.getAllUsers().ToList();

            foreach (var sfitem in svfvm)
            {
                var obj = school.Where(a => a.BuildingID == sfitem.SchoolID).SingleOrDefault();
                var tec = users.Where(a => a.UserID == sfitem.UserID).SingleOrDefault();
                var adm = users.Where(a => a.UserID == sfitem.AssignedBy).SingleOrDefault();
                sfitem.SchoolName = obj.Building_Name;
                sfitem.Zone = obj.Zone;
                sfitem.Technician_Name = tec.DisplayName;
                sfitem.Assigned_Name = adm.DisplayName;

                if (sfitem.Status_Flag == 0)
                {
                    sfitem.Status = "Assigned";
                }
                if (sfitem.Status_Flag == 1)
                {
                    sfitem.Status = "Pending";
                }
                if (sfitem.Status_Flag == 2)
                {
                    sfitem.Status = "Completed";
                }
            }

            return View(svfvm);
        }

        public ActionResult AssignService()
        {
            var userid = AppSession.GetCurrentUserId();
            var admin = userService.GetUser(userid);
            var school = erpService.GetAllSchools().Where(a => a.IsActive == 1 && a.Zone == admin.Zone).ToList();
            ViewBag.School = school;
            var obj = school.Select(a => a.School_Type).Distinct().ToList();
            ViewBag.SchoolGroup = obj;

            var sengs = userService.getAllUsers().Where(a => a.Zone == admin.Zone && a.GroupID == 3 && a.Admin_ID == userid).ToList();
            ViewBag.ServiceEngineers = sengs;
            var serviceType = sengs.Select(a => a.ServiceType).Distinct().ToList();
            ViewBag.ServiceType = serviceType;
            return View();
        }


        public ActionResult NewServiceForm()
        {
            var userid = AppSession.GetCurrentUserId();
            var user = userService.GetUser(userid);
            var school = erpService.GetAllSchools().Where(a => a.IsActive == 1 && a.Zone == user.Zone).ToList();
            ViewBag.School = school;
            var obj = school.Select(a => a.School_Type).Distinct().ToList();
            ViewBag.SchoolGroup = obj;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignService(ServiceFormViewModel inputSF)
        {
            inputSF.AssignedBy = AppSession.GetCurrentUserId();
            inputSF.Status_Flag = 0;
            var result = erpService.AssignService(inputSF);
            //var result = 2;
            if (result > 0)
            {
                return getSuccessfulOperation("OK", result.ToString());
            }
            else
            {
                return getFailedOperation();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewServiceForm(ServiceFormViewModel inputSF)
        {
            inputSF.UserID = AppSession.GetCurrentUserId();
            inputSF.AssignedBy = userService.GetUser(inputSF.UserID ?? default(int)).Admin_ID;
            inputSF.Status_Flag = 1;
            var result = erpService.AssignService(inputSF);
            //var result = 2;
            if (result > 0)
            {
                return getSuccessfulOperation("OK", result.ToString());
            }
            else
            {
                return getFailedOperation();
            }

        }

        public ActionResult ServiceFormSave(int id)
        {
            var svform = erpService.GetServiceForm(id);
            var school = erpService.GetSchool(svform.SchoolID ?? default(int));
            svform.SchoolName = school.Building_Name;
            svform.Address = school.Address;

            var user = userService.GetUser(svform.UserID ?? default(int));
            ViewBag.TechName = user.DisplayName;
            return View(svform);
        }

        public ActionResult ChecklistFormat()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ServiceFormSave(ServiceFormViewModel inputSF)
        {
            string halfPath = "/images/ServiceFormFiles/" + AppSession.GetCurrentUserId() + "/";

            var result = erpService.ServiceFormSave(inputSF, Path.Combine(Server.MapPath("~/images/ServiceFormFiles/" + AppSession.GetCurrentUserId() + "/")), halfPath);

            var sfm = erpService.GetServiceForm(inputSF.ServiceID);
            if (sfm.Status_Flag == 2)
            {
                var res = GenerateSFPDF(inputSF.ServiceID);
            }

            //var result = 2;
            if (result > 0)
            {
                return getSuccessfulOperation("OK", result.ToString());
            }
            else
            {
                return getFailedOperation();
            }

        }

        public ActionResult PrintServiceForm(int id)
        {
            var cpyid = AppSession.GetCompanyId();
            var sfm = erpService.GetServiceForm(id);
            var school = erpService.GetSchool(sfm.SchoolID ?? default(int));
            ViewBag.SchoolName = school.Building_Name;
            ViewBag.Address = school.Address;
            ViewBag.Tel = school.Mobile;
            ViewBag.OPMngr = school.Op_Manager_Name;
            ViewBag.Email = school.Email;
            var user = userService.GetUser(sfm.UserID ?? default(int));
            ViewBag.TechName = user.DisplayName;
            ViewBag.CompanyID = cpyid;
            return View(sfm);
        }

        public int GenerateSFPDF(int id)
        {
            try
            {

                var path = Path.Combine(Server.MapPath("~/images/SFPdfFiles/"), "SF_" + id + ".pdf");


                var actionPDF = new Rotativa.ActionAsPdf("PrintServiceFormPdf/" + id)
                {
                    //FileName = fn
                    //PageMargins = { Left = 20, Bottom = 20, Right = 20, Top = 30 },
                };

                var applicationPDFData = actionPDF.BuildFile(ControllerContext);
                var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                fileStream.Write(applicationPDFData, 0, applicationPDFData.Length);
                fileStream.Close();

                return 1;
            }
            catch (Exception ex)
            {
                logger.Debug("SF Pdf Error1:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return 0;
            }
        }

        public ActionResult PrintServiceFormPdf(int id)
        {
            var cpyid = AppSession.GetCompanyId();
            var sfm = erpService.GetServiceForm(id);
            var school = erpService.GetSchool(sfm.SchoolID ?? default(int));
            ViewBag.SchoolName = school.Building_Name;
            ViewBag.Address = school.Address;
            ViewBag.Tel = school.Mobile;
            ViewBag.OPMngr = school.Op_Manager_Name;
            ViewBag.Email = school.Email;
            var user = userService.GetUser(sfm.UserID ?? default(int));
            ViewBag.TechName = user.DisplayName;
            ViewBag.CompanyID = cpyid;
            return View(sfm);
        }

        #endregion


        public ActionResult PrintCheckListFormat(string CLType, string Frequency)
        {
            var cpyid = AppSession.GetCompanyId();
            var chklst = erpService.GetAllCheckListItems().Where(a => a.ServiceType == CLType && (a.Frequency == Frequency || a.Frequency == null)).OrderBy(a => a.OrderBy).ToList();

            var title = "";
            var annex = "";
            var TitleH = "";

            if (Frequency == "Monthly")
            {
                annex = "ANNEX D7_1";
            }
            if (Frequency == "Quarterly")
            {
                annex = "ANNEX D7_2";
            }
            if (Frequency == "HalfYearly")
            {
                annex = "ANNEX D7_3";
            }
            if (Frequency == "Yearly")
            {
                annex = "ANNEX D7_4";
            }

            var spl = "";
            if (CLType == "SP-LIFTSYS")
                spl = "LIFTS SYSTEM(PASSENGER, FIREMAN & PLATFORM, HOIST)";
            if (CLType == "SP-SECUTYSYS")
                spl = "SECURITY SYSTEMS (INCLUDING DECAMS)";
            if (CLType == "SP-FIREALMSYS")
                spl = "ALL TYPES OF FIRE ALARM SYSTEMS(INCLUDING DECAMS)";
            if (CLType == "SP-PLCWDWL")
                spl = "PROGRAMMABLE LOGIC CONTROLLER (WIRED & WIRELESS)";
            if (CLType == "SP-HYDPUMP")
                spl = "HYDRANT PUMPING SYSTEM";
            if (CLType == "SP-PASISCM")
                spl = "PUBLIC ADDRESS SYSTEM, ISCM and SOUND REINFORCEMENT SYSTEM";
            if (CLType == "SP-BASBMSSYS")
                spl = "BAS, BMS & MMS";
            if (CLType == "SP-CHILLERSYS")
                spl = "CHILLER SYSTEMS(WATER OR AIR - COOLED TYPES)";
            if (CLType == "SP-FUMECESYS")
                spl = "FUME CUPBOARD EXHAUST SYSTEMS(DUCT OR DUCTLESS)";
            if (CLType == "SP-FIRESPNKLR")
                spl = "FIRE SPRINKLER SYSTEMS";
            if (CLType == "SP-WETCHSUSYS")
                spl = "WET CHEMICAL SUPRESSION SYSTEMS";
            if (CLType == "SP-HVLSFMPIS")
                spl = "HIGH VOLUME LOW SPEED FAN(MPH & ISH)";

            if (CLType == "SP-SMKCTLSYS")
                spl = "SMOKE CONTROL SYSTEMS";
            if (CLType == "SP-OTHLIFTS")
                spl = "OTHER TYPES OF LIFT(CHAIRLIFT & DUMBWAITER)";
            if (CLType == "SP-GENSET")
                spl = "GENERATOR SET(EXCLUDING LIFT, SMOKE AND FIRE FIGHTING SYSTEMS)";
            if (CLType == "SP-SIOCDSYS")
                spl = "SERVICING & INSPECTION OF CIVIL DEFENCE SHELTERS";


            if (CLType == "Electrical")
            {
                title = CLType + " & Domestic";
                TitleH = "M & E INSPECTION FORM - " + CLType + "(" + Frequency + ")";
            }
            else if (CLType == "Mechanical")
            {
                title = CLType;
                TitleH = "M & E INSPECTION FORM - " + CLType + "(" + Frequency + ")";
            }
            else
            {
                title = "Specialist";
                annex = "ANNEX D7_5";
                TitleH = "M & E INSPECTION FORM (SPECIALIST) - " + spl;
            }


            ViewBag.Title = TitleH;
            ViewBag.Annexure = annex;
            //ViewBag.CompanyID = 1;
            ViewBag.CompanyID = cpyid;
            ViewBag.CLType = CLType;
            ViewBag.Frequency = Frequency;
            ViewBag.HVLSFan_count = 1;
            ViewBag.LIFT_count = 1;

            ViewBag.CLCount = chklst.Count;

            ViewBag.CheckListItems = chklst;
            return View();

        }

        public ActionResult CLSummaryMatrix()
        {
            var now = DateTime.Now;
            var firstdate = new DateTime(now.Year, now.Month, 1);
            ViewBag.Mon = firstdate.ToString("MMM yyyy");
            var filter = new FilterViewModel();

            var userid = AppSession.GetCurrentUserId();
            var user = userService.GetUser(userid);
            ViewBag.Zone = user.Zone;
            var prj = erpService.GetProjectFromDate(firstdate);
            ViewBag.Freq = prj.Frequency;

            var lst = erpService.GetCurCLSummaryMatrix(userid);

            filter.cls = lst;
            return View(filter);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CLSummaryMatrix(FilterViewModel filter)
        {
            if (ModelState.IsValid)
            {
                var fildt = filter.Month + filter.Year;
                fildt = DateTime.ParseExact(fildt, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
                var firstdate = Convert.ToDateTime(fildt);

                ViewBag.Mon = firstdate.ToString("MMM yyyy");

                var fil = new FilterViewModel();
                fil.Month = filter.Month;
                fil.Year = filter.Year;
                fil.userid = AppSession.GetCurrentUserId();

                var user = userService.GetUser(fil.userid);
                ViewBag.Zone = user.Zone;
                var prj = erpService.GetProjectFromDate(firstdate);
                ViewBag.Freq = prj.Frequency;

                var lst = erpService.GetFilterCLSummaryMatrix(fil);
                fil.cls = lst;

                return View(fil);

            }
            return View();

        }


        public ActionResult CLSummaryMatrixDirector()
        {
            var now = DateTime.Now;
            var firstdate = new DateTime(now.Year, now.Month, 1);
            ViewBag.Mon = "";

            //ViewBag.Mon = firstdate.ToString("MMM yyyy");
            var filter = new FilterViewModel();

            var userid = AppSession.GetCurrentUserId();
            var user = userService.GetUser(userid);
            ViewBag.Zone = user.Zone;
            var prj = erpService.GetProjectFromDate(firstdate);
            ViewBag.Freq = "";
            //ViewBag.Freq = prj.Frequency;

            var lst = erpService.GetCurCLSummaryMatrix(userid);

            filter.cls = lst;
            return View(filter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CLSummaryMatrixDirector(FilterViewModel filter)
        {
            if (ModelState.IsValid)
            {
                var fildt = filter.Month + filter.Year;
                fildt = DateTime.ParseExact(fildt, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
                var firstdate = Convert.ToDateTime(fildt);

                ViewBag.Mon = firstdate.ToString("MMM yyyy");

                var fil = new FilterViewModel();
                fil.Month = filter.Month;
                fil.Year = filter.Year;
                fil.userid = AppSession.GetCurrentUserId();
                fil.Zone = filter.Zone;

                var user = userService.GetUser(fil.userid);
                ViewBag.Zone = filter.Zone;
                var prj = erpService.GetProjectFromDate(firstdate);
                ViewBag.Freq = prj.Frequency;

                var lst = erpService.GetFilterCLSummaryMatrix(fil);
                fil.cls = lst;

                return View(fil);

            }
            return View();

        }

        public ActionResult CLDownload()
        {
            var fil = new FilterViewModel();
            var cpyid = AppSession.GetCompanyId();
            var usrid = AppSession.GetCurrentUserId();
            var user = userService.GetUser(usrid);
            var school = new List<BuildingMasterViewModel>();
            if (user.Zone != "ALL")
            {
                school = erpService.GetAllSchools().Where(a => a.IsActive == 1 && a.Zone == user.Zone).ToList();
            }
            else
            {
                school = erpService.GetAllSchools().Where(a => a.IsActive == 1 && a.CompanyID == cpyid).ToList();
            }

            ViewBag.School = school;
            var obj = school.Select(a => a.School_Type).Distinct().ToList();
            ViewBag.SchoolGroup = obj;
            ViewBag.SchoolID = 0;
            fil.clfdownload = new List<ChecklistFileViewModel>();
            return View(fil);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CLDownload(FilterViewModel filter)
        {
            if (ModelState.IsValid)
            {
                var fildt = filter.Month + filter.Year;
                fildt = DateTime.ParseExact(fildt, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
                var firstdate = Convert.ToDateTime(fildt);

                ViewBag.Mon = firstdate.ToString("MMM yyyy");

                var fil = new FilterViewModel();
                fil.Month = filter.Month;
                fil.Year = filter.Year;
                fil.userid = AppSession.GetCurrentUserId();

                var user = userService.GetUser(fil.userid);
                ViewBag.Zone = user.Zone;
                var prj = erpService.GetProjectFromDate(firstdate);
                ViewBag.Freq = prj.Frequency;

                var lst = new List<ChecklistFileViewModel>();
                filter.serviceType = user.ServiceType;
                if (user.GroupID != 3)
                {
                    lst = erpService.GetFilterCLDownload(filter);
                }
                else
                {
                    lst = erpService.GetFilterCLDownload_ServiceType(filter);
                }

                fil.clfdownload = lst;
                if (lst.Count > 0)
                    ViewBag.ShowDLZip = 1;
                else
                    ViewBag.ShowDLZip = 0;

                var school = new List<BuildingMasterViewModel>();
                if (user.Zone != "ALL")
                {
                    school = erpService.GetAllSchools().Where(a => a.IsActive == 1 && a.Zone == user.Zone).ToList();
                }
                else
                {
                    school = erpService.GetAllSchools().Where(a => a.IsActive == 1 && a.CompanyID == user.CompanyID).ToList();
                }

                ViewBag.School = school;
                var obj = school.Select(a => a.School_Type).Distinct().ToList();
                ViewBag.SchoolGroup = obj;
                ViewBag.SchoolID = filter.SchoolID;

                return View(fil);

            }
            return View();

        }


        public ActionResult CLDownloadLiftOperator()
        {

            var fil = new FilterViewModel();
            fil.clfdownload = new List<ChecklistFileViewModel>();
            return View(fil);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CLDownloadLiftOperator(FilterViewModel filter)
        {
            if (ModelState.IsValid)
            {

                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                List<int> schoolids = new List<int>();
                if (user.Zone != "ALL")
                {
                    schoolids = erpService.GetAllSchools().Where(a => a.Zone == user.Zone).Select(a => a.BuildingID).ToList();
                }
                else
                {
                    schoolids = erpService.GetAllSchools().Where(a => a.CompanyID == user.CompanyID).Select(a => a.BuildingID).ToList();
                }

                //var prjs = new List<ProjectMasterViewModel>();
                List<int> liftschoolids = new List<int>();
                liftschoolids = erpService.GetLiftProjects(schoolids, user.DisplayName).Select(a => a.BuildingID ?? default(int)).Distinct().ToList();

                List<int> pdfchklstids = new List<int>();
                pdfchklstids = erpService.GetCLTMIDsfromTransMaster(liftschoolids, usrid).Select(a => a.CLTMID).ToList();

                filter.schoolids = liftschoolids;
                filter.pdfclids = pdfchklstids;

                var fildt = filter.Month + filter.Year;
                fildt = DateTime.ParseExact(fildt, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
                var firstdate = Convert.ToDateTime(fildt);

                ViewBag.Mon = firstdate.ToString("MMM yyyy");

                var fil = new FilterViewModel();
                fil.Month = filter.Month;
                fil.Year = filter.Year;
                fil.userid = AppSession.GetCurrentUserId();


                ViewBag.Zone = user.Zone;
                var prj = erpService.GetProjectFromDate(firstdate);
                ViewBag.Freq = prj.Frequency;

                var lst = erpService.GetFilterCLDownloadLift(filter);
                List<ChecklistFileViewModel> newclfvm = new List<ChecklistFileViewModel>();

                foreach (var ls in lst)
                {
                    var sch = erpService.GetSchool(ls.SchoolID ?? default(int));
                    foreach (var pl in pdfchklstids)
                    {

                        if (ls.FilePath.Contains(pl.ToString()))
                        {
                            newclfvm.Add(
                                new ChecklistFileViewModel()
                                {
                                    ReportID = ls.ReportID,
                                    SchoolID = ls.SchoolID,
                                    Month_Start_Date = ls.Month_Start_Date,
                                    Frequency = ls.Frequency,
                                    CLType = ls.CLType,
                                    FilePath = ls.FilePath,
                                    School_Name = sch.Building_Name,
                                    Zone = sch.Zone

                                }
                                );
                        }
                    }


                }

                fil.clfdownload = newclfvm;

                if (lst.Count > 0)
                    ViewBag.ShowDLZip = 1;
                else
                    ViewBag.ShowDLZip = 0;

                var school = new List<BuildingMasterViewModel>();
                if (user.Zone != "ALL")
                {
                    school = erpService.GetAllSchools().Where(a => a.IsActive == 1 && a.Zone == user.Zone).ToList();
                }
                else
                {
                    school = erpService.GetAllSchools().Where(a => a.IsActive == 1 && a.CompanyID == user.CompanyID).ToList();
                }

                ViewBag.School = school;
                var obj = school.Select(a => a.School_Type).Distinct().ToList();
                ViewBag.SchoolGroup = obj;
                ViewBag.SchoolID = filter.SchoolID;

                return View(fil);

            }
            return View();

        }

        public ActionResult CLDownloadAdmin()
        {

            var fil = new FilterViewModel();
            var cpyid = AppSession.GetCompanyId();
            var usrid = AppSession.GetCurrentUserId();
            var user = userService.GetUser(usrid);
            var school = new List<BuildingMasterViewModel>();
            if (user.Zone != "ALL")
            {
                school = erpService.GetAllSchools().Where(a => a.IsActive == 1 && a.Zone == user.Zone).ToList();
            }
            else
            {
                school = erpService.GetAllSchools().Where(a => a.IsActive == 1 && a.CompanyID == cpyid).ToList();
            }

            ViewBag.School = school;
            var obj = school.Select(a => a.School_Type).Distinct().ToList();
            ViewBag.SchoolGroup = obj;
            ViewBag.SchoolID = 0;
            fil.clfdownload = new List<ChecklistFileViewModel>();
            return View(fil);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CLDownloadAdmin(FilterViewModel filter)
        {
            if (ModelState.IsValid)
            {
                var fildt = filter.Month + filter.Year;
                fildt = DateTime.ParseExact(fildt, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
                var firstdate = Convert.ToDateTime(fildt);

                ViewBag.Mon = firstdate.ToString("MMM yyyy");

                var fil = new FilterViewModel();
                fil.Month = filter.Month;
                fil.Year = filter.Year;
                fil.userid = AppSession.GetCurrentUserId();

                var user = userService.GetUser(fil.userid);
                ViewBag.Zone = filter.Zone;
                var prj = erpService.GetProjectFromDate(firstdate);
                ViewBag.Freq = prj.Frequency;

                var lst = new List<ChecklistFileViewModel>();
                filter.serviceType = user.ServiceType;
                if (user.GroupID != 3)
                {
                    lst = erpService.GetFilterCLDownload(filter);
                }
                else
                {
                    lst = erpService.GetFilterCLDownload_ServiceType(filter);
                }

                fil.clfdownload = lst;
                if (lst.Count > 0)
                    ViewBag.ShowDLZip = 1;
                else
                    ViewBag.ShowDLZip = 0;

                var school = new List<BuildingMasterViewModel>();
                if (user.Zone != "ALL")
                {
                    school = erpService.GetAllSchools().Where(a => a.IsActive == 1 && a.Zone == user.Zone).ToList();
                }
                else
                {
                    school = erpService.GetAllSchools().Where(a => a.IsActive == 1 && a.CompanyID == user.CompanyID).ToList();
                }

                ViewBag.School = school;
                var obj = school.Select(a => a.School_Type).Distinct().ToList();
                ViewBag.SchoolGroup = obj;
                ViewBag.SchoolID = filter.SchoolID;
                ViewBag.Schoollist = filter.Zone + "-" + filter.SchoolID;

                return View(fil);

            }
            return View();

        }

        public FileResult DownloadZipFile(int schoolid, string month, string year)
        {
            var school = erpService.GetSchool(schoolid);
            var filter = new FilterViewModel();
            filter.SchoolID = schoolid;
            filter.Month = month;
            filter.Year = year;

            var lst = erpService.GetFilterCLDownload(filter).ToList();
            // if (lst.Count > 0)
            // {
            var freq = lst.FirstOrDefault().Frequency;
            var schname = school.Zone + "_" + school.Building_Name + "_" + freq;
            //var fileName = string.Format("{0}_CLFiles.zip", DateTime.Today.Date.ToString("dd-MM-yyyy") + "_1");
            var fileName = string.Format("{0}_CLFiles.zip", schname);
            var tempOutPutPath = Server.MapPath(Url.Content("/images/")) + fileName;

            using (ZipOutputStream s = new ZipOutputStream(System.IO.File.Create(tempOutPutPath)))
            {
                s.SetLevel(9); // 0-9, 9 being the highest compression  

                byte[] buffer = new byte[4096];

                var ImageList = new List<string>();
                foreach (var item in lst)
                {
                    var path = item.FilePath.Split('\\');
                    var newfn = path[path.Count() - 3] + "/" + path[path.Count() - 2] + "/" + path[path.Count() - 1];
                    ImageList.Add(Server.MapPath("/" + newfn));
                }

                //ImageList.Add(Server.MapPath("/Images/02.jpg"));


                for (int i = 0; i < ImageList.Count; i++)
                {
                    ZipEntry entry = new ZipEntry(Path.GetFileName(ImageList[i]));
                    entry.DateTime = DateTime.Now;
                    entry.IsUnicodeText = true;
                    s.PutNextEntry(entry);

                    using (FileStream fs = System.IO.File.OpenRead(ImageList[i]))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = fs.Read(buffer, 0, buffer.Length);
                            s.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }
                s.Finish();
                s.Flush();
                s.Close();

            }

            byte[] finalResult = System.IO.File.ReadAllBytes(tempOutPutPath);
            if (System.IO.File.Exists(tempOutPutPath))
                System.IO.File.Delete(tempOutPutPath);

            if (finalResult == null || !finalResult.Any())
                throw new Exception(String.Format("No Files found with Image"));

            return File(finalResult, "application/zip", fileName);
            //}

            //return File("application/zip", "a.txt");
        }

        public ActionResult CLSearchIndex()
        {
            var fil = new FilterViewModel();
            var cpyid = AppSession.GetCompanyId();
            var usrid = AppSession.GetCurrentUserId();
            var user = userService.GetUser(usrid);
            ViewBag.SearchCheck = 0;

            fil.clfdownload = new List<ChecklistFileViewModel>();
            return View(fil);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CLSearchIndex(FilterViewModel filter)
        {
            if (ModelState.IsValid)
            {
                var fildt = filter.Month + filter.Year;
                fildt = DateTime.ParseExact(fildt, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
                var firstdate = Convert.ToDateTime(fildt);
                var lastdate = firstdate.AddMonths(1).AddDays(-1);

                ViewBag.Mon = firstdate.ToString("MMM yyyy");

                var fil = new FilterViewModel();
                fil.Month = filter.Month;
                fil.Year = filter.Year;
                fil.userid = AppSession.GetCurrentUserId();

                var user = userService.GetUser(fil.userid);
                ViewBag.Zone = user.Zone;
                var prj = erpService.GetProjectFromDate(firstdate);
                ViewBag.Freq = prj.Frequency;
                ViewBag.FirstDate = firstdate;
                ViewBag.LastDate = lastdate;

                var lst = new List<ChecklistFileViewModel>();
                ViewBag.SearchCheck = 1;
                return View(fil);

            }
            return View();

        }


        public ActionResult CLSearchIndexNew()
        {
            var fil = new FilterViewModel();
            var cpyid = AppSession.GetCompanyId();
            var usrid = AppSession.GetCurrentUserId();
            var user = userService.GetUser(usrid);
            ViewBag.SearchCheck = 0;



            var school = new List<BuildingMasterViewModel>();
            if (user.Zone != "ALL")
            {
                school = erpService.GetAllSchools().Where(a => a.IsActive == 1 && a.Zone == user.Zone).ToList();
            }
            else
            {
                school = erpService.GetAllSchools().Where(a => a.IsActive == 1 && a.CompanyID == cpyid).ToList();
            }

            ViewBag.School = school;
            var obj = school.Select(a => a.School_Type).Distinct().ToList();
            ViewBag.SchoolGroup = obj;
            ViewBag.SchoolID = 0;


            fil.clfdownload = new List<ChecklistFileViewModel>();
            return View(fil);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CLSearchIndexNew(FilterViewModel filter)
        {
            if (ModelState.IsValid)
            {
                var frmdt = "";
                var todt = "";
                //string [] schoolID={};

                var schoolID = filter.Schoollist.Split('-');



                frmdt = DateTime.ParseExact(filter.startDate, "dd/MM/yyyy  hh:mm:ss tt", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
                var firstdate = Convert.ToDateTime(frmdt);

                todt = DateTime.ParseExact(filter.endDate, "dd/MM/yyyy  hh:mm:ss tt", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
                var lastdate = Convert.ToDateTime(todt);

                ViewBag.Mon = firstdate.ToString("MMM yyyy");

                var fil = new FilterViewModel();
                fil.startDate = filter.startDate;
                fil.endDate = filter.endDate;
                fil.userid = AppSession.GetCurrentUserId();

                var user = userService.GetUser(fil.userid);
                // ViewBag.Zone = user.Zone;
                //   var prj = erpService.GetProjectFromDate(firstdate);

                var school = new List<BuildingMasterViewModel>();
                if (user.Zone != "ALL")
                {
                    school = erpService.GetAllSchools().Where(a => a.IsActive == 1 && a.Zone == user.Zone).ToList();
                }
                else
                {
                    school = erpService.GetAllSchools().Where(a => a.IsActive == 1 && a.CompanyID == user.CompanyID).ToList();
                }

                ViewBag.School = school;
                var obj = school.Select(a => a.School_Type).Distinct().ToList();
                ViewBag.SchoolGroup = obj;
                ViewBag.SchoolID = filter.SchoolID;
                ViewBag.Schoollist = filter.Zone + "-" + filter.SchoolID;


                ViewBag.Zone = filter.Zone;

                ViewBag.schoolID = schoolID[1];

                ViewBag.CLType = filter.CLType;
                ViewBag.Freq = filter.Frequency;
                ViewBag.CLstatus = filter.CLStatus;
                ViewBag.FromDate = firstdate;
                ViewBag.ToDate = lastdate;



                var lst = new List<ChecklistFileViewModel>();
                ViewBag.SearchCheck = 1;
                return View(fil);

            }
            return View();

        }


        public ActionResult GetSearchChecklist(DateTime firstdate, DateTime lastdate)
        {
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                var grpid = AppSession.GetCurrentUserGroup();
                List<CheckListIndexViewModel> newCL = new List<CheckListIndexViewModel>();

                //Admin or director group id
                if (grpid == 2)
                {
                    //List<int> schoolids = new List<int>();
                    //var schools = erpService.GetAllSchools().ToList();
                    //schoolids = schools.Where(a => a.CompanyID == user.CompanyID).Select(a => a.SchoolID).ToList();

                    //var liftschoolids = erpService.GetApplicableSchoolIDs(schoolids, user.ServiceType, user.UserID).ToList();
                    //var now = DateTime.Now;
                    // var firstdate = new DateTime(now.Year, now.Month, 1);
                    //var premonth_firstdate = firstdate.AddMonths(-1);
                    //var lastdate = firstdate.AddMonths(1).AddDays(-1);
                    //var prjs = erpService.GetAllProjectsBasedSchoolIDs(schoolids, lastdate).Where(a => a.Month_Start_Date == firstdate).ToList();

                    //List<int> newList = liftschoolids.Where(v => v != null).Select(v => v.Value).ToList();
                    //var chkLists = erpService.GetAllSubmittedCheckList_School(schoolids).Where(a => a.CheckedBy_Signature != null && a.CLMonth == firstdate).ToList();

                    //var leftObj = (from a in prjs
                    //               join b in chkLists on a.PrjMasID equals b.PrjMasID
                    //                into pcTable
                    //               from c in pcTable.DefaultIfEmpty()
                    //               select new { a.PrjMasID, a.SchoolID, a.Frequency, a.Month_End_Date, a.CLType, c }).ToList();

                    //foreach (var chl in leftObj)
                    //{
                    //    var sch = schools.Where(a => a.SchoolID == chl.SchoolID).FirstOrDefault();
                    //    var sts = "";
                    //    if (chl.c != null)
                    //    {
                    //        if (chl.c.EndoresBy_Signature == null)
                    //        {
                    //            sts = "Pending User";
                    //        }
                    //        else if (chl.c.VerifiedBy_Signature == null)
                    //        {
                    //            sts = "Pending MA";
                    //        }
                    //        else
                    //        {
                    //            sts = "Completed";
                    //        }

                    //        if (chl.c.VerifiedBy_Signature == null && chl.c.EndoresBy_Signature == null)
                    //        {
                    //            sts = "Pending User & MA";
                    //        }

                    //        //var sch = schools.Where(a => a.SchoolID == chl.SchoolID).FirstOrDefault();
                    //        var projmas = erpService.GetProject(chl.c.PrjMasID ?? default(int)).Month_End_Date;
                    //        newCL.Add(new CheckListIndexViewModel()
                    //        {
                    //            CLTMID = chl.c.CLTMID,
                    //            PrjMasID = chl.c.PrjMasID.Value,
                    //            SchoolID = chl.c.SchoolID.Value,
                    //            Status = sts,
                    //            Zone = sch.Zone,
                    //            SchoolName = sch.School_Name,
                    //            Frequency = chl.c.Frequency,
                    //            CLType = chl.c.CLType,
                    //            Month_Name = projmas.Value.ToString("MMMM yyyy")
                    //        });
                    //    }
                    //    else
                    //    {
                    //        sts = "New Submit";
                    //        newCL.Add(new CheckListIndexViewModel()
                    //        {
                    //            CLTMID = 0,
                    //            PrjMasID = chl.PrjMasID,
                    //            SchoolID = chl.SchoolID.Value,
                    //            Status = sts,
                    //            Zone = sch.Zone,
                    //            SchoolName = sch.School_Name,
                    //            Frequency = chl.Frequency,
                    //            CLType = chl.CLType,
                    //            Month_Name = chl.Month_End_Date.Value.ToString("MMMM yyyy")
                    //        });
                    //    }
                    //}

                    // newCL = newCL.Where(a => a.CLType != "SP-FIREALMSYS" && a.CLType != "SP-PLCWDWL" && a.CLType != "SP-HYDPUMP" && a.CLType != "SP-FIRESPNKLR" && a.CLType != "SP-SMKCTLSYS" && a.CLType != "SP-GENSET" && a.CLType != "SP_SIOCDSYS").ToList();
                    if (user.CompanyID == 1 || user.CompanyID == 2)
                        newCL = erpService.GetAdminChecklistIndex(user.CompanyID ?? default(int), firstdate, lastdate).Where(a => a.CLType != "SP-FIREALMSYS" && a.CLType != "SP-PLCWDWL" && a.CLType != "SP-HYDPUMP" && a.CLType != "SP-FIRESPNKLR" && a.CLType != "SP-SMKCTLSYS" && a.CLType != "SP-GENSET" && a.CLType != "SP_SIOCDSYS").ToList();
                    else
                        newCL = erpService.GetAdminChecklistIndex(user.CompanyID ?? default(int), firstdate, lastdate).ToList();

                }

                //Project Manager or M&E
                if (grpid == 6 || grpid == 8)
                {
                    //var sts = "";
                    //List<int> schoolids = new List<int>();
                    //var schools = erpService.GetAllSchools().ToList();
                    //schoolids = schools.Where(a => a.Zone == user.Zone && a.CompanyID == user.CompanyID).Select(a => a.SchoolID).ToList();
                    //var chkLists = erpService.GetAllSubmittedCheckList_School(schoolids).Where(a => a.CheckedBy_Signature != null && a.CLMonth == firstdate).ToList();
                    //foreach (var chl in chkLists)
                    //{
                    //    if (chl.EndoresBy_Signature == null)
                    //    {
                    //        sts = "Pending User";
                    //    }
                    //    else if (chl.VerifiedBy_Signature == null)
                    //    {
                    //        sts = "Pending MA";
                    //    }
                    //    else
                    //    {
                    //        sts = "Completed";
                    //    }

                    //    if (chl.VerifiedBy_Signature == null && chl.EndoresBy_Signature == null)
                    //    {
                    //        sts = "Pending User & MA";
                    //    }

                    //    var sch = schools.Where(a => a.SchoolID == chl.SchoolID).FirstOrDefault();
                    //    var projmas = erpService.GetProject(chl.PrjMasID ?? default(int)).Month_End_Date;
                    //    newCL.Add(new CheckListIndexViewModel()
                    //    {
                    //        CLTMID = chl.CLTMID,
                    //        PrjMasID = chl.PrjMasID.Value,
                    //        SchoolID = chl.SchoolID.Value,
                    //        Status = sts,
                    //        Zone = sch.Zone,
                    //        SchoolName = sch.School_Name,
                    //        Frequency = chl.Frequency,
                    //        CLType = chl.CLType,
                    //        Month_Name = projmas.Value.ToString("MMMM yyyy")
                    //    });

                    //}

                    newCL = erpService.GetUserChecklistIndex_Future(user.CompanyID ?? default(int), user.UserID, firstdate, lastdate).ToList();

                }

                //MA
                if (grpid == 5)
                {
                    //var chk = "";
                    //var tst = 0;

                    //var sts = "";
                    //List<int> schoolids = new List<int>();
                    //var schools = erpService.GetAllSchools().ToList();
                    //schoolids = schools.Where(a => a.Zone == user.Zone && a.CompanyID == user.CompanyID).Select(a => a.SchoolID).ToList();
                    //var chkLists = erpService.GetAllSubmittedCheckList_School(schoolids).Where(a => a.CheckedBy_Signature != null && a.CLMonth == firstdate).ToList();

                    ////logger.Info(chkLists.Count);                    

                    //foreach (var chl in chkLists)
                    //{
                    //    //tst++;

                    //    if (chl.EndoresBy_Signature == null)
                    //    {
                    //        sts = "Pending User";
                    //    }
                    //    else if (chl.VerifiedBy_Signature == null)
                    //    {
                    //        sts = "Pending MA";
                    //    }
                    //    else
                    //    {
                    //        sts = "Completed";
                    //    }

                    //    if (chl.VerifiedBy_Signature == null && chl.EndoresBy_Signature == null)
                    //    {
                    //        sts = "Pending User & MA";
                    //    }

                    //    var sch = schools.Where(a => a.SchoolID == chl.SchoolID).FirstOrDefault();
                    //    var projmas = erpService.GetProject(chl.PrjMasID ?? default(int)).Month_End_Date;

                    //    //chk = tst.ToString() + "-" + chl.CLTMID;
                    //    //logger.Info(chk);

                    //    //if (sts == "Pending MA" || sts == "Pending User & MA")
                    //        newCL.Add(new CheckListIndexViewModel()
                    //        {
                    //            CLTMID = chl.CLTMID,
                    //            PrjMasID = chl.PrjMasID.Value,
                    //            SchoolID = chl.SchoolID.Value,
                    //            Status = sts,
                    //            Zone = sch.Zone,
                    //            SchoolName = sch.School_Name,
                    //            Frequency = chl.Frequency,
                    //            CLType = chl.CLType,
                    //            Month_Name = projmas.Value.ToString("MMMM yyyy")
                    //        });

                    //}
                    newCL = erpService.GetUserChecklistIndex_Future(user.CompanyID ?? default(int), user.UserID, firstdate, lastdate).ToList();

                }

                return Json(new { value = "OK", data = newCL }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Debug("ChkList Admin Index:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return View();
            }
        }




        public ActionResult GetSearchChecklistNew(string zone, int schoolID, string cltype, string frequency, int clstatus, string fromdate, string todate)
        {
            try
            {
                var usrid = AppSession.GetCurrentUserId();
                var user = userService.GetUser(usrid);
                var grpid = AppSession.GetCurrentUserGroup();
                List<CheckListIndexViewModel> newCL = new List<CheckListIndexViewModel>();


                var frmdt = DateTime.ParseExact(fromdate, "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");

                // var frmdt = DateTime.ParseExact(fromdate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
                var firstdate = Convert.ToDateTime(frmdt);

                // var todt = DateTime.ParseExact(todate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");

                var todt = DateTime.ParseExact(todate, "dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
                var to_date = Convert.ToDateTime(todt);


                if (clstatus == 1)
                {
                    newCL = erpService.GetAdminChecklistIndexNew(zone, schoolID, cltype, frequency, clstatus, firstdate, to_date).Where(a => a.Status == "New Submit").ToList();
                }
                else if (clstatus == 2)
                {
                    newCL = erpService.GetAdminChecklistIndexNew(zone, schoolID, cltype, frequency, clstatus, firstdate, to_date).Where(a => a.Status == "Pending User").ToList();
                }
                else if (clstatus == 3)
                {
                    newCL = erpService.GetAdminChecklistIndexNew(zone, schoolID, cltype, frequency, clstatus, firstdate, to_date).Where(a => a.Status == "Pending MA").ToList();
                }
                else if (clstatus == 4)
                {
                    newCL = erpService.GetAdminChecklistIndexNew(zone, schoolID, cltype, frequency, clstatus, firstdate, to_date).Where(a => a.Status == "Pending User & MA").ToList();
                }
                else if (clstatus == 5)
                {
                    newCL = erpService.GetAdminChecklistIndexNew(zone, schoolID, cltype, frequency, clstatus, firstdate, to_date).Where(a => a.Status == "Completed").ToList();
                }
                else
                {
                    newCL = erpService.GetAdminChecklistIndexNew(zone, schoolID, cltype, frequency, clstatus, firstdate, to_date).ToList();

                }

                //Admin or director group id
                if (grpid == 2)
                {


                    // newCL = newCL.Where(a => a.CLType != "SP-FIREALMSYS" && a.CLType != "SP-PLCWDWL" && a.CLType != "SP-HYDPUMP" && a.CLType != "SP-FIRESPNKLR" && a.CLType != "SP-SMKCTLSYS" && a.CLType != "SP-GENSET" && a.CLType != "SP_SIOCDSYS").ToList();
                    //if (user.CompanyID == 1 || user.CompanyID == 2)
                    // newCL = erpService.GetAdminChecklistIndex(user.CompanyID ?? default(int), firstdate, lastdate).Where(a => a.CLType != "SP-FIREALMSYS" && a.CLType != "SP-PLCWDWL" && a.CLType != "SP-HYDPUMP" && a.CLType != "SP-FIRESPNKLR" && a.CLType != "SP-SMKCTLSYS" && a.CLType != "SP-GENSET" && a.CLType != "SP_SIOCDSYS").ToList();
                    //  else
                    //   newCL = erpService.GetAdminChecklistIndex(user.CompanyID ?? default(int), firstdate, lastdate).ToList();

                }

                //Project Manager or M&E
                if (grpid == 6 || grpid == 8)
                {

                    //newCL = erpService.GetUserChecklistIndex_Future(user.CompanyID ?? default(int), user.UserID, firstdate, lastdate).ToList();

                }

                //MA
                if (grpid == 5)
                {

                    //newCL = erpService.GetUserChecklistIndex_Future(user.CompanyID ?? default(int), user.UserID, firstdate, lastdate).ToList();

                }

                return Json(new { value = "OK", data = newCL }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Debug("ChkList Admin Index:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return View();
            }
        }



        public ActionResult CLLiftMatrix()
        {

            var fil = new FilterViewModel();
            var cpyid = AppSession.GetCompanyId();
            var usrid = AppSession.GetCurrentUserId();
            var user = userService.GetUser(usrid);

            ViewBag.Zone = user.Zone;

            fil.pmvm = new List<ProjectMasterViewModel>();
            return View(fil);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CLLiftMatrix(FilterViewModel filter)
        {
            if (ModelState.IsValid)
            {
                var fildt = filter.Month + filter.Year;
                fildt = DateTime.ParseExact(fildt, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
                var firstdate = Convert.ToDateTime(fildt);

                ViewBag.Mon = firstdate.ToString("MMM yyyy");

                var fil = new FilterViewModel();
                fil.Month = filter.Month;
                fil.Year = filter.Year;
                fil.userid = AppSession.GetCurrentUserId();

                var user = userService.GetUser(fil.userid);
                ViewBag.Zone = user.Zone;
                var prj = erpService.GetProjectFromDate(firstdate);
                ViewBag.Freq = prj.Frequency;
                ViewBag.Zone = filter.Zone;

                var lst = new List<ProjectMasterViewModel>();
                filter.userid = fil.userid;

                lst = erpService.GetCLLiftMatrix(filter);

                fil.pmvm = lst;

                return View(fil);

            }
            return View();

        }


        public int PDFReGenerate(int id)
        {
            try
            {
                var path = "";
                var clitem = erpService.GetCheckListItems(id);
                var prjmon = erpService.GetProject(clitem.PrjMasID ?? default(int)).Month_Start_Date;
                //string[] elecArr = { "Electrical", "SP-FIREALMSYS", "SP-HVLSFMPIS", "SP-SMKCTLSYS", "SP-GENSET", "SP-SIOCDSYS" };
                string[] elecArr = { "Electrical", "SP-FIREALMSYS", "SP-SMKCTLSYS", "SP-GENSET", "SP-SIOCDSYS" };
                string[] mechArr = { "Mechanical", "SP-PLCWDWL", "SP-HYDPUMP", "SP-FUMECESYS", "SP_FIRESPNKLR" };
                string mailAttach = "";

                var newfile = new ChecklistFileViewModel();
                newfile.Frequency = clitem.Frequency;
                newfile.Month_Start_Date = prjmon;
                newfile.SchoolID = clitem.SchoolID;

                var school = erpService.GetSchool(clitem.SchoolID ?? default(int));
                var project = erpService.GetProject(clitem.PrjMasID ?? default(int));

                if (school.CompanyID != 3)
                {
                    if (clitem.CLType == "Electrical")
                    {
                        var applicCL = erpService.GetProjectsForApplicableCLs(clitem.SchoolID ?? default(int), prjmon ?? default(DateTime)).Where(a => elecArr.Contains(a.CLType)).ToList();
                        //Select(a=>a.CLType)
                        foreach (var item in applicCL)
                        {
                            if (item.CLType == "Electrical")
                            {
                                path = Path.Combine(Server.MapPath("~/images/CLPdfFiles/"), "CL_" + id + ".pdf");
                                mailAttach = mailAttach + path + ";";
                            }
                            else
                            {
                                path = Path.Combine(Server.MapPath("~/images/CLPdfFiles/"), "CL_" + id + "_" + item.CLType + ".pdf");
                                mailAttach = mailAttach + path + ";";
                            }
                            newfile.FilePath = path;
                            newfile.CLType = item.CLType;
                            var actionPDF = new Rotativa.ActionAsPdf("PrintCheckListPdf", new { id = id, cltype = item.CLType })
                            {
                                PageMargins = new Rotativa.Options.Margins(20, 5, 20, 5),
                                PageSize = Rotativa.Options.Size.A4,
                                PageOrientation = Rotativa.Options.Orientation.Portrait
                            };

                            var applicationPDFData = actionPDF.BuildFile(ControllerContext);
                            var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                            fileStream.Write(applicationPDFData, 0, applicationPDFData.Length);
                            fileStream.Close();
                            //if (mailflag == 1 || mailflag == 3)
                            //{
                            //    var res = erpService.AddPdfFiles(newfile);
                            //}

                        }

                    }
                    else if (clitem.CLType == "Mechanical")
                    {
                        var applicCL = erpService.GetProjectsForApplicableCLs(clitem.SchoolID ?? default(int), prjmon ?? default(DateTime)).Where(a => mechArr.Contains(a.CLType)).ToList();
                        foreach (var item in applicCL)
                        {
                            if (item.CLType == "Mechanical")
                            {
                                path = Path.Combine(Server.MapPath("~/images/CLPdfFiles/"), "CL_" + id + ".pdf");
                                mailAttach = mailAttach + path + ";";
                            }
                            else
                            {
                                path = Path.Combine(Server.MapPath("~/images/CLPdfFiles/"), "CL_" + id + "_" + item.CLType + ".pdf");
                                mailAttach = mailAttach + path + ";";
                            }
                            newfile.FilePath = path;
                            newfile.CLType = item.CLType;
                            var actionPDF = new Rotativa.ActionAsPdf("PrintCheckListPdf", new { id = id, cltype = item.CLType })
                            {
                                PageMargins = new Rotativa.Options.Margins(20, 5, 20, 5),
                                PageSize = Rotativa.Options.Size.A4,
                                PageOrientation = Rotativa.Options.Orientation.Portrait
                            };

                            var applicationPDFData = actionPDF.BuildFile(ControllerContext);
                            var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                            fileStream.Write(applicationPDFData, 0, applicationPDFData.Length);
                            fileStream.Close();
                            //if (mailflag == 1 || mailflag == 3)
                            //{
                            //    var res = erpService.AddPdfFiles(newfile);
                            //}

                        }
                    }
                    else
                    {
                        path = Path.Combine(Server.MapPath("~/images/CLPdfFiles/"), "CL_" + id + ".pdf");
                        mailAttach = mailAttach + path + ";";
                        var actionPDF = new Rotativa.ActionAsPdf("PrintCheckListPdf", new { id = id, cltype = clitem.CLType })
                        {
                            PageMargins = new Rotativa.Options.Margins(20, 5, 20, 5),
                            PageSize = Rotativa.Options.Size.A4,
                            PageOrientation = Rotativa.Options.Orientation.Portrait
                        };
                        newfile.FilePath = path;
                        newfile.CLType = clitem.CLType;

                        var applicationPDFData = actionPDF.BuildFile(ControllerContext);
                        var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                        fileStream.Write(applicationPDFData, 0, applicationPDFData.Length);
                        fileStream.Close();
                        //if (mailflag == 1 || mailflag == 3)
                        //{
                        //    var res = erpService.AddPdfFiles(newfile);
                        //}
                    }
                }
                else
                {
                    path = Path.Combine(Server.MapPath("~/images/CLPdfFiles/"), "CL_" + id + ".pdf");
                    mailAttach = mailAttach + path + ";";
                    var actionPDF = new Rotativa.ActionAsPdf("PrintCheckListPdf", new { id = id, cltype = clitem.CLType })
                    {
                        PageMargins = new Rotativa.Options.Margins(20, 5, 20, 5),
                        PageSize = Rotativa.Options.Size.A4,
                        PageOrientation = Rotativa.Options.Orientation.Portrait
                    };
                    newfile.FilePath = path;
                    newfile.CLType = clitem.CLType;

                    var applicationPDFData = actionPDF.BuildFile(ControllerContext);
                    var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                    fileStream.Write(applicationPDFData, 0, applicationPDFData.Length);
                    fileStream.Close();
                    //if (mailflag == 1 || mailflag == 3)
                    //{
                    //    var res = erpService.AddPdfFiles(newfile);
                    //}
                }



                return 1;
            }
            catch (Exception ex)
            {
                logger.Debug("Pdf Regen Error1:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                if (ex.InnerException.Message != null)
                {
                    logger.Debug(ex.InnerException.Message);
                    logger.Debug(ex.InnerException.StackTrace);
                }
                return 0;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteChecklistFile(int id)
        {
            var result = erpService.DeleteChecklistFile(id);

            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }

        }




        #region  checkListType

        public ActionResult CheckListTypeIndex()
        {
             var cpyid = AppSession.GetCompanyId();
            //var userid = AppSession.GetCurrentUserId();
            return View(erpService.GetAllCheckListType().Where(a => a.IsActive == 1 && a.CompanyID==cpyid ).ToList());
        }


        public ActionResult CheckListTypeCreate()
        {
            var cpy = userService.GetAllCompanies().Where(a => a.IsActive == 1).ToList();
            cpy.Insert(0, new CompanyMasterViewModel() { CompanyID = 0, CompanyName = "-Select-" });
            ViewBag.CompanyID = new SelectList(cpy, "CompanyID", "CompanyName");
            return View();
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateServiceType(CheckListTypeMasterViewModel cltype)
        {
            //if (ModelState.IsValid)
            // {

            if (erpService.CheckCLType(cltype.ServiceType,cltype.CompanyID) == false)
            {
                cltype.CreatedBy = AppSession.GetCurrentUserId();
                cltype.CreatedDate = DateTime.Now;
                cltype.IsActive = 1;


                var result = erpService.CreateCheckListType(cltype);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            else
            {
                return getFailedOperation("Checklist Type Already exits!");
            }
            //return views(cltype);
            //  }
        }


        [HttpPost, ActionName("DeleteCheckListType")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCheckListType(int id)
        {
            var result = erpService.DeleteCheckListType(id);

            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }

        }

        public ActionResult CheckListTypeEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //   eng_users eng_users = db.eng_users.Find(id);
            var checklisttype = erpService.GetCheckListType(id ?? default(int));
            if (checklisttype == null)
            {
                return HttpNotFound();
            }

            return View(checklisttype);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckListTypeEdit(CheckListTypeMasterViewModel checklisttype)
        {
            if (ModelState.IsValid)
            {
                checklisttype.UpdatedBy = AppSession.GetCurrentUserId();
                checklisttype.UpdatedDate = DateTime.Now;

                var result = erpService.SaveCheckListType(checklisttype);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            return View(checklisttype);
        }
        #endregion

        #region  Asset

        public ActionResult AssetIndex()
        {
            var userid = AppSession.GetCurrentUserId();
            int cpyid = AppSession.GetCompanyId();
            return View(erpService.GetAllNewAsset(userid).Where(a => a.IsActive == 1).ToList());
            //var cpyid = AppSession.GetCompanyId();
            //return View(erpService.GetAllAsset().ToList());
        }

        public ActionResult AssetCreate()
        {
            var building = erpService.GetAllSchools().ToList();
            building.Insert(0, new BuildingMasterViewModel() { BuildingID = 0, Building_Name = "-Select-" });
            ViewBag.BuildingID = new SelectList(building, "BuildingID", "Building_Name ");

            var cpy = userService.GetAllCompanies().Where(a => a.IsActive == 1).ToList();
            cpy.Insert(0, new CompanyMasterViewModel() { CompanyID = 0, CompanyName = "-Select-" });
            ViewBag.CompanyID = new SelectList(cpy, "CompanyID", "CompanyName");

            var cltype = erpService.GetAllCheckListType().Where(a => a.IsActive == 1).ToList();
            cltype.Insert(0, new CheckListTypeMasterViewModel() { ServiceType = "-Select-" });
            ViewBag.ServiceType = new SelectList(cltype, "ServiceType", "ServiceType");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAssetType(ProjectMasterViewModel asset)
        {
            string mnth = asset.MonthName;
            string year = asset.year;
           
            string month_start_date = year + "-" + mnth + "-01";
            DateTime getMonthStartdate = DateTime.ParseExact(month_start_date, "yyyy-MM-dd", null);
            asset.Month_Start_Date = getMonthStartdate;
            asset.MonthName = getMonthStartdate.ToString("MMMM");
            asset.CLType = asset.ServiceType;
            //if (ModelState.IsValid)
            // {

            if (erpService.CheckAsset(asset.BuildingID, asset.CLType, asset.MonthName, asset.CompanyID) == false)
            {
                var result = erpService.CreateAssetMaster(asset);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            else
            {
                return getFailedOperation("Asset Already exits!");
            }
        }

        [HttpPost, ActionName("DeleteAsset")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAsset(int id)
        {
            var result = erpService.DeleteAsset(id);

            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }

        }

        #endregion asset

        #region checklist item

        public ActionResult CheckListItemIndex()
        {
            var cpyid = AppSession.GetCompanyId();
            //var userid = AppSession.GetCurrentUserId();
            return View(erpService.GetAllCheckListType().Where(a => a.CompanyID == cpyid).ToList());
        }


        public ActionResult CheckListItemCreate(int? id)
        {
            //ViewBag.CLid = id;
            var res = new CheckListItemTitleMasterViewModel();
            res.ChklstTypeId = id ?? default(int);
            return View(res);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateClItem(CheckListItemTitleMasterViewModel clitem)
        {
            //if (ModelState.IsValid)
            // {

            //if (erpService.CheckCLItem(clitem.ChecklistType,clitem.Frequency) == false)
            //{


           
            var id = clitem.ChklstTypeId;
            var checklisttype = erpService.GetCheckListtype(id);
            clitem.CheckListType = checklisttype.ServiceType;
            clitem.FormName = clitem.CheckListType + '-' + clitem.Frequency;
            var result = erpService.CreateCheckListItem(clitem);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            //}
            //else
            //{
            //    return getFailedOperation("Checklist Type Already exits!");
            //}
            //return views(cltype);
            //  }
        }



        [HttpPost, ActionName("DeleteCheckListItem")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCheckListItem(int id)
        {
            var result = erpService.DeleteCheckListItem(id);

            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }

        }



        public ActionResult CheckListItemEdit(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //   eng_users eng_users = db.eng_users.Find(id);
            var checklistitem = erpService.GetCheckListItem(id ?? default(int));
            if (checklistitem == null)
            {
                return HttpNotFound();
            }
            var cltype = erpService.GetAllCheckListType().Where(a => a.IsActive == 1).ToList();
            cltype.Insert(0, new CheckListTypeMasterViewModel() { ServiceType = "-Select-" });
            ViewBag.ServiceType = new SelectList(cltype, "ServiceType", "ServiceType",checklistitem.CheckListType);

            return View(checklistitem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckListItemEdit(CheckListItemTitleMasterViewModel clitem)
        {
            if (ModelState.IsValid)
            {
               

                var result = erpService.SaveCheckListItem(clitem);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            return View(clitem);
        }


        #endregion checklist item


        #region checklist child

        public ActionResult ChecklistItemChildIIndex()
        {
            //var cltype = erpService.GetAllCheckListType().Where(a => a.IsActive == 1).ToList();
            //cltype.Insert(0, new CheckListTypeMasterViewModel() { ServiceType = "-Select-" });
            //ViewBag.ServiceType = new SelectList(cltype, "ServiceType", "ServiceType");
            return View();
        }


        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

