using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Collections.Generic;

namespace GetTogether.Studio.Web.UI
{
    public class ControlPaging : GetTogether.Web.UI.ControlPaging
    {
        public SessionObjects CurrentSession
        {
            get { return SessionHelper.GetCurrentSession(); }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Total == 0)
            {
                Response.Clear();
                Response.Write(Web.HtmlHelper.MsgBoxHtml(GetTogether.Resource.Language.Record_not_found, this.Page));
                Response.End();
            }
            base.OnLoad(e);
        }
    }
}