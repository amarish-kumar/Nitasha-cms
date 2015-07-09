using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NITASA.Core.ViewModels
{
    public class PluginListModel
    {
        public PluginListModel()
        {
            AvailableLoadModes = new List<SelectListItem>();
            AvailableGroups = new List<SelectListItem>();
        }

        public int SearchLoadModeId { get; set; }
        public string SearchGroup { get; set; }
        public IList<SelectListItem> AvailableLoadModes { get; set; }
        public IList<SelectListItem> AvailableGroups { get; set; }
    }
}
