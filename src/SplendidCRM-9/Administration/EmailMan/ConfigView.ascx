<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ConfigView.ascx.cs" Inherits="SplendidCRM.Administration.EmailMan.ConfigView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
<script type="text/javascript">
function GmailDefaults()
{
	var fldMAIL_SMTPSERVER   = document.getElementById('<%= MAIL_SMTPSERVER  .ClientID %>');
	var fldMAIL_SMTPPORT     = document.getElementById('<%= MAIL_SMTPPORT    .ClientID %>');
	var fldMAIL_SMTPAUTH_REQ = document.getElementById('<%= MAIL_SMTPAUTH_REQ.ClientID %>');
	var fldMAIL_SMTPSSL      = document.getElementById('<%= MAIL_SMTPSSL     .ClientID %>');
	fldMAIL_SMTPSERVER.value     = 'smtp.gmail.com';
	// http://forums.microsoft.com/msdn/showpost.aspx?postid=7575&siteid=1&sb=0&d=1&at=7&ft=11&tf=0&pageid=1
	// Gmail Smtp has 2 ports exposed 465 and 587. 465 port is for Exchange structre and thats why the Smtp protocol doesnt work against it. 
	// We have testcases internally that try to send mail to port 587 of gmail and it has been working fine.
	fldMAIL_SMTPPORT.value       = '587';
	fldMAIL_SMTPAUTH_REQ.checked = true;
	fldMAIL_SMTPSSL.checked      = true;
}
function toggleAllSecurityOptions()
{
	var sParentID = '<%= this.ClientID %>';
	var arrDangerousTags = '<%= sDangerousTags %>'.split('|');
	var bToggle = document.getElementById('<%= SECURITY_TOGGLE_ALL.ClientID %>').checked;
	document.getElementById('<%= SECURITY_OUTLOOK_DEFAULTS.ClientID %>').checked = false;
	for ( var i = 0; i < arrDangerousTags.length; i++ )
	{
		document.getElementById(sParentID + '_SECURITY_' + arrDangerousTags[i].toUpperCase()).checked = bToggle;
	}
}
function setOutlookDefaults()
{
	var sParentID = '<%= this.ClientID %>';
	var arrDangerousTags = '<%= sDangerousTags %>'.split('|');
	var arrOutlookTags   = '<%= sOutlookTags   %>'.split('|');
	document.getElementById('<%= SECURITY_TOGGLE_ALL.ClientID %>').checked = false;
	if ( document.getElementById('<%= SECURITY_OUTLOOK_DEFAULTS.ClientID %>').checked )
	{
		for ( var i = 0; i < arrDangerousTags.length; i++ )
		{
			document.getElementById(sParentID + '_SECURITY_' + arrDangerousTags[i].toUpperCase()).checked = false;
		}
		for ( var i = 0; i < arrOutlookTags.length; i++ )
		{
			document.getElementById(sParentID + '_SECURITY_' + arrOutlookTags[i].toUpperCase()).checked = true;
		}
	}
}
</script>
<div id="divEditView">
	<p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="EditButtons" Src="~/_controls/EditButtons.ascx" %>
	<SplendidCRM:EditButtons ID="ctlEditButtons" Visible="<%# !PrintView %>" Runat="Server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableHeaderRow>
						<asp:TableHeaderCell ColumnSpan="4"><h4><asp:Label Text='<%# L10n.Term("EmailMan.LBL_NOTIFY_TITLE") %>' runat="server" /></h4></asp:TableHeaderCell>
					</asp:TableHeaderRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("EmailMan.LBL_NOTIFY_FROMNAME") %>' runat="server" /><asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox ID="NOTIFY_FROMNAME" size="25" MaxLength="128" Runat="server" />
							<asp:RequiredFieldValidator ID="reqNOTIFY_FROMNAME" ControlToValidate="NOTIFY_FROMNAME" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("EmailMan.LBL_NOTIFY_ON") %>' runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:CheckBox ID="NOTIFY_ON" Enabled="false" CssClass="checkbox" Runat="server" />
							<em><asp:Label Text='<%# L10n.Term("EmailMan.LBL_NOTIFICATION_ON_DESC") %>' runat="server" /></em>
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("EmailMan.LBL_NOTIFY_FROMADDRESS") %>' runat="server" /><asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField">
							<asp:TextBox ID="NOTIFY_FROMADDRESS" size="25" MaxLength="128" Runat="server" />
							<asp:RequiredFieldValidator ID="reqNOTIFY_FROMADDRESS" ControlToValidate="NOTIFY_FROMADDRESS" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
						<asp:TableCell CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("EmailMan.LBL_NOTIFY_SEND_BY_DEFAULT") %>' runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:CheckBox ID="NOTIFY_SEND_BY_DEFAULT" Enabled="false" CssClass="checkbox" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell></asp:TableCell>
						<asp:TableCell></asp:TableCell>
						<asp:TableCell CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("EmailMan.LBL_NOTIFY_SEND_FROM_ASSIGNING_USER") %>' runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:CheckBox ID="NOTIFY_SEND_FROM_ASSIGNING_USER" Enabled="false" CssClass="checkbox" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell ColumnSpan="4">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("EmailMan.LBL_MAIL_SENDTYPE") %>' runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField">
							<asp:Label ID="MAIL_SENDTYPE" Text="SMTP" Runat="server" />
							&nbsp;
							<asp:LinkButton OnClientClick="GmailDefaults(); return false;" Text='<%# L10n.Term("EmailMan.LBL_EMAIL_GMAIL_DEFAULTS") %>' runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("EmailMan.LBL_MAIL_SMTPSERVER") %>' runat="server" /><asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox ID="MAIL_SMTPSERVER" size="25" MaxLength="64" Runat="server" />
							<asp:RequiredFieldValidator ID="reqMAIL_SMTPSERVER" ControlToValidate="MAIL_SMTPSERVER" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("EmailMan.LBL_MAIL_SMTPPORT") %>' runat="server" /><asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox ID="MAIL_SMTPPORT" size="25" MaxLength="64" Runat="server" />
							<asp:RequiredFieldValidator ID="reqMAIL_SMTPPORT" ControlToValidate="MAIL_SMTPPORT" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("EmailMan.LBL_MAIL_SMTPAUTH_REQ") %>' runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:CheckBox ID="MAIL_SMTPAUTH_REQ" Enabled="false" CssClass="checkbox" Runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("EmailMan.LBL_MAIL_SMTPSSL") %>' runat="server" /></asp:TableCell>
						<asp:TableCell CssClass="dataField"><asp:CheckBox ID="MAIL_SMTPSSL" CssClass="checkbox" Runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("EmailMan.LBL_MAIL_SMTPUSER") %>' runat="server" /><asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox ID="MAIL_SMTPUSER" size="25" MaxLength="64" Runat="server" />
							<asp:RequiredFieldValidator ID="reqMAIL_SMTPUSER" ControlToValidate="MAIL_SMTPUSER" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
						<asp:TableCell Width="15%" CssClass="dataLabel"><asp:Label Text='<%# L10n.Term("EmailMan.LBL_MAIL_SMTPPASS") %>' runat="server" /><asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						<asp:TableCell Width="35%" CssClass="dataField">
							<asp:TextBox ID="MAIL_SMTPPASS" size="25" MaxLength="64" TextMode="Password" Runat="server" />
							<asp:RequiredFieldValidator ID="reqMAIL_SMTPPASS" ControlToValidate="MAIL_SMTPPASS" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	<p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableHeaderRow>
						<asp:TableHeaderCell ColumnSpan="4"><h4><asp:Label ID="Label1" Text='<%# L10n.Term("EmailMan.LBL_SECURITY_TITLE") %>' runat="server" /></h4></asp:TableHeaderCell>
					</asp:TableHeaderRow>
					<asp:TableRow>
						<asp:TableCell ColumnSpan="4"><asp:Label Text='<%# L10n.Term("EmailMan.LBL_SECURITY_DESC") %>' runat="server" /></asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell ColumnSpan="2">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell Width="1%" VerticalAlign="Top">
							<asp:CheckBox ID="EMAIL_INBOUND_SAVE_RAW" CssClass="checkbox" runat="server" />
						</asp:TableCell>
						<asp:TableCell>
							<asp:Label Text='<%# L10n.Term("EmailMan.LBL_SECURITY_PRESERVE_RAW") %>' runat="server" /><br />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell ColumnSpan="2">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell VerticalAlign="Top">
							<asp:CheckBox ID="SECURITY_TOGGLE_ALL" CssClass="checkbox" onclick="toggleAllSecurityOptions();" runat="server" />
						</asp:TableCell>
						<asp:TableCell>
							<asp:Label Text='<%# L10n.Term("EmailMan.LBL_SECURITY_TOGGLE_ALL") %>' runat="server" /><br />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell ColumnSpan="2">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell VerticalAlign="Top">
							<asp:CheckBox ID="SECURITY_OUTLOOK_DEFAULTS" CssClass="checkbox" onclick="setOutlookDefaults();" runat="server" />
						</asp:TableCell>
						<asp:TableCell>
							<asp:Label Text='<%# L10n.Term("EmailMan.LBL_SECURITY_OUTLOOK_DEFAULTS") %>' runat="server" /><br />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell ColumnSpan="2">&nbsp;</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell ColumnSpan="2">
							<asp:Table ID="tblSECURITY_TAGS" runat="server" />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
</div>
