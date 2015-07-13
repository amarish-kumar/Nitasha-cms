using NITASA.Areas.Admin.Helper.Sitemap;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NITASA.Areas.Admin.ViewModels
{
    public class Sitemap
    {
        [Display(Name = "Current Sitemap")]
        public double? SitemapText { get; set; }

        public SitemapChangeFrequency? postFreqency { get; set; }

        [Range(0.1, 1.0, ErrorMessage = "Only 0.1 to 1.0 allowed.")]
        public double? postPriority { get; set; }

        public SitemapChangeFrequency? pageFreqency { get; set; }
        [Range(0.1, 1.0, ErrorMessage = "Only 0.1 to 1.0 allowed.")]
        public double? pagePriority { get; set; }

        public SitemapChangeFrequency? categoryFreqency { get; set; }
        [Range(0.1, 1.0, ErrorMessage = "Only 0.1 to 1.0 allowed.")]
        public double? categoryPriority { get; set; }

        public SitemapChangeFrequency? labelFreqency { get; set; }
        [Range(0.1, 1.0, ErrorMessage = "Only 0.1 to 1.0 allowed.")]
        public double? labelPriority { get; set; }
    }
}