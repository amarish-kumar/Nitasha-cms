using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    public class LabelController : Controller
    {
        public ActionResult List()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }
    }
}
