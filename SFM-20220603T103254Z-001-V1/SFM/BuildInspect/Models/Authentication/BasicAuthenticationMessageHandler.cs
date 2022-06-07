using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Net;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using BuildInspect.Models.Service.Imp;

namespace BuildInspect.Models.Authentication
{
    public class BasicAuthenticationMessageHandler : DelegatingHandler
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        private const string UnauthorizedMessage = "Unauthorized request";
        private const string OldWebServiceMessage = "Please update your app to the latest version.";
        private const string CREDENTIAL_USER_TYPE = "User";
        private const string CREDENTIAL_SUBSCRIBER_TYPE = "Subscriber";
        //private IUserService userService ;
        //private ISubscriberService subscriberService;

        /// <summary>
        /// Validates the request. If successful pass the request to the controller.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string rest = request.Content.ReadAsStringAsync().Result;
            logger.Debug("Raw Http Message: {0}", rest);
            //if (request.RequestUri.AbsolutePath.ToLower().StartsWith(SettingsHelper.GetLatestWebServiceAbsolutePathSetting()))
            {
                //if (!(request.RequestUri.AbsolutePath.ToLower().Equals(SettingsHelper.GetLatestWebServiceAbsolutePathSetting() + "user/login")))
                {
                    var isAuthenticated = IsAuthenticated(request);
                    logger.Debug("isAuthenticated : " + isAuthenticated);
                    if (!isAuthenticated)
                    {
                        logger.Debug("return 401");
                        var response = request.CreateErrorResponse(HttpStatusCode.Unauthorized, UnauthorizedMessage);
                        response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(
                            Configuration.BasicAuthenticationScheme));
                        return response;
                    }
                }
            }
            //else
            //{
            //    var response = request.CreateErrorResponse(HttpStatusCode.Unauthorized, OldWebServiceMessage);
            //    return response;
            //}
            return await base.SendAsync(request, cancellationToken);
        }
        /// <summary>
        /// Validates the request.
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        protected bool IsAuthenticated(HttpRequestMessage request)
        {

            bool authenticated = false;
            if (request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/"))
            {
                if (request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/binlogin") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/postuserimage") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getchecklist") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getallschools") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/submitchecklist") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/submitsignaturechecklist") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getserviceformindex") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/newserviceform") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getserviceform") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/serviceformsave") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/submitmasignaturechecklist") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/getdashboardsummary") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/submitcheckListoffline") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/mobilelogin/serviceformcreateoffline") ||
                    request.RequestUri.AbsolutePath.ToLower().Contains("/api/v1/user/postuserimage") 
                    )
                {
                    authenticated = true;
                }
                else
                    if (request.Method.Equals(HttpMethod.Post) || (request.Method.Equals(HttpMethod.Get)))
                {

                    AuthenticationHeaderValue authValue = request.Headers.Authorization;
                    if (authValue != null && !String.IsNullOrWhiteSpace(authValue.Parameter))
                    {
                        Credentials parsedCredentials = ParseAuthorizationHeader(authValue.Parameter);

                        if (parsedCredentials != null)
                        {
                           
                            IUserServices userService = request.GetDependencyScope().GetService(typeof(IUserServices)) as IUserServices;
                            //var returnUser = userService.ValidateUserLogin(parsedCredentials.Username, parsedCredentials.Password);
                            //if (returnUser != null)
                            //{
                            //    if (returnUser.IsActive == 1)
                            //    {
                            //        authenticated = true;
                            //    }
                            //}

                                          


                        }
                    }
                }
            }
           

            return authenticated;
        }


        /// <summary>
        /// Returns the credentials of the request.
        /// </summary>
        /// <param name="authHeader"></param>
        /// <returns></returns>
        private Credentials ParseAuthorizationHeader(string authHeader)
        {
            string[] credentials = Encoding.ASCII.GetString(Convert
                                                            .FromBase64String(authHeader))
                                                            .Split(
                                                            new[] { ':' });



            SecureString credPwd = new SecureString();
            try
            {

               // credPwd = CryptAlgorithm.GetEncryptedSecureString(credentials[1]);

                //logger.Info(string.Format("UserName:{0} - PassWord:{1}", credentials[0], encryptedPassword));
                logger.Info(string.Format("UserName:{0} - Type:{1}", credentials[0], credentials[2]));

                if (string.IsNullOrEmpty(credentials[0])
                    || string.IsNullOrEmpty(credentials[1])) return null;
                return new Credentials()
                {
                    Username = credentials[0],
                    Password = credentials[1],
                    Type = credentials[2]

                };

            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                try
                {
                    credPwd.Dispose();
                }
                catch (Exception ex)
                {

                }

            }

        }
    }

    public class Configuration
    {
        public const string UsernameHeader = "X-ApiAuth-Username";
        public const string AuthenticationScheme = "ApiAuth";
        public const string BasicAuthenticationScheme = "Basic";
        public const int ValidityPeriodInMinutes = 5;
    }

    public class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
    }
}