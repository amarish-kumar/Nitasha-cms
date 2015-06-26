using NITASA.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    public class ThemeController : Controller
    {
        public NTSDBContext context;
        public ThemeController()
        {
            context = new NTSDBContext();
        }
        List<string> NotListedDirectory = new List<string> { "configure", "register", "shared", "default" };
        public ActionResult List()
        {
            List<string> ThemeList = new DirectoryInfo(Server.MapPath("/Views/")).GetDirectories().Where(x => !NotListedDirectory.Contains(x.Name.ToLower()))
                                    .Select(m => Path.GetFileNameWithoutExtension(m.FullName)).ToList();
            return View(ThemeList);
        }
        [HttpGet]
        public ActionResult Add()
        {
            return RedirectToAction("List");
        }
        [HttpPost]
        public ActionResult Add(HttpPostedFileBase ThemeFile)
        {
            if (ModelState.IsValid)
            {
                if (Path.GetExtension(ThemeFile.FileName).ToLower() == ".zip")
                {
                    string NewThemeDirectory = Server.MapPath("/Views/") + Path.GetFileNameWithoutExtension(ThemeFile.FileName);
                    string zipPath = Server.MapPath("/Views/") + ThemeFile.FileName;
                    try
                    {
                        if (Directory.Exists(NewThemeDirectory))
                        {
                            TempData["ErrorMessage"] = "Theme already exists, please upload new theme.";
                        }
                        else
                        {
                            //Directory.CreateDirectory(NewThemeDirectory);

                            ThemeFile.SaveAs(zipPath);
                            ZipFile.ExtractToDirectory(zipPath, Server.MapPath("/Views/"));
                            
                            bool IsValid = false;
                            List<string> MissingFiles = IsValidTheme(NewThemeDirectory, ref IsValid);
                            if (!IsValid)
                            {
                                FileInfo f = new FileInfo(zipPath);
                                f.Delete();
                                cleanDirectory(NewThemeDirectory);
                                Directory.Delete(NewThemeDirectory);
                                TempData["ErrorMessage"] = "Below files are missing,<br/>" + string.Join(", ", MissingFiles);
                            }
                            else
                                TempData["SuccessMessage"] = "Theme installed successfully.";
                        }
                    }
                    catch (Exception ex)
                    {
                        FileInfo f = new FileInfo(zipPath);
                        f.Delete();
                        cleanDirectory(NewThemeDirectory);
                        Directory.Delete(NewThemeDirectory);
                        TempData["ErrorMessage"] = "Oops, there seems to be some problem, Please try again.";
                    }
                }
                else
                    TempData["ErrorMessage"] = "Please upload theme in .zip format.";
            }
            return RedirectToAction("List");
        }
        private void cleanDirectory(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dirc in dir.GetDirectories())
            {
                dirc.Delete(true);
            }
        }
        private List<string> IsValidTheme(string NewThemePath, ref bool isValid)
        {
            isValid = true;
            List<string> misingFiles = new List<string>();
            DirectoryInfo ThemeDir = new DirectoryInfo(NewThemePath);
            if (ThemeDir != null)
            {
                List<string> allViews = ThemeDir.GetFiles().Select(m => m.Name.ToLower()).ToList();
                if (!allViews.Contains("master.cshtml")) { misingFiles.Add("master.cshtml"); isValid = false; }
                if (!allViews.Contains("index.cshtml")) { misingFiles.Add("index.cshtml"); isValid = false; }
                if (!allViews.Contains("category.cshtml")) { misingFiles.Add("category.cshtml"); isValid = false; }
                if (!allViews.Contains("label.cshtml")) { misingFiles.Add("label.cshtml"); isValid = false; }
                if (!allViews.Contains("archive.cshtml")) { misingFiles.Add("archive.cshtml"); isValid = false; }
                if (!allViews.Contains("search_form.cshtml")) { misingFiles.Add("search_form.cshtml"); isValid = false; }
                if (!allViews.Contains("search_result.cshtml")) { misingFiles.Add("search_result.cshtml"); isValid = false; }
                if (!allViews.Contains("content.cshtml")) { misingFiles.Add("content.cshtml"); isValid = false; }
                if (!allViews.Contains("header.cshtml")) { misingFiles.Add("header.cshtml"); isValid = false; }
                if (!allViews.Contains("footer.cshtml")) { misingFiles.Add("footer.cshtml"); isValid = false; }
                if (!allViews.Contains("slider.cshtml")) { misingFiles.Add("slider.cshtml"); isValid = false; }
                if (!allViews.Contains("navigation.cshtml")) { misingFiles.Add("navigation.cshtml"); isValid = false; }
                if (!allViews.Contains("commentslist.cshtml")) { misingFiles.Add("commentslist.cshtml"); isValid = false; }
                if (!allViews.Contains("commentform.cshtml")) { misingFiles.Add("commentform.cshtml"); isValid = false; }
                if (!allViews.Contains("recentcomments.cshtml")) { misingFiles.Add("recentcomments.cshtml"); isValid = false; }
                if (!allViews.Contains("pager.cshtml")) { misingFiles.Add("Ppager.cshtml"); isValid = false; }
                
                if (!allViews.Contains("404.cshtml")) { misingFiles.Add("404.cshtml"); isValid = false; }
                if (!allViews.Contains("500.cshtml")) { misingFiles.Add("500.cshtml"); isValid = false; }

                if (!allViews.Contains("sidebar.cshtml")) { misingFiles.Add("sidebar.cshtml"); isValid = false; }
                if (!allViews.Contains("sb_categories.cshtml")) { misingFiles.Add("sb_categories.cshtml"); isValid = false; }
                if (!allViews.Contains("sb_labels.cshtml")) { misingFiles.Add("sb_labels.cshtml"); isValid = false; }
                if (!allViews.Contains("sb_pageview.cshtml")) { misingFiles.Add("sb_pageview.cshtml"); isValid = false; }
                if (!allViews.Contains("sb_pagelist.cshtml")) { misingFiles.Add("sb_pagelist.cshtml"); isValid = false; }
                if (!allViews.Contains("sb_mostpopular.cshtml")) { misingFiles.Add("sb_mostpopular.cshtml"); isValid = false; }
                if (!allViews.Contains("sb_recentview.cshtml")) { misingFiles.Add("sb_recentview.cshtml"); isValid = false; }
                if (!allViews.Contains("sb_related.cshtml")) { misingFiles.Add("sb_related.cshtml"); isValid = false; }
                if (!allViews.Contains("sb_recentcomments.cshtml")) { misingFiles.Add("sb_recentcomments.cshtml"); isValid = false; }
                
                if (!allViews.Contains("featureimage.jpg")) { misingFiles.Add("featureimage.jpg"); isValid = false; }
            }
            else
            {
                misingFiles.Add("Theme not found");
                isValid = false;
            }
            return misingFiles;
        }
        public ActionResult Delete(string themename)
        {
            string ThemePath = Server.MapPath("/Views/") + themename;
            if (!NotListedDirectory.Contains(themename.ToLower()) && Directory.Exists(ThemePath))
            {
                FileInfo f = new FileInfo(ThemePath + ".zip");
                f.Delete();
                cleanDirectory(ThemePath);
                Directory.Delete(ThemePath);

                Setting CurrentTheme = context.Settings.FirstOrDefault(m => m.Name == "CurrentTheme");
                if (CurrentTheme != null && CurrentTheme.Value.ToLower() == themename)
                {
                    Request.RequestContext.HttpContext.Application["CurrentTheme"] = "Default";
                }
                TempData["SuccessMessage"] = "Theme deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Theme Not Found.";
            }
            return RedirectToAction("List");
        }
        [HttpGet]
        public ActionResult Activate(string themename)
        {
            try
            {
                string ThemePath = Server.MapPath("/Views/") + themename;
                if (Directory.Exists(ThemePath))
                {
                    Setting CurrentTheme = context.Settings.FirstOrDefault(m => m.Name == "CurrentTheme");
                    if (CurrentTheme == null)
                    {
                        CurrentTheme = new Setting() { Name = "CurrentTheme", Value = themename };
                        context.Settings.Add(CurrentTheme);
                    }
                    else CurrentTheme.Value = themename;
                    context.SaveChanges();
                    Request.RequestContext.HttpContext.Application["CurrentTheme"] = themename;
                    TempData["SuccessMessage"] = "Theme has been activated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Theme not found, Please try again.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Oops, there seems to be some problem, Please try again.";
            }
            return RedirectToAction("List");
        }
    }
}
