<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Backups.ListView" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
</script>
<div id="divListView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Administration" Title="Administration.LBL_BACKUPS_TITLE" EnablePrint="false" HelpName="Backups" EnableHelp="true" Runat="Server" />
	
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	</asp:Panel>

	<asp:Table CellPadding="0" CellSpacing="0" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<%= L10n.Term("Administration.LBL_BACKUP_DATABASE_INSTRUCTIONS") %>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<br />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table CellPadding="0" CellSpacing="0" runat="server">
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Administration.LBL_BACKUP_FILENAME") %></asp:TableCell>
						<asp:TableCell CssClass="dataField">
							<asp:TextBox ID="NAME" size="70" MaxLength="255" Runat="server" />&nbsp;
							<asp:RequiredFieldValidator ID="NAME_REQUIRED" ControlToValidate="NAME" ErrorMessage='<%# L10n.Term("Administration.LBL_BACKUP_FILENAME_ERROR") %>' CssClass="required" EnableViewState="false" EnableClientScript="false" Enabled="false" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<br>
	<table width="100%" cellpadding="0" cellspacing="0" border="0">
		<tr>
			<td align="left" ><asp:Button ID="btnBack" CommandName="Back" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_BACK_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_BACK_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_BACK_BUTTON_KEY") %>' Enabled="False" Runat="server" /></td>
			<td align="right"><asp:Button ID="btnNext" CommandName="Next" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_NEXT_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_NEXT_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_NEXT_BUTTON_KEY") %>' Runat="server" /></td>
			</td>
		</tr>
	</table>
	</p>
	<table width="100%" cellpadding="0" cellspacing="0" border="0">
		<tr>
			<td><asp:Literal ID="lblImportErrors" Runat="server" /></td>
		</tr>
	</table>
</div>
