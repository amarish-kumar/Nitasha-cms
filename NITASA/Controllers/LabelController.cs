using NITASA.Data;
using NITASA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Controllers
{
    public class LabelController : AppController
    {
        public NTSDBContext context;
        public LabelController()
        {
            this.context = new NTSDBContext();
        }

        public ActionResult List(string URL)
        {
            Label label = context.Label.Where(x => x.Slug.ToLower() == URL.ToLower() && x.IsDeleted == false).FirstOrDefault();

            if (label == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            List<Content> PostList = context.ContentLabel.Include("Content").Where(x => x.LabelID == label.ID)
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

            CL_Label data = new CL_Label();
            data.Name = label.Name;
            data.Description = label.Description;
            data.URL = label.Slug;
            data.Posts = Posts;

            return View(viewName: activeTheme + "label.cshtml", model: data);
        }
	}
}