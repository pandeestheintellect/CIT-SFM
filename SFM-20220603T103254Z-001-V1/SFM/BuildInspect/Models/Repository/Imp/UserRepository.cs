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
using System.IO;
using System.Globalization;

namespace BuildInspect.Models.Repository.Interface
{
    public class UserRepository : IUserRepository
    {
        BuildInspectEntities BInDB = new BuildInspectEntities();
         Logger logger = LogManager.GetCurrentClassLogger();

        public UserViewModel ValidateUserLoginMobile(string username, string password, string deviceid)
        {
            UserViewModel userViewModel = new UserViewModel();
            try
            {
                var user = BInDB.users.Where(a => a.UserName == username.Trim() && a.Password == password.Trim()).FirstOrDefault();  
                userViewModel = Mapper.Map<UserViewModel>(user);

                if (userViewModel != null)
                {
                   

                        if (userViewModel.IsActive == 1)
                        {
                            var devUser = BInDB.users.Find(userViewModel.UserID);
                            devUser.DeviceID = deviceid;
                            devUser.LastLogin = DateTime.Now;
                            BInDB.Entry(devUser).State = EntityState.Modified;
                            BInDB.SaveChanges();
                        }

                    
                    


                }

                        return userViewModel;

            }
            catch (Exception ex)
            {
                logger.Debug("ValidateLoginMobile:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                logger.Info(username + ":" + password + ";");
                return userViewModel;
            }

        }

        public UserViewModel ValidateUserLoginWeb(string username, string password)
        {
            UserViewModel userViewModel = new UserViewModel();
            try
            {
                var user = BInDB.users.Where(a => a.UserName == username.Trim() && a.Password == password.Trim()).FirstOrDefault();
                userViewModel = Mapper.Map<UserViewModel>(user);
                user.LastLogin = DateTime.Now;
                BInDB.Entry(user).State = EntityState.Modified;
                BInDB.SaveChanges();
                /*
                var company_active_chk = BInDB.company_master.Where(a => a.CompanyID == userViewModel.CompanyID && a.IsActive == 1).FirstOrDefault();

                    if (company_active_chk == null)
                    {

                    }
                    */

                return userViewModel;

            }
            catch (Exception ex)
            {
                logger.Debug("ValidateLoginWeb:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                logger.Info(username + ":" + password + ";");
                return userViewModel;
            }

        }

        public bool VerifyCurrentPassword(int uid, string pwd)
        {
            var user = BInDB.users.Find(uid);
            if (user.Password == pwd)
                return true;
            else
                return false;
        }

        public UserViewModel GetUser(int uid)
        {
            var user = BInDB.users.Find(uid);
            return Mapper.Map<UserViewModel>(user);
        }

        public int CreateUser(UserViewModel user)
        {
            try
            {
                var _db_user = Mapper.Map<user>(user);
                BInDB.users.Add(_db_user);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("CreateUser:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public int SaveUser(UserViewModel user)
        {
            try
            {
                var _db_user = BInDB.users.Find(user.UserID);
                _db_user.UserName = user.UserName;
                _db_user.Password = user.Password;
                _db_user.DisplayName = user.DisplayName;
                _db_user.Zone = user.Zone;
                _db_user.Email = user.Email;
                _db_user.Mobile = user.Mobile;
                _db_user.FirstName = user.FirstName;
                _db_user.LastName = user.LastName;
                _db_user.Designation = user.Designation;
                _db_user.UpdatedBy = user.UpdatedBy;
                _db_user.UpdatedDate = user.UpdatedDate;

                //var _db_user = Mapper.Map<user>(user);
                BInDB.Entry(_db_user).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("SaveUser:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public int DeleteUser(int userID)
        {
            try
            {
                var _db_user = BInDB.users.First(a => a.UserID == userID);
                _db_user.UpdatedBy = AppSession.GetCurrentUserId();
                _db_user.UpdatedDate = DateTime.Now;
                _db_user.IsActive = 0;

                BInDB.Entry(_db_user).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("DeleteUser:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public List<UserViewModel> getAllUsers()
        {
            var users = BInDB.users.ToList();
            var lstUserView = Mapper.Map<List<UserViewModel>>(users);
            return lstUserView;
        }

        public List<GroupViewModel> GetUserGroups()
        {
            var usergroup = BInDB.usergroups.ToList();
            var lstGroupView = Mapper.Map<List<GroupViewModel>>(usergroup);
            return lstGroupView;
        }

        public List<ServiceTypeMasterViewModel> GetAllServiceTypes()
        {
            var svrlist = BInDB.servicetype_master.ToList();
            var lstSvrView = Mapper.Map<List<ServiceTypeMasterViewModel>>(svrlist);
            return lstSvrView;
        }

        public bool CheckUser(string username)
        {
            try
            {
                var user = BInDB.users.Where(a => a.UserName == username).SingleOrDefault();
                if (user == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public int UpdateProfile(UserViewModel BIn_user, string newPwd)
        {
            try
            {
                var _db_user = BInDB.users.Find(BIn_user.UserID);
                _db_user.DisplayName = BIn_user.DisplayName;
                _db_user.Password = newPwd;
                BInDB.Entry(_db_user).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("ProfileUpdate:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }


        public List<ModuleViewModel> GetMenuScreenNames(int userid)
        {
            var sql = "exec GetMenuScreen " + userid + "";

            var obj = BInDB.Database.SqlQuery<ModuleViewModel>(sql).ToList();
            return obj;

        }

        #region Company


        public List<CompanyMasterViewModel> GetAllCompanies()
        {
            var res = BInDB.company_master.OrderBy(a => a.CompanyName).ToList();
            var lists = Mapper.Map<List<CompanyMasterViewModel>>(res);

            return lists;
        }

        public bool CheckCompnay(string name)
        {
            try
            {
                var res = BInDB.company_master.Where(a => a.CompanyName == name).SingleOrDefault();
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

        
        public  CompanyMasterViewModel GetCompany(int id)
        {
            var res = BInDB.company_master.Find(id);
            return Mapper.Map<CompanyMasterViewModel>(res);
        }

       
        public int CreateCompany(CompanyMasterViewModel cpy, string path)
        {
            try
            {
                if (cpy.profile_Path != null)
                {
                    var filename = Path.GetFileName(cpy.profile_Path.FileName);
                    var rn = DateTime.Now.ToString("yyMMddhhmmss");
                    var ext = Path.GetExtension(cpy.profile_Path.FileName);
                    var newfn = cpy.ShortName + "_" + rn + ext;

                    Directory.CreateDirectory(path);
                    var finalPath = path + newfn;
                    cpy.profile_Path.SaveAs(finalPath);
                    cpy.LogoPath = finalPath;
                }
                var db_cpy = Mapper.Map<company_master>(cpy);
                BInDB.company_master.Add(db_cpy);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("CompanyCreation:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public int SaveCompany(CompanyMasterViewModel cpy, string path)
        {
            try
            {
                if (cpy.profile_Path == null)
                {
                    var _db_cpy = Mapper.Map<company_master>(cpy);
                    BInDB.Entry(_db_cpy).State = EntityState.Modified;
                    return BInDB.SaveChanges();
                }
                else
                {
                    if(cpy.LogoPath != null) { 
                    File.Delete(cpy.LogoPath);
                    }

                    var _db_cpy = Mapper.Map<company_master>(cpy);

                    var filename = Path.GetFileName(cpy.profile_Path.FileName);
                    var rn = DateTime.Now.ToString("yyMMddhhmmss");
                    var ext = Path.GetExtension(cpy.profile_Path.FileName);
                    var newfn = cpy.ShortName + "_" + rn + ext;

                    //Directory.CreateDirectory(path);
                    var finalPath = path + newfn;
                    cpy.profile_Path.SaveAs(finalPath);
                    _db_cpy.LogoPath = finalPath;

                    BInDB.Entry(_db_cpy).State = EntityState.Modified;
                    return BInDB.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SaveCompany:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }

        }

        public int DeleteCompany(int cID)
        {
            try
            {
                var _db_res = BInDB.company_master.First(a => a.CompanyID == cID);
                _db_res.UpdatedBy = AppSession.GetCurrentUserId();
                _db_res.UpdatedDate = DateTime.Now;
                _db_res.IsActive = 0;

                BInDB.Entry(_db_res).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("CompanyDeletion:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;
            }
        }



        public bool Company_Active_Check(int ComapanyID)
        {
            try
            {
                var company_chk = BInDB.company_master.Where(a => a.CompanyID == ComapanyID && a.IsActive == 1).SingleOrDefault();
                if (company_chk == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }


        }


        #endregion


        #region license

        public List<LicenseViewModel> getAllLicenses()
        {
            var licenses = BInDB.License_Master.ToList();
            var lstUserView = Mapper.Map<List<LicenseViewModel>>(licenses);
            return lstUserView;
        }


        public int CreateLicense(LicenseViewModel license)
        {
            try
            {
                if (!string.IsNullOrEmpty(license.License_Start_Date))
                    license.License_Start_Date = DateTime.ParseExact(license.License_Start_Date, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");

                //license.License_End_Date = DateTime.ParseExact(DateTime.Now.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
                var _db_license = Mapper.Map<License_Master>(license);

                DateTime date = Convert.ToDateTime(license.License_Start_Date);
                var endldt = date.AddDays(license.No_of_Days);

                _db_license.License_End_Date = endldt;
                _db_license.Creation_Date = DateTime.Now;
                BInDB.License_Master.Add(_db_license);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("CreateLicense:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }




       


        

        public int CheckLicense(LicenseViewModel code)
        {
            var curDate = DateTime.Today;
            /*
            var inactive_company = BInDB.company_master.Where(a => a.CompanyID == code.CompanyID && a.IsActive == 1).FirstOrDefault();

            if(inactive_company == null)
            {
                return -10;
            }
            */

            var obj = BInDB.License_Master.Where(a => a.CompanyID == code.CompanyID  && a.IsActive == 1).FirstOrDefault();

            // isActive == 0  license issued, but not activated
            // isActive == 1  license currently in use, 
            // isActive == 2  license expired. 



            if (obj != null)
            {
                
                if(curDate <= obj.License_End_Date)
                {
                    return 1;
                }
                else
                {
                    obj.IsActive = 2;
                    BInDB.Entry(obj).State = EntityState.Modified;
                    BInDB.SaveChanges();

                    return -1;

                }
            }
            else
            {
                
                    return -2;
                


            }
               // return RedirectToAction("LicenseActivation", "Login");

           
        }

        public int ActivateLicenseKey(LicenseViewModel model)
        {
           var obj = BInDB.License_Master.Where(a => a.LicenseKey == model.LicenseKey ).FirstOrDefault();

            

            if (obj != null)
            {

                if(obj.IsActive == 0)
                {
                    obj.IsActive = 1;

                    BInDB.Entry(obj).State = EntityState.Modified;
                    return BInDB.SaveChanges();
                }
                else if(obj.IsActive == 1)
                {
                    return -1;
                }
                else
                {
                    return -2;
                }
                
            }
            else
            {
                return -3;
            }

            
        }


        public LicenseViewModel GetLicenseKey(int uid)
        {
            var Lkey = BInDB.License_Master.Find(uid);
            return Mapper.Map<LicenseViewModel>(Lkey);
        }

        #endregion

        public List<GroupViewModel> getAllGroup()
        {
            var users = BInDB.usergroups.ToList();
            var lstUserView = Mapper.Map<List<GroupViewModel>>(users);
            return lstUserView;
        }

        public int CreateGroup(GroupViewModel grp)
        {
            try
            {
                var _db_user = Mapper.Map<usergroup>(grp);
                BInDB.usergroups.Add(_db_user);
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("CreateGroup:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }
        public bool CheckGroup(string grpname)
        {
            try
            {
                var user = BInDB.usergroups.Where(a => a.GroupName == grpname).SingleOrDefault();
                if (user == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        public GroupViewModel GetGroup(int uid)        {
            var grp = BInDB.usergroups.Find(uid);
            return Mapper.Map<GroupViewModel>(grp);
        }
        public int SaveGroup(GroupViewModel grp)
        {
            try
            {
                var _db_user = BInDB.usergroups.Find(grp.GroupID);
                _db_user.GroupName = grp.GroupName;
               
                BInDB.Entry(_db_user).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("SaveUser:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }

        public int DeleteGroup(int grpID)
        {
            try
            {
                var _db_user = BInDB.usergroups.First(a => a.GroupID == grpID);
                _db_user.UpdatedBy = AppSession.GetCurrentUserId();
                _db_user.UpdatedDate = DateTime.Now;
                _db_user.IsActive = 0;

                BInDB.Entry(_db_user).State = EntityState.Modified;
                return BInDB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Debug("DeleteUser:");
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return -1;

            }
        }
    }

}