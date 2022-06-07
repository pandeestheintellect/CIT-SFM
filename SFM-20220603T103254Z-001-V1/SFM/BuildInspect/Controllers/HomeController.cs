using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BuildInspect.Models.Security;
using BuildInspect.Models.Service.Imp;
using BuildInspect.Models.Service.Interface;
using BuildInspect.Models.Utility;
using BuildInspect.Models.ViewModel;
using System.IO;
using System.Configuration;
using System.Net;

namespace BuildInspect.Controllers
{
    [AccessDeniedAuthorize]
    public class HomeController : SuperBaseController
    {
        private readonly IUserServices userService;
        private readonly IERPServices erpService;

        public HomeController(UserServices _userService, IERPServices _erpService)
        {
            userService = _userService;
            erpService = _erpService;
        }

        public ActionResult Index()
        {
            var groupid = Models.Utility.AppSession.GetCurrentUserGroup();
            var userid = Models.Utility.AppSession.GetCurrentUserId();
            var Ivm = new IndexViewModel();
            var user = userService.GetUser(userid);
            List<int> schoolids = new List<int>();
            Ivm.Zone = user.Zone;
            Ivm.UserType = user.ServiceType;

                if (user.Zone != "ALL")
                    Ivm.NoOfSchools = erpService.GetAllSchools().Where(a => a.Zone == user.Zone).Count();
                else
                    Ivm.NoOfSchools = erpService.GetAllSchools().Where(a => a.CompanyID == user.CompanyID).Count();

            if (user.ServiceType == "SP-LIFTSYS")
            {
                if (user.Zone != "ALL")
                {
                    schoolids = erpService.GetAllSchools().Where(a => a.Zone == user.Zone).Select(a => a.BuildingID).ToList();
                }
                else
                {
                    schoolids = erpService.GetAllSchools().Where(a => a.CompanyID == user.CompanyID).Select(a => a.BuildingID).ToList();
                }
                Ivm.NoOfSchools = erpService.GetLiftProjects(schoolids, user.DisplayName).Select(a => a.BuildingID).Distinct().Count();
                //Ivm.NoOfSchools = erpService.GetAllSchools().Where(a => a.CompanyID == user.CompanyID).Count();
            }

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

            Ivm.dbsvm = dbs;
            Ivm.DisplayName = user.DisplayName;

            return View(Ivm);
        }
        public ActionResult Navbar()
        {            
            return PartialView();
        }
        public ActionResult LeftMenu()
        {
            List<ModuleViewModel> lstModules = userService.GetMenuScreenNames(AppSession.GetCurrentUserId());
          
            return PartialView(lstModules);
            //return PartialView();
        }

        public ActionResult profile_modal()
        {
            var uid = AppSession.GetCurrentUserId();
            var user = userService.GetUser(uid);
            var sKey = ConfigurationManager.AppSettings["PtStK"];
            SecurityController Scon = new SecurityController();
            var plnPwd = Scon.Decrypt(sKey, user.Password);
            user.Password = plnPwd;

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(UserViewModel Bin_user)
        {
            ModelState.Remove("Email");
            if (ModelState.IsValid)
            {
                var sKey = ConfigurationManager.AppSettings["PtStK"];
                SecurityController Scon = new SecurityController();
                var encWord = Scon.Encrypt(sKey, Bin_user.Curr_Password);

                if (userService.VerifyCurrentPassword(Bin_user.UserID, encWord))
                {
                    //Session["success"] = "Profile updated successfully";
                    var newPwd = Scon.Encrypt(sKey, Bin_user.Password);
                    var res = userService.UpdateProfile(Bin_user, newPwd);
                    return getSuccessfulOperation();
                }
                else
                {
                    //Session["error"] = "Incorrect Password";
                    return getFailedOperation("Incorrect Password");
                }
                
            }
            return getFailedOperation();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        // GET: Company
        //public ActionResult CompanyIndex()
        //{
        //    var company = userService.getCompany();
        //    return View(company);
        //}

        //[HttpPost]
        //public ActionResult CompanyIndex(CompanyMasterViewModel company)
        //{
        //    string halfPath = "/images/CompanyLogo/logo.png";


        //    var path = Path.Combine(Server.MapPath("~/images/CompanyLogo/"), "logo.png");
        //    if (company.profile_Path != null)
        //    {
        //        company.profile_Path.SaveAs(path);

        //    }
        //    company.LogoPath = path;

        //    var result = userService.CreateAndUpdateCompany(company);
        //    if (result > 0)
        //    {
        //        return getSuccessfulOperation();
        //    }
        //    else
        //    {
        //        return getFailedOperation();
        //    }

        //    // return View();
        //}


        #region Company

        // GET: Company
        public ActionResult CompanyIndex()
        {
            return View(userService.GetAllCompanies().Where(a => a.IsActive == 1).ToList());
        }

        public ActionResult CompanyCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompanyCreate(CompanyMasterViewModel company)
        {
           
                
                var path = Path.Combine(Server.MapPath("~/images/CompanyLogo/"));
                company.IsActive = 1;
                company.CreatedBy = AppSession.GetCurrentUserId();
                company.CreatedDate = DateTime.Now;

                var result = userService.CreateCompany(company, path);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
        }

        public ActionResult CompanyEdit(int? id)
        {            
            var cpy = userService.GetCompany(id ?? default(int));
            if (cpy == null)
            {
                return HttpNotFound();
            }
            return View(cpy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompanyEdit(CompanyMasterViewModel company)
        {
            if (ModelState.IsValid)
            {
                var path = Path.Combine(Server.MapPath("~/images/CompanyLogo/"));
                company.UpdatedBy = AppSession.GetCurrentUserId();
                company.UpdatedDate = DateTime.Now;

                var result = userService.SaveCompany(company, path);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            return View(company);
        }


        // POST: User/Delete/5
        [HttpPost, ActionName("DeleteCompany")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCompany(int id)
        {
            var result = userService.DeleteCompany(id);

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

        #region license


        public ActionResult LicenseIndex()
        {
            var uid = AppSession.GetCurrentUserId();
            var gid = AppSession.GetCurrentUserGroup();
            var cid = AppSession.GetCompanyId();

            var lst = (userService.getAllLicenses().ToList());

            foreach (var ls in lst)
            {
                ls.CompanyName = userService.GetCompany(ls.CompanyID).CompanyName;

            }
            return View(lst);
        }

        #endregion


        #region createlicense

        public ActionResult CreateLicense()
        {
         
            if (AppSession.GetCurrentUserGroup() == 1)
            {
                

                var cpy = userService.GetAllCompanies().Where(a => a.IsActive == 1).ToList();
                cpy.Insert(0, new CompanyMasterViewModel() { CompanyID = 0, CompanyName = "-Select-" });
                ViewBag.CompanyID = new SelectList(cpy, "CompanyID", "CompanyName");

                                             
            }
            var cpyID = AppSession.GetCompanyId();
            
          
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLicense(LicenseViewModel license)
        {
            if (ModelState.IsValid)
            {

                license.LicenseKey = AppSettings.Encrypt(license.CompanyID.ToString(), license.No_of_Days.ToString(), "s@1tV@lue", "SHA1", 2, "@1B2c3D4e5F6g7H8#", 256); 


                    var result = userService.CreateLicense(license);

                    if (result > 0)
                    {
                        return getSuccessfulOperation();
                    }
                    else
                    {
                        return getFailedOperation();
                    }
               
            }

            return View(license);
        }




        public ActionResult ShowLicense(int? id)
        {



            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //   eng_users eng_users = db.eng_users.Find(id);
            var Lkey = userService.GetLicenseKey(id ?? default(int));
            if (Lkey == null)
            {
                return HttpNotFound();
            }
            

            Lkey.CompanyName = userService.GetCompany(Lkey.CompanyID).CompanyName;

            return View(Lkey);
        }


        #endregion
        




    }
}