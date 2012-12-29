<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DownloadFiles.ascx.cs"
    Inherits="Components_Downloads_DownloadFiles" %>
<table class="" style="width:100%;line-height:2em;" cellpadding="0px"
    cellspacing="0px">
    <tr><td colspan="5" class="header">Downloads</td></tr>
    <tr class="header-2">
        <td style="white-space: nowrap;">
            File Name</td>
        <td>
            Create On</td>
        <td>
            File Type</td>
        <td>
            Size</td>
        <td>
        </td>
    </tr>
                    <tr>
                <td colspan="5">
                    <div class="line-sub">
                    </div>
                </td>
            </tr>
    <asp:Repeater ID="rptResult" runat="server">
        <ItemTemplate>
                        <tr style="<%#Container.ItemIndex==0?"display:none;":""%>">
                <td colspan="5">
                    <div class="line-sub">
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                        <%#Eval("FileName") %>

                </td>
                <td>
                    <%#Eval("CreateOn") %>
                </td>
                <td>
                    <%#Eval("FileType") %>
                </td>
                <td>
                    <%#Eval("Length") %>
                    <%#Eval("LengthDesc") %>
                </td>
                <td style="text-align:right;padding-right:5px;">
                    <a href="../Include/Downloads/<%#Eval("FileName") %>">Download</a>
                </td>
            </tr>

        </ItemTemplate>
    </asp:Repeater>
</table>
