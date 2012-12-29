<%@ Page Title="" Language="C#" MasterPageFile="~/MP.master" AutoEventWireup="true"
    CodeFile="BackupChangedFiles.aspx.cs" Inherits="Tools_SVN_BackupChenagedFiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MP1" runat="Server">
    <div class="box-option">Backup SVN Files (Double click on the textarea to enlarge)</div>
    <div id="dv-container" class="box">
        <form method="post" action="BackupChangedFiles.aspx">
            <table style="width:100%;margin-top:2px;">
                <tr>
                    <td style="width:5%;white-space:nowrap;">Source Directory</td>
                    <td> <input type="text" style="width: 99.9%;" value="<%=string.IsNullOrEmpty(Request.Form["project-path"])?"D:\\Projects\\":Request.Form["project-path"] %>"
                    class="txt" name="project-path" /></td>
                </tr>
                <tr>
  <td colspan="2">
    <div class="line-sub"></div>
  </td>
</tr>
                <tr>
                    <td style="width:5%;white-space:nowrap;">Destination Directory</td>
                    <td> <input type="text" style="width: 99.9%;" class="txt" value="<%=Request.Form["backup-folder"] %>"
                    name="backup-folder" /></td>
                </tr>
                <tr>
  <td colspan="2">
    <div class="line-sub"></div>
  </td>
</tr>
                <tr>
                    <td style="width:5%;white-space:nowrap;vertical-align:top;"> Changed Files</td>
                    <td> <div class="box"><textarea name="changed-files" style="height: 150px; width: 99.9%;"><%=Request.Form["changed-files"] %></textarea></div></td>
                </tr>
                <tr>
  <td colspan="2">
    <div class="line-sub"></div>
  </td>
</tr>
            </table>


            <div style="padding: 2px; text-align: center;">
                <input type="submit" class="btn5" value="Submit" />
            </div>
        </form>
        <%if (Logs.Length > 0)
          { %>
        <div class="box" style="margin-top: 5px;">
            <div style="text-align: left; padding: 5px;">
                Logging (Double click on the textarea to enlarge)
            </div>
            <textarea style="width: 99.9%; height: 150px; padding-left: 5px;"><%=Logs.ToString()%></textarea>
        </div>
    </div>
    <%} %>
    <script language="javascript">
        $().ready(function () {
            EnhanceTextarea('dv-container');
        });
    </script>
</asp:Content>
