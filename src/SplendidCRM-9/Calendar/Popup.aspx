<%@ Page language="c#" Codebehind="Popup.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Calendar.Popup" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
</script>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" > 
<html>
<head runat="server">
	<title>Calendar</title>
	<base target="_self" />
</head>
<script type="text/javascript">
function SelectDate(sDATE)
{
	if ( window.opener != null && window.opener.ChangeDate != null )
	{
		window.opener.ChangeDate(sDATE);
		window.close();
	}
	else
	{
		alert('Original window has closed.  Date cannot be set.');
	}
}
</script>
<body leftmargin="0" topmargin="0" rightmargin="0" bottommargin="0">
	<form id="frm" method="post" runat="server">
		<asp:Calendar ID="ctlCalendar" OnSelectionChanged="ctlCalendar_SelectionChanged" CssClass="Calendar" Runat="server">
			<TitleStyle         CssClass="CalendarTitle" />
			<DayHeaderStyle     CssClass="CalendarDayHeader" />
			<DayStyle           CssClass="CalendarDay" />
			<OtherMonthDayStyle CssClass="CalendarOtherMonthDay" />
			<NextPrevStyle      CssClass="" />
			<SelectedDayStyle   CssClass="" />
			<SelectorStyle      CssClass="" />
			<TodayDayStyle      CssClass="CalendarToday" />
			<WeekendDayStyle    CssClass="CalendarWeekendDay" />
		</asp:Calendar>
	</form>
</body>
</html>
