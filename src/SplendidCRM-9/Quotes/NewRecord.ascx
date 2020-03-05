<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NewRecord.ascx.cs" Inherits="SplendidCRM.Quotes.NewRecord" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<SplendidCRM:HeaderLeft ID="ctlHeaderLeft" Title="Quotes.LBL_NEW_FORM_TITLE" Runat="Server" />

	<script type="text/javascript">
	function NewRecordChangeAccount(sPARENT_ID, sPARENT_NAME)
	{
		document.getElementById('<%= txtACCOUNT_ID.ClientID   %>').value = sPARENT_ID  ;
		document.getElementById('<%= txtACCOUNT_NAME.ClientID %>').value = sPARENT_NAME;
	}
	function NewRecordAccountPopup()
	{
		ChangeAccount = NewRecordChangeAccount;
		return window.open('../Accounts/Popup.aspx','AccountPopup','width=600,height=400,resizable=1,scrollbars=1');
	}
	</script>
	<asp:Panel Width="100%" CssClass="leftColumnModuleS3" runat="server">
		<asp:Label        ID="lblNAME"         Text='<%# L10n.Term("Quotes.LBL_NAME") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /><br />
		<asp:TextBox      ID="txtNAME"         MaxLength="50" Runat="server" /><br />
		<asp:Label        ID="lblACCOUNT_NAME" Text='<%# L10n.Term("Quotes.LBL_ACCOUNT_NAME") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /><br />
		<asp:TextBox      ID="txtACCOUNT_NAME" ReadOnly="True" size="16" Runat="server" />
		<asp:HiddenField  ID="txtACCOUNT_ID"   runat="server" />
		<asp:Button       ID="btnChangeAccount" OnClientClick="NewRecordAccountPopup(); return false;" CssClass="button" Text='<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_CHANGE_BUTTON_KEY") %>' runat="server" /><br />
		<asp:Label        ID="lblQUOTE_STAGE"  Text='<%# L10n.Term("Quotes.LBL_QUOTE_STAGE") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /><br />
		<asp:DropDownList ID="lstQUOTE_STAGE"  DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /><br />
		
		<asp:Button ID="btnSave" CommandName="NewRecord" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SAVE_BUTTON_LABEL"  ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_SAVE_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_BUTTON_KEY") %>' Runat="server" /><br />
		<asp:RequiredFieldValidator                             ID="reqNAME"        ControlToValidate="txtNAME"        ErrorMessage="(required)" CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
		<SplendidCRM:RequiredFieldValidatorForHiddenInputs ID="reqACCOUNT_ID"  ControlToValidate="txtACCOUNT_ID"  ErrorMessage="(required)" CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
		<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	</asp:Panel>
	<%= Utils.RegisterEnterKeyPress(txtNAME.ClientID, btnSave.ClientID) %>
</div>
