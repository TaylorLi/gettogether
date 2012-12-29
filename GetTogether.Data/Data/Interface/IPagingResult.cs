using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Data
{
    public interface IPagingResult<T, C>
    {
        C Result
        {
            get;
            set;
        }

        int Total
        {
            get;
            set;
        }
    }
}
