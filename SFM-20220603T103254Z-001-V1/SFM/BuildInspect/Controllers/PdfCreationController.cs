using BuildInspect.Models.Service.Imp;
using BuildInspect.Models.Utility;
using BuildInspect.Models.ViewModel;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuildInspect.Controllers
{
    public class PdfCreationController : Controller
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IERPServices erpService;
        private readonly IUserServices userService;

        public PdfCreationController(IERPServices _erpService, IUserServices _userService)
        {
            erpService = _erpService;
            userService = _userService;
        }

        // GET: PdfCreation
        public int GeneratePDF(int id, string key, int mailflag)
        {
            try
            {
                if (key == "@@India1947!")
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
                            sub = sub + "SFM - Maintenance Inspection for the project: " + school.Building_Name + "-" + pmon + " " + styp;
                            cpy = "SFM";
                            if (school.Email != null && school.Email != "")
                            {
                                to = school.Email;
                            }
                            else
                            {
                                to = "sm.anandh@gmail.com";
                            }

                            cc = "rajeowens@gmail.com";
                        }
                        if (school.CompanyID == 2)
                        {
                            sub = sub + "SFM - Maintenance Inspection for the project: " + school.Building_Name+ "-" + pmon + " " + styp;
                            cpy = "SFM";
                            if (school.Email != null && school.Email != "")
                            {
                                to = school.Email;
                            }
                            else
                            {
                                to = "sm.anandh@gmail.com";
                            }
                            cc = "rajeowens@gmail.com";
                        }
                        if (school.CompanyID == 3)
                        {
                            sub = sub + "SFM - Maintenance Inspection for the project: " + school.Building_Name + "-" + pmon + " " + styp;
                            cpy = "SFM";
                            if (school.Email != null && school.Email != "")
                            {
                                to = school.Email;
                                //to = "project_moe@propell.com.sg";
                            }
                            else
                            {                                
                                to = "sm.anandh@gmail.com";
                            }
                            cc = "rajeowens@gmail.com";
                        }


                        var body = "Dear Sir/Madam, \n\nPlease find attached the checklist reports for the below detail.\n\n";
                       

                        //sub = sub + " Maintenance Inspection Report for the School:" + school.School_Name;

                        body = body + " Type: " + clitem.CLType + ".\n";
                        body = body + " \n Frequency: " + clitem.Frequency + ".\n";
                        body = body + " Month: " + project.MonthName + " " + Convert.ToDateTime(project.Month_Start_Date).ToString("yyyy") + ".\n";

                        body = body + " \n\nPlease feel free to approach our manager if any clarification/concern.\n\n";
                        body = body + "Thanks & Regards,\n \n";
                        body = body + cpy;
                        body = body + "\n \n **Please don't reply to this email and this is auto generated email from software.** ";
                                               
                        //to = school.Email;

                        to = to + ";" + "pandees@gmail.com";
                        //to = to + ";" + "sm.anandh@gmail.com";

                        //cc = cc + "," + "prabhu.thil@gmail.com";
                        //cc = "sowdambbikainfotechservices@gmail.com";
                        cc = cc + "," + "saran_75@yahoo.com";

                        AppSettings.sendEmail(sub, body, to, cc, mailAttach, school.CompanyID ?? default(int));

                    }


                }
                return 1;
            
            }
            catch (Exception ex)
            {
                logger.Debug("Pdf Error from Mobile:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                return 0;
            }
        }
        //[HttpGet, ActionName("mySecretAction")]
        public ActionResult PrintCheckListPdf(int id, string cltype)
        {
            
            var formName = "";
            var clm = erpService.GetCheckListItems(id);
            ViewBag.CompanyID = userService.GetUser(clm.UserID ?? default(int)).CompanyID;
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
                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName.StartsWith("Electrical") && (a.Frequency == clm.Frequency || a.Frequency == null) && a.ServiceType == cltype).OrderBy(a => a.OrderBy).ToList(); // modified order by
                }
                else if (clm.CLType == "Mechanical")
                {
                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName.StartsWith("Mechanical") && (a.Frequency == clm.Frequency || a.Frequency == null) && a.ServiceType == cltype).OrderBy(a => a.OrderBy).ToList(); // modified order by
                }
                else
                {
                    chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && (a.Frequency == clm.Frequency || a.Frequency == null) && a.ServiceType == cltype).OrderBy(a => a.OrderBy).ToList(); // modified order by
                }
                //chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && a.ServiceType == cltype && (a.Frequency == clm.Frequency || a.Frequency == null)).ToList();
            }

            //var chklst = erpService.GetAllCheckListItems().Where(a => a.FormName == formName && a.ServiceType == cltype && (a.Frequency == clm.Frequency || a.Frequency == null)).ToList();
            
            var school = erpService.GetSchool(clm.SchoolID ?? default(int));
            ViewBag.SchoolName = school.Building_Name;
            ViewBag.Address = school.Address;
            ViewBag.Tel = school.Mobile;
            ViewBag.OPMngr = school.Op_Manager_Name;
            ViewBag.Email = school.Email;
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


        public int GenerateSFPDF(int id,string key)
        {
            try
            {
                if (key == "@@India1947!")
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
                }
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
            var sfm = erpService.GetServiceForm(id);
            var school = erpService.GetSchool(sfm.SchoolID ?? default(int));
            ViewBag.SchoolName = school.Building_Name;
            ViewBag.Address = school.Address;
            ViewBag.Tel = school.Mobile;
            ViewBag.OPMngr = school.Op_Manager_Name;
            ViewBag.Email = school.Email;
            var user = userService.GetUser(sfm.UserID ?? default(int));
            ViewBag.TechName = user.DisplayName;
            return View(sfm);
        }

        public string SendMailReminder(int id, string key, int mailflag)
        {
            try
            {
                string html = "";
                var htmlstring = "";
                var pmmailid = "";
                var adminmailid = "";
                if (key == "@@India1947!")
                {
                    var now = DateTime.Now;
                    var firstdate = new DateTime(now.Year, now.Month, 1);
                    var lastdate = firstdate.AddMonths(1).AddDays(-1);
                    TimeSpan difference = lastdate - now;
                    var days = difference.TotalDays;

                    var sub = "Sub:- ";
                    string mailAttach = "Reminder";

                    if (days < 7)
                    {
                        var school = erpService.GetAllSchools().ToList().OrderBy(a => a.CompanyID);

                        var to = "";
                        var cc = "";
                        var body = "";
                        var zonelist = school.Select(a => a.Zone).Distinct().ToList();

                        foreach (var zn in zonelist)
                        {
                            try
                            {

                                var users = userService.getAllUsers().ToList();
                                var cpyid = school.Where(a => a.Zone == zn).FirstOrDefault().CompanyID;
                                html = html + zn + ",";
                                pmmailid = users.Where(a => a.Zone == zn && a.GroupID == 6).FirstOrDefault().Email;
                                adminmailid = users.Where(a => a.Zone == zn && a.GroupID == 8).FirstOrDefault().Email;

                                var clmatrix = erpService.GetReminderCLSummaryMatrix(-1, zn).ToList();

                                sub = "Date: " + now.ToShortDateString() + " - Checklist Reminder - Zone: " + zn;
                                htmlstring = RenderRazorViewToString("MailReminderCLMatrix", clmatrix);

                                body = "Dear Team, \n\nPlease find below the remider matrix.\n\n";
                                body = body + "Legend:- NA: Not applicable. X: Not Submitted.  PU/M: Pending at User OM and MA.\n\n";
                                body = body + "PU: Pending at User OM.  PM: Pending at MA\n\n";
                                body = body + htmlstring;

                                body = body + "<br /> Best Regards, <br /> Team - SBI.";

                                if (cpyid == 1)
                                {
                                    if (pmmailid != null && pmmailid != "")
                                    {
                                        to = pmmailid;
                                    }
                                    else
                                    {
                                        to = "arul@mkv.com.sg";
                                    }
                                    cc = "sbi@mkv.com.sg";
                                }

                                if (cpyid == 2)
                                {
                                    if (pmmailid != null && pmmailid != "")
                                    {
                                        to = pmmailid;
                                    }
                                    else
                                    {
                                        to = "arul@mkv.com.sg";
                                    }

                                    cc = "sbi@hec.com.sg";
                                }

                                if (cpyid == 3)
                                {
                                    if (pmmailid != null && pmmailid != "")
                                    {
                                        to = pmmailid;
                                    }
                                    else
                                    {
                                        to = "project_moe@propell.com.sg";
                                    }
                                    cc = "rskumar@timothy.com.sg";
                                }

                                if (adminmailid != null && adminmailid != "")
                                {
                                    to = to + ";" + adminmailid;
                                }

                                to = to + ";" + "sm.anandh@gmail.com";
                                to = to + ";" + "pandees@gmail.com";

                                cc = cc + "," + "rajeowens@gmail.com";

                                //to = "sm.anandh@gmail.com";
                                //cc = "rajeowens@gmail.com";
                                AppSettings.sendEmail(sub, body, to, cc, mailAttach, cpyid ?? default(int));


                                html = html + sub + "<br />Mail Sent....<br />";
                            }
                            catch (Exception ex)
                            {
                                logger.Debug("Mail Reminder error from Scheduler:");
                                logger.Debug(ex.Message);
                                logger.Debug(ex.StackTrace);
                                continue;
                            }
                        }

                        return html;
                    }
                    
                }
                return "Test OK";
            }

            catch (Exception ex)
            {
                logger.Debug("Mail Reminder outer loop error from Scheduler:");
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                return "Not OK";
               
            }
            
        }

        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}