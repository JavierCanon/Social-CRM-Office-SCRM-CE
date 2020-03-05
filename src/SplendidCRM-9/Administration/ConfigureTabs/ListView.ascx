<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.ConfigureTabs.ListView" %>
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
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Administration" Title="Administration.LBL_CONFIGURE_TABS" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>

	<br />
	<SplendidCRM:ListHeader ID="ctlListHeader" Title="Administration.LBL_CONFIGURE_TABS" Visible="false" Runat="Server" />
	
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
					<asp:BoundColumn    HeaderText="Dropdown.LBL_KEY"    DataField="MODULE_NAME" ItemStyle-Width="55%" />
					<asp:BoundColumn    HeaderText="Administration.LBL_TAB_ORDER" DataField="TAB_ORDER"   ItemStyle-Width="5%" />
					<asp:TemplateColumn HeaderText="" ItemStyle-Width="10%" ItemStyle-Wrap="false">
						<ItemTemplate>
							<asp:ImageButton CommandName="ConfigureTabs.MoveUp"   Visible='<%# Sql.ToBoolean(Eval("TAB_ENABLED")) || Sql.ToBoolean(Eval("TAB_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Dropdown.LNK_UP"  ) %>' ImageUrl='<%# Session["themeURL"] + "images/uparrow_inline.gif"   %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
							<asp:LinkButton  CommandName="ConfigureTabs.MoveUp"   Visible='<%# Sql.ToBoolean(Eval("TAB_ENABLED")) || Sql.ToBoolean(Eval("TAB_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Dropdown.LNK_UP") %>' Runat="server" />
							&nbsp;
							<asp:ImageButton CommandName="ConfigureTabs.MoveDown" Visible='<%# Sql.ToBoolean(Eval("TAB_ENABLED")) || Sql.ToBoolean(Eval("TAB_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Dropdown.LNK_DOWN") %>' ImageUrl='<%# Session["themeURL"] + "images/downarrow_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
							<asp:LinkButton  CommandName="ConfigureTabs.MoveDown" Visible='<%# Sql.ToBoolean(Eval("TAB_ENABLED")) || Sql.ToBoolean(Eval("TAB_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Dropdown.LNK_DOWN") %>' Runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="" ItemStyle-Width="5%" ItemStyle-Wrap="false">
						<ItemTemplate>
							<asp:Label Visible='<%#  Sql.ToBoolean(Eval("TAB_ENABLED")) %>' Text='<%# L10n.Term(".LBL_YES") %>' Runat="server" />
							<asp:Label Visible='<%# !Sql.ToBoolean(Eval("TAB_ENABLED")) %>' Text='<%# L10n.Term(".LBL_NO" ) %>' Runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Administration.LBL_VISIBLE" ItemStyle-Width="10%" ItemStyle-Wrap="false">
						<ItemTemplate>
							<asp:ImageButton CommandName="ConfigureTabs.Hide"     Visible='<%#  Sql.ToBoolean(Eval("TAB_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Administration.LNK_HIDE") %>' ImageUrl='<%# Session["themeURL"] + "images/minus_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
							<asp:LinkButton  CommandName="ConfigureTabs.Hide"     Visible='<%#  Sql.ToBoolean(Eval("TAB_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Administration.LNK_HIDE"         ) %>' Runat="server" />
							<asp:ImageButton CommandName="ConfigureTabs.Show"     Visible='<%# !Sql.ToBoolean(Eval("TAB_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Administration.LNK_SHOW") %>' ImageUrl='<%# Session["themeURL"] + "images/plus_inline.gif"  %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
							<asp:LinkButton  CommandName="ConfigureTabs.Show"     Visible='<%# !Sql.ToBoolean(Eval("TAB_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Administration.LNK_SHOW"         ) %>' Runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>

					<asp:TemplateColumn HeaderText="" ItemStyle-Width="5%" ItemStyle-Wrap="false">
						<ItemTemplate>
							<asp:Label Visible='<%#  Sql.ToBoolean(Eval("MOBILE_ENABLED")) %>' Text='<%# L10n.Term(".LBL_YES") %>' Runat="server" />
							<asp:Label Visible='<%# !Sql.ToBoolean(Eval("MOBILE_ENABLED")) %>' Text='<%# L10n.Term(".LBL_NO" ) %>' Runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Administration.LBL_MOBILE" ItemStyle-Width="10%" ItemStyle-Wrap="false">
						<ItemTemplate>
							<asp:ImageButton CommandName="ConfigureTabs.HideMobile" Visible='<%#  Sql.ToBoolean(Eval("MOBILE_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Administration.LNK_HIDE") %>' ImageUrl='<%# Session["themeURL"] + "images/minus_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
							<asp:LinkButton  CommandName="ConfigureTabs.HideMobile" Visible='<%#  Sql.ToBoolean(Eval("MOBILE_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Administration.LNK_HIDE"         ) %>' Runat="server" />
							<asp:ImageButton CommandName="ConfigureTabs.ShowMobile" Visible='<%# !Sql.ToBoolean(Eval("MOBILE_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Administration.LNK_SHOW") %>' ImageUrl='<%# Session["themeURL"] + "images/plus_inline.gif"  %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
							<asp:LinkButton  CommandName="ConfigureTabs.ShowMobile" Visible='<%# !Sql.ToBoolean(Eval("MOBILE_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Administration.LNK_SHOW"         ) %>' Runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>

					<asp:TemplateColumn HeaderText="" ItemStyle-Width="5%" ItemStyle-Wrap="false">
						<ItemTemplate>
							<asp:Label Visible='<%#  Sql.ToBoolean(Eval("MODULE_ENABLED")) %>' Text='<%# L10n.Term(".LBL_YES") %>' Runat="server" />
							<asp:Label Visible='<%# !Sql.ToBoolean(Eval("MODULE_ENABLED")) %>' Text='<%# L10n.Term(".LBL_NO" ) %>' Runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Administration.LNK_ENABLED" ItemStyle-Width="10%" ItemStyle-Wrap="false">
						<ItemTemplate>
							<asp:ImageButton CommandName="ConfigureTabs.Disable"  Visible='<%#  Sql.ToBoolean(Eval("MODULE_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Administration.LNK_DISABLE") %>' ImageUrl='<%# Session["themeURL"] + "images/minus_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
							<asp:LinkButton  CommandName="ConfigureTabs.Disable"  Visible='<%#  Sql.ToBoolean(Eval("MODULE_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Administration.LNK_DISABLE"         ) %>' Runat="server" />
							<asp:ImageButton CommandName="ConfigureTabs.Enable"   Visible='<%# !Sql.ToBoolean(Eval("MODULE_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Administration.LNK_ENABLE" ) %>' ImageUrl='<%# Session["themeURL"] + "images/plus_inline.gif"  %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
							<asp:LinkButton  CommandName="ConfigureTabs.Enable"   Visible='<%# !Sql.ToBoolean(Eval("MODULE_ENABLED")) %>' CommandArgument='<%# Eval("ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Administration.LNK_ENABLE"          ) %>' Runat="server" />
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</SplendidCRM:SplendidGrid>
		</ContentTemplate>
	</asp:UpdatePanel>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>
