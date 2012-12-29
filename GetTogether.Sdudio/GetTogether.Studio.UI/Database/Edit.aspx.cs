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
using System.Text.RegularExpressions;
using GetTogether.Studio.Database;

public partial class MS_MSSQL : GetTogether.Studio.Web.UI.Page
{
    public GetTogether.Studio.Database.ProjectParameter Parameter;
    public string ProjectName
    {
        get
        {
            return Request["pn"];
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = ProjectName;
        Parameter = GetTogether.Studio.Database.ProjectParameter.GetSettingsByProjectName(CurrentSession.UserCode, ProjectName);
        if (Parameter == null)
        {
            Response.Redirect("Index.aspx");
        }
        ProjectHistory ph = GetTogether.Studio.Database.Projects.GetProjectHistory(ProjectParameter.GetSettingsPath(CurrentSession.UserCode), ProjectName);
        if (ph == null)
        {
            ph = new ProjectHistory();
        }
        ph.RecentUsed = DateTime.Now;
        GetTogether.Utility.FileHelper.SerializeToFile(ph, GetTogether.Studio.Database.Projects.GetProjectHistoryFile(ProjectParameter.GetSettingsPath(CurrentSession.UserCode), ProjectName, true));
    }

    public string GetDefaultStoreProcedure()
    {
        switch (Parameter.DatabaseTypeForCodeEngineer)
        {
            case GetTogether.Data.DatabaseType.MySQL:
                return "";
            case GetTogether.Data.DatabaseType.Oracle:
                break;
            case GetTogether.Data.DatabaseType.SQLServer:
                return "sp_who";
            default:
                break;
        }
        return "";
    }
}


