<%@ Control CodeBehind="MyActivities.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Activities.MyActivities" %>
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
<div id="divActivitiesMyActivities">
	<asp:Table Width="100%" BorderWidth="0" CellSpacing="0" CellPadding="0" CssClass="h3Row" runat="server">
		<asp:TableRow>
			<asp:TableCell Wrap="false">
				<h3><asp:Image Height="11" Width="11" BorderWidth="0" AlternateText="Account List" ImageUrl='<%# Session["themeURL"] + "images/h3Arrow.gif" %>' Runat="server" />&nbsp;<%= L10n.Term("Activities.LBL_UPCOMING") %></h3>
			</asp:TableCell>
			<asp:TableCell Wrap="false">
				&nbsp;&nbsp;
				<%= L10n.Term("Activities.LBL_TODAY") %><asp:DropDownList ID="lstTHROUGH" DataValueField="NAME" DataTextField="DISPLAY_NAME" SelectedIndexChanged="Page_Command" AutoPostBack="true" Runat="server" />
				<asp:Label ID="txtTHROUGH" Runat="server" />
			</asp:TableCell>
			<asp:TableCell width="50%"><asp:Image SkinID="blank" runat="server" /></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	
	<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	<SplendidCRM:SplendidGrid id="grdMain" Width="100%" CssClass="listView"
		CellPadding="3" CellSpacing="0" border="0"
		AllowPaging="true" PageSize="20" AllowSorting="true" 
		AutoGenerateColumns="false" 
		EnableViewState="true" runat="server">
		<ItemStyle            CssClass="oddListRowS1"  />
		<AlternatingItemStyle CssClass="evenListRowS1" />
		<HeaderStyle          CssClass="listViewThS1"  />
		<PagerStyle HorizontalAlign="Right" Mode="NextPrev" PageButtonCount="6" Position="Top" CssClass="listViewPaginationTdS1" PrevPageText=".LNK_LIST_PREVIOUS" NextPageText=".LNK_LIST_NEXT" />
		<Columns>
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Center">
				<ItemTemplate>
					<asp:Image ImageUrl='<%# Session["themeURL"] + "images/" + DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE") + ".gif" %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="Activities.LBL_LIST_CLOSE" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Center">
				<ItemTemplate>
					<asp:HyperLink Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE")), "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ASSIGNED_USER_ID"))) >= 0 %>' NavigateUrl='<%# "~/" + DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE") + "/edit.aspx?id=" + DataBinder.Eval(Container.DataItem, "ID") + "&Status=Close" %>' Runat="server">
						<asp:Image ImageUrl='<%# Session["themeURL"] + "images/close_inline.gif" %>' BorderWidth="0" Width="12" Height="14" ImageAlign="AbsMiddle" AlternateText='<%# L10n.Term("Activities.LBL_LIST_CLOSE") %>' Runat="server" />
					</asp:HyperLink>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn  HeaderText="Activities.LBL_LIST_DATE" SortExpression="DATE_START" ItemStyle-Width="10%" ItemStyle-Wrap="false">
				<ItemTemplate>
					<font CssClass="<%# (Sql.ToDateTime(DataBinder.Eval(Container.DataItem, "DATE_START")) < DateTime.Now) ? "overdueTask" : "futureTask" %>"><%# Sql.ToDateString(T10n.FromServerTime(Sql.ToDateTime(DataBinder.Eval(Container.DataItem, "DATE_START")))) %></font>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="Activities.LBL_ACCEPT_THIS" ItemStyle-Width="10%" ItemStyle-Wrap="false">
				<ItemTemplate>
					<div style="DISPLAY: <%# String.Compare((DataBinder.Eval(Container.DataItem, "ACCEPT_STATUS") as string), "none", true) == 0 ? "inline" : "none" %>">
						<asp:ImageButton Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE")), "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Activity.Accept"    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" ImageUrl='<%# Session["themeURL"] + "images/accept_inline.gif"    %>' AlternateText='<%# L10n.Term(".dom_meeting_accept_options.accept"   ) %>' BorderWidth="0" Width="12" Height="14" ImageAlign="AbsMiddle" Runat="server" />
						<asp:ImageButton Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE")), "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Activity.Tentative" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" ImageUrl='<%# Session["themeURL"] + "images/tentative_inline.gif" %>' AlternateText='<%# L10n.Term(".dom_meeting_accept_options.tentative") %>' BorderWidth="0" Width="12" Height="14" ImageAlign="AbsMiddle" Runat="server" />
						<asp:ImageButton Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE")), "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Activity.Decline"   CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" ImageUrl='<%# Session["themeURL"] + "images/decline_inline.gif"   %>' AlternateText='<%# L10n.Term(".dom_meeting_accept_options.decline"  ) %>' BorderWidth="0" Width="12" Height="14" ImageAlign="AbsMiddle" Runat="server" />
					</div>
					<div style="DISPLAY: <%# String.Compare((DataBinder.Eval(Container.DataItem, "ACCEPT_STATUS") as string), "none", true) != 0 ? "inline" : "none" %>">
						<%# L10n.Term(".dom_meeting_accept_status." + DataBinder.Eval(Container.DataItem, "ACCEPT_STATUS")) %>
					</div>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
	<br />
</div>
