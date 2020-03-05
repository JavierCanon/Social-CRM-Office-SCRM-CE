<%@ Page language="c#" MasterPageFile="~/PopupView.Master" Codebehind="PopupMultiSelect.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Teams.PopupMultiSelect" %>
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
	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchBasic" Src="SearchBasic.ascx" %>
	<SplendidCRM:SearchBasic ID="ctlSearch" Runat="Server" />
	<br />

<script type="text/javascript">
function SelectTeam(sPARENT_ID, sPARENT_NAME)
{
	if ( window.opener != null && window.opener.ChangeTeam != null )
	{
		window.opener.ChangeTeam(sPARENT_ID, sPARENT_NAME);
		window.close();
	}
	else
	{
		alert('Original window has closed.  Team cannot be assigned.' + '\n' + sPARENT_ID + '\n' + sPARENT_NAME);
	}
}
function SelectTeams()
{
	if ( window.opener != null && window.opener.ChangeTeam != null )
	{
		var sTeams = '';
		for ( var i = 0 ; i < document.all.length ; i++ )
		{
			if ( document.all[i].name == 'chkMain' )
			{
				if ( document.all[i].checked )
				{
					if ( sTeams.length > 0 )
						sTeams += ',';
					sTeams += document.all[i].value;
				}
			}
		}
		window.opener.ChangeTeam(sTeams, '');
		window.close();
	}
	else
	{
		alert('Original window has closed.  Team cannot be assigned.');
	}
}
function Cancel()
{
	window.close();
}
</script>
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Teams.LBL_LIST_FORM_TITLE" Runat="Server" />
	<asp:Panel ID="pnlPopupButtons" CssClass="button-panel" runat="server">
		<asp:Button ID="btnPopupSelect" OnClientClick="SelectTeams(); return false;" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SELECT_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_SELECT_BUTTON_TITLE") %>' runat="server" />
		<asp:Button ID="btnPopupCancel" OnClientClick="Cancel(); return false;"      CssClass="button" Text='<%# "  " + L10n.Term(".LBL_DONE_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_DONE_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_DONE_BUTTON_KEY") %>' runat="server" />
		<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	</asp:Panel>
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
			<asp:TemplateColumn HeaderText="" ItemStyle-Width="2%">
				<ItemTemplate>
					<input name="chkMain" class="checkbox" type="checkbox" value="<%# DataBinder.Eval(Container.DataItem, "ID") %>" />
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</SplendidCRM:SplendidGrid>
	<%@ Register TagPrefix="SplendidCRM" Tagname="CheckAll" Src="~/_controls/CheckAll.ascx" %>
	<SplendidCRM:CheckAll Visible="<%# !PrintView %>" Runat="Server" />

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</asp:Content>
