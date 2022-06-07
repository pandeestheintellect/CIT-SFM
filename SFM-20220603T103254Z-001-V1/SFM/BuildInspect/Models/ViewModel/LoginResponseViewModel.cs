using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class LoginResponseViewModel : ResponseViewModel
    {
        public BInLiteUserViewModel User { get; set; }
        public String downtimeMessage { get; set; }
    }

    public class BInLiteUserViewModel
    {
        public string loginUserType;
        public string userId { get; set; }
        public string groupID { get; set; }
        public string userName { get; set; }
        public string userFullName { get; set; }
        public string CompanyID { get; set; }
        public string ServiceType { get; set; }
        public string Zone { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }


    }

   
    public class ResponseViewModel
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public String ToString()
        {
            return string.Format("Success:{0}  } ErrorMessage:{2}", Success, ErrorMessage);
        }

        // New variables required for content service

        public string Credentials { get; set; }


        public int userid { get; set; }
    }
}