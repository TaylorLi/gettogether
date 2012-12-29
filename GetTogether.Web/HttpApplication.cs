using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Web
{
    public class HttpApplication : System.Web.HttpApplication
    {
        protected void Application_Start_Base(object sender, EventArgs e)
        {

        }

        protected void Application_Error_Base(object sender, EventArgs e)
        {
            RequestHandler.Process();
        }

        public void Session_OnStart_Base(object sender, EventArgs e)
        {

        }

        public void Application_BeginRequest_Base(object sender, EventArgs e)
        {
            
        }

        public void Session_End_Base(object sender, EventArgs e)
        {
            GetTogether.Web.SimultaneousLogin.ValidateOnSessionEnd(Application, Session);
        }
    }
}
