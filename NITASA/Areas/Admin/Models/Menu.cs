using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Models
{
    public class Menu
    {
        [Key]
        public int ID { get; set; }
        public string GUID { get; set; }
        public int ParentMenuID { get; set; }

        [MaxLength(40)]
        [StringLength(40, ErrorMessage = "Menu title should not exceed 40 characters.")]
        [Required(ErrorMessage = "Please enter Menu title.")]
        [AllowHtml]
        public string Title { get; set; }

        public string Type { get; set; }
        public int RefID { get; set; }
        public string Slug { get; set; }
        public int DisplayOrder { get; set; }

        public DateTime AddedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int AddedBy { get; set; }
        public int? ModifiedBy { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}