<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TabMenu.ascx.cs" Inherits="SplendidCRM.Themes.Sugar.TabMenu" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Import Namespace="System.Data" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
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
						<div id="divTabMenu">
							<table class="tabFrame" cellspacing="0" cellpadding="0">
								<tr>
									<td style="padding-left:14px;" class="otherTabRight">&nbsp;</td>
<%
int nRow = 0;
int nDisplayedTabs = 0;
int nMaxTabs = Sql.ToInteger(Session["max_tabs"]);
// 09/24/2007 Paul.  Max tabs is a config variable and needs the CONFIG in front of the name. 
if ( nMaxTabs == 0 )
	nMaxTabs = Sql.ToInteger(Application["CONFIG.default_max_tabs"]);
if ( nMaxTabs == 0 )
	nMaxTabs = 12;
for ( ; nRow < dtMenu.Rows.Count; nRow++ )
{
	DataRow row = dtMenu.Rows[nRow];
	string sMODULE_NAME   = Sql.ToString(row["MODULE_NAME"  ]);
	string sRELATIVE_PATH = Sql.ToString(row["RELATIVE_PATH"]);
	string sDISPLAY_NAME  = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
	string sTAB_CLASS     = (sMODULE_NAME == sActiveTab) ? "currentTab" : "otherTab";
	// 12/05/2006 Paul.  The TabMenu view does not filter the Calendar or Activites tabs as they are virtual. 
	if ( SplendidCRM.Security.GetUserAccess(sMODULE_NAME, "access") >= 0 )
	{
		if ( nDisplayedTabs < nMaxTabs )
		{
			nDisplayedTabs++;
				%>
									<td valign="bottom">
										<table class="tabFrame" cellspacing="0" cellpadding="0" height="25">
											<tr>
												<td class="<%= sTAB_CLASS %>Left"><asp:Image SkinID="blank" Width="5" Height="25" runat="server" /></td>
												<td class="<%= sTAB_CLASS %>" nowrap><a class="<%= sTAB_CLASS %>Link"  href="<%= sRELATIVE_PATH.Replace("~/", sApplicationPath) %>"><%= sDISPLAY_NAME %></a></td>
												<td class="<%= sTAB_CLASS %>Right"><asp:Image SkinID="blank" Width="5" Height="25" runat="server" /></td>
											</tr>
										</table>
									</td>
				<%
		}
		else
		{
			HyperLink lnk = new HyperLink();
			lnk.Text        = sDISPLAY_NAME;
			lnk.NavigateUrl = sRELATIVE_PATH.Replace("~/", sApplicationPath);
			lnk.CssClass    = "menuItem";
			lnk.Attributes.Add("onmouseover", "hiliteItem(this,'yes');");
			lnk.Attributes.Add("onmouseout" , "unhiliteItem(this);"    );
			pnlTabMenuMore.Controls.Add(lnk);
		}
	}
}
%>
									<td style="DISPLAY: <%= (pnlTabMenuMore.Controls.Count > 0) ? "inline" : "none" %>">
										<table class="tabFrame" cellspacing="0" cellpadding="0">
											<tr>
												<td class="otherTabLeft"><asp:Image SkinID="blank" Width="5" Height="25" runat="server" /></td>
												<td class="otherTab"><asp:Image ID="imgTabMenuMore" SkinID="more" runat="server" /></td>
												<td class="otherTabRight"><asp:Image SkinID="blank" Width="5" Height="25" runat="server" /></td>
											</tr>
										</table>
									</td>

									<td width="100%" class="tabRow"><asp:Image SkinID="blank" Width="1" Height="1" runat="server" /></td>
								</tr>
							</table>
							<table class="tabFrame" cellspacing="0" cellpadding="0" height="20">
								<tr>
									<td id="subtabs"><asp:Image SkinID="blank" Width="1" Height="1" runat="server" /></td>
								</tr>
							</table>
						</div>
<ajaxToolkit:HoverMenuExtender TargetControlID="imgTabMenuMore" PopupControlID="pnlTabMenuMore" PopupPosition="Bottom" PopDelay="50" OffsetX="-12" OffsetY="-3" runat="server" />
<asp:Panel ID="pnlTabMenuMore" CssClass="menu" runat="server" />
