using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BuildInspect.Models.ViewModel;
using BuildInspect.Models.Service.Imp;
using BuildInspect.Models.Utility;
using BuildInspect.Models.Security;
using BuildInspect.Models.Domain;
using System.Collections.Generic;
using System.Configuration;

namespace BuildInspect.Controllers
{
    [AccessDeniedAuthorize]
    public class UserController : SuperBaseController
    {
        private BuildInspectEntities db = new BuildInspectEntities();
        private readonly IUserServices userService;

        public UserController(IUserServices _userService)
        {
            userService = _userService;
        }

        // GET: User
        public ActionResult Index()
        {
            var uid = AppSession.GetCurrentUserId();
            var gid = AppSession.GetCurrentUserGroup();
            var cid = AppSession.GetCompanyId();
            if (gid == 1)
            {
                return View(userService.getAllUsers().Where(a => a.GroupID > 1 && a.IsActive != 0).ToList());
            }
            else
            {
                return View(userService.getAllUsers().Where(a => a.GroupID > 2 && a.IsActive != 0 && (a.CompanyID == cid || a.CompanyID == null)).ToList());
            }
        }

        // GET: User/Create
        public ActionResult Create()
        {
            if (AppSession.GetCurrentUserGroup() == 1)
            {
                var grp = userService.GetUserGroups().Where(a => a.GroupID != 1).ToList();
                grp.Insert(0, new GroupViewModel() { GroupID = 0, GroupName = "-Select-" });
                ViewBag.GroupID = new SelectList(grp, "GroupID", "GroupName");

                var cpy = userService.GetAllCompanies().Where(a => a.IsActive == 1).ToList();
                cpy.Insert(0, new CompanyMasterViewModel() { CompanyID = 0, CompanyName = "-Select-" });
                ViewBag.CompanyID = new SelectList(cpy, "CompanyID", "CompanyName");
            }
            var cpyID = AppSession.GetCompanyId();

            if (AppSession.GetCurrentUserGroup() == 2)
            {
                var grp = userService.GetUserGroups().Where(a => a.GroupID > 2 && a.GroupID != 4 && a.GroupID != 7).ToList();
                grp.Insert(0, new GroupViewModel() { GroupID = 0, GroupName = "-Select-" });
                ViewBag.GroupID = new SelectList(grp, "GroupID", "GroupName");

                var cpy = userService.GetAllCompanies().Where(a => a.IsActive == 1 && a.CompanyID == cpyID).ToList();
                cpy.Insert(0, new CompanyMasterViewModel() { CompanyID = 0, CompanyName = "-Select-" });
                ViewBag.CompanyID = new SelectList(cpy, "CompanyID", "CompanyName");
            }

            var svID = userService.GetAllServiceTypes().ToList();
            svID.Insert(0, new ServiceTypeMasterViewModel() { ServiceType = "Select", ServiceDescription = "-Select-" });
            svID.Insert(1, new ServiceTypeMasterViewModel() { ServiceType = "Common", ServiceDescription = "Common" });
            ViewBag.ServiceType = new SelectList(svID, "ServiceType", "ServiceDescription");


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserViewModel user)
        {
            if (ModelState.IsValid)
            {
                if (userService.CheckUser(user.UserName) == false)
                {
                    var cpyid = AppSession.GetCompanyId();
                    var sKey = ConfigurationManager.AppSettings["PtStK"];
                    SecurityController Scon = new SecurityController();
                    var encWord = Scon.Encrypt(sKey, user.Password);

                    user.CreatedBy = AppSession.GetCurrentUserId();
                    user.CreatedDate = DateTime.Now;
                    user.IsActive = 1;
                    user.Password = encWord;
                    user.CompanyID = cpyid;


                    var result = userService.CreateUser(user);
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
                    return getFailedOperation("Username Already exits!");
                }
            }

            return View(user);
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // win_users win_users = db.win_users.Find(id);
            var user = userService.GetUser(id ?? default(int));
            if (user == null)
            {
                return HttpNotFound();
            }


            return View(user);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //   eng_users eng_users = db.eng_users.Find(id);
            var user = userService.GetUser(id ?? default(int));
            if (user == null)
            {
                return HttpNotFound();
            }

            if (AppSession.GetCurrentUserGroup() == 1)
            {
                var grp = userService.GetUserGroups().Where(a => a.GroupID != 1).ToList();
                grp.Insert(0, new GroupViewModel() { GroupID = 0, GroupName = "-Select-" });
                ViewBag.GroupID = new SelectList(grp, "GroupID", "GroupName", user.GroupID);

                var cpy = userService.GetAllCompanies().Where(a => a.IsActive == 1).ToList();
                cpy.Insert(0, new CompanyMasterViewModel() { CompanyID = 0, CompanyName = "-Select-" });
                ViewBag.CompanyID = new SelectList(cpy, "CompanyID", "CompanyName", user.CompanyID);
            }
            var cpyID = AppSession.GetCompanyId();

            if (AppSession.GetCurrentUserGroup() == 2)
            {
                var grp = userService.GetUserGroups().Where(a => a.GroupID > 2 && a.GroupID != 4 && a.GroupID != 7).ToList();
                grp.Insert(0, new GroupViewModel() { GroupID = 0, GroupName = "-Select-" });
                ViewBag.GroupID = new SelectList(grp, "GroupID", "GroupName", user.GroupID);

                var cpy = userService.GetAllCompanies().Where(a => a.IsActive == 1 && a.CompanyID == cpyID).ToList();
                cpy.Insert(0, new CompanyMasterViewModel() { CompanyID = 0, CompanyName = "-Select-" });
                ViewBag.CompanyID = new SelectList(cpy, "CompanyID", "CompanyName", user.CompanyID);
            }

            var svID = userService.GetAllServiceTypes().ToList();
            svID.Insert(0, new ServiceTypeMasterViewModel() { ServiceType = "Select", ServiceDescription = "-Select-" });
            svID.Insert(1, new ServiceTypeMasterViewModel() { ServiceType = "Common", ServiceDescription = "Common" });
            ViewBag.ServiceType = new SelectList(svID, "ServiceType", "ServiceDescription", user.ServiceType);

            //var grp = userService.GetUserGroups().Where(a => a.GroupID != 1).ToList();
            //grp.Insert(0, new GroupViewModel() { GroupID = 0, GroupName = "-Select-" });
            //ViewBag.GroupID = new SelectList(grp, "GroupID", "GroupName", user.GroupID);

            var sKey = ConfigurationManager.AppSettings["PtStK"];
            SecurityController Scon = new SecurityController();
            var plnPwd = Scon.Decrypt(sKey, user.Password);
            user.Password = plnPwd;

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserViewModel user)
        {
            if (ModelState.IsValid)
            {
                var sKey = ConfigurationManager.AppSettings["PtStK"];
                SecurityController Scon = new SecurityController();
                var encWord = Scon.Encrypt(sKey, user.Password);

                user.UpdatedBy = AppSession.GetCurrentUserId();
                user.UpdatedDate = DateTime.Now;
                user.Password = encWord;

                var result = userService.SaveUser(user);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var result = userService.DeleteUser(id);

            if (result > 0)
            {
                return getSuccessfulOperation();
            }
            else
            {
                return getFailedOperation();
            }
           
        }

        #region GROUP

        public ActionResult GroupIndex()
        {
         
                return View(userService.getAllGroup().Where(a => a.IsActive == 1).ToList());
            
        }

        public ActionResult GroupCreate()
        {
             return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GroupCreate(GroupViewModel grp)
        {
            //if (ModelState.IsValid)
            //{
                if (userService.CheckGroup(grp.GroupName) == false)
                {


                    grp.CreatedBy = AppSession.GetCurrentUserId();
                    grp.CreatedDate = DateTime.Now;
                    grp.IsActive = 1;
                    


                    var result = userService.CreateGroup(grp);
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
                    return getFailedOperation("Groupname Already exits!");
                }
           // }

            //return View(grp);
        }


        public ActionResult GroupDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // win_users win_users = db.win_users.Find(id);
            var grp = userService.GetGroup(id ?? default(int));
            if (grp == null)
            {
                return HttpNotFound();
            }


            return View(grp);
        }

        public ActionResult GroupEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //   eng_users eng_users = db.eng_users.Find(id);
            var grp = userService.GetGroup(id ?? default(int));
            if (grp == null)
            {
                return HttpNotFound();
            }

          
            
            return View(grp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GroupEdit(GroupViewModel grp)
        {
            if (ModelState.IsValid)
            {
                grp.UpdatedBy = AppSession.GetCurrentUserId();
                grp.UpdatedDate = DateTime.Now;

                var result = userService.SaveGroup(grp);
                if (result > 0)
                {
                    return getSuccessfulOperation();
                }
                else
                {
                    return getFailedOperation();
                }
            }
            return View(grp);
        }

        [HttpPost, ActionName("DeleteGroup")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteGroup(int id)
        {
            var result = userService.DeleteGroup(id);

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
