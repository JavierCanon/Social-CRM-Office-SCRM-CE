<%@ Control CodeBehind="UserRolesView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.ACLRoles.UserRolesView" %>
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
function ChangeRole(sPARENT_ID, sPARENT_NAME)
{
	document.getElementById('<%= txtROLE_ID.ClientID   %>').value = sPARENT_ID  ;
	document.forms[0].submit();
}
function RoleMultiSelect()
{
	return window.open('PopupMultiSelect.aspx','RolePopup','width=600,height=400,resizable=1,scrollbars=1');
}
</script>
<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
<SplendidCRM:ListHeader Visible="<%# !PrintView %>" Title="ACLRoles.LBL_SEARCH_FORM_TITLE" Runat="Server" />
<div id="divSearch">
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabSearchView" runat="server">
					<asp:TableRow>
						<asp:TableCell width="20%" CssClass="dataLabel"><%= L10n.Term(".LBL_ASSIGNED_TO") %></asp:TableCell>
						<asp:TableCell width="70%" CssClass="dataField"><asp:DropDownList ID="lstUSERS" DataValueField="ID" DataTextField="USER_NAME" TabIndex="1" AutoPostBack="True" Runat="server" /></asp:TableCell>
						<asp:TableCell width="10%" CssClass="dataLabel" HorizontalAlign="Right" Wrap="false">
							<asp:Button ID="btnSearch" CommandName="Search" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_SEARCH_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_SEARCH_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SEARCH_BUTTON_KEY") %>' Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
<br />

<div id="divAccessRights">
	<%@ Register TagPrefix="SplendidCRM" Tagname="AccessView" Src="AccessView.ascx" %>
	<SplendidCRM:AccessView ID="ctlAccessView" EnableACLEditing="false" Runat="Server" />
</div>

<div id="divUsersRoles">
	<input ID="txtROLE_ID" type="hidden" Runat="server" />
	<br />
	
	<asp:Table SkinID="tabDetailViewButtons" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Button ID="btnSelectRole" OnClientClick="RoleMultiSelect(); return false;" CssClass="button" Text='<%# L10n.Term(".LBL_SELECT_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_SELECT_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SELECT_BUTTON_KEY") %>' Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
				<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<SplendidCRM:SplendidGrid id="grdMain" Width="100%" CssClass="listView"
		CellPadding="3" CellSpacing="0" border="0"
		AllowPaging="false" PageSize="20" AllowSorting="false" 
		AutoGenerateColumns="false" 
		EnableViewState="true" runat="server">
		<ItemStyle            CssClass="oddListRowS1"  />
		<AlternatingItemStyle CssClass="evenListRowS1" />
		<HeaderStyle          CssClass="listViewThS1"  />
		<PagerStyle HorizontalAlign="Right" Mode="NextPrev" PageButtonCount="6" Position="Top" CssClass="listViewPaginationTdS1" PrevPageText=".LNK_LIST_PREVIOUS" NextPageText=".LNK_LIST_NEXT" />
		<Columns>
			<asp:HyperLinkColumn HeaderText="ACLRoles.LBL_NAME"        DataTextField="ROLE_NAME" ItemStyle-Width="60%" ItemStyle-CssClass="listViewTdLinkS1" DataNavigateUrlField="ROLE_ID" DataNavigateUrlFormatString="~/Administration/ACLRoles/view.aspx?id={0}" />
			<asp:BoundColumn     HeaderText="ACLRoles.LBL_DESCRIPTION" DataField="DESCRIPTION"   ItemStyle-Width="30%" />
			<asp:TemplateColumn  HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<asp:ImageButton CommandName="Roles.Edit"   CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ROLE_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" ImageUrl='<%# Session["themeURL"] + "images/edit_inline.gif" %>' AlternateText='<%# L10n.Term(".LNK_EDIT") %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
					<asp:LinkButton  CommandName="Roles.Edit"   CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ROLE_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Runat="server"><%# L10n.Term(".LNK_EDIT") %></asp:LinkButton>
					&nbsp;
					<asp:ImageButton CommandName="Roles.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ROLE_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_REMOVE") %>' ImageUrl='<%# Session["themeURL"] + "images/delete_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
					<asp:LinkButton  CommandName="Roles.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ROLE_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_REMOVE") %>' Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>
