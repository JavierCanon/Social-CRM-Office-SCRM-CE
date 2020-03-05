<%@ Control CodeBehind="MyTasks.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Tasks.MyTasks" %>
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
<div id="divTasksMyTasks">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Tasks.LBL_LIST_MY_TASKS" Runat="Server" />
	
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
			<asp:TemplateColumn HeaderText="Tasks.LBL_LIST_CLOSE" SortExpression="STATUS" ItemStyle-Width="1%">
				<ItemTemplate>
					<asp:HyperLink NavigateUrl='<%# "~/Tasks/edit.aspx?id=" + DataBinder.Eval(Container.DataItem, "ID") + "&Status=Completed" %>' Runat="server">
						<asp:Image ImageUrl='<%# Session["themeURL"] + "images/close_inline.gif" %>' BorderWidth="0" Width="12" Height="14" ImageAlign="AbsMiddle" AlternateText='<%# L10n.Term("Tasks.LBL_LIST_CLOSE") %>' Runat="server" />
					</asp:HyperLink>
				</ItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn  HeaderText="Tasks.LBL_LIST_DUE_DATE"                      SortExpression="DATE_DUE" ItemStyle-Width="10%" ItemStyle-Wrap="false">
				<ItemTemplate>
					<font class="<%# (Sql.ToDateTime(DataBinder.Eval(Container.DataItem, "DATE_DUE")) < DateTime.Now) ? "overdueTask" : "futureTask" %>"><%# Sql.ToString(T10n.FromServerTime(DataBinder.Eval(Container.DataItem, "DATE_DUE"))) %></font>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
</div>
<br>
