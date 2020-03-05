<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.EditCustomFields.ListView" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
%>
<div id="divListView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="EditCustomFields" Title="EditCustomFields.LBL_MODULE_TITLE" EnableModuleLabel="false" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchBasic" Src="SearchBasic.ascx" %>
	<SplendidCRM:SearchBasic ID="ctlSearch" Runat="Server" />
	<br>
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader ID="ctlListTitle" Title="EditCustomFields.LBL_CUSTOM_FIELDS" Runat="Server" />
	<p>
	<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	<SplendidCRM:SplendidGrid id="grdMain" Width="100%" CssClass="listView"
		CellPadding="3" CellSpacing="0" border="0"
		AllowPaging="false" PageSize="20" AllowSorting="true" 
		AutoGenerateColumns="false" 
		EnableViewState="true" runat="server">
		<ItemStyle            CssClass="oddListRowS1"  />
		<AlternatingItemStyle CssClass="evenListRowS1" />
		<HeaderStyle          CssClass="listViewThS1"  />
		<PagerStyle HorizontalAlign="Right" Mode="NextPrev" PageButtonCount="6" Position="Top" CssClass="listViewPaginationTdS1" PrevPageText=".LNK_LIST_PREVIOUS" NextPageText=".LNK_LIST_NEXT" />
		<Columns>
			<asp:HyperLinkColumn HeaderText="EditCustomFields.COLUMN_TITLE_NAME"            DataTextField="NAME"        SortExpression="NAME"            ItemStyle-Width="22%" ItemStyle-CssClass="listViewTdLinkS1" DataNavigateUrlField="ID"         DataNavigateUrlFormatString="edit.aspx?id={0}" />
			<asp:BoundColumn     HeaderText="EditCustomFields.COLUMN_TITLE_LABEL"           DataField="LABEL"           SortExpression="LABEL"           ItemStyle-Width="22%" />
			<asp:BoundColumn     HeaderText="EditCustomFields.COLUMN_TITLE_DATA_TYPE"       DataField="DATA_TYPE"       SortExpression="DATA_TYPE"       ItemStyle-Width="10%" />
			<asp:BoundColumn     HeaderText="EditCustomFields.COLUMN_TITLE_MAX_SIZE"        DataField="MAX_SIZE"        SortExpression="MAX_SIZE"        ItemStyle-Width="10%" />
			<asp:BoundColumn     HeaderText="EditCustomFields.COLUMN_TITLE_REQUIRED_OPTION" DataField="REQUIRED_OPTION" SortExpression="REQUIRED_OPTION" ItemStyle-Width="10%" />
			<asp:BoundColumn     HeaderText="EditCustomFields.COLUMN_TITLE_DEFAULT_VALUE"   DataField="DEFAULT_VALUE"   SortExpression="DEFAULT_VALUE"   ItemStyle-Width="10%" />
			<asp:BoundColumn     HeaderText="EditCustomFields.COLUMN_TITLE_DROPDOWN"        DataField="EXT1"            SortExpression="EXT1"            ItemStyle-Width="10%" />
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<span onclick="return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>')">
						<asp:ImageButton CommandName="EditCustomFields.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' ImageUrl='<%# Session["themeURL"] + "images/delete_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
						<asp:LinkButton  CommandName="EditCustomFields.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_DELETE") %>' Runat="server" />
					</span>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>
