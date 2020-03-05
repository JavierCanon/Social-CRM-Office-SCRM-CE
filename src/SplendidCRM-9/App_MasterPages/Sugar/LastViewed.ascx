<%@ Control Language="c#" AutoEventWireup="false" Codebehind="LastViewed.ascx.cs" Inherits="SplendidCRM._controls.LastViewed" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005-2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
%>
<div id="divLastViewed">
	<b><%= L10n.Term(".LBL_LAST_VIEWED") %>:&nbsp;&nbsp;</b>
	<asp:Repeater id="ctlRepeater" runat="server">
		<HeaderTemplate />
		<ItemTemplate>
			<nobr>
				<asp:HyperLink NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "RELATIVE_PATH") + "view.aspx?ID=" + DataBinder.Eval(Container.DataItem, "ITEM_ID") %>' 
					ToolTip='<%# "[" + L10n.Term(".LBL_ALT_HOT_KEY") + "+" + DataBinder.Eval(Container.DataItem, "ROW_NUMBER") + "]" %>' AccessKey='<%# DataBinder.Eval(Container.DataItem, "ROW_NUMBER") %>' CssClass="lastViewLink" Runat="server">
					<asp:Image ImageUrl='<%# Session["themeURL"] + "images/" + DataBinder.Eval(Container.DataItem, "IMAGE_NAME") + ".gif" %>' AlternateText='<%# DataBinder.Eval(Container.DataItem, "ITEM_SUMMARY") %>' BorderWidth="0" Align="absmiddle" Width="16" Height="16" Runat="server" />
					&nbsp;<%# DataBinder.Eval(Container.DataItem, "ITEM_SUMMARY") %></asp:HyperLink>&nbsp;
			</nobr>
		</ItemTemplate>
		<FooterTemplate />
	</asp:Repeater>
	<div style="DISPLAY: <%= vwLastViewed != null && vwLastViewed.Count > 0 ? "NONE" : "INLINE" %>">
		<%= L10n.Term(".NTC_NO_ITEMS_DISPLAY") %>
	</div>
</div>
