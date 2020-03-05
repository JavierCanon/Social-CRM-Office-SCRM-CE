<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Shortcuts.ListView" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
</script>
<div id="divListView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Shortcuts" Title=".moduleList.Home" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/_controls/SearchView.ascx" %>
	<SplendidCRM:SearchView ID="ctlSearchView" Module="Shortcuts" ShowSearchTabs="false" ShowSearchViews="false" Visible="<%# !PrintView %>" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader ID="ctlListHeader" Module="Shortcuts" Title="Shortcuts.LBL_LIST_FORM_TITLE" Runat="Server" />
	
	<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	<SplendidCRM:SplendidGrid id="grdMain" Width="100%" CssClass="listView"
		CellPadding="3" CellSpacing="0" border="0"
		AllowPaging="<%# !PrintView %>" PageSize="20" AllowSorting="false" 
		AutoGenerateColumns="false" 
		EnableViewState="true" runat="server">
		<ItemStyle            CssClass="oddListRowS1"  />
		<AlternatingItemStyle CssClass="evenListRowS1" />
		<HeaderStyle          CssClass="listViewThS1"  />
		<PagerStyle HorizontalAlign="Right" Mode="NextPrev" PageButtonCount="6" Position="Top" CssClass="listViewPaginationTdS1" PrevPageText=".LNK_LIST_PREVIOUS" NextPageText=".LNK_LIST_NEXT" />
		<Columns>
			<asp:BoundColumn     HeaderText="Shortcuts.LBL_LIST_MODULE_NAME"    DataField="MODULE_NAME"      SortExpression="MODULE_NAME"    ItemStyle-Width="10%" />
			<asp:HyperLinkColumn HeaderText="Shortcuts.LBL_LIST_DISPLAY_NAME"   DataTextField="DISPLAY_NAME" SortExpression="DISPLAY_NAME"   ItemStyle-Width="22%" ItemStyle-CssClass="listViewTdLinkS1" DataNavigateUrlField="ID" DataNavigateUrlFormatString="edit.aspx?id={0}" />
			<asp:BoundColumn     HeaderText="Shortcuts.LBL_LIST_RELATIVE_PATH"  DataField="RELATIVE_PATH"    SortExpression="RELATIVE_PATH"  ItemStyle-Width="10%" />
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
				<ItemTemplate>
					<%# DataBinder.Eval(Container.DataItem, "SHORTCUT_ORDER") %>
					&nbsp;
					<asp:ImageButton CommandName="Shortcuts.MoveUp"   CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Shortcuts.LNK_UP") %>' ImageUrl='<%# Session["themeURL"] + "images/uparrow_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
					<asp:LinkButton  CommandName="Shortcuts.MoveUp"   CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Shortcuts.LNK_UP") %>' Runat="server" />
					&nbsp;
					<asp:ImageButton CommandName="Shortcuts.MoveDown" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Shortcuts.LNK_DOWN") %>' ImageUrl='<%# Session["themeURL"] + "images/downarrow_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
					<asp:LinkButton  CommandName="Shortcuts.MoveDown" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Shortcuts.LNK_DOWN") %>' Runat="server" />
					&nbsp;
					<asp:ImageButton CommandName="Shortcuts.Edit"     CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_EDIT") %>' ImageUrl='<%# Session["themeURL"] + "images/edit_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
					<asp:LinkButton  CommandName="Shortcuts.Edit"     CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_EDIT") %>' Runat="server" />
					&nbsp;
					<asp:ImageButton CommandName="Shortcuts.Delete"   CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' ImageUrl='<%# Session["themeURL"] + "images/delete_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
					<asp:LinkButton  CommandName="Shortcuts.Delete"   CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_DELETE") %>' Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>
