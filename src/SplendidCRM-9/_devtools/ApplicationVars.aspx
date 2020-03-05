<%@ Page language="c#" Codebehind="ApplicationVars.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM._devtools.ApplicationVars" %>
<script runat="server">
/**********************************************************************************************************************
 * The contents of this file are subject to the SugarCRM Public License Version 1.1.3 ("License"); You may not use this
 * file except in compliance with the License. You may obtain a copy of the License at http://www.sugarcrm.com/SPL
 * Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 * express or implied.  See the License for the specific language governing rights and limitations under the License.
 *
 * All copies of the Covered Code must include on each user interface screen:
 *    (i) the "Powered by SugarCRM" logo and
 *    (ii) the SugarCRM copyright notice
 *    (iii) the SplendidCRM copyright notice
 * in the same form as they appear in the distribution.  See full license for requirements.
 *
 * The Original Code is: SplendidCRM Open Source
 * The Initial Developer of the Original Code is SplendidCRM Software, Inc.
 * Portions created by SplendidCRM Software are Copyright (C) 2005 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
</script>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" > 

<html>
<head runat="server">
	<title>Application Variables</title>
</head>
<body>
<table border="1" cellpadding="0" cellspacing="2">
<%
// 01/11/2006 Paul.  Only a developer/administrator should see this. 
if ( SplendidCRM.Security.IS_ADMIN && Request.ServerVariables["SERVER_NAME"] == "localhost" )
{
	foreach(string key in Application.Keys)
	{
		Response.Write("<tr>");
		Response.Write("<td>");
		Response.Write(key);
		Response.Write("</td>");
		Response.Write("<td>");
		Response.Write(Application[key].ToString());
		Response.Write("</td>");
		Response.Write("</tr>");
	}
}
%>
</table>
<table border="1" cellpadding="0" cellspacing="2">
<%
if ( SplendidCRM.Security.IS_ADMIN )
{
	foreach(string key in Request.ServerVariables.Keys)
	{
		Response.Write("<tr>");
		Response.Write("<td>");
		Response.Write(key);
		Response.Write("</td>");
		Response.Write("<td>");
		Response.Write(Request.ServerVariables[key].ToString());
		Response.Write("</td>");
		Response.Write("</tr>");
	}
}
%>
</table>
</body>
</html>
