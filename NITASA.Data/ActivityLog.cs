using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NITASA.Data
{
    public class ActivityLog
    {
        [Key]
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime AddedOn { get; set; }

        public string ActionType { get; set; }
        public string PageUrl { get; set; }
        public string RemoteAddress { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
    }
}