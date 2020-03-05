<%@ Control Language="c#" AutoEventWireup="false" Codebehind="EditLineItemsView.ascx.cs" Inherits="SplendidCRM._controls.EditLineItemsView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
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
var num_grp_sep = '<%= Sql.IsEmptyString(Session["USER_SETTINGS/GROUP_SEPARATOR"  ]) ? "," : Sql.ToString(Session["USER_SETTINGS/GROUP_SEPARATOR"  ]) %>';
var dec_sep     = '<%= Sql.IsEmptyString(Session["USER_SETTINGS/DECIMAL_SEPARATOR"]) ? "." : Sql.ToString(Session["USER_SETTINGS/DECIMAL_SEPARATOR"]) %>';

var sSelectNameUserContext = '';
function ChangeProductTemplate(sPARENT_ID, sPARENT_NAME)
{
	var fldNAME = document.getElementById(sSelectNameUserContext + 'NAME');
	if ( fldNAME != null )
	{
		fldNAME.value = sPARENT_NAME;
		ItemNameChanged('<%= CURRENCY_ID.ClientID %>', fldNAME);
	}
}
function ProductPopup(fldSELECT_NAME)
{
	// 02/04/2007 Paul.  We need to have an easy way to locate the correct text fields, 
	// so use the current field to determine the label prefix and send that in the userContact field. 
	sSelectNameUserContext = fldSELECT_NAME.id.replace('SELECT_NAME', '');
	var sNAME = '';
	var fldNAME = document.getElementById(sSelectNameUserContext + 'NAME');
	if ( fldNAME != null )
	{
		sNAME = fldNAME.value;
	}
	window.open('../Products/ProductCatalog/Popup.aspx?ClearDisabled=1&NAME=' + escape(sNAME),'ProductPopup','width=600,height=400,resizable=1,scrollbars=1');
	return false;
}
</script>
<div id="divEditLineItemsView">
	<asp:UpdatePanel ID="ctlLineHeaderPanel" runat="server">
		<ContentTemplate>
			<asp:Table SkinID="tabForm" runat="server">
				<asp:TableRow>
					<asp:TableCell>
						<asp:Table SkinID="tabFrame" runat="server">
							<asp:TableRow>
								<asp:TableHeaderCell ColumnSpan="10"><h4><asp:Label Text='<%# L10n.Term(m_sMODULE + ".LBL_LINE_ITEMS_TITLE") %>' runat="server" /></h4></asp:TableHeaderCell>
							</asp:TableRow>
							<asp:TableRow>
								<asp:TableCell>
									<asp:Label ID="LBL_CURRENCY"         Text='<%# L10n.Term(m_sMODULE + ".LBL_CURRENCY"        ) %>' runat="server" />&nbsp;<asp:DropDownList ID="CURRENCY_ID" DataValueField="ID" DataTextField="NAME_SYMBOL" OnSelectedIndexChanged="CURRENCY_ID_Changed" AutoPostBack="true" Runat="server" />
									&nbsp;
									<asp:Label ID="LBL_CONVERSION_RATE"  Text='<%# L10n.Term(m_sMODULE + ".LBL_CONVERSION_RATE" ) %>' runat="server" />&nbsp;<asp:TextBox ID="EXCHANGE_RATE" Size="4" MaxLength="10" runat="server" />&nbsp;
								</asp:TableCell>
								<asp:TableCell><asp:Label ID="LBL_TAXRATE"          Text='<%# L10n.Term(m_sMODULE + ".LBL_TAXRATE"         ) %>' runat="server" />&nbsp;<asp:DropDownList ID="TAXRATE_ID"  DataValueField="ID" DataTextField="NAME" OnSelectedIndexChanged="TAXRATE_ID_Changed" AutoPostBack="true" Runat="server" /></asp:TableCell>
								<asp:TableCell><asp:Label ID="LBL_SHIPPER"          Text='<%# L10n.Term(m_sMODULE + ".LBL_SHIPPER"         ) %>' runat="server" />&nbsp;<asp:DropDownList ID="SHIPPER_ID"  DataValueField="ID" DataTextField="NAME" Runat="server" /></asp:TableCell>
								<asp:TableCell><asp:Label ID="LBL_SHOW_LINE_NUMS"   Text='<%# L10n.Term(m_sMODULE + ".LBL_SHOW_LINE_NUMS"  ) %>' Visible="false" runat="server" />&nbsp;<asp:CheckBox     ID="SHOW_LINE_NUMS"   CssClass="checkbox" Visible="false" Runat="server" /></asp:TableCell>
								<asp:TableCell><asp:Label ID="LBL_CALC_GRAND_TOTAL" Text='<%# L10n.Term(m_sMODULE + ".LBL_CALC_GRAND_TOTAL") %>' Visible="false" runat="server" />&nbsp;<asp:CheckBox     ID="CALC_GRAND_TOTAL" CssClass="checkbox" Visible="false" Runat="server" /></asp:TableCell>
								<asp:TableCell>&nbsp;</asp:TableCell>
							</asp:TableRow>
						</asp:Table>
					</asp:TableCell>
				</asp:TableRow>
			</asp:Table>
		</ContentTemplate>
	</asp:UpdatePanel>
	<asp:UpdatePanel ID="ctlLineItemsPanel" runat="server">
		<ContentTemplate>
			<asp:GridView ID="grdMain" AutoGenerateColumns="false" AllowPaging="false" AllowSorting="false" 
				AutoGenerateEditButton="false" AutoGenerateDeleteButton="false" OnRowCreated="grdMain_RowCreated" OnRowDataBound="grdMain_RowDataBound"
				OnRowEditing="grdMain_RowEditing" OnRowDeleting="grdMain_RowDeleting" OnRowUpdating="grdMain_RowUpdating" OnRowCancelingEdit="grdMain_RowCancelingEdit" 
				Width="100%" runat="server">
				<RowStyle            CssClass="oddListRowS1"  VerticalAlign="Top" />
				<AlternatingRowStyle CssClass="evenListRowS1" VerticalAlign="Top" />
				<HeaderStyle         CssClass="listViewThS1"  />
				<Columns>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_QUANTITY" ItemStyle-Width="10%" HeaderStyle-Wrap="false">
						<ItemTemplate><%# Eval("QUANTITY") %></ItemTemplate>
						<EditItemTemplate>
							<asp:HiddenField ID="ID"                    value='<%# Eval("ID"                 ) %>' runat="server" />
							<asp:HiddenField ID="LINE_ITEM_TYPE"        value='<%# Eval("LINE_ITEM_TYPE"     ) %>' runat="server" />
							<asp:HiddenField ID="PRODUCT_TEMPLATE_ID"   value='<%# Eval("PRODUCT_TEMPLATE_ID") %>' runat="server" />
							<asp:HiddenField ID="PREVIOUS_NAME"         value='<%# Eval("NAME"               ) %>' runat="server" />
							<asp:HiddenField ID="VENDOR_PART_NUM"       value='<%# Eval("VENDOR_PART_NUM"    ) %>' runat="server" />
							<asp:HiddenField ID="PREVIOUS_MFT_PART_NUM" value='<%# Eval("MFT_PART_NUM"       ) %>' runat="server" />
							<asp:HiddenField ID="COST_USDOLLAR"         value='<%# Sql.ToDecimal(Eval("COST_USDOLLAR"    )).ToString("0.000") %>' runat="server" />
							<asp:HiddenField ID="LIST_USDOLLAR"         value='<%# Sql.ToDecimal(Eval("LIST_USDOLLAR"    )).ToString("0.000") %>' runat="server" />
							<asp:HiddenField ID="UNIT_USDOLLAR"         value='<%# Sql.ToDecimal(Eval("UNIT_USDOLLAR"    )).ToString("0.000") %>' runat="server" />
							<asp:HiddenField ID="EXTENDED_USDOLLAR"     value='<%# Sql.ToDecimal(Eval("EXTENDED_USDOLLAR")).ToString("0.000") %>' runat="server" />

							<asp:TextBox ID="QUANTITY" Text='<%# Eval("QUANTITY") %>' Width="50" TabIndex="11" onblur="ItemQuantityChanged(this);" autocomplete="off" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_NAME" ItemStyle-Width="20%" HeaderStyle-Wrap="false">
						<ItemTemplate><%# Eval("NAME") %><br /><%# Eval("DESCRIPTION") %></ItemTemplate>
						<EditItemTemplate>
							<nobr>
							<asp:TextBox ID="NAME" Text='<%# Eval("NAME") %>' TabIndex="12" onblur=<%# "ItemNameChanged('" + CURRENCY_ID.ClientID + "', this);" %> autocomplete="off" runat="server" />
							<asp:Button ID="SELECT_NAME" OnClientClick="return ProductPopup(this);" CssClass="button" Text='<%# L10n.Term(".LBL_SELECT_BUTTON_LABEL") %>' ToolTip='<%# L10n.Term(".LBL_SELECT_BUTTON_TITLE") %>' Runat="server" />
							<br /></nobr>
							<asp:TextBox ID="DESCRIPTION" Text='<%# Eval("DESCRIPTION") %>' TabIndex="22" TextMode="MultiLine" Rows="2" Width="180px" autocomplete="off" runat="server" />
							<ajaxToolkit:AutoCompleteExtender ID="autoNAME" TargetControlID="NAME" ServiceMethod="ItemNameList" ServicePath="~/Products/ProductCatalog/AutoComplete.asmx" MinimumPrefixLength="2" CompletionInterval="250" EnableCaching="true" CompletionSetCount="12" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_MFT_PART_NUM" ItemStyle-Width="20%" HeaderStyle-Wrap="false">
						<ItemTemplate><%# Eval("MFT_PART_NUM") %></ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="MFT_PART_NUM" Text='<%# Eval("MFT_PART_NUM") %>' TabIndex="13" onblur=<%# "ItemPartNumberChanged('" + CURRENCY_ID.ClientID + "', this);" %> autocomplete="off" runat="server" />
							<ajaxToolkit:AutoCompleteExtender ID="autoMFT_PART_NUM" TargetControlID="MFT_PART_NUM" ServiceMethod="ItemNumberList" ServicePath="~/Products/ProductCatalog/AutoComplete.asmx" MinimumPrefixLength="2" CompletionInterval="250" EnableCaching="true" CompletionSetCount="12" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_TAX_CLASS" ItemStyle-Width="10%" HeaderStyle-Wrap="false">
						<ItemTemplate><%# Eval("TAX_CLASS") %></ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList ID="TAX_CLASS" DataValueField="NAME" DataTextField="DISPLAY_NAME" TabIndex="14" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_COST_PRICE" ItemStyle-Width="15%" HeaderStyle-Wrap="false">
						<ItemTemplate><%# Sql.ToDecimal(Eval("COST_PRICE")).ToString("c") %></ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="COST_PRICE" Text='<%# Sql.ToDecimal(Eval("COST_PRICE")).ToString("#,###.##") %>' Width="60" TabIndex="15" autocomplete="off" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_LIST_PRICE" ItemStyle-Width="15%" HeaderStyle-Wrap="false">
						<ItemTemplate><%# Sql.ToDecimal(Eval("LIST_PRICE")).ToString("c") %></ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="LIST_PRICE" Text='<%# Sql.ToDecimal(Eval("LIST_PRICE")).ToString("#,###.##") %>' Width="60" TabIndex="16" autocomplete="off" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_UNIT_PRICE" ItemStyle-Width="15%" HeaderStyle-Wrap="false">
						<ItemTemplate><%# Sql.ToDecimal(Eval("UNIT_PRICE")).ToString("c") %></ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="UNIT_PRICE" Text='<%# Sql.ToDecimal(Eval("UNIT_PRICE")).ToString("#,###.##") %>' Width="60" TabIndex="17" onblur="ItemUnitPriceChanged(this);" autocomplete="off" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText=".LBL_LIST_ITEM_EXTENDED_PRICE" ItemStyle-Width="15%" HeaderStyle-Wrap="false">
						<ItemTemplate><%# Sql.ToDecimal(Eval("EXTENDED_PRICE")).ToString("c") %></ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox ID="EXTENDED_PRICE" Text='<%# Sql.ToDecimal(Eval("EXTENDED_PRICE")).ToString("#,###.##") %>' Width="60" TabIndex="18" ReadOnly="true" autocomplete="off" runat="server" />
						</EditItemTemplate>
					</asp:TemplateField>
					<asp:CommandField ButtonType="Button" ShowEditButton="true" ShowDeleteButton="true" ControlStyle-CssClass="button" EditText=".LBL_EDIT_BUTTON_LABEL" DeleteText=".LBL_DELETE_BUTTON_LABEL" UpdateText=".LBL_UPDATE_BUTTON_LABEL" CancelText=".LBL_CANCEL_BUTTON_LABEL" ItemStyle-Width="10%" />
				</Columns>
			</asp:GridView>
			<asp:Label ID="lblLineItemError" ForeColor="Red" EnableViewState="false" runat="server" />
		</ContentTemplate>
	</asp:UpdatePanel>
	
	<span id="AjaxErrors" style="color:Red"></span>
	
	<asp:UpdatePanel ID="ctlSummaryPanel" runat="server">
		<ContentTemplate>
			<asp:Table SkinID="tabForm" runat="server">
				<asp:TableRow>
					<asp:TableCell>
						<asp:HiddenField ID="DISCOUNT_USDOLLAR" runat="server" />
						<asp:HiddenField ID="SHIPPING_USDOLLAR" runat="server" />
						<asp:Table SkinID="tabFrame" runat="server">
							<asp:TableRow><asp:TableCell Width="65%"></asp:TableCell><asp:TableCell CssClass="dataLabel" Width="15%"><asp:Label ID="LBL_SUBTOTAL" Text='<%# L10n.Term(m_sMODULE + ".LBL_SUBTOTAL") %>' runat="server" /></asp:TableCell><asp:TableCell class="dataField" width="20%"><asp:TextBox ID="SUBTOTAL" ReadOnly="true" BackColor="#dddddd" runat="server" /></asp:TableCell></asp:TableRow>
							<asp:TableRow><asp:TableCell            ></asp:TableCell><asp:TableCell CssClass="dataLabel"            ><asp:Label ID="LBL_DISCOUNT" Text='<%# L10n.Term(m_sMODULE + ".LBL_DISCOUNT") %>' runat="server" /></asp:TableCell><asp:TableCell class="dataField"            ><asp:TextBox ID="DISCOUNT"                                     runat="server" /></asp:TableCell></asp:TableRow>
							<asp:TableRow><asp:TableCell            ></asp:TableCell><asp:TableCell CssClass="dataLabel"            ><asp:Label ID="LBL_SHIPPING" Text='<%# L10n.Term(m_sMODULE + ".LBL_SHIPPING") %>' runat="server" /></asp:TableCell><asp:TableCell class="dataField"            ><asp:TextBox ID="SHIPPING"                                     runat="server" /></asp:TableCell></asp:TableRow>
							<asp:TableRow><asp:TableCell            ></asp:TableCell><asp:TableCell CssClass="dataLabel"            ><asp:Label ID="LBL_TAX"      Text='<%# L10n.Term(m_sMODULE + ".LBL_TAX"     ) %>' runat="server" /></asp:TableCell><asp:TableCell class="dataField"            ><asp:TextBox ID="TAX"      ReadOnly="true" BackColor="#dddddd" runat="server" /></asp:TableCell></asp:TableRow>
							<asp:TableRow><asp:TableCell            ></asp:TableCell><asp:TableCell CssClass="dataLabel"            ><asp:Label ID="LBL_TOTAL"    Text='<%# L10n.Term(m_sMODULE + ".LBL_TOTAL"   ) %>' runat="server" /></asp:TableCell><asp:TableCell class="dataField"            ><asp:TextBox ID="TOTAL"    ReadOnly="true" BackColor="#dddddd" runat="server" /></asp:TableCell></asp:TableRow>
						</asp:Table>
					</asp:TableCell>
				</asp:TableRow>
			</asp:Table>
		</ContentTemplate>
	</asp:UpdatePanel>
</div>
