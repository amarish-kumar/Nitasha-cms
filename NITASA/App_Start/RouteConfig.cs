using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NITASA
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "_client_Error",
                "NotFound",
                new { controller = "Home", action = "NotFound" },
                new[] { "NITASA.Controllers" }
           );

            routes.MapRoute(
                "_Search",
                "Search/Results/{text}",
                new { controller = "Search", action = "Results" },
                new[] { "NITASA.Controllers" }
            );

            routes.MapRoute(
                "_Single",
                "Single/Details/{contentURL}",
                new { controller = "Content", action = "Details", contentURL = UrlParameter.Optional },
                new[] { "NITASA.Controllers" }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "NITASA.Controllers" }
            );

        }
    }
}