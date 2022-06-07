using BuildInspect.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BuildInspect.Models.Domain;
using BuildInspect.Models.Service.Imp;

using AutoMapper;
using BuildInspect.Models.Utility;
using System.Threading.Tasks;
using System.Web;
using System.Globalization;
using BuildInspect.Models.Service.Interface;
using System.IO;
using NLog;
using System.Configuration;
using System.Web.Http.Cors;
using System.Web.Razor;

namespace BuildInspect.Controllers
{
    
    
    public class MobileLoginController : ApiController
    {
        private readonly IUserServices userService;       
        private readonly IEmployeeServices employeeService;
        private readonly IERPServices erpService;

        Logger logger = LogManager.GetCurrentClassLogger();
      


        public MobileLoginController(IUserServices _userService, IEmployeeServices _employeeService,
            IERPServices _erpService )
        {
            userService = _userService;            
            employeeService = _employeeService;
            erpService = _erpService;

        }
        [HttpGet]
        public LoginResponseViewModel BInLogin(string username, string password, string deviceid)
        {
            //logger.Debug(string.Format("Login Username:{0}", username));

            //SecureString secPwd = new SecureString();
            //var deviceid = "";
            try
            {
                var sKey = ConfigurationManager.AppSettings["PtStK"];
                SecurityController Scon = new SecurityController();
                var encWord = Scon.Encrypt(sKey, password);

                var returnUser = userService.ValidateUserLoginMobile(username, encWord, deviceid);
                if (returnUser != null)
                {
                    if (returnUser.IsActive == 1)
                    {
                        LoginResponseViewModel returnResponse = new Models.ViewModel.LoginResponseViewModel();
                        returnResponse.Credentials = returnUser.Password;
                        returnResponse.Success = true;
                        returnResponse.User = new BInLiteUserViewModel() { groupID = returnUser.GroupID.ToString(), userId = returnUser.UserID.ToString(), userFullName = returnUser.DisplayName,
                            userName = returnUser.UserName, CompanyID = returnUser.CompanyID.ToString(), ServiceType = returnUser.ServiceType, Zone = returnUser.Zone, Password = password };

                        return returnResponse;
                    }
                    else
                    {

                    }

                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            return new LoginResponseViewModel();
            //return authService.SingleLogin(username, password);
        }

        public CheckListTransMasterMobileViewModel GetCheckList(int userid, int PrjMasID, int SchoolID, string Frequency)
        {            
            try
            {
                //Frequency = "HalfYearly";

                var clm = new CheckListTransMasterMobileViewModel();
                var chklst = new List<CheckListItemMasterViewModel>();
                var prj = erpService.GetProject(PrjMasID);
                //string[] elecArr = { "Electrical", "SP-FIREALMSYS", "SP-HVLSFMPIS", "SP-SMKCTLSYS", "SP-GENSET", "SP-SIOCDSYS" };
                string[] elecArr = { "Electrical", "SP-FIREALMSYS", "SP-SMKCTLSYS", "SP-GENSET", "SP-SIOCDSYS" };
                string[] mechArr = { "Mechanical", "SP-PLCWDWL", "SP-HYDPUMP", "SP-FUMECESYS", "SP_FIRESPNKLR" };

                var user = userService.GetUser(userid);
                var servicetype = user.ServiceType;                
                var school = erpService.GetSchool(SchoolID);

                clm.SchoolName = school.Building_Name;
                clm.Address = school.Address;
                clm.Tel = school.Mobile;
                clm.OPMngr = school.Op_Manager_Name;
                clm.Email = school.Email;
                if (servicetype == "SP-LIFTSYS")
                {
                    clm.LIFT_count = erpService.GetLiftCount(SchoolID, userid, prj.Month_Start_Date ?? default(DateTime));
                }
                //clm.LIFT_count = school.LIFT_count ?? default(int);
                clm.HVLSFan_count = school.HVLSFan_count ?? default(int);
                clm.CHILLER_count = school.CHILLER_count ?? default(int);
                var formName = "";
                var title = "";
                if (servicetype == "Electrical")
                {
                    formName = "Electrical-" + Frequency;
                    title = servicetype + " & Domestic";
                    var applicCL = erpService.GetProjectsForApplicableCLs(SchoolID, prj.Month_Start_Date ?? default(DateTime)).Where(a => elecArr.Contains(a.CLType)).Select(a => a.CLType).ToList();
                    //chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && applicCL.Contains(a.ServiceType)).OrderBy(a => a.OrderBy).ToList();
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
                    //chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && applicCL.Contains(a.ServiceType)).OrderBy(a => a.OrderBy).ToList();
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
                    //chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName).OrderBy(a => a.OrderBy).ToList();
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
                clm.Title = Title;

                clm.CheckListItems = chklst;                
                clm.Frequency = Frequency;
                clm.CLType = user.ServiceType;
                clm.UserID = user.UserID;
                clm.SchoolID = SchoolID;
                clm.PrjMasID = PrjMasID;

                return clm;

            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            //return new List<CheckListItemMasterViewModel>();
            
        }

        public List<BuildingMasterViewModel> GetAllSchools(int CompanyID)
        {
            try
            {
                var sch = new List<BuildingMasterViewModel>();                
                sch = erpService.GetAllSchools().Where(a => a.IsActive == 1 && a.CompanyID == CompanyID).ToList();              

                return sch;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
            //return new List<CheckListItemMasterViewModel>();

        }

        [HttpPost]        
        public ResponseViewModel SubmitCheckList(CheckListTransMasterViewModel inputCLTM)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                string halfPath = "/images/CheckListFiles/";                

                inputCLTM.Status_Flag = 0;
                inputCLTM.CreatedDate = DateTime.Now;
                //response.Success = false;
                //return response;
                var user = userService.GetUser(inputCLTM.UserID ?? default(int));
                if (user.ServiceType != "SP-LIFTSYS")
                {
                    var submitFlag = erpService.CheckCLAlreadySubmitted(inputCLTM.PrjMasID ?? default(int), inputCLTM.SchoolID ?? default(int));
                    if (submitFlag)
                    {
                        response.ErrorMessage = "Check List already Submitted";
                        response.Success = false;
                        return response;
                    }
                }

                var prj = erpService.GetProject(inputCLTM.PrjMasID ?? default(int));
                inputCLTM.CLMonth = prj.Month_Start_Date;

                var result = erpService.SubmitChecklistForMobile(inputCLTM, inputCLTM.clTrnDetail, Path.Combine(HttpContext.Current.Server.MapPath("~/images/CheckListFiles/")), halfPath);
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
                    

                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Submit CheckList Error:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
            }

            return response;

        }

        [HttpPost]
        public ResponseViewModel SubmitCheckListOffline(CheckListTransMasterViewModel inputCLTM)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                var secUrl = ConfigurationManager.AppSettings["PdfWebUrl"];
                string halfPath = "/images/CheckListFiles/";

                inputCLTM.Status_Flag = 0;
                inputCLTM.CreatedDate = DateTime.Now;

                var user = userService.GetUser(inputCLTM.UserID ?? default(int));
                if (user.ServiceType != "SP-LIFTSYS")
                {
                    var submitFlag = erpService.CheckCLAlreadySubmitted(inputCLTM.PrjMasID ?? default(int), inputCLTM.SchoolID ?? default(int));
                    if (submitFlag)
                    {
                        response.ErrorMessage = "Check List already Submitted";
                        response.Success = false;
                        return response;
                    }
                }

                var prj = erpService.GetProject(inputCLTM.PrjMasID ?? default(int));
                inputCLTM.CLMonth = prj.Month_Start_Date;

                var result = erpService.SubmitChecklistForMobileOffline(inputCLTM, inputCLTM.clTrnDetail, Path.Combine(HttpContext.Current.Server.MapPath("~/images/CheckListFiles/")), halfPath);
                if (result > 0)
                {
                    WebClient wc = new WebClient();
                    wc.DownloadData(secUrl + "PdfCreation/GeneratePDF?id=" + result + "&key=@@India1947!&mailflag=2");
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Submit CheckList Error:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
            }

            return response;

        }

        public List<CheckListIndexMobileViewModel> GetCheckListIndex(int userid)
        {
            try
            {
                var user = userService.GetUser(userid);
                var chklst = new List<CheckListIndexMobileViewModel>();
                //Project Manager or M&E
                if (user.GroupID == 6)
                {
                    var sts = "";
                    List<int> schoolids = new List<int>();
                    var schools = erpService.GetAllSchools().ToList();
                    schoolids = schools.Where(a => a.Zone == user.Zone && a.CompanyID == user.CompanyID).Select(a => a.BuildingID).ToList();
                    // var chkLists = erpService.GetAllSubmittedCheckList_School(schoolids).Where(a => a.CheckedBy_Signature != null).ToList();
                    var chkLists = erpService.GetAllSubmittedCheckList_School(schoolids).Where(a => a.CheckedBy_Signature != null && (a.EndoresBy_Signature == null || a.VerifiedBy_Signature == null )).OrderBy(a => a.CLTMID).Take(60);
                    foreach (var chl in chkLists)
                    {
                        if (chl.EndoresBy_Signature == null)
                        {
                            sts = "Pending User";
                        }
                        else if (chl.VerifiedBy_Signature == null)
                        {
                            sts = "Pending MA";
                        }
                        else
                        {
                            sts = "Completed";
                        }

                        if (chl.VerifiedBy_Signature == null && chl.EndoresBy_Signature == null)
                        {
                            sts = "Pending User & MA";
                        }

                        var sch = schools.Where(a => a.BuildingID == chl.SchoolID).FirstOrDefault();
                        var projmas = erpService.GetProject(chl.PrjMasID ?? default(int)).Month_End_Date;
                        chklst.Add(new CheckListIndexMobileViewModel()
                        {
                            CLTMID = chl.CLTMID,
                            PrjMasID = chl.PrjMasID.Value,
                            SchoolID = chl.SchoolID.Value,
                            Status = sts,
                            Zone = sch.Zone,
                            SchoolName = sch.Building_Name,
                            Frequency = chl.Frequency,
                            CLType = chl.CLType,
                            Month_Name = projmas.Value.ToString("MMMM yyyy")
                        });

                    }

                }


                //Team Lead
                //if (user.GroupID == 7)
                //{
                //    var sts = "";
                //    List<int> schoolids = new List<int>();
                //    var schools = erpService.GetAllSchools().ToList();
                //    List<int> userids = userService.getAllUsers().Where(a => a.TeamLead_ID == userid).Select(a => a.UserID).ToList();
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
                //        var sch = schools.Where(a => a.SchoolID == chl.SchoolID).FirstOrDefault();

                //        chklst.Add(new CheckListIndexMobileViewModel()
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

                //MA
                if (user.GroupID == 5)
                {
                    //var sts = "";
                    //List<int> schoolids = new List<int>();
                    //var schools = erpService.GetAllSchools().ToList();
                    //schoolids = schools.Where(a => a.Zone == user.Zone && a.CompanyID == user.CompanyID).Select(a => a.SchoolID).ToList();
                    //var chkLists = erpService.GetAllSubmittedCheckList_School(schoolids).Where(a => a.CheckedBy_Signature != null && a.VerifiedBy_Signature == null).OrderBy(a=>a.CLTMID).Take(75);
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
                    //    chklst.Add(new CheckListIndexMobileViewModel()
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

                    chklst = erpService.GetMASearchChecklistIndex_Mobile(user.CompanyID ?? default(int), user.Zone, user.GroupID ?? default(int)).OrderBy(a => a.CLTMID).Take(75).ToList();

                }


                //User/Technician login
                if (user.GroupID == 3)
                {
                    List<int> schoolids = new List<int>();
                    List<int> pasrs_schoolids = new List<int>();

                    if (user.ServiceType == "Electrical" || user.ServiceType == "Mechanical")
                    {
                        var now = DateTime.Now;
                        var firstdate = new DateTime(now.Year, now.Month, 1);
                        var lastdate = firstdate.AddMonths(1).AddDays(-1);
                        var curmon = lastdate.Month;

                        //        if (user.Zone != "ALL")
                        //        {
                        //            schoolids = erpService.GetAllSchools().Where(a => a.Zone == user.Zone).Select(a => a.SchoolID).ToList();
                        //        }
                        //        else
                        //        {
                        //            schoolids = erpService.GetAllSchools().Where(a => a.CompanyID == user.CompanyID).Select(a => a.SchoolID).ToList();
                        //        }

                        //var liftschoolids = erpService.GetApplicableSchoolIDs(schoolids, user.ServiceType, user.UserID).ToList();


                        //    var prjs = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date <= lastdate).ToList();
                        //    //var prjs = erpService.GetAllProjects().Where(a => schoolids.Contains(a.SchoolID ?? default(int)) && a.Month_End_Date <= lastdate).ToList();

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
                        //    if (obj.c != null)
                        //    {
                        //        if (obj.c.CLTMID != 0)
                        //        {
                        //            if (obj.c.UserID == userid || user.Designation == "TL")
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
                        //                if (sts== "Initiated" || sts== "Pending User" || sts== "Pending User & MA")
                        //                chklst.Add(new CheckListIndexMobileViewModel()
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
                        //        chklst.Add(new CheckListIndexMobileViewModel()
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
                        chklst = erpService.GetUserChecklistIndex_Mobile(user.CompanyID ?? default(int), userid, firstdate, lastdate).Where(a => a.Status != "Completed").ToList();
                        if (user.ServiceType == "Electrical" && user.CompanyID <= 2)
                        {
                            if (curmon != 6)
                            {
                                var nxtyr = AppSettings.GetNextYearly(lastdate);
                                var prjsB = erpService.GetUserChecklistIndex_Future_Mobile(user.CompanyID ?? default(int), userid, firstdate, nxtyr).ToList();
                                chklst.AddRange(prjsB);
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
                        //    schoolids = erpService.GetAllSchoolsFromSchoolUSerMappings(user.UserID, user.ServiceType).Select(a => a.SchoolID ?? default(int)).ToList();
                        //}
                        //if (user.ServiceType == "SP-PASISCM")
                        //{
                        //    pasrs_schoolids = erpService.GetAllSchoolsFromSchoolUSerMappings(user.UserID, user.ServiceType).Select(a => a.SchoolID ?? default(int)).ToList();
                        //}

                        //var liftschoolids = erpService.GetApplicableSchoolIDs(schoolids, user.ServiceType, user.UserID).ToList();



                        //List<int> newList = liftschoolids.Where(v => v != null).Select(v => v.Value).ToList();
                        //var prjs = new List<ProjectMasterViewModel>();
                        //if (user.ServiceType == "SP-LIFTSYS")
                        //{
                        //    prjs = erpService.GetLiftProjects(schoolids, user.DisplayName).Where(a => a.Month_End_Date <= lastdate).ToList();
                        //}
                        //else if (user.ServiceType == "SP-SECUTYSYS")
                        //{
                        //    var nxtqtr = AppSettings.GetNextQuartelry(lastdate);
                        //    prjs = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date <= nxtqtr).ToList();
                        //}
                        //else if (user.ServiceType == "SP-WETCHSUSYS")
                        //{
                        //    var nxtyr = AppSettings.GetNextYearly(lastdate);
                        //    prjs = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date <= nxtyr).ToList();
                        //}
                        //else if (user.ServiceType == "SP-PASISCM")
                        //{

                        //    prjs = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date <= lastdate).ToList();

                        //    List<ProjectMasterViewModel> newprjvm = new List<ProjectMasterViewModel>();
                        //    foreach (var pr in prjs)
                        //    {
                        //        if (pr.Month_End_Date.Value.Month != 12 && pr.Month_End_Date.Value.Month != 6)
                        //        {
                        //            if (pasrs_schoolids.Contains(pr.SchoolID ?? default(int)))
                        //                newprjvm.Add(
                        //                    new ProjectMasterViewModel()
                        //                    {
                        //                        PrjMasID = pr.PrjMasID,
                        //                        SchoolID = pr.SchoolID,
                        //                        MonthName = pr.MonthName,
                        //                        CLTMID = pr.CLTMID,
                        //                        CLType = pr.CLType,
                        //                        Frequency = pr.Frequency,
                        //                        Is_Completed = pr.Is_Completed,
                        //                        Is_Rescheduled = pr.Is_Rescheduled,
                        //                        Lift_Count = pr.Lift_Count,
                        //                        Lift_Opr_Name = pr.Lift_Opr_Name,
                        //                        Month_End_Date = pr.Month_End_Date,
                        //                        Month_Start_Date = pr.Month_Start_Date,
                        //                        Rescheduled_Date = pr.Rescheduled_Date,
                        //                        Rescheduled_Remarks = pr.Rescheduled_Remarks
                        //                    }
                        //                    );
                        //        }
                        //        else
                        //        {
                        //            newprjvm.Add(
                        //                   new ProjectMasterViewModel()
                        //                   {
                        //                       PrjMasID = pr.PrjMasID,
                        //                       SchoolID = pr.SchoolID,
                        //                       MonthName = pr.MonthName,
                        //                       CLTMID = pr.CLTMID,
                        //                       CLType = pr.CLType,
                        //                       Frequency = pr.Frequency,
                        //                       Is_Completed = pr.Is_Completed,
                        //                       Is_Rescheduled = pr.Is_Rescheduled,
                        //                       Lift_Count = pr.Lift_Count,
                        //                       Lift_Opr_Name = pr.Lift_Opr_Name,
                        //                       Month_End_Date = pr.Month_End_Date,
                        //                       Month_Start_Date = pr.Month_Start_Date,
                        //                       Rescheduled_Date = pr.Rescheduled_Date,
                        //                       Rescheduled_Remarks = pr.Rescheduled_Remarks
                        //                   }
                        //                   );
                        //        }
                        //    }
                        //    prjs = newprjvm;
                        //    if (curmon != 12 && curmon != 6)
                        //    {
                        //        var nxthfyr = AppSettings.GetNextHalfYearly(lastdate);
                        //        var prjsB = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date == nxthfyr).ToList();
                        //        prjs.AddRange(prjsB);
                        //    }

                        //}
                        //else if (user.ServiceType == "SP-HVLSFMPIS")
                        //{
                        //    prjs = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date <= lastdate).ToList();

                        //    if (curmon != 12)
                        //    {
                        //        var nxthfyr = AppSettings.GetNextHalfYearly(lastdate);
                        //        var prjsB = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date == nxthfyr).ToList();
                        //        prjs.AddRange(prjsB);

                        //    }
                        //}
                        //else if (user.ServiceType == "SP-FUMECESYS")
                        //{
                        //    prjs = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date <= lastdate).ToList();

                        //    if (curmon != 12)
                        //    {
                        //        var nxthfyr = AppSettings.GetNextHalfYearly(lastdate);
                        //        var prjsB = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date == nxthfyr).ToList();
                        //        prjs.AddRange(prjsB);

                        //    }
                        //}
                        //else
                        //{
                        //    prjs = erpService.GetAllProjectsOtherthanLift(schoolids, user.ServiceType).Where(a => a.Month_End_Date <= lastdate).ToList();
                        //}


                        ////var prjs = erpService.GetAllProjects().Where(a => liftschholids.Contains(a.SchoolID ?? default(int)) && a.Month_End_Date <= lastdate).ToList();
                        ////var chkLists = erpService.GetAllSubmittedCheckList_School(newList).ToList();

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
                        //            if (obj.c.UserID == userid || user.Designation == "TL")
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
                        //                if (sts == "Initiated" || sts == "Pending User" || sts == "Pending User & MA")
                        //                chklst.Add(new CheckListIndexMobileViewModel()
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
                        //        chklst.Add(new CheckListIndexMobileViewModel()
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

                        if (user.ServiceType == "SP-LIFTSYS")
                        {
                            chklst = erpService.GetUserChecklistIndex_Mobile(user.CompanyID ?? default(int), userid, firstdate, lastdate).Where(a => a.Status != "Completed").ToList();
                        }
                        else if (user.ServiceType == "SP-SECUTYSYS")
                        {
                            var nxtqtr = AppSettings.GetNextQuartelry(lastdate);
                            chklst = erpService.GetUserChecklistIndex_Mobile(user.CompanyID ?? default(int), userid, firstdate, nxtqtr).Where(a => a.Status != "Completed").ToList();
                        }
                        else if (user.ServiceType == "SP-WETCHSUSYS")
                        {
                            var nxtyr = AppSettings.GetNextYearly(lastdate);
                            chklst = erpService.GetUserChecklistIndex_Mobile(user.CompanyID ?? default(int), userid, firstdate, nxtyr).ToList();
                        }
                        else if (user.ServiceType == "SP-CHILLERSYS")
                        {
                            chklst = erpService.GetUserChecklistIndex_Mobile(user.CompanyID ?? default(int), userid, firstdate, lastdate).ToList();
                        }
                        else if (user.ServiceType == "SP-HVLSFMPIS")
                        {
                            chklst = erpService.GetUserChecklistIndex_Mobile(user.CompanyID ?? default(int), userid, firstdate, lastdate).ToList();
                            if (curmon != 12)
                            {
                                var nxthfyr = AppSettings.GetNextHalfYearly(lastdate);
                                var prjsB = erpService.GetUserChecklistIndex_Future_Mobile(user.CompanyID ?? default(int), userid, firstdate, nxthfyr).ToList();
                                chklst.AddRange(prjsB);
                            }
                        }
                        else if (user.ServiceType == "SP-FUMECESYS")
                        {
                            chklst = erpService.GetUserChecklistIndex_Mobile(user.CompanyID ?? default(int), userid, firstdate, lastdate).ToList();
                            if (curmon != 12)
                            {
                                var nxthfyr = AppSettings.GetNextHalfYearly(lastdate);
                                var prjsB = erpService.GetUserChecklistIndex_Future_Mobile(user.CompanyID ?? default(int), userid, firstdate, nxthfyr).ToList();
                                chklst.AddRange(prjsB);
                            }
                        }
                        else if (user.ServiceType == "SP-PASISCM")
                        {
                            chklst = erpService.GetUserChecklistIndex_Mobile(user.CompanyID ?? default(int), userid, firstdate, lastdate).ToList();
                            if (curmon != 12 && curmon != 6)
                            {
                                var nxthfyr = AppSettings.GetNextHalfYearly(lastdate);
                                var prjsB = erpService.GetUserChecklistIndex_Future_Mobile(user.CompanyID ?? default(int), userid, firstdate, nxthfyr).ToList();
                                chklst.AddRange(prjsB);
                            }

                        }
                        else
                        {
                            //chklst = erpService.GetUserChecklistIndex_Mobile(user.CompanyID ?? default(int), userid, firstdate, lastdate).Where(a => a.Status != "Completed").ToList();
                            chklst = erpService.GetUserChecklistIndex_Mobile(user.CompanyID ?? default(int), userid, firstdate, lastdate).Where(a => a.Status != "Completed").OrderBy(a => a.CLTMID).Take(75).ToList();
                                                        
                        }


                    }

                }

                return chklst;

            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //secPwd.Dispose();
            }
           

        }

        public CheckListTransMasterMobileViewModel GetChecklistForEdit(int id)
        {
            try
            {
                //string[] elecArr = { "Electrical", "SP-FIREALMSYS", "SP-HVLSFMPIS", "SP-SMKCTLSYS", "SP-GENSET", "SP-SIOCDSYS" };
                string[] elecArr = { "Electrical", "SP-FIREALMSYS", "SP-SMKCTLSYS", "SP-GENSET", "SP-SIOCDSYS" };
                string[] mechArr = { "Mechanical", "SP-PLCWDWL", "SP-HYDPUMP", "SP-FUMECESYS", "SP_FIRESPNKLR" };
                var chklst = new List<CheckListItemMasterViewModel>();

                //var cpyid = AppSession.GetCompanyId();
                var formName = "";
                var clm = erpService.GetCheckListItemsforMobile(id);
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
                clm.SchoolName = school.Building_Name;
                clm.Address = school.Address;
                clm.Tel = school.Mobile;
                clm.OPMngr = school.Op_Manager_Name;
                clm.Email = school.Email;
                if (clm.CLType == "SP-LIFTSYS")
                {
                    clm.LIFT_count = erpService.GetLiftCount(clm.SchoolID ?? default(int), clm.UserID ?? default(int), prj.Month_Start_Date ?? default(DateTime));
                }
                //clm.LIFT_count = school.LIFT_count ?? default(int);
                clm.HVLSFan_count = school.HVLSFan_count ?? default(int);
                clm.CHILLER_count = school.CHILLER_count ?? default(int);

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
                clm.Title = Title;

                foreach (var cli in clm.chklst_trn_detail)
                {
                    var obj = chklst.Where(a => a.ChkListID == cli.ChkListID).SingleOrDefault();
                    obj.IsDone = cli.IsDone;
                    obj.DateDone = cli.DateDone;
                    obj.Remarks = cli.Remarks;
                    obj.CLTDID = cli.CLTDID;
                }

                //new SelectList(school, "SchoolID", "School_Name");

                clm.CheckListItems = chklst;

                return clm;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        [HttpPost]
        public ResponseViewModel SubmitSignatureCheckList(CheckListTransMasterViewModel inputCLTM)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                var secUrl = ConfigurationManager.AppSettings["PdfWebUrl"];
                var result = erpService.SubmitSignatureCheckListForMobile(inputCLTM);

                var clm = erpService.GetCheckListItems(inputCLTM.CLTMID);
                if (clm.Status_Flag == 4)
                {
                    WebClient wc = new WebClient();
                    wc.DownloadData(secUrl + "PdfCreation/GeneratePDF?id=" + inputCLTM.CLTMID + "&key=@@India1947!&mailflag=1");
                }
                if (clm.Status_Flag == 2)
                {
                    WebClient wc = new WebClient();
                    wc.DownloadData(secUrl + "PdfCreation/GeneratePDF?id=" + inputCLTM.CLTMID + "&key=@@India1947!&mailflag=2");
                }

                if (result > 0)
                {
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Submit Signature Error:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
            }

            return response;

        }


        [HttpPost]
        public ResponseViewModel SubmitMASignatureCheckList(CheckListTransMasterViewModel inputCLTM)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                var secUrl = ConfigurationManager.AppSettings["PdfWebUrl"];
                var result = erpService.SubmitMASignatureCheckListForMobile(inputCLTM);

                var clm = erpService.GetCheckListItems(inputCLTM.CLTMID);
                if (clm.Status_Flag == 4)
                {
                    WebClient wc = new WebClient();
                    wc.DownloadData(secUrl + "PdfCreation/GeneratePDF?id=" + inputCLTM.CLTMID + "&key=@@India1947!&mailflag=3");
                }
                if (result > 0)
                {
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Submit Signature Error:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
            }

            return response;

        }

        public List<ServiceFormViewModel> GetServiceFormIndex(int userid)
        {
            try
            {
                var user = userService.GetUser(userid);   

                List<ServiceFormViewModel> svfvm = new List<ServiceFormViewModel>();
                if (user.GroupID == 3)
                {
                    svfvm = erpService.GetAllServiceForms().Where(a => a.UserID == userid).ToList();
                }
                if (user.GroupID == 8)
                {
                    svfvm = erpService.GetAllServiceForms().Where(a => a.AssignedBy == userid).ToList();
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

                return svfvm;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        [HttpPost]        
        public ResponseViewModel NewServiceForm(ServiceFormViewModel inputSF)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {                
                inputSF.AssignedBy = userService.GetUser(inputSF.UserID ?? default(int)).Admin_ID;
                inputSF.Status_Flag = 1;
                var result = erpService.AssignService(inputSF);
                //var result = 2;
                if (result > 0)
                {
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("New Service Form Error:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
            }

            return response;

        }

        public ServiceFormViewModel GetServiceForm(int id)
        {
            try
            {
                var svform = erpService.GetServiceForm(id);
                var school = erpService.GetSchool(svform.SchoolID ?? default(int));
                svform.SchoolName = school.Building_Name;
                svform.Address = school.Address;

                return svform;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]        
        public ResponseViewModel ServiceFormSave(ServiceFormMobileViewModel inputSF)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                var secUrl = ConfigurationManager.AppSettings["PdfWebUrl"];
                string halfPath = "/images/ServiceFormFiles/" + inputSF.UserID + "/";

                var result = erpService.ServiceFormMobileSave(inputSF, Path.Combine(HttpContext.Current.Server.MapPath("~/images/ServiceFormFiles/" + inputSF.UserID + "/")), halfPath);
                var sfm = erpService.GetServiceForm(inputSF.ServiceID);
                                
                if (sfm.Status_Flag == 2)
                {
                    WebClient wc = new WebClient();
                    wc.DownloadData(secUrl + "PdfCreation/GenerateSFPDF?id=" + inputSF.ServiceID + "&key=@@India1947!");
                }
                //var result = 2;
                if (result > 0)
                {
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Save Service Form Error:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
            }

            return response;

        }


        [HttpPost]
        public ResponseViewModel ServiceFormCreateOffline(ServiceFormMobileViewModel inputSF)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                inputSF.AssignedBy = userService.GetUser(inputSF.UserID ?? default(int)).Admin_ID;

                if (inputSF.Customer_incharge_Signature != null)
                    inputSF.Status_Flag = 2;
               
                var secUrl = ConfigurationManager.AppSettings["PdfWebUrl"];
                string halfPath = "/images/ServiceFormFiles/" + inputSF.UserID + "/";
                var result = 0;
                if (inputSF.ServiceID == 0)
                {
                   result = erpService.ServiceFormMobileOffline(inputSF, Path.Combine(HttpContext.Current.Server.MapPath("~/images/ServiceFormFiles/" + inputSF.UserID + "/")), halfPath);
                }

                if (inputSF.ServiceID > 0)
                {
                    result = erpService.ServiceFormMobileOfflineSave(inputSF, Path.Combine(HttpContext.Current.Server.MapPath("~/images/ServiceFormFiles/" + inputSF.UserID + "/")), halfPath);
                }

                if (result > 0)
                {
                    var sfm = erpService.GetServiceForm(result);
                    if (sfm.Status_Flag == 2)
                    {
                        WebClient wc = new WebClient();
                        wc.DownloadData(secUrl + "PdfCreation/GenerateSFPDF?id=" + result + "&key=@@India1947!");
                    }
                }
                //var result = 2;
                if (result > 0)
                {
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Save Service Form offline Error:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
            }

            return response;

        }

        public List<DashboardSummaryViewModel> GetDashboardSummary(int userid)
        {
            try
            {
                var user = userService.GetUser(userid);
                var now = DateTime.Now;
                 var firstdate = new DateTime(now.Year, now.Month, 1);
                 var lastdate = firstdate.AddMonths(1).AddDays(-1);

            List<DashboardSummaryViewModel> dbs = new List<DashboardSummaryViewModel>();
            dbs.Add(new DashboardSummaryViewModel()
            {
                 item_desc = "PendingSubmission", Frequency = "Monthly", itemcount = 0
            });
            dbs.Add(new DashboardSummaryViewModel()
            {
                item_desc = "PendingSubmission",Frequency = "Quarterly",itemcount = 0
            });
            dbs.Add(new DashboardSummaryViewModel()
            {
                item_desc = "PendingSubmission",Frequency = "HalfYearly",itemcount = 0
            });
            dbs.Add(new DashboardSummaryViewModel()
            {
                item_desc = "PendingSubmission",Frequency = "Yearly",itemcount = 0
            });
            dbs.Add(new DashboardSummaryViewModel()
            {
                item_desc = "PendingSubmission",Frequency = "BiYearly",itemcount = 0
            });


            dbs.Add(new DashboardSummaryViewModel()
            {
                 item_desc = "PendingUser", Frequency = "Monthly", itemcount = 0
            });
            dbs.Add(new DashboardSummaryViewModel()
            {
                item_desc = "PendingUser",Frequency = "Quarterly",itemcount = 0
            });
            dbs.Add(new DashboardSummaryViewModel()
            {
                item_desc = "PendingUser",Frequency = "HalfYearly",itemcount = 0
            });
            dbs.Add(new DashboardSummaryViewModel()
            {
                item_desc = "PendingUser",Frequency = "Yearly",itemcount = 0
            });
            dbs.Add(new DashboardSummaryViewModel()
            {
                item_desc = "PendingUser",Frequency = "BiYearly",itemcount = 0
            });


            dbs.Add(new DashboardSummaryViewModel()
            {
                 item_desc = "PendingMA", Frequency = "Monthly", itemcount = 0
            });
            dbs.Add(new DashboardSummaryViewModel()
            {
                item_desc = "PendingMA",Frequency = "Quarterly",itemcount = 0
            });
            dbs.Add(new DashboardSummaryViewModel()
            {
                item_desc = "PendingMA",Frequency = "HalfYearly",itemcount = 0
            });
            dbs.Add(new DashboardSummaryViewModel()
            {
                item_desc = "PendingMA",Frequency = "Yearly",itemcount = 0
            });
            dbs.Add(new DashboardSummaryViewModel()
            {
                item_desc = "PendingMA",Frequency = "BiYearly",itemcount = 0
            });



            dbs.Add(new DashboardSummaryViewModel()
            {
                 item_desc = "Completed", Frequency = "Monthly", itemcount = 0
            });
            dbs.Add(new DashboardSummaryViewModel()
            {
                item_desc = "Completed",Frequency = "Quarterly",itemcount = 0
            });
            dbs.Add(new DashboardSummaryViewModel()
            {
                item_desc = "Completed",Frequency = "HalfYearly",itemcount = 0
            });
            dbs.Add(new DashboardSummaryViewModel()
            {
                item_desc = "Completed",Frequency = "Yearly",itemcount = 0
            });
            dbs.Add(new DashboardSummaryViewModel()
            {
                item_desc = "Completed",Frequency = "BiYearly",itemcount = 0
            });


            var dbsres = erpService.GetDashboardSummary(user.UserID, user.GroupID ?? default(int) , user.CompanyID ?? default(int), firstdate, lastdate);
            foreach (var loop in dbsres)
            {
                dbs.Where(a => a.item_desc == loop.item_desc && a.Frequency == loop.Frequency).FirstOrDefault().itemcount = loop.itemcount;
            }

                return dbs;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
