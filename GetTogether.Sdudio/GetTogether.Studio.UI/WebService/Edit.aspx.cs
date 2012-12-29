using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GetTogether.Studio.WebService;

public partial class WebService_Edit : GetTogether.Studio.Web.UI.Page
{
    public ProjectParameter Parameter;
    public string ProjectName
    {
        get
        {
            return Request["pn"];
        }
    }

    public string Codes = string.Empty;
    private string GetPath(string username)
    {
        string methodName = Request["method"];
        string historyName = Request["history"];
        string sourcePath = System.IO.Path.Combine(string.Concat(GetTogether.Studio.WebService.ProjectParameter.GetSettingsPath(username), Parameter.ProjectName, "(History)"), methodName);
        sourcePath = System.IO.Path.Combine(sourcePath, historyName);
        return sourcePath;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = ProjectName;
        Parameter = ProjectParameter.GetSettingsByProjectName(CurrentSession.UserCode, ProjectName, CurrentSession.ShareUserCode);
        if (Parameter == null)
        {
            Response.Redirect("Index.aspx");
        }
        if (!string.IsNullOrEmpty(CurrentSession.ShareUserCode))
        {
            GetTogether.Utility.DirectoryHelper.CopyParameter cp = new GetTogether.Utility.DirectoryHelper.CopyParameter();
            cp.Source = GetPath(CurrentSession.ShareUserCode);
            if (System.IO.Directory.Exists(cp.Source))
            {
                cp.Destination = GetPath(CurrentSession.UserCode);
                if (!System.IO.Directory.Exists(cp.Destination)) System.IO.Directory.CreateDirectory(cp.Destination);
                cp.IsOverwrite = true;
                GetTogether.Utility.DirectoryHelper.Copy(cp);
            }
        }
        if (!string.IsNullOrEmpty(Request["gc"]))
        {
            Codes = WsdlHelper.GetCodes(Parameter.Address);
            Response.ContentType = "text/plain";
            Response.Write(Codes);
            Response.End();
        }
        else
        {
            ProjectHistory ph = Projects.GetProjectHistory(ProjectParameter.GetSettingsPath(CurrentSession.UserCode), ProjectName);
            if (ph == null)
            {
                ph = new ProjectHistory();
            }
            ph.RecentUsed = DateTime.Now;
            string path = GetTogether.Studio.WebService.Projects.GetProjectHistoryFile(ProjectParameter.GetSettingsPath(CurrentSession.UserCode), ProjectName, true);
            GetTogether.Utility.FileHelper.SerializeToFile(ph, path);
        }
    }
}