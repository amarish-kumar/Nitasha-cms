using NITASA.Data;
using NITASA.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Controllers
{
    [AllAction]
    public class CategoryController : Controller
    {
        public NTSDBContext context;
        public CategoryController()
        {
            this.context = new NTSDBContext();
        }

        // GET: /Category/
        public ActionResult Details()
        {            
            return View();
        }
	}
}