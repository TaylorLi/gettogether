using System;
using System.Data;
using System.Configuration;
using System.Web;

namespace GetTogether.Studio.Web.UI
{
    public class Control : GetTogether.Web.UI.Control
    {
        public SessionObjects CurrentSession
        {
            get { return SessionHelper.GetCurrentSession(); }
        }
        public Control()
        {

        }
    }
}