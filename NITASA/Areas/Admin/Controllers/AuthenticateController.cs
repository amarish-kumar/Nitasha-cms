using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    public class AuthenticateController : Controller
    {
        //
        // GET: /Admin/Authenticate/

        public ActionResult Login()
        {
            return View();
        }

    }
}
