<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.Administration.EditCustomFields.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divEditView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="EditCustomFields" Title="EditCustomFields.LBL_MODULE_TITLE" EnableModuleLabel="false" EnablePrint="true" HelpName="EditView" EnableHelp="true" Runat="Server" />
	<p>
	<asp:Table SkinID="tabEditViewButtons" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Button ID="btnSave"   CommandName="Save"   OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SAVE_BUTTON_LABEL"  ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_SAVE_BUTTON_TITLE"  ) %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_BUTTON_KEY"  ) %>' Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
				<asp:Button ID="btnCancel" CommandName="Cancel" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_CANCEL_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_CANCEL_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_CANCEL_BUTTON_KEY") %>' Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
				<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel"><%= L10n.Term("EditCustomFields.COLUMN_TITLE_NAME") + ":" %><asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField"><asp:TextBox ID="txtNAME" Enabled="false" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel">&nbsp;</asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("EditCustomFields.COLUMN_TITLE_LABEL") + ":" %><asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:TextBox ID="txtLABEL" Enabled="false" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel">&nbsp;</asp:TableCell>
						<asp:TableCell CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("EditCustomFields.COLUMN_TITLE_DATA_TYPE") + ":" %><asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField">
							<asp:DropDownList ID="lstDATA_TYPE" Enabled="false" Runat="server">
								<asp:ListItem Value="varchar">Text</asp:ListItem>
								<asp:ListItem Value="text"   >Text Area</asp:ListItem>
								<asp:ListItem Value="int"    >Integer</asp:ListItem>
								<asp:ListItem Value="float"  >Decimal</asp:ListItem>
								<asp:ListItem Value="bool"   >Checkbox</asp:ListItem>
								<asp:ListItem Value="date"   >Date</asp:ListItem>
								<asp:ListItem Value="enum"   >Dropdown</asp:ListItem>
								<asp:ListItem Value="guid"   >Guid</asp:ListItem>
							</asp:DropDownList>
						</asp:TableCell>
						<asp:TableCell CssClass="dataLabel">&nbsp;</asp:TableCell>
						<asp:TableCell CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow ID="trMAX_SIZE" Runat="server">
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("EditCustomFields.COLUMN_TITLE_MAX_SIZE") + ":" %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:TextBox ID="txtMAX_SIZE" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel">&nbsp;</asp:TableCell>
						<asp:TableCell CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("EditCustomFields.COLUMN_TITLE_REQUIRED_OPTION") + ":" %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:CheckBox ID="chkREQUIRED" CssClass="checkbox" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel">&nbsp;</asp:TableCell>
						<asp:TableCell CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("EditCustomFields.COLUMN_TITLE_DEFAULT_VALUE") + ":" %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:TextBox ID="txtDEFAULT_VALUE" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel">&nbsp;</asp:TableCell>
						<asp:TableCell CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow Visible="false">
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("EditCustomFields.COLUMN_TITLE_AUDIT") + ":" %></asp:TableCell>
						<asp:TableCell><asp:CheckBox ID="chkAUDITED" CssClass="checkbox" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow ID="trDROPDOWN_LIST" Visible="false" Runat="server">
						<asp:TableCell ColumnSpan="4" Wrap="false">
							<font CssClass="dataLabel"><%= L10n.Term("EditCustomFields.LBL_DROPDOWN_LIST") + ":" %></font>
							<asp:DropDownList ID="lstDROPDOWN_LIST" OnSelectedIndexChanged="lstDROPDOWN_LIST_Changed" DataTextField="LIST_NAME" DataValueField="LIST_NAME" AutoPostBack="true" Runat="server" />
							<asp:Table Width="100%" BorderWidth="1" CellPadding="0" CellSpacing="0" runat="server">
								<asp:TableRow>
									<asp:TableCell>
										<asp:DataGrid ID="grdPICK_LIST_VALUES" Width="100%" BorderWidth="0" CellPadding="3" CellSpacing="0" BackColor="white" 
											AutoGenerateColumns="false" AllowPaging="false" AllowSorting="false" EnableViewState="true" runat="server">
											<Columns>
												<asp:BoundColumn  DataField="NAME"         />
												<asp:BoundColumn  DataField="DISPLAY_NAME" />
											</Columns>
										</asp:DataGrid>
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
</div>
