<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.Administration.ProductCategories.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<script type="text/javascript">
function ChangeProductCategory(sPARENT_ID, sPARENT_NAME)
{
	var frm = document.forms[0];
	frm['<%= txtPARENT_ID.ClientID   %>'].value = sPARENT_ID  ;
	frm['<%= txtPARENT_NAME.ClientID %>'].value = sPARENT_NAME;
}
function ClearParent()
{
	var frm = document.forms[0];
	frm['<%= txtPARENT_ID.ClientID   %>'].value = '';
	frm['<%= txtPARENT_NAME.ClientID %>'].value = '';
}
function ProductCategoryPopup()
{
	return window.open('Popup.aspx','ProductCategoryPopup','width=600,height=400,resizable=1,scrollbars=1');
}
</script>
<div id="divEditView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader ID="ctlListHeader" Title="ProductCategories.LBL_NAME" Runat="Server" />
	<p>
	<asp:Table SkinID="tabEditViewButtons" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Button ID="btnSave"    CommandName="Save"    OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SAVE_BUTTON_LABEL"    ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_SAVE_BUTTON_TITLE"    ) %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_BUTTON_KEY"    ) %>' Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
				<asp:Button ID="btnSaveNew" CommandName="SaveNew" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SAVE_NEW_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_SAVE_NEW_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_NEW_BUTTON_KEY") %>' Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
				<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
			</asp:TableCell>
			<asp:TableCell HorizontalAlign="Right" Wrap="false">
				<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
				<asp:Label Text='<%# L10n.Term(".NTC_REQUIRED") %>' Runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top" Wrap="false"><%= L10n.Term("ProductCategories.LBL_NAME") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="85%" CssClass="dataField">
							<asp:TextBox ID="txtNAME" TabIndex="1" size="60" MaxLength="50" Runat="server" />
							<asp:RequiredFieldValidator ID="reqNAME" ControlToValidate="txtNAME" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("ProductCategories.LBL_PARENT") %></asp:TableCell>
						<asp:TableCell CssClass="dataField">
							<asp:TextBox ID="txtPARENT_NAME" ReadOnly="True" Runat="server" />
							<input ID="txtPARENT_ID" type="hidden" runat="server" NAME="txtPARENT_ID"/>
							<input ID="btnChangeParent" type="button" CssClass="button" onclick="return ProductCategoryPopup();" title="<%# L10n.Term(".LBL_SELECT_BUTTON_TITLE") %>" AccessKey="<%# L10n.AccessKey(".LBL_SELECT_BUTTON_KEY") %>" value="<%# L10n.Term(".LBL_SELECT_BUTTON_LABEL") %>" />
							<input ID="btnClearParent"  type="button" CssClass="button" onclick="return ClearParent();"          title="<%# L10n.Term(".LBL_CLEAR_BUTTON_TITLE" ) %>" AccessKey="<%# L10n.AccessKey(".LBL_CLEAR_BUTTON_KEY" ) %>" value="<%# L10n.Term(".LBL_CLEAR_BUTTON_LABEL" ) %>" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("ProductCategories.LBL_DESCRIPTION") %></asp:TableCell>
						<asp:TableCell CssClass="dataField">
							<asp:TextBox ID="txtDESCRIPTION" TabIndex="3"  TextMode="MultiLine" Rows="8" Columns="50" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("ProductCategories.LBL_ORDER") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField">
							<asp:TextBox ID="txtLIST_ORDER" TabIndex="4" size="5" MaxLength="10" Runat="server" />
							<asp:RequiredFieldValidator ID="reqLIST_ORDER" ControlToValidate="txtLIST_ORDER" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Display="Dynamic" Runat="server" />&nbsp;
							<em><%= L10n.Term("ProductCategories.LBL_ORDER_DESC") %></em>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
</div>
