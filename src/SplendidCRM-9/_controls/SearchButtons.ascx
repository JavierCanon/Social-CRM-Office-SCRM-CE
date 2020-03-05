<%@ Control Language="c#" AutoEventWireup="false" Codebehind="SearchButtons.ascx.cs" Inherits="SplendidCRM._controls.SearchButtons" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
<asp:Panel ID="pnlSearchButtons" CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
	<asp:Table ID="tblSearchButtons" Width="100%" CellPadding="0" CellSpacing="0" style="padding-top: 8px;" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Button ID="btnSearch" CommandName="Search" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_SEARCH_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_SEARCH_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SEARCH_BUTTON_KEY") %>' Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
				<asp:Button ID="btnClear"  CommandName="Clear"  OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_CLEAR_BUTTON_LABEL" ) %>' ToolTip='<%# L10n.Term(".LBL_CLEAR_BUTTON_TITLE" ) %>' AccessKey='<%# L10n.AccessKey(".LBL_CLEAR_BUTTON_KEY" ) %>' Runat="server" />
				<asp:Label CssClass="white-space" Text="&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;" Visible="false" runat="server" />
				<asp:Label Font-Bold="true" Text='<%# L10n.Term(".LBL_SAVED_SEARCH_SHORTCUT" ) %>' Visible="false" runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</asp:Panel>
