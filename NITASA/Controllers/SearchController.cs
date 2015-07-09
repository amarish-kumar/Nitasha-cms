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
    public class SearchController : AppController
    {
        public NTSDBContext context;
        int PageSize = 0;
        public SearchController()
        {
            this.context = new NTSDBContext();
        }

        public ActionResult Results(Pager pager)
        {
            string SearchText = pager.SearchText;

            PageSize = Functions.PageSize();

            if (string.IsNullOrEmpty(SearchText))
            {
                return RedirectToAction("Index", "Home");
            }
            IQueryable<Content> query = context.Content.Where(x => x.Type.ToLower() == "post" && (x.Title.Contains(SearchText) || x.Description.Contains(SearchText))
                    && x.isPublished == true && x.IsDeleted == false);

            if (pager.CurrentPageIndex == null || pager.PageSize == null || pager.PageCount == null) // first time
            {
                int totalRecords = query.Count();
                int totalPage = (totalRecords / PageSize) + (totalRecords % PageSize == 0 ? 0 : 1);
                pager = new Pager { PageSize = PageSize, CurrentPageIndex = 0, PageCount = totalPage, Controller = "Search", Action = "Results", SearchText = SearchText };
            }

            CL_SearchResult data = new CL_SearchResult();
            data.SearchText = SearchText;
            data.Posts = GetPosts(query, pager);
            data.Pager = pager;

            Functions.IncreaseContentView(0, Request);
            return View(viewName: activeTheme + "search_result.cshtml", model: data);
        }
        private List<CL_Content> GetPosts(IQueryable<Content> query, Pager pager)
        {
            List<Content> PostList = query.OrderBy(x => x.Title).Skip((int)pager.CurrentPageIndex * (int)pager.PageSize).Take((int)pager.PageSize).ToList();
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
                           CommentsCount = x.Comments.Where(c => c.ContentID == c.ID && c.IsModerated == true && c.IsAbused == false).Count(),
                           Category = x.Categories.Select(c => new CL_Category
                            {
                                Name = c.Category.Name,
                                Description = c.Category.Description,
                                URL = c.Category.Slug
                            }).ToList(),
                           Labels = x.Labels.Select(c => new CL_Label
                           {
                               Name = c.Label.Name,
                               Description = c.Label.Description,
                               URL = c.Label.Slug
                           }).ToList(),
                           Comments = x.Comments.Select(c => new CL_Comments
                           {
                               UserName = c.UserName,
                               AddedOn = c.AddedOn,
                               CommentAs = c.CommentAs,
                               CommentID = c.ID,
                               Description = c.Description,
                               ContentID = c.ContentID,
                               Website = c.Website,
                               ProfilePicUrl = c.ProfilePicUrl
                           }).ToList()
                       }
                   ).ToList();
            return Posts;
        }
    }
}