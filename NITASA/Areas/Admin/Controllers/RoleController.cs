using NITASA.Areas.Admin.Helper;
using NITASA.Data;
using NITASA.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    [CheckLogin]
    public class RoleController : Controller
    {
        public NTSDBContext context;
        List<AccessPermission> AllRightsList = UserRights.GetAllAccessPermission();

        public RoleController()
        {
            this.context = new NTSDBContext();
        }

        [HttpGet]
        public ActionResult List(string roleName = "")
        {
            if (!UserRights.HasRights(Rights.ViewRoles))
                return RedirectToAction("AccessDenied", "Home");

            List<Role> Roles = context.Role.Where(m => m.Name.Contains(roleName) && m.IsDeleted == false).ToList();
            List<Tuple<Role, int>> RoleList = (from role in Roles
                                               select new Tuple<Role, int>(role, (from ur in context.User where ur.RoleID == role.ID select ur.ID).Count())).ToList();
            return View(RoleList);

        }

        [HttpGet]
        public ActionResult Add()
        {
            if (!UserRights.HasRights(Rights.CreateNewRoles))
                return RedirectToAction("AccessDenied", "Home");

            return View();

        }

        [HttpPost]
        public ActionResult Add(Role role, FormCollection f)
        {
            if (!UserRights.HasRights(Rights.CreateNewRoles))
                return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid)
            {
                int duplicateRole = context.Role.Where(m => m.Name == role.Name && m.IsDeleted == false).Count();
                if (duplicateRole == 0)
                {
                    Role dbRole = new Role();
                    dbRole.GUID = Common.GetRandomGUID();
                    dbRole.Name = role.Name;
                    dbRole.AddedOn = DateTime.UtcNow;
                    dbRole.AddedBy = Common.CurrentUserID();
                    dbRole.IsDeleted = false;
                    context.Role.Add(dbRole);
                    context.SaveChanges();

                    for (int i = 0; i < AllRightsList.Count; i++)
                    {
                        if (f[AllRightsList[i].Name] != null)
                        {
                            bool isChecked = f[AllRightsList[i].Name].Contains("true");
                            if (isChecked)
                            {
                                RightsInRole RIR = new RightsInRole();
                                RIR.RightsName = AllRightsList[i].Name;
                                RIR.RoleID = dbRole.ID;
                                context.RightsInRole.Add(RIR);
                                context.SaveChanges();
                            }
                        }
                    }

                    TempData["SuccessMessage"] = "Role added successfully.";
                    return RedirectToAction("List");
                }
                else
                {
                    TempData["ErrorMessage"] = "Role is already exist with this name. Please enter different role name.";
                }
            }
            return View(role);
        }
        [HttpGet]
        public ActionResult Edit(string GUID)
        {
            Role role = context.Role.Where(m => m.GUID == GUID && m.IsDeleted == false).FirstOrDefault();
            if (role != null)
            {
                if (Common.CurrentUserID() == role.AddedBy)
                {
                    if (!UserRights.HasRights(Rights.EditOwnRoles))
                        return RedirectToAction("AccessDenied", "Home");
                }
                else
                {
                    if (!UserRights.HasRights(Rights.EditOtherUsersRoles))
                        return RedirectToAction("AccessDenied", "Home");
                }
                List<RightsInRole> RightsInRoleList = context.RightsInRole.Where(m => m.RoleID == role.ID).ToList();
                //List<AccessPermission> AllRightsList = UserRights.GetAllAccessPermission();

                AllRightsList = (from r in AllRightsList
                                 select new AccessPermission() { Name = r.Name, Group = r.Group, IsChecked = RightsInRoleList.Exists(rir => rir.RightsName == r.Name) }).ToList();

                ViewBag.AllRightsList = AllRightsList;

                return View(role);
            }
            else
            {
                TempData["ErrorMessage"] = "Role Not Found.";
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult Edit(Role role, FormCollection f)
        {
            if (Common.CurrentUserID() == role.AddedBy)
            {
                if (!UserRights.HasRights(Rights.EditOwnRoles))
                    return RedirectToAction("AccessDenied", "Home");
            }
            else
            {
                if (!UserRights.HasRights(Rights.EditOtherUsersRoles))
                    return RedirectToAction("AccessDenied", "Home");
            }

            if (ModelState.IsValid)
            {
                int duplicateRole = context.Role.Where(m => m.Name == role.Name && m.ID != role.ID && m.IsDeleted == false).Count();
                if (duplicateRole == 0)
                {
                    Role dbRole = context.Role.Where(m => m.ID == role.ID && m.IsDeleted == false).FirstOrDefault();
                    if (dbRole != null)
                    {
                        dbRole.Name = role.Name;
                        dbRole.ModifiedOn = DateTime.UtcNow;
                        dbRole.ModifiedBy = Common.CurrentUserID();
                        context.SaveChanges();

                        List<RightsInRole> RightsInRoleList = context.RightsInRole.Where(m => m.RoleID == role.ID).ToList();
                        context.RightsInRole.RemoveRange(RightsInRoleList);
                        context.SaveChanges();

                        //List<AccessPermission> AllRightsList = UserRights.GetAllAccessPermission();
                        for (int i = 0; i < AllRightsList.Count; i++)
                        {
                            if (f[AllRightsList[i].Name] != null)
                            {
                                bool isChecked = f[AllRightsList[i].Name].Contains("true");
                                if (isChecked)
                                {
                                    RightsInRole RIR = new RightsInRole();
                                    RIR.RightsName = AllRightsList[i].Name;
                                    RIR.RoleID = dbRole.ID;
                                    context.RightsInRole.Add(RIR);
                                    context.SaveChanges();
                                }
                            }
                        }
                        TempData["SuccessMessage"] = "Role updated successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Oops, there seems to be some problem please try again.";
                    }
                    return RedirectToAction("List");
                }
                else
                {
                    TempData["ErrorMessage"] = "Role is already exist with this name. Please enter different role name.";
                }
            }
            return RedirectToAction("Edit");
        }
        [HttpGet]
        public ActionResult Delete(string GUID)
        {
            if (!UserRights.HasRights(Rights.DeleteRoles))
                return RedirectToAction("AccessDenied", "Home");

            Role role = context.Role.Where(m => m.GUID == GUID).FirstOrDefault();
            if (role != null)
            {

                role.IsDeleted = true;
                context.SaveChanges();

                TempData["SuccessMessage"] = "Role deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Role Not Found.";
            }
            return RedirectToAction("List");
        }
    }
}
