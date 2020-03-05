<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.Administration.Shortcuts.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005-2008 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
</script>
<div id="divEditView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Shortcuts" EnablePrint="false" HelpName="EditView" EnableHelp="true" Runat="Server" />
	<p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="EditButtons" Src="~/_controls/EditButtons.ascx" %>
	<SplendidCRM:EditButtons ID="ctlEditButtons" Visible="<%# !PrintView %>" Runat="Server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table ID="tblMain" CssClass="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableCell width="15%" CssClass="dataLabel"><%= L10n.Term("Shortcuts.LBL_MODULE_NAME") %></asp:TableCell>
						<asp:TableCell width="35%" CssClass="dataField"><asp:DropDownList ID="MODULE_NAME" DataValueField="MODULE_NAME" DataTextField="MODULE_NAME" Runat="server" /></asp:TableCell>
						<asp:TableCell width="15%" CssClass="dataLabel"><%= L10n.Term("Shortcuts.LBL_DISPLAY_NAME") %></asp:TableCell>
						<asp:TableCell width="35%" CssClass="dataField"><asp:TextBox ID="DISPLAY_NAME" MaxLength="150" size="35" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Shortcuts.LBL_RELATIVE_PATH") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:TextBox ID="RELATIVE_PATH" MaxLength="255" size="35" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Shortcuts.LBL_IMAGE_NAME") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:TextBox ID="IMAGE_NAME" MaxLength="50" size="35" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Shortcuts.LBL_SHORTCUT_ORDER") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:TextBox ID="SHORTCUT_ORDER" MaxLength="10" size="25" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Shortcuts.LBL_SHORTCUT_ENABLED") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:CheckBox ID="SHORTCUT_ENABLED" CssClass="checkbox" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Shortcuts.LBL_SHORTCUT_MODULE") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:DropDownList ID="SHORTCUT_MODULE" DataValueField="MODULE_NAME" DataTextField="MODULE_NAME" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Shortcuts.LBL_SHORTCUT_ACLTYPE") %></asp:TableCell>
						<asp:TableCell CssClass="dataField">
							<asp:DropDownList ID="SHORTCUT_ACLTYPE" Runat="server">
								<asp:ListItem Value="edit"   Text="edit"   />
								<asp:ListItem Value="list"   Text="list"   />
								<asp:ListItem Value="import" Text="import" />
								<asp:ListItem Value="view"   Text="view"   />
							</asp:DropDownList>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
</div>
