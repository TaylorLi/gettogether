<%@ Page Language="C#" MasterPageFile="~/MP.master" AutoEventWireup="true" CodeFile="Index.aspx.cs"
    Inherits="MSSQL_Index" %>

<%@ Register Src="~/Database/Components/Projects.ascx" TagName="Projects"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MP1" runat="Server">


        <div id="dv-projects" style="text-align: left; min-height: 50px;padding:">
        </div>
        <div style="text-align: right;">
            <input type="button" class="btn6" value="Create New Project" onclick="AjaxMsg('Create a new project','750','560',SerUrl,'Database/Callback/Index.aspx?type=1&g=1','content',null);" /></div>

    <script language="javascript">
 //function CommonCall(Url,type,divId,divBgId,divParId,inputPrefix,parAppend,dType,jsFun)
 //function AjaxMsg(title,width,height,serUrl,page,sId,func)
 $(document).ready(function(){
LoadProjects();
 });
 function LoadProjects(){ CommonCall('Database/Callback/Index.aspx',0,'dv-projects','dv-projects','dv-projects','TP_','','text',null);}
 function CreateProject(txtId)
 {
    ProjectExec(txtId,1);
 }
  function ProjectExec(txtId,type)
 {
    var errLabel=o('dv-error');
    errLabel.innerHTML='';
    if(IsEmpty(oa(txtId)))
    {
        errLabel.innerHTML='Please input the project.';
    }
    else
    {
        $.ajax({
        url:SerUrl+'Database/Callback/Index.aspx?type='+type+'&c='+oa(txtId),
        type:'post',
        cache:false,
        dataType:'json',
        data:'',
        error: function() {},
        success:function(data, textStatus) 
        {
            if(!data.success){errLabel.innerHTML=data.message;}
            else
            {
                CloseMsgBox();
                LoadProjects();
            }
        }   
        });
    }
 }
    </script>

</asp:Content>
