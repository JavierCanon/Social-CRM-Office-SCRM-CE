<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Reports.ListView" %>
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
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Reports" Title=".moduleList.Home" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />

	<table width="100%" border="0" cellpadding="0" cellspacing="0" Visible="<%# !PrintView %>" runat="server">
		<tr>
			<td style="padding-bottom: 2px;">
				<asp:Button Visible='<%# SplendidCRM.Security.GetUserAccess("Reports", "edit"  ) >= 0 %>' CommandName="Reports.Create" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term("Reports.LBL_CREATE_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term("Reports.LBL_CREATE_BUTTON_LABEL") %>' Runat="server" />
				<asp:Button Visible='<%# SplendidCRM.Security.GetUserAccess("Reports", "import") >= 0 %>' CommandName="Reports.Import" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term("Reports.LBL_IMPORT_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term("Reports.LBL_IMPORT_BUTTON_TITLE") %>' Runat="server" />
				<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
			</td>
		</tr>
	</table>
	<br />

	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader ID="ctlListHeaderMySaved" Visible="<%# !PrintView %>" Title=".saved_reports_dom.All" Runat="Server" />
	<SplendidCRM:SplendidGrid id="grdMySaved" Width="100%" CssClass="listView"
		CellPadding="3" CellSpacing="0" border="0"
		AllowPaging="<%# !PrintView %>" PageSize="20" AllowSorting="true" 
		AutoGenerateColumns="false" 
		EnableViewState="true" runat="server">
		<ItemStyle            CssClass="oddListRowS1"  />
		<AlternatingItemStyle CssClass="evenListRowS1" />
		<HeaderStyle          CssClass="listViewThS1"  />
		<PagerStyle HorizontalAlign="Right" Mode="NextPrev" PageButtonCount="6" Position="Top" CssClass="listViewPaginationTdS1" PrevPageText=".LNK_LIST_PREVIOUS" NextPageText=".LNK_LIST_NEXT" />
		<Columns>
			<asp:HyperLinkColumn HeaderText="Reports.LBL_LIST_REPORT_NAME"    DataTextField="NAME"  SortExpression="NAME" ItemStyle-Width="35%" ItemStyle-CssClass="listViewTdLinkS1" DataNavigateUrlField="ID" DataNavigateUrlFormatString="edit.aspx?id={0}" />
			<asp:BoundColumn     HeaderText="Reports.LBL_LIST_MODULE_NAME"    DataField="MODULE_NAME"                     ItemStyle-Width="15%" />
			<asp:BoundColumn     HeaderText="Reports.LBL_LIST_REPORT_TYPE"    DataField="REPORT_TYPE"                     ItemStyle-Width="25%" />
			<asp:TemplateColumn  HeaderText="" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
				<ItemTemplate>
					<asp:HyperLink NavigateUrl='<%# "~/Reports/ExportRDL.aspx?id=" + DataBinder.Eval(Container.DataItem, "ID") %>' Target="_blank" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Reports.LBL_EXPORT_RDL") %>' Visible='<%# nACLACCESS_Export >= 0 %>' Runat="server">
						<asp:Image ImageUrl='<%# Session["themeURL"] + "images/export.gif" %>' BorderWidth="0" Width="10" Height="10" Runat="server" />&nbsp;<%# L10n.Term(".LBL_EXPORT") %>
					</asp:HyperLink>
					&nbsp;
					<asp:HyperLink NavigateUrl='<%# "edit.aspx?DuplicateID=" + Eval("ID") %>' Text='<%# L10n.Term(".LBL_DUPLICATE_BUTTON_LABEL") %>' CssClass="listViewTdToolsS1" Runat="server" />
					&nbsp;
					<asp:ImageButton CommandName="Reports.View" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Reports.LNK_VIEW") %>' ImageUrl='<%# Session["themeURL"] + "images/view_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
					<asp:LinkButton  CommandName="Reports.View" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Reports.LNK_VIEW") %>' Runat="server" />
					&nbsp;
					<span onclick="return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>')">
						<asp:ImageButton Visible='<%# SplendidCRM.Security.GetUserAccess("Reports", "delete", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Reports.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' ImageUrl='<%# Session["themeURL"] + "images/delete_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
						<asp:LinkButton  Visible='<%# SplendidCRM.Security.GetUserAccess("Reports", "delete", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Reports.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_DELETE") %>' Runat="server" />
					</span>
					&nbsp;
					<asp:ImageButton CommandName="Reports.Publish" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Reports.LNK_PUBLISH") %>' ImageUrl='<%# Session["themeURL"] + "images/unpublish_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
					<asp:LinkButton  CommandName="Reports.Publish" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Reports.LNK_PUBLISH") %>' Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>

	<br />
	<SplendidCRM:ListHeader ID="ctlListHeaderPublished" Visible="<%# !PrintView %>" Title=".published_reports_dom.All" Runat="Server" />
	<SplendidCRM:SplendidGrid id="grdPublished" Width="100%" CssClass="listView"
		CellPadding="3" CellSpacing="0" border="0"
		AllowPaging="<%# !PrintView %>" PageSize="20" AllowSorting="true" 
		AutoGenerateColumns="false" 
		EnableViewState="true" runat="server">
		<ItemStyle            CssClass="oddListRowS1"  />
		<AlternatingItemStyle CssClass="evenListRowS1" />
		<HeaderStyle          CssClass="listViewThS1"  />
		<PagerStyle HorizontalAlign="Right" Mode="NextPrev" PageButtonCount="6" Position="Top" CssClass="listViewPaginationTdS1" PrevPageText=".LNK_LIST_PREVIOUS" NextPageText=".LNK_LIST_NEXT" />
		<Columns>
			<asp:HyperLinkColumn HeaderText="Reports.LBL_LIST_REPORT_NAME"    DataTextField="NAME"  SortExpression="NAME" ItemStyle-Width="35%" ItemStyle-CssClass="listViewTdLinkS1" DataNavigateUrlField="ID" DataNavigateUrlFormatString="edit.aspx?id={0}" />
			<asp:BoundColumn     HeaderText="Reports.LBL_LIST_MODULE_NAME"    DataField="MODULE_NAME"                     ItemStyle-Width="15%" />
			<asp:BoundColumn     HeaderText="Reports.LBL_LIST_REPORT_TYPE"    DataField="REPORT_TYPE"                     ItemStyle-Width="25%" />
			<asp:TemplateColumn  HeaderText="" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
				<ItemTemplate>
					<span visible='<%# SplendidCRM.Security.IS_ADMIN || (Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ASSIGNED_USER_ID")) == SplendidCRM.Security.USER_ID) %>' runat="server">
						<asp:HyperLink NavigateUrl='<%# "~/Reports/ExportRDL.aspx?id=" + DataBinder.Eval(Container.DataItem, "ID") %>' Target="_blank" AlternateText='<%# L10n.Term(".LBL_EXPORT") %>' Visible='<%# nACLACCESS_Export >= 0 %>' Runat="server">
							<asp:Image ImageUrl='<%# Session["themeURL"] + "images/export.gif" %>' BorderWidth="0" Width="10" Height="10" Runat="server" />&nbsp;<%# L10n.Term(".LBL_EXPORT") %>
						</asp:HyperLink>
						&nbsp;
					</span>
					<asp:ImageButton CommandName="Reports.View" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Reports.LNK_VIEW") %>' ImageUrl='<%# Session["themeURL"] + "images/view_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
					<asp:LinkButton  CommandName="Reports.View" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Reports.LNK_VIEW") %>' Runat="server" />
					&nbsp;
					<span visible='<%# SplendidCRM.Security.IS_ADMIN || (Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ASSIGNED_USER_ID")) == SplendidCRM.Security.USER_ID) %>' runat="server">
						<asp:ImageButton CommandName="Reports.Unpublish" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Reports.LNK_UNPUBLISH") %>' ImageUrl='<%# Session["themeURL"] + "images/unpublish_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
						<asp:LinkButton  CommandName="Reports.Unpublish" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Reports.LNK_UNPUBLISH") %>' Runat="server" />
					</span>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>
