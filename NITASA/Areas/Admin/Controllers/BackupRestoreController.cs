using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    public class BackupRestoreController : Controller
    {
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add()
        {
            return View();
        }
	}
}