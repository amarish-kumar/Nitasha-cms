using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    public class TemplateController : Controller
    {
        public ActionResult Index()
        {
            var typeList = new SelectList(new[] 
            {
                new { ID = "stylesheet", Name = "Stylesheet" },
                new { ID = "javascript", Name = "Javascript" },
                new { ID = "html", Name = "Html" },
            },
            "ID", "Name", 1);

            ViewBag.typeList = typeList;
            return View();
        }

        public ActionResult GetFiles()
        {
            List<string> fileList = new List<string>();
            string fileType = Request.Form["ddlFileType"];
            if (fileType == "stylesheet")
            {
                fileList = Directory.GetFiles(Server.MapPath("~/assets/css")).ToList();
            }
            else if (fileType == "javascript")
            {
                fileList = Directory.GetFiles(Server.MapPath("~/assets/js")).ToList();
            }
            else if (fileType == "html")
            {
                fileList = Directory.GetFiles(Server.MapPath("~/views")).ToList();
            }

            ViewData["fileList"] = fileList;
            return View("Index");
        }
	}
}