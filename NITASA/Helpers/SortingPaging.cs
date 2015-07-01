using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Helpers
{
    public class Pager
    {
        public int? PageSize { get; set; }
        public int? PageCount { get; set; }
        public int? CurrentPageIndex { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        [AllowHtml]
        public string SearchText { get; set; }
    }
}