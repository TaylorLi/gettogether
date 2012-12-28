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
using System.Net;
using System.IO;

namespace GetTogether.Studio.WebService
{
    public class WsdlHelper
    {
        public static object GetParameterDefaultValue(Wsdl wsdl, Type paramType)
        {
            if (!IsSystemType(paramType))
            {
                object paramObj = wsdl.GetObject(paramType);
                return paramObj;
            }
            else
            {
                return GetDefaultValueByType(paramType);
            }
        }

        public static string ParameterToString(object paramObj)
        {
            Type paramType = paramObj.GetType();
            if (!IsSystemType(paramType))
            {
                if (paramType.IsEnum)
                {
                    return ((int)paramObj).ToString();
                }
                else
                {
                    return GetTogether.Utility.SerializationHelper.SerializeToXml(paramObj);
                }
            }
            else
            {
                switch (System.Type.GetTypeCode(paramType))
                {
                    case TypeCode.Boolean: return paramObj.ToString();
                    case TypeCode.Byte: return paramObj == null ? "" : paramObj.ToString();
                    case TypeCode.Char: return paramObj == null ? "" : paramObj.ToString();
                    case TypeCode.DBNull: return "";
                    case TypeCode.DateTime: return paramObj == null ? DateTime.Now.ToString() : paramObj.ToString();
                    case TypeCode.Decimal: return paramObj == null ? "0" : paramObj.ToString();
                    case TypeCode.Double: return paramObj == null ? "0" : paramObj.ToString();
                    case TypeCode.Empty: return paramObj == null ? "" : paramObj.ToString();
                    case TypeCode.Int16: return paramObj == null ? "0" : paramObj.ToString();
                    case TypeCode.Int32: return paramObj == null ? "0" : paramObj.ToString();
                    case TypeCode.Int64: return paramObj == null ? "0" : paramObj.ToString();
                    case TypeCode.Object:
                        break;
                    case TypeCode.SByte: return paramObj == null ? "0" : paramObj.ToString();
                    case TypeCode.Single: return paramObj == null ? "0" : paramObj.ToString();
                    case TypeCode.String: return paramObj == null ? "" : paramObj.ToString();
                    case TypeCode.UInt16: return paramObj == null ? "0" : paramObj.ToString();
                    case TypeCode.UInt32: return paramObj == null ? "0" : paramObj.ToString();
                    case TypeCode.UInt64: return paramObj == null ? "0" : paramObj.ToString();
                    default:
                        break;
                }
                return string.Empty;
            }
        }

        public static object GetDefaultValueByType(System.Type filedType)
        {
            switch (System.Type.GetTypeCode(filedType))
            {
                case TypeCode.Boolean: return false;
                case TypeCode.Byte: return (byte)0;
                case TypeCode.Char: return "";
                case TypeCode.DBNull: return null;
                case TypeCode.DateTime: return DateTime.Now;
                case TypeCode.Decimal: return (decimal)0;
                case TypeCode.Double: return (double)0;
                case TypeCode.Empty: return string.Empty;
                case TypeCode.Int16: return (Int16)0;
                case TypeCode.Int32: return (int)0;
                case TypeCode.Int64: return (Int64)0;
                case TypeCode.Object: return null;
                case TypeCode.SByte: return (sbyte)0;
                case TypeCode.Single: return (Single)0;
                case TypeCode.String: return "";
                case TypeCode.UInt16: return (UInt16)0;
                case TypeCode.UInt32: return (UInt32)0;
                case TypeCode.UInt64: return (UInt64)0;
                default:
                    break;
            }
            return null;
        }
        public static string GetEnumInfo(Type type)
        {
            if (!type.IsEnum) return string.Empty;
            StringBuilder sbInfo = new StringBuilder();
            foreach (object a in Enum.GetValues(type))
            {
                //object obj = Enum.ToObject(type, a);
                if (sbInfo.Length > 0) sbInfo.Append(", ");
                sbInfo.AppendFormat("{0}={1}", Enum.GetName(type, a), (int)a);
            }
            //sbInfo.Insert(0, " (").Append(")");
            return string.Concat("<span style='color:blue;'>enum</span> <span style='color:#2B91AF;'>", type.FullName, "</span> { ", sbInfo.ToString(), " }");
        }
        public static bool IsSystemType(System.Type type)
        {
            if (System.Type.GetTypeCode(type) == TypeCode.Object)
                return false;
            else if (type.IsEnum)
                return false;
            return true;
        }

        public static object ConvertParameter(Wsdl wsdl, System.Reflection.ParameterInfo pi, string rqtValue)
        {
            System.TypeCode typeCode = System.Type.GetTypeCode(pi.ParameterType);
            if (pi.ParameterType.IsEnum)
            {
                return Enum.ToObject(pi.ParameterType, int.Parse(rqtValue));
            }
            if (typeCode == TypeCode.Object)
            {
                return GetTogether.Utility.SerializationHelper.DeserializeObject<object>(wsdl.GetObject(pi.ParameterType).GetType(), rqtValue);
            }
            return Convert.ChangeType(rqtValue, pi.ParameterType);
        }

        public static object ConvertParameter(Wsdl wsdl, FieldInfo fi, string rqtValue)
        {
            System.TypeCode typeCode = System.Type.GetTypeCode(fi.FieldType);
            if (fi.FieldType.IsEnum)
            {
                return Enum.ToObject(fi.FieldType, int.Parse(rqtValue));
            }
            if (typeCode == TypeCode.Object)
            {
                return GetTogether.Utility.SerializationHelper.DeserializeObject<object>(wsdl.GetObject(fi.FieldType).GetType(), rqtValue);
            }
            return Convert.ChangeType(rqtValue, fi.FieldType);
        }

        public static string GetCodes(string url)
        {
            try
            {
                string address = string.Concat(url, "?WSDL");
                string CmdString_Class = @"wsdl.exe {0} /out:{1}";
                string dirBase = System.IO.Path.Combine(Setting.GetIncludeFolder(), "WebService");
                string timeFlag = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string fileNoSuffix = string.Concat(dirBase, timeFlag);
                string fileExe = string.Concat(dirBase, timeFlag, ".bat");
                string fileCode = string.Concat(dirBase, timeFlag, ".cs");
                System.IO.File.WriteAllText(fileExe, string.Format(CmdString_Class, address, fileCode), System.Text.Encoding.ASCII);
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = fileExe;
                p.StartInfo.WorkingDirectory = dirBase;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                string ret = p.StandardOutput.ReadToEnd();
                int timeSlip = 0;
                while (true)
                {
                    if (System.IO.File.Exists(fileCode))
                    {
                        break;
                    }
                    timeSlip++;
                    if (timeSlip > 20)
                    {
                        return ret;
                    }
                    System.Threading.Thread.Sleep(500);
                }
                string code = System.IO.File.ReadAllText(fileCode, System.Text.Encoding.UTF8);
                System.IO.File.Delete(fileCode);
                System.IO.File.Delete(fileExe);
                return code;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static FieldInfo[] GetSoapHeaders(MethodInfo method, bool isIn)
        {
            System.Type declaringType = method.DeclaringType;
            SoapHeaderAttribute[] customAttributes = (SoapHeaderAttribute[])method.GetCustomAttributes(typeof(SoapHeaderAttribute), true);
            ArrayList list = new ArrayList();
            for (int i = 0; i < customAttributes.Length; i++)
            {
                SoapHeaderAttribute attribute = customAttributes[i];
                if (((attribute.Direction == SoapHeaderDirection.InOut) || (isIn && (attribute.Direction == SoapHeaderDirection.In))) || (!isIn && (attribute.Direction == SoapHeaderDirection.Out)))
                {
                    FieldInfo field = declaringType.GetField(attribute.MemberName);
                    list.Add(field);
                }
            }
            return (FieldInfo[])list.ToArray(typeof(FieldInfo));
        }

        public static Dictionary<string, Type> GetEnumTypeList(Wsdl wsdl, Type type, Dictionary<string, Type> dEnumType)
        {
            Dictionary<string, Type> DicEnumType = null;
            if (dEnumType == null) DicEnumType = new Dictionary<string, Type>();
            else DicEnumType = dEnumType;
            if (type.IsArray)
            {
                string typeName = type.FullName.Replace("[]", "").Replace("&", "");
                System.Type arrayObjectType = System.Type.GetType(typeName);
                if (typeName == "System.Xml.XmlAttribute") return null;
                if (arrayObjectType == null)
                {
                    arrayObjectType = wsdl.ProxyAssemble.GetType(typeName, true, true);
                }
                GetEnumTypeList(wsdl, arrayObjectType, DicEnumType);
            }
            else
            {
                foreach (FieldInfo fi in type.GetFields())
                {
                    if (fi.FieldType.IsEnum)
                    {
                        DicEnumType[fi.Name] = fi.FieldType;
                    }
                    else if (System.Type.GetTypeCode(fi.FieldType) == TypeCode.Object)
                    {
                        GetEnumTypeList(wsdl, fi.FieldType, DicEnumType);
                    }
                }
            }
            return DicEnumType;
        }
    }
}