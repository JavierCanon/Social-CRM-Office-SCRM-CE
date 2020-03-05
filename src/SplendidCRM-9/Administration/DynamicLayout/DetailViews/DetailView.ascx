<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DetailView.ascx.cs" Inherits="SplendidCRM.Administration.DynamicLayout.DetailViews.DetailView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divDetailView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Administration" Title="DynamicLayout.LBL_DETAIL_VIEW_LAYOUT" EnablePrint="true" HelpName="DetailView" EnableHelp="true" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="SearchBasic" Src="../_controls/SearchBasic.ascx" %>
	<SplendidCRM:SearchBasic ID="ctlSearch" ViewTableName="vwDETAILVIEWS_Layout" ViewFieldName="DETAIL_NAME" Runat="Server" />
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader ID="ctlListHeader" Runat="Server" />
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="LayoutButtons" Src="../_controls/LayoutButtons.ascx" %>
	<SplendidCRM:LayoutButtons ID="ctlLayoutButtons" Visible="<%# !PrintView %>" Runat="Server" />

	<input type="hidden" id="txtFieldState" runat="server" />
	<table ID="tblMain" class="tabDetailView" runat="server">
	</table>
</div>
