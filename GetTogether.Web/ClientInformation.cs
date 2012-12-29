using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Web
{
    public class ClientInformation
    {
        private string _OperationSystem;

        public string OperationSystem
        {
            get { return _OperationSystem; }
            set { _OperationSystem = value; }
        }


        private const string _SessionKey = "Client-Information";
        public static ClientInformation GetClientInformation()
        {
            ClientInformation ci= (ClientInformation)System.Web.HttpContext.Current.Session[_SessionKey];
            if (ci == null) ci = new ClientInformation();
            return ci;
        }

        public static void SetClientInformation(ClientInformation ci)
        {
            System.Web.HttpContext.Current.Session[_SessionKey] = ci;
        }
    }
}
