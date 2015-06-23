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
                return RedirectToAction("AccessDenied", "Dashboard");

            return View(pages);
        }
/*
        [HttpGet]
        public ActionResult Add(string guid = "")
        {
            if (guid.Trim() == string.Empty)
            {
                if (!UserRights.HasRights(Rights.CreateNewPages))
                    return RedirectToAction("AccessDenied", "Dashboard");
                ViewBag.Templatelist = new SelectList(dbContext.ContentTemplate.Where(m => m.IsDeleted == false).ToList(), "TemplateID", "TemplateName");
                return View();
            }
            else
            {
                Content cont = dbContext.Content.Where(m => m.ContentType == "page" && m.ContentGUID == guid).FirstOrDefault();
                if (cont != null)
                {
                    if (cont.CreatedBy == Common.CurrentUserID())
                    {
                        if (!UserRights.HasRights(Rights.EditOwnPages))
                            return RedirectToAction("AccessDenied", "Dashboard");
                    }
                    else
                    {
                        if (!UserRights.HasRights(Rights.EditOtherUsersPage))
                            return RedirectToAction("AccessDenied", "Dashboard");
                    }

                    PageModel pModel = new PageModel();
                    cont.ContentText = cont.ContentText.Replace("<img src=\"../../", "<img src=\"../../../");
                    pModel.content = cont;
                    pModel.meta = dbContext.Meta.Where(m => m.ContentID == cont.ContentID).FirstOrDefault();
                    ViewBag.Templatelist = new SelectList(dbContext.ContentTemplate.Where(m => m.IsDeleted == false).ToList(), "TemplateID", "TemplateName", cont.TemplateID);
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
                string pageContent = contentPage.content.ContentText.Replace("&nbsp;", "").Replace("<p>", "").Replace("</p>", "").Trim();
                if (pageContent != string.Empty)
                {
                    if (SaveType == "Publish")
                        contentPage.content.isPublished = true;
                    else
                        contentPage.content.isPublished = false;

                    if (Request.Form["content.ContentID"] == null) // Add
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
                }
                else
                {
                    TempData["ErrorMessage"] = "Please enter page description";
                    return RedirectToAction("Add");
                }
            }
            return RedirectToAction("List");
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
                contentNew.GUID = Common.GetRandomGUID();
                //var sanitizer = new Html.HtmlSanitizer();
                //contentNew.ContentText = sanitizer.Sanitize(contentPage.content.ContentText);
                contentNew.Description= contentPage.content.Description;
                contentNew.Title = contentPage.content.Title;
                contentNew.Type = "page";
                contentNew.URL = Common.ToUrlSlug(contentPage.content.URL, "Page", 0);
                //contentNew.TemplateID = contentPage.content.TemplateID;
                contentNew.ContentHeaderText = contentPage.content.HeaderText;
                if (!string.IsNullOrEmpty(contentPage.content.ContentHeaderImage))
                {
                    contentNew.ContentHeaderImage = contentPage.content.ContentHeaderImage;
                }
                else
                {
                    contentNew.ContentHeaderImage = null;
                }
                contentNew.CreatedBy = Common.CurrentUserID();
                contentNew.CreatedOn = DateTime.Now;
                contentNew.isPublished = contentPage.content.isPublished;
                if (contentPage.content.isPublished)
                    contentNew.PublishedOn = DateTime.Now;
                dbContext.Content.Add(contentNew);
                dbContext.SaveChanges();

                //Page meta data add
                if (!string.IsNullOrWhiteSpace(contentPage.meta.MetaKeyword) || !string.IsNullOrWhiteSpace(contentPage.meta.MetaDescription) || !string.IsNullOrWhiteSpace(contentPage.meta.MetaAuthor))
                {
                    Meta metaData = new Meta();
                    metaData.ContentID = contentNew.ContentID;
                    metaData.MetaKeyword = contentPage.meta.MetaKeyword;
                    metaData.MetaDescription = contentPage.meta.MetaDescription;
                    metaData.MetaAuthor = contentPage.meta.MetaAuthor;
                    metaData.CreatedOn = DateTime.Now;
                    dbContext.Meta.Add(metaData);
                    dbContext.SaveChanges();
                }
            }
            else
            {
                Content contentUpdate = dbContext.Content.Where(m => m.ContentType == "page" && m.ContentGUID == contentPage.content.ContentGUID).FirstOrDefault();
                //var sanitizer = new Html.HtmlSanitizer();
                contentUpdate.ContentText = contentPage.content.ContentText.Replace("<img src=\"../../../", "<img src=\"../../");
                contentUpdate.ContentTitle = contentPage.content.ContentTitle;
                if (contentUpdate.ContentURL.ToLower() != contentPage.content.ContentURL.ToLower())
                    contentUpdate.ContentURL = Common.ToUrlSlug(contentPage.content.ContentURL, "Page", contentUpdate.ContentID);
                contentUpdate.TemplateID = contentPage.content.TemplateID;
                contentUpdate.ContentHeaderText = contentPage.content.ContentHeaderText;
                if (!string.IsNullOrEmpty(contentPage.content.ContentHeaderImage))
                {
                    contentUpdate.ContentHeaderImage = contentPage.content.ContentHeaderImage;
                }
                else
                {
                    contentUpdate.ContentHeaderImage = null;
                }
                contentUpdate.ModifiedBy = Common.CurrentUserID();
                contentUpdate.ModifiedOn = DateTime.Now;
                contentUpdate.isPublished = contentPage.content.isPublished;
                if (contentPage.content.isPublished)
                    contentUpdate.PublishedOn = DateTime.Now;
                dbContext.SaveChanges();


                if (dbContext.Meta.Where(m => m.ContentID == contentUpdate.ContentID).Count() > 0)
                {
                    Meta meta = dbContext.Meta.Where(m => m.ContentID == contentUpdate.ContentID).SingleOrDefault();
                    meta.MetaKeyword = contentPage.meta.MetaKeyword;
                    meta.MetaDescription = contentPage.meta.MetaDescription;
                    meta.MetaAuthor = contentPage.meta.MetaAuthor;
                    dbContext.SaveChanges();
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(contentPage.meta.MetaKeyword) || !string.IsNullOrWhiteSpace(contentPage.meta.MetaDescription) || !string.IsNullOrWhiteSpace(contentPage.meta.MetaAuthor))
                    {
                        Meta metaData = new Meta();
                        metaData.ContentID = contentUpdate.ContentID;
                        metaData.MetaKeyword = contentPage.meta.MetaKeyword;
                        metaData.MetaDescription = contentPage.meta.MetaDescription;
                        metaData.MetaAuthor = contentPage.meta.MetaAuthor;
                        metaData.CreatedOn = DateTime.Now;
                        dbContext.Meta.Add(metaData);
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        public ActionResult Delete(string guid)
        {
            Content contentToDelete = dbContext.Content.Where(m => m.ContentType == "page" && m.ContentGUID == guid).Single();
            if (contentToDelete != null)
            {
                if (contentToDelete.CreatedBy == Common.CurrentUserID())
                {
                    if (!UserRights.HasRights(Rights.DeleteOwnPages))
                        return RedirectToAction("AccessDenied", "Dashboard");
                }
                else
                {
                    if (!UserRights.HasRights(Rights.DeleteOtherUsersPages))
                        return RedirectToAction("AccessDenied", "Dashboard");
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
        }*/
    }
}
