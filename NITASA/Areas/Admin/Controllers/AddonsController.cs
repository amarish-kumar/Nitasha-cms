using NITASA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    public class AddonsController : Controller
    {
        public NTSDBContext context;
        public AddonsController()
        {
            this.context = new NTSDBContext();
        }

        public ActionResult List()
        {
            return View();
        }
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Content model) 
        {
            return View();
        }
        [HttpGet]
        public ActionResult Delete(string GUID)
        {
            return RedirectToAction("List");
        }
	}
}