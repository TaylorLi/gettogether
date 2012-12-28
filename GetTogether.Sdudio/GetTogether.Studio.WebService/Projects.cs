using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Studio.WebService
{
    public class Projects : GetTogether.ObjectBase.ListBase<Project>
    {
        public Projects()
        {

        }

        public static Projects GetProjects(string username)
        {
            Projects ts = GetProjectsByPath(GetTogether.Studio.WebService.ProjectParameter.GetSettingsPath(username));
            if (ts == null || ts.Count == 0)
            {
                GetTogether.Utility.DirectoryHelper.Copy(
                    GetTogether.Studio.WebService.ProjectParameter.GetSettingsPathPublic(),
                    GetTogether.Studio.WebService.ProjectParameter.GetSettingsPath(username),
                    false);
                ts = GetProjectsByPath(GetTogether.Studio.WebService.ProjectParameter.GetSettingsPath(username));
            }

            return ts;
        }

        public static Projects GetProjectsByPath(string path)
        {
            if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
            Projects pjs = new Projects();
            foreach (string s in System.IO.Directory.GetFiles(path))
            {
                Project pj = new Project();
                System.IO.FileInfo fi = new System.IO.FileInfo(s);
                ProjectParameter p = new ProjectParameter();
                p = p.FormXml(System.IO.File.ReadAllText(s));
                if (p != null)
                {
                    pj.Parameter = p;
                }
                pj.History = GetProjectHistory(path, fi.Name);
                pjs.Add(pj);
            }
            pjs.SortBy("RecentUsed", false);
            return pjs;
        }

        public static string GetProjectHistoryFile(string path, string projectName, bool isCreateWhenNotExists)
        {
            string dir = string.Concat(System.IO.Path.Combine(path, projectName), "(History)");
            if (isCreateWhenNotExists && !System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
            return string.Concat(dir, "/", projectName, ".History");
        }

        public static ProjectHistory GetProjectHistory(string path, string projectName)
        {
            try
            {
                string historyFile = GetProjectHistoryFile(path, projectName, false);
                ProjectHistory ph = null;
                if (System.IO.File.Exists(historyFile))
                {
                    ph = GetTogether.Utility.SerializationHelper.FromXml<ProjectHistory>(System.IO.File.ReadAllText(historyFile));
                }
                return ph;
            }
            catch (Exception ex)
            {
                GetTogether.Utility.LogHelper.Write(GetTogether.Utility.LogHelper.LogTypes.Error, ex.ToString());
                return null;
            }
        }
    }

    public class Project
    {
        private ProjectParameter _Parameter;

        public ProjectParameter Parameter
        {
            get { return _Parameter; }
            set { _Parameter = value; }
        }

        private ProjectHistory _History;

        public ProjectHistory History
        {
            get { return _History; }
            set { _History = value; }
        }

        public DateTime RecentUsed
        {
            get { return History == null ? DateTime.MinValue : this.History.RecentUsed; }
        }

        public Project()
        {

        }
    }
}
