using NITASA.Data;
using NITASA.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Xml;
using System.IO;
using NITASA.ViewModels;

namespace NITASA.Controllers
{
    [AllAction]
    public class HomeController : AppController
    {
        public NTSDBContext context;
        public HomeController()
        {
            this.context = new NTSDBContext();
        }
        public ActionResult Index()
        {
            Content indexPage = context.Content.Where(x => x.Title.ToLower() == "index").FirstOrDefault();

            string str = HttpUtility.HtmlDecode(indexPage.Description);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.OptionFixNestedTags = false;
            doc.OptionCheckSyntax = false;
            doc.LoadHtml(str);

            var Sliders = doc.DocumentNode.SelectNodes("//slider");
            foreach (var item in Sliders)
            {
                var HTMLSlider = "";
                int sliderid = 0;
                if (Int32.TryParse(item.InnerText, out sliderid))
                {
                    List<CL_Slide> slides = context.Slides.Where(x => x.SliderId == sliderid).Select(x =>
                        new CL_Slide { Image = x.Image, Link = x.Link, Text = x.Text, Title = x.Title, DisplayOrder = x.DisplayOrder }
                        ).ToList();
                    HTMLSlider = RenderRazorViewToString(this.ControllerContext, activeTheme + "slider.cshtml", slides);
                }
                var newNode = HtmlAgilityPack.HtmlNode.CreateNode(HTMLSlider);
                item.ParentNode.ReplaceChild(newNode, item);
            }

            var Addons = doc.DocumentNode.SelectNodes("//addons");
            foreach (var item in Addons)
            {
                var HTMLAddon = "";
                int addonid = 0;
                if (Int32.TryParse(item.InnerText, out addonid))
                {
                    Content addon = context.Content.Where(x => x.ID == addonid).FirstOrDefault();
                    if (addon != null)
                    {
                        string MasterLayout = addon.AddonMasterLayout;
                        string SubLayout = addon.AddonSubLayout;
                     //   HTMLAddon = 
                    }
                }
                var newNode = HtmlAgilityPack.HtmlNode.CreateNode(HTMLAddon);
                item.ParentNode.ReplaceChild(newNode, item);
            }
            string HTMLContent = doc.DocumentNode.OuterHtml;

            return View(activeTheme + "index.cshtml", HTMLContent);

        }
        public static String RenderRazorViewToString(ControllerContext controllerContext, String viewName, Object model)
        {
            if (!string.IsNullOrEmpty(viewName))
            {
                controllerContext.Controller.ViewData.Model = model;

                using (var sw = new StringWriter())
                {
                    var ViewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                    var ViewContext = new ViewContext(controllerContext, ViewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, sw);
                    ViewResult.View.Render(ViewContext, sw);
                    ViewResult.ViewEngine.ReleaseView(controllerContext, ViewResult.View);
                    return sw.GetStringBuilder().ToString();
                }
            }
            return "";
        }
        public ActionResult NotFound()
        {
            return View();
        }
    }
}
