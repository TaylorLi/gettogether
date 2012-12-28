<%@ Control Language="C#" AutoEventWireup="true" CodeFile="InvokeHistory.ascx.cs"
    Inherits="WebService_Components_InvokeHistory" %>

<div class="header-2" style="padding-left:2px;">
    Invoke History
    <%if(Recent!=null){ %>
    <a href="javascript:;;" onclick="LoadRecent('<%=MethodName %>','<%=Recent.Name %>')">
                        <%=Recent.Name%>
                        </a><%=GetActionDateString(Recent.ActionTime)%>
                        <a href="javascript:;;" onclick="DeleteInvokeHistory('<%=MethodName %>','<%=Recent.Name%>');" style="color:Red;">X</a>
    <%} %>
    </div>
    <div class="line-sub"></div>
    <%if (Histories != null && Histories.Count > 0)
  { %>
<div class="header-2" style="padding-left:2px;">
            <asp:Repeater ID="rptResult" runat="server">
                <ItemTemplate>
                    <%#Container.ItemIndex>0?"&nbsp;&nbsp;<span class='mm-split'>|</span>&nbsp;&nbsp;": ""%>
                    <a href="javascript:;;" onclick="LoadRecent('<%=MethodName %>','<%#Eval("Name") %>');">
                        <%#Eval("Name") %>
                        </a><%#GetActionDateString((DateTime)Eval("ActionTime"))%>
                        <a href="javascript:;;" onclick="DeleteInvokeHistory('<%=MethodName %>','<%#System.Web.HttpUtility.UrlEncode((string) Eval("Name")) %>');" style="color:Red;">X</a>
                </ItemTemplate>
            </asp:Repeater>

</div>
<%} %>