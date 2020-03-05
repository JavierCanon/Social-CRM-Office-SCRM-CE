<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.EmailMan.ListView" %>
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
<script type="text/javascript">
function Refresh()
{
	window.location.href = 'default.aspx';
}
function CampaignEmailPreview(gID)
{
	return window.open('Preview.aspx?ID=' + gID, 'CampaignEmailPreview','width=600,height=800,resizable=1,scrollbars=1,status=1');
}
</script>
<div id="divListView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="EmailMan" Title="EmailMan.LBL_MODULE_TITLE" EnableModuleLabel="false" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="Search" Src="SearchBasic.ascx" %>
	<SplendidCRM:Search ID="ctlSearch" Visible="<%# !PrintView %>" Runat="server" />
	<br />
	
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Button CommandName="SendQueued" OnCommand="Page_Command" CssClass="button" Text='<%# " " + L10n.Term(".LBL_CAMPAIGNS_SEND_QUEUED") + " " %>' ToolTip='<%# L10n.Term(".LBL_CAMPAIGNS_SEND_QUEUED") %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_BUTTON_KEY") %>' Runat="server" />
		<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	</asp:Panel>

	
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="EmailMan.LBL_LIST_FORM_TITLE" Runat="Server" />
	
	<SplendidCRM:SplendidGrid id="grdMain" Width="100%" CssClass="listView"
		CellPadding="3" CellSpacing="0" border="0"
		AllowPaging="<%# !PrintView %>" PageSize="20" AllowSorting="true" 
		AutoGenerateColumns="false" 
		EnableViewState="true" runat="server">
		<ItemStyle            CssClass="oddListRowS1"  />
		<AlternatingItemStyle CssClass="evenListRowS1" />
		<HeaderStyle          CssClass="listViewThS1"  />
		<PagerStyle HorizontalAlign="Right" Mode="NextPrev" PageButtonCount="6" Position="Top" CssClass="listViewPaginationTdS1" PrevPageText=".LNK_LIST_PREVIOUS" NextPageText=".LNK_LIST_NEXT" />
		<Columns>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%">
				<ItemTemplate>
					<input name="chkMain" class="checkbox" type="checkbox" value="<%# DataBinder.Eval(Container.DataItem, "ID") %>" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%" ItemStyle-Wrap="false">
				<ItemTemplate>
					<asp:ImageButton OnClientClick=<%# "CampaignEmailPreview(\'" + DataBinder.Eval(Container.DataItem, "ID") + "\'); return false;" %> CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Campaigns.LBL_PREVIEW_LABEL") %>' ImageUrl='<%# Session["themeURL"] + "images/view_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
					<asp:LinkButton  OnClientClick=<%# "CampaignEmailPreview(\'" + DataBinder.Eval(Container.DataItem, "ID") + "\'); return false;" %> CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Campaigns.LBL_PREVIEW_LABEL") %>' Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn     HeaderText="EmailMan.LBL_LIST_CAMPAIGN"        DataField="CAMPAIGN_NAME"        SortExpression="CAMPAIGN_NAME"        ItemStyle-Width="10%" />
			<asp:BoundColumn     HeaderText="EmailMan.LBL_LIST_RECIPIENT_NAME"  DataField="RECIPIENT_NAME"       SortExpression="RECIPIENT_NAME"       ItemStyle-Width="10%" />
			<asp:BoundColumn     HeaderText="EmailMan.LBL_LIST_RECIPIENT_EMAIL" DataField="RECIPIENT_EMAIL"      SortExpression="RECIPIENT_EMAIL"      ItemStyle-Width="10%" />
			<asp:BoundColumn     HeaderText="EmailMan.LBL_LIST_MESSAGE_NAME"    DataField="EMAIL_MARKETING_NAME" SortExpression="EMAIL_MARKETING_NAME" ItemStyle-Width="10%" />
			<asp:TemplateColumn  HeaderText="EmailMan.LBL_LIST_SEND_DATE_TIME"                                   SortExpression="SEND_DATE_TIME"       ItemStyle-Width="10%">
				<ItemTemplate>
					<%# Sql.ToString(T10n.FromServerTime(DataBinder.Eval(Container.DataItem, "SEND_DATE_TIME"))) %>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn     HeaderText="EmailMan.LBL_LIST_SEND_ATTEMPTS"   DataField="SEND_ATTEMPTS"   SortExpression="SEND_ATTEMPTS"   ItemStyle-Width="10%" />
			<asp:BoundColumn     HeaderText="EmailMan.LBL_LIST_IN_QUEUE"        DataField="IN_QUEUE"        SortExpression="IN_QUEUE"        ItemStyle-Width="10%" />
		</Columns>
	</SplendidCRM:SplendidGrid>
	<%@ Register TagPrefix="SplendidCRM" Tagname="CheckAll" Src="~/_controls/CheckAll.ascx" %>
	<SplendidCRM:CheckAll Visible="<%# !PrintView %>" Runat="Server" />
	<%@ Register TagPrefix="SplendidCRM" Tagname="MassUpdate" Src="MassUpdate.ascx" %>
	<SplendidCRM:MassUpdate ID="ctlMassUpdate" Visible="<%# !PrintView %>" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>
