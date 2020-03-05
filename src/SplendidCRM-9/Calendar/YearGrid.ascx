<%@ Control CodeBehind="YearGrid.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Calendar.YearGrid" %>
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
<div id="divYear">
	<%@ Register TagPrefix="SplendidCRM" Tagname="CalendarHeader" Src="CalendarHeader.ascx" %>
	<SplendidCRM:CalendarHeader ID="ctlCalendarHeader" ActiveTab="Year" Runat="Server" />
	<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />

	<asp:Table SkinID="tabFrame" CssClass="monthBox" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabFrame" CssClass="monthHeader" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="1%" CssClass="monthHeaderPrevTd" Wrap="false">
							<asp:ImageButton CommandName="Year.Previous" OnCommand="Page_Command" CssClass="NextPrevLink" AlternateText='<%# L10n.Term("Calendar.LBL_PREVIOUS_YEAR") %>' ImageUrl='<%# Session["themeURL"] + "images/calendar_previous.gif" %>' BorderWidth="0" Width="6" Height="9" ImageAlign="AbsMiddle" Runat="server" />&nbsp;
							<asp:LinkButton  CommandName="Year.Previous" OnCommand="Page_Command" CssClass="NextPrevLink" Text='<%# L10n.Term("Calendar.LBL_PREVIOUS_YEAR") %>' Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="98%" HorizontalAlign="Center">
							<span class="monthHeaderH3"><%= dtCurrentDate.Year %></span>
						</asp:TableCell>
						<asp:TableCell Width="1%" HorizontalAlign="Right" CssClass="monthHeaderNextTd" Wrap="false">
							<asp:LinkButton  CommandName="Year.Next" OnCommand="Page_Command" CssClass="NextPrevLink" Text='<%# L10n.Term("Calendar.LBL_NEXT_YEAR") %>' Runat="server" />&nbsp;
							<asp:ImageButton CommandName="Year.Next" OnCommand="Page_Command" CssClass="NextPrevLink" AlternateText='<%# L10n.Term("Calendar.LBL_NEXT_YEAR") %>' ImageUrl='<%# Session["themeURL"] + "images/calendar_next.gif" %>' BorderWidth="0" Width="6" Height="9" ImageAlign="AbsMiddle" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="monthCalBody">
				<table id="tblDailyCalTable" width="100%" border="0" cellpadding="0" cellspacing="1" Runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabFrame" CssClass="monthFooter" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="50%" CssClass="monthFooterPrev" Wrap="false">
							<asp:ImageButton CommandName="Year.Previous" OnCommand="Page_Command" CssClass="NextPrevLink" AlternateText='<%# L10n.Term("Calendar.LBL_PREVIOUS_YEAR") %>' ImageUrl='<%# Session["themeURL"] + "images/calendar_previous.gif" %>' BorderWidth="0" Width="6" Height="9" ImageAlign="AbsMiddle" Runat="server" />&nbsp;
							<asp:LinkButton  CommandName="Year.Previous" OnCommand="Page_Command" CssClass="NextPrevLink" Text='<%# L10n.Term("Calendar.LBL_PREVIOUS_YEAR") %>' Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="50%" HorizontalAlign="Right" CssClass="monthFooterNext" Wrap="false">
							<asp:LinkButton  CommandName="Year.Next" OnCommand="Page_Command" CssClass="NextPrevLink" Text='<%# L10n.Term("Calendar.LBL_NEXT_YEAR") %>' Runat="server" />&nbsp;
							<asp:ImageButton CommandName="Year.Next" OnCommand="Page_Command" CssClass="NextPrevLink" AlternateText='<%# L10n.Term("Calendar.LBL_NEXT_YEAR") %>' ImageUrl='<%# Session["themeURL"] + "images/calendar_next.gif" %>' BorderWidth="0" Width="6" Height="9" ImageAlign="AbsMiddle" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
