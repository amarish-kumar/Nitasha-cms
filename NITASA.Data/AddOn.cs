using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Data
{
    public class AddOn
    {
        [Key]
        public int ID { get; set; }

        public string GUID { get; set; }

        [Required(ErrorMessage = "Please enter Addon Name.")]
        [MaxLength(200)]
        [StringLength(200, ErrorMessage = "Addon name cannot exceed 200 characters.")]
        public string Name { get; set; }

        public string Code { get; set; }

        public string Thumbnail { get; set; }

        [AllowHtml]
        public string Description { get; set; }

        [RegularExpression(@"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$", ErrorMessage = "Enter Valid Addon Link")]
        [AllowHtml]
        public string Link { get; set; }

        public DateTime AddedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int AddedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public bool IsPublished { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}