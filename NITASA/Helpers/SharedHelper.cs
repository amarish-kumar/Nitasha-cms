using NITASA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Helpers
{
    public class SharedHelper
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
    }
}