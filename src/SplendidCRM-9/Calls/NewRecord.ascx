<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NewRecord.ascx.cs" Inherits="SplendidCRM.Calls.NewRecord" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<SplendidCRM:HeaderLeft ID="ctlHeaderLeft" Title="Calls.LBL_NEW_FORM_TITLE" Runat="Server" />

	<asp:Panel Width="100%" CssClass="leftColumnModuleS3" runat="server">
		<asp:RadioButton ID="radScheduleCall"    GroupName="grpSchedule" class="radio" Checked="true" Runat="server" /><asp:Label Text='<%# L10n.Term("Calls.LNK_NEW_CALL"   ) %>' runat="server" /><br />
		<asp:RadioButton ID="radScheduleMeeting" GroupName="grpSchedule" class="radio"                Runat="server" /><asp:Label Text='<%# L10n.Term("Calls.LNK_NEW_MEETING") %>' runat="server" /><br />
		<asp:Label Text='<%# L10n.Term("Meetings.LBL_SUBJECT") %>' runat="server" /><asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /><br />
		<asp:TextBox ID="txtNAME" size="25" MaxLength="50" Runat="server" /><br />
		<asp:Label Text='<%# L10n.Term("Meetings.LBL_DATE") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" />&nbsp;<asp:Label ID="lblDATEFORMAT" CssClass="dateFormat" Runat="server" /></span><br />
		<%@ Register TagPrefix="SplendidCRM" Tagname="DatePicker" Src="~/_controls/DatePicker.ascx" %>
		<SplendidCRM:DatePicker ID="ctlDATE_START" Runat="Server" />
		<asp:Label Text='<%# L10n.Term("Meetings.LBL_TIME") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" />&nbsp;<asp:Label ID="lblTIMEFORMAT" CssClass="dateFormat" Runat="server" /></span><br />
		<asp:TextBox ID="txtTIME_START" size="15" MaxLength="10" Runat="server" /><br />
		
		<asp:Button ID="btnSave" CommandName="NewRecord" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SAVE_BUTTON_LABEL"  ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_SAVE_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_BUTTON_KEY") %>' Runat="server" /><br />
		<asp:RequiredFieldValidator                      ID="reqNAME"       ControlToValidate="txtNAME"       CssClass="required" EnableClientScript="false" EnableViewState="false"  Enabled="false" Runat="server" />
		<SplendidCRM:RequiredFieldValidatorForDatePicker ID="reqDATE_START" ControlToValidate="ctlDATE_START" CssClass="required" EnableClientScript="false" EnableViewState="false"  Enabled="false" Runat="server" />
		<asp:RequiredFieldValidator                      ID="reqTIME_START" ControlToValidate="txtTIME_START" CssClass="required" EnableClientScript="false" EnableViewState="false"  Enabled="false" Runat="server" />
		<SplendidCRM:DatePickerValidator                 ID="valDATE_START" ControlToValidate="ctlDATE_START" CssClass="required" EnableClientScript="false" EnableViewState="false"  Enabled="false" Runat="server" />
		<SplendidCRM:TimeValidator                       ID="valTIME_START" ControlToValidate="txtTIME_START" CssClass="required" EnableClientScript="false" EnableViewState="false"  Enabled="false" Runat="server" />
		<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	</asp:Panel>
	<%= Utils.RegisterEnterKeyPress(txtNAME.ClientID          , btnSave.ClientID) %>
	<%= Utils.RegisterEnterKeyPress(ctlDATE_START.DateClientID, btnSave.ClientID) %>
	<%= Utils.RegisterEnterKeyPress(txtTIME_START.ClientID    , btnSave.ClientID) %>
</div>
