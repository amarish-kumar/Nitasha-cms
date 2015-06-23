using NITASA.Areas.Admin.Helper;
using NITASA.Data;
using NITASA.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    [CheckLogin]
    public class LabelController : Controller
    {
        public NTSDBContext context;
        public LabelController()
        {
            this.context = new NTSDBContext();
        }
        
        public ActionResult List(string SearchLabel = "")
        {
            List<Label> LabelList = context.Label.Where(m => m.Name.Contains(SearchLabel) && m.IsDeleted == false).ToList().OrderByDescending(m => m.ID).ToList();
            ViewBag.LabelList = LabelList;
            return View();
        }
         [HttpPost]
        public ActionResult Add(Label model, string LGUID)
        {
            if (ModelState.IsValid)
            {
                int duplicateLabel =0;
                if (string.IsNullOrEmpty(LGUID))
                    duplicateLabel = context.Label.Where(m => m.Name == model.Name && m.IsDeleted == false).Count();
                else
                    duplicateLabel = context.Label.Where(m => m.Name  == model.Name  && m.GUID != LGUID && m.IsDeleted == false).Count();

                if (duplicateLabel == 0)
                {
                    if (string.IsNullOrEmpty(LGUID)) // Add
                    {
                        Label label = new Label();

                        label.Name  = model.Name ;
                        label.GUID = Common.GetRandomGUID();
                        string LabelSlug = string.IsNullOrEmpty(model.Slug) ? model.Name : model.Slug;
                        label.Slug = Common.ToUrlSlug(LabelSlug, "label", 0);
                        label.Description = model.Description;
                        label.AddedOn = DateTime.UtcNow;
                        label.AddedBy = Convert.ToInt32(Session["UserID"]);
                        label.IsDeleted = false;

                        context.Label.Add(label);
                        context.SaveChanges();
                        TempData["SuccessMessage"] = "Label added successfully.";
                    }
                    else // update
                    {
                        Label label = context.Label.Where(m => m.GUID == LGUID).FirstOrDefault();
                        if (label != null)
                        {
                            label.Name= model.Name;
                            string LabelSlug = string.IsNullOrEmpty(model.Slug) ? model.Name : model.Slug;
                            label.Slug = Common.ToUrlSlug(LabelSlug, "label", label.ID);
                            label.Description = model.Description;
                            label.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                            label.ModifiedOn = DateTime.UtcNow;
                            context.SaveChanges();
                            TempData["SuccessMessage"] = "Label updated successfully.";
                        }
                        else
                            TempData["SuccessMessage"] = "Label not found.";
                    }
                }
                else
                    TempData["ErrorMessage"] = "Label already Exist. Please enter different Label.";
            }
            return RedirectToAction("List");
        }
        [HttpGet]
        public ActionResult Delete(string GUID)
        {
            Label curLabel = context.Label.Where(m => m.GUID == GUID && m.IsDeleted == false).FirstOrDefault();
            if (curLabel != null)
            {
                List<ContentLabel> conLabelList = context.ContentLabel.Where(m => m.LabelID == curLabel.ID).ToList();
                if (conLabelList != null)
                {
                    context.ContentLabel.RemoveRange(conLabelList);
                }
                curLabel.IsDeleted = true;
                context.SaveChanges();
                TempData["SuccessMessage"] = "Label deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Label Not Found.";
            }
            return RedirectToAction("List");
        }  
    }
}
