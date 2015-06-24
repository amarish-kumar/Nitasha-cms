using NITASA.Data;
using NITASA.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace NITASA.Areas.Admin.Helper
{
    public enum ActionType
    {
        Add,
        Update,
        Delete,
        Search,
        View,
        Active,
        InActive,
        Publish,
        Draft,
        UnPublish
    }

    public class LogSector
    {
        public static string websiteRootPath;

        public LogSector()
        {
            websiteRootPath = HttpRuntime.AppDomainAppPath;
        }
        #region Error Log Related Method

        /// <summary>
        /// Get Error Log File name for today as we are creating day wise error log file
        /// </summary>
        ///<history>
        /// Created BY: Nikhil M. Prajapati---
        /// Created On: 24-Jun-15 ---
        /// </history>
        public static string GetErrorLogFileName(bool includeFolderPath)
        {
            //Sample : errorlogs/2015_jun_24_nitasa_errorlog.xml
            string errorLogFileName = string.Empty;
            string year = DateTime.Now.Year.ToString();
            string monthName = DateTime.Now.ToString("MMM");
            string monthDay = DateTime.Now.Day.ToString();

            if (includeFolderPath)
            {
                errorLogFileName = "/Admin/ActivityLog/ErrorLogs/" + year + "_" + monthName + "_" + monthDay + "_nitasa_errorlog.xml";
            }
            else
            {

                errorLogFileName = year + "_" + monthName + "_" + monthDay + "_nitasa_errorlog.xml";
            }
            return errorLogFileName;
        }

        /// <summary>
        /// function to generate and log errors in xml following file format   
        /// <?xml version="1.0" encoding="utf-8" ?>
        /// <errors>
        ///        <error>
        ///            <pagename>Add New Post</pagename>
        ///            <errortime>2015-06-26 10:25:232 AM</errortime>
        ///            <errormessage>New record added with ID</errormessage>
        ///            <innererrormessage>New record added with ID</innererrormessage>
        ///        </error>
        ///</errors>
        ///</summary>
        /// 
        public static void LogError(Exception ex)
        {
            string fileName = Path.Combine(websiteRootPath, LogSector.GetErrorLogFileName(true));
            //Debug.WriteLine(fileName);
            XmlDocument xmlDoc = new XmlDocument();

            if (!File.Exists(fileName))
            {
                XmlNode rootErrorsNode = xmlDoc.CreateElement("errors");
                xmlDoc.AppendChild(rootErrorsNode);
                XmlNode errorNode = xmlDoc.CreateElement("error");
                rootErrorsNode.AppendChild(errorNode);

                XmlNode pageNameNode = xmlDoc.CreateElement("pagename");
                pageNameNode.InnerText = HttpContext.Current.Request.RawUrl;
                errorNode.AppendChild(pageNameNode);

                XmlNode errorTimeNode = xmlDoc.CreateElement("errortime");
                errorTimeNode.InnerText = DateTime.Now.ToString();
                errorNode.AppendChild(errorTimeNode);

                XmlNode errorMessageNode = xmlDoc.CreateElement("errormessage");
                errorMessageNode.InnerText = ex.Message;
                errorNode.AppendChild(errorMessageNode);


                if (HttpContext.Current != null)
                {
                    //this will be user request
                    XmlNode errorIpNode = xmlDoc.CreateElement("REMOTE_ADDR");
                    errorIpNode.InnerText = HttpContext.Current.Request.UserHostAddress;
                    errorNode.AppendChild(errorIpNode);
                }

                if (ex.InnerException != null)
                {
                    XmlNode innerErrorMessageNode = xmlDoc.CreateElement("innererrormessage");
                    innerErrorMessageNode.InnerText = ex.InnerException.Message;
                    errorNode.AppendChild(innerErrorMessageNode);
                }
                xmlDoc.Save(fileName);
            }
            else
            {
                xmlDoc.Load(fileName);
                XmlNode rootErrorsNode = xmlDoc.DocumentElement;
                XmlNode errorNode = xmlDoc.CreateElement("error");
                rootErrorsNode.AppendChild(errorNode);

                XmlNode pageNameNode = xmlDoc.CreateElement("pagename");
                if (HttpContext.Current != null)
                {
                    pageNameNode.InnerText = HttpContext.Current.Request.RawUrl;
                    errorNode.AppendChild(pageNameNode);
                }
                else
                {
                    pageNameNode.InnerText = "Not in a page(May be thread)";
                    errorNode.AppendChild(pageNameNode);
                }

                XmlNode errorTimeNode = xmlDoc.CreateElement("errortime");
                errorTimeNode.InnerText = DateTime.Now.ToString();
                errorNode.AppendChild(errorTimeNode);

                XmlNode errorMessageNode = xmlDoc.CreateElement("errormessage");
                errorMessageNode.InnerText = ex.Message;
                errorNode.AppendChild(errorMessageNode);

                if (HttpContext.Current != null)
                {
                    //this will be user request
                    XmlNode errorIpNode = xmlDoc.CreateElement("REMOTE_ADDR");
                    errorIpNode.InnerText = HttpContext.Current.Request.UserHostAddress;
                    errorNode.AppendChild(errorIpNode);
                }


                if (ex.InnerException != null)
                {
                    XmlNode innerErrorMessageNode = xmlDoc.CreateElement("innererrormessage");
                    innerErrorMessageNode.InnerText = ex.InnerException.Message;
                    errorNode.AppendChild(innerErrorMessageNode);
                }
                xmlDoc.Save(fileName);
            }
        }

        #endregion

        #region Action Log Related Method
        /// <summary>
        /// Get Log File name for today as we are creating day wise log file
        /// </summary>
        ///<history>
        /// Created BY: Nikhil M. Prajapati---
        /// Created On: 26-Jun-15 ---
        /// </history>
        public static string GetLogFileName(bool includeFolderPath)
        {
            //Sample : logs/2014_mar_26_nitasa_log.xml
            string logFileName = string.Empty;
            string year = DateTime.Now.Year.ToString();
            string monthName = DateTime.Now.ToString("MMM");
            string monthDay = DateTime.Now.Day.ToString();

            if (includeFolderPath)
            {
                logFileName = "/Admin/ActivityLog/ActionLogs" + year + "_" + monthName + "_" + monthDay + "_nitasa_log.xml";
            }
            else
            {

                logFileName = year + "_" + monthName + "_" + monthDay + "_nitasa_log.xml";
            }
            return logFileName;
        }

        public static void LogAction(ActionType actionType, string pageURL, string fieldName, List<Field> fieldList)
        {
            NTSDBContext context = new NTSDBContext();
            string currentTime = DateTime.Now.ToString();

            string userName = context.User.Where(m => m.ID == Common.CurrentUserID()).Select(m => m.FullName).FirstOrDefault();

            if (actionType == ActionType.Add)  //Insert Log for Add action here
            {
                LogSector.Log(ActionType.Add, userName, pageURL, currentTime, "New record inserted with detail : " + fieldName);
            }
            else if (actionType == ActionType.Update)  //Insert Log for edit action here
            {
                if (fieldList == null && fieldName != string.Empty)
                {
                    LogSector.LogEditText(userName, pageURL, currentTime, fieldName);
                }
                else
                    LogSector.LogEdit(userName, pageURL, currentTime, fieldList);
            }
            else if (actionType == ActionType.Search)  //Insert Log for Search action here
            {
                LogSector.Log(ActionType.Search, userName, pageURL, currentTime, "Record search with string : " + fieldName);
            }
            else if (actionType == ActionType.Delete)  //Insert Log for delete action here
            {
                LogSector.Log(ActionType.Delete, userName, pageURL, currentTime, "Delete Record with ID " + fieldName);
            }
            else if (actionType == ActionType.View)  //Insert Log for view action here
            {
                //Ignore as of now
            }
            else if (actionType == ActionType.Active)  //Insert Log for Active action here
            {
                LogSector.Log(ActionType.Active, userName, pageURL, currentTime, "Make record active with ID " + fieldName);
            }
            else if (actionType == ActionType.InActive)  //Insert Log for InActive action here
            {
                LogSector.Log(ActionType.Delete, userName, pageURL, currentTime, "Make record deactive with ID " + fieldName);
            }
            else if (actionType == ActionType.Publish)  //Insert Log for Publish action
            {
                LogSector.Log(ActionType.Add, userName, pageURL, currentTime, "Published :" + fieldName);
            }
            else if (actionType == ActionType.Draft)  //Insert Log for Draft action
            {
                LogSector.Log(ActionType.Add, userName, pageURL, currentTime, "Drafted :" + fieldName);
            }
            else if (actionType == ActionType.UnPublish)  //Insert Log for Unpublish action
            {
                LogSector.Log(ActionType.Add, userName, pageURL, currentTime, "Unpublished : " + fieldName);
            }
        }
        /// <summary>
        /// Insert add,view,delete,active and inactive action log to XML file.
        /// </summary>
        /// <remarks>
        /// Created By : Tarun Dudhatra
        /// Created On : 02-Apr-14
        /// </remarks>
        private static void Log(ActionType actionType, string userName, string pageName, string actionTime, string actionDetail)
        {
            XmlDocument doc = new XmlDocument();
            string actionLogFileName = Path.Combine(websiteRootPath, LogSector.GetLogFileName(true));
            //If there is no current file, then create a new one
            if (!System.IO.File.Exists(actionLogFileName))
            {
                //Create neccessary nodes
                XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                XmlComment comment = doc.CreateComment("This is an XML Action Log File");
                XmlElement root = doc.CreateElement("actions");
                XmlElement action = doc.CreateElement("action");
                XmlAttribute actiontype = doc.CreateAttribute("type");
                XmlElement username = doc.CreateElement("username");
                XmlElement pagename = doc.CreateElement("pagename");
                XmlElement actiontime = doc.CreateElement("actiontime");
                XmlElement actiondetail = doc.CreateElement("actiondetail");

                //Add the values for each nodes
                actiontype.Value = actionType.ToString();
                username.InnerText = userName;
                pagename.InnerText = pageName;
                actiontime.InnerText = actionTime;
                actiondetail.InnerText = actionDetail;

                //Construct the document
                doc.AppendChild(declaration);
                doc.AppendChild(comment);
                doc.AppendChild(root);
                root.AppendChild(action);
                action.Attributes.Append(actiontype);
                action.AppendChild(username);
                action.AppendChild(pagename);
                action.AppendChild(actiontime);
                action.AppendChild(actiondetail);
                if (HttpContext.Current != null)
                {
                    //this will be user request
                    XmlNode errorIpNode = doc.CreateElement("REMOTE_ADDR");
                    errorIpNode.InnerText = HttpContext.Current.Request.UserHostAddress;
                    action.AppendChild(errorIpNode);
                }

                doc.Save(actionLogFileName);
            }
            else //If there is already a file
            {
                //Load the XML File
                doc.Load(actionLogFileName);

                //Get the root element
                XmlElement root = doc.DocumentElement;

                XmlElement action = doc.CreateElement("action");
                XmlAttribute actiontype = doc.CreateAttribute("type");
                XmlElement username = doc.CreateElement("username");
                XmlElement pagename = doc.CreateElement("pagename");
                XmlElement actiontime = doc.CreateElement("actiontime");
                XmlElement actiondetail = doc.CreateElement("actiondetail");

                //Add the values for each nodes
                actiontype.Value = actionType.ToString();
                username.InnerText = userName;
                pagename.InnerText = pageName;
                actiontime.InnerText = actionTime;
                actiondetail.InnerText = actionDetail;

                root.AppendChild(action);
                action.Attributes.Append(actiontype);
                action.AppendChild(username);
                action.AppendChild(pagename);
                action.AppendChild(actiontime);
                action.AppendChild(actiondetail);

                if (HttpContext.Current != null)
                {
                    //this will be user request
                    XmlNode errorIpNode = doc.CreateElement("REMOTE_ADDR");
                    errorIpNode.InnerText = HttpContext.Current.Request.UserHostAddress;
                    action.AppendChild(errorIpNode);
                }

                doc.Save(actionLogFileName);
            }
        }

        /// <summary>
        /// Insert edit action log to XML file.
        /// </summary>
        /// <remarks>
        /// Created By : Nikhil M. Prajapati
        /// Created On : 24-Jun-15
        /// </remarks>
        private static void LogEdit(string userName, string pageName, string actionTime, List<Field> fieldList)
        {
            XmlDocument doc = new XmlDocument();
            string actionLogFileName = Path.Combine(websiteRootPath, LogSector.GetLogFileName(true));
            //If there is no current file, then create a new one
            if (!System.IO.File.Exists(actionLogFileName))
            {
                //Create neccessary nodes
                XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                XmlComment comment = doc.CreateComment("This is an XML Action Log File");
                XmlElement root = doc.CreateElement("actions");
                XmlElement action = doc.CreateElement("action");
                XmlAttribute actiontype = doc.CreateAttribute("type");
                XmlElement username = doc.CreateElement("username");
                XmlElement pagename = doc.CreateElement("pagename");
                XmlElement actiontime = doc.CreateElement("actiontime");
                XmlElement actiondetail = doc.CreateElement("actiondetail");
                XmlElement fields = doc.CreateElement("fields");

                foreach (Field singleField in fieldList)
                {
                    XmlElement field;
                    XmlElement fieldid = doc.CreateElement("fieldid");
                    XmlElement fieldName = doc.CreateElement("fieldname");
                    XmlElement fieldoldvalue = doc.CreateElement("fieldoldvalue");
                    XmlElement fieldnewvalue = doc.CreateElement("fieldnewvalue");

                    field = doc.CreateElement("field");
                    fieldid.InnerText = singleField._ID;
                    fieldName.InnerText = singleField._fieldName;
                    fieldoldvalue.InnerText = singleField._oldValue;
                    fieldnewvalue.InnerText = singleField._newValue;

                    field.AppendChild(fieldid);
                    field.AppendChild(fieldName);
                    field.AppendChild(fieldoldvalue);
                    field.AppendChild(fieldnewvalue);
                    fields.AppendChild(field);
                }
                //Add the values for each nodes
                actiontype.Value = "Edit";
                username.InnerText = userName;
                pagename.InnerText = pageName;
                actiontime.InnerText = actionTime;
                //actiondetail.InnerText = actionDetail;

                //Construct the document
                actiondetail.AppendChild(fields);
                doc.AppendChild(declaration);
                doc.AppendChild(comment);
                doc.AppendChild(root);
                root.AppendChild(action);
                action.Attributes.Append(actiontype);
                action.AppendChild(username);
                action.AppendChild(pagename);
                action.AppendChild(actiontime);
                action.AppendChild(actiondetail);


                if (HttpContext.Current != null)
                {
                    //this will be user request
                    XmlNode errorIpNode = doc.CreateElement("REMOTE_ADDR");
                    errorIpNode.InnerText = HttpContext.Current.Request.UserHostAddress;
                    action.AppendChild(errorIpNode);
                }

                doc.Save(actionLogFileName);
            }
            else //If there is already a file
            {
                //Load the XML File
                doc.Load(actionLogFileName);

                //Get the root element
                XmlElement root = doc.DocumentElement;

                XmlElement action = doc.CreateElement("action");
                XmlAttribute actiontype = doc.CreateAttribute("type");
                XmlElement username = doc.CreateElement("username");
                XmlElement pagename = doc.CreateElement("pagename");
                XmlElement actiontime = doc.CreateElement("actiontime");
                XmlElement actiondetail = doc.CreateElement("actiondetail");
                XmlElement fields = doc.CreateElement("fields");


                foreach (Field singleField in fieldList)
                {
                    XmlElement field;
                    XmlElement fieldid = doc.CreateElement("fieldid");
                    XmlElement fieldName = doc.CreateElement("fieldname");
                    XmlElement fieldoldvalue = doc.CreateElement("fieldoldvalue");
                    XmlElement fieldnewvalue = doc.CreateElement("fieldnewvalue");

                    field = doc.CreateElement("field");
                    fieldid.InnerText = singleField._ID;
                    fieldName.InnerText = singleField._fieldName;
                    fieldoldvalue.InnerText = singleField._oldValue;
                    fieldnewvalue.InnerText = singleField._newValue;

                    field.AppendChild(fieldid);
                    field.AppendChild(fieldName);
                    field.AppendChild(fieldoldvalue);
                    field.AppendChild(fieldnewvalue);
                    fields.AppendChild(field);
                }

                //Add the values for each nodes
                actiontype.Value = "Edit";
                username.InnerText = userName;
                pagename.InnerText = pageName;
                actiontime.InnerText = actionTime;
                //actiondetail.InnerText = actionDetail;
                actiondetail.AppendChild(fields);
                root.AppendChild(action);
                action.Attributes.Append(actiontype);
                action.AppendChild(username);
                action.AppendChild(pagename);
                action.AppendChild(actiontime);
                action.AppendChild(actiondetail);

                if (HttpContext.Current != null)
                {
                    //this will be user request
                    XmlNode errorIpNode = doc.CreateElement("REMOTE_ADDR");
                    errorIpNode.InnerText = HttpContext.Current.Request.UserHostAddress;
                    action.AppendChild(errorIpNode);
                }

                doc.Save(actionLogFileName);
            }
        }

        /// <summary>
        /// Insert edit action log to XML file.
        /// </summary>
        /// <remarks>
        /// Created By : Nikhil M. Prajapati
        /// Created On : 24-Jun-15
        /// </remarks>
        private static void LogEditText(string userName, string pageName, string actionTime, string fieldName)
        {
            XmlDocument doc = new XmlDocument();
            string actionLogFileName = Path.Combine(websiteRootPath, LogSector.GetLogFileName(true));
            //If there is no current file, then create a new one
            if (!System.IO.File.Exists(actionLogFileName))
            {
                //Create neccessary nodes
                XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                XmlComment comment = doc.CreateComment("This is an XML Action Log File");
                XmlElement root = doc.CreateElement("actions");
                XmlElement action = doc.CreateElement("action");
                XmlAttribute actiontype = doc.CreateAttribute("type");
                XmlElement username = doc.CreateElement("username");
                XmlElement pagename = doc.CreateElement("pagename");
                XmlElement actiontime = doc.CreateElement("actiontime");
                XmlElement actiondetail = doc.CreateElement("actiondetail");

                //Add the values for each nodes
                actiontype.Value = "Edit";
                username.InnerText = userName;
                pagename.InnerText = pageName;
                actiontime.InnerText = actionTime;
                actiondetail.InnerText = fieldName;

                doc.AppendChild(declaration);
                doc.AppendChild(comment);
                doc.AppendChild(root);
                root.AppendChild(action);
                action.Attributes.Append(actiontype);
                action.AppendChild(username);
                action.AppendChild(pagename);
                action.AppendChild(actiontime);
                action.AppendChild(actiondetail);

                if (HttpContext.Current != null)
                {
                    //this will be user request
                    XmlNode errorIpNode = doc.CreateElement("REMOTE_ADDR");
                    errorIpNode.InnerText = HttpContext.Current.Request.UserHostAddress;
                    action.AppendChild(errorIpNode);
                }

                doc.Save(actionLogFileName);
            }
            else //If there is already a file
            {
                //Load the XML File
                doc.Load(actionLogFileName);

                //Get the root element
                XmlElement root = doc.DocumentElement;

                XmlElement action = doc.CreateElement("action");
                XmlAttribute actiontype = doc.CreateAttribute("type");
                XmlElement username = doc.CreateElement("username");
                XmlElement pagename = doc.CreateElement("pagename");
                XmlElement actiontime = doc.CreateElement("actiontime");
                XmlElement actiondetail = doc.CreateElement("actiondetail");

                //Add the values for each nodes
                actiontype.Value = "Edit";
                username.InnerText = userName;
                pagename.InnerText = pageName;
                actiontime.InnerText = actionTime;
                actiondetail.InnerText = fieldName;

                root.AppendChild(action);
                action.Attributes.Append(actiontype);
                action.AppendChild(username);
                action.AppendChild(pagename);
                action.AppendChild(actiontime);
                action.AppendChild(actiondetail);

                if (HttpContext.Current != null)
                {
                    //this will be user request
                    XmlNode errorIpNode = doc.CreateElement("REMOTE_ADDR");
                    errorIpNode.InnerText = HttpContext.Current.Request.UserHostAddress;
                    action.AppendChild(errorIpNode);
                }

                doc.Save(actionLogFileName);
            }
        }
        #endregion
    }

    /// <summary>
    /// Created for Manipulating fields
    /// </summary>
    /// <remarks>
    /// Created By : Nikhil M. Prajapati
    /// Created On : 26-Jun-15
    /// </remarks>
    public class Field
    {
        public string _ID { get; set; }
        public string _fieldName { get; set; }
        public string _oldValue { get; set; }
        public string _newValue { get; set; }

        public Field(string ID, string fieldName, string oldValue, string newValue)
        {
            _ID = ID;
            _fieldName = fieldName;
            _oldValue = oldValue;
            _newValue = newValue;
        }
    }
}