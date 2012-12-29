using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Web.UI
{
    public class MasterPageOnlineUser : System.Web.UI.MasterPage
    {
        override protected void OnInit(EventArgs e)
        {
            //if (Global.EnableSimultaneousLogin > 0)
            GetTogether.Web.SimultaneousLogin.Validate(Page, "~/Login.aspx?ko=1&lang=" + GetTogether.Utility.MutiLanguage.GetLanguageString());
            base.OnInit(e);
        }
    }
}
