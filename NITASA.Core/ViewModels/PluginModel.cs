using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NITASA.Core.ViewModels
{
    public class PluginModel
    {
        [AllowHtml]
        public string Group { get; set; }

        [AllowHtml]
        public string FriendlyName { get; set; }

        [AllowHtml]
        public string SystemName { get; set; }

        [AllowHtml]
        public string Version { get; set; }

        [AllowHtml]
        public string Author { get; set; }

        public int DisplayOrder { get; set; }

        public string ConfigurationUrl { get; set; }

        public bool Installed { get; set; }

        public bool CanChangeEnabled { get; set; }
        public bool IsEnabled { get; set; }

        public string LogoUrl { get; set; }
    }
}