<%@ Page language="c#" MasterPageFile="~/DefaultView.Master" Codebehind="default.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Home.Default" %>
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
<asp:Content ID="cntSidebar" ContentPlaceHolderID="cntSidebar" runat="server">
	<%@ Register TagPrefix="SplendidCRM" Tagname="Shortcuts" Src="~/_controls/Shortcuts.ascx" %>
	<SplendidCRM:Shortcuts ID="ctlShortcuts" SubMenu="Home" Title=".LBL_SHORTCUTS" Runat="Server" />
	<asp:PlaceHolder ID="plcSubPanelLeft" Runat="server" />
</asp:Content>

<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
	<asp:Label ID="lblUpgradeWarning" CssClass="error" Visible="false" runat="server" />
	<table border="0" cellpadding="0" cellspacing="0" width="100%" Visible="<%# !this.IsMobile %>" runat="server">
		<tr>
			<td width="60%" valign="top">
				<asp:UpdatePanel ID="ajaxDetailSubPanel" runat="server">
					<ContentTemplate>
						<asp:PlaceHolder ID="plcSubPanelBody" Runat="server" />
					</ContentTemplate>
				</asp:UpdatePanel>
				</div>
			</td>
			<td style="padding-left: 10px; vertical-align: top;">
				<asp:UpdatePanel ID="ajaxRightbar" runat="server">
					<ContentTemplate>
						<asp:PlaceHolder ID="plcSubPanelRight" Runat="server" />
					</ContentTemplate>
				</asp:UpdatePanel>
			</td>
		</tr>
	</table>

	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
</asp:Content>
