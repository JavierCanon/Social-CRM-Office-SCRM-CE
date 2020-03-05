<%@ Control CodeBehind="AccessView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.ACLRoles.AccessView" %>
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
	<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	<SplendidCRM:ACLGrid id="grdACL" Width="100%" CssClass="tabDetailView"
		CellPadding="0" CellSpacing="1" border="0"
		AllowPaging="false" PageSize="20" AllowSorting="false" 
		AutoGenerateColumns="false" EnableACLEditing="false"
		EnableViewState="true" runat="server">
		<ItemStyle            CssClass="tabDetailViewDF" />
		<AlternatingItemStyle CssClass="tabDetailViewDF" />
		<HeaderStyle          CssClass="tabDetailViewDL" />
	</SplendidCRM:ACLGrid>
</div>
