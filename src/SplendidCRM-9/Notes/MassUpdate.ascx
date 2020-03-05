<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MassUpdate.ascx.cs" Inherits="SplendidCRM.Notes.MassUpdate" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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

	<%@ Register TagPrefix="SplendidCRM" Tagname="ParentPopupScripts" Src="~/_controls/ParentPopupScripts.ascx" %>
	<SplendidCRM:ParentPopupScripts ID="ctlParentPopupScripts" ListField="lstPARENT_TYPE" NameField="txtPARENT_NAME" HiddenField="txtPARENT_ID" Runat="Server" />
	<script type="text/javascript">
	function MassUpdateChangeContact(sPARENT_ID, sPARENT_NAME)
	{
		document.getElementById('<%= txtCONTACT_ID.ClientID   %>').value = sPARENT_ID  ;
		document.getElementById('<%= txtCONTACT_NAME.ClientID %>').value = sPARENT_NAME;
	}
	function MassUpdateContactPopup()
	{
		ChangeContact = MassUpdateChangeContact;
		return window.open('../Contacts/Popup.aspx','ContactPopup','width=600,height=400,resizable=1,scrollbars=1');
	}
	</script>
	<asp:Table Width="100%" CellPadding="0" CellSpacing="0" CssClass="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<%@ Register TagPrefix="SplendidCRM" Tagname="TeamAssignedMassUpdate" Src="~/_controls/TeamAssignedMassUpdate.ascx" %>
				<SplendidCRM:TeamAssignedMassUpdate ID="ctlTeamAssignedMassUpdate" ShowAssigned="false" Runat="Server" />
				<asp:Table Width="100%" CellPadding="0" CellSpacing="0" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:DropDownList ID="lstPARENT_TYPE" DataValueField="NAME" DataTextField="DISPLAY_NAME" onChange="ChangeParentType();" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox     ID="txtPARENT_NAME" ReadOnly="True" Runat="server" />
							<asp:HiddenField ID="txtPARENT_ID" runat="server" />&nbsp;
							<asp:Button      ID="btnPARENT_ID" OnClientClick="ParentPopup(); return false;" Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Contacts.LBL_CONTACT") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox     ID="txtCONTACT_NAME" ReadOnly="True" Runat="server" />
							<asp:HiddenField ID="txtCONTACT_ID" runat="server" />&nbsp;
							<asp:Button      ID="btnCONTACT_ID" OnClientClick="MassUpdateContactPopup(); return false;" Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
