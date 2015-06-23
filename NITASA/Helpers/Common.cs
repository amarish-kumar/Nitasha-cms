using NITASA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace NITASA.Helpers
{
    public class Common
    {
        private static NTSDBContext _dbContext = new NTSDBContext();

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
            else // for Page and post
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
            return newUrl;
        }
        public static string RemoveHTMLTags(string sourceString)
        {
            sourceString = Regex.Replace(sourceString, @"<[^>]+>|&nbsp;|\n|\r", string.Empty);
            sourceString = sourceString.Replace("&lt;Addon&gt;", string.Empty).Replace("&lt;/Addon&gt;", string.Empty); // replace addon tag
            return sourceString;
        }
    }
}