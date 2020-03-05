<%@ Control CodeBehind="DocumentRevisions.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Documents.DocumentRevisions" %>
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
<div id="divDocumentsDocumentRevisions">
	<br />
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Documents.LBL_MODULE_NAME" Runat="Server" />
	
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Button ID="btnCreateRevisions" CommandName="Documents.CreateRevision" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_NEW_BUTTON_LABEL"   ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_NEW_BUTTON_TITLE"   ) %>' AccessKey='<%# L10n.AccessKey(".LBL_NEW_BUTTON_KEY"   ) %>' Runat="server" />
		<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	</asp:Panel>

	<script type="text/javascript">
	function ConfirmDeleteDocumentVersion(gRevisionID)
	{
		if ( gRevisionID == gCurrentRevisionID )
		{
			alert('<%= L10n.Term("Documents.ERR_DELETE_LATEST_VERSION") %>');
			return false;
		}
		else
		{
			return confirm('<%= L10n.Term("Documents.ERR_DELETE_CONFIRM") %>');
		}
	}
	</script>
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
			<asp:TemplateColumn  HeaderText="" ItemStyle-Width="2%">
				<ItemTemplate>
					<asp:HyperLink NavigateUrl='<%# "~/Documents/Document.aspx?id=" + DataBinder.Eval(Container.DataItem, "ID") %>' Target="_blank" AlternateText='<%# L10n.Term("Documents.LBL_LIST_VIEW_DOCUMENT") %>' Runat="server">
						<asp:Image ImageUrl='<%# Session["themeURL"] + "images/def_image_inline.gif" %>' BorderWidth="0" Width="16" Height="16" Runat="server" />
					</asp:HyperLink>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn     HeaderText="Documents.LBL_REV_LIST_REVISION" DataField="REVISION"     ItemStyle-Width="10%" />
			<asp:TemplateColumn  HeaderText="Documents.LBL_REV_LIST_ENTERED"                           ItemStyle-Width="15%" ItemStyle-Wrap="false">
				<ItemTemplate>
					<%# Sql.ToDateString(T10n.FromServerTime(DataBinder.Eval(Container.DataItem, "DATE_ENTERED"))) %>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn     HeaderText="Documents.LBL_REV_LIST_CREATED"  DataField="CREATED_BY"   ItemStyle-Width="10%" />
			<asp:BoundColumn     HeaderText="Documents.LBL_REV_LIST_LOG"      DataField="CHANGE_LOG"   ItemStyle-Width="55%" />
			<asp:TemplateColumn  HeaderText="" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<span onclick="return ConfirmDeleteDocumentVersion('<%# DataBinder.Eval(Container.DataItem, "ID") %>');">
						<asp:ImageButton CommandName="Documents.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' ImageUrl='<%# Session["themeURL"] + "images/delete_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
						<asp:LinkButton  CommandName="Documents.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_DELETE") %>' Runat="server" />
					</span>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>
