<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.Users.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Users" EnablePrint="false" HelpName="EditView" EnableHelp="true" Runat="Server" />
	<p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="EditButtons" Src="~/_controls/EditButtons.ascx" %>
	<SplendidCRM:EditButtons ID="ctlEditButtons" Visible="<%# !PrintView %>" Runat="Server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Users.LBL_FIRST_NAME") %></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:TextBox ID="txtFIRST_NAME" TabIndex="1" MaxLength="30" size="25" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Users.LBL_USER_NAME") %>&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox ID="txtUSER_NAME"  TabIndex="2" MaxLength="30" size="20" Runat="server" />
							<asp:RequiredFieldValidator ID="reqUSER_NAME" ControlToValidate="txtUSER_NAME" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Users.LBL_LAST_NAME") %>&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField">
							<asp:TextBox ID="txtLAST_NAME" TabIndex="1" MaxLength="30" size="25" Runat="server" />
							<asp:RequiredFieldValidator ID="reqLAST_NAME" ControlToValidate="txtLAST_NAME" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
						<asp:TableCell CssClass="dataLabel">&nbsp;</asp:TableCell>
						<asp:TableCell CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Users.LBL_STATUS") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:DropDownList ID="lstSTATUS" DataValueField="NAME" DataTextField="DISPLAY_NAME" TabIndex="1" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel">&nbsp;</asp:TableCell>
						<asp:TableCell CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	<p>
	<script type="text/javascript">
	function ChangeUser(sPARENT_ID, sPARENT_NAME)
	{
		document.getElementById('<%= new DynamicControl(this, "REPORTS_TO_ID"  ).ClientID %>').value = sPARENT_ID  ;
		document.getElementById('<%= new DynamicControl(this, "REPORTS_TO_NAME").ClientID %>').value = sPARENT_NAME;
	}
	function UserPopup()
	{
		return window.open('../Users/Popup.aspx','UserPopup','width=600,height=400,resizable=1,scrollbars=1');
	}
	</script>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell ColumnSpan="3"><h4><asp:Label Text='<%# L10n.Term("Users.LBL_USER_SETTINGS") %>' runat="server" /></h4></asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow visible="<%# SplendidCRM.Security.IS_ADMIN %>" runat="server">
						<asp:TableCell Width="20%" CssClass="dataLabel"><%= L10n.Term("Users.LBL_ADMIN") %></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataField"><asp:CheckBox ID="chkIS_ADMIN" TabIndex="3" CssClass="checkbox" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="65%" CssClass="dataField"><%= L10n.Term("Users.LBL_ADMIN_TEXT") %></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow visible="<%# SplendidCRM.Security.IS_ADMIN %>" runat="server">
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Users.LBL_PORTAL_ONLY") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:CheckBox ID="chkPORTAL_ONLY" TabIndex="3" CssClass="checkbox" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><%= L10n.Term("Users.LBL_PORTAL_ONLY_TEXT") %></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Users.LBL_RECEIVE_NOTIFICATIONS") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:CheckBox ID="chkRECEIVE_NOTIFICATIONS" TabIndex="3" CssClass="checkbox" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><%= L10n.Term("Users.LBL_RECEIVE_NOTIFICATIONS_TEXT") %></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Users.LBL_THEME") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:DropDownList ID="lstTHEME" DataValueField="NAME" DataTextField="NAME" TabIndex="3" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><%= L10n.Term("Users.LBL_THEME_TEXT") %></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Users.LBL_LANGUAGE") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:DropDownList ID="lstLANGUAGE" DataValueField="NAME" DataTextField="NATIVE_NAME" OnSelectedIndexChanged="lstLANGUAGE_Changed" AutoPostBack="true" TabIndex="3" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><%= L10n.Term("Users.LBL_LANGUAGE_TEXT") %></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Users.LBL_DATE_FORMAT") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:DropDownList ID="lstDATE_FORMAT" TabIndex="3" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><%= L10n.Term("Users.LBL_DATE_FORMAT_TEXT") %></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Users.LBL_TIME_FORMAT") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:DropDownList ID="lstTIME_FORMAT" TabIndex="3" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><%= L10n.Term("Users.LBL_TIME_FORMAT_TEXT") %></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Users.LBL_TIMEZONE") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:DropDownList ID="lstTIMEZONE" DataValueField="ID" DataTextField="NAME" TabIndex="3" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><%= L10n.Term("Users.LBL_TIMEZONE_TEXT") %></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Users.LBL_GRIDLINE") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:CheckBox ID="chkGRIDLINE" TabIndex="3" CssClass="checkbox" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><%= L10n.Term("Users.LBL_GRIDLINE_TEXT") %></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Users.LBL_CURRENCY") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:DropDownList ID="lstCURRENCY" DataValueField="ID" DataTextField="NAME_SYMBOL" TabIndex="3" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><%= L10n.Term("Users.LBL_CURRENCY_TEXT") %></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Users.LBL_NUMBER_GROUPING_SEP") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:TextBox ID="txtGROUP_SEPARATOR" TabIndex="3" MaxLength="1" size="1" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><%= L10n.Term("Users.LBL_NUMBER_GROUPING_SEP_TEXT")%></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Users.LBL_DECIMAL_SEP")%></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:TextBox ID="txtDECIMAL_SEPARATOR" TabIndex="3" MaxLength="1" size="1" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><%= L10n.Term("Users.LBL_DECIMAL_SEP_TEXT")%></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow visible="false">
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Users.LBL_REMINDER") %></asp:TableCell>
						<asp:TableCell CssClass="dataField">
							<!-- 08/05/2006 Paul.  Remove stub of unsupported code. Reminder is not supported at this time. -->
							<asp:CheckBox ID="chkSHOULD_REMIND" TabIndex="3" CssClass="checkbox" Runat="server" />
							<asp:DropDownList ID="lstREMINDER_TIME" DataValueField="NAME" DataTextField="DISPLAY_NAME" TabIndex="3" Runat="server" />
						</asp:TableCell>
						<asp:TableCell CssClass="dataField"><%= L10n.Term("Users.LBL_REMINDER_TEXT") %></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	<p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<table ID="tblMain" class="tabEditView" runat="server">
					<tr>
						<th colspan="4"><h4><asp:Label Text='<%# L10n.Term("Users.LBL_USER_SETTINGS") %>' runat="server" /></h4></th>
					</tr>
				</table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	<p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<table ID="tblAddress" class="tabEditView" runat="server">
					<tr>
						<th colspan="4"><h4><asp:Label Text='<%# L10n.Term("Users.LBL_ADDRESS_INFORMATION") %>' runat="server" /></h4></th>
					</tr>
				</table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	<p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<table ID="tblMailOptions" class="tabEditView" runat="server">
					<tr>
						<th colspan="4"><h4><asp:Label Text='<%# L10n.Term("Users.LBL_MAIL_OPTIONS_TITLE") %>' runat="server" /></h4></th>
					</tr>
				</table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	<!-- 08/05/2006 Paul.  Remove stub of unsupported code. Calendar Publish Key is not supported at this time.
	<p>
	<asp:Table BorderWidth="0" CellPadding="0" CellSpacing="0" CssClass="tabForm" Visible="false" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell ColumnSpan="4"><h4><asp:Label Text='<%# L10n.Term("Users.LBL_CALENDAR_OPTIONS") %>' runat="server" /></h4></asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Users.LBL_PUBLISH_KEY") %></asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataField"><asp:TextBox ID="txtCALENDAR_PUBLISH_KEY" TabIndex="10" MaxLength="25" size="25" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="65%" CssClass="dataField"><%= L10n.Term("Users.LBL_CHOOSE_A_KEY") %></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	-->
</div>
