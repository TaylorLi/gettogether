using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Studio.Database
{
    public class TableScriptHelper
    {
        public static void GetConditionsByPrimaryKeyString(StringBuilder sbCodes, ColumnMapping.ColumnInfos colInfos, StringBuilder sbPrimaryKeyValues)
        {
            StringBuilder sbMethodContent = new StringBuilder();
            sbMethodContent.AppendLine("\t\t\tstring[] primaryKeys = primaryKeyString.Split('|');");
            int index = 0;
            foreach (ColumnMapping.ColumnInfo ci in colInfos)
            {
                if (ci.IsPrimaryKey)
                {
                    string columnType = ColumnMapping.GetColumnType(colInfos, ci.ColumnName);
                    sbMethodContent.Append("\t\t\t").AppendLine(GetFieldDefine(columnType, ci.ColumnName, index));
                    index++;
                }
            }
            sbMethodContent.AppendFormat("\t\t\treturn GetConditionsByPrimaryKey({0});", sbPrimaryKeyValues.ToString());
            sbCodes.AppendLine("\t\t///<summary>");
            sbCodes.AppendLine("\t\t///Get conditions by primary key string.");
            sbCodes.AppendLine("\t\t///</summary>");
            sbCodes.AppendLine(string.Format("\t\tpublic static ParameterCollection GetConditionsByPrimaryKeyString({0})", "string primaryKeyString"));
            sbCodes.AppendLine("\t\t{");
            sbCodes.AppendLine(sbMethodContent.ToString());
            sbCodes.AppendLine("\t\t}");
        }

        public static void GetObjectByPrimaryKeyString(StringBuilder sbCodes, ProjectParameter projParam, string tableName)
        {
            sbCodes.AppendLine("\t\t///<summary>");
            sbCodes.AppendLine("\t\t///Get object by primary key string.");
            sbCodes.AppendLine("\t\t///</summary>");
            sbCodes.AppendLine(string.Format("\t\tpublic static {0}{1}.{2}{1} GetObjectByPrimaryKeyString(string primaryKeyString)", projParam.DOPrefix, tableName, projParam.UOPrefix));
            sbCodes.AppendLine("\t\t{");
            sbCodes.AppendLine(string.Format("\t\t\t{0}{1} da = new {0}{1}();", projParam.DOPrefix, tableName));
            sbCodes.AppendLine(string.Format("\t\t\t{0}{1}.{2}{1} l = da.GetList(GetConditionsByPrimaryKeyString(primaryKeyString));", projParam.DOPrefix, tableName, projParam.UOListPrefix));
            sbCodes.AppendLine("\t\t\treturn l.Count > 0 ? l[0] : null;");
            sbCodes.AppendLine("\t\t}");
        }

        public static string GetFieldDefine(string columnType, string columnName, int primaryKeyIndex)
        {
            string[] columnTypeNameInfo = columnType.Split('.');
            string columnTypeName = columnTypeNameInfo[columnTypeNameInfo.Length - 1];
            if (columnTypeName.ToLower() != "string")
            {
                return string.Format("{0} {1} = Convert.To{2}(primaryKeys[{3}]);", columnType, columnName, columnTypeName, primaryKeyIndex);
            }
            return string.Format("{0} {1} = primaryKeys[{2}];", columnType, columnName, primaryKeyIndex);
        }
    }
}
