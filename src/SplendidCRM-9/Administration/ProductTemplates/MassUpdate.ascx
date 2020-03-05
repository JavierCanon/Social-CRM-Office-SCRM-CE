<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MassUpdate.ascx.cs" Inherits="SplendidCRM.Administration.ProductTemplates.MassUpdate" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<script type="text/javascript">
// 02/26/2007 Paul.  We need to redefine the calendar function so that it points two levels up. 
function CalendarPopup(ctlDate, clientX, clientY)
{
	clientX = window.screenLeft + parseInt(clientX);
	clientY = window.screenTop  + parseInt(clientY);
	if ( clientX < 0 )
		clientX = 0;
	if ( clientY < 0 )
		clientY = 0;
	return window.open('../../Calendar/Popup.aspx?Date=' + ctlDate.value,'CalendarPopup','width=193,height=155,resizable=1,scrollbars=0,left=' + clientX + ',top=' + clientY);
}
</script>
<div id="divMassUpdate">
	<br />
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title=".LBL_MASS_UPDATE_TITLE" Runat="Server" />

	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Button ID="btnUpdate" CommandName="MassUpdate" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_UPDATE") %>' Runat="server" />
		<asp:Button ID="btnDelete" CommandName="MassDelete" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_DELETE") %>' Runat="server" />
	</asp:Panel>

	<script type="text/javascript">
	function MassUpdateChangeAccount(sPARENT_ID, sPARENT_NAME)
	{
		document.getElementById('<%= txtACCOUNT_ID.ClientID   %>').value = sPARENT_ID  ;
		document.getElementById('<%= txtACCOUNT_NAME.ClientID %>').value = sPARENT_NAME;
	}
	function MassUpdateAccountPopup()
	{
		ChangeAccount = MassUpdateChangeAccount;
		return window.open('../../Accounts/Popup.aspx','AccountPopup','width=600,height=400,resizable=1,scrollbars=1');
	}
	</script>
	<asp:Table Width="100%" CellPadding="0" CellSpacing="0" CssClass="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table Width="100%" CellPadding="0" CellSpacing="0" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("ProductTemplates.LBL_DATE_COST_PRICE") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><SplendidCRM:DatePicker ID="ctlDATE_COST_PRICE" Runat="Server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("ProductTemplates.LBL_STATUS") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="lstSTATUS" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("ProductTemplates.LBL_TAX_CLASS") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="lstTAX_CLASS" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("ProductTemplates.LBL_DATE_AVAILABLE") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><SplendidCRM:DatePicker ID="ctlDATE_AVAILABLE" Runat="Server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("ProductTemplates.LBL_SUPPORT_TERM") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="lstSUPPORT_TERM" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("ProductTemplates.LBL_ACCOUNT_NAME") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox     ID="txtACCOUNT_NAME" ReadOnly="True" Runat="server" />
							<asp:HiddenField ID="txtACCOUNT_ID" runat="server" />&nbsp;
							<asp:Button      ID="btnACCOUNT_ID" OnClientClick="MassUpdateAccountPopup(); return false;" Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
