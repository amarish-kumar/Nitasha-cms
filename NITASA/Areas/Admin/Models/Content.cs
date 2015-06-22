using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Models
{
    public class Content
    {
        [Key]
        public int ID { get; set; }
        public string GUID { get; set; }
        public string Type { get; set; }

        [Required(ErrorMessage = "Please enter Title.")]
        [MaxLength(200)]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]        
        [Display(Name = "Title")]
        [AllowHtml]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter description.")]
        [Display(Name = "Description")]
        [UIHint("tinymce_simple"), AllowHtml]
        public string Description { get; set; }

        public string URL { get; set; }

        public string CoverContent { get; set; }
        public string FeaturedImage { get; set; }

        public bool IsSlugEdited { get; set; }
        public bool IsFeatured { get; set; }

        [DefaultValue(0)]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Enter only integer value")]
        public int ContentOrder { get; set; }
        [Display(Name = "Comment Enabled?")]
        public bool EnableComment { get; set; }
        public int CommentEnabledTill { get; set; }

        [DefaultValue(0)]
        public int ContentView { get; set; }

        public DateTime AddedOn { get; set; }

        public bool isPublished { get; set; }

        public DateTime? PublishedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? AddedBy { get; set; }

        public int? ModifiedBy { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        [ForeignKey("AddedBy")]
        public virtual User user { get; set; }
        [ForeignKey("ContentID")]
        public virtual ICollection<ContentCategory> contentCategory { get; set; }
        [ForeignKey("ContentID")]
        public virtual ICollection<ContentLabel> contentLabel { get; set; }
    }

    public class ContentCategory
    {
        [Key]
        public int ID { get; set; }

        public int CategoryID { get; set; }

        public int ContentID { get; set; }

        public DateTime? AddedOn { get; set; }
        public int AddedBy { get; set; }

        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }

        [ForeignKey("ContentID")]
        public virtual Content Content { get; set; }
    }

    public class ContentLabel
    {
        [Key]
        public int ID { get; set; }

        public int LabelID { get; set; }

        public int ContentID { get; set; }

        public DateTime? AddedOn { get; set; }
        public int AddedBy { get; set; }

        [ForeignKey("LabelID")]
        public virtual Label Label { get; set; }

        [ForeignKey("ContentID")]
        public virtual Content Content { get; set; }
    }

    public class ContentView
    {
        [Key]
        public int ID { get; set; }
        public int ContentID { get; set; }
        public DateTime? ViewedOn { get; set; }
        public string IPAddress { get; set; }
        public string Browser { get; set; }
        public string BVersion { get; set; }
        public string Resolution { get; set; }
        public string OSName { get; set; }
        public string OSVersion { get; set; }
    }
}
