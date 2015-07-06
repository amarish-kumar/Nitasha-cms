using Newtonsoft.Json;
using NITASA.Areas.Admin.Helper;
using NITASA.Areas.Admin.ViewModels;
using NITASA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    [CheckLogin]
    public class HomeController : Controller
    {
        public NTSDBContext context;

        public HomeController()
        {
            this.context = new NTSDBContext();
        }

        public ActionResult Dashboard()
        {
            if (!UserRights.HasRights(Rights.ShowDashboard))
                return RedirectToAction("Details", "Profile");
                
            CustomizedDashboard dBoard = new CustomizedDashboard();

            dBoard.TotalPage = context.Content.Where(x => x.Type == "page" && x.IsDeleted == false).Count();
            #region Getting Last 12 Pages
            var pages = context.Content.Where(m => m.Type == "page" && m.IsDeleted == false).OrderByDescending(m => m.AddedOn).ToList();
            var pages1 = from r in pages
                         group r by r.AddedOn.Date.ToString("d") into g
                         select new { Date = g.Key, Pages = g.Count() };

            var getPages = pages1.Select(m => m.Pages).ToArray().Take(12);
            string Last10Pages = String.Join(",", getPages);

            if (pages1.Count() < 12)
            {
                string temp = "";
                int tempPages = pages1.Count();
                for (int i = 0; i < (12 - tempPages); i++)
                    temp += "0,";
                Last10Pages = temp + Last10Pages;
            }
            dBoard.LastPages = Last10Pages;
            #endregion

            dBoard.TotalPost = context.Content.Where(x => x.Type == "post" && x.IsDeleted == false).Count();
            #region Getting Last 12 Posts
            var posts = context.Content.Where(m => m.Type == "post" && m.IsDeleted == false).OrderByDescending(m => m.AddedOn).ToList();
            var posts1 = from r in posts
                         group r by r.AddedOn.Date.ToString("d") into g
                         select new { Date = g.Key, Posts = g.Count() };

            var getPosts = posts1.Select(m => m.Posts).ToArray().Take(12);
            string Last10Posts = String.Join(",", getPosts);

            if (posts1.Count() < 12)
            {
                string temp = "";
                int tempPosts = posts1.Count();
                for (int i = 0; i < (12 - tempPosts); i++)
                    temp += "0,";
                Last10Posts = temp + Last10Posts;
            }
            dBoard.LastPosts = Last10Posts;
            #endregion

            dBoard.TotalCategory = context.Category.Where(x => x.IsDeleted == false).Count();
            #region Getting Last 12 Categories

            var category = context.Category.Where(m => m.IsDeleted == false).OrderByDescending(m => m.AddedOn).ToList();
            var category1 = from r in category
                            group r by r.AddedOn.Date.ToString("d") into g
                            select new { Date = g.Key, Category = g.Count() };

            var getPcategory = category1.Select(m => m.Category).ToArray().Take(12);
            string Last10Category = String.Join(",", getPcategory);

            if (category1.Count() < 12)
            {
                string temp = "";
                int tempPosts = category1.Count();
                for (int i = 0; i < (12 - tempPosts); i++)
                    temp += "0,";
                Last10Category = temp + Last10Category;
            }
            dBoard.LastCategories = Last10Category;
            #endregion

            dBoard.TotalMedia = context.Media.Where(x => x.IsDeleted == false).Count();
            #region Getting Last 12 Medias
            var media = context.Media.Where(m => m.IsDeleted == false).OrderByDescending(m => m.AddedOn).ToList();
            var media1 = from r in media
                         group r by r.AddedOn.Value.Date.ToString("d") into g
                         select new { Date = g.Key, media = g.Count() };


            var getMedia = media1.Select(m => m.media).ToArray().Take(12);
            string Last10Media = String.Join(",", getMedia);

            if (media1.Count() < 12)
            {
                string temp = "";
                int tempPosts = media1.Count();
                for (int i = 0; i < (12 - tempPosts); i++)
                    temp += "0,";
                Last10Media = temp + Last10Media;
            }
            dBoard.LastMedia = Last10Media;
            #endregion

            dBoard.TotalDraft = context.Content.Where(x => x.isPublished == false && x.IsDeleted == false).Count();
            #region Getting Last 12 Drafts
            var drafts = context.Content.Where(m => m.isPublished == false && m.IsDeleted == false).OrderByDescending(m => m.AddedOn).ToList();
            var drafts1 = from r in drafts
                          group r by r.AddedOn.Date.ToString("d") into g
                          select new { Date = g.Key, Drafts = g.Count() };

            var getDrafts = drafts1.Select(m => m.Drafts).ToArray().Take(12);
            string Last10Drafts = String.Join(",", getDrafts);

            if (drafts1.Count() < 12)
            {
                string temp = "";
                int tempPosts = drafts1.Count();
                for (int i = 0; i < (12 - tempPosts); i++)
                    temp += "0,";
                Last10Drafts = temp + Last10Drafts;
            }
            dBoard.LastDrafts = Last10Drafts;
            #endregion

            dBoard.TotalUsers = context.User.Where(x => x.IsDeleted == false).Count();
            #region Getting Last 12 Users
            var users = context.User.Where(m => m.IsDeleted == false).OrderByDescending(m => m.AddedOn).ToList();
            var users1 = from r in users
                         group r by r.AddedOn.Date.ToString("d") into g
                         select new { Date = g.Key, Users = g.Count() };

            var getUsers = users1.Select(m => m.Users).ToArray().Take(12);
            string Last10Users = String.Join(",", getUsers);

            if (users1.Count() < 12)
            {
                string temp = "";
                int tempPosts = users1.Count();
                for (int i = 0; i < (12 - tempPosts); i++)
                    temp += "0,";
                Last10Users = temp + Last10Users;
            }
            dBoard.LastUsers = Last10Users;
            #endregion

            dBoard.TotalRoles = context.Role.Where(x => x.IsDeleted == false).Count();
            #region Getting Last 12 Roles
            var roles = context.Role.Where(m => m.IsDeleted == false).OrderByDescending(m => m.AddedOn).ToList();
            var roles1 = from r in roles
                         group r by r.AddedOn.Date.ToString("d") into g
                         select new { Date = g.Key, Roles = g.Count() };

            var getRoles = roles1.Select(m => m.Roles).ToArray().Take(12);
            string Last10Roles = String.Join(",", getRoles);

            if (roles1.Count() < 12)
            {
                string temp = "";
                int tempPosts = roles1.Count();
                for (int i = 0; i < (12 - tempPosts); i++)
                    temp += "0,";
                Last10Roles = temp + Last10Roles;
            }
            dBoard.LastRoles = Last10Roles;
            #endregion

            dBoard.TotalNewUsers = context.User.Where(x => x.IsDeleted == false).OrderByDescending(m => m.AddedOn).ToList().Where(s => s.AddedOn > DateTime.Now.AddMonths(-1)).Count();
            #region Getting Last 12 New Users
            var newUsers = context.User.Where(m => m.IsDeleted == false).OrderByDescending(m => m.AddedOn).ToList().Where(s => s.AddedOn > DateTime.Now.AddMonths(-1)).ToList();
            var newUsers1 = from r in newUsers
                            group r by r.AddedOn.Date.ToString("d") into g
                            select new { Date = g.Key, Roles = g.Count() };

            var getNewUsers = newUsers1.Select(m => m.Roles).ToArray().Take(12);
            string Last10NewUsers = String.Join(",", getNewUsers);

            if (newUsers1.Count() < 12)
            {
                string temp = "";
                int tempPosts = newUsers1.Count();
                for (int i = 0; i < (12 - tempPosts); i++)
                    temp += "0,";
                Last10NewUsers = temp + Last10NewUsers;
            }
            dBoard.LastNewUsers = Last10NewUsers;
            #endregion

            Setting TotalPageView = context.Settings.Where(m => m.Name == "TotalPageView").FirstOrDefault();
            if (TotalPageView != null)
                dBoard.TotalPageview = Convert.ToInt32(TotalPageView.Value);
            else
                dBoard.TotalPageview = 0;

            #region Getting Last 12 days PageViews


            var pageviews = context.ContentView.ToList();
            var pageviews1 = from r in pageviews
                             group r by r.ViewedOn.Value.Date.ToString("d") into g
                             select new { Date = g.Key, Views = g.Count() };

            var getViews = pageviews1.Select(m => m.Views).ToArray().Take(12);
            //string Last10Views = String.Join(",", getViews);

            //if (pageviews1.Count() < 12)
            //{
            //    string temp = "";
            //    int tempPosts = pageviews1.Count();
            //    for (int i = 0; i < (12 - tempPosts); i++)
            //        temp += "0,";
            //    Last10Views = temp + Last10Views;
            //}
            //dBoard.LastPageview = Last10Views;

            #endregion

            dBoard.PostList = context.Content.Where(content => content.Type.ToLower() == "post" && content.IsDeleted == false && content.isPublished == true).OrderByDescending(content => content.AddedOn).ToList().Where(m => m.AddedOn > DateTime.Now.AddDays(-7)).Take(5).ToList();
            dBoard.RecentPublishedPostList = context.Content.Where(content => content.Type.ToLower() == "post" && content.IsDeleted == false && content.isPublished == true).OrderByDescending(content => content.AddedOn).Take(5).ToList();
            dBoard.RecentPages = context.Content.Where(content => content.Type.ToLower() == "page" && content.IsDeleted == false && content.isPublished == true).OrderByDescending(content => content.AddedOn).Take(5).ToList();
            dBoard.UnModeratedComments = context.Comment.Where(comment => comment.IsModerated == false).OrderByDescending(x => x.AddedOn).Take(5).ToList();
            //dBoard.ActivityLogs = context.ActivityLog.OrderByDescending(log => log.AddedOn).Take(5).ToList();
            var pageView = context.ContentView.OrderByDescending(m => m.ViewedOn).ToList();
            var pageView1 = from r in pageView
                            group r by Convert.ToDateTime(r.ViewedOn).ToString("MMM-dd") into g
                            select new ChartData { x = g.Key, y = g.Count() };
            string chartData = JsonConvert.SerializeObject(pageView1.ToList());
            //JsonSerializerSettings js = new JsonSerializerSettings();
            //js.
            //JsonConvert.SerializeObject(
            dBoard.chartData = chartData;

            //dBoard.chartData = context.ContentView.OrderByDescending(x => x.ViewedOn)
            //    .Select( g => new  ChartData { x = Convert.ToString(g.ViewedOn),y = g.ViewID.CompareTo})
            //    .GroupBy(content => content.ViewedOn)
            //    .Select().Take(7)
            //    .ToList();
            //dBoard.TotalComments = context.cmsadminco.Where(x => x.IsDeleted == false).Count();


            //Dashboard dBoard = new Dashboard();
            //// context.Content.Where(x => x.IsDeleted == false).OrderBy(x => x.CategoryName).ToList();
            //dBoard.TotalPost = context.Content.Where(x => x.Type == "post").Count();
            //dBoard.TotalPages = context.Content.Where(x => x.Type == "page").Count();
            //dBoard.TotalCategory = context.Category.Where(x => x.IsDeleted == false).Count();
            //dBoard.TotalLabels = context.Label.Count();
            //dBoard.TotalPageView = context.Config.FirstOrDefault().TotalPageView;
            //dBoard.TotalMedia = context.Media.Where(x=>x.IsDeleted==false).Count();
            //dBoard.PostDrafted = context.Content.Where(x => x.Type == "post" && x.isPublished==false && x.IsDeleted==false).Count();
            //dBoard.PostPublished = context.Content.Where(x => x.Type == "post" && x.isPublished == true && x.IsDeleted == false).Count();
            //dBoard.PageDrafted = context.Content.Where(x => x.Type == "page" && x.isPublished == false && x.IsDeleted == false).Count(); 
            //dBoard.PagePublished = context.Content.Where(x => x.Type == "page" && x.isPublished == true && x.IsDeleted == false).Count(); 
            return View(dBoard);
        }

        [HttpPost]
        public ActionResult AddDraft(CustomizedDashboard dboard)
        {
            Content cont = new Content();
            cont.Type = "Post";
            cont.Title = dboard.PostTitle;
            var sanitizer = new Ganss.XSS.HtmlSanitizer();
            cont.Description = sanitizer.Sanitize(dboard.PostDescription);
            cont.GUID = Functions.GetRandomGUID();
            cont.URL = Functions.ToUrlSlug(dboard.PostTitle, "post", 0);

            //cont.EnableComment = Convert.ToBoolean(Request.Form["ckhCommentEnabled"]);
            //int commentEnabledDays = Convert.ToInt32(Request.Form["ddlEnableTill"]);
            //cont.CommentEnabledTill = commentEnabledDays;

            cont.EnableComment = false;
            cont.CommentEnabledTill = 0;
            cont.AddedBy = Functions.CurrentUserID();
            cont.AddedOn = DateTime.Now;
            cont.isPublished = false;
            cont.IsDeleted = false;
            context.Content.Add(cont);
            context.SaveChanges();
            TempData["PostMessage"] = "Post saved to draft successfully.";

            return RedirectToAction("Dashboard");
        }
        public ActionResult AccessDenied()
        {
            return View();
        }
        public ActionResult NotFound()
        {
            return View();
        }
    }
}
