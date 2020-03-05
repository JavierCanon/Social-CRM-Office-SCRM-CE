<%@ Control Language="c#" AutoEventWireup="false" Codebehind="PostView.ascx.cs" Inherits="SplendidCRM.Threads.PostView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divPostView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="PostButtons" Src="_controls/PostButtons.ascx" %>
	<SplendidCRM:PostButtons ID="ctlPostButtons" Visible="<%# !PrintView %>" Runat="Server" />

	<asp:HiddenField ID="txtPOST_ID" runat="server" />
	<table ID="tblMain" class="tabDetailView" runat="server">
		<tr>
			<td class="tabDetailViewDF"><asp:Label ID="txtTITLE" Font-Bold="true" runat="server" /></td>
			<td width="10%" class="tabDetailViewDL"><asp:Label ID="txtCREATED_BY" runat="server" />:</td>
			<td width="10%" class="tabDetailViewDL" nowrap><asp:Label ID="txtDATE_ENTERED" runat="server" /></td>
		</tr>
		<tr>
			<td colspan="3" style="background-color: #ffffff; padding-left: 3mm; padding-right: 3mm; ">
				<asp:Literal ID="txtDESCRIPTION" runat="server" />
			</td>
		</tr>
		<tr id="trModified" visible="false" runat="server">
			<td class="tabDetailViewDF">&nbsp;</td>
			<td width="10%" class="tabDetailViewDL" nowrap><%# L10n.Term("Threads.LBL_MODIFIED_BY") %>&nbsp;<asp:Label ID="txtMODIFIED_BY" runat="server" />:</td>
			<td width="10%" class="tabDetailViewDL" nowrap><asp:Label ID="txtDATE_MODIFIED" runat="server" /></td>
		</tr>
	</table>
	<br />
</div>
