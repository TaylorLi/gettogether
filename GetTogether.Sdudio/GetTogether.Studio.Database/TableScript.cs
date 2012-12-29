using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Studio.Database.SQLServer
{
    public static class TableScript
    {
        public static string GetDAL(
            ProjectParameter projParam,
            string tableName,
            ColumnMapping.ColumnInfos colInfos,
            Database.DAL.DO_ColumnDescription.UOList_ColumnDescription columnDescriptions,
            bool appendUsing)
        {

            StringBuilder sbCodes = new StringBuilder();
            if (appendUsing)
            {
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
                sbCodes.AppendLine("using GetTogether.Data;");
                sbCodes.AppendLine("using GetTogether.Mapping;");
                sbCodes.AppendLine("");
                sbCodes.Append("namespace ").AppendLine(projParam.DataObjectNameSpace);
                sbCodes.AppendLine("{");
            }
            sbCodes.AppendLine(string.Format("\tpublic partial class {1}{0} : DOBase<{1}{0}.{2}{0}, {1}{0}.{3}{0}>", tableName, projParam.DOPrefix, projParam.UOPrefix, projParam.UOListPrefix));
            sbCodes.AppendLine("\t{");
            StringBuilder sbPks = new StringBuilder();
            foreach (ColumnMapping.ColumnInfo p in colInfos)
            {
                if (p.IsPrimaryKey)
                {
                    if (p.ColumnName.Trim().IndexOf(" ") > 0) p.ColumnName = p.ColumnName.Trim().Replace(" ", "_space_");
                    if (sbPks.Length > 0) sbPks.Append(", ");
                    sbPks.Append("\"").Append(p.ColumnName).Append("\"");
                }
            }
            sbPks.Insert(0, "new string[] {");
            sbPks.Append("}");
            sbCodes.Append(@"               public static ConnectionInformation GetConnectionInformation()
                {
                    return new   ConnectionInformation(
                        ""{Table}"",
                        {ConnectionKey},
                        {PrimaryKey},
                        {DatabaseType});
                }".Replace("{Table}", tableName).Replace("{ConnectionKey}", projParam.ConnectionKey).Replace("{PrimaryKey}", sbPks.ToString()).Replace("{DatabaseType}", projParam.DatabaseTypeVariables));
            sbCodes.AppendLine(@"
                public override ConnectionInformation GetDefaultConnectionInformation()
                {
                    return GetConnectionInformation();
                }");
            sbCodes.AppendLine("\t\tpublic enum Columns");
            sbCodes.AppendLine("\t\t{");
            StringBuilder sb_columns = new StringBuilder();
            sb_columns.AppendLine("\t\t\t#region Columns");
            foreach (ColumnMapping.ColumnInfo c in colInfos)
            {
                if (c.ColumnName.Trim().IndexOf(" ") > 0) c.ColumnName = c.ColumnName.Trim().Replace(" ", "_space_");
                StringBuilder sbColumnInfo = new StringBuilder();
                if (c.IsPrimaryKey)
                {
                    sbColumnInfo.Append("Primary Key");
                }
                //sbColumnInfo.Append("Database Type:").Append(c.ColumnType);
                if (c.IsAutoIncrement)
                {
                    sbColumnInfo.Append(",Auto Increment");
                }
                foreach (Database.DAL.DO_ColumnDescription.UO_ColumnDescription r in columnDescriptions)
                {
                    if (r.Name.Equals(c.ColumnName) && !string.IsNullOrEmpty(r.Remark))
                    {
                        sbColumnInfo.Append(",Remark:").Append(r.Remark);
                        break;
                    }
                }
                if (sbColumnInfo.Length > 0)
                {
                    sbCodes.AppendLine("\t\t\t/// <summary>");
                    sbCodes.Append("\t\t\t///").AppendLine(sbColumnInfo.ToString());
                    sbCodes.AppendLine("\t\t\t/// </summary>");
                }
                sbCodes.Append("\t\t\t").Append(c.ColumnName).AppendLine(",");
                string vtype = c.ColumnType;// ColumnMapping.GetColumnType(c.Column);
                sb_columns.AppendLine(string.Format("\t\t\tprivate {0} _{1};", vtype, c.ColumnName));
                if (sbColumnInfo.Length > 0)
                {
                    sb_columns.AppendLine("\t\t\t/// <summary>");
                    sb_columns.Append("\t\t\t///").AppendLine(sbColumnInfo.ToString());
                    sb_columns.AppendLine("\t\t\t/// </summary>");
                }
                sb_columns.Append(string.Format("\t\t\t[Mapping(\"{0}", c.ColumnName));
                if (c.IsAutoIncrement)
                {
                    sb_columns.Append(",un-insert,un-update");
                }
                else if (c.IsPrimaryKey)
                {
                    sb_columns.Append(",un-update");
                }
                else if (projParam.UnInsertAndUnUpdate.ToLower().IndexOf(c.ColumnName.ToLower()) >= 0)
                {
                    sb_columns.Append(",un-insert,un-update");
                }
                else if (projParam.UnInsert.ToLower().IndexOf(c.ColumnName.ToLower()) >= 0)
                {
                    sb_columns.Append(",un-insert");
                }
                else if (projParam.UnUpdate.ToLower().IndexOf(c.ColumnName.ToLower()) >= 0)
                {
                    sb_columns.Append(",un-update");
                }
                sb_columns.AppendLine("\")]");
                sb_columns.AppendLine(string.Format("\t\t\tpublic {0} {1}", vtype, c.ColumnName));
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
            sb_columns.Append("\t\t\t#endregion");
            sbCodes.AppendLine("\t\t}");
            sbCodes.AppendLine(string.Format("\t\tpublic {1}{0}()", tableName, projParam.DOPrefix));
            sbCodes.AppendLine("\t\t{");
            //if (listPK.Count > 0)
            //{
            //    sbCodes.AppendLine(string.Format("\t\t\tConnInfo = new ConnectionInformation(\"{0}\", {1},{2});", tableName, cf.ConnectionKey, sbPks.ToString()));
            //}
            //else
            //{
            //    sbCodes.AppendLine(string.Format("\t\t\tConnInfo = new ConnectionInformation(\"{0}\", {1});", tableName, cf.ConnectionKey));
            //}
            sbCodes.AppendLine("\t\t}");
            sbCodes.AppendLine(string.Format("\t\tpublic partial class {1}{0} : UOBase<{1}{0},{2}{0}>", tableName, projParam.UOPrefix, projParam.UOListPrefix));
            sbCodes.AppendLine("\t\t{");
            sbCodes.AppendLine(@"public override ConnectionInformation GetDefaultConnectionInformation()
            {
                return GetConnectionInformation();
            }");
            sbCodes.AppendLine(sb_columns.ToString());
            sbCodes.AppendLine(string.Format("\t\t\tpublic {1}{0}()", tableName, projParam.UOPrefix));
            sbCodes.AppendLine("\t\t\t{");
            //sbCodes.AppendLine(string.Format("\t\t\t\tConnInfo = new {0}{1}().ConnInfo;", cf.DOPrefix, tableName));
            sbCodes.AppendLine("\t\t\t}");
            sbCodes.AppendLine("\t\t}");
            sbCodes.AppendLine(string.Format("\t\tpublic partial class {1}{0} : GetTogether.ObjectBase.ListBase<{2}{0}>", tableName, projParam.UOListPrefix, projParam.UOPrefix));
            sbCodes.AppendLine("\t\t{");
            sbCodes.AppendLine(string.Format("\t\t\tpublic {1}{0}()", tableName, projParam.UOListPrefix));
            sbCodes.AppendLine("\t\t\t{");
            sbCodes.AppendLine("\t\t\t}");
            sbCodes.AppendLine("\t\t}");
            sbCodes.AppendLine("\t}");
            if (appendUsing)
            {
                sbCodes.AppendLine("}");
            }
            return sbCodes.ToString();
        }

        public static string GetBLL(ProjectParameter projParam,
            string tableName, ColumnMapping.ColumnInfos colInfos,
            bool isSentence,
            bool appendUsing)
        {
            StringBuilder sbCodes = new StringBuilder(), sbPrimaryKeyConditions = new StringBuilder(),
             sbPrimaryKeyParameters = new StringBuilder(), sbPrimaryKeyValues = new StringBuilder(),
             sbAutoIncrementParameters = new StringBuilder(), sbAutoIncrementConditions = new StringBuilder(),
             sbAutoIncrementValues = new StringBuilder();
            #region Paging Conditions
            StringBuilder sbPagingCondition = new StringBuilder();
            sbPagingCondition.AppendLine("\t\t\tParameterCollection objectConditions = new ParameterCollection();");
            sbPagingCondition.AppendLine("\t\t\tTokenTypes tt = tokenTypes;");
            sbPagingCondition.AppendLine("\t\t\tParameterType pt = isAnd ? ParameterType.And : ParameterType.Or;");
            foreach (ColumnMapping.ColumnInfo c in colInfos)
            {
                if (c.ColumnName.Trim().IndexOf(" ") > 0) c.ColumnName = c.ColumnName.Replace(" ", "_space_");
                if (c.ColumnType.IndexOf("String") >= 0)
                {
                    sbPagingCondition.AppendLine(string.Format("\t\t\tif (!string.IsNullOrEmpty(parameterObj.{0}))", c.ColumnName));
                    sbPagingCondition.AppendLine("\t\t\t{");
                    sbPagingCondition.AppendLine(string.Format("\t\t\t\tobjectConditions.AddCondition(pt, GetColumnTokenType(tt,{0}{1}.Columns.{2},extTokens), {0}{1}.Columns.{2},parameterObj.{2});", projParam.DOPrefix, tableName, c.ColumnName));
                    sbPagingCondition.AppendLine("\t\t\t}");
                }
                else if (c.ColumnType.IndexOf("Int") >= 0 || c.ColumnType.IndexOf("Decimal") >= 0 || c.ColumnType.IndexOf("Float") >= 0)
                {
                    sbPagingCondition.AppendLine(string.Format("\t\t\tif (parameterObj.{0} != 0 || (extTokens != null && extTokens.ContainsKey({1}{2}.Columns.{0})))", c.ColumnName, projParam.DOPrefix, tableName));
                    sbPagingCondition.AppendLine("\t\t\t{");
                    sbPagingCondition.AppendLine(string.Format("\t\t\t\tobjectConditions.AddCondition(pt, GetColumnTokenType(tt,{0}{1}.Columns.{2},extTokens), {0}{1}.Columns.{2},parameterObj.{2});", projParam.DOPrefix, tableName, c.ColumnName));
                    sbPagingCondition.AppendLine("\t\t\t}");
                }
                else if (c.ColumnType.Equals("DateTime"))
                {
                    sbPagingCondition.AppendLine(string.Format("\t\t\tif (parameterObj.{0} != DateTime.MinValue)", c.ColumnName));
                    sbPagingCondition.AppendLine("\t\t\t{");
                    sbPagingCondition.AppendLine(string.Format("\t\t\t\tobjectConditions.AddCondition(pt, GetColumnTokenType(tt,{0}{1}.Columns.{2},extTokens), {0}{1}.Columns.{2},parameterObj.{2});", projParam.DOPrefix, tableName, c.ColumnName));
                    sbPagingCondition.AppendLine("\t\t\t}");
                }
            }
            #endregion
            #region Primary Key
            sbPrimaryKeyConditions.AppendLine("\t\t\tParameterCollection primaryConditions = new ParameterCollection();");
            int index = 0;
            foreach (ColumnMapping.ColumnInfo ci in colInfos)
            {
                if (ci.IsPrimaryKey)
                {
                    if (ci.ColumnName.Trim().IndexOf(" ") > 0) ci.ColumnName = ci.ColumnName.Trim().Replace(" ", "_space_");
                    if (sbPrimaryKeyParameters.Length > 0) sbPrimaryKeyParameters.Append(",");
                    sbPrimaryKeyParameters.Append(ColumnMapping.GetColumnType(colInfos, ci.ColumnName)).Append(" ").Append(ci.ColumnName);
                    if (sbPrimaryKeyValues.Length > 0) sbPrimaryKeyValues.Append(",");
                    sbPrimaryKeyValues.Append(ci.ColumnName);
                    if (index == 0)
                    {
                        sbPrimaryKeyConditions.AppendLine(string.Format("\t\t\tprimaryConditions.AddCondition(ParameterType.Initial, TokenTypes.Equal, {0}{1}.Columns.{2}, {2});", projParam.DOPrefix, tableName, ci.ColumnName, ci.ColumnName));
                    }
                    else
                    {
                        sbPrimaryKeyConditions.AppendLine(string.Format("\t\t\tprimaryConditions.AddCondition(ParameterType.And, TokenTypes.Equal, {0}{1}.Columns.{2}, {2});", projParam.DOPrefix, tableName, ci.ColumnName, ci.ColumnName));
                    }
                    index++;
                }
            }
            #endregion
            #region Auto Increment
            if (colInfos.AutoIncrements.Count > 0)
            {
                sbAutoIncrementConditions.AppendLine("\t\t\tParameterCollection autoIncrementConditions = new ParameterCollection();");
                int autoIncrementIndex = 0;
                foreach (ColumnMapping.ColumnInfo ci in colInfos)
                {
                    if (ci.IsAutoIncrement)
                    {
                        string name = ci.ColumnName;
                        if (name.Trim().IndexOf(" ") > 0) name = name.Trim().Replace(" ", "_space_");
                        if (sbAutoIncrementParameters.Length > 0) sbAutoIncrementParameters.Append(",");
                        sbAutoIncrementParameters.Append(ColumnMapping.GetColumnType(colInfos, name)).Append(" ").Append(name);
                        if (sbAutoIncrementValues.Length > 0) sbAutoIncrementValues.Append(",");
                        sbAutoIncrementValues.Append(name);
                        if (autoIncrementIndex == 0)
                        {
                            sbAutoIncrementConditions.AppendLine(string.Format("\t\t\tautoIncrementConditions.AddCondition(ParameterType.Initial, TokenTypes.Equal, {0}{1}.Columns.{2}, {2});", projParam.DOPrefix, tableName, name, name));
                        }
                        else
                        {
                            sbAutoIncrementConditions.AppendLine(string.Format("\t\t\tautoIncrementConditions.AddCondition(ParameterType.And, TokenTypes.Equal, {0}{1}.Columns.{2}, {2});", projParam.DOPrefix, tableName, name, name));
                        }
                        autoIncrementIndex++;
                    }
                }
            }
            #endregion
            #region Using
            if (appendUsing)
            {
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
            }
            #endregion
            sbCodes.AppendLine(string.Format("\tpublic partial class {0}{1}", projParam.BOPrefix, tableName));
            sbCodes.AppendLine("\t{");
            #region Conditions
            sbCodes.AppendLine("\t\t#region Condition functions");
            if (colInfos.PrimaryKeys.Count > 0)
            {
                TableScriptHelper.GetConditionsByPrimaryKeyString(sbCodes, colInfos, sbPrimaryKeyValues);
                sbCodes.AppendLine("");
                sbCodes.AppendLine("\t\t///<summary>");
                sbCodes.AppendLine("\t\t///Get conditions by primary key.");
                sbCodes.AppendLine("\t\t///</summary>");
                sbCodes.AppendLine(string.Format("\t\tpublic static ParameterCollection GetConditionsByPrimaryKey({0})", sbPrimaryKeyParameters.ToString()));
                sbCodes.AppendLine("\t\t{");
                sbCodes.Append(sbPrimaryKeyConditions.ToString());
                sbCodes.AppendLine("\t\t\treturn primaryConditions;");
                sbCodes.AppendLine("\t\t}");
            }
            if (colInfos.AutoIncrements.Count > 0)
            {
                sbCodes.AppendLine("\t\t///<summary>");
                sbCodes.AppendLine("\t\t///Get conditions by primary key.");
                sbCodes.AppendLine("\t\t///</summary>");
                sbCodes.AppendLine(string.Format("\t\tpublic static ParameterCollection GetConditionsById({0})", sbAutoIncrementParameters.ToString()));
                sbCodes.AppendLine("\t\t{");
                sbCodes.Append(sbAutoIncrementConditions.ToString());
                sbCodes.AppendLine("\t\t\treturn autoIncrementConditions;");
                sbCodes.AppendLine("\t\t}");
            }
            sbCodes.AppendLine("");
            sbCodes.AppendLine("\t\t///<summary>");
            sbCodes.AppendLine("\t\t///Get the tokenType of the column of condition query.");
            sbCodes.AppendLine("\t\t///</summary>");
            sbCodes.AppendLine(string.Format("\t\tprivate static TokenTypes GetColumnTokenType(TokenTypes defaultTokenType,{0}{1}.Columns column,Dictionary<{0}{1}.Columns,TokenTypes> extTokens)", projParam.DOPrefix, tableName, projParam.UOPrefix));
            sbCodes.AppendLine("\t\t{");
            sbCodes.AppendLine("\t\t\tif (extTokens != null && extTokens.ContainsKey(column))");
            sbCodes.AppendLine("\t\t\t\treturn extTokens[column];");
            sbCodes.AppendLine("\t\t\telse");
            sbCodes.AppendLine("\t\t\t\treturn defaultTokenType;");
            sbCodes.AppendLine("\t\t}");
            sbCodes.AppendLine("");
            sbCodes.AppendLine("\t\t///<summary>");
            sbCodes.AppendLine("\t\t///Get conditions by object with Multi-TokenType.");
            sbCodes.AppendLine("\t\t///</summary>");
            sbCodes.AppendLine(string.Format("\t\tpublic static ParameterCollection GetConditionsByObject({0}{1}.{2}{1} parameterObj, bool isAnd, TokenTypes tokenTypes, Dictionary<{0}{1}.Columns, TokenTypes> extTokens)", projParam.DOPrefix, tableName, projParam.UOPrefix));
            sbCodes.AppendLine("\t\t{");
            sbCodes.Append(sbPagingCondition.ToString());
            sbCodes.AppendLine("\t\t\treturn objectConditions;");
            sbCodes.AppendLine("\t\t}");
            sbCodes.AppendLine("\t\t#endregion");
            sbCodes.AppendLine("");
            #endregion
            #region Query
            sbCodes.AppendLine("\t\t#region Query functions");
            sbCodes.AppendLine("");
            sbCodes.AppendLine("\t\t///<summary>");
            sbCodes.AppendLine("\t\t///Get all records.");
            sbCodes.AppendLine("\t\t///</summary>");
            sbCodes.AppendLine(string.Format("\t\tpublic static {0}{1}.{2}{1} GetAllList({3})", projParam.DOPrefix, tableName, projParam.UOListPrefix, ""));
            sbCodes.AppendLine("\t\t{");
            sbCodes.AppendLine(string.Format("\t\t\t{0}{1} da = new {0}{1}();", projParam.DOPrefix, tableName));
            sbCodes.AppendLine("\t\t\treturn da.GetAllList();");
            sbCodes.AppendLine("\t\t}");

            sbCodes.AppendLine("");
            sbCodes.AppendLine("\t\t///<summary>");
            sbCodes.AppendLine("\t\t///Get all records count.");
            sbCodes.AppendLine("\t\t///</summary>");
            sbCodes.AppendLine(string.Format("\t\tpublic static int GetAllRecordsCount({0})", ""));
            sbCodes.AppendLine("\t\t{");
            sbCodes.AppendLine(string.Format("\t\t\t{0}{1} da = new {0}{1}();", projParam.DOPrefix, tableName));
            sbCodes.AppendLine("\t\t\treturn da.GetRecordsCount();");
            sbCodes.AppendLine("\t\t}");

            sbCodes.AppendLine("");
            sbCodes.AppendLine("\t\t///<summary>");
            sbCodes.AppendLine("\t\t///Get records count.");
            sbCodes.AppendLine("\t\t///</summary>");
            sbCodes.AppendLine(string.Format("\t\tpublic static int GetRecordsCount({3}{0}{1}.{2}{1} parameterObj)", projParam.DOPrefix, tableName, projParam.UOPrefix, ""));
            sbCodes.AppendLine("\t\t{");
            sbCodes.AppendLine(string.Format("\t\t\treturn GetRecordsCount({0}parameterObj, true, TokenTypes.Equal,null);", ""));
            sbCodes.AppendLine("\t\t}");

            sbCodes.AppendLine("");
            sbCodes.AppendLine("\t\t///<summary>");
            sbCodes.AppendLine("\t\t///Get records count.");
            sbCodes.AppendLine("\t\t///</summary>");
            sbCodes.AppendLine(string.Format("\t\tpublic static int GetRecordsCount({3}{0}{1}.{2}{1} parameterObj, bool isAnd, TokenTypes tokenTypes, Dictionary<{0}{1}.Columns, TokenTypes> extTokens)", projParam.DOPrefix, tableName, projParam.UOPrefix, ""));
            sbCodes.AppendLine("\t\t{");
            sbCodes.AppendLine(string.Format("\t\t\t{0}{1} da = new {0}{1}();", projParam.DOPrefix, tableName));

            sbCodes.AppendLine("\t\t\treturn da.GetRecordsCount(GetConditionsByObject(parameterObj, isAnd, tokenTypes, extTokens));");
            sbCodes.AppendLine("\t\t}");

            sbCodes.AppendLine("");
            sbCodes.AppendLine("\t\t///<summary>");
            sbCodes.AppendLine("\t\t///Get list by object.");
            sbCodes.AppendLine("\t\t///</summary>");
            sbCodes.AppendLine(string.Format("\t\tpublic static {0}{1}.{2}{1} GetList({4}{0}{1}.{3}{1} parameterObj, bool isAnd, TokenTypes tokenTypes, Dictionary<{0}{1}.Columns, TokenTypes> extTokens)", projParam.DOPrefix, tableName, projParam.UOListPrefix, projParam.UOPrefix, ""));
            sbCodes.AppendLine("\t\t{");

            sbCodes.AppendLine("\t\t\treturn parameterObj.GetList(GetConditionsByObject(parameterObj, isAnd, tokenTypes, extTokens));");
            sbCodes.AppendLine("\t\t}");

            sbCodes.AppendLine("");
            sbCodes.AppendLine("\t\t///<summary>");
            sbCodes.AppendLine("\t\t///Get list by object.");
            sbCodes.AppendLine("\t\t///</summary>");
            sbCodes.AppendLine(string.Format("\t\tpublic static {0}{1}.{2}{1} GetList({4}{0}{1}.{3}{1} parameterObj)", projParam.DOPrefix, tableName, projParam.UOListPrefix, projParam.UOPrefix, ""));
            sbCodes.AppendLine("\t\t{");

            sbCodes.AppendLine(string.Format("\t\t\treturn GetList({0}parameterObj, true, TokenTypes.Equal, null);", ""));
            sbCodes.AppendLine("\t\t}");
            if (colInfos.PrimaryKeys.Count > 0)
            {
                sbCodes.AppendLine("");
                sbCodes.AppendLine("\t\t///<summary>");
                sbCodes.AppendLine("\t\t///Get object by primary key.");
                sbCodes.AppendLine("\t\t///</summary>");
                sbCodes.AppendLine(string.Format("\t\tpublic static {0}{1}.{2}{1} GetObject({4}{3})", projParam.DOPrefix, tableName, projParam.UOPrefix, sbPrimaryKeyParameters.ToString(), ""));
                sbCodes.AppendLine("\t\t{");
                sbCodes.AppendLine(string.Format("\t\t\t{0}{1} da = new {0}{1}();", projParam.DOPrefix, tableName));

                sbCodes.AppendLine(string.Format("\t\t\t{0}{1}.{2}{1} l = da.GetList(GetConditionsByPrimaryKey({3}));", projParam.DOPrefix, tableName, projParam.UOListPrefix, sbPrimaryKeyValues.ToString()));
                sbCodes.AppendLine("\t\t\treturn l.Count > 0 ? l[0] : null;");
                sbCodes.AppendLine("\t\t}");
                sbCodes.AppendLine("");
                TableScriptHelper.GetObjectByPrimaryKeyString(sbCodes, projParam, tableName);
                sbCodes.AppendLine("");
                sbCodes.AppendLine("\t\t///<summary>");
                sbCodes.AppendLine("\t\t///Get paging list.");
                sbCodes.AppendLine("\t\t///</summary>");
                sbCodes.AppendLine(string.Format("\t\tpublic static PagingResult<{0}{1}.{3}{1}, {0}{1}.{2}{1}> GetPagingList({4}{0}{1}.{3}{1} parameterObj,int pageNumber, int pageSize,string sortBy,bool isAsc, bool isAnd, TokenTypes tokenTypes, Dictionary<{0}{1}.Columns, TokenTypes> extTokens)", projParam.DOPrefix, tableName, projParam.UOListPrefix, projParam.UOPrefix, ""));
                sbCodes.AppendLine("\t\t{");

                sbCodes.AppendLine(string.Format("\t\t\treturn parameterObj.GetPagingList(GetConditionsByObject(parameterObj, isAnd, tokenTypes,extTokens), pageNumber, pageSize, sortBy, isAsc);", projParam.DOPrefix, tableName));
                sbCodes.AppendLine("\t\t}");

                sbCodes.AppendLine("");
                sbCodes.AppendLine("\t\t///<summary>");
                sbCodes.AppendLine("\t\t///Get paging list.");
                sbCodes.AppendLine("\t\t///</summary>");
                sbCodes.AppendLine(string.Format("\t\tpublic static PagingResult<{0}{1}.{3}{1}, {0}{1}.{2}{1}> GetPagingList({4}{0}{1}.{3}{1} parameterObj,int pageNumber, int pageSize,string sortBy,bool isAsc)", projParam.DOPrefix, tableName, projParam.UOListPrefix, projParam.UOPrefix, ""));
                sbCodes.AppendLine("\t\t{");

                sbCodes.AppendLine(string.Format("\t\t\treturn parameterObj.GetPagingList(GetConditionsByObject(parameterObj, true, TokenTypes.Like,null), pageNumber, pageSize, sortBy, isAsc);", ""));
                sbCodes.AppendLine("\t\t}");
            }
            if (colInfos.AutoIncrements.Count > 0)
            {
                sbCodes.AppendLine("");
                sbCodes.AppendLine("\t\t///<summary>");
                sbCodes.AppendLine("\t\t///Get object by Id.");
                sbCodes.AppendLine("\t\t///</summary>");
                sbCodes.AppendLine(string.Format("\t\tpublic static {0}{1}.{2}{1} GetObjectById({4}{3})", projParam.DOPrefix, tableName, projParam.UOPrefix, sbAutoIncrementParameters.ToString(), ""));
                sbCodes.AppendLine("\t\t{");
                sbCodes.AppendLine(string.Format("\t\t\t{0}{1} da = new {0}{1}();", projParam.DOPrefix, tableName));

                sbCodes.AppendLine(string.Format("\t\t\t{0}{1}.{2}{1} l = da.GetList(GetConditionsById({3}));", projParam.DOPrefix, tableName, projParam.UOListPrefix, sbAutoIncrementValues.ToString()));
                sbCodes.AppendLine("\t\t\treturn l.Count > 0 ? l[0] : null;");
                sbCodes.AppendLine("\t\t}");
            }
            sbCodes.AppendLine("\t\t#endregion");
            #endregion
            if (colInfos.PrimaryKeys.Count > 0 && !isSentence)
            {
                #region Update
                sbCodes.AppendLine("");
                sbCodes.AppendLine("\t\t#region Update functions");
                sbCodes.AppendLine("\t\t///<summary>");
                sbCodes.AppendLine("\t\t///Update object by primary key.");
                sbCodes.AppendLine("\t\t///</summary>");
                sbCodes.AppendLine(string.Format("\t\tpublic static bool UpdateObject({4}{0}{1}.{2}{1} obj, {3})", projParam.DOPrefix, tableName, projParam.UOPrefix, sbPrimaryKeyParameters.ToString(), ""));
                sbCodes.AppendLine("\t\t{");

                sbCodes.AppendLine(string.Format("\t\t\treturn obj.Update(GetConditionsByPrimaryKey({0})) > 0;", sbPrimaryKeyValues.ToString()));
                sbCodes.AppendLine("\t\t}");

                sbCodes.AppendLine("");
                sbCodes.AppendLine("\t\t///<summary>");
                sbCodes.AppendLine("\t\t///Update object by primary key(with transation).");
                sbCodes.AppendLine("\t\t///</summary>");
                sbCodes.AppendLine(string.Format("\t\tpublic static bool UpdateObject({4}{0}{1}.{2}{1} obj, {3}, IDbConnection connection, IDbTransaction transaction)", projParam.DOPrefix, tableName, projParam.UOPrefix, sbPrimaryKeyParameters.ToString(), ""));
                sbCodes.AppendLine("\t\t{");

                sbCodes.AppendLine(string.Format("\t\t\treturn obj.Update(connection, transaction, GetConditionsByPrimaryKey({0})) > 0;", sbPrimaryKeyValues.ToString()));
                sbCodes.AppendLine("\t\t}");
                sbCodes.AppendLine("");

                sbCodes.AppendLine("\t\t///<summary>");
                sbCodes.AppendLine("\t\t///Update object by primary key string.");
                sbCodes.AppendLine("\t\t///</summary>");
                sbCodes.AppendLine(string.Format("\t\tpublic static bool UpdateObjectByPrimaryKeyString({0}{1}.{2}{1} obj, string primaryKeyString)", projParam.DOPrefix, tableName, projParam.UOPrefix));
                sbCodes.AppendLine("\t\t{");
                sbCodes.AppendLine("\t\t\treturn obj.Update(GetConditionsByPrimaryKeyString(primaryKeyString)) > 0;");
                sbCodes.AppendLine("\t\t}");

                sbCodes.AppendLine("");
                sbCodes.AppendLine("\t\t///<summary>");
                sbCodes.AppendLine("\t\t///Update object by primary key string(with transation).");
                sbCodes.AppendLine("\t\t///</summary>");
                sbCodes.AppendLine(string.Format("\t\tpublic static bool UpdateObjectByPrimaryKeyString({0}{1}.{2}{1} obj, string primaryKeyString, IDbConnection connection, IDbTransaction transaction)", projParam.DOPrefix, tableName, projParam.UOPrefix));
                sbCodes.AppendLine("\t\t{");
                sbCodes.AppendLine("\t\t\treturn obj.Update(connection, transaction, GetConditionsByPrimaryKeyString(primaryKeyString)) > 0;");
                sbCodes.AppendLine("\t\t}");
                sbCodes.AppendLine("\t\t#endregion");
                sbCodes.AppendLine("");
                #endregion
                #region Delete
                sbCodes.AppendLine("\t\t#region Delete functions");
                sbCodes.AppendLine("\t\t///<summary>");
                sbCodes.AppendLine("\t\t///Delete object by primary key.");
                sbCodes.AppendLine("\t\t///</summary>");
                sbCodes.AppendLine(string.Format("\t\tpublic static int Delete({1}{0})", sbPrimaryKeyParameters.ToString(), ""));
                sbCodes.AppendLine("\t\t{");
                sbCodes.AppendLine(string.Format("\t\t\t{0}{1} da = new {0}{1}();", projParam.DOPrefix, tableName));

                sbCodes.AppendLine(string.Format("\t\t\treturn da.Delete(GetConditionsByPrimaryKey({0}));", sbPrimaryKeyValues.ToString()));
                sbCodes.AppendLine("\t\t}");

                sbCodes.AppendLine("");
                sbCodes.AppendLine("\t\t///<summary>");
                sbCodes.AppendLine("\t\t///Delete object by primary key(with transation).");
                sbCodes.AppendLine("\t\t///</summary>");
                sbCodes.AppendLine(string.Format("\t\tpublic static int Delete({1}{0}, IDbConnection connection, IDbTransaction transaction)", sbPrimaryKeyParameters.ToString(), ""));
                sbCodes.AppendLine("\t\t{");
                sbCodes.AppendLine(string.Format("\t\t\t{0}{1} da = new {0}{1}();", projParam.DOPrefix, tableName));

                sbCodes.AppendLine(string.Format("\t\t\treturn da.Delete(connection, transaction, GetConditionsByPrimaryKey({0}));", sbPrimaryKeyValues.ToString()));
                sbCodes.AppendLine("\t\t}");
                sbCodes.AppendLine("");

                sbCodes.AppendLine("\t\t///<summary>");
                sbCodes.AppendLine("\t\t///Delete object by primary key string.");
                sbCodes.AppendLine("\t\t///</summary>");
                sbCodes.AppendLine("\t\tpublic static int DeleteByPrimaryKeyString(string primaryKeyString)");
                sbCodes.AppendLine("\t\t{");
                sbCodes.AppendLine(string.Format("\t\t\t{0}{1} da = new {0}{1}();", projParam.DOPrefix, tableName));

                sbCodes.AppendLine("\t\t\treturn da.Delete(GetConditionsByPrimaryKeyString(primaryKeyString));");
                sbCodes.AppendLine("\t\t}");

                sbCodes.AppendLine("");
                sbCodes.AppendLine("\t\t///<summary>");
                sbCodes.AppendLine("\t\t///Delete object by primary key string(with transation).");
                sbCodes.AppendLine("\t\t///</summary>");
                sbCodes.AppendLine("\t\tpublic static int DeleteByPrimaryKeyString(string primaryKeyString, IDbConnection connection, IDbTransaction transaction)");
                sbCodes.AppendLine("\t\t{");
                sbCodes.AppendLine(string.Format("\t\t\t{0}{1} da = new {0}{1}();", projParam.DOPrefix, tableName));

                sbCodes.AppendLine("\t\t\treturn da.Delete(connection, transaction, GetConditionsByPrimaryKeyString(primaryKeyString));");
                sbCodes.AppendLine("\t\t}");
                sbCodes.AppendLine("\t\t#endregion");
                sbCodes.AppendLine("");
                #endregion
            }
            sbCodes.AppendLine("\t}");
            if (appendUsing)
            {
                sbCodes.AppendLine("}");
            }
            return sbCodes.ToString();
        }
    }
}
