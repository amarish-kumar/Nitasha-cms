﻿using Newtonsoft.Json;
using NITASA.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace NITASA.Helpers
{
    public class Common
    {
        public static string Encrypt(string toEncrypt, string key, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string toDecrypt, string key, bool useHashing)
        {
            byte[] keyArray;

            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
            string decryptValue = string.Empty;
            try
            {
                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                decryptValue = UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return decryptValue;
        }

        public static int GetSaltKey()
        {
            Random ran = new Random();
            int number = ran.Next(1000, 9999);
            ran.Next();
            return number;
        }

        public static string GeneratePassword(byte Length)
        {
            char[] Chars = new char[] {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            string String = string.Empty;
            Random Random = new Random();

            for (byte a = 0; a < Length; a++)
            {
                String += Chars[Random.Next(0, 61)];
            };

            return (String);
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
            return Common.GetNewUrl(value, type, id);
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
        //public static string GetAddons()
        //{
        //    NTSDBContext _dbContext = new NTSDBContext();

        //    List<string> temp = new List<string> { "page", "post" };
        //    List<Content> AllAddons = _dbContext.Content.Where(x => x.isPublished==true && x.IsDeleted == false && !temp.Contains(x.Type)).ToList();

        //    List<string> addontypes = AllAddons .Select(x => x.Type).Distinct().ToList();

        //    string strAddons = string.Empty;

        //    foreach (string name in addontypes)
        //    {
        //        List<Content> contents = AllAddons.Where(x => x.Type == name).ToList();
        //        string allcontents = string.Join("", contents.Select(x => "&lt;Addon name=\"" + x.Title + "\"&gt;" + x.ID + "&lt;/Addon&gt;").ToList());
        //        strAddons += "{" +
        //        "text: '" + name + "'," +
        //        //"onclick: function(){" +
        //        //"             editor.insertContent('" + allcontents + "');return false;" +
        //        //"}," +
        //        "menu: [";

        //        if (contents.Count() > 1)
        //        {
        //            strAddons += "    {" +
        //                        "        text: 'All'," +
        //                        "        id:'foobar',"+
        //                        "        onclick: function(){" +
        //                        "             editor.insertContent('" + allcontents + "');" +
        //                        "        }" +
        //                        "    },";
        //        }
        //        foreach (var item in contents)
        //        {
        //            strAddons += "    {" +
        //                        "        text: '" + item.Title + "'," +
        //                        "        onclick: function(){" +
        //                        "             editor.insertContent('&lt;Addon name=\"" + item.Title + "\"&gt;" + item.ID + "&lt;/Addon&gt;');" +
        //                        "        }" +
        //                        "    },";
        //        }
        //        strAddons = strAddons.Remove(strAddons.LastIndexOf(","), 1);
        //        strAddons += "]" +
        //        "},";
        //    }
        //    strAddons = strAddons.Remove(strAddons.LastIndexOf(","), 1);
        //    return strAddons;
        //}
        public static string GetAddons()
        {
            NTSDBContext _dbContext = new NTSDBContext();
            List<string> temp = new List<string> { "page", "post" };
            List<Content> AllAddons = _dbContext.Content.Where(x => x.isPublished == true && x.IsDeleted == false && !temp.Contains(x.Type)).ToList();

            List<string> addontypes = AllAddons.Select(x => x.Type).Distinct().ToList();

            List<Menu> itemList =new List<Menu>();
            itemList = (from at in addontypes
                       select
                            new Menu{
                                    text=at,
                                    SubMenu = (from ad in AllAddons
                                              where ad.Type == at
                                              select new Menu {id = ad.ID.ToString(),text =ad.Title}).ToList()
                            }
                        ).ToList();
            Random r = new Random();
            itemList.ForEach(x=>
                x.SubMenu.InsertRange(0, new List<Menu> { new Menu { id = string.Concat("tinyAll",r.Next()), text = "All" } })
            );
            string json = JsonConvert.SerializeObject(itemList);

            /*string json = JsonConvert.SerializeObject(
                                new List<Menu>
                                    {
                                        new Menu{ 
                                            //id ="1", 
                                            text ="A"
                                        },
                                        new Menu{ 
                                            //id ="2", 
                                            text ="B",
                                            SubMenu =  new List<Menu> {
                                                new Menu{ 
                                                    id ="3", 
                                                    text ="C"
                                                },
                                                new Menu{ 
                                                    id ="4", 
                                                    text ="D' company"
                                                }
                                            }
                                        }
                                    });*/
            return json;
        }
    }

    public class Menu
    {
        public string text { get; set; }
        public string id { get; set; }
        [JsonProperty(PropertyName = "menu")]
        public List<Menu> SubMenu { get; set; }
    }
}