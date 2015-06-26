using NITASA.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Controllers
{
    [AllAction]
    public class HomeController : AppController
    {
        public ActionResult Index()
        {
            return View(activeTheme + "index.cshtml");
        }
        public ActionResult NotFound()
        {
            return View();
        }
    }
}
