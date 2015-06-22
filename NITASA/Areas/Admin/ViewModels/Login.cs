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
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Please enter valid Email.")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Please Enter Password.")]
        //[MinLength(6, ErrorMessage = "The Password must be 6 characters long.")]
        //[MaxLength(100)]
        //[StringLength(1000, MinimumLength = 6, ErrorMessage = "Password must be 6 characters long.")]
        [Display(Name = "Password")]
        [AllowHtml]
        public string Password { get; set; }
    }
}