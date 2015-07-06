using NITASA.Areas.Admin.Helper;
using NITASA.Areas.Admin.ViewModels;
using NITASA.Data;
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
            int userID = Functions.CurrentUserID();
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
            int userID = Functions.CurrentUserID();
            User userEdit = context.User.Find(userID);

            if (usr != null)
            {
                try
                {
                    if (ProfilePicURL != null && !Functions.IsValidImage(ProfilePicURL.FileName.ToLower()))
                    {
                        TempData["Error"] = "Please upload valid profile picture";
                        usr.ProfilePicURL = userEdit.ProfilePicURL;
                    }
                    else
                    {
                        if (ProfilePicURL != null && ProfilePicURL.ContentLength > 0)
                        {
                            string relativePath = Functions.GetNewFileName("/Areas/Admin/assets/images/avatars/", ProfilePicURL.FileName);
                            ProfilePicURL.SaveAs(Server.MapPath(relativePath));
                            userEdit.ProfilePicURL = relativePath;
                            usr.ProfilePicURL = relativePath;
                        }
                        userEdit.FirstName = usr.FirstName;
                        userEdit.LastName = usr.LastName;
                        context.SaveChanges();
                        TempData["Message"] = "User profile updated successfully.";
                    }
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
            if (ModelState.IsValid)
            {
                int currentUserID = Functions.CurrentUserID();
                User userToChange = context.User.Where(m => m.ID == currentUserID).Single();
                if (userToChange == null)
                {
                    TempData["Error"] = "Oops, there seemps to be some problem please try again";
                    return RedirectToAction("ChangePassword");
                }
                else
                {
                    string passwordHash = CryptoUtility.GetPasswordHash(cpassword.currentpassword, userToChange.SaltKey);
                    if (string.Compare(userToChange.Password, passwordHash, false) == 0)
                    {
                        userToChange.SaltKey = CryptoUtility.GetNewSalt();
                        userToChange.Password = CryptoUtility.GetPasswordHash(cpassword.newpassword, userToChange.SaltKey);
                        context.SaveChanges();
                        TempData["Message"] = "Password changed successfully";
                    }
                    else
                    {
                        TempData["Error"] = "Please enter correct current password";
                    }
                }
            }
            return View();
        }
    }
}
