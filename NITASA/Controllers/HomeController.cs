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
    public class HomeController : Controller
    {
        string currentThemePath;
        public HomeController()
        {
            //this.currentThemePath = "/Views/" + Request.RequestContext.HttpContext.Application["CurrentTheme"].ToString() + "/";
        }
        public ActionResult Index()
        {
            currentThemePath = "/Views/" + Request.RequestContext.HttpContext.Application["CurrentTheme"].ToString() + "/";
            return View(currentThemePath + "index.cshtml");
        }
        public ActionResult NotFound()
        {
            return View();
        }
    }
}
