<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NewRecord.ascx.cs" Inherits="SplendidCRM.Accounts.NewRecord" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005-2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
</script>
<div id="divNewRecord">
	<%@ Register TagPrefix="SplendidCRM" Tagname="HeaderLeft" Src="~/_controls/HeaderLeft.ascx" %>
	<SplendidCRM:HeaderLeft ID="ctlHeaderLeft" Title="Accounts.LBL_NEW_FORM_TITLE" Runat="Server" />

	<asp:Panel Width="100%" CssClass="leftColumnModuleS3" runat="server">
		<asp:Label   ID="lblNAME"         Text='<%# L10n.Term("Accounts.LBL_ACCOUNT_NAME") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /><br />
		<asp:TextBox ID="txtNAME"         Runat="server" /><br />
		<asp:Label   ID="lblPHONE_OFFICE" Text='<%# L10n.Term("Accounts.LBL_PHONE"       ) %>' runat="server" /><br />
		<asp:TextBox ID="txtPHONE_OFFICE" Runat="server" /><br />
		<asp:Label   ID="lblWEBSITE"      Text='<%# L10n.Term("Accounts.LBL_WEBSITE"     ) %>' runat="server" /><br />
		<asp:TextBox ID="txtWEBSITE"      Runat="server">http://</asp:TextBox><br />
		
		<asp:Button ID="btnSave" CommandName="NewRecord" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SAVE_BUTTON_LABEL"  ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_SAVE_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_BUTTON_KEY") %>' Runat="server" /><br />
		<asp:RequiredFieldValidator ID="reqNAME" ControlToValidate="txtNAME" ErrorMessage="(required)" CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
		<!--
		http://www.regexlib.com/
		Expression :  ^(?:(?:[\+]?(?<CountryCode>[\d]{1,3}(?:[ ]+|[\-.])))?[(]?(?<AreaCode>[\d]{3})[\-/)]?(?:[ ]+)?)?(?<Number>[a-zA-Z2-9][a-zA-Z0-9 \-.]{6,})(?:(?:[ ]+|[xX]|(i:ext[\.]?)){1,2}(?<Ext>[\d]{1,5}))?$
		Description:  This allows the formatting of most phone numbers.
		Matches    :  [1-800-DISCOVER], [(610) 310-5555 x5555], [533-1123]
		Non-Matches:  [1 533-1123], [553334], [66/12343]
		-->
		<asp:RegularExpressionValidator ID="reqPHONE_OFFICE" ControlToValidate="txtPHONE_OFFICE" ErrorMessage="(invalid phone)" CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" 
			ValidationExpression="^(?:(?:[\+]?(?<CountryCode>[\d]{1,3}(?:[ ]+|[\-.])))?[(]?(?<AreaCode>[\d]{3})[\-/)]?(?:[ ]+)?)?(?<Number>[a-zA-Z2-9][a-zA-Z0-9 \-.]{6,})(?:(?:[ ]+|[xX]|(i:ext[\.]?)){1,2}(?<Ext>[\d]{1,5}))?$" />
		<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	</asp:Panel>
	<%= Utils.RegisterEnterKeyPress(txtNAME.ClientID        , btnSave.ClientID) %>
	<%= Utils.RegisterEnterKeyPress(txtPHONE_OFFICE.ClientID, btnSave.ClientID) %>
	<%= Utils.RegisterEnterKeyPress(txtWEBSITE.ClientID     , btnSave.ClientID) %>
</div>
