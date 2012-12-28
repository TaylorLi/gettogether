using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace GetTogether.Data
{
    public interface IStoreProcBase<T, R>
        where T : class, new()
        where R : class, new()
    {
        /// <summary>
        /// Get result object
        /// </summary>
        /// <param name="parameters">IDataParameter[] parameters</param>
        /// <returns>Object</returns>
        R GetResults(IDataParameter[] parameters);
        /// <summary>
        /// Get DataSet result
        /// </summary>
        /// <param name="parameters">IDataParameter[] parameters</param>
        /// <returns>DataSet</returns>
        DataSet GetDataSet(IDataParameter[] parameters);
        /// <summary>
        /// Get DataReader
        /// </summary>
        /// <param name="parameters">IDataParameter[] parameters</param>
        /// <returns>IDataParameter</returns>
        IDataReader GetDataReader(IDataParameter[] parameters);

        GetTogether.Data.StoreProcInformation GetDefaultConnectionInformation();

        GetTogether.Data.StoreProcInformation GetCurrentConnectionInformation();

        void SetConnectionInformation(GetTogether.Data.StoreProcInformation storeProcInfo);

        void SetDatabaseType(GetTogether.Data.DatabaseType dbType);
    }
}
