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
            base.OnActionExecuting(filterContext);
        }
    }
}