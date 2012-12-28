using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace GetTogether.Data.MySQL
{
    public class Log
    {
        public static bool EnableLog = false;
        //public static void InitLogging(string url)
        //{
        //    EnableLog = true;
        //    Data.Logging.SetConfig(url);
        //}

        public static string GetCommandMessage(System.Data.IDbCommand cmd)
        {
            if (cmd.CommandType == CommandType.StoredProcedure) return GenStroedProcedureMessage(cmd);
            StringBuilder sb = new StringBuilder();
            //sb.Append(cmd.CommandType.ToString());
            sb.Append("Use ").Append(cmd.Connection.Database).Append(" ");
            sb.Append("exec sp_executesql N'");
            sb.Append(cmd.CommandText.Replace("'", "''")).Append("'");
            if (cmd.Parameters != null && cmd.Parameters.Count > 0)
            {
                sb.Append(",N'");
                bool isFirst = true;
                foreach (MySqlParameter sp in cmd.Parameters)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                        sb.Append(",");
                    sb.Append(sp.ParameterName).Append(" ").Append(GetLogType(sp));
                }
                sb.Append("',");
                isFirst = true;
                foreach (MySqlParameter sp in cmd.Parameters)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        sb.Append(",");
                    }
                    sb.Append(sp.ParameterName).Append("=").Append(GetLogValue(sp));
                }
            }
            return sb.ToString();
        }

        public static string GenStroedProcedureMessage(System.Data.IDbCommand cmd)
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append("Use ").Append(cmd.Connection.Database).Append(" ");
            sb.Append("call ").Append(cmd.CommandText).Append("(");
            bool isFirst = true;
            foreach (MySqlParameter sp in cmd.Parameters)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    sb.Append(",");
                }
                sb.Append(sp.ParameterName).Append("=").Append(GetLogValue(sp));
            }
            sb.Append(")");
            return sb.ToString();
        }

        public static string GetLogValue(MySqlParameter sp)
        {
            //bool appendN = false;
            bool appendSingleQuotes = false;
            switch (sp.DbType)
            {
                case DbType.AnsiString: appendSingleQuotes = true;
                    break;
                case DbType.AnsiStringFixedLength: appendSingleQuotes = true;
                    break;
                case DbType.Binary:
                    break;
                case DbType.Boolean:
                    break;
                case DbType.Byte:
                    break;
                case DbType.Currency:
                    break;
                case DbType.Date:
                    break;
                case DbType.DateTime:
                    break;
                case DbType.DateTime2:
                    break;
                case DbType.DateTimeOffset:
                    break;
                case DbType.Decimal:
                    break;
                case DbType.Double:
                    break;
                case DbType.Guid:
                    break;
                case DbType.Int16:
                    break;
                case DbType.Int32:
                    break;
                case DbType.Int64:
                    break;
                case DbType.Object:
                    break;
                case DbType.SByte:
                    break;
                case DbType.Single:
                    break;
                case DbType.String: appendSingleQuotes = true;
                    break;
                case DbType.StringFixedLength: appendSingleQuotes = true;
                    break;
                case DbType.Time:
                    break;
                case DbType.UInt16:
                    break;
                case DbType.UInt32:
                    break;
                case DbType.UInt64:
                    break;
                case DbType.VarNumeric:
                    break;
                case DbType.Xml:
                    break;
                default:
                    break;
            }
            if (appendSingleQuotes && sp.Value != null)
            {
                return string.Concat("'", sp.Value.ToString().Replace("'", "''"), "'");
            }
            else
            {
                return sp.Value == null ? "NULL" : sp.Value.ToString();
            }
        }

        public static string GetLogType(MySqlParameter sp)
        {
            bool appendType = false;
            switch (sp.DbType)
            {
                case DbType.AnsiString:
                    break;
                case DbType.AnsiStringFixedLength:
                    break;
                case DbType.Binary:
                    break;
                case DbType.Boolean:
                    break;
                case DbType.Byte:
                    break;
                case DbType.Currency:
                    break;
                case DbType.Date:
                    break;
                case DbType.DateTime:
                    break;
                case DbType.DateTime2:
                    break;
                case DbType.DateTimeOffset:
                    break;
                case DbType.Decimal:
                    break;
                case DbType.Double:
                    break;
                case DbType.Guid:
                    break;
                case DbType.Int16:
                    break;
                case DbType.Int32:
                    break;
                case DbType.Int64:
                    break;
                case DbType.Object:
                    break;
                case DbType.SByte:
                    break;
                case DbType.Single:
                    break;
                case DbType.String: appendType = true;
                    break;
                case DbType.StringFixedLength: appendType = true;
                    break;
                case DbType.Time:
                    break;
                case DbType.UInt16:
                    break;
                case DbType.UInt32:
                    break;
                case DbType.UInt64:
                    break;
                case DbType.VarNumeric:
                    break;
                case DbType.Xml: appendType = true;
                    break;
                default:
                    break;
            }
            if (appendType)
                return string.Concat(sp.DbType, "(", sp.Size == 0 ? 1 : sp.Size, ")");
            else
                return sp.DbType.ToString();
        }

        public static void LogCommand(System.Data.IDbCommand cmd)
        {
            string ctm = string.Empty;
            if (Config.CommandTimeout > 0)
            {
                if (EnableLog)
                {
                    ctm = string.Concat("Command timeout changed:", cmd.CommandTimeout, " > ", Config.CommandTimeout, "\r\n");
                }
                cmd.CommandTimeout = Config.CommandTimeout;
            }
            if (!EnableLog || cmd == null) return;
            string text = cmd.CommandText.ToLower();
            string msg = string.Concat(ctm, GetCommandMessage(cmd));
            if (cmd.CommandType == CommandType.StoredProcedure)
            {
                StoredProcedure(msg);
            }
            else
            {
                if (text.IndexOf("insert ") >= 0)
                    Insert(msg);
                else if (text.IndexOf("select ") >= 0)
                    Select(msg);
                else if (text.IndexOf("update ") >= 0)
                    Update(msg);
                else if (text.IndexOf("delete ") >= 0)
                    Delete(msg);
                else
                    Select(string.Concat(cmd.CommandType, " : ", msg));
            }
        }

        public static void Info(string l)
        {
            if (EnableLog && Config.Log.LogInfo) Data.Log.WriteInfo(Config.Log.Logger.Info, string.Concat(GetTogether.Data.DatabaseType.MySQL, ":\r\n", l));
        }

        public static void StoredProcedure(string l)
        {
            if (EnableLog && Config.Log.LogStoredProcedure) Data.Log.WriteInfo(Config.Log.Logger.StoredProcedure, string.Concat(GetTogether.Data.DatabaseType.MySQL, ":\r\n", l));
        }

        public static void Warning(string l)
        {
            if (EnableLog && Config.Log.LogWarning) Data.Log.WriteWarn(Config.Log.Logger.Warning, string.Concat(GetTogether.Data.DatabaseType.MySQL, ":\r\n", l));
        }

        public static void Debug(string l)
        {
            if (EnableLog && Config.Log.LogDebug) Data.Log.WriteInfo(Config.Log.Logger.Debug, string.Concat(GetTogether.Data.DatabaseType.MySQL, ":\r\n", l));
        }

        public static void Insert(string l)
        {
            if (EnableLog && Config.Log.LogInsert) Data.Log.WriteInfo(Config.Log.Logger.Insert, string.Concat(GetTogether.Data.DatabaseType.MySQL, ":\r\n", l));
        }

        public static void Select(string l)
        {
            if (EnableLog && Config.Log.LogSelect) Data.Log.WriteInfo(Config.Log.Logger.Select, string.Concat(GetTogether.Data.DatabaseType.MySQL, ":\r\n", l));
        }

        public static void Update(string l)
        {
            if (EnableLog && Config.Log.LogUpdate) Data.Log.WriteInfo(Config.Log.Logger.Update, string.Concat(GetTogether.Data.DatabaseType.MySQL, ":\r\n", l));
        }

        public static void Delete(string l)
        {
            if (EnableLog && Config.Log.LogDelete) Data.Log.WriteInfo(Config.Log.Logger.Delete, string.Concat(GetTogether.Data.DatabaseType.MySQL, ":\r\n", l));
        }
    }
}
