using NITASA.Areas.Admin.Helper;
using NITASA.Data;
using NITASA.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    [CheckLogin]
    public class MediaController : Controller
    {
        private NTSDBContext context;
        public MediaController()
        {
            this.context = new NTSDBContext();
        }

        public ActionResult List()
        {
            List<Media> MediaList = context.Media.Where(m => m.IsDeleted != true).OrderByDescending(m => m.AddedOn).ToList();
            ViewBag.MediaList = MediaList;
            return View();
        }
        [HttpPost]
        public ActionResult Add(Media model, HttpPostedFileBase FileName)
        {
            if (!UserRights.HasRights(Rights.AddNewMedias))
                return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid && FileName != null && FileName.ContentLength > 0)
            {
                bool isValidFile = false;
                if (Common.IsValidImage(FileName.FileName.ToLower()))
                {
                    model.Type = "Image";
                    isValidFile = true;
                }
                else if (Common.IsValidVideo(FileName.FileName.ToLower()))
                {
                    model.Type = "Video";
                    isValidFile = true;
                }
                if (isValidFile)
                {
                    Media media = new Media();

                    string mediaFullPath = Common.GetNewFileName("/content/media/", FileName.FileName);
                    FileName.SaveAs(Server.MapPath(mediaFullPath));

                    media.GUID = Common.GetRandomGUID();
                    media.FileName = FileName.FileName;
                    //media.Path = mediaFullPath;
                    media.Type = model.Type;
                    media.Attribute = model.Attribute;
                    //media.ShowOnGallaryPage = model.ShowOnGallaryPage;
                    media.Extention = Path.GetExtension(mediaFullPath);
                    media.AddedBy = Common.CurrentUserID();
                    media.AddedOn = DateTime.UtcNow;
                    context.Media.Add(media);
                    context.SaveChanges();

                    TempData["SuccessMessage"] = "Media uploaded successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Please upload valid media.";
                }
            }
            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult Delete(string GUID)
        {
            if (!UserRights.HasRights(Rights.DeleteMedias))
                return RedirectToAction("AccessDenied", "Home");
            Media mediaToDelete = context.Media.Where(m => m.GUID == GUID).Single();
            if (mediaToDelete == null)
            {
                TempData["ErrorMessage"] = "Media not found.";
            }
            else
            {
                string pathToDelete = Server.MapPath("/content/media/"+mediaToDelete.FileName);
                mediaToDelete.IsDeleted = true;
                //dbContext.Media.Remove(mediaToDelete);
                context.SaveChanges();
                if (System.IO.File.Exists(pathToDelete))
                {
                    try
                    {
                        System.IO.File.Delete(pathToDelete);
                    }
                    finally { }
                }
                TempData["SuccessMessage"] = "Media deleted successfully.";
            }
            return RedirectToAction("List");
        }
        [HttpGet]
        public JsonResult MediaAttribute(string term)
        {
            List<string> Attributes = context.Media.Where(m => m.Attribute.ToLower().Contains(term)).Select(m => m.Attribute).Distinct().ToList();

            return Json(Attributes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetMedia()
        {
            List<Media> AllMediaList = context.Media.Where(m => m.Type == "Image" && m.IsDeleted == false).ToList();
            return PartialView(AllMediaList);
        }

    }
}
