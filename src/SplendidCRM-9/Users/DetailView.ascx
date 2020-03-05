<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DetailView.ascx.cs" Inherits="SplendidCRM.Users.DetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005-2008 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
%>
<div id="divMain">
	<script type="text/javascript">
	function PasswordPopup()
	{
		return window.open('Password.aspx','ProjectPopup','width=320,height=230,resizable=1,scrollbars=1');
	}

	function ChangePassword(sOLD_PASSWORD, sNEW_PASSWORD, sCONFIRM_PASSWORD)
	{
		document.getElementById('<%= txtOLD_PASSWORD.ClientID     %>').value = sOLD_PASSWORD    ;
		document.getElementById('<%= txtNEW_PASSWORD.ClientID     %>').value = sNEW_PASSWORD    ;
		document.getElementById('<%= txtCONFIRM_PASSWORD.ClientID %>').value = sCONFIRM_PASSWORD;
		document.forms[0].submit();
	}
	</script>
	<input ID="txtOLD_PASSWORD"     type="hidden" Runat="server" />
	<input ID="txtNEW_PASSWORD"     type="hidden" Runat="server" />
	<input ID="txtCONFIRM_PASSWORD" type="hidden" Runat="server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Users" EnablePrint="true" HelpName="DetailView" EnableHelp="true" Runat="Server" />

	<asp:Table SkinID="tabDetailViewButtons" Visible="<%# !PrintView %>" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Button ID="btnEdit"           CommandName="Edit"           OnCommand="Page_Command"          CssClass="button" Text='<%# "  " + L10n.Term(".LBL_EDIT_BUTTON_LABEL"     ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_EDIT_BUTTON_TITLE"                ) %>' AccessKey='<%# L10n.AccessKey(".LBL_EDIT_BUTTON_KEY"                ) %>' Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
				<asp:Button ID="btnChangePassword" OnClientClick="PasswordPopup(); return false;"                 CssClass="button" Text='<%# L10n.Term("Users.LBL_CHANGE_PASSWORD_BUTTON_LABEL")    %>' ToolTip='<%# L10n.Term("Users.LBL_CHANGE_PASSWORD_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey("Users.LBL_CHANGE_PASSWORD_BUTTON_KEY") %>' Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
				<asp:Button ID="btnDuplicate"      CommandName="Duplicate"      OnCommand="Page_Command"          CssClass="button" Text='<%# " "  + L10n.Term(".LBL_DUPLICATE_BUTTON_LABEL") + " "  %>' ToolTip='<%# L10n.Term(".LBL_DUPLICATE_BUTTON_TITLE"           ) %>' AccessKey='<%# L10n.AccessKey(".LBL_DUPLICATE_BUTTON_KEY"           ) %>' Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
				<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
			</asp:TableCell>
			<asp:TableCell HorizontalAlign="Right">
				<asp:LinkButton ID="btnReset"      CommandName="Users.ResetDefaults" OnCommand="Page_Command" CssClass="listViewTdToolsS1" Runat="server"><%# L10n.Term("Users.LBL_RESET_PREFERENCES") %></asp:LinkButton>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<p>
	<asp:Table SkinID="tabDetailView" runat="server">
		<asp:TableRow>
			<asp:TableCell width="15%" VerticalAlign="top" CssClass="tabDetailViewDL"><%= L10n.Term("Users.LBL_NAME") %></asp:TableCell>
			<asp:TableCell width="35%" VerticalAlign="top" CssClass="tabDetailViewDF"><asp:Label ID="txtNAME" Runat="server" /></asp:TableCell>
			<asp:TableCell width="15%" VerticalAlign="top" CssClass="tabDetailViewDL"><%= L10n.Term("Users.LBL_USER_NAME") %></asp:TableCell>
			<asp:TableCell width="35%" VerticalAlign="top" CssClass="tabDetailViewDF"><asp:Label ID="txtUSER_NAME" Runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell VerticalAlign="top" CssClass="tabDetailViewDL"><%= L10n.Term("Users.LBL_STATUS") %></asp:TableCell>
			<asp:TableCell VerticalAlign="top" CssClass="tabDetailViewDF"><asp:Label ID="txtSTATUS" Runat="server" /></asp:TableCell>
			<asp:TableCell VerticalAlign="top" CssClass="tabDetailViewDL">&nbsp;</asp:TableCell>
			<asp:TableCell VerticalAlign="top" CssClass="tabDetailViewDF">&nbsp;</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
</div>

<div id="divUserSettings">
	<p>
	<asp:Table SkinID="tabDetailView" runat="server">
		<asp:TableRow>
			<asp:TableHeaderCell ColumnSpan="3" CssClass="dataLabel"><h4><asp:Label Text='<%# L10n.Term("Users.LBL_USER_SETTINGS") %>' runat="server" /></h4></asp:TableHeaderCell>
		</asp:TableRow>
		<asp:TableRow Visible="<%# SplendidCRM.Security.IS_ADMIN %>" runat="server">
			<asp:TableCell width="20%" CssClass="tabDetailViewDL"><%= L10n.Term("Users.LBL_ADMIN") %>&nbsp;</asp:TableCell>
			<asp:TableCell width="15%" CssClass="tabDetailViewDF"><asp:CheckBox ID="chkIS_ADMIN" Enabled="false" CssClass="checkbox" Runat="server" />&nbsp;</asp:TableCell>
			<asp:TableCell width="65%" CssClass="tabDetailViewDF"><%= L10n.Term("Users.LBL_ADMIN_TEXT") %>&nbsp;</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow Visible="<%# SplendidCRM.Security.IS_ADMIN %>" runat="server">
			<asp:TableCell CssClass="tabDetailViewDL"><%= L10n.Term("Users.LBL_PORTAL_ONLY") %>&nbsp;</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><asp:CheckBox ID="chkPORTAL_ONLY" Enabled="false" CssClass="checkbox" Runat="server" />&nbsp;</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><%= L10n.Term("Users.LBL_PORTAL_ONLY_TEXT") %>&nbsp;</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL"><%= L10n.Term("Users.LBL_RECEIVE_NOTIFICATIONS") %>&nbsp;</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><asp:CheckBox ID="chkRECEIVE_NOTIFICATIONS" Enabled="false" CssClass="checkbox" Runat="server" />&nbsp;</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><%= L10n.Term("Users.LBL_RECEIVE_NOTIFICATIONS_TEXT") %>&nbsp;</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL"><%= L10n.Term("Users.LBL_LANGUAGE") %>&nbsp;</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><asp:Label ID="txtLANGUAGE" Runat="server" /></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><%= L10n.Term("Users.LBL_LANGUAGE_TEXT") %>&nbsp;</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL"><%= L10n.Term("Users.LBL_DATE_FORMAT") %>&nbsp;</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><asp:Label ID="txtDATEFORMAT" Runat="server" /></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><%= L10n.Term("Users.LBL_DATE_FORMAT_TEXT") %>&nbsp;</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL"><%= L10n.Term("Users.LBL_TIME_FORMAT") %>&nbsp;</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><asp:Label ID="txtTIMEFORMAT" Runat="server" /></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><%= L10n.Term("Users.LBL_TIME_FORMAT_TEXT") %>&nbsp;</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL"><%= L10n.Term("Users.LBL_TIMEZONE") %>&nbsp;</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><asp:Label ID="txtTIMEZONE" Runat="server" /></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><%= L10n.Term("Users.LBL_TIMEZONE_TEXT") %>&nbsp;</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow Visible="false">
			<asp:TableCell CssClass="tabDetailViewDL"><%= L10n.Term("Users.LBL_GRIDLINE") %>&nbsp;</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF">
				<!-- 08/05/2006 Paul.  Remove stub of unsupported code. Show Gridline is not supported at this time. ->
				<asp:CheckBox ID="chkGRIDLINE" Enabled="false" CssClass="checkbox" Runat="server" />&nbsp;
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><%= L10n.Term("Users.LBL_GRIDLINE_TEXT") %>&nbsp;</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL"><%= L10n.Term("Users.LBL_CURRENCY") %>&nbsp;</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><asp:Label ID="txtCURRENCY" Runat="server" /></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><%= L10n.Term("Users.LBL_CURRENCY_TEXT") %>&nbsp;</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL"><%= L10n.Term("Users.LBL_NUMBER_GROUPING_SEP") %></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><asp:Label ID="txtGROUP_SEPARATOR" Runat="server" /></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><%= L10n.Term("Users.LBL_NUMBER_GROUPING_SEP_TEXT")%></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL"><%= L10n.Term("Users.LBL_DECIMAL_SEP")%></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><asp:Label ID="txtDECIMAL_SEPARATOR" Runat="server" /></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><%= L10n.Term("Users.LBL_DECIMAL_SEP_TEXT")%></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow Visible="false">
			<asp:TableCell CssClass="tabDetailViewDL"><%= L10n.Term("Users.LBL_REMINDER") %>&nbsp;</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF">
				<!-- 08/05/2006 Paul.  Remove stub of unsupported code. Reminder is not supported at this time. -->
				<asp:CheckBox ID="chkREMINDER" Enabled="false" CssClass="checkbox" Runat="server" />&nbsp;
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF"><%= L10n.Term("Users.LBL_REMINDER_TEXT") %>&nbsp;</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
</div>

<div id="divDetailView">
	<p>
	<table ID="tblMain" class="tabDetailView" runat="server">
		<tr>
			<th colspan="4" class="dataLabel"><h4><asp:Label Text='<%# L10n.Term("Users.LBL_USER_INFORMATION") %>' runat="server" /></h4></th>
		</tr>
	</table>
	</p>
</div>

<div id="divMailOptions">
	<p>
	<table ID="tblMailOptions" class="tabDetailView" runat="server">
		<tr>
			<th colspan="4" class="dataLabel"><h4><asp:Label Text='<%# L10n.Term("Users.LBL_MAIL_OPTIONS_TITLE") %>' runat="server" /></h4></th>
		</tr>
	</table>
	</p>
</div>

<!-- 08/05/2006 Paul.  Remove stub of unsupported code. Calendar Publish Key is not supported at this time. 
<div id="divCalendarOptions">
	<p>
	<asp:Table SkinID="tabDetailView" Visible="false" runat="server">
		<asp:TableRow>
			<asp:TableHeaderCell ColumnSpan="3" CssClass="dataLabel"><h4><asp:Label Text='<%# L10n.Term("Users.LBL_CALENDAR_OPTIONS") %>' runat="server" /></h4></asp:TableHeaderCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell width="20%" CssClass="tabDetailViewDL"><%= L10n.Term("Users.LBL_PUBLISH_KEY") %>&nbsp;</asp:TableCell>
			<asp:TableCell width="15%" CssClass="tabDetailViewDF"><asp:Label ID="txtCALENDAR_PUBLISH_KEY" Runat="server" /></asp:TableCell>
			<asp:TableCell width="65%" CssClass="tabDetailViewDF"><%= L10n.Term("Users.LBL_CHOOSE_A_KEY") %>&nbsp;</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL"><%= L10n.Term("Users.LBL_YOUR_PUBLISH_URL") %>&nbsp;</asp:TableCell>
			<asp:TableCell ColumnSpan="2" CssClass="tabDetailViewDF"><asp:Label ID="txtCALENDAR_PUBLISH_URL" Runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL"><%= L10n.Term("Users.LBL_SEARCH_URL") %>&nbsp;</asp:TableCell>
			<asp:TableCell ColumnSpan="2" CssClass="tabDetailViewDF"><asp:Label ID="txtCALENDAR_SEARCH_URL" Runat="server" /></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
</div>
-->

<div id="divAccessRights">
	<%@ Register TagPrefix="SplendidCRM" Tagname="AccessView" Src="~/Administration/ACLRoles/AccessView.ascx" %>
	<SplendidCRM:AccessView ID="ctlAccessView" EnableACLEditing="false" Runat="Server" />
</div>

<div id="divDetailSubPanel">
	<%@ Register TagPrefix="SplendidCRM" Tagname="Roles" Src="Roles.ascx" %>
	<SplendidCRM:Roles ID="ctlRoles" Runat="Server" />
	<br />
	<%@ Register TagPrefix="SplendidCRM" Tagname="Teams" Src="Teams.ascx" %>
	<SplendidCRM:Teams ID="ctlTeams" Runat="Server" />
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
