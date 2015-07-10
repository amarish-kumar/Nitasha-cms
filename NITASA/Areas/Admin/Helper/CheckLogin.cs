﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NITASA.Services.Security;

namespace NITASA.Areas.Admin.Helper
{
    public class CheckLoginAttribute : ActionFilterAttribute
    {
        IAclService aclService;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (ConfigurationManager.AppSettings["Installed"] == null)
            {
                filterContext.Result = new RedirectResult(@"~/Configure/Install");
            }
            else
            {
                HttpContext hContext = HttpContext.Current;
#if DEBUG
                if (hContext.Session["UserID"] == null)
                {
                    hContext.Session["UserID"] = 1;
                    var temp = new NITASA.Data.NTSDBContext().Role.Where(model => model.ID == 1 && model.IsDeleted == false);
                    hContext.Session["UserRole"] = temp.Select(m => m.Name).FirstOrDefault();
                    aclService.SetRights(1, temp.FirstOrDefault().ID);
                }
#endif

                string url = HttpContext.Current.Request.Url.AbsoluteUri;

                if (hContext.Session["UserID"] == null)
                    filterContext.Result = new RedirectResult(@"~/Admin/Authenticate/Login?retUrl=" + HttpUtility.UrlEncode(url));
            }
            base.OnActionExecuting(filterContext);
        }
    }
}