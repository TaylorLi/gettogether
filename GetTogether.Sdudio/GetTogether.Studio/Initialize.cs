using System;
using System.Collections.Generic;
using System.Text;
using GetTogether.Utility;

namespace GetTogether.Studio
{
    public class Initialize
    {
        public static void SetInitialize()
        {
            SetAppSettings();
            LogHelper.SetConfig(System.Web.HttpContext.Current.Server.MapPath(Config.Original.LogConfig));
        }
        public static void SetAppSettings()
        {
            Config.InitConfig();
        }
    }
}
