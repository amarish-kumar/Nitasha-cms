using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NITASA.ViewModels
{
    public class Register
    {
        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter first name.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter your email address.")]
        [RegularExpression(@"^[a-zA-Z0-9][-\w\.]*@([a-zA-Z0-9][\w\-]*\.)+[a-zA-Z]{2,3}$", ErrorMessage = "Please enter a valid email address.")]
        //[MaxLength(100)]
        [StringLength(100, ErrorMessage = "Email address cannot exceed 100 characters.")]
        [DisplayName("User Email")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Please enter password.")]
        //[MaxLength(20)]
        [StringLength(20, ErrorMessage = "Password cannot exceed 20 characters.")]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm enter password.")]
        // [MaxLength(20)]
        [StringLength(20, ErrorMessage = "Confirm password cannot exceed 20 characters.")]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}