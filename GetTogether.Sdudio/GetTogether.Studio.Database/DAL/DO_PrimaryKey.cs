//------------------------------------------------------------------------------
// <auto-generated>
//     Date time = 10/16/2012 11:15:44 AM
//     This code was generated by tool,Version=12.10.11.
//     Changes to this code may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GetTogether.Data;
using GetTogether.Mapping;

namespace GetTogether.Studio.Database.DAL
{
    public partial class DO_PrimaryKey : DOBase<DO_PrimaryKey.UO_PrimaryKey, DO_PrimaryKey.UOList_PrimaryKey>
    {
        public static ConnectionInformation GetConnectionInformation()
        {
            return null;
        }
        public override ConnectionInformation GetDefaultConnectionInformation()
        {
            return GetConnectionInformation();
        }
        public enum Columns
        {
            Name,
            AutoIncrement,
        }
        public DO_PrimaryKey()
        {
        }
        public partial class UO_PrimaryKey : UOBase<UO_PrimaryKey, UOList_PrimaryKey>
        {
            public override ConnectionInformation GetDefaultConnectionInformation()
            {
                return GetConnectionInformation();
            }
            #region Columns
            private System.String _Name;
            [Mapping("Name")]
            public System.String Name
            {
                get
                {
                    return _Name;
                }
                set
                {
                    _Name = value;
                }
            }
            private System.Int32 _AutoIncrement;
            [Mapping("AutoIncrement")]
            public System.Int32 AutoIncrement
            {
                get
                {
                    return _AutoIncrement;
                }
                set
                {
                    _AutoIncrement = value;
                }
            }
            #endregion

            public UO_PrimaryKey()
            {
            }
        }
        public partial class UOList_PrimaryKey : GetTogether.ObjectBase.ListBase<UO_PrimaryKey>
        {
            public UOList_PrimaryKey()
            {
            }
        }
    }
}