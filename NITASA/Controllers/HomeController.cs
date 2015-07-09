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
using System.Data.Entity;

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

            indexPage.ContentView = indexPage.ContentView + 1;
            context.Entry(indexPage).State = EntityState.Modified;
            context.SaveChanges();
            Functions.IncreaseContentView(indexPage.ID, Request);

            ViewBag.HomePageContentPosition = indexPage.ContentPosition;

            string HTMLContent = Functions.ReplaceSliderAndAddons(this.ControllerContext, activeTheme, indexPage.Description);

            return View(viewName: activeTheme + "index.cshtml", model: HTMLContent);
        }

        public ActionResult NotFound404()
        {
            return View(viewName: activeTheme + "404.cshtml");
        }
        public ActionResult NotFound500()
        {
            return View(viewName: activeTheme + "500.cshtml");
        }
    }
}
