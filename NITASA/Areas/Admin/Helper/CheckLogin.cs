using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Helper
{
    public class CheckLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (ConfigurationManager.AppSettings["Installed"] == null)
            {
                filterContext.Result = new RedirectResult(@"~/Configure/Install");
            }
            else
            {
                string url = HttpContext.Current.Request.Url.AbsoluteUri;
                HttpContext hContext = HttpContext.Current;
                if (hContext.Session["UserID"] == null)
                    filterContext.Result = new RedirectResult(@"~/Admin/Authenticate/Login?retUrl=" + HttpUtility.UrlEncode(url));
            }
            base.OnActionExecuting(filterContext);
        }
    }
}