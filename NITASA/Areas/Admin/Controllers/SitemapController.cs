using NITASA.Areas.Admin.Helper;
using NITASA.Areas.Admin.Helper.Sitemap;
using NITASA.Areas.Admin.ViewModels;
using NITASA.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace NITASA.Areas.Admin.Controllers
{
    [CheckLogin]
    public class SitemapController : Controller
    {
        public NTSDBContext context;

        public SitemapController()
        {
            this.context = new NTSDBContext();
        }
        
        public ActionResult Generate()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Generate(Sitemap model)
        {
            //string siteDomain = "http://nixon.mynitasa.com/";
            string siteDomain = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";

            var file = GetSitemapXML(siteDomain, model);

            var siteRoot = Server.MapPath("~");
            var fileName = Path.Combine(siteRoot, "Sitemap.xml");

            try
            {
                file.Save(fileName);
            }
            catch (Exception)
            {

            }

            TempData["PostMessage"] = "Sitemap generated";

            return View();
        }


        public XDocument GetSitemapXML(string siteDomain, Sitemap model)
        {
            
            var posts = context.Content.Where(c => c.Type.ToLower() == "post" && c.IsDeleted==false && c.isPublished==true).ToList();
            var pages = context.Content.Where(c => c.Type.ToLower() == "page" && c.IsDeleted == false && c.isPublished == true).ToList();
            var categories = context.Category.Where(c => c.IsDeleted==false).ToList();
            var tags = context.Label.Where(c => c.IsDeleted == false).ToList();

            var items = new List<SitemapItem>();
            var homeTag = new SitemapItem(siteDomain + "home/index", DateTime.UtcNow, NITASA.Areas.Admin.Helper.Sitemap.SitemapChangeFrequency.Daily, 1.0);
            items.Add(homeTag);

            foreach (var post in posts)
            {
                items.Add(new SitemapItem(
                (siteDomain + "content/" + post.URL),
                (post.ModifiedOn.HasValue ? post.ModifiedOn.Value : (DateTime?)null),
                (model.postFreqency.HasValue ? model.postFreqency.Value : SitemapChangeFrequency.Never), 
                (model.postPriority.HasValue?model.postPriority.Value:1)));
            }

            foreach (var page in pages)
            {
                items.Add(new SitemapItem(
                (siteDomain + "content/" + page.URL),
                (page.ModifiedOn.HasValue ? page.ModifiedOn.Value : (DateTime?)null),
                (model.postFreqency.HasValue ? model.postFreqency.Value : SitemapChangeFrequency.Monthly),
                (model.postPriority.HasValue ? model.postPriority.Value : 1)));
            }

            foreach (var cat in categories)
            {
                items.Add(new SitemapItem(
                (siteDomain + "category/" + cat.Slug),
                (cat.ModifiedOn.HasValue ? cat.ModifiedOn.Value : (DateTime?)null),
                (model.postFreqency.HasValue ? model.postFreqency.Value : SitemapChangeFrequency.Monthly),
                (model.postPriority.HasValue ? model.postPriority.Value : 1)));
            }

            foreach (var tag in tags)
            {
                items.Add(new SitemapItem(
                (siteDomain + "label/" + tag.Slug),
                (tag.ModifiedOn.HasValue ? tag.ModifiedOn.Value : (DateTime?)null),
                (model.postFreqency.HasValue ? model.postFreqency.Value : SitemapChangeFrequency.Monthly),
                (model.postPriority.HasValue ? model.postPriority.Value : 1)));
            }

            SitemapGenerator generator = new SitemapGenerator();
            var file = generator.GenerateSiteMap(items);
            return file;
        }


        //public FileStreamResult SitemapResult(XDocument xmlFile)
        //{
        //    string name = "Sitemap.xml";
        //    var str = new MemoryStream();
        //    xmlFile.Save(str);
        //    str.Flush();
        //    str.Position = 0;

        //    return File(str, "text/xml", name);
        //}
    }
}
