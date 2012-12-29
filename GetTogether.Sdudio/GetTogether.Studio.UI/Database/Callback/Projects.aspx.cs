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

public partial class Callback_MSSQL_Projects : GetTogether.Studio.Web.UI.PageCallback
{
    public string ProjectName
    {
        get { return Request["pn"]; }
    }

    public string Content
    {
        get { return Request["content"]; }
    }

    public bool IsGetNewProject
    {
        get { return !string.IsNullOrEmpty(Request["get"]); }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        switch (type)
        {
            case 0:
                GetProjects();
                break;
            case 1:
                CreateProjects();
                break;
            case 2:
                EditProjects();
                break;
            case 3:
                DeleteProject();
                break;
            default:
                break;
        }
    }

    #region Functions

    private void GetProjects()
    {
        Control c = Page.LoadControl("~/Database/Components/ProjectGroup.ascx");
        this.Controls.Add(c);
    }

    private string GetProjectContent(string xml, string btnText, string jsFunc)
    {
        return string.Concat(
                        "<div style='padding:3px 3px 3px 4px;'><div class='box' style='background-color:#fff;'><textarea id='txtProject' name='txtProject' style='width:99.5%;height:400px;' cols='50' rows='27'>",
                       xml,
                        "</textarea></div><div id='dv-error' style='color:red;text-align:center;'></div>",
                        "<div style='text-align:center;padding:5px 5px 2px 5px;'><input type='button' onclick=\"",
                        jsFunc,
                        "\" class='btn5' value='",
                        btnText,
                        "' />&nbsp;<input type='button' onclick='CloseMsgBox();' class='btn5' value='Cancel' /></div></div>"
                        );
    }

    private void EditProjects()
    {
        if (IsGetNewProject)
        {
            try { Response.Write(GetProjectContent(ProjectParameter.GetSettingsByProjectName(CurrentSession.UserCode, ProjectName).ToXml(), "Save", "ProjectExec('txtProject',2);")); }
            catch (Exception ex) { Response.Write(ex.Message); }
            Response.End();
        }
        else
        {
            try
            {
                GetTogether.Studio.Database.ProjectParameter pj = new GetTogether.Studio.Database.ProjectParameter();
                pj = pj.FormXml(Content);
                string filePath = string.Concat(GetTogether.Studio.Database.ProjectParameter.GetSettingsPath(CurrentSession.UserCode), pj.ProjectName);
                System.IO.File.WriteAllText(GetTogether.Studio.Database.ProjectParameter.GetSettingsPath(CurrentSession.UserCode) + pj.ProjectName, pj.ToXml());
                Response.Write("OK");
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }

    private void CreateProjects()
    {
        if (IsGetNewProject)
        {
            Response.Write(GetProjectContent(new ProjectParameter().ToXml(), "Create", "ProjectExec('txtProject',1);"));
            Response.End();
        }
        else
        {
            try
            {
                ProjectParameter st = new ProjectParameter();
                st = st.FormXml(Content);
                string filePath = string.Concat(ProjectParameter.GetSettingsPath(CurrentSession.UserCode), st.ProjectName);
                if (System.IO.File.Exists(filePath))
                {
                    Response.Write(string.Concat("The project \"", st.ProjectName, "\" already existed."));
                }
                else
                {
                    System.IO.File.WriteAllText(ProjectParameter.GetSettingsPath(CurrentSession.UserCode) + st.ProjectName, st.ToXml());
                    Response.Write("OK");
                }

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }

    private void DeleteProject()
    {
        try
        {
            string path = GetTogether.Studio.Database.ProjectParameter.GetSettingsPath(CurrentSession.UserCode);
            path = System.IO.Path.Combine(path, Request["pn"]);
            if (System.IO.File.Exists(path))
            {
                string historyPath = string.Concat(path, "(History)");
                if (System.IO.Directory.Exists(historyPath)) System.IO.Directory.Delete(historyPath, true);
                System.IO.File.Delete(path);
            }
            JsonSuccess();
        }
        catch (Exception ex)
        {
            GetTogether.Studio.Logging.Files.Log.Error(ex);
            JsonError(ex.ToString());
        }
    }
    #endregion
}