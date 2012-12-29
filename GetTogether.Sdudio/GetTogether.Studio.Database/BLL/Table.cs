using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GetTogether.Data;

namespace GetTogether.Studio.Database.BLL
{
    public class Table
    {
        public static DataSet GetTables(ProjectParameter projParam)
        {
            string sql = string.Empty; switch (projParam.DatabaseTypeForCodeEngineer)
            {
                case DatabaseType.MySQL:
                    sql = "select table_name as Name,table_type as TableType from INFORMATION_SCHEMA.tables where table_type<>'system view' and table_schema='{Database}' ORDER BY table_name";
                    break;
                case DatabaseType.Oracle:
                    break;
                case DatabaseType.SQLServer:
                    sql = "select Table_Name as Name,Table_Type as TableType from INFORMATION_SCHEMA.tables where table_catalog='{Database}' ORDER BY [Name]";
                    break;
                default:
                    break;
            }
            sql = sql.Replace("{Database}", BLL.BO_Common.GetDatabase(projParam));
            return BLL.BO_Common.GetDataSet(projParam, sql);
        }

        public static DAL.DO_PrimaryKey.UOList_PrimaryKey GetPrimaryKey(ProjectParameter projParam, string tableName)
        {
            string sql = string.Empty;
            switch (projParam.DatabaseTypeForCodeEngineer)
            {
                case DatabaseType.MySQL:
                    sql = string.Format("SELECT C.Column_name as Name,case when C.extra='auto_increment' then 1 else 0 end as AutoIncrement FROM information_schema.`COLUMNS` C where C.table_schema='{0}' and table_name='{1}' and column_key='PRI' ORDER BY C.Column_name", BLL.BO_Common.GetDatabase(projParam), tableName);
                    break;
                case DatabaseType.Oracle:
                    break;
                case DatabaseType.SQLServer:
                    sql = string.Format("select syscolumns.name AS [Name],columnproperty(syscolumns.id, syscolumns.name, 'IsIdentity') AS AutoIncrement from syscolumns,sysobjects,sysindexes,sysindexkeys where syscolumns.id = object_id('{0}') and sysobjects.xtype = 'pk' and sysobjects.parent_obj = syscolumns.id and sysindexes.id = syscolumns.id and sysobjects.name = sysindexes.name and sysindexkeys.id = syscolumns.id and sysindexkeys.indid = sysindexes.indid and syscolumns.colid = sysindexkeys.colid ORDER BY [Name]", tableName);
                    break;
                default:
                    break;
            }
            DAL.DO_PrimaryKey da = new GetTogether.Studio.Database.DAL.DO_PrimaryKey();
            ConnectionInformation conInfo = new ConnectionInformation();
            conInfo.ConnectionString = projParam.ConnectionString;
            conInfo.DbType = projParam.DatabaseTypeForCodeEngineer;
            da.SetConnectionInformation(conInfo);
            GetTogether.Studio.Database.DAL.DO_PrimaryKey.UOList_PrimaryKey primaryKeys = da.GetList(sql);
            if (primaryKeys == null || primaryKeys.Count == 0)
            {
                List<string> autoIncrementColumns = BLL.Table.GetAutoIncrement(projParam, tableName);
                primaryKeys = new GetTogether.Studio.Database.DAL.DO_PrimaryKey.UOList_PrimaryKey();
                foreach (string pk in autoIncrementColumns)
                {
                    Database.DAL.DO_PrimaryKey.UO_PrimaryKey upk = new GetTogether.Studio.Database.DAL.DO_PrimaryKey.UO_PrimaryKey();
                    upk.Name = pk;
                    primaryKeys.Add(upk);
                }
            }
            return primaryKeys;
        }

        public static List<string> GetAutoIncrement(ProjectParameter projParam, string tableName)
        {
            string sql = string.Empty;
            switch (projParam.DatabaseTypeForCodeEngineer)
            {
                case DatabaseType.MySQL:
                    sql = string.Format("select C.Column_name as Name FROM INFORMATION_SCHEMA.`COLUMNS` C where C.table_schema='{0}' and table_name='{1}' and extra='auto_increment'", BLL.BO_Common.GetDatabase(projParam), tableName);
                    break;
                case DatabaseType.Oracle:
                    break;
                case DatabaseType.SQLServer:
                    sql = string.Format("select sc.name AS [Name] from sysobjects so Inner Join syscolumns sc on so.id = sc.id where columnproperty(sc.id, sc.name, 'IsIdentity') = 1 and upper(so.name) = upper('{0}')", tableName);
                    break;
                default:
                    break;
            }
            using (System.Data.IDbConnection conn = GetTogether.Data.ConnectionHelper.CreateConnection(projParam.ConnectionString, projParam.DatabaseTypeForCodeEngineer))
            {
                return GetTogether.Data.DbUtil.GetExecuteList<string>(conn, sql, projParam.DatabaseTypeForCodeEngineer);
            }
        }

        public static DAL.DO_ColumnDescription.UOList_ColumnDescription GetColumnDescription(ProjectParameter projParam, string tableName)
        {
            DAL.DO_ColumnDescription da = new GetTogether.Studio.Database.DAL.DO_ColumnDescription();
            ConnectionInformation conInfo = new ConnectionInformation();
            conInfo.ConnectionString = projParam.ConnectionString;
            conInfo.DbType = projParam.DatabaseTypeForCodeEngineer;
            da.SetConnectionInformation(conInfo);
            string sql = string.Empty;
            switch (projParam.DatabaseTypeForCodeEngineer)
            {
                case DatabaseType.MySQL:
                    sql = string.Format("SELECT C.Column_name as Name,case when C.extra='auto_increment' then 1 else 0 end as AutoIncrement FROM information_schema.`COLUMNS` C where C.table_schema='{0}' and table_name='{1}' and column_key='PRI'", BLL.BO_Common.GetDatabase(projParam), tableName);
                    break;
                case DatabaseType.Oracle:
                    break;
                case DatabaseType.SQLServer:
                    sql = string.Format("select c.name column_name,p.value remark from sysproperties p join sysobjects o on p.id=o.id join syscolumns c on o.id=c.id and c.colid=p.smallid where p.name='MS_Description' and o.name='{0}'", tableName);
                    try
                    {
                        return da.GetList(sql);
                    }
                    catch
                    {
                        string sqlRetry = string.Format("select c.name AS [Name],p.value Remark from sys.extended_properties p join sysobjects o on p.major_id=o.id join syscolumns c on o.id=c.id and c.colid=p.minor_id where p.name='MS_Description' and o.name='{0}'", tableName);
                        return da.GetList(sqlRetry);
                    }
                default:
                    break;
            }
            return da.GetList(sql);
        }

        public static DAL.DO_ColumnDetail.UOList_ColumnDetail GetColumnDetail(ProjectParameter projParam, string tableName)
        {
            DAL.DO_ColumnDetail da = new GetTogether.Studio.Database.DAL.DO_ColumnDetail();
            ConnectionInformation conInfo = new ConnectionInformation();
            conInfo.ConnectionString = projParam.ConnectionString;
            conInfo.DbType = projParam.DatabaseTypeForCodeEngineer;
            da.SetConnectionInformation(conInfo);
            string sql = string.Empty;
            switch (projParam.DatabaseTypeForCodeEngineer)
            {
                case DatabaseType.MySQL:
                    sql = string.Format("select column_name AS ColumnName,column_default AS DefaultValue,case when is_nullable='YES' THEN 1 ELSE 0 end AS IsNullable,data_type AS DataType,character_maximum_length as MaxLength from INFORMATION_SCHEMA.COLUMNS where table_name='{0}'", tableName);
                    break;
                case DatabaseType.Oracle:
                    break;
                case DatabaseType.SQLServer:
                    sql = string.Format("select column_name AS ColumnName,column_default AS DefaultValue,case when is_nullable='YES' THEN 1 ELSE 0 end AS IsNullable,data_type AS DataType,character_maximum_length as MaxLength from INFORMATION_SCHEMA.COLUMNS where table_name='{0}'", tableName);
                    break;
                default:
                    break;
            }
            return da.GetList(sql);
        }

    }
}
