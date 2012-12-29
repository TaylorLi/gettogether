using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GetTogether.Data;


namespace GetTogether.Data
{
    public class PagingResult<T, C> : IPagingResult<T, C>
        where T : class, new()
        where C : ICollection<T>, new()
    {
        private C _Result;

        public C Result
        {
            get { return _Result; }
            set { _Result = value; }
        }
        private int _Total;

        public int Total
        {
            get { return _Total; }
            set { _Total = value; }
        }
        public PagingResult()
        {

        }

        public static Data.PagingResult<T, C> GetPagingList(IDbConnection conn, string tableName, string[] primaryKeys, int pageIndex, int pageSize, string[] fieldsOrder, bool isAsc, string where, GetTogether.Data.DatabaseType dbType, params string[] fieldsShow)
        {
            PagingResult<T, C> ret = new PagingResult<T, C>();
            switch (dbType)
            {
                case DatabaseType.SQLServer:
                    using (IDataReader idr =
                        GetTogether.Data.SQL.SqlUtil.ExecuteProcedureReader(conn, "sp_Paging",
                            new System.Data.SqlClient.SqlParameter[]{new System.Data.SqlClient.SqlParameter("@TableName", tableName),
                    new System.Data.SqlClient.SqlParameter("@PrimaryKeys", GetTogether.Data.SQL.SqlScriptHandler.ArrayToString(primaryKeys,",",true)),
                    new System.Data.SqlClient.SqlParameter("@PageIndex", pageIndex),
                    new System.Data.SqlClient.SqlParameter("@PageSize", pageSize),
                    new System.Data.SqlClient.SqlParameter("@FieldsShow", (fieldsShow==null)?"*":GetTogether.Data.SQL.SqlScriptHandler.ArrayToString(fieldsShow,",",true)),
                    new System.Data.SqlClient.SqlParameter("@FieldsOrder", fieldsOrder==null?string.Empty:string.Concat(GetTogether.Data.SQL.SqlScriptHandler.ArrayToString(fieldsOrder,",",true),(!isAsc?" desc":""))),
                    new System.Data.SqlClient.SqlParameter("@Where", where)}))
                    {
                        ret.Result = Mapping.ObjectHelper.FillCollection<T, C>(idr);
                        if (idr.NextResult())
                        {
                            if (idr.Read())
                            {
                                ret.Total = (int)idr[0];
                            }
                        }
                    }
                    if (ret.Total == 0)
                    {
                        ret.Total = ret.Result.Count;
                    }
                    return ret;
                case DatabaseType.MySQL:
                    using (IDataReader idr =
                        GetTogether.Data.MySQL.SqlUtil.ExecuteProcedureReader(conn, "sp_Paging",
                            new MySql.Data.MySqlClient.MySqlParameter[]{new MySql.Data.MySqlClient.MySqlParameter("?TableName", tableName),
                    new MySql.Data.MySqlClient.MySqlParameter("?PrimaryKeys", GetTogether.Data.MySQL.SqlScriptHandler.ArrayToString(primaryKeys,",",true)),
                    new MySql.Data.MySqlClient.MySqlParameter("?PageIndex", pageIndex),
                    new MySql.Data.MySqlClient.MySqlParameter("?PageSize", pageSize),
                    new MySql.Data.MySqlClient.MySqlParameter("?FieldsShow", (fieldsShow==null)?"*":GetTogether.Data.MySQL.SqlScriptHandler.ArrayToString(fieldsShow,",",true)),
                    new MySql.Data.MySqlClient.MySqlParameter("?FieldsOrder",fieldsOrder==null?string.Empty:GetTogether.Data.MySQL.SqlScriptHandler.ArrayToString(fieldsOrder,",",true)),
                    new MySql.Data.MySqlClient.MySqlParameter("?IsDesc",(isAsc?0:1)),
                    new MySql.Data.MySqlClient.MySqlParameter("?Conditions", where),
                    new MySql.Data.MySqlClient.MySqlParameter("?QueryType",0)}))
                    {
                        ret.Result = Mapping.ObjectHelper.FillCollection<T, C>(idr);
                        if (idr.NextResult())
                        {
                            if (idr.Read())
                            {
                                ret.Total = int.Parse(idr[0].ToString());
                            }
                        }
                    }
                    if (ret.Total == 0)
                    {
                        ret.Total = ret.Result.Count;
                    }
                    return ret;
                default: return null;
            }

        }
    }
}
