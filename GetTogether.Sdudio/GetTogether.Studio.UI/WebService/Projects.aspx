<%@ Page Title="" Language="C#" MasterPageFile="~/MP.master" AutoEventWireup="true"
    CodeFile="Projects.aspx.cs" Inherits="WebService_Projects" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MP1" runat="Server">
    <div id="dv-content" style="width: 100%; min-height: 500px;">
        <div style="margin-bottom: 5px;">
            Web Service Projects
            <%if (GetTogether.Utility.NumberHelper.ToInt(Request["show"], 0) == 0)
              { %>
            &nbsp;&nbsp;<span class="mm-split">|</span>&nbsp;&nbsp;<a
                style="" href="Projects.aspx?show=1">Show All</a>
            <%}
              else
              { %>
            &nbsp;&nbsp;<span class="mm-split">|</span>&nbsp;&nbsp;<a
                style="" href="Projects.aspx?show=0">By Group</a><%} %>
        </div>
        <div id="dv-projects" style="text-align: left; min-height: 1px;">
        </div>
        <div style="text-align: right; margin-bottom: 5px;">
            <input type="button" class="btn6" value="Create a new project"
                onclick="AjaxMsg('Create a new project', $(window).width() / 3 * 2, '470', SerUrl, 'WebService/Callback/Projects.aspx?type=1&get=1', 'dv-content', null);" />
        </div>
    </div>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            LoadProjects();
        });
        function LoadProjects() { CommonCall('WebService/Callback/Projects.aspx?show=' + getUrlParam('show'), 0, 'dv-projects', 'dv-projects', 'dv-projects', 'TP_', '', 'text', null); }
        function ProjectExec(txtId, type) {
            var errLabel = o('dv-error');
            errLabel.innerHTML = '';
            if (IsEmpty(oa(txtId))) {
                errLabel.innerHTML = 'Please input the project.';
            }
            else {
                $.ajax({
                    url: SerUrl + 'WebService/Callback/Projects.aspx?type=' + type + '&content=' + oa(txtId),
                    type: 'post',
                    cache: false,
                    dataType: 'text',
                    data: '',
                    error: function () { },
                    success: function (data, textStatus) {
                        if (data != "OK") { errLabel.innerHTML = data; }
                        else {
                            CloseMsgBox();
                            LoadProjects();
                        }
                    }
                });
            }
        }
        function DeleteProject(projName) {
            Confirm('Are you sure you want to delete "' + projName + '"?', 'Delete Project', false, 'dv-content', 'DeleteProjectExec(\'' + projName + '\');', '', '', '');
        }
        function DeleteProjectExec(projName) {
            CommonCall('WebService/Callback/Projects.aspx?pn=' + projName, 3, 'dv-projects', 'dv-projects', 'dv-projects', 'TP_', '', 'json', function (data) {
                if (data.success) {
                    CM();
                    LoadProjects();
                }
                else {
                    MsgBox(data.message);
                }
            });
        }
    </script>
</asp:Content>
