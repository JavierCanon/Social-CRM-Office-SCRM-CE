<%@ Control Language="c#" AutoEventWireup="false" Codebehind="VerifyFunctions.ascx.cs" Inherits="SplendidCRM.Administration.SyncSchema.VerifyFunctions" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Import Namespace="System.Xml" %>
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
<div id="divEditView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Administration" EnablePrint="false" EnableHelp="true" Runat="Server" />
	<p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="WizardButtons" Src="WizardButtons.ascx" %>
	<SplendidCRM:WizardButtons ID="ctlWizardButtons" Visible="<%# !PrintView %>" Runat="Server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabSearchView" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("Administration.LBL_SOURCE_PROVIDER") %></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top"><asp:Label ID="lblSOURCE_PROVIDER" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("Administration.LBL_DESTINATION_PROVIDER") %></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top"><asp:Label ID="lblDESTINATION_PROVIDER" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("Administration.LBL_SOURCE_CONNECTION") %></asp:TableCell>
						<asp:TableCell CssClass="dataField" VerticalAlign="top"><asp:Label ID="lblSOURCE_CONNECTION" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("Administration.LBL_DESTINATION_CONNECTION") %></asp:TableCell>
						<asp:TableCell CssClass="dataField" VerticalAlign="top"><asp:Label ID="lblDESTINATION_CONNECTION" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" ColumnSpan="2"><asp:Label ID="lblSourceError"      ForeColor="Red" EnableViewState="False" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel" ColumnSpan="2"><asp:Label ID="lblDestinationError" ForeColor="Red" EnableViewState="False" Runat="server" /></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	<p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabSearchView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell><%= L10n.Term("Administration.LBL_UNIQUE_FUNCTIONS") %></asp:TableHeaderCell>
						<asp:TableHeaderCell><%= L10n.Term("Administration.LBL_UNIQUE_FUNCTIONS") %></asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><asp:Literal ID="litSOURCE_UNIQUE"      Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><asp:Literal ID="litDESTINATION_UNIQUE" Runat="server" /></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	<p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabSearchView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell><%= L10n.Term("Administration.LBL_ALL_FUNCTIONS") %></asp:TableHeaderCell>
						<asp:TableHeaderCell><%= L10n.Term("Administration.LBL_ALL_FUNCTIONS") %></asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><asp:Literal ID="litSOURCE_LIST"      Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><asp:Literal ID="litDESTINATION_LIST" Runat="server" /></asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
</div>
<%
#if DEBUG
	XmlUtil.Dump(GetXml());
#endif
%>
