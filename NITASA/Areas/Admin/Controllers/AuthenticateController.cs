using NITASA.Areas.Admin.ViewModels;
using NITASA.Data;
using NITASA.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    public class AuthenticateController : Controller
    {
        string returnURL;
        private string keyToDescript = ConfigurationManager.AppSettings["EncKey"];

        public NTSDBContext context;
        public AuthenticateController()
        {
            this.context = context;
        }
        public ActionResult Login()
        {
            if (Request.QueryString["retUrl"] != null)
                returnURL = Request.QueryString["retUrl"];

            if (Common.CurrentUserID() > 0)
            {
                if (returnURL == null || returnURL == "" || returnURL.ToLower().Contains("/authentication/login"))
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    return Redirect(returnURL);
                }
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Login(Login login)
        {
            if (ModelState.IsValid)
            {
                string password = Common.Encrypt(login.Password, keyToDescript, true);
                User User = context.User.Where(m => m.Email == login.EmailAddress && m.Password == password && m.IsDeleted == false).FirstOrDefault();
                if (User != null)
                {
                    HttpContext.Session["UserID"] = User.ID;
                    HttpContext.Session["UserRole"] = context.Role.Where(model => model.ID == User.RoleID && model.IsDeleted == false).Select(m => m.Name).FirstOrDefault();
                    //UserRights.BindRights();

                    if (Request.QueryString["retUrl"] != null)
                        returnURL = Request.QueryString["retUrl"];

                    if (returnURL == null || returnURL == string.Empty || returnURL.ToLower().Contains("/authentication/login"))
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {
                        return Redirect(returnURL);
                    }
                }
                else
                {
                    TempData["Login_ErrorMsg"] = "Error: Incorrect Username or Password.";
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.RemoveAll();
            TempData["loginmessage"] = "Success: You have succesfully logged out.";
            return RedirectToAction("Login", "Authentication");
        }
    }
}
