using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    public class OpenIDController : Controller
    {
        public ActionResult Index(string signInWith)
        {
            return View();
        }
	}
}