<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ConvertView.ascx.cs" Inherits="SplendidCRM.Leads.ConvertView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
 * Portions created by SplendidCRM Software are Copyright (C) 2008 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
%>
<div id="divEditView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Leads" EnablePrint="false" EnableHelp="true" Runat="Server" />
	<p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="EditButtons" Src="~/_controls/EditButtons.ascx" %>
	<SplendidCRM:EditButtons ID="ctlEditButtons" Visible="<%# !PrintView %>" Runat="Server" />
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<table ID="tblMain" class="tabEditView" runat="server">
					<tr>
						<th colspan="4"><h4><%= L10n.Term("Leads.LNK_NEW_CONTACT") %></h4></th>
					</tr>
				</table>
				<div id="divCreateContactNoteLink"><a href="javascript:toggleDisplay('divCreateContactNote');toggleDisplay('divCreateContactNoteLink');"><%= L10n.Term("Leads.LNK_NEW_NOTE") %></a></div>
				<div id="divCreateContactNote" style="display:none">
					<p>
					<asp:Table BorderWidth="0" CellPadding="0" CellSpacing="0" runat="server">
						<asp:TableRow>
							<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Notes.LBL_NOTE_SUBJECT") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
						</asp:TableRow>
						<asp:TableRow>
							<asp:TableCell CssClass="dataField">
								<asp:TextBox ID="txtCONTACT_NOTES_NAME" MaxLength="255" size="85" Runat="server" />
								<asp:RequiredFieldValidator ID="reqCONTACT_NOTES_NAME" ControlToValidate="txtCONTACT_NOTES_NAME" ErrorMessage="(required)" CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
							</asp:TableCell>
						</asp:TableRow>
						<asp:TableRow>
							<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Notes.LBL_NOTE") %></asp:TableCell>
						</asp:TableRow>
						<asp:TableRow>
							<asp:TableCell CssClass="dataField"><asp:TextBox ID="txtCONTACT_NOTES_NAME_DESCRIPTION" TextMode="MultiLine" Rows="4" Columns="85" Runat="server" /></asp:TableCell>
						</asp:TableRow>
					</asp:Table>
					</p>
				</div>
				<br />
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
	<p>
	<asp:Table SkinID="tabForm" runat="server">
		<asp:TableRow>
			<asp:TableCell>
				<asp:Table SkinID="tabEditView" runat="server">
					<asp:TableRow>
						<asp:TableHeaderCell CssClass="dataLabel">
							<h4 CssClass="dataLabel"><%= L10n.Term(".LBL_RELATED_RECORDS") %></h4>
						</asp:TableHeaderCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell VerticalAlign="top">
							<div id="divSelectAccount" style="display:inline">
								<script type="text/javascript">
									function ChangeAccount(sPARENT_ID, sPARENT_NAME)
									{
										document.getElementById('<%= txtSELECT_ACCOUNT_ID.ClientID   %>').value = sPARENT_ID  ;
										document.getElementById('<%= txtSELECT_ACCOUNT_NAME.ClientID %>').value = sPARENT_NAME;
									}
									function AccountPopup()
									{
										return window.open('../Accounts/Popup.aspx','AccountPopup','width=600,height=400,resizable=1,scrollbars=1');
									}
									function ToggleCreateAccount()
									{
										var divCreateAccount = document.getElementById('divCreateAccount');
										var divSelectAccount = document.getElementById('divSelectAccount');
										if( divCreateAccount.style.display == 'none' )
										{
											divCreateAccount.style.display = 'inline';
											divSelectAccount.style.display = 'none'  ;
											ClearSelectAccount();
										}
										else
										{
											divCreateAccount.style.display = 'none'  ;
											divSelectAccount.style.display = 'inline';
										}
									}
									function ClearSelectAccount()
									{
										document.getElementById('txtSELECT_ACCOUNT_ID'  ).value   = '';
										document.getElementById('txtSELECT_ACCOUNT_NAME').value = '';
										return false;
									}
								</script>
								<b><%= L10n.Term("Leads.LNK_SELECT_ACCOUNT") %></b>&nbsp;
								<asp:TextBox ID="txtSELECT_ACCOUNT_NAME" ReadOnly="True" Runat="server" />
								<input ID="txtSELECT_ACCOUNT_ID" type="hidden" runat="server" />
								<input ID="btnChangeSelectAccount" type="button" CssClass="button" onclick="return AccountPopup();"       title="<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>" AccessKey="<%# L10n.AccessKey(".LBL_CHANGE_BUTTON_KEY") %>" value="<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>" />
								<input ID="btnClearSelectAccount"  type="button" CssClass="button" onclick="return ClearSelectAccount();" title="<%# L10n.Term(".LBL_CLEAR_BUTTON_TITLE" ) %>" AccessKey="<%# L10n.AccessKey(".LBL_CLEAR_BUTTON_KEY" ) %>" value="<%# L10n.Term(".LBL_CLEAR_BUTTON_LABEL" ) %>" />
								<SplendidCRM:RequiredFieldValidatorForHiddenInputs ID="reqSELECT_ACCOUNT_ID" ControlToValidate="txtSELECT_ACCOUNT_ID" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
								<b><%= L10n.Term(".LBL_OR") %></b>
							</div>
							<h5 CssClass="dataLabel">
								<asp:CheckBox ID="chkCreateAccount" CssClass="checkbox" Runat="server" />
								<%= L10n.Term("Leads.LNK_NEW_ACCOUNT") %>
							</h5>
							<div id="divCreateAccount" style="display:<%= (chkCreateAccount.Checked ? "inline" : "none") %>">
								<asp:Table runat="server" CssClass="tabEditView">
									<asp:TableRow>
										<asp:TableCell width="20%" Wrap="false" CssClass="dataLabel"><%= L10n.Term("Accounts.LBL_ACCOUNT_NAME") %>&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
										<asp:TableCell width="80%" Wrap="false" CssClass="dataLabel"><%= L10n.Term("Accounts.LBL_DESCRIPTION") %></asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell Wrap="false" CssClass="dataField">
											<asp:TextBox ID="txtACCOUNT_NAME" Runat="server" />
											<asp:RequiredFieldValidator ID="reqACCOUNT_NAME" ControlToValidate="txtACCOUNT_NAME" ErrorMessage="(required)" CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
										</asp:TableCell>
										<asp:TableCell rowspan="5" CssClass="dataField">
											<asp:TextBox ID="txtACCOUNT_DESCRIPTION" TextMode="MultiLine" Rows="6" Columns="50" Runat="server" />
										</asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell Wrap="false" CssClass="dataLabel"><%= L10n.Term("Accounts.LBL_PHONE") %></asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell Wrap="false" CssClass="dataField"><asp:TextBox ID="txtACCOUNT_PHONE_WORK" Runat="server" /></asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell Wrap="false" CssClass="dataLabel"><%= L10n.Term("Accounts.LBL_WEBSITE") %></asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell Wrap="false" CssClass="dataField"><asp:TextBox ID="txtACCOUNT_WEBSITE" Runat="server">http://</asp:TextBox></asp:TableCell>
									</asp:TableRow>
								</asp:Table>
								<div id="divCreateAccountNoteLink"><a href="javascript:toggleDisplay('divCreateAccountNote');toggleDisplay('divCreateAccountNoteLink');"><%= L10n.Term("Leads.LNK_NEW_NOTE") %></a></div>
								<div id="divCreateAccountNote" style="display:none">
									<p>
									<asp:Table BorderWidth="0" CellPadding="0" CellSpacing="0" runat="server">
										<asp:TableRow>
											<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Notes.LBL_NOTE_SUBJECT") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
										</asp:TableRow>
										<asp:TableRow>
											<asp:TableCell CssClass="dataField">
												<asp:TextBox ID="txtACCOUNT_NOTES_NAME" MaxLength="255" size="85" Runat="server" />
												<asp:RequiredFieldValidator ID="reqACCOUNT_NOTES_NAME" ControlToValidate="txtACCOUNT_NOTES_NAME" ErrorMessage="(required)" CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
											</asp:TableCell>
										</asp:TableRow>
										<asp:TableRow>
											<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Notes.LBL_NOTE") %></asp:TableCell>
										</asp:TableRow>
										<asp:TableRow>
											<asp:TableCell CssClass="dataField"><asp:TextBox ID="txtACCOUNT_NOTES_NAME_DESCRIPTION" TextMode="MultiLine" Rows="4" Columns="85" Runat="server" /></asp:TableCell>
										</asp:TableRow>
									</asp:Table>
									</p>
								</div>
								<br />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell VerticalAlign="top">
							<h5 CssClass="dataLabel">
								<asp:CheckBox ID="chkCreateOpportunity" CssClass="checkbox" Runat="server" />
								<%= L10n.Term("Leads.LNK_NEW_OPPORTUNITY") %>
							</h5>
							<div id="divCreateOpportunity" style="display:<%= (chkCreateOpportunity.Checked ? "inline" : "none") %>">
								<asp:Table runat="server" CssClass="tabEditView">
									<asp:TableRow>
										<asp:TableCell Width="20%" CssClass="dataLabel"><%= L10n.Term("Opportunities.LBL_NAME"       ) %>&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
										<asp:TableCell Width="80%" CssClass="dataLabel"><%= L10n.Term("Opportunities.LBL_DESCRIPTION") %></asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell CssClass="dataField">
											<asp:TextBox ID="txtOPPORTUNITY_NAME" MaxLength="50" Runat="server" />
											<asp:RequiredFieldValidator ID="reqOPPORTUNITY_NAME" ControlToValidate="txtOPPORTUNITY_NAME" ErrorMessage="(required)" CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
										</asp:TableCell>
										<asp:TableCell CssClass="dataField" rowspan="7"><asp:TextBox ID="txtOPPORTUNITY_DESCRIPTION" TextMode="MultiLine" Rows="5" Columns="50" Runat="server" /></asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell CssClass="dataLabel">
											<%= L10n.Term("Opportunities.LBL_DATE_CLOSED") %>&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /><br />
											<asp:Label ID="lblDATEFORMAT" CssClass="dateFormat" Runat="server" />
										</asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell CssClass="dataField">
											<%@ Register TagPrefix="SplendidCRM" Tagname="DatePicker" Src="~/_controls/DatePicker.ascx" %>
											<SplendidCRM:DatePicker ID="ctlOPPORTUNITY_DATE_CLOSED" Runat="Server" />
										</asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Opportunities.LBL_SALES_STAGE") %>&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell CssClass="dataField">
											<asp:DropDownList ID="lstOPPORTUNITY_SALES_STAGE" DataValueField="NAME" DataTextField="DISPLAY_NAME" Runat="server" />
										</asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Opportunities.LBL_AMOUNT") %>&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell CssClass="dataField">
											<asp:TextBox ID="txtOPPORTUNITY_AMOUNT" MaxLength="10" Runat="server" />
											<asp:RequiredFieldValidator ID="reqOPPORTUNITY_AMOUNT" ControlToValidate="txtOPPORTUNITY_AMOUNT" ErrorMessage="(required)" CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
										</asp:TableCell>
									</asp:TableRow>
								</asp:Table>
								<div id="divCreateOpportunityNoteLink"><a href="javascript:toggleDisplay('divCreateOpportunityNote');toggleDisplay('divCreateOpportunityNoteLink');"><%= L10n.Term("Leads.LNK_NEW_NOTE") %></a></div>
								<div id="divCreateOpportunityNote" style="display:none">
									<input type="hidden" name="OpportunityNotesparent_type" value="Accounts">
									<p>
										<asp:Table BorderWidth="0" CellPadding="0" CellSpacing="0" runat="server">
											<asp:TableRow>
												<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Notes.LBL_NOTE_SUBJECT") %> <asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
											</asp:TableRow>
											<asp:TableRow>
												<asp:TableCell CssClass="dataField">
													<asp:TextBox ID="txtOPPORTUNITY_NOTES_NAME" MaxLength="255" size="85" Runat="server" />
													<asp:RequiredFieldValidator ID="reqOPPORTUNITY_NOTES_NAME" ControlToValidate="txtOPPORTUNITY_NOTES_NAME" ErrorMessage="(required)" CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
												</asp:TableCell>
											</asp:TableRow>
											<asp:TableRow>
												<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Notes.LBL_NOTE") %></asp:TableCell>
											</asp:TableRow>
											<asp:TableRow>
												<asp:TableCell CssClass="dataField"><asp:TextBox ID="txtOPPORTUNITY_NOTES_NAME_DESCRIPTION" TextMode="MultiLine" Rows="4" Columns="85" Runat="server" /></asp:TableCell>
											</asp:TableRow>
										</asp:Table>
									</p>
								</div>
								<br />
						</asp:TableCell>
					</asp:TableRow>
					<asp:TableRow>
						<asp:TableCell VerticalAlign="top">
							<h5 CssClass="dataLabel">
								<asp:CheckBox ID="chkCreateAppointment" CssClass="checkbox" Runat="server" />
								<%= L10n.Term("Leads.LNK_NEW_APPOINTMENT") %>
							</h5>
							<div id="divCreateAppointment" style="display:<%= (chkCreateAppointment.Checked ? "inline" : "none") %>">
								<asp:Table runat="server" CssClass="tabEditView">
									<asp:TableRow>
										<asp:TableCell CssClass="dataLabel" width="20%">
											<asp:RadioButton ID="radScheduleCall" GroupName="grpSchedule" CssClass="radio" Runat="server" /><span class="dataLabel"><%= L10n.Term("Calls.LNK_NEW_CALL") %></span>
										</asp:TableCell>
										<asp:TableCell CssClass="dataLabel" width="80%"><%= L10n.Term("Meetings.LBL_DESCRIPTION") %></asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell CssClass="dataLabel">
											<asp:RadioButton ID="radScheduleMeeting" GroupName="grpSchedule" CssClass="radio" Checked="true" Runat="server" /><span class="dataLabel"><%= L10n.Term("Calls.LNK_NEW_MEETING") %></span>
										</asp:TableCell>
										<asp:TableCell rowspan="8" CssClass="dataField">
											<asp:TextBox ID="txtAPPOINTMENT_DESCRIPTION" Rows="5" Columns="50" Runat="server" />
										</asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Meetings.LBL_SUBJECT") %>&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell CssClass="dataField">
											<asp:TextBox ID="txtAPPOINTMENT_NAME" size="25" MaxLength="50" Runat="server" />
											<asp:RequiredFieldValidator ID="reqAPPOINTMENT_NAME" ControlToValidate="txtAPPOINTMENT_NAME" ErrorMessage="(required)" CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
										</asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Meetings.LBL_DATE") %>&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" />&nbsp;<span class="dateFormat"><%= System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern.ToUpper() %></span></asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell CssClass="dataField">
											<SplendidCRM:DatePicker ID="ctlAPPOINTMENT_DATE_START" Runat="Server" />
										</asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell CssClass="dataLabel"><%= L10n.Term("Meetings.LBL_TIME") %>&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /></asp:TableCell>
									</asp:TableRow>
									<asp:TableRow>
										<asp:TableCell CssClass="dataField">
											<asp:TextBox ID="txtAPPOINTMENT_TIME_START" size="12" MaxLength="10" Runat="server" />
											<asp:RequiredFieldValidator ID="reqAPPOINTMENT_TIME_START" ControlToValidate="txtAPPOINTMENT_TIME_START" ErrorMessage="(required)" CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
										</asp:TableCell>
									</asp:TableRow>
								</asp:Table>
							</div>
							<br />
						</asp:TableCell>
					</asp:TableRow>
				</asp:Table>
			</asp:TableCell>
		</asp:TableRow>
	</asp:Table>
	</p>
</div>
