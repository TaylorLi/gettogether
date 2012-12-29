using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class WebService_Components_InvokeHistory : GetTogether.Studio.Web.UI.Control
{
    public GetTogether.Studio.WebService.ProjectParameter Parameter;
    public string MethodName;
    public InvokeHistorys.InvokeHistory Recent = null;
    public InvokeHistorys Histories = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        string path = GetTogether.Studio.WebService.ProjectParameter.GetSettingsPath(CurrentSession.UserCode);
        path = System.IO.Path.Combine(path, string.Concat(Parameter.ProjectName, "(History)"));
        path = System.IO.Path.Combine(path, MethodName);
        if (System.IO.Directory.Exists(path))
        {
            string[] directories = System.IO.Directory.GetDirectories(path);
            if (directories.Length > 0)
            {
                Histories = new InvokeHistorys();
                foreach (string h in directories)
                {
                    InvokeHistorys.InvokeHistory ih = new InvokeHistorys.InvokeHistory();
                    System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(h);
                    ih.Name = dInfo.Name;
                    ih.ActionTime = GetActionDate(h);
                    if (dInfo.Name.Trim().ToLower().Equals("recent"))
                    {
                        Recent = ih;
                        continue;
                    }
                    Histories.Add(ih);
                }
                if (Histories.Count > 0)
                {
                    Histories.SortBy("ActionTime", false);
                }
                rptResult.DataSource = Histories;
                rptResult.DataBind();
            }
        }
    }
    public DateTime GetActionDate(string directory)
    {
        string[] files = System.IO.Directory.GetFiles(directory);
        if (files.Length > 0)
        {
            return new System.IO.FileInfo(files[0]).LastWriteTime;
        }
        else
        {
            return DateTime.MinValue;
        }
    }
    public string GetActionDateString(DateTime dt)
    {
        if (dt != DateTime.MinValue)
        {
            return string.Concat("<span class='comment'>(", dt.ToString("yyyy/MM/dd HH:mm:ss"), ")</span>");
        }
        else
        {
            return string.Empty;
        }
    }


}
public class InvokeHistorys : GetTogether.ObjectBase.ListBase<InvokeHistorys.InvokeHistory>
{
    public InvokeHistorys()
    {
    }

    public class InvokeHistory
    {
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
        private DateTime _ActionTime;
        public DateTime ActionTime
        {
            get
            {
                return _ActionTime;
            }
            set
            {
                _ActionTime = value;
            }
        }
        public InvokeHistory()
        {
        }
    }
}