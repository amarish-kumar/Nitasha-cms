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
    public class CategoryController : Controller
    {
        public NTSDBContext context;
        public CategoryController()
        {
            this.context = new NTSDBContext();
        }
        
        public ActionResult List()
        {
            List<Category> CategoryList = context.Category.Where(m => m.IsDeleted == false).ToList().OrderByDescending(m => m.ID).ToList();
            ViewBag.CategoryList = CategoryList;
            return View();
        }
        [HttpGet]
        public ActionResult Add()
        {
            return RedirectToAction("List");
        }
        [HttpPost]
        public ActionResult Add(Category cmv, string CGUID)
        {
            if (ModelState.IsValid)
            {
                int duplicateCategory =0;
                if (string.IsNullOrEmpty(CGUID))
                    duplicateCategory = context.Category.Where(m => m.Name == cmv.Name && m.IsDeleted == false).Count();
                else
                    duplicateCategory = context.Category.Where(m => m.Name == cmv.Name && m.GUID != CGUID && m.IsDeleted == false).Count();

                if (duplicateCategory == 0)
                {
                    if (string.IsNullOrEmpty(CGUID)) // Add
                    {
                        Category cat = new Category();

                        cat.Name = cmv.Name;
                        cat.GUID = Functions.GetRandomGUID();
                        string catSlug = string.IsNullOrEmpty(cmv.Slug) ? cmv.Name : cmv.Slug;
                        cat.Slug = Functions.ToUrlSlug(catSlug, "category", 0);
                        cat.Description = cmv.Description;
                        cat.ParentCategoryID = 0;
                        cat.AddedOn= DateTime.UtcNow;
                        cat.AddedBy= Convert.ToInt32(Session["UserID"]);
                        cat.IsDeleted = false;

                        context.Category.Add(cat);
                        context.SaveChanges();
                        TempData["SuccessMessage"] = "Category added successfully.";
                    }
                    else // update
                    {
                        Category cat = context.Category.Where(m => m.GUID == CGUID).FirstOrDefault();
                        if (cat != null)
                        {
                            cat.Name = cmv.Name;
                            string catSlug = string.IsNullOrEmpty(cmv.Slug) ? cmv.Name : cmv.Slug;
                            cat.Slug = Functions.ToUrlSlug(catSlug, "category", cat.ID);
                            cat.Description = cmv.Description;
                            cat.ModifiedBy = Convert.ToInt32(Session["UserID"]);
                            cat.ModifiedOn = DateTime.UtcNow;
                            context.SaveChanges();
                            TempData["SuccessMessage"] = "Category updated successfully.";
                        }
                        else
                            TempData["SuccessMessage"] = "Category not found.";
                    }
                }
                else
                    TempData["ErrorMessage"] = "Category already Exist. Please enter different category.";
            }
            return RedirectToAction("List");
        }
        [HttpPost]
        public JsonResult PostCount(string GUID)
        {
            int postCount = (from r in context.ContentCategory
                             join c in context.Category on r.CategoryID equals c.ID
                             where c.GUID == GUID
                             select r.ID).Count();

            return Json(postCount, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Delete(string GUID)
        {
            Category curCat = context.Category.Where(m => m.GUID == GUID && m.IsDeleted == false).FirstOrDefault();
            if (curCat != null)
            {
                List<ContentCategory> conCatList = context.ContentCategory.Where(m => m.CategoryID == curCat.ID).ToList();
                if (conCatList != null)
                {
                    context.ContentCategory.RemoveRange(conCatList);
                }
                curCat.IsDeleted = true;
                context.SaveChanges();

                TempData["SuccessMessage"] = "Category deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Category Not Found.";
            }
            return RedirectToAction("List");
        }

        public JsonResult GetCategoryDetails(string GUID)
        {
            Category cat = context.Category.Where(m => m.GUID == GUID).FirstOrDefault();
            return Json(cat, JsonRequestBehavior.AllowGet);
        }
    }
}
