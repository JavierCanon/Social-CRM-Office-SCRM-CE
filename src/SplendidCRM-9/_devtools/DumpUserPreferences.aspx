<%@ Page language="c#" Codebehind="DumpUserPreferences.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM._devtools.DumpUserPreferences" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.Common" %>
<%@ Import Namespace="System.Diagnostics" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" > 

<html>
<head runat="server">
	<title>DumpUserPreferences</title>
</head>
<body MS_POSITIONING="GridLayout">
<%
// 01/11/2006 Paul.  Only a developer/administrator should see this. 
if ( SplendidCRM.Security.IS_ADMIN && Request.ServerVariables["SERVER_NAME"] == "localhost" )
{
	XmlDocument xml = new XmlDocument();
	try
	{
		xml.LoadXml(Sql.ToString(Session["USER_PREFERENCES"]));
		XmlUtil.Dump(xml);
		Response.Write("<pre>");
		string sPHP = XmlUtil.ConvertToPHP(xml.DocumentElement);
		foreach(char ch in sPHP.ToCharArray())
		{
			if ( ch == ';' )
			{
				Response.Write(";\r\n");
			}
			else if ( ch == '{' )
			{
				Response.Write("\r\n{\r\n");
			}
			else if ( ch == '}' )
			{
				Response.Write("}\r\n");
			}
			else
			{
				Response.Write(ch);
			}
		}
		Response.Write("</pre>");
	}
	catch
	{
	}
}
%>
</body>
</html>
