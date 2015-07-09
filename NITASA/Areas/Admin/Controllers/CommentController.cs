using NITASA.Areas.Admin.Helper;
using NITASA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    [CheckLogin]
    public class CommentController : Controller    
    {
        public NTSDBContext context;

        public CommentController()
        {
            context = new NTSDBContext();
        }

        public ActionResult List(int page = 1, string sort = "AddedOn", string sortdir = "DESC")
        {
            IQueryable<Comment> comments = context.Comment.Include("Content");
            List<Comment> commentList;
            if (sortdir == "DESC")
            {
                if (sort == "AddedOn")
                    comments = comments.OrderByDescending(comment => comment.AddedOn);
                if (sort == "UserName")
                    comments = comments.OrderByDescending(comment => comment.UserName);
                if (sort == "Content.Title")
                    comments = comments.OrderByDescending(comment => comment.Content.Title);
                if (sort == "IsModerated")
                    comments = comments.OrderByDescending(comment => comment.IsModerated);
            }
            else
            {
                if (sort == "AddedOn")
                    comments = comments.OrderBy(comment => comment.AddedOn);
                if (sort == "UserName")
                    comments = comments.OrderBy(comment => comment.UserName);
                if (sort == "Content.Title")
                    comments = comments.OrderBy(comment => comment.Content.Title);
                if (sort == "IsModerated")
                    comments = comments.OrderBy(comment => comment.IsModerated);
            }
            commentList = comments.Skip((page - 1) * 20).Take(20).ToList();
            ViewBag.TotalRows = context.Comment.Count();
            return View(commentList);
        }

        public ActionResult Abusive(int page = 1, string sort = "AddedOn", string sortdir = "DESC") 
        {
            IQueryable<Comment> comments = context.Comment.Where(x => x.IsAbused == true);
            List<Comment> commentList;
            if (sortdir == "DESC")
            {
                if (sort == "AddedOn")
                    comments = comments.OrderByDescending(comment => comment.AddedOn);
                if (sort == "UserName")
                    comments = comments.OrderByDescending(comment => comment.UserName);
                if (sort == "AbusedBy")
                    comments = comments.OrderByDescending(comment => comment.AbusedBy);
            }
            else
            {
                if (sort == "AddedOn")
                    comments = comments.OrderBy(comment => comment.AddedOn);
                if (sort == "UserName")
                    comments = comments.OrderBy(comment => comment.UserName);
                if (sort == "AbusedBy")
                    comments = comments.OrderBy(comment => comment.AbusedBy);
            }
            commentList = comments.Skip((page - 1) * 20).Take(20).ToList();
            ViewBag.TotalRows = context.Comment.Count();
            return View(commentList);
        }

        public ActionResult ChangeStatus(int id, bool status)
        {
            var comment = context.Comment.Find(id);
            comment.IsModerated = status;
            comment.ModeratedBy = Functions.CurrentUserID();
            comment.ModeratedOn = DateTime.UtcNow;
            context.SaveChanges();
            return Json(comment.IsModerated, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AbusiveAction(int id, string delete, string unabusive)
        {
            var comment = context.Comment.Find(id);
            if (delete == "Delete")
            {
                context.Comment.Remove(comment);
                TempData["AbusiveAction"] = "Comment is deleted.";
            }
            else if (unabusive == "Un-Abused")
            {
                comment.IsAbused = false;
                comment.AbusedBy = null;
                comment.AbusedReason = null;
                TempData["AbusiveAction"] = "Comment is marked as un abusive";
            }
            context.SaveChanges();
            return RedirectToAction("Abusive");
        }
	}
}