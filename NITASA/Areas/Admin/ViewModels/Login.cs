using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.ViewModels
{
    public class Login
    {
        [Required(ErrorMessage = "Please Enter Email Address.")]
        [MaxLength(100)]
        [StringLength(100, ErrorMessage = "Email address cannot exceed 100 characters")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please Enter valid Email")]
        [Display(Name = "Email Address")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Please Enter Password.")]
        [Display(Name = "Password")]
        [AllowHtml]
        public string Password { get; set; }
    }
}