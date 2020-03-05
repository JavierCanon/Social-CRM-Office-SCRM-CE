<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MassUpdate.ascx.cs" Inherits="SplendidCRM.Bugs.MassUpdate" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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

	<asp:Table Width="100%" CellPadding="0" CellSpacing="0" CssClass="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<%@ Register TagPrefix="SplendidCRM" Tagname="TeamAssignedMassUpdate" Src="~/_controls/TeamAssignedMassUpdate.ascx" %>
				<SplendidCRM:TeamAssignedMassUpdate ID="ctlTeamAssignedMassUpdate" Runat="Server" />
				<asp:Table Width="100%" CellPadding="0" CellSpacing="0" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Bugs.LBL_STATUS") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="lstSTATUS" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Bugs.LBL_PRIORITY") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="lstPRIORITY" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Bugs.LBL_RESOLUTION") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="lstRESOLUTION" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Bugs.LBL_TYPE") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="lstTYPE" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Bugs.LBL_SOURCE") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="lstSOURCE" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("Bugs.LBL_PRODUCT_CATEGORY") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="lstPRODUCT_CATEGORY" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
