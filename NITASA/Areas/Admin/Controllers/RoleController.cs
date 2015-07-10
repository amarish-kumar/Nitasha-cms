using NITASA.Areas.Admin.Helper;
using NITASA.Core.Caching;
using NITASA.Data;
using NITASA.Services.Security;
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
        List<AccessPermission> AllRightsList;// = UserRights.GetAllAccessPermission();
        IAclService aclService;

        public RoleController(IAclService aclService)
        {
            this.context = new NTSDBContext();
            this.aclService = aclService;
            AllRightsList = aclService.GetAllAccessPermission();
        }

        [HttpGet]
        public ActionResult List(string roleName = "")
        {
            //if (!aclService.HasRight(Rights.ViewRoles))
            if (!aclService.HasRight(Rights.ViewRoles))
                return RedirectToAction("AccessDenied", "Home");
            ViewBag.aclService = aclService;
            List<Role> Roles = context.Role.Where(m => m.Name.Contains(roleName) && m.IsDeleted == false).ToList();
            List<Tuple<Role, int>> RoleList = (from role in Roles
                                               select new Tuple<Role, int>(role, (from ur in context.User where ur.RoleID == role.ID select ur.ID).Count())).ToList();
            return View(RoleList);

        }

        [HttpGet]
        public ActionResult Add()
        {
            //if (!aclService.HasRight(Rights.CreateNewRoles))
            if (!aclService.HasRight(Rights.CreateNewRoles))
                return RedirectToAction("AccessDenied", "Home");
            ViewBag.aclService = aclService;
            return View();
        }

        [HttpPost]
        public ActionResult Add(Role role, FormCollection f)
        {
            //if (!aclService.HasRight(Rights.CreateNewRoles))
            if (!aclService.HasRight(Rights.CreateNewRoles))
                return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid)
            {
                int duplicateRole = context.Role.Where(m => m.Name == role.Name && m.IsDeleted == false).Count();
                if (duplicateRole == 0)
                {
                    Role dbRole = new Role();
                    dbRole.GUID = Functions.GetRandomGUID();
                    dbRole.Name = role.Name;
                    dbRole.AddedOn = DateTime.UtcNow;
                    dbRole.AddedBy = Functions.CurrentUserID();
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
            ViewBag.aclService = aclService;
            return View(role);
        }
        [HttpGet]
        public ActionResult Edit(string GUID)
        {
            Role role = context.Role.Where(m => m.GUID == GUID && m.IsDeleted == false).FirstOrDefault();
            if (role != null)
            {
                if (Functions.CurrentUserID() == role.AddedBy)
                {
                    //if (!aclService.HasRight(Rights.EditOwnRoles))
                    if (!aclService.HasRight(Rights.EditOwnRoles))
                        return RedirectToAction("AccessDenied", "Home");
                }
                else
                {
                    //if (!aclService.HasRight(Rights.EditOtherUsersRoles))
                    if (!aclService.HasRight(Rights.EditOtherUsersRoles))
                        return RedirectToAction("AccessDenied", "Home");
                }
                List<RightsInRole> RightsInRoleList = context.RightsInRole.Where(m => m.RoleID == role.ID).ToList();
                //List<AccessPermission> AllRightsList = UserRights.GetAllAccessPermission();

                AllRightsList = (from r in AllRightsList
                                 select new AccessPermission() { Name = r.Name, Group = r.Group, IsChecked = RightsInRoleList.Exists(rir => rir.RightsName == r.Name) }).ToList();

                ViewBag.AllRightsList = AllRightsList;
                ViewBag.aclService = aclService;
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
            if (Functions.CurrentUserID() == role.AddedBy)
            {
                //if (!aclService.HasRight(Rights.EditOwnRoles))
                if (!aclService.HasRight(Rights.EditOwnRoles))
                    return RedirectToAction("AccessDenied", "Home");
            }
            else
            {
                //if (!aclService.HasRight(Rights.EditOtherUsersRoles))
                if (!aclService.HasRight(Rights.EditOtherUsersRoles))
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
                        dbRole.ModifiedBy = Functions.CurrentUserID();
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

                        var users = context.User.Where(x => x.RoleID == role.ID).ToList();
                        foreach (var user in users)
                            aclService.SetRights(user.ID, role.ID);

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
            ViewBag.aclService = aclService;
            return RedirectToAction("Edit");
        }
        [HttpGet]
        public ActionResult Delete(string GUID)
        {
            //if (!aclService.HasRight(Rights.DeleteRoles))
            if (!aclService.HasRight(Rights.DeleteRoles))
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
