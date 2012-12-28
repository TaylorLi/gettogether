using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class WebService_Components_ProjectGroup : GetTogether.Studio.Web.UI.Control
{
    public GetTogether.Studio.WebService.Projects Projects;
    protected void Page_Load(object sender, EventArgs e)
    {
        Projects = GetTogether.Studio.WebService.Projects.GetProjects(CurrentSession.UserCode);
        Dictionary<string, GetTogether.Studio.WebService.Projects> projectGrouped = new Dictionary<string, GetTogether.Studio.WebService.Projects>();
        foreach (GetTogether.Studio.WebService.Project p in Projects)
        {
            if (!projectGrouped.ContainsKey(p.Parameter.Category)) projectGrouped[p.Parameter.Category] = new GetTogether.Studio.WebService.Projects();
            projectGrouped[p.Parameter.Category].Add(p);
        }
        int index = 1;
        foreach (string key in projectGrouped.Keys)
        {
            WebService_Components_Projects controlProject = (WebService_Components_Projects)Page.LoadControl("~/WebService/Components/Projects.ascx");
            controlProject.Projects = projectGrouped[key];
            bool isShowAll = GetTogether.Utility.NumberHelper.ToInt(Request["show"], 0) == 1;
            controlProject.IsShow = (index < 2 || isShowAll);
            Response.Write(controlProject.Html);
            index++;
        }
    }
}