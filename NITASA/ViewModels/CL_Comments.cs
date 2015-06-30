using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NITASA.ViewModels
{
    public class CL_Comments
    {
        public string UserName { get; set; }
        public string ProfilePicUrl { get; set; }
        public string Description { get; set; }
        public DateTime AddedOn { get; set; }
    }
}