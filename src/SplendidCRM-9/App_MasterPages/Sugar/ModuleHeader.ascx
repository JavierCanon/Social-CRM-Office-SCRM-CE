<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ModuleHeader.ascx.cs" Inherits="SplendidCRM.Themes.Sugar.ModuleHeader" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
	<div id="divModuleHeader">
		<script type="text/javascript">
		function PopupHelp()
		{
			var url = document.getElementById('<%= lnkHelpText.ClientID %>').href;
			window.open(url,'helpwin','width=600,height=600,status=0,resizable=1,scrollbars=1,toolbar=0,location=1');
		}
		</script>
		<p>
		<asp:Table SkinID="tabFrame" CssClass="moduleTitle" runat="server">
			<asp:TableRow>
				<asp:TableCell VerticalAlign="top"><asp:Image ImageUrl='<%# Session["themeURL"] + "images/" + sModule +".gif" %>' AlternateText='<%# L10n.Term(".moduleList." + sModule) %>' BorderWidth="0" Width="16" Height="16" style="margin-top: 3px" Runat="server" />&nbsp;</asp:TableCell>
				<asp:TableCell Width="99%"><h2><asp:Label ID="lblTitle" Runat="server" /></h2></asp:TableCell>
				<asp:TableCell VerticalAlign="top" HorizontalAlign="Right" style="padding-top:3px; padding-left: 5px;" Wrap="false">
					<div visible="<%# !PrintView %>" runat="server">
						<asp:ImageButton CommandName="Print" OnCommand="Page_Command" CssClass="utilsLink" AlternateText='<%# L10n.Term(".LNK_PRINT") %>' ImageUrl='<%# Session["themeURL"] + "images/print.gif" %>' BorderWidth="0" Width="13" Height="13" ImageAlign="AbsMiddle" Visible="<%# bEnablePrint %>" Runat="server" />
						<asp:LinkButton  CommandName="Print" OnCommand="Page_Command" CssClass="utilsLink" Text='<%# L10n.Term(".LNK_PRINT") %>' Visible="<%# bEnablePrint %>" Runat="server" />
						&nbsp;
						<asp:HyperLink ID="lnkHelpImage" onclick="PopupHelp(); return false;" CssClass="utilsLink" Target="_blank" Visible="<%# bEnableHelp %>" Runat="server">
							<asp:Image ImageUrl='<%# Session["themeURL"] + "images/help.gif" %>' AlternateText='<%# L10n.Term(".LNK_HELP") %>' BorderWidth="0" Width="13" Height="13" ImageAlign="AbsMiddle" Runat="server" />
						</asp:HyperLink>
						<asp:HyperLink ID="lnkHelpText" onclick="PopupHelp(); return false;" CssClass="utilsLink" Target="_blank" Visible="<%# bEnableHelp %>" Runat="server"><%# L10n.Term(".LNK_HELP") %></asp:HyperLink>
					</div>
					<div visible="<%# PrintView %>" runat="server">
						<asp:ImageButton CommandName="PrintOff" OnCommand="Page_Command" CssClass="utilsLink" AlternateText='<%# L10n.Term(".LBL_BACK") %>' ImageUrl='<%# Session["themeURL"] + "images/print.gif" %>' BorderWidth="0" Width="13" Height="13" ImageAlign="AbsMiddle" Visible="<%# bEnablePrint %>" Runat="server" />
						<asp:LinkButton  CommandName="PrintOff" OnCommand="Page_Command" CssClass="utilsLink" Text='<%# L10n.Term(".LBL_BACK") %>' Visible="<%# bEnablePrint %>" Runat="server" />
					</div>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
		</p>
	</div>
