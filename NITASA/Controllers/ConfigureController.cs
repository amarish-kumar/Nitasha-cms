using NITASA.Areas.Admin.Helper;
using NITASA.Data;
using NITASA.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace NITASA.Controllers
{
    public class ConfigureController : Controller
    {
        string ConnectionString = "Integrated Security=False;Initial Catalog=master;Data Source=#DSOURCE;User Id=#USERID;Password=#PASSWORD;";

        public ActionResult Install()
        {
            if (IsSetup())
            {
                TempData["message"] = "It's look like you have already configured site earlier, Please browse your site.";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        private bool IsSetup()
        {
            var result = false;
            try
            {
                var configuration = WebConfigurationManager.OpenWebConfiguration("~");
                KeyValueConfigurationCollection kvConCollection = configuration.AppSettings.Settings;
                string[] allKeys = configuration.AppSettings.Settings.AllKeys;
                if (allKeys.Contains("Installed"))
                {
                    result = true;
                }
            }
            catch { result = false; }
            finally { }
            return result;
        }

        [HttpPost]
        public ActionResult Install(Install install)
        {
            string dbName = install.DatabaseName;
            if (ModelState.IsValid)
            {
                //Set connection string
                ConnectionString = ConnectionString.Replace("#DSOURCE", install.DatabaseHost).Replace("#USERID", install.Username).Replace("#PASSWORD", install.Password);

                if (IsCheckServerExists(install.DatabaseHost, 1433))
                {
                    if (!IsDatabaseExists(dbName))
                    {
                        //if (CreateDatabase(dbName))
                        //{
                        //    return SaveConfig(dbName);
                        //}
                        if (SaveConfig(dbName))
                        {
                            return RedirectToAction("Users", "Register");
                        }
                        else
                        {
                            ViewBag.invalidData = "Error occured while creating database.";
                        }
                    }
                    else
                    {
                        if (install.IsDBInstalled != "true")
                        {
                            ViewBag.invalidData = "Database exist with same name, please enter different database.";
                            ViewBag.confirmRequired = true;
                        }
                        else
                        {
                            ViewBag.confirmRequired = false;
                            if (SaveConfig(dbName))
                            {
                                Request.RequestContext.HttpContext.Application["CurrentTheme"] = "Default";
                                return RedirectToAction("Users", "Register");
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.invalidData = "No database server exist, please enter valid sql server address";
                }
            }
            else
            {
                ViewBag.invalidData = "Please enter required field value.";
            }
            return View();
        }

        private bool IsCheckServerExists(string address, int port)
        {
            int timeout = 500;
            if (ConfigurationManager.AppSettings["RemoteTestTimeout"] != null)
                timeout = int.Parse(ConfigurationManager.AppSettings["RemoteTestTimeout"]);
            var result = false;
            try
            {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    IAsyncResult asyncResult = socket.BeginConnect(address, port, null, null);
                    result = asyncResult.AsyncWaitHandle.WaitOne(timeout, true);
                    socket.Close();
                }
                return result;
            }
            catch { return false; }
        }
        private bool IsDatabaseExists(string dbname)
        {
            var result = false;
            try
            {
                SqlDataReader reader = null;
                SqlConnection conn = null;
                SqlCommand cmd = null;

                string sql = "SELECT name FROM master.dbo.sysdatabases WHERE ('[' + name + ']' = @dbname OR name = @dbname)";

                List<SqlParameter> sqlParam = new List<SqlParameter>();

                sqlParam.Add(new SqlParameter("@dbname", dbname));


                conn = new SqlConnection(ConnectionString);
                conn.Open();
                cmd = new SqlCommand(sql, conn);
                try
                {
                    cmd.Parameters.AddRange(sqlParam.ToArray());
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        result = true;
                    }
                }
                catch (SqlException ex)
                {
                    ViewBag.invalidData = ex.Message;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
                return result;
            }
            catch { return false; }
        }
        private bool SaveConfig(string dbName)
        {
            string connectionstring = "";
            try
            {
                var configuration = WebConfigurationManager.OpenWebConfiguration("~");
                var section = (ConnectionStringsSection)configuration.GetSection("connectionStrings");
                connectionstring = ConnectionString.Replace("master", dbName);

                NTSDBContext context = new NTSDBContext("NITASAConnection");
                context.Database.Connection.ConnectionString = connectionstring;
                context.Database.CreateIfNotExists();
                InitializeDatabase(context);

                section.ConnectionStrings["NITASAConnection"].ConnectionString = connectionstring;
                string[] allKeys = configuration.AppSettings.Settings.AllKeys;
                if (allKeys.Contains("Installed"))
                {
                    configuration.AppSettings.Settings["Installed"].Value = "true";
                }
                else
                {
                    KeyValueConfigurationElement kv = new KeyValueConfigurationElement("Installed", "true");
                    configuration.AppSettings.Settings.Add(kv);
                }

                configuration.Save();
                TempData["message"] = "Configuration file updated successfully. Please enter admin login credentials.";
                return true;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                var fullErrorMessage = string.Join("; ", errorMessages);
                ViewBag.invalidData = "Error occured while updating configuration file.";
                return false;
            }
        }
        protected void InitializeDatabase(NTSDBContext context)
        {
            Role AdminRole = new Role();
            AdminRole.GUID = Functions.GetRandomGUID();
            AdminRole.Name = "Administrator";
            AdminRole.AddedBy = 1;
            AdminRole.AddedOn = DateTime.UtcNow;
            context.Role.Add(AdminRole);
            context.SaveChanges();

            User user = new User();
            user.GUID = Functions.GetRandomGUID();
            user.FirstName = "Super";
            user.LastName = "Admin";
            user.Email = "admin@nitasa.com";
            user.SaltKey = CryptoUtility.GetNewSalt();
            string DefaultPassword = "admin@1234";
            user.Password = CryptoUtility.GetPasswordHash(DefaultPassword, user.SaltKey);
            user.ProfilePicURL = "/Areas/Admin/assets/images/avatars/noprofile.jpg";
            user.IsActive = true;
            user.IsDefault = true;
            user.AddedOn = DateTime.UtcNow;
            user.AddedBy = 1;
            user.RoleID = AdminRole.ID;
            user.IsSuperAdmin = true;
            context.User.Add(user);
            context.SaveChanges();
            int userID = user.ID;

            List<AccessPermission> AllRightsList = UserRights.GetAllAccessPermission();
            #region Assign Admin Access Rights
            /*List<RightsInRole> AdminRightsInRoleList = new List<RightsInRole>();
            AdminRightsInRoleList = (from rid in AllRightsList select new RightsInRole { RightsName = rid.Name, RoleID = AdminRole.RoleID }).ToList();

            context.RightsInRole.AddRange(AdminRightsInRoleList);
            context.SaveChanges();*/
            #endregion

            #region Add Editors Role and Assign Access Rights
            Role EditorsRole = new Role();
            EditorsRole.GUID = Functions.GetRandomGUID();
            EditorsRole.Name = "Editors";
            EditorsRole.AddedBy = userID;
            EditorsRole.AddedOn = DateTime.UtcNow;
            context.Role.Add(EditorsRole);
            context.SaveChanges();
            string[] EditorsRights = new string[]{"ShowDashboard","AddNewMedias","DeleteMedias",
                "ViewAllPosts","ViewAllPages","ViewRoles","ViewUsers"};

            List<RightsInRole> EditorsRightsInRoleList = new List<RightsInRole>();
            EditorsRightsInRoleList = (from rid in AllRightsList.Where(m => EditorsRights.Contains(m.Name))
                                       select new RightsInRole { RightsName = rid.Name, RoleID = EditorsRole.ID }).ToList();
            context.RightsInRole.AddRange(EditorsRightsInRoleList);
            context.SaveChanges();
            #endregion

            /*Config config = new Config();
            config.FacebookPageName = "netcms";
            config.FacebookUserName = "netcms";
            config.GPlusPageName = "netcms";
            config.GPlusUsername = "netcms";
            config.LogoPath = "/content/logo.png";
            config.PintrestUserName = "netcms";
            config.ShowNoOfLastPost = 5;
            config.SiteName = "NITASA CMS";
            config.SiteTitle = "NITASA CMS";
            config.TwitterUsername = "netcms";
            context.Config.Add(config);
            context.SaveChanges();*/

            //Add Default Content
            Content content = new Content();
            content.GUID = Functions.GetRandomGUID();
            content.Type = "post";
            content.Title = "Welcome to Nixon DotNet CMS ";
            content.URL = "Welcome-to-Nixon-DotNet-CMS";
            content.Description = "Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of \"de Finibus Bonorum et Malorum\" (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, \"Lorem ipsum dolor sit amet..\", comes from a line in section 1.10.32.";
            content.AddedBy = userID;
            content.AddedOn = DateTime.UtcNow;
            content.isPublished = true;
            content.PublishedOn = DateTime.UtcNow;
            content.IsSlugEdited = true;
            content.IsFeatured = false;
            content.EnableComment = true;
            content.CommentEnabledTill = 180;
            context.Content.Add(content);
            context.SaveChanges();

            Content page = new Content();
            page.GUID = Functions.GetRandomGUID();
            page.Type = "page";
            page.Title = "Index";
            page.URL = "";
            page.Description = "Home page";
            page.AddedBy = userID;
            page.AddedOn = DateTime.UtcNow;
            page.isPublished = true;
            page.PublishedOn = DateTime.UtcNow;
            page.IsSlugEdited = false;
            page.IsFeatured = false;
            page.EnableComment = false;
            context.Content.Add(page);
            context.SaveChanges();

            // Add default category
            Category conCategory = new Category();
            conCategory.Name = "DotNet";
            conCategory.GUID = Functions.GetRandomGUID();
            conCategory.Slug = "dotnet";
            conCategory.ParentCategoryID = 0;
            conCategory.AddedBy = userID;
            conCategory.AddedOn = DateTime.UtcNow;
            context.Category.Add(conCategory);
            context.SaveChanges();

            ContentCategory contentCategory = new ContentCategory();
            contentCategory.CategoryID = conCategory.ID;
            contentCategory.ContentID = content.ID;
            contentCategory.AddedBy = userID;
            contentCategory.AddedOn = DateTime.UtcNow;
            context.ContentCategory.Add(contentCategory);
            context.SaveChanges();

            //Add Default Meta
            Meta meta = new Meta();
            meta.ContentID = content.ID;
            meta.Keyword = "Lorem, Ipsum,dummy,industry";
            meta.Description = "Contrary to popular belief, Lorem Ipsum is not simply random text.";
            meta.Author = "Tarun Dudhatra";
            meta.CreatedOn = DateTime.UtcNow;
            context.Meta.Add(meta);
            context.SaveChanges();

            //Add Widget
            Widget gadget = new Widget();
            gadget.Name = "Recent Content";
            gadget.Title = "Recent Content";
            gadget.GUID = Functions.GetRandomGUID();
            gadget.Option = "{ \"title\": \"Recent Content\",\"count\": 5,\"showthumb\": false }";
            gadget.DisplayOrder = 1;
            gadget.IsActive = true;
            context.Widget.Add(gadget);

            gadget = new Widget();
            gadget.Name = "Most Viewed Content";
            gadget.Title = "Most Viewed Content";
            gadget.GUID = Functions.GetRandomGUID();
            gadget.Option = "{ \"title\": \"Most Viewed Content\",\"count\": 5,\"showthumb\": false }";
            gadget.DisplayOrder = 2;
            gadget.IsActive = true;
            context.Widget.Add(gadget);

            gadget = new Widget();
            gadget.Name = " Related Content";
            gadget.Title = " Related Content";
            gadget.GUID = Functions.GetRandomGUID();
            gadget.Option = "{ \"title\": \"Related Content\",\"count\": 3,\"showthumb\": false }";
            gadget.DisplayOrder = 3;
            gadget.IsActive = false;
            context.Widget.Add(gadget);

            gadget = new Widget();
            gadget.Name = "Total Page View";
            gadget.Title = "Total Page View";
            gadget.GUID = Functions.GetRandomGUID();
            gadget.Option = "{ \"title\": \"Total Page View\",\"count\": 0,\"showthumb\": false }";
            gadget.DisplayOrder = 4;
            gadget.IsActive = false;
            context.Widget.Add(gadget);

            gadget = new Widget();
            gadget.Name = "Category List";
            gadget.Title = "Category List";
            gadget.GUID = Functions.GetRandomGUID();
            gadget.Option = "{ \"title\": \"Category List\",\"count\": 3,\"showthumb\": false }";
            gadget.DisplayOrder = 5;
            gadget.IsActive = true;
            context.Widget.Add(gadget);

            gadget = new Widget();
            gadget.Name = "Label List";
            gadget.Title = "Label List";
            gadget.GUID = Functions.GetRandomGUID();
            gadget.Option = "{ \"title\": \"Label List\",\"count\": 3,\"showthumb\": false }";
            gadget.DisplayOrder = 6;
            gadget.IsActive = true;
            context.Widget.Add(gadget);
            context.SaveChanges();

            gadget = new Widget();
            gadget.Name = "Recent Comments";
            gadget.Title = "Recent Comments";
            gadget.GUID = Functions.GetRandomGUID();
            gadget.Option = "{ \"title\": \"Recent Comments\",\"count\": 3,\"showthumb\": false }";
            gadget.DisplayOrder = 6;
            gadget.IsActive = true;
            context.Widget.Add(gadget);
            context.SaveChanges();

            gadget = new Widget();
            gadget.Name = "Page List";
            gadget.Title = "Page List";
            gadget.GUID = Functions.GetRandomGUID();
            gadget.Option = "{ \"title\": \"Page List\",\"count\": 3,\"showthumb\": false }";
            gadget.DisplayOrder = 6;
            gadget.IsActive = true;
            context.Widget.Add(gadget);
            context.SaveChanges();

        }
    }
}
