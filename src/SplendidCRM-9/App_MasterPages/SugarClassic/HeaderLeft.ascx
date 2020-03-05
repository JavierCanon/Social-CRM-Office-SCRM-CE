<%@ Control Language="c#" AutoEventWireup="false" Codebehind="HeaderLeft.ascx.cs" Inherits="SplendidCRM.Themes.Sugar.HeaderLeft" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005-2008 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
</script>
<div id="divShortcuts">
<p>
<div visible="<%# !L10n.IsLanguageRTL() %>" runat="server">
	<asp:Table SkinID="tabFrame" CssClass="leftColumnModuleHead" onMouseOver="hiliteItem(this,'no');" runat="server">
		<asp:TableRow>
			<asp:TableHeaderCell Width="5"><asp:Image ImageUrl='<%# Session["themeURL"] + "images/moduleTab_left.gif" %>'  BorderWidth="0" Width="5" Height="21" Runat="server" /></asp:TableHeaderCell>
			<asp:TableHeaderCell Width="100%" CssClass="moduleTab_middle"><%= L10n.Term(sTitle) %></asp:TableHeaderCell>
			<asp:TableHeaderCell Width="9"><asp:Image ImageUrl='<%# Session["themeURL"] + "images/moduleTab_right.gif" %>' BorderWidth="0" Width="9" Height="21" Runat="server" /></asp:TableHeaderCell>
		</asp:TableRow>
	</asp:Table>
</div>
<div visible="<%# L10n.IsLanguageRTL() %>" runat="server">
	<asp:Table SkinID="tabFrame" CssClass="leftColumnModuleHead" onMouseOver="hiliteItem(this,'no');" runat="server">
		<asp:TableRow>
			<asp:TableHeaderCell Width="9"><asp:Image ImageUrl='<%# Session["themeURL"] + "images/moduleTab_right.gif" %>' BorderWidth="0" Width="9" Height="21" Runat="server" /></asp:TableHeaderCell>
			<asp:TableHeaderCell Width="100%" CssClass="moduleTab_middle"><%= L10n.Term(sTitle) %></asp:TableHeaderCell>
			<asp:TableHeaderCell Width="5"><asp:Image ImageUrl='<%# Session["themeURL"] + "images/moduleTab_left.gif" %>'  BorderWidth="0" Width="5" Height="21" Runat="server" /></asp:TableHeaderCell>
		</asp:TableRow>
	</asp:Table>
</div>
