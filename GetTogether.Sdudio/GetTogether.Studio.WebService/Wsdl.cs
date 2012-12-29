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
using System.Web.Services.Protocols;
using System.Collections;

namespace GetTogether.Studio.WebService
{
    public class Wsdl
    {
        #region Attributes

        public string Error;
        public System.Reflection.Assembly ProxyAssemble;
        public string Address;
        public string ComileNamespace = string.Empty;
        private List<System.Reflection.MethodInfo> _Methods;
        public AddressType AddressType = AddressType.WebService;
        public List<System.Reflection.MethodInfo> Methods
        {
            get
            {
                return GetWebMethods();
            }
        }
        public object ServiceObject;

        #endregion

        public Wsdl(string address, AddressType addressType)
        {
            this.Address = address;
            this.AddressType = addressType;
        }

        #region Private Functions

        private List<System.Reflection.MethodInfo> GetWebMethods()
        {
            if (_Methods != null && _Methods.Count > 0) return _Methods;
            _Methods = new List<System.Reflection.MethodInfo>();
            if (this.AddressType == AddressType.WebService)
            {
                if (this.ProxyAssemble != null)
                {
                    foreach (System.Type type in this.ProxyAssemble.GetTypes())
                    {
                        if (MethodHelper.IsWebService(type))
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
                                if (MethodHelper.IsWebMethod(info))
                                {
                                    _Methods.Add(info);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (System.Reflection.MethodInfo info in this.ServiceObject.GetType().GetMethods())
                {
                    if (info.MemberType == MemberTypes.Method && info.Name == "SendRequest" || info.Name == "SendRequestAdvanced")
                    {
                        _Methods.Add(info);
                    }
                }
            }
            return _Methods;
        }

        #endregion

        public object Invoke(string methodName, object[] parameters)
        {
            try
            {
                System.Reflection.MethodInfo mi = GetMethodByName(methodName);
                return mi.Invoke(this.ServiceObject, parameters);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public void Generate()
        {
            if (this.AddressType == AddressType.WebService)
            {
                string className = MethodHelper.GetClassName(this.Address);
                //Get wsdl information
                System.Net.WebClient wc = new System.Net.WebClient();
                System.IO.Stream strm = wc.OpenRead(Address + "?WSDL");
                ServiceDescription sd = ServiceDescription.Read(strm);
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(this.ComileNamespace);
                //Generate client proxy class
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider csc = new CSharpCodeProvider();
                ICodeCompiler icc = csc.CreateCompiler();
                //Setting compile parameters
                CompilerParameters compilerParams = new CompilerParameters();
                compilerParams.GenerateExecutable = false;
                compilerParams.GenerateInMemory = true;
                compilerParams.ReferencedAssemblies.Add("System.dll");
                compilerParams.ReferencedAssemblies.Add("System.XML.dll");
                compilerParams.ReferencedAssemblies.Add("System.Web.Services.dll");
                compilerParams.ReferencedAssemblies.Add("System.Data.dll");
                //compile agentcy class
                CompilerResults cr = icc.CompileAssemblyFromDom(compilerParams, ccu);
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }
                this.ProxyAssemble = cr.CompiledAssembly;
                Type t = this.ProxyAssemble.GetType(string.Concat(this.ComileNamespace, ".", className), true, true);
                ServiceObject = Activator.CreateInstance(t);
            }
            else
            {
                ServiceObject = new NormalRequest();
            }
            SetUrl(this.Address);
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

        public void SetUrl(string address)
        {
            if (this.ServiceObject != null)
                GetTogether.Mapping.ObjectHelper.SetValue(this.ServiceObject, "Url", address);
        }

        public void SetTimeout(int timeout)
        {
            if (this.ServiceObject != null)
                GetTogether.Mapping.ObjectHelper.SetValue(this.ServiceObject, "Timeout", timeout);
        }
        public FieldInfo[] GetHeaders(MethodInfo mi)
        {
            return WsdlHelper.GetSoapHeaders(mi, true);
        }
        public object GetObject(Type type)
        {
            bool isArray = type.FullName.IndexOf("[]") > 0;
            string typeName = type.FullName.Replace("[]", "").Replace("&", "");
            System.Type systemType = System.Type.GetType(typeName);
            if (typeName == "System.Xml.XmlAttribute" || typeName == "System.Globalization.CultureInfo" || typeName == "System.Data.DataColumn") return null;
            Type t = null;
            if (systemType == null)
            {
                t = this.ProxyAssemble.GetType(typeName, false, true);
                if (t == null) t = type;
                //t = type;
            }
            else
                t = systemType;
            object obj = null;
            if (systemType == null)
                obj = Activator.CreateInstance(t);
            else
                obj = WsdlHelper.GetDefaultValueByType(systemType);
            if (obj == null) obj = Activator.CreateInstance(t);
            TypeCode typeCode = Type.GetTypeCode(t);
            if (typeCode == TypeCode.Object && !type.IsEnum && obj != null)
            {
                if (typeName == "System.Data.DataTable") GetTogether.Mapping.ObjectHelper.SetValue(obj, "TableName", string.Concat("TableName-", obj.GetHashCode()));
                foreach (System.Reflection.MemberInfo mi in obj.GetType().GetMembers())
                {
                    Type currentType = mi.GetType();
                    if (mi.MemberType == MemberTypes.Field)
                    {
                        FieldInfo field = obj.GetType().GetField(mi.Name);
                        if (field.FieldType.IsAbstract) continue;
                        if (Type.GetTypeCode(field.FieldType) == TypeCode.Object && !field.FieldType.IsEnum)
                        {
                            field.SetValue(obj, GetObject(field.FieldType));
                        }
                        else
                        {
                            field.SetValue(obj, WsdlHelper.GetDefaultValueByType(field.FieldType));
                        }
                    }
                    else if (mi.MemberType == MemberTypes.Property)
                    {
                        PropertyInfo pi = obj.GetType().GetProperty(mi.Name);
                        if (pi.PropertyType.IsAbstract) continue;
                        if (!pi.CanWrite) continue;
                        if (Type.GetTypeCode(pi.PropertyType) == TypeCode.Object && !pi.PropertyType.IsEnum)
                        {
                            pi.SetValue(obj, GetObject(pi.PropertyType), null);
                        }
                        else
                        {
                            object defaultValue = pi.GetValue(obj, null);
                            if (defaultValue == null)
                            {
                                pi.SetValue(obj, WsdlHelper.GetDefaultValueByType(pi.PropertyType), null);
                            }
                        }
                    }
                }
            }
            if (isArray && obj != null)
            {
                Array arr = Array.CreateInstance(obj.GetType(), 2);
                arr.SetValue(obj, 0);
                arr.SetValue(obj, 1);
                return arr;
            }
            return obj;
        }
    }
}