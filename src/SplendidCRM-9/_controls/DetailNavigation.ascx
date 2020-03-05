<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DetailNavigation.ascx.cs" Inherits="SplendidCRM._controls.DetailNavigation" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005-2008 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
</script>
<div id="divModuleHeader">
	<script type="text/javascript">
	function PopupAudit()
	{
		window.open('<%= Application["rootURL"] %>Audit/Popup.aspx?ID=<%= gID %>&Module=<%= sModule %>','Audit','width=600,height=400,status=0,resizable=1,scrollbars=1,toolbar=0,location=0');
		return false;
	}
	</script>
	<asp:Table Width="100%" CellPadding="0" CellSpacing="0" CssClass="" runat="server">
		<asp:TableRow>
			<asp:TableCell CssClass="listViewPaginationTdS1">
				<asp:LinkButton ID="lnkViewChangeLog" OnClientClick="PopupAudit(); return false;" Text='<%# L10n.Term(".LNK_VIEW_CHANGE_LOG") %>' CssClass="listViewPaginationLinkS1" runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="listViewPaginationTdS1" HorizontalAlign="Right">
				<asp:HyperLink ID="lnkReturnToList" Text='<%# L10n.Term(".LNK_LIST_RETURN") %>' CssClass="listViewPaginationLinkS1" runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
