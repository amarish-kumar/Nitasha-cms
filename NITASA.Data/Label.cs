using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace NITASA.Data
{
    public class Label
    {
        [Key]
        public int ID { get; set; }
        public string GUID { get; set; }

        [Required(ErrorMessage = "Please enter Label.")]
        [MaxLength(100)]
        [StringLength(100, ErrorMessage = "Label cannot exceed 100 characters.")]
        [RegularExpression(@"^[A-Za-z0-9\ _.#&*+-]*$", ErrorMessage = "Only alpha numeric, space and special characters like '_ , . , # , & , * , + , - '  are allowed")]
        [Display(Name = "Label")]
        [AllowHtml]
        public string Name { get; set; }

        [AllowHtml]
        public string Slug { get; set; }

        [AllowHtml]
        public string Description { get; set; }

        public DateTime AddedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int AddedBy { get; set; }
        public int? ModifiedBy { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}
