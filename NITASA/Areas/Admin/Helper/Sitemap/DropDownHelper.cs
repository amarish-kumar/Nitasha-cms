using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Helper.Sitemap
{
    public static class DropDownHelper
    {
        public static SelectList GetFrequencyDDL(string selected)
        {
            IEnumerable<SitemapChangeFrequency> actionTypes = Enum.GetValues(typeof(SitemapChangeFrequency)).Cast<SitemapChangeFrequency>();
            var temp = new SelectList(from action in actionTypes
                                      select new SelectListItem
                                      {
                                          Text = action.ToString(),
                                          Value = ((int)action).ToString(),
                                          Selected = selected == ((int)action).ToString()
                                      }, "Value", "Text");
            return temp;
        }
    }
}