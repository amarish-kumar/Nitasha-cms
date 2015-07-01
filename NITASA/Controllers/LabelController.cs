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
    public class LabelController : AppController
    {
        public NTSDBContext context;
        int PageSize = 0;
        public LabelController()
        {
            this.context = new NTSDBContext();
        }

        public ActionResult List(string URL, Pager pager)
        {
            PageSize = Functions.PageSize();

            Label label = context.Label.Where(x => x.Slug.ToLower() == URL.ToLower() && x.IsDeleted == false).FirstOrDefault();

            if (label == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            IQueryable<Content> query = context.ContentLabel.Include("Content").Where(x => x.LabelID == label.ID)
                .Select(x=>x.Content).Where(x=>x.isPublished==true && x.IsDeleted==false);

            if (pager.CurrentPageIndex == null || pager.PageSize == null || pager.PageCount == null) // first time
            {
                int totalRecords = query.Count();
                int totalPage = (totalRecords / PageSize) + (totalRecords % PageSize == 0 ? 0 : 1);
                pager = new Pager { PageSize = PageSize, CurrentPageIndex = 0, PageCount = totalPage, Controller = "Label", Action = "List" };
            }

            CL_Label data = new CL_Label();
            data.Name = label.Name;
            data.Description = label.Description;
            data.URL = label.Slug;
            data.Posts = GetPosts(query, pager);
            data.Pager = pager;

            return View(viewName: activeTheme + "label.cshtml", model: data);
        }
        private List<CL_Content> GetPosts(IQueryable<Content> query, Pager pager)
        {
            List<Content> PostList = query.OrderByDescending(content => content.PublishedOn).Skip((int)pager.CurrentPageIndex * (int)pager.PageSize).Take((int)pager.PageSize).ToList();
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
            return Posts;
        }
	}
}