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
        int PageSize = 0;
        public CategoryController()
        {
            this.context = new NTSDBContext();
        }

        public ActionResult List(string URL, Pager pager)
        {
            PageSize = Functions.PageSize();

            Category category = context.Category.Where(x => x.Slug.ToLower() == URL.ToLower() && x.IsDeleted == false).FirstOrDefault();

            if (category == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            IQueryable<Content> query = context.ContentCategory.Include("Content").Where(x => x.CategoryID == category.ID)
            .Select(x => x.Content).Where(x => x.isPublished == true && x.IsDeleted == false);

            if (pager.CurrentPageIndex == null || pager.PageSize== null || pager.PageCount == null) // first time
            {
                int totalRecords = query.Count();
                int totalPage = (totalRecords / PageSize) + (totalRecords % PageSize == 0 ? 0 : 1);
                pager = new Pager { PageSize = PageSize, CurrentPageIndex = 0, PageCount = totalPage, Controller = "Category", Action = "List" };
            }

            CL_Category data = new CL_Category();
            data.Name = category.Name;
            data.Description = category.Description;
            data.ParentCategoryID = category.ParentCategoryID;
            data.URL = category.Slug;
            data.Posts = GetPosts(query,pager);
            data.Pager = pager;
            Functions.IncreaseContentView(0, Request);
            return View(viewName: activeTheme + "category.cshtml", model: data);
        }

        private List<CL_Content> GetPosts(IQueryable<Content> query, Pager pager)
        {
            List<Content> PostList = query.OrderBy(x => x.ContentOrder).Skip((int)pager.CurrentPageIndex * (int)pager.PageSize).Take((int)pager.PageSize).ToList();
            List<CL_Content> Posts = PostList.Select(x =>
                       new CL_Content
                       {
                           ContentID = x.ID,
                           Title = x.Title,
                           Description = x.Description,
                           FeaturedImage = x.FeaturedImage,
                           URL = x.URL,
                           CoverContent = x.CoverContent,
                           PublishedOn = x.AddedOn,
                           AddedBy = x.AddedBy,
                           CommentsCount = x.Comments.Where(c => c.ContentID == c.ID && c.IsModerated == true && c.IsAbused == false).Count()
                       }
                   ).ToList();
            return Posts;
        }
    }
}