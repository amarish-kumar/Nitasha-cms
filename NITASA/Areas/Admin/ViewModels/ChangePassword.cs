using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NITASA.Areas.Admin.ViewModels
{
    public class ChangePassword
    {
        [Required(ErrorMessage = "Please enter current password.")]
        public string currentpassword { get; set; }
        
        [Required(ErrorMessage = "Please enter new password.")]
        [StringLength(100, ErrorMessage = "Password must be atleast 8 characters long.", MinimumLength = 8)]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[0-9]).{8,}$", ErrorMessage = "Password must contain at least one digit and one upper case letter and must be 8 characters long.")]
        public string newpassword { get; set; }

        [Required(ErrorMessage = "Please enter confirm password.")]
        [Compare("newpassword", ErrorMessage = "Password does not matched.")]
        public string confirmpassword { get; set; }
    }
}