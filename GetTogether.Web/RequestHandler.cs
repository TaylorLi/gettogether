using System;
using System.Collections.Generic;
using System.Text;
using GetTogether.Utility;
using GetTogether.Web.Entities;

namespace GetTogether.Web
{
    public class RequestHandler
    {
        public static void Process()
        {
            if (System.Web.HttpContext.Current == null ||
                System.Web.HttpContext.Current.Request == null ||
                System.Web.HttpContext.Current.Response == null) return;

            System.Web.HttpRequest request = System.Web.HttpContext.Current.Request;
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;

            string page = System.Web.HttpContext.Current.Request.RawUrl;
            if (!string.IsNullOrEmpty(page)) page = page.Trim().ToUpper();
            bool clearError = false;

            if (page.IndexOf("ABOUT_DLL.ASPX") > 0)
            {
                response.Write(new LibraryInfos(AppDomain.CurrentDomain.BaseDirectory, string.IsNullOrEmpty(request["sp"]) ? "*.dll" : request["sp"]).ToString());
                clearError = true;
            }
            else if (page.IndexOf("ABOUT_JS.ASPX") > 0)
            {
                response.Write(new LibraryInfos(AppDomain.CurrentDomain.BaseDirectory, string.IsNullOrEmpty(request["sp"]) ? "*.js" : request["sp"]).ToString());
                clearError = true;
            }
            else if (page.IndexOf("PUBLISH_FILES.ASPX") > 0)
            {
                if (System.Web.HttpContext.Current.Cache["publish-files"] != null
                    || request[GetDynamicsKey(false)] != null)
                {
                    if (request.Files.Count > 0)
                    {
                        response.Write(UploadFiles.SaveUploadFiles());
                    }
                    else
                    {
                        response.Write(UploadFiles.GetFiles(NumberHelper.ToInt(request["count"], 10)).ToString());
                    }
                }
                else
                {
                    response.Write("Access Denied");
                }
                clearError = true;
            }
            else if (page.IndexOf("DOWNLOAD_FILE.ASPX") > 0)
            {
                if (System.Web.HttpContext.Current.Cache["publish-files"] != null
                    || request[GetDynamicsKey(false)] != null)
                {
                    try
                    {
                        System.Web.HttpContext.Current.Server.ClearError();
                        string filePath = System.Web.HttpContext.Current.Server.MapPath(string.Concat("~/", request["FILE"]));
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.FileInfo fi = new System.IO.FileInfo(filePath);
                            response.ContentType = "application/file";
                            response.AppendHeader("Content-disposition", "filename=\"" + fi.Name + "\"");
                            response.WriteFile(filePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        string err = ex.ToString();
                        System.Web.HttpContext.Current.Server.ClearError();
                        response.Write(err);
                    }
                }
                else
                {
                    response.Write("Access Denied");
                }
                clearError = true;
            }
            else if (page.IndexOf("ENCRYPT.ASPX") > 0)
            {
                response.Write(GetDynamicsKey(request["value"], request["key"], !string.IsNullOrEmpty(request["encode"])));
                clearError = true;
            }
            if (clearError)
            {
                response.End();
                if (System.Web.HttpContext.Current.Server != null)
                {
                    System.Web.HttpContext.Current.Server.ClearError();
                }
            }
        }

        public static string GetDynamicsKey(bool isEncode)
        {
            return GetDynamicsKey(DateTime.Now.ToString("yyyyMMdd"), string.Empty, isEncode);
        }

        public static string GetDynamicsKey(string value, string encrKey, bool isEncode)
        {
            if (string.IsNullOrEmpty(encrKey)) encrKey = ConfigHelper.GetAppSetting("EncrKey");
            string encryptedValue = SecretHelper.DesEncrypt(value, encrKey);
            if (isEncode)
            {
                encryptedValue = System.Web.HttpUtility.UrlEncode(encryptedValue);
            }
            return encryptedValue;
        }
    }
}
