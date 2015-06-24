using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Data
{
    public class Widget
    {
        [Key]
        public int ID { get; set; }
        public string GUID { get; set; }
        [AllowHtml]
        public string Name { get; set; }
        [AllowHtml]
        public string Title { get; set; }
        public string Option { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public int? ActiveBy { get; set; }
        public DateTime? ActiveOn { get; set; }
    }
}