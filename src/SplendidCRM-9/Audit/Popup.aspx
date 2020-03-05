<%@ Page language="c#" MasterPageFile="~/PopupView.Master" Codebehind="Popup.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Audit.Popup" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005-2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
</script>
<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
	<p>
	<table width="100%" border="0" cellpadding="0" cellspacing="0" class="moduleTitle">
		<tr>
			<td valign="top"><asp:Image ImageUrl='<%# Session["themeURL"] + "images/" + sModule +".gif" %>' AlternateText='<%# L10n.Term(".moduleList." + sModule) %>' BorderWidth="0" Width="16" Height="16" style="margin-top: 3px" Runat="server" />&nbsp;</td>
			<td width="100%"><h2><asp:Label ID="lblTitle" Runat="server" /></h2></td>
			<td valign="top" align="right" style="padding-top:3px; padding-left: 5px;" nowrap>
				<asp:ImageButton OnClientClick="print(); return false;" CssClass="utilsLink" AlternateText='<%# L10n.Term(".LNK_PRINT") %>' ImageUrl='<%# Session["themeURL"] + "images/print.gif" %>' BorderWidth="0" Width="13" Height="13" ImageAlign="AbsMiddle" Runat="server" />
				<asp:LinkButton  OnClientClick="print(); return false;" CssClass="utilsLink" Text='<%# L10n.Term(".LNK_PRINT") %>' Runat="server" />
			</td>
		</tr>
	</table>
	</p>

	<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	<SplendidCRM:SplendidGrid id="grdMain" Width="100%" CssClass="listView"
		CellPadding="3" CellSpacing="0" border="0"
		AllowPaging="true" PageSize="20" AllowSorting="true" 
		AutoGenerateColumns="false" 
		EnableViewState="true" runat="server">
		<ItemStyle            CssClass="oddListRowS1"  />
		<AlternatingItemStyle CssClass="evenListRowS1" />
		<HeaderStyle          CssClass="listViewThS1"  />
		<PagerStyle HorizontalAlign="Right" Mode="NextPrev" PageButtonCount="6" Position="Top" CssClass="listViewPaginationTdS1" PrevPageText=".LNK_LIST_PREVIOUS" NextPageText=".LNK_LIST_NEXT" />
		<Columns>
			<asp:BoundColumn HeaderText="Audit.LBL_FIELD_NAME" DataField="FIELD_NAME"   ItemStyle-Width="5%"  />
			<asp:BoundColumn HeaderText="Audit.LBL_OLD_NAME"   DataField="BEFORE_VALUE" ItemStyle-Width="40%" />
			<asp:BoundColumn HeaderText="Audit.LBL_NEW_VALUE"  DataField="AFTER_VALUE"  ItemStyle-Width="40%" />
			<asp:BoundColumn HeaderText="Audit.LBL_CREATED_BY" DataField="CREATED_BY"   ItemStyle-Width="5%"  />
			<asp:BoundColumn HeaderText="Audit.LBL_LIST_DATE"  DataField="DATE_CREATED" ItemStyle-Width="10%" ItemStyle-Wrap="false" />
		</Columns>
	</SplendidCRM:SplendidGrid>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</asp:Content>
