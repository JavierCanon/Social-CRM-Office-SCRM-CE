<%@ Control Language="c#" AutoEventWireup="false" Codebehind="CalendarHeader.ascx.cs" Inherits="SplendidCRM.Calendar.CalendarHeader" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005-2008 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
%>
<asp:Table SkinID="tabDetailViewButtons" Visible="<%# !PrintView %>" runat="server">
	<asp:TableRow>
		<asp:TableCell>
			<asp:Button ID="btnDay"    CommandName="Day.Current"    OnCommand="Page_Command" CssClass="button" Text='<%# " " + L10n.Term("Calendar.LBL_DAY"   ) + " " %>' ToolTip='<%# L10n.Term("Calendar.LBL_DAY"   ) %>' Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
			<asp:Button ID="btnWeek"   CommandName="Week.Current"   OnCommand="Page_Command" CssClass="button" Text='<%# " " + L10n.Term("Calendar.LBL_WEEK"  ) + " " %>' ToolTip='<%# L10n.Term("Calendar.LBL_WEEK"  ) %>' Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
			<asp:Button ID="btnMonth"  CommandName="Month.Current"  OnCommand="Page_Command" CssClass="button" Text='<%# " " + L10n.Term("Calendar.LBL_MONTH" ) + " " %>' ToolTip='<%# L10n.Term("Calendar.LBL_MONTH" ) %>' Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
			<asp:Button ID="btnYear"   CommandName="Year.Current"   OnCommand="Page_Command" CssClass="button" Text='<%# " " + L10n.Term("Calendar.LBL_YEAR"  ) + " " %>' ToolTip='<%# L10n.Term("Calendar.LBL_YEAR"  ) %>' Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
			<asp:Button ID="btnShared" CommandName="Shared.Current" OnCommand="Page_Command" CssClass="button" Text='<%# " " + L10n.Term("Calendar.LBL_SHARED") + " " %>' ToolTip='<%# L10n.Term("Calendar.LBL_SHARED") %>' Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
