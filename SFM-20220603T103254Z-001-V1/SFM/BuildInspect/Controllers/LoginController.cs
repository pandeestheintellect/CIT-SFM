using BuildInspect.Models.ViewModel;
using BuildInspect.Models.Service.Imp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BuildInspect.Models.Utility;
using BuildInspect.Models.Service.Interface;
using System.Configuration;
using NLog;

namespace BuildInspect.Controllers
{
    public class LoginController : SuperBaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IUserServices userService;
       

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        /// <param name="_userService"></param>
        public LoginController(IUserServices _userService)
        {
            userService = _userService;          

        }

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginViewModel model, string returnUrl)
        {

            if (ModelState.IsValid)
            {
                var sKey = ConfigurationManager.AppSettings["PtStK"];
                SecurityController Scon = new SecurityController();
                var encWord = Scon.Encrypt(sKey, model.Password);

                var returnUser = userService.ValidateUserLoginWeb(model.UserName, encWord);


               


                //var company = userService.getCompany();
                if (returnUser != null)
                {

                    var company_chk = userService.Company_Active_Check(returnUser.CompanyID ?? default(int));

                    if (returnUser.CompanyID > 0)
                    {

                        if (company_chk == false)
                        {
                            model.StatusFlag = 5;
                            return View(model);
                        }
                    }

                    if (returnUser.GroupID == 1)
                    {

                                               
                        if (returnUser.IsActive == 1)
                        {
                            FormsAuthentication.SetAuthCookie(returnUser.UserName, false);
                            AppSession.SetCurrentUserId(returnUser.UserID);
                            AppSession.SetCurrentUserGroup(returnUser.GroupID ?? default(int));
                            AppSession.SetCurrentUserName(returnUser.DisplayName);
                            AppSession.SetCompanyId(returnUser.CompanyID ?? default(int));

                            //AppSession.SetCompanyDetail(company);
                            //logger.Info("Login successful:");

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            model.StatusFlag = 2;
                            return View(model);
                        }

                    }
                    else
                    {

                        //var lichk =userService.l ;

                        var lvm = new LicenseViewModel();

                        if(returnUser.IsActive == 1 )
                        {
                            lvm.CompanyID = returnUser.CompanyID ?? default(int);
                            var chklicense = userService.CheckLicense(lvm);

                            if(chklicense == 1)
                            {
                                FormsAuthentication.SetAuthCookie(returnUser.UserName, false);
                                AppSession.SetCurrentUserId(returnUser.UserID);
                                AppSession.SetCurrentUserGroup(returnUser.GroupID ?? default(int));
                                AppSession.SetCurrentUserName(returnUser.DisplayName);
                                AppSession.SetCompanyId(returnUser.CompanyID ?? default(int));

                                //AppSession.SetCompanyDetail(company);
                                //logger.Info("Login successful:");

                                return RedirectToAction("Index", "Home");
                            }
                           else if(chklicense == -1)
                            {
                                return RedirectToAction("LicenseActivation", "Login");
                            }
                            else
                            {
                                return RedirectToAction("LicenseActivation", "Login", new { dsf = "name" });
                            }

                        }


                        //return RedirectToAction("LicenseActivation", "Login");


                    }

                }
                model.StatusFlag = 1;
                return View(model);
            }

            return View(model);
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();

            return RedirectToAction("Index", "Login");
        }

        //[AccessDeniedAuthorize]
        public ActionResult LicenseActivation(string dsf)
        {
            LicenseViewModel model = new LicenseViewModel();
            //logger.Info("Activation login:" + id);

            //  model.Advisor_Code = AppSession.GetCurrentUserAdvCode();
            if (dsf == "name")
            {
                ViewBag.ActivationFlag = 5;
                return View(model);
            }
            else
            {
                ModelState.AddModelError("LicenseKey", "License expired");
                ViewBag.ActivationFlag = 0;

                return View(model);
            }

        }


        [HttpPost]
        public ActionResult LicenseActivation(LicenseViewModel lk)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    //Pre-check
                    //var pc = userService.CheckLicense(lk);
                    //if (pc == true)
                    //{
                    //    ModelState.AddModelError("LicenseKey", "License Key expired");
                    //    return View(lk);
                    //}

                    //var LicKey = AppSettings.Decrypt(lk.LicenseKey, lk.Advisor_Code, "s@1tValue", "SHA1", 2, "@1B2c3D4e5F6g7H8", 256);
                    //if (LicKey == "" || !LicKey.Contains("-"))
                    //{
                    //    ModelState.AddModelError("LicenseKey", "Invalid License Key");
                    //    return View(lk);
                    //}

                    //string[] st = LicKey.Split(new char[] { '-' });
                    //lk.NoOfDays = Convert.ToInt32(st[0]);

                    var actLK = userService.ActivateLicenseKey(lk);

                    if(actLK > 0)
                    {
                        ModelState.AddModelError("LicenseKey", "License Key Activated");
                        ViewBag.ActivationFlag = 1;
                        return View(lk);
                        //return RedirectToAction("Index", "Login");
                    }
                    else if(actLK == -1)
                    {
                        ModelState.AddModelError("LicenseKey", "License Key already activated");
                        ViewBag.ActivationFlag = 2;
                        return View(lk);
                    }
                    else if (actLK == -2)
                    {
                        ModelState.AddModelError("LicenseKey", "License Key expired");
                        return View(lk);
                    }
                    else if (actLK == -10)
                    {
                        ModelState.AddModelError("LicenseKey", "Company is inactive");
                        return View(lk);
                    }
                    else
                    {
                        ModelState.AddModelError("LicenseKey", "Invalid License Key");
                        return View(lk);
                    }

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("LicenseKey", "Invalid License Key");
                    return View(lk);
                }
            }

            return View();
        }



    }
}