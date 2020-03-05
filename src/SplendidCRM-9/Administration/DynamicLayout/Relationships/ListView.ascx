<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.DynamicLayout.Relationships.ListView" %>
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
<div id="divListView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Administration" Title="DynamicLayout.LBL_RELATIONSHIPS_LAYOUT" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchBasic" Src="../_controls/SearchBasic.ascx" %>
	<SplendidCRM:SearchBasic ID="ctlSearch" ViewTableName="vwDETAILVIEWS_Layout" ViewFieldName="DETAIL_NAME" Runat="Server" />
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader ID="ctlListHeader" Runat="Server" />
	
	<asp:UpdatePanel runat="server">
		<ContentTemplate>
			<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
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
					<asp:BoundColumn    HeaderText="DynamicLayout.LBL_LIST_MODULE_NAME" DataField="MODULE_NAME" ItemStyle-Width="25%" />
					<asp:TemplateColumn HeaderText="DynamicLayout.LBL_LIST_TITLE"                               ItemStyle-Width="30%" ItemStyle-Wrap="false">
						<ItemTemplate>
							<%# L10n.Term(Sql.ToString(Eval("TITLE"))) %>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:BoundColumn    HeaderText="Administration.LBL_TAB_ORDER" DataField="RELATIONSHIP_ORDER" ItemStyle-Width="5%" />
					<asp:TemplateColumn HeaderText="" ItemStyle-Width="10%" ItemStyle-Wrap="false">
						<ItemTemplate>
							<asp:ImageButton CommandName="Relationships.MoveUp"   Visible='<%#  Sql.ToBoolean(Eval("RELATIONSHIP_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Dropdown.LNK_UP"  ) %>' ImageUrl='<%# Session["themeURL"] + "images/uparrow_inline.gif"   %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
							<asp:LinkButton  CommandName="Relationships.MoveUp"   Visible='<%#  Sql.ToBoolean(Eval("RELATIONSHIP_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Dropdown.LNK_UP") %>' Runat="server" />
							&nbsp;
							<asp:ImageButton CommandName="Relationships.MoveDown" Visible='<%#  Sql.ToBoolean(Eval("RELATIONSHIP_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Dropdown.LNK_DOWN") %>' ImageUrl='<%# Session["themeURL"] + "images/downarrow_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
							<asp:LinkButton  CommandName="Relationships.MoveDown" Visible='<%#  Sql.ToBoolean(Eval("RELATIONSHIP_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Dropdown.LNK_DOWN") %>' Runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Administration.LNK_ENABLED" ItemStyle-Width="5%" ItemStyle-Wrap="false">
						<ItemTemplate>
							<asp:Label Visible='<%#  Sql.ToBoolean(Eval("RELATIONSHIP_ENABLED")) %>' Text='<%# L10n.Term(".LBL_YES") %>' Runat="server" />
							<asp:Label Visible='<%# !Sql.ToBoolean(Eval("RELATIONSHIP_ENABLED")) %>' Text='<%# L10n.Term(".LBL_NO" ) %>' Runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="" ItemStyle-Width="10%" ItemStyle-Wrap="false">
						<ItemTemplate>
							<asp:ImageButton CommandName="Relationships.Disable"  Visible='<%#  Sql.ToBoolean(Eval("RELATIONSHIP_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Administration.LNK_DISABLE") %>' ImageUrl='<%# Session["themeURL"] + "images/minus_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
							<asp:LinkButton  CommandName="Relationships.Disable"  Visible='<%#  Sql.ToBoolean(Eval("RELATIONSHIP_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Administration.LNK_DISABLE"         ) %>' Runat="server" />
							<asp:ImageButton CommandName="Relationships.Enable"   Visible='<%# !Sql.ToBoolean(Eval("RELATIONSHIP_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Administration.LNK_ENABLE" ) %>' ImageUrl='<%# Session["themeURL"] + "images/plus_inline.gif"  %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
							<asp:LinkButton  CommandName="Relationships.Enable"   Visible='<%# !Sql.ToBoolean(Eval("RELATIONSHIP_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Administration.LNK_ENABLE"          ) %>' Runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</SplendidCRM:SplendidGrid>
		</ContentTemplate>
	</asp:UpdatePanel>
</div>
