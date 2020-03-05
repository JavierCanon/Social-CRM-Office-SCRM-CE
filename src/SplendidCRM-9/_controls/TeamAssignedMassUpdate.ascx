<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TeamAssignedMassUpdate.ascx.cs" Inherits="SplendidCRM._controls.TeamAssignedMassUpdate" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
%>
<div id="divTeamAssignedMassUpdate">
				<%@ Register TagPrefix="SplendidCRM" Tagname="TeamAssignedPopupScripts" Src="~/_controls/TeamAssignedPopupScripts.ascx" %>
				<SplendidCRM:TeamAssignedPopupScripts ID="ctlTeamAssignedPopupScripts" Runat="Server" />
				<asp:Table Width="100%" CellPadding="0" CellSpacing="0" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label ID="lblASSIGNED_TO" Visible="<%# bShowAssigned %>" Text='<%# L10n.Term(".LBL_ASSIGNED_TO") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox     ID="ASSIGNED_TO"         Visible="<%# bShowAssigned %>" ReadOnly="True" Runat="server" />
							<asp:HiddenField ID="ASSIGNED_USER_ID"    Visible="<%# bShowAssigned %>" runat="server" />&nbsp;
							<asp:Button      ID="btnASSIGNED_USER_ID" Visible="<%# bShowAssigned %>" OnClientClick="UserPopup(); return false;" Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label ID="lblTEAM" Visible="<%# SplendidCRM.Crm.Config.enable_team_management() %>" Text='<%# L10n.Term("Teams.LBL_TEAM") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox     ID="TEAM_NAME"     Visible="<%# SplendidCRM.Crm.Config.enable_team_management() %>" ReadOnly="True" Runat="server" />
							<asp:HiddenField ID="TEAM_ID"       Visible="<%# SplendidCRM.Crm.Config.enable_team_management() %>" runat="server" />&nbsp;
							<asp:Button      ID="btnChangeTeam" Visible="<%# SplendidCRM.Crm.Config.enable_team_management() %>" OnClientClick="TeamPopup(); return false;" Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' CssClass="button" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
</div>
