using NITASA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NITASA.Services.Caching;

namespace NITASA.Services.Security
{
    public class AclService : IAclService
    {
        ICacheManager cacheManager;
        NTSDBContext context;

        public AclService(ICacheManager cacheManager, NTSDBContext context)
        {
            this.cacheManager = cacheManager;
            this.context = context;
        }

        public Dictionary<string, string> GetAll()
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

        public List<AccessPermission> GetAllAccessPermission()
        {
            Dictionary<string, string> perList = GetAll();
            List<AccessPermission> RightsList =
             (from r in perList select new AccessPermission() { Name = r.Key, Group = r.Value, IsChecked = false }).ToList();

            return RightsList;
        }

        public List<string> GetRights(int userId)
        {
            if (cacheManager.IsSet(userId.ToString()))
                return cacheManager.Get<List<string>>(userId.ToString());
            return new List<string>();
        }

        public bool HasRight(Rights right)
        {
            int currentUser = CurrentUserID();

            if (CurrentUserRole() == "Administrator")
                return true;

            var temp = cacheManager.Get<List<string>>(currentUser.ToString());
            string rightName = right.ToString("G");
            if (temp.Contains(rightName))
                return true;
            return false;
        }

        public void SetRights(int UserID, int RoleId)
        {
            var rights = (from d in context.RightsInRole
                          where d.RoleID == RoleId
                          select d.RightsName).ToList();

            cacheManager.Set(UserID.ToString(), rights, 60);
        }

        public int CurrentUserID()
        {
            int userID = 0;
            if (HttpContext.Current.Session["UserID"] != null)
            {
                try
                {
                    userID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                }
                catch { }
            }
            return userID;
        }

        public string CurrentUserRole()
        {
            string userRoll = string.Empty;
            if (HttpContext.Current.Session["UserRole"] != null)
            {
                try
                {
                    userRoll = Convert.ToString(HttpContext.Current.Session["UserRole"]);
                }
                catch { }
            }
            return userRoll;
        }
    }

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
        DeleteOtherUsers,

        ManageTemplates
    }
}