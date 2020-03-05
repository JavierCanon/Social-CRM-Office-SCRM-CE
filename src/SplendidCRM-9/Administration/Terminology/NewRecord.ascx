<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NewRecord.ascx.cs" Inherits="SplendidCRM.Administration.Terminology.NewRecord" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
	<SplendidCRM:HeaderLeft ID="ctlHeaderLeft" Title="Terminology.LBL_NEW_FORM_TITLE" Runat="Server" />

	<asp:Panel Width="100%" CssClass="leftColumnModuleS3" runat="server">
		<asp:Label        ID="lblNAME"         Text='<%# L10n.Term("Terminology.LBL_NAME"           ) %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /><br />
		<asp:TextBox      ID="txtNAME"         Runat="server" /><br />
		<asp:Label        ID="lblLANGUAGE"     Text='<%# L10n.Term("Terminology.LBL_LANG"           ) %>' runat="server" />&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /><br />
		<asp:DropDownList ID="lstLANGUAGE"     DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" /><br />
		<asp:Label        ID="lblMODULE_NAME"  Text='<%# L10n.Term("Terminology.LBL_MODULE_NAME"    ) %>' runat="server" /><br />
		<asp:DropDownList ID="lstMODULE_NAME"  DataValueField="MODULE_NAME" DataTextField="MODULE_NAME" Runat="server" /><br />
		<asp:Label        ID="lblLIST_NAME"    Text='<%# L10n.Term("Terminology.LBL_LIST_NAME_LABEL") %>' runat="server" /><br />
		<asp:DropDownList ID="lstLIST_NAME"    DataValueField="LIST_NAME" DataTextField="LIST_NAME" Runat="server" /><br />
		<asp:Label        ID="lblLIST_ORDER"   Text='<%# L10n.Term("Terminology.LBL_LIST_ORDER"     ) %>' runat="server" /><br />
		<asp:TextBox      ID="txtLIST_ORDER"   Runat="server" /><br />
		<asp:Label        ID="lblDISPLAY_NAME" Text='<%# L10n.Term("Terminology.LBL_DISPLAY_NAME"   ) %>' runat="server" /><br />
		<asp:TextBox      ID="txtDISPLAY_NAME" Runat="server" /><br />
		
		<asp:Button ID="btnSave" CommandName="NewRecord" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SAVE_BUTTON_LABEL"  ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_SAVE_BUTTON_TITLE") %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_BUTTON_KEY") %>' Runat="server" /><br />
		<asp:RequiredFieldValidator ID="reqNAME"         ControlToValidate="txtNAME" ErrorMessage="(required)" CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
		<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	</asp:Panel>
	<%= Utils.RegisterEnterKeyPress(txtNAME.ClientID, btnSave.ClientID) %>
</div>
<script type="text/javascript">
<%
// 01/12/2006 Paul.  NewRecord is having a problem setting the value in the code-behind. 
if ( !IsPostBack )
	Response.Write("SelectOption('" + lstLANGUAGE.ClientID + "','" + L10N.NormalizeCulture(L10n.NAME) + "');");
%>
</script>
