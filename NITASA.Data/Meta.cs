using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Data
{
    public class Meta
    {
        [Key]
        public int ID { get; set; }
        public int ContentID { get; set; }
        [AllowHtml]
        [Display(Name="Keyword")]
        public string Keyword { get; set; }

        [AllowHtml]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [AllowHtml]
        [Display(Name = "Author")]
        public string Author { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}