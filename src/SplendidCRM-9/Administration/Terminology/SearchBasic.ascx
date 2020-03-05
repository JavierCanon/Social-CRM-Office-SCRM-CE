<%@ Control Language="c#" AutoEventWireup="false" Codebehind="SearchBasic.ascx.cs" Inherits="SplendidCRM.Administration.Terminology.SearchBasic" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
<SplendidCRM:ListHeader Title="Terminology.LBL_SEARCH_FORM_TITLE" Runat="Server" />
<div id="divSearch">
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabSearchView" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="20%" CssClass="dataLabel"><%= L10n.Term("Terminology.LBL_NAME"        ) %></asp:TableCell>
						<asp:TableCell Width="25%" CssClass="dataField"><asp:TextBox ID="txtNAME"         TabIndex="1" Size="25" MaxLength="50" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="20%" CssClass="dataLabel"><%= L10n.Term("Terminology.LBL_LANG"       ) %></asp:TableCell>
						<asp:TableCell Width="25%" CssClass="dataField"><asp:DropDownList ID="lstLANGUAGE" TabIndex="2" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Terminology.LBL_MODULE_NAME") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:DropDownList ID="lstMODULE_NAME" TabIndex="4" DataValueField="MODULE_NAME" DataTextField="MODULE_NAME" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel"><asp:CheckBox ID="chkGLOBAL_TERMS" TabIndex="5" OnCheckedChanged="chkGLOBAL_TERMS_CheckedChanged" AutoPostBack="True" CssClass="checkbox" Runat="server" />&nbsp;<%= L10n.Term("Terminology.LBL_GLOBAL_TERMS") %></asp:TableCell>
						<asp:TableCell CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Terminology.LBL_LIST_NAME_LABEL") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:DropDownList ID="lstLIST_NAME" TabIndex="6" DataValueField="LIST_NAME" DataTextField="LIST_NAME" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel"><asp:CheckBox ID="chkINCLUDE_LISTS" TabIndex="7" OnCheckedChanged="chkINCLUDE_LISTS_CheckedChanged" AutoPostBack="True" CssClass="checkbox" Runat="server" />&nbsp;<%= L10n.Term("Terminology.LBL_INCLUDE_LISTS") %></asp:TableCell>
						<asp:TableCell CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Terminology.LBL_DISPLAY_NAME") %></asp:TableCell>
						<asp:TableCell CssClass="dataField" ColumnSpan="3"><asp:TextBox ID="txtDISPLAY_NAME" TabIndex="3" TextMode="MultiLine" Columns="90" Rows="2" Runat="server" /></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchButtons" Src="~/_controls/SearchButtons.ascx" %>
	<SplendidCRM:SearchButtons ID="ctlSearchButtons" Visible="<%# !PrintView %>" Runat="Server" />
	<%= Utils.RegisterEnterKeyPress(txtNAME.ClientID        , ctlSearchButtons.SearchClientID) %>
	<%= Utils.RegisterEnterKeyPress(txtDISPLAY_NAME.ClientID, ctlSearchButtons.SearchClientID) %>
</div>
