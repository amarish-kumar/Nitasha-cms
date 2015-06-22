using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Data
{
    public class Role
    {
        [Key]
        public int ID { get; set; }

        public string GUID { get; set; }

        [MaxLength(100)]
        [StringLength(100, ErrorMessage = "Role name cannot exceed 100 characters.")]
        [Required(ErrorMessage = "Please enter Role Name.")]
        [Display(Name = "Role Name")]
        [AllowHtml]
        public string Name { get; set; }

        public DateTime AddedOn { get; set; }

        public int? AddedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }

    public class RightsInRole
    {
        [Key]
        public int ID { get; set; }
        public int RoleID { get; set; }
        public string RightsName { get; set; }
    }
}