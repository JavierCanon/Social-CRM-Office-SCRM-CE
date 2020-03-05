<%@ Page language="c#" MasterPageFile="~/PopupView.Master" Codebehind="Password.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Users.Password" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005-2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
</script>
<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
<script type="text/javascript">
function ChangePassword()
{
	var txtIS_ADMIN         = document.getElementById('<%= txtIS_ADMIN.ClientID         %>');
	var txtOLD_PASSWORD     = document.getElementById('<%= txtOLD_PASSWORD.ClientID     %>');
	var txtNEW_PASSWORD     = document.getElementById('<%= txtNEW_PASSWORD.ClientID     %>');
	var txtCONFIRM_PASSWORD = document.getElementById('<%= txtCONFIRM_PASSWORD.ClientID %>');
	if ( txtIS_ADMIN.value == 0 && txtOLD_PASSWORD.value == '' )
	{
		alert('<%= L10n.TermJavaScript("Users.ERR_ENTER_OLD_PASSWORD") %>');
		return false;
	}
	if ( txtNEW_PASSWORD.value == '' )
	{
		alert('<%= L10n.TermJavaScript("Users.ERR_ENTER_NEW_PASSWORD") %>');
		return false;
	}
	if ( txtCONFIRM_PASSWORD.value == '' )
	{
		alert('<%= L10n.TermJavaScript("Users.ERR_ENTER_CONFIRMATION_PASSWORD") %>');
		return false;
	}

	if ( txtNEW_PASSWORD.value == txtCONFIRM_PASSWORD.value )
	{
		if ( window.opener != null && window.opener.ChangePassword != null )
		{
			window.opener.ChangePassword(txtOLD_PASSWORD.value, txtNEW_PASSWORD.value, txtCONFIRM_PASSWORD.value);
			window.close();
		}
		return true;
	}
	else
	{
		alert('<%= L10n.TermJavaScript("Users.ERR_REENTER_PASSWORDS") %>');
		return false;
	}
	return false;
}
function Cancel()
{
	window.close();
	return false;
}

// 08/30/2006 Paul.  Fix onload to support Firefox. 
window.onload = function()
{
	var txtIS_ADMIN         = document.getElementById('<%= txtIS_ADMIN.ClientID         %>');
	var txtOLD_PASSWORD     = document.getElementById('<%= txtOLD_PASSWORD.ClientID     %>');
	var txtNEW_PASSWORD     = document.getElementById('<%= txtNEW_PASSWORD.ClientID     %>');
	if ( txtIS_ADMIN.value == 0 )
		txtOLD_PASSWORD.focus();
	else
		txtNEW_PASSWORD.focus();
}
</script>
<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
<SplendidCRM:ListHeader Title="Users.LBL_CHANGE_PASSWORD" Runat="Server" />
<br />
<asp:HiddenField ID="txtIS_ADMIN" Value='<%# SplendidCRM.Security.IS_ADMIN ? "1" : "0" %>' runat="server" />
<asp:Table Width="100%" runat="server">
	<asp:TableRow style='<%# SplendidCRM.Security.IS_ADMIN ? "display:none" : "display:inline" %>'>
		<asp:TableCell Width="40%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Users.LBL_OLD_PASSWORD") %>' runat="server" /></asp:TableCell>
		<asp:TableCell Width="60%" CssClass="dataField"><asp:TextBox ID="txtOLD_PASSWORD"     TextMode="Password" TabIndex="1" size="15" MaxLength="15" CssClass="dataField" Runat="server" /></asp:TableCell>
	</asp:TableRow>
	<asp:TableRow>
		<asp:TableCell Width="40%" CssClass="dataLabel" Wrap="false"><asp:Label Text='<%# L10n.Term("Users.LBL_NEW_PASSWORD") %>' runat="server" /></asp:TableCell>
		<asp:TableCell Width="60%" CssClass="dataField"><asp:TextBox ID="txtNEW_PASSWORD"     TextMode="Password" TabIndex="1" size="15" MaxLength="15" CssClass="dataField" Runat="server" /></asp:TableCell>
	</asp:TableRow>
	<asp:TableRow>
		<asp:TableCell Width="40%" CssClass="dataLabel" Wrap="false"><asp:Label Text='<%# L10n.Term("Users.LBL_CONFIRM_PASSWORD") %>' runat="server" /></asp:TableCell>
		<asp:TableCell Width="60%" CssClass="dataField"><asp:TextBox ID="txtCONFIRM_PASSWORD" TextMode="Password" TabIndex="1" size="15" MaxLength="15" CssClass="dataField" Runat="server" /></asp:TableCell>
	</asp:TableRow>
	<asp:TableRow>
		<asp:TableCell Width="40%" CssClass="dataLabel"></asp:TableCell>
		<asp:TableCell Width="60%" CssClass="dataField"></asp:TableCell>
	</asp:TableRow>
</asp:Table>
<br />
<asp:Table Width="100%" runat="server">
	<asp:TableRow>
		<asp:TableCell HorizontalAlign="Center">
			<asp:Button OnClientClick="return ChangePassword();" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SAVE_BUTTON_LABEL"  ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_SAVE_BUTTON_TITLE"  ) %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_BUTTON_KEY"  ) %>' runat="server" />
			&nbsp;
			<asp:Button OnClientClick="return Cancel(); "        CssClass="button" Text='<%# "  " + L10n.Term(".LBL_CANCEL_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_CANCEL_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_CANCEL_BUTTON_KEY") %>' runat="server" />
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
</asp:Content>
