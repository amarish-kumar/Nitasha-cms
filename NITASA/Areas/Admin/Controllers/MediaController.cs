using NITASA.Areas.Admin.Helper;
using NITASA.Data;
using NITASA.Services.Security;
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
        int MediaPageSize = 10; int MediaPopUpPageSize = 10;
        IAclService aclService;

        public MediaController(IAclService aclService)
        {
            this.context = new NTSDBContext();
            this.aclService = aclService;
        }

        public ActionResult List()
        {
            int count = context.Media.Where(m => m.IsDeleted == false).Count();
            ViewBag.TotalPage = (count / MediaPageSize) + (count % MediaPageSize == 0 ? 0 : 1);
            List<Media> MediaList = context.Media.Where(m => m.IsDeleted == false)
                .OrderByDescending(m => m.ID).Take(MediaPageSize).ToList();
            return View(MediaList);
        }

        public ActionResult GetMediaList(int CurrentPageIndex)
        {
            List<Media> MediaList = context.Media.Where(c => c.IsDeleted == false && c.Type == "Image")
                        .OrderByDescending(m => m.ID).Skip(CurrentPageIndex * MediaPageSize).Take(MediaPageSize).ToList();
            return PartialView("~/Areas/Admin/Views/Media/ImageList.cshtml", MediaList);
        }


        [HttpPost]
        public ActionResult Add(string Attribute, List<HttpPostedFileBase> MediaFiles)
        {
            if (!aclService.HasRight(Rights.AddNewMedias))
                return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid && MediaFiles != null && MediaFiles.Count() > 0)
            {
                int successcount = 0;
                foreach (var item in MediaFiles)
                {
                    bool isValidFile = false;
                    string MediaType = string.Empty;
                    if (Functions.IsValidImage(item.FileName.ToLower()))
                    {
                        MediaType = "Image";
                        isValidFile = true;
                    }
                    if (isValidFile)
                    {
                        Media media = new Media();
                        string mediaFullPath = Functions.GetNewFileName("/content/media/", item.FileName);
                        item.SaveAs(Server.MapPath(mediaFullPath));
                        media.GUID = Functions.GetRandomGUID();
                        media.FileName = mediaFullPath;
                        media.Type = MediaType;
                        media.Attribute = Attribute;
                        media.Extention = Path.GetExtension(mediaFullPath);
                        media.AddedBy = Functions.CurrentUserID();
                        media.AddedOn = DateTime.UtcNow;
                        context.Media.Add(media);
                        context.SaveChanges();
                        successcount++;
                    }
                }
                TempData["SuccessMessage"] = successcount + " Media uploaded successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Please select media.";
            }
            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult Delete(string GUID)
        {
            if (!aclService.HasRight(Rights.DeleteMedias))
                return RedirectToAction("AccessDenied", "Home");
            Media mediaToDelete = context.Media.Where(m => m.GUID == GUID).Single();
            if (mediaToDelete == null)
            {
                TempData["ErrorMessage"] = "Media not found.";
            }
            else
            {
                string pathToDelete = Server.MapPath(mediaToDelete.FileName);
                mediaToDelete.IsDeleted = true;
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
            List<string> Attributes = context.Media.Where(m => m.IsDeleted == false && m.Attribute.ToLower().Contains(term)).Select(m => m.Attribute).Distinct().ToList();
            return Json(Attributes, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public ActionResult GetMedia(bool isEditor = false)
        {
            List<Media> AllMediaList = context.Media.Where(m => m.Type == "Image" && m.IsDeleted == false).OrderByDescending(m => m.ID).ToList();
            int totalPage = (AllMediaList.Count() / MediaPopUpPageSize) + (AllMediaList.Count() % MediaPopUpPageSize == 0 ? 0 : 1);
            MediaPopUp AllMedias = new MediaPopUp()
            {
                TabName = "All",
                TotalPages = totalPage,
                Medias = AllMediaList.Take(MediaPopUpPageSize).ToList()
            };

            List<MediaPopUp> mediaPopUp = new List<MediaPopUp>();
            mediaPopUp.Add(AllMedias);

            List<string> Attributes = AllMediaList.Where(x => !string.IsNullOrEmpty(x.Attribute)).Select(m => m.Attribute).Distinct().ToList();
            foreach (string attribute in Attributes)
            {
                int attcnt = AllMediaList.Where(m => m.Attribute == attribute).Count();
                totalPage = (attcnt / MediaPopUpPageSize) + (attcnt % MediaPopUpPageSize == 0 ? 0 : 1);
                MediaPopUp attWise = new MediaPopUp()
                {
                    TabName = attribute,
                    TotalPages = totalPage,
                    Medias = AllMediaList.Where(m => m.Attribute == attribute).Take(MediaPopUpPageSize).ToList()
                };
                mediaPopUp.Add(attWise);
            }

            ViewBag.isEditor = isEditor;
            return PartialView(mediaPopUp);
        }
        public ActionResult LoadPopupMedia(string attributename, int CurrentPageIndex)
        {
            List<Media> MediaList = new List<Media>();
            if (attributename == "all")
            {
                MediaList = context.Media.Where(m => m.Type == "Image" && m.IsDeleted == false)
                    .OrderByDescending(m => m.ID).Skip(CurrentPageIndex * MediaPopUpPageSize).Take(MediaPopUpPageSize).ToList();
            }
            else
            {
                MediaList = context.Media.Where(m => m.Type == "Image" && m.Attribute == attributename && m.IsDeleted == false)
                   .OrderByDescending(m => m.ID).Skip(CurrentPageIndex * MediaPopUpPageSize).Take(MediaPopUpPageSize).ToList();
            }
            return PartialView("~/Areas/Admin/Views/Media/SingleImage.cshtml", MediaList);
        }
    }
}
