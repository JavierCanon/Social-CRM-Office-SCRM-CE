<%@ Control Language="c#" AutoEventWireup="false" Codebehind="LoginView.ascx.cs" Inherits="SplendidCRM.Users.LoginView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<script type="text/javascript">
function set_focus()
{
	var user_name     = document.getElementById('<%= txtUSER_NAME.ClientID %>');
	var user_password = document.getElementById('<%= txtPASSWORD.ClientID  %>');
	if ( user_name != null )
	{
		if ( user_name.value != '' )
		{
			user_password.focus();
			user_password.select();
		}
		else
		{
			user_name.focus();
		}
	}
}

function toggleDisplay(id)
{
	var divMore = document.getElementById(id);
	divMore.style.display = (divMore.style.display == 'none') ? 'inline' : 'none';
}
</script>

<style type="text/css">
	.buttonLogin {
		border: 1px solid #444444;
		font-size: 11px;
		color: #ffffff;
		background-color: #666666;
		font-weight: bold;
		}
</style>
<div id="divLoginView">
	<div Visible="<%# !this.IsMobile %>" runat="server">
		<br />
		<br />
	</div>
	<asp:Table SkinID="tabFrame" HorizontalAlign="Center" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table CellPadding="0" CellSpacing="0" HorizontalAlign="Center" style="border: 1px solid #888888;" runat="server">
					<asp:TableRow>
						<asp:TableCell HorizontalAlign="Right" style="padding: 4px; background-color: #ffffff; border-bottom: 1px solid #bbbbbb;">
							SplendidCRM
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell style="background-color: #dddddd; padding: 5px;" align="center">
							<asp:Table Width="100%" BorderWidth="0" CellPadding="0" CellSpacing="2" HorizontalAlign="Center" runat="server">
								<asp:TableRow Visible="<%# !this.IsMobile %>" runat="server">
									<asp:TableCell ColumnSpan="2">
										<h1>SplendidCRM Open-Source</h1>
									</asp:TableCell>
								</asp:TableRow>
								<asp:TableRow>
									<asp:TableCell ColumnSpan="2" style="font-size: 12px; padding-bottom: 5px;">
										<asp:Label ID="lblInstructions" Runat="server"><%# L10n.Term(".NTC_LOGIN_MESSAGE") %></asp:Label>
									</asp:TableCell>
								</asp:TableRow>
								<asp:TableRow ID="trError" Visible="false" runat="server">
									<asp:TableCell Width="30%" CssClass="dataLabel"><%= L10n.Term("Users.LBL_ERROR") %></asp:TableCell>
									<asp:TableCell Width="70%">
										<asp:Label ID="lblError" EnableViewState="false" CssClass="error" Runat="server" />
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
							<asp:Table ID="tblUser" Width="100%" BorderWidth="0" CellPadding="0" CellSpacing="2" HorizontalAlign="Center" Runat="server">
								<asp:TableRow>
									<asp:TableCell Width="30%" CssClass="dataLabel"><%= L10n.Term("Users.LBL_USER_NAME") %></asp:TableCell>
									<asp:TableCell Width="70%">
										<asp:TextBox ID="txtUSER_NAME" size="20" style="width: 125px" Runat="server" /> &nbsp;<%= (Sql.IsEmptyString(Application["CONFIG.default_user_name"]) ? String.Empty : "(" + Sql.ToString(Application["CONFIG.default_user_name"]) + ")") %>
									</asp:TableCell>
								</asp:TableRow>
								<asp:TableRow>
									<asp:TableCell Width="30%" CssClass="dataLabel"><%= L10n.Term("Users.LBL_PASSWORD") %></asp:TableCell>
									<asp:TableCell Width="70%">
										<asp:TextBox ID="txtPASSWORD" size="20" style="width: 125px" TextMode="Password" Runat="server" /> &nbsp;<%= (Sql.IsEmptyString(Application["CONFIG.default_password"]) ? String.Empty : "(" + Sql.ToString(Application["CONFIG.default_password"]) + ")") %>
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
							<asp:Table Width="100%" BorderWidth="0" CellPadding="0" CellSpacing="2" HorizontalAlign="Center" runat="server">
								<asp:TableRow>
									<asp:TableCell Width="30%">&nbsp;</asp:TableCell>
									<asp:TableCell Width="70%">
										<asp:Button ID="btnLogin" CommandName="Login" OnCommand="Page_Command" CssClass="buttonLogin" Text='<%# " "  + L10n.Term("Users.LBL_LOGIN_BUTTON_LABEL") + " "  %>' ToolTip='<%# L10n.Term("Users.LBL_LOGIN_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey("Users.LBL_LOGIN_BUTTON_KEY") %>' Runat="server" />
									</asp:TableCell>
								</asp:TableRow>
								<asp:TableRow Visible="<%# !this.IsMobile %>" runat="server">
									<asp:TableCell ColumnSpan="2" HorizontalAlign="Right">
										<asp:HyperLink NavigateUrl="javascript:toggleDisplay('divMore');" CssClass="utilsLink" runat="server">
											<asp:Image SkinID="advanced_search" runat="server" />&nbsp;<asp:Label Text='<%# L10n.Term("Users.LBL_LOGIN_OPTIONS") %>' runat="server" />
										</asp:HyperLink>
									</asp:TableCell>
								</asp:TableRow>
							</asp:Table>
							<div id="divMore">
								<asp:Table Width="100%" BorderWidth="0" CellPadding="0" CellSpacing="2" HorizontalAlign="Center" Visible="<%# !this.IsMobile %>" runat="server">
									<asp:TableRow>
										<asp:TableCell Width="30%" CssClass="dataLabel"><%= L10n.Term("Users.LBL_THEME") %></asp:TableCell>
										<asp:TableCell Width="70%">
											<asp:DropDownList ID="lstTHEME" DataValueField="NAME" DataTextField="NAME" Runat="server" />
										</asp:TableCell>
									</asp:TableRow>
									<asp:TableRow Visible="false" runat="server">
										<asp:TableCell Width="30%" CssClass="dataLabel"><%= L10n.Term("Users.LBL_LANGUAGE") %></asp:TableCell>
										<asp:TableCell Width="70%">
											<!--  // 01/20/2006 Paul.  The language setting is now in User Settings. -->
											<asp:DropDownList ID="lstLANGUAGE" DataValueField="NAME" DataTextField="NATIVE_NAME" Runat="server" />
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
	<br />
	<br />
<%
if ( tblUser.Visible )
{
	Response.Write(Utils.RegisterEnterKeyPress(txtUSER_NAME.ClientID, btnLogin.ClientID));
	Response.Write(Utils.RegisterEnterKeyPress(txtPASSWORD.ClientID , btnLogin.ClientID));
}
%>
</div>
