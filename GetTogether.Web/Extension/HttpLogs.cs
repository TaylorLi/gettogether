using System;
using System.Web;
using System.IO;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace GetTogether.Web.Extension
{
    public class HttpLogs : System.Web.IHttpModule
    {
        string LogsFolder = GetTogether.Utility.ConfigHelper.GetAppSetting("HttpLogs", "");
        string[] LogsTypes = GetTogether.Utility.ConfigHelper.GetAppSetting("HttpLogsTypes", ".asmx").Split(',');
        const string LogSplit = "----------------------------------------------------------------------------------------------------------------------------------------------";
        class PageFilter : Stream
        {
            Stream responseStream;
            long position;
            public StringBuilder ResponseContent;
            public PageFilter(Stream inputStream)
            {
                responseStream = inputStream;
                ResponseContent = new StringBuilder();
            }

            #region Filter overrides
            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return true; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public override void Close()
            {
                responseStream.Close();
            }

            public override void Flush()
            {
                responseStream.Flush();
            }

            public override long Length
            {
                get { return 0; }
            }

            public override long Position
            {
                get { return position; }
                set { position = value; }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return responseStream.Seek(offset, origin);
            }

            public override void SetLength(long length)
            {
                responseStream.SetLength(length);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return responseStream.Read(buffer, offset, count);
            }
            #endregion

            #region Dirty work
            public override void Write(byte[] buffer, int offset, int count)
            {
                string strBuffer = System.Text.UTF8Encoding.UTF8.GetString(buffer, offset, count);

                ResponseContent.Append(strBuffer);
                responseStream.Write(buffer, 0, count);
            }
            #endregion
        }
        class RequestState
        {
            public string RequestContent = "";
        }

        static Hashtable hPages = new Hashtable();
        static bool Inited = false;

        public HttpLogs()
        {
            if (!Inited)
            {
                Inited = true;
            }
        }
        #region IHttpModule Members
        public void Init(System.Web.HttpApplication context)
        {
            context.AuthenticateRequest += new EventHandler(AuthenticateRequest);
            context.ReleaseRequestState += new EventHandler(context_ReleaseRequestState);
            context.PostRequestHandlerExecute += new EventHandler(context_PostRequestHandlerExecute);
            context.EndRequest += new EventHandler(context_EndRequest);
        }

        public void Dispose()
        {
            // TODO:  Add TestModule.Dispose implementation
        }
        public static string GetValueForSplit(string v, char separator, int lastIndex)
        {
            if (lastIndex == 0) lastIndex = 1;
            if (string.IsNullOrEmpty(v)) return v;
            string[] vi = v.Split('/');
            return vi[vi.Length - lastIndex];
        }
        private void context_ReleaseRequestState(object sender, EventArgs e)
        {
            if (!Inited) return;

            HttpResponse response = HttpContext.Current.Response;
            RequestState rs = EnsureRequestState();
            rs.RequestContent = string.Empty;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            rs.RequestContent = GetRequestString(HttpContext.Current);
            //if (response.ContentType == "text/html")
            //{
            PageFilter pf = new PageFilter(response.Filter);
            response.Filter = pf;

            //}
        }
        #endregion

        RequestState EnsureRequestState()
        {
            RequestState rs = HttpContext.Current.Items["HttpOutputRequestState"] as RequestState;
            if (rs == null)
            {
                rs = new RequestState();
                HttpContext.Current.Items["HttpOutputRequestState"] = rs;
            }
            return rs;
        }
        private void context_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            //if (HttpContext.Current.Session != null && HttpContext.Current.Session.Count > 1)
            //{
            //    RequestState rs = EnsureRequestState();
            //}
            RequestState rs = EnsureRequestState();
        }

        private void context_PreSendRequestContent(object sender, EventArgs e)
        {

        }

        private void AuthenticateRequest(object sender, EventArgs e)
        {

        }
        private bool IsTracePage(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            url = url.ToLower();
            foreach (string s in LogsTypes)
            {
                if (url.IndexOf(s) > 0 || s == "*") return true;
            }
            return false;
        }
        private void SaveHtm(string html, string url, string requestInfo, bool isWebService)
        {
            string logFile = Path.Combine(LogsFolder, DateTime.Now.ToString("yyyy-MM-dd"));
            string soapAction = System.Web.HttpContext.Current.Request.Headers["SOAPAction"];
            string currentPage = "Unkown";
            string error = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    if (isWebService)
                    {
                        logFile = Path.Combine(logFile, "WebServices");
                        if (string.IsNullOrEmpty(soapAction))
                        {
                            logFile = Path.Combine(logFile, GetValueForSplit(url, '/', 2).Split('.')[0]);
                            currentPage = GetValueForSplit(url, '/', 1).Split('.')[0];
                        }
                        else
                        {
                            logFile = Path.Combine(logFile, GetValueForSplit(url, '/', 1).Split('.')[0]);
                            currentPage = GetValueForSplit(soapAction, '/', 1).Replace("\"", "");
                        }
                    }
                    else
                    {
                        logFile = Path.Combine(logFile, "Pages");
                        int paramIndex = url.IndexOf("?");
                        if (paramIndex > 0) url = url.Substring(0, paramIndex);
                        currentPage = string.Concat(GetValueForSplit(url, '/', 1).Split('.')[0], DateTime.Now.ToString(" - (HH mm ss)"));
                    }
                    if (!System.IO.Directory.Exists(logFile))
                    {
                        System.IO.Directory.CreateDirectory(logFile);
                    }
                }
            }
            catch (Exception ex)
            {
                currentPage = "Exception";
                error = string.Concat("Url:", url, "\r\n", ex.ToString());
                if (!System.IO.Directory.Exists(logFile))
                {
                    System.IO.Directory.CreateDirectory(logFile);
                }
            }

            logFile = Path.Combine(logFile, currentPage);
            logFile += ".txt";
            using (FileStream fs = new FileStream(logFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                StreamWriter w = new StreamWriter(fs);
                if (!string.IsNullOrEmpty(error))
                {
                    w.WriteLine(error);
                }
                w.WriteLine(LogSplit);
                w.WriteLine(DateTime.Now);
                w.WriteLine("Content Start:");
                w.WriteLine("Request:");
                w.WriteLine(requestInfo);
                w.WriteLine(GetTogether.Web.WebHelper.GetRequestInfo(HttpContext.Current.Request));
                w.WriteLine("Response:");
                w.WriteLine(html);
                w.WriteLine("Content End");
                w.Flush();
            }
        }

        private void context_EndRequest(object sender, EventArgs e)
        {
            string url = HttpContext.Current.Request.Url.ToString();
            if (!string.IsNullOrEmpty(LogsFolder) && IsTracePage(url))
            {
                PageFilter filter = HttpContext.Current.Response.Filter as PageFilter;
                if (filter == null) return;
                RequestState rs = EnsureRequestState();
                SaveHtm(filter.ResponseContent.ToString(), url, rs.RequestContent, HttpContext.Current.Request.CurrentExecutionFilePath.ToLower().IndexOf(".asmx") > 0);
            }
        }

        public string GetRequestString(HttpContext context)
        {
            System.IO.Stream stream = context.Request.InputStream;
            stream.Position = 0;
            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            StringBuilder sbRequest = new StringBuilder();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                sbRequest.Append(line);
            }
            return sbRequest.ToString();
        }
    }
}