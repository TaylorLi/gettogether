using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace GetTogether.Studio.Database
{
    public class CodeGenerator
    {
        public enum CodeType
        {
            DAL,
            BLL,
        }

        private ProjectParameter _Parameter;

        public ProjectParameter Parameter
        {
            get { return _Parameter; }
            set { _Parameter = value; }
        }

        public CodeGenerator(ProjectParameter st)
        {
            this._Parameter = st;
        }

        #region Functions

        public DataSet GetTables()
        {
            return Database.BLL.Table.GetTables(Parameter);
        }

        public DataSet GetStoreProcedures()
        {
            return Database.BLL.StoreProcedure.GetStoreProcedures(Parameter);
        }

        public string GetStoreProcedureSimple(string spName)
        {
            switch (Parameter.DatabaseTypeForCodeEngineer)
            {
                case GetTogether.Data.DatabaseType.SQLServer:
                    return Database.BLL.StoreProcedure.GetStoreProcedureSimple(Parameter, spName);
                case GetTogether.Data.DatabaseType.MySQL:
                    return Database.BLL.StoreProcedure.GetStoreProcedureSimple(Parameter, spName);
                case GetTogether.Data.DatabaseType.Oracle:
                    break;
                default:
                    break;
            }
            return spName;
        }

        public string GenerateCodeByScript(CodeType codeType, string tableName, string sql, string primaryKey, bool appendUsing)
        {
            System.Data.IDataReader dReader = Database.BLL.BO_Common.GetDataReader(Parameter, sql);
            using (dReader)
            {
                GetTogether.Studio.Database.ColumnMapping.ColumnInfos cis = GetTogether.Studio.Database.ColumnMapping.GetColumnInfo(dReader, this.Parameter, string.Empty, primaryKey);
                if (codeType == CodeType.BLL)
                    return GetTogether.Studio.Database.SQLServer.TableScript.GetBLL(Parameter, tableName, cis, true, appendUsing);
                else
                    return GetTogether.Studio.Database.SQLServer.TableScript.GetDAL(Parameter, tableName, cis, Database.BLL.Table.GetColumnDescription(Parameter, tableName), appendUsing);
            }
        }

        public GetTogether.Studio.Database.ColumnMapping.ColumnInfos GetColumnsInfo(string tableName, string primaryKeyOverwride)
        {
            using (IDataReader dReader = Database.BLL.BO_Common.GetDataReaderByTable(Parameter, tableName))
            {
                GetTogether.Studio.Database.ColumnMapping.ColumnInfos cis = GetTogether.Studio.Database.ColumnMapping.GetColumnInfo(dReader, this.Parameter, tableName, primaryKeyOverwride);
                return cis;
            }
        }

        public string GenerateCodeByTable(CodeType codeType, string tableName, string primaryKeyOverwride, bool appendUsing)
        {
            using (IDataReader dReader = Database.BLL.BO_Common.GetDataReaderByTable(Parameter, tableName))
            {
                GetTogether.Studio.Database.ColumnMapping.ColumnInfos cis = GetTogether.Studio.Database.ColumnMapping.GetColumnInfo(dReader, this.Parameter, tableName, primaryKeyOverwride);
                if (codeType == CodeType.BLL)
                    return GetTogether.Studio.Database.SQLServer.TableScript.GetBLL(Parameter, tableName, cis, false, appendUsing);
                else
                    return GetTogether.Studio.Database.SQLServer.TableScript.GetDAL(Parameter, tableName, cis, Database.BLL.Table.GetColumnDescription(Parameter, tableName), appendUsing);
            }
        }

        public string GenerateCodeByStoreProcedure(CodeType codeType, string objName, string spName, string sql, string returnObjMapping)
        {
            if (codeType == CodeType.BLL)
                return GetTogether.Studio.Database.StoreProcedure.GetBLL(Parameter, objName, spName);
            else
            {
                return GetTogether.Studio.Database.StoreProcedure.GetDAL(Parameter, objName, spName, sql, returnObjMapping);
            }
        }

        #endregion
    }
}
