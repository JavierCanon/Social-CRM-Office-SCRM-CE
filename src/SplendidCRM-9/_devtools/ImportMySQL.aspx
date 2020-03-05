<%@ Page language="c#" Codebehind="ImportMySQL.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM._devtools.ImportMySQL" %>
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
	<title>ImportMySQL</title>
</head>
<body MS_POSITIONING="GridLayout">
<%
// 01/11/2006 Paul.  Only a developer/administrator should see this. 
// 12/22/2007 Paul.  Allow an admin to import data. 
if ( SplendidCRM.Security.IS_ADMIN )
{
	%>
	<form id="frm" method="post" runat="server">
		<input id="fileUNC" type="file" MaxLength="511" runat="server" />
		<asp:Button ID="btnUpload" Text="Go" CommandName="Upload" OnCommand="Page_ItemCommand" CssClass="btn" Runat="server" />
	</form>
	<%
}
%>
</body>
</html>
