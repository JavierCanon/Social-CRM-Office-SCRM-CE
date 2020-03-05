<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ChartHeader.ascx.cs" Inherits="SplendidCRM._controls.ChartHeader" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
<asp:Table Width="100%" BorderWidth="0" CellPadding="0" CellSpacing="0" CssClass="h3Row" runat="server">
	<asp:TableRow>
		<asp:TableCell Wrap="false">
			<h3><asp:Image SkinID="h3Arrow" Runat="server" />&nbsp;<asp:Label Text='<%# L10n.Term(sTitle) %>' runat="server" /></h3>
		</asp:TableCell>
		<asp:TableCell HorizontalAlign="Right" Wrap="false">
			<asp:ImageButton CommandName="Refresh" OnCommand="Page_Command" CssClass="chartToolsLink" AlternateText='<%# L10n.Term("Dashboard.LBL_REFRESH") %>' SkinID="refresh" ImageAlign="AbsMiddle" Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
			<asp:LinkButton  CommandName="Refresh" OnCommand="Page_Command" CssClass="chartToolsLink"          Text='<%# L10n.Term("Dashboard.LBL_REFRESH") %>' Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
			<span onclick="toggleDisplay('<%= DivEditName %>'); return false;">
				<asp:ImageButton CommandName="Edit" OnCommand="Page_Command" CssClass="chartToolsLink" AlternateText='<%# L10n.Term("Dashboard.LBL_EDIT"  ) %>' SkinID="edit"    ImageAlign="AbsMiddle" Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
				<asp:LinkButton  CommandName="Edit" OnCommand="Page_Command" CssClass="chartToolsLink"          Text='<%# L10n.Term("Dashboard.LBL_EDIT"  ) %>' Runat="server" />
			</span>
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
