<%@ Control Language="C#" EnableViewState="false" AutoEventWireup="false" Inherits="FredCK.FCKeditorV2.FileBrowser.Config" %>
<%--
 * FCKeditor - The text editor for Internet - http://www.fckeditor.net
 * Copyright (C) 2003-2009 Frederico Caldeira Knabben
 *
 * == BEGIN LICENSE ==
 *
 * Licensed under the terms of any of the following licenses at your
 * choice:
 *
 *  - GNU General Public License Version 2 or later (the "GPL")
 *    http://www.gnu.org/licenses/gpl.html
 *
 *  - GNU Lesser General Public License Version 2.1 or later (the "LGPL")
 *    http://www.gnu.org/licenses/lgpl.html
 *
 *  - Mozilla Public License Version 1.1 or later (the "MPL")
 *    http://www.mozilla.org/MPL/MPL-1.1.html
 *
 * == END LICENSE ==
 *
 * Configuration file for the File Browser Connector for ASP.NET.
--%>

<script runat="server">

    /**
	 * This function must check the user session to be sure that he/she is
	 * authorized to upload and access files in the File Browser.
	 */

    //public AdminCMS.Web.SessionObject sess
    //{
    //    get { return AdminCMS.Web.SessionObject.GetSessionObject(); }
    //    set { AdminCMS.Web.SessionObject.SetValue(AdminCMS.Definition.Session.ADM_SESSION_OBJECT, value); }
    //}
    private bool CheckAuthentication()
    {
        //if (sess == null || sess.Profiles == null)
        //{
        //    ErrorMessage = "Operation timeout,please login again.";
        //}
        //else if (sess.AdminRights == null)
        //{
        //    ErrorMessage = "You have no right to view this page.";
        //}
        //else
        //{
        //    switch (Request["PageType"])
        //    {
        //        case "News":
        //            if (!sess.AdminRights.ViewNews)
        //                ErrorMessage = "You have no right to view this page.";
        //            break;             
        //        default:
        //            ErrorMessage = "You have no right to view this page.";
        //            break;
        //    }
        //}
        return string.IsNullOrEmpty(ErrorMessage);
    }

    public void SetUserFilesPath(out string appendUrl)
    {
        appendUrl = "";
        string pageFolder = string.IsNullOrEmpty(Request["PageType"]) ? string.Empty : string.Concat(Request["PageType"], "/");
        string pageAbsoluteFolder = string.IsNullOrEmpty(Request["PageType"]) ? string.Empty : string.Concat(Request["PageType"], "\\");
        switch (Request["UserFilesPath"])
        {
            case "ManageFileUploadUrl":

                UserFilesPath = string.Concat(GetTogether.Studio.Config.Original.ManageFileUploadUrl, "Content/", pageFolder);
                UserFilesAbsolutePath = string.Concat(System.IO.Path.Combine(GetTogether.Studio.Config.Original.ManageFileUploadPath, "Content\\"), pageAbsoluteFolder);
                //language folder
                appendUrl += GetLanguageFolder();
                //com folder
                //if (sess.AdminRights.BkgViewAll)
                //{
                //}
                //else if (sess.AdminRights.BkgViewByCompany)
                //{
                //    appendUrl = string.Concat(sess.Profiles.Company.CompanyCode, "/");
                //}
                //else if (sess.AdminRights.BkgViewByTeam)
                //{
                //    appendUrl = string.Concat(sess.Profiles.Company.CompanyCode, "/", sess.Profiles.User.TeamCode, "/");
                //}
                //else
                //{
                //    appendUrl = string.Concat(sess.Profiles.Company.CompanyCode, "/", sess.Profiles.User.TeamCode, "/", sess.Profiles.User.UserCode, "/");
                //}

                break;
            default:
                break;
        }
    }
    public string GetLanguageFolder()
    {
        switch (Request["LanguageType"])
        {
            case "en_us":
                return "en_us/";

            case "zh_cn":
                return "zh_cn/";

            case "zh_tw":
                return "zh_tw/";

            default:
                return string.Empty;
        }
    }

    public override void SetConfig()
    {
        // SECURITY: You must explicitly enable this "connector". (Set it to "true").
        Enabled = CheckAuthentication();

        if (!Enabled)
            return;
        string appendUrl;
        SetUserFilesPath(out appendUrl);
        //// URL path to user files.
        //UserFilesPath = /userfiles/;

        //// The connector tries to resolve the above UserFilesPath automatically.
        //// Use the following setting it you prefer to explicitely specify the
        //// absolute path. Examples: 'C:\\MySite\\userfiles\\' or '/root/mysite/userfiles/'.
        //// Attention: The above 'UserFilesPath' URL must point to the same directory.
        //UserFilesAbsolutePath = /root/mysite/userfiles/;

        // Due to security issues with Apache modules, it is recommended to leave the
        // following setting enabled.
        ForceSingleExtension = true;

        // Allowed Resource Types
        AllowedTypes = new string[] { "File", "Image", "Flash" };

        // For security, HTML is allowed in the first Kb of data for files having the
        // following extensions only.
        HtmlExtensions = new string[] { "html", "htm", "xml", "xsd", "txt" };

        TypeConfig["File"].AllowedExtensions = new string[] { "7z", "bmp", "csv", "doc", "gif", "gz", "gzip", "jpeg", "jpg", "pdf", "png", "ppt", "rar", "rtf", "swf", "tar", "tif", "tiff", "txt", "xls", "xml", "zip", "flv" };
        TypeConfig["File"].DeniedExtensions = new string[] { };
        TypeConfig["File"].FilesPath = string.Concat("%UserFilesPath%file/", appendUrl);
        TypeConfig["File"].FilesAbsolutePath = (UserFilesAbsolutePath == "" ? "" : string.Concat("%UserFilesAbsolutePath%file/", appendUrl));
        TypeConfig["File"].QuickUploadPath = string.Concat("%UserFilesPath%file/", appendUrl);
        TypeConfig["File"].QuickUploadAbsolutePath = (UserFilesAbsolutePath == "" ? "" : string.Concat("%UserFilesAbsolutePath%file/", appendUrl));

        TypeConfig["Image"].AllowedExtensions = new string[] { "bmp", "gif", "jpeg", "jpg", "png" };
        TypeConfig["Image"].DeniedExtensions = new string[] { };
        TypeConfig["Image"].FilesPath = string.Concat("%UserFilesPath%image/", appendUrl);
        TypeConfig["Image"].FilesAbsolutePath = (UserFilesAbsolutePath == "" ? "" : string.Concat("%UserFilesAbsolutePath%image/", appendUrl));
        TypeConfig["Image"].QuickUploadPath = string.Concat("%UserFilesPath%image/", appendUrl);
        TypeConfig["Image"].QuickUploadAbsolutePath = (UserFilesAbsolutePath == "" ? "" : string.Concat("%UserFilesAbsolutePath%image/", appendUrl));

        TypeConfig["Flash"].AllowedExtensions = new string[] { "swf", "flv" };
        TypeConfig["Flash"].DeniedExtensions = new string[] { };
        TypeConfig["Flash"].FilesPath = string.Concat("%UserFilesPath%flash/", appendUrl);
        TypeConfig["Flash"].FilesAbsolutePath = (UserFilesAbsolutePath == "" ? "" : string.Concat("%UserFilesAbsolutePath%flash/", appendUrl));
        TypeConfig["Flash"].QuickUploadPath = string.Concat("%UserFilesPath%flash/", appendUrl);
        TypeConfig["Flash"].QuickUploadAbsolutePath = (UserFilesAbsolutePath == "" ? "" : string.Concat("%UserFilesAbsolutePath%flash/", appendUrl));

        //TypeConfig["Media"].AllowedExtensions = new string[] { "aiff", "asf", "avi", "bmp", "fla", "flv", "gif", "jpeg", "jpg", "mid", "mov", "mp3", "mp4", "mpc", "mpeg", "mpg", "png", "qt", "ram", "rm", "rmi", "rmvb", "swf", "tif", "tiff", "wav", "wma", "wmv" };
        //TypeConfig["Media"].DeniedExtensions = new string[] { };
        //TypeConfig["Media"].FilesPath = "%UserFilesPath%media/";
        //TypeConfig["Media"].FilesAbsolutePath = (UserFilesAbsolutePath == "" ? "" : "%UserFilesAbsolutePath%media/");
        //TypeConfig["Media"].QuickUploadPath = "%UserFilesPath%flash/";
        //TypeConfig["Media"].QuickUploadAbsolutePath = (UserFilesAbsolutePath == "" ? "" : "%UserFilesAbsolutePath%");
    }
</script>

