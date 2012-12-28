using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using GetTogether.Studio.WebService;
using System.Text;
using System.Web.Services.Description;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;

namespace GetTogether.Studio.WebService
{
    public class _Wsdl
    {
        public string Error;
        public System.Reflection.Assembly ProxyAssemble;
        public string Address;
        public List<System.Reflection.MethodInfo> Methods
        {
            get
            {
                return GetWebMethods();
            }
        }

        public _Wsdl(string address)
        {
            this.Address = address;
        }

        public void Generate()
        {
            string codes = WsdlHelper.GetCodes(this.Address);
            Microsoft.CSharp.CSharpCodeProvider codePrivoder = new Microsoft.CSharp.CSharpCodeProvider();
            System.CodeDom.Compiler.ICodeCompiler codeCompiler = codePrivoder.CreateCompiler();
            System.CodeDom.Compiler.CompilerParameters compilerParameters = new System.CodeDom.Compiler.CompilerParameters();
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("System.XML.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Web.Services.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Data.dll");
            compilerParameters.GenerateExecutable = false;
            compilerParameters.GenerateInMemory = true;
            System.CodeDom.Compiler.CompilerResults cr = codeCompiler.CompileAssemblyFromSource(compilerParameters, codes);
            if (cr.Errors.HasErrors)
            {
                Error = "Compile Error:";
                foreach (System.CodeDom.Compiler.CompilerError err in cr.Errors)
                {
                    Error += err.ErrorText;
                }
            }
            else
            {
                this.ProxyAssemble = cr.CompiledAssembly;
            }
        }

        public System.Reflection.MethodInfo GetMethodByName(string methodName)
        {
            if (Methods == null || Methods.Count == 0) return null;
            foreach (System.Reflection.MethodInfo mi in Methods)
            {
                if (mi.Name == methodName) return mi;
            }
            return null;
        }
        private List<System.Reflection.MethodInfo> _Methods;
        private List<System.Reflection.MethodInfo> GetWebMethods()
        {
            if (_Methods != null && _Methods.Count > 0) return _Methods;
            _Methods = new List<System.Reflection.MethodInfo>();
            if (this.ProxyAssemble != null)
            {
                foreach (System.Type type in this.ProxyAssemble.GetTypes())
                {
                    if (IsWebService(type))
                    {
                        System.Web.Services.Protocols.HttpWebClientProtocol proxy = (System.Web.Services.Protocols.HttpWebClientProtocol)Activator.CreateInstance(type);
                        proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                        System.Web.Services.Protocols.SoapHttpClientProtocol protocol2 = proxy as System.Web.Services.Protocols.SoapHttpClientProtocol;
                        if (protocol2 != null)
                        {
                            protocol2.CookieContainer = new System.Net.CookieContainer();
                            protocol2.AllowAutoRedirect = true;
                        }
                        foreach (System.Reflection.MethodInfo info in type.GetMethods())
                        {
                            if (IsWebMethod(info))
                            {
                                _Methods.Add(info);
                            }
                        }
                    }
                }
            }
            return _Methods;
        }

        public object GetObject(string objName)
        {
            Type t = this.ProxyAssemble.GetType(objName, true, true);
            object obj = Activator.CreateInstance(t);
            return obj;
        }

        public static bool IsWebMethod(System.Reflection.MethodInfo method)
        {
            object[] customAttributes = method.GetCustomAttributes(typeof(System.Web.Services.Protocols.SoapRpcMethodAttribute), true);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                return true;
            }
            customAttributes = method.GetCustomAttributes(typeof(System.Web.Services.Protocols.SoapDocumentMethodAttribute), true);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                return true;
            }
            customAttributes = method.GetCustomAttributes(typeof(System.Web.Services.Protocols.HttpMethodAttribute), true);
            return ((customAttributes != null) && (customAttributes.Length > 0));
        }

        public static bool IsWebService(System.Type type)
        {
            return typeof(System.Web.Services.Protocols.HttpWebClientProtocol).IsAssignableFrom(type);
        }
    }
}