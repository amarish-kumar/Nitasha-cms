using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    public class CommentController : Controller
    {
        public ActionResult List(int page = 1, string sort = "AddedOn", string sortdir = "DESC")
        {
            return View();
        }
        public ActionResult Abusive(int page = 1, string sort = "AddedOn", string sortdir = "DESC") 
        {
            return View();
        }
	}
}