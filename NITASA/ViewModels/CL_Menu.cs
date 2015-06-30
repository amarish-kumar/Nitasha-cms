using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NITASA.ViewModels
{
    public class CL_Menu
    {
        public int MenuID { get; set; }
        public int ParentMenuID { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string URL { get; set; }
        public int DisplayOrder { get; set; }
    }
}