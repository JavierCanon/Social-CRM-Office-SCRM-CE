<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TrackDetailView.ascx.cs" Inherits="SplendidCRM.Campaigns.TrackDetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divTrackDetailView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Campaigns" EnablePrint="true" HelpName="TrackDetailView" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="DetailButtons" Src="_controls/CampaignButtons.ascx" %>
	<SplendidCRM:DetailButtons ID="ctlDetailButtons" ShowViewTrack="false" ShowDeleteTest="true" Visible="<%# !PrintView %>" Runat="Server" />

	<table ID="tblMain" class="tabDetailView" runat="server">
	</table>

	<object id="hBarF" width="800" height="400" align="" classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" codebase="https://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0" viewastext>
		<param name="movie"   value="<%= Application["chartURL" ] %>hBarF.swf?filename=<%= Server.UrlEncode(Application["rootURL"] + "Campaigns/xml/ResponseByRecipientActivity.aspx?ID=" + gID.ToString() + "&") %>">
		<param name="bgcolor" value="#FFFFFF" />
		<param name="wmode"   value="transparent" />
		<param name="quality" value="high" />
		<embed src="<%= Application["chartURL" ] %>hBarF.swf?filename=<%= Server.UrlEncode(Application["rootURL"] + "Campaigns/xml/ResponseByRecipientActivity.aspx?ID=" + gID.ToString() + "&") %>" wmode="transparent" quality="high" bgcolor="#FFFFFF" height="400" width="800" name="hBarF" align="" type="application/x-shockwave-flash" pluginspage="https://www.macromedia.com/go/getflashplayer" />
	</object>

	<div id="divDetailSubPanel">
		<asp:PlaceHolder ID="plcSubPanel" Runat="server" />
	</div>
</div>

<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
