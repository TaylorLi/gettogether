using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Data
{
    public class StoreProcInformation
    {
        #region Attributes

        private string _ConnectionKey;
        public string ConnectionKey
        {
            get { return _ConnectionKey; }
            set { _ConnectionKey = value; }
        }

        private string _StoreProcName;
        public string StoreProcName
        {
            get { return _StoreProcName; }
            set { _StoreProcName = value; }
        }

        private string _ConnectionString;

        public string ConnectionString
        {
            get { return _ConnectionString; }
            set { _ConnectionString = value; }
        }

        private DatabaseType _DbType = DatabaseType.SQLServer;

        public DatabaseType DbType
        {
            get { return _DbType; }
            set { _DbType = value; }
        }

        private System.Data.IDbConnection _Connection;

        public System.Data.IDbConnection Connection
        {
            get
            {
                if (_Connection == null)
                {
                    if (string.IsNullOrEmpty(_ConnectionString))
                    {
                        _Connection = ConnectionHelper.CreateConnectionByKey(this._ConnectionKey, this.DbType);
                    }
                    else
                    {
                        _Connection = ConnectionHelper.CreateConnection(this._ConnectionString, this.DbType);
                    }
                }
                return _Connection;
            }
        }


        #endregion

        public StoreProcInformation()
        {

        }

        public StoreProcInformation(string connKey, string storeProcName,DatabaseType dbType)
        {
            this._ConnectionKey = connKey;
            this._StoreProcName = storeProcName;
            this._DbType = dbType;
        }
    }
}
