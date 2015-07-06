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
    public class SliderController : Controller
    {
        private NTSDBContext context;
        public SliderController()
        {
            this.context = new NTSDBContext();
        }
        public ActionResult List()
        {
            List<Slider> SliderMasterList = context.Sliders.Where(m => m.IsDeleted == false).ToList().OrderByDescending(m => m.Id).ToList();
            ViewBag.Sliders = SliderMasterList;
            return View();
        }
        [HttpPost]
        public ActionResult Add(Slider model, string SMGUID)
        {
            if (ModelState.IsValid)
            {
                int duplicateSlider = 0;
                if (string.IsNullOrEmpty(SMGUID))
                    duplicateSlider = context.Sliders.Where(m => m.Name == model.Name && m.IsDeleted == false).Count();
                else
                    duplicateSlider = context.Sliders.Where(m => m.Name == model.Name && m.GUID != SMGUID && m.IsDeleted == false).Count();

                if (duplicateSlider == 0)
                {
                    if (string.IsNullOrEmpty(SMGUID)) // Add
                    {
                        Slider slider = new Slider();

                        slider.Name = model.Name;
                        slider.GUID = Functions.GetRandomGUID();
                        
                        slider.Code = model.Code;
                        slider.AddedOn = DateTime.UtcNow;
                        slider.AddedBy = Functions.CurrentUserID();
                        slider.IsDeleted = false;

                        context.Sliders.Add(slider);
                        context.SaveChanges();
                        TempData["SuccessMessage"] = "Slider added successfully.";
                    }
                    else // update
                    {
                        Slider slider = context.Sliders.Where(m => m.GUID == SMGUID).FirstOrDefault();
                        if (slider != null)
                        {
                            slider.Name = model.Name;
                            slider.Code = model.Code;
                            slider.ModifiedBy = Functions.CurrentUserID();
                            slider.ModifiedOn = DateTime.UtcNow;
                            context.SaveChanges();
                            TempData["SuccessMessage"] = "Slider updated successfully.";
                        }
                        else
                            TempData["SuccessMessage"] = "Slider not found.";
                    }
                }
                else
                    TempData["ErrorMessage"] = "Slider name already Exist. Please enter different Slider name.";
            }
            return RedirectToAction("List");
        }
        [HttpGet]
        public ActionResult Delete(string GUID)
        {
            Slider curSlider = context.Sliders.Where(m => m.GUID == GUID && m.IsDeleted == false).FirstOrDefault();
            if (curSlider != null)
            {
                List<Slide> SlideList = context.Slides.Where(m => m.SliderId == curSlider.Id).ToList();
                if (SlideList != null)
                {
                    context.Slides.RemoveRange(SlideList);
                }
                curSlider.IsDeleted = true;
                context.SaveChanges();
                TempData["SuccessMessage"] = "Slider deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Slider Not Found.";
            }
            return RedirectToAction("List");
        }

        public JsonResult GetSliderDetails(string GUID)
        {
            context.Configuration.LazyLoadingEnabled = false;
            context.Configuration.ProxyCreationEnabled = false;
            Slider slider = context.Sliders.Where(m => m.GUID == GUID).FirstOrDefault();
            return Json(slider, JsonRequestBehavior.AllowGet);
        }
	}
}