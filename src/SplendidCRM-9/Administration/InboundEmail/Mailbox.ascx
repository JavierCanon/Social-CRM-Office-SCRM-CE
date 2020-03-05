<%@ Control CodeBehind="Mailbox.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.InboundEmail.Mailbox" %>
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
<div id="divProductTemplatesProductTemplates">
	<br />
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="InboundEmail.LBL_MAILBOX_DEFAULT" Runat="Server" />

	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Button CommandName="Mailbox.CheckMail"   OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term("Emails.LBL_BUTTON_CHECK" ) + "  " %>' ToolTip='<%# L10n.Term("Emails.LBL_BUTTON_CHECK_TITLE" ) %>' AccessKey='<%# L10n.AccessKey("Emails.LBL_BUTTON_CHECK_KEY" ) %>' Runat="server" />
		<asp:Button CommandName="Mailbox.CheckBounce" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term("Emails.LBL_BUTTON_BOUNCE") + "  " %>' ToolTip='<%# L10n.Term("Emails.LBL_BUTTON_BOUNCE_TITLE") %>' AccessKey='<%# L10n.AccessKey("Emails.LBL_BUTTON_BOUNCE_KEY") %>' Runat="server" />
		<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	</asp:Panel>

	<SplendidCRM:SplendidGrid id="grdMain" Width="100%" CssClass="listView"
		CellPadding="3" CellSpacing="0" border="0"
		AllowPaging="true" PageSize="20" AllowSorting="false" 
		AutoGenerateColumns="true" 
		EnableViewState="true" runat="server">
		<ItemStyle            CssClass="oddListRowS1"  VerticalAlign="Top" />
		<AlternatingItemStyle CssClass="evenListRowS1" VerticalAlign="Top" />
		<HeaderStyle          CssClass="listViewThS1"  />
		<PagerStyle HorizontalAlign="Right" Mode="NextPrev" PageButtonCount="6" Position="Top" CssClass="listViewPaginationTdS1" PrevPageText=".LNK_LIST_PREVIOUS" NextPageText=".LNK_LIST_NEXT" />
		<Columns>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>
