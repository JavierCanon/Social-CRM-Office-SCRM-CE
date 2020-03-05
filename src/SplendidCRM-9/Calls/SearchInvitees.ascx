<%@ Control Language="c#" AutoEventWireup="false" Codebehind="SearchInvitees.ascx.cs" Inherits="SplendidCRM.Calls.SearchInvitees" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divSearchInvitees">
	<br />
	<h5 CssClass="listViewSubHeadS1"><%= L10n.Term("Calls.LBL_ADD_INVITEE") %></h5>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabSearchView" runat="server">
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" Wrap="false"><%= L10n.Term("Calls.LBL_FIRST_NAME") %>&nbsp;&nbsp;<asp:TextBox ID="txtFIRST_NAME"   CssClass="dataField" size="10" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel" Wrap="false"><%= L10n.Term("Calls.LBL_LAST_NAME" ) %>&nbsp;&nbsp;<asp:TextBox ID="txtLAST_NAME"    CssClass="dataField" size="10" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel" Wrap="false"><%= L10n.Term("Calls.LBL_EMAIL"     ) %>&nbsp;&nbsp;<asp:TextBox ID="txtEMAIL"        CssClass="dataField" size="15" Runat="server" /></asp:TableCell>
						<asp:TableCell HorizontalAlign="Right">
							<asp:Button ID="btnSearch" CommandName="Search" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_SEARCH_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_SEARCH_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SEARCH_BUTTON_KEY") %>' Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<%= Utils.RegisterEnterKeyPress(txtFIRST_NAME.ClientID, btnSearch.ClientID) %>
	<%= Utils.RegisterEnterKeyPress(txtLAST_NAME.ClientID , btnSearch.ClientID) %>
	<%= Utils.RegisterEnterKeyPress(txtEMAIL.ClientID     , btnSearch.ClientID) %>
</div>
