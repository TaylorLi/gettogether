using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GetTogether.Studio.Database.DAL;
using GetTogether.Data;

namespace GetTogether.Studio.Database.BLL
{
    public partial class BO_Common
    {
        public static System.Data.IDataReader GetDataReader(ProjectParameter projParam, string sql)
        {
            DO_Common da = new DO_Common();
            ConnectionInformation connInfo = da.GetCurrentConnectionInformation();
            SetConnectionInformation(connInfo, projParam);
            connInfo.TableName = sql;
            connInfo.IsSqlSentence = true;
            da.SetConnectionInformation(connInfo);
            System.Data.IDbConnection conn = da.GetCurrentConnectionInformation().Connection;
            return da.GetIDbCommand(conn, null, 1, true).ExecuteReader();
        }

        public static string GetString(ProjectParameter projParam, string sql, string fieldName)
        {
            using (IDataReader dReader = GetDataReader(projParam, sql))
            {
                StringBuilder sbResult = new StringBuilder();
                while (dReader.Read())
                {
                    if (string.IsNullOrEmpty(fieldName))
                        sbResult.Append(dReader[0]);
                    else
                        sbResult.Append(dReader[fieldName]);
                }
                return sbResult.ToString();
            }
        }

        public static System.Data.IDataReader GetDataReaderByTable(ProjectParameter projParam, string table)
        {
            string sql = "";
            switch (projParam.DatabaseTypeForCodeEngineer)
            {
                case DatabaseType.MySQL:
                    sql = string.Concat("select * from ", table, " limit 1,1");
                    break;
                case DatabaseType.Oracle:
                    break;
                case DatabaseType.SQLServer:
                    sql = string.Concat("select top 1 * from ", table);
                    break;
                default:
                    break;
            }
            return GetDataReader(projParam, sql);
        }

        public static void SetConnectionInformation(ConnectionInformation connInfo, ProjectParameter projParam)
        {
            connInfo.ConnectionString = projParam.ConnectionString;
            connInfo.DbType = projParam.DatabaseTypeForCodeEngineer;
        }

        public static DataSet GetDataSet(ProjectParameter projParam, string sql)
        {
            DO_Common da = new DO_Common();
            ConnectionInformation connInfo = da.GetCurrentConnectionInformation();
            SetConnectionInformation(connInfo, projParam);
            da.SetConnectionInformation(connInfo);
            return da.GetDataSet(sql);
        }

        public static string GetDatabase(ProjectParameter projParam)
        {
            using (IDbConnection conn = GetTogether.Data.ConnectionHelper.CreateConnection(projParam.ConnectionString, projParam.DatabaseTypeForCodeEngineer))
            {
                return conn.Database;
            }
        }
    }
}
