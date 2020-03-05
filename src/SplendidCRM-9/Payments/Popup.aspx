<%@ Page language="c#" MasterPageFile="~/PopupView.Master" Codebehind="Popup.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Payments.Popup" %>
<script runat="server">
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
</script>
<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchView" Src="~/_controls/SearchView.ascx" %>
	<SplendidCRM:SearchView ID="ctlSearchView" Module="Payments" IsPopupSearch="true" ShowSearchTabs="false" Visible="<%# !PrintView %>" Runat="Server" />

<script type="text/javascript">
function SelectPayment(sPARENT_ID, sPARENT_NAME)
{
	if ( window.opener != null && window.opener.ChangePayment != null )
	{
		window.opener.ChangePayment(sPARENT_ID, sPARENT_NAME);
		window.close();
	}
	else
	{
		alert('Original window has closed.  Payment cannot be assigned.' + '\n' + sPARENT_ID + '\n' + sPARENT_NAME);
	}
}
function Clear()
{
	if ( window.opener != null && window.opener.ChangePayment != null )
	{
		window.opener.ChangePayment('', '');
		window.close();
	}
	else
	{
		alert('Original window has closed.  Payment cannot be assigned.');
	}
}
function Cancel()
{
	window.close();
}
</script>
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Payments.LBL_LIST_FORM_TITLE" Runat="Server" />
	<asp:Panel ID="pnlPopupButtons" CssClass="button-panel" runat="server">
		<asp:Button ID="btnPopupClear"  OnClientClick="Clear(); return false;"  CssClass="button" Text='<%# "  " + L10n.Term(".LBL_CLEAR_BUTTON_LABEL" ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_CLEAR_BUTTON_TITLE" ) %>' AccessKey='<%# L10n.AccessKey(".LBL_CLEAR_BUTTON_KEY" ) %>' Visible='<%# (Request["ClearDisabled"] == "1") ? false : true %>' runat="server" />
		<asp:Button ID="btnPopupCancel" OnClientClick="Cancel(); return false;" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_CANCEL_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_CANCEL_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_CANCEL_BUTTON_KEY") %>' runat="server" />
		<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	</asp:Panel>
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
		</Columns>
	</SplendidCRM:SplendidGrid>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</asp:Content>
