<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DetailView.ascx.cs" Inherits="SplendidCRM.Feeds.DetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
%>
<div id="divDetailView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Feeds" EnablePrint="true" HelpName="DetailView" EnableHelp="true" Runat="Server" />

	<table width="100%" border="0" cellpadding="0" cellspacing="0" Visible="<%# !PrintView %>" runat="server">
		<tr>
			<td style="padding-bottom: 2px;">
				<asp:Button ID="btnAdd"    CommandName="Add"    OnCommand="Page_Command" Visible="false" CssClass="button" Text='<%# "  " + L10n.Term("Feeds.LBL_ADD_FAV_BUTTON_LABEL"   ) + "  " %>' ToolTip='<%# L10n.Term("Feeds.LBL_ADD_FAV_BUTTON_TITLE"   ) %>' AccessKey='<%# L10n.AccessKey(".LBL_EDIT_BUTTON_KEY"  ) %>' Runat="server" />
				<asp:Button ID="btnDelete" CommandName="Delete" OnCommand="Page_Command" Visible="false" CssClass="button" Text='<%# " "  + L10n.Term("Feeds.LBL_DELETE_FAV_BUTTON_LABEL") + " "  %>' ToolTip='<%# L10n.Term("Feeds.LBL_DELETE_FAV_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_DELETE_BUTTON_KEY") %>' Runat="server" />
				<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
			</td>
		</tr>
	</table>

	<%@ Register TagPrefix="SplendidCRM" Tagname="FeedDetailView" Src="FeedDetailView.ascx" %>
	<SplendidCRM:FeedDetailView ID="ctlFeedDetailView" Runat="Server" />
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
