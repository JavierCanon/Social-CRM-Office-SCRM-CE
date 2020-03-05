<%@ Control CodeBehind="ListView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Administration.Terminology.Import.ListView" %>
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
<div id="divListView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Terminology" Title=".LBL_IMPORT" EnablePrint="false" HelpName="index" EnableHelp="true" Runat="Server" />
	<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
	<SplendidCRM:ListHeader Title="Administration.LBL_IMPORT_TERMINOLOGY_TITLE" Runat="Server" />
	
	<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	<asp:Table SkinID="tabSearchView" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Label Text='<%# L10n.Term("Terminology.LBL_IMPORT_TERMINOLOGY_INSTRUCTIONS") %>' runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<br>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabSearchView" runat="server">
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Import.LBL_SELECT_FILE") %>&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField">
							<input id="fileIMPORT" type="file" size="60" MaxLength="255" TabIndex="2" runat="server" />
							<asp:RequiredFieldValidator ID="reqFILENAME" ControlToValidate="fileIMPORT" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
						<asp:TableCell CssClass="dataLabel">&nbsp;</asp:TableCell>
						<asp:TableCell CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Administration.LBL_IMPORT_LANGUAGE_TRUNCATE") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:CheckBox ID="chkTruncate" CssClass="checkbox" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel">&nbsp;</asp:TableCell>
						<asp:TableCell CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Administration.LBL_IMPORT_FORCE_UTF8") %></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:CheckBox ID="chkForceUTF8" CssClass="checkbox" Checked="true" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel">&nbsp;</asp:TableCell>
						<asp:TableCell CssClass="dataField">&nbsp;</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<br>
	<asp:Table SkinID="tabSearchView" runat="server">
		<asp:TableRow>
			<asp:TableCell ><asp:Button ID="btnBack" CommandName="Back" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_BACK_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_BACK_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_BACK_BUTTON_KEY") %>' Enabled="False" Runat="server" /></asp:TableCell>
			<asp:TableCell HorizontalAlign="Right"><asp:Button ID="btnNext" CommandName="Next" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_NEXT_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term(".LBL_NEXT_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_NEXT_BUTTON_KEY") %>' Runat="server" /></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	<asp:Table runat="server" width="100%" cellpadding="0" cellspacing="0" border="0">
		<asp:TableRow>
			<asp:TableCell><asp:Literal ID="lblImportErrors" Runat="server" /></asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<%@ Register TagPrefix="SplendidCRM" Tagname="LanguagePacks" Src="LanguagePacks.ascx" %>
	<SplendidCRM:LanguagePacks ID="ctlLanguagePacks" Runat="Server" />
</div>
