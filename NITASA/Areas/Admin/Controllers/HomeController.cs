using NITASA.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Admin/Home/

        public ActionResult Dashboard()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddDraft(CustomizedDashboard dboard)
        {
            return RedirectToAction("Dashboard");
        }
        public ActionResult AccessDenied()
        {
            return View();
        }
        public ActionResult NotFound(string aspxerrorpath = "")
        {
            if (aspxerrorpath.ToLower().Contains("client"))
                return RedirectToRoute("_client_Error");
            return View();
        }
    }
}
