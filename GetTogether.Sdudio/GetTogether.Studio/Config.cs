using System;
using System.Collections.Generic;
using System.Text;
using GetTogether.Utility;

namespace GetTogether.Studio
{
    public class Config
    {
        public static GetTogether.Utility.ConfigManager.SystemMode Mode = GetTogether.Utility.ConfigManager.SystemMode.DEV;
        public static OriginalSettings Original = new OriginalSettings();

        public static string[] RecipientEmails;

        #region Functions

        public static void InitConfig()
        {
            Config.Mode = (GetTogether.Utility.ConfigManager.SystemMode)NumberHelper.ToInt(ConfigHelper.GetAppSetting("Mode"), -1);
            Original.ReadSetting();
            Original.InitSetting();
            Original.ManageFileUploadPath = ProcessPath(Original.ManageFileUploadPath);
        }

        public static string ProcessPath(string path)
        {
            if (path.StartsWith("~"))
            {
                path = System.Web.HttpContext.Current.Server.MapPath(path);
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
            }
            return path;
        }
        #endregion

    }
}
