<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.EmailTemplates.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<div id="divEditView">
<script type="text/javascript">
// 08/08/2006 Paul.  Fixed access to selected index.  The line was missing the closing square bracket.
function ShowVariable()
{
	document.getElementById('<%= txtVariableText.ClientID %>').value = '$' + document.getElementById('<%= lstVariableName.ClientID %>').options[document.getElementById('<%= lstVariableName.ClientID %>').selectedIndex].value;
}
function ShowTracker()
{
	document.getElementById('<%= txtTrackerText.ClientID %>').value = '{' + document.getElementById('<%= lstTrackerName.ClientID %>').options[document.getElementById('<%= lstTrackerName.ClientID %>').selectedIndex].value + '}';
}
function InsertVariable()
{
	try
	{
		var s = document.getElementById('<%= txtVariableText.ClientID %>').value;
		
		var oEditor = FCKeditorAPI.GetInstance('<%= txtBODY.ClientID %>') ;
		// 09/22/2007 Paul.  Only allow insert in Wysiwyg mode. 
		if ( oEditor.EditMode == 0 )
			oEditor.InsertHtml(s);
	}
	catch(e)
	{
	}
}
function InsertTracker()
{
	try
	{
		var s = document.getElementById('<%= txtTrackerText.ClientID %>').value;
		var sTrackerURL = '<a href="' + s + '"><%= L10n.Term("EmailTemplates.LBL_DEFAULT_LINK_TEXT") %></a>';
		
		var oEditor = FCKeditorAPI.GetInstance('<%= txtBODY.ClientID %>') ;
		// 09/22/2007 Paul.  Only allow insert in Wysiwyg mode. 
		if ( oEditor.EditMode == 0 )
			oEditor.InsertHtml(sTrackerURL);
	}
	catch(e)
	{
	}
}
function AddFile()
{
	for(var i=0;i<10;i++)
	{
		var elem = document.getElementById('file'+i);
		if(elem.style.display == 'none')
		{
			elem.style.display='block';
			break;
		}
	}
}
function DeleteFile(index)
{
	var elem = document.getElementById('file'+index);
	elem.style.display='none';
}
</script>

	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="EmailTemplates" EnablePrint="false" HelpName="EditView" EnableHelp="true" Runat="Server" />
	<p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="EditButtons" Src="~/_controls/EditButtons.ascx" %>
	<SplendidCRM:EditButtons ID="ctlEditButtons" Visible="<%# !PrintView %>" Runat="Server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="TeamAssignedPopupScripts" Src="~/_controls/TeamAssignedPopupScripts.ascx" %>
	<SplendidCRM:TeamAssignedPopupScripts ID="ctlTeamAssignedPopupScripts" Runat="Server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("EmailTemplates.LBL_NAME") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataField"><asp:TextBox ID="txtNAME" TabIndex="1" MaxLength="255" size="30" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><%= L10n.Term("EmailTemplates.LBL_READ_ONLY") %> <asp:CheckBox ID="chkREAD_ONLY" TabIndex="1" CssClass="checkbox" Runat="server" /></asp:TableCell>
						<asp:TableCell Width="30%" CssClass="dataLabel">
							<div id="divTEAM" style="DISPLAY: <%= SplendidCRM.Crm.Config.enable_team_management() ? "INLINE" : "NONE" %>">
								<%= L10n.Term("Teams.LBL_TEAM") %>
								<asp:TextBox ID="TEAM_NAME" ReadOnly="True" Runat="server" />
								<input ID="TEAM_ID" type="hidden" runat="server" />
								<input ID="btnChangeTeam" type="button" CssClass="button" onclick="return TeamPopup();" title="<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>" AccessKey="<%# L10n.AccessKey(".LBL_CHANGE_BUTTON_KEY") %>" value="<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>" />
							</div>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("EmailTemplates.LBL_DESCRIPTION") %></asp:TableCell>
						<asp:TableCell CssClass="dataField" ColumnSpan="3"><asp:TextBox ID="txtDESCRIPTION" TabIndex="1" TextMode="MultiLine" Columns="90" Rows="1" style="overflow-y:auto;" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell ColumnSpan="4">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("EmailTemplates.LBL_INSERT_VARIABLE") %></asp:TableCell>
						<asp:TableCell CssClass="dataField" ColumnSpan="3">
							<asp:DropDownList ID="lstVariableModule" TabIndex="1" OnSelectedIndexChanged="lstVariableModule_Changed" AutoPostBack="true" Runat="server" />
							<asp:DropDownList ID="lstVariableName"   TabIndex="1" onchange="ShowVariable()" Runat="server" />
							<span class="dataLabel">:</span>
							<asp:TextBox ID="txtVariableText"   size="30" Runat="server" />
							<asp:Button  ID="btnVariableInsert" OnClientClick="InsertVariable(); return false;" CssClass="button" Text='<%# L10n.Term("EmailTemplates.LBL_INSERT") %>' runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow Visible='<%# !Sql.IsEmptyGuid(Request["CAMPAIGN_ID"]) %>'>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("EmailTemplates.LBL_INSERT_TRACKER_URL") %></asp:TableCell>
						<asp:TableCell CssClass="dataField" ColumnSpan="3">
							<asp:DropDownList ID="lstTrackerName" TabIndex="1" onchange="ShowTracker()" Runat="server" />
							<span class="dataLabel">:</span>
							<asp:TextBox ID="txtTrackerText"   size="30" Runat="server" />
							<asp:Button  ID="btnTrackerInsert" OnClientClick="InsertTracker(); return false;" CssClass="button" Text='<%# L10n.Term("EmailTemplates.LBL_INSERT_URL_REF") %>' runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("EmailTemplates.LBL_SUBJECT") %></asp:TableCell>
						<asp:TableCell CssClass="dataField" ColumnSpan="3"><asp:TextBox ID="txtSUBJECT" TabIndex="1" TextMode="MultiLine" Columns="90" Rows="1" style="overflow-y:auto;" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("EmailTemplates.LBL_BODY") %></asp:TableCell>
						<asp:TableCell CssClass="dataField" ColumnSpan="3">
							<%@ Register TagPrefix="FCKeditorV2" Namespace="FredCK.FCKeditorV2" Assembly="FredCK.FCKeditorV2" %>
							<FCKeditorV2:FCKeditor id="txtBODY" ToolbarSet="SplendidCRM" BasePath="~/FCKeditor/" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Emails.LBL_ATTACHMENTS") %></asp:TableCell>
						<asp:TableCell CssClass="dataField" ColumnSpan="3">
							<asp:Repeater id="ctlAttachments" runat="server">
								<HeaderTemplate />
								<ItemTemplate>
										<asp:HyperLink Text='<%# DataBinder.Eval(Container.DataItem, "FILENAME") %>' NavigateUrl='<%# "~/Notes/Attachment.aspx?ID=" + DataBinder.Eval(Container.DataItem, "NOTE_ATTACHMENT_ID") %>' Target="_blank" Runat="server" /><br />
								</ItemTemplate>
								<FooterTemplate />
							</asp:Repeater>
							<div id="uploads_div">
							<div style="display: none" id="file0"><input id="email_attachment0" type="file" tabindex="0" size="40" runat="server" />&nbsp;<input type="button" onclick="DeleteFile('0');" CssClass="button" value="<%= L10n.Term(".LBL_REMOVE") %>"/></div>
							<div style="display: none" id="file1"><input id="email_attachment1" type="file" tabindex="0" size="40" runat="server" />&nbsp;<input type="button" onclick="DeleteFile('1');" CssClass="button" value="<%= L10n.Term(".LBL_REMOVE") %>"/></div>
							<div style="display: none" id="file2"><input id="email_attachment2" type="file" tabindex="0" size="40" runat="server" />&nbsp;<input type="button" onclick="DeleteFile('2');" CssClass="button" value="<%= L10n.Term(".LBL_REMOVE") %>"/></div>
							<div style="display: none" id="file3"><input id="email_attachment3" type="file" tabindex="0" size="40" runat="server" />&nbsp;<input type="button" onclick="DeleteFile('3');" CssClass="button" value="<%= L10n.Term(".LBL_REMOVE") %>"/></div>
							<div style="display: none" id="file4"><input id="email_attachment4" type="file" tabindex="0" size="40" runat="server" />&nbsp;<input type="button" onclick="DeleteFile('4');" CssClass="button" value="<%= L10n.Term(".LBL_REMOVE") %>"/></div>
							<div style="display: none" id="file5"><input id="email_attachment5" type="file" tabindex="0" size="40" runat="server" />&nbsp;<input type="button" onclick="DeleteFile('5');" CssClass="button" value="<%= L10n.Term(".LBL_REMOVE") %>"/></div>
							<div style="display: none" id="file6"><input id="email_attachment6" type="file" tabindex="0" size="40" runat="server" />&nbsp;<input type="button" onclick="DeleteFile('6');" CssClass="button" value="<%= L10n.Term(".LBL_REMOVE") %>"/></div>
							<div style="display: none" id="file7"><input id="email_attachment7" type="file" tabindex="0" size="40" runat="server" />&nbsp;<input type="button" onclick="DeleteFile('7');" CssClass="button" value="<%= L10n.Term(".LBL_REMOVE") %>"/></div>
							<div style="display: none" id="file8"><input id="email_attachment8" type="file" tabindex="0" size="40" runat="server" />&nbsp;<input type="button" onclick="DeleteFile('8');" CssClass="button" value="<%= L10n.Term(".LBL_REMOVE") %>"/></div>
							<div style="display: none" id="file9"><input id="email_attachment9" type="file" tabindex="0" size="40" runat="server" />&nbsp;<input type="button" onclick="DeleteFile('9');" CssClass="button" value="<%= L10n.Term(".LBL_REMOVE") %>"/></div>
							</div>
							<input type="button" CssClass="button" onclick="AddFile();" value="<%= L10n.Term("Emails.LBL_ADD_FILE") %>" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
</div>
