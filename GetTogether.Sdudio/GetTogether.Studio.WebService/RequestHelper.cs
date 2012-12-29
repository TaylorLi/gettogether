using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Net;
using System.IO;

namespace GetTogether.Studio.WebService
{
    public class RequestHelper
    {
        public static string SendWebRequest(GetTogether.Studio.WebService.ProjectParameter parameter, System.Reflection.MethodInfo methodInfo, string requestXml)
        {
            System.Type declaringType = methodInfo.DeclaringType;
            string action = "";
            System.Web.Services.Protocols.SoapDocumentMethodAttribute[] customAttributes =
                (System.Web.Services.Protocols.SoapDocumentMethodAttribute[])methodInfo.GetCustomAttributes(
                    typeof(System.Web.Services.Protocols.SoapDocumentMethodAttribute), true);
            for (int i = 0; i < customAttributes.Length; i++)
            {
                System.Web.Services.Protocols.SoapDocumentMethodAttribute attribute = customAttributes[i];
                action = attribute.Action;
            }
            string url = parameter.Address;
            string contentType = "text/xml; charset=utf-8";
            string method = "post";
            int timeout = parameter.Timeout;
            return SendWebRequest(url, action, requestXml, method, contentType, timeout);
        }

        public static string SendWebRequest(string url, string action, string rqt, string method, string requestContentType, int timeout)
        {
            string responseXml = string.Empty;
            Encoding encoding = new UTF8Encoding(true);
            HttpWebRequest request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            System.Net.CookieContainer cc = new CookieContainer();
            request.Method = method;
            request.ContentType = requestContentType;
            request.Headers["SOAPAction"] = action;
            request.AllowAutoRedirect = true;
            request.AllowWriteStreamBuffering = true;
            request.SendChunked = false;
            request.KeepAlive = false;
            request.Pipelined = false;
            request.PreAuthenticate = false;
            request.Timeout = timeout;
            request.CookieContainer = new CookieContainer();
            CredentialCache cache = new CredentialCache();
            #region Pending
            //request.Proxy=...    
            //bool flag = false;
            //if (basicAuthUserName != null && basicAuthUserName!=null))
            //{
            //    cache.Add(new Uri(selectedObject.Url), "Basic", new NetworkCredential(basicAuthUserName, basicAuthPassword));
            //    flag = true;
            //}
            //if (useDefaultCredential)
            //{
            //    cache.Add(new Uri(selectedObject.Url), "NTLM", (NetworkCredential)CredentialCache.DefaultCredentials);
            //    flag = true;
            //}
            //if (flag)
            //{
            //    request.Credentials = cache;
            //}
            #endregion
            request.ContentLength = rqt.Length + encoding.GetPreamble().Length;
            StreamWriter writer = new StreamWriter(request.GetRequestStream(), encoding);
            writer.Write(rqt);
            writer.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                responseXml = GetResponse(response);
                response.Close();
            }
            catch (WebException exception)
            {
                if (exception.Response != null)
                {
                    responseXml = GetResponse((HttpWebResponse)exception.Response);
                }
                else
                {
                    responseXml = exception.ToString();
                }
            }
            catch (Exception exception2)
            {
                responseXml = exception2.ToString();
            }
            return responseXml;
        }

        public static string SendRequest(string requestContent, RequestProperty properties)
        {
            string responseXml = string.Empty;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(properties.Address);
            if (properties.Timeout > 0) req.Timeout = properties.Timeout;
            byte[] request_bytes = System.Text.Encoding.UTF8.GetBytes(requestContent);
            req.Method = properties.Method;//"POST";
            req.ContentType = properties.ContentType;//"text/xml,application/x-www-form-urlencoded
            req.ContentLength = request_bytes.Length;
            if (properties.Headers != null && properties.Headers.Length > 0)
            {
                foreach (string s in properties.Headers)
                {
                    string[] headerInfo = s.Split('|');
                    if (headerInfo.Length == 2)
                        req.Headers[headerInfo[0]] = headerInfo[1];
                }
            }
            CredentialCache cache = new CredentialCache();
            bool flag = false;
            if (!string.IsNullOrEmpty(properties.BasicAuthUserName) && !string.IsNullOrEmpty(properties.BasicAuthPassword))
            {
                cache.Add(new Uri(properties.Address), "Basic", new NetworkCredential(properties.BasicAuthUserName, properties.BasicAuthPassword));
                flag = true;
            }
            if (properties.UseDefaultCredential)
            {
                cache.Add(new Uri(properties.Address), "NTLM", (NetworkCredential)CredentialCache.DefaultCredentials);
                flag = true;
            }
            if (flag)
            {
                req.Credentials = cache;
            }
            req.UserAgent = properties.UserAgent;
            req.AllowAutoRedirect = properties.AllowAutoRedirect;
            req.AllowWriteStreamBuffering = properties.AllowWriteStreamBuffering;
            req.KeepAlive = properties.KeepAlive;
            req.Pipelined = properties.Pipelined;
            req.PreAuthenticate = properties.PreAuthenticate;
            req.Referer = properties.Referer;
            req.SendChunked = properties.SendChunked;
            req.TransferEncoding = properties.TransferEncoding;
            req.Accept = properties.Accept;
            req.Expect = properties.Expect;
            req.MediaType = properties.MediaType;

            Stream request_stream = req.GetRequestStream();
            request_stream.Write(request_bytes, 0, request_bytes.Length);
            request_stream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                responseXml = RequestHelper.GetResponse(response);
                response.Close();
            }
            catch (WebException exception)
            {
                if (exception.Response != null)
                {
                    responseXml = RequestHelper.GetResponse((HttpWebResponse)exception.Response);
                }
                else
                {
                    responseXml = exception.ToString();
                }
            }
            catch (Exception exception2)
            {
                responseXml = exception2.ToString();
            }
            return responseXml;
        }

        public static string GetResponse(WebResponse response)
        {
            Stream responseStream = response.GetResponseStream();
            StringBuilder builder = new StringBuilder();
            System.IO.StreamReader reader = new System.IO.StreamReader(responseStream, System.Text.Encoding.UTF8);
            builder.Append(reader.ReadToEnd());
            string xml = builder.ToString();
            return GetTogether.Utility.Xml.XmlHelper.FormatXml(xml);
        }
    }
}