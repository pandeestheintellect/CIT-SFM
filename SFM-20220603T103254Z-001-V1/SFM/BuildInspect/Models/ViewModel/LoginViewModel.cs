using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BuildInspect.Models.ViewModel
{
    public class LoginViewModel
    {
        [Display(Name = "User Name"), Required, StringLength(50)]
        public string UserName { get; set; }
        [Display(Name = "Password"), Required, DataType(DataType.Password),
      RegularExpression(@"^(?=.*\d)(?=.*[a-zA-Z]).{8,120}$", ErrorMessage = "Password must be alphanumeric with a minimum of 8 characters in length")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public int StatusFlag { get; set; }
    }
}