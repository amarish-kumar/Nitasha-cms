using NITASA.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NITASA.ViewModels
{
    public class CL_Label
    {
        public string Name { get; set; }
        public string URL { get; set; }
        public string Description { get; set; }
        public List<CL_Content> Posts { get; set; }
        public Pager Pager { get; set; }
    }
}