using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Studio.Database
{
    public class ColumnMapping
    {
        #region Attributes

        public class ColumnInfos : List<ColumnInfo>
        {
            private List<string> _AutoIncrements = new List<string>();

            public List<string> AutoIncrements
            {
                get { return _AutoIncrements; }
                set { _AutoIncrements = value; }
            }
            private List<string> _PrimaryKeys = new List<string>();

            public List<string> PrimaryKeys
            {
                get { return _PrimaryKeys; }
                set { _PrimaryKeys = value; }
            }

            public ColumnInfos()
            {

            }
        }

        public class ColumnInfo
        {
            private string _ColumnName;
            public string ColumnName
            {
                get { return _ColumnName; }
                set { _ColumnName = value; }
            }
            private string _ColumnType;
            public string ColumnType
            {
                get { return _ColumnType; }
                set { _ColumnType = value; }
            }
            private bool _IsPrimaryKey;
            public bool IsPrimaryKey
            {
                get { return _IsPrimaryKey; }
                set { _IsPrimaryKey = value; }
            }
            private bool _IsAutoIncrement;
            public bool IsAutoIncrement
            {
                get { return _IsAutoIncrement; }
                set { _IsAutoIncrement = value; }
            }
            private DAL.DO_ColumnDetail.UO_ColumnDetail _ColumnDetail;
            public DAL.DO_ColumnDetail.UO_ColumnDetail ColumnDetail
            {
                get { return _ColumnDetail; }
                set { _ColumnDetail = value; }
            }
        }

        #endregion

        public static ColumnInfos GetColumnInfo(System.Data.IDataReader dr, ProjectParameter projParam, string tableName, string primaryKeyOverwride)
        {
            DAL.DO_ColumnDetail.UOList_ColumnDetail columnDetailList = null;
            List<string> autoIncrementColumns = null;
            DAL.DO_PrimaryKey.UOList_PrimaryKey primaryKeys = null;
            if (!string.IsNullOrEmpty(tableName))
            {
                columnDetailList = BLL.Table.GetColumnDetail(projParam, tableName);
                autoIncrementColumns = BLL.Table.GetAutoIncrement(projParam, tableName);
                primaryKeys = BLL.Table.GetPrimaryKey(projParam, tableName);
            }
            ColumnInfos ret = new ColumnInfos();
            string m = string.Empty;
            for (int i = 0; i < dr.FieldCount; i++)
            {
                ColumnInfo ci = new ColumnInfo();
                ci.ColumnName = dr.GetName(i);
                ci.ColumnType = dr.GetFieldType(i).ToString();
                if (autoIncrementColumns != null) ci.IsAutoIncrement = autoIncrementColumns.Contains(ci.ColumnName);
                if (primaryKeys != null)
                {
                    foreach (DAL.DO_PrimaryKey.UO_PrimaryKey pk in primaryKeys)
                    {
                        if (pk.Name == ci.ColumnName)
                        {
                            ci.IsPrimaryKey = true;
                            if (pk.AutoIncrement > 0) ci.IsAutoIncrement = true;
                        }
                    }
                }
                if (columnDetailList != null)
                {
                    foreach (DAL.DO_ColumnDetail.UO_ColumnDetail cd in columnDetailList)
                    {
                        if (cd.ColumnName == ci.ColumnName)
                        {
                            ci.ColumnDetail = cd;
                        }
                    }
                }
                if (ci.IsPrimaryKey)
                {
                    ret.PrimaryKeys.Add(ci.ColumnName);
                }
                if (ci.IsAutoIncrement)
                {
                    ret.AutoIncrements.Add(ci.ColumnName);
                }

                ret.Add(ci);
            }
            if (!string.IsNullOrEmpty(primaryKeyOverwride))
            {
                ret.PrimaryKeys.Clear();
                List<string> overwritePrimaryKeys = GetTogether.Utility.StringHelper.StringToList<string>(primaryKeyOverwride, ',');
                foreach (ColumnMapping.ColumnInfo ci in ret)
                {
                    ci.IsPrimaryKey = false;
                    ci.IsPrimaryKey = overwritePrimaryKeys.Contains(ci.ColumnName);
                    if (ci.IsPrimaryKey)
                    {
                        ret.PrimaryKeys.Add(ci.ColumnName);
                    }
                }
            }
            return ret;
        }

        public static string FromDatabaseType(string type)
        {
            if (type.IndexOf(".") > 0) return type;
            string reval;

            switch (type.ToLower())
            {
                case "int":
                    reval = "int";
                    break;
                case "text":
                    reval = "string";
                    break;
                case "bigint":
                    reval = "Int64";
                    break;
                case "binary":
                    reval = "byte[]";
                    break;
                case "bit":
                    reval = "bool";
                    break;
                case "char":
                    reval = "string";
                    break;
                case "datetime":
                    reval = "DateTime";
                    break;
                case "decimal":
                    reval = "decimal";
                    break;
                case "float":
                    reval = "double";
                    break;
                case "image":
                    reval = "byte[]";
                    break;
                case "money":
                    reval = "decimal";
                    break;
                case "nchar":
                    reval = "string";
                    break;
                case "ntext":
                    reval = "string";
                    break;
                case "numeric":
                    reval = "decimal";
                    break;
                case "nvarchar":
                    reval = "string";
                    break;
                case "real":
                    reval = "Single";
                    break;
                case "smalldatetime":
                    reval = "DateTime";
                    break;
                case "smallint":
                    reval = "Int16";
                    break;
                case "smallmoney":
                    reval = "decimal";
                    break;
                case "timestamp":
                    reval = "DateTime";
                    break;
                case "tinyint":
                    reval = "byte";
                    break;
                case "uniqueidentifier":
                    reval = "Guid";
                    break;
                case "varbinary":
                    reval = "byte[]";
                    break;
                case "varchar":
                    reval = "string";
                    break;
                case "Variant":
                    reval = "object";
                    break;
                default:
                    reval = "string";
                    break;
            }
            return reval;
        }

        public static string GetColumnType(ColumnMapping.ColumnInfos ci, string column)
        {
            foreach (ColumnMapping.ColumnInfo c in ci)
            {
                if (c.ColumnName == column)
                {
                    return c.ColumnType;
                }
            }
            return "string";
        }
    }
}
