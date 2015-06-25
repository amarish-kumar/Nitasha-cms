using NITASA.Areas.Admin.Helper;
using NITASA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    [CheckLogin]
    public class SlidesController : Controller
    {
        private NTSDBContext context;
        public SlidesController()
        {
            this.context = new NTSDBContext();
        }

        [HttpGet]
        public ActionResult List(string GUID)
        {
            Slider slider = context.Sliders.Where(m => m.GUID == GUID && m.IsDeleted == false).FirstOrDefault();
            if (slider != null)
            {
                List<Slide> SlideList = slider.Slides.OrderBy(m => m.DisplayOrder).ToList();
                ViewBag.Slides = SlideList;

                Slide slide = new Slide();
                slide.SliderId = slider.Id;
                ViewBag.MGUID = GUID;
                ViewBag.MID = slider.Id;
                return View(slide);
            }
            else
            {
                TempData["ErrorMessage"] = "Slider not found.";
                return RedirectToAction("List","Slider");
            }
        }

        [HttpPost]
        public ActionResult Add(Slide model, string MGUID)
        {
            if (ModelState.IsValid)
            {
                Slider slider = context.Sliders.Where(x => x.Id == model.SliderId && x.IsDeleted == false).FirstOrDefault();
                if (slider != null)
                {
                    Slide slide = new Slide();
                    slide.GUID = Functions.GetRandomGUID();
                    slide.SliderId = slider.Id;
                    slide.Image = model.Image;
                    slide.Title = model.Title;
                    slide.Link = model.Link;
                    slide.Text = model.Text;

                    int slidecount = 0;

                    if (slider.Slides != null)
                        slidecount = slider.Slides.Count();

                    slide.AddedOn = DateTime.UtcNow;
                    slide.DisplayOrder = slidecount;
                    slide.AddedBy = Functions.CurrentUserID();
                    context.Slides.Add(slide);
                    context.SaveChanges();

                    TempData["SuccessMessage"] = "Slide saved successfully.";
                }
                else
                    TempData["ErrorMessage"] = "Slider not found.";
            }
            else
            {
                TempData["ErrorMessage"] = "Please select slide image.";
            }
            return RedirectToAction("List", "Slides", new { GUID = MGUID }); // here GUID is Slider GUID
        }

        [HttpGet]
        public ActionResult Delete(string GUID, string MGUID)
        {
            Slide slideToDelete = context.Slides.Where(m => m.GUID == GUID).Single();
            if (slideToDelete != null)
            {
                string pathToDelete = Server.MapPath(slideToDelete.Image);
                context.Slides.Remove(slideToDelete);
                context.SaveChanges();
                TempData["SuccessMessage"] = "Slide deleted successfully.";
            }
            else
                TempData["ErrorMessage"] = "Slide not found.";

            return RedirectToAction("List", "Slides", new { GUID = MGUID });
        }

        [HttpPost]
        public ActionResult UpdateDisplayOrder(string slideidlist, int MID)
        {
            Slider slider = context.Sliders.Where(x => x.Id == MID && x.IsDeleted == false).FirstOrDefault();
            if (slider != null && slider.Slides != null && slideidlist != null)
            {
                List<int> slideIdList = slideidlist.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(m => Convert.ToInt32(m)).ToList();
                int displayorder = 0;
                foreach (int sliderID in slideIdList)
                {
                    Slide slide = slider.Slides.Where(m => m.SliderId == sliderID).FirstOrDefault();
                    if (slider != null)
                    {
                        slide.DisplayOrder = displayorder++;
                        context.SaveChanges();
                    }
                }
                TempData["SuccessMessage"] = "Slide display order updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Slider not found.";
            }
            return RedirectToAction("List", new { GUID = slider.GUID });
        }
	}
}