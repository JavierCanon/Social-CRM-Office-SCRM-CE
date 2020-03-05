<%@ Control CodeBehind="ImportView.ascx.cs" Language="c#" AutoEventWireup="false" Inherits="SplendidCRM.Import.ImportView" %>
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
<div id="divImportView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Import" Title="Import.LBL_MODULE_NAME" EnableModuleLabel="false" EnablePrint="false" EnableHelp="true" Runat="Server" />
	<p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="ImportButtons" Src="_controls/ImportButtons.ascx" %>
	<SplendidCRM:ImportButtons ID="ctlImportButtons" Visible="<%# !PrintView %>" Runat="Server" />
	</p>

	<script type="text/javascript">
	function SelectWizardTab(key)
	{
		for ( var i = 1; i <= 5; i++ )
		{
			var sListClass = '';
			var sLinkClass = '';
			var sListStyle = 'none';

			if ( key == i )
			{
				sListClass = 'active' ;
				sLinkClass = 'current';
				sListStyle = 'block'  ;
			}
			document.getElementById('liImportStep'   + i).className     = sListClass;
			document.getElementById('linkImportStep' + i).className     = sLinkClass;
			document.getElementById('divImportStep'  + i).style.display = sListStyle;
		}
		document.getElementById('<%= txtACTIVE_TAB.ClientID %>').value = key;
	}
	function SelectSourceFormat()
	{
		var radEXCEL            = document.getElementById('<%= radEXCEL           .ClientID %>');
		var radXML_SPREADSHEET  = document.getElementById('<%= radXML_SPREADSHEET .ClientID %>');
		var radXML              = document.getElementById('<%= radXML             .ClientID %>');
		var radSALESFORCE       = document.getElementById('<%= radSALESFORCE      .ClientID %>');
		var radACT_2005         = document.getElementById('<%= radACT_2005        .ClientID %>');
		var radCUSTOM_CSV       = document.getElementById('<%= radCUSTOM_CSV      .ClientID %>');
		var radCUSTOM_TAB       = document.getElementById('<%= radCUSTOM_TAB      .ClientID %>');
		var radCUSTOM_DELIMITED = document.getElementById('<%= radCUSTOM_DELIMITED.ClientID %>');
		
		document.getElementById('tblInstructionsExcel'         ).style.display = radEXCEL           .checked ? 'inline' : 'none';
		document.getElementById('tblInstructionsXmlSpreadsheet').style.display = radXML_SPREADSHEET .checked ? 'inline' : 'none';
		document.getElementById('tblInstructionsXML'           ).style.display = radXML             .checked ? 'inline' : 'none';
		document.getElementById('tblInstructionsSalesForce'    ).style.display = radSALESFORCE      .checked ? 'inline' : 'none';
		document.getElementById('tblInstructionsAct'           ).style.display = radACT_2005        .checked ? 'inline' : 'none';
		document.getElementById('tblInstructionsCommaDelimited').style.display = radCUSTOM_CSV      .checked ? 'inline' : 'none';
		document.getElementById('tblInstructionsTabDelimited'  ).style.display = radCUSTOM_TAB      .checked ? 'inline' : 'none';
		document.getElementById('tblInstructionsCommaDelimited').style.display = radCUSTOM_DELIMITED.checked ? 'inline' : 'none';
		document.getElementById('divCUSTOM_DELIMITER_VAL'      ).style.display = radCUSTOM_DELIMITED.checked ? 'inline' : 'none';

		var sImportModule = '<%= sImportModule %>';
		document.getElementById('tblNotesAccounts'     ).style.display = (sImportModule == 'Accounts'     ) ? 'inline' : 'none';
		document.getElementById('tblNotesContacts'     ).style.display = (sImportModule == 'Contacts'     ) ? 'inline' : 'none';
		document.getElementById('tblNotesOpportunities').style.display = (sImportModule == 'Opportunities') ? 'inline' : 'none';
	}
	</script>

	<input id="txtACTIVE_TAB" type="hidden" runat="server" />
	<ul class="tablist">
		<li id="liImportStep1" class="active"><a id="linkImportStep1" href="javascript:SelectWizardTab(1);" class="current"><%= L10n.Term("Import.LBL_IMPORT_STEP1") %></a></li>
		<li id="liImportStep2" class=""      ><a id="linkImportStep2" href="javascript:SelectWizardTab(2);" class=""       ><%= L10n.Term("Import.LBL_IMPORT_STEP2") %></a></li>
		<li id="liImportStep3" class=""      ><a id="linkImportStep3" href="javascript:SelectWizardTab(3);" class=""       ><%= L10n.Term("Import.LBL_IMPORT_STEP3") %></a></li>
		<li id="liImportStep4" class=""      ><a id="linkImportStep4" href="javascript:SelectWizardTab(4);" class=""       ><%= L10n.Term("Import.LBL_IMPORT_STEP4") %></a></li>
		<li id="liImportStep5" class=""      ><a id="linkImportStep5" href="javascript:SelectWizardTab(5);" class=""       ><%= L10n.Term("Import.LBL_IMPORT_STEP5") %></a></li>
	</ul>
	<div id="divImportStep1" style="DISPLAY:none">
		<asp:Table SkinID="tabForm" runat="server">
			<asp:TableRow>
				<asp:TableCell>
					<table class="tabEditView">
						<tr>
							<td class="dataLabel" colspan="2"><h4><asp:Label Text='<%# L10n.Term("Import.LBL_WHAT_IS") %>' runat="server" /></h4></td>
						</tr>
						<tr>
							<td width="35%" class="dataLabel">
								<nobr>
									<asp:RadioButton ID="radEXCEL"            GroupName="radSOURCE" Text='<%# L10n.Term("Import.LBL_EXCEL"           ) %>' CssClass="checkbox" runat="server" /><br />
									<asp:RadioButton ID="radXML_SPREADSHEET"  GroupName="radSOURCE" Text='<%# L10n.Term("Import.LBL_XML_SPREADSHEET" ) %>' CssClass="checkbox" runat="server" /><br />
									<asp:RadioButton ID="radXML"              GroupName="radSOURCE" Text='<%# L10n.Term("Import.LBL_XML"             ) %>' CssClass="checkbox" runat="server" /><br />
									<asp:RadioButton ID="radSALESFORCE"       GroupName="radSOURCE" Text='<%# L10n.Term("Import.LBL_SALESFORCE"      ) %>' CssClass="checkbox" runat="server" /><br />
									<asp:RadioButton ID="radACT_2005"         GroupName="radSOURCE" Text='<%# L10n.Term("Import.LBL_ACT_2005"        ) %>' CssClass="checkbox" runat="server" /><br />
									<asp:RadioButton ID="radCUSTOM_CSV"       GroupName="radSOURCE" Text='<%# L10n.Term("Import.LBL_CUSTOM_CSV"      ) %>' CssClass="checkbox" runat="server" /><br />
									<asp:RadioButton ID="radCUSTOM_TAB"       GroupName="radSOURCE" Text='<%# L10n.Term("Import.LBL_CUSTOM_TAB"      ) %>' CssClass="checkbox" runat="server" /><br />
									<asp:RadioButton ID="radCUSTOM_DELIMITED" GroupName="radSOURCE" Text='<%# L10n.Term("Import.LBL_CUSTOM_DELIMETED") %>' CssClass="checkbox" runat="server" /><br />
									<div id="divCUSTOM_DELIMITER_VAL" style="DISPLAY: none">
										&nbsp; &nbsp; <%= L10n.Term("Import.LBL_CUSTOM_DELIMETER") %> <asp:TextBox ID="txtCUSTOM_DELIMITER_VAL" MaxLength="1" size="3" runat="server" />
									</div>
								</nobr>
							</td>
							<td valign="top">
								<table border="0" cellspacing="0" cellpadding="0">
									<tr>
										<td class="dataLabel"><h4><asp:Label Text='<%# L10n.Term("Import.LBL_NAME") %>' runat="server" /></h4></td>
										<td class="dataField">
											<asp:TextBox ID="txtNAME" TabIndex="2" size="35" MaxLength="150" Runat="server" />
											<asp:Button ID="btnSave" CommandName="Import.Save" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term("Import.LBL_SAVE_BUTTON_LABEL") + "  " %>' ToolTip='<%# L10n.Term("Import.LBL_SAVE_BUTTON_TITLE") %>' Runat="server" />
											<asp:RequiredFieldValidator ID="reqNAME" ControlToValidate="txtNAME" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Display="Dynamic" Runat="server" />
										</td>
									</tr>
								</table>
							</td>
						</tr>
					</table>
					<br />
					<%@ Register TagPrefix="SplendidCRM" Tagname="ListHeader" Src="~/_controls/ListHeader.ascx" %>
					<SplendidCRM:ListHeader Visible="<%# !PrintView %>" Title="Import.LBL_MY_SAVED" Runat="Server" />
					<SplendidCRM:SplendidGrid id="grdMySaved" Width="100%" CssClass="listView"
						CellPadding="3" CellSpacing="0" border="0"
						AllowPaging="false" PageSize="20" AllowSorting="false" 
						AutoGenerateColumns="false" 
						EnableViewState="true" runat="server">
						<ItemStyle            CssClass="oddListRowS1"  />
						<AlternatingItemStyle CssClass="evenListRowS1" />
						<HeaderStyle          CssClass="listViewThS1"  />
						<PagerStyle HorizontalAlign="Right" Mode="NextPrev" PageButtonCount="6" Position="Top" CssClass="listViewPaginationTdS1" PrevPageText=".LNK_LIST_PREVIOUS" NextPageText=".LNK_LIST_NEXT" />
						<Columns>
							<asp:TemplateColumn HeaderText="Import.LBL_LIST_NAME"  SortExpression="NAME" ItemStyle-Width="85%" ItemStyle-CssClass="listViewTdLinkS1">
								<ItemTemplate>
									<asp:LinkButton  CommandName="Import.Load" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# DataBinder.Eval(Container.DataItem, "NAME") %>' Runat="server" />
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false">
								<ItemTemplate>
									<span onclick="return confirm('<%= L10n.TermJavaScript(".NTC_DELETE_CONFIRMATION") %>')">
										<asp:ImageButton CommandName="Import.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" AlternateText='<%# L10n.Term(".LNK_DELETE") %>' ImageUrl='<%# Session["themeURL"] + "images/delete_inline.gif" %>' BorderWidth="0" Width="12" Height="12" ImageAlign="AbsMiddle" Runat="server" />
										<asp:LinkButton  CommandName="Import.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="listViewTdToolsS1" Text='<%# L10n.Term(".LNK_DELETE") %>' Runat="server" />
									</span>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</SplendidCRM:SplendidGrid>

				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</div>
	<div id="divImportStep2" style="DISPLAY:none">
		<asp:PlaceHolder ID="phDefaultsView" Runat="server" />
	</div>
	<div id="divImportStep3" style="DISPLAY:none">
		<asp:Table SkinID="tabForm" runat="server">
			<asp:TableRow>
				<asp:TableCell>
					<table id="tblInstructionsExcel" width="100%" cellpadding="0" cellspacing="0" border="0" style="DISPLAY: none">
						<tr>
							<td class="body">
								<p><%= L10n.Term("Import.LBL_IMPORT_EXCEL_TITLE") %></p>
							</td>
						</tr>
					</table>
					<table id="tblInstructionsXmlSpreadsheet" width="100%" cellpadding="0" cellspacing="0" border="0" style="DISPLAY: none">
						<tr>
							<td class="body">
								<p><%= L10n.Term("Import.LBL_IMPORT_XML_SPREADSHEET_TITLE") %></p>
							</td>
						</tr>
					</table>
					<table id="tblInstructionsXML" width="100%" cellpadding="0" cellspacing="0" border="0" style="DISPLAY: none">
						<tr>
							<td class="body">
								<p><%= L10n.Term("Import.LBL_IMPORT_XML_TITLE") %></p>
								<pre>
			&lt;xml&gt;
			   &lt;<%= sImportModule.ToLower() %>&gt;
				  &lt;id&gt;&lt;/id&gt;
				  &lt;name&gt;&lt;/name&gt;
			   &lt;/<%= sImportModule.ToLower() %>&gt;
			&lt;/xml&gt;</pre>
							</td>
						</tr>
					</table>
					<table id="tblInstructionsSalesForce" width="100%" cellpadding="0" cellspacing="0" border="0" style="DISPLAY: none">
						<tr>
							<td class="body">
								<p><%= L10n.Term("Import.LBL_IMPORT_SF_TITLE") %></p>
							</td>
						</tr>
						<tr>
							<td class="body">
								<table>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_1") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_SF_NUM_1") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_2") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_SF_NUM_2") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_3") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_SF_NUM_3") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_4") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_SF_NUM_4") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_5") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_SF_NUM_5") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_6") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_SF_NUM_6") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_7") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_SF_NUM_7") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_8") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_SF_NUM_8") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_9") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_SF_NUM_9") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_10") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_SF_NUM_10") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_11") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_SF_NUM_11") %></td>
									</tr>
								</table>
							</td>
						</tr>
					</table>
					<table id="tblInstructionsAct" width="100%" cellpadding="0" cellspacing="0" border="0" style="DISPLAY: none">
						<tr>
							<td class="body">
								<p><%= L10n.Term("Import.LBL_IMPORT_ACT_TITLE") %></p>
							</td>
						</tr>
						<tr>
							<td class="body">
								<table>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_1") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_ACT_NUM_1") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_2") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_ACT_NUM_2") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_3") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_ACT_NUM_3") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_4") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_ACT_NUM_4") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_5") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_ACT_NUM_5") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_6") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_ACT_NUM_6") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_7") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_ACT_NUM_7") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_8") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_ACT_NUM_8") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_9") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_ACT_NUM_9") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_10") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_ACT_NUM_10") %></td>
									</tr>
								</table>
							</td>
						</tr>
					</table>
					<table id="tblInstructionsCommaDelimited" width="100%" cellpadding="0" cellspacing="0" border="0" style="DISPLAY: none">
						<tr>
							<td class="body">
								<p><%= L10n.Term("Import.LBL_IMPORT_CUSTOM_TITLE") %></p>
							</td>
						</tr>
						<tr>
							<td class="body">
								<table>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_1") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_CUSTOM_NUM_1") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_2") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_CUSTOM_NUM_2") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_3") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_CUSTOM_NUM_3") %></td>
									</tr>
								</table>
							</td>
						</tr>
					</table>
					<table id="tblInstructionsTabDelimited" width="100%" cellpadding="0" cellspacing="0" border="0" style="DISPLAY: none">
						<tr>
							<td class="body">
								<p><%= L10n.Term("Import.LBL_IMPORT_TAB_TITLE") %></p>
							</td>
						</tr>
						<tr>
							<td class="body">
								<table>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_1") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_TAB_NUM_1") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_2") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_TAB_NUM_2") %></td>
									</tr>
									<tr>
										<td class="body" valign="top"><b><%= L10n.Term("Import.LBL_NUM_3") %></b></td>
										<td class="body"><%= L10n.Term("Import.LBL_TAB_NUM_3") %></td>
									</tr>
								</table>
							</td>
						</tr>
					</table>
					<br>
					<table border="0" cellspacing="0" cellpadding="0" width="100%">
						<tr>
							<td align="left" class="dataLabel" colspan="4">
								<%= L10n.Term("Import.LBL_SELECT_FILE") %>&nbsp;<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" />
								<asp:RequiredFieldValidator ID="reqFILENAME" ControlToValidate="fileIMPORT" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" Enabled="false" EnableClientScript="false" EnableViewState="false" Runat="server" />
							</td>
						</tr>
						<tr>
							<td class="dataLabel">
								<input id="fileIMPORT" type="file" size="60" MaxLength="255" runat="server" />
								<asp:Button ID="btnUpload" CommandName="Import.Upload" OnCommand="Page_Command" CssClass="button" Text='<%# "  " + L10n.Term("Import.LBL_UPLOAD_BUTTON_LABEL" ) + "  " %>' ToolTip='<%# L10n.Term("Import.LBL_UPLOAD_BUTTON_TITLE" ) %>' Runat="server" />
							</td>
						</tr>
						<tr>
							<td class="dataField">
								<%= L10n.Term("Import.LBL_HAS_HEADER") %>&nbsp;
								<asp:CheckBox ID="chkHasHeader" CssClass="checkbox" Runat="server" />
							</td>
						</tr>
					</table>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</div>
	<div id="divImportStep4" style="DISPLAY:none">
		<table width="100%" cellpadding="0" cellspacing="0" border="0" visible="true">
			<tr>
				<td align="right" nowrap>
					<asp:Label CssClass="required" Text='<%# L10n.Term(".LBL_REQUIRED_SYMBOL") %>' Runat="server" /> <%= L10n.Term(".NTC_REQUIRED") %>
				</td>
			</tr>
			<tr>
				<td class="body">
					<%= L10n.Term("Import.LBL_SELECT_FIELDS_TO_MAP") %>
				</td>
			</tr>
		</table>
		<table id="tblImportMappings" class="tabDetailView" runat="server" />

		<table id="tblNotesAccounts" width="100%" cellpadding="0" cellspacing="0" border="0" style="DISPLAY: none">
			<tr>
				<td class="body">
					<br />
					<b><%= L10n.Term("Import.LBL_NOTES") %></b>
					<ul>
						<li><%= L10n.Term("Import.LBL_ACCOUNTS_NOTE_1") %></li>
						<!--
						<li><%= L10n.Term("Import.LBL_ACCOUNTS_NOTE_2") %></li>
						-->
					</ul>
				</td>
			</tr>
		</table>
		<table id="tblNotesContacts" width="100%" cellpadding="0" cellspacing="0" border="0" style="DISPLAY: none">
			<tr>
				<td class="body">
					<br />
					<b><%= L10n.Term("Import.LBL_NOTES") %></b>
					<ul>
						<li><%= L10n.Term("Import.LBL_CONTACTS_NOTE_1") %></li>
						<!--
						<li><%= L10n.Term("Import.LBL_CONTACTS_NOTE_2") %></li>
						<li><%= L10n.Term("Import.LBL_CONTACTS_NOTE_3") %></li>
						<li><%= L10n.Term("Import.LBL_CONTACTS_NOTE_4") %></li>
						-->
					</ul>
				</td>
			</tr>
		</table>
		<table id="tblNotesOpportunities" width="100%" cellpadding="0" cellspacing="0" border="0" style="DISPLAY: none">
			<tr>
				<td class="body">
					<br />
					<b><%= L10n.Term("Import.LBL_NOTES") %></b>
					<ul>
						<li><%= L10n.Term("Import.LBL_OPPORTUNITIES_NOTE_1") %></li>
					</ul>
				</td>
			</tr>
		</table>
	</div>
	<div id="divImportStep5" style="DISPLAY:none">
		<asp:Label ID="lblStatus"       Font-Bold="true" runat="server" /><br />
		<asp:Label ID="lblSuccessCount" runat="server" /><br />
		<asp:Label ID="lblFailedCount"  runat="server" /><br />
		<br />
		<%= L10n.Term("Import.LBL_USE_TRANSACTION") %><asp:CheckBox ID="chkUseTransaction" Checked="True" CssClass="checkbox" runat="server" />
		<br />
		<SplendidCRM:ListHeader ID="ctlListHeader" Visible="<%# !PrintView %>" Title="Import.LBL_LAST_IMPORTED" Runat="Server" />
		<SplendidCRM:SplendidGrid id="grdMain" Width="100%" CssClass="listView"
			CellPadding="3" CellSpacing="0" border="0"
			AllowPaging="<%# !PrintView %>" PageSize="20" AllowSorting="true" 
			AutoGenerateColumns="false" 
			EnableViewState="true" runat="server">
			<ItemStyle            CssClass="oddListRowS1"  />
			<AlternatingItemStyle CssClass="evenListRowS1" />
			<HeaderStyle          CssClass="listViewThS1"  />
			<PagerStyle HorizontalAlign="Right" Mode="NextPrev" PageButtonCount="6" Position="Top" CssClass="listViewPaginationTdS1" PrevPageText=".LNK_LIST_PREVIOUS" NextPageText=".LNK_LIST_NEXT" />
		</SplendidCRM:SplendidGrid>
	</div>
	<br>
	<script type="text/javascript">
		SelectWizardTab(<%= txtACTIVE_TAB.Value %>);
		SelectSourceFormat();
	</script>
<%
if ( bDebug )
{
	Response.Write("<div style=\"width: 1200; border: 1px solid black; overflow: hidden; \">");
	XmlUtil.Dump(xmlMapping);
	Response.Write("</div>");
	Response.Write("<br />");

	Response.Write("<div style=\"width: 1200; border: 1px solid black; overflow: hidden; \">");
	XmlUtil.Dump(xml);
	Response.Write("</div>");
	Response.Write("<br />");

	Response.Write("<div style=\"width: 1200; border: 1px solid black; overflow: hidden; \">");
	Response.Write("<pre>");
	Response.Write(sbImport.ToString());
	Response.Write("</pre>");
	Response.Write("</div>");
}
%>
</div>
