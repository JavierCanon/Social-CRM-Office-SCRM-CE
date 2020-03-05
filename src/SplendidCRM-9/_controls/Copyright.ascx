<%@ Control Language="c#" AutoEventWireup="false" Codebehind="Copyright.ascx.cs" Inherits="SplendidCRM._controls.Copyright" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2005-2008 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
</script>
<div id="divFooterCopyright">
	<asp:Table Width="100%" BorderWidth="0" CellPadding="0" CellSpacing="0" runat="server">
		<asp:TableRow>
			<asp:TableCell HorizontalAlign="Center" CssClass="copyRight">
				Portions Copyright &copy; 2005-2008 <asp:HyperLink ID="lnkSplendidCRM" NavigateUrl="http://www.splendidcrm.com" Text="SplendidCRM Software, Inc." Target="_blank" CssClass="copyRightLink" runat="server" /> All Rights Reserved.<br \>
				Copyright &copy; 2005 <asp:HyperLink ID="lnkSugarCRM" NavigateUrl="http://www.sugarcrm.com" Text="SugarCRM Inc." Target="_blank" CssClass="copyRightLink" runat="server" /> All Rights Reserved.<br \>
				<asp:HyperLink ID="lnkFooterSugarCRM" NavigateUrl="http://www.sugarcrm.com" Target="_blank" Runat="server">
					<asp:Image ID="imgFooterSugarCRM" ImageUrl='<%# Application["imageURL"] + "powered_by_sugarcrm.gif" %>' AlternateText="Powered By SugarCRM" BorderWidth="0" Width="134" Height="26"  style="margin-top: 2px" Runat="server" />
				</asp:HyperLink>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
</div>
