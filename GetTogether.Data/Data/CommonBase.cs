using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GetTogether.Data;

namespace GetTogether.Data
{
    public class CommonBase<T, C> : ICommonBase<T, C>
        where T : class, new()
        where C : ICollection<T>, new()
    {
        private GetTogether.Data.ConnectionInformation ci;
        private GetTogether.Data.ConnectionInformation ConnInfo
        {
            get
            {
                if (ci == null)
                {
                    ci = GetDefaultConnectionInformation();
                }
                return ci;
            }
        }

        public virtual GetTogether.Data.ConnectionInformation GetDefaultConnectionInformation()
        {
            return null;
        }

        public GetTogether.Data.ConnectionInformation GetCurrentConnectionInformation()
        {
            return ConnInfo;
        }

        public void SetConnectionInformation(GetTogether.Data.ConnectionInformation connInfo)
        {
            this.ci = connInfo;
        }

        public void SetDatabaseType(GetTogether.Data.DatabaseType dbType)
        {
            this.ConnInfo.DbType = dbType;
        }

        public CommonBase()
        {

        }

        #region Query Functions

        /// <summary>
        /// Get database time.
        /// </summary>
        /// <returns>DateTime</returns>
        public DateTime GetDbTime()
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        return GetTogether.Data.SQL.SqlUtil.GetDbTime(conn);
                    case DatabaseType.MySQL:
                        return GetTogether.Data.MySQL.SqlUtil.GetDbTime(conn);
                    default:
                        return DateTime.MinValue;
                }
            }
        }

        /// <summary>
        /// Get records count.
        /// </summary>
        /// <returns>int</returns>
        public int GetRecordsCount()
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                if (ConnInfo.DbType == DatabaseType.SQLServer)
                {
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName).Count;
                    }
                    else
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetExecuteScalar(conn, string.Format(GetTogether.Data.SQL.SqlScriptHandler.Search.SelectRecordCountString, ConnInfo.TableName));
                    }
                }
                else if (ConnInfo.DbType == DatabaseType.MySQL)
                {
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName).Count;
                    }
                    else
                    {
                        return int.Parse(DbUtil.GetExecuteScalar<long>(conn, string.Format(GetTogether.Data.MySQL.SqlScriptHandler.Search.SelectRecordCountString, ConnInfo.TableName), ConnInfo.DbType).ToString());
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Get records by parameter.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>int</returns>
        public int GetRecordsCount(ParameterCollection pc)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:

                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc).Count;
                        }
                        else
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetRecordsCountByParsCollection(conn, ConnInfo.TableName, pc);
                        }

                    case DatabaseType.MySQL:

                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc).Count;
                        }
                        else
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetRecordsCountByParsCollection(conn, ConnInfo.TableName, pc);
                        }

                    default: return 0;
                }
            }
        }

        /// <summary>
        /// Get paging records by parameter list.
        /// </summary>
        /// <param name="parameters">Parameter list</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Order by</param>
        /// <param name="isAsc">Is order by asc</param>
        /// <param name="fields">Return fileds.</param>
        /// <returns>PagingResult</returns>
        public PagingResult<T, C> GetPagingList(ParameterCollection pc, int pageIndex, int pageSize, string sortBy, bool isAsc, params string[] fields)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    return GetPagingList(GetTogether.Data.SQL.SqlScriptHandler.GetConditionsFromParameterCollection(pc), pageIndex, pageSize, sortBy, isAsc, fields);
                case DatabaseType.MySQL:
                    return GetPagingList(GetTogether.Data.MySQL.SqlScriptHandler.GetConditionsFromParameterCollection(pc), pageIndex, pageSize, sortBy, isAsc, fields);
                default: return null;
            }

        }

        /// <summary>
        /// Get paging records by sql conditions.
        /// </summary>
        /// <param name="parameters">Sql conditions</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Order by</param>
        /// <param name="isAsc">Is order by asc</param>
        /// <param name="fields">Return fileds.</param>
        /// <returns>PagingResult</returns>
        public PagingResult<T, C> GetPagingList(string conditions, int pageIndex, int pageSize, string sortBy, bool isAsc, params string[] fields)
        {
            using (IDbConnection conn = ConnInfo.Connection)
            {
                return PagingResult<T, C>.GetPagingList(conn, ConnInfo.TableName, ConnInfo.PrimaryKeys, pageIndex, pageSize, sortBy == null ? null : new string[] { sortBy }, isAsc, conditions, ConnInfo.DbType, fields);
            }
        }

        /// <summary>
        /// Get paging records by parameter list.
        /// </summary>
        /// <param name="parameters">Parameter list</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Order by</param>
        /// <param name="isAsc">Is order by asc</param>
        /// <returns>PagingResult</returns>
        public PagingResult<T, C> GetPagingList(ParameterCollection pc, int pageIndex, int pageSize, string sortBy, bool isAsc)
        {
            return GetPagingList(pc, pageIndex, pageSize, sortBy, isAsc, null);
        }

        /// <summary>
        /// Get paging records by parameter list.
        /// </summary>
        /// <param name="parameters">Parameter list</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Order by</param>
        /// <param name="isAsc">Is order by asc</param>
        /// <returns>PagingResult</returns>
        public PagingResult<T, C> GetPagingList(ParameterCollection pc, int pageIndex, int pageSize, object sortBy, bool isAsc)
        {
            return GetPagingList(pc, pageIndex, pageSize, sortBy.ToString(), isAsc, null);
        }

        public C GetList(int count)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            C c = GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName);
                            return ReturnTop(c, count);
                        }
                        else
                        {
                            return GetList(GetTogether.Data.SQL.SqlScriptHandler.Search.GetSelectTopString(ConnInfo.TableName, count));
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            C c = GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName);
                            return ReturnTop(c, count);
                        }
                        else
                        {
                            return GetList(GetTogether.Data.MySQL.SqlScriptHandler.Search.GetSelectTopString(ConnInfo.TableName, count));
                        }
                    default: return default(C);
                }
            }
        }

        public C GetList(object[] columns, int count)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetList(count);
                    }
                    else
                    {
                        return GetList(GetTogether.Data.SQL.SqlScriptHandler.Search.GetSelectTopString(ConnInfo.TableName, columns, count));
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetList(count);
                    }
                    else
                    {
                        return GetList(GetTogether.Data.MySQL.SqlScriptHandler.Search.GetSelectTopString(ConnInfo.TableName, columns, count));
                    }
                default: return default(C);
            }

        }

        private C ReturnTop(C c, int count)
        {
            C ret = new C();
            int i = 0;
            foreach (T t in c)
            {
                if (i >= count) break;
                ret.Add(t);
                i++;
            }
            return ret;
        }

        public C GetList(int count, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            C c = GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, true, orderBy);
                            return ReturnTop(c, count);
                        }
                        else
                        {
                            return GetList(count, true, orderBy);
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            C c = GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, true, orderBy);
                            return ReturnTop(c, count);
                        }
                        else
                        {
                            return GetList(count, true, orderBy);
                        }
                    default: return default(C);
                }

            }
        }

        public C GetList(int count, params object[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            C c = GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, true, orderBy);
                            return ReturnTop(c, count);
                        }
                        else
                        {
                            return GetList(count, true, orderBy);
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            C c = GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, true, orderBy);
                            return ReturnTop(c, count);
                        }
                        else
                        {
                            return GetList(count, true, orderBy);
                        }
                    default: return default(C);
                }

            }
        }

        public C GetList(int count, bool asc, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            C c = GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                            return ReturnTop(c, count);
                        }
                        else
                        {
                            return GetList(GetTogether.Data.SQL.SqlScriptHandler.Search.GetSelectTopOrderString(ConnInfo.TableName, count, asc, orderBy));
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            C c = GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                            return ReturnTop(c, count);
                        }
                        else
                        {
                            return GetList(GetTogether.Data.MySQL.SqlScriptHandler.Search.GetSelectTopOrderString(ConnInfo.TableName, count, asc, orderBy));
                        }
                    default: return default(C);
                }
            }
        }

        public C GetList(int count, bool asc, params object[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            C c = GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                            return ReturnTop(c, count);
                        }
                        else
                        {
                            return GetList(GetTogether.Data.SQL.SqlScriptHandler.Search.GetSelectTopOrderString(ConnInfo.TableName, count, asc, orderBy));
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            C c = GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                            return ReturnTop(c, count);
                        }
                        else
                        {
                            return GetList(GetTogether.Data.MySQL.SqlScriptHandler.Search.GetSelectTopOrderString(ConnInfo.TableName, count, asc, orderBy));
                        }
                    default: return default(C);
                }

            }
        }

        public C GetAllList()
        {
            return GetList(ConnInfo.DefaultSelect);
        }

        public C GetAllList(object[] columns)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetAllList();
                    }
                    else
                    {
                        return GetList(GetTogether.Data.SQL.SqlScriptHandler.Search.GetSelectString(ConnInfo.TableName, columns));
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetAllList();
                    }
                    else
                    {
                        return GetList(GetTogether.Data.MySQL.SqlScriptHandler.Search.GetSelectString(ConnInfo.TableName, columns));
                    }
                default: return default(C);
            }

        }

        public C GetAllList(string[] columns)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetAllList();
                    }
                    else
                    {
                        return GetList(GetTogether.Data.SQL.SqlScriptHandler.Search.GetSelectString(ConnInfo.TableName, columns));
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetAllList();
                    }
                    else
                    {
                        return GetList(GetTogether.Data.MySQL.SqlScriptHandler.Search.GetSelectString(ConnInfo.TableName, columns));
                    }
                default: return default(C);
            }

        }

        public C GetAllList(bool asc, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                        }
                        else
                        {
                            return GetList(GetTogether.Data.SQL.SqlScriptHandler.Search.GetSelectOrderString(ConnInfo.TableName, asc, orderBy));
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                        }
                        else
                        {
                            return GetList(GetTogether.Data.MySQL.SqlScriptHandler.Search.GetSelectOrderString(ConnInfo.TableName, asc, orderBy));
                        }
                    default: return default(C);
                }

            }
        }

        public C GetAllList(bool asc, params object[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                        }
                        else
                        {
                            return GetAllList(asc, GetTogether.Data.SQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                        }
                        else
                        {
                            return GetAllList(asc, GetTogether.Data.MySQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                        }
                    default: return default(C);
                }

            }
        }

        public C GetAllList(string[] columns, bool asc, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                        }
                        else
                        {
                            return GetList(GetTogether.Data.SQL.SqlScriptHandler.Search.GetSelectOrderString(ConnInfo.TableName, columns, asc, orderBy));
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                        }
                        else
                        {
                            return GetList(GetTogether.Data.MySQL.SqlScriptHandler.Search.GetSelectOrderString(ConnInfo.TableName, columns, asc, orderBy));
                        }
                    default: return default(C);
                }
            }
        }

        public C GetAllList(object[] columns, bool asc, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:

                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                        }
                        else
                        {
                            return GetList(GetTogether.Data.SQL.SqlScriptHandler.Search.GetSelectOrderString(ConnInfo.TableName, columns, asc, orderBy));
                        }
                    case DatabaseType.MySQL:

                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                        }
                        else
                        {
                            return GetList(GetTogether.Data.MySQL.SqlScriptHandler.Search.GetSelectOrderString(ConnInfo.TableName, columns, asc, orderBy));
                        }
                    default: return default(C);
                }
            }
        }

        public C GetList(string cmdText)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, cmdText);
                    case DatabaseType.MySQL:
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, cmdText);
                    default: return default(C);
                }

            }
        }

        public C GetList(ParameterCollection pc)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc);
                        }
                        else
                        {
                            return GetList(pc, null);
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc);
                        }
                        else
                        {
                            return GetList(pc, null);
                        }
                    default: return default(C);
                }
            }
        }

        public C GetList(ParameterCollection pc, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                        }
                        else
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, orderBy);
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                        }
                        else
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, orderBy);
                        }
                    default: return default(C);
                }
            }
        }

        public C GetList(ParameterCollection pc, params object[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                        }
                        else
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, GetTogether.Data.SQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                        }
                        else
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, GetTogether.Data.SQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                        }
                    default: return default(C);
                }
            }
        }

        public C GetList(ParameterCollection pc, int count, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            C c = GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                            return ReturnTop(c, count);
                        }
                        else
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, orderBy);
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            C c = GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                            return ReturnTop(c, count);
                        }
                        else
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, orderBy);
                        }
                    default: return default(C);
                }
            }
        }

        public C GetList(ParameterCollection pc, int count, params object[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            C c = GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                            return ReturnTop(c, count);
                        }
                        else
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, GetTogether.Data.SQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            C c = GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                            return ReturnTop(c, count);
                        }
                        else
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, GetTogether.Data.SQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                        }
                    default: return default(C);
                }
            }
        }

        public C GetList(ParameterCollection pc, bool asc, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                        }
                        else
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, asc, orderBy);
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                        }
                        else
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, asc, orderBy);
                        }
                    default: return default(C);
                }

            }
        }

        public C GetList(ParameterCollection pc, bool asc, params object[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                        }
                        else
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, asc, GetTogether.Data.SQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                        }
                        else
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, asc, GetTogether.Data.SQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                        }
                    default: return default(C);
                }

            }
        }

        public C GetList(ParameterCollection pc, int count, bool asc, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            C c = GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                            return ReturnTop(c, count);
                        }
                        else
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, asc, orderBy);
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            C c = GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                            return ReturnTop(c, count);
                        }
                        else
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, asc, orderBy);
                        }
                    default: return default(C);
                }
            }
        }

        public C GetList(ParameterCollection pc, int count, bool asc, params object[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            C c = GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                            return ReturnTop(c, count);
                        }
                        else
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, asc, GetTogether.Data.SQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            C c = GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                            return ReturnTop(c, count);
                        }
                        else
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, asc, GetTogether.Data.SQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                        }
                    default: return default(C);
                }
            }
        }

        public C GetList(IDbCommand cmd)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                cmd.Connection = conn;
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(cmd);
                    case DatabaseType.MySQL:
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(cmd);
                    default: return default(C);
                }

            }
        }

        #region DataSet

        public DataSet GetDataSet(string cmdText)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        return GetTogether.Data.SQL.SqlUtil.ExecuteDataSet(conn, cmdText);
                    case DatabaseType.MySQL:
                        return GetTogether.Data.MySQL.SqlUtil.ExecuteDataSet(conn, cmdText);
                    default: return null;
                }
            }
        }

        public DataSet GetDataSet()
        {
            return GetDataSet(this.ConnInfo.DefaultSelect);
        }

        public DataSet GetDataSet(ParameterCollection pc)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        return GetTogether.Data.SQL.SqlUtil.ExecuteDataSetByParsCollection(conn, this.ConnInfo.TableName, pc);
                    case DatabaseType.MySQL:
                        return GetTogether.Data.MySQL.SqlUtil.ExecuteDataSetByParsCollection(conn, this.ConnInfo.TableName, pc);
                    default: return null;
                }

            }
        }

        #endregion

        #region Command

        public IDbCommand GetIDbCommand(ParameterCollection pc, string[] columns, bool asc, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, pc, asc, orderBy);
                        }
                        else
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, columns, pc, 0, asc, orderBy);
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, pc, asc, orderBy);
                        }
                        else
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, columns, pc, 0, asc, orderBy);
                        }
                    default: return null;
                }
            }
        }

        public IDbCommand GetIDbCommand(ParameterCollection pc, int top, bool asc, params string[] orderBy)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, pc, asc, orderBy);
                        }
                        else
                        {
                            return GetTogether.Data.SQL.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, pc, top, asc, orderBy);
                        }
                    case DatabaseType.MySQL:
                        if (ConnInfo.IsSqlSentence)
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, pc, asc, orderBy);
                        }
                        else
                        {
                            return GetTogether.Data.MySQL.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, pc, top, asc, orderBy);
                        }
                    default: return null;
                }

            }
        }

        #endregion

        #endregion

        #region Query Functions With Connection

        /// <summary>
        /// Get database time.
        /// </summary>
        /// <returns>DateTime</returns>
        public DateTime GetDbTime(System.Data.IDbConnection conn)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    return GetTogether.Data.SQL.SqlUtil.GetDbTime(conn);
                case DatabaseType.MySQL:
                    return GetTogether.Data.MySQL.SqlUtil.GetDbTime(conn);
                default: return DateTime.Now;
            }

        }

        /// <summary>
        /// Get records count.
        /// </summary>
        /// <returns>int</returns>
        public int GetRecordsCount(System.Data.IDbConnection conn)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName).Count;
                    }
                    else
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetExecuteScalar(conn, string.Format(GetTogether.Data.SQL.SqlScriptHandler.Search.SelectRecordCountString, ConnInfo.TableName));
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName).Count;
                    }
                    else
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetExecuteScalar(conn, string.Format(GetTogether.Data.SQL.SqlScriptHandler.Search.SelectRecordCountString, ConnInfo.TableName));
                    }
                default: return 0;
            }

        }

        /// <summary>
        /// Get records by parameter.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>int</returns>
        public int GetRecordsCount(System.Data.IDbConnection conn, ParameterCollection pc)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc).Count;
                    }
                    else
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetRecordsCountByParsCollection(conn, ConnInfo.TableName, pc);
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc).Count;
                    }
                    else
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetRecordsCountByParsCollection(conn, ConnInfo.TableName, pc);
                    }
                default: return 0;
            }
        }

        /// <summary>
        /// Get paging records by parameter list.
        /// </summary>
        /// <param name="parameters">Parameter list</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Order by</param>
        /// <param name="isAsc">Is order by asc</param>
        /// <param name="fields">Return fileds.</param>
        /// <returns>PagingResult</returns>
        public PagingResult<T, C> GetPagingList(IDbConnection conn, ParameterCollection pc, int pageIndex, int pageSize, string sortBy, bool isAsc, params string[] fields)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    return GetPagingList(conn, GetTogether.Data.SQL.SqlScriptHandler.GetConditionsFromParameterCollection(pc), pageIndex, pageSize, sortBy, isAsc, fields);
                case DatabaseType.MySQL:
                    return GetPagingList(conn, GetTogether.Data.MySQL.SqlScriptHandler.GetConditionsFromParameterCollection(pc), pageIndex, pageSize, sortBy, isAsc, fields);
                default: return new PagingResult<T, C>();
            }
        }

        /// <summary>
        /// Get paging records by sql conditions.
        /// </summary>
        /// <param name="parameters">Sql conditions</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Order by</param>
        /// <param name="isAsc">Is order by asc</param>
        /// <param name="fields">Return fileds.</param>
        /// <returns>PagingResult</returns>
        public PagingResult<T, C> GetPagingList(IDbConnection conn, string conditions, int pageIndex, int pageSize, string sortBy, bool isAsc, params string[] fields)
        {
            return PagingResult<T, C>.GetPagingList(conn, ConnInfo.TableName, ConnInfo.PrimaryKeys, pageIndex, pageSize, sortBy == null ? null : new string[] { sortBy }, isAsc, conditions, ConnInfo.DbType, fields);
        }

        /// <summary>
        /// Get paging records by parameter list.
        /// </summary>
        /// <param name="parameters">Parameter list</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Order by</param>
        /// <param name="isAsc">Is order by asc</param>
        /// <returns>PagingResult</returns>
        public PagingResult<T, C> GetPagingList(IDbConnection conn, ParameterCollection pc, int pageIndex, int pageSize, string sortBy, bool isAsc)
        {
            return GetPagingList(conn, pc, pageIndex, pageSize, sortBy, isAsc, null);
        }

        /// <summary>
        /// Get paging records by parameter list.
        /// </summary>
        /// <param name="parameters">Parameter list</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Order by</param>
        /// <param name="isAsc">Is order by asc</param>
        /// <returns>PagingResult</returns>
        public PagingResult<T, C> GetPagingList(IDbConnection conn, ParameterCollection pc, int pageIndex, int pageSize, object sortBy, bool isAsc)
        {
            return GetPagingList(conn, pc, pageIndex, pageSize, sortBy.ToString(), isAsc, null);
        }

        public C GetList(IDbConnection conn, int count)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        C c = GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName);
                        return ReturnTop(c, count);
                    }
                    else
                    {
                        return GetList(conn, GetTogether.Data.SQL.SqlScriptHandler.Search.GetSelectTopString(ConnInfo.TableName, count));
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        C c = GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName);
                        return ReturnTop(c, count);
                    }
                    else
                    {
                        return GetList(conn, GetTogether.Data.MySQL.SqlScriptHandler.Search.GetSelectTopString(ConnInfo.TableName, count));
                    }
                default: return default(C);
            }
        }

        public C GetList(IDbConnection conn, object[] columns, int count)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetList(conn, count);
                    }
                    else
                    {
                        return GetList(conn, GetTogether.Data.SQL.SqlScriptHandler.Search.GetSelectTopString(ConnInfo.TableName, columns, count));
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetList(conn, count);
                    }
                    else
                    {
                        return GetList(conn, GetTogether.Data.MySQL.SqlScriptHandler.Search.GetSelectTopString(ConnInfo.TableName, columns, count));
                    }
                default: return default(C);
            }
        }

        public C GetList(IDbConnection conn, int count, params string[] orderBy)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        C c = GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, true, orderBy);
                        return ReturnTop(c, count);
                    }
                    else
                    {
                        return GetList(count, true, orderBy);
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        C c = GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, true, orderBy);
                        return ReturnTop(c, count);
                    }
                    else
                    {
                        return GetList(count, true, orderBy);
                    }
                default: return default(C);
            }
        }

        public C GetList(IDbConnection conn, int count, params object[] orderBy)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        C c = GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, true, orderBy);
                        return ReturnTop(c, count);
                    }
                    else
                    {
                        return GetList(count, true, orderBy);
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        C c = GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, true, orderBy);
                        return ReturnTop(c, count);
                    }
                    else
                    {
                        return GetList(count, true, orderBy);
                    }
                default: return default(C);
            }
        }

        public C GetList(IDbConnection conn, int count, bool asc, params string[] orderBy)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        C c = GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                        return ReturnTop(c, count);
                    }
                    else
                    {
                        return GetList(conn, GetTogether.Data.SQL.SqlScriptHandler.Search.GetSelectTopOrderString(ConnInfo.TableName, count, asc, orderBy));
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        C c = GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                        return ReturnTop(c, count);
                    }
                    else
                    {
                        return GetList(conn, GetTogether.Data.MySQL.SqlScriptHandler.Search.GetSelectTopOrderString(ConnInfo.TableName, count, asc, orderBy));
                    }
                default: return default(C);
            }
        }

        public C GetList(IDbConnection conn, int count, bool asc, params object[] orderBy)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        C c = GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                        return ReturnTop(c, count);
                    }
                    else
                    {
                        return GetList(conn, GetTogether.Data.SQL.SqlScriptHandler.Search.GetSelectTopOrderString(ConnInfo.TableName, count, asc, orderBy));
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        C c = GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                        return ReturnTop(c, count);
                    }
                    else
                    {
                        return GetList(conn, GetTogether.Data.MySQL.SqlScriptHandler.Search.GetSelectTopOrderString(ConnInfo.TableName, count, asc, orderBy));
                    }
                default: return default(C);
            }
        }

        public C GetAllList(IDbConnection conn)
        {
            return GetList(conn, ConnInfo.DefaultSelect);
        }

        public C GetAllList(IDbConnection conn, object[] columns)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetAllList(conn);
                    }
                    else
                    {
                        return GetList(conn, GetTogether.Data.SQL.SqlScriptHandler.Search.GetSelectString(ConnInfo.TableName, columns));
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetAllList(conn);
                    }
                    else
                    {
                        return GetList(conn, GetTogether.Data.MySQL.SqlScriptHandler.Search.GetSelectString(ConnInfo.TableName, columns));
                    }
                default: return default(C);
            }
        }

        public C GetAllList(IDbConnection conn, string[] columns)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetAllList(conn);
                    }
                    else
                    {
                        return GetList(conn, GetTogether.Data.SQL.SqlScriptHandler.Search.GetSelectString(ConnInfo.TableName, columns));
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetAllList(conn);
                    }
                    else
                    {
                        return GetList(conn, GetTogether.Data.MySQL.SqlScriptHandler.Search.GetSelectString(ConnInfo.TableName, columns));
                    }
                default: return default(C);
            }
        }

        public C GetAllList(IDbConnection conn, bool asc, params string[] orderBy)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                    }
                    else
                    {
                        return GetList(conn, GetTogether.Data.SQL.SqlScriptHandler.Search.GetSelectOrderString(ConnInfo.TableName, asc, orderBy));
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                    }
                    else
                    {
                        return GetList(conn, GetTogether.Data.MySQL.SqlScriptHandler.Search.GetSelectOrderString(ConnInfo.TableName, asc, orderBy));
                    }
                default: return default(C);
            }
        }

        public C GetAllList(IDbConnection conn, bool asc, params object[] orderBy)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                    }
                    else
                    {
                        return GetAllList(conn, asc, GetTogether.Data.SQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                    }
                    else
                    {
                        return GetAllList(conn, asc, GetTogether.Data.MySQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                    }
                default: return default(C);
            }
        }

        public C GetAllList(IDbConnection conn, string[] columns, bool asc, params string[] orderBy)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                    }
                    else
                    {
                        return GetList(conn, GetTogether.Data.SQL.SqlScriptHandler.Search.GetSelectOrderString(ConnInfo.TableName, columns, asc, orderBy));
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                    }
                    else
                    {
                        return GetList(conn, GetTogether.Data.MySQL.SqlScriptHandler.Search.GetSelectOrderString(ConnInfo.TableName, columns, asc, orderBy));
                    }
                default: return default(C);
            }
        }

        public C GetAllList(IDbConnection conn, object[] columns, bool asc, params string[] orderBy)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                    }
                    else
                    {
                        return GetList(conn, GetTogether.Data.SQL.SqlScriptHandler.Search.GetSelectOrderString(ConnInfo.TableName, columns, asc, orderBy));
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, null, asc, orderBy);
                    }
                    else
                    {
                        return GetList(conn, GetTogether.Data.MySQL.SqlScriptHandler.Search.GetSelectOrderString(ConnInfo.TableName, columns, asc, orderBy));
                    }
                default: return default(C);
            }
        }

        public C GetList(IDbConnection conn, string cmdText)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, cmdText);
                case DatabaseType.MySQL:
                    return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, cmdText);
                default: return default(C);
            }
        }

        public C GetList(IDbConnection conn, ParameterCollection pc)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc);
                    }
                    else
                    {
                        return GetList(conn, pc, null);
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc);
                    }
                    else
                    {
                        return GetList(conn, pc, null);
                    }
                default: return default(C);
            }
        }

        public C GetList(IDbConnection conn, ParameterCollection pc, params string[] orderBy)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                    }
                    else
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, orderBy);
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                    }
                    else
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, orderBy);
                    }
                default: return default(C);
            }
        }

        public C GetList(IDbConnection conn, ParameterCollection pc, params object[] orderBy)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                    }
                    else
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, GetTogether.Data.SQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                    }
                    else
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, GetTogether.Data.SQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                    }
                default: return default(C);
            }
        }

        public C GetList(IDbConnection conn, ParameterCollection pc, int count, params string[] orderBy)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        C c = GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                        return ReturnTop(c, count);
                    }
                    else
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, orderBy);
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        C c = GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                        return ReturnTop(c, count);
                    }
                    else
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, orderBy);
                    }
                default: return default(C);
            }
        }

        public C GetList(IDbConnection conn, ParameterCollection pc, int count, params object[] orderBy)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        C c = GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                        return ReturnTop(c, count);
                    }
                    else
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, GetTogether.Data.SQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        C c = GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, true, orderBy);
                        return ReturnTop(c, count);
                    }
                    else
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, GetTogether.Data.SQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                    }
                default: return default(C);
            }
        }

        public C GetList(IDbConnection conn, ParameterCollection pc, bool asc, params string[] orderBy)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                    }
                    else
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, asc, orderBy);
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                    }
                    else
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, asc, orderBy);
                    }
                default: return default(C);
            }
        }

        public C GetList(IDbConnection conn, ParameterCollection pc, bool asc, params object[] orderBy)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                    }
                    else
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, asc, GetTogether.Data.SQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                    }
                    else
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, 0, asc, GetTogether.Data.SQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                    }
                default: return default(C);
            }
        }

        public C GetList(IDbConnection conn, ParameterCollection pc, int count, bool asc, params string[] orderBy)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        C c = GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                        return ReturnTop(c, count);
                    }
                    else
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, asc, orderBy);
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        C c = GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                        return ReturnTop(c, count);
                    }
                    else
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, asc, orderBy);
                    }
                default: return default(C);
            }
        }

        public C GetList(IDbConnection conn, ParameterCollection pc, int count, bool asc, params object[] orderBy)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        C c = GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                        return ReturnTop(c, count);
                    }
                    else
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, asc, GetTogether.Data.SQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        C c = GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, asc, orderBy);
                        return ReturnTop(c, count);
                    }
                    else
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(conn, ConnInfo.TableName, pc, count, asc, GetTogether.Data.SQL.SqlScriptHandler.ObjectsToStrings(orderBy));
                    }
                default: return default(C);
            }
        }

        public C GetList(IDbConnection conn, IDbCommand cmd)
        {
            cmd.Connection = conn;
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    return GetTogether.Data.SQL.SqlUtil.GetObjectList<T, C>(cmd);
                case DatabaseType.MySQL:
                    return GetTogether.Data.MySQL.SqlUtil.GetObjectList<T, C>(cmd);
                default: return default(C);
            }

        }

        #region DataSet

        public DataSet GetDataSet(IDbConnection conn, string cmdText)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    return GetTogether.Data.SQL.SqlUtil.ExecuteDataSet(conn, cmdText);
                case DatabaseType.MySQL:
                    return GetTogether.Data.MySQL.SqlUtil.ExecuteDataSet(conn, cmdText);
                default: return null;
            }
        }

        public DataSet GetDataSet(IDbConnection conn)
        {
            return GetDataSet(conn, this.ConnInfo.DefaultSelect);
        }

        public DataSet GetDataSet(IDbConnection conn, ParameterCollection pc)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    return GetTogether.Data.SQL.SqlUtil.ExecuteDataSetByParsCollection(conn, this.ConnInfo.TableName, pc);
                case DatabaseType.MySQL:
                    return GetTogether.Data.MySQL.SqlUtil.ExecuteDataSetByParsCollection(conn, this.ConnInfo.TableName, pc);
                default: return null;
            }
        }

        #endregion

        #region Command

        public IDbCommand GetIDbCommand(IDbConnection conn, ParameterCollection pc, string[] columns, bool asc, params string[] orderBy)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, pc, asc, orderBy);
                    }
                    else
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, columns, pc, 0, asc, orderBy);
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, pc, asc, orderBy);
                    }
                    else
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, columns, pc, 0, asc, orderBy);
                    }
                default: return null;
            }
        }

        public IDbCommand GetIDbCommand(IDbConnection conn, ParameterCollection pc, int top, bool asc, params string[] orderBy)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, pc, asc, orderBy);
                    }
                    else
                    {
                        return GetTogether.Data.SQL.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, pc, top, asc, orderBy);
                    }
                case DatabaseType.MySQL:
                    if (ConnInfo.IsSqlSentence)
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, pc, asc, orderBy);
                    }
                    else
                    {
                        return GetTogether.Data.MySQL.SqlUtil.GetIDbCommand(conn, ConnInfo.TableName, pc, top, asc, orderBy);
                    }
                default: return null;
            }
        }

        #endregion

        #endregion

        #region Other Functions

        /// <summary>
        /// Get IDbconnection by this connection key.
        /// </summary>
        /// <returns>IDbConnection</returns>
        public IDbConnection GetConnection()
        {
            if (!string.IsNullOrEmpty(this.ConnInfo.ConnectionString))
                return ConnectionHelper.CreateConnection(this.ConnInfo.ConnectionString, this.ConnInfo.DbType);
            else
                return ConnectionHelper.CreateConnectionByKey(this.ConnInfo.ConnectionKey, this.ConnInfo.DbType);
        }

        public DataTable GetTableSchema()
        {
            string key = this.ToString();
            if (!Stater.DataTableSchema.ContainsKey(key))
            {
                string sql = string.Concat(this.ConnInfo.DefaultSelect, this.ConnInfo.DefaultSelect.ToLower().IndexOf(" where ") > 0 ? " and " : " where ", "1=2");
                Stater.DataTableSchema[key] = GetDataSet(sql).Tables[0];
            }
            return Stater.DataTableSchema[key];
        }

        #endregion
    }
}
