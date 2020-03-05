<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NewRecord.ascx.cs" Inherits="SplendidCRM.Campaigns.NewRecord" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<SplendidCRM:HeaderLeft ID="ctlHeaderLeft" Title="Campaigns.LBL_NEW_FORM_TITLE" Runat="Server" />

	<asp:Panel Width="100%" CssClass="leftColumnModuleS3" runat="server">
		<asp:Label        ID="lblNAME"          Text='<%# L10n.Term("Campaigns.LBL_NAME") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /><br />
		<asp:TextBox      ID="txtNAME"          size="27" MaxLength="255" Runat="server" /><br />
		<asp:Label        ID="lblSTATUS"        Text='<%# L10n.Term("Campaigns.LBL_CAMPAIGN_STATUS") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /><br />
		<asp:DropDownList ID="lstSTATUS"        DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /><br />
		<asp:Label        ID="lblEND_DATE"      Text='<%# L10n.Term("Campaigns.LBL_CAMPAIGN_END_DATE") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /><br />
		<%@ Register TagPrefix="SplendidCRM" Tagname="DatePicker" Src="~/_controls/DatePicker.ascx" %>
		<SplendidCRM:DatePicker ID="ctlEND_DATE" Runat="Server" /><br />
		<asp:Label        ID="lblCAMPAIGN_TYPE" Text='<%# L10n.Term("Campaigns.LBL_CAMPAIGN_TYPE") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /><br />
		<asp:DropDownList ID="lstCAMPAIGN_TYPE" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /><br />
		
		<asp:Button ID="btnSave" CommandName="NewRecord" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SAVE_BUTTON_LABEL"  ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_SAVE_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_BUTTON_KEY") %>' Runat="server" /><br />
		<asp:RequiredFieldValidator                        ID="reqNAME"          ControlToValidate="txtNAME"          CssClass="required" EnableClientScript="false" EnableViewState="false" Enabled="false" Runat="server" />
		<SplendidCRM:RequiredFieldValidatorForDatePicker   ID="reqEND_DATE"      ControlToValidate="ctlEND_DATE"      CssClass="required" EnableClientScript="false" EnableViewState="false" Enabled="false" Runat="server" />
		<SplendidCRM:DatePickerValidator                   ID="valEND_DATE"      ControlToValidate="ctlEND_DATE"      CssClass="required" EnableClientScript="false" EnableViewState="false" Enabled="false" Runat="server" />
		<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	</asp:Panel>
	<%= Utils.RegisterEnterKeyPress(txtNAME.ClientID, btnSave.ClientID) %>
</div>
