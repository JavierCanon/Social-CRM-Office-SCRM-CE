<%@ Control Language="c#" AutoEventWireup="false" Codebehind="SpecifyDatabases.ascx.cs" Inherits="SplendidCRM.Administration.SyncSchema.SpecifyDatabases" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("Administration.LBL_SOURCE_PROVIDER") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
							<asp:DropDownList ID="lstSOURCE_PROVIDER" TabIndex="1" Runat="server">
								<asp:ListItem Value="System.Data.SqlClient"   >SQL Server</asp:ListItem>
								<asp:ListItem Value="Oracle.DataAccess.Client">Oracle</asp:ListItem>
								<asp:ListItem Value="IBM.Data.DB2"            >DB2</asp:ListItem>
								<asp:ListItem Value="MySql.Data"              >MySQL</asp:ListItem>
								<asp:ListItem Value="iAnywhere.Data.AsaClient">SQL Anywhere</asp:ListItem>
								<asp:ListItem Value="Sybase.Data.AseClient"   >Sybase ASE</asp:ListItem>
							</asp:DropDownList>
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("Administration.LBL_DESTINATION_PROVIDER") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField" VerticalAlign="top">
							<asp:DropDownList ID="lstDESTINATION_PROVIDER" TabIndex="2" Runat="server">
								<asp:ListItem Value="System.Data.SqlClient"   >SQL Server</asp:ListItem>
								<asp:ListItem Value="Oracle.DataAccess.Client">Oracle</asp:ListItem>
								<asp:ListItem Value="IBM.Data.DB2"            >DB2</asp:ListItem>
								<asp:ListItem Value="MySql.Data"              >MySQL</asp:ListItem>
								<asp:ListItem Value="iAnywhere.Data.AsaClient">SQL Anywhere</asp:ListItem>
								<asp:ListItem Value="Sybase.Data.AseClient"   >Sybase ASE</asp:ListItem>
							</asp:DropDownList>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("Administration.LBL_SOURCE_CONNECTION") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="txtSOURCE_CONNECTION" TabIndex="1" TextMode="MultiLine" Rows="4" Columns="50" Runat="server" />
						</asp:TableCell>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("Administration.LBL_DESTINATION_CONNECTION") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' EnableViewState="False" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField" VerticalAlign="top">
							<asp:TextBox ID="txtDESTINATION_CONNECTION" TabIndex="2" TextMode="MultiLine" Rows="4" Columns="50" Runat="server" />
						</asp:TableCell>
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
</div>
<%
#if DEBUG
	XmlUtil.Dump(GetXml());
#endif
%>
