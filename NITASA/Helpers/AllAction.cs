using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace NITASA.Helpers
{
    public class AllActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (ConfigurationManager.AppSettings["Installed"] == null)
            {
                filterContext.Result = new RedirectResult(@"~/Configure/Install");
            }
            /*string url = HttpContext.Current.Request.Url.AbsoluteUri;
            HttpContext hContext = HttpContext.Current;
            if (hContext.Session["UserID"] == null)
                filterContext.Result = new RedirectResult(@"~/CMSAdmin/Authentication/Login?retUrl=" + HttpUtility.UrlEncode(url));

            base.OnActionExecuting(filterContext);

            
            //if (Common.CurrentUserID() > 0)
            //    filterContext.Result = new RedirectResult(@"~/CMSAdmin/Authentication/Login?retUrl=" + HttpUtility.UrlEncode(url));
            //base.OnActionExecuting(filterContext);*/
        }
    }
}