<%@ Page language="c#" MasterPageFile="~/DefaultView.Master" Codebehind="SetTimezone.aspx.cs" AutoEventWireup="false" Inherits="SplendidCRM.Users.SetTimezone" %>
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
<asp:Content ID="cntBody" ContentPlaceHolderID="cntBody" runat="server">
	<asp:Label ID="lblError" ForeColor="Red" EnableViewState="false" Runat="server" />
	
	<asp:Table Width="400" BorderWidth="1" BorderColor="#444444" CellPadding="8" CellSpacing="2" HorizontalAlign="Center" CssClass="" runat="server">
		<asp:TableRow>
			<asp:TableCell />
		</asp:TableRow>
		<asp:TableRow>
			<asp:TableCell style="padding-bottom: 5px;">
				<asp:Label Text='<%# L10n.Term("Users.LBL_PICK_TZ_WELCOME") %>' runat="server" />
				<br />
				<br />
				<asp:Label Text='<%# L10n.Term("Users.LBL_PICK_TZ_DESCRIPTION") %>' runat="server" />
				<br />
				<br />
				<asp:DropDownList ID="lstTIMEZONE" DataValueField="ID" DataTextField="NAME" TabIndex="3" Runat="server" />
				&nbsp;<asp:Button ID="btnSave" CommandName="Save" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term(".LBL_SAVE_BUTTON_LABEL"  ) + "  " %>' ToolTip='<%# L10n.Term(".LBL_SAVE_BUTTON_TITLE"  ) %>' AccessKey='<%# L10n.AccessKey(".LBL_SAVE_BUTTON_KEY"  ) %>' Runat="server" /><br />
				<asp:Label Text='<%# L10n.Term("Users.LBL_DST_INSTRUCTIONS") %>' CssClass="dateFormat" Visible="false" runat="server" />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	<br />
	<script type="text/javascript" language="JavaScript">
	var lstTIMEZONE  = document.getElementById('<%= lstTIMEZONE.ClientID %>');
	if ( lstTIMEZONE.options.selectedIndex == 0 )
	{
		var dtJanuary = new Date((new Date()).getFullYear(), 0, 1, 0, 0, 0);
		
		var sDefaultOffset;
		if ( dtJanuary.getTimezoneOffset() > 0 )
			sDefaultOffset = '(GMT-' + ('0' +    dtJanuary.getTimezoneOffset()/60 + ':00').substring(0, 5) + ')';
		else
			sDefaultOffset = '(GMT+' + ('0' + -1*dtJanuary.getTimezoneOffset()/60 + ':00').substring(0, 5) + ')';

		for ( i = 0; i < lstTIMEZONE.options.length; i++ )
		{
			if ( lstTIMEZONE.options[i].text.substring(0, sDefaultOffset.length) == sDefaultOffset )
			{
				lstTIMEZONE.options.selectedIndex = i;
				break;
			}
		}
	}
	</script>
</asp:Content>
