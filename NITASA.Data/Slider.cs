using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Data
{
    public class Slide
    {
        [Key]
        public int ID { get; set; }        
        public string GUID { get; set; }

        public int SliderId { get; set; }

        [Required(ErrorMessage = "Please select slider image.")]
        public string Image { get; set; }
        [AllowHtml]
        public string Title { get; set; }
        [AllowHtml]
        public string Text { get; set; }
        [RegularExpression(@"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$", ErrorMessage = "Enter valid image link.")]
        public string Link { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime AddedOn { get; set; }
        public int AddedBy { get; set; }

        [ForeignKey("SliderId")]
        public virtual Slider Slider { get; set; }
    }

    public class Slider
    {
        [Key]
        public int Id { get; set; }
        public string GUID { get; set; }
        [Required(ErrorMessage = "Please enter slider name.")]
        [AllowHtml]
        public string Name { get; set; }
        public string Code { get; set; }

        public DateTime AddedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int AddedBy { get; set; }
        public int? ModifiedBy { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        public virtual ICollection<Slide> Slides { get; set; }

    }
}