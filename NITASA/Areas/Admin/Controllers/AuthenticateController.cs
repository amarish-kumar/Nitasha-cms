﻿using NITASA.Areas.Admin.Helper;
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
        public NTSDBContext context;

        public AuthenticateController()
        {
            this.context = new NTSDBContext();
        }

        public ActionResult Login()
        {
            if (Request.QueryString["retUrl"] != null)
                returnURL = Request.QueryString["retUrl"];

            if (Common.CurrentUserID() > 0)
            {
                if (returnURL == null || returnURL == "" || returnURL.ToLower().Contains("/authenticate/login"))
                {
                    return RedirectToAction("Dashboard", "Home");
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
        public ActionResult Login(Login model)
        {
            if (ModelState.IsValid)
            {
                User User = context.User.Where(m => m.Email == model.EmailAddress && m.IsDeleted == false).FirstOrDefault();
                if (User != null)
                {
                    string passwordHash = CryptoUtility.GetPasswordHash(model.Password, User.SaltKey);
                    if (string.Compare(User.Password, passwordHash, false) == 0)
                    {
                        HttpContext.Session["UserID"] = User.ID;
                        HttpContext.Session["UserRole"] = context.Role.Where(m => m.ID == User.RoleID && m.IsDeleted == false).Select(m => m.Name).FirstOrDefault();
                        UserRights.BindRights();

                        if (Request.QueryString["retUrl"] != null)
                            returnURL = Request.QueryString["retUrl"];

                        if (returnURL == null || returnURL == string.Empty || returnURL.ToLower().Contains("/authenticate/login"))
                        {
                            return RedirectToAction("Dashboard", "Home");
                        }
                        else
                        {
                            return Redirect(returnURL);
                        }
                    }
                }
                TempData["Login_ErrorMsg"] = "Error: Incorrect Username or Password.";
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.RemoveAll();
            TempData["loginmessage"] = "Success: You have succesfully logged out.";
            return RedirectToAction("Login", "Authenticate");
        }

       
    }
}
