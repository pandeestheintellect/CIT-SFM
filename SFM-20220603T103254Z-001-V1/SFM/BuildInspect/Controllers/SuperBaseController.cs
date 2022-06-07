using BuildInspect.Models.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace BuildInspect.Controllers
{
    public class SuperBaseController : Controller
    {

        public string autoSaveOption = String.Empty;
        public string autoRefreshOption = String.Empty;
        public SuperBaseController()
        {
            //userService = DependencyResolver.Current.GetService<IUserService>();
            //agencyService = DependencyResolver.Current.GetService<IAgencyService>();
        }

        [OutputCache(Location = OutputCacheLocation.None)]
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.AppendCacheExtension("no-store, must-revalidate");
            Response.AppendHeader("Pragma", "no-cache");
            Response.AppendHeader("Expires", "0");
            filterContext.HttpContext.Response.AddHeader(
             "X-Frame-Options", "SAMEORIGIN");

           
           
            ViewResultBase view = filterContext.Result as ViewResultBase;
            if (view == null)
                return;

            string cultureName = Thread.CurrentThread.CurrentCulture.Name;

            if (cultureName == LocalizationHelper.GetDefaultCulture())
                return;

            base.OnActionExecuted(filterContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if ((filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "MAMap" || filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "SystemMap" || filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "BackCasting") && (filterContext.ActionDescriptor.ActionName == "SaveToDB" || filterContext.ActionDescriptor.ActionName == "Save"))
            {
                foreach (var parameter in filterContext.ActionParameters)
                {
                    if (parameter.Key == "autoSave")
                        autoSaveOption = parameter.Value == null ? "" : parameter.Value.ToString().Replace('"', ' ').Trim();
                }
            }

        }
        protected override void ExecuteCore()
        {
            string cultureName = null;
            HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else
                cultureName = Request.UserLanguages[0];

            cultureName = LocalizationHelper.GetImplementedCulture(cultureName);

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            base.ExecuteCore();
        }
        /// <summary>
        /// Returns a Json Result with the message.
        /// </summary>
        /// <returns></returns>
        public ActionResult getFailedOperation()
        {
            return getFailedOperation("Failed");
        }
        /// <summary>
        /// Returns a Json Result with the message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ActionResult getFailedOperation(string message)
        {
            return Json(new
            {
                value = string.Format("{0}", message)
            });
        }

        /// <summary>
        /// Returns a Json Result with the message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ActionResult getFailedEmailOperation(string message)
        {
            return Json(new
            {
                value = string.Format("{0}", "EmailSendFailed"),
                message = message
            });
        }
        /// <summary>
        ///  Returns a Json Result with an OK message.
        /// </summary>
        /// <returns></returns>
        public ActionResult getSuccessfulOperation()
        {
            return Json(new
            {
                value = string.Format("{0}", "OK")
            });
        }
        /// <summary>
        ///  Returns a Json Result with a custom message.
        /// </summary>
        /// <returns></returns>
        public ActionResult getSuccessfulOperation(string message)
        {
            return Json(new
            {
                value = string.Format("{0}", message)
            });
        }
        /// <summary>
        ///  Returns a Json Result with a custom message and custom Object.
        /// </summary>
        /// <returns></returns>
        public ActionResult getSuccessfulOperation(string message, List<object> customObject)
        {
            return Json(new
            {
                value = string.Format("{0}", message),
                info = customObject

            });
        }

       

      

        /// <summary>
        ///  Returns a Json Result with a custom message.
        /// </summary>
        /// <returns></returns>
        public JsonResult getOperation(bool success, string data)
        {
            return Json(new
            {
                success = string.Format("{0}", success),
                data = string.Format("{0}", data)
            });
        }
        /// <summary>
        ///  Returns a Json Result with a custom message.
        /// </summary>
        /// <returns></returns>
        public JsonResult getOperation(bool success, string data, JsonRequestBehavior behavior)
        {
            //Security Review - Javascript Hijacking
            //No sensitive data send via this get response.
            return Json(new
            {
                success = string.Format("{0}", success),
                data = string.Format("{0}", data)
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///  Returns a Json Result with a custom message.
        /// </summary>
        /// <returns></returns>
        public ActionResult getSuccessfulOperation(string message, string newId)
        {
            return Json(new
            {
                value = string.Format("{0}", message),
                newId = string.Format("{0}", newId)
            });
        }
        /// <summary>
        /// Generates string from a view
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public string RenderRazorViewToString(string viewName, object model)
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

        protected override void HandleUnknownAction(string actionName)
        {
            // TODO: check for actionName in database
            // Response.Redirect(SettingsHelper.GetPortalUrl() + "/Error/Error");
            RedirectToAction("CustomError", "Error").ExecuteResult(this.ControllerContext);
        }
        
    }
}