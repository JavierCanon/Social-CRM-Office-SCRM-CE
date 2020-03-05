<%@ Control CodeBehind="SharedGrid.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Calendar.SharedGrid" %>
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
<div id="SharedGrid">
	<%@ Register TagPrefix="SplendidCRM" Tagname="CalendarHeader" Src="CalendarHeader.ascx" %>
	<SplendidCRM:CalendarHeader ID="ctlCalendarHeader" ActiveTab="Shared" Runat="Server" />

	<p>
	<asp:Table SkinID="tabFrame" runat="server">
		<asp:TableRow>
			<asp:TableCell Wrap="false">
				<h3><asp:Image ImageUrl='<%# Session["themeURL"] + "images/h3Arrow.gif" %>' Height="11" Width="11" BorderWidth="0" Runat="server" />&nbsp;<%= L10n.Term("Calendar.LBL_SHARED_CAL_TITLE") %></h3>
			</asp:TableCell>
			<asp:TableCell HorizontalAlign="Right" Wrap="false">
				<span onclick="toggleDisplay('shared_cal_edit'); return false;">
					<asp:ImageButton CommandName="Edit" OnCommand="Page_Command" CssClass="chartToolsLink" AlternateText='<%# L10n.Term("Calendar.LBL_EDIT") %>' ImageUrl='<%# Session["themeURL"] + "images/edit.gif" %>' BorderWidth="0" Width="13" Height="13" ImageAlign="AbsMiddle" Runat="server" />&nbsp;
					<asp:LinkButton  CommandName="Edit" OnCommand="Page_Command" CssClass="chartToolsLink" Text='<%# L10n.Term("Calendar.LBL_EDIT") %>' Runat="server" />
				</span>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>

	<div ID="shared_cal_edit" style="DISPLAY: none">
		<asp:Table SkinID="tabFrame" HorizontalAlign="Center" runat="server">
			<asp:TableRow>
				<asp:TableHeaderCell VerticalAlign="Top" HorizontalAlign="Center" ColumnSpan="2"><%= L10n.Term("Calendar.LBL_SELECT_USERS") %></asp:TableHeaderCell>
			</asp:TableRow>
			<asp:TableRow>
				<asp:TableCell>
					<asp:Table BorderWidth="0" CellPadding="1" CellSpacing="1" HorizontalAlign="Center" CssClass="chartForm" runat="server">
						<asp:TableRow>
							<asp:TableCell VerticalAlign="Top" Wrap="false"><b><%= L10n.Term("Calendar.LBL_USERS") %></b></asp:TableCell>
							<asp:TableCell VerticalAlign="Top">
								<asp:ListBox ID="lstUSERS" DataValueField="ID" DataTextField="USER_NAME" SelectionMode="Multiple" Rows="3" Runat="server" />
							</asp:TableCell>
							<asp:TableCell VerticalAlign="Top">
								<a onclick="javascript:MoveUp('<%= lstUSERS.ClientID %>');">
									<asp:Image ImageUrl='<%# Session["themeURL"] + "images/uparrow_big.gif" %>' AlternateText='<%# L10n.Term(".LNK_SORT") %>' style="margin-bottom: 1px;" BorderWidth="0" Width="16" Height="16" Runat="server" />
								</a><br>
								<a onclick="javascript:MoveDown('<%= lstUSERS.ClientID %>');">
									<asp:Image ImageUrl='<%# Session["themeURL"] + "images/downarrow_big.gif" %>' AlternateText='<%# L10n.Term(".LNK_SORT") %>' style="margin-top: 1px;" BorderWidth="0" Width="16" Height="16" Runat="server" />
								</a>
							</asp:TableCell>
						</asp:TableRow>
						<asp:TableRow>
							<asp:TableCell HorizontalAlign="Right" ColumnSpan="2">
								<asp:Button ID="btnSubmit" CommandName="Submit" OnCommand="Page_Command"                   CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SELECT_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_SELECT_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SELECT_BUTTON_KEY") %>' runat="server" />&nbsp;
								<asp:Button ID="btnCancel" OnClientClick="toggleDisplay('shared_cal_edit'); return false;" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_CANCEL_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_DONE_CANCEL_TITLE"  ) %>' AccessKey='<%# L10n.AccessKey(".LBL_CANCEL_BUTTON_KEY") %>' runat="server" />
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</div>

	<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	<asp:Table SkinID="tabFrame" CssClass="monthBox" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabFrame" CssClass="monthHeader" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="1%" CssClass="monthHeaderPrevTd" Wrap="false">
							<asp:ImageButton CommandName="Shared.Previous" OnCommand="Page_Command" CssClass="NextPrevLink" AlternateText='<%# L10n.Term(".LNK_LIST_PREVIOUS") %>' ImageUrl='<%# Session["themeURL"] + "images/calendar_previous.gif" %>' BorderWidth="0" Width="6" Height="9" ImageAlign="AbsMiddle" Runat="server" />&nbsp;
							<asp:LinkButton  CommandName="Shared.Previous" OnCommand="Page_Command" CssClass="NextPrevLink" Text='<%# L10n.Term(".LNK_LIST_PREVIOUS") %>' Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="98%" HorizontalAlign="Center">
							<span class="monthHeaderH3"><%= dtCurrentWeek.ToLongDateString() + " - " + dtCurrentWeek.AddDays(6).ToLongDateString() %></span>
						</asp:TableCell>
						<asp:TableCell Width="1%" HorizontalAlign="Right" CssClass="monthHeaderNextTd" Wrap="false">
							<asp:LinkButton  CommandName="Shared.Next" OnCommand="Page_Command" CssClass="NextPrevLink" Text='<%# L10n.Term(".LBL_NEXT_BUTTON_LABEL") %>' Runat="server" />
							<asp:ImageButton CommandName="Shared.Next" OnCommand="Page_Command" CssClass="NextPrevLink" AlternateText='<%# L10n.Term(".LBL_NEXT_BUTTON_LABEL") %>' ImageUrl='<%# Session["themeURL"] + "images/calendar_next.gif" %>' BorderWidth="0" Width="6" Height="9" ImageAlign="AbsMiddle" Runat="server" />&nbsp;
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="monthCalBody">
				<asp:PlaceHolder ID="plcWeekRows" Runat="server" />
			</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabFrame" CssClass="monthFooter" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="50%" CssClass="monthFooterPrev" Wrap="false">
							<asp:ImageButton CommandName="Shared.Previous" OnCommand="Page_Command" CssClass="NextPrevLink" AlternateText='<%# L10n.Term(".LNK_LIST_PREVIOUS") %>' ImageUrl='<%# Session["themeURL"] + "images/calendar_previous.gif" %>' BorderWidth="0" Width="6" Height="9" ImageAlign="AbsMiddle" Runat="server" />&nbsp;
							<asp:LinkButton  CommandName="Shared.Previous" OnCommand="Page_Command" CssClass="NextPrevLink" Text='<%# L10n.Term(".LNK_LIST_PREVIOUS") %>' Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="50%" HorizontalAlign="Right" CssClass="monthFooterNext" Wrap="false">
							<asp:LinkButton  CommandName="Shared.Next" OnCommand="Page_Command" CssClass="NextPrevLink" Text='<%# L10n.Term(".LBL_NEXT_BUTTON_LABEL") %>' Runat="server" />
							<asp:ImageButton CommandName="Shared.Next" OnCommand="Page_Command" CssClass="NextPrevLink" AlternateText='<%# L10n.Term(".LBL_NEXT_BUTTON_LABEL") %>' ImageUrl='<%# Session["themeURL"] + "images/calendar_next.gif" %>' BorderWidth="0" Width="6" Height="9" ImageAlign="AbsMiddle" Runat="server" />&nbsp;
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</div>
