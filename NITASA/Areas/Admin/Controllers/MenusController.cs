using Newtonsoft.Json.Linq;
using NITASA.Areas.Admin.Helper;
using NITASA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NITASA.Areas.Admin.Controllers
{
    [CheckLogin]
    public class MenusController : Controller
    {
        private NTSDBContext context;
        public MenusController()
        {
            this.context = new NTSDBContext();
        }

        [HttpGet]
        public ActionResult List()
        {
            List<Category> categoryList = context.Category.Where(m => m.IsDeleted == false).OrderBy(m => m.Name).ToList();
            ViewBag.categoryList = categoryList;
            List<Label> labelList = context.Label.Where(m => m.IsDeleted == false).OrderBy(m => m.Name).ToList();
            ViewBag.labelList = labelList;
            List<Content> pageList = context.Content.Where(m => m.Type == "page" && m.IsDeleted == false).OrderBy(m => m.Title).ToList();
            ViewBag.pageList = pageList;

            ViewBag.strMenu = GetMenu();

            return View();
        }

        private string GetMenu()
        {
            string strMenu = string.Empty;
            List<Menu> AllMenuList = context.Menu.Where(m => m.IsDeleted == false).ToList();
            List<Menu> AllMenuListSlug =
                (from m in AllMenuList
                 select new Menu()
                 {
                     ID = m.ID,
                     GUID = m.GUID,
                     ParentMenuID = m.ParentMenuID,
                     Title = m.Title,
                     Type = m.Type,
                     RefID = m.RefID,
                     DisplayOrder = m.DisplayOrder,
                     AddedOn = m.AddedOn,
                     AddedBy = m.AddedBy,
                     Slug = (m.Type == "Link" ? m.Slug :
                               (m.Type == "Category" ? context.Category.Where(c => c.ID == m.RefID).Select(s => s.Slug).FirstOrDefault() :
                               (m.Type == "Label" ? context.Label.Where(c => c.ID == m.RefID).Select(s => s.Slug).FirstOrDefault() :
                               (m.Type == "Page" ? context.Content.Where(c => c.ID == m.RefID).Select(s => s.URL).FirstOrDefault() : "")
                               )))
                 }
                ).ToList();

            List<Menu> MenuList1 = AllMenuListSlug.Where(m => m.ParentMenuID == 0 && m.IsDeleted == false).ToList();
            foreach (Menu menu1 in MenuList1)// Level 1
            {
                string root1 = GetMenuHTML(menu1.Title, menu1.Type, menu1.RefID, menu1.Slug, menu1.DisplayOrder);
                strMenu += root1;

                List<Menu> MenuList2 = AllMenuListSlug.Where(m => m.ParentMenuID == menu1.ID && m.ParentMenuID != 0 && m.IsDeleted == false).ToList();
                if (MenuList2.Count() > 0)
                {
                    strMenu += StartParentMenuHTML();
                }
                foreach (Menu menu2 in MenuList2)// Level 2
                {
                    string root2 = GetMenuHTML(menu2.Title, menu2.Type, menu2.RefID, menu2.Slug, menu2.DisplayOrder);
                    strMenu += root2;

                    List<Menu> MenuList3 = AllMenuListSlug.Where(m => m.ParentMenuID == menu2.ID && m.ParentMenuID != 0 && m.IsDeleted == false).ToList();
                    if (MenuList3.Count() > 0)
                    {
                        strMenu += StartParentMenuHTML();
                    }
                    foreach (Menu menu3 in MenuList3)// Level 3
                    {
                        string root3 = GetMenuHTML(menu3.Title, menu3.Type, menu3.RefID, menu3.Slug, menu3.DisplayOrder);
                        strMenu += root3;

                        List<Menu> MenuList4 = AllMenuListSlug.Where(m => m.ParentMenuID == menu3.ID && m.ParentMenuID != 0 && m.IsDeleted == false).ToList();
                        if (MenuList4.Count() > 0)
                        {
                            strMenu += StartParentMenuHTML();
                        }
                        foreach (Menu menu4 in MenuList4)// Level 4
                        {
                            string root4 = GetMenuHTML(menu4.Title, menu4.Type, menu4.RefID, menu4.Slug, menu4.DisplayOrder);
                            strMenu += root3;

                            List<Menu> MenuList5 = AllMenuListSlug.Where(m => m.ParentMenuID == menu4.ID && m.ParentMenuID != 0 && m.IsDeleted == false).ToList();
                            if (MenuList5.Count() > 0)
                            {
                                strMenu += StartParentMenuHTML();
                            }
                            foreach (Menu menu5 in MenuList5)// Level 5
                            {
                                string root5 = GetMenuHTML(menu5.Title, menu5.Type, menu5.RefID, menu5.Slug, menu5.DisplayOrder);
                                strMenu += root3;
                            }
                            strMenu += EndParentMenuHTML(MenuList5.Count() > 0);
                        }
                        strMenu += EndParentMenuHTML(MenuList4.Count() > 0);
                    }
                    strMenu += EndParentMenuHTML(MenuList3.Count() > 0);
                }
                strMenu += EndParentMenuHTML(MenuList2.Count() > 0);
            }
            return strMenu;
        }
        private string StartParentMenuHTML()
        {
            return "<ol class=\"dd-list\">";
        }
        private string GetMenuHTML(string MenuTitle, string MenuType, int RefID, string MenuSlug, int DisplayOrder)
        {
            string strSlug = string.Empty;
            strSlug = MenuType == "Link" ? MenuSlug : getURLSlug(MenuType, MenuSlug);

            string strMenu = "<li class=\"dd-item dd3-item\"  data-type=\"" + MenuType + "\" data-slug=\"" + strSlug + "\" data-title=\"" + MenuTitle + "\"  data-id=\"" + RefID + "\" data-autoid=\"" + DisplayOrder + "\" >" +
                                    "<div class=\"dd-handle dd3-handle\"></div>" +
                                    "<div class=\"dd3-content\">" +
                                        "<div class=\"showSetting\">" +
                                            "<span class=\"menutitle\">" + MenuTitle + "</span>" +
                                            "<span class=\"menutype\">" + MenuType + "</span>" +
                                        "</div>" +
                                    "</div>" +
                                    "<div class=\"menu-item-settings\">" +
                                        "<p>Navigation Title<br>" +
                                            "<input type=\"text\" value=\"" + MenuTitle.Trim() + "\" class=\"itemtitle\" maxlength=\"40\" />" +
                                        "</p>" +
                                        "<p>Navigation URL <br><a target=\"_blank\" href=\"" + strSlug + "\">" + strSlug + "</a></p>" +
                                        "<a class=\"item-delete\">Remove</a> | <a class=\"item-cancel\">Cancel</a>" +
                                    "</div>";
            return strMenu;
        }
        private string getURLSlug(string type, string slug)
        {
            if (type == "Page")
                type = "Content";

            string strURL = Request.Url.AbsoluteUri.ToString();
            strURL = strURL.Remove(strURL.IndexOf("Admin"));
            return strURL + type + "/Details/" + slug;
        }
        private string EndParentMenuHTML(bool hasParent)
        {
            if (hasParent)
                return "</ol></li>";
            else
                return "</li>";
        }

        [HttpPost]
        public JsonResult AddMenu(string jMenu)
        {
            List<Menu> menuList = context.Menu.Where(m => m.IsDeleted == false).ToList();
            foreach (Menu menu in menuList)
            {
                menu.IsDeleted = true;
            }
            context.SaveChanges();

            int autoid = 1;
            string GUID = Functions.GetRandomGUID();
            DateTime CreatedOn = DateTime.UtcNow;

            var root1 = JArray.Parse(jMenu);     // Level 1
            foreach (JObject obj1 in root1)
            {
                int Refid1 = (int)obj1["id"];
                string title1 = (string)obj1["title"];
                string slug1 = (string)obj1["slug"];
                string type1 = (string)obj1["type"];
                // save
                int menuID1 = SaveMenu(GUID, 0, title1, type1, Refid1, slug1, autoid, CreatedOn);
                // End
                autoid++;
                if (obj1["children"] != null)
                {
                    var root2 = JArray.Parse(Convert.ToString(obj1["children"]));       // Level 2
                    foreach (JObject obj2 in root2)
                    {
                        int Refid2 = (int)obj2["id"];
                        string title2 = (string)obj2["title"];
                        string slug2 = (string)obj2["slug"];
                        string type2 = (string)obj2["type"];
                        // save
                        int menuID2 = SaveMenu(GUID, menuID1, title2, type2, Refid2, slug2, autoid, CreatedOn);
                        // End
                        autoid++;
                        if (obj2["children"] != null)
                        {
                            var root3 = JArray.Parse(Convert.ToString(obj2["children"]));     // Level 3
                            foreach (JObject obj3 in root3)
                            {
                                int Refid3 = (int)obj3["id"];
                                string title3 = (string)obj3["title"];
                                string slug3 = (string)obj3["slug"];
                                string type3 = (string)obj3["type"];
                                // save
                                int menuID3 = SaveMenu(GUID, menuID2, title3, type3, Refid3, slug3, autoid, CreatedOn);
                                // End
                                autoid++;
                                if (obj3["children"] != null)
                                {
                                    var root4 = JArray.Parse(Convert.ToString(obj3["children"]));     // Level 4
                                    foreach (JObject obj4 in root4)
                                    {
                                        int Refid4 = (int)obj4["id"];
                                        string title4 = (string)obj4["title"];
                                        string slug4 = (string)obj4["slug"];
                                        string type4 = (string)obj4["type"];
                                        Refid4 = (type4 == "Link" ? 0 : Refid4);
                                        // save
                                        int menuID4 = SaveMenu(GUID, menuID3, title4, type4, Refid4, slug4, autoid, CreatedOn);
                                        // End
                                        autoid++;
                                        if (obj4["children"] != null)
                                        {
                                            var root5 = JArray.Parse(Convert.ToString(obj4["children"]));     // Level 5
                                            foreach (JObject obj5 in root5)
                                            {
                                                int Refid5 = (int)obj5["id"];
                                                string title5 = (string)obj5["title"];
                                                string slug5 = (string)obj5["slug"];
                                                string type5 = (string)obj5["type"];
                                                // save
                                                int menuID5 = SaveMenu(GUID, menuID4, title5, type5, Refid5, slug5, autoid, CreatedOn);
                                                // End
                                                autoid++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //if (autoid > 1)
            //{
            //    TempData["MenuMessage"] = "Menu saved successfully";
            //}
            //else
            //    TempData["MenuError"] = "oops, there seems to be some problem please try again.";
            TempData["MenuMessage"] = "Menu saved successfully";
            return Json("");
        }

        private int SaveMenu(string MenuGUID, int ParentMenuID, string MenuTitle, string MenuType, int RefID, string MenuSlug, int DisplayOrder, DateTime CreatedOn)
        {
            NITASA.Data.Menu menu = new Data.Menu();
            menu.GUID = MenuGUID;
            menu.ParentMenuID = ParentMenuID;
            menu.Title = MenuTitle.Length > 40 ? MenuTitle.Substring(0, 39) : MenuTitle;
            menu.Type = MenuType;
            menu.RefID = MenuType == "Link" ? 0 : RefID;
            menu.Slug = MenuType == "Link" ? MenuSlug : "";
            menu.DisplayOrder = DisplayOrder;
            menu.AddedOn = CreatedOn;
            menu.AddedBy = Convert.ToInt32(Session["UserID"]);
            menu.IsDeleted = false;

            context.Menu.Add(menu);
            context.SaveChanges();

            return menu.ID;
        }
    }
}
