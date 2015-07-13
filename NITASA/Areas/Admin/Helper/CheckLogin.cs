using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NITASA.Data;
using NITASA.Services.Caching;
using NITASA.Services.Security;

namespace NITASA.Areas.Admin.Helper
{
    public class CheckLoginAttribute : ActionFilterAttribute
    {
        public IAclService aclService { get; set; }

        public CheckLoginAttribute()
        {

        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (ConfigurationManager.AppSettings["Installed"] == null)
            {
                filterContext.Result = new RedirectResult(@"~/Configure/Install");
            }
            else
            {
                HttpContext hContext = HttpContext.Current;
                string url = HttpContext.Current.Request.Url.AbsoluteUri;

                if (hContext.Session["UserID"] == null)
                    filterContext.Result = new RedirectResult(@"~/Admin/Authenticate/Login?retUrl=" + HttpUtility.UrlEncode(url));
                else if (!aclService.IsActiveUser(Convert.ToInt32(hContext.Session["UserID"])))
                {
                    hContext.Session["UserID"] = null;
                    filterContext.Result = new RedirectResult(@"~/Admin/Authenticate/Login?retUrl=" + HttpUtility.UrlEncode(url));
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}