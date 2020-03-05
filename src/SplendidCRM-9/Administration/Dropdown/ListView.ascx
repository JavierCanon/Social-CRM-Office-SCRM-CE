<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Dropdown.ListView" %>
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
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Dropdown" Title=".moduleList.Home" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchBasic" Src="SearchBasic.ascx" %>
	<SplendidCRM:SearchBasic ID="ctlSearch" Runat="Server" />
	<br>
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader ID="ctlListHeader" Title="Dropdown.LBL_LIST_FORM_TITLE" Visible="false" Runat="Server" />
	
	<script type="text/javascript">
	function AddItem(nINDEX)
	{
		//window.open('Popup.aspx?dropdown_select=<%= Server.UrlEncode(ctlSearch.DROPDOWN) %>&language_select=<%= Server.UrlEncode(ctlSearch.LANGUAGE) %>&array_index=' + nINDEX, 'DropdownPopup',' width=600,height=70,resizable=1,scrollbars=1');
		window.open('Popup.aspx?array_index=' + nINDEX, 'DropdownPopup',' width=600,height=70,resizable=1,scrollbars=1');
	}
	function EditItem(sKEY, sVALUE, nINDEX)
	{
		//window.open('Popup.aspx?dropdown_select=<%= Server.UrlEncode(ctlSearch.DROPDOWN) %>&language_select=<%= Server.UrlEncode(ctlSearch.LANGUAGE) %>&array_index=' + nINDEX, 'DropdownPopup',' width=600,height=70,resizable=1,scrollbars=1');
		window.open('Popup.aspx?key=' + escape(sKEY) + '&value=' + escape(sVALUE) + '&array_index=' + nINDEX, 'DropdownPopup',' width=600,height=70,resizable=1,scrollbars=1');
	}
	function ChangeItem(sKEY, sVALUE, nINDEX)
	{
		document.getElementById('<%= txtINSERT.ClientID %>').value = '1'   ;
		document.getElementById('<%= txtKEY.ClientID    %>').value = sKEY  ;
		document.getElementById('<%= txtVALUE.ClientID  %>').value = sVALUE;
		document.getElementById('<%= txtINDEX.ClientID  %>').value = nINDEX;
		document.forms[0].submit();
	}
	</script>
	<input ID="txtINSERT" type="hidden" Runat="server" />
	<input ID="txtKEY"    type="hidden" Runat="server" />
	<input ID="txtVALUE"  type="hidden" Runat="server" />
	<input ID="txtINDEX"  type="hidden" Runat="server" />
	
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<div style="DISPLAY: <%= bEnableAdd ? "inline" : "none" %>" >
			<asp:Button OnClientClick="AddItem(-1); return false;" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_ADD_BUTTON") + "  " %>' ToolTip='<%# L10n.Term(".LBL_ADD_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_ADD_BUTTON_KEY") %>' Runat="server" />
		</div>
		<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
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
			<asp:BoundColumn HeaderText="Dropdown.LBL_KEY"    DataField="NAME"         ItemStyle-Width="30%" />
			<asp:TemplateColumn HeaderText="Dropdown.LBL_KEY" ItemStyle-Width="40%">
				<ItemTemplate>
					<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "DISPLAY_NAME") as string) %>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
				<ItemTemplate>
					<%# DataBinder.Eval(Container.DataItem, "LIST_ORDER") %>
					&nbsp;
					<asp:ImageButton CommandName="Dropdown.MoveUp"   CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Dropdown.LNK_UP") %>' ImageUrl='<%# Session["themeURL"] + "images/uparrow_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
					<asp:LinkButton  CommandName="Dropdown.MoveUp"   CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Dropdown.LNK_UP") %>' Runat="server" />
					&nbsp;
					<asp:ImageButton CommandName="Dropdown.MoveDown" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Dropdown.LNK_DOWN") %>' ImageUrl='<%# Session["themeURL"] + "images/downarrow_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
					<asp:LinkButton  CommandName="Dropdown.MoveDown" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Dropdown.LNK_DOWN") %>' Runat="server" />
					&nbsp;
					<span onclick="AddItem(<%# DataBinder.Eval(Container.DataItem, "LIST_ORDER") %>);return false;">
						<asp:ImageButton CommandName="Dropdown.Insert"   CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term("Dropdown.LNK_INS") %>' ImageUrl='<%# Session["themeURL"] + "images/plus_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
						<asp:LinkButton  CommandName="Dropdown.Insert"   CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term("Dropdown.LNK_INS") %>' Runat="server" />
					</span>
					&nbsp;
					<span onclick="EditItem('<%# Sql.EscapeJavaScript(Sql.ToString(DataBinder.Eval(Container.DataItem, "NAME"))) %>', '<%# Sql.EscapeJavaScript(Sql.ToString(DataBinder.Eval(Container.DataItem, "DISPLAY_NAME"))) %>', <%# DataBinder.Eval(Container.DataItem, "LIST_ORDER") %>);return false;">
						<asp:ImageButton CommandName="Dropdown.Edit"     CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_EDIT") %>' ImageUrl='<%# Session["themeURL"] + "images/edit_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
						<asp:LinkButton  CommandName="Dropdown.Edit"     CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_EDIT") %>' Runat="server" />
					</span>
					&nbsp;
					<asp:ImageButton CommandName="Dropdown.Delete"   CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' ImageUrl='<%# Session["themeURL"] + "images/delete_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
					<asp:LinkButton  CommandName="Dropdown.Delete"   CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_DELETE") %>' Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>
