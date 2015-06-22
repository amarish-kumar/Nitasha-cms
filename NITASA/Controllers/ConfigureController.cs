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
                            if(SaveConfig(dbName))
                                return RedirectToAction("Users", "Register");                           
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
                    @ViewBag.invalidData = ex.Message;
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
        /*private bool CreateDatabase(string dbName)
        {
            var result = false;
            SqlConnection myConn = new SqlConnection(ConnectionString);
            string sql = "CREATE DATABASE " + dbName + "; ";
            SqlCommand myCommand = new SqlCommand(sql, myConn);
            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();
                result = true;
            }
            catch (System.Exception ex) { }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
            }
            return result;
        }*/
        private bool SaveConfig(string dbName)
        {
            string connectionstring = "";
            if (SaveConnectionStringToConfig(dbName,ref connectionstring ))
            {
                NTSDBContext context = new NTSDBContext("NITASAConnection");
                context.Database.Connection.ConnectionString = connectionstring;
                context.Database.CreateIfNotExists();
                InitializeDatabase(context);
                TempData["message"] = "Configuration file updated successfully. Please enter admin login credentials.";
                return true;
            }
            else
            {
                ViewBag.invalidData = "Error occured while updating configuration file.";
            }
            return false;
        }
        private bool SaveConnectionStringToConfig(string dbname,ref string connectionstring)
        {
            var result = false;
            try
            {
                var configuration = WebConfigurationManager.OpenWebConfiguration("~");
                var section = (ConnectionStringsSection)configuration.GetSection("connectionStrings");
                
                connectionstring = ConnectionString.Replace("master", dbname);
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
                result = true;
            }
            catch { return false; }
            finally { }
            return result;
        }

        protected void InitializeDatabase(NTSDBContext context)
        {
            Role AdminRole = new Role();
            AdminRole.GUID = Guid.NewGuid().ToString().Replace("-", "");
            AdminRole.Name = "Administrator";
            AdminRole.AddedBy = 1;
            AdminRole.AddedOn = DateTime.UtcNow;
            context.Role.Add(AdminRole);
            context.SaveChanges();

            User user = new User();
            user.GUID = Guid.NewGuid().ToString().Replace("-", "");
            user.FirstName = "Admin";
            user.LastName = "DRC";
            user.Email = "admin@drc.com";
            user.Password = "jRXwvDYeMhgzKTpu9G673Q==";
            user.ProfilePicURL = "/Areas/CMSAdmin/assets/images/avatars/noprofile.jpg";
            user.SaltKey = Convert.ToInt32(ConfigurationManager.AppSettings["EncKey"]);
            user.IsActive = true;
            user.IsDefault = true;
            user.AddedOn = DateTime.UtcNow;
            user.AddedBy = 1;
            user.RoleID = AdminRole.ID;
            context.User.Add(user);
            context.SaveChanges();
            int userID = user.ID;

            #region Assign Admin Access Rights
            List<AccessPermission> AllRightsList = UserRights.GetAllAccessPermission();

            /*List<RightsInRole> AdminRightsInRoleList = new List<RightsInRole>();
            AdminRightsInRoleList = (from rid in AllRightsList select new RightsInRole { RightsName = rid.Name, RoleID = AdminRole.RoleID }).ToList();

            context.RightsInRole.AddRange(AdminRightsInRoleList);
            context.SaveChanges();*/
            #endregion

            #region Add Editors Role and Assign Access Rights
            Role EditorsRole = new Role();
            EditorsRole.GUID = Guid.NewGuid().ToString().Replace("-", "");
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

            /*List<ContentTemplate> contentTemplate = new List<ContentTemplate>() 
            {
                new ContentTemplate(){TemplateName="Default",TemplateGUID=Guid.NewGuid().ToString().Replace("-", ""),CreatedBy=userID,CreatedOn=DateTime.UtcNow},
                new ContentTemplate(){TemplateName="Page",TemplateGUID=Guid.NewGuid().ToString().Replace("-", ""),CreatedBy=userID,CreatedOn=DateTime.UtcNow},
                new ContentTemplate(){TemplateName="Testimonial",TemplateGUID=Guid.NewGuid().ToString().Replace("-", ""),CreatedBy=userID,CreatedOn=DateTime.UtcNow},
                new ContentTemplate(){TemplateName="Services",TemplateGUID=Guid.NewGuid().ToString().Replace("-", ""),CreatedBy=userID,CreatedOn=DateTime.UtcNow},
                new ContentTemplate(){TemplateName="Product",TemplateGUID=Guid.NewGuid().ToString().Replace("-", ""),CreatedBy=userID,CreatedOn=DateTime.UtcNow},
                new ContentTemplate(){TemplateName="Project",TemplateGUID=Guid.NewGuid().ToString().Replace("-", ""),CreatedBy=userID,CreatedOn=DateTime.UtcNow},
                new ContentTemplate(){TemplateName="Client",TemplateGUID=Guid.NewGuid().ToString().Replace("-", ""),CreatedBy=userID,CreatedOn=DateTime.UtcNow},
                new ContentTemplate(){TemplateName="PortFolio",TemplateGUID=Guid.NewGuid().ToString().Replace("-", ""),CreatedBy=userID,CreatedOn=DateTime.UtcNow},
                new ContentTemplate(){TemplateName="News",TemplateGUID=Guid.NewGuid().ToString().Replace("-", ""),CreatedBy=userID,CreatedOn=DateTime.UtcNow},
                new ContentTemplate(){TemplateName="FAQ",TemplateGUID=Guid.NewGuid().ToString().Replace("-", ""),CreatedBy=userID,CreatedOn=DateTime.UtcNow}
            };
            context.ContentTemplate.AddRange(contentTemplate);
            context.SaveChanges();*/

            //Add Default Content
            Content content = new Content();
            content.GUID = Guid.NewGuid().ToString().Replace("-", "");
            content.Type = "Post";
            content.Title = "Welcome to Nixon DotNet CMS ";
            content.URL = "Welcome-to-Nixon-DotNet-CMS";
            content.Description = "Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of \"de Finibus Bonorum et Malorum\" (The Extremes of Good and Evil) by Cicero, written in 45 BC. This book is a treatise on the theory of ethics, very popular during the Renaissance. The first line of Lorem Ipsum, \"Lorem ipsum dolor sit amet..\", comes from a line in section 1.10.32.";
            content.AddedBy = userID;
            content.AddedOn = DateTime.UtcNow;
            content.isPublished = true;
            content.PublishedOn = DateTime.UtcNow;
            //content.TemplateID = context.ContentTemplate.Where(m => m.TemplateName == "Default").FirstOrDefault().TemplateID;
            content.IsSlugEdited = true;
            content.IsFeatured = false;
            content.EnableComment = true;
            content.CommentEnabledTill = 180;
            context.Content.Add(content);
            context.SaveChanges();

            // Add default category
            Category conCategory = new Category();
            conCategory.Name = "DotNet";
            conCategory.GUID = Guid.NewGuid().ToString().Replace("-", "");
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

            //Add Gadgets
            Widget gadget = new Widget();
            gadget.WidgetName = "Recent Content";
            gadget.WidgetTitle = "Recent Content";
            gadget.WidgetGUID = Guid.NewGuid().ToString().Replace("-", "");
            gadget.WidgetOption = "{ \"title\": \"Recent Content\",\"count\": 5,\"showthumb\": false }";
            gadget.WidgetOrder = 1;
            gadget.IsActive = true;
            context.Widget.Add(gadget);

            gadget = new Widget();
            gadget.WidgetName = "Most Viewed Content";
            gadget.WidgetTitle = "Most Viewed Content";
            gadget.WidgetGUID = Guid.NewGuid().ToString().Replace("-", "");
            gadget.WidgetOption = "{ \"title\": \"Most Viewed Content\",\"count\": 5,\"showthumb\": false }";
            gadget.WidgetOrder = 2;
            gadget.IsActive = true;
            context.Widget.Add(gadget);

            gadget = new Widget();
            gadget.WidgetName = " Related Content";
            gadget.WidgetTitle = " Related Content";
            gadget.WidgetGUID = Guid.NewGuid().ToString().Replace("-", "");
            gadget.WidgetOption = "{ \"title\": \"Related Content\",\"count\": 3,\"showthumb\": false }";
            gadget.WidgetOrder = 3;
            gadget.IsActive = false;
            context.Widget.Add(gadget);

            gadget = new Widget();
            gadget.WidgetName = "Total Page View";
            gadget.WidgetTitle = "Total Page View";
            gadget.WidgetGUID = Guid.NewGuid().ToString().Replace("-", "");
            gadget.WidgetOption = "{ \"title\": \"Total Page View\",\"count\": 0,\"showthumb\": false }";
            gadget.WidgetOrder = 4;
            gadget.IsActive = false;
            context.Widget.Add(gadget);

            gadget = new Widget();
            gadget.WidgetName = "Category List";
            gadget.WidgetTitle = "Category List";
            gadget.WidgetGUID = Guid.NewGuid().ToString().Replace("-", "");
            gadget.WidgetOption = "{ \"title\": \"Category List\",\"count\": 3,\"showthumb\": false }";
            gadget.WidgetOrder = 5;
            gadget.IsActive = true;
            context.Widget.Add(gadget);

            gadget = new Widget();
            gadget.WidgetName = "Label List";
            gadget.WidgetTitle = "Label List";
            gadget.WidgetGUID = Guid.NewGuid().ToString().Replace("-", "");
            gadget.WidgetOption = "{ \"title\": \"Label List\",\"count\": 3,\"showthumb\": false }";
            gadget.WidgetOrder = 6;
            gadget.IsActive = true;
            context.Widget.Add(gadget);
            context.SaveChanges();
        }
    }
}
