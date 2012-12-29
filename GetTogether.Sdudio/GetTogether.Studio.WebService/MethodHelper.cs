using System;
using System.Collections.Generic;
using System.Web;

namespace GetTogether.Studio.WebService
{
    public class MethodHelper
    {
        public static bool IsWebMethod(System.Reflection.MethodInfo method)
        {
            object[] customAttributes = method.GetCustomAttributes(typeof(System.Web.Services.Protocols.SoapRpcMethodAttribute), true);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                return true;
            }
            customAttributes = method.GetCustomAttributes(typeof(System.Web.Services.Protocols.SoapDocumentMethodAttribute), true);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                return true;
            }
            customAttributes = method.GetCustomAttributes(typeof(System.Web.Services.Protocols.HttpMethodAttribute), true);
            return ((customAttributes != null) && (customAttributes.Length > 0));
        }

        public static bool IsWebService(System.Type type)
        {
            return typeof(System.Web.Services.Protocols.HttpWebClientProtocol).IsAssignableFrom(type);
        }

        public static string GetClassName(string url)
        {
            string[] parts = url.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');
            return pps[0];
        }
    }
}