﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Models
{
    public class Media
    {
        [Key]
        public int ID { get; set; }

        public string GUID { get; set; }
        [MaxLength(100)]
        [StringLength(100, ErrorMessage = "Media name cannot exceed 100 characters.")]
        [Display(Name = "Media Name")]
        [AllowHtml]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select media")]
        [Display(Name = "Select Media ")]
        public string Path { get; set; }

        public string Type { get; set; } //(Image,Video)

        [AllowHtml]
        public string Attribute { get; set; }
        //public bool ShowOnGallaryPage { get; set; }
        public string Extention { get; set; }

        public DateTime? AddedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int AddedBy { get; set; }
        public int ModifiedBy { get; set; }

        [DefaultValue(0)]
        public bool IsDeleted { get; set; }
    }
}