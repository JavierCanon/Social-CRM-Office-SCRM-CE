<%@ Control Language="c#" AutoEventWireup="false" Codebehind="InboundButtons.ascx.cs" Inherits="SplendidCRM.Emails._controls.InboundButtons" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
</script>
<script type="text/javascript">
function ConfirmDelete()
{
	return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>');
}
</script>
<asp:Table Width="100%" CellPadding="0" CellSpacing="0" style="padding-bottom: 2px;" runat="server">
	<asp:TableRow>
		<asp:TableCell HorizontalAlign="Left">
			<asp:Button ID="btnForward"   CommandName="Forward"   OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term("Emails.LBL_BUTTON_FORWARD"       ) + "  " %>' ToolTip='<%# L10n.Term("Emails.LBL_BUTTON_FORWARD_TITLE" ) %>' AccessKey='<%# L10n.AccessKey("Emails.LBL_BUTTON_REPLY_KEY"  ) %>' Runat="server" />
			<asp:Button ID="btnReply"     CommandName="Reply"     OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term("Emails.LBL_BUTTON_REPLY"         ) + "  " %>' ToolTip='<%# L10n.Term("Emails.LBL_BUTTON_REPLY_TITLE"   ) %>' AccessKey='<%# L10n.AccessKey("Emails.LBL_BUTTON_FORWARD_KEY") %>' Runat="server" />
			<asp:Button ID="btnDelete"    CommandName="Delete"    OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# " "  + L10n.Term(".LBL_DELETE_BUTTON_LABEL"        ) + " "  %>' ToolTip='<%# L10n.Term(".LBL_DELETE_BUTTON_TITLE"        ) %>' AccessKey='<%# L10n.AccessKey(".LBL_DELETE_BUTTON_KEY"       ) %>' OnClientClick='return ConfirmDelete();' Runat="server" />
			<asp:Button ID="btnShowRaw"   CommandName="ShowRaw"   OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term("Emails.LBL_BUTTON_RAW_LABEL"     ) + "  " %>' ToolTip='<%# L10n.Term("Emails.LBL_BUTTON_RAW_TITLE"     ) %>' AccessKey='<%# L10n.AccessKey("Emails..LBL_BUTTON_RAW_KEY"   ) %>' Visible="false" Runat="server" />
			<asp:Button ID="btnHideRaw"   CommandName="HideRaw"   OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term("Emails.LBL_BUTTON_RAW_LABEL_HIDE") + "  " %>' ToolTip='<%# L10n.Term("Emails.LBL_BUTTON_RAW_LABEL_HIDE") %>' AccessKey='<%# L10n.AccessKey("Emails..LBL_BUTTON_RAW_KEY"   ) %>' Visible="false" Runat="server" />
			<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
		</asp:TableCell>
		<asp:TableCell HorizontalAlign="Right" Wrap="false">
			<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" />
			<asp:Literal Text="&nbsp;" runat="server" />
			<asp:Label Text='<%# L10n.Term(".NTC_REQUIRED") %>' runat="server" />
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
