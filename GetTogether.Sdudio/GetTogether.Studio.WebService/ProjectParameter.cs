using System;
using System.Collections.Generic;
using System.Web;

namespace GetTogether.Studio.WebService
{
    public enum AddressType
    {
        WebService,
        Normal,
    }
    public class ProjectParameter : GetTogether.ObjectBase.SerializationBase<ProjectParameter>
    {
        #region Attributes

        private string _ProjectName;

        public string ProjectName
        {
            get { return _ProjectName; }
            set { _ProjectName = value; }
        }

        private string _Address;

        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }

        private int _Timeout;

        public int Timeout
        {
            get { return _Timeout; }
            set { _Timeout = value; }
        }
        private AddressType _AddressType;

        public AddressType AddressType
        {
            get { return _AddressType; }
            set { _AddressType = value; }
        }

        private string _Category;

        public string Category
        {
            get
            {
                if (string.IsNullOrEmpty(_Category) || this._Category == "Unknown")
                {
                    if (!string.IsNullOrEmpty(Address) && this.ProjectName != "GetTogether")
                    {
                        string[] addressInfo = Address.Split(new string[] { "//" }, StringSplitOptions.None);
                        if (addressInfo.Length > 0)
                        {
                            return addressInfo[(addressInfo.Length >= 2 ? 1 : 0)].Split('/')[0];
                        }
                        else
                        {
                            return "Unknown";
                        }
                    }
                    else
                        return "Unknown";
                }
                return _Category;
            }
            set { _Category = value; }
        }

        #endregion

        #region Functions

        public static string GetSettingsPathPublic()
        {
            return System.IO.Path.Combine(Setting.GetIncludeFolder(), "WebService\\Projects");
        }

        public static string GetSettingsPath(string username)
        {
            string directory = string.Concat(GetSettingsPathPublic(), "\\", username, "\\");
            if (!System.IO.Directory.Exists(directory)) System.IO.Directory.CreateDirectory(directory);
            return directory;
        }

        public static GetTogether.Studio.WebService.ProjectParameter GetSettingsByProjectName(string username, string projName, string shareUsername)
        {
            GetTogether.Studio.WebService.ProjectParameter s = new GetTogether.Studio.WebService.ProjectParameter();
            string file = string.Concat(GetSettingsPath(username), projName);
            if (!string.IsNullOrEmpty(shareUsername) && !System.IO.File.Exists(file))
            {
                System.IO.File.Copy(string.Concat(GetSettingsPath(shareUsername), projName), file);
            }
            if (!System.IO.File.Exists(file))
            {
                return null;
            }
            s = s.FormXml(System.IO.File.ReadAllText(file));
            return s;
        }

        #endregion

        public ProjectParameter()
        {
            _ProjectName = "GetTogether";
            _Address = "http://www.xxx.com/WebService.asmx";
            _Timeout = GetTogether.Utility.ConfigHelper.GetIntergerSetting("WebServiceTimeout");
            _AddressType = AddressType.WebService;
            _Category = "";
        }
    }
}