using NITASA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NITASA.Areas.Admin.Helper;
using NITASA.Areas.Admin.ViewModels;
using NITASA.Helpers;

namespace NITASA.Areas.Admin.Controllers
{
    [CheckLogin]
    public class WidgetController : Controller
    {
        public NTSDBContext context;
        public WidgetController()
        {
            context = new NTSDBContext();
        }

        public ActionResult List()
        {
            List<Widget> widgets = context.Widget.OrderByDescending(m => m.IsActive).ThenBy(m => m.WidgetOrder).ToList();
            return View(widgets);
        }

        //public ActionResult Active(string id)
        //{
        //    if (!UserRights.HasRights(Rights.ManageWidgets))
        //        return RedirectToAction("AccessDenied", "Home");

        //    Widget widgetToActivate = context.Widget.Where(m => m.WidgetGUID == id).FirstOrDefault();
        //    if (widgetToActivate == null)
        //    {
        //        return RedirectToAction("NotFound", "Home");
        //    }
        //    else
        //    {
        //        widgetToActivate.IsActive = true;
        //        context.SaveChanges();
        //        TempData["Message"] = "Widget activated successfully.";
        //    }
        //    return RedirectToAction("List");
        //}

        //public ActionResult Deactive(string id)
        //{
        //    if (!UserRights.HasRights(Rights.ManageWidgets))
        //        return RedirectToAction("AccessDenied", "Home");

        //    Widget widgetToActivate = context.Widget.Where(m => m.WidgetGUID == id).FirstOrDefault();
        //    if (widgetToActivate == null)
        //    {
        //        return RedirectToAction("NotFound", "Home");
        //    }
        //    else
        //    {
        //        widgetToActivate.IsActive = false;
        //        context.SaveChanges();
        //        TempData["Message"] = "Widget deactivated successfully.";
        //    }
        //    return RedirectToAction("List");
        //}

        [HttpGet]
        public ActionResult changeStatus(string GUID, bool isActive)
        {
            Widget widget = context.Widget.Where(m => m.WidgetGUID == GUID).FirstOrDefault();
            if (widget != null)
            {
                if (!UserRights.HasRights(Rights.ManageWidgets))
                    return RedirectToAction("AccessDenied", "Home");

                widget.IsActive = isActive;
                context.SaveChanges();

                TempData["SuccessMessage"] = "Widget status updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Widget Not Found.";
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult Update(WidgetOption wOption)
        {
            if (!UserRights.HasRights(Rights.ManageWidgets))
                return RedirectToAction("AccessDenied", "Home");

            if (wOption.WidgetID != null)
            {
                int widgetID = Convert.ToInt32(wOption.WidgetID);
                Widget widgetToUpdate = context.Widget.Where(m => m.WidgetID == widgetID).FirstOrDefault();
                if (widgetToUpdate == null)
                {
                    return RedirectToAction("NotFound", "Home");
                }
                else
                {
                    string widgetOption = "{ \"title\": \"" + wOption.Title + "\",\"count\": \"" + wOption.Count + "\",\"showthumb\": \"" + wOption.ShowThumbnail.ToString().ToLower() + "\" }";
                    widgetToUpdate.WidgetOrder = wOption.WidgetOrder;
                    widgetToUpdate.WidgetTitle = wOption.Title;
                    widgetToUpdate.WidgetOption = widgetOption;
                    context.SaveChanges();
                    TempData["Message"] = "Widget configuration setup successfully.";
                }
                return RedirectToAction("List");
            }
            else
            {
                TempData["Error"] = "Error occured while updating widget configuration.";
                return RedirectToAction("List");
            }
        }
    }
}