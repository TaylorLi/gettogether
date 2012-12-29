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

public partial class Callback_MSSQL_Index : GetTogether.Studio.Web.UI.PageCallback
{
    public string ProjectName
    {
        get { return Request["pn"]; }
    }
    string PrimaryKeyOverwrite
    {
        get
        {
            return Request["primary-key-overwrite"];
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        switch (type)
        {
            case 0:
                GetTables();
                break;
            case 1:
                GenDataObjectByTable(Request["tn"]);
                break;
            case 2:
                GenBusinessObjectByTable(Request["tn"]);
                break;
            case 3:
                GetDataObjectByQuery(Request["tn"], Request["pk"], Request["sql"]);
                break;
            case 4:
                GetBusinessObjectByQuery(Request["tn"], Request["pk"], Request["sql"]);
                break;
            case 5:
                GetStoreProcedures();
                break;
            case 6:
                GetStoreProcedureParameters(Request["spn"]);
                break;
            case 7:
                GetDataObjectByStoreProce(Request["on"], Request["spn"], Request["sql"], Request["ret-obj-mapping"]);
                break;
            case 8:
                GetDataBusinessByStoreProce(Request["on"], Request["spn"], Request["sql"], Request["ret-obj-mapping"]);
                break;
            default:
                break;
        }
    }
    #region Functions

    #region From Table

    private void GenDataObjectByTable(string table)
    {
        string doString = string.Empty;
        string primaryKeys = string.Empty;
        string autoIncrement = string.Empty;
        try
        {
            if (!string.IsNullOrEmpty(ProjectName) && !string.IsNullOrEmpty(table))
            {
                ProjectParameter st = ProjectParameter.GetSettingsByProjectName(CurrentSession.UserCode, ProjectName);
                CodeGenerator cg = new GetTogether.Studio.Database.CodeGenerator(st);
                doString = cg.GenerateCodeByTable(GetTogether.Studio.Database.CodeGenerator.CodeType.DAL, table, PrimaryKeyOverwrite, true);
                ColumnMapping.ColumnInfos columnInfos = cg.GetColumnsInfo(table, "");
                primaryKeys = GetTogether.Utility.StringHelper.ArrayToString(columnInfos.PrimaryKeys.ToArray(), ",");
                autoIncrement = GetTogether.Utility.StringHelper.ArrayToString(columnInfos.AutoIncrements.ToArray(), ",");
                if (string.IsNullOrEmpty(primaryKeys)) primaryKeys = "-";
                if (string.IsNullOrEmpty(autoIncrement)) autoIncrement = "-";
            }
        }
        catch (Exception ex)
        {
            doString = ex.ToString();
        }
        Response.Write(string.Format(@"<div class='box-option' style='padding-left:2px;'>{0}</div>
<div class='header-2 box' style='padding:2px;'>
    <div>
        Auto Increment: <span style='font-style:italic;'>{3}</span>
    </div>
    <div class='line-sub'></div>
    <div>
        Primary Key(s): <span style='font-style:italic;'>{2}</span>
    </div>
    <div class='line-sub'></div>
    <div>
        Primary Key Overwrite:<input type=""text"" style='width:50%;' id=""primary-key-overwrite"" value=""{1}"" class=""txt"" />
    </div>
    <div class='line-sub' style='margin:3px 0px;'></div>
    <div style='text-align:right;'>
        <input class='btn6' type='button' onclick=""GenDoTableCode('{0}');"" value=""Re-Generate"" />
        <input class='btn6' type='button' style='margin-left:5px;' onclick=""GenerateManagePage('{0}');"" value=""Management"" />
        <input class='btn6' type='button' style='margin-left:5px;' onclick=""GenerateManagePage('{0}',true);"" value=""Download Management Pages"" />
    </div>
</div>", table, PrimaryKeyOverwrite, primaryKeys, autoIncrement));
        Response.Write(string.Concat(@"<div class='box-option' style='margin-top:5px;'>Data Object (Double click on the textarea to enlarge)
</div><div class='box'><textarea rows='10' style='width:99.8%;'>", doString, "</textarea></div>"));
    }

    private void GenBusinessObjectByTable(string table)
    {
        string boString = string.Empty;
        try
        {
            if (!string.IsNullOrEmpty(ProjectName) && !string.IsNullOrEmpty(table))
            {
                ProjectParameter st = ProjectParameter.GetSettingsByProjectName(CurrentSession.UserCode, ProjectName);
                boString = new GetTogether.Studio.Database.CodeGenerator(st).GenerateCodeByTable(GetTogether.Studio.Database.CodeGenerator.CodeType.BLL, table, PrimaryKeyOverwrite, true);
            }
        }
        catch (Exception ex)
        {
            boString = ex.ToString();
        }
        Response.Write(string.Concat(@"<div class='box-option'>Business Object (Double click on the textarea to enlarge)</div>
<div class='box'><textarea rows='10' style='width:99.8%;'>", boString, "</textarea></div>"));
    }

    private void GetTables()
    {
        if (!string.IsNullOrEmpty(ProjectName))
        {
            ProjectParameter st = ProjectParameter.GetSettingsByProjectName(CurrentSession.UserCode, ProjectName);
            Components_MSSQL_Tables ct = Page.LoadControl("~/Database/Components/Tables.ascx") as Components_MSSQL_Tables;
            ct.Parameter = st;
            this.Controls.Add(ct);
        }
    }

    #endregion

    #region From Query

    private void GetDataObjectByQuery(string tableName, string primaryKeys, string sql)
    {
        string doString = string.Empty;
        if (!string.IsNullOrEmpty(ProjectName) && !string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(primaryKeys))
        {
            try
            {
                ProjectParameter st = ProjectParameter.GetSettingsByProjectName(CurrentSession.UserCode, ProjectName);
                doString = new GetTogether.Studio.Database.CodeGenerator(st).GenerateCodeByScript(GetTogether.Studio.Database.CodeGenerator.CodeType.DAL, tableName, sql, primaryKeys, true);
            }
            catch (Exception ex)
            {
                doString = ex.ToString();
            }
        }
        else
        {
            doString = string.Format("Invalid Parameters[Project Name:{0},Table Name:{1},Primary Key(s):{2},SQL:{3}", ProjectName, tableName, primaryKeys, sql);
        }
        Response.Write(string.Concat(@"<div class='box-option'>Data Object (Double click on the textarea to enlarge)</div>
<div class='box'><textarea rows='10' style='width:99.8%;'>", doString, "</textarea></div>"));
    }

    private void GetBusinessObjectByQuery(string tableName, string primaryKeys, string sql)
    {
        string boString = string.Empty;
        if (!string.IsNullOrEmpty(ProjectName) && !string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(primaryKeys))
        {
            try
            {
                ProjectParameter pj = ProjectParameter.GetSettingsByProjectName(CurrentSession.UserCode, ProjectName);
                boString = new GetTogether.Studio.Database.CodeGenerator(pj).GenerateCodeByScript(GetTogether.Studio.Database.CodeGenerator.CodeType.BLL, tableName, sql, primaryKeys, true);
            }
            catch (Exception ex)
            {
                boString = ex.ToString();
            }
        }
        else
        {
            boString = string.Format("Invalid Parameters[Project Name:{0},Table Name:{1},Primary Key(s):{2},SQL:{3}", ProjectName, tableName, primaryKeys, sql);
        }
        Response.Write(string.Concat(@"<div class='box-option' style='margin-top:5px;'>Business Object (Double click on the textarea to enlarge)</div>
<div class='box'><textarea rows='10' style='width:99.8%;'>", boString, "</textarea></div>"));
    }

    #endregion

    #region From Store Procedure

    private void GetStoreProcedures()
    {
        if (!string.IsNullOrEmpty(ProjectName))
        {
            ProjectParameter pj = ProjectParameter.GetSettingsByProjectName(CurrentSession.UserCode, ProjectName);
            Components_MSSQL_StoreProcedures sp = Page.LoadControl("~/Database/Components/StoreProcedures.ascx") as Components_MSSQL_StoreProcedures;
            sp.Parameter = pj;
            this.Controls.Add(sp);
        }
    }

    private void GetStoreProcedureParameters(string spName)
    {
        if (!string.IsNullOrEmpty(ProjectName) && !string.IsNullOrEmpty(spName))
        {
            string simpleSql = new CodeGenerator(ProjectParameter.GetSettingsByProjectName(CurrentSession.UserCode, ProjectName)).GetStoreProcedureSimple(spName);
            Response.Write(string.Concat("<textarea rows='10' cols=\"10\" id='SP_Sql' style='width:99.5%;' class=\"txt\">", simpleSql, "</textarea>"));
        }
    }

    private void GetDataObjectByStoreProce(string objName, string spName, string sql, string returnObjMapping)
    {
        if (!string.IsNullOrEmpty(returnObjMapping) && returnObjMapping.Trim().StartsWith("//"))
            returnObjMapping = string.Empty;
        string doString = string.Empty;
        if (!string.IsNullOrEmpty(ProjectName) && !string.IsNullOrEmpty(objName) && !string.IsNullOrEmpty(spName) && !string.IsNullOrEmpty(sql))
        {
            try
            {
                ProjectParameter pj = ProjectParameter.GetSettingsByProjectName(CurrentSession.UserCode, ProjectName);
                doString = new GetTogether.Studio.Database.CodeGenerator(pj).GenerateCodeByStoreProcedure(CodeGenerator.CodeType.DAL, objName, spName, sql, returnObjMapping);
            }
            catch (Exception ex)
            {
                doString = ex.ToString();
            }
        }
        else
        {
            doString = string.Format("Invalid Parameters[Project Name:{0},Object Name:{1},Store Procedure:{2},SQL:{3}", ProjectName, objName, spName, sql);
        }
        Response.Write(string.Concat(@"<div class='box-option' style='margin-top:5px;'>Data Object (Double click on the textarea to enlarge)</div>
<div class='box'><textarea rows='10' style='width:99.8%;'>", doString, "</textarea></div>"));
    }
    private void GetDataBusinessByStoreProce(string objName, string spName, string sql, string returnObjMapping)
    {
        if (!string.IsNullOrEmpty(returnObjMapping) && returnObjMapping.Trim().StartsWith("//"))
            returnObjMapping = string.Empty;
        string doString = string.Empty;
        if (!string.IsNullOrEmpty(ProjectName) && !string.IsNullOrEmpty(objName) && !string.IsNullOrEmpty(spName) && !string.IsNullOrEmpty(sql))
        {
            try
            {
                ProjectParameter pj = ProjectParameter.GetSettingsByProjectName(CurrentSession.UserCode, ProjectName);
                doString = new GetTogether.Studio.Database.CodeGenerator(pj).GenerateCodeByStoreProcedure(CodeGenerator.CodeType.BLL, objName, spName, sql, returnObjMapping);
            }
            catch (Exception ex)
            {
                doString = ex.ToString();
            }
        }
        else
        {
            doString = string.Format("Invalid Parameters[Project Name:{0},Object Name:{1},Store Procedure:{2},SQL:{3}", ProjectName, objName, spName, sql);
        }
        Response.Write(string.Concat(@"<div class='box-option' style='margin-top:5px;'>Business Object (Double click on the textarea to enlarge)</div>
<div class='box'><textarea rows='10' style='width:99.8%;'>", doString, "</textarea></div>"));
    }

    #endregion

    #endregion
}