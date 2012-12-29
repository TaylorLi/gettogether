using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GetTogether.Data;

namespace GetTogether.Studio.Database.BLL
{
    public class StoreProcedure
    {
        public static string GetStoreProcedureText(ProjectParameter projParam, string spName)
        {
            switch (projParam.DatabaseTypeForCodeEngineer)
            {
                case DatabaseType.MySQL:
                    return BLL.BO_Common.GetString(projParam, string.Concat("show create procedure ", spName), "Create Procedure");
                case DatabaseType.Oracle:
                    break;
                case DatabaseType.SQLServer:
                    return BLL.BO_Common.GetString(projParam, string.Concat("sp_helptext ", spName), string.Empty);
                default:
                    break;
            }
            return string.Empty;
        }

        public static DataSet GetStoreProcedures(ProjectParameter projParam)
        {
            string sql = string.Empty;
            switch (projParam.DatabaseTypeForCodeEngineer)
            {
                case DatabaseType.MySQL:
                    sql = string.Format("SELECT specific_name as Name,created as CreateOn,last_altered as UpdateOn  FROM information_schema.ROUTINES where ROUTINE_SCHEMA='{0}' ORDER BY specific_name", BLL.BO_Common.GetDatabase(projParam));
                    break;
                case DatabaseType.Oracle:
                    break;
                case DatabaseType.SQLServer:
                    sql = "select [Name],crdate as CreateOn from sysobjects where xtype='p' and category=0 order by name";
                    break;
                default:
                    break;
            }
            return BLL.BO_Common.GetDataSet(projParam, sql);
        }

        public static DAL.DO_StoreProcedureParameter.UOList_StoreProcedureParameter GetStoreProcedureParameters(ProjectParameter projParam, string spName)
        {
            string sql = string.Empty;
            switch (projParam.DatabaseTypeForCodeEngineer)
            {
                case DatabaseType.MySQL:
                    return Database.BLL.MySQL.GetStoreProcedureParameters(projParam, spName);
                case DatabaseType.Oracle:
                    break;
                case DatabaseType.SQLServer:
                    DAL.DO_StoreProcedureParameter da = new GetTogether.Studio.Database.DAL.DO_StoreProcedureParameter();
                    da.GetCurrentConnectionInformation().DbType = projParam.DatabaseTypeForCodeEngineer;
                    da.GetCurrentConnectionInformation().ConnectionString = projParam.ConnectionString;
                    sql = string.Format("select sc.name as [Name],st.name DataType,st.length Length,sc.isnullable as IsNullAble,isoutparam as IsOutParam from systypes st right join syscolumns sc on st.xtype=sc.xtype right join sysobjects so on sc.id=so.id where so.xtype='p' and category=0 and so.name='{0}' and st.name<>'sysname'", spName);
                    return da.GetList(sql);
                default:
                    break;
            }
            return null;
        }

        public static string GetStoreProcedureSimple(ProjectParameter st, string spName)
        {
            Database.DAL.DO_StoreProcedureParameter.UOList_StoreProcedureParameter spParameters = BLL.StoreProcedure.GetStoreProcedureParameters(st, spName);
            StringBuilder sbInput = new StringBuilder();
            switch (st.DatabaseTypeForCodeEngineer)
            {
                case DatabaseType.MySQL:

                    foreach (Database.DAL.DO_StoreProcedureParameter.UO_StoreProcedureParameter p in spParameters)
                    {
                        string v = "0";
                        if (ColumnMapping.FromDatabaseType(p.DataType).Equals("string"))
                        {
                            v = string.Format("'{0}'", p.Name);
                        }
                        sbInput.Append(v).Append(",");
                    }
                    if (sbInput.Length > 0) sbInput.Remove(sbInput.Length - 1, 1);
                    sbInput.Insert(0, string.Format("Call {0}(", spName));
                    sbInput.Append(")");
                    break;
                case DatabaseType.Oracle:
                    break;
                case DatabaseType.SQLServer:
                    for (int i = 0; i < spParameters.Count; i++)
                    {
                        Database.DAL.DO_StoreProcedureParameter.UO_StoreProcedureParameter p = spParameters[i];
                        string v = "0";
                        if (ColumnMapping.FromDatabaseType(p.DataType).Equals("string"))
                        {
                            v = "N''";
                        }
                        sbInput.Append("\t").Append(p.Name).Append(" = ").Append(v);
                        if (i < spParameters.Count - 1) sbInput.Append(",");
                        sbInput.Append("\t\t--Database type:").Append(p.DataType);
                        sbInput.Append(", Length:").Append(p.Length);
                        sbInput.Append(", Is nullable:").AppendLine(p.IsNullAble > 0 ? "True" : "False");
                    }
                    if (sbInput.Length > 0) sbInput.Remove(sbInput.Length - 1, 1);
                    sbInput.Insert(0, string.Format("EXEC\t[{0}]\r\n", spName));
                    break;
                default:
                    break;
            }

            return sbInput.ToString();
        }

        /// <summary>
        /// /*{Result:Result1,Result2,Result3}*/
        /// </summary>
        /// <param name="projParam"></param>
        /// <param name="spName"></param>
        /// <returns></returns>
        public static string[] GetStoreProcedureObjectMapping(ProjectParameter projParam, string spName)
        {
            string text = GetStoreProcedureText(projParam, spName);
            if (string.IsNullOrEmpty(text)) return null;
            System.Text.RegularExpressions.Regex regx = new System.Text.RegularExpressions.Regex("{Result:.*?}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match m = regx.Match(text);
            if (m.Success && !string.IsNullOrEmpty(m.Value))
            {
                string mv = m.Value.Substring(m.Value.IndexOf(':') + 1);
                mv = mv.Substring(0, mv.Length - 1);
                return mv.Split(',');
            }
            return null;
        }
    }
}
