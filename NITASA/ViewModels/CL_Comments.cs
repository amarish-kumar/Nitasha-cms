using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NITASA.ViewModels
{
    public class CL_Comments
    {
        public int CommentID { get; set; }
        public string UserName { get; set; }
        public string Website { get; set; }
        public string CommentAs { get; set; }
        public string ProfilePicUrl { get; set; }
        public string Description { get; set; }
        public int ContentID { get; set; }
        public DateTime AddedOn { get; set; }
    }
}