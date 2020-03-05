<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.Administration.EmailMan.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="EditButtons" Src="~/_controls/EditButtons.ascx" %>
	<SplendidCRM:EditButtons ID="ctlEditButtons" Visible="<%# !PrintView %>" Runat="Server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="40%" CssClass="dataLabel" VerticalAlign="top" Wrap="false"><%= L10n.Term("EmailMan.LBL_EMAILS_PER_RUN") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="60%" CssClass="dataField">
							<asp:TextBox ID="EMAILS_PER_RUN" TabIndex="1" size="10" MaxLength="10" Runat="server" />
							<asp:RequiredFieldValidator ID="reqEMAILS_PER_RUN" ControlToValidate="EMAILS_PER_RUN" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel" VerticalAlign="top"><%= L10n.Term("EmailMan.LBL_LOCATION_TRACK") %></asp:TableCell>
						<asp:TableCell CssClass="dataField">
							<asp:RadioButton ID="SITE_LOCATION_DEFAULT" GroupName="SITE_LOCATION_TYPE" Text='<%# L10n.Term("EmailMan.LBL_DEFAULT_LOCATION") %>' CssClass="radio" runat="server" />
							<asp:RadioButton ID="SITE_LOCATION_CUSTOM"  GroupName="SITE_LOCATION_TYPE" Text='<%# L10n.Term("EmailMan.LBL_CUSTOM_LOCATION" ) %>' CssClass="radio" runat="server" /><br />
							<asp:TextBox     ID="SITE_LOCATION"         Width="200" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
</div>
