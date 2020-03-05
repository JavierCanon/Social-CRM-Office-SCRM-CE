<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ImportDefaultsView.ascx.cs" Inherits="SplendidCRM.Accounts.ImportDefaultsView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divDefaultsView">
	<p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="TeamAssignedPopupScripts" Src="~/_controls/TeamAssignedPopupScripts.ascx" %>
	<SplendidCRM:TeamAssignedPopupScripts ID="ctlTeamAssignedPopupScripts" Runat="Server" />
	<script type="text/javascript">
	function ChangeAccount(sPARENT_ID, sPARENT_NAME)
	{
		document.getElementById('<%= new DynamicControl(this, "PARENT_ID"  ).ClientID %>').value = sPARENT_ID  ;
		document.getElementById('<%= new DynamicControl(this, "PARENT_NAME").ClientID %>').value = sPARENT_NAME;
	}
	function AccountPopup()
	{
		return window.open('../Accounts/Popup.aspx','AccountPopup','width=600,height=400,resizable=1,scrollbars=1');
	}
	</script>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<table ID="tblMain" class="tabEditView" runat="server">
					<tr>
						<th colspan="4"><h4><asp:Label Text='<%# L10n.Term("Accounts.LBL_ACCOUNT_INFORMATION") %>' runat="server" /></h4></th>
					</tr>
				</table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	<p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<table ID="tblAddress" class="tabEditView" runat="server">
					<tr>
						<th colspan="5"><h4><asp:Label Text='<%# L10n.Term("Accounts.LBL_ADDRESS_INFORMATION") %>' runat="server" /></h4></th>
					</tr>
				</table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	<p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<table ID="tblDescription" class="tabEditView" runat="server">
					<tr>
						<th colspan="2"><h4><asp:Label Text='<%# L10n.Term("Accounts.LBL_DESCRIPTION_INFORMATION") %>' runat="server" /></h4></th>
					</tr>
				</table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>

	<script type="text/javascript">
	function copyAddressRight()
	{
		document.getElementById('<%= new DynamicControl(this, "SHIPPING_ADDRESS_STREET"    ).ClientID %>').value = document.getElementById('<%= new DynamicControl(this, "BILLING_ADDRESS_STREET"    ).ClientID %>').value;
		document.getElementById('<%= new DynamicControl(this, "SHIPPING_ADDRESS_CITY"      ).ClientID %>').value = document.getElementById('<%= new DynamicControl(this, "BILLING_ADDRESS_CITY"      ).ClientID %>').value;
		document.getElementById('<%= new DynamicControl(this, "SHIPPING_ADDRESS_STATE"     ).ClientID %>').value = document.getElementById('<%= new DynamicControl(this, "BILLING_ADDRESS_STATE"     ).ClientID %>').value;
		document.getElementById('<%= new DynamicControl(this, "SHIPPING_ADDRESS_POSTALCODE").ClientID %>').value = document.getElementById('<%= new DynamicControl(this, "BILLING_ADDRESS_POSTALCODE").ClientID %>').value;
		document.getElementById('<%= new DynamicControl(this, "SHIPPING_ADDRESS_COUNTRY"   ).ClientID %>').value = document.getElementById('<%= new DynamicControl(this, "BILLING_ADDRESS_COUNTRY"   ).ClientID %>').value;
		return true;
	}
	function copyAddressLeft()
	{
		document.getElementById('<%= new DynamicControl(this, "BILLING_ADDRESS_STREET"    ).ClientID %>').value = document.getElementById('<%= new DynamicControl(this, "SHIPPING_ADDRESS_STREET"    ).ClientID %>').value;
		document.getElementById('<%= new DynamicControl(this, "BILLING_ADDRESS_CITY"      ).ClientID %>').value = document.getElementById('<%= new DynamicControl(this, "SHIPPING_ADDRESS_CITY"      ).ClientID %>').value;
		document.getElementById('<%= new DynamicControl(this, "BILLING_ADDRESS_STATE"     ).ClientID %>').value = document.getElementById('<%= new DynamicControl(this, "SHIPPING_ADDRESS_STATE"     ).ClientID %>').value;
		document.getElementById('<%= new DynamicControl(this, "BILLING_ADDRESS_POSTALCODE").ClientID %>').value = document.getElementById('<%= new DynamicControl(this, "SHIPPING_ADDRESS_POSTALCODE").ClientID %>').value;
		document.getElementById('<%= new DynamicControl(this, "BILLING_ADDRESS_COUNTRY"   ).ClientID %>').value = document.getElementById('<%= new DynamicControl(this, "SHIPPING_ADDRESS_COUNTRY"   ).ClientID %>').value;
		return true;
	}
	</script>
</div>
