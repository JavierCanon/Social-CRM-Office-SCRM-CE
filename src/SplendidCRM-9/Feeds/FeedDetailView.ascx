<%@ Control CodeBehind="FeedDetailView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Feeds.FeedDetailView" %>
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
<div id="divDetailView">
	<asp:Table SkinID="tabFrame" CssClass="tabDetailView" runat="server">
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDF">
				<asp:Table SkinID="tabFrame" CssClass="mod" runat="server">
					<asp:TableRow>
						<asp:TableCell bgcolor="aaaaaa">
							<asp:Table Width="100%" BorderWidth="0" CellPadding="2" CellSpacing="0" runat="server">
								<asp:TableRow>
									<asp:TableCell CssClass="modtitle" width="98%">
										<asp:HyperLink Text='<%# sChannelTitle %>' NavigateUrl='<%# sChannelLink %>' Runat="server" />
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell>
							<asp:DataGrid id="grdMain" Width="100%" CellPadding="3" CellSpacing="0" border="0"
								AllowPaging="false" AllowSorting="false" AutoGenerateColumns="false" 
								ShowHeader="false" EnableViewState="false" runat="server">
								<Columns>
									<asp:TemplateColumn>
										<ItemTemplate>
											<asp:Table CellPadding="0" CellSpacing="2" runat="server">
												<asp:TableRow>
													<asp:TableCell CssClass="itemtitle">
														<asp:HyperLink Text='<%# DataBinder.Eval(Container.DataItem, "title") %>' NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "link") %>' Target="_new" Runat="server" />
													</asp:TableCell>
												</asp:TableRow>
												<asp:TableRow><asp:TableCell CssClass="itemdate"><%# DataBinder.Eval(Container.DataItem, "pubDate"    ) %></asp:TableCell></asp:TableRow>
												<asp:TableRow><asp:TableCell CssClass="itemdesc"><%# DataBinder.Eval(Container.DataItem, "description") %></asp:TableCell></asp:TableRow>
											</asp:Table>
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
							</asp:DataGrid>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<br>
</div>
