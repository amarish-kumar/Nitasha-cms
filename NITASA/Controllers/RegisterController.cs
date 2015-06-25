using NITASA.Areas.Admin.Helper;
using NITASA.Data;
using NITASA.Helpers;
using NITASA.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Controllers
{
    [AllAction]
    public class RegisterController : Controller
    {
        public NTSDBContext context;
        public RegisterController()
        {
            this.context = new NTSDBContext();
        }
        public ActionResult Users()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Users(Register model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password == model.ConfirmPassword)
                {
                    try
                    {
                        NITASA.Data.User user = new Data.User();
                        user.GUID = Functions.GetRandomGUID();
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        user.Email = model.UserEmail;
                        user.SaltKey = CryptoUtility.GetNewSalt();
                        user.Password = CryptoUtility.GetPasswordHash(model.Password, user.SaltKey);
                        user.ProfilePicURL = "/Areas/Admin/assets/images/avatars/noprofile.jpg";
                        user.IsActive = true;
                        user.IsDefault = true;
                        user.AddedOn = DateTime.Now;
                        user.RoleID = 2;
                        context.User.Add(user);
                        context.SaveChanges();

                        TempData["message"] = "NITASA CMS setup successfully.";
                        return RedirectToAction("Index", "Home");
                    }
                    catch (DbEntityValidationException ex)
                    {
                        var errorMessages = ex.EntityValidationErrors
                                .SelectMany(x => x.ValidationErrors)
                                .Select(x => x.ErrorMessage);

                        var fullErrorMessage = string.Join("; ", errorMessages);

                        var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                        TempData["message"] = ex.Message;
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
	}
}