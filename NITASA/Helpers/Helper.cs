using NITASA.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace NITASA.Helpers
{
    public class Helper
    {
        public static NTSDBContext getDbContextObject()
        {
            return new NTSDBContext();
        }

        public static bool HasLogo()
        {
            var dbContext = getDbContextObject();
            if (!String.IsNullOrWhiteSpace(dbContext.Settings.FirstOrDefault(m => m.Name == "LogoPath").Value))
                return true;
            else
                return false;
        }

        public static string GetLogoPath()
        {
            var dbContext = getDbContextObject();
            Setting LogoPath = dbContext.Settings.FirstOrDefault(m => m.Name == "LogoPath");
            return ((LogoPath == null) ? string.Empty : Convert.ToString(LogoPath.Value));
        }

        public static string GetSiteName()
        {
            var dbContext = getDbContextObject();
            Setting SiteName = dbContext.Settings.FirstOrDefault(m => m.Name == "SiteName");
            return ((SiteName == null) ? string.Empty : Convert.ToString(SiteName.Value));
        }

        public static string GetSiteTitle()
        {
            var dbContext = getDbContextObject();
            Setting SiteTitle = dbContext.Settings.FirstOrDefault(m => m.Name == "SiteTitle");
            return ((SiteTitle == null) ? string.Empty : Convert.ToString(SiteTitle.Value));
        }

        public static string GetFaviconURL()
        {
            var dbContext = getDbContextObject();
            Setting FaviconPath = dbContext.Settings.FirstOrDefault(m => m.Name == "FaviconPath");
            if (FaviconPath != null)
            {
                string strURL = FaviconPath.Value;
                if (File.Exists(HttpContext.Current.Server.MapPath(strURL)))
                    return "<link href='" + strURL + "' rel='icon' type='image/x-icon'/>";
                else
                    return "";
            }
            else
                return "";
        }

        public static string GetMetaData(string contentURL)
        {
            var dbContext = getDbContextObject();
            Content contentEdit = dbContext.Content.Where(content =>
                   content.URL == contentURL && content.IsDeleted == false && content.isPublished == true).FirstOrDefault();

            List<Meta> meta = (from mt in dbContext.Meta
                               join cont in dbContext.Content on mt.ContentID equals cont.ID
                               where cont.URL == contentURL && cont.IsDeleted == false && cont.isPublished == true && cont.IsDeleted == false
                               select mt).ToList();

            string strMeta = "";
            if (meta.Count() > 0)
            {
                strMeta = "<meta name=\"description\" content=\"" + meta[0].Description + "\">";
                strMeta += "<meta name=\"author\" content=\"" + meta[0].Author + "\">";
                strMeta += "<meta name=\"keywords\" content=\"" + meta[0].Keyword + "\">";
            }
            return strMeta;
        }

        public static string GetSiteURL()
        {
            return "/home";
        }

        public static List<Category> GetCategoryList()
        {
            var dbContext = getDbContextObject();
            List<Category> catList = dbContext.Category.Where(x => x.IsDeleted == false).OrderBy(x => x.Name).ToList();
            return catList;
        }

        public static bool HasThumbnail(int contentID)
        {
            bool hasThumb = false;
            MatchCollection matchesImgSrc = FindImages(contentID);
            if (matchesImgSrc.Count > 0)
            {
                hasThumb = true;
            }
            return hasThumb;
        }

        public static string GetThumbnail(int contentID)
        {
            string link = string.Empty;
            MatchCollection matchesImgSrc = FindImages(contentID);
            //"<img src="../..//content/media/big truck.jpg" alt width="2897" height="1932">
            string originalTag = matchesImgSrc[0].Value;
            link = matchesImgSrc[0].Value;
            link = link.Replace("<img src=\"", "");
            int length = link.IndexOf("\"");
            if (length > 0 && link.Length > length)
            {
                link = link.Substring(0, length);
            }
            else
            {
                link = originalTag;
            }
            return link;
        }

        public static MatchCollection FindImages(int contentID)
        {
            string regexImgSrc = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
            var dbContext = getDbContextObject();
            Content content = dbContext.Content.Where(x => x.ID == contentID).FirstOrDefault();
            return Regex.Matches(content.Description, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        public static bool HasImage(string content)
        {
            bool hasImg = false;
            string regexImgSrc = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
            MatchCollection matchesImgSrc = Regex.Matches(content, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (matchesImgSrc.Count > 0)
            {
                hasImg = true;
            }
            return hasImg;
        }

        public static string GetImage(string content)
        {
            string link = string.Empty;
            string regexImgSrc = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
            MatchCollection matchesImgSrc = Regex.Matches(content, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            link = matchesImgSrc[0].Groups[1].Value;
            return link;
        }

        public static bool IsCommentEnabled(int contentID)
        {
            var dbContext = getDbContextObject();
            var content = dbContext.Content.Where(x => x.ID == contentID).Where(x => x.EnableComment != false).ToList();

            if (content.Count() > 0)
                return true;
            else
                return false;
        }

        public static int GetCommentEnabledTill(int contentID)
        {
            var dbContext = getDbContextObject();
            int tillDate = dbContext.Content.Where(x => x.ID == contentID).Select(x => x.CommentEnabledTill).FirstOrDefault();
            return tillDate;
        }

        public static string GetMenu()
        {
            var dbContext = getDbContextObject();
            List<Menu> AllMenuList = dbContext.Menu.Where(m => m.IsDeleted == false).ToList();
            List<ClientSideMenu> AllMenuListSlug =
                (from m in AllMenuList
                 select new ClientSideMenu()
                 {
                     MenuID = m.ID,
                     ParentMenuID = m.ParentMenuID,
                     MenuTitle = m.Title,
                     MenuType = m.Type,
                     DisplayOrder = m.DisplayOrder,
                     MenuSlug = (m.Type == "Link" ? m.Slug : getURLSlug(m.Type, m.Slug) +
                               (m.Type == "Category" ? dbContext.Category.Where(c => c.ID == m.RefID).Select(s => s.Slug).FirstOrDefault() :
                               (m.Type == "Label" ? dbContext.Label.Where(c => c.ID == m.RefID).Select(s => s.Slug).FirstOrDefault() :
                               (m.Type == "Page" ? dbContext.Content.Where(c => c.ID == m.RefID).Select(s => s.URL).FirstOrDefault() : "")
                               )))
                 }
                ).ToList();

            string jsonString = System.Web.Helpers.Json.Encode(AllMenuListSlug);

            return jsonString;
        }

        private static string getURLSlug(string type, string slug)
        {
            if (type == "Page")
                type = "Single";

            string strURL = HttpContext.Current.Request.Url.AbsoluteUri.ToString();

            strURL = strURL.Remove(strURL.IndexOf(HttpContext.Current.Request.Url.AbsolutePath.ToString()));
            return strURL + "/" + type + "/Index/" + slug;
        }

        public static string GetDefaultMetaData()
        {
            var dbContext = getDbContextObject();
            Setting DefaultMetaTags = dbContext.Settings.FirstOrDefault(m => m.Name == "DefaultMetaTags");
            return ((DefaultMetaTags == null) ? string.Empty : Convert.ToString(DefaultMetaTags.Value));
        }

        public static string GetHeaderCustomCode()
        {
            var dbContext = getDbContextObject();
            Setting CustomHeadTag = dbContext.Settings.FirstOrDefault(m => m.Name == "CustomHeadTag");
            return ((CustomHeadTag == null) ? string.Empty : Convert.ToString(CustomHeadTag.Value));
        }

        public static string GetFooterCustomCode()
        {
            var dbContext = getDbContextObject();
            //string FooterScript = string.Empty;
            //AspNetCMS.Areas.CMSAdmin.Models.Config config = _dbContext.Config.FirstOrDefault();
            //if (config != null)
            //{
            //    FooterScript = config.FooterScript;
            //}
            //return FooterScript;
            //return HttpContext.Current.Application["CustomHeadTag"].ToString().Trim();
            return "";
        }

        public static List<string> GetMediaList(string MediaType)
        {
            var dbContext = getDbContextObject();
            return dbContext.Media.Where(m => m.Type == MediaType && m.IsDeleted == false).OrderByDescending(m => m.AddedOn).Select(m => m.FileName).ToList();
        }

        public static List<Content> GetContentByCategory(string categoryName, int contentCount)
        {
            var dbContext = getDbContextObject();
            int catid = dbContext.Category.Where(cat => cat.Name == categoryName).Select(cat => cat.ID).FirstOrDefault();

            if (contentCount <= 0)
                return (from cont in dbContext.Content
                        join concat in dbContext.ContentCategory on cont.ID equals concat.ContentID
                        where concat.CategoryID == catid && cont.isPublished == true && cont.Type.ToLower() == "post" && cont.IsDeleted == false
                        select cont).OrderByDescending(x => x.ContentOrder).ToList();
            else
                return (from cont in dbContext.Content
                        join concat in dbContext.ContentCategory on cont.ID equals concat.ContentID
                        where concat.CategoryID == catid && cont.isPublished == true && cont.Type.ToLower() == "post" && cont.IsDeleted == false
                        select cont).OrderByDescending(x => x.ContentOrder).Take(contentCount).ToList();
        }

        //public static List<Addons> GetAddonList(string AddonCode, int NoOfAddons = 1)
        //{
        //    var dbContext = getDbContextObject();
        //    return dbContext.Content.Where(m => m.AddonCode == AddonCode && m.IsDeleted == false).OrderBy(m => m.DisplayOrder).Take(NoOfAddons).ToList();
        //}

        public static string GetFacebookURL()
        {
            var dbContext = getDbContextObject();
            Setting FacebookURL = dbContext.Settings.FirstOrDefault(m => m.Name == "FacebookURL");
            return ((FacebookURL == null) ? "#" : Convert.ToString(FacebookURL.Value));
        }

        public static string GetTwitterURL()
        {
            var dbContext = getDbContextObject();
            Setting TwitterURL = dbContext.Settings.FirstOrDefault(m => m.Name == "TwitterURL");
            return ((TwitterURL == null) ? "#" : Convert.ToString(TwitterURL.Value));
        }

        public static string GetGooglePlusURL()
        {
            var dbContext = getDbContextObject();
            Setting GooglePlusURL = dbContext.Settings.FirstOrDefault(m => m.Name == "GooglePlusURL");
            return ((GooglePlusURL == null) ? "#" : Convert.ToString(GooglePlusURL.Value));
        }

        public static string GetPinterestURL()
        {
            var dbContext = getDbContextObject();
            Setting PinterestURL = dbContext.Settings.FirstOrDefault(m => m.Name == "PinterestURL");
            return ((PinterestURL == null) ? "#" : Convert.ToString(PinterestURL.Value));
        }

        public static string GetCustomJavaScript()
        {
            var dbContext = getDbContextObject();
            Setting CustomJavaScript = dbContext.Settings.FirstOrDefault(m => m.Name == "CustomJavaScript");
            return ((CustomJavaScript == null) ? string.Empty : Convert.ToString(CustomJavaScript.Value));
        }

        public static string GetCustomCSS()
        {
            var dbContext = getDbContextObject();
            Setting CustomCSS = dbContext.Settings.FirstOrDefault(m => m.Name == "CustomCSS");
            return ((CustomCSS == null) ? string.Empty : Convert.ToString(CustomCSS.Value));
        }

        public static string GetGoogleAnalytics()
        {
            var dbContext = getDbContextObject();
            Setting GoogleAnalytics = dbContext.Settings.FirstOrDefault(m => m.Name == "GoogleAnalytics");
            return ((GoogleAnalytics == null) ? string.Empty : Convert.ToString(GoogleAnalytics.Value));
        }
    }
}