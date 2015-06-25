using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.IO.Compression;
using NITASA.Areas.Admin.Helper;

namespace NITASA.Areas.Admin.Controllers
{
    public class BackupRestoreController : Controller
    {
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Add()
        {
            try
            {
                StringBuilder scripFile = new StringBuilder();
                string conString = ConfigurationManager.ConnectionStrings["NITASAConnection"].ConnectionString;
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(conString);
                Server myServer = new Server(builder.DataSource);

                //Using sql server authentication
                myServer.ConnectionContext.LoginSecure = false;
                myServer.ConnectionContext.Login = builder.UserID;
                myServer.ConnectionContext.Password = builder.Password;
                myServer.ConnectionContext.Connect();
                Scripter scripter = new Scripter(myServer);
                Microsoft.SqlServer.Management.Smo.Database nitasaCms = myServer.Databases[builder.InitialCatalog];
                Urn[] DatabaseURNs = new Urn[] { nitasaCms.Urn, };
                StringCollection scriptCollection = scripter.Script(DatabaseURNs);
                foreach (string script in scriptCollection)
                    scripFile.Append(script);
                if (myServer.ConnectionContext.IsOpen)
                    myServer.ConnectionContext.Disconnect();

                string currentTimeStamp = Functions.CurrentTimeStamp();
                string backupPath = Server.MapPath(ConfigurationManager.AppSettings["backupDir"]);
                string sqlScriptFile = backupPath + "temp/" + currentTimeStamp + ".sql";
                using (System.IO.File.Create(sqlScriptFile)) { }
                System.Threading.Thread.Sleep(5000);
                ZipFile.CreateFromDirectory(backupPath + "temp/", backupPath + currentTimeStamp + ".zip", CompressionLevel.Fastest, true);
                System.IO.File.Delete(sqlScriptFile);
                TempData["SuccessMessage"] = "Database backuped successfully.";
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult Delete(string guid)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(guid))
                {
                    string backupPath = Server.MapPath(ConfigurationManager.AppSettings["backupDir"]);
                    string fullFileName = backupPath + guid + ".zip";
                    System.IO.File.Delete(fullFileName);
                    TempData["SuccessMessage"] = "Selected backup deleted successfully.";
                    return RedirectToAction("List");
                }
                else
                {
                    TempData["ErrorMessage"] = "File not found.";
                    return RedirectToAction("List");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("List");
        }
	}
}