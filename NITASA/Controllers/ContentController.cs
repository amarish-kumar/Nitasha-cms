using NITASA.Data;
using NITASA.Helpers;
using NITASA.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace NITASA.Controllers
{
    [AllAction]
    public class ContentController : AppController
    {
        public NTSDBContext context;
        public ContentController()
        {
            this.context = new NTSDBContext();                
        }
        public ActionResult Details(string URL, string prv)
        {
            Content content;
            bool isPreview = false;
            if (Session["Preview"] != null && prv=="true")
            {
                content = (Content)Session["Preview"];
                isPreview = true;
            }
            else
            {
                content = context.Content.Where(x => x.URL.ToLower() == URL.ToLower() && x.IsDeleted == false && x.isPublished == true).FirstOrDefault();

                if (string.IsNullOrEmpty(URL) || content == null)
                {
                    ActionResult actionResult = View(viewName: activeTheme + "404.cshtml");
                    return actionResult;
                }
                ViewBag.ContentID = content.ID;
            }
            CL_Content data = new CL_Content();
            data.ContentID = content.ID;
            data.URL = content.URL;
            data.Title = content.Title;
            data.FeaturedImage = content.FeaturedImage;
            data.CoverContent = content.CoverContent;
            data.AddedBy = content.AddedBy;
            data.PublishedOn = (DateTime)content.PublishedOn;
            
            if (content.Type.ToLower() == "page")
            {
                data.Description = Functions.ReplaceSliderAndAddons(this.ControllerContext, activeTheme, content.Description);
                data.Type = "page";
            }
            else
            {
                data.Description = content.Description;
                data.Type = "post";
                data.PostCommentEnable = content.EnableComment;
                data.PostCommentEnabledTill = content.CommentEnabledTill;
                var comments = context.Comment.Where(x => x.ContentID == content.ID && x.IsModerated == true && x.IsAbused == false).OrderBy(x=>x.AddedOn).ToList();
                data.CommentsCount = comments.Count();
                data.Comments = comments.Select(x =>
                    new CL_Comments
                    {
                        CommentID = x.ID,
                        ContentID = content.ID,
                        Website = x.Website,
                        CommentAs = x.CommentAs,
                        Description = x.Description,
                        UserName = x.UserName,
                        ProfilePicUrl = x.ProfilePicUrl,
                        AddedOn = x.AddedOn
                    }).ToList();
            }
            if (!isPreview)
            {
                int ContentView = content.ContentView;
                if (ContentView == 0)
                    ContentView = 1;
                else
                    ContentView++;
                content.ContentView = ContentView;
                context.Entry(content).State = EntityState.Modified;
                context.SaveChanges();

                Functions.IncreaseContentView(content.ID, Request);
            }
            return View(viewName: activeTheme + "content.cshtml", model: data); 
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(string CommentDescription, string UserName, string ProfilePicUrl, string Website, string CommentAs, int ContentID)
        {
            if(string.IsNullOrEmpty(CommentDescription) && ContentID != 0)
            {
                TempData["comment-error"] ="Comment is required";
            }
            else if (CommentAs == "google" && string.IsNullOrEmpty(UserName))
            {
                TempData["comment-error"] ="Google login required";
            }
            else if (CommentAs == "nameurl" && (string.IsNullOrEmpty(UserName)))
            {
                TempData["comment-error"] ="Name required";
            }
            else
            {
                if (CommentAs == "nameurl")
                {
                    ProfilePicUrl = null;
                }
                else if (CommentAs == "anonymous")
                {
                    UserName = "Anonymous";
                    Website = null;
                    ProfilePicUrl = null;
                }
                Comment comment = new Comment();
                comment.Description = CommentDescription;
                comment.UserName = UserName;
                comment.ProfilePicUrl = ProfilePicUrl;
                comment.Website = Website;
                comment.CommentAs = CommentAs;
                comment.ContentID = ContentID;
                comment.AddedOn = DateTime.UtcNow;
                comment.ProfilePicUrl = ProfilePicUrl;
                context.Comment.Add(comment);
                context.SaveChanges();
                TempData["comment-added"] = true;
            }
            return Redirect(Request.UrlReferrer.AbsolutePath);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFlag(string AbuseReason, string AbuseByUserName, int CommentID)
        {
            if (!string.IsNullOrEmpty(AbuseReason) && CommentID != 0)
            {
                Comment comment = context.Comment.Find(CommentID);
                comment.IsAbused = true;
                comment.AbusedBy = AbuseByUserName.Trim();
                comment.AbusedReason = AbuseReason;
                context.SaveChanges();
                TempData["flag-added"] = true;
            }
            else
            {
                TempData["flag-error"] = "Something wrong happen while flagging comment.";
            }
            return Redirect(Request.UrlReferrer.AbsolutePath);
        }
	}
}