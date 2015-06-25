using Newtonsoft.Json;
using NITASA.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace NITASA.Areas.Admin.Helper
{
    public class Functions
    {
        public static string GetUserProfilePhoto()
        {
            string src = "";
            if (HttpContext.Current.Session["UserID"] != null)
            {
                int uID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                NTSDBContext context = new NTSDBContext();
                src = context.User.Where(m => m.ID == uID).Select(x => x.ProfilePicURL).FirstOrDefault();
            }
            return src;
        }

        public static string GetUserName()
        {
            string src = "";
            if (HttpContext.Current.Session["UserID"] != null)
            {
                int uID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                NTSDBContext context = new NTSDBContext();
                src = context.User.Where(m => m.ID == uID).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
            }
            return src;
        }

        public static int CurrentUserID()
        {
            int userID = 0;
            if (HttpContext.Current.Session["UserID"] != null)
            {
                try
                {
                    userID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                }
                catch { }
            }
            return userID;
        }

        public static string CurrentUserRole()
        {
            string userRoll = string.Empty;
            if (HttpContext.Current.Session["UserRole"] != null)
            {
                try
                {
                    userRoll = Convert.ToString(HttpContext.Current.Session["UserRole"]);
                }
                catch { }
            }
            return userRoll;
        }

        public static string GetRandomGUID()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        public static string ToUrlSlug(string value, string type, int id)
        {
            //First to lower case
            value = value.ToLowerInvariant();
            //Remove all accents
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);
            value = Encoding.ASCII.GetString(bytes);
            //Replace spaces
            value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);
            //Remove invalid chars
            value = Regex.Replace(value, @"[^\w\s\p{Pd}]", "", RegexOptions.Compiled);
            //Trim dashes from end
            value = value.Trim('-', '_');
            //Replace double occurences of - or \_
            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            //return value;
            return GetNewUrl(value, type, id);
        }

        public static string GetNewUrl(string url, string type, int id)
        {
            string newUrl = url.ToLower();
            using (NTSDBContext _dbContext = new NTSDBContext())
            {
                // check for Post
                //if (type.ToLower() == "post")
                //{
                //    bool toContinue = true;
                //    int urlCounter = 1;
                //    do
                //    {
                //        var checkUrlExist = _dbContext.Content.Where(m => m.ContentType.ToLower() == "post").Where(m => 
                //                m.ContentURL.ToLower() == newUrl && (id==0 ||  m.ContentID != id));
                //        if (checkUrlExist.Count() > 0)
                //        {
                //            newUrl = url + urlCounter.ToString();
                //            urlCounter++;
                //        }
                //        else
                //        {
                //            toContinue = false;
                //        }
                //    } while (toContinue);
                //}
                if (type.ToLower() == "category")
                {
                    bool toContinue = true;
                    int urlCounter = 1;
                    do
                    {
                        var checkUrlExist = _dbContext.Category.Where(m => m.Slug.ToLower() == newUrl && (id == 0 || m.ID != id));
                        if (checkUrlExist.Count() > 0)
                        {
                            newUrl = url + urlCounter.ToString();
                            urlCounter++;
                        }
                        else
                        {
                            toContinue = false;
                        }
                    } while (toContinue);
                }
                else if (type.ToLower() == "label")
                {
                    bool toContinue = true;
                    int urlCounter = 1;
                    do
                    {
                        var checkUrlExist = _dbContext.Label.Where(m => m.Slug.ToLower() == newUrl && (id == 0 || m.ID != id));
                        if (checkUrlExist.Count() > 0)
                        {
                            newUrl = url + urlCounter.ToString();
                            urlCounter++;
                        }
                        else
                        {
                            toContinue = false;
                        }
                    } while (toContinue);
                }
                else // for page and post
                {
                    bool toContinue = true;
                    int urlCounter = 1;
                    do
                    {
                        var checkUrlExist = _dbContext.Content.Where(m => m.URL.ToLower() == newUrl && (id == 0 || m.ID != id));
                        if (checkUrlExist.Count() > 0)
                        {
                            newUrl = url + urlCounter.ToString();
                            urlCounter++;
                        }
                        else
                        {
                            toContinue = false;
                        }
                    } while (toContinue);
                }
            }
            return newUrl;
        }

        public static string RemoveHTMLTags(string sourceString)
        {
            sourceString = Regex.Replace(sourceString, @"<[^>]+>|&nbsp;|\n|\r", string.Empty);
            sourceString = sourceString.Replace("&lt;Addon&gt;", string.Empty).Replace("&lt;/Addon&gt;", string.Empty); // replace addon tag
            return sourceString;
        }

        public static string GetNewFileName(string targetFolder, string currentFileName)
        {
            string fileExtension = System.IO.Path.GetExtension(currentFileName);
            string fileName = currentFileName.Replace(fileExtension, "");
            string newFileName = fileName + fileExtension;
            string newFilePath = Path.Combine(targetFolder, newFileName);
            int fileCounter = 1;
            while (File.Exists(HttpContext.Current.Server.MapPath(newFilePath)))
            {
                newFileName = fileName + fileCounter + fileExtension;
                newFilePath = Path.Combine(targetFolder, newFileName);
                fileCounter++;
            }

            return newFilePath;
        }

        public static bool IsValidImage(string FileName)
        {
            string[] extension = new string[] { ".jpg", ".bmp", ".jpeg", ".png", ".gif" };

            if (extension.Contains(Path.GetExtension(FileName)))
                return true;
            else
                return false;
        }

        public static bool IsValidVideo(string FileName)
        {
            string[] extension = new string[] { ".mp4" };

            if (extension.Contains(Path.GetExtension(FileName)))
                return true;
            else
                return false;
        }

        public static string CurrentTimeStamp()
        {
            StringBuilder sbTimeStamp = new StringBuilder();
            sbTimeStamp.Append(DateTime.Now.Year.ToString());
            sbTimeStamp.Append(DateTime.Now.Month.ToString());
            sbTimeStamp.Append(DateTime.Now.Day.ToString());
            sbTimeStamp.Append(DateTime.Now.Hour.ToString());
            sbTimeStamp.Append(DateTime.Now.Minute.ToString());
            sbTimeStamp.Append(DateTime.Now.Second.ToString());
            return sbTimeStamp.ToString();
        }
        
        public static string GetAddons()
        {
            NTSDBContext _dbContext = new NTSDBContext();
            List<string> temp = new List<string> { "page", "post" };
            List<Content> AllAddons = _dbContext.Content.Where(x => x.isPublished == true && x.IsDeleted == false && !temp.Contains(x.Type)).ToList();

            List<string> addontypes = AllAddons.Select(x => x.Type).Distinct().ToList();

            List<TMenu> itemList = new List<TMenu>();
            itemList = (from at in addontypes
                        select
                             new TMenu
                             {
                                 text = at,
                                 SubMenu = (from ad in AllAddons
                                            where ad.Type == at
                                            select new TMenu { id = ad.ID.ToString(), text = ad.Title }).ToList()
                             }
                        ).ToList();
            Random r = new Random();
            itemList.ForEach(x =>
                x.SubMenu.InsertRange(0, new List<TMenu> { new TMenu { id = string.Concat("tinyAll", r.Next()), text = "All" } })
            );
            string json = JsonConvert.SerializeObject(itemList);

            return json;
        }
    }

    public class TMenu
    {
        public string text { get; set; }
        public string id { get; set; }
        [JsonProperty(PropertyName = "menu")]
        public List<TMenu> SubMenu { get; set; }
    }
}