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

public partial class Components_MSSQL_Projects : GetTogether.Studio.Web.UI.Control
{
    public GetTogether.Studio.Database.Projects Projects;
    public bool IsShow = false;
    protected void Page_Load(object sender, EventArgs e)
    {
        rptResult.DataSource = Projects;// GetTogether.Studio.Database.Projects.GetProjects(CurrentSession.UserCode);
        rptResult.DataBind();
    }
}