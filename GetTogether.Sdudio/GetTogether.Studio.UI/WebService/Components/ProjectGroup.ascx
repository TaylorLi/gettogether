<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProjectGroup.ascx.cs" Inherits="WebService_Components_ProjectGroup" %>
<%@ Register src="Projects.ascx" tagname="Projects" tagprefix="uc1" %>

<%if(Projects==null || Projects.Count==0){ %>
<div class="header box" style="padding-left:5px;margin-bottom:5px;">No project found</div>
<%} %>