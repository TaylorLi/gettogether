using System;
using System.Data;
using System.Configuration;
using System.Web;

namespace GetTogether.Studio.Web
{
    public class SessionObjects
    {
        public SessionObjects()
        {
            
        }

        private string _Firstname;

        public string Firstname
        {
            get { return _Firstname; }
            set { _Firstname = value; }
        }
        private string _Lastname;

        public string Lastname
        {
            get { return _Lastname; }
            set { _Lastname = value; }
        }

        private string _UserCode;

        public string UserCode
        {
            get { return _UserCode; }
            set { _UserCode = value; }
        }

        public string ShareUserCode
        {
            get
            {
                return System.Web.HttpContext.Current.Request["share-usercode"];
            }
        }
    }
}