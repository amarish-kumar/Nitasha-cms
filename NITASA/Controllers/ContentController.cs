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
        public ActionResult Details(string URL)
        {
            Content content = context.Content.Where(x => x.URL.ToLower() == URL.ToLower() && x.IsDeleted == false && x.isPublished == true).FirstOrDefault();

            if (string.IsNullOrEmpty(URL) || content == null)
            {
                return RedirectToAction("NotFound", "Home"); 
            }
            ViewBag.ContentID = content.ID;

            CL_Content data = new CL_Content();
            data.URL = content.URL;
            data.Title = content.Title;
            data.FeaturedImage = content.FeaturedImage;
            data.CoverContent = content.CoverContent;
            data.AddedBy = content.AddedBy;
            data.PublishedOn = (DateTime)content.PublishedOn;
            
            if (content.Type.ToLower() == "page")
            {
                data.Description = ReplaceSliderAndAddons(content.Description);
                data.Type = "page";
            }
            else
            {
                data.Description = content.Description;
                data.Type = "post";
                data.PostCommentEnable = content.EnableComment;
                data.PostCommentEnabledTill = content.CommentEnabledTill;
                var comments = context.Comment.Where(x => x.ContentID == content.ID && x.IsModerated == true && x.IsAbused == false).ToList();
                data.CommentsCount = comments.Count();
                data.Comments = comments.Select(x =>
                    new CL_Comments { Description = x.Description, UserName = x.UserName, ProfilePicUrl = x.ProfilePicUrl, AddedOn = x.AddedOn }
                    ).ToList();                
            }
            
            int ContentView = content.ContentView;
            if (ContentView == 0)
                ContentView = 1;
            else
                ContentView++;
            content.ContentView = ContentView;
            context.Entry(content).State = EntityState.Modified; 
            context.SaveChanges();

            Functions.IncreaseContentView(content.ID, Request);

            return View(viewName: activeTheme + "content.cshtml", model: data); 
        }
        private string ReplaceSliderAndAddons(string ContentText)
        {
            string HTMLContent = HttpUtility.HtmlDecode(ContentText);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.OptionFixNestedTags = false;
            doc.OptionCheckSyntax = false;
            doc.LoadHtml(HTMLContent);

            var Sliders = doc.DocumentNode.SelectNodes("//slider");
            if (Sliders != null)
            {
                foreach (var item in Sliders)
                {
                    var HTMLSlider = "";
                    int sliderid = 0;
                    if (Int32.TryParse(item.InnerText, out sliderid))
                    {
                        List<CL_Slide> slides = context.Slides.Where(x => x.SliderId == sliderid).Select(x =>
                            new CL_Slide { Image = x.Image, Link = x.Link, Text = x.Text, Title = x.Title, DisplayOrder = x.DisplayOrder }
                            ).ToList();
                        HTMLSlider = Functions.RenderRazorViewToString(this.ControllerContext, activeTheme + "slider.cshtml", slides);
                    }
                    //HtmlAgilityPack.HtmlNode newNode = HtmlAgilityPack.HtmlNode.CreateNode(HTMLSlider);
                    //item.ParentNode.ReplaceChild(newNode, item);

                    HtmlAgilityPack.HtmlNode newNode = doc.CreateElement("div");
                    newNode.Attributes.Add("class", "divNTS");
                    newNode.InnerHtml = HTMLSlider;
                    item.ParentNode.ReplaceChild(newNode, item);
                }
            }
            var Addons = doc.DocumentNode.SelectNodes("//addon");
            if (Addons != null)
            {
                foreach (var item in Addons)
                {
                    var HTMLAddon = "";
                    int addonid = 0;
                    if (Int32.TryParse(item.InnerText, out addonid))
                    {
                        Content addon = context.Content.Where(x => x.ID == addonid && x.isPublished == true && x.IsDeleted == false).FirstOrDefault();
                        if (addon != null)
                        {
                            //string MasterLayout = addon.AddonMasterLayout;
                            HTMLAddon = addon.AddonSubLayout;
                            HTMLAddon = HTMLAddon.Replace("{{Title}}", addon.Title);
                            HTMLAddon = HTMLAddon.Replace("{{URL}}", (string.IsNullOrEmpty(addon.URL) ? "#" : addon.URL));
                            HTMLAddon = HTMLAddon.Replace("{{Description}}", addon.Description);
                        }
                    }
                    var newNode = HtmlAgilityPack.HtmlNode.CreateNode(HTMLAddon);
                    item.ParentNode.ReplaceChild(newNode, item);
                }
            }
            HTMLContent = doc.DocumentNode.OuterHtml;
            //HTMLContent = HTMLContent.Replace("<div class=\"divNTS\">", "");
            //HTMLContent =HTMLContent.Substring(0,HTMLContent.LastIndexOf("</div>"));
            return HTMLContent;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(string CommentDescription, int ContentID)
        {
            if (Session["CommentBy"] != null)
            {
                if (!string.IsNullOrEmpty(CommentDescription))
                {
                    Comment comment = new Comment();
                    comment.Description = CommentDescription;
                    comment.UserName = (string)Session["CommentBy"];
                    comment.ContentID = ContentID;
                    comment.AddedOn = DateTime.Now;
                    comment.ProfilePicUrl = (string)Session["CommentByImage"];
                    context.Comment.Add(comment);
                    context.SaveChanges();
                    TempData["comment-added"] = true;
                }
                else
                {
                    TempData["comment-error"] = "Comment is required";
                }
                return Redirect(Request.UrlReferrer.AbsolutePath);
            }
            else
                return Redirect(Request.UrlReferrer.AbsolutePath);
        }
	}
}