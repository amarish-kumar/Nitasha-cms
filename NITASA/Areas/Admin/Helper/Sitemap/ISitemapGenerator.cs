using System.Collections.Generic;
using System.Xml.Linq;

namespace NITASA.Areas.Admin.Helper.Sitemap
{
    public interface ISitemapGenerator
    {
        XDocument GenerateSiteMap(IEnumerable<ISitemapItem> items);
    }
}
