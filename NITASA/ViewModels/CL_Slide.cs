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
        public int Image { get; set; }
        public int Title { get; set; }
        public int Text { get; set; }
        public int Link { get; set; }
        public int DisplayOrder { get; set; }
	}
}