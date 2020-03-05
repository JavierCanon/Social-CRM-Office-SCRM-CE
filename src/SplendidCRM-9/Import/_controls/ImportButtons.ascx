<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ImportButtons.ascx.cs" Inherits="SplendidCRM.Import._controls.ImportButtons" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<asp:Table Width="100%" CellPadding="0" CellSpacing="0" style="padding-bottom: 2px;" runat="server">
	<asp:TableRow>
		<asp:TableCell HorizontalAlign="Left">
			<asp:Button ID="btnRun"     CommandName="Import.Run"     OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term("Import.LBL_RUN_BUTTON_LABEL"    ) + "  " %>' ToolTip='<%# L10n.Term("Import.LBL_RUN_BUTTON_TITLE"    ) %>' AccessKey='<%# L10n.AccessKey("Import.LBL_RUN_BUTTON_KEY"    ) %>' Runat="server" />
			<asp:Button ID="btnPreview" CommandName="Import.Preview" OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term("Import.LBL_PREVIEW_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term("Import.LBL_PREVIEW_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey("Import.LBL_PREVIEW_BUTTON_KEY") %>' Runat="server" />
			<asp:Button ID="btnCancel"  CommandName="Cancel"         OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term(".LBL_CANCEL_BUTTON_LABEL"       ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_CANCEL_BUTTON_TITLE"       ) %>' AccessKey='<%# L10n.AccessKey(".LBL_CANCEL_BUTTON_KEY"       ) %>' Runat="server" />
			<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
		</asp:TableCell>
		<asp:TableCell HorizontalAlign="Right" Wrap="false">
			<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" />
			<asp:Literal Text="&nbsp;" runat="server" />
			<asp:Label Text='<%# L10n.Term(".NTC_REQUIRED") %>' runat="server" />
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
