<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.Administration.ConfigureSettings.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divEditView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Administration" EnablePrint="false" HelpName="EditView" EnableHelp="true" Runat="Server" />
	<p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="EditButtons" Src="~/_controls/EditButtons.ascx" %>
	<SplendidCRM:EditButtons ID="ctlEditButtons" Visible="<%# !PrintView %>" Runat="Server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell ColumnSpan="4"><h4><asp:Label Text='<%# L10n.Term("Administration.LBL_NOTIFY_TITLE") %>' runat="server" /></h4></asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Administration.LBL_NOTIFY_FROMNAME") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField">
							<asp:TextBox ID="txtNOTIFY_FROMNAME" TabIndex="1" size="25" MaxLength="128" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("Administration.LBL_NOTIFY_ON") %></asp:TableCell>
						<asp:TableCell Width="40%" CssClass="dataField" VerticalAlign="top">
							<asp:CheckBox ID="chkNOTIFY_ON" TabIndex="1" CssClass="checkbox" Runat="server" />
							<em><%= L10n.Term("Administration.LBL_NOTIFICATION_ON_DESC") %></em>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Administration.LBL_NOTIFY_FROMADDRESS") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField">
							<asp:TextBox ID="txtNOTIFY_FROMADDRESS" TabIndex="1" size="25" MaxLength="128" Runat="server" />
						</asp:TableCell>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Administration.LBL_NOTIFY_SEND_BY_DEFAULT") %></asp:TableCell>
						<asp:TableCell CssClass="dataField">
							<asp:CheckBox ID="chkNOTIFY_SEND_BY_DEFAULT" TabIndex="1" CssClass="checkbox" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Administration.LBL_MAIL_SENDTYPE") %></asp:TableCell>
						<asp:TableCell CssClass="dataField">
							<asp:DropDownList ID="lstMAIL_SENDTYPE" DataValueField="NAME" DataTextField="DISPLAY_NAME" TabIndex="1" onChange="notify_setrequired(document.ConfigureSettings);" Runat="server" />
						</asp:TableCell>
						<asp:TableCell CssClass="dataLabel"></asp:TableCell>
						<asp:TableCell CssClass="dataField"></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell ColumnSpan="4">
							<div ID="smtp_settings">
								<asp:Table SkinID="tabEditView" runat="server">
									<asp:TableRow>
										<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Administration.LBL_MAIL_SMTPSERVER") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
										<asp:TableCell Width="35%" CssClass="dataField">
											<asp:TextBox ID="txtMAIL_SMTPSERVER" TabIndex="1" size="25" MaxLength="64" Runat="server" />
										</asp:TableCell>
										<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Administration.LBL_MAIL_SMTPPORT") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
										<asp:TableCell Width="35%" CssClass="dataField">
											<asp:TextBox ID="txtMAIL_SMTPPORT" TabIndex="1" size="5" MaxLength="5" Runat="server" />
										</asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Administration.LBL_MAIL_SMTPAUTH_REQ") %></asp:TableCell>
										<asp:TableCell ColumnSpan="3">
											<asp:CheckBox ID="chkMAIL_SMTPAUTH_REQ" TabIndex="1" CssClass="checkbox" onclick="notify_setrequired(document.ConfigureSettings);" Runat="server" />
										</asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell ColumnSpan="4">
											<div ID="smtp_auth">
												<asp:Table SkinID="tabEditView" runat="server">
													<asp:TableRow>
														<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Administration.LBL_MAIL_SMTPUSER") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
														<asp:TableCell Width="35%" CssClass="dataField">
															<asp:TextBox ID="txtMAIL_SMTPUSER" TabIndex="1" size="25" MaxLength="64" Runat="server" />
														</asp:TableCell>
														<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("Administration.LBL_MAIL_SMTPPASS") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
														<asp:TableCell Width="35%" CssClass="dataField">
															<asp:TextBox ID="txtMAIL_SMTPPASS" TextMode="Password" TabIndex="1" size="25" MaxLength="64" Runat="server" />
														</asp:TableCell>
													</asp:TableRow>
												</asp:Table>
											</div>
										</asp:TableCell>
									</asp:TableRow>
								</asp:Table>
							</div>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	<p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell ColumnSpan="4"><h4><asp:Label Text='<%# L10n.Term("Administration.LBL_PORTAL_TITLE") %>' runat="server" /></h4></asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="25%" CssClass="dataLabel" VerticalAlign="middle"><%= L10n.Term("Administration.LBL_PORTAL_ON") %></asp:TableCell>
						<asp:TableCell Width="75%" CssClass="dataField" VerticalAlign="middle">
							<asp:CheckBox ID="chkPORTAL_ON" TabIndex="1" CssClass="checkbox" Runat="server" />
							<em><%= L10n.Term("Administration.LBL_PORTAL_ON_DESC") %></em>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell ColumnSpan="4">
							<div id="portal_config">
								<asp:Table SkinID="tabEditView" runat="server">
									<asp:TableRow>
										<asp:TableCell Width="15%" CssClass="dataLabel">&nbsp;</asp:TableCell>
										<asp:TableCell Width="35%" CssClass="dataField">&nbsp;</asp:TableCell>
									</asp:TableRow>
								</asp:Table>
							</div>
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
</div>
