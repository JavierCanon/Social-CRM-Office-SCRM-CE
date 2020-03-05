<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NewRecord.ascx.cs" Inherits="SplendidCRM.ProjectTasks.NewRecord" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<SplendidCRM:HeaderLeft ID="ctlHeaderLeft" Title="ProjectTask.LBL_NEW_FORM_TITLE" Runat="Server" />

	<script type="text/javascript">
	function ChangeProject(sPARENT_ID, sPARENT_NAME)
	{
		document.getElementById('<%= txtPROJECT_ID.ClientID   %>').value = sPARENT_ID  ;
		document.getElementById('<%= txtPROJECT_NAME.ClientID %>').value = sPARENT_NAME;
	}
	function ProjectPopup()
	{
		return window.open('../Projects/Popup.aspx','ProjectPopup','width=600,height=400,resizable=1,scrollbars=1');
	}
	</script>
	<asp:Panel Width="100%" CssClass="leftColumnModuleS3" runat="server">
		<asp:Label        ID="lblNAME"             Text='<%# L10n.Term("ProjectTask.LBL_NAME"     ) %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /><br />
		<asp:TextBox      ID="txtNAME"             Runat="server" /><br />
		<asp:Label        ID="lblPROJECT_NAME"     Text='<%# L10n.Term("ProjectTask.LBL_PARENT_ID") %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /><br />
		<asp:TextBox      ID="txtPROJECT_NAME"     ReadOnly="True" size="18" MaxLength="50" Runat="server" />
		<asp:HiddenField  ID="txtPROJECT_ID"       runat="server" />
		<asp:Button       ID="btnSelectProject"    OnClientClick="ProjectPopup(); return false;" CssClass="button" Text='<%# L10n.Term(".LBL_SELECT_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_SELECT_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SELECT_BUTTON_KEY") %>' runat="server" /><br />
		<asp:Label        ID="lblASSIGNED_USER_ID" Text='<%# L10n.Term(".LBL_ASSIGNED_TO") %>' runat="server" /><br />
		<asp:DropDownList ID="lstASSIGNED_USER_ID" DataValueField="ID" DataTextField="USER_NAME" Runat="server" /><br />
		
		<asp:Button ID="btnSave" CommandName="NewRecord" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SAVE_BUTTON_LABEL"  ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_SAVE_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_BUTTON_KEY") %>' Runat="server" /><br />
		<asp:RequiredFieldValidator ID="reqNAME"        ControlToValidate="txtNAME"        ErrorMessage="(required)" CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
		<SplendidCRM:RequiredFieldValidatorForHiddenInputs ID="reqPROJECT_ID" ControlToValidate="txtPROJECT_ID" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
	</asp:Panel>
	<%= Utils.RegisterEnterKeyPress(txtNAME.ClientID, btnSave.ClientID) %>
</div>
