﻿using CsQuery;
using NITASA.Areas.Admin.Helper;
using NITASA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    [CheckLogin]
    public class PageController : Controller
    {
        private NTSDBContext dbContext;
        public PageController()
        {
            this.dbContext = new NTSDBContext();
        }

        public ActionResult List()
        {
            List<Content> pages;
            if (UserRights.HasRights(Rights.ViewAllPages))
                pages = dbContext.Content
                    .Include("user")
                    .Where(m => m.Type == "page" && m.IsDeleted == false)
                    .OrderByDescending(m => m.AddedOn)
                    .ToList();
            else if (UserRights.HasRights(Rights.ViewUnPublishedPages))
                pages = dbContext.Content
                    .Include("user")
                    .Where(m => m.Type == "page" && m.IsDeleted == false && m.isPublished == false)
                    .OrderByDescending(m => m.AddedOn)
                    .ToList();
            else
                return RedirectToAction("AccessDenied", "Home");

            return View(pages);
        }

        [HttpGet]
        public ActionResult Add(string guid = "")
        {
            if (guid.Trim() == string.Empty)
            {
                if (!UserRights.HasRights(Rights.CreateNewPages))
                    return RedirectToAction("AccessDenied", "Home");
                return View();
            }
            else
            {
                Content cont = dbContext.Content.Where(m => m.Type == "page" && m.GUID == guid).FirstOrDefault();
                if (cont != null)
                {
                    if (cont.AddedBy == Functions.CurrentUserID())
                    {
                        if (!UserRights.HasRights(Rights.EditOwnPages))
                            return RedirectToAction("AccessDenied", "Home");
                    }
                    else
                    {
                        if (!UserRights.HasRights(Rights.EditOtherUsersPage))
                            return RedirectToAction("AccessDenied", "Home");
                    }

                    PageModel pModel = new PageModel();
                    cont.Description= cont.Description.Replace("<img src=\"../../", "<img src=\"../../../");
                    pModel.content = cont;
                    pModel.meta = dbContext.Meta.Where(m => m.ContentID == cont.ID).FirstOrDefault();
                    if (cont.Title.ToLower() == "index")
                    {
                        ViewBag.isIndexpage = true;
                    }
                    return View(pModel);
                }
                else
                {
                    TempData["ErrorMessage"] = "Page Not Found.";
                    return RedirectToAction("List");
                }
            }
        }

        [HttpPost]
        public ActionResult Add(PageModel contentPage, string SaveType)
        {
            if (ModelState.IsValid)
            {
                if (contentPage.content.Title.ToLower() == "index") // add new page
                {
                    var index = dbContext.Content.FirstOrDefault(x => x.Title.ToLower() == "index");
                    if (index != null && (contentPage.content.ID != index.ID))
                    {
                        TempData["ErrorMessage"] = "Index page already added, please add diffrent page.";
                        return View(contentPage);
                    }
                    else
                    { ViewBag.isIndexpage = true; }
                }
                string pageContent = contentPage.content.Description.Replace("&nbsp;", "").Replace("<p>", "").Replace("</p>", "").Trim();
                if (pageContent != string.Empty)
                {
                    if (SaveType == "Publish")
                        contentPage.content.isPublished = true;
                    else
                        contentPage.content.isPublished = false;

                    if (contentPage.content.ID == 0) // Add
                    {
                        AddUpdateContentWithMeta(contentPage, 'A');
                    }
                    else //  Update
                    {
                        AddUpdateContentWithMeta(contentPage, 'U');
                    }
                    if (contentPage.content.isPublished)
                        TempData["SuccessMessage"] = "Page published successfully.";
                    else
                        TempData["SuccessMessage"] = "Page saved to draft successfully.";
                    return RedirectToAction("List");
                }
                else
                {
                    TempData["ErrorMessage"] = "Please enter page description";
                }
            }
            return View(contentPage);
        }

        public void AddUpdateContentWithMeta(PageModel contentPage, char operationType)
        {
            CQ doc = CQ.Create(contentPage.content.Description);
            string[] BlackList = new string[] { "script" };
            string selector = String.Join(",", BlackList);
            doc = doc[selector].Remove();
            contentPage.content.Description = doc.Render();

            if (operationType == 'A')
            {
                Content contentNew = new Content();
                contentNew.GUID = Functions.GetRandomGUID();
                //var sanitizer = new Html.HtmlSanitizer();
                //contentNew.ContentText = sanitizer.Sanitize(contentPage.content.ContentText);
                contentNew.Description= contentPage.content.Description;
                contentNew.Title = contentPage.content.Title;
                contentNew.Type = "page";
                contentNew.URL = Functions.ToUrlSlug(contentPage.content.URL, "page", 0);
                contentNew.CoverContent = contentPage.content.CoverContent;
                if (!string.IsNullOrEmpty(contentPage.content.FeaturedImage))
                {
                    contentNew.FeaturedImage = contentPage.content.FeaturedImage;
                }
                else
                {
                    contentNew.FeaturedImage = null;
                }
                if (contentPage.content.Title.ToLower() == "index")
                {
                    contentNew.ContentPosition = contentPage.content.ContentPosition;
                }
                else
                {
                    contentNew.ContentPosition = null;
                }
                contentNew.AddedBy = Functions.CurrentUserID();
                contentNew.AddedOn = DateTime.Now;
                contentNew.isPublished = contentPage.content.isPublished;
                if (contentPage.content.isPublished)
                    contentNew.PublishedOn = DateTime.Now;
                dbContext.Content.Add(contentNew);
                dbContext.SaveChanges();

                //Page meta data add
                if (!string.IsNullOrWhiteSpace(contentPage.meta.Keyword) || !string.IsNullOrWhiteSpace(contentPage.meta.Description) || !string.IsNullOrWhiteSpace(contentPage.meta.Author))
                {
                    Meta metaData = new Meta();
                    metaData.ContentID = contentNew.ID;
                    metaData.Keyword = contentPage.meta.Keyword;
                    metaData.Description = contentPage.meta.Description;
                    metaData.Author = contentPage.meta.Author;
                    metaData.CreatedOn = DateTime.Now;
                    dbContext.Meta.Add(metaData);
                    dbContext.SaveChanges();
                }
            }
            else
            {
                Content contentUpdate = dbContext.Content.Where(m => m.Type == "page" && m.GUID == contentPage.content.GUID).FirstOrDefault();
                //var sanitizer = new Html.HtmlSanitizer();
                contentUpdate.Description= contentPage.content.Description.Replace("<img src=\"../../../", "<img src=\"../../");
                contentUpdate.Title = contentPage.content.Title;
                if (contentUpdate.URL.ToLower() != contentPage.content.URL.ToLower())
                    contentUpdate.URL = Functions.ToUrlSlug(contentPage.content.URL, "Page", contentUpdate.ID);
                contentUpdate.CoverContent = contentPage.content.CoverContent;
                if (!string.IsNullOrEmpty(contentPage.content.FeaturedImage))
                {
                    contentUpdate.FeaturedImage = contentPage.content.FeaturedImage;
                }
                else
                {
                    contentUpdate.FeaturedImage = null;
                }
                if (contentPage.content.Title.ToLower() == "index")
                {
                    contentUpdate.ContentPosition = contentPage.content.ContentPosition;
                }
                contentUpdate.ModifiedBy = Functions.CurrentUserID();
                contentUpdate.ModifiedOn = DateTime.Now;
                contentUpdate.isPublished = contentPage.content.isPublished;
                if (contentPage.content.isPublished)
                    contentUpdate.PublishedOn = DateTime.Now;
                dbContext.SaveChanges();


                if (dbContext.Meta.Where(m => m.ContentID == contentUpdate.ID).Count() > 0)
                {
                    Meta meta = dbContext.Meta.Where(m => m.ContentID == contentUpdate.ID).SingleOrDefault();
                    meta.Keyword = contentPage.meta.Keyword;
                    meta.Description = contentPage.meta.Description;
                    meta.Author = contentPage.meta.Author;
                    dbContext.SaveChanges();
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(contentPage.meta.Keyword) || !string.IsNullOrWhiteSpace(contentPage.meta.Description) || !string.IsNullOrWhiteSpace(contentPage.meta.Author))
                    {
                        Meta metaData = new Meta();
                        metaData.ContentID = contentUpdate.ID;
                        metaData.Keyword = contentPage.meta.Keyword;
                        metaData.Description = contentPage.meta.Description;
                        metaData.Author = contentPage.meta.Author;
                        metaData.CreatedOn = DateTime.Now;
                        dbContext.Meta.Add(metaData);
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        public ActionResult Delete(string guid)
        {
            Content contentToDelete = dbContext.Content.Where(m => m.Type == "page" && m.GUID == guid).Single();
            if (contentToDelete != null && contentToDelete.Title.ToLower() != "index")
            {
                if (contentToDelete.AddedBy == Functions.CurrentUserID())
                {
                    if (!UserRights.HasRights(Rights.DeleteOwnPages))
                        return RedirectToAction("AccessDenied", "Home");
                }
                else
                {
                    if (!UserRights.HasRights(Rights.DeleteOtherUsersPages))
                        return RedirectToAction("AccessDenied", "Home");
                }
                contentToDelete.IsDeleted = true;
                dbContext.SaveChanges();
                TempData["SuccessMessage"] = "Page deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Page Not Found.";
            }
            return RedirectToAction("List");
        }
    }
}
