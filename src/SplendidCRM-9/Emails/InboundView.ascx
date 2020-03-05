<%@ Control Language="c#" AutoEventWireup="false" Codebehind="InboundView.ascx.cs" Inherits="SplendidCRM.Emails.InboundView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
%>
<div id="divDetailView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Emails" EnablePrint="true" HelpName="DetailView" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="InboundButtons" Src="_controls/InboundButtons.ascx" %>
	<SplendidCRM:InboundButtons ID="ctlInboundButtons" Visible="<%# !PrintView %>" Runat="Server" />

	<table ID="tblMain" class="tabDetailView" runat="server">
	</table>
	<table ID="tblAttachments" class="tabDetailView" runat="server">
		<tr>
			<td width="15%" class="tabDetailViewDL" valign="top">
				<%= L10n.Term("Emails.LBL_ATTACHMENTS") %>
			</td>
			<td colspan="3" class="tabDetailViewDF" valign="top">
				<asp:Repeater id="ctlAttachments" runat="server">
					<HeaderTemplate />
					<ItemTemplate>
							<asp:HyperLink NavigateUrl='<%# "~/Notes/Attachment.aspx?ID=" + DataBinder.Eval(Container.DataItem, "NOTE_ATTACHMENT_ID") %>' Target="_blank" Runat="server" >
							<%# DataBinder.Eval(Container.DataItem, "FILENAME") %>
							</asp:HyperLink><br>
					</ItemTemplate>
					<FooterTemplate />
				</asp:Repeater>
			</td>
		</tr>
	</table>

	<div id="divDetailSubPanel">
		<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
	</div>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
