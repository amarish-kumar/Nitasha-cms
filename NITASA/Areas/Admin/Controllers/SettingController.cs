using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NITASA.Areas.Admin.Helper;
using NITASA.Data;
using System.IO;
using NITASA.Areas.Admin.ViewModels;

namespace NITASA.Areas.Admin.Controllers
{
    [CheckLogin]
    public class SettingController : Controller
    {
        public NTSDBContext context;
        public SettingController()
        {
            context = new NTSDBContext();
        }

        public ActionResult Index()
        {
            if (!UserRights.HasRights(Rights.ManageSettings))
                return RedirectToAction("AccessDenied", "Home");

            SiteSettings siteSettings = new SiteSettings();
            siteSettings.LogoPath = context.Settings.Where(m => m.Name == "LogoPath").Select(m => m.Value).FirstOrDefault();
            siteSettings.FaviconPath = context.Settings.Where(m => m.Name == "FaviconPath").Select(m => m.Value).FirstOrDefault();
            siteSettings.SiteName = context.Settings.Where(m => m.Name == "SiteName").Select(m => m.Value).FirstOrDefault();
            siteSettings.SiteTitle = context.Settings.Where(m => m.Name == "SiteTitle").Select(m => m.Value).FirstOrDefault();
            siteSettings.ShowNoOfLastPostsAtHome = context.Settings.Where(m => m.Name == "ShowNoOfLastPostsAtHome").Select(m => m.Value).FirstOrDefault();
            siteSettings.FacebookURL = context.Settings.Where(m => m.Name == "FacebookURL").Select(m => m.Value).FirstOrDefault();
            siteSettings.TwitterURL = context.Settings.Where(m => m.Name == "TwitterURL").Select(m => m.Value).FirstOrDefault();
            siteSettings.LinkedInURL = context.Settings.Where(m => m.Name == "LinkedInURL").Select(m => m.Value).FirstOrDefault();
            siteSettings.GooglePlusURL = context.Settings.Where(m => m.Name == "GooglePlusURL").Select(m => m.Value).FirstOrDefault();
            siteSettings.PinterestURL = context.Settings.Where(m => m.Name == "PinterestURL").Select(m => m.Value).FirstOrDefault();
            siteSettings.CustomHeadTag = context.Settings.Where(m => m.Name == "CustomHeadTag").Select(m => m.Value).FirstOrDefault();
            siteSettings.DefaultMetaTags = context.Settings.Where(m => m.Name == "DefaultMetaTags").Select(m => m.Value).FirstOrDefault();
            siteSettings.CustomJavaScript = context.Settings.Where(m => m.Name == "CustomJavaScript").Select(m => m.Value).FirstOrDefault();
            siteSettings.CustomCSS = context.Settings.Where(m => m.Name == "CustomCSS").Select(m => m.Value).FirstOrDefault();
            siteSettings.GoogleAnalytics = context.Settings.Where(m => m.Name == "GoogleAnalytics").Select(m => m.Value).FirstOrDefault();
            siteSettings.CurrentTheme = context.Settings.Where(m => m.Name == "CurrentTheme").Select(m => m.Value).FirstOrDefault();
            Setting ListingPostsPageSize = context.Settings.Where(m => m.Name == "ListingPostsPageSize").FirstOrDefault();
            if (ListingPostsPageSize != null)
                siteSettings.ListingPostsPageSize = Convert.ToInt32(ListingPostsPageSize.Value);
            else
                siteSettings.ListingPostsPageSize = 0;

            List<string> ThemeList = new DirectoryInfo(Server.MapPath("/Views/themes/")).GetDirectories().Select(m => Path.GetFileNameWithoutExtension(m.FullName)).ToList();
            ViewBag.Themes = new SelectList(ThemeList);
            return View(siteSettings);
        }

        [HttpPost]
        public ActionResult Index(SiteSettings con, HttpPostedFileBase logopath, HttpPostedFileBase FaviconIcon, HttpPostedFileBase CoverImage)
        {
            if (!UserRights.HasRights(Rights.ManageSettings))
                return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid)
            {
                bool validImage = true;
                if (logopath != null && logopath.ContentLength > 0)
                {
                    if (!Functions.IsValidImage(logopath.FileName.ToLower()))
                    {
                        TempData["ErrorMessage"] = "Please upload valid logo.";
                        validImage = false;
                    }
                }
                if (FaviconIcon != null && FaviconIcon.ContentLength > 0)
                {
                    if (Path.GetExtension(FaviconIcon.FileName.ToLower()) != ".ico")
                    {
                        TempData["ErrorMessage"] = "Please upload valid Favicon Icon.";
                        validImage = false;
                    }
                }
                if (CoverImage != null && CoverImage.ContentLength > 0)
                {
                    if (!Functions.IsValidImage(CoverImage.FileName.ToLower()))
                    {
                        TempData["ErrorMessage"] = "Please upload valid cover image.";
                        validImage = false;
                    }
                }
                if (validImage)
                {
                    try
                    {
                        if (logopath != null && logopath.ContentLength > 0)
                        {
                            string logoFullPath = Functions.GetNewFileName("/content/", logopath.FileName);
                            logopath.SaveAs(Server.MapPath(logoFullPath));

                            Setting LogoPath = context.Settings.FirstOrDefault(m => m.Name == "LogoPath");
                            if (LogoPath == null)
                            {
                                LogoPath = new Setting() { Name = "LogoPath", Value = logoFullPath };
                                context.Settings.Add(LogoPath);
                            }
                            else LogoPath.Value = logoFullPath;
                        }
                        if (FaviconIcon != null && FaviconIcon.ContentLength > 0)
                        {
                            string FaviconIconFullPath = Functions.GetNewFileName("/content/", FaviconIcon.FileName);
                            FaviconIcon.SaveAs(Server.MapPath(FaviconIconFullPath));
                            Setting FaviconPath = context.Settings.FirstOrDefault(m => m.Name == "FaviconPath");
                            if (FaviconPath == null)
                            {
                                FaviconPath = new Setting() { Name = "FaviconPath", Value = FaviconIconFullPath };
                                context.Settings.Add(FaviconPath);
                            }
                            else FaviconPath.Value = FaviconIconFullPath;
                        }

                        Setting SiteName = context.Settings.FirstOrDefault(m => m.Name == "SiteName");
                        if (SiteName == null)
                        {
                            SiteName = new Setting() { Name = "SiteName", Value = con.SiteName };
                            context.Settings.Add(SiteName);
                        }
                        else SiteName.Value = con.SiteName;

                        Setting SiteTitle = context.Settings.FirstOrDefault(m => m.Name == "SiteTitle");
                        if (SiteTitle == null)
                        {
                            SiteTitle = new Setting() { Name = "SiteTitle", Value = con.SiteTitle };
                            context.Settings.Add(SiteTitle);
                        }
                        else SiteTitle.Value = con.SiteTitle;

                        Setting ShowNoOfLastPostsAtHome = context.Settings.FirstOrDefault(m => m.Name == "ShowNoOfLastPostsAtHome");
                        if (ShowNoOfLastPostsAtHome == null)
                        {
                            ShowNoOfLastPostsAtHome = new Setting() { Name = "ShowNoOfLastPostsAtHome", Value = con.ShowNoOfLastPostsAtHome };
                            context.Settings.Add(ShowNoOfLastPostsAtHome);
                        }
                        else ShowNoOfLastPostsAtHome.Value = con.ShowNoOfLastPostsAtHome;

                        Setting ListingPostsPageSize= context.Settings.FirstOrDefault(m => m.Name == "ListingPostsPageSize");
                        if (ListingPostsPageSize == null)
                        {
                            ListingPostsPageSize = new Setting() { Name = "ListingPostsPageSize", Value = con.ListingPostsPageSize.ToString() };
                            context.Settings.Add(ListingPostsPageSize);
                        }
                        else ListingPostsPageSize.Value = con.ListingPostsPageSize.ToString();

                        Setting FacebookURL = context.Settings.FirstOrDefault(m => m.Name == "FacebookURL");
                        if (FacebookURL == null)
                        {
                            FacebookURL = new Setting() { Name = "FacebookURL", Value = con.FacebookURL };
                            context.Settings.Add(FacebookURL);
                        }
                        else FacebookURL.Value = con.FacebookURL;

                        Setting TwitterURL = context.Settings.FirstOrDefault(m => m.Name == "TwitterURL");
                        if (TwitterURL == null)
                        {
                            TwitterURL = new Setting() { Name = "TwitterURL", Value = con.TwitterURL };
                            context.Settings.Add(TwitterURL);
                        }
                        else TwitterURL.Value = con.TwitterURL;

                        Setting GooglePlusURL = context.Settings.FirstOrDefault(m => m.Name == "GooglePlusURL");
                        if (GooglePlusURL == null)
                        {
                            GooglePlusURL = new Setting() { Name = "GooglePlusURL", Value = con.GooglePlusURL };
                            context.Settings.Add(GooglePlusURL);
                        }
                        else GooglePlusURL.Value = con.GooglePlusURL;

                        Setting linkedInUrl = context.Settings.FirstOrDefault(m => m.Name == "LinkedInURL");
                        if (linkedInUrl == null)
                        {
                            linkedInUrl = new Setting() { Name = "LinkedInURL", Value = con.LinkedInURL };
                            context.Settings.Add(linkedInUrl);
                        }
                        else linkedInUrl.Value = con.LinkedInURL;

                        Setting PinterestURL = context.Settings.FirstOrDefault(m => m.Name == "PinterestURL");
                        if (PinterestURL == null)
                        {
                            PinterestURL = new Setting() { Name = "PinterestURL", Value = con.PinterestURL };
                            context.Settings.Add(PinterestURL);
                        }
                        else PinterestURL.Value = con.PinterestURL;

                        Setting CustomHeadTag = context.Settings.FirstOrDefault(m => m.Name == "CustomHeadTag");
                        if (CustomHeadTag == null)
                        {
                            CustomHeadTag = new Setting() { Name = "CustomHeadTag", Value = con.CustomHeadTag };
                            context.Settings.Add(CustomHeadTag);
                        }
                        else CustomHeadTag.Value = con.CustomHeadTag;

                        Setting DefaultMetaTags = context.Settings.FirstOrDefault(m => m.Name == "DefaultMetaTags");
                        if (DefaultMetaTags == null)
                        {
                            DefaultMetaTags = new Setting() { Name = "DefaultMetaTags", Value = con.DefaultMetaTags };
                            context.Settings.Add(DefaultMetaTags);
                        }
                        else DefaultMetaTags.Value = con.DefaultMetaTags;

                        Setting CustomJavaScript = context.Settings.FirstOrDefault(m => m.Name == "CustomJavaScript");
                        if (CustomJavaScript == null)
                        {
                            CustomJavaScript = new Setting() { Name = "CustomJavaScript", Value = con.CustomJavaScript };
                            context.Settings.Add(CustomJavaScript);
                        }
                        else CustomJavaScript.Value = con.CustomJavaScript;

                        Setting CustomCSS = context.Settings.FirstOrDefault(m => m.Name == "CustomCSS");
                        if (CustomCSS == null)
                        {
                            CustomCSS = new Setting() { Name = "CustomCSS", Value = con.CustomCSS };
                            context.Settings.Add(CustomCSS);
                        }
                        else CustomCSS.Value = con.CustomCSS;

                        Setting GoogleAnalytics = context.Settings.FirstOrDefault(m => m.Name == "GoogleAnalytics");
                        if (GoogleAnalytics == null)
                        {
                            GoogleAnalytics = new Setting() { Name = "GoogleAnalytics", Value = con.GoogleAnalytics };
                            context.Settings.Add(GoogleAnalytics);
                        }
                        else GoogleAnalytics.Value = con.GoogleAnalytics;

                        Setting CurrentTheme = context.Settings.FirstOrDefault(m => m.Name == "CurrentTheme");
                        if (CurrentTheme == null)
                        {
                            CurrentTheme = new Setting() { Name = "CurrentTheme", Value = con.CurrentTheme };
                            context.Settings.Add(CurrentTheme);
                        }
                        else CurrentTheme.Value = con.CurrentTheme;
                        Request.RequestContext.HttpContext.Application["CurrentTheme"] = con.CurrentTheme;
                        context.SaveChanges();
                        TempData["SuccessMessage"] = "Settings updated successfully.";
                        //function.ReBindConfig();
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "Problem occurred during operation. " + ex.Message;
                    }
                }
            }
            return RedirectToAction("Index");
        }

    }
}
