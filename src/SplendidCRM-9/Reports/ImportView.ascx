<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ImportView.ascx.cs" Inherits="SplendidCRM.Reports.ImportView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script type="text/javascript">
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
<div id="divListView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Reports" Title=".moduleList.Home" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="ImportButtons" Src="_controls/ImportButtons.ascx" %>
	<SplendidCRM:ImportButtons ID="ctlImportButtons" Visible="<%# !PrintView %>" Runat="Server" />

	<asp:Table Width="100%" CellPadding="0" CellSpacing="0" CssClass="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table ID="tblMain" Width="100%" CellSpacing="4" CellPadding="0" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Reports.LBL_MODULE_NAME") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="dataField">
							<asp:DropDownList ID="lstMODULE" TabIndex="1" DataValueField="MODULE_NAME" DataTextField="DISPLAY_NAME" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Reports.LBL_REPORT_NAME") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="dataField">
							<asp:TextBox ID="txtNAME" TabIndex="2" size="35" MaxLength="150" Runat="server" />
							&nbsp;
							<asp:RequiredFieldValidator ID="reqNAME" ControlToValidate="txtNAME" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Reports.LBL_REPORT_TYPE") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="dataField">
							<asp:DropDownList ID="lstREPORT_TYPE" TabIndex="3" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Reports.LBL_ASSIGNED_USER_ID") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="dataField">
							<asp:TextBox ID="txtASSIGNED_TO" ReadOnly="True" Runat="server" />
							<input ID="txtASSIGNED_USER_ID" type="hidden" runat="server" />
							<input ID="btnChangeUser" type="button" class="button" onclick="return UserPopup();" title="<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>" AccessKey="<%# L10n.AccessKey(".LBL_CHANGE_BUTTON_KEY") %>" value="<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Import.LBL_SELECT_FILE") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="dataField" ColumnSpan="3">
							<input id="fileIMPORT" type="file" size="60" MaxLength="255" runat="server" />
							&nbsp;
							<asp:RequiredFieldValidator ID="reqFILENAME" ControlToValidate="fileIMPORT" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
