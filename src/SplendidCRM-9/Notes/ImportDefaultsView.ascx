<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ImportDefaultsView.ascx.cs" Inherits="SplendidCRM.Notes.ImportDefaultsView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
function DirectChangeContact(sPARENT_ID, sPARENT_NAME)
{
	document.getElementById('<%= new DynamicControl(this, "CONTACT_ID"  ).ClientID %>').value = sPARENT_ID  ;
	document.getElementById('<%= new DynamicControl(this, "CONTACT_NAME").ClientID %>').value = sPARENT_NAME;
}
function ContactPopup()
{
	// 08/28/2006 Paul.  A Note can have a Parent Contact and a direct Contact relationship. Overwrite the ChangeContact as necessary. 
	ChangeContact= DirectChangeContact;
	return window.open('../Contacts/Popup.aspx?ClearDisabled=0','ContactPopup','width=600,height=400,resizable=1,scrollbars=1');
}
</script>
<div id="divImportDefaultsView">
	<p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="TeamAssignedPopupScripts" Src="~/_controls/TeamAssignedPopupScripts.ascx" %>
	<SplendidCRM:TeamAssignedPopupScripts ID="ctlTeamAssignedPopupScripts" Runat="Server" />
	<%@ Register TagPrefix="SplendidCRM" Tagname="ParentPopupScripts" Src="~/_controls/ParentPopupScripts.ascx" %>
	<SplendidCRM:ParentPopupScripts ID="ctlParentPopupScripts" Runat="Server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<table ID="tblMain" class="tabEditView" runat="server">
				</table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
</div>
