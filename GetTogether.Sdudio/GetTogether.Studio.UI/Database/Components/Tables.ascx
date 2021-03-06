﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Tables.ascx.cs" Inherits="Components_MSSQL_Tables" %>
<div class="box-option">
    Tables(Click to generate)</div>
<div class="box">
    <%if (string.IsNullOrEmpty(Error))
  { %>
    <div id="dv-table-list" style="margin: 2px; padding:0px; text-align: left; overflow: scroll;">
                        <table width="100%" cellspacing="0" cellpadding="0" border="0" style="line-height: 2em;">
                            <asp:Repeater ID="rptResult" runat="server">
                                <ItemTemplate>
                                    <tr valign="top">
                                        <td><span class="comment" style="margin-right:2px;">»</span>
                                            <a href="javascript:;;" onclick="GenTableCode('<%#Eval("name") %>');">
                                                <%#Eval("name") %>
                                            </a>
                                        </td>
                                    </tr>
                                    <tr><td><div class="line-sub"></div></td></tr>
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
