<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MassUpdate.ascx.cs" Inherits="SplendidCRM.Quotes.MassUpdate" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="SplendidCRM" Tagname="DatePicker" Src="~/_controls/DatePicker.ascx" %>
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
<div id="divMassUpdate">
	<br />
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title=".LBL_MASS_UPDATE_TITLE" Runat="Server" />

	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Button ID="btnUpdate" CommandName="MassUpdate" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_UPDATE") %>' Runat="server" />
		<asp:Button ID="btnDelete" CommandName="MassDelete" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_DELETE") %>' Runat="server" />
	</asp:Panel>

	<script type="text/javascript">
	var ChangeAccount = null;
	var ChangeContact = null;

	function ChangeShippingAccount(sPARENT_ID, sPARENT_NAME)
	{
		document.getElementById('<%= txtSHIPPING_ACCOUNT_ID.ClientID   %>').value = sPARENT_ID  ;
		document.getElementById('<%= txtSHIPPING_ACCOUNT_NAME.ClientID %>').value = sPARENT_NAME;
	}
	function ChangeBillingAccount(sPARENT_ID, sPARENT_NAME)
	{
		document.getElementById('<%= txtBILLING_ACCOUNT_ID.ClientID   %>').value = sPARENT_ID  ;
		document.getElementById('<%= txtBILLING_ACCOUNT_NAME.ClientID %>').value = sPARENT_NAME;
	}
	function ChangeShippingContact(sPARENT_ID, sPARENT_NAME)
	{
		document.getElementById('<%= txtSHIPPING_CONTACT_ID.ClientID   %>').value = sPARENT_ID  ;
		document.getElementById('<%= txtSHIPPING_CONTACT_NAME.ClientID %>').value = sPARENT_NAME;
	}
	function ChangeBillingContact(sPARENT_ID, sPARENT_NAME)
	{
		document.getElementById('<%= txtBILLING_CONTACT_ID.ClientID   %>').value = sPARENT_ID  ;
		document.getElementById('<%= txtBILLING_CONTACT_NAME.ClientID %>').value = sPARENT_NAME;
	}
	function AccountPopup()
	{
		return window.open('../Accounts/Popup.aspx','AccountPopup','width=600,height=400,resizable=1,scrollbars=1');
	}
	function ContactPopup()
	{
		return window.open('../Contacts/Popup.aspx','ContactPopup','width=600,height=400,resizable=1,scrollbars=1');
	}
	</script>
	<asp:Table Width="100%" CellPadding="0" CellSpacing="0" CssClass="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<%@ Register TagPrefix="SplendidCRM" Tagname="TeamAssignedMassUpdate" Src="~/_controls/TeamAssignedMassUpdate.ascx" %>
				<SplendidCRM:TeamAssignedMassUpdate ID="ctlTeamAssignedMassUpdate" Runat="Server" />
				<asp:Table Width="100%" CellPadding="0" CellSpacing="0" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Quotes.LBL_DATE_VALID_UNTIL") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><SplendidCRM:DatePicker ID="ctlDATE_QUOTE_EXPECTED_CLOSED" Runat="Server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Quotes.LBL_ORIGINAL_PO_DATE") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><SplendidCRM:DatePicker ID="ctlORIGINAL_PO_DATE" Runat="Server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Quotes.LBL_PAYMENT_TERMS") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="lstPAYMENT_TERMS" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Quotes.LBL_QUOTE_STAGE") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="lstQUOTE_STAGE" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Quotes.LBL_SHIPPING_ACCOUNT_NAME") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox     ID="txtSHIPPING_ACCOUNT_NAME" ReadOnly="True" Runat="server" />
							<asp:HiddenField ID="txtSHIPPING_ACCOUNT_ID" runat="server" />&nbsp;
							<asp:Button      ID="btnSHIPPING_ACCOUNT_ID" OnClientClick="ChangeAccount=ChangeShippingAccount;AccountPopup(); return false;" Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Quotes.LBL_SHIPPING_CONTACT_NAME") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox     ID="txtSHIPPING_CONTACT_NAME" ReadOnly="True" Runat="server" />
							<asp:HiddenField ID="txtSHIPPING_CONTACT_ID" runat="server" />&nbsp;
							<asp:Button      ID="btnSHIPPING_CONTACT_ID" OnClientClick="ChangeContact=ChangeShippingContact;ContactPopup(); return false;" Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Quotes.LBL_BILLING_ACCOUNT_NAME") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox     ID="txtBILLING_ACCOUNT_NAME" ReadOnly="True" Runat="server" />
							<asp:HiddenField ID="txtBILLING_ACCOUNT_ID" runat="server" />&nbsp;
							<asp:Button      ID="btnBILLING_ACCOUNT_ID" OnClientClick="ChangeAccount=ChangeBillingAccount;AccountPopup(); return false;" Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Quotes.LBL_BILLING_CONTACT_NAME") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox     ID="txtBILLING_CONTACT_NAME" ReadOnly="True" Runat="server" />
							<asp:HiddenField ID="txtBILLING_CONTACT_ID" runat="server" />&nbsp;
							<asp:Button      ID="btnBILLING_CONTACT_ID" OnClientClick="ChangeContact=ChangeBillingContact;ContactPopup(); return false;" Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
