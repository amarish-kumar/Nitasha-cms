using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    public class SitemapController : Controller
    {
        public ActionResult Generate()
        {
            return View();
        }

    }
}
