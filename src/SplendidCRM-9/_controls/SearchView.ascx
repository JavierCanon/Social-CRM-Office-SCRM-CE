<%@ Control Language="c#" AutoEventWireup="false" Codebehind="SearchView.ascx.cs" Inherits="SplendidCRM._controls.SearchView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="SplendidCRM._controls" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
</script>
<div id="divSearchView">
	<script type="text/javascript">
		function ToggleSearch()
		{
			// 01/24/2008 Paul.  On PocketPC and SmartPhone, getElementById is not available. 
			var divSearch;
			if ( document.getElementById == undefined )
				divSearch = <%= pnlSearchPanel.ClientID %>;
			else
				divSearch = document.getElementById('<%= pnlSearchPanel.ClientID %>');
			if ( divSearch.style.display == 'inline' )
				divSearch.style.display = 'none';
			else
				divSearch.style.display = 'inline';
		}
		function ToggleSavedSearch()
		{
			var divSavedSearchPanel = document.getElementById('<%= pnlSavedSearchPanel.ClientID %>');
			var imgBasicSearch      = document.getElementById('<%= imgBasicSearch     .ClientID %>');
			var imgAdvancedSearch   = document.getElementById('<%= imgAdvancedSearch  .ClientID %>');
			if ( divSavedSearchPanel.style.display == 'inline' )
			{
				divSavedSearchPanel.style.display = 'none';
				imgBasicSearch.style.display      = 'none';
				imgAdvancedSearch.style.display   = 'inline';
			}
			else
			{
				divSavedSearchPanel.style.display = 'inline';
				imgBasicSearch.style.display      = 'inline';
				imgAdvancedSearch.style.display   = 'none';
			}
		}
		function ToggleUnassignedOnly()
		{
			var sASSIGNED_USER_ID = '<%= new DynamicControl(this, "ASSIGNED_USER_ID").ClientID %>';
			var sUNASSIGNED_ONLY  = '<%= new DynamicControl(this, "UNASSIGNED_ONLY" ).ClientID %>';
			if ( sASSIGNED_USER_ID.length > 0 && sUNASSIGNED_ONLY.length > 0 )
			{
				var lstASSIGNED_USER_ID = document.getElementById(sASSIGNED_USER_ID);
				var chkUNASSIGNED_ONLY  = document.getElementById(sUNASSIGNED_ONLY );
				if ( lstASSIGNED_USER_ID != null && chkUNASSIGNED_ONLY != null )
					lstASSIGNED_USER_ID.disabled = chkUNASSIGNED_ONLY.checked;
			}
		}
	</script>

	<asp:Panel ID="pnlMobileButtons" Visible="<%# !IsPopupSearch && IsMobile %>" runat="server">
		&nbsp;<asp:HyperLink ID="lnkSearch" NavigateUrl="javascript:ToggleSearch();void(null);" Text='<%# L10n.Term(".LBL_SEARCH_BUTTON_LABEL") %>' runat="server" />
		&nbsp;<a href="edit.aspx"><%# L10n.Term(".LBL_NEW_BUTTON_LABEL") %></a>
	</asp:Panel>
	<asp:Panel ID="pnlSearchPanel" style='<%# (!IsPopupSearch && IsMobile) ? "display: none" : "display: inline" %>' runat="server">
		<asp:Panel ID="pnlSearchTabs" Visible="<%# ShowSearchTabs && !IsMobile %>" runat="server">
			<ul class="tablist">
				<li><asp:HyperLink ID="lnkBasicSearch"    Text='<%# L10n.Term(".LNK_BASIC_SEARCH"   ) %>' CssClass=""        Runat="server" /></li>
				<li><asp:HyperLink ID="lnkAdvancedSearch" Text='<%# L10n.Term(".LNK_ADVANCED_SEARCH") %>' CssClass="current" Runat="server" /></li>
			</ul>
		</asp:Panel>
		
		<asp:Table SkinID="tabForm" runat="server">
			<asp:TableRow>
				<asp:TableCell>
					<table id="tblSearch" class="tabSearchView" runat="server">
					</table>
					<asp:HyperLink NavigateUrl="javascript:ToggleSavedSearch();void(0);" Visible='<%# ShowSearchViews && !IsPopupSearch && !IsMobile %>' runat="server">
						<asp:Image ID="imgBasicSearch"    SkinID="basic_search"    BorderWidth="0" style="display: none;"   runat="server" />
						<asp:Image ID="imgAdvancedSearch" SkinID="advanced_search" BorderWidth="0" style="display: inline;" runat="server" />
						<asp:Literal Text="&nbsp;" runat="server" />
						<%# L10n.Term(".LNK_SAVED_VIEWS") %>
					</asp:HyperLink>
					<asp:Literal Text="&nbsp;" runat="server" />
					<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" runat="server" />
					<asp:Panel ID="pnlSavedSearchPanel" style="display: none;" runat="server">
						<asp:Table Width="100%" CellPadding="0" CellSpacing="1" runat="server">
							<asp:TableRow>
								<asp:TableCell Width="15%"><asp:Label Text='<%# L10n.Term("SavedSearch.LBL_ORDER_BY_COLUMNS") %>' runat="server" /></asp:TableCell>
								<asp:TableCell Width="35%"><asp:DropDownList ID="lstColumns" DataValueField="NAME" DataTextField="DISPLAY_NAME" runat="server" /></asp:TableCell>
								<asp:TableCell Width="15%"><asp:Label Text='<%# L10n.Term("SavedSearch.LBL_DIRECTION") %>' runat="server" /></asp:TableCell>
								<asp:TableCell Width="35%">
									<asp:RadioButton ID="radSavedSearchDESC" GroupName="SavedSearchDirection" Text='<%# L10n.Term("SavedSearch.LBL_DESCENDING") %>' CssClass="checkbox" runat="server" /><br />
									<asp:RadioButton ID="radSavedSearchASC"  GroupName="SavedSearchDirection" Text='<%# L10n.Term("SavedSearch.LBL_ASCENDING" ) %>' CssClass="checkbox" Checked="true" runat="server" /><br />
								</asp:TableCell>
							</asp:TableRow>
							<asp:TableRow>
								<asp:TableCell Width="15%"><asp:Label Text='<%# L10n.Term("SavedSearch.LBL_SAVE_SEARCH_AS") %>' runat="server" /></asp:TableCell>
								<asp:TableCell Width="35%">
									<asp:TextBox ID="txtSavedSearchName" runat="server" />
									<asp:Literal Text="&nbsp;" runat="server" />
									<asp:Button ID="btnSavedSearchSave" CommandName="SavedSearch.Save" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_SAVE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_SAVE_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_BUTTON_KEY") %>' Runat="server" />
									<asp:Label ID="lblSavedNameRequired" Visible="false" Text='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableViewState="false" runat="server" />
								</asp:TableCell>
								<asp:TableCell Width="15%">
									<asp:Label Text='<%# L10n.Term("SavedSearch.LBL_MODIFY_CURRENT_SEARCH") %>' runat="server" />
								</asp:TableCell>
								<asp:TableCell Width="35%">
									<asp:Button ID="btnSavedSearchUpdate" CommandName="SavedSearch.Update" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_UPDATE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_UPDATE_BUTTON_TITLE") %>' Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
									<asp:Button ID="btnSavedSearchDelete" CommandName="SavedSearch.Delete" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_DELETE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_DELETE_BUTTON_TITLE") %>' Runat="server" /><asp:Literal Text="&nbsp;" runat="server" />
									<asp:Label ID="lblCurrentSearch" runat="server" />
								</asp:TableCell>
							</asp:TableRow>
						</asp:Table>
					</asp:Panel>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
		<asp:Panel ID="pnlSearchButtons" CssClass="button-panel" runat="server">
			<asp:Table ID="tblSearchButtons" Width="100%" CellPadding="0" CellSpacing="0" style="padding-top: 8px;" runat="server">
				<asp:TableRow>
					<asp:TableCell>
						<asp:Button ID="btnSearch" CommandName="Search" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_SEARCH_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_SEARCH_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SEARCH_BUTTON_KEY") %>' Runat="server" /><asp:Literal ID="Literal1" Text="&nbsp;" runat="server" />
						<asp:Button ID="btnClear"  CommandName="Clear"  OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_CLEAR_BUTTON_LABEL" ) %>' ToolTip='<%# L10n.Term(".LBL_CLEAR_BUTTON_TITLE" ) %>' AccessKey='<%# L10n.AccessKey(".LBL_CLEAR_BUTTON_KEY" ) %>' Runat="server" />

						<span Visible="<%# ShowSearchViews && !IsPopupSearch %>" runat="server">
							<asp:Label CssClass="white-space" Text="&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;" runat="server" />
							<asp:Label Font-Bold="true" Text='<%# L10n.Term(".LBL_SAVED_SEARCH_SHORTCUT" ) %>' Visible='<%# !IsMobile %>' runat="server" />
							<asp:DropDownList ID="lstSavedSearches" DataValueField="ID" DataTextField="NAME" OnSelectedIndexChanged="lstSavedSearches_Changed" AutoPostBack="true" runat="server" />
							<asp:Label ID="lblCurrentXML" Visible="<%# bDebug %>" runat="server" />
						</span>
					</asp:TableCell>
				</asp:TableRow>
			</asp:Table>
		</asp:Panel>
	</asp:Panel>

	<%
	if ( bRegisterEnterKeyPress && dtFields != null )
	{
		foreach(DataRowView row in dtFields.DefaultView)
		{
			string sFIELD_TYPE = Sql.ToString (row["FIELD_TYPE"]);
			string sDATA_FIELD = Sql.ToString (row["DATA_FIELD"]);
			DynamicControl ctl = new DynamicControl(this, sDATA_FIELD);
			if ( ctl != null )
			{
				if ( sFIELD_TYPE == "TextBox" )
				{
					ctl.Text = String.Empty;
					Response.Write(Utils.RegisterEnterKeyPress(ctl.ClientID, btnSearch.ClientID));
				}
				else if ( sFIELD_TYPE == "DateRange" )
				{
					// 01/17/2008 Paul.  We need to register the date text fields otherwise an Enter key 
					// will cause the search view not to render.  We should also investigate an alternative 
					// just in case the enter occurs elsewhere. 
					SplendidCRM._controls.DatePicker ctlDateStart = FindControl(sDATA_FIELD + "_AFTER" ) as SplendidCRM._controls.DatePicker;
					SplendidCRM._controls.DatePicker ctlDateEnd   = FindControl(sDATA_FIELD + "_BEFORE") as SplendidCRM._controls.DatePicker;
					Response.Write(Utils.RegisterEnterKeyPress(ctlDateStart.DateClientID, btnSearch.ClientID));
					Response.Write(Utils.RegisterEnterKeyPress(ctlDateEnd  .DateClientID, btnSearch.ClientID));
				}
			}
		}
	}
	%>
</div>
