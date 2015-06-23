using System.Web.Mvc;

namespace NITASA.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapRoute(
                "_admin_Error",
                "Admin/NotFound",
                new { controller = "Home", action = "NotFound" },
                new[] { "NITASA.Areas.Admin.Controllers" }
            );

            context.MapRoute(
                "cms_admin_default",
                "Admin",
                new { controller = "Home", action = "Dashboard" },
                new[] { "NITASA.Areas.Admin.Controllers" }
            );
            
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{GUID}",
                new { action = "Dashboard", GUID = UrlParameter.Optional },
                new[] { "NITASA.Areas.Admin.Controllers" }
            );
        }
    }
}
