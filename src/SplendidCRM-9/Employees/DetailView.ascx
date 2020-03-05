<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DetailView.ascx.cs" Inherits="SplendidCRM.Employees.DetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Employees" EnablePrint="true" HelpName="DetailView" EnableHelp="true" Runat="Server" />

	<table width="100%" border="0" cellpadding="0" cellspacing="0" runat="server">
		<tr>
			<td style="padding-bottom: 2px;">
				<asp:Button ID="btnEdit"      CommandName="Edit"      OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_EDIT_BUTTON_LABEL"     ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_EDIT_BUTTON_TITLE"     ) %>' AccessKey='<%# L10n.AccessKey(".LBL_EDIT_BUTTON_KEY"     ) %>' Runat="server" />
				<asp:Button ID="btnDuplicate" CommandName="Duplicate" OnCommand="Page_Command" CssClass="button" Text='<%# " "  + L10n.Term(".LBL_DUPLICATE_BUTTON_LABEL") + " "  %>' ToolTip='<%# L10n.Term(".LBL_DUPLICATE_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_DUPLICATE_BUTTON_KEY") %>' Runat="server" />
				<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
			</td>
			<td align="right">
				<asp:LinkButton ID="btnReset" CommandName="Employees.ResetDefaults" OnCommand="Page_Command" CssClass="listViewTdToolsS1" Runat="server"><%# L10n.Term("Employees.LBL_RESET_PREFERENCES") %></asp:LinkButton>
			</td>
		</tr>
	</table>

	<table ID="tblMain" class="tabDetailView" runat="server">
		<tr>
			<th colspan="4" class="dataLabel"><h4><asp:Label Text='<%# L10n.Term("Employees.LBL_EMPLOYEE_INFORMATION") %>' runat="server" /></h4></th>
		</tr>
	</table>

	<div id="divDetailSubPanel">
		<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
	</div>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
