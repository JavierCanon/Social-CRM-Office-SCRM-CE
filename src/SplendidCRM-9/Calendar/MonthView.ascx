<%@ Control CodeBehind="MonthView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Calendar.MonthView" %>
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
<div id="divDetailView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Calendar" Title="Calendar.LBL_MODULE_TITLE" EnableModuleLabel="false" EnablePrint="true" EnableHelp="true" Runat="Server" />
	
	<asp:Table SkinID="tabFrame" runat="server">
		<asp:TableRow>
			<asp:TableCell VerticalAlign="Top">
				<%@ Register TagPrefix="SplendidCRM" Tagname="CalendarHeader" Src="CalendarHeader.ascx" %>
				<SplendidCRM:CalendarHeader ID="ctlCalendarHeader" ActiveTab="Month" Runat="Server" />
				<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
				
				<asp:Calendar ID="ctlCalendar" Width="100%" CssClass="monthBox" ShowGridLines="true" 
					SelectionMode="Day" OnSelectionChanged="ctlCalendar_SelectionChanged" 
					OnDayRender="ctlCalendar_DayRender" OnVisibleMonthChanged="ctlCalendar_VisibleMonthChanged"
					Runat="server">
					<TitleStyle         CssClass="monthHeader monthHeaderH3"   />
					<NextPrevStyle      CssClass="monthHeader monthFooterPrev" />
					<DayHeaderStyle     CssClass="monthCalBodyTHDay"           />
					<DayStyle           CssClass="monthCalBodyWeekDay monthCalBodyWeekDayDateLink"      VerticalAlign="Top" />
					<TodayDayStyle      CssClass="monthCalBodyTodayWeekDay monthCalBodyWeekDayDateLink" VerticalAlign="Top" />
					<WeekendDayStyle    CssClass="monthCalBodyWeekEnd monthCalBodyWeekDayDateLink"      VerticalAlign="Top" />
					<OtherMonthDayStyle CssClass="monthCalBodyWeekDay" ForeColor="#fafafa"              VerticalAlign="Top" />
				</asp:Calendar>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>
