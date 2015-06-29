using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NITASA.ViewModels
{
    public class CL_Slide
	{
        public string Image { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
        public int DisplayOrder { get; set; }
	}
}