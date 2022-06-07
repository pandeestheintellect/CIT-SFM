using BuildInspect.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.Utility
{
    public class AppSession
    {
        private static string CURRENT_USER_NAME = "CurrentUserName";
        private static string CURRENT_USER_ID = "CurrentUserId";
        private static string CURRENT_USER_GROUP = "CurrentUserGroup";
        private static string CURRENT_COMPANY_DETAIL = "CompanyDetail";
        private static string CURRENT_COMPANY_ID = "CompanyId";


        private static void Set<T>(string key, T item)
        {
            HttpContext current = HttpContext.Current;
            if (current != null)
            {
                current.Session.Add(key, item);
            }

        }
        private static T Get<T>(string key)
        {
            HttpContext current = HttpContext.Current;
            if (current == null)
            {
                return default(T);
            }
            object value = current.Session[key];
            if (value != null)
            {
                return (T)value;
            }
            return default(T);
        }

        public static void SetCurrentUserId(int id)
        {
            Set<int>(CURRENT_USER_ID, id);
        }
        public static int GetCurrentUserId()
        {
            return Get<int>(CURRENT_USER_ID);
        }

        public static void SetCompanyId(int id)
        {
            Set<int>(CURRENT_COMPANY_ID, id);
        }
        public static int GetCompanyId()
        {
            return Get<int>(CURRENT_COMPANY_ID);
        }

        public static void SetCurrentUserGroup(int id)
        {
            Set<int>(CURRENT_USER_GROUP, id);
        }
        public static int GetCurrentUserGroup()
        {
            return Get<int>(CURRENT_USER_GROUP);
        }

        public static void SetCurrentUserName(string name)
        {
            Set<string>(CURRENT_USER_NAME, name);
        }
        public static string GetCurrentUserName()
        {
            return Get<string>(CURRENT_USER_NAME);
        }

       
        public static void SetCompanyDetail(CompanyMasterViewModel company)
        {
            Set<CompanyMasterViewModel>(CURRENT_COMPANY_DETAIL, company);
        }
        public static CompanyMasterViewModel GetCompanyDetail()
        {
            return Get<CompanyMasterViewModel>(CURRENT_COMPANY_DETAIL);
        }

    }
}