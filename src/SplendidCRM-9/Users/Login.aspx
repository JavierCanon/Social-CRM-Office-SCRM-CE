<%@ Page language="c#" MasterPageFile="~/DefaultView.Master" Codebehind="Login.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Users.Login" %>
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
<asp:Content ID="cntUnifiedSearch" ContentPlaceHolderID="cntUnifiedSearch" runat="server" />
<asp:Content ID="cntLastViewed" ContentPlaceHolderID="cntLastViewed" runat="server" />
<asp:Content ID="cntSidebar" ContentPlaceHolderID="cntSidebar" runat="server" />

<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
<script type="text/javascript">
window.onload = function()
{
	set_focus();
	try
	{
		// 01/08/2008 Paul.  showLeftCol does not exist on the mobile master page. 
		showLeftCol(false, false);
	}
	catch(e)
	{
	}
}
</script>
	<%@ Register TagPrefix="SplendidCRM" Tagname="LoginView" Src="LoginView.ascx" %>
	<SplendidCRM:LoginView ID="ctlLoginView" Runat="Server" />
</asp:Content>
