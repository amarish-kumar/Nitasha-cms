using NITASA.Data;
using NITASA.Helpers;
using NITASA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Controllers
{
    [AllAction]
    public class RegisterController : Controller
    {
        public NTSDBContext context;
        public RegisterController(NTSDBContext context)
        {
            this.context = context;
        }
        public ActionResult Users()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Users(Register adminUser)
        {
            if (ModelState.IsValid)
            {
                if (adminUser.Password == adminUser.ConfirmPassword)
                {
                    if (InsertAdminUser(adminUser.UserEmail, adminUser.Password))
                    {
                        TempData["message"] = ".Net CMS setup successfully.";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        //TempData["message"] = "Error occured while registering admin user.";
                    }
                }
                else
                {
                    TempData["message"] = "Confirm password dosen't match with password.";
                }
            }
            else
            {
                TempData["message"] = "Please enter required field value.";
            }
            return View();
        }

        private bool InsertAdminUser(string userName, string password)
        {
            try
            {
                int saltKey = Common.GetSaltKey();
                string encryptedPassword = Common.Encrypt(password, Convert.ToString(saltKey), true);

                NITASA.Data.User user = new Data.User();
                user.Email = userName;
                user.Password = encryptedPassword;
                user.SaltKey = saltKey;
                user.IsActive = true;
                user.IsDefault = true;
                user.AddedOn = DateTime.Now;
                context.User.Add(user);
                context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
                return false;
            }
        }
	}
}