<%@ Control Language="c#" AutoEventWireup="false" Codebehind="DatePicker.ascx.cs" Inherits="SplendidCRM._controls.DatePicker" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
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
<script type="text/javascript">
function ChangeDate<%= txtDATE.ClientID.Replace(":", "_") %>(sDATE)
{
	document.getElementById('<%= txtDATE.ClientID %>').value = sDATE;
}
</script>
<asp:Table BorderWidth="0" CellPadding="0" CellSpacing="0" runat="server">
	<asp:TableRow>
		<asp:TableCell Wrap="false">
			<asp:TextBox ID="txtDATE" size="11" MaxLength="40" Runat="server" />
			<span onclick="ChangeDate=ChangeDate<%= txtDATE.ClientID.Replace(":", "_") %>;CalendarPopup(document.getElementById('<%= txtDATE.ClientID %>'), event.clientX, event.clientY);">
				<asp:Image ID="imgCalendar" ImageUrl='<%# Session["themeURL"] + "images/Calendar.gif" %>' AlternateText='<%# L10n.Term(".LBL_ENTER_DATE") %>' BorderWidth="0" Width="16" Height="16" ImageAlign="AbsMiddle" Runat="server" />
			</span>
			&nbsp;
			<asp:Label ID="lblDateFormat" CssClass="dateFormat" Text="<%# System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern.ToUpper() %>" runat="server" />
			<!-- 08/31/2006 Paul.  We cannot use a regular expression validator because there are just too many date formats. -->
			<SplendidCRM:DateValidator  ID="valDATE" ControlToValidate="txtDATE" CssClass="required" EnableClientScript="false" EnableViewState="false" Enabled="false" Display="dynamic" Runat="server" />
			<asp:RequiredFieldValidator ID="reqDATE" ControlToValidate="txtDATE" CssClass="required" EnableClientScript="false" EnableViewState="false" Enabled="false" Display="dynamic" Runat="server" />
		</asp:TableCell>
	</asp:TableRow>
</asp:Table>
