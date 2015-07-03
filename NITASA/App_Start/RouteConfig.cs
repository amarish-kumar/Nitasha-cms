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
                "_Search",
                "Search/{SearchText}",
                new { controller = "Search", action = "Results", SearchText = UrlParameter.Optional },
                new[] { "NITASA.Controllers" }
            );
            routes.MapRoute(
                "_Content-Comment",
                "Content/AddComment/{ContentID}",
                new { controller = "Content", action = "AddComment", ContentID = UrlParameter.Optional },
                new[] { "NITASA.Controllers" }
            );
            routes.MapRoute(
                "_Content-Comment-Flag",
                "Content/AddFlag/{CommentID}",
                new { controller = "Content", action = "AddFlag", CommentID = UrlParameter.Optional },
                new[] { "NITASA.Controllers" }
            );
            routes.MapRoute(
                "_Content",
                "Content/{URL}",
                new { controller = "Content", action = "Details", URL = UrlParameter.Optional },
                new[] { "NITASA.Controllers" }
            );
            routes.MapRoute(
              "_Category",
              "Category/{URL}",
              new { controller = "Category", action = "List", URL = UrlParameter.Optional },
              new[] { "NITASA.Controllers" }
            );
            routes.MapRoute(
              "_Label",
              "Label/{URL}",
              new { controller = "Label", action = "List", URL = UrlParameter.Optional },
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