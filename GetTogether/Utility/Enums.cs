using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Utility
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
        Unknown
    }
}
