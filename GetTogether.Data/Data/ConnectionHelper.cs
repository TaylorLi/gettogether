using System;
using System.Data;
using System.Configuration;
using System.Web;
using MySql.Data.MySqlClient;

namespace GetTogether.Data
{
    public class ConnectionHelper
    {
        public ConnectionHelper()
        {

        }
        public static IDbConnection CreateConnection(string connectionString, DatabaseType dbType)
        {
            IDbConnection cnn = null;
            switch (dbType)
            {
                case DatabaseType.SQLServer:
                    cnn = new System.Data.SqlClient.SqlConnection(connectionString);
                    cnn.Open();
                    break;
                case DatabaseType.MySQL:
                    cnn = new MySqlConnection(connectionString);
                    cnn.Open();
                    break;
                default:
                    break;
            }

            return cnn;
        }

        public static IDbConnection CreateConnectionByKey(string key, DatabaseType dbType)
        {
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[string.Concat(key, "_", (int)dbType)];
            string connectionString = key;
            if (connectionStringSettings != null && !string.IsNullOrEmpty(connectionStringSettings.ConnectionString))
                connectionString = connectionStringSettings.ConnectionString;
            return CreateConnection(connectionString, dbType);
        }

        public static void DisposeConnection(IDbConnection conn)
        {
            conn.Close();
            conn.Dispose();
        }
    }
}
