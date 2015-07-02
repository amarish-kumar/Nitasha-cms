using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.ViewModels
{
    public class CL_Widget
    {
        public int WidgetID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Option { get; set; }
        public int DisplayOrder { get; set; }
    }
    public class CL_WidgetOption
    {
        [AllowHtml]
        public string Title { get; set; }
        public string WidgetID { get; set; }
        public int Count { get; set; }
        public bool showthumb { get; set; }
        public int GadgetOrder { get; set; }
    }
}