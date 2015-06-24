using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.ViewModels
{
    public class WidgetOption
    {
        [AllowHtml]
        public string Title { get; set; }
        public string WidgetID { get; set; }
        public int Count { get; set; }
        public bool ShowThumbnail { get; set; }
        public int WidgetOrder { get; set; }
    }
}