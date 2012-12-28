using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Data
{
    public enum ParameterType
    {
        And,
        Or,
        Initial,
    }

    public enum TokenTypes
    {
        Equal,
        IsNull,
        IsNotNull,
        Like,
        LeftLike,
        RightLike,
        LessThanEqual,
        LessThan,
        GreaterThan,
        GreaterThanEqual,
        Unknown,
        In,
        NotIn,
        NotEqual,
    }

    public enum DatabaseType
    {
        SQLServer = 1,
        MySQL = 2,
        Oracle = 3,
    }
}
