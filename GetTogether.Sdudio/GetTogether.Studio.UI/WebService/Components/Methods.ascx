<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Methods.ascx.cs" Inherits="WebService_Components_Methods" %>
<div class="box-option">
    Method(s)</div>
<div class="box">
    <%if (string.IsNullOrEmpty(Error))
      { %>
    <div id="dv-web-method-list" style="padding: 0px; text-align: left; overflow: scroll;
        margin: 1px;">
        <table width="100%" cellspacing="0" cellpadding="0" border="0" style="line-height: 2em;">
            <asp:Repeater ID="rptResult" runat="server">
                <ItemTemplate>
                    <tr valign="top">
                        <td><span class="comment">»</span>
                            <a href="javascript:;;" onclick="GetWebMethodInfo('<%#((System.Reflection.MethodInfo)Container.DataItem).Name %>');">
                                <%#((System.Reflection.MethodInfo)Container.DataItem).Name %>
                            </a>
                            <%--                                            <a style="float:left;" href="javascript:;;" onclick="LoadRecent('<%#((System.Reflection.MethodInfo)Container.DataItem).Name %>','Recent');">
                                               <%#((System.Reflection.MethodInfo)Container.DataItem).Name %>
                                            </a>--%></td><td style="text-align:right;">
                            <a style="color: #676767;" target="_blank" href="<%=Parameter.Address %>?op=<%#((System.Reflection.MethodInfo)Container.DataItem).Name %>">
                                [Intro]</a>
                        </td>

                    </tr>
                    <tr>
                        <td colspan="2">
                            <div class="line-sub">
                            </div>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </div>
    <%}
      else
      { %>
    <%=Error %>
    <%} %>
</div>
