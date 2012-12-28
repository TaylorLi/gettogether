<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Header.ascx.cs" Inherits="Components_AT_ATHeader" %>
<div id="header">
    <div id="menu">
        <div id="menu-left">
            <p>
                <strong>Work Studio</strong><span class="mm-split" style="margin: 0px 5px;">|</span>User:<%=CurrentSession.UserCode %>
                <%--<span style="margin-left:0px;">Version:<%=GetTogether.Studio.Config.Original.Version%></span>--%>
            </p>
        </div>
        <div id="menu-right">
            <ul id="nav-menu">
                <%=GetTogether.Studio.Web.HtmlHelper.GenMenus(this.Page)%>
            </ul>
        </div>
    </div>
</div>
