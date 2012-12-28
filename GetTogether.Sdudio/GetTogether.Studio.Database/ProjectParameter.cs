using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Studio.Database
{
    public class ProjectParameter : GetTogether.ObjectBase.SerializationBase<ProjectParameter>
    {
        #region Attributes

        private string _ProjectName;

        public string ProjectName
        {
            get { return _ProjectName; }
            set { _ProjectName = value; }
        }

        private string _ConnectionKey;

        public string ConnectionKey
        {
            get { return _ConnectionKey; }
            set { _ConnectionKey = value; }
        }
        private string _DataObjectNameSpace;

        public string DataObjectNameSpace
        {
            get { return _DataObjectNameSpace; }
            set { _DataObjectNameSpace = value; }
        }
        private string _BusinessObjectNameSpace;

        public string BusinessObjectNameSpace
        {
            get { return _BusinessObjectNameSpace; }
            set { _BusinessObjectNameSpace = value; }
        }

        private string _ConnectionString;

        public string ConnectionString
        {
            get { return _ConnectionString; }
            set { _ConnectionString = value; }
        }

        private string _UnInsertAndUnUpdate;

        public string UnInsertAndUnUpdate
        {
            get { return _UnInsertAndUnUpdate; }
            set { _UnInsertAndUnUpdate = value; }
        }
        private string _UnInsert;

        public string UnInsert
        {
            get { return _UnInsert; }
            set { _UnInsert = value; }
        }
        private string _UnUpdate;

        public string UnUpdate
        {
            get { return _UnUpdate; }
            set { _UnUpdate = value; }
        }
        private string _DatabaseTypeVariables;

        public string DatabaseTypeVariables
        {
            get { return _DatabaseTypeVariables; }
            set { _DatabaseTypeVariables = value; }
        }

        private string _DOPrefix;

        public string DOPrefix
        {
            get { return _DOPrefix; }
            set { _DOPrefix = value; }
        }
        private string _UOPrefix;

        public string UOPrefix
        {
            get { return _UOPrefix; }
            set { _UOPrefix = value; }
        }
        private string _UOListPrefix;

        public string UOListPrefix
        {
            get { return _UOListPrefix; }
            set { _UOListPrefix = value; }
        }
        private string _BOPrefix;

        public string BOPrefix
        {
            get { return _BOPrefix; }
            set { _BOPrefix = value; }
        }

        private GetTogether.Data.DatabaseType _DatabaseTypeForCodeEngineer;

        public GetTogether.Data.DatabaseType DatabaseTypeForCodeEngineer
        {
            get { return _DatabaseTypeForCodeEngineer; }
            set { _DatabaseTypeForCodeEngineer = value; }
        }
        private string _Category;

        public string Category
        {
            get
            {
                if (string.IsNullOrEmpty(_Category) || this._Category == "Unknown")
                {
                    if (!string.IsNullOrEmpty(ConnectionString) && this.ProjectName != "GetTogether")
                    {
                        string[] addressInfo = ConnectionString.Split(new string[] { "Data Source" }, StringSplitOptions.RemoveEmptyEntries);
                        if (addressInfo.Length > 0)
                        {
                            return addressInfo[(addressInfo.Length >= 2 ? 1 : 0)].Split(';')[0].Replace("=", "");
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
            //return string.Concat(System.Web.HttpContext.Current.Server.MapPath("~/."), "\\Include\\Database\\");
            return System.IO.Path.Combine(Setting.GetIncludeFolder(), "Database\\Projects");
        }

        public static string GetSettingsPath(string username)
        {
            return string.Concat(GetSettingsPathPublic(), "\\", username, "\\");
        }

        public static GetTogether.Studio.Database.ProjectParameter GetSettingsByProjectName(string username, string n)
        {
            GetTogether.Studio.Database.ProjectParameter s = new GetTogether.Studio.Database.ProjectParameter();
            string file = string.Concat(GetSettingsPath(username), n);
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
            _ConnectionString = "Data Source=.;Initial Catalog=Master;Persist Security Info=True;User ID=sa;Password=gzuat;Connect Timeout=30";
            _ConnectionKey = "Config.ConnectionKeys.DatabaseKey";
            _BOPrefix = "BO_";
            _UOListPrefix = "UOList_";
            _DOPrefix = "DO_";
            _UOPrefix = "UO_";
            _BusinessObjectNameSpace = "GetTogether.Database.BLL";
            _DataObjectNameSpace = "GetTogether.Database.DAL";
            _DatabaseTypeForCodeEngineer = GetTogether.Data.DatabaseType.SQLServer;
            _DatabaseTypeVariables = "Config.ConnectionKeys.DatabaseType";
            _UnInsert = "UpdateOn,UpdateBy";
            _UnUpdate = "CreateBy";
            _UnInsertAndUnUpdate = "CreateOn";
        }
    }
}
