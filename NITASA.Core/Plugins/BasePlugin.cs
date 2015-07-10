using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace NITASA.Core.Plugins
{
    public class BasePlugin : IPlugin
    {
        public virtual PluginDescriptor PluginDescriptor { get; set; }

        public virtual void Install()
        {
            PluginManager.MarkPluginAsInstalled(this.PluginDescriptor.SystemName);
        }

        public virtual void Uninstall()
        {
            PluginManager.MarkPluginAsUninstalled(this.PluginDescriptor.SystemName);
        }


        public virtual void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "Nivo";
            routeValues = new RouteValueDictionary { { "Namespaces", "NivoSlider.Controllers" }, { "area", null } };
        }
    }
}