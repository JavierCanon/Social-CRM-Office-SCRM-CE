<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ExportHeader.ascx.cs" Inherits="SplendidCRM._controls.ExportHeader" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<asp:Table SkinID="tabFrame" CssClass="h3Row" runat="server">
		<asp:TableRow>
			<asp:TableCell Wrap="false">
				<h3><asp:Image SkinID="h3Arrow" Runat="server" />&nbsp;<asp:Label Text='<%# L10n.Term(sTitle) %>' runat="server" /></h3>
			</asp:TableCell>
			<asp:TableCell HorizontalAlign="Right">
				<div id="divExport" Visible='<%# !IsMobile && !PrintView && SplendidCRM.Security.GetUserAccess(sModule, "export") >= 0 %>' Runat="server">
					<asp:DropDownList ID="lstEXPORT_RANGE" Runat="server" />
					<asp:DropDownList ID="lstEXPORT_FORMAT" Runat="server" />
					<asp:Button     ID="btnExport" CommandName="Export" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_EXPORT_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_EXPORT_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_EXPORT_BUTTON_KEY") %>' Runat="server" />
					<asp:LinkButton ID="lnkExport" CommandName="Export" OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LBL_EXPORT_BUTTON_TITLE") %>' Visible="false" Runat="server" />
				</div>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
