<%@ Page language="c#" MasterPageFile="~/PopupView.Master" Codebehind="Preview.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Administration.EmailMan.Preview" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
</script>
<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="EmailMan" EnableModuleLabel="false" EnablePrint="true" EnableHelp="false" Runat="Server" />

<script type="text/javascript">
function UpdateParent()
{
	if ( window.opener != null )
	{
		window.opener.Refresh();
		window.close();
	}
	else
	{
		window.close();
	}
}
</script>
	<asp:Panel CssClass="button-panel" Visible="<%# !PrintView %>" runat="server">
		<asp:Button ID="btnSend"   CommandName="Send"   OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# "  " + L10n.Term("Emails.LBL_SEND_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term("Emails.LBL_SEND_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey("Emails.LBL_SEND_BUTTON_KEY") %>' Runat="server" />
		<span onclick="return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>')">
			<asp:Button ID="btnDelete" CommandName="Delete" OnCommand="Page_Command" CssClass="button" style="margin-right: 3px;" Text='<%# " "  + L10n.Term(".LBL_DELETE_BUTTON_LABEL"    ) + " "  %>' ToolTip='<%# L10n.Term(".LBL_DELETE_BUTTON_TITLE"    ) %>' AccessKey='<%# L10n.AccessKey(".LBL_DELETE_BUTTON_KEY"    ) %>' Runat="server" />
		</span>
		<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	</asp:Panel>

	<asp:Table Width="100%" BorderWidth="0" CellSpacing="0" CellPadding="0" CssClass="tabDetailView" runat="server">
		<asp:TableRow>
			<asp:TableCell Width="15%" CssClass="tabDetailViewDL" VerticalAlign="top"><asp:Label Text='<%# L10n.Term("Emails.LBL_DATE_SENT") %>' runat="server" /></asp:TableCell>
			<asp:TableCell Width="85%" CssClass="tabDetailViewDF" VerticalAlign="top"><asp:Label ID="txtSEND_DATE_TIME" runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL" VerticalAlign="top"><asp:Label Text='<%# L10n.Term("Emails.LBL_FROM") %>' runat="server" /></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF" VerticalAlign="top"><asp:Label ID="txtFROM" runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL" VerticalAlign="top"><asp:Label Text='<%# L10n.Term("Emails.LBL_TO") %>' runat="server" /></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF" VerticalAlign="top"><asp:Label ID="txtTO" runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL" VerticalAlign="top"><asp:Label Text='<%# L10n.Term("Emails.LBL_SUBJECT") %>' runat="server" /></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF" VerticalAlign="top"><asp:Label ID="txtSUBJECT" runat="server" /></asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL">&nbsp;</asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF">&nbsp;</asp:TableCell>
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell CssClass="tabDetailViewDL" VerticalAlign="top"><asp:Label Text='<%# L10n.Term("Emails.LBL_BODY") %>' runat="server" /></asp:TableCell>
			<asp:TableCell CssClass="tabDetailViewDF" VerticalAlign="top"><asp:Label ID="txtBODY_HTML" runat="server" /></asp:TableCell>
		</asp:TableRow>
	</asp:Table>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</asp:Content>
