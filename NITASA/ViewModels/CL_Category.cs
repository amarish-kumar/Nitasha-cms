using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NITASA.ViewModels
{
    public class CL_Category
    {
        public string Name { get; set; }
        public string URL { get; set; }
        public string Description { get; set; }
        public int? ParentCategoryID { get; set; }
        public List<CL_Content> Posts { get; set; }
    }
}