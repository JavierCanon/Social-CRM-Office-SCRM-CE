<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DetailView.ascx.cs" Inherits="SplendidCRM.Threads.DetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Threads" EnablePrint="true" HelpName="DetailView" EnableHelp="true" Runat="Server" />

	<asp:HyperLink ID="lnkForum" Text='<%# L10n.Term("Threads.LBL_BACK_TO_PARENT") %>' NavigateUrl="~/Forums/" Visible="false" runat="server" />
	<%@ Register TagPrefix="SplendidCRM" Tagname="DetailButtons" Src="_controls/ThreadButtons.ascx" %>
	<SplendidCRM:DetailButtons ID="ctlDetailButtons" Visible="<%# !PrintView %>" Runat="Server" />

	<table ID="tblMain" class="tabDetailView" runat="server">
		<tr>
			<td class="tabDetailViewDF"><asp:Label ID="txtTITLE" Font-Bold="true" runat="server" /></td>
			<td width="10%" class="tabDetailViewDL"><asp:Label ID="txtCREATED_BY" runat="server" />:</td>
			<td width="10%" class="tabDetailViewDL" nowrap><asp:Label ID="txtDATE_ENTERED" runat="server" /></td>
		</tr>
		<tr>
			<td colspan="3" style="background-color: #ffffff; padding-left: 3mm; padding-right: 3mm; ">
				<asp:Literal ID="txtDESCRIPTION" runat="server" />
			</td>
		</tr>
		<tr id="trModified" visible="false" runat="server">
			<td class="tabDetailViewDF">&nbsp;</td>
			<td width="10%" class="tabDetailViewDL" nowrap><%# L10n.Term("Threads.LBL_MODIFIED_BY") %>&nbsp;<asp:Label ID="txtMODIFIED_BY" runat="server" />:</td>
			<td width="10%" class="tabDetailViewDL" nowrap><asp:Label ID="txtDATE_MODIFIED" runat="server" /></td>
		</tr>
	</table>

	<div id="divDetailSubPanel">
		<%@ Register TagPrefix="SplendidCRM" Tagname="Posts" Src="Posts.ascx" %>
		<SplendidCRM:Posts ID="ctlPosts" Runat="Server" />
		<%@ Register TagPrefix="SplendidCRM" Tagname="ThreadView" Src="ThreadView.ascx" %>
		<SplendidCRM:ThreadView ID="ctlThreadView" Runat="Server" />

		<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
	</div>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
