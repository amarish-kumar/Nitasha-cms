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
            if (SaveConnectionStringToConfig(dbName))
            {

                NTSDBContext context = new NTSDBContext("NITASAConnection");
                context.Database.CreateIfNotExists();
                TempData["message"] = "Configuration file updated successfully. Please enter admin login credentials.";
                return true;
            }
            else
            {
                ViewBag.invalidData = "Error occured while updating configuration file.";
            }
            return false;
        }
        private bool SaveConnectionStringToConfig(string dbname)
        {
            var result = false;
            try
            {
                var configuration = WebConfigurationManager.OpenWebConfiguration("~");
                var section = (ConnectionStringsSection)configuration.GetSection("connectionStrings");
                section.ConnectionStrings["NITASAConnection"].ConnectionString = ConnectionString.Replace("master", dbname);
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
    }
}
