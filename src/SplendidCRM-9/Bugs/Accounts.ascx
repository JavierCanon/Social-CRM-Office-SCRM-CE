<%@ Control CodeBehind="Accounts.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Opportunities.Accounts" %>
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
function ChangeAccount(sPARENT_ID, sPARENT_NAME)
{
	document.getElementById('<%= txtACCOUNT_ID.ClientID %>').value = sPARENT_ID  ;
}
function AccountPopup()
{
	return window.open('../Accounts/Popup.aspx','AccountPopup','width=600,height=400,resizable=1,scrollbars=1');
}
</script>
<div id="divBugsAccounts">
	<input ID="txtACCOUNT_ID" type="hidden" Runat="server" />
	<br>
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Accounts.LBL_LIST_FORM_TITLE" Runat="Server" />
	
	<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
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
			<asp:TemplateColumn  HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<asp:ImageButton Visible='<%# SplendidCRM.Security.GetUserAccess("Accounts", "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Accounts.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ACCOUNT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_EDIT") %>' ImageUrl='<%# Session["themeURL"] + "images/edit_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
					<asp:LinkButton  Visible='<%# SplendidCRM.Security.GetUserAccess("Accounts", "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Accounts.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ACCOUNT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_EDIT") %>' Runat="server" />
					&nbsp;
					<span onclick="return confirm('<%= L10n.TermJavaScript("Bugs.NTC_REMOVE_ACCOUNT_CONFIRMATION") %>')">
						<asp:ImageButton Visible='<%# SplendidCRM.Security.GetUserAccess("Bugs", "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "BUG_ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Accounts.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ACCOUNT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_REMOVE") %>' ImageUrl='<%# Session["themeURL"] + "images/delete_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
						<asp:LinkButton  Visible='<%# SplendidCRM.Security.GetUserAccess("Bugs", "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "BUG_ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Accounts.Remove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ACCOUNT_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_REMOVE") %>' Runat="server" />
					</span>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>
