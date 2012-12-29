using System;
using System.Collections.Generic;
using System.Text;
using GetTogether;
using System.Data;

namespace GetTogether.Data
{
    public class DOBase<T, C> : CommonBase<T, C>, IDOBase<T, C>
        where T : class, new()
        where C : ICollection<T>, new()
    {
        private GetTogether.Data.ConnectionInformation ConnInfo
        {
            get
            {
                return GetCurrentConnectionInformation();
            }
        }

        #region Constructors

        public override GetTogether.Data.ConnectionInformation GetDefaultConnectionInformation()
        {
            return base.GetDefaultConnectionInformation();
        }

        public DOBase()
        {

        }

        #endregion

        #region Delete Functions
        public int Delete(IDbConnection cnn, IDbTransaction tran, ParameterCollection pc)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    return GetTogether.Data.SQL.SqlUtil.ExecuteDelete(cnn, tran, ConnInfo.TableName, pc);
                case DatabaseType.MySQL:
                    return GetTogether.Data.MySQL.SqlUtil.ExecuteDelete(cnn, tran, ConnInfo.TableName, pc);
                default: return 0;
            }
        }

        public int Delete(ParameterCollection pc)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        return GetTogether.Data.SQL.SqlUtil.ExecuteDelete(conn, ConnInfo.TableName, pc);
                    case DatabaseType.MySQL:
                        return GetTogether.Data.MySQL.SqlUtil.ExecuteDelete(conn, ConnInfo.TableName, pc);
                    default: return 0;
                }
            }
        }

        public int Delete(IDbConnection conn, ParameterCollection pc)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    return GetTogether.Data.SQL.SqlUtil.ExecuteDelete(conn, ConnInfo.TableName, pc);
                case DatabaseType.MySQL:
                    return GetTogether.Data.MySQL.SqlUtil.ExecuteDelete(conn, ConnInfo.TableName, pc);
                default: return 0;
            }
            
        }

        #endregion

        #region Update Functions

        public int Update(IDbConnection conn, IDbTransaction tran, ParameterCollection pcValues, ParameterCollection pcConditions)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    return GetTogether.Data.SQL.SqlUtil.Update(conn, tran, ConnInfo.TableName, pcValues, pcConditions);
                case DatabaseType.MySQL:
                    return GetTogether.Data.MySQL.SqlUtil.Update(conn, tran, ConnInfo.TableName, pcValues, pcConditions);
                default: return 0;
            }
            
        }

        public int Update(ParameterCollection pcValues, ParameterCollection pcConditions)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        return GetTogether.Data.SQL.SqlUtil.Update(conn, ConnInfo.TableName, pcValues, pcConditions);
                    case DatabaseType.MySQL:
                        return GetTogether.Data.MySQL.SqlUtil.Update(conn, ConnInfo.TableName, pcValues, pcConditions);
                    default: return 0;
                }
            }
        }

        public bool UpdateColumn(object key, object keyValue, object columnName, object columnValue)
        {
            return UpdateColumn(key, keyValue, columnName, columnValue, null);
        }

        public bool UpdateColumn(object key, object keyValue, object columnName, object columnValue, ParameterCollection pcAdditionValues)
        {
            ParameterCollection pcCondition = new ParameterCollection();
            pcCondition.Add(new Parameter(ParameterType.Initial, TokenTypes.Equal, key, keyValue));
            ParameterCollection pcValues = new ParameterCollection();
            pcValues.Add(new Parameter(ParameterType.Initial, TokenTypes.Unknown, columnName, columnValue));
            if (pcAdditionValues != null)
            {
                foreach (Parameter p in pcAdditionValues)
                {
                    pcValues.Add(new Parameter(ParameterType.Initial, TokenTypes.Unknown, p.Column, p.Value));
                }
            }
            return Update(pcValues, pcCondition) > 0;
        }

        public int Update(IDbConnection conn, ParameterCollection pcValues, ParameterCollection pcConditions)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    return GetTogether.Data.SQL.SqlUtil.Update(conn, ConnInfo.TableName, pcValues, pcConditions);
                case DatabaseType.MySQL:
                    return GetTogether.Data.MySQL.SqlUtil.Update(conn, ConnInfo.TableName, pcValues, pcConditions);
                default: return 0;
            }
        }

        public bool UpdateColumn(IDbConnection conn, object key, object keyValue, object columnName, object columnValue)
        {
            return UpdateColumn(conn, key, keyValue, columnName, columnValue, null);
        }

        public bool UpdateColumn(IDbConnection conn, object key, object keyValue, object columnName, object columnValue, ParameterCollection pcAdditionValues)
        {
            ParameterCollection pcCondition = new ParameterCollection();
            pcCondition.Add(new Parameter(ParameterType.Initial, TokenTypes.Equal, key, keyValue));
            ParameterCollection pcValues = new ParameterCollection();
            pcValues.Add(new Parameter(ParameterType.Initial, TokenTypes.Unknown, columnName, columnValue));
            if (pcAdditionValues != null)
            {
                foreach (Parameter p in pcAdditionValues)
                {
                    pcValues.Add(new Parameter(ParameterType.Initial, TokenTypes.Unknown, p.Column, p.Value));
                }
            }
            return Update(conn, pcValues, pcCondition) > 0;
        }

        #endregion

        #region Insert Functions
        public int Insert(ParameterCollection pc)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                return Insert(pc);
            }
        }

        public int Insert(System.Data.IDbConnection conn, ParameterCollection pc)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    return GetTogether.Data.SQL.SqlUtil.ExecuteInsert(conn, ConnInfo.TableName, pc);
                case DatabaseType.MySQL:
                    return GetTogether.Data.MySQL.SqlUtil.ExecuteInsert(conn, ConnInfo.TableName, pc);
                default: return 0;
            }
        }

        public int Insert(IDbConnection cnn, IDbTransaction tran, ParameterCollection pc)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    return GetTogether.Data.SQL.SqlUtil.ExecuteInsert(cnn, tran, ConnInfo.TableName, pc);
                case DatabaseType.MySQL:
                    return GetTogether.Data.MySQL.SqlUtil.ExecuteInsert(cnn, tran, ConnInfo.TableName, pc);
                default: return 0;
            }
        }
        #endregion

    }
}
