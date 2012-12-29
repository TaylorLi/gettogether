using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Studio.WebService
{
    public class ProjectHistory
    {
        private DateTime _RecentUsed;

        public DateTime RecentUsed
        {
            get { return _RecentUsed; }
            set { _RecentUsed = value; }
        }
    }
}
