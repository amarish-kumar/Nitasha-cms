using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.ViewModels
{
    public class ChangePassword
    {
        [AllowHtml]
        public string currentpassword { get; set; }
        [AllowHtml]
        public string newpassword { get; set; }
        [AllowHtml]
        public string confirmpassword { get; set; }
    }
}