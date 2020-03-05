<%@ Control CodeBehind="DetailView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Help.DetailView" %>
<!--
/**********************************************************************************************************************
 * The contents of this file are subject to the SugarCRM Public License Version 1.1.3 ("License"); You may not use this 
 * file except in compliance with the * License. You may obtain a copy of the License at http://www.sugarcrm.com/SPL
 * Software distributed under the License is distributed on an "AS IS" basis, * WITHOUT WARRANTY OF ANY KIND, either 
 * express or implied.  See the License * for the specific language governing rights and limitations under the License.
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
-->
<div id="divDetailView">
	<script type="text/javascript">
	function createBookmarkLink()
	{
		var sTitle = '<%= SplendidCRM.Sql.EscapeJavaScript(sPageTitle) %>';
		var sURL = window.location.href;
		if ( document.all )
			window.external.AddFavorite(sURL, sTitle);
		else if ( window.sidebar )
			window.sidebar.addPanel(sTitle, sURL, '');
	}
	</script>
	<asp:Table width="100%" border="0" cellspacing="2" cellpadding="0" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<%@ Register TagPrefix="SplendidCRM" Tagname="DetailButtons" Src="~/_controls/DetailButtons.ascx" %>
				<SplendidCRM:DetailButtons ID="ctlDetailButtons" Visible="<%# !PrintView %>" ShowDuplicate="false" ShowDelete="false" Runat="Server" />
			</asp:TableCell>
			<asp:TableCell HorizontalAlign="Right">
				<asp:HyperLink ID="lnkPRINT" NavigateUrl="javascript:window.print();" Text='<%# L10n.Term("Help.LBL_HELP_PRINT") %>' runat="server" />
				-
				<asp:HyperLink ID="lnkEMAIL" NavigateUrl="#" Text='<%# L10n.Term("Help.LBL_HELP_EMAIL") %>' runat="server" />
				-
				<asp:HyperLink ID="lnkBOOKMARK" NavigateUrl="#" onmousedown="createBookmarkLink()" Text='<%# L10n.Term("Help.LBL_HELP_BOOKMARK") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Literal ID="lblDISPLAY_TEXT" runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>

	<script type="text/javascript">
		document.getElementById('<%= new DynamicControl(this, "lnkEMAIL").ClientID %>').href = 'mailto:?subject=<%= Server.HtmlEncode(sPageTitle) %>&body=' + escape(window.location.href);
	</script>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
