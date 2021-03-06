﻿using NITASA.Areas.Admin.Helper;
using NITASA.Data;
using NITASA.Services.Security;
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
        IAclService aclService;
        public UserController(IAclService aclService)
        {
            this.context = new NTSDBContext();
             this.aclService = aclService;
        }

        [HttpGet]
        public ActionResult List(string userName = "")
        {
            //if (!aclService.HasRight(Rights.ViewUsers))
            if (!aclService.HasRight(Rights.ViewUsers))
                return RedirectToAction("AccessDenied", "Home");
            
            List<User> Users = context.User.Where(m => (m.FirstName.Contains(userName) || m.LastName.Contains(userName)) && m.IsDeleted != true).ToList();
            List<Tuple<User, string>> UserList = 
            (from role in Users select new Tuple<User, string>(role, String.Join(",", (from rl in context.Role where role.RoleID == rl.ID select rl.Name).ToArray()))).ToList();
            return View(UserList);
        }

        [HttpGet]
        public ActionResult Add()
        {
            //if (!aclService.HasRight(Rights.CreateNewUsers))
            if (!aclService.HasRight(Rights.CreateNewUsers))
                return RedirectToAction("AccessDenied", "Home");

            ViewBag.RoleList = new SelectList(context.Role.Where(m => m.IsDeleted != true).ToList(), "ID", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Add(User user, HttpPostedFileBase ProfilePic)
        {
            //if (!aclService.HasRight(Rights.CreateNewUsers))
            if (!aclService.HasRight(Rights.CreateNewUsers))
                return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid)
            {
                if (ProfilePic != null && !Functions.IsValidImage(ProfilePic.FileName.ToLower()))
                {
                    TempData["ErrorMessage"] = "Please upload valid profile picture";
                }
                else
                {
                    int duplicateUser = context.User.Where(m => m.Email == user.Email && m.IsDeleted == false).Count();
                    if (duplicateUser == 0)
                    {
                        user.GUID = Functions.GetRandomGUID();
                        user.SaltKey = CryptoUtility.GetNewSalt();
                        user.Password = CryptoUtility.GetPasswordHash(user.Password, user.SaltKey);

                        if (ProfilePic != null && ProfilePic.ContentLength > 0)
                        {
                            string relativePath = Functions.GetNewFileName("/Areas/Admin/assets/images/avatars/", ProfilePic.FileName);
                            ProfilePic.SaveAs(Server.MapPath(relativePath));
                            user.ProfilePicURL = relativePath;
                        }
                        else
                        {
                            user.ProfilePicURL = "/Areas/Admin/assets/images/avatars/noprofile.jpg";
                        }
                        user.AddedOn = DateTime.UtcNow;
                        user.AddedBy = Functions.CurrentUserID();
                        user.IsDefault = false;
                        user.IsDeleted = false;
                        user.IsSuperAdmin = false;
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
                if (Functions.CurrentUserID() == user.ID)
                {
                    //if (!aclService.HasRight(Rights.EditUserSelf))
                    if (!aclService.HasRight(Rights.EditUserSelf))
                        return RedirectToAction("AccessDenied", "Home");
                }
                else
                {
                    //if (!aclService.HasRight(Rights.EditOtherUsers))
                    if (!aclService.HasRight(Rights.EditOtherUsers))
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
            if (Functions.CurrentUserID() == user.ID)
            {
                //if (!aclService.HasRight(Rights.EditUserSelf))
                if (!aclService.HasRight(Rights.EditUserSelf))
                    return RedirectToAction("AccessDenied", "Home");
            }
            else
            {
                //if (!aclService.HasRight(Rights.EditOtherUsers))
                if (!aclService.HasRight(Rights.EditOtherUsers))
                    return RedirectToAction("AccessDenied", "Home");
            }
            if (!user.IsChangePassword)
            {
                ModelState.Remove("Password");
            }
            if (ModelState.IsValid)
            {
                if (ProfilePic != null && !Functions.IsValidImage(ProfilePic.FileName.ToLower()))
                {
                    TempData["ErrorMessage"] = "Please upload valid profile picture";
                }
                else
                {
                    int duplicateUser = context.User.Where(m => m.Email == user.Email && m.ID != user.ID && m.IsDeleted == false).Count();
                    if (duplicateUser == 0)
                    {
                        User userUpdate = context.User.Find(user.ID);

                        userUpdate.Email = user.Email;
                        if (user.IsChangePassword)
                        {
                            userUpdate.SaltKey = CryptoUtility.GetNewSalt();
                            userUpdate.Password = CryptoUtility.GetPasswordHash(user.Password, userUpdate.SaltKey);
                        }
                        userUpdate.FirstName = user.FirstName;
                        userUpdate.LastName = user.LastName;
                        if (ProfilePic != null && ProfilePic.ContentLength > 0)
                        {
                            string relativePath = Functions.GetNewFileName("/Areas/Admin/assets/images/avatars/", ProfilePic.FileName);
                            ProfilePic.SaveAs(Server.MapPath(relativePath));
                            userUpdate.ProfilePicURL = relativePath;
                        }
                        userUpdate.RoleID = user.RoleID;
                        user.ModifiedOn = DateTime.UtcNow;
                        user.ModifiedBy = Functions.CurrentUserID();
                        userUpdate.IsActive = user.IsActive;
                        userUpdate.IsDefault = user.IsDefault;
                        context.SaveChanges();
                        aclService.SetRights(user.ID, userUpdate.RoleID);
                        TempData["SuccessMessage"] = "User updated successfully.";
                        return RedirectToAction("List");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "User already exist with same email address. Please enter different email.";
                    }
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
                int CurrentUserID = Functions.CurrentUserID();
                if (CurrentUserID == user.ID)
                {
                    //if (!aclService.HasRight(Rights.DeleteUserSelf))
                    if (!aclService.HasRight(Rights.DeleteUserSelf))
                        return RedirectToAction("AccessDenied", "Home");
                }
                else
                {
                    //if (!aclService.HasRight(Rights.DeleteOtherUsers))
                    if (!aclService.HasRight(Rights.DeleteOtherUsers))
                        return RedirectToAction("AccessDenied", "Home");
                }
                user.ModifiedOn = DateTime.UtcNow;
                user.ModifiedBy = CurrentUserID;  
                user.IsDeleted = true;
                context.SaveChanges();

                if (Functions.CurrentUserID() == user.ID)
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
                int CurrentUserID = Functions.CurrentUserID();
                if (CurrentUserID == user.ID)
                {
                    //if (!aclService.HasRight(Rights.EditUserSelf))
                    if (!aclService.HasRight(Rights.EditUserSelf))
                        return RedirectToAction("AccessDenied", "Home");
                }
                else
                {
                    //if (!aclService.HasRight(Rights.EditOtherUsers))
                    if (!aclService.HasRight(Rights.EditOtherUsers))
                        return RedirectToAction("AccessDenied", "Home");
                }

                user.IsActive = isActive;
                user.ModifiedOn = DateTime.UtcNow;
                user.ModifiedBy = CurrentUserID;                
                context.SaveChanges();

                if (isActive)
                    aclService.SetActiveUser(user.ID);
                else
                    aclService.RemoveActiveUser(user.ID);

                if(isActive == false && CurrentUserID == user.ID)
                {
                    Session.RemoveAll();
                    TempData["loginmessage"] = "Your Account De-Activated Successfully";
                    return RedirectToAction("Login", "Authenticate");
                }
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
