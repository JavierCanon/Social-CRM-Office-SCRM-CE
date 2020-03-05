<%@ Control CodeBehind="Users.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Roles.Users" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005-2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
%>
<script type="text/javascript">
function ChangeUser(sPARENT_ID, sPARENT_NAME)
{
	document.getElementById('<%= txtUSER_ID.ClientID   %>').value = sPARENT_ID  ;
	document.forms[0].submit();
}
function UserMultiSelect()
{
	return window.open('../../Users/PopupMultiSelect.aspx','UserPopup','width=600,height=400,resizable=1,scrollbars=1');
}
</script>
<div id="divRolesUsers">
	<input ID="txtUSER_ID" type="hidden" Runat="server" />
	<br />
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Users.LBL_MODULE_NAME" Runat="Server" />
	
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Button OnClientClick="UserMultiSelect(); return false;" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SELECT_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_SELECT_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SELECT_BUTTON_KEY") %>' Runat="server" />
		<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	</asp:Panel>

	<SplendidCRM:SplendidGrid id="grdMain" Width="100%" CssClass="listView"
		CellPadding="3" CellSpacing="0" border="0"
		AllowPaging="true" PageSize="20" AllowSorting="false" 
		AutoGenerateColumns="false" 
		EnableViewState="true" runat="server">
		<ItemStyle            CssClass="oddListRowS1"  />
		<AlternatingItemStyle CssClass="evenListRowS1" />
		<HeaderStyle          CssClass="listViewThS1"  />
		<PagerStyle HorizontalAlign="Right" Mode="NextPrev" PageButtonCount="6" Position="Top" CssClass="listViewPaginationTdS1" PrevPageText=".LNK_LIST_PREVIOUS" NextPageText=".LNK_LIST_NEXT" />
		<Columns>
			<asp:HyperLinkColumn HeaderText="Users.LBL_LIST_NAME"       DataTextField="FULL_NAME" ItemStyle-Width="25%" ItemStyle-CssClass="listViewTdLinkS1" DataNavigateUrlField="USER_ID" DataNavigateUrlFormatString="~/Users/view.aspx?id={0}" />
			<asp:BoundColumn     HeaderText="Users.LBL_LIST_USER_NAME"  DataField="USER_NAME"     ItemStyle-Width="25%" />
			<asp:BoundColumn     HeaderText="Users.LBL_LIST_TITLE"      DataField="TITLE"         ItemStyle-Width="25%" />
			<asp:BoundColumn     HeaderText="Users.LBL_LIST_DEPARTMENT" DataField="DEPARTMENT"    ItemStyle-Width="21%" ItemStyle-Wrap="false" />
			<asp:TemplateColumn  HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<asp:ImageButton CommandName="Users.Edit"   CommandArgument='<%# DataBinder.Eval(Container.DataItem, "USER_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" ImageUrl='<%# Session["themeURL"] + "images/edit_inline.gif" %>' AlternateText='<%# L10n.Term(".LNK_EDIT") %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
					<asp:LinkButton  CommandName="Users.Edit"   CommandArgument='<%# DataBinder.Eval(Container.DataItem, "USER_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Runat="server"><%# L10n.Term(".LNK_EDIT") %></asp:LinkButton>
					&nbsp;
					<asp:ImageButton CommandName="Users.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "USER_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_REMOVE") %>' ImageUrl='<%# Session["themeURL"] + "images/delete_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
					<asp:LinkButton  CommandName="Users.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "USER_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_REMOVE") %>' Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>
