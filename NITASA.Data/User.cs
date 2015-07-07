using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Data
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        public string GUID { get; set; }

        [MaxLength(100)]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [Required(ErrorMessage = "Please enter email address.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter valid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessage = "Please enter password")]
        [StringLength(100, ErrorMessage = "Password must be atleast 8 characters long", MinimumLength = 8)]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[0-9]).{8,}$", ErrorMessage = "Password must contain at least one digit and one upper case letter and must be 8 characters long")]
        [Display(Name = "Password")]
        [AllowHtml]
        public string Password { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter first name")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        [Display(Name = "First Name")]
        [AllowHtml]
        public string FirstName { get; set; }

        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        [Display(Name = "Last Name")]
        [AllowHtml]
        public string LastName { get; set; }

        [Display(Name = "Profile Picture")]
        public string ProfilePicURL { get; set; }

        public int SaltKey { get; set; }

        public DateTime AddedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int AddedBy { get; set; }
        public int ModifiedBy { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        [DefaultValue(false)]
        public bool IsSuperAdmin { get; set; }

        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", this.FirstName, this.LastName);
            }
        }

        public int RoleID { get; set; }

        [ForeignKey("RoleID")]
        public virtual Role Role { get; set; }

        [NotMapped]
        public bool IsChangePassword { get; set; }
    }
}