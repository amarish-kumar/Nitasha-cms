using NITASA.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NITASA.Areas.Admin.ViewModels
{
    public class CustomizedDashboard
    {
        [Display(Name = "Total Page")]
        public int TotalPage { get; set; }
        public string LastPages { get; set; }

        [Display(Name = "Total Post")]
        public int TotalPost { get; set; }
        public string LastPosts { get; set; }

        [Display(Name = "Category")]
        public int TotalCategory { get; set; }
        public string LastCategories { get; set; }

        [Display(Name = "Media")]
        public int TotalMedia { get; set; }
        public string LastMedia { get; set; }

        [Display(Name = "Drafts")]
        public int TotalDraft { get; set; }
        public string LastDrafts { get; set; }

        [Display(Name = "Users")]
        public int TotalUsers { get; set; }
        public string LastUsers { get; set; }

        [Display(Name = "Roles")]
        public int TotalRoles { get; set; }
        public string LastRoles { get; set; }

        [Display(Name = "Page View")]
        public int TotalPageview { get; set; }
        public string LastPageview { get; set; }

        [Display(Name = "Comments")]
        public int TotalComments { get; set; }
        public string LastComments { get; set; }

        [Display(Name = "New Users")]
        public int TotalNewUsers { get; set; }
        public string LastNewUsers { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Please Enter Title.")]
        // [MaxLength(100)]
        [StringLength(100, ErrorMessage = "Post Title should not be greater than 100 characters long.")]
        public string PostTitle { get; set; }

        public List<Content> PostList { get; set; }
        public List<Content> RecentPublishedPostList { get; set; }
        public List<Content> RecentPages { get; set; }
        public List<Comment> UnModeratedComments { get; set; }
        

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Please Enter Description.")]
        public string PostDescription { get; set; }

        public string chartData { get; set; }



        public int PostPublished { get; set; }
        public int PageDrafted { get; set; }
        public int PagePublished { get; set; }
    }

    public class ChartData
    {
        public string x { get; set; }
        public int y { get; set; }
    }
    class MonthlyData
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Count { get; set; }
    }

}