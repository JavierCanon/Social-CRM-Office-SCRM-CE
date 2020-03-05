<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.RenameTabs.ListView" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005-2008 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
%>
<div id="divListView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Administration" Title="Administration.LBL_RENAME_TABS" EnablePrint="true" HelpName="index" EnableHelp="true" Runat="Server" />
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>

	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchBasic" Src="SearchBasic.ascx" %>
	<SplendidCRM:SearchBasic ID="ctlSearch" Runat="Server" />
	<br />
	<SplendidCRM:ListHeader ID="ctlListHeader" Title="Administration.LBL_EDIT_TABS" Visible="false" Runat="Server" />
	
	<script type="text/javascript">
	function EditItem(sKEY, sVALUE)
	{
		window.open('Popup.aspx?key=' + escape(sKEY) + '&value=' + escape(sVALUE), 'RenameTabsPopup',' width=600,height=70,resizable=1,scrollbars=1');
	}
	function ChangeItem(sKEY, sVALUE)
	{
		document.getElementById('<%= txtRENAME.ClientID %>').value = '1'   ;
		document.getElementById('<%= txtKEY.ClientID    %>').value = sKEY  ;
		document.getElementById('<%= txtVALUE.ClientID  %>').value = sVALUE;
		document.forms[0].submit();
	}
	</script>
	<input ID="txtRENAME" type="hidden" Runat="server" />
	<input ID="txtKEY"    type="hidden" Runat="server" />
	<input ID="txtVALUE"  type="hidden" Runat="server" />
	
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
			<asp:BoundColumn HeaderText="Dropdown.LBL_KEY"    DataField="NAME"         ItemStyle-Width="30%" />
			<asp:TemplateColumn HeaderText="Dropdown.LBL_VALUE" ItemStyle-Width="40%">
				<ItemTemplate>
					<%# Server.HtmlEncode(DataBinder.Eval(Container.DataItem, "DISPLAY_NAME") as string) %>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
				<ItemTemplate>
					<span onclick="EditItem('<%# Sql.EscapeJavaScript(Sql.ToString(DataBinder.Eval(Container.DataItem, "NAME"))) %>', '<%# Sql.EscapeJavaScript(Sql.ToString(DataBinder.Eval(Container.DataItem, "DISPLAY_NAME"))) %>', <%# DataBinder.Eval(Container.DataItem, "LIST_ORDER") %>);return false;">
						<asp:ImageButton CommandName="RenameTabs.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_EDIT") %>' ImageUrl='<%# Session["themeURL"] + "images/edit_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
						<asp:LinkButton  CommandName="RenameTabs.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_EDIT") %>' Runat="server" />
					</span>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>
