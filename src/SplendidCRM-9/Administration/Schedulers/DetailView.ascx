<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DetailView.ascx.cs" Inherits="SplendidCRM.Administration.Schedulers.DetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Schedulers" Title="Schedulers.LBL_MODULE_TITLE" EnablePrint="true" HelpName="DetailView" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DetailButtons" Src="~/_controls/DetailButtons.ascx" %>
	<SplendidCRM:DetailButtons ID="ctlDetailButtons" Visible="<%# !PrintView %>" Runat="Server" />

	<asp:Table Width="100%" CellPadding="0" CellSpacing="0" CssClass="tabDetailView" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term("Schedulers.LBL_JOB"   ) %></asp:TableCell>
			<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="JOB"    Runat="server" /></asp:TableCell>
			<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term("Schedulers.LBL_STATUS") %></asp:TableCell>
			<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="STATUS" Runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term("Schedulers.LBL_DATE_TIME_START") %></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="DATE_TIME_START" Runat="server" /></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term("Schedulers.LBL_TIME_FROM"      ) %></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="TIME_FROM"       Runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term("Schedulers.LBL_DATE_TIME_END") %></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="DATE_TIME_END" Runat="server" /></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term("Schedulers.LBL_TIME_TO"      ) %></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="TIME_TO"       Runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term("Schedulers.LBL_LAST_RUN") %></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="LAST_RUN"     Runat="server" /></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term("Schedulers.LBL_INTERVAL") %></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="JOB_INTERVAL" Runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term("Schedulers.LBL_CATCH_UP"    ) %></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="CATCH_UP"     Runat="server" /></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL">&nbsp;</asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF">&nbsp;</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term(".LBL_DATE_ENTERED" ) %></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="DATE_ENTERED"  Runat="server" /></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDL"><%= L10n.Term(".LBL_DATE_MODIFIED") %></asp:TableCell>
			<asp:TableCell VerticalAlign="Top" CssClass="tabDetailViewDF"><asp:Label ID="DATE_MODIFIED" Runat="server" /></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
