using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
namespace NITASA.Areas.Admin.Controllers
{
    public class TemplateController : Controller
    {
        public ActionResult Index()
        {
            if (ViewBag.typeList == null)
            {
            var typeList = new SelectList(new[] 
                {
                    new { ID = "stylesheet", Name = "Stylesheet" },
                    new { ID = "javascript", Name = "Javascript" },
                    new { ID = "html", Name = "Html" },
                },
               "ID", "Name", 1);
            ViewBag.typeList = typeList;
                }

            if (ViewData["fileList"] == null)
            {
                List<SelectListItem> fileList = GetFileList(Directory.GetFiles(Server.MapPath("~/assets/css"))).ToList();
                fileList.AddRange(GetFileList(Directory.GetFiles(Server.MapPath("~/assets/js"))));
                fileList.AddRange(GetFileList(Directory.GetFiles(Server.MapPath("~/views"))));
                ViewData["fileList"] = fileList.AsEnumerable();
            }
            return View();
        }

        public ActionResult GetFiles(string fileType)
        {
            List<string> fileList;
            if (fileType == "stylesheet")
            {
                fileList = Directory.GetFiles(Server.MapPath("~/assets/css"), "*.css").ToList();
            }
            else if (fileType == "javascript")
            {
                fileList = Directory.GetFiles(Server.MapPath("~/assets/js"), "*.js").ToList();
            }
            else if (fileType == "html")
            {
                fileList = Directory.GetFiles(Server.MapPath("~/views"),"*.cshtml").ToList();
            }
            else
            {
                fileList = Directory.GetFiles(Server.MapPath("~/assets/css"), "*.css").ToList();
                fileList.AddRange(Directory.GetFiles(Server.MapPath("~/assets/js"), "*.js").AsEnumerable());
                fileList.AddRange(Directory.GetFiles(Server.MapPath("~/views"), "*.cshtml").AsEnumerable());
            }
            StringBuilder fileContent = new StringBuilder();
            
            string removeString = Server.MapPath("~");
            foreach(string fileName in fileList)
            {
                fileContent.Append(fileName.Replace(removeString, "")+",");
            }
            return Content(fileContent.ToString().Trim(new char[]{','}));
        }

        public ActionResult FileDataContent(string filePath)
        {
            string fileToRead = Server.MapPath("~") + filePath;
            StringBuilder fileContent = new StringBuilder();
            string[] lines = System.IO.File.ReadAllLines(fileToRead);
            int lineCount = 0;
            foreach (string line in lines)
            {
                if (lineCount == 0) { fileContent.Append(line); }
                else { fileContent.Append(Environment.NewLine + line); }
                lineCount++;
                
            }
            return Content(fileContent.ToString());
        }

        [HttpPost,ValidateInput(false)]
        public ActionResult SaveFile(string txtFileContent, string hdnfilename)
        {
            string fileName = hdnfilename;
            string fileContent = txtFileContent;
            string[] lines = fileContent.Split(new string[] { Environment.NewLine },StringSplitOptions.None);
            string fileToWrite = Server.MapPath("~") + fileName;

            System.IO.File.WriteAllLines(fileToWrite, lines);
            ViewData["LastUpdatedFile"] = fileName;
            TempData["SuccessMessage"] = "File Content saved successfully.";

            if (ViewBag.typeList == null)
            {
                var typeList = new SelectList(new[] 
                {
                    new { ID = "stylesheet", Name = "Stylesheet" },
                    new { ID = "javascript", Name = "Javascript" },
                    new { ID = "html", Name = "Html" },
                },
                   "ID", "Name", 1);
                ViewBag.typeList = typeList;
            }

            if (ViewData["fileList"] == null)
            {
                List<SelectListItem> fileList = GetFileList(Directory.GetFiles(Server.MapPath("~/assets/css"))).ToList();
                fileList.AddRange(GetFileList(Directory.GetFiles(Server.MapPath("~/assets/js"))));
                fileList.AddRange(GetFileList(Directory.GetFiles(Server.MapPath("~/views"))));
                ViewData["fileList"] = fileList.AsEnumerable();
            }
            return View("Index");

        }
        public IEnumerable<SelectListItem> GetFileList(string[] fileArray)
        {
            List<SelectListItem> newList = new List<SelectListItem>();

            foreach(string fileName in fileArray)
            {
                if(fileName.EndsWith(".css",StringComparison.OrdinalIgnoreCase) 
                    || fileName.EndsWith(".js",StringComparison.OrdinalIgnoreCase)
                    || fileName.EndsWith(".cshtml",StringComparison.OrdinalIgnoreCase))
                { 
                string removeString = Server.MapPath("~");
                string modifiedName = fileName.Remove(0, removeString.Length);
                SelectListItem selListItem = new SelectListItem() { Value = modifiedName, Text = modifiedName };
                newList.Add(selListItem);
                }
            }
            IEnumerable<SelectListItem> abc = newList.AsEnumerable();
            return abc;
        }
	}
}