using System.Net;
using System;
using System.Web.Services.Description;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;

namespace GetTogether.Web
{
    public static class WebServiceHelper
    {
        public static object Invoke(string url, string methodName, object[] parameters)
        {
            return WebServiceHelper.Invoke(url, null, methodName, parameters);
        }

        public static object Invoke(string url, string className, string methodName, object[] parameters)
        {
            string compileNamespace = "Dynamic.To.Call.WebService";
            if ((className == null) || (className == ""))
            {
                className = WebServiceHelper.GetClassName(url);
            }

            //Get wsdl information
            WebClient wc = new WebClient();
            System.IO.Stream strm = wc.OpenRead(url + "?WSDL");
            ServiceDescription sd = ServiceDescription.Read(strm);
            ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
            sdi.AddServiceDescription(sd, "", "");
            CodeNamespace cn = new CodeNamespace(compileNamespace);


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


            //Build agent entity & call method
            System.Reflection.Assembly assembly = cr.CompiledAssembly;
            Type t = assembly.GetType(string.Concat(compileNamespace, ".", className), true, true);
            object obj = Activator.CreateInstance(t);
            System.Reflection.MethodInfo mi = t.GetMethod(methodName);
            if (parameters != null && parameters.Length > 0)
                return mi.Invoke(obj, parameters);
            else
                //Invoke(obj, BindingFlags.Default, new MyBinder(), new Object[] { (int)32, (int)32 }, CultureInfo.CurrentCulture);//ref to msdn
                return mi.Invoke(obj, BindingFlags.Default, null, null, null);
        }

        //public static object Invoke(string url, string className, string methodName, object[] parameters)
        //{
        //    string compileNamespace = "Dynamic.To.Call.WebService";
        //    if ((className == null) || (className == ""))
        //    {
        //        className = WebServiceHelper.GetClassName(url);
        //    }

        //    //Get wsdl information
        //    WebClient wc = new WebClient();
        //    System.IO.Stream strm = wc.OpenRead(url + "?WSDL");
        //    ServiceDescription sd = ServiceDescription.Read(strm);
        //    ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
        //    sdi.AddServiceDescription(sd, "", "");
        //    CodeNamespace cn = new CodeNamespace(compileNamespace);


        //    //Generate client proxy class
        //    CodeCompileUnit ccu = new CodeCompileUnit();
        //    ccu.Namespaces.Add(cn);
        //    sdi.Import(cn, ccu);
        //    CSharpCodeProvider csc = new CSharpCodeProvider();
        //    ICodeCompiler icc = csc.CreateCompiler();


        //    //Setting compile parameters
        //    CompilerParameters compilerParams = new CompilerParameters();
        //    compilerParams.GenerateExecutable = false;
        //    compilerParams.GenerateInMemory = true;
        //    compilerParams.ReferencedAssemblies.Add("System.dll");
        //    compilerParams.ReferencedAssemblies.Add("System.XML.dll");
        //    compilerParams.ReferencedAssemblies.Add("System.Web.Services.dll");
        //    compilerParams.ReferencedAssemblies.Add("System.Data.dll");


        //    //compile agentcy class
        //    CompilerResults cr = icc.CompileAssemblyFromDom(compilerParams, ccu);
        //    if (true == cr.Errors.HasErrors)
        //    {
        //        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //        foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
        //        {
        //            sb.Append(ce.ToString());
        //            sb.Append(System.Environment.NewLine);
        //        }
        //        throw new Exception(sb.ToString());
        //    }


        //    //Build agent entity & call method
        //    System.Reflection.Assembly assembly = cr.CompiledAssembly;
        //    Type t = assembly.GetType(string.Concat(compileNamespace, ".", className), true, true);
        //    object obj = Activator.CreateInstance(t);
        //    System.Reflection.MethodInfo mi = t.GetMethod(methodName);
        //    if (parameters != null && parameters.Length > 0)
        //        return mi.Invoke(obj, parameters);
        //    else
        //        //Invoke(obj, BindingFlags.Default, new MyBinder(), new Object[] { (int)32, (int)32 }, CultureInfo.CurrentCulture);//ref to msdn
        //        return mi.Invoke(obj, BindingFlags.Default, null, null, null);
        //}

        private static string GetClassName(string url)
        {
            string[] parts = url.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');
            return pps[0];
        }
    }
}