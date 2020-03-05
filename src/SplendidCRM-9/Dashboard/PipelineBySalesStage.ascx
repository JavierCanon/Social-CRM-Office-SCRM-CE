<%@ Control CodeBehind="PipelineBySalesStage.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Dashboard.PipelineBySalesStage" %>
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
<div id="divDashboardPipelineBySalesStage">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ChartDatePicker" Src="~/_controls/ChartDatePicker.ascx" %>
	<%@ Register TagPrefix="SplendidCRM" Tagname="ChartHeader" Src="~/_controls/ChartHeader.ascx" %>
	<SplendidCRM:ChartHeader Title="Dashboard.LBL_SALES_STAGE_FORM_TITLE" DivEditName="pipeline_by_sales_stage_edit" Runat="Server" />
	<p>
	<div ID="pipeline_by_sales_stage_edit" style="DISPLAY: <%= bShowEditDialog ? "inline" : "none" %>">
		<asp:Table SkinID="tabFrame" HorizontalAlign="Center" CssClass="chartForm" runat="server">
			<asp:TableRow>
				<asp:TableCell VerticalAlign="top" Wrap="false">
					<b><%= L10n.Term("Dashboard.LBL_DATE_START") %></b><br />
					<asp:Label CssClass="dateFormat" Text='<%# System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern.ToUpper() %>' runat="server" />
				</asp:TableCell>
				<asp:TableCell VerticalAlign="top">
					<SplendidCRM:ChartDatePicker ID="ctlDATE_START" Runat="Server" />
				</asp:TableCell>
			</asp:TableRow>
			<asp:TableRow>
				<asp:TableCell VerticalAlign="top" Wrap="false">
					<b><%= L10n.Term("Dashboard.LBL_DATE_END") %></b><br />
					<asp:Label CssClass="dateFormat" Text='<%# System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern.ToUpper() %>' runat="server" />
				</asp:TableCell>
				<asp:TableCell VerticalAlign="top">
					<SplendidCRM:ChartDatePicker ID="ctlDATE_END" Runat="Server" />
				</asp:TableCell>
			</asp:TableRow>
			<asp:TableRow>
				<asp:TableCell VerticalAlign="top" Wrap="false"><b><%= L10n.Term("Dashboard.LBL_SALES_STAGES") %></b></asp:TableCell>
				<asp:TableCell VerticalAlign="top">
					<asp:ListBox ID="lstSALES_STAGE" DataValueField="NAME" DataTextField="DISPLAY_NAME" SelectionMode="Multiple" Rows="3" Runat="server" />
				</asp:TableCell>
			</asp:TableRow>
			<asp:TableRow>
				<asp:TableCell VerticalAlign="top" Wrap="false"><b><%= L10n.Term("Dashboard.LBL_USERS") %></b></asp:TableCell>
				<asp:TableCell VerticalAlign="top">
					<asp:ListBox ID="lstUSERS" DataValueField="ID" DataTextField="USER_NAME" SelectionMode="Multiple" Rows="3" Runat="server" />
				</asp:TableCell>
			</asp:TableRow>
			<asp:TableRow>
				<asp:TableCell HorizontalAlign="Right" ColumnSpan="2">
					<asp:Button ID="btnSubmit" CommandName="Submit" OnCommand="Page_Command"                                CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SELECT_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_SELECT_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SELECT_BUTTON_KEY") %>' runat="server" />
					<asp:Button ID="btnCancel" OnClientClick="toggleDisplay('pipeline_by_sales_stage_edit'); return false;" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_CANCEL_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_DONE_CANCEL_TITLE"  ) %>' AccessKey='<%# L10n.AccessKey(".LBL_CANCEL_BUTTON_KEY") %>' runat="server" />
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</div>
	</p>
	<p align="center">

	<object width="800" height="400" align="" classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" codebase="https://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0" viewastext>
		<param name="movie"   value="<%= Application["chartURL" ] %>hBarF.swf?filename=<%= Server.UrlEncode(Application["rootURL"] + "Opportunities/xml/PipelineBySalesStage.aspx?" + ViewState["PipelineBySalesStageQueryString"]) %>">
		<param name="bgcolor" value="#FFFFFF" />
		<param name="wmode"   value="transparent" />
		<param name="quality" value="high" />
		<embed src="<%= Application["chartURL" ] %>hBarF.swf?filename=<%= Server.UrlEncode(Application["rootURL"] + "Opportunities/xml/PipelineBySalesStage.aspx?" + ViewState["PipelineBySalesStageQueryString"]) %>" wmode="transparent" quality=high bgcolor=#FFFFFF  WIDTH="800" HEIGHT="400" NAME="hBarF" ALIGN="" TYPE="application/x-shockwave-flash" PLUGINSPAGE="https://www.macromedia.com/go/getflashplayer" />
	</object>
	</p>
	<span class="chartFootnote">
		<p align="center"><%= L10n.Term("Dashboard.LBL_PIPELINE_FORM_TITLE_DESC") %></p>
		<p align="right"><i><%= L10n.Term("Dashboard.LBL_CREATED_ON") + T10n.FromServerTime(DateTime.Now).ToString() %></i></p>
	</span>
</div>
