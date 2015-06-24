using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.ViewModels
{
    public class SiteSettings
    {
        [Display(Name = "Select Logo")]
        public string LogoPath { get; set; }

        [Display(Name = "Select Favicon")]
        public string FaviconPath { get; set; }

        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "Site Name cannot exceed 50 characters.")]
        [Required(ErrorMessage = "Please enter Site Name")]
        [Display(Name = "Site Name")]
        [AllowHtml]
        public string SiteName { get; set; }

        [MaxLength(50)]
        [StringLength(50, ErrorMessage = "Site Title cannot exceed 50 characters")]
        [Required(ErrorMessage = "Please enter Site title")]
        [Display(Name = "Site Title ")]
        [AllowHtml]
        public string SiteTitle { get; set; }

        [Display(Name = "Show # of last post on home page")]
        [DefaultValue(10)]
        public string ShowNoOfLastPostsAtHome { get; set; }

        [Display(Name = "Facebook URL")]
        public string FacebookURL { get; set; }

        [Display(Name = "Twitter URL")]
        public string TwitterURL { get; set; }

        [Display(Name = "Google Plus URL")]
        public string GooglePlusURL { get; set; }

        [Display(Name = "LinkedIn URL")]
        public string LinkedInURL { get; set; }

        [Display(Name = "Pinterest URL")]
        public string PinterestURL { get; set; }

        [AllowHtml]
        [Display(Name = "Custom Head Tags")]
        public string CustomHeadTag { get; set; }

        [Display(Name = "Default Meta Tags. (This will be included in head section of the page.)")]
        [AllowHtml]
        public string DefaultMetaTags { get; set; }

        [AllowHtml]
        [Display(Name = "Custom Javascript")]
        public string CustomJavaScript { get; set; }

        [AllowHtml]
        [Display(Name = "Custom CSS")]
        public string CustomCSS { get; set; }

        [AllowHtml]
        [Display(Name = "Google Analytics ")]
        public string GoogleAnalytics { get; set; }
    }
}