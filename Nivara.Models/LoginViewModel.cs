using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Nivara.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

       
        
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirm password does not match")]
        public string ConfirmPassword { get; set; }

        public string ResetCode { get; set; }
        public string UserName { get; set; }
    }
}
