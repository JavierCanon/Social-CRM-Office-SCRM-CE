<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TimePicker.ascx.cs" Inherits="SplendidCRM._controls.TimePicker" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
</script>
<asp:Table CellPadding="0" CellSpacing="0" runat="server">
	<asp:TableRow>
		<asp:TableCell Wrap="false">
			<asp:DropDownList ID="lstHOUR"     OnSelectedIndexChanged="Date_Changed" Runat="server" />&nbsp;
			<asp:DropDownList ID="lstMINUTE"   OnSelectedIndexChanged="Date_Changed" Runat="server" />&nbsp;
			<asp:DropDownList ID="lstMERIDIEM" OnSelectedIndexChanged="Date_Changed" Visible="false" Runat="server" />
		</asp:TableCell>
	</asp:TableRow>
	<asp:TableRow>
		<asp:TableCell Wrap="false"><asp:Label ID="lblTIMEFORMAT" CssClass="dateFormat" Runat="server" /></asp:TableCell>
	</asp:TableRow>
</asp:Table>
