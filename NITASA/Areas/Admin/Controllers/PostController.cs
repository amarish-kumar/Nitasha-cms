﻿using CsQuery;
using NITASA.Areas.Admin.Helper;
using NITASA.Data;
using NITASA.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    [CheckLogin]
    public class PostController : Controller
    {   
        public NTSDBContext context;
        IAclService aclService;

        public PostController(IAclService aclService)
        {
            this.context = new NTSDBContext();
            this.aclService = aclService;
        }
        public ActionResult List(int cid = 0)
        {
            bool ViewAllPostsRights = aclService.HasRight(Rights.ViewAllPosts);
            bool ViewUnPublishedPostsRights = aclService.HasRight(Rights.ViewUnPublishedPosts);

            if (!ViewAllPostsRights && !ViewUnPublishedPostsRights)
                return RedirectToAction("AccessDenied", "Home");

            List<Category> categoryList = context.Category.Where(m => m.IsDeleted == false).ToList();
            ViewBag.categoryList = new SelectList(categoryList, "ID", "Name");

            IQueryable<Content> Content = context.Content.Include("user").Include("Categories").Include("Labels");
            if (ViewAllPostsRights)
                Content = Content.Where(m => m.Type.ToLower() == "post" && m.IsDeleted == false);
            else if (ViewUnPublishedPostsRights)
                Content = Content.Where(m => m.Type.ToLower() == "post" && m.IsDeleted == false && m.isPublished == false);

            if (cid > 0)
                Content = Content.Where(cont => cont.Categories.Where(m => m.CategoryID == cid).Count() > 0);

            List<Content> ContentList = Content.OrderByDescending(m => m.PublishedOn).ToList();

            return View(ContentList);
        }
        [HttpGet]
        public ActionResult Add()
        {
            if (!aclService.HasRight(Rights.CreateNewPosts))
                return RedirectToAction("AccessDenied", "Home");

            ViewBag.Labellist = new SelectList(context.Label.Where(x=>x.IsDeleted==false).ToList(), "ID", "Name");
            ViewBag.Categorylist = new SelectList(context.Category.Where(m => m.IsDeleted == false).ToList(), "ID", "Name");
            Content content = new Content();
            content.ContentOrder = context.Content.Where(x => x.Type.ToLower() == "post" && x.IsDeleted == false).Count() + 1;
            return View(content);
        }
        [HttpPost]
        public ActionResult Add(Content content)
        {
            if (!aclService.HasRight(Rights.CreateNewPosts))
                return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid)
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
                TempData["ErrorMessage"] = "Please solve form errors";
            }
            ViewBag.Labellist = new SelectList(context.Label.Where(x => x.IsDeleted == false).ToList(), "ID", "Name");
            ViewBag.Categorylist = new SelectList(context.Category.Where(m => m.IsDeleted == false).ToList(), "ID", "Name");

            return View(content);
        }
        public void SavePostDetails(Content Model, bool isPublished)
        {
            Content cont = new Content();
            cont.Type = "post";
            cont.Title = Model.Title;

            //var sanitizer = new Html.HtmlSanitizer();
            CQ doc = CQ.Create(Model.Description);
            string[] BlackList = new string[] { "script" };
            string selector = String.Join(",", BlackList);
            doc = doc[selector].Remove();
            Model.Description = doc.Render();

            cont.Description = Model.Description;
            cont.GUID = Functions.GetRandomGUID();
            if (string.IsNullOrEmpty(Model.URL))
                Model.URL = Model.Title;
            cont.URL = Functions.ToUrlSlug(Model.URL, "post", 0);
            cont.IsSlugEdited = Model.IsSlugEdited;
            cont.IsFeatured = Model.IsFeatured;
            cont.ContentOrder = Model.ContentOrder;

            cont.EnableComment = Model.EnableComment;
            cont.CommentEnabledTill = Model.CommentEnabledTill;
            cont.CoverContent = Model.CoverContent;
            if (!string.IsNullOrEmpty(Model.FeaturedImage))
            {
                cont.FeaturedImage = Model.FeaturedImage;
            }
            else
            {
                cont.FeaturedImage = null;
            }
            cont.AddedOn = DateTime.UtcNow;
            cont.AddedBy = Functions.CurrentUserID();
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
                    contLabel.AddedOn = DateTime.UtcNow;
                    contLabel.AddedBy = Functions.CurrentUserID();
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
                    contentCategory.AddedOn = DateTime.UtcNow;
                    contentCategory.AddedBy = Functions.CurrentUserID();
                    context.ContentCategory.Add(contentCategory);
                    context.SaveChanges();
                }
            }
            if (!string.IsNullOrWhiteSpace(Request.Form["txtMetaTitle"]) || !string.IsNullOrWhiteSpace(Request.Form["txtMetaKeyword"]) || !string.IsNullOrWhiteSpace(Request.Form["txtMetaDescription"]) || !string.IsNullOrWhiteSpace(Request.Form["txtMetaAuthor"]))
            {
                Meta metaData = new Meta();
                metaData.ContentID = cont.ID;
                metaData.Title = Request.Form["txtMetaTitle"].ToString();
                metaData.Keyword = Request.Form["txtMetaKeyword"].ToString();
                metaData.Description = Request.Form["txtMetaDescription"].ToString();
                metaData.Author = Request.Form["txtMetaAuthor"].ToString();
                metaData.CreatedOn = DateTime.UtcNow;
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
                lbl.GUID = Functions.GetRandomGUID();
                lbl.Slug = Functions.ToUrlSlug(labelName, "label", 0);
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
                cat.GUID = Functions.GetRandomGUID();
                cat.Slug = Functions.ToUrlSlug(CategoryName, "category", 0);
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
                if (Functions.CurrentUserID() == curCon.AddedBy)
                {
                    if (!aclService.HasRight(Rights.DeleteOwnPosts))
                        return RedirectToAction("AccessDenied", "Home");
                }
                else
                {
                    if (!aclService.HasRight(Rights.DeleteOtherUsersPosts))
                        return RedirectToAction("AccessDenied", "Home");
                }

                curCon.IsDeleted = true;
                context.SaveChanges();

                TempData["SuccessMessage"] = "Post deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Post Not Found.";
            }
            return RedirectToAction("List");
        }
        [HttpGet]
        public ActionResult Edit(string GUID)
        {
            Content curCont = context.Content.Where(m => m.GUID == GUID).FirstOrDefault();
            if (curCont != null)    // if content not found
            {
                if (Functions.CurrentUserID() == curCont.AddedBy)
                {
                    if (!aclService.HasRight(Rights.EditOwnPosts))
                        return RedirectToAction("AccessDenied", "Home");
                }
                else
                {
                    if (!aclService.HasRight(Rights.EditOtherUsersPosts))
                        return RedirectToAction("AccessDenied", "Home");
                }

                curCont.Description= curCont.Description.Replace("<img src=\"../../", "<img src=\"../../../");

                List<SelectListItem> Labellist = new SelectList(context.Label.Where(x => x.IsDeleted == false).ToList(), "ID", "Name").ToList();
                List<string> selectedLabel = (from dt in context.ContentLabel.Where(m => m.ContentID == curCont.ID)
                                              select dt.LabelID.ToString()).ToList();

                for (int i = 0; i < Labellist.Count(); i++)
                {
                    if (selectedLabel.Contains(Labellist[i].Value))
                    {
                        Labellist[i].Selected = true;
                    }
                }
                ViewBag.Labellist = Labellist;

                List<SelectListItem> Categorylist = new SelectList(context.Category.Where(m => m.IsDeleted == false).ToList(), "ID", "Name").ToList();
                List<string> selectedCategory = (from dt in context.ContentCategory.Where(m => m.ContentID == curCont.ID)
                                                 select dt.CategoryID.ToString()).ToList();
                for (int i = 0; i < Categorylist.Count(); i++)
                {
                    if (selectedCategory.Contains(Categorylist[i].Value))
                    {
                        Categorylist[i].Selected = true;
                    }
                }
                ViewBag.Categorylist = Categorylist;

                Meta meta  = context.Meta.FirstOrDefault(m => m.ContentID == curCont.ID);
                if (meta == null)
                    meta = new Meta();
                ViewBag.meta = meta;

                return View(curCont);
            }
            else
            {
                TempData["ErrorMessage"] = "Post Not Found.";
                return RedirectToAction("list", "Post");
            }
        }

        [HttpPost]
        public ActionResult Edit(Content Model, string GUID, string UpdateType)
        {
            Content content = context.Content.Where(m => m.GUID == GUID).FirstOrDefault();
            if (content != null)    // if content not found
            {
                if (Functions.CurrentUserID() == content.AddedBy)
                {
                    if (!aclService.HasRight(Rights.EditOwnPosts))
                        return RedirectToAction("AccessDenied", "Home");
                }
                else
                {
                    if (!aclService.HasRight(Rights.EditOtherUsersPosts))
                        return RedirectToAction("AccessDenied", "Home");
                }

                if (ModelState.IsValid)
                {
                    CQ doc = CQ.Create(Model.Description);
                    string[] BlackList = new string[] { "script" };
                    string selector = String.Join(",", BlackList);
                    doc = doc[selector].Remove();
                    Model.Description= doc.Render();

                    string postContent = Model.Description.Replace("&nbsp;", "").Replace("<p>", "").Replace("</p>", "").Trim();
                    if (!string.IsNullOrEmpty(postContent))
                    {
                        content.Title = Model.Title;
                        content.Description= Model.Description.Replace("<img src=\"../../../", "<img src=\"../../");
                        if (!string.IsNullOrEmpty(Model.URL) && content.URL.ToLower() != Model.URL.ToLower())
                            content.URL = Functions.ToUrlSlug(Model.URL, "post", content.ID);
                        content.IsSlugEdited = Model.IsSlugEdited;
                        content.IsFeatured = Model.IsFeatured;
                        content.ContentOrder = Model.ContentOrder;
                        content.ModifiedOn = DateTime.UtcNow;
                        content.ModifiedBy = Functions.CurrentUserID();

                        content.EnableComment = Model.EnableComment;
                        content.CommentEnabledTill = Model.CommentEnabledTill;
                        content.CoverContent = Model.CoverContent;
                        if (!string.IsNullOrEmpty(Model.FeaturedImage))
                        {
                            content.FeaturedImage = Model.FeaturedImage;
                        }
                        else
                        {
                            content.FeaturedImage = null;
                        }
                        if (UpdateType == "Publish")
                        {
                            content.isPublished = true;
                            content.PublishedOn = content.ModifiedOn;
                        }
                        else
                        {
                            content.isPublished = false;
                            content.PublishedOn = null;
                        }
                        context.SaveChanges();

                        #region update label
                        List<ContentLabel> contentLabel = context.ContentLabel.Where(m => m.ContentID == content.ID).ToList();
                        context.ContentLabel.RemoveRange(contentLabel);
                        context.SaveChanges();
                        if (Request.Form["LabelId"] != null)
                        {
                            string[] allLables = Request.Form["LabelId"].Split(',');
                            foreach (string currLblID in allLables)
                            {
                                ContentLabel contLabel = new ContentLabel();
                                contLabel.ContentID = content.ID;
                                contLabel.LabelID = Convert.ToInt32(currLblID);
                                contLabel.AddedOn = DateTime.UtcNow;
                                contLabel.AddedBy = Functions.CurrentUserID();
                                context.ContentLabel.Add(contLabel);
                                context.SaveChanges();
                            }
                        }
                        #endregion
                        #region update Category
                        List<ContentCategory> contentCategoryList = context.ContentCategory.Where(m => m.ContentID == content.ID).ToList();
                        context.ContentCategory.RemoveRange(contentCategoryList);
                        context.SaveChanges();
                        if (Request.Form["CategoryId"] != null)
                        {
                            string[] allCategories = Request.Form["CategoryId"].Split(',');
                            foreach (string currCatID in allCategories)
                            {
                                ContentCategory contentCategory = new ContentCategory();
                                contentCategory.ContentID = content.ID;
                                contentCategory.CategoryID = Convert.ToInt32(currCatID);
                                contentCategory.AddedOn = DateTime.UtcNow;
                                contentCategory.AddedBy = Functions.CurrentUserID();
                                context.ContentCategory.Add(contentCategory);
                                context.SaveChanges();
                            }
                        }
                        #endregion
                        #region update meta
                        Meta meta = context.Meta.FirstOrDefault(m => m.ContentID == content.ID);
                        if (meta == null)
                        {
                            meta = new Meta();
                            meta.ContentID = content.ID;
                            meta.CreatedOn = DateTime.UtcNow;
                            meta.Title = Request.Form["txtMetaTitle"].ToString();
                            meta.Keyword = Request.Form["txtMetaKeyword"].ToString();
                            meta.Description = Request.Form["txtMetaDescription"].ToString();
                            meta.Author = Request.Form["txtMetaAuthor"].ToString();
                            context.Meta.Add(meta);
                        }
                        else
                        {
                            meta.Title = Request.Form["txtMetaTitle"].ToString();
                            meta.Keyword = Request.Form["txtMetaKeyword"].ToString();
                            meta.Description = Request.Form["txtMetaDescription"].ToString();
                            meta.Author = Request.Form["txtMetaAuthor"].ToString();
                        }
                        context.SaveChanges();
                        #endregion
                        TempData["SuccessMessage"] = "Post updated successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please enter post content.";
                        return RedirectToAction("Edit");
                    }
                }
                else
                {
                    return RedirectToAction("Edit");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Post Not Found.";
            }
            return RedirectToAction("List", "Post");
        }

        [HttpPost]
        public ActionResult Preview(Content post)
        {
            post.PublishedOn = new DateTime();
            post.Type = "post";
            Session["Preview"] = null;
            Session["Preview"] = post;
            return Content("success");
        }
    }
}
