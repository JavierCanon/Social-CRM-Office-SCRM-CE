<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.Administration.Schedulers.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<script type="text/javascript">
// 01/19/2008 Paul.  We need to redefine the calendar function so that it points two levels up. 
function CalendarPopup(ctlDate, clientX, clientY)
{
	clientX = window.screenLeft + parseInt(clientX);
	clientY = window.screenTop  + parseInt(clientY);
	if ( clientX < 0 )
		clientX = 0;
	if ( clientY < 0 )
		clientY = 0;
	return window.open('../../Calendar/Popup.aspx?Date=' + ctlDate.value,'CalendarPopup','width=193,height=155,resizable=1,scrollbars=0,left=' + clientX + ',top=' + clientY);
}
</script>
<div id="divEditView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Schedulers" EnablePrint="false" HelpName="EditView" EnableHelp="true" Runat="Server" />
	<p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="EditButtons" Src="~/_controls/EditButtons.ascx" %>
	<SplendidCRM:EditButtons ID="ctlEditButtons" Visible="<%# !PrintView %>" Runat="Server" />

	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell ColumnSpan="4"><h4><asp:Label Text='<%# L10n.Term("Schedulers.LBL_BASIC_OPTIONS") %>' runat="server" /></h4></asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_NAME"  ) %> <asp:Label Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' CssClass="required" runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox ID="NAME" size="35" MaxLength="255" Runat="server" />
							<asp:RequiredFieldValidator ID="NAME_REQUIRED" ControlToValidate="NAME" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableViewState="false" EnableClientScript="false" Enabled="false" runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_STATUS") %> <asp:Label Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' CssClass="required" runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField"><asp:DropDownList ID="STATUS" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell VerticalAlign="Top" CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_JOB") %> <asp:Label Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' CssClass="required" runat="server" /></asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataField"><asp:DropDownList ID="JOB" Runat="server" /></asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataLabel">&nbsp;</asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell VerticalAlign="Top" CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_INTERVAL") %> <asp:Label Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' CssClass="required" runat="server" /></asp:TableCell>
						<asp:TableCell>
							<asp:Table CellPadding="0" CellSpacing="0" runat="server">
								<asp:TableRow>
									<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_MINS"        ) %></asp:TableCell>
									<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_HOURS"       ) %></asp:TableCell>
									<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_DAY_OF_MONTH") %></asp:TableCell>
									<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_MONTHS"      ) %></asp:TableCell>
									<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_DAY_OF_WEEK" ) %></asp:TableCell>
								</asp:TableRow>
								<asp:TableRow>
									<asp:TableCell CssClass="dataField"><asp:TextBox ID="CRON_MINUTES"    size="3" MaxLength="25" runat="server" /></asp:TableCell>
									<asp:TableCell CssClass="dataField"><asp:TextBox ID="CRON_HOURS"      size="3" MaxLength="25" runat="server" /></asp:TableCell>
									<asp:TableCell CssClass="dataField"><asp:TextBox ID="CRON_DAYOFMONTH" size="3" MaxLength="25" runat="server" /></asp:TableCell>
									<asp:TableCell CssClass="dataField"><asp:TextBox ID="CRON_MONTHS"     size="3" MaxLength="25" runat="server" /></asp:TableCell>
									<asp:TableCell CssClass="dataField"><asp:TextBox ID="CRON_DAYOFWEEK"  size="3" MaxLength="25" runat="server" /></asp:TableCell>
								</asp:TableRow>
								<asp:TableRow>
									<asp:TableCell ColumnSpan="5"><em><%= L10n.Term("Schedulers.LBL_CRONTAB_EXAMPLES") %></em></asp:TableCell>
								</asp:TableRow>
							</asp:Table>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	<p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="DateTimePicker" Src="~/_controls/DateTimePicker.ascx" %>
	<%@ Register TagPrefix="SplendidCRM" Tagname="TimePicker" Src="~/_controls/TimePicker.ascx" %>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell ColumnSpan="4"><h4><asp:Label Text='<%# L10n.Term("Schedulers.LBL_ADV_OPTIONS") %>' runat="server" /></h4></asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_CATCH_UP") %></asp:TableCell>
						<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="dataField"><asp:CheckBox ID="CATCH_UP" CssClass="checkbox" ToolTip='<%# L10n.Term("Schedulers.LBL_CATCH_UP_WARNING") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="15%" VerticalAlign="Top" CssClass="dataLabel">&nbsp;</asp:TableCell>
						<asp:TableCell Width="35%" VerticalAlign="Top" CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell VerticalAlign="Top" CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_DATE_TIME_START") %></asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataField"><SplendidCRM:DateTimePicker ID="DATE_TIME_START" MinutesIncrement="5" Runat="server" /></asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_TIME_FROM"      ) %></asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataField"><SplendidCRM:TimePicker ID="TIME_FROM" MinutesIncrement="5" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell VerticalAlign="Top" CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_DATE_TIME_END") %></asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataField"><SplendidCRM:DateTimePicker ID="DATE_TIME_END" MinutesIncrement="5" Runat="server" /></asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataLabel"><%= L10n.Term("Schedulers.LBL_TIME_TO"      ) %></asp:TableCell>
						<asp:TableCell VerticalAlign="Top" CssClass="dataField"><SplendidCRM:TimePicker ID="TIME_TO" MinutesIncrement="5" Runat="server" /></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
</div>
