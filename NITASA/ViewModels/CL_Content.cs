using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NITASA.ViewModels
{
    public class CL_Content
    {
        public int ContentID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public string CoverContent { get; set; }
        public string FeaturedImage { get; set; }
        public DateTime PublishedOn { get; set; }
        public int? AddedBy { get; set; }
        public int CommentsCount { get; set; }
        public string Type { get; set; }

        public bool PostCommentEnable { get; set; }
        public int PostCommentEnabledTill { get; set; }
        
        public List<CL_Comments> Comments { get; set; }
    }
}