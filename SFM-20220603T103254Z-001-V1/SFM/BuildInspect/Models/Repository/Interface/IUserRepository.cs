using BuildInspect.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.Repository.Imp
{
    public interface IUserRepository
    {
        UserViewModel ValidateUserLoginMobile(string username, string password, string deviceid);
        UserViewModel ValidateUserLoginWeb(string username, string password);
        bool VerifyCurrentPassword(int uid, string pwd);
        UserViewModel GetUser(int uid);
        int CreateUser(UserViewModel user);
        int SaveUser(UserViewModel user);
        int DeleteUser(int userID);
        List<UserViewModel> getAllUsers();
        List<GroupViewModel> GetUserGroups();
        bool CheckUser(string username);
        //CompanyMasterViewModel getCompany();
        //int CreateAndUpdateCompany(CompanyMasterViewModel company);
        int UpdateProfile(UserViewModel BIn_user, string newPwd);

        List<CompanyMasterViewModel> GetAllCompanies();
        bool CheckCompnay(string name);
        int CreateCompany(CompanyMasterViewModel company, string path);
        CompanyMasterViewModel GetCompany(int id);
        int SaveCompany(CompanyMasterViewModel company, string path);
        int DeleteCompany(int cID);
        List<ServiceTypeMasterViewModel> GetAllServiceTypes();

        List<LicenseViewModel> getAllLicenses();

        int CreateLicense(LicenseViewModel license);

        int CheckLicense(LicenseViewModel code);

        int ActivateLicenseKey(LicenseViewModel model);

        bool Company_Active_Check(int ComapanyID);

        LicenseViewModel GetLicenseKey(int uid);

        List<ModuleViewModel> GetMenuScreenNames(int userid);

        List<GroupViewModel> getAllGroup();
        int CreateGroup(GroupViewModel grp);
        bool CheckGroup(string grpname);
        GroupViewModel GetGroup(int uid);
        int SaveGroup(GroupViewModel grp);
        int DeleteGroup(int grpID);
    }
}