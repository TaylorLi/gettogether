using System;
using System.Collections.Generic;
using System.Web;

namespace GetTogether.Studio.WebService
{
    public class WebServiceHelper
    {
        public WebServiceHelper()
        {

        }

        public static Wsdl GetWsdl(GetTogether.Studio.WebService.ProjectParameter parameter, bool isRefresh)
        {
            Wsdl wsdl = GetTogether.Web.CacheHelper.GetCache(parameter.ProjectName) as Wsdl;
            if (wsdl == null || isRefresh)
            {
                wsdl = new Wsdl(parameter.Address, parameter.AddressType);
                wsdl.Generate();
                if (parameter.Timeout > 0)
                {
                    wsdl.SetTimeout(parameter.Timeout);
                }
                GetTogether.Web.CacheHelper.SetCache(parameter.ProjectName, wsdl);
            }
            return wsdl;
        }
    }
}