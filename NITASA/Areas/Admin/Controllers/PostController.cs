using CsQuery;
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
    public class PostController : Controller
    {
        
        public NTSDBContext context;

        public PostController()
        {
            this.context = new NTSDBContext();
        }
        public ActionResult List(int cid = 0)
        {
            bool ViewAllPostsRights = UserRights.HasRights(Rights.ViewAllPosts);
            bool ViewUnPublishedPostsRights = UserRights.HasRights(Rights.ViewUnPublishedPosts);

            if (!ViewAllPostsRights && !ViewUnPublishedPostsRights)
                return RedirectToAction("AccessDenied", "Home");

            List<Category> categoryList = context.Category.Where(m => m.IsDeleted == false).ToList();
            ViewBag.categoryList = new SelectList(categoryList, "CategoryID", "CategoryName");

            IQueryable<Content> Content = context.Content.Include("user").Include("contentCategory").Include("contentLabel");
            if (ViewAllPostsRights)
                Content = Content.Where(m => m.Type.ToLower() == "post" && m.IsDeleted == false);
            else if (ViewUnPublishedPostsRights)
                Content = Content.Where(m => m.Type.ToLower() == "post" && m.IsDeleted == false && m.isPublished == false);

            if (cid > 0)
                Content = Content.Where(cont => cont.contentCategory.Where(m => m.CategoryID == cid).Count() > 0);

            List<Content> ContentList = Content.OrderByDescending(m => m.PublishedOn).ToList();

            return View(ContentList);
        }
        [HttpGet]
        public ActionResult Add()
        {
            if (!UserRights.HasRights(Rights.CreateNewPosts))
                return RedirectToAction("AccessDenied", "Home");

            ViewBag.Labellist = new SelectList(context.Label.ToList(), "LabelID", "LabelName");
            ViewBag.Categorylist = new SelectList(context.Category.Where(m => m.IsDeleted == false).ToList(), "CategoryID", "CategoryName");
            //ViewBag.Templatelist = new SelectList(context.ContentTemplate.Where(m => m.IsDeleted == false).ToList(), "TemplateID", "TemplateName");

            return View();
        }
        [HttpPost]
        public ActionResult Add(Content content)
        {
            if (!UserRights.HasRights(Rights.CreateNewPosts))
                return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid)
            {
                string postContent = content.Description.Replace("&nbsp;", "").Replace("<p>", "").Replace("</p>", "").Trim();
                if (!string.IsNullOrEmpty(postContent))
                {
                    if (Request.Form["hdnIsPost"] != null)
                    {
                        string currOperation = Request.Form["hdnIsPost"];
                        if (currOperation == "Post")
                        {
                            SavePostDetails(content, true);
                            TempData["SuccessMessage"] = "Post published successfully.";
                        }
                        else
                        {
                            SavePostDetails(content, false);
                            TempData["SuccessMessage"] = "Post saved to draft successfully.";
                        }
                        return RedirectToAction("List", "Post");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Please enter post content";
                }
            }
            ViewBag.Labellist = new SelectList(context.Label.ToList(), "LabelID", "LabelName");
            ViewBag.Categorylist = new SelectList(context.Category.Where(m => m.IsDeleted == false).ToList(), "CategoryID", "CategoryName");
            //ViewBag.Templatelist = new SelectList(context.ContentTemplate.Where(m => m.IsDeleted == false).ToList(), "TemplateID", "TemplateName");

            return View(content);
        }
        public void SavePostDetails(Content Model, bool isPublished)
        {
            Content cont = new Content();
            cont.Type = "Post";
            cont.Title = Model.Title;

            //var sanitizer = new Html.HtmlSanitizer();
            CQ doc = CQ.Create(Model.Description);
            string[] BlackList = new string[] { "script" };
            string selector = String.Join(",", BlackList);
            doc = doc[selector].Remove();
            Model.Description = doc.Render();

            cont.Description = Model.Description;
            cont.GUID = Common.GetRandomGUID();
            cont.URL = Common.ToUrlSlug(Model.URL, "post", 0);
            //cont.TemplateID = Model.TemplateID;
            cont.IsSlugEdited = Model.IsSlugEdited;
            cont.IsFeatured = Model.IsFeatured;
            cont.ContentOrder = Model.ContentOrder;

            cont.EnableComment = Model.EnableComment;
            cont.CommentEnabledTill = Model.CommentEnabledTill;

            cont.AddedOn = DateTime.Now;
            cont.AddedBy = Common.CurrentUserID();
            if (isPublished)
            {
                cont.isPublished = true;
                cont.PublishedOn = cont.AddedOn;
            }
            else
            {
                cont.isPublished = false;
            }
            context.Content.Add(cont);
            context.SaveChanges();

            if (Request.Form["LabelId"] != null)
            {
                string[] allLables = Request.Form["LabelId"].Split(',');
                foreach (string currLblID in allLables)
                {
                    ContentLabel contLabel = new ContentLabel();
                    contLabel.ContentID = cont.ID;
                    contLabel.LabelID = Convert.ToInt32(currLblID);
                    contLabel.AddedOn = DateTime.Now;
                    contLabel.AddedBy = Common.CurrentUserID();
                    context.ContentLabel.Add(contLabel);
                    context.SaveChanges();
                }
            }
            if (Request.Form["CategoryId"] != null)
            {
                string[] allCategories = Request.Form["CategoryId"].Split(',');
                foreach (string currCatID in allCategories)
                {
                    ContentCategory contentCategory = new ContentCategory();
                    contentCategory.ContentID = cont.ID;
                    contentCategory.CategoryID = Convert.ToInt32(currCatID);
                    contentCategory.AddedOn = DateTime.Now;
                    contentCategory.AddedBy = Common.CurrentUserID();
                    context.ContentCategory.Add(contentCategory);
                    context.SaveChanges();
                }
            }
            if (!string.IsNullOrWhiteSpace(Request.Form["txtMetaKeyword"]) || !string.IsNullOrWhiteSpace(Request.Form["txtMetaDescription"]) || !string.IsNullOrWhiteSpace(Request.Form["txtMetaAuthor"]))
            {
                Meta metaData = new Meta();
                metaData.ContentID = cont.ID;
                metaData.Keyword = Request.Form["txtMetaKeyword"].ToString();
                metaData.Description = Request.Form["txtMetaDescription"].ToString();
                metaData.Author = Request.Form["txtMetaAuthor"].ToString();
                metaData.CreatedOn = DateTime.Now;
                context.Meta.Add(metaData);
                context.SaveChanges();
            }
        }
        [HttpPost]
        public JsonResult AddLabel(string labelName)
        {
            var dupLabel = context.Label.Where(m => m.Name.ToLower() == labelName.Trim().ToLower());
            if (dupLabel.Count() == 0)
            {
                Label lbl = new Label();
                lbl.Name = labelName;
                lbl.Slug = Common.ToUrlSlug(labelName, "label", 0);
                lbl.Description = "";
                lbl.AddedOn = DateTime.UtcNow;
                lbl.AddedBy = Convert.ToInt32(Session["UserID"]);
                lbl.IsDeleted = false;
                context.Label.Add(lbl);
                context.SaveChanges();
                int lastRecordID = lbl.ID;
                return Json(lastRecordID, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult AddCategory(string CategoryName)
        {
            var dupCat = context.Category.Where(m => m.Name.ToLower() == CategoryName.Trim().ToLower() && m.IsDeleted == false);
            if (dupCat.Count() == 0)
            {
                Category cat = new Category();
                cat.Name = CategoryName;
                cat.Slug = Common.ToUrlSlug(CategoryName, "category", 0);
                cat.ParentCategoryID = 0;
                cat.AddedOn = DateTime.UtcNow;
                cat.AddedBy = Convert.ToInt32(Session["UserID"]);
                cat.IsDeleted = false;

                context.Category.Add(cat);
                context.SaveChanges();
                int lastRecordID = cat.ID;
                return Json(lastRecordID, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetMedia(Content content)
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult Delete(string GUID)
        {
            Content curCon = context.Content.Where(m => m.GUID == GUID).FirstOrDefault();
            if (curCon != null)
            {
                if (Common.CurrentUserID() == curCon.AddedBy)
                {
                    if (!UserRights.HasRights(Rights.DeleteOwnPosts))
                        return RedirectToAction("AccessDenied", "Home");
                }
                else
                {
                    if (!UserRights.HasRights(Rights.DeleteOtherUsersPosts))
                        return RedirectToAction("AccessDenied", "Home");
                }

                curCon.IsDeleted = true;
                context.SaveChanges();

                TempData["SuccessMessage"] = "Post deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Category Not Found.";
            }
            return RedirectToAction("List");
        }
    }
}
