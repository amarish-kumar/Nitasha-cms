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
    public class ContentController : AppController
    {
        //
        // GET: /Content/
        public ActionResult Details()
        {
            return View(activeTheme + "content.cshtml");
        }
	}
}