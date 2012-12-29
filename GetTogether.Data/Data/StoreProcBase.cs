using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GetTogether.Data;

namespace GetTogether.Data
{
    public abstract class StoreProcBase<T, R> : IStoreProcBase<T, R>
        where T : class, new()
        where R : class, new()
    {
        #region Store procedure information.

        private GetTogether.Data.StoreProcInformation spi;
        private GetTogether.Data.StoreProcInformation StoreProcInfo
        {
            get
            {
                if (spi == null)
                {
                    spi = GetDefaultConnectionInformation();
                }
                return spi;
            }
        }

        public virtual GetTogether.Data.StoreProcInformation GetDefaultConnectionInformation()
        {
            return null;
        }

        public GetTogether.Data.StoreProcInformation GetCurrentConnectionInformation()
        {
            return StoreProcInfo;
        }

        public void SetConnectionInformation(GetTogether.Data.StoreProcInformation storeProcInfo)
        {
            this.spi = storeProcInfo;
        }

        public void SetDatabaseType(GetTogether.Data.DatabaseType dbType)
        {
            this.StoreProcInfo.DbType = dbType;
        }


        #endregion

        #region Query functions

        public abstract R GetResults(IDataParameter[] parameters);

        public virtual DataSet GetDataSet(IDataParameter[] parameters)
        {
            switch (StoreProcInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    using (this.StoreProcInfo.Connection)
                    {
                        return GetTogether.Data.SQL.SqlUtil.ExecuteProcedureDataSet(StoreProcInfo.Connection, StoreProcInfo.StoreProcName, parameters);
                    }
                case DatabaseType.MySQL:
                    using (this.StoreProcInfo.Connection)
                    {
                        return GetTogether.Data.MySQL.SqlUtil.ExecuteProcedureDataSet(StoreProcInfo.Connection, StoreProcInfo.StoreProcName, parameters);
                    }
                case DatabaseType.Oracle:
                    break;
                default:
                    break;
            }
            return null;
        }

        public virtual IDataReader GetDataReader(IDataParameter[] parameters)
        {
            switch (StoreProcInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    return GetTogether.Data.SQL.SqlUtil.ExecuteProcedureReader(StoreProcInfo.Connection, StoreProcInfo.StoreProcName, parameters);
                case DatabaseType.MySQL:
                    return GetTogether.Data.MySQL.SqlUtil.ExecuteProcedureReader(StoreProcInfo.Connection, StoreProcInfo.StoreProcName, parameters);
                case DatabaseType.Oracle:
                    break;
                default:
                    break;
            }
            return null;
        }

        #endregion
    }
}
