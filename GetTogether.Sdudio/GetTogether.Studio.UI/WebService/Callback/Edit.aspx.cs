using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GetTogether.Studio.WebService;
using System.Text;
using System.Web.Services.Description;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;

public partial class WebService_Callback_Edit : GetTogether.Studio.Web.UI.PageCallback
{
    #region Attributes

    public string ProjectName
    {
        get { return Request["pn"]; }
    }
    public GetTogether.Studio.WebService.ProjectParameter Parameter
    {
        get
        {
            return GetTogether.Studio.WebService.ProjectParameter.GetSettingsByProjectName(CurrentSession.UserCode, ProjectName, CurrentSession.ShareUserCode);
        }
    }
    public string MethodName
    {
        get
        {
            return Request["mn"];
        }
    }
    public string HistoryName
    {
        get
        {
            string name = Request["history"];
            if (string.IsNullOrEmpty(name) || name.Trim().ToLower() == "undefined")
            {
                return "Recent";
            }
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, ' ');
            }
            return name;
        }
    }
    public Wsdl CurrentWsdl
    {
        get
        {
            return WebServiceHelper.GetWsdl(Parameter, GetTogether.Utility.NumberHelper.ToInt(Request["refresh"], 0) == 1);
        }
    }
    string containerDiv = "<div id='dv-method-{0}'>{1}</div>";
    string refreshMethod = "&nbsp;&nbsp;<span class='mm-split'>|</span>&nbsp;&nbsp;<a href='javascript:;;' onclick='ClearMethod(\"{0}\")'>Refresh</a>&nbsp;&nbsp;<span class='mm-split'>|</span>&nbsp;&nbsp;<a target='_blank' href='{1}?op={0}'>Intro</a>";

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            switch (type)
            {
                case 0:
                    #region Get Methods
                    WebService_Components_Methods c = (WebService_Components_Methods)Page.LoadControl("~/WebService/Components/Methods.ascx");
                    try
                    {
                        c.Methods = CurrentWsdl.Methods;
                        c.Parameter = this.Parameter;
                    }
                    catch (Exception ex)
                    {
                        c.Error = ex.ToString();
                    }
                    this.Page.Controls.Add(c);
                    break;
                    #endregion
                case 1:
                    #region Get Method Information
                    GetMethodInformationMain();
                    break;
                    #endregion
                case 2:
                    #region Invoke Method
                    string retXml = string.Empty;
                    DateTime startDt = DateTime.Now;
                    try
                    {
                        System.Reflection.MethodInfo mi = CurrentWsdl.GetMethodByName(MethodName);
                        MethodSetting methodSetting = new MethodSetting();
                        methodSetting.RqtMode = MethodSetting.RequestMode.Object;
                        if (GetTogether.Utility.NumberHelper.ToInt(Request["WMP_SOAP_" + MethodName], 0) == 1)
                        {
                            methodSetting.RqtMode = MethodSetting.RequestMode.SOAP;
                        }
                        string historyPath = GetInvokeHistoryPath();
                        if (!System.IO.Directory.Exists(historyPath)) System.IO.Directory.CreateDirectory(historyPath);
                        System.IO.File.WriteAllText(GetInvokeHistoryParameterFile("[Request-Mode]"),
                            GetTogether.Utility.SerializationHelper.SerializeToXml(methodSetting));
                        if (methodSetting.RqtMode == MethodSetting.RequestMode.Object)
                        {
                            List<object> parameters = new List<object>();
                            StringBuilder sbParam = new StringBuilder();
                            List<object> referenceObjects = new List<object>();
                            foreach (System.Reflection.ParameterInfo pi in mi.GetParameters())
                            {
                                string rqtValue = Request.Form["WMP_" + pi.Name];
                                sbParam.AppendLine(rqtValue);
                                object p = WsdlHelper.ConvertParameter(CurrentWsdl, pi, rqtValue);
                                if (pi.ParameterType.IsByRef)
                                {
                                    referenceObjects.Add(p);
                                }
                                parameters.Add(p);
                                if (!System.IO.Directory.Exists(historyPath)) System.IO.Directory.CreateDirectory(historyPath);
                                System.IO.File.WriteAllText(GetInvokeHistoryParameterFile(pi.Name), WsdlHelper.ParameterToString(p));
                            }
                            foreach (FieldInfo fi in CurrentWsdl.GetHeaders(mi))
                            {
                                string rqtValue = Request.Form["WMP_" + fi.Name];
                                sbParam.AppendLine(rqtValue);
                                object p = WsdlHelper.ConvertParameter(CurrentWsdl, fi, rqtValue);
                                if (fi.FieldType.IsByRef)
                                {
                                    referenceObjects.Add(p);
                                }
                                GetTogether.Mapping.ObjectHelper.SetValue(CurrentWsdl.ServiceObject, fi.Name, p);
                                fi.SetValue(CurrentWsdl.ServiceObject, p);
                                //parameters.Add(p);
                                if (!System.IO.Directory.Exists(historyPath)) System.IO.Directory.CreateDirectory(historyPath);
                                System.IO.File.WriteAllText(GetInvokeHistoryParameterFile(fi.Name), WsdlHelper.ParameterToString(p));
                            }
                            object ret = CurrentWsdl.Invoke(MethodName, parameters.Count > 0 ? parameters.ToArray() : null);
                            if (ret == null)
                                retXml = "";
                            else
                            {
                                if (Parameter.AddressType == AddressType.WebService)
                                    retXml = GetTogether.Utility.SerializationHelper.SerializeToXml(ret);
                                else
                                    retXml = (string)ret;
                            }
                            if (referenceObjects.Count > 0)
                            {
                                retXml += "\r\n---------------------------";
                                retXml += "\r\nBy Reference Object(s)";
                                retXml += "\r\n---------------------------";

                                foreach (object obj in referenceObjects)
                                {
                                    retXml += "\r\n" + GetTogether.Utility.SerializationHelper.SerializeToXml(obj);
                                }
                            }
                        }
                        else
                        {
                            string requestXml = Request["WMP_SOAP_Request_" + MethodName];
                            retXml = RequestHelper.SendWebRequest(Parameter, mi, requestXml);
                            System.IO.File.WriteAllText(GetInvokeHistoryParameterFile("[SOAP-Request]"), requestXml);
                        }
                    }
                    catch (Exception ex)
                    {
                        retXml = ex.ToString();
                    }
                    string forwardUrl = @"<div style='display:none;' id='dv-forward-{3}'>
<textarea style='width:500px;height:80px;margin:5px;' class='txt'>http://{0}/{1}/WebService/Edit.aspx?pn={2}&method={3}&history={4}&share-usercode={5}</textarea></div>";
                    string forwardHtml = string.Format(forwardUrl, Request.Url.Host, Request.Url.AbsolutePath.Split('/')[1], System.Web.HttpUtility.UrlEncode(ProjectName), MethodName, System.Web.HttpUtility.UrlEncode(HistoryName), CurrentSession.UserCode);
                    string duration = DateTime.Now.Subtract(startDt).TotalSeconds.ToString();
                    StringBuilder sbOpenInNewWindow = new StringBuilder();
                    if (!string.IsNullOrEmpty(retXml))
                    {
                        sbOpenInNewWindow.Append("&nbsp;&nbsp;<span class='mm-split'>|</span>&nbsp;&nbsp;");
                        sbOpenInNewWindow.Append("Open in New Window : ");
                        string opTemp = "<a href='javascript:;;' onclick=\"$('#type-{0}').val('{1}');$('#form-{0}')[0].submit();\">{2}</a>";
                        sbOpenInNewWindow.AppendFormat(opTemp, MethodName, "text/xml", "XML");
                        sbOpenInNewWindow.Append(" , ");
                        sbOpenInNewWindow.AppendFormat(opTemp, MethodName, "text/plain", "TEXT");
                        sbOpenInNewWindow.Append(" , ");
                        sbOpenInNewWindow.AppendFormat(opTemp, MethodName, "text/html", "HTML");
                    }

                    string info = string.Format("<div class='header-2' style='padding-left:2px;'>Duration:{0}&nbsp;&nbsp;<span class='mm-split'>|</span>&nbsp;&nbsp;Content Length:{2} {1}</div><div class='line-sub'></div>", duration, sbOpenInNewWindow.ToString(), retXml.Length);
                    string html = string.Empty;
                    if (!string.IsNullOrEmpty(retXml))
                        html = string.Format("<form id='form-{1}' method='post' target='_blank' action='../Viewer.aspx'><input name='type-{1}' id='type-{1}' type='hidden' /><textarea rows='10' name='result-{1}' style='width:99.8%;'>{0}</textarea></form>", retXml, MethodName);
                    else
                        html = "<div class='header-2' style='padding-left:2px;'>Method was invoked successfully.</div><div class='line-sub'></div>";
                    Response.Write(info + html + forwardHtml);
                    break;
                    #endregion
                case 3:
                    #region Get Invoke History
                    try
                    {
                        Dictionary<string, object> parameters = new Dictionary<string, object>();
                        System.Reflection.MethodInfo mi = CurrentWsdl.GetMethodByName(MethodName);
                        FieldInfo[] headers = CurrentWsdl.GetHeaders(mi);
                        foreach (System.Reflection.FieldInfo fi in headers)
                        {
                            string hFile = GetInvokeHistoryParameterFile(fi.Name);
                            if (System.IO.File.Exists(hFile))
                            {
                                string hValue = System.IO.File.ReadAllText(hFile, System.Text.Encoding.UTF8);
                                parameters.Add(fi.Name, WsdlHelper.ConvertParameter(CurrentWsdl, fi, hValue));
                            }
                        }
                        foreach (System.Reflection.ParameterInfo pi in mi.GetParameters())
                        {
                            string hFile = GetInvokeHistoryParameterFile(pi.Name);
                            if (System.IO.File.Exists(hFile))
                            {
                                string hValue = System.IO.File.ReadAllText(hFile, System.Text.Encoding.UTF8);
                                parameters.Add(pi.Name, WsdlHelper.ConvertParameter(CurrentWsdl, pi, hValue));
                            }
                        }

                        if (mi.GetParameters().Length != parameters.Count - headers.Length)
                            GetMethodInformationMain();
                        else
                            Response.Write(GetMethodInformation(MethodName, parameters, mi.ReturnType, GetMethodSetting()));
                    }
                    catch (Exception ex)
                    {
                        string exError = string.Format("<textarea rows='5' style='width:99.8%;height:100px;'>{0}</textarea>", ex.ToString());
                        string header = string.Format("<div class='header-2' style='padding-left:2px;'>{0}{1}</div><div class='line-sub'></div>", MethodName, string.Format(refreshMethod, MethodName, Parameter.Address));
                        Response.Write(string.Format(containerDiv, MethodName, header + exError));
                    }
                    break;
                    #endregion
                case 4:
                    #region Delete Invoke History
                    try
                    {
                        string path = GetInvokeHistoryPath();
                        if (System.IO.Directory.Exists(path))
                        {
                            string pathDeleted = System.IO.Path.Combine(GetTogether.Studio.WebService.ProjectParameter.GetSettingsPath(CurrentSession.UserCode), "Deleted Invoke History\\" + MethodName + "\\" + HistoryName);
                            if (!System.IO.Directory.Exists(pathDeleted))
                                System.IO.Directory.CreateDirectory(pathDeleted);
                            GetTogether.Utility.DirectoryHelper.CopyParameter cp = new GetTogether.Utility.DirectoryHelper.CopyParameter();
                            cp.Destination = pathDeleted;
                            cp.Source = path;
                            cp.IsOverwrite = true;
                            GetTogether.Utility.DirectoryHelper.Copy(cp);
                            System.IO.Directory.Delete(path, true);
                        }
                        JsonSuccess();
                    }
                    catch (Exception ex)
                    {
                        JsonError(ex.ToString());
                    }
                    break;
                    #endregion
                case 5:
                    #region Format Xml
                    Response.Write(GetTogether.Utility.Xml.XmlHelper.FormatXml(Request["xml"]));
                    break;
                    #endregion
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            Response.Write(ex.ToString());
        }
    }

    private string GetMethodInformation(string methodName, Dictionary<string, object> parameters, Type returnType, MethodSetting methodSetting)
    {
        string buttons = string.Format(@"
<div class='header-2' style='padding:1px;padding-left:2px;'>
History Name:
<input type='text' maxlength='50' style='width:38%;' id='invoke-history-{0}' class='txt' value='{1}' />
<input type='button' id='invoke-{0}' class='btn' 
value='Invoke' onclick='InvokeWebMethod(""{0}"")' />
<input type='button' value='Share' class='btn' style='display:none;' onclick='ShareInvoke(""{0}"");' id='btn-forward-{0}' />
</div>", MethodName, HistoryName);
        try
        {
            System.Reflection.MethodInfo mi = CurrentWsdl.GetMethodByName(MethodName);
            StringBuilder sbParameter = new StringBuilder();
            StringBuilder sbMethod = new StringBuilder();
            StringBuilder sbHtml = new StringBuilder();
            #region Method Parameters
            FieldInfo[] headers = CurrentWsdl.GetHeaders(mi);
            foreach (string pName in parameters.Keys)
            {
                bool isHeader = false;
                foreach (FieldInfo fi in headers)
                {
                    if (pName == fi.Name)
                    {
                        isHeader = true; break;
                    }
                }
                object p = parameters[pName];
                Type pType = p.GetType();
                bool isSystemParam = WsdlHelper.IsSystemType(pType);
                string paramName = pType.Name;
                string paramColor = isSystemParam ? "blue" : "#2B91AF";
                string defaultValue = string.Empty;
                string styleHeight = (isSystemParam || pType.IsEnum) ? "height:18px;" : (parameters.Count > 2 ? "height:100px;" : "height:300px;");
                if (Parameter.AddressType == AddressType.Normal)
                {
                    if (parameters.Count == 1)
                        styleHeight = "height:300px;";
                    else if (pName == "requestContent")
                        styleHeight = "height:250px;";
                    else if (pName == "properties")
                        styleHeight = "height:100px;";
                }
                string styleWidth = (isSystemParam || pType.IsEnum) ? "width:99.8%;" : "width:99.8%;";
                defaultValue = WsdlHelper.ParameterToString(p);
                string enumInfo = WsdlHelper.GetEnumInfo(pType);
                sbParameter.AppendFormat("<tr><td style='width:5%;white-space:nowrap;vertical-align:top;'><span style='padding-left:2px;color:{1}'>{2}</span> {0} {3}", pName, paramColor, pType.Name, "");
                sbParameter.AppendFormat("</td><td><textarea class='txt-2' rows='5' id='WMP_{0}' style='{1}{3}'>{2}</textarea></td></tr>", pName, styleHeight, defaultValue, styleWidth);
                if (!string.IsNullOrEmpty(enumInfo))
                {
                    sbParameter.AppendFormat("<tr><td></td><td><div class='comment box' style='padding:5px;'>{0}</div></td></tr>", enumInfo);
                }
                if (isHeader)
                {
                    sbParameter.AppendFormat("<tr><td></td><td><div class='comment box' style='padding:5px;'>{0}</div></td></tr>", "Header");
                }
                if (!pType.IsEnum)
                {
                    Dictionary<string, Type> dicEnumType = GetTogether.Studio.WebService.WsdlHelper.GetEnumTypeList(CurrentWsdl, pType, null);
                    foreach (string k in dicEnumType.Keys)
                    {
                        sbParameter.AppendFormat("<tr><td></td><td><div class='comment box' style='padding:5px;'>{0} : {1}</div></td></tr>", k, WsdlHelper.GetEnumInfo(dicEnumType[k]));
                    }
                }
                if (Parameter.AddressType == AddressType.Normal && pName == "requestContent")
                {
                    sbParameter.AppendFormat(@"<tr><td></td><td><div class='comment box' style='padding:5px;'><a href='javascript:;;' onclick='FormatXml(""WMP_{0}"");' >Format XML</a></div></tr>", pName);
                }
                sbParameter.Append("<tr><td colspan='2'><div class='line-sub'></div></td></tr>");
                if (sbMethod.Length > 0) sbMethod.Append(",");
                sbMethod.AppendFormat("<span style='color:{1};'>{0}</span>", paramName, paramColor).Append(" ").Append(pName);
            }
            sbMethod.Insert(0, "(").Insert(0, MethodName);
            sbMethod.Append(")").AppendFormat(refreshMethod, methodName, Parameter.Address);
            if (Parameter.AddressType == AddressType.WebService)
            {
                sbMethod.AppendFormat(@"&nbsp;&nbsp;<span class='mm-split'>|</span>&nbsp;&nbsp;<input type='radio' id='WMP_Object_{0}' {1} value='{2}' name='request-mode-{0}' onclick='ChangeRequestMode(""{0}"",""object"");' /><label for='WMP_Object_{0}'>Parameter</label>", methodName, methodSetting.RqtMode == MethodSetting.RequestMode.Object ? "checked='checked'" : "", methodSetting.RqtMode == MethodSetting.RequestMode.Object ? "1" : "");

                sbMethod.AppendFormat(@"&nbsp;&nbsp;<span class='mm-split'>|</span>&nbsp;&nbsp;<input type='radio' id='WMP_SOAP_{0}' {1} value='{2}' name='request-mode-{0}' onclick='ChangeRequestMode(""{0}"",""soap"");' /><label for='WMP_SOAP_{0}'>SOAP</label>", methodName, methodSetting.RqtMode == MethodSetting.RequestMode.SOAP ? "checked='checked'" : "", methodSetting.RqtMode == MethodSetting.RequestMode.Object ? "0" : "1");
            }
            sbHtml.Append(string.Format("<div style='padding-left:2px;line-height:25px;'>{0}</div>", sbMethod.ToString()));
            string objParamStyle = methodSetting.RqtMode == MethodSetting.RequestMode.SOAP ? "display:none;" : "";
            string soapStyle = methodSetting.RqtMode == MethodSetting.RequestMode.Object ? "display:none;" : "";
            if (sbParameter.Length > 0)
            {
                sbParameter.Insert(0, string.Format("<div id='dv-parameter-{0}' style='{1}'><table style='width:100%;' cellpadding='0px' cellspacing='1px'>", methodName, objParamStyle));
                sbParameter.Append("</table></div>");
                sbHtml.Append("<div class='line-sub'></div>");
                sbHtml.Append(sbParameter.ToString());
            }
            else
            {
                sbHtml.Append("<div class='line-sub'></div>");
            }
            if (Parameter.AddressType == AddressType.WebService)
            {
                sbHtml.AppendFormat(@"<div id='dv-parameter-soap-{0}' style='{1}'><table style='width:100%;' cellpadding='0px' cellspacing='1px'><tr><td style='width:5%;white-space:nowrap;vertical-align:top;'>SOAP Request<div class='header-2'></div>", methodName, soapStyle);

                string soapRequestXml = string.Empty;
                string soapRequestXmlPath = GetInvokeHistoryParameterFile("[SOAP-Request]");
                if (System.IO.File.Exists(soapRequestXmlPath)) soapRequestXml = System.IO.File.ReadAllText(soapRequestXmlPath, System.Text.Encoding.UTF8);
                sbHtml.AppendFormat(@"</td><td><textarea class='txt-2' rows='5' id='WMP_SOAP_Request_{0}' 
style='height:300px;width:99.9%;'>{1}</textarea></td></tr><tr><td></td><td><div class='comment box' style='padding:5px;'><a href='javascript:;;' onclick='FormatXml(""WMP_SOAP_Request_{0}"");' >Format XML</a></div></td></tr></table></div>", methodName, soapRequestXml);
            }
            #endregion
            #region Return Type
            StringBuilder sbReturn = new StringBuilder();
            sbReturn.Append("<table style='width:100%;' cellpadding='0px' cellspacing='1px'>");

            object returnObject = null;
            if (mi.ReturnType.Name != "Void" && System.Type.GetTypeCode(returnType) == TypeCode.Object)
                returnObject = CurrentWsdl.GetObject(mi.ReturnType);
            string returnObjectXml = GetTogether.Utility.SerializationHelper.SerializeToXml(returnObject);
            string returnColor = !WsdlHelper.IsSystemType(returnType) ? "#2B91AF" : "blue";
            string returnTypeId = returnType.Name.Replace("[]", "").Replace("&", "");
            sbReturn.AppendFormat(@"<tr><td style='width:5%;white-space:nowrap;padding:5px 2px;'>
Return <span style='color:{0}'>{1}</span></td><td>
<form id='form-{2}-{4}' method='post' target='_blank' action='../Viewer.aspx'>
<input name='type-{2}-{4}' id='type-{2}-{4}' type='hidden' />
<textarea class='txt-2' rows='5' id='result-{2}-{4}' name='result-{2}-{4}' style='display:none;width:99.8%;height:18px;'>{3}</textarea></form>",
returnColor, returnType.Name, methodName, returnObjectXml, returnTypeId);
            if (returnObject != null)
            {
                string opTemp = "<a href='javascript:;;' onclick=\"$('#type-{0}-{1}').val('{2}');$('#form-{0}-{1}')[0].submit();\">{3}</a>";
                sbReturn.Append("View Object : ");
                sbReturn.AppendFormat(opTemp, MethodName, returnTypeId, "text/xml", "XML");
                sbReturn.Append(" , ");
                sbReturn.AppendFormat(opTemp, MethodName, returnTypeId, "text/plain", "TEXT");
            }
            sbReturn.Append("</td></tr>");
            if (!returnType.IsEnum)
            {
                Dictionary<string, Type> dicEnumType = GetTogether.Studio.WebService.WsdlHelper.GetEnumTypeList(CurrentWsdl, returnType, null);
                foreach (string k in dicEnumType.Keys)
                {
                    sbReturn.AppendFormat("<tr><td></td><td><div class='comment box' style='padding:5px;'>{0} : {1}</div></td></tr>", k, WsdlHelper.GetEnumInfo(dicEnumType[k]));
                }
            }
            else
            {
                sbReturn.AppendFormat("<tr><td colspan='2'><div class='comment box' style='padding:5px;'>{0}</div></td></tr>", WsdlHelper.GetEnumInfo(returnType));
            }
            sbReturn.Append("<tr><td colspan='2'><div class='line-sub'></div></td></tr>");
            sbReturn.Append("</table>");
            sbHtml.Append(sbReturn.ToString());


            #endregion
            sbHtml.Append(GetInvokeHistory());
            sbHtml.Append(buttons);
            return string.Format(containerDiv, MethodName, sbHtml.ToString());
        }
        catch (Exception ex)
        {
            string exError = string.Format("<textarea rows='5' style='width:99.8%;height:100px;'>{0}</textarea>", ex.ToString());
            string header = string.Format("<div class='header-2' style='padding-left:2px;'>{0}{1}</div><div class='line-sub'></div>", MethodName, string.Format(refreshMethod, methodName, Parameter.Address));
            return string.Format(containerDiv, MethodName, header + exError);
        }
    }

    private void GetMethodInformationMain()
    {
        try
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            System.Reflection.MethodInfo mi = CurrentWsdl.GetMethodByName(MethodName);
            foreach (FieldInfo fi in CurrentWsdl.GetHeaders(mi))
            {
                object objDefault = WsdlHelper.GetParameterDefaultValue(CurrentWsdl, fi.FieldType);
                parameters.Add(fi.Name, objDefault);
            }
            foreach (System.Reflection.ParameterInfo pi in mi.GetParameters())
            {
                object objDefault = WsdlHelper.GetParameterDefaultValue(CurrentWsdl, pi.ParameterType);
                parameters.Add(pi.Name, objDefault);
            }
            Response.Write(GetMethodInformation(MethodName, parameters, mi.ReturnType, GetMethodSetting()));
        }
        catch (Exception ex)
        {
            string exError = string.Format("<textarea rows='5' style='width:99.8%;height:100px;'>{0}</textarea>", ex.ToString());
            string header = string.Format("<div class='header-2' style='padding-left:2px;'>{0}{1}</div><div class='line-sub'></div>", MethodName, string.Format(refreshMethod, MethodName, Parameter.Address));
            Response.Write(string.Format(containerDiv, MethodName, header + exError));
        }
    }

    private string GetInvokeHistoryPath()
    {
        string path = GetTogether.Studio.WebService.ProjectParameter.GetSettingsPath(CurrentSession.UserCode);
        path = System.IO.Path.Combine(path, string.Concat(ProjectName, "(History)\\", MethodName));
        string htyName = HistoryName;
        if (string.IsNullOrEmpty(htyName)) htyName = "Recent";
        path = System.IO.Path.Combine(path, htyName);
        return path;
    }

    private string GetInvokeHistoryParameterFile(string pName)
    {
        return System.IO.Path.Combine(GetInvokeHistoryPath(), string.Concat(pName, ".txt"));
    }

    private string GetInvokeHistory()
    {
        string invokeHistory = string.Empty;
        WebService_Components_InvokeHistory c = (WebService_Components_InvokeHistory)Page.LoadControl("~/WebService/Components/InvokeHistory.ascx");
        c.Parameter = this.Parameter;
        c.MethodName = MethodName;
        invokeHistory = c.Html;
        return invokeHistory;
    }

    private MethodSetting GetMethodSetting()
    {
        string requestMethodFile = GetInvokeHistoryParameterFile("[Request-Mode]");
        MethodSetting methodSetting = null;
        if (System.IO.File.Exists(requestMethodFile))
        {
            methodSetting = GetTogether.Utility.SerializationHelper.FromXml<MethodSetting>(System.IO.File.ReadAllText(requestMethodFile, System.Text.Encoding.UTF8));
        }
        if (methodSetting == null)
        {
            methodSetting = new MethodSetting();
            methodSetting.RqtMode = MethodSetting.RequestMode.Object;
        }
        return methodSetting;
    }
}

public class MethodSetting
{
    public enum RequestMode
    {
        Object,
        SOAP,
    }
    public RequestMode RqtMode = RequestMode.Object;
}
