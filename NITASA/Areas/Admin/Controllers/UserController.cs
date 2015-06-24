using NITASA.Areas.Admin.Helper;
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
    public class UserController : Controller
    {
        public NTSDBContext context;

        public UserController()
        {
            this.context = new NTSDBContext();
        }

        [HttpGet]
        public ActionResult List(string userName = "")
        {
            if (!UserRights.HasRights(Rights.ViewUsers))
                return RedirectToAction("AccessDenied", "Home");

            List<User> Users = context.User.Where(m => (m.FirstName.Contains(userName) || m.LastName.Contains(userName)) && m.IsDeleted != true).ToList();
            List<Tuple<User, string>> UserList = 
            (from role in Users select new Tuple<User, string>(role, String.Join(",", (from rl in context.Role where role.RoleID == rl.ID select rl.Name).ToArray()))).ToList();
            return View(UserList);
        }

        [HttpGet]
        public ActionResult Add()
        {
            if (!UserRights.HasRights(Rights.CreateNewUsers))
                return RedirectToAction("AccessDenied", "Home");

            ViewBag.RoleList = new SelectList(context.Role.Where(m => m.IsDeleted != true).ToList(), "ID", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Add(User user, HttpPostedFileBase ProfilePic)
        {
            if (!UserRights.HasRights(Rights.CreateNewUsers))
                return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid)
            {
                int duplicateUser = context.User.Where(m => m.Email == user.Email && m.IsDeleted == false).Count();
                if (duplicateUser == 0)
                {
                    user.GUID = Common.GetRandomGUID();                    
                    user.SaltKey = CryptoUtility.GetNewSalt();
                    user.Password = CryptoUtility.GetPasswordHash(user.Password, user.SaltKey); 

                    if (ProfilePic != null && ProfilePic.ContentLength > 0)
                    {
                        string relativePath = "/Areas/Admin/assets/images/avatars/" + ProfilePic.FileName;
                        string logoFullPath = Server.MapPath(relativePath);
                        ProfilePic.SaveAs(logoFullPath);
                        user.ProfilePicURL = relativePath;
                    }
                    user.AddedOn = DateTime.UtcNow;
                    user.AddedBy = Common.CurrentUserID();
                    user.IsDefault = false;
                    user.IsDeleted = false;
                    context.User.Add(user);
                    context.SaveChanges();
                    TempData["SuccessMessage"] = "User added successfully.";
                    return RedirectToAction("List");
                }
                else
                {
                    TempData["ErrorMessage"] = "User already exist with same email address. Please enter different email.";
                }
            }
            ViewBag.RoleList = new SelectList(context.Role.Where(m => m.IsDeleted != true).ToList(), "ID", "Name");
            return View(user);
        }

        [HttpGet]
        public ActionResult Edit(string GUID)
        {
            User user = context.User.Where(m => m.GUID == GUID && m.IsDeleted == false).FirstOrDefault();
            if (user != null)
            {
                if (Common.CurrentUserID() == user.ID)
                {
                    if (!UserRights.HasRights(Rights.EditUserSelf))
                        return RedirectToAction("AccessDenied", "Home");
                }
                else
                {
                    if (!UserRights.HasRights(Rights.EditOtherUsers))
                        return RedirectToAction("AccessDenied", "Home");
                }


                ViewBag.RoleList = new SelectList(context.Role.Where(m => m.IsDeleted != true).ToList(), "ID", "Name", user.RoleID);
                return View(user);
            }
            else
            {
                TempData["ErrorMessage"] = "User Not Found.";
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public ActionResult Edit(User user, HttpPostedFileBase ProfilePic)
        {
            if (Common.CurrentUserID() == user.ID)
            {
                if (!UserRights.HasRights(Rights.EditUserSelf))
                    return RedirectToAction("AccessDenied", "Home");
            }
            else
            {
                if (!UserRights.HasRights(Rights.EditOtherUsers))
                    return RedirectToAction("AccessDenied", "Home");
            }

            if (ModelState.IsValid)
            {
                int duplicateUser = context.User.Where(m => m.Email == user.Email && m.ID != user.ID && m.IsDeleted == false).Count();
                if (duplicateUser == 0)
                {
                    User userUpdate = context.User.Find(user.ID);

                    userUpdate.Email = user.Email;
                    userUpdate.SaltKey = CryptoUtility.GetNewSalt();
                    userUpdate.Password = CryptoUtility.GetPasswordHash(user.Password, user.SaltKey); 

                    userUpdate.FirstName = user.FirstName;
                    userUpdate.LastName = user.LastName;
                    if (ProfilePic != null && ProfilePic.ContentLength > 0)
                    {
                        string relativePath = "/Areas/Admin/assets/images/avatars/" + ProfilePic.FileName;
                        string logoFullPath = Server.MapPath(relativePath);
                        ProfilePic.SaveAs(logoFullPath);
                        userUpdate.ProfilePicURL = relativePath;
                    }
                    userUpdate.RoleID = user.RoleID;
                    user.ModifiedOn = DateTime.UtcNow;
                    user.ModifiedBy = Common.CurrentUserID();
                    userUpdate.IsActive = user.IsActive;
                    userUpdate.IsDefault = user.IsDefault;
                    context.SaveChanges();
                    TempData["SuccessMessage"] = "User updated successfully.";
                    return RedirectToAction("List");
                }
                else
                {
                    TempData["ErrorMessage"] = "User already exist with same email address. Please enter different email.";
                }
            }
            ViewBag.RoleList = new SelectList(context.Role.Where(m => m.IsDeleted != true).ToList(), "ID", "Name", user.RoleID);
            return View(user);
        }
        [HttpGet]
        public ActionResult Delete(string GUID)
        {
            User user = context.User.Where(m => m.GUID == GUID && m.IsDeleted == false).FirstOrDefault();
            if (user != null)
            {
                if (Common.CurrentUserID() == user.ID)
                {
                    if (!UserRights.HasRights(Rights.DeleteUserSelf))
                        return RedirectToAction("AccessDenied", "Home");
                }
                else
                {
                    if (!UserRights.HasRights(Rights.DeleteOtherUsers))
                        return RedirectToAction("AccessDenied", "Home");
                }

                user.IsDeleted = true;
                context.SaveChanges();

                if (Common.CurrentUserID() == user.ID)
                {
                    Session.RemoveAll();
                    TempData["loginmessage"] = "Your account deleted successfully";
                    return RedirectToAction("Login", "Authenticate");
                }
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "User Not Found.";
            }
            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult changeStatus(string GUID, bool isActive)
        {
            User user = context.User.Where(m => m.GUID == GUID && m.IsDeleted == false).FirstOrDefault();
            if (user != null)
            {
                if (Common.CurrentUserID() == user.ID)
                {
                    if (!UserRights.HasRights(Rights.EditUserSelf))
                        return RedirectToAction("AccessDenied", "Home");
                }
                else
                {
                    if (!UserRights.HasRights(Rights.EditOtherUsers))
                        return RedirectToAction("AccessDenied", "Home");
                }

                user.IsActive = isActive;
                context.SaveChanges();

                TempData["SuccessMessage"] = "User status changed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "User Not Found.";
            }
            return RedirectToAction("List");
        }

    }
}
