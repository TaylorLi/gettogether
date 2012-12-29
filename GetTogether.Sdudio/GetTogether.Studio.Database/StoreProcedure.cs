using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace GetTogether.Studio.Database
{
    public class StoreProcedure
    {
        public static string GetDAL(ProjectParameter projParam, string objName, string spName, string simpleSql, string returnObjMapping)
        {
            using (IDataReader dr = Database.BLL.BO_Common.GetDataReader(projParam, simpleSql))
            {
                return GetDAL(projParam, dr, objName, spName, returnObjMapping);
            }
        }

        private static string GetObjectName(int index, string[] objMapping)
        {
            if (objMapping != null && objMapping.Length >= index)
            {
                return objMapping[index - 1];
            }
            else
            {
                return string.Format("Result{0}", index);
            }
        }

        public static string GetDAL(ProjectParameter projParam, IDataReader dr, string objName, string storeProcName, string returnObjMapping)
        {
            string[] objMapping = null;
            if (string.IsNullOrEmpty(returnObjMapping))
                objMapping = Database.BLL.StoreProcedure.GetStoreProcedureObjectMapping(projParam, storeProcName);
            else
                objMapping = returnObjMapping.Split(',');
            int index = 1;
            StringBuilder sbCodes = new StringBuilder();
            Dictionary<int, ColumnMapping.ColumnInfos> tableColumnInfos = new Dictionary<int, ColumnMapping.ColumnInfos>();
            tableColumnInfos[index] = ColumnMapping.GetColumnInfo(dr, projParam, string.Empty, string.Empty);
            while (dr.NextResult())
            {
                index++;
                tableColumnInfos[index] = ColumnMapping.GetColumnInfo(dr, projParam, string.Empty, string.Empty);
            }
            sbCodes.AppendLine(string.Format(@"
//------------------------------------------------------------------------------
// <auto-generated>
//     Date time = {1}
//     This code was generated by tool,Version={0}.
//     Changes to this code may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------", Config.Version, DateTime.Now));
            sbCodes.AppendLine("using System;");
            sbCodes.AppendLine("using System.Collections.Generic;");
            sbCodes.AppendLine("using System.Text;");
            sbCodes.AppendLine("using System.Data;");
            sbCodes.AppendLine("using GetTogether.Data;");
            sbCodes.AppendLine("using GetTogether.Mapping;");
            sbCodes.AppendLine("");
            sbCodes.Append("namespace ").AppendLine(projParam.DataObjectNameSpace);
            sbCodes.AppendLine("{");
            StringBuilder sbObjects = new StringBuilder();
            StringBuilder sbAttributes = new StringBuilder();
            for (int i = 1; i <= tableColumnInfos.Count; i++)
            {
                sbObjects.AppendLine(GetSubDAL(projParam, i, tableColumnInfos[i], objMapping));
                sbObjects.Append("\t\t").AppendLine(string.Format("public class {0}{1}: List<{2}{1}>", projParam.UOListPrefix, GetObjectName(i, objMapping), projParam.UOPrefix));
                sbObjects.AppendLine("\t\t{");
                sbObjects.AppendLine(string.Format("\t\t\tpublic {0}{1}()", projParam.UOListPrefix, GetObjectName(i, objMapping)));
                sbObjects.AppendLine("\t\t\t{");
                sbObjects.AppendLine("\t\t\t}");
                sbObjects.AppendLine("\t\t}");

                sbAttributes.AppendLine(string.Format("\t\tprivate {0}{1} _{1};", projParam.UOListPrefix, GetObjectName(i, objMapping)));
                sbAttributes.AppendLine(string.Format("\t\tpublic {0}{1} {1}", projParam.UOListPrefix, GetObjectName(i, objMapping)));
                sbAttributes.AppendLine("\t\t{");
                sbAttributes.AppendLine("\t\t\tget");
                sbAttributes.AppendLine("\t\t\t{");
                sbAttributes.AppendLine(string.Format("\t\t\t\treturn this._{0};", GetObjectName(i, objMapping)));
                sbAttributes.AppendLine("\t\t\t}");
                sbAttributes.AppendLine("\t\t\tset");
                sbAttributes.AppendLine("\t\t\t{");
                sbAttributes.AppendLine(string.Format("\t\t\t\tthis._{0} = value;", GetObjectName(i, objMapping)));
                sbAttributes.AppendLine("\t\t\t}");
                sbAttributes.AppendLine("\t\t}");
            }
            sbCodes.AppendLine(string.Format("\tpublic class {0}{1} : StoreProcBase<{0}{1}, {0}{1}.Results>", projParam.DOPrefix, objName));
            sbCodes.AppendLine("\t{");


            sbCodes.AppendLine(@"public static StoreProcInformation GetConnectionInformation()
        {
            return new StoreProcInformation(
                {ConnectionKey}, ""{StoreProcedureName}"", {DatabaseType});
        }
        public override StoreProcInformation GetDefaultConnectionInformation()
        {
            return GetConnectionInformation();
        }".Replace("{ConnectionKey}", projParam.ConnectionKey).Replace("{StoreProcedureName}", storeProcName).Replace("{DatabaseType}", projParam.DatabaseTypeVariables));

            sbCodes.AppendLine(string.Format("\t\tpublic {0}{1}()", projParam.DOPrefix, objName));
            sbCodes.AppendLine("\t\t{");
            //sbCodes.AppendLine(string.Format("\t\t\tStoreProcInfo = new StoreProcInformation({0}, \"{1}\",{2});", cf.ConnectionKey, storeProcName,cf.DatabaseTypeVariables));
            sbCodes.AppendLine("\t\t}");
            sbCodes.AppendLine("\t\tpublic override Results GetResults(IDataParameter[] parameters)");
            sbCodes.AppendLine("\t\t{");
            sbCodes.AppendLine("\t\t\tusing(this.GetCurrentConnectionInformation().Connection)");
            sbCodes.AppendLine("\t\t\t{");
            sbCodes.AppendLine(string.Format("\t\t\t\tResults results{0} = new Results();", objName));
            sbCodes.AppendLine("\t\t\t\tIDataReader dr = GetDataReader(parameters);");
            sbCodes.AppendLine(string.Format("\t\t\t\tresults{0}.{3} = ObjectHelper.FillCollection<{1}{3}, {2}{3}>(dr);", objName, projParam.UOPrefix, projParam.UOListPrefix, GetObjectName(1, objMapping)));
            for (int i = 2; i <= tableColumnInfos.Count; i++)
            {
                sbCodes.AppendLine("\t\t\t\tif (dr.NextResult())");
                sbCodes.AppendLine("\t\t\t\t{");
                sbCodes.AppendLine(string.Format("\t\t\t\t\t\tresults{0}.{1} = ObjectHelper.FillCollection<{2}{1}, {3}{1}>(dr);", objName, GetObjectName(i, objMapping), projParam.UOPrefix, projParam.UOListPrefix));
                sbCodes.AppendLine("\t\t\t\t}");
            }
            sbCodes.AppendLine(string.Format("\t\t\t\treturn results{0};", objName));
            sbCodes.AppendLine("\t\t\t}");
            sbCodes.AppendLine("\t\t}");
            //sbCodes.AppendLine("\t\tpublic override IDataReader GetDataReader(IDataParameter[] parameters)");
            //sbCodes.AppendLine("\t\t{");
            //sbCodes.AppendLine("\t\t\treturn GetTogether.Data.SQL.SqlUtil.ExecuteProcedureReader(StoreProcInfo.Connection, StoreProcInfo.StoreProcName, parameters);");
            //sbCodes.AppendLine("\t\t}");

            //sbCodes.AppendLine("\t\tpublic override DataSet GetDataSet(IDataParameter[] parameters)");
            //sbCodes.AppendLine("\t\t{");
            //sbCodes.AppendLine("\t\t\tusing(this.StoreProcInfo.Connection)");
            //sbCodes.AppendLine("\t\t\t{");
            //sbCodes.AppendLine("\t\t\t\treturn GetTogether.Data.SQL.SqlUtil.ExecuteProcedureDataSet(StoreProcInfo.Connection, StoreProcInfo.StoreProcName, parameters);");
            //sbCodes.AppendLine("\t\t\t}");
            //sbCodes.AppendLine("\t\t}");
            sbCodes.AppendLine("\t\tpublic class Results");
            sbCodes.AppendLine("\t\t\t{");
            sbCodes.AppendLine("\t\t\t\t#region Attributes");
            sbCodes.AppendLine(sbAttributes.ToString());
            sbCodes.AppendLine("\t\t\t\t#endregion");
            sbCodes.AppendLine("\t\t\t\tpublic Results()");
            sbCodes.AppendLine("\t\t\t\t{");
            sbCodes.AppendLine("\t\t\t\t}");
            sbCodes.AppendLine("\t\t\t}");
            sbCodes.AppendLine("\t\t#region Return objects");
            sbCodes.Append(sbObjects.ToString());
            sbCodes.AppendLine("\t\t#endregion");
            sbCodes.AppendLine("\t}");
            sbCodes.AppendLine("}");
            return sbCodes.ToString();
        }

        public static string GetSubDAL(ProjectParameter projParam, int index, ColumnMapping.ColumnInfos colInfos, string[] objMapping)
        {
            StringBuilder sbCodes = new StringBuilder();
            sbCodes.AppendLine(string.Format("\t\tpublic class {0}{1}", projParam.UOPrefix, GetObjectName(index, objMapping)));
            sbCodes.AppendLine("\t\t{");
            sbCodes.AppendLine(string.Format("\t\t\tpublic {0}{1}()", projParam.UOPrefix, GetObjectName(index, objMapping)));
            sbCodes.AppendLine("\t\t\t{");
            sbCodes.AppendLine("\t\t\t}");
            StringBuilder sb_columns = new StringBuilder();
            sb_columns.AppendLine("\t\t\t#region Columns");
            foreach (ColumnMapping.ColumnInfo c in colInfos)
            {
                sb_columns.AppendLine(string.Format("\t\t\tprivate {0} _{1};", c.ColumnType, c.ColumnName));
                sb_columns.Append(string.Format("\t\t\t[Mapping(\"{0}", c.ColumnName));
                sb_columns.AppendLine("\")]");
                sb_columns.AppendLine(string.Format("\t\t\tpublic {0} {1}", c.ColumnType, c.ColumnName));
                sb_columns.AppendLine("\t\t\t{");
                sb_columns.AppendLine("\t\t\t\tget");
                sb_columns.AppendLine("\t\t\t\t{");
                sb_columns.AppendLine(string.Format("\t\t\t\t\treturn _{0};", c.ColumnName));
                sb_columns.AppendLine("\t\t\t\t}");
                sb_columns.AppendLine("\t\t\t\tset");
                sb_columns.AppendLine("\t\t\t\t{");
                sb_columns.AppendLine(string.Format("\t\t\t\t\t_{0} = value;", c.ColumnName));
                sb_columns.AppendLine("\t\t\t\t}");
                sb_columns.AppendLine("\t\t\t}");
            }
            sb_columns.AppendLine("\t\t\t#endregion");

            sbCodes.AppendLine(sb_columns.ToString());
            sbCodes.Append("\t\t}");
            return sbCodes.ToString();
        }

        public static string GetBLL(ProjectParameter projParam, string objName, string storeProcName)
        {
            StringBuilder sbArgs = new StringBuilder();
            StringBuilder sbArgsValues = new StringBuilder();
            StringBuilder sbParameters = new StringBuilder();
            sbParameters.AppendLine("\t\t\tList<IDataParameter> parameters = new List<IDataParameter>();");
            sbParameters.AppendLine("");
            foreach (Database.DAL.DO_StoreProcedureParameter.UO_StoreProcedureParameter p in Database.BLL.StoreProcedure.GetStoreProcedureParameters(projParam, storeProcName))
            {
                if (p.IsOutParam > 0) continue;
                string paramName = p.Name.Replace("@", "");
                if (sbArgs.Length > 0) sbArgs.Append(", ");
                if (sbArgsValues.Length > 0) sbArgsValues.Append(", ");
                sbArgsValues.Append(paramName);
                sbArgs.Append(GetTogether.Studio.Database.ColumnMapping.FromDatabaseType(p.DataType)).Append(" ").Append(paramName);
                if (GetTogether.Studio.Database.ColumnMapping.FromDatabaseType(p.DataType).Equals("DateTime"))
                {
                    sbParameters.AppendLine(string.Format("\t\t\tparameters.Add(new System.Data.SqlClient.SqlParameter(\"{0}\", {1}.ToString(\"yyyyMMdd\", System.Globalization.DateTimeFormatInfo.InvariantInfo)));", p.Name, paramName));
                }
                else
                {
                    sbParameters.AppendLine(string.Format("\t\t\tparameters.Add(new System.Data.SqlClient.SqlParameter(\"{0}\", {1}));", p.Name, paramName));
                }
            }
            StringBuilder sbCodes = new StringBuilder();
            sbCodes.AppendLine(string.Format(@"//------------------------------------------------------------------------------
// <auto-generated>
//     Date time = {1}
//     This code was generated by tool,Version={0}.
//     Changes to this code may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------", Config.Version, DateTime.Now));
            sbCodes.AppendLine("using System;");
            sbCodes.AppendLine("using System.Collections.Generic;");
            sbCodes.AppendLine("using System.Text;");
            sbCodes.AppendLine("using System.Data;");
            sbCodes.AppendLine("using GetTogether.Data;");
            sbCodes.Append("using ").Append(projParam.DataObjectNameSpace).AppendLine(";");
            sbCodes.AppendLine("");
            sbCodes.Append("namespace ").AppendLine(projParam.BusinessObjectNameSpace);
            sbCodes.AppendLine("{");
            sbCodes.AppendLine(string.Format("\tpublic partial class {0}{1}", projParam.BOPrefix, objName));
            sbCodes.AppendLine("\t{");
            sbCodes.Append("\t\t#region This source code was auto-generated by tool,Version=").AppendLine(Config.Version);

            sbCodes.AppendLine("\t\t/// <summary>");
            sbCodes.AppendLine("\t\t/// Get parameters");
            sbCodes.AppendLine("\t\t/// </summary>");
            sbCodes.AppendLine(string.Format("\t\tpublic static IDataParameter[] GetParameters({0})", sbArgs.ToString()));
            sbCodes.AppendLine("\t\t{");
            sbCodes.AppendLine(sbParameters.ToString());
            sbCodes.AppendLine("\t\t\treturn parameters.ToArray();");
            sbCodes.AppendLine("\t\t}");
            sbCodes.AppendLine("\t\t/// <summary>");
            sbCodes.AppendLine("\t\t/// Get object result");
            sbCodes.AppendLine("\t\t/// </summary>");
            sbCodes.AppendLine(string.Format("\t\tpublic static {0}{1}.Results GetResults({3}{2})", projParam.DOPrefix, objName, sbArgs.ToString(), ""));
            sbCodes.AppendLine("\t\t{");
            sbCodes.AppendLine(string.Format("\t\t\t{0}{1} da = new {0}{1}();", projParam.DOPrefix, objName));

            sbCodes.AppendLine(string.Format("\t\t\treturn da.GetResults(GetParameters({0}));", sbArgsValues.ToString()));
            sbCodes.AppendLine("\t\t}");
            sbCodes.AppendLine("\t\t/// <summary>");
            sbCodes.AppendLine("\t\t/// Get DataSet result");
            sbCodes.AppendLine("\t\t/// </summary>");
            sbCodes.AppendLine(string.Format("\t\tpublic static DataSet GetDataSet({1}{0})", sbArgs.ToString(), ""));
            sbCodes.AppendLine("\t\t{");
            sbCodes.AppendLine(string.Format("\t\t\t{0}{1} da = new {0}{1}();", projParam.DOPrefix, objName));

            sbCodes.AppendLine(string.Format("\t\t\treturn da.GetDataSet(GetParameters({0}));", sbArgsValues.ToString()));
            sbCodes.AppendLine("\t\t}");
            sbCodes.AppendLine("\t\t#endregion");
            sbCodes.AppendLine("\t\t#region User extensions");
            sbCodes.AppendLine("\t\t#endregion");
            sbCodes.AppendLine("\t}");
            sbCodes.AppendLine("}");
            return sbCodes.ToString();
        }
    }
}
