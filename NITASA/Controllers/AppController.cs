﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Controllers
{
    public class AppController : Controller
    {
        public string activeTheme = string.Empty;
        
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            activeTheme = "/Views/" + Request.RequestContext.HttpContext.Application["CurrentTheme"].ToString() + "/";
            ViewBag.ActiveTheme = activeTheme;

            base.OnActionExecuting(filterContext);
        }
	}
}