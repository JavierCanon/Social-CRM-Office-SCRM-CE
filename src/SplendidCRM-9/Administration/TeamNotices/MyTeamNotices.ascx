<%@ Control CodeBehind="MyTeamNotices.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.TeamNotices.MyTeamNotices" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
</script>
<div id="divTeamNoticesMyTeamNotices">
	<br />
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Home.LBL_TEAM_NOTICES_TITLE" Runat="Server" />
	<marquee behavior="scroll" scrollamount="1" scrolldelay="100" direction="up" height="60" width="100%" class="monthBox">
		<div  style="margin: 4px">
			<asp:Repeater id="ctlRepeater" runat="server">
				<ItemTemplate>
					<b><%# DataBinder.Eval(Container.DataItem, "NAME") %></b><br />
					<%# DataBinder.Eval(Container.DataItem, "DESCRIPTION") %><br />
					<asp:HyperLink NavigateUrl='<%# DataBinder.Eval(Container.DataItem, "URL") %>' Text='<%# DataBinder.Eval(Container.DataItem, "URL_TITLE") %>' Runat="server" /><br />
				</ItemTemplate>
			</asp:Repeater>
		</div>
	</marquee>
</div>
