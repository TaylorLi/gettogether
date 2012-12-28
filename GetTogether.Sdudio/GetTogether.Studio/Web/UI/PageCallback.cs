using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace GetTogether.Studio.Web.UI
{
    public class PageCallback : GetTogether.Web.UI.PageCallback
    {
        public SessionObjects CurrentSession
        {
            get { return SessionHelper.GetCurrentSession(); }
        }
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
        }
    }
}