using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Tools_SVN_BackupChenagedFiles : System.Web.UI.Page
{
    public System.Text.StringBuilder Logs = new System.Text.StringBuilder();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.Form.Count > 0)
        {
            string projectPath = Request.Form["project-path"];
            string changedFiles = Request.Form["changed-files"];
            string backupFolder = Request.Form["backup-folder"];
            if (System.IO.Directory.Exists(projectPath))
            {
                if (!string.IsNullOrEmpty(changedFiles))
                {
                    if (!string.IsNullOrEmpty(backupFolder))
                    {
                        string backupFolderPath = System.IO.Path.Combine(projectPath, backupFolder);
                        if (!System.IO.Directory.Exists(backupFolderPath)) System.IO.Directory.CreateDirectory(backupFolderPath);
                        int index = 0;
                        foreach (string s in changedFiles.Split(new string[] { "\r\n" }, StringSplitOptions.None))
                        {
                            if (string.IsNullOrEmpty(s) || s.Trim() == "") continue;
                            string file = s.Replace("/", "\\").Trim();
                            string filePath = System.IO.Path.Combine(projectPath, file);
                            if (System.IO.File.Exists(filePath))
                            {
                                string[] filePathInfo = s.Trim().Split('/');
                                string backupFolderPathBase = backupFolderPath;
                                for (int i = 0; i < filePathInfo.Length - 1; i++)
                                {
                                    backupFolderPathBase += "\\" + filePathInfo[i];
                                    if (!System.IO.Directory.Exists(backupFolderPathBase))
                                        System.IO.Directory.CreateDirectory(backupFolderPathBase);
                                }
                                string dstFile = System.IO.Path.Combine(backupFolderPath, file);
                                System.IO.File.Copy(filePath, dstFile, true);
                                index++;
                                Logs.Append("Copyed : ").AppendLine(dstFile);
                            }
                        }
                        Logs.Insert(0, string.Format("Total {0} File(s) Copyed\r\n", index.ToString()));
                        if (index > 0)
                        {
                            System.IO.File.WriteAllText(System.IO.Path.Combine(backupFolderPath, "Changed Files.txt"), changedFiles);
                        }
                    }
                    else
                    {
                        Logs.AppendLine("Please input destination directory");
                    }
                }
                else
                {
                    Logs.AppendLine("Please input changed files");
                }
            }
            else
            {
                Logs.AppendLine("Invalid source directory");
            }
        }
    }
}