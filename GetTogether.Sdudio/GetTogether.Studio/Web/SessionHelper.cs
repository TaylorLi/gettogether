using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Studio.Web
{
    public class SessionHelper
    {
        public static SessionObjects GetCurrentSession()
        {
            SessionObjects sess = (SessionObjects)System.Web.HttpContext.Current.Session[Definition.Session.SESSION_KEY];
            if (sess == null)
            {
                sess = new GetTogether.Studio.Web.SessionObjects();
                sess.UserCode = System.Web.HttpContext.Current.Request.UserHostAddress.Replace("::1", "127.0.0.1");
            }
            return sess;
        }
    }
}
