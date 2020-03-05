<%@ Page language="c#" MasterPageFile="~/PopupView.Master" Codebehind="Popup.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Administration.RenameTabs.Popup" %>
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
<script type="text/javascript">
function ChangeItem()
{
	if ( window.opener != null && window.opener.ChangeItem != null )
	{
		window.opener.ChangeItem(document.getElementById('<%= txtKEY.ClientID %>').value, document.getElementById('<%= txtVALUE.ClientID %>').value);
		window.close();
	}
}
function Cancel()
{
	window.close();
}
// 08/30/2006 Paul.  Fix onload to support Firefox. 
window.onload = function()
{
	document.getElementById('<%= txtKEY.ClientID %>').focus();
}
</script>
<asp:Table SkinID="tabForm" runat="server">
	<asp:TableRow>
		<asp:TableCell>
			<table width="100%" border="0" cellspacing="0" cellpadding="0">
				<tr>
					<td class="dataLabel" noWrap><asp:Label Text='<%# L10n.Term("Dropdown.LBL_KEY"  ) %>' runat="server" />&nbsp;&nbsp;<asp:TextBox ID="txtKEY"   CssClass="dataField" Runat="server" /></td>
					<td class="dataLabel" noWrap><asp:Label Text='<%# L10n.Term("Dropdown.LBL_VALUE") %>' runat="server" />&nbsp;&nbsp;<asp:TextBox ID="txtVALUE" CssClass="dataField" Runat="server" /></td>
					<td align="right">
						<asp:Button ID="btnPopupSelect" OnClientClick="ChangeItem(); return false;" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SAVE_BUTTON_LABEL"  ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_SAVE_BUTTON_LABEL"  ) %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_BUTTON_KEY"  ) %>' runat="server" />
						<asp:Button ID="btnPopupCancel" OnClientClick="Cancel(); return false;"     CssClass="button" Text='<%# "  " + L10n.Term(".LBL_CANCEL_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_CANCEL_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_CANCEL_BUTTON_KEY") %>' runat="server" />
					</td>
				</tr>
			</table>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
</asp:Content>
