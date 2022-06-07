using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using BuildInspect.Models.Utility;
using NLog;
namespace BuildInspect.Models.Security
{
    public class AccessDeniedAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// on authorizing an user
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {



                //filterContext.Result = new RedirectResult("~/Login/RedirectToLogin");
                filterContext.Result = new RedirectResult("~/Login/Index");

                return;
            }
            else
            {
                //    try
                //    {
                //        var userinfo = ModelHelper.GetRedisRecord(SessionHelper.GetCurrentUserName().Trim());
                //        if (userinfo != null)
                //        {
                //            if (userinfo.SessionID != filterContext.HttpContext.Session.SessionID)
                //            {
                //                FormsAuthentication.SignOut();
                //                filterContext.HttpContext.Session.Clear();
                //                filterContext.Result = new RedirectResult("~/Login/RedirectToLogin?Length=2");
                //                return;
                //            }
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        LogManager.GetCurrentClassLogger().Error("Redis connection failed " + ex.Message + " " + ex.StackTrace);
                //    }
                //}

                if (filterContext.Result is HttpUnauthorizedResult)
                {
                    filterContext.Result = new RedirectResult("~/Login/Denied");
                }
            }
        }
    }
}