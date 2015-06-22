using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Models
{
    public class Slider
    {
        [Key]
        public int ID { get; set; }        
        public string GUID { get; set; }

        public int SliderMasterID { get; set; }

        [Required(ErrorMessage = "Please select slider image")]
        public string Image { get; set; }
        [AllowHtml]
        public string Title { get; set; }
        [AllowHtml]
        public string Text { get; set; }
        [RegularExpression(@"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$", ErrorMessage = "Enter Valid Image Link")]
        public string Link { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime AddedOn { get; set; }
        public int AddedBy { get; set; }
    }

    public class SliderMaster
    { 

    }
}