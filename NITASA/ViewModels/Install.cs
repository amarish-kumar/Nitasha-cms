using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NITASA.ViewModels
{
    public class Install
    {
        [Required(ErrorMessage = "Please enter database hostname.")]
        // [MaxLength(100)]
        [StringLength(100, ErrorMessage = "Database hostname cannot exceed 100 characters.")]
        [DisplayName("Database Host")]
        public string DatabaseHost { get; set; }

        [Required(ErrorMessage = "Please enter database name.")]
        //[MaxLength(20)]
        [StringLength(20, ErrorMessage = "Database name cannot exceed 20 characters.")]
        [DisplayName("Database Name")]
        public string DatabaseName { get; set; }

        [Required(ErrorMessage = "Please enter username.")]
        // [MaxLength(20)]
        [StringLength(20, ErrorMessage = "Username cannot exceed 20 characters.")]
        [DisplayName("User Name")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter password.")]
        //[MaxLength(20)]
        [StringLength(20, ErrorMessage = "Password cannot exceed 20 characters.")]
        [DisplayName("Password")]
        public string Password { get; set; }

        public string IsDBInstalled { get; set; }
    }
}