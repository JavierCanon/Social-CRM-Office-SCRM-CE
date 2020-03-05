<%@ Control Language="c#" AutoEventWireup="false" Codebehind="Shortcuts.ascx.cs" Inherits="SplendidCRM.Themes.Sugar.Shortcuts" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Import Namespace="System.Data" %>
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
<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderLeft" Src="~/_controls/HeaderLeft.ascx" %>
<SplendidCRM:HeaderLeft ID="ctlHeaderLeft" runat="Server" Title=".LBL_SHORTCUTS" />


<div onMouseOver="hiliteItem(this,'no');" style="width: 180;">
	<ul class="subMenu">
<%
string sApplicationPath = Request.ApplicationPath;
if ( !sApplicationPath.EndsWith("/") )
	sApplicationPath += "/";
DataTable dt = SplendidCache.Shortcuts(sSubMenu);
if ( SplendidCRM.Security.IS_ADMIN || !AdminShortcuts )
	{
	foreach(DataRow row in dt.Rows)
	{
		string sRELATIVE_PATH = Sql.ToString(row["RELATIVE_PATH"]);
		string sIMAGE_NAME    = Sql.ToString(row["IMAGE_NAME"   ]);
		string sID            = Sql.ToString(row["DISPLAY_NAME" ]).Replace(" ", "_");
		string sDISPLAY_NAME  = L10n.Term(Sql.ToString(row["DISPLAY_NAME"]));
		if ( sRELATIVE_PATH.StartsWith("~/") )
			sRELATIVE_PATH = sRELATIVE_PATH.Replace("~/", sApplicationPath);
		%>
		<li><a href="<%= sRELATIVE_PATH %>"><img src="<%= Sql.ToString(Session["themeURL"]) + "images/" + sIMAGE_NAME %>"alt="<%= L10n.Term(sDISPLAY_NAME) %>" border="0" width="16" height="16" align="absmiddle">&nbsp;<%= L10n.Term(sDISPLAY_NAME) %></a></li>
		<%
	}
}
%>
	</ul>
</div>
</p>
<asp:Image SkinID="spacer" Height="1" Width="180" runat="server" /><br />
</div>
