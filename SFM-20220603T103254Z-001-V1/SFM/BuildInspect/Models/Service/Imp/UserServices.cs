using BuildInspect.Models.Repository.Imp;
using BuildInspect.Models.Service.Imp;
using BuildInspect.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.Service.Interface
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository userRepository;
        
        public UserServices(IUserRepository _userRepository)
        {
            userRepository = _userRepository;
        }

        public UserViewModel ValidateUserLoginMobile(string username, string password, string deviceid)
        {
            return userRepository.ValidateUserLoginMobile(username, password, deviceid);
        }
        public UserViewModel ValidateUserLoginWeb(string username, string password)
        {
            return userRepository.ValidateUserLoginWeb(username, password);
        }
        public bool VerifyCurrentPassword(int uid, string pwd)
        {
            return userRepository.VerifyCurrentPassword(uid, pwd);
        }
        public UserViewModel GetUser(int uid)
        {
            return userRepository.GetUser(uid);
        }
        public int CreateUser(UserViewModel user)
        {
            return userRepository.CreateUser(user);
        }
        public int SaveUser(UserViewModel user)
        {
            return userRepository.SaveUser(user);
        }
        public int DeleteUser(int userID)
        {
            return userRepository.DeleteUser(userID);
        }
        public List<UserViewModel> getAllUsers()
        {
            return userRepository.getAllUsers();
        }
        public List<GroupViewModel> GetUserGroups()
        {
            return userRepository.GetUserGroups();
        }
        public bool CheckUser(string username)
        {
            return userRepository.CheckUser(username);
        }
        //public CompanyMasterViewModel getCompany()
        //{
        //    return userRepository.getCompany();
        //}
        //public int CreateAndUpdateCompany(CompanyMasterViewModel company)
        //{
        //    return userRepository.CreateAndUpdateCompany(company);
        //}
        public int UpdateProfile(UserViewModel BIn_user, string newPwd)
        {
            return userRepository.UpdateProfile(BIn_user, newPwd);
        }

        public List<CompanyMasterViewModel> GetAllCompanies()
        {
            return userRepository.GetAllCompanies();
        }
        public bool CheckCompnay(string name)
        {
            return userRepository.CheckCompnay(name);
        }
        public int CreateCompany(CompanyMasterViewModel company, string path)
        {
            return userRepository.CreateCompany(company, path);
        }
        public CompanyMasterViewModel GetCompany(int id)
        {
            return userRepository.GetCompany(id);
        }
        public int SaveCompany(CompanyMasterViewModel company,string path)
        {
            return userRepository.SaveCompany(company, path);
        }
        public int DeleteCompany(int cID)
        {
            return userRepository.DeleteCompany(cID);
        }
        public List<ServiceTypeMasterViewModel> GetAllServiceTypes()
        {
            return userRepository.GetAllServiceTypes();
        }


        public List<LicenseViewModel> getAllLicenses()
        {

            return userRepository.getAllLicenses();
        }


        public int CreateLicense(LicenseViewModel license)
        {
            return userRepository.CreateLicense(license);

        }

        public int CheckLicense(LicenseViewModel code)
        {
            return userRepository.CheckLicense(code);
        }

        public int ActivateLicenseKey(LicenseViewModel model)
        {
            return userRepository.ActivateLicenseKey(model);
        }


        public bool Company_Active_Check(int ComapanyID)
        {

            return userRepository.Company_Active_Check(ComapanyID);

        }


        public LicenseViewModel GetLicenseKey(int uid)
        {

            return userRepository.GetLicenseKey(uid);
        }


        public List<ModuleViewModel> GetMenuScreenNames(int userid)
        {
            return userRepository.GetMenuScreenNames(userid);
        }
        public List<GroupViewModel> getAllGroup()
        {
            return userRepository.getAllGroup();
        }
        public int CreateGroup(GroupViewModel grp)
        {
            return userRepository.CreateGroup(grp);
        }
        public bool CheckGroup(string grpname)
        {
            return userRepository.CheckGroup(grpname);
        }
        public GroupViewModel GetGroup(int uid)
        {
            return userRepository.GetGroup(uid);
        }
        public int SaveGroup(GroupViewModel grp)
        {
            return userRepository.SaveGroup(grp);
        }
        public int DeleteGroup(int grpID)
        {
            return userRepository.DeleteGroup(grpID);
        }
    }
    
}
