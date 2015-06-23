﻿using System;
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
        public int WidgetID { get; set; }
        public string WidgetGUID { get; set; }
        [AllowHtml]
        public string WidgetName { get; set; }
        [AllowHtml]
        public string WidgetTitle { get; set; }
        public string WidgetOption { get; set; }
        public int WidgetOrder { get; set; }
        public bool IsActive { get; set; }
        public int? ActiveBy { get; set; }
        public DateTime? ActiveOn { get; set; }
    }
}