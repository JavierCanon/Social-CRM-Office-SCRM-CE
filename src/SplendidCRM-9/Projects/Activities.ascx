<%@ Control CodeBehind="Activities.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Projects.Activities" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005-2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
%>
<div id="divProjectsActivitiesOpen">
	<br />
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="ProjectTask.LBL_ACTIVITIES_SUBPANEL_TITLE" Runat="Server" />
	
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Button Visible='<%# SplendidCRM.Security.GetUserAccess("Meetings", "edit") >= 0 %>' CommandName="Meetings.Create" OnCommand="Page_Command" CssClass="button" Text='<%# " " + L10n.Term("Activities.LBL_SCHEDULE_MEETING_BUTTON_LABEL") + " " %>' ToolTip='<%# L10n.Term("Activities.LBL_SCHEDULE_MEETING_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey("Activities.LBL_SCHEDULE_MEETING_BUTTON_KEY") %>' Runat="server" />
		<asp:Button Visible='<%# SplendidCRM.Security.GetUserAccess("Calls"   , "edit") >= 0 %>' CommandName="Calls.Create"    OnCommand="Page_Command" CssClass="button" Text='<%# " " + L10n.Term("Activities.LBL_SCHEDULE_CALL_BUTTON_LABEL"   ) + " " %>' ToolTip='<%# L10n.Term("Activities.LBL_SCHEDULE_CALL_BUTTON_TITLE"   ) %>' AccessKey='<%# L10n.AccessKey("Activities.LBL_SCHEDULE_CALL_BUTTON_KEY"   ) %>' Runat="server" />
		<asp:Button Visible='<%# SplendidCRM.Security.GetUserAccess("Emails"  , "edit") >= 0 %>' CommandName="Emails.Compose"  OnCommand="Page_Command" CssClass="button" Text='<%# " " + L10n.Term(".LBL_COMPOSE_EMAIL_BUTTON_LABEL"             ) + " " %>' ToolTip='<%# L10n.Term(".LBL_COMPOSE_EMAIL_BUTTON_TITLE"             ) %>' AccessKey='<%# L10n.AccessKey(".LBL_COMPOSE_EMAIL_BUTTON_KEY"             ) %>' Runat="server" />
		<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	</asp:Panel>
	
	<SplendidCRM:SplendidGrid id="grdOpen" Width="100%" CssClass="listView"
		CellPadding="3" CellSpacing="0" border="0"
		AllowPaging="true" PageSize="20" AllowSorting="false" 
		AutoGenerateColumns="false" 
		EnableViewState="true" runat="server">
		<ItemStyle            CssClass="oddListRowS1"  />
		<AlternatingItemStyle CssClass="evenListRowS1" />
		<HeaderStyle          CssClass="listViewThS1"  />
		<PagerStyle HorizontalAlign="Right" Mode="NextPrev" PageButtonCount="6" Position="Top" CssClass="listViewPaginationTdS1" PrevPageText=".LNK_LIST_PREVIOUS" NextPageText=".LNK_LIST_NEXT" />
		<Columns>
			<asp:TemplateColumn  HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Center">
				<ItemTemplate>
					<asp:Image ImageUrl='<%# Session["themeURL"] + "images/" + DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE") + ".gif" %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn  HeaderText="Activities.LBL_LIST_CLOSE" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Center">
				<ItemTemplate>
					<asp:HyperLink Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE")), "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ACTIVITY_ASSIGNED_USER_ID"))) >= 0 %>' NavigateUrl='<%# "~/" + DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE") + "/edit.aspx?id=" + DataBinder.Eval(Container.DataItem, "ACTIVITY_ID") + "&Status=Close" + "&PARENT_ID=" + gID.ToString() %>' Runat="server">
						<asp:Image ImageUrl='<%# Session["themeURL"] + "images/close_inline.gif" %>' BorderWidth="0" Width="12" Height="14" ImageAlign="AbsMiddle" AlternateText='<%# L10n.Term("Activities.LBL_LIST_CLOSE") %>' Runat="server" />
					</asp:HyperLink>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn  HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<asp:HyperLink Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE")), "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ACTIVITY_ASSIGNED_USER_ID"))) >= 0 %>' NavigateUrl='<%# "~/" + DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE") + "/edit.aspx?id=" + DataBinder.Eval(Container.DataItem, "ACTIVITY_ID") %>' CssClass="listViewTdToolsS1" Runat="server">
						<asp:Image ImageUrl='<%# Session["themeURL"] + "images/edit_inline.gif" %>' AlternateText='<%# L10n.Term(".LNK_EDIT") %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />&nbsp;<%# L10n.Term(".LNK_EDIT") %>
					</asp:HyperLink>
					&nbsp;
					<span onclick="return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>')">
						<asp:ImageButton Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE")), "delete", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ACTIVITY_ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Activities.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ACTIVITY_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' ImageUrl='<%# Session["themeURL"] + "images/delete_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
						<asp:LinkButton  Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE")), "delete", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ACTIVITY_ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Activities.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ACTIVITY_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_DELETE") %>' Runat="server" />
					</span>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>

<div id="divProjectsActivitiesHistory">
	<br />
	<SplendidCRM:ListHeader Title="Activities.LBL_HISTORY" Runat="Server" />
	
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Button Visible='<%# SplendidCRM.Security.GetUserAccess("Notes" , "edit") >= 0 %>' CommandName="Notes.Create"   OnCommand="Page_Command" CssClass="button" Text='<%# " " + L10n.Term("Activities.LBL_NEW_NOTE_BUTTON_LABEL"   ) + " " %>' ToolTip='<%# L10n.Term("Activities.LBL_NEW_NOTE_BUTTON_TITLE"   ) %>' AccessKey='<%# L10n.AccessKey("Activities.LBL_NEW_NOTE_BUTTON_KEY"   ) %>' Runat="server" />
		<asp:Button Visible='<%# SplendidCRM.Security.GetUserAccess("Emails", "edit") >= 0 %>' CommandName="Emails.Archive" OnCommand="Page_Command" CssClass="button" Text='<%# " " + L10n.Term("Activities.LBL_TRACK_EMAIL_BUTTON_LABEL") + " " %>' ToolTip='<%# L10n.Term("Activities.LBL_TRACK_EMAIL_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey("Activities.LBL_TRACK_EMAIL_BUTTON_KEY") %>' Runat="server" />
	</asp:Panel>
	
	<SplendidCRM:SplendidGrid id="grdHistory" Width="100%" CssClass="listView"
		CellPadding="3" CellSpacing="0" border="0"
		AllowPaging="true" PageSize="20" AllowSorting="false" 
		AutoGenerateColumns="false" 
		EnableViewState="true" runat="server">
		<ItemStyle            CssClass="oddListRowS1"  />
		<AlternatingItemStyle CssClass="evenListRowS1" />
		<HeaderStyle          CssClass="listViewThS1"  />
		<PagerStyle HorizontalAlign="Right" Mode="NextPrev" PageButtonCount="6" Position="Top" CssClass="listViewPaginationTdS1" PrevPageText=".LNK_LIST_PREVIOUS" NextPageText=".LNK_LIST_NEXT" />
		<Columns>
			<asp:TemplateColumn  HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Center">
				<ItemTemplate>
					<asp:Image ImageUrl='<%# Session["themeURL"] + "images/" + DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE") + ".gif" %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn  HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
				<ItemTemplate>
					<asp:HyperLink Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE")), "edit", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ACTIVITY_ASSIGNED_USER_ID"))) >= 0 %>' NavigateUrl='<%# "~/" + DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE") + "/edit.aspx?id=" + DataBinder.Eval(Container.DataItem, "ACTIVITY_ID") %>' CssClass="listViewTdToolsS1" Runat="server">
						<asp:Image ImageUrl='<%# Session["themeURL"] + "images/edit_inline.gif" %>' AlternateText='<%# L10n.Term(".LNK_EDIT") %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />&nbsp;<%# L10n.Term(".LNK_EDIT") %>
					</asp:HyperLink>
					&nbsp;
					<span onclick="return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>')">
						<asp:ImageButton Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE")), "delete", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ACTIVITY_ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Activities.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ACTIVITY_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' ImageUrl='<%# Session["themeURL"] + "images/delete_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
						<asp:LinkButton  Visible='<%# SplendidCRM.Security.GetUserAccess(Sql.ToString(DataBinder.Eval(Container.DataItem, "ACTIVITY_TYPE")), "delete", Sql.ToGuid(DataBinder.Eval(Container.DataItem, "ACTIVITY_ASSIGNED_USER_ID"))) >= 0 %>' CommandName="Activities.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ACTIVITY_ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_DELETE") %>' Runat="server" />
					</span>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>
