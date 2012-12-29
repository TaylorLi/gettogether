using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class WebService_Components_Methods : System.Web.UI.UserControl
{
    private string _Error;

    public new string Error
    {
        get { return _Error; }
        set { _Error = value; }
    }

    public GetTogether.Studio.WebService.ProjectParameter Parameter;

    public System.Collections.Generic.List<System.Reflection.MethodInfo> Methods;

    protected void Page_Load(object sender, EventArgs e)
    {
        rptResult.DataSource = Methods;
        rptResult.DataBind();
    }
}
