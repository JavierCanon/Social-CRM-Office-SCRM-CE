<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.ListView" %>
<%@ Import Namespace="SplendidCRM.Crm" %>
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
<div id="divListView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Administration" Title="Administration.LBL_MODULE_TITLE" EnableModuleLabel="false" EnablePrint="false" HelpName="index" EnableHelp="true" Runat="Server" />
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>

	<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	<p>
	<SplendidCRM:ListHeader Title="Administration.LBL_SPLENDIDCRM_NETWORK_TITLE" Runat="Server" />
	<asp:Table ID="Table1" Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Wrap="false">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/sugarupdate.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_SPLENDIDCRM_UPDATE_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_SPLENDIDCRM_UPDATE_TITLE") %>' NavigateUrl="~/Administration/Updater/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_SPLENDIDCRM_UPDATE") %></asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/OnlineDocumentation.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_DOCUMENTATION_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:LinkButton Text='<%# L10n.Term("Administration.LBL_DOCUMENTATION_TITLE") %>' OnClientClick=<%# "window.open('http://www.sugarcrm.com/network/help.php?version=" + Sql.ToString(Application["CONFIG.sugar_version"]) + "&edition=OS&lang=" + Sql.ToString(Session["USER_SETTINGS/CULTURE"]).Replace("-", "_") + "', 'helpwin', 'width=600,height=600,status=0,resizable=1,scrollbars=1,toolbar=0,location=0'); return false;" %> CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_DOCUMENTATION") %></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	<p>
	<SplendidCRM:ListHeader Title="Administration.LBL_ADMINISTRATION_HOME_TITLE" Runat="Server" />
	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Wrap="false">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Administration.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_CONFIGURE_SETTINGS_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_CONFIGURE_SETTINGS_TITLE") %>' NavigateUrl="~/Administration/Config/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_CONFIGURE_SETTINGS") %></asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Upgrade.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_UPGRADE_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_SYSTEM_CHECK_TITLE") %>' NavigateUrl="~/SystemCheck.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
				<br />
				<div align="center">
				(
				<asp:LinkButton Text="Reload"     CommandName="System.Reload" OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				&nbsp;
				<asp:HyperLink  Text='Precompile' NavigateUrl="~/_devtools/Precompile.aspx" CssClass="tabDetailViewDL2Link" Target="PrecompileSplendidCRM" Runat="server" />
				)
				</div>
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_SYSTEM_CHECK") %></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Currencies.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_MANAGE_CURRENCIES") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MANAGE_CURRENCIES") %>' NavigateUrl="~/Administration/Currencies/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_CURRENCY") %></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Schema.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_SYNC_SCHEMA_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_SYNC_SCHEMA_TITLE") %>' NavigateUrl="~/Administration/SyncSchema/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_SYNC_SCHEMA") %></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Import.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_IMPORT_DATABASE_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_IMPORT_DATABASE_TITLE") %>' NavigateUrl="~/Administration/Import/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_IMPORT_DATABASE") %></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Export.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_EXPORT_DATABASE_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_EXPORT_DATABASE_TITLE") %>' NavigateUrl="~/Administration/Export/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_EXPORT_DATABASE") %></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Schedulers.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_SUGAR_SCHEDULER_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_SUGAR_SCHEDULER_TITLE") %>' NavigateUrl="~/Administration/Schedulers/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_SUGAR_SCHEDULER") %></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Backups.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_BACKUPS_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_BACKUPS_TITLE") %>' NavigateUrl="~/Administration/Backups/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_BACKUPS") %></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>

	<p>
	<SplendidCRM:ListHeader Title="Administration.LBL_USERS_TITLE" Runat="Server" />
	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Users.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_MANAGE_USERS_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MANAGE_USERS_TITLE") %>' NavigateUrl="~/Users/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
				<br />
				<div align="center">
				(
				<asp:LinkButton ID="btnUserRequired" Visible='<%# !Config.require_user_assignment() %>' Text="Require"  CommandName="UserAssignement.Require"  OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				&nbsp;
				<asp:LinkButton ID="btnUserOptional" Visible='<%#  Config.require_user_assignment() %>' Text="Optional" CommandName="UserAssignement.Optional" OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				)
				</div>
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2">
				<%= L10n.Term("Administration.LBL_MANAGE_USERS") %><br />
				User Assignment is <%# Config.require_user_assignment() ? "Required" : "Optional" %>
			</asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Roles.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_MANAGE_ROLES_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MANAGE_ROLES_TITLE") %>' NavigateUrl="~/Administration/ACLRoles/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_MANAGE_ROLES") %></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Teams.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_TEAMS_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_TEAMS_TITLE") %>' NavigateUrl="~/Administration/Teams/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
				<br />
				<div align="center">
				(
				<asp:LinkButton ID="btnTeamsEnable"   Visible='<%# !Config.enable_team_management() %>' Text="Enable"  CommandName="Teams.Enable"   OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				&nbsp;
				<asp:LinkButton ID="btnTeamsDisable"  Visible='<%#  Config.enable_team_management() %>' Text="Disable" CommandName="Teams.Disable"  OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				&nbsp;
				<asp:LinkButton ID="btnTeamsRequired" Visible='<%# !Config.require_team_management() && Config.enable_team_management() %>' Text="Require"  CommandName="Teams.Require"  OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				&nbsp;
				<asp:LinkButton ID="btnTeamsOptional" Visible='<%#  Config.require_team_management() && Config.enable_team_management() %>' Text="Optional" CommandName="Teams.Optional" OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				)
				</div>
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2">
				<%= L10n.Term("Administration.LBL_TEAMS_DESC") %><br />
				Currently <%# Config.enable_team_management() ? "Enabled" : "Disabled" %> <%# Config.require_team_management() && Config.enable_team_management() ? " and Required" : "" %>
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDL2"></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>

	<p>
	<SplendidCRM:ListHeader Title="Administration.LBL_STUDIO_TITLE" Runat="Server" />
	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2" Wrap="false">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Layout.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_MANAGE_LAYOUT") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MANAGE_LAYOUT") %>' NavigateUrl="~/Administration/DynamicLayout/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_LAYOUT") %></asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Dropdown.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_CONFIGURE_SETTINGS_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_DROPDOWN_EDITOR") %>' NavigateUrl="~/Administration/Dropdown/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.DESC_DROPDOWN_EDITOR") %></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/FieldLabels.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_EDIT_CUSTOM_FIELDS") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_EDIT_CUSTOM_FIELDS") %>' NavigateUrl="~/Administration/EditCustomFields/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
				<br />
				<div align="center">
				(
				<asp:LinkButton Text="Recompile"     CommandName="System.RecompileViews" OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				&nbsp;
				<asp:LinkButton Text="Rebuild Audit" CommandName="System.RebuildAudit"   OnCommand="Page_Command" CssClass="tabDetailViewDL2Link" Runat="server" />
				)
				</div>
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.DESC_EDIT_CUSTOM_FIELDS") %></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/ConfigureTabs.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_CONFIGURE_TABS") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_CONFIGURE_TABS") %>' NavigateUrl="~/Administration/ConfigureTabs/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_CHOOSE_WHICH") %></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/RenameTabs.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_CONFIGURE_TABS") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_RENAME_TABS") %>' NavigateUrl="~/Administration/RenameTabs/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_CHANGE_NAME_TABS") %></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/iFrames.gif" %>' AlternateText='<%# L10n.Term("Administration.DESC_IFRAME") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_IFRAME") %>' NavigateUrl="~/iFrames/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.DESC_IFRAME") %></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Terminology.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_MANAGE_TERMINOLOGY_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MANAGE_TERMINOLOGY_TITLE") %>' NavigateUrl="~/Administration/Terminology/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_MANAGE_TERMINOLOGY") %></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Terminology.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_IMPORT_TERMINOLOGY_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_IMPORT_TERMINOLOGY_TITLE") %>' NavigateUrl="~/Administration/Terminology/Import/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_IMPORT_TERMINOLOGY") %></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Shortcuts.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_MANAGE_SHORTCUTS_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MANAGE_SHORTCUTS_TITLE") %>' NavigateUrl="~/Administration/Shortcuts/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_MANAGE_SHORTCUTS") %></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDL2"></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>

	<p>
	<SplendidCRM:ListHeader Title="Administration.LBL_PRODUCTS_QUOTES_TITLE" Runat="Server" />
	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/ProductTemplates.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_PRODUCT_TEMPLATES_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_PRODUCT_TEMPLATES_TITLE") %>' NavigateUrl="~/Administration/ProductTemplates/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_PRODUCT_TEMPLATES_DESC") %></asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Manufacturers.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_MANUFACTURERS_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MANUFACTURERS_TITLE") %>' NavigateUrl="~/Administration/Manufacturers/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_MANUFACTURERS_DESC") %></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/ProductCategories.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_PRODUCT_CATEGORIES_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_PRODUCT_CATEGORIES_TITLE") %>' NavigateUrl="~/Administration/ProductCategories/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_PRODUCT_CATEGORIES_DESC") %></asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Shippers.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_SHIPPERS_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_SHIPPERS_TITLE") %>' NavigateUrl="~/Administration/Shippers/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_SHIPPERS_DESC") %></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/ProductTypes.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_PRODUCT_TYPES_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_PRODUCT_TYPES_TITLE") %>' NavigateUrl="~/Administration/ProductTypes/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_PRODUCT_TYPES_DESC") %></asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/TaxRates.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_TAX_RATES_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_TAX_RATES_TITLE") %>' NavigateUrl="~/Administration/TaxRates/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_TAX_RATES_DESC") %></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>

	<p>
	<SplendidCRM:ListHeader Title="Administration.LBL_EMAIL_TITLE" Runat="Server" />
	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/EmailMan.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_MASS_EMAIL_CONFIG_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MASS_EMAIL_CONFIG_TITLE") %>' NavigateUrl="~/Administration/EmailMan/config.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_MASS_EMAIL_CONFIG_DESC") %></asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/EmailMan.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_MASS_EMAIL_MANAGER_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MASS_EMAIL_MANAGER_TITLE") %>' NavigateUrl="~/Administration/EmailMan/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_MASS_EMAIL_MANAGER_DESC") %></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/InboundEmail.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_INBOUND_EMAIL_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_INBOUND_EMAIL_TITLE") %>' NavigateUrl="~/Administration/InboundEmail/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_MAILBOX_DESC") %></asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Campaigns.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_CAMPAIGN_EMAIL_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_CAMPAIGN_EMAIL_TITLE") %>' NavigateUrl="~/Administration/EmailMan/edit.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_CAMPAIGN_EMAIL_DESC") %></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>

	<p>
	<SplendidCRM:ListHeader Title="Administration.LBL_BUG_TITLE" Runat="Server" />
	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Releases.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_MANAGE_RELEASES") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_MANAGE_RELEASES") %>' NavigateUrl="~/Administration/Releases/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_RELEASE") %></asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">&nbsp;</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2">&nbsp;</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>

	<!--
	<p>
	<SplendidCRM:ListHeader Title="Administration.LBL_FORECASTS_TITLE" Runat="Server" />
	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/TimePeriods.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_TIME_PERIODS_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_TIME_PERIODS_TITLE") %>' NavigateUrl="~/Administration/TimePeriods/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_TIME_PERIODS_DESC") %></asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">&nbsp;</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2">&nbsp;</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	-->

	<p>
	<SplendidCRM:ListHeader Title="Administration.LBL_CONTRACTS_TITLE" Runat="Server" />
	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/Contracts.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_CONTRACT_TYPES_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_CONTRACT_TYPES_TITLE") %>' NavigateUrl="~/Administration/ContractTypes/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_CONTRACT_TYPES_DESC") %></asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">&nbsp;</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2">&nbsp;</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>

	<p>
	<SplendidCRM:ListHeader Title="Administration.LBL_FORUM_TOPICS_TITLE" Runat="Server" />
	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/ForumTopics.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_FORUM_TOPICS") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Text='<%# L10n.Term("Administration.LBL_FORUM_TOPICS") %>' NavigateUrl="~/Administration/ForumTopics/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_FORUM_TOPICS_DESC") %></asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">&nbsp;</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2">&nbsp;</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>

	<!--
	<p>
	<SplendidCRM:ListHeader Title="Administration.LBL_MASS_EMAIL_MANAGER_TITLE" Runat="Server" />
	<asp:Table Width="100%" CssClass="tabDetailView2" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">
				<asp:Image ImageUrl='<%# Session["themeURL"] + "images/EmailMan.gif" %>' AlternateText='<%# L10n.Term("Administration.LBL_MASS_EMAIL_MANAGER_TITLE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
				&nbsp;
				<asp:HyperLink Enabled="false" Text='<%# L10n.Term("Administration.LBL_MASS_EMAIL_MANAGER_TITLE") %>' NavigateUrl="~/Administration/EmailMan/default.aspx" CssClass="tabDetailViewDL2Link" Runat="server" />
			</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF2"><%= L10n.Term("Administration.LBL_MASS_EMAIL_MANAGER_DESC") %></asp:TableCell>
			<asp:TableCell Width="20%" CssClass="tabDetailViewDL2">&nbsp;</asp:TableCell>
			<asp:TableCell Width="30%" CssClass="tabDetailViewDF2">&nbsp;</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	-->
</div>
