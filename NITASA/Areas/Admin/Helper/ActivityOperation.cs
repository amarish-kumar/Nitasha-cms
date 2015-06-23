using NITASA.Data;
using NITASA.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NITASA.Areas.Admin.Helper
{
    public class ActivityOperation
    {
        public enum LogOperation
        {
            Add,
            Update,
            Delete,
            Search,
            View,
            Active,
            InActive,
            Publish,
            UnPublish
        }

        public static void InsertLog(LogOperation logOp, string title, string description, string pageUrl)
        {
            NTSDBContext db = new NTSDBContext();
            int UserID = Common.CurrentUserID();
            string operation = getOperationType(logOp);

            ActivityLog log = new ActivityLog();
            log.Title = title;
            log.Description = description;
            log.UserID = UserID;
            log.AddedOn = DateTime.Now;
            db.ActivityLog.Add(log);
            db.SaveChanges();
        }

        public static string getOperationType(LogOperation logOp)
        {
            if (logOp == LogOperation.Add)
                return "Add";
            else if (logOp == LogOperation.Update)
                return "Update";
            else if (logOp == LogOperation.Delete)
                return "Delete";
            else if (logOp == LogOperation.Search)
                return "Search";
            else if (logOp == LogOperation.View)
                return "View";
            else if (logOp == LogOperation.Active)
                return "Active";
            else if (logOp == LogOperation.InActive)
                return "InActive";
            else if (logOp == LogOperation.Publish)
                return "Publish";
            else if (logOp == LogOperation.UnPublish)
                return "UnPublish";


            else
                return "";
        }
    }
}