using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace GetTogether.Data
{
    public class DbUtil
    {
        public static T GetDbObject<T>(IDbConnection conn, string cmd, DatabaseType dbType)
        {
            IDataReader read = ExecuteReader(conn, cmd, dbType);
            using (read)
            {
                T t = default(T);
                if (read.Read())
                {
                    t = (T)read[0];
                }
                return t;
            }
        }

        public static IDataReader ExecuteReader(IDbConnection conn, string cmdText, DatabaseType dbType)
        {
            IDbCommand cmd = GetCommandByScript(conn, cmdText);
            switch (dbType)
            {
                case DatabaseType.SQLServer:
                    SQL.Log.LogCommand(cmd);
                    break;
                case DatabaseType.MySQL:
                    MySQL.Log.LogCommand(cmd);
                    break;
                default:
                    break;
            }
            return cmd.ExecuteReader();
        }

        public static IDbCommand GetCommandByScript(IDbConnection conn, string cmdText)
        {
            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = cmdText;
            return cmd;
        }

        public static T GetExecuteScalar<T>(IDbConnection conn, string cmdText, DatabaseType dbType)
        {
            IDbCommand cmd = GetCommandByScript(conn, cmdText);
            switch (dbType)
            {
                case DatabaseType.SQLServer:
                    SQL.Log.LogCommand(cmd);
                    break;
                case DatabaseType.MySQL:
                    MySQL.Log.LogCommand(cmd);
                    break;
                default:
                    break;
            }
            return (T)cmd.ExecuteScalar();
        }

        public static string GetSelectString(string tableName)
        {
            return string.Format("select * from {0}", tableName);
        }

        public static List<T> GetExecuteList<T>(IDbConnection conn, string cmdText, DatabaseType dbType)
        {
            IDbCommand cmd = GetCommandByScript(conn, cmdText);
            switch (dbType)
            {
                case DatabaseType.SQLServer:
                    SQL.Log.LogCommand(cmd);
                    break;
                case DatabaseType.MySQL:
                    MySQL.Log.LogCommand(cmd);
                    break;
                default:
                    break;
            }
            List<T> ret = new List<T>();
            IDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                T t = (T)dr[0];
                ret.Add(t);
            }
            return ret;
        }
    }
}
