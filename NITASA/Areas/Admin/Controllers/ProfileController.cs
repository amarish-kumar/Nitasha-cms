using NITASA.Areas.Admin.Helper;
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
    [CheckLogin]
    public class ProfileController : Controller
    {   
        public NTSDBContext context;

        public ProfileController()
        {
            this.context = new NTSDBContext();
        }

        public ActionResult Details()
        {
            int userID = Common.CurrentUserID();
            User usr = new User();
            usr = context.User.Find(userID);
            if (usr == null)
            {
                usr = new User();
            }
            return View(usr);
        }
        [HttpPost]
        public ActionResult Details(User usr, HttpPostedFileBase ProfilePicURL)
        {
            int userID = Common.CurrentUserID();
            User userEdit = context.User.Find(userID);

            if (usr != null)
            {
                try
                {
                    if (ProfilePicURL != null && ProfilePicURL.ContentLength > 0)
                    {
                        string relativePath = "/Areas/Admin/assets/images/avatars/" + ProfilePicURL.FileName;
                        string logoFullPath = Server.MapPath(relativePath);
                        ProfilePicURL.SaveAs(logoFullPath);
                        userEdit.ProfilePicURL = relativePath;
                        usr.ProfilePicURL = relativePath;
                    }
                    else
                    {
                        usr.ProfilePicURL = userEdit.ProfilePicURL;
                    }
                    userEdit.FirstName = usr.FirstName;
                    userEdit.LastName = usr.LastName;
                    context.SaveChanges();
                    TempData["Message"] = "User profile updated successfully.";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Oops there seems to be some problem please try again" + ex.Message;
                }
            }
            return View(usr);
        }
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePassword cpassword)
        {
            string keyToDescript = ConfigurationManager.AppSettings["EncKey"];
            if (string.IsNullOrWhiteSpace(cpassword.currentpassword) || string.IsNullOrWhiteSpace(cpassword.newpassword) || string.IsNullOrWhiteSpace(cpassword.confirmpassword))
            {
                TempData["Error"] = "Please enter all 3 password value to change password";
            }
            else
            {
                if (cpassword.newpassword != cpassword.confirmpassword)
                {
                    TempData["Error"] = "Confirm password does not match with new password";
                }
                else
                {
                    int currentUserID = Common.CurrentUserID();
                    User userToChange = context.User.Where(m => m.ID == currentUserID).Single();
                    if (userToChange == null)
                    {
                        TempData["Error"] = "Oops, there seemps to be some problem please try again";
                        return RedirectToAction("ChangePassword");
                    }
                    else
                    {
                        string password = Common.Decrypt(userToChange.Password, keyToDescript, true);
                        if (password != cpassword.currentpassword)
                        {
                            TempData["Error"] = "Please enter correct current password";
                        }
                        else
                        {
                            userToChange.Password = Common.Encrypt(cpassword.newpassword, keyToDescript, true);
                            context.SaveChanges();
                            TempData["Message"] = "Password changed successfully";
                        }
                    }
                }
            }
            return View();
        }
    }
}
