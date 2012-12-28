using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using GetTogether.Studio.Database;
using System.IO;
using System.Text;
using GetTogether.Studio.Database.DAL;

public partial class Database_Callback_GenerateManage : GetTogether.Studio.Web.UI.PageCallback
{
    #region Attribures
    string ProjectName
    {
        get { return Request["pn"]; }
    }
    string TableName
    {
        get
        {
            return Request["tn"];
        }
    }
    string PrimaryKeyOverwrite
    {
        get
        {
            return Request["primary-key-overwrite"];
        }
    }
    ProjectParameter Parameter;
    CodeGenerator Generator;
    ColumnMapping.ColumnInfos ColumnInfos;
    string OutputFolder;
    string ReferFolder;
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Parameter = ProjectParameter.GetSettingsByProjectName(CurrentSession.UserCode, ProjectName);
            Generator = new CodeGenerator(Parameter);
            ColumnInfos = Generator.GetColumnsInfo(TableName, PrimaryKeyOverwrite);
            ReferFolder = Path.Combine(Server.MapPath(".."), "Manage\\Refer");
            OutputFolder = Path.Combine(Server.MapPath(".."), string.Concat("Manage\\", CurrentSession.UserCode, "\\", TableName));
            if (!System.IO.Directory.Exists(OutputFolder)) System.IO.Directory.CreateDirectory(OutputFolder);
            switch (type)
            {
                case 1:
                    Search();
                    Callback();
                    Result();
                    Edit();
                    GetTogether.Mapping.MappingInfoCache.Caches.Clear();
                    if (!string.IsNullOrEmpty(Request["download"]))
                        Compress(OutputFolder, string.Concat(OutputFolder, "-Management.zip"));
                    break;
                default:
                    break;
            }
            JsonSuccess();
        }
        catch (Exception ex)
        {
            JsonError(ex.Message);
        }
    }

    private void Search()
    {
        string referAspx = File.ReadAllText(Path.Combine(ReferFolder, "Default.aspx.txt"));
        string referCsValue = File.ReadAllText(Path.Combine(ReferFolder, "Default.aspx.cs.txt"));
        string createAspx = Path.Combine(OutputFolder, TableName + ".aspx");
        string createCs = string.Concat(createAspx, ".cs");
        StringBuilder searchParamHtml = new StringBuilder();
        searchParamHtml.AppendLine("<ul>");
        string textHtml = string.Format(@"<li>{1}:</li><li><input type=""text"" class=""txt"" id=""{0}_{1}"" name=""{0}_{1}"" /></li>", TableName, "{0}");
        foreach (ColumnMapping.ColumnInfo ci in ColumnInfos)
        {
            searchParamHtml.AppendFormat(textHtml, ci.ColumnName).AppendLine("");
        }
        searchParamHtml.AppendLine("</ul>");
        File.WriteAllText(createAspx, referAspx.Replace("{TableName}", TableName).Replace("{SearchParameter}", searchParamHtml.ToString()));
        File.WriteAllText(createCs, referCsValue.Replace("{TableName}", TableName));
    }

    private void Callback()
    {
        string referAspx = File.ReadAllText(Path.Combine(ReferFolder, "Default_Callback.aspx.txt"));
        string referCs = File.ReadAllText(Path.Combine(ReferFolder, "Default_Callback.aspx.cs.txt"));
        string referCsPrimaryKey = File.ReadAllText(Path.Combine(ReferFolder, "Default_Callback.aspx.cs.PrimaryKey.txt"));
        string dataObject = Generator.GenerateCodeByTable(CodeGenerator.CodeType.DAL, TableName, PrimaryKeyOverwrite, false);
        string businessObject = Generator.GenerateCodeByTable(CodeGenerator.CodeType.BLL, TableName, PrimaryKeyOverwrite, false);
        string createAspx = Path.Combine(OutputFolder, TableName + "_Callback.aspx");
        string createCs = string.Concat(createAspx, ".cs");
        string referCsFile = referCs;
        string autoIncrementField = "ID";
        string sortColumn = string.Empty;
        if (ColumnInfos.AutoIncrements.Count > 0)
            sortColumn = ColumnInfos.AutoIncrements[0];
        else if (ColumnInfos.PrimaryKeys.Count > 0)
            sortColumn = ColumnInfos.PrimaryKeys[0];
        else
            sortColumn = ColumnInfos[0].ColumnName;

        if (ColumnInfos.AutoIncrements.Count == 0)
            referCsFile = referCsPrimaryKey;
        else
            autoIncrementField = ColumnInfos.AutoIncrements[0];
        File.WriteAllText(createAspx, referAspx.Replace("{TableName}", TableName));
        File.WriteAllText(createCs, referCsFile.
            Replace("{TableName}", TableName).
            Replace("{DataObjectNameSpace}", Parameter.DataObjectNameSpace).
            Replace("{DataObject}", dataObject).
            Replace("{BusinessObject}", businessObject).
            Replace(Parameter.ConnectionKey, @"@""" + Parameter.ConnectionString + @"""").
            Replace("{AutoIncrement}", autoIncrementField).
            Replace("{SortColumn}", sortColumn).
            Replace(Parameter.DatabaseTypeVariables, Parameter.DatabaseTypeForCodeEngineer == GetTogether.Data.DatabaseType.SQLServer ? "DatabaseType.SQLServer" : "DatabaseType.MySQL"));

    }

    private void Result()
    {
        string referAscx = File.ReadAllText(Path.Combine(ReferFolder, "Default_Result.ascx.txt"));
        string referCs = File.ReadAllText(Path.Combine(ReferFolder, "Default_Result.ascx.cs.txt"));
        string createAscx = Path.Combine(OutputFolder, TableName + "_Result.ascx");
        string createCs = string.Concat(createAscx, ".cs");
        string rowKey = string.Empty;

        string checkboxForDeleteBinding = @"<%#Eval(""{0}"")%>";
        StringBuilder sbCheckboxForDeleteValue = new StringBuilder();
        StringBuilder sbCheckboxForDeleteId = new StringBuilder();
        if (ColumnInfos.AutoIncrements.Count > 0)
        {
            rowKey = ColumnInfos.AutoIncrements[0];
            sbCheckboxForDeleteValue.AppendFormat(checkboxForDeleteBinding, ColumnInfos.AutoIncrements[0]);
            sbCheckboxForDeleteId.AppendFormat(checkboxForDeleteBinding, ColumnInfos.AutoIncrements[0]);
        }
        else if (ColumnInfos.PrimaryKeys.Count > 0)
        {
            foreach (ColumnMapping.ColumnInfo ci in ColumnInfos)
            {
                if (ci.IsPrimaryKey)
                {
                    if (sbCheckboxForDeleteValue.Length > 0) sbCheckboxForDeleteValue.Append("|");
                    if (sbCheckboxForDeleteId.Length > 0) sbCheckboxForDeleteId.Append("_");
                    sbCheckboxForDeleteValue.AppendFormat(checkboxForDeleteBinding, ci.ColumnName);
                    sbCheckboxForDeleteId.AppendFormat(checkboxForDeleteBinding, ci.ColumnName);
                }
            }
        }
        StringBuilder sbHeader = new StringBuilder();
        StringBuilder sbContent = new StringBuilder();
        string tdHtml = "<td>{0}</td>";
        string selectAll = string.Format(@"
<input id=""selAll"" type=""checkbox"" onclick=""SelectAll(this,'{0}_','{0}-Result')"" style=""border-width: 0px;"" />
<img src=""<%=ResolveUrl(""~"") %>themes/skin-1/images/delete_icon.gif"" onclick=""{0}_Delete();"" />", TableName);
        string contentDeleteCheckbox = string.Format(@"<input type=""checkbox"" value=""{1}"" id=""{0}_{2}"" />",
            TableName, sbCheckboxForDeleteValue.ToString(), sbCheckboxForDeleteId.ToString());

        if (sbCheckboxForDeleteId.Length > 0)
        {
            sbHeader.AppendFormat("<td style='white-space:nowrap;'>{0}</td>", selectAll).AppendLine("");
            sbContent.AppendFormat(tdHtml, contentDeleteCheckbox).AppendLine("");
        }
        bool isFirst = true;
        foreach (ColumnMapping.ColumnInfo ci in ColumnInfos)
        {
            bool isHtmlEditor = false;
            if (ci.ColumnDetail != null && ci.ColumnDetail.MaxLength > 1000) isHtmlEditor = true;
            sbHeader.AppendFormat(tdHtml, string.Format(@"<%=GetSortHeader(""{0}"", ""{0}"")%>", ci.ColumnName));
            if (isFirst)
            {
                isFirst = false;
                sbContent.AppendFormat(tdHtml, string.Format(@"<a href=""javascript:;;"" onclick=""{2}_Edit('{1}');""><%#Eval(""{0}"")%></a>", ci.ColumnName, sbCheckboxForDeleteValue, TableName)).AppendLine("");
            }
            else
            {
                //<%#System.Web.HttpUtility.HtmlDecode((string)Eval(""{0}""))%>
                sbContent.AppendFormat(tdHtml, string.Format(isHtmlEditor ? @"<%#System.Web.HttpUtility.HtmlDecode((string)Eval(""{0}""))%>" : @"<%#Eval(""{0}"")%>", ci.ColumnName)).AppendLine("");
            }
        }
        File.WriteAllText(createAscx, referAscx.
            Replace("{TableName}", TableName).
            Replace("{Header}", sbHeader.ToString()).
            Replace("{Content}", sbContent.ToString()));
        File.WriteAllText(createCs, referCs.Replace("{TableName}", TableName));
    }

    private void Edit()
    {
        string referAspx = File.ReadAllText(Path.Combine(ReferFolder, "Default_Edit.aspx.txt"));
        string referCs = File.ReadAllText(Path.Combine(ReferFolder, "Default_Edit.aspx.cs.txt"));
        string referCsPrimaryKey = File.ReadAllText(Path.Combine(ReferFolder, "Default_Edit.aspx.cs.PrimaryKey.txt"));
        string dataObject = Generator.GenerateCodeByTable(CodeGenerator.CodeType.DAL, TableName, PrimaryKeyOverwrite, false);
        string businessObject = Generator.GenerateCodeByTable(CodeGenerator.CodeType.BLL, TableName, PrimaryKeyOverwrite, false);
        string createAspx = Path.Combine(OutputFolder, TableName + "_Edit.aspx");
        string createCs = string.Concat(createAspx, ".cs");
        string incrementType = "Int32";
        if (ColumnInfos.AutoIncrements.Count > 0)
        {
            string iType = GetTogether.Studio.Database.ColumnMapping.GetColumnType(ColumnInfos, ColumnInfos.AutoIncrements[0]);
            string[] iTypeInfo = iType.Split('.');
            incrementType = iTypeInfo[iTypeInfo.Length - 1];
        }
        StringBuilder htmlEdit = new StringBuilder();
        string htmlRow = @"<tr>
<td style=""width:5%;"">{0}</td>
<td>{1}</td>
</tr>";
        string htmlText = @"<input type=""text"" class=""txt""{1} id=""" + TableName + @"_{0}"" value=""<%=UO.{0}%>"" style=""width:100%;"" name=""" + TableName + @"_{0}"" />";
        string htmlLabel = @"<%=UO.{0}%>";
        string htmlTextarea = string.Concat(@"<textarea class=""textarea"" id=""", TableName, @"""_{0}""{1} row=""20"" style=""width:100%;height:80px;"" name=""", TableName, @"_{0}"" /><%=UO.{0}%></textarea>");
        string htmlNullableTip = @"<span style=""color: Red; padding-left: 3px;"">*</span>";
        //<%if(!string.IsNullOrEmpty(Request[""id""]){%><%}else{%>
        StringBuilder htmlEditShift = new StringBuilder();
        StringBuilder htmlDisplay = new StringBuilder();
        foreach (ColumnMapping.ColumnInfo ci in ColumnInfos)
        {
            string contentType = string.Empty;
            string tips = string.Empty;
            bool isNullable = true;
            if (ci.ColumnDetail != null) isNullable = ci.ColumnDetail.IsNullable > 0;
            if (ci.IsAutoIncrement) continue;
            if (ci.ColumnDetail != null && ci.ColumnDetail.IsNullable == 0)
                tips = htmlNullableTip;
            string editField = string.Empty;
            if (ci.ColumnDetail != null && ci.ColumnDetail.MaxLength > 200)
            {
                if (ci.ColumnDetail.DataType == "ntext" || ci.ColumnDetail.MaxLength > 1000) contentType = @" content-type=""1""";
                if (!isNullable) contentType += @" required=""1""";
                editField = string.Format(htmlTextarea, ci.ColumnName, contentType);
            }
            else
            {
                editField = string.Format(htmlText, ci.ColumnName, isNullable ? "" : @" required=""1""");
            }
            if (Parameter.UnInsert.IndexOf(ci.ColumnName) >= 0 ||
                Parameter.UnInsertAndUnUpdate.IndexOf(ci.ColumnName) >= 0 ||
                Parameter.UnUpdate.IndexOf(ci.ColumnName) >= 0)
            {
                htmlDisplay.Append(string.Concat(@"<%if(!string.IsNullOrEmpty(Request[""id""])){%>", string.Format(htmlRow, ci.ColumnName, string.Format(htmlLabel, ci.ColumnName)), "<%}%>"));
                htmlDisplay.AppendLine("");
                continue;
            }
            if (ci.IsPrimaryKey)
            {
                editField = string.Concat(@"<%if(!string.IsNullOrEmpty(Request[""id""])){%>", string.Format(htmlLabel, ci.ColumnName), "<%}else{%>", editField, "<%}%>");
                htmlEditShift.Append(string.Format(htmlRow, string.Concat(ci.ColumnName, tips), editField));
                htmlEditShift.AppendLine("");
                continue;
            }
            htmlEdit.Append(string.Format(htmlRow, string.Concat(ci.ColumnName, tips), editField));
            htmlEdit.AppendLine("");
        }
        //if (htmlDisplay.Length > 0) htmlEdit.Insert(0, htmlDisplay.ToString());
        if (htmlEditShift.Length > 0) htmlEdit.Insert(0, htmlEditShift.ToString());
        htmlEdit.Insert(0, @"<table style=""width:100%;"">");
        if (htmlDisplay.Length > 0) htmlEdit.Append(htmlDisplay);
        htmlEdit.AppendLine("</table>");

        string referCsFile = referCs;
        if (ColumnInfos.AutoIncrements.Count == 0) referCsFile = referCsPrimaryKey;
        File.WriteAllText(createAspx, referAspx.
            Replace("{TableName}", TableName).
            Replace("{EditContent}", htmlEdit.ToString()));
        File.WriteAllText(createCs, referCsFile.
            Replace("{TableName}", TableName).
            Replace("{DataObjectNameSpace}", Parameter.DataObjectNameSpace).
            Replace("{DataObject}", dataObject).
            Replace("{BusinessObject}", businessObject).
            Replace(Parameter.ConnectionKey, @"@""" + Parameter.ConnectionString + @"""").
            Replace("{AutoIncrementType}", incrementType).
            Replace(Parameter.DatabaseTypeVariables, Parameter.DatabaseTypeForCodeEngineer == GetTogether.Data.DatabaseType.SQLServer ? "DatabaseType.SQLServer" : "DatabaseType.MySQL"));
    }

    public static void Compress(string sourceFolder, string outFile)
    {
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.FileName = GetTogether.Utility.ConfigHelper.GetAppSetting("CompressFileName");
        p.StartInfo.Arguments = string.Concat(@"a -t7z -mx9 ", outFile, " -w ", sourceFolder);
        p.Start();
        string log = p.StandardOutput.ReadToEnd();
        p.WaitForExit();
        string logFile = string.Concat(outFile, ".txt");
        System.IO.File.WriteAllText(logFile, log);
    }
}