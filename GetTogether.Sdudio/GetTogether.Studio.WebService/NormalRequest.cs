using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace GetTogether.Studio.WebService
{
    public class NormalRequest
    {
        private string _Url;

        public string Url
        {
            get { return _Url; }
            set { _Url = value; }
        }

        private int _Timeout;

        public int Timeout
        {
            get { return _Timeout; }
            set { _Timeout = value; }
        }

        //public object SendRequest(string requestContent)
        //{
        //    string retXml = GetTogether.Utility.RequestHelper.GetRequest(this.Url, requestContent, "POST", "application/x-www-form-urlencoded", this.Timeout);
        //    return GetTogether.Utility.Xml.XmlHelper.FormatXml(retXml);
        //}

        public string SendRequest(string requestContent, RequestProperty properties)
        {
            if (properties.Accept != null) properties.Accept = properties.Accept.Trim();
            if (properties.Address != null) properties.Address = properties.Address.Trim();
            if (properties.BasicAuthPassword != null) properties.BasicAuthPassword = properties.BasicAuthPassword.Trim();
            if (properties.BasicAuthUserName != null) properties.BasicAuthUserName = properties.BasicAuthUserName.Trim();
            if (properties.ContentType != null) properties.ContentType = properties.ContentType.Trim();
            if (properties.Expect != null) properties.Expect = properties.Expect.Trim();
            if (properties.MediaType != null) properties.MediaType = properties.MediaType.Trim();
            if (properties.Method != null) properties.Method = properties.Method.Trim();
            if (properties.Referer != null) properties.Referer = properties.Referer.Trim();
            if (properties.TransferEncoding != null) properties.TransferEncoding = properties.TransferEncoding.Trim();
            if (properties.UserAgent != null) properties.UserAgent = properties.UserAgent.Trim();

            if (string.IsNullOrEmpty(properties.Address)) properties.Address = this.Url;
            if (properties.Timeout == 0) properties.Timeout = this.Timeout;
            if (string.IsNullOrEmpty(properties.Method)) properties.Method = "post";
            if (string.IsNullOrEmpty(properties.ContentType)) properties.ContentType = "text/xml; charset=utf-8";//application/x-www-form-urlencoded

            return RequestHelper.SendRequest(requestContent, properties);
        }
    }
}
