using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NITASA.Data
{
    public class Comment
    {
        [Key]
        public int ID { get; set; }

        public int ContentID { get; set; }

        [MaxLength(int.MaxValue)]
        [StringLength(int.MaxValue)]
        [Required(ErrorMessage = "Please enter comment description")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public string UserName { get; set; }
        public string ProfilePicUrl { get; set; }
        //public string EmailAddress{ get; set; }
        //public string URL{ get; set; }

        public DateTime AddedOn { get; set; }

        [DefaultValue(false)]
        public bool IsModerated { get; set; }
        public int? ModeratedBy { get; set; }
        public DateTime? ModeratedOn { get; set; }

        [DefaultValue(false)]
        public bool IsAbused { get; set; }

        public string AbusedBy { get; set; }

        public string AbusedReason { get; set; }

        [ForeignKey("ContentID")]
        public virtual Content Content { get; set; }
    }
}

