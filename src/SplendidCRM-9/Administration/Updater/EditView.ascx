<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.Administration.Updater.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divEditView">
	<p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="EditButtons" Src="~/_controls/EditButtons.ascx" %>
	<SplendidCRM:EditButtons ID="ctlEditButtons" Visible="<%# !PrintView %>" Runat="Server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell CssClass="dataLabel"><h4><asp:Label Text='<%# L10n.Term("Administration.LBL_SPLENDIDCRM_UPDATE_TITLE") %>' runat="server" /></h4></asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataField">
							<asp:Label Text='<%# L10n.Term("Administration.HEARTBEAT_MESSAGE") %>' runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell>
							<asp:Table CellPadding="4" runat="server">
								<asp:TableRow>
									<asp:TableCell Width="1%" CssClass="dataField" VerticalAlign="Top">
										<asp:CheckBox ID="SEND_USAGE_INFO" CssClass="checkbox" Runat="server" />
									</asp:TableCell>
									<asp:TableCell Width="99%" CssClass="dataField">
										<asp:Label Text='<%# L10n.Term("Administration.LBL_SEND_STAT") %>' runat="server" />
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell>
							<asp:Table CellPadding="4" runat="server">
								<asp:TableRow>
									<asp:TableCell Width="1%" CssClass="dataField" VerticalAlign="Top">
										<asp:CheckBox ID="CHECK_UPDATES" CssClass="checkbox" Runat="server" />
									</asp:TableCell>
									<asp:TableCell Width="99%" CssClass="dataField">
										<asp:Label Text='<%# L10n.Term("Administration.LBL_UPDATE_CHECK_TYPE") %>' runat="server" />
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell>
							<asp:Table CellPadding="4" runat="server">
								<asp:TableRow>
									<asp:TableCell CssClass="dataField">
										<asp:Button OnCommand="Page_Command" CommandName="CheckNow" Text='<%# "  " + L10n.Term("Administration.LBL_CHECK_NOW_LABEL") + "  " %>' ToolTip='<%# L10n.Term("Administration.LBL_CHECK_NOW_TITLE") %>' CssClass="buttonOn" runat="server" />
										&nbsp;
										<b>Version <%= Application["CONFIG.sugar_version"] %> Build <%= Application["SplendidVersion"] %></b>
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataField">
							<asp:Label ID="NO_UPDATES" Text='<%# L10n.Term("Administration.LBL_UPTODATE") %>' Visible="false" runat="server" />

							<SplendidCRM:SplendidGrid id="grdMain" Width="100%" CssClass="listView"
								CellPadding="3" CellSpacing="0" border="0"
								AllowPaging="false" PageSize="20" AllowSorting="false" 
								AutoGenerateColumns="false" 
								EnableViewState="true" runat="server">
								<ItemStyle            CssClass="oddListRowS1"  />
								<AlternatingItemStyle CssClass="evenListRowS1" />
								<HeaderStyle          CssClass="listViewThS1"  />
								<PagerStyle HorizontalAlign="Right" Mode="NextPrev" PageButtonCount="6" Position="Top" CssClass="listViewPaginationTdS1" PrevPageText=".LNK_LIST_PREVIOUS" NextPageText=".LNK_LIST_NEXT" />
								<Columns>
									<asp:BoundColumn     HeaderText="Build"       DataField="Build"       ItemStyle-Width="15%" ItemStyle-Wrap="false" />
									<asp:BoundColumn     HeaderText="Date"        DataField="Date"        ItemStyle-Width="15%" ItemStyle-Wrap="false" />
									<asp:BoundColumn     HeaderText="Description" DataField="Description" ItemStyle-Width="60%" ItemStyle-Wrap="true" />
									<asp:TemplateColumn  HeaderText="" ItemStyle-Width="1%">
										<ItemTemplate>
											<asp:HyperLink  NavigateUrl='<%# Eval("URL") %>' ImageUrl='<%# Session["themeURL"] + "images/backup.gif" %>' runat="server" />
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
							</SplendidCRM:SplendidGrid>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
</div>
