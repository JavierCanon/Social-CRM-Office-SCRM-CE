<%@ Page language="c#" Codebehind="TimeZoneTests.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM._devtools.TimeZoneTests" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.Common" %>
<%@ Import Namespace="System.Diagnostics" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" > 

<html>
<head runat="server">
	<title>TimeZoneTests</title>
</head>
<body MS_POSITIONING="GridLayout">
<%
// 01/11/2006 Paul.  Only a developer/administrator should see this. 
if ( SplendidCRM.Security.IS_ADMIN && Request.ServerVariables["SERVER_NAME"] == "localhost" )
{
	SplendidCRM.TimeZone oEST = Application["TIMEZONE.BFA61AF7-26ED-4020-A0C1-39A15E4E9E0A"] as SplendidCRM.TimeZone;
	DateTime dt;
	if ( oEST != null )
	{
		%>
<h1>Timezone Conversions</h1>
<table border="1" cellpadding="3" cellspacing="0">
<tr><th>GMT</th><th>FromUniversalTime</th><th>correct</th></tr>
<tr><td>4/3/2005 05:05 AM GMT</td><td><% dt = oEST.FromUniversalTime(new DateTime(2005, 4, 3, 5, 5, 0)); Response.Write(dt.ToString() + " " + oEST.Abbreviation(dt)); %></td><td>4/3/2005 12:05 AM EST</td></tr>
<tr><td>4/3/2005 06:05 AM GMT</td><td><% dt = oEST.FromUniversalTime(new DateTime(2005, 4, 3, 6, 5, 0)); Response.Write(dt.ToString() + " " + oEST.Abbreviation(dt)); %></td><td>4/3/2005 01:05 AM EST</td></tr>
<tr><td>4/3/2005 07:05 AM GMT</td><td><% dt = oEST.FromUniversalTime(new DateTime(2005, 4, 3, 7, 5, 0)); Response.Write(dt.ToString() + " " + oEST.Abbreviation(dt)); %></td><td>4/3/2005 03:05 AM EDT</td></tr>
<tr><td>4/3/2005 08:05 AM GMT</td><td><% dt = oEST.FromUniversalTime(new DateTime(2005, 4, 3, 8, 5, 0)); Response.Write(dt.ToString() + " " + oEST.Abbreviation(dt)); %></td><td>4/3/2005 04:05 AM EDT</td></tr>
</table>
<br>
<table border="1" cellpadding="3" cellspacing="0">
<tr><th>GMT</th><th>FromUniversalTime</th><th>correct</th></tr>
<tr><td>10/30/2005 04:05 AM GMT</td><td><% dt = oEST.FromUniversalTime(new DateTime(2005, 10, 30, 4, 5, 0)); Response.Write(dt.ToString() + " " + oEST.Abbreviation(dt)); %></td><td>10/30/2005 12:05 AM EDT</td></tr>
<tr><td>10/30/2005 05:05 AM GMT</td><td><% dt = oEST.FromUniversalTime(new DateTime(2005, 10, 30, 5, 5, 0)); Response.Write(dt.ToString() + " " + oEST.Abbreviation(dt)); %></td><td>10/30/2005 01:05 AM EDT</td></tr>
<tr><td>10/30/2005 06:05 AM GMT</td><td><% dt = oEST.FromUniversalTime(new DateTime(2005, 10, 30, 6, 5, 0)); Response.Write(dt.ToString() + " " + oEST.Abbreviation(dt)); %></td><td>10/30/2005 01:05 AM EST (ambiguous calculation, we display EDT)</td></tr>
<tr><td>10/30/2005 07:05 AM GMT</td><td><% dt = oEST.FromUniversalTime(new DateTime(2005, 10, 30, 7, 5, 0)); Response.Write(dt.ToString() + " " + oEST.Abbreviation(dt)); %></td><td>10/30/2005 02:05 AM EST</td></tr>
</table>
<br>
<table border="1" cellpadding="3" cellspacing="0">
<tr><th>local</th><th>ToUniversalTime</th><th>correct</th></tr>
<tr><td>4/3/2005 12:05 AM EST</td><td><% dt = oEST.ToUniversalTime(new DateTime(2005, 4, 3, 0, 5, 0)); Response.Write(dt.ToString() + " GMT"); %></td><td>4/3/2005 05:05 AM GMT</td></tr>
<tr><td>4/3/2005 01:05 AM EST</td><td><% dt = oEST.ToUniversalTime(new DateTime(2005, 4, 3, 1, 5, 0)); Response.Write(dt.ToString() + " GMT"); %></td><td>4/3/2005 06:05 AM GMT</td></tr>
<tr><td>4/3/2005 03:05 AM EDT</td><td><% dt = oEST.ToUniversalTime(new DateTime(2005, 4, 3, 3, 5, 0)); Response.Write(dt.ToString() + " GMT"); %></td><td>4/3/2005 07:05 AM GMT</td></tr>
<tr><td>4/3/2005 04:05 AM EDT</td><td><% dt = oEST.ToUniversalTime(new DateTime(2005, 4, 3, 4, 5, 0)); Response.Write(dt.ToString() + " GMT"); %></td><td>4/3/2005 08:05 AM GMT</td></tr>
</table>
<br>
<table border="1" cellpadding="3" cellspacing="0">
<tr><th>local</th><th>ToUniversalTime</th><th>correct</th></tr>
<tr><td>10/30/2005 12:05 AM EDT</td><td><% dt = oEST.ToUniversalTime(new DateTime(2005, 10, 30, 0, 5, 0)); Response.Write(dt.ToString() + " GMT" ); %></td><td>10/30/2005 04:05 AM GMT</td></tr>
<tr><td>10/30/2005 01:05 AM EDT</td><td><% dt = oEST.ToUniversalTime(new DateTime(2005, 10, 30, 1, 5, 0)); Response.Write(dt.ToString() + " GMT" ); %></td><td>10/30/2005 05:05 AM GMT</td></tr>
<tr><td>10/30/2005 01:05 AM EST (cannot be represented)</td><td><% dt = oEST.ToUniversalTime(new DateTime(2005, 10, 30, 1, 5, 0)); Response.Write(dt.ToString() + " GMT" ); %></td><td>10/30/2005 06:05 AM GMT</td></tr>
<tr><td>10/30/2005 02:05 AM EST</td><td><% dt = oEST.ToUniversalTime(new DateTime(2005, 10, 30, 2, 5, 0)); Response.Write(dt.ToString() + " GMT" ); %></td><td>10/30/2005 07:05 AM GMT</td></tr>
</table>
		<%
	}
}
%>
</body>
</html>
