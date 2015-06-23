using NITASA.Data;
using NITASA.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NITASA.Areas.Admin.Helper
{
    public class AccessPermission
    {
        public string Name { get; set; }
        public bool IsChecked { get; set; }
        public string Group { get; set; }
    }
    public enum Rights
    {
        ShowDashboard,
        ManageWidgets,
        ManageSettings,

        AddNewMedias,
        DeleteMedias,

        ViewAllPosts,
        ViewUnPublishedPosts,
        CreateNewPosts,
        EditOwnPosts,
        EditOtherUsersPosts,
        DeleteOwnPosts,
        DeleteOtherUsersPosts,
        PublishOwnPosts,
        PublishOtherUsersPosts,

        ViewAllPages,
        ViewUnPublishedPages,
        CreateNewPages,
        EditOwnPages,
        EditOtherUsersPage,
        DeleteOwnPages,
        DeleteOtherUsersPages,
        PublishOwnPages,
        PublishOtherUsersPages,

        ViewAllAddons,
        ViewUnPublishedAddons,
        CreateNewAddons,
        EditOwnAddons,
        EditOtherUsersAddons,
        DeleteOwnAddons,
        DeleteOtherUsersAddons,
        PublishOwnAddons,
        PublishOtherUsersAddons,

        ViewRoles,
        CreateNewRoles,
        EditOwnRoles,
        EditOtherUsersRoles,
        DeleteRoles,

        ViewUsers,
        CreateNewUsers,
        EditUserSelf,
        EditOtherUsers,
        DeleteUserSelf,
        DeleteOtherUsers
    };

    public static class UserRights
    {
        public static Dictionary<string, string> GetAll()
        {
            Dictionary<string, string> perList = new Dictionary<string, string>();
            perList.Add("ShowDashboard", "General");
            perList.Add("ManageWidgets", "General");
            perList.Add("ManageSettings", "General");

            perList.Add("AddNewMedias", "Medias");
            perList.Add("DeleteMedias", "Medias");

            perList.Add("ViewAllPosts", "Posts");
            perList.Add("ViewUnPublishedPosts", "Posts");
            perList.Add("CreateNewPosts", "Posts");
            perList.Add("EditOwnPosts", "Posts");
            perList.Add("EditOtherUsersPosts", "Posts");
            perList.Add("DeleteOwnPosts", "Posts");
            perList.Add("DeleteOtherUsersPosts", "Posts");
            perList.Add("PublishOwnPosts", "Posts");
            perList.Add("PublishOtherUsersPosts", "Posts");

            perList.Add("ViewAllPages", "Pages");
            perList.Add("ViewUnPublishedPages", "Pages");
            perList.Add("CreateNewPages", "Pages");
            perList.Add("EditOwnPages", "Pages");
            perList.Add("EditOtherUsersPage", "Pages");
            perList.Add("DeleteOwnPages", "Pages");
            perList.Add("DeleteOtherUsersPages", "Pages");
            perList.Add("PublishOwnPages", "Pages");
            perList.Add("PublishOtherUsersPages", "Pages");

            perList.Add("ViewAllAddons", "Addons");
            perList.Add("ViewUnPublishedAddons", "Addons");
            perList.Add("CreateNewAddons", "Addons");
            perList.Add("EditOwnAddons", "Addons");
            perList.Add("EditOtherUsersAddons", "Addons");
            perList.Add("DeleteOwnAddons", "Addons");
            perList.Add("DeleteOtherUsersAddons", "Addons");
            perList.Add("PublishOwnAddons", "Addons");
            perList.Add("PublishOtherUsersAddons", "Addons");

            perList.Add("ViewRoles", "Roles");
            perList.Add("CreateNewRoles", "Roles");
            perList.Add("EditOwnRoles", "Roles");
            perList.Add("EditOtherUsersRoles", "Roles");
            perList.Add("DeleteRoles", "Roles");

            perList.Add("ViewUsers", "Users");
            perList.Add("CreateNewUsers", "Users");
            perList.Add("EditUserSelf", "Users");
            perList.Add("EditOtherUsers", "Users");
            perList.Add("DeleteUserSelf", "Users");
            perList.Add("DeleteOtherUsers", "Users");

            return perList;
        }

        public static List<AccessPermission> GetAllAccessPermission()
        {
            Dictionary<string, string> perList = GetAll();
            List<AccessPermission> RightsList =
             (from r in perList select new AccessPermission() { Name = r.Key, Group = r.Value, IsChecked = false }).ToList();

            return RightsList;
        }

        private static List<string> RightsList
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.Session["SystemAccessRightsList"] != null)
                    return (List<string>)HttpContext.Current.Session["SystemAccessRightsList"];
                else
                    return new List<string>();
            }
            set
            {
                HttpContext.Current.Session["SystemAccessRightsList"] = value;
            }
        }

        public static bool HasRights(Rights value)
        {
            if (Common.CurrentUserRole() == "Administrator")
                return true;

            string rightsName = value.ToString("G");

            if (RightsList.Contains(rightsName))
                return true;
            else
                return false;
        }

        public static void BindRights()
        {
            //CMSDBContext db = (CMSDBContext)DependencyResolver.Current.GetService(typeof(CMSDBContext));
            NTSDBContext db = new NTSDBContext();

            int UserID = Common.CurrentUserID();
            RightsList = new List<string>();
            RightsList = (from rir in db.RightsInRole
                          join usr in db.User on rir.RoleID equals usr.RoleID
                          where usr.ID == UserID
                          select rir.RightsName).ToList();
        }
    }
}