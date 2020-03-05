<%@ Control CodeBehind="LanguagePacks.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Terminology.Import.LanguagePacks" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
%>
<div id="divBugsAccounts">
	<input ID="txtACCOUNT_ID" type="hidden" Runat="server" />
	<br>
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="SugarCRM Language Packs" Runat="Server" />
	
	<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	<SplendidCRM:SplendidGrid id="grdMain" Width="100%" CssClass="listView"
		CellPadding="3" CellSpacing="0" border="0"
		AllowPaging="true" PageSize="20" AllowSorting="true" 
		AutoGenerateColumns="false" 
		EnableViewState="true" runat="server">
		<ItemStyle            CssClass="oddListRowS1"  />
		<AlternatingItemStyle CssClass="evenListRowS1" />
		<HeaderStyle          CssClass="listViewThS1"  />
		<PagerStyle HorizontalAlign="Right" Mode="NextPrev" PageButtonCount="6" Position="Top" CssClass="listViewPaginationTdS1" PrevPageText=".LNK_LIST_PREVIOUS" NextPageText=".LNK_LIST_NEXT" />
		<Columns>
			<asp:TemplateColumn  HeaderText="Name"                                SortExpression="Name"        ItemStyle-Width="25%" ItemStyle-Wrap="false">
				<ItemTemplate>
					<asp:ImageButton CommandName="LanguagePack.Import" CommandArgument='<%# Eval("URL") %>' CausesValidation="false" OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='Import' ImageUrl='<%# Session["themeURL"] + "images/import.gif" %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
					<asp:LinkButton  CommandName="LanguagePack.Import" CommandArgument='<%# Eval("URL") %>' CausesValidation="false" OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# Eval("Name") %>' Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn     HeaderText="Date"        DataField="Date"        SortExpression="Date"        ItemStyle-Width="15%" ItemStyle-Wrap="false" />
			<asp:BoundColumn     HeaderText="Description" DataField="Description" SortExpression="Description" ItemStyle-Width="60%" ItemStyle-Wrap="true" />
			<asp:TemplateColumn  HeaderText="" ItemStyle-Width="1%">
				<ItemTemplate>
					<asp:HyperLink  NavigateUrl='<%# Eval("URL") %>' ImageUrl='<%# Session["themeURL"] + "images/backup.gif" %>' runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>
