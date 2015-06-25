﻿using NITASA.Data;
using NITASA.Areas.Admin.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CsQuery;
using NITASA.Helpers;

namespace NITASA.Areas.Admin.Controllers
{
    [CheckLogin]
    public class AddonsController : Controller
    {
        public NTSDBContext context;
        public AddonsController()
        {
            this.context = new NTSDBContext();
        }

        public ActionResult List()
        {
            List<Content> ContentList = null;
            //try
            //{
            bool ViewAllAddonsRights = UserRights.HasRights(Rights.ViewAllAddons);
            bool ViewUnPublishedAddonsRights = UserRights.HasRights(Rights.ViewUnPublishedAddons);

            if (!ViewAllAddonsRights && !ViewUnPublishedAddonsRights)
                return RedirectToAction("AccessDenied", "Home");

            IQueryable<Content> Content = context.Content.Include("user");
            if (ViewAllAddonsRights)
                Content = Content.Where(m => m.Type.ToLower() != "post" && m.Type.ToLower() != "page" && m.IsDeleted == false);
            else if (ViewUnPublishedAddonsRights)
                Content = Content.Where(m => m.Type.ToLower() != "post" && m.Type.ToLower() != "page" && m.IsDeleted == false && m.isPublished == false);
            ContentList = Content.OrderByDescending(m => m.PublishedOn).ToList();
            //}
            //catch (Exception es)
            //{
            //    LogSector.LogError(es);
            //}
            return View(ContentList);
        }

        public ActionResult Add()
        {
            //try
            //{
            if (!UserRights.HasRights(Rights.CreateNewAddons))
                return RedirectToAction("AccessDenied", "Home");
            ViewBag.Addonlist = new SelectList(context.Content.Where(m => m.Type.ToLower() != "post" && m.Type.ToLower() != "page").ToList(), "ID", "Name");
            //}
            //catch (Exception es)
            //{
            //    LogSector.LogError(es);
            //}
            return View();
        }

        [HttpPost]
        public ActionResult Add(Content content)
        {
            //try
            //{
            if (!UserRights.HasRights(Rights.CreateNewAddons))
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
                            SaveAddonDetails(content, true);
                            //LogSector.LogAction(ActionType.Publish, Request.Url.ToString(), "Addon Name - " + content.Title, null);
                            TempData["SuccessMessage"] = "Addon published successfully.";
                        }
                        else
                        {
                            SaveAddonDetails(content, false);
                            //LogSector.LogAction(ActionType.Draft, Request.Url.ToString(), "Addon Name - " + content.Title, null);
                            TempData["SuccessMessage"] = "Addon saved to draft successfully.";
                        }
                        return RedirectToAction("List");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Please enter Addon content";
                }
            }
            //}
            //catch (Exception es)
            //{
            //    LogSector.LogError(es);
            //}
            return View(content);
        }

        public void SaveAddonDetails(Content Model, bool isPublished)
        {
            //try
            //{
            Content cont = new Content();
            cont.Type = Model.Type;
            cont.Title = Model.Title;

            //var sanitizer = new Html.HtmlSanitizer();
            CQ doc = CQ.Create(Model.Description);
            string[] BlackList = new string[] { "script" };
            string selector = String.Join(",", BlackList);
            doc = doc[selector].Remove();
            Model.Description = doc.Render();

            cont.Description = Model.Description;
            cont.GUID = Common.GetRandomGUID();
            cont.URL = "";
            cont.IsSlugEdited = false;
            cont.IsFeatured = false;
            cont.ContentOrder = Model.ContentOrder;
            cont.EnableComment = false;
            cont.CommentEnabledTill = 1;

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
            //}
            //catch (Exception es)
            //{
            //    LogSector.LogError(es);
            //}
        }

        public ActionResult Edit(string GUID)
        {
            Content curCont = context.Content.Where(m => m.GUID == GUID).FirstOrDefault();
            if (curCont != null)    // if content not found
            {
                //try
                //{
                if (Common.CurrentUserID() == curCont.AddedBy)
                {
                    if (!UserRights.HasRights(Rights.EditOwnAddons))
                        return RedirectToAction("AccessDenied", "Home");
                }
                else
                {
                    if (!UserRights.HasRights(Rights.EditOtherUsersAddons))
                        return RedirectToAction("AccessDenied", "Home");
                }

                curCont.Description = curCont.Description.Replace("<img src=\"../../", "<img src=\"../../../");
                //}
                //catch (Exception es)
                //{
                //    LogSector.LogError(es);
                //}
                return View(curCont);
            }
            else
            {
                TempData["ErrorMessage"] = "Addon Not Found.";
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public ActionResult Edit(Content Model, string GUID, string UpdateType)
        {
            //try
            //{
            Content content = context.Content.Where(m => m.GUID == GUID).FirstOrDefault();
            if (content != null)    // if content not found
            {
                if (Common.CurrentUserID() == content.AddedBy)
                {
                    if (!UserRights.HasRights(Rights.EditOwnAddons))
                        return RedirectToAction("AccessDenied", "Home");
                }
                else
                {
                    if (!UserRights.HasRights(Rights.EditOtherUsersAddons))
                        return RedirectToAction("AccessDenied", "Home");
                }

                if (ModelState.IsValid)
                {
                    CQ doc = CQ.Create(Model.Description);
                    string[] BlackList = new string[] { "script" };
                    string selector = String.Join(",", BlackList);
                    doc = doc[selector].Remove();
                    Model.Description = doc.Render();

                    string postContent = Model.Description.Replace("&nbsp;", "").Replace("<p>", "").Replace("</p>", "").Trim();
                    if (!string.IsNullOrEmpty(postContent))
                    {
                        content.Type = Model.Type;
                        content.Title = Model.Title;
                        content.Description = Model.Description.Replace("<img src=\"../../../", "<img src=\"../../");
                        content.ContentOrder = Model.ContentOrder;
                        content.ModifiedOn = DateTime.Now;
                        content.ModifiedBy = Common.CurrentUserID();
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

                        #region insert Update History
                        //List<Field> field = new List<Field>();
                        //if (content.Type != Model.Type)
                        //    field.Add(new Field(content.ID.ToString(), "Addon Name", content.Type.Trim(), Model.Type.Trim()));
                        //if (content.Title != Model.Title)
                        //    field.Add(new Field(content.ID.ToString(), "Addon Title", content.Title.Trim(), Model.Title.Trim()));
                        //if (content.Title != Model.Title)
                        //    field.Add(new Field(content.ID.ToString(), "Addon Title", content.Title.Trim(), Model.Title.Trim()));
                        //LogSector.LogAction(ActionType.Update, Request.Url.ToString(), "Addon", field);
                        #endregion

                        TempData["SuccessMessage"] = "Addon updated successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please enter addon content";
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
                TempData["ErrorMessage"] = "Addon Not Found.";
            }
            //}
            //catch (Exception es)
            //{
            //    LogSector.LogError(es);
            //}
            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult Delete(string GUID)
        {
            //try
            //{
            Content curCon = context.Content.Where(m => m.GUID == GUID).FirstOrDefault();
            if (curCon != null)
            {
                if (Common.CurrentUserID() == curCon.AddedBy)
                {
                    if (!UserRights.HasRights(Rights.DeleteOwnAddons))
                        return RedirectToAction("AccessDenied", "Home");
                }
                else
                {
                    if (!UserRights.HasRights(Rights.DeleteOtherUsersAddons))
                        return RedirectToAction("AccessDenied", "Home");
                }

                curCon.IsDeleted = true;
                context.SaveChanges();
                //LogSector.LogAction(ActionType.Delete, Request.Url.ToString(), "Addon Name - " + curCon.Title, null);
                TempData["SuccessMessage"] = "Addon deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Addon Not Found.";
            }
            //}
            //catch (Exception es)
            //{
            //    LogSector.LogError(es);
            //}
            return RedirectToAction("List");
        }


        [HttpGet]
        public JsonResult GetAddonNames(string term)
        {
            List<string> Attributes = context.Content.Where(m => m.Type.ToLower() != "post" && m.Type.ToLower() != "page" && m.Type.ToLower().Contains(term)).Select(m => m.Type).Distinct().ToList();
            return Json(Attributes, JsonRequestBehavior.AllowGet);
        }
    }
}