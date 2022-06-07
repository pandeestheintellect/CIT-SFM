using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace BuildInspect.Filters
{
    public class UnhandledExceptionFilter : ExceptionFilterAttribute
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        public override void OnException(HttpActionExecutedContext context)
        {
            try
            {
                logger.Error(context.Exception.Message);
              
                    Exception ex = (Exception)context.Exception;

                    if (ex != null)
                    {
                        string emailSubject = "Eng webservice error on " + DateTime.Now.ToString();
                        System.Text.StringBuilder emailBody = new System.Text.StringBuilder();
                        emailBody.AppendLine("User Name : ");
                        emailBody.AppendLine(" ");
                        emailBody.AppendLine("Error Message :");
                        emailBody.AppendLine(" ");
                        emailBody.AppendLine(ex.Message.ToString());
                        emailBody.AppendLine(" ");
                        emailBody.AppendLine("Error StackTrace :");
                        emailBody.AppendLine(ex.StackTrace);
                        emailBody.AppendLine(" ");
                        emailBody.AppendLine("Error Source :");
                        emailBody.AppendLine(" ");
                        emailBody.AppendLine(ex.Source);
                        if (ex.InnerException != null)
                        {
                            emailBody.AppendLine("InnerException Error Message :");
                            emailBody.AppendLine(" ");
                            emailBody.AppendLine(ex.InnerException.Message.ToString());
                            emailBody.AppendLine(" ");
                            emailBody.AppendLine("InnerException Error StackTrace :");
                            emailBody.AppendLine(ex.InnerException.StackTrace);
                            emailBody.AppendLine(" ");
                            emailBody.AppendLine("InnerException Error Source :");
                            emailBody.AppendLine(" ");
                            emailBody.AppendLine(ex.InnerException.Source);
                        }
                     //   EmailHelper.sendEmail(emailSubject, emailBody.ToString(), SettingsHelper.GetSupportBatchEmailTo());
                    }
                
            }
            catch (Exception) { }

        }


    }
}
