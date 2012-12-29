using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Studio.Database
{
    public class Setting
    {
        public static string GetIncludeFolder()
        {
            string path = GetTogether.Utility.ConfigHelper.GetAppSetting("IncludeFolder");
            if (string.IsNullOrEmpty(path))
                return System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Include");
            else
                return path;
        }
    }
}
