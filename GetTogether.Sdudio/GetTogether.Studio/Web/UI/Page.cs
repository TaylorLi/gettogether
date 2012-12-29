using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Studio.Web.UI
{
    public class Page : GetTogether.Web.UI.Page
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
        }
        public SessionObjects CurrentSession
        {
            get { return SessionHelper.GetCurrentSession(); }
        }
        public Page()
        {

        }

    }
}
