using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GetTogether;
using System.Collections;
using System.Reflection;
using System.Data.SqlClient;
using GetTogether.Data;

namespace GetTogether.Data
{
    [Serializable]
    public class UOBase<T, C> : CommonBase<T, C>, IUOBase<T, C>, IFormattable
        where T : class, new()
        where C : ICollection<T>, new()
    {
        private GetTogether.Data.ConnectionInformation ConnInfo
        {
            get
            {
               return GetCurrentConnectionInformation();
            }
        }

        public UOBase()
        {

        }

        #region Insert Functions
        public int Insert()
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        return GetTogether.Data.SQL.SqlUtil.ExecuteInsert<T>(conn, ConnInfo.TableName, this as T);
                    case DatabaseType.MySQL:
                        return GetTogether.Data.MySQL.SqlUtil.ExecuteInsert<T>(conn, ConnInfo.TableName, this as T);
                    default: return 0;
                }
                
            }
        }

        public int Insert(IDbConnection cnn, IDbTransaction tran)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    return GetTogether.Data.SQL.SqlUtil.ExecuteInsert<T>(cnn, tran, ConnInfo.TableName, this as T);
                case DatabaseType.MySQL:
                    return GetTogether.Data.MySQL.SqlUtil.ExecuteInsert<T>(cnn, tran, ConnInfo.TableName, this as T);
                default: return 0;
            }
        }
        #endregion

        #region Update Functions

        public int Update(ParameterCollection pc)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        return GetTogether.Data.SQL.SqlUtil.ExecuteUpdate<T>(conn, ConnInfo.TableName, this as T, pc, false);
                    case DatabaseType.MySQL:
                        return GetTogether.Data.MySQL.SqlUtil.ExecuteUpdate<T>(conn, ConnInfo.TableName, this as T, pc, false);
                    default: return 0;
                }
            }
        }

        public int Update(IDbConnection cnn, IDbTransaction tran, ParameterCollection pc)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    return GetTogether.Data.SQL.SqlUtil.ExecuteUpdate<T>(cnn, tran, ConnInfo.TableName, this as T, pc, false);
                case DatabaseType.MySQL:
                    return GetTogether.Data.MySQL.SqlUtil.ExecuteUpdate<T>(cnn, tran, ConnInfo.TableName, this as T, pc, false);
                default: return 0;
            }
        }

        public int UpdateAllColumns(ParameterCollection pc)
        {
            using (System.Data.IDbConnection conn = ConnInfo.Connection)
            {
                switch (ConnInfo.DbType)
                {
                    case DatabaseType.SQLServer:
                        return GetTogether.Data.SQL.SqlUtil.ExecuteUpdate<T>(conn, ConnInfo.TableName, this as T, pc, true);
                    case DatabaseType.MySQL:
                        return GetTogether.Data.MySQL.SqlUtil.ExecuteUpdate<T>(conn, ConnInfo.TableName, this as T, pc, true);
                    default: return 0;
                }
            }
        }

        public int UpdateAllColumns(IDbConnection cnn, IDbTransaction tran, ParameterCollection pc)
        {
            switch (ConnInfo.DbType)
            {
                case DatabaseType.SQLServer:
                    return GetTogether.Data.SQL.SqlUtil.ExecuteUpdate<T>(cnn, tran, ConnInfo.TableName, this as T, pc, true);
                case DatabaseType.MySQL:
                    return GetTogether.Data.MySQL.SqlUtil.ExecuteUpdate<T>(cnn, tran, ConnInfo.TableName, this as T, pc, true);
                default: return 0;
            }
        }

        #endregion

        #region Other Functions

        public override string ToString()
        {
            return this.ConnInfo.TableName;
        }

        string System.IFormattable.ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider != null)
            {
                ICustomFormatter fmt = formatProvider.GetFormat(this.GetType()) as ICustomFormatter;
                if (fmt != null)
                    return fmt.Format(format, this, formatProvider);
                switch (format)
                {
                    case "n": return ToString();
                    case "s": return ConnInfo.DefaultSelect;
                    case "c": return ConnInfo.ConnectionKey;
                    case "cns": return ConnInfo.ConnectionKey + "|" + ConnInfo.TableName + "|" + ConnInfo.DefaultSelect;
                    default: return ToString();
                }
            }
            return ToString();
        }

        #endregion

        #region Other
        //public T Clone()
        //{
        //    object newObject = Activator.CreateInstance(typeof(T));
        //    FieldInfo[] fields = newObject.GetType().GetFields();
        //    int i = 0;
        //    foreach (FieldInfo fi in this.GetType().GetFields())
        //    {
        //        Type ICloneType = fi.FieldType.GetInterface("ICloneable", true);
        //        if (ICloneType != null)
        //        {
        //            ICloneable IClone = (ICloneable)fi.GetValue(this);
        //            fields[i].SetValue(newObject, IClone.Clone());
        //        }
        //        else
        //        {
        //            fields[i].SetValue(newObject, fi.GetValue(this));
        //        }
        //        Type IEnumerableType = fi.FieldType.GetInterface("IEnumerable", true);
        //        if (IEnumerableType != null)
        //        {
        //            IEnumerable IEnum = (IEnumerable)fi.GetValue(this);
        //            Type IListType = fields[i].FieldType.GetInterface("IList", true);
        //            Type IDicType = fields[i].FieldType.GetInterface("IDictionary", true);
        //            int j = 0;
        //            if (IListType != null)
        //            {
        //                IList list = (IList)fields[i].GetValue(newObject);
        //                foreach (object obj in IEnum)
        //                {
        //                    ICloneType = obj.GetType().GetInterface("ICloneable", true);
        //                    if (ICloneType != null)
        //                    {
        //                        ICloneable clone = (ICloneable)obj;
        //                        list[j] = clone.Clone();
        //                    }
        //                    j++;
        //                }
        //            }
        //            else if (IDicType != null)
        //            {
        //                IDictionary dic = (IDictionary)fields[i].GetValue(newObject);
        //                j = 0;
        //                foreach (DictionaryEntry de in IEnum)
        //                {
        //                    ICloneType = de.Value.GetType().
        //                        GetInterface("ICloneable", true);
        //                    if (ICloneType != null)
        //                    {
        //                        ICloneable clone = (ICloneable)de.Value;
        //                        dic[de.Key] = clone.Clone();
        //                    }
        //                    j++;
        //                }
        //            }
        //        }
        //        i++;
        //    }
        //    return (T)newObject;
        //}
        #endregion

        public object this[string name, params string[] formatDateTime]
        {
            get
            {
                return Mapping.ObjectHelper.GetValue<T>(this as T, name);
            }
            set
            {
                if (formatDateTime.Length > 0)
                {
                    Mapping.ObjectHelper.SetValue(this as T, name, value, formatDateTime[0]);
                }
                else
                {
                    Mapping.ObjectHelper.SetValue(this as T, name, value);
                }
            }
        }
    }
}