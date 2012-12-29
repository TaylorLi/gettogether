using System;
using GetTogether;

namespace GetTogether.Data
{
    [Serializable]
    public class ConnectionInformation
    {
        #region Constructors
        public ConnectionInformation()
        {

        }

        public ConnectionInformation(string tableName, string connKey)
        {
            this.TableName = tableName;
            this.ConnectionKey = connKey;
        }
        public ConnectionInformation(string tableName, string connKey, string[] pirmaryKeys)
        {
            this.TableName = tableName;
            this.ConnectionKey = connKey;
            this.PrimaryKeys = pirmaryKeys;
        }

        public ConnectionInformation(string tableName, string connKey, DatabaseType dbType)
        {
            this.TableName = tableName;
            this.ConnectionKey = connKey;
            this.DbType = dbType;
        }
        public ConnectionInformation(string tableName, string connKey, string[] pirmaryKeys,DatabaseType dbType)
        {
            this.TableName = tableName;
            this.ConnectionKey = connKey;
            this.PrimaryKeys = pirmaryKeys;
            this.DbType = dbType;
        }

        #endregion

        private DatabaseType _DbType;

        public DatabaseType DbType
        {
            get { return _DbType; }
            set { _DbType = value; }
        }


        #region Attribute
        private string _TableName;
        private string _DefaultSelect;
        private string _ConnectionKey;
        private bool _IsSqlSentence;

        public bool IsSqlSentence
        {
            get { return _IsSqlSentence; }
            set
            {
                _IsSqlSentence = value;
                if (value)
                {
                    this._DefaultSelect = _TableName;
                }
            }
        }

        public string TableName
        {
            get
            {
                return this._TableName;
            }
            set
            {
                this._TableName = value;
            }
        }

        public string DefaultSelect
        {
            get
            {
                if (string.IsNullOrEmpty(_DefaultSelect))
                    _DefaultSelect = DbUtil.GetSelectString(_TableName);
                return _DefaultSelect;
            }
        }

        public string ConnectionKey
        {
            get { return _ConnectionKey; }
            set { _ConnectionKey = value; }
        }

        private string[] _PrimaryKeys;

        public string[] PrimaryKeys
        {
            get { return _PrimaryKeys; }
            set { _PrimaryKeys = value; }
        }

        private string _ConnectionString;

        public string ConnectionString
        {
            get { return _ConnectionString; }
            set { _ConnectionString = value; }
        }

        private bool _IsNolock = true;

        public bool IsNolock
        {
            get { return _IsNolock; }
            set { _IsNolock = value; }
        }

        public System.Data.IDbConnection Connection
        {
            get
            {
                if (string.IsNullOrEmpty(_ConnectionString))
                {
                    return ConnectionHelper.CreateConnectionByKey(this._ConnectionKey, this.DbType);
                }
                else
                {
                    return ConnectionHelper.CreateConnection(this._ConnectionString, this.DbType);
                }
            }
        }

        #endregion
    }
}
