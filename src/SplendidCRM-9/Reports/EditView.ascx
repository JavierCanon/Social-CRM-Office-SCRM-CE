<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditView.ascx.cs" Inherits="SplendidCRM.Reports.EditView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="SplendidCRM" Tagname="DatePicker" Src="~/_controls/DatePicker.ascx" %>
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
function ChangeUser(sPARENT_ID, sPARENT_NAME)
{
	document.getElementById('<%= txtASSIGNED_USER_ID.ClientID %>').value = sPARENT_ID  ;
	document.getElementById('<%= txtASSIGNED_TO.ClientID      %>').value = sPARENT_NAME;
	document.forms[0].submit();
}
function UserPopup()
{
	return window.open('../Users/Popup.aspx?ClearDisabled=1','UserPopup','width=600,height=400,resizable=1,scrollbars=1');
}
</script>
<div id="divEditView">
	<%@ Register TagPrefix="SplendidCRM" Tagname="ModuleHeader" Src="~/_controls/ModuleHeader.ascx" %>
	<SplendidCRM:ModuleHeader ID="ctlModuleHeader" Module="Reports" EnablePrint="false" HelpName="EditView" EnableHelp="true" Runat="Server" />
	<p>
	<%@ Register TagPrefix="SplendidCRM" Tagname="ReportButtons" Src="_controls/ReportButtons.ascx" %>
	<SplendidCRM:ReportButtons ID="ctlReportButtons" Visible="<%# !PrintView %>" Runat="Server" />
	</p>

<asp:UpdatePanel ID="ctlEditViewPanel" runat="server">
	<ContentTemplate>
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
			document.getElementById('liReportWizard'   + i).className     = sListClass;
			document.getElementById('linkReportWizard' + i).className     = sLinkClass;
			document.getElementById('divReportWizard'  + i).style.display = sListStyle;
		}
		document.getElementById('<%= txtACTIVE_TAB.ClientID %>').value = key;
		//document.getElementById('divReportWizard2').style.display = 'block';
		//document.getElementById('divReportWizard4').style.display = 'block';
	}
	</script>

	<input id="txtACTIVE_TAB" type="hidden" runat="server" />
	<ul CssClass="tablist">
		<li id="liReportWizard1" CssClass="<%= txtACTIVE_TAB.Value == "1" ? "active" : "" %>"><a id="linkReportWizard1" href="javascript:SelectWizardTab(1);" CssClass="<%= txtACTIVE_TAB.Value == "1" ? "current" : "" %>"><%= L10n.Term("Reports.LBL_WIZARD_STEP1") %></a></li>
		<li id="liReportWizard2" CssClass="<%= txtACTIVE_TAB.Value == "2" ? "active" : "" %>"><a id="linkReportWizard2" href="javascript:SelectWizardTab(2);" CssClass="<%= txtACTIVE_TAB.Value == "2" ? "current" : "" %>"><%= L10n.Term("Reports.LBL_WIZARD_STEP2") %></a></li>
		<li id="liReportWizard3" CssClass="<%= txtACTIVE_TAB.Value == "3" ? "active" : "" %>"><a id="linkReportWizard3" href="javascript:SelectWizardTab(3);" CssClass="<%= txtACTIVE_TAB.Value == "3" ? "current" : "" %>"><%= L10n.Term("Reports.LBL_WIZARD_STEP3") %></a></li>
		<li id="liReportWizard4" CssClass="<%= txtACTIVE_TAB.Value == "4" ? "active" : "" %>"><a id="linkReportWizard4" href="javascript:SelectWizardTab(4);" CssClass="<%= txtACTIVE_TAB.Value == "4" ? "current" : "" %>"><%= L10n.Term("Reports.LBL_WIZARD_STEP4") %></a></li>
		<li id="liReportWizard5" CssClass="" style="DISPLAY: none"                           ><a id="linkReportWizard5" href="javascript:SelectWizardTab(5);" CssClass="" style="DISPLAY: none"><%= L10n.Term("Reports.LBL_WIZARD_STEP5") %></a></li>
	</ul>
	<div id="divReportWizard1" style="DISPLAY:<%= txtACTIVE_TAB.Value == "1" ? "block" : "none" %>">
		<asp:Table SkinID="tabForm" runat="server">
			<asp:TableRow>
				<asp:TableCell>
					<asp:Table BorderWidth="0" CellSpacing="0" CellPadding="0" runat="server">
						<asp:TableRow>
							<asp:TableCell Width="15%" CssClass="dataLabel"><h4><asp:Label Text='<%# L10n.Term("Reports.LBL_MODULE_NAME") %>' runat="server" /></h4></asp:TableCell>
							<asp:TableCell Width="35%" CssClass="dataField">
								<asp:DropDownList ID="lstMODULE" TabIndex="1" DataValueField="MODULE_NAME" DataTextField="DISPLAY_NAME" OnSelectedIndexChanged="lstMODULE_Changed" AutoPostBack="true" Runat="server" />
								<asp:Label ID="lblMODULE" runat="server" />
							</asp:TableCell>
							<asp:TableCell Width="15%" CssClass="dataLabel"><h4><asp:Label Text='<%# L10n.Term("Reports.LBL_REPORT_NAME") %>' runat="server" /></h4></asp:TableCell>
							<asp:TableCell Width="35%" CssClass="dataField">
								<asp:TextBox ID="txtNAME" TabIndex="2" size="35" MaxLength="150" Runat="server" />
								<asp:RequiredFieldValidator ID="reqNAME" ControlToValidate="txtNAME" ErrorMessage='<%# L10n.Term(".ERR_REQUIRED_FIELD") %>' CssClass="required" EnableClientScript="false" EnableViewState="false" Runat="server" />
							</asp:TableCell>
						</asp:TableRow>
						<asp:TableRow>
							<asp:TableCell CssClass="dataLabel"><h4><asp:Label Text='<%# L10n.Term("Reports.LBL_RELATED") %>' runat="server" /></h4></asp:TableCell>
							<asp:TableCell CssClass="dataField">
								<asp:DropDownList ID="lstRELATED" TabIndex="3" DataValueField="MODULE_NAME" DataTextField="DISPLAY_NAME" OnSelectedIndexChanged="lstRELATED_Changed" AutoPostBack="true" Runat="server" /><br />
								<asp:Label ID="lblRELATED" runat="server" />
							</asp:TableCell>
							<asp:TableCell CssClass="dataLabel"><h4><asp:Label Text='<%# L10n.Term("Reports.LBL_ASSIGNED_USER_ID") %>' runat="server" /></h4></asp:TableCell>
							<asp:TableCell CssClass="dataField">
								<asp:TextBox ID="txtASSIGNED_TO" ReadOnly="True" Runat="server" />
								<input ID="txtASSIGNED_USER_ID" type="hidden" runat="server" />
								<input ID="btnChangeUser" type="button" CssClass="button" onclick="return UserPopup();" title="<%# L10n.Term(".LBL_CHANGE_BUTTON_TITLE") %>" AccessKey="<%# L10n.AccessKey(".LBL_CHANGE_BUTTON_KEY") %>" value="<%# L10n.Term(".LBL_CHANGE_BUTTON_LABEL") %>" />
							</asp:TableCell>
						</asp:TableRow>
						<asp:TableRow>
							<asp:TableCell ColumnSpan="2">
								<div style="DISPLAY: none">
									<asp:RadioButton ID="radREPORT_TYPE_TABULAR"   GroupName="radREPORT_TYPE" Text='<%# L10n.Term(".dom_report_types.tabular"         ) %>' CssClass="checkbox" AutoPostBack="true" runat="server" /><br />
									<asp:RadioButton ID="radREPORT_TYPE_SUMMATION" GroupName="radREPORT_TYPE" Text='<%# L10n.Term(".dom_report_types.summary"         ) %>' CssClass="checkbox" AutoPostBack="true" runat="server" /><br />
									<asp:RadioButton ID="radREPORT_TYPE_DETAILED"  GroupName="radREPORT_TYPE" Text='<%# L10n.Term(".dom_report_types.detailed_summary") %>' CssClass="checkbox" AutoPostBack="true" Visible="false" runat="server" />
								</div>
							</asp:TableCell>
							<asp:TableCell CssClass="dataLabel">
								<h4 CssClass="dataLabel">
									<%= L10n.Term("Reports.LBL_SHOW_QUERY") %>
								</h4>
							</asp:TableCell>
							<asp:TableCell CssClass="dataField"><asp:CheckBox ID="chkSHOW_QUERY" CssClass="checkbox" runat="server" /></asp:TableCell>
						</asp:TableRow>
					</asp:Table>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</div>
	<div id="divReportWizard2" style="DISPLAY:<%= txtACTIVE_TAB.Value == "2" ? "block" : "none" %>">
		<asp:Table BorderWidth="0" CellPadding="0" CellSpacing="0" CssClass="tabForm" runat="server">
			<asp:TableRow>
				<asp:TableCell>
					<h4 CssClass="dataLabel"><%= L10n.Term("Reports.LBL_FILTERS") %></h4>
					<asp:Button ID="btnAddFilter" CommandName="Filters.Add" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term("Reports.LBL_ADD_FILTER_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term("Reports.LBL_ADD_FILTER_BUTTON_LABEL") %>' Runat="server" />
				</asp:TableCell>
			</asp:TableRow>
			<asp:TableRow>
				<asp:TableCell>
					<asp:DataGrid ID="dgFilters" AutoGenerateColumns="false"
						CellPadding="3" CellSpacing="0" 
						AllowPaging="false" AllowSorting="false" ShowHeader="true"
						EnableViewState="true" runat="server">
						<Columns>
							<asp:BoundColumn HeaderText="Module"   DataField="MODULE_NAME" />
							<asp:BoundColumn HeaderText="Field"    DataField="DATA_FIELD"  />
							<asp:BoundColumn HeaderText="Type"     DataField="DATA_TYPE"   />
							<asp:BoundColumn HeaderText="Operator" DataField="OPERATOR"    />
							<asp:BoundColumn HeaderText="Search"   DataField="SEARCH_TEXT" />
							<asp:TemplateColumn HeaderText="" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Left" ItemStyle-Wrap="false">
								<ItemTemplate>
									<asp:Button ID="btnEditFilter" CommandName="Filters.Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_EDIT_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_EDIT_BUTTON_TITLE") %>' Runat="server" />
									&nbsp;
									<asp:Button ID="btnDeleteFilter" CommandName="Filters.Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term("Reports.LBL_REMOVE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term("Reports.LBL_REMOVE_BUTTON_TITLE") %>' Runat="server" />
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>
				</asp:TableCell>
			</asp:TableRow>
			<asp:TableRow>
				<asp:TableCell>
					<input id="txtFILTER_ID" type="hidden" runat="server" />
					<asp:Table SkinID="tabEditView" runat="server">
						<asp:TableRow>
							<asp:TableCell VerticalAlign="top">
								<asp:DropDownList ID="lstFILTER_COLUMN_SOURCE" TabIndex="10" DataValueField="MODULE_NAME" DataTextField="DISPLAY_NAME" OnSelectedIndexChanged="lstFILTER_COLUMN_SOURCE_Changed" AutoPostBack="true" Runat="server" /><br />
								<asp:Label ID="lblFILTER_COLUMN_SOURCE" runat="server" />
							</asp:TableCell>
							<asp:TableCell VerticalAlign="top">
								<asp:DropDownList ID="lstFILTER_COLUMN" TabIndex="11" DataValueField="NAME" DataTextField="DISPLAY_NAME" OnSelectedIndexChanged="lstFILTER_COLUMN_Changed" AutoPostBack="true" Runat="server" /><br />
								<asp:Label ID="lblFILTER_COLUMN" runat="server" />
							</asp:TableCell>
							<asp:TableCell VerticalAlign="top">
								<asp:DropDownList ID="lstFILTER_OPERATOR" TabIndex="12" DataValueField="NAME" DataTextField="DISPLAY_NAME" OnSelectedIndexChanged="lstFILTER_OPERATOR_Changed" AutoPostBack="true" Runat="server" /><br />
								<asp:Label ID="lblFILTER_OPERATOR_TYPE" runat="server" />
								<asp:Label ID="lblFILTER_OPERATOR" runat="server" />
							</asp:TableCell>
							<asp:TableCell VerticalAlign="top" Wrap="false">
								<asp:Table BorderWidth="0" CellSpacing="0" CellPadding="0" runat="server">
									<asp:TableRow>
										<asp:TableCell>
											<input type="hidden" id="txtFILTER_SEARCH_ID" runat="server" />
											<input type="hidden" id="txtFILTER_SEARCH_DATA_TYPE" runat="server" />
											
											<asp:TextBox ID="txtFILTER_SEARCH_TEXT"   runat="server" />
											
											<asp:DropDownList ID="lstFILTER_SEARCH_DROPDOWN" DataValueField="NAME" DataTextField="DISPLAY_NAME" runat="server" />
											<asp:ListBox      ID="lstFILTER_SEARCH_LISTBOX"  DataValueField="NAME" DataTextField="DISPLAY_NAME" SelectionMode="Multiple" runat="server" />
											
											<SplendidCRM:DatePicker ID="ctlFILTER_SEARCH_START_DATE" EnableDateFormat="false" Runat="Server" />
										</asp:TableCell>
										<asp:TableCell>
											<asp:Label ID="lblFILTER_AND_SEPARATOR" Text='<%# L10n.Term("Schedulers.LBL_AND") %>' runat="server" />
										</asp:TableCell>
										<asp:TableCell>
											<SplendidCRM:DatePicker ID="ctlFILTER_SEARCH_END_DATE" EnableDateFormat="false" Runat="Server" />
											
											<asp:TextBox ID="txtFILTER_SEARCH_TEXT2"  runat="server" />
											
											<asp:Button ID="btnFILTER_SEARCH_SELECT" OnClientClick="SearchPopup(); return false;" CssClass="button" Text='<%# L10n.Term(".LBL_SELECT_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_SELECT_BUTTON_TITLE") %>' runat="server" />
										</asp:TableCell>
									</asp:TableRow>
								</asp:Table>
								<asp:Label ID="lblFILTER_ID" runat="server" />
							</asp:TableCell>
							<asp:TableCell VerticalAlign="top">
								<asp:Button CommandName="Filters.Update" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_UPDATE_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_UPDATE_BUTTON_TITLE") %>' Runat="server" />
							</asp:TableCell>
							<asp:TableCell VerticalAlign="top">
								<asp:Button CommandName="Filters.Cancel" OnCommand="Page_Command" CssClass="button" Text='<%# L10n.Term(".LBL_CANCEL_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_CANCEL_BUTTON_TITLE") %>' Runat="server" />
							</asp:TableCell>
							<asp:TableCell Width="80%"></asp:TableCell>
						</asp:TableRow>
					</asp:Table>
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</div>
	<div id="divReportWizard3" style="DISPLAY:<%= txtACTIVE_TAB.Value == "3" ? "block" : "none" %>">
		<asp:Table SkinID="tabForm" runat="server">
			<asp:TableRow>
				<asp:TableCell>
					Grouping is not supported at this time. 
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</div>
	<div id="divReportWizard4" style="DISPLAY:<%= txtACTIVE_TAB.Value == "4" ? "block" : "none" %>">
		<asp:Table SkinID="tabForm" runat="server">
			<asp:TableRow>
				<asp:TableCell>
					<asp:Table runat="server" border="0" cellspacing="0" cellpadding="0">
						<asp:TableRow>
							<asp:TableCell CssClass="dataLabel"><h4><asp:Label Text='<%# L10n.Term("Reports.LBL_MODULE_NAME") %>' runat="server" /></h4></asp:TableCell>
							<asp:TableCell CssClass="dataField">
								<asp:DropDownList ID="lstMODULE_COLUMN_SOURCE" TabIndex="1" DataValueField="MODULE_NAME" DataTextField="DISPLAY_NAME" OnSelectedIndexChanged="lstMODULE_COLUMN_SOURCE_Changed" AutoPostBack="true" Runat="server" /><br />
								<asp:Label ID="lblMODULE_COLUMN_SOURCE" runat="server" />
							</asp:TableCell>
						</asp:TableRow>
					</asp:Table>
					<asp:Label ID="lblDisplayColumnsRequired" CssClass="required" Text='<%# L10n.Term("Reports.LBL_DISPLAY_COLUMNS_REQUIRED") %>' EnableViewState="false" runat="server" />
					<%@ Register TagPrefix="SplendidCRM" Tagname="Chooser" Src="~/_controls/Chooser.ascx" %>
					<SplendidCRM:Chooser ID="ctlDisplayColumnsChooser" ChooserTitle="Reports.LBL_CHOOSE_COLUMNS" LeftTitle="Reports.LBL_DISPLAY_COLUMNS" RightTitle="Reports.LBL_AVAILABLE_COLUMNS" Enabled="true" Runat="Server" />
				</asp:TableCell>
			</asp:TableRow>
		</asp:Table>
	</div>
	<div id="divReportWizard5" style="DISPLAY:<%= txtACTIVE_TAB.Value == "5" ? "block" : "none" %>">
	</div>
	<script type="text/javascript">
		//SelectWizardTab(<%= txtACTIVE_TAB.Value %>);
	</script>
	
	<asp:Literal ID="litREPORT_QUERY" runat="server" />

	<%@ Register TagPrefix="SplendidCRM" Tagname="ReportView" Src="ReportView.ascx" %>
	<SplendidCRM:ReportView ID="ctlReportView" Runat="Server" />
	
	<%@ Register TagPrefix="SplendidCRM" Tagname="DumpSQL" Src="~/_controls/DumpSQL.ascx" %>
	<SplendidCRM:DumpSQL ID="ctlDumpSQL" Visible="<%# !PrintView %>" Runat="Server" />
	</ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
function MoveLeftToRight(sLeftID, sRightID, bReverse)
{
	var lstLeft  = document.getElementById(sLeftID );
	var lstRight = document.getElementById(sRightID);
	var lstModule = document.getElementById('<%= lstMODULE_COLUMN_SOURCE.ClientID %>');
	var sModuleName = lstModule.options[lstModule.selectedIndex].text;
	for ( i=0; i < lstLeft.options.length ; i++ )
	{
		if ( lstLeft.options[i].selected == true )
		{
			var oOption = document.createElement("OPTION");
			if ( bReverse == 1 )
				oOption.text  = sModuleName + ': ' + lstLeft.options[i].text;
			else if ( lstLeft.options[i].text.indexOf(': ') >= 0 )
				oOption.text  = lstLeft.options[i].text.substring(lstLeft.options[i].text.indexOf(': ')+2);
			else
				oOption.text  = lstLeft.options[i].text;
			oOption.value = lstLeft.options[i].value;
			lstRight.options.add(oOption);
		}
	}
	for ( i=lstLeft.options.length-1; i >= 0  ; i-- )
	{
		if ( lstLeft.options[i].selected == true )
		{
			// 10/11/2006 Paul.  Firefox does not support options.remove(), so just set the option to null. 
			lstLeft.options[i] = null;
		}
	}
	// 08/05/2005 Paul. Don't use the sLeftID & sRightID values as they can be reversed. 
	CopyToHidden('<%= ctlDisplayColumnsChooser.lstLeft.ClientID  %>', '<%= ctlDisplayColumnsChooser.txtLeft.ClientID  %>');
	CopyToHidden('<%= ctlDisplayColumnsChooser.lstRight.ClientID %>', '<%= ctlDisplayColumnsChooser.txtRight.ClientID %>');
}
</script>
<%
// 01/19/2007 Paul.  Response.Write does not work in an AJAX UpdatePanel. 
// 08/13/2007 Paul.  Comment out instead of using false to avoid unreachable warning. 
/*
if ( bDebug )
{
	Response.Write("<div style=\"width: 1200; border: 1px solid black; overflow: hidden; \">");
	XmlUtil.Dump(rdl);
	Response.Write("</div>");
}
*/
%>
</div>


