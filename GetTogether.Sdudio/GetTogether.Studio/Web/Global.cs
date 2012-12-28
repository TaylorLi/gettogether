using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Web;
using System.Web.SessionState;
using System.Configuration;
using System.Globalization;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace GetTogether.Studio.Web
{
    public class Global : GetTogether.Web.HttpApplication
    {
        public Global()
        {

        }

        protected void Application_Start(Object sender, EventArgs e)
        {
            base.Application_Start_Base(sender, e);
            GetTogether.Studio.Initialize.SetInitialize();
            GetTogether.Data.Log.InitLogging(string.Concat(Server.MapPath("~/."), "/DA.config"));
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            base.Application_Error_Base(sender, e);
            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Server != null)
            {
                GetTogether.Studio.Logging.Files.Log.Error(System.Web.HttpContext.Current.Server.GetLastError());
            }
        }

        public void Session_OnStart(object sender, EventArgs e)
        {
            base.Session_OnStart_Base(sender, e);
            Session["culture_string"] = "en-us";
        }

        public void Session_End(object sender, EventArgs e)
        {
            base.Session_End_Base(sender, e);
        }
    }
}