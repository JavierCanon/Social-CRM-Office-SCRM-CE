<%@ Control Language="c#" AutoEventWireup="false" Codebehind="CampaignButtons.ascx.cs" Inherits="SplendidCRM.Campaigns._controls.CampaignButtons" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
			<asp:Button ID="btnEdit"       CommandName="Edit"       OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term(".LBL_EDIT_BUTTON_LABEL"                 ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_EDIT_BUTTON_TITLE"                 ) %>' AccessKey='<%# L10n.AccessKey(".LBL_EDIT_BUTTON_KEY"          ) %>' Runat="server" />
			<asp:Button ID="btnDuplicate"  CommandName="Duplicate"  OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# " "  + L10n.Term(".LBL_DUPLICATE_BUTTON_LABEL"            ) + " "  %>' ToolTip='<%# L10n.Term(".LBL_DUPLICATE_BUTTON_TITLE"            ) %>' AccessKey='<%# L10n.AccessKey(".LBL_DUPLICATE_BUTTON_KEY"     ) %>' Runat="server" />
			<asp:Button ID="btnDelete"     CommandName="Delete"     OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# " "  + L10n.Term(".LBL_DELETE_BUTTON_LABEL"               ) + " "  %>' ToolTip='<%# L10n.Term(".LBL_DELETE_BUTTON_TITLE"               ) %>' AccessKey='<%# L10n.AccessKey(".LBL_DELETE_BUTTON_KEY"        ) %>' OnClientClick='return ConfirmDelete();' Runat="server" />
			<asp:Button ID="btnSendTest"   CommandName="SendTest"   OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term("Campaigns.LBL_TEST_BUTTON_LABEL"        ) + "  " %>' ToolTip='<%# L10n.Term("Campaigns.LBL_TEST_BUTTON_TITLE"        ) %>' AccessKey='<%# L10n.AccessKey("Campaigns.LBL_TEST_BUTTON_KEY" ) %>' Visible="false" Runat="server" />
			<asp:Button ID="btnSendEmails" CommandName="SendEmail"  OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term("Campaigns.LBL_QUEUE_BUTTON_LABEL"       ) + "  " %>' ToolTip='<%# L10n.Term("Campaigns.LBL_QUEUE_BUTTON_TITLE"       ) %>' AccessKey='<%# L10n.AccessKey("Campaigns.LBL_QUEUE_BUTTON_KEY") %>' Visible="false" Runat="server" />
			<asp:Button ID="btnMailMerge"  CommandName="MailMerge"  OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term(".LBL_MAILMERGE"                         ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_MAILMERGE"                         ) %>' Visible="false" Runat="server" />
			<asp:Button ID="btnDeleteTest" CommandName="DeleteTest" OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term("Campaigns.LBL_TRACK_DELETE_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term("Campaigns.LBL_TRACK_DELETE_BUTTON_TITLE") %>' Visible="false" Runat="server" />
			<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
		</asp:TableCell>
		<asp:TableCell HorizontalAlign="Right" Wrap="false">
			<asp:HyperLink ID="lnkViewTrack"   NavigateUrl='<%# "~/Campaigns/track.aspx?ID=" + Sql.ToString(Request["ID"]) %>' Text='<%# L10n.Term("Campaigns.LBL_TRACK_BUTTON_LABEL"    ) %>' ToolTip='<%# L10n.Term("Campaigns.LBL_TRACK_BUTTON_TITLE"    ) %>' CssClass="listViewTdLinkS1" style="margin-left: 10px;" runat="server" />
			<asp:HyperLink ID="lnkViewDetails" NavigateUrl='<%# "~/Campaigns/view.aspx?ID="  + Sql.ToString(Request["ID"]) %>' Text='<%# L10n.Term("Campaigns.LBL_TODETAIL_BUTTON_LABEL" ) %>' ToolTip='<%# L10n.Term("Campaigns.LBL_TODETAIL_BUTTON_TITLE" ) %>' CssClass="listViewTdLinkS1" style="margin-left: 10px;" runat="server" />
			<asp:HyperLink ID="lnkViewROI"     NavigateUrl='<%# "~/Campaigns/roi.aspx?ID="   + Sql.ToString(Request["ID"]) %>' Text='<%# L10n.Term("Campaigns.LBL_TRACK_ROI_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term("Campaigns.LBL_TRACK_ROI_BUTTON_LABEL") %>' CssClass="listViewTdLinkS1" style="margin-left: 10px;" runat="server" />
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
