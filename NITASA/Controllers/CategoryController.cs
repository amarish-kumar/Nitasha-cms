using NITASA.Data;
using NITASA.Helpers;
using NITASA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Controllers
{
    [AllAction]
    public class CategoryController : AppController
    {
        public NTSDBContext context;
        public CategoryController()
        {
            this.context = new NTSDBContext();
        }

        public ActionResult List(string URL)
        {
            Category category = context.Category.Where(x => x.Slug.ToLower() == URL.ToLower() && x.IsDeleted == false).FirstOrDefault();

            if (category == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            List<Content> PostList = context.ContentCategory.Include("Content").Where(x=> x.CategoryID == category.ID)
            .Select(x=>x.Content).Where(x=>x.isPublished==true && x.IsDeleted==false).OrderByDescending(x=>x.PublishedOn).ToList();

            List<CL_Content> Posts = PostList.Select(x =>
                        new CL_Content
                        {
                            Title = x.Title,
                            Description = x.Description,
                            FeaturedImage = x.FeaturedImage,
                            URL = x.URL,
                            CoverContent = x.CoverContent,
                            PublishedOn = (DateTime)x.PublishedOn,
                            AddedBy = x.AddedBy,
                            CommentsCount = context.Comment.Where(c => c.ContentID == c.ID && c.IsModerated == true && c.IsAbused == false).Count()
                        }
                    ).ToList();

            CL_Category data = new CL_Category();
            data.Name = category.Name;
            data.Description = category.Description;
            data.ParentCategoryID = category.ParentCategoryID;
            data.URL = category.Slug;
            data.Posts = Posts;

            return View(viewName: activeTheme + "category.cshtml", model: data);
        }
	}
}