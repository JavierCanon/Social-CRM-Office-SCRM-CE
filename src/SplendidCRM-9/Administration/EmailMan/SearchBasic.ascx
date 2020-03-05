<%@ Control Language="c#" AutoEventWireup="false" Codebehind="SearchBasic.ascx.cs" Inherits="SplendidCRM.Administration.EmailMan.SearchBasic" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
%>
<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
<SplendidCRM:ListHeader Title="EmailMan.LBL_SEARCH_FORM_TITLE" Runat="Server" />
<div id="divSearch">
	<table border="0" cellpadding="0" cellspacing="0" width="100%" class="tabForm">
		<tr>
			<td>
				<table width="100%" border="0" cellspacing="0" cellpadding="0">
					<tr>
						<td width="10%" class="dataLabel" align="right" nowrap><%= L10n.Term("EmailMan.LBL_LIST_CAMPAIGN"       ) %></td>
						<td width="15%" class="dataField" align="left"  nowrap><asp:TextBox ID="txtCAMPAIGN_NAME"   TabIndex="1" Size="25" MaxLength="50" Runat="server" /></td>
						<td width="15%" class="dataLabel" align="right" nowrap><%= L10n.Term("EmailMan.LBL_LIST_RECIPIENT_NAME" ) %></td>
						<td width="15%" class="dataField" align="left"  nowrap><asp:TextBox ID="txtRECIPIENT_NAME"  TabIndex="1" Size="25" MaxLength="100" Runat="server" /></td>
						<td width="15%" class="dataLabel" align="right" nowrap><%= L10n.Term("EmailMan.LBL_LIST_RECIPIENT_EMAIL") %></td>
						<td width="15%" class="dataField" align="left"  nowrap><asp:TextBox ID="txtRECIPIENT_EMAIL" TabIndex="1" Size="25" MaxLength="100" Runat="server" /></td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchButtons" Src="~/_controls/SearchButtons.ascx" %>
	<SplendidCRM:SearchButtons ID="ctlSearchButtons" Visible="<%# !PrintView %>" Runat="Server" />
	<%= Utils.RegisterEnterKeyPress(txtCAMPAIGN_NAME.ClientID  , ctlSearchButtons.SearchClientID) %>
	<%= Utils.RegisterEnterKeyPress(txtRECIPIENT_NAME.ClientID , ctlSearchButtons.SearchClientID) %>
	<%= Utils.RegisterEnterKeyPress(txtRECIPIENT_EMAIL.ClientID, ctlSearchButtons.SearchClientID) %>
</div>
