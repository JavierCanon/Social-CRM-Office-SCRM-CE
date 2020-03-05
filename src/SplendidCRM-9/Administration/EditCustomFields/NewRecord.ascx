<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NewRecord.ascx.cs" Inherits="SplendidCRM.Administration.EditCustomFields.NewRecord" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divNewRecord" Visible="false" runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderLeft" Src="~/_controls/HeaderLeft.ascx" %>
	<SplendidCRM:HeaderLeft ID="ctlHeaderLeft" Title="EditCustomFields.LBL_ADD_FIELD" Runat="Server" />

	<table border="0" cellpadding="3" cellspacing="0" width="100%" class="tabForm">
		<tr>
			<td class="dataLabel"><asp:Label Text='<%# L10n.Term("EditCustomFields.COLUMN_TITLE_NAME") + ":" %>' runat="server" /><asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></td>
			<td><asp:TextBox ID="txtNAME" Runat="server" /></td>
		</tr>
		<tr>
			<td class="dataLabel"><asp:Label Text='<%# L10n.Term("EditCustomFields.COLUMN_TITLE_LABEL") + ":" %>' runat="server" /><asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></td>
			<td><asp:TextBox ID="txtLABEL" Runat="server" /></td>
		</tr>
		<tr>
			<td class="dataLabel"><asp:Label Text='<%# L10n.Term("EditCustomFields.COLUMN_TITLE_DATA_TYPE") + ":" %>' runat="server" /><asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></td>
			<td>
				<asp:DropDownList ID="lstDATA_TYPE" OnSelectedIndexChanged="lstDATA_TYPE_Changed" AutoPostBack="true" Runat="server">
					<asp:ListItem Value="varchar">Text</asp:ListItem>
					<asp:ListItem Value="text"   >Text Area</asp:ListItem>
					<asp:ListItem Value="int"    >Integer</asp:ListItem>
					<asp:ListItem Value="float"  >Decimal</asp:ListItem>
					<asp:ListItem Value="bool"   >Checkbox</asp:ListItem>
					<asp:ListItem Value="date"   >Date</asp:ListItem>
					<asp:ListItem Value="enum"   >Dropdown</asp:ListItem>
					<asp:ListItem Value="guid"   >Guid</asp:ListItem>
				</asp:DropDownList>
			</td>
		</tr>
		<tr id="trMAX_SIZE" Runat="server">
			<td class="dataLabel"><asp:Label Text='<%# L10n.Term("EditCustomFields.COLUMN_TITLE_MAX_SIZE") + ":" %>' runat="server" /></td>
			<td><asp:TextBox ID="txtMAX_SIZE" Runat="server" /></td>
		</tr>
		<tr>
			<td class="dataLabel"><asp:Label Text='<%# L10n.Term("EditCustomFields.COLUMN_TITLE_REQUIRED_OPTION") + ":" %>' runat="server" /></td>
			<td><asp:CheckBox ID="chkREQUIRED" CssClass="checkbox" Runat="server" /></td>
		</tr>
		<tr>
			<td class="dataLabel"><asp:Label Text='<%# L10n.Term("EditCustomFields.COLUMN_TITLE_DEFAULT_VALUE") + ":" %>' runat="server" /></td>
			<td><asp:TextBox ID="txtDEFAULT_VALUE" Runat="server" /></td>
		</tr>
		<!--
		<tr>
			<td class="dataLabel"><asp:Label Text='<%# L10n.Term("EditCustomFields.COLUMN_TITLE_AUDIT") + ":" %>' runat="server" /></td>
			<td><asp:CheckBox ID="chkAUDITED" CssClass="checkbox" Runat="server" /></td>
		</tr>
		-->
		<tr id="trDROPDOWN_LIST" Visible="false" Runat="server">
			<td colspan="2" nowrap>
				<font class="dataLabel"><asp:Label Text='<%# L10n.Term("EditCustomFields.LBL_DROPDOWN_LIST") + ":" %>' runat="server" /></font>
				<asp:DropDownList ID="lstDROPDOWN_LIST" OnSelectedIndexChanged="lstDROPDOWN_LIST_Changed" DataTextField="LIST_NAME" DataValueField="LIST_NAME" AutoPostBack="true" Runat="server" />
				<table border="1" cellpadding="0" cellspacing="0" width="100%">
					<tr>
						<td>
							<asp:DataGrid id="grdPICK_LIST_VALUES" Width="100%" CellPadding="3" CellSpacing="0" border="0" BackColor="white" 
								AutoGenerateColumns="false" AllowPaging="false" AllowSorting="false" EnableViewState="true" runat="server">
								<Columns>
									<asp:BoundColumn  DataField="NAME"         />
									<asp:BoundColumn  DataField="DISPLAY_NAME" />
								</Columns>
							</asp:DataGrid>
						</td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td colspan="2" align="left">
				<asp:Button ID="btnSave" CommandName="NewRecord" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SAVE_BUTTON_LABEL"  ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_SAVE_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_BUTTON_KEY") %>' Runat="server" /><br />
				<asp:RegularExpressionValidator ID="regNAME" ControlToValidate="txtNAME" ErrorMessage="(invalid field name)" CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" 
					ValidationExpression="^[A-Za-z_]\w*" />
				<asp:RequiredFieldValidator ID="reqNAME"  ControlToValidate="txtNAME"  ErrorMessage="(required)" CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
				<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
			</td>
		</tr>
	</table>
</div>
