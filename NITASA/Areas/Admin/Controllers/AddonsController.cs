using NITASA.Data;
using NITASA.Areas.Admin.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    public class AddonsController : Controller
    {
        public NTSDBContext context;
        public AddonsController()
        {
            this.context = new NTSDBContext();
        }

        public ActionResult List()
        {          
            bool ViewAllAddonsRights = UserRights.HasRights(Rights.ViewAllAddons);
            bool ViewUnPublishedAddonsRights = UserRights.HasRights(Rights.ViewUnPublishedAddons);

            if (!ViewAllAddonsRights && !ViewUnPublishedAddonsRights)
                return RedirectToAction("AccessDenied", "Home");

            //List<Category> categoryList = context.Category.Where(m => m.IsDeleted == false).ToList();
            //ViewBag.categoryList = new SelectList(categoryList, "CategoryID", "CategoryName");

            IQueryable<Content> Content = context.Content.Include("user");
            if (ViewAllAddonsRights)
                Content = Content.Where(m => m.Type.ToLower() != "post" && m.Type.ToLower() != "page" && m.IsDeleted == false);
            else if (ViewUnPublishedAddonsRights)
                Content = Content.Where(m => m.Type.ToLower() == "post" && m.Type.ToLower() != "page" && m.IsDeleted == false && m.isPublished == false);

            //if (cid > 0)
            //    Content = Content.Where(cont => cont.contentCategory.Where(m => m.CategoryID == cid).Count() > 0);

            List<Content> ContentList = Content.OrderByDescending(m => m.PublishedOn).ToList();

            return View(ContentList);
        }
        public ActionResult Add()
        {
            ActivityOperation.InsertLog(LogOperation.Add, "New Addon Added", "New Addon added with name='Services', title='Our Services'", Request.Url.ToString());
            return View();
        }

        [HttpPost]
        public ActionResult Add(Content model) 
        {
            return View();
        }

        [HttpGet]
        public ActionResult Delete(string GUID)
        {
            return RedirectToAction("List");
        }
	}
}