<%@ Control CodeBehind="InviteesView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Calls.InviteesView" %>
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
<div id="divListView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchInvitees" Src="SearchInvitees.ascx" %>
	<SplendidCRM:SearchInvitees ID="ctlSearch" Runat="Server" />
	<br>
	<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	<div id="divInvitees" style="DISPLAY: block; OVERFLOW: auto; WIDTH: 100%; HEIGHT: 125px" visible="false" runat="server">
		<SplendidCRM:SplendidGrid id="grdMain" Width="100%" CssClass="listView"
			CellPadding="3" CellSpacing="0" border="0"
			AllowPaging="true" PageSize="20" AllowSorting="true" 
			AutoGenerateColumns="false" 
			EnableViewState="true" runat="server">
			<ItemStyle            CssClass="oddListRowS1"  />
			<AlternatingItemStyle CssClass="evenListRowS1" />
			<HeaderStyle          CssClass="listViewThS1"  />
			<Columns>
				<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Center">
					<ItemTemplate>
						<asp:Image ImageUrl='<%# Session["themeURL"] + "images/" + DataBinder.Eval(Container.DataItem, "INVITEE_TYPE") + ".gif" %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:BoundColumn HeaderText="Calls.LBL_NAME"  DataField="NAME"  SortExpression="NAME"  ItemStyle-Width="30%" />
				<asp:BoundColumn HeaderText="Calls.LBL_EMAIL" DataField="EMAIL" SortExpression="EMAIL" ItemStyle-Width="30%" />
				<asp:BoundColumn HeaderText="Calls.LBL_PHONE" DataField="PHONE" SortExpression="PHONE" ItemStyle-Width="30%" />
				<asp:TemplateColumn HeaderText="" ItemStyle-HorizontalAlign="Center">
					<ItemTemplate>
						<asp:Button CommandName="Invitees.Add" OnCommand="Page_Command" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' CssClass="button" Text='<%# " " + L10n.Term("Calls.LBL_ADD_BUTTON") + " " %>' ToolTip='<%# L10n.Term("Calls.LBL_ADD_BUTTON") %>' Enabled='<%# !IsExistingInvitee(DataBinder.Eval(Container.DataItem, "ID").ToString()) %>' Runat="server" />
					</ItemTemplate>
				</asp:TemplateColumn>
			</Columns>
		</SplendidCRM:SplendidGrid>
	</div>
</div>
