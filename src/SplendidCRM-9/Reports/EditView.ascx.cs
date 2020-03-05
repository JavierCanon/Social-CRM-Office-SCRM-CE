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
 * Portions created by SplendidCRM Software are Copyright (C) 2005 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
using System;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
using System.Xml;
using System.Text;
using System.Collections;
using System.Threading;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Reports
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected SplendidCRM._controls.ModuleHeader ctlModuleHeader         ;
		protected _controls.ReportButtons            ctlReportButtons        ;
		protected SplendidCRM._controls.Chooser      ctlDisplayColumnsChooser;
		protected ReportView                         ctlReportView           ;

		protected RdlDocument     rdl                = null;
		protected bool            bRun               = false;
		protected Guid            gID                     ;
		protected TextBox         txtNAME                 ;
		protected DropDownList    lstMODULE               ;
		protected DropDownList    lstRELATED              ;
		protected DropDownList    lstMODULE_COLUMN_SOURCE ;
		protected RadioButton     radREPORT_TYPE_TABULAR  ;
		protected RadioButton     radREPORT_TYPE_SUMMATION;
		protected RadioButton     radREPORT_TYPE_DETAILED ;
		protected TextBox         txtASSIGNED_TO          ;
		protected HtmlInputHidden txtASSIGNED_USER_ID     ;
		protected CheckBox        chkSHOW_QUERY           ;

		protected string          sReportSQL              ;
		protected DataGrid        dgFilters               ;
		protected HtmlInputHidden txtFILTER_ID            ;
		protected DropDownList    lstFILTER_COLUMN_SOURCE ;
		protected DropDownList    lstFILTER_COLUMN        ;
		protected DropDownList    lstFILTER_OPERATOR      ;
		protected Label           lblMODULE               ;
		protected Label           lblRELATED              ;
		protected Label           lblMODULE_COLUMN_SOURCE ;
		protected Label           lblFILTER_COLUMN_SOURCE ;
		protected Label           lblFILTER_COLUMN        ;
		protected Label           lblFILTER_OPERATOR_TYPE ;
		protected Label           lblFILTER_OPERATOR      ;
		protected Label           lblFILTER_ID            ;
		
		protected HtmlInputHidden txtFILTER_SEARCH_ID        ;
		protected HtmlInputHidden txtFILTER_SEARCH_DATA_TYPE ;
		protected TextBox         txtFILTER_SEARCH_TEXT      ;
		protected TextBox         txtFILTER_SEARCH_TEXT2     ;
		protected DropDownList    lstFILTER_SEARCH_DROPDOWN  ;
		protected ListBox         lstFILTER_SEARCH_LISTBOX   ;
		protected Button          btnFILTER_SEARCH_SELECT    ;
		protected Label           lblFILTER_AND_SEPARATOR    ;

		protected SplendidCRM._controls.DatePicker ctlFILTER_SEARCH_START_DATE;
		protected SplendidCRM._controls.DatePicker ctlFILTER_SEARCH_END_DATE  ;

		protected RequiredFieldValidator reqNAME;
		protected HtmlInputHidden txtACTIVE_TAB            ;
		protected Label           lblDisplayColumnsRequired;
		protected Literal         litREPORT_QUERY          ;

		private string GetReportType()
		{
			string sREPORT_TYPE = "tabular";
			if ( radREPORT_TYPE_SUMMATION.Checked )
				sREPORT_TYPE = "summary";
			else if ( radREPORT_TYPE_DETAILED.Checked )
				sREPORT_TYPE = "detailed_summary";
			return sREPORT_TYPE;
		}

		private void SetReportType(string sREPORT_TYPE)
		{
			if ( sREPORT_TYPE == "summary" )
			{
				radREPORT_TYPE_TABULAR.Checked = false;
				radREPORT_TYPE_SUMMATION.Checked = true;
				radREPORT_TYPE_DETAILED.Checked = false;
			}
			else if ( sREPORT_TYPE == "detailed_summary" )
			{
				radREPORT_TYPE_TABULAR.Checked = false;
				radREPORT_TYPE_SUMMATION.Checked = false;
				radREPORT_TYPE_DETAILED.Checked = true;
			}
			else
			{
				radREPORT_TYPE_TABULAR.Checked = true;
				radREPORT_TYPE_SUMMATION.Checked = false;
				radREPORT_TYPE_DETAILED.Checked = false;
			}
		}

		protected void ResetSearchText()
		{
			lstFILTER_COLUMN_SOURCE.SelectedIndex = 0;
			lstFILTER_COLUMN_SOURCE_Changed(null, null);
			lstFILTER_COLUMN.SelectedIndex = 0;
			lstFILTER_COLUMN_Changed(null, null);
			lstFILTER_OPERATOR.SelectedIndex = 0;
			lstFILTER_OPERATOR_Changed(null, null);

			txtFILTER_ID               .Value    = String.Empty;
			lblFILTER_ID               .Text     = String.Empty;
			txtFILTER_SEARCH_TEXT      .Text     = String.Empty;
			txtFILTER_SEARCH_TEXT2     .Text     = String.Empty;
			ctlFILTER_SEARCH_START_DATE.DateText = String.Empty;
			ctlFILTER_SEARCH_END_DATE  .DateText = String.Empty;
		}

		#region Page_Command
		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				XmlNodeList nlColumns = rdl.SelectNodesNS("Body/ReportItems/Table/Details/TableRows/TableRow/TableCells/TableCell/ReportItems");
				if ( e.CommandName == "Save" || e.CommandName == "Run" )
				{
					//reqNAME.Enabled = true;
					//reqNAME.Validate();
					if ( !reqNAME.IsValid )
					{
						txtACTIVE_TAB.Value = "1";
					}
					else if ( nlColumns.Count == 0 )
					{
						txtACTIVE_TAB.Value = "4";
						lblDisplayColumnsRequired.Visible = true;
					}
				}
				if ( e.CommandName == "Save" )
				{
					if ( Page.IsValid && nlColumns.Count > 0 )
					{
						try
						{
							string sREPORT_TYPE = GetReportType();
							// 10/22/2007 Paul.  Use the Assigned User ID field when saving the record. 
							SqlProcs.spREPORTS_Update(ref gID, Sql.ToGuid(txtASSIGNED_USER_ID.Value), txtNAME.Text, lstMODULE.SelectedValue, sREPORT_TYPE, rdl.OuterXml);
						}
						catch(Exception ex)
						{
							SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
							ctlReportButtons.ErrorText = ex.Message;
							return;
						}
						Response.Redirect("edit.aspx?ID=" + gID.ToString());
					}
				}
				else if ( e.CommandName == "Run" )
				{
					if ( Page.IsValid && nlColumns.Count > 0 )
					{
						// 07/09/2006 Paul.  The ReportViewer has a bug that prevents it from reloading a previous RDL.
						// The only solution is to create a new ReportViewer object. 
						Session["rdl"] = rdl.OuterXml;
						Response.Redirect("edit.aspx?ID=" + gID.ToString() + "&Run=1");
						//ctlReportView.RunReport(rdl.OuterXml);
					}
				}
				else if ( e.CommandName == "Filters.Cancel" )
				{
					ResetSearchText();
				}
				else if ( e.CommandName == "Filters.Add" )
				{
					ResetSearchText();
				}
				else if ( e.CommandName == "Filters.Delete" )
				{
					FiltersDelete(Sql.ToString(e.CommandArgument));
					ResetSearchText();
				}
				else if ( e.CommandName == "Filters.Edit" )
				{
					string sFILTER_ID = Sql.ToString(e.CommandArgument);
					string sMODULE_NAME  = String.Empty;
					string sDATA_FIELD   = String.Empty;
					string sDATA_TYPE    = String.Empty;
					string sOPERATOR     = String.Empty;
					string sSEARCH_TEXT1 = String.Empty;
					string sSEARCH_TEXT2 = String.Empty;
					string[] arrSEARCH_TEXT = new string[0];
					FiltersGet(sFILTER_ID, ref sMODULE_NAME, ref sDATA_FIELD, ref sDATA_TYPE, ref sOPERATOR, ref arrSEARCH_TEXT );
					txtFILTER_ID               .Value    = sFILTER_ID;
					lblFILTER_ID               .Text     = txtFILTER_ID.Value;
					txtFILTER_SEARCH_DATA_TYPE .Value    = sDATA_TYPE;
					txtFILTER_SEARCH_TEXT      .Text     = String.Empty;
					txtFILTER_SEARCH_TEXT2     .Text     = String.Empty;
					ctlFILTER_SEARCH_START_DATE.DateText = String.Empty;
					ctlFILTER_SEARCH_END_DATE  .DateText = String.Empty;
					
					lstFILTER_COLUMN_SOURCE.SelectedValue = sMODULE_NAME;
					lstFILTER_COLUMN_SOURCE_Changed(null, null);
					
					lstFILTER_COLUMN       .SelectedValue = sDATA_FIELD ;
					lstFILTER_COLUMN_Changed(null, null);
					lstFILTER_OPERATOR     .SelectedValue = sOPERATOR   ;
					lstFILTER_OPERATOR_Changed(null, null);
					
					if ( arrSEARCH_TEXT.Length > 0 )
						sSEARCH_TEXT1 = arrSEARCH_TEXT[0];
					if ( arrSEARCH_TEXT.Length > 1 )
						sSEARCH_TEXT2 = arrSEARCH_TEXT[1];
					
					// 07/06/2007 Paul.  ansistring is treated the same as string. 
					string sCOMMON_DATA_TYPE = sDATA_TYPE;
					if ( sCOMMON_DATA_TYPE == "ansistring" )
						sCOMMON_DATA_TYPE = "string";
					switch ( sCOMMON_DATA_TYPE )
					{
						case "string":
						{
							switch ( sOPERATOR )
							{
								case "equals"        :  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  break;
								case "contains"      :  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  break;
								case "starts_with"   :  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  break;
								case "ends_with"     :  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  break;
								case "not_equals_str":  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  break;
								case "empty"         :  break;
								case "not_empty"     :  break;
							}
							break;
						}
						case "datetime":
						{
							if ( arrSEARCH_TEXT.Length > 0 )
							{
								DateTime dtSEARCH_TEXT1 = DateTime.ParseExact(sSEARCH_TEXT1, "yyyy/MM/dd", Thread.CurrentThread.CurrentCulture.DateTimeFormat);
								DateTime dtSEARCH_TEXT2 = DateTime.MinValue;
								if ( arrSEARCH_TEXT.Length > 1 )
									dtSEARCH_TEXT2 = DateTime.ParseExact(sSEARCH_TEXT2, "yyyy/MM/dd", Thread.CurrentThread.CurrentCulture.DateTimeFormat);
								switch ( sOPERATOR )
								{
									case "on"             :  ctlFILTER_SEARCH_START_DATE.DateText = dtSEARCH_TEXT1.ToShortDateString();  break;
									case "before"         :  ctlFILTER_SEARCH_START_DATE.DateText = dtSEARCH_TEXT1.ToShortDateString();  break;
									case "after"          :  ctlFILTER_SEARCH_START_DATE.DateText = dtSEARCH_TEXT1.ToShortDateString();  break;
									case "not_equals_str" :  ctlFILTER_SEARCH_START_DATE.DateText = dtSEARCH_TEXT1.ToShortDateString();  break;
									case "between_dates"  :
										ctlFILTER_SEARCH_START_DATE.DateText = dtSEARCH_TEXT1.ToShortDateString();
										if ( arrSEARCH_TEXT.Length > 1 )
											ctlFILTER_SEARCH_END_DATE  .DateText = dtSEARCH_TEXT2.ToShortDateString();
										break;
									case "empty"          :  break;
									case "not_empty"      :  break;
									case "tp_yesterday"   :  break;
									case "tp_today"       :  break;
									case "tp_tomorrow"    :  break;
									case "tp_last_7_days" :  break;
									case "tp_next_7_days" :  break;
									case "tp_last_month"  :  break;
									case "tp_this_month"  :  break;
									case "tp_next_month"  :  break;
									case "tp_last_30_days":  break;
									case "tp_next_30_days":  break;
									case "tp_last_year"   :  break;
									case "tp_this_year"   :  break;
									case "tp_next_year"   :  break;
								}
							}
							break;
						}
						case "int32":
						{
							switch ( sOPERATOR )
							{
								case "equals"    :  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  break;
								case "less"      :  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  break;
								case "greater"   :  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  break;
								case "between"   :  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  txtFILTER_SEARCH_TEXT2.Text = sSEARCH_TEXT2;  break;
								case "not_equals":  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  break;
								case "empty"     :  break;
								case "not_empty" :  break;
							}
							break;
						}
						case "decimal":
						{
							switch ( sOPERATOR )
							{
								case "equals"    :  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  break;
								case "less"      :  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  break;
								case "greater"   :  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  break;
								case "between"   :  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  txtFILTER_SEARCH_TEXT2.Text = sSEARCH_TEXT2;  break;
								case "not_equals":  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  break;
								case "empty"     :  break;
								case "not_empty" :  break;
							}
							break;
						}
						case "float":
						{
							switch ( sOPERATOR )
							{
								case "equals"    :  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  break;
								case "less"      :  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  break;
								case "greater"   :  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  break;
								case "between"   :  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  txtFILTER_SEARCH_TEXT2.Text = sSEARCH_TEXT2;  break;
								case "not_equals":  txtFILTER_SEARCH_TEXT.Text = sSEARCH_TEXT1;  break;
								case "empty"     :  break;
								case "not_empty" :  break;
							}
							break;
						}
						case "bool":
						{
							switch ( sOPERATOR )
							{
								case "equals"    :
									try
									{
										// 12/20/2006 Paul.  Catch and ignore the exception. 
										lstFILTER_SEARCH_DROPDOWN.SelectedValue = sSEARCH_TEXT1;
									}
									catch
									{
									}
									break;
								case "empty"     :  break;
								case "not_empty" :  break;
							}
							break;
						}
						case "guid":
						{
							switch ( sOPERATOR )
							{
								case "is"            :  txtFILTER_SEARCH_ID  .Value = sSEARCH_TEXT1;  break;
								case "equals"        :  txtFILTER_SEARCH_TEXT.Text  = sSEARCH_TEXT1;  break;
								case "contains"      :  txtFILTER_SEARCH_TEXT.Text  = sSEARCH_TEXT1;  break;
								case "starts_with"   :  txtFILTER_SEARCH_TEXT.Text  = sSEARCH_TEXT1;  break;
								case "ends_with"     :  txtFILTER_SEARCH_TEXT.Text  = sSEARCH_TEXT1;  break;
								case "not_equals_str":  txtFILTER_SEARCH_TEXT.Text  = sSEARCH_TEXT1;  break;
								case "empty"         :  break;
								case "not_empty"     :  break;
							}
							break;
						}
						case "enum":
						{
							switch ( sOPERATOR )
							{
								case "is"            :
									try
									{
										// 12/20/2006 Paul.  Catch and ignore the exception. 
										lstFILTER_SEARCH_DROPDOWN.SelectedValue = sSEARCH_TEXT1;
									}
									catch
									{
									}
									break;
								case "one_of":
								{
									foreach ( string s in arrSEARCH_TEXT )
									{
										for ( int i = 0; i < lstFILTER_SEARCH_LISTBOX.Items.Count; i++ )
										{
											if ( s == lstFILTER_SEARCH_LISTBOX.Items[i].Value )
												lstFILTER_SEARCH_LISTBOX.Items[i].Selected = true;
										}
									}
									break;
								}
								case "empty"         :  break;
								case "not_empty"     :  break;
							}
							break;
						}
					}
				}
				else if ( e.CommandName == "Filters.Update" )
				{
					string sFILTER_ID    = txtFILTER_ID.Value;
					string sMODULE_NAME  = lstFILTER_COLUMN_SOURCE.SelectedValue;
					string sDATA_FIELD   = lstFILTER_COLUMN       .SelectedValue;
					string sDATA_TYPE    = txtFILTER_SEARCH_DATA_TYPE.Value;
					string sOPERATOR     = lstFILTER_OPERATOR     .SelectedValue;
					
					string[] arrSEARCH_TEXT = new string[0];
					// 07/06/2007 Paul.  ansistring is treated the same as string. 
					string sCOMMON_DATA_TYPE = sDATA_TYPE;
					if ( sCOMMON_DATA_TYPE == "ansistring" )
						sCOMMON_DATA_TYPE = "string";
					switch ( sCOMMON_DATA_TYPE )
					{
						case "string":
						{
							switch ( sOPERATOR )
							{
								case "equals"        :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "contains"      :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "starts_with"   :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "ends_with"     :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "not_equals_str":  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "empty"         :  break;
								case "not_empty"     :  break;
							}
							break;
						}
						case "datetime":
						{
							switch ( sOPERATOR )
							{
								case "on"             :  arrSEARCH_TEXT = new string[] { Sql.ToDateTime(ctlFILTER_SEARCH_START_DATE.DateText).ToString("yyyy/MM/dd") };  break;
								case "before"         :  arrSEARCH_TEXT = new string[] { Sql.ToDateTime(ctlFILTER_SEARCH_START_DATE.DateText).ToString("yyyy/MM/dd") };  break;
								case "after"          :  arrSEARCH_TEXT = new string[] { Sql.ToDateTime(ctlFILTER_SEARCH_START_DATE.DateText).ToString("yyyy/MM/dd") };  break;
								case "not_equals_str" :  arrSEARCH_TEXT = new string[] { Sql.ToDateTime(ctlFILTER_SEARCH_START_DATE.DateText).ToString("yyyy/MM/dd") };  break;
								case "between_dates"  :  arrSEARCH_TEXT = new string[] { Sql.ToDateTime(ctlFILTER_SEARCH_START_DATE.DateText).ToString("yyyy/MM/dd"), Sql.ToDateTime(ctlFILTER_SEARCH_END_DATE.DateText).ToString("yyyy/MM/dd") };  break;
								case "empty"          :  break;
								case "not_empty"      :  break;
								case "tp_yesterday"   :  break;
								case "tp_today"       :  break;
								case "tp_tomorrow"    :  break;
								case "tp_last_7_days" :  break;
								case "tp_next_7_days" :  break;
								case "tp_last_month"  :  break;
								case "tp_this_month"  :  break;
								case "tp_next_month"  :  break;
								case "tp_last_30_days":  break;
								case "tp_next_30_days":  break;
								case "tp_last_year"   :  break;
								case "tp_this_year"   :  break;
								case "tp_next_year"   :  break;
							}
							break;
						}
						case "int32":
						{
							switch ( sOPERATOR )
							{
								case "equals"    :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "less"      :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "greater"   :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "between"   :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text, txtFILTER_SEARCH_TEXT2.Text };  break;
								case "not_equals":  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "empty"     :  break;
								case "not_empty" :  break;
							}
							break;
						}
						case "decimal":
						{
							switch ( sOPERATOR )
							{
								case "equals"    :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "less"      :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "greater"   :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "between"   :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text, txtFILTER_SEARCH_TEXT2.Text };  break;
								case "not_equals":  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "empty"     :  break;
								case "not_empty" :  break;
							}
							break;
						}
						case "float":
						{
							switch ( sOPERATOR )
							{
								case "equals"    :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "less"      :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "greater"   :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "between"   :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text, txtFILTER_SEARCH_TEXT2.Text };  break;
								case "not_equals":  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "empty"     :  break;
								case "not_empty" :  break;
							}
							break;
						}
						case "bool":
						{
							switch ( sOPERATOR )
							{
								case "equals"    :  arrSEARCH_TEXT = new string[] { lstFILTER_SEARCH_DROPDOWN.SelectedValue };  break;
								case "empty"     :  break;
								case "not_empty" :  break;
							}
							break;
						}
						case "guid":
						{
							switch ( sOPERATOR )
							{
								case "is"            :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_ID.Value  };  break;
								case "equals"        :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "contains"      :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "starts_with"   :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "ends_with"     :  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "not_equals_str":  arrSEARCH_TEXT = new string[] { txtFILTER_SEARCH_TEXT.Text };  break;
								case "empty"         :  break;
								case "not_empty"     :  break;
							}
							break;
						}
						case "enum":
						{
							switch ( sOPERATOR )
							{
								case "is"            :  arrSEARCH_TEXT = new string[] { lstFILTER_SEARCH_DROPDOWN.SelectedValue };  break;
								case "one_of"        :  arrSEARCH_TEXT = Sql.ToStringArray(lstFILTER_SEARCH_LISTBOX);  break;
								case "empty"         :  break;
								case "not_empty"     :  break;
							}
							break;
						}
					}
					FiltersUpdate(sFILTER_ID, sMODULE_NAME, sDATA_FIELD, sDATA_TYPE, sOPERATOR, arrSEARCH_TEXT );
					BuildReportSQL();
					ResetSearchText();
				}
				else if ( e.CommandName == "Cancel" )
				{
					Response.Redirect("default.aspx");
				}
			}
			catch(Exception ex)
			{
				ctlReportButtons.ErrorText = ex.Message;
			}
		}
		#endregion

		#region Changed
		protected void lstMODULE_Changed(Object sender, EventArgs e)
		{
			lblMODULE.Text = lstMODULE.SelectedValue;
			// 05/26/2006 Paul.  If the module changes, then throw away everything. 
			// The display columns don't count, the group columns don't count, etc. 
			rdl = new RdlDocument(txtNAME.Text, txtASSIGNED_TO.Text);
			rdl.SetCustomProperty("Module"        , lstMODULE.SelectedValue );
			rdl.SetCustomProperty("Related"       , String.Empty);
			rdl.SetCustomProperty("RelatedModules", String.Empty);
			rdl.SetCustomProperty("Relationships" , String.Empty);
			rdl.SetCustomProperty("Filters"       , String.Empty);
			lstRELATED_Bind();
			lblRELATED.Text = lstRELATED.SelectedValue;
			dgFilters.DataSource = ReportFilters();
			dgFilters.DataBind();
			BuildReportSQL();
			// 07/13/2006 Paul.  The DisplayColumns List must be bound after the ReportSQL is built. 
			lstLeftListBox_Bind();
			ctlReportView.ClearReport();
		}

		protected void lstRELATED_Changed(Object sender, EventArgs e)
		{
			lblRELATED.Text = lstRELATED.SelectedValue;
			rdl.SetCustomProperty("Related"      , lstRELATED.SelectedValue);
			rdl.SetCustomProperty("Relationships", String.Empty);
			lstFILTER_COLUMN_SOURCE_Bind();
			// 06/13/2006 Paul.  If the related module changes, then make sure to remove any unavailable filters. 
			RemoveInvalidFilters();
			// 07/13/2006 Paul.  Remove invalid display columns as well. 
			RemoveInvalidDisplayColumns();
			BuildReportSQL();
			// 07/13/2006 Paul.  The DisplayColumns List must be bound after the ReportSQL is built. 
			lstLeftListBox_Bind();
		}

		protected void lstMODULE_COLUMN_SOURCE_Changed(Object sender, EventArgs e)
		{
			lblMODULE_COLUMN_SOURCE.Text = lstMODULE_COLUMN_SOURCE.SelectedValue;
			ctlDisplayColumnsChooser_Bind();
		}

		protected void lstFILTER_COLUMN_SOURCE_Changed(Object sender, EventArgs e)
		{
			lblFILTER_COLUMN_SOURCE.Text = lstFILTER_COLUMN_SOURCE.SelectedValue;
			lstFILTER_COLUMN_Bind();
		}

		protected void lstFILTER_COLUMN_Changed(Object sender, EventArgs e)
		{
			lblFILTER_COLUMN.Text = lstFILTER_COLUMN.SelectedValue;
			lblFILTER_OPERATOR_Bind();
		}

		protected void lstFILTER_OPERATOR_Changed(Object sender, EventArgs e)
		{
			lblFILTER_OPERATOR.Text = lstFILTER_OPERATOR.SelectedValue;
			BindSearchText();
		}
		#endregion

		#region Bind
		private void lstRELATED_Bind()
		{
			DataView vwRelationships = new DataView(SplendidCache.ReportingRelationships());
			vwRelationships.RowFilter = "       RELATIONSHIP_TYPE = 'many-to-many' " + ControlChars.CrLf
			                          + "   and LHS_MODULE        = \'" + lstMODULE.SelectedValue + "\'" + ControlChars.CrLf;
			// 06/10/2006 Paul.  Filter by the modules that the user has access to. 
			Sql.AppendParameter(vwRelationships, SplendidCache.ReportingModulesList(), "RHS_MODULE", false);

			XmlDocument xmlRelationships = new XmlDocument();
			xmlRelationships.AppendChild(xmlRelationships.CreateElement("Relationships"));
			
			XmlNode xRelationship = null;
			foreach(DataRowView row in vwRelationships)
			{
				string sRELATIONSHIP_NAME = Sql.ToString(row["RELATIONSHIP_NAME"]);
				string sLHS_MODULE        = Sql.ToString(row["LHS_MODULE"       ]);
				string sLHS_TABLE         = Sql.ToString(row["LHS_TABLE"        ]).ToUpper();
				string sLHS_KEY           = Sql.ToString(row["LHS_KEY"          ]).ToUpper();
				string sRHS_MODULE        = Sql.ToString(row["RHS_MODULE"       ]);
				string sRHS_TABLE         = Sql.ToString(row["RHS_TABLE"        ]).ToUpper();
				string sRHS_KEY           = Sql.ToString(row["RHS_KEY"          ]).ToUpper();
				string sJOIN_TABLE        = Sql.ToString(row["JOIN_TABLE"       ]).ToUpper();
				string sJOIN_KEY_LHS      = Sql.ToString(row["JOIN_KEY_LHS"     ]).ToUpper();
				string sJOIN_KEY_RHS      = Sql.ToString(row["JOIN_KEY_RHS"     ]).ToUpper();
				string sMODULE_NAME       = sRHS_MODULE + " " + sRHS_TABLE;
				string sDISPLAY_NAME      = L10n.Term(".moduleList." + sRHS_MODULE);
				if ( bDebug )
				{
					sDISPLAY_NAME = "[" + sMODULE_NAME + "] " + sDISPLAY_NAME;
				}
				
				xRelationship = xmlRelationships.CreateElement("Relationship");
				xmlRelationships.DocumentElement.AppendChild(xRelationship);
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RELATIONSHIP_NAME", sRELATIONSHIP_NAME);
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "LHS_MODULE"       , sLHS_MODULE       );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "LHS_TABLE"        , sLHS_TABLE        );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "LHS_KEY"          , sLHS_KEY          );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RHS_MODULE"       , sRHS_MODULE       );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RHS_TABLE"        , sRHS_TABLE        );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RHS_KEY"          , sRHS_KEY          );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "JOIN_TABLE"       , sJOIN_TABLE       );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "JOIN_KEY_LHS"     , sJOIN_KEY_LHS     );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "JOIN_KEY_RHS"     , sJOIN_KEY_RHS     );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RELATIONSHIP_TYPE", "many-to-many"    );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "MODULE_NAME"      , sMODULE_NAME      );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "DISPLAY_NAME"     , sDISPLAY_NAME     );
			}
			rdl.SetCustomProperty("RelatedModules", xmlRelationships.OuterXml.Replace("</Relationship>", "</Relationship>" + ControlChars.CrLf));

			DataTable dtModules = XmlUtil.CreateDataTable(xmlRelationships.DocumentElement, "Relationship", new string[] {"MODULE_NAME", "DISPLAY_NAME"});
			DataView vwModules = new DataView(dtModules);
			vwModules.Sort = "DISPLAY_NAME";
			lstRELATED.DataSource = vwModules;
			lstRELATED.DataBind();
			lstRELATED.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
			
			lstFILTER_COLUMN_SOURCE_Bind();
		}

		private void lstFILTER_COLUMN_SOURCE_Bind()
		{
			// 07/13/2006 Paul.  Convert the module name to the correct table name. 
			string sModule = lstMODULE.SelectedValue;
			DataView vwRelationships = new DataView(SplendidCache.ReportingRelationships());
			vwRelationships.RowFilter = "       RELATIONSHIP_TYPE = 'one-to-many' " + ControlChars.CrLf
			                          + "   and RHS_MODULE        = \'" + sModule + "\'" + ControlChars.CrLf;
			// 06/10/2006 Paul.  Filter by the modules that the user has access to. 
			Sql.AppendParameter(vwRelationships, SplendidCache.ReportingModulesList(), "RHS_MODULE", false);
			vwRelationships.Sort = "RHS_KEY";


			XmlDocument xmlRelationships = new XmlDocument();
			xmlRelationships.AppendChild(xmlRelationships.CreateElement("Relationships"));
			
			XmlNode xRelationship = xmlRelationships.CreateElement("Relationship");
			xmlRelationships.DocumentElement.AppendChild(xRelationship);

			string sMODULE_TABLE = Sql.ToString(Application["Modules." + sModule + ".TableName"]);
			XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RELATIONSHIP_NAME", sModule      );
			XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "LHS_MODULE"       , sModule      );
			XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "LHS_TABLE"        , sMODULE_TABLE);
			XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "LHS_KEY"          , String.Empty );
			XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RHS_MODULE"       , String.Empty );
			XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RHS_TABLE"        , String.Empty );
			XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RHS_KEY"          , String.Empty );
			XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RELATIONSHIP_TYPE", String.Empty );
			XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "MODULE_ALIAS"     , sMODULE_TABLE);
			XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "MODULE_NAME"      , sModule + " " + sMODULE_TABLE);
			if ( bDebug )
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "DISPLAY_NAME"     , "[" + sModule + " " + sMODULE_TABLE + "] " + sModule);
			else
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "DISPLAY_NAME"     , sModule);
			
			foreach(DataRowView row in vwRelationships)
			{
				string sRELATIONSHIP_NAME = Sql.ToString(row["RELATIONSHIP_NAME"]);
				string sLHS_MODULE        = Sql.ToString(row["LHS_MODULE"       ]);
				string sLHS_TABLE         = Sql.ToString(row["LHS_TABLE"        ]).ToUpper();
				string sLHS_KEY           = Sql.ToString(row["LHS_KEY"          ]).ToUpper();
				string sRHS_MODULE        = Sql.ToString(row["RHS_MODULE"       ]);
				string sRHS_TABLE         = Sql.ToString(row["RHS_TABLE"        ]).ToUpper();
				string sRHS_KEY           = Sql.ToString(row["RHS_KEY"          ]).ToUpper();
				// 07/13/2006 Paul.  It may seem odd the way we are combining LHS_TABLE and RHS_KEY,  but we do it this way for a reason.  
				// The table alias to get to an Email Assigned User ID will be USERS_ASSIGNED_USER_ID. 
				string sMODULE_NAME       = sLHS_MODULE + " " + sLHS_TABLE + "_" + sRHS_KEY;
				string sDISPLAY_NAME      = sRHS_MODULE;
				
				// 07/09/2007 Paul.  Fixes from Version 1.2 on 04/17/2007 were not included in Version 1.4 tree.
				switch ( sRHS_KEY.ToUpper() )
				{
					// 04/17/2007 Paul.  CREATED_BY was renamed CREATED_BY_ID in all views a long time ago. It is just now being fixed here. 
					case "CREATED_BY_ID":
						sDISPLAY_NAME = L10n.Term(".moduleList." + sRHS_MODULE) + ": " + L10n.Term(".LBL_CREATED_BY_USER");
						break;
					case "MODIFIED_USER_ID":
						sDISPLAY_NAME = L10n.Term(".moduleList." + sRHS_MODULE) + ": " + L10n.Term(".LBL_MODIFIED_BY_USER");
						break;
					case "ASSIGNED_USER_ID":
						sDISPLAY_NAME = L10n.Term(".moduleList." + sRHS_MODULE) + ": " + L10n.Term(".LBL_ASSIGNED_TO_USER");
						break;
					// 04/17/2007 Paul.  PARENT_ID is a special case where we want to know the type of the parent. 
					case "PARENT_ID":
						sDISPLAY_NAME = L10n.Term(".moduleList." + sRHS_MODULE) + ": " + L10n.Term(".moduleList." + sLHS_MODULE) + " " + L10n.Term(".LBL_PARENT_ID");
						break;
					default:
						sDISPLAY_NAME = L10n.Term(".moduleList." + sRHS_MODULE) + ": " + TableColumnName(sRHS_MODULE, sRHS_KEY);
						break;
				}
				if ( bDebug )
				{
					sDISPLAY_NAME = "[" + sMODULE_NAME + "] " + sDISPLAY_NAME;
				}
				
				xRelationship = xmlRelationships.CreateElement("Relationship");
				xmlRelationships.DocumentElement.AppendChild(xRelationship);
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RELATIONSHIP_NAME", sRELATIONSHIP_NAME);
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "LHS_MODULE"       , sLHS_MODULE       );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "LHS_TABLE"        , sLHS_TABLE        );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "LHS_KEY"          , sLHS_KEY          );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RHS_MODULE"       , sRHS_MODULE       );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RHS_TABLE"        , sRHS_TABLE        );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RHS_KEY"          , sRHS_KEY          );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RELATIONSHIP_TYPE", "one-to-many"     );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "MODULE_ALIAS"     , sLHS_TABLE + "_" + sRHS_KEY);  // This is just the alias. 
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "MODULE_NAME"      , sMODULE_NAME      );  // Module name includes the alias. 
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "DISPLAY_NAME"     , sDISPLAY_NAME     );
			}
			if ( !Sql.IsEmptyString(lstRELATED.SelectedValue) )
			{
				xRelationship = xmlRelationships.CreateElement("Relationship");
				xmlRelationships.DocumentElement.AppendChild(xRelationship);
				string sRELATED_MODULE = lstRELATED.SelectedValue.Split(' ')[0];
				string sRELATED_ALIAS  = lstRELATED.SelectedValue.Split(' ')[1];
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RELATIONSHIP_NAME", sRELATED_MODULE);
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "LHS_MODULE"       , sRELATED_MODULE);
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "LHS_TABLE"        , sRELATED_ALIAS );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "LHS_KEY"          , String.Empty      );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RHS_MODULE"       , String.Empty      );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RHS_TABLE"        , String.Empty      );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RHS_KEY"          , String.Empty      );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "RELATIONSHIP_TYPE", "many-to-many"    );
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "MODULE_ALIAS"     , sRELATED_ALIAS);
				XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "MODULE_NAME"      , sRELATED_MODULE + " " + sRELATED_ALIAS);
				if ( bDebug )
					XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "DISPLAY_NAME"     , "[" + sRELATED_MODULE + " " + sRELATED_ALIAS + "] " + sRELATED_MODULE);
				else
					XmlUtil.SetSingleNode(xmlRelationships, xRelationship, "DISPLAY_NAME"     , sRELATED_MODULE);
			}
			rdl.SetCustomProperty("Relationships", xmlRelationships.OuterXml.Replace("</Relationship>", "</Relationship>" + ControlChars.CrLf));

			DataTable dtModuleColumnSource = XmlUtil.CreateDataTable(xmlRelationships.DocumentElement, "Relationship", new string[] {"MODULE_NAME", "DISPLAY_NAME"});
			lstMODULE_COLUMN_SOURCE.DataSource = dtModuleColumnSource;
			lstMODULE_COLUMN_SOURCE.DataBind();
			lblMODULE_COLUMN_SOURCE.Text = lstMODULE_COLUMN_SOURCE.SelectedValue;
			// 05/29/2006 Paul.  Filter column source is always the same as module column source. 
			lstFILTER_COLUMN_SOURCE.DataSource = dtModuleColumnSource;
			lstFILTER_COLUMN_SOURCE.DataBind();
			lblFILTER_COLUMN_SOURCE.Text = lstFILTER_COLUMN_SOURCE.SelectedValue;

			ctlDisplayColumnsChooser_Bind();
			lstFILTER_COLUMN_Bind();
		}

		private string TableColumnName(string sModule, string sDISPLAY_NAME)
		{
			/*
			// 07/04/2006 Paul.  Some columns have global terms. 
			// 06/05/2007 Paul.  Add Team global term. 
			if (  sDISPLAY_NAME == "DATE_ENTERED" 
			   || sDISPLAY_NAME == "DATE_MODIFIED"
			   || sDISPLAY_NAME == "ASSIGNED_TO"  
			   || sDISPLAY_NAME == "CREATED_BY"   
			   || sDISPLAY_NAME == "MODIFIED_BY"  
			   || sDISPLAY_NAME == "TEAM_NAME"    )
			{
				sDISPLAY_NAME = L10n.Term(".LBL_" + sDISPLAY_NAME).Replace(":", "");
			}
			else
			{
				// 07/04/2006 Paul.  Column names are aliased so that we don't have to redefine terms. 
				sDISPLAY_NAME = L10n.AliasedTerm(sModule + ".LBL_" + sDISPLAY_NAME).Replace(":", "");
			}
			return sDISPLAY_NAME;
			*/
			// 12/08/2007 Paul.  Move column naming to a common function so that it can be used by SavedSearch. 
			return Utils.TableColumnName(L10n, sModule, sDISPLAY_NAME);
		}

		private void ctlDisplayColumnsChooser_Bind()
		{
			string[] arrModule = lstMODULE_COLUMN_SOURCE.SelectedValue.Split(' ');
			string sModule     = arrModule[0];
			string sTableAlias = arrModule[1];

			string sMODULE_TABLE = Sql.ToString(Application["Modules." + sModule + ".TableName"]);
			DataTable dtColumns = SplendidCache.ReportingFilterColumns(sMODULE_TABLE).Copy();
			foreach(DataRow row in dtColumns.Rows)
			{
				row["NAME"] = sTableAlias + "." + Sql.ToString(row["NAME"]);
				// 07/04/2006 Paul.  Some columns have global terms. 
				row["DISPLAY_NAME"] = TableColumnName(sModule, Sql.ToString(row["DISPLAY_NAME"]));
			}
			// 06/21/2006 Paul.  Do not sort the columns  We want it to remain sorted by COLID. This should keep the NAME at the top. 
			DataView vwColumns = new DataView(dtColumns);

			// 06/15/2006 Paul.  The list of Display Columns is now stored in a custom field. 
			// This is because the DataSet/Fields tag is used to store all available fields and their data types. 
			StringBuilder sbFieldsList = new StringBuilder();
			
			XmlDocument xmlDisplayColumns = rdl.GetCustomProperty("DisplayColumns");
			XmlNodeList nlFields = xmlDisplayColumns.DocumentElement.SelectNodes("DisplayColumn/Field");
			foreach ( XmlNode xField in nlFields )
			{
				if ( sbFieldsList.Length > 0 )
					sbFieldsList.Append(", ");
				sbFieldsList.Append("'" + xField.InnerText + "'");
			}
			
			string sSelectedFields = sbFieldsList.ToString();
			if ( !Sql.IsEmptyString(sSelectedFields) )
				vwColumns.RowFilter = "NAME not in (" + sSelectedFields + ")";
			
			ListBox lstRight = ctlDisplayColumnsChooser.RightListBox;
			lstRight.DataValueField = "NAME";
			lstRight.DataTextField  = "DISPLAY_NAME";
			lstRight.DataSource     = vwColumns;
			lstRight.DataBind();
		}
		#endregion

		private void lstFILTER_COLUMN_Bind()
		{
			lstFILTER_COLUMN.DataSource = null;
			lstFILTER_COLUMN.DataBind();

			string[] arrModule = lstFILTER_COLUMN_SOURCE.SelectedValue.Split(' ');
			string sModule     = arrModule[0];
			string sTableAlias = arrModule[1];

			string sMODULE_TABLE = Sql.ToString(Application["Modules." + sModule + ".TableName"]);
			DataTable dtColumns = SplendidCache.ReportingFilterColumns(sMODULE_TABLE).Copy();
			foreach(DataRow row in dtColumns.Rows)
			{
				row["NAME"        ] = sTableAlias + "." + Sql.ToString(row["NAME"]);
				// 07/04/2006 Paul.  Some columns have global terms. 
				row["DISPLAY_NAME"] = TableColumnName(sModule, Sql.ToString(row["DISPLAY_NAME"]));
			}
			ViewState["FILTER_COLUMNS"] = dtColumns;
			
			// 06/21/2006 Paul.  Do not sort the columns  We want it to remain sorted by COLID. This should keep the NAME at the top. 
			DataView vwColumns = new DataView(dtColumns);
			lstFILTER_COLUMN.DataSource = vwColumns;
			lstFILTER_COLUMN.DataBind();
			lblFILTER_COLUMN.Text = lstFILTER_COLUMN.SelectedValue;

			lblFILTER_OPERATOR_Bind();
		}

		private void lblFILTER_OPERATOR_Bind()
		{
			lstFILTER_OPERATOR.DataSource = null;
			lstFILTER_OPERATOR.Items.Clear();

			string[] arrModule = lstFILTER_COLUMN_SOURCE.SelectedValue.Split(' ');
			string sModule     = arrModule[0];
			string sTableAlias = arrModule[1];
			
			string[] arrColumn = lstFILTER_COLUMN.SelectedValue.Split('.');
			string sColumnName = arrColumn[0];
			if ( arrColumn.Length > 1 )
				sColumnName = arrColumn[1];
			
			string sMODULE_TABLE = Sql.ToString(Application["Modules." + sModule + ".TableName"]);
			DataView vwColumns = new DataView(SplendidCache.ReportingFilterColumns(sMODULE_TABLE).Copy());
			vwColumns.RowFilter = "ColumnName = '" + sColumnName + "'";
			
			if ( vwColumns.Count > 0 )
			{
				DataRowView row = vwColumns[0];
				string sCsType = Sql.ToString(row["CsType"]);
				lblFILTER_OPERATOR_TYPE.Text = sCsType.ToLower();
				txtFILTER_SEARCH_DATA_TYPE.Value = sCsType.ToLower();
				
				lstFILTER_OPERATOR.DataSource = SplendidCache.List(sCsType.ToLower() + "_operator_dom");
				lstFILTER_OPERATOR.DataBind();
				lblFILTER_OPERATOR.Text = lstFILTER_OPERATOR.SelectedValue;
			}
			BindSearchText();
		}

		private void lstLeftListBox_Bind()
		{
			// This is because it requires the Fields node be fully populated, and that does not occur 
			// until after the ReportSQL is built. 
			ListBox lstLeftListBox = ctlDisplayColumnsChooser.LeftListBox;
			lstLeftListBox.DataTextField  = "text";
			lstLeftListBox.DataValueField = "value";
			lstLeftListBox.DataSource     = rdl.CreateDataTable();
			lstLeftListBox.DataBind();
		}

		private void BindSearchText()
		{
			txtFILTER_SEARCH_TEXT      .Visible = false;
			txtFILTER_SEARCH_TEXT2     .Visible = false;
			lstFILTER_SEARCH_LISTBOX   .Visible = false;
			lstFILTER_SEARCH_DROPDOWN  .Visible = false;
			ctlFILTER_SEARCH_START_DATE.Visible = false;
			ctlFILTER_SEARCH_END_DATE  .Visible = false;
			lblFILTER_AND_SEPARATOR    .Visible = false;
			btnFILTER_SEARCH_SELECT    .Visible = false;
			// 02/09/2007 Paul.  Clear the lookups if not used. 
			if ( txtFILTER_SEARCH_DATA_TYPE.Value != "enum" )
			{
				lstFILTER_SEARCH_DROPDOWN.DataSource = null;
				lstFILTER_SEARCH_LISTBOX .DataSource = null;
				lstFILTER_SEARCH_DROPDOWN.DataBind();
				lstFILTER_SEARCH_LISTBOX .DataBind();
			}
			// 07/06/2007 Paul.  ansistring is treated the same as string. 
			string sCOMMON_DATA_TYPE = txtFILTER_SEARCH_DATA_TYPE.Value;
			if ( sCOMMON_DATA_TYPE == "ansistring" )
				sCOMMON_DATA_TYPE = "string";
			switch ( sCOMMON_DATA_TYPE )
			{
				case "string":
				{
					switch ( lstFILTER_OPERATOR.SelectedValue )
					{
						case "equals"        :  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "contains"      :  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "starts_with"   :  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "ends_with"     :  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "not_equals_str":  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "empty"         :  break;
						case "not_empty"     :  break;
					}
					break;
				}
				case "datetime":
				{
					switch ( lstFILTER_OPERATOR.SelectedValue )
					{
						case "on"             :  ctlFILTER_SEARCH_START_DATE.Visible = true;  break;
						case "before"         :  ctlFILTER_SEARCH_START_DATE.Visible = true;  break;
						case "after"          :  ctlFILTER_SEARCH_START_DATE.Visible = true;  break;
						case "between_dates"  :  ctlFILTER_SEARCH_START_DATE.Visible = true;  lblFILTER_AND_SEPARATOR.Visible = true;  ctlFILTER_SEARCH_END_DATE.Visible = true;  break;
						case "not_equals_str" :  ctlFILTER_SEARCH_START_DATE.Visible = true;  break;
						case "empty"          :  break;
						case "not_empty"      :  break;
						case "tp_yesterday"   :  break;
						case "tp_today"       :  break;
						case "tp_tomorrow"    :  break;
						case "tp_last_7_days" :  break;
						case "tp_next_7_days" :  break;
						case "tp_last_month"  :  break;
						case "tp_this_month"  :  break;
						case "tp_next_month"  :  break;
						case "tp_last_30_days":  break;
						case "tp_next_30_days":  break;
						case "tp_last_year"   :  break;
						case "tp_this_year"   :  break;
						case "tp_next_year"   :  break;
					}
					break;
				}
				case "int32":
				{
					switch ( lstFILTER_OPERATOR.SelectedValue )
					{
						case "equals"    :  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "less"      :  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "greater"   :  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "between"   :  txtFILTER_SEARCH_TEXT.Visible = true ;  lblFILTER_AND_SEPARATOR.Visible = true;  txtFILTER_SEARCH_TEXT2.Visible = true ;  break;
						case "not_equals":  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "empty"     :  break;
						case "not_empty" :  break;
					}
					break;
				}
				case "decimal":
				{
					switch ( lstFILTER_OPERATOR.SelectedValue )
					{
						case "equals"    :  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "less"      :  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "greater"   :  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "between"   :  txtFILTER_SEARCH_TEXT.Visible = true ;  lblFILTER_AND_SEPARATOR.Visible = true;  txtFILTER_SEARCH_TEXT2.Visible = true ;  break;
						case "not_equals":  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "empty"     :  break;
						case "not_empty" :  break;
					}
					break;
				}
				case "float":
				{
					switch ( lstFILTER_OPERATOR.SelectedValue )
					{
						case "equals"    :  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "less"      :  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "greater"   :  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "between"   :  txtFILTER_SEARCH_TEXT.Visible = true ;  lblFILTER_AND_SEPARATOR.Visible = true;  txtFILTER_SEARCH_TEXT2.Visible = true ;  break;
						case "not_equals":  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "empty"     :  break;
						case "not_empty" :  break;
					}
					break;
				}
				case "bool":
				{
					switch ( lstFILTER_OPERATOR.SelectedValue )
					{
						case "equals"    :
							// 12/20/2006 Paul.  We need to populate the dropdown for booleans with 1 and 0. 
							lstFILTER_SEARCH_DROPDOWN.Visible = true ;
							lstFILTER_SEARCH_DROPDOWN.Items.Clear();
							lstFILTER_SEARCH_DROPDOWN.Items.Add(new ListItem(L10n.Term(".LBL_YES"), "1"));
							lstFILTER_SEARCH_DROPDOWN.Items.Add(new ListItem(L10n.Term(".LBL_NO" ), "0"));
							break;
						case "empty"     :  break;
						case "not_empty" :  break;
					}
					break;
				}
				case "guid":
				{
					switch ( lstFILTER_OPERATOR.SelectedValue )
					{
						case "is"            :  txtFILTER_SEARCH_TEXT.Visible = true ;  txtFILTER_SEARCH_TEXT.ReadOnly = true ;  btnFILTER_SEARCH_SELECT.Visible = false;  break;
						case "equals"        :  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "contains"      :  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "starts_with"   :  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "ends_with"     :  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "not_equals_str":  txtFILTER_SEARCH_TEXT.Visible = true ;  break;
						case "empty"         :  break;
						case "not_empty"     :  break;
					}
					break;
				}
				case "enum":
				{
					// 02/09/2007 Paul.  If this is an enum, then populate the listbox with list names pulled from EDITVIEWS_FIELDS.
					string[] arrModule = lstFILTER_COLUMN_SOURCE.SelectedValue.Split(' ');
					string sModule = arrModule[0];
					
					string[] arrColumn = lstFILTER_COLUMN.SelectedValue.Split('.');
					string sColumnName = arrColumn[0];
					if ( arrColumn.Length > 1 )
						sColumnName = arrColumn[1];
					
					string sMODULE_TABLE = Sql.ToString(Application["Modules." + sModule + ".TableName"]);
					string sLIST_NAME = SplendidCache.ReportingFilterColumnsListName(sMODULE_TABLE, sColumnName);
					if ( Sql.IsEmptyString(sLIST_NAME) )
					{
						lstFILTER_SEARCH_DROPDOWN.DataSource = null;
						lstFILTER_SEARCH_LISTBOX .DataSource = null;
						lstFILTER_SEARCH_DROPDOWN.DataBind();
						lstFILTER_SEARCH_LISTBOX .DataBind();
					}
					else
					{
						lstFILTER_SEARCH_DROPDOWN.DataSource = SplendidCache.List(sLIST_NAME);
						lstFILTER_SEARCH_DROPDOWN.DataBind();
						lstFILTER_SEARCH_LISTBOX .DataSource = lstFILTER_SEARCH_DROPDOWN.DataSource;
						lstFILTER_SEARCH_LISTBOX .DataBind();
						switch ( lstFILTER_OPERATOR.SelectedValue )
						{
							case "is"            :  lstFILTER_SEARCH_DROPDOWN.Visible = true;  break;
							case "one_of"        :  lstFILTER_SEARCH_LISTBOX .Visible = true;  break;
							case "empty"         :  break;
							case "not_empty"     :  break;
						}
					}
					break;
				}
			}
		}

		// 04/17/2007 Paul.  We need to apply ACL rules a little different from the standard.
		// 07/09/2007 Paul.  Fixes from Version 1.2 on 04/17/2007 were not included in Version 1.4 tree.
		public static void ACLFilter(StringBuilder sbJoin, StringBuilder sbWhere, string sMODULE_NAME, string sACCESS_TYPE, string sASSIGNED_USER_ID_Field, bool bIsCaseSignificantDB)
		{
			// 12/07/2006 Paul.  Not all views use ASSIGNED_USER_ID as the assigned field.  Allow an override. 
			// 11/25/2006 Paul.  Administrators should not be restricted from seeing items because of the team rights.
			// This is so that an administrator can fix any record with a bad team value. 
			bool bEnableTeamManagement  = Crm.Config.enable_team_management();
			bool bRequireTeamManagement = Crm.Config.require_team_management();
			bool bIsAdmin = Security.IS_ADMIN;
#if DEBUG
			bIsAdmin = false;
#endif
			if ( bIsAdmin )
				bRequireTeamManagement = false;
			if ( bEnableTeamManagement )
			{
				if ( bRequireTeamManagement )
					sbJoin.Append("       inner join vwTEAM_MEMBERSHIPS  vwTEAM_MEMBERSHIPS_" + sMODULE_NAME + ControlChars.CrLf);
				else
					sbJoin.Append("  left outer join vwTEAM_MEMBERSHIPS  vwTEAM_MEMBERSHIPS_" + sMODULE_NAME + ControlChars.CrLf);
				sbJoin.Append("               on vwTEAM_MEMBERSHIPS_" + sMODULE_NAME + ".MEMBERSHIP_TEAM_ID = " + sMODULE_NAME + ".TEAM_ID" + ControlChars.CrLf);
				sbJoin.Append("              and vwTEAM_MEMBERSHIPS_" + sMODULE_NAME + ".MEMBERSHIP_USER_ID = @MEMBERSHIP_USER_ID" + ControlChars.CrLf);
				//Sql.AddParameter(cmd, "@MEMBERSHIP_USER_ID", Security.USER_ID);
			}

			if ( bEnableTeamManagement && !bRequireTeamManagement && !bIsAdmin )
				sbWhere.Append("   and (" + sMODULE_NAME + ".TEAM_ID is null or vwTEAM_MEMBERSHIPS_" + sMODULE_NAME + ".MEMBERSHIP_ID is not null)" + ControlChars.CrLf);
			int nACLACCESS = Security.GetUserAccess(sMODULE_NAME, sACCESS_TYPE);
			if ( nACLACCESS == ACL_ACCESS.OWNER )
			{
				sASSIGNED_USER_ID_Field = sMODULE_NAME + "." + sASSIGNED_USER_ID_Field;
				string sFieldPlaceholder = "MEMBERSHIP_USER_ID";  //Sql.NextPlaceholder(cmd, sASSIGNED_USER_ID_Field);
				// 01/22/2007 Paul.  If ASSIGNED_USER_ID is null, then let everybody see it. 
				// This was added to work around a bug whereby the ASSIGNED_USER_ID was not automatically assigned to the creating user. 
				bool bShowUnassigned = Crm.Config.show_unassigned();
				if ( bShowUnassigned )
				{
					if ( bIsCaseSignificantDB )
						sbWhere.Append("   and (" + sASSIGNED_USER_ID_Field + " is null or upper(" + sASSIGNED_USER_ID_Field + ") = upper(@" + sFieldPlaceholder + "))" + ControlChars.CrLf);
					else
						sbWhere.Append("   and (" + sASSIGNED_USER_ID_Field + " is null or "       + sASSIGNED_USER_ID_Field +  " = @"       + sFieldPlaceholder + ")"  + ControlChars.CrLf);
				}
				else
				{
					if ( bIsCaseSignificantDB )
						sbWhere.Append("   and upper(" + sASSIGNED_USER_ID_Field + ") = upper(@" + sFieldPlaceholder + ")" + ControlChars.CrLf);
					else
						sbWhere.Append("   and "       + sASSIGNED_USER_ID_Field +  " = @"       + sFieldPlaceholder       + ControlChars.CrLf);
				}
				//Sql.AddParameter(cmd, "@" + sFieldPlaceholder, Security.USER_ID);
			}
		}

		private void BuildReportSQL()
		{
			bool bIsOracle = false;
			bool bIsDB2    = false;
			bool bIsMySQL  = false;
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					bIsOracle = Sql.IsOracle(cmd);
					bIsDB2    = Sql.IsDB2   (cmd);
					bIsMySQL  = Sql.IsMySQL (cmd);
				}
			}
			
			StringBuilder sb = new StringBuilder();
			StringBuilder sbACLWhere = new StringBuilder();
			if ( rdl.DocumentElement != null )
			{
				string sMODULE_TABLE = Sql.ToString(Application["Modules." + lstMODULE.SelectedValue + ".TableName"]);
				int nMaxLen = Math.Max(sMODULE_TABLE.Length, 15);
				Hashtable hashRequiredModules  = new Hashtable();
				Hashtable hashAvailableModules = new Hashtable();
				sb.Append("select ");
				
				bool bSelectAll = true;
				// 05/29/2006 Paul.  If the module is used in a filter, then it is required. 
				XmlDocument xmlDisplayColumns = rdl.GetCustomProperty("DisplayColumns");
				XmlNodeList nlFields = xmlDisplayColumns.DocumentElement.SelectNodes("DisplayColumn/Field");
				foreach ( XmlNode xField in nlFields )
					nMaxLen = Math.Max(nMaxLen, xField.InnerText.Length);
				
				string sFieldSeparator = "";
				foreach ( XmlNode xField in nlFields )
				{
					bSelectAll = false;
					sb.Append(sFieldSeparator);
					string sMODULE_ALIAS = xField.InnerText.Split('.')[0];
					if ( !hashRequiredModules.ContainsKey(sMODULE_ALIAS) )
						hashRequiredModules.Add(sMODULE_ALIAS, null);
					sb.Append(xField.InnerText);
					sb.Append(Strings.Space(nMaxLen - xField.InnerText.Length));
					sb.Append(" as \"" + xField.InnerText + "\"");
					sb.Append(ControlChars.CrLf);
					sFieldSeparator = "     , ";
				}
				if ( bSelectAll )
				{
					sb.Append("*" + ControlChars.CrLf);
				}
				
				// 05/29/2006 Paul.  If the module is used in a filter, then it is required. 
				XmlDocument xmlFilters = rdl.GetCustomProperty("Filters");
				XmlNodeList nlFilters = xmlFilters.DocumentElement.SelectNodes("Filter");
				foreach ( XmlNode xFilter in nlFilters )
				{
					string sDATA_FIELD = XmlUtil.SelectSingleNode(xFilter, "DATA_FIELD");
					string sMODULE_ALIAS = sDATA_FIELD.Split('.')[0];
					if ( !hashRequiredModules.ContainsKey(sMODULE_ALIAS) )
						hashRequiredModules.Add(sMODULE_ALIAS, null);
				}

				if ( hashRequiredModules.ContainsKey(sMODULE_TABLE) )
					hashRequiredModules.Remove(sMODULE_TABLE);
				
				sb.Append("  from            vw" + sMODULE_TABLE + " " + Strings.Space(nMaxLen - sMODULE_TABLE.Length) + sMODULE_TABLE + ControlChars.CrLf);
				// 04/17/2007 Paul.  Apply ACL rules. 
				if ( sMODULE_TABLE != "USERS" )
					ACLFilter(sb, sbACLWhere, sMODULE_TABLE, "list", "ASSIGNED_USER_ID", bIsOracle || bIsDB2);
				hashAvailableModules.Add(sMODULE_TABLE, sMODULE_TABLE);
				if ( !Sql.IsEmptyString(lstRELATED.SelectedValue) )
				{
					XmlDocument xmlRelatedModules = rdl.GetCustomProperty("RelatedModules");
					string sRELATED       = lstRELATED.SelectedValue.Split(' ')[0];
					string sRELATED_ALIAS = lstRELATED.SelectedValue.Split(' ')[1];
					
					if ( hashRequiredModules.ContainsKey(sRELATED_ALIAS) )
						hashRequiredModules.Remove(sRELATED_ALIAS);

					XmlNode xRelationship = xmlRelatedModules.DocumentElement.SelectSingleNode("Relationship[RHS_MODULE=\'" + sRELATED + "\']");
					if ( xRelationship != null )
					{
						string sRELATIONSHIP_NAME = XmlUtil.SelectSingleNode(xRelationship, "RELATIONSHIP_NAME");
						//string sLHS_MODULE        = XmlUtil.SelectSingleNode(xRelationship, "LHS_MODULE"       );
						string sLHS_TABLE         = XmlUtil.SelectSingleNode(xRelationship, "LHS_TABLE"        );
						string sLHS_KEY           = XmlUtil.SelectSingleNode(xRelationship, "LHS_KEY"          );
						//string sRHS_MODULE        = XmlUtil.SelectSingleNode(xRelationship, "RHS_MODULE"       );
						string sRHS_TABLE         = XmlUtil.SelectSingleNode(xRelationship, "RHS_TABLE"        );
						string sRHS_KEY           = XmlUtil.SelectSingleNode(xRelationship, "RHS_KEY"          );
						string sJOIN_TABLE        = XmlUtil.SelectSingleNode(xRelationship, "JOIN_TABLE"       );
						string sJOIN_KEY_LHS      = XmlUtil.SelectSingleNode(xRelationship, "JOIN_KEY_LHS"     );
						string sJOIN_KEY_RHS      = XmlUtil.SelectSingleNode(xRelationship, "JOIN_KEY_RHS"     );
						if ( Sql.IsEmptyString(sJOIN_TABLE) )
						{
							nMaxLen = Math.Max(nMaxLen, sRHS_TABLE.Length + sRHS_KEY.Length + 1);
							sb.Append("       inner join vw" + sRHS_TABLE + " "            + Strings.Space(nMaxLen - sRHS_TABLE.Length                      ) + sRHS_TABLE + ControlChars.CrLf);
							sb.Append("               on "   + sRHS_TABLE + "." + sRHS_KEY + Strings.Space(nMaxLen - sRHS_TABLE.Length - sRHS_KEY.Length - 1) + " = " + sLHS_TABLE + "." + sLHS_KEY + ControlChars.CrLf);
							// 04/17/2007 Paul.  Apply ACL rules. 
							if ( sRHS_TABLE != "USERS" )
								ACLFilter(sb, sbACLWhere, sRHS_TABLE, "list", "ASSIGNED_USER_ID", bIsOracle || bIsDB2);
						}
						else
						{
							nMaxLen = Math.Max(nMaxLen, sJOIN_TABLE.Length + sJOIN_KEY_LHS.Length + 1);
							nMaxLen = Math.Max(nMaxLen, sRHS_TABLE.Length + sRHS_KEY.Length      + 1);
							sb.Append("       inner join vw" + sJOIN_TABLE + " "                 + Strings.Space(nMaxLen - sJOIN_TABLE.Length                           ) + sJOIN_TABLE + ControlChars.CrLf);
							sb.Append("               on "   + sJOIN_TABLE + "." + sJOIN_KEY_LHS + Strings.Space(nMaxLen - sJOIN_TABLE.Length - sJOIN_KEY_LHS.Length - 1) + " = " + sLHS_TABLE  + "." + sLHS_KEY      + ControlChars.CrLf);
							sb.Append("       inner join vw" + sRHS_TABLE + " "                  + Strings.Space(nMaxLen - sRHS_TABLE.Length                            ) + sRHS_TABLE + ControlChars.CrLf);
							sb.Append("               on "   + sRHS_TABLE + "." + sRHS_KEY       + Strings.Space(nMaxLen - sRHS_TABLE.Length - sRHS_KEY.Length - 1      ) + " = " + sJOIN_TABLE + "." + sJOIN_KEY_RHS + ControlChars.CrLf);
							// 04/17/2007 Paul.  Apply ACL rules. 
							if ( sRHS_TABLE != "USERS" )
								ACLFilter(sb, sbACLWhere, sRHS_TABLE, "list", "ASSIGNED_USER_ID", bIsOracle || bIsDB2);
						}
						if ( !hashAvailableModules.ContainsKey(sRHS_TABLE) )
							hashAvailableModules.Add(sRHS_TABLE, sRHS_TABLE);
					}
				}
				if ( hashRequiredModules.Count > 0 )
				{
					XmlDocument xmlRelationships = rdl.GetCustomProperty("Relationships");
					foreach ( string sMODULE_ALIAS in hashRequiredModules.Keys )
					{
						XmlNode xRelationship = xmlRelationships.DocumentElement.SelectSingleNode("Relationship[MODULE_ALIAS=\'" + sMODULE_ALIAS + "\']");
						if ( xRelationship != null )
						{
							string sRELATIONSHIP_NAME = XmlUtil.SelectSingleNode(xRelationship, "RELATIONSHIP_NAME");
							//string sLHS_MODULE        = XmlUtil.SelectSingleNode(xRelationship, "LHS_MODULE"       );
							string sLHS_TABLE         = XmlUtil.SelectSingleNode(xRelationship, "LHS_TABLE"        );
							string sLHS_KEY           = XmlUtil.SelectSingleNode(xRelationship, "LHS_KEY"          );
							//string sRHS_MODULE        = XmlUtil.SelectSingleNode(xRelationship, "RHS_MODULE"       );
							string sRHS_TABLE         = XmlUtil.SelectSingleNode(xRelationship, "RHS_TABLE"        );
							string sRHS_KEY           = XmlUtil.SelectSingleNode(xRelationship, "RHS_KEY"          );
							nMaxLen = Math.Max(nMaxLen, sLHS_TABLE.Length );
							nMaxLen = Math.Max(nMaxLen, sMODULE_ALIAS.Length + sLHS_KEY.Length + 1);
							sb.Append("  left outer join vw" + sLHS_TABLE + " "               + Strings.Space(nMaxLen - sLHS_TABLE.Length                        ) + sMODULE_ALIAS + ControlChars.CrLf);
							sb.Append("               on "   + sMODULE_ALIAS + "." + sLHS_KEY + Strings.Space(nMaxLen - sMODULE_ALIAS.Length - sLHS_KEY.Length - 1) + " = " + sRHS_TABLE + "." + sRHS_KEY + ControlChars.CrLf);
							// 04/17/2007 Paul.  Apply ACL rules. 
							if ( sLHS_TABLE != "USERS" )
								ACLFilter(sb, sbACLWhere, sMODULE_ALIAS, "list", "ASSIGNED_USER_ID", bIsOracle || bIsDB2);
							// 07/13/2006 Paul.  The key needs to be the alias, and the value is the main table. 
							// This is because the same table may be referenced more than once, 
							// such as the Users table to display the last modified user and the assigned to user. 
							if ( !hashAvailableModules.ContainsKey(sMODULE_ALIAS) )
								hashAvailableModules.Add(sMODULE_ALIAS, sLHS_TABLE);
						}
					}
				}
				sb.Append(" where 1 = 1" + ControlChars.CrLf);
				sb.Append(sbACLWhere.ToString());
				try
				{
					rdl.SetSingleNode("DataSets/DataSet/Query/QueryParameters", String.Empty);
					XmlNode xQueryParameters = rdl.SelectNode("DataSets/DataSet/Query/QueryParameters");
					xQueryParameters.RemoveAll();
					if ( xmlFilters.DocumentElement != null )
					{
						int nParameterIndex = 0;
						foreach ( XmlNode xFilter in xmlFilters.DocumentElement )
						{
							string sMODULE_NAME    = XmlUtil.SelectSingleNode(xFilter, "MODULE_NAME");
							string sDATA_FIELD     = XmlUtil.SelectSingleNode(xFilter, "DATA_FIELD" );
							string sDATA_TYPE      = XmlUtil.SelectSingleNode(xFilter, "DATA_TYPE"  );
							string sOPERATOR       = XmlUtil.SelectSingleNode(xFilter, "OPERATOR"   );
							// 07/04/2006 Paul.  We need to use the parameter index in the parameter name 
							// because a parameter can be used more than once and we need a unique name. 
							string sPARAMETER_NAME = RdlDocument.RdlParameterName(sDATA_FIELD, nParameterIndex, false);
							string sSECONDARY_NAME = RdlDocument.RdlParameterName(sDATA_FIELD, nParameterIndex, true );
							string sSEARCH_TEXT1   = String.Empty;
							string sSEARCH_TEXT2   = String.Empty;
							
							XmlNodeList nlValues = xFilter.SelectNodes("SEARCH_TEXT_VALUES");
							string[] arrSEARCH_TEXT = new string[nlValues.Count];
							int i = 0;
							foreach ( XmlNode xValue in nlValues )
							{
								arrSEARCH_TEXT[i++] = xValue.InnerText;
							}
							if ( arrSEARCH_TEXT.Length > 0 )
								sSEARCH_TEXT1 = arrSEARCH_TEXT[0];
							if ( arrSEARCH_TEXT.Length > 1 )
								sSEARCH_TEXT2 = arrSEARCH_TEXT[1];

							string sSQL = string.Empty;
							// 07/09/2007 Paul.  ansistring is treated the same as string. 
							string sCOMMON_DATA_TYPE = sDATA_TYPE;
							if ( sCOMMON_DATA_TYPE == "ansistring" )
								sCOMMON_DATA_TYPE = "string";
							switch ( sCOMMON_DATA_TYPE )
							{
								case "string":
								{
									// 07/16/2006 Paul.  Oracle and DB2 are case-significant.  Keep SQL Server code fast by not converting to uppercase. 
									if ( bIsOracle || bIsDB2 )
									{
										sSEARCH_TEXT1 = sSEARCH_TEXT1.ToUpper();
										sSEARCH_TEXT2 = sSEARCH_TEXT2.ToUpper();
										sDATA_FIELD   = "upper(" + sDATA_FIELD + ")";
									}
									switch ( sOPERATOR )
									{
										case "equals"        :  sb.Append("   and " + sDATA_FIELD + " = "    + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "contains"      :  sb.Append("   and " + sDATA_FIELD + " like " + sPARAMETER_NAME + (bIsMySQL ? " escape '\\\\'" : " escape '\\'") + ControlChars.CrLf);
											sSQL = '%' + Sql.EscapeSQLLike(sSEARCH_TEXT1) + '%';
											if ( bIsMySQL )
												sSQL = sSQL.Replace("\\", "\\\\");  // 07/16/2006 Paul.  MySQL requires that slashes be escaped, even in the escape clause. 
											rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, sSQL);
											break;
										case "starts_with"   :  sb.Append("   and " + sDATA_FIELD + " like " + sPARAMETER_NAME + (bIsMySQL ? " escape '\\\\'" : " escape '\\'") + ControlChars.CrLf);
											sSQL =       Sql.EscapeSQLLike(sSEARCH_TEXT1) + '%';
											if ( bIsMySQL )
												sSQL = sSQL.Replace("\\", "\\\\");  // 07/16/2006 Paul.  MySQL requires that slashes be escaped, even in the escape clause. 
											rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, sSQL);
											break;
										case "ends_with"     :  sb.Append("   and " + sDATA_FIELD + " like " + sPARAMETER_NAME + (bIsMySQL ? " escape '\\\\'" : " escape '\\'") + ControlChars.CrLf);
											sSQL = '%' + Sql.EscapeSQLLike(sSEARCH_TEXT1)      ;
											if ( bIsMySQL )
												sSQL = sSQL.Replace("\\", "\\\\");  // 07/16/2006 Paul.  MySQL requires that slashes be escaped, even in the escape clause. 
											rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, sSQL);
											break;
										case "not_equals_str":  sb.Append("   and " + sDATA_FIELD + " <> "   + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "empty"         :  sb.Append("   and " + sDATA_FIELD + " is null"     + ControlChars.CrLf);  break;
										case "not_empty"     :  sb.Append("   and " + sDATA_FIELD + " is not null" + ControlChars.CrLf);  break;
									}
									break;
								}
								case "datetime":
								{
									if ( arrSEARCH_TEXT.Length > 0 )
									{
										//CalendarControl.SqlDateTimeFormat, ciEnglish.DateTimeFormat
										DateTime dtSEARCH_TEXT1 = DateTime.ParseExact(sSEARCH_TEXT1, "yyyy/MM/dd", Thread.CurrentThread.CurrentCulture.DateTimeFormat);
										DateTime dtSEARCH_TEXT2 = DateTime.MinValue;
										if ( arrSEARCH_TEXT.Length > 1 )
											dtSEARCH_TEXT2 = DateTime.ParseExact(sSEARCH_TEXT2, "yyyy/MM/dd", Thread.CurrentThread.CurrentCulture.DateTimeFormat);
										switch ( sOPERATOR )
										{
											case "on"             :  sb.Append("   and dbo.fnDateOnly(" + sDATA_FIELD + ") = "  + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, sSEARCH_TEXT1);  break;
											case "before"         :  sb.Append("   and dbo.fnDateOnly(" + sDATA_FIELD + ") < "  + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, sSEARCH_TEXT1);  break;
											case "after"          :  sb.Append("   and dbo.fnDateOnly(" + sDATA_FIELD + ") > "  + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, sSEARCH_TEXT1);  break;
											case "not_equals_str" :  sb.Append("   and dbo.fnDateOnly(" + sDATA_FIELD + ") <> " + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, sSEARCH_TEXT1);  break;
											case "between_dates"  :  sb.Append("   and dbo.fnDateOnly(" + sDATA_FIELD + ") between " + sPARAMETER_NAME + " and " + sSECONDARY_NAME + ControlChars.CrLf);
												rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, dtSEARCH_TEXT1.ToShortDateString());
												rdl.AddQueryParameter(xQueryParameters, sSECONDARY_NAME, sDATA_TYPE, dtSEARCH_TEXT2.ToShortDateString());
												break;
										}
									}
									else
									{
										switch ( sOPERATOR )
										{
											case "empty"          :  sb.Append("   and " + sDATA_FIELD + " is null"     + ControlChars.CrLf);  break;
											case "not_empty"      :  sb.Append("   and " + sDATA_FIELD + " is not null" + ControlChars.CrLf);  break;
											case "tp_yesterday"   :  sb.Append("   and dbo.fnDateOnly(" + sDATA_FIELD + ") = " + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, "DATEADD(DAY, -1, TODAY())");  break;
											case "tp_today"       :  sb.Append("   and dbo.fnDateOnly(" + sDATA_FIELD + ") = " + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, "TODAY()");  break;
											case "tp_tomorrow"    :  sb.Append("   and dbo.fnDateOnly(" + sDATA_FIELD + ") = " + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, "DATEADD(DAY, 1, TODAY())");  break;
											case "tp_last_7_days" :  sb.Append("   and dbo.fnDateOnly(" + sDATA_FIELD + ") between " + sPARAMETER_NAME + " and " + sSECONDARY_NAME + ControlChars.CrLf);
												rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, "DATEADD(DAY, -7, TODAY())");
												rdl.AddQueryParameter(xQueryParameters, sSECONDARY_NAME, sDATA_TYPE, "TODAY()");
												break;
											case "tp_next_7_days" :  sb.Append("   and dbo.fnDateOnly(" + sDATA_FIELD + ") between " + sPARAMETER_NAME + " and " + sSECONDARY_NAME + ControlChars.CrLf);
												rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, "TODAY()");
												rdl.AddQueryParameter(xQueryParameters, sSECONDARY_NAME, sDATA_TYPE, "DATEADD(DAY, 7, TODAY())");
												break;
											// 07/05/2006 Paul.  Month math must also include the year. 
											case "tp_last_month"  :  sb.Append("   and month(" + sDATA_FIELD + ") = " + sPARAMETER_NAME + ControlChars.CrLf);
																	 sb.Append("   and year("  + sDATA_FIELD + ") = " + sSECONDARY_NAME + ControlChars.CrLf);
												rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, "MONTH(DATEADD(MONTH, -1, TODAY()))");
												rdl.AddQueryParameter(xQueryParameters, sSECONDARY_NAME, sDATA_TYPE, "YEAR(DATEADD(MONTH, -1, TODAY()))");
												break;
											case "tp_this_month"  :  sb.Append("   and month(" + sDATA_FIELD + ") = " + sPARAMETER_NAME + ControlChars.CrLf);
																	 sb.Append("   and year("  + sDATA_FIELD + ") = " + sSECONDARY_NAME + ControlChars.CrLf);
												rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, "MONTH(TODAY())");
												rdl.AddQueryParameter(xQueryParameters, sSECONDARY_NAME, sDATA_TYPE, "YEAR(TODAY())");
												break;
											case "tp_next_month"  :  sb.Append("   and month(" + sDATA_FIELD + ") = " + sPARAMETER_NAME + ControlChars.CrLf);
																	 sb.Append("   and year("  + sDATA_FIELD + ") = " + sSECONDARY_NAME + ControlChars.CrLf);
												rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, "MONTH(DATEADD(MONTH, 1, TODAY()))");
												rdl.AddQueryParameter(xQueryParameters, sSECONDARY_NAME, sDATA_TYPE, "YEAR(DATEADD(MONTH, 1, TODAY()))");
												break;
											case "tp_last_30_days":  sb.Append("   and dbo.fnDateOnly(" + sDATA_FIELD + ") between " + sPARAMETER_NAME + " and " + sSECONDARY_NAME + ControlChars.CrLf);
												rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, "DATEADD(DAY, -30, TODAY())");
												rdl.AddQueryParameter(xQueryParameters, sSECONDARY_NAME, sDATA_TYPE, "TODAY()");
												break;
											case "tp_next_30_days":  sb.Append("   and dbo.fnDateOnly(" + sDATA_FIELD + ") between " + sPARAMETER_NAME + " and " + sSECONDARY_NAME + ControlChars.CrLf);
												rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, "TODAY()");
												rdl.AddQueryParameter(xQueryParameters, sSECONDARY_NAME, sDATA_TYPE, "DATEADD(DAY, 30, TODAY())");
												break;
											case "tp_last_year"   :  sb.Append("   and year(" + sDATA_FIELD + ") = " + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, "YEAR(DATEADD(YEAR, -1, TODAY()))");  break;
											case "tp_this_year"   :  sb.Append("   and year(" + sDATA_FIELD + ") = " + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, "YEAR(TODAY())");  break;
											case "tp_next_year"   :  sb.Append("   and year(" + sDATA_FIELD + ") = " + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, "YEAR(DATEADD(YEAR, 1, TODAY()))");  break;
										}
									}
									break;
								}
								case "int32":
								{
									switch ( sOPERATOR )
									{
										case "equals"    :  sb.Append("   and " + sDATA_FIELD + " = "    + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "less"      :  sb.Append("   and " + sDATA_FIELD + " < "    + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "greater"   :  sb.Append("   and " + sDATA_FIELD + " > "    + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "not_equals":  sb.Append("   and " + sDATA_FIELD + " <> "   + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "between"   :  sb.Append("   and " + sDATA_FIELD + " between "   + sPARAMETER_NAME + "1 and " + sPARAMETER_NAME + "2" + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, sSEARCH_TEXT1, sSEARCH_TEXT2);  break;
										case "empty"     :  sb.Append("   and " + sDATA_FIELD + " is null"     + ControlChars.CrLf);  break;
										case "not_empty" :  sb.Append("   and " + sDATA_FIELD + " is not null" + ControlChars.CrLf);  break;
									}
									break;
								}
								case "decimal":
								{
									switch ( sOPERATOR )
									{
										case "equals"    :  sb.Append("   and " + sDATA_FIELD + " = "    + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "less"      :  sb.Append("   and " + sDATA_FIELD + " < "    + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "greater"   :  sb.Append("   and " + sDATA_FIELD + " > "    + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "not_equals":  sb.Append("   and " + sDATA_FIELD + " <> "   + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "between"   :  sb.Append("   and " + sDATA_FIELD + " between "   + sPARAMETER_NAME + "1 and " + sPARAMETER_NAME + "2" + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, sSEARCH_TEXT1, sSEARCH_TEXT2);  break;
										case "empty"     :  sb.Append("   and " + sDATA_FIELD + " is null"     + ControlChars.CrLf);  break;
										case "not_empty" :  sb.Append("   and " + sDATA_FIELD + " is not null" + ControlChars.CrLf);  break;
									}
									break;
								}
								case "float":
								{
									switch ( sOPERATOR )
									{
										case "equals"    :  sb.Append("   and " + sDATA_FIELD + " = "    + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "less"      :  sb.Append("   and " + sDATA_FIELD + " < "    + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "greater"   :  sb.Append("   and " + sDATA_FIELD + " > "    + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "not_equals":  sb.Append("   and " + sDATA_FIELD + " <> "   + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "between"   :  sb.Append("   and " + sDATA_FIELD + " between "   + sPARAMETER_NAME + "1 and " + sPARAMETER_NAME + "2" + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, sSEARCH_TEXT1, sSEARCH_TEXT2);  break;
										case "empty"     :  sb.Append("   and " + sDATA_FIELD + " is null"     + ControlChars.CrLf);  break;
										case "not_empty" :  sb.Append("   and " + sDATA_FIELD + " is not null" + ControlChars.CrLf);  break;
									}
									break;
								}
								case "bool":
								{
									switch ( sOPERATOR )
									{
										case "equals"    :  sb.Append("   and " + sDATA_FIELD + " = "    + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "empty"     :  sb.Append("   and " + sDATA_FIELD + " is null"     + ControlChars.CrLf);  break;
										case "not_empty" :  sb.Append("   and " + sDATA_FIELD + " is not null" + ControlChars.CrLf);  break;
									}
									break;
								}
								case "guid":
								{
									// 07/16/2006 Paul.  Oracle and DB2 are case-significant.  Keep SQL Server code fast by not converting to uppercase. 
									if ( bIsOracle || bIsDB2 )
									{
										sSEARCH_TEXT1 = sSEARCH_TEXT1.ToUpper();
										sSEARCH_TEXT2 = sSEARCH_TEXT2.ToUpper();
										sDATA_FIELD   = "upper(" + sDATA_FIELD + ")";
									}
									switch ( sOPERATOR )
									{
										case "is"            :  sb.Append("   and " + sDATA_FIELD + " = "    + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "equals"        :  sb.Append("   and " + sDATA_FIELD + " = "    + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "contains"      :  sb.Append("   and " + sDATA_FIELD + " like " + sPARAMETER_NAME + (bIsMySQL ? " escape '\\\\'" : " escape '\\'") + ControlChars.CrLf);
											sSQL = '%' + Sql.EscapeSQLLike(sSEARCH_TEXT1) + '%';
											if ( bIsMySQL )
												sSQL = sSQL.Replace("\\", "\\\\");  // 07/16/2006 Paul.  MySQL requires that slashes be escaped, even in the escape clause. 
											rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, sSQL);
											break;
										case "starts_with"   :  sb.Append("   and " + sDATA_FIELD + " like " + sPARAMETER_NAME + (bIsMySQL ? " escape '\\\\'" : " escape '\\'") + ControlChars.CrLf);
											sSQL =       Sql.EscapeSQLLike(sSEARCH_TEXT1) + '%';
											if ( bIsMySQL )
												sSQL = sSQL.Replace("\\", "\\\\");  // 07/16/2006 Paul.  MySQL requires that slashes be escaped, even in the escape clause. 
											rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, sSQL);
											break;
										case "ends_with"     :  sb.Append("   and " + sDATA_FIELD + " like " + sPARAMETER_NAME + (bIsMySQL ? " escape '\\\\'" : " escape '\\'") + ControlChars.CrLf);
											sSQL = '%' + Sql.EscapeSQLLike(sSEARCH_TEXT1)      ;
											if ( bIsMySQL )
												sSQL = sSQL.Replace("\\", "\\\\");  // 07/16/2006 Paul.  MySQL requires that slashes be escaped, even in the escape clause. 
											rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE, sSQL);
											break;
										case "not_equals_str":  sb.Append("   and " + sDATA_FIELD + " <> "   + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "empty"         :  sb.Append("   and " + sDATA_FIELD + " is null"     + ControlChars.CrLf);  break;
										case "not_empty"     :  sb.Append("   and " + sDATA_FIELD + " is not null" + ControlChars.CrLf);  break;
									}
									break;
								}
								case "enum":
								{
									// 07/16/2006 Paul.  Oracle and DB2 are case-significant.  Keep SQL Server code fast by not converting to uppercase. 
									if ( bIsOracle || bIsDB2 )
									{
										sSEARCH_TEXT1 = sSEARCH_TEXT1.ToUpper();
										sSEARCH_TEXT2 = sSEARCH_TEXT2.ToUpper();
										sDATA_FIELD   = "upper(" + sDATA_FIELD + ")";
									}
									switch ( sOPERATOR )
									{
										// 02/09/2007 Paul.  enum uses is instead of equals operator. 
										case "is"    :  sb.Append("   and " + sDATA_FIELD + " = "   + sPARAMETER_NAME + ControlChars.CrLf);  rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME, sDATA_TYPE,       sSEARCH_TEXT1      );  break;
										case "one_of":
										{
											arrSEARCH_TEXT = Sql.ToStringArray(lstFILTER_SEARCH_LISTBOX);
											if ( arrSEARCH_TEXT.Length > 0 )
											{
												sb.Append("   and " + sDATA_FIELD + " in (");
												for ( int j = 0; j < arrSEARCH_TEXT.Length; j++ )
												{
													if ( j > 0 )
														sb.Append(", ");
													sb.Append(sPARAMETER_NAME + "_" + j.ToString("000"));
													rdl.AddQueryParameter(xQueryParameters, sPARAMETER_NAME + "_" + j.ToString("000"), "string", Sql.ToString(arrSEARCH_TEXT[j]));
												}
												sb.Append(")" + ControlChars.CrLf);
											}
											break;
										}
										case "empty"         :  sb.Append("   and " + sDATA_FIELD + " is null"     + ControlChars.CrLf);  break;
										case "not_empty"     :  sb.Append("   and " + sDATA_FIELD + " is not null" + ControlChars.CrLf);  break;
									}
									break;
								}
							}
							nParameterIndex++;
						}
					}
					// 06/18/2006 Paul.  The element 'QueryParameters' in namespace 'http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition' has incomplete content. List of possible elements expected: 'http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition:QueryParameter'. 
					if ( xQueryParameters.ChildNodes.Count == 0 )
					{
						xQueryParameters.ParentNode.RemoveChild(xQueryParameters);
					}
				}
				catch(Exception ex)
				{
					ctlReportButtons.ErrorText = ex.Message;
				}
				// 06/15/2006 Paul.  Completely rebuild the Fields list based on the available modules. 
				rdl.SetSingleNode("DataSets/DataSet/Fields", String.Empty);
				XmlNode xFields = rdl.SelectNode("DataSets/DataSet/Fields");
				xFields.RemoveAll();
				// 07/13/2006 Paul.  The key is the alias and the value is the module. 
				// This is so that the same module can be referenced many times with many aliases. 
				foreach ( string sTableAlias in hashAvailableModules.Keys )
				{
					string sModule = Sql.ToString(hashAvailableModules[sTableAlias]);
					DataTable dtColumns = SplendidCache.ReportingFilterColumns(sModule).Copy();
					foreach(DataRow row in dtColumns.Rows)
					{
						string sFieldName = sTableAlias + "." + Sql.ToString(row["NAME"]);
						string sCsType = Sql.ToString(row["CsType"]);
						string sFieldType = String.Empty;
						switch ( sCsType )
						{
							case "Guid"      :  sFieldType = "System.Guid"    ;  break;
							case "string"    :  sFieldType = "System.String"  ;  break;
							case "ansistring":  sFieldType = "System.String"  ;  break;
							case "DateTime"  :  sFieldType = "System.DateTime";  break;
							case "bool"      :  sFieldType = "System.Boolean" ;  break;
							case "float"     :  sFieldType = "System.Double"  ;  break;
							case "decimal"   :  sFieldType = "System.Decimal" ;  break;
							case "short"     :  sFieldType = "System.Int16"   ;  break;
							case "Int32"     :  sFieldType = "System.Int32"   ;  break;
							case "Int64"     :  sFieldType = "System.Int64"   ;  break;
							default          :  sFieldType = "System.String"  ;  break;
						}
						rdl.CreateField(xFields, sFieldName, sFieldType);
					}
				}
			}
			sReportSQL = sb.ToString();
			rdl.SetSingleNode("DataSets/DataSet/Query/CommandText", sReportSQL);
		}

		private DataTable ReportFilters()
		{
			DataTable dtFilters = new DataTable();
			XmlDocument xmlFilters = rdl.GetCustomProperty("Filters");
			dtFilters = XmlUtil.CreateDataTable(xmlFilters.DocumentElement, "Filter", new string[] {"ID", "MODULE_NAME", "DATA_FIELD", "DATA_TYPE", "OPERATOR", "SEARCH_TEXT"});
			return dtFilters;
		}

		private DataTable ReportColumnSource()
		{
			DataTable dtColumnSource = new DataTable();
			XmlDocument xmlRelationships = rdl.GetCustomProperty("Relationships");
			dtColumnSource = XmlUtil.CreateDataTable(xmlRelationships.DocumentElement, "Relationship", new string[] {"MODULE_NAME", "DISPLAY_NAME"});
			return dtColumnSource;
		}

		#region Filter Editing
		protected void FiltersGet(string sID, ref string sMODULE_NAME, ref string sDATA_FIELD, ref string sDATA_TYPE, ref string sOPERATOR, ref string[] arrSEARCH_TEXT)
		{
			XmlDocument xmlFilters = rdl.GetCustomProperty("Filters");
			XmlNode xFilter = xmlFilters.DocumentElement.SelectSingleNode("Filter[ID=\'" + sID + "\']");
			if ( xFilter != null )
			{
				sMODULE_NAME = XmlUtil.SelectSingleNode(xFilter, "MODULE_NAME");
				sDATA_FIELD  = XmlUtil.SelectSingleNode(xFilter, "DATA_FIELD" );
				sDATA_TYPE   = XmlUtil.SelectSingleNode(xFilter, "DATA_TYPE"  );
				sOPERATOR    = XmlUtil.SelectSingleNode(xFilter, "OPERATOR"   );
				//sSEARCH_TEXT = XmlUtil.GetSingleNode(xFilter, "SEARCH_TEXT");
				XmlNodeList nlValues = xFilter.SelectNodes("SEARCH_TEXT_VALUES");
				arrSEARCH_TEXT = new string[nlValues.Count];
				int i = 0;
				foreach ( XmlNode xValue in nlValues )
				{
					arrSEARCH_TEXT[i++] = xValue.InnerText;
				}
			}
		}

		protected void RemoveInvalidDisplayColumns()
		{
			Hashtable hashMODULES = new Hashtable();
			string sMODULE_TABLE = Sql.ToString(Application["Modules." + lstMODULE.SelectedValue + ".TableName"]);
			hashMODULES.Add(sMODULE_TABLE, lstMODULE.SelectedValue);

			XmlDocument xmlRelationships = rdl.GetCustomProperty("Relationships");
			DataView vwModuleColumnSource = new DataView(XmlUtil.CreateDataTable(xmlRelationships.DocumentElement, "Relationship", new string[] { "MODULE_NAME", "MODULE_ALIAS", "DISPLAY_NAME", "RELATIONSHIP_TYPE" }));
			vwModuleColumnSource.RowFilter = "RELATIONSHIP_TYPE = 'one-to-many'";
			foreach ( DataRowView row in vwModuleColumnSource )
			{
				hashMODULES.Add(Sql.ToString(row["MODULE_ALIAS"]), Sql.ToString(row["MODULE_NAME"]));
			}
			// 07/13/2006 Paul.  Related may not exist, so not forget to check. 
			if ( lstRELATED.SelectedValue.IndexOf(' ') >= 0 )
			{
				string sRELATED       = lstRELATED.SelectedValue.Split(' ')[0];
				string sRELATED_ALIAS = lstRELATED.SelectedValue.Split(' ')[1];
				hashMODULES.Add(sRELATED_ALIAS, sRELATED);
			}
			XmlDocument xmlDisplayColumns = rdl.GetCustomProperty("DisplayColumns");
			try
			{
				ArrayList arrDeleted = new ArrayList();
				XmlNodeList nlFields = xmlDisplayColumns.DocumentElement.SelectNodes("DisplayColumn/Field");
				foreach ( XmlNode xField in nlFields )
				{
					// 07/13/2006 Paul.  The column stores the module and the alias.  We need to verify the alias. 
					string sDATA_FIELD = xField.InnerText;
					if ( sDATA_FIELD.IndexOf('.') >= 0 )
					{
						string sMODULE_ALIAS = sDATA_FIELD.Split('.')[0];
						if ( !hashMODULES.ContainsKey(sMODULE_ALIAS) )
						{
							arrDeleted.Add(xField);
						}
					}
					else
					{
						// Delete filter if not formatted properly.  It must include the table alias. 
						arrDeleted.Add(xField);
					}
				}
				foreach ( XmlNode xField in arrDeleted )
				{
					rdl.RemoveField(xField.InnerText);
					xmlDisplayColumns.DocumentElement.RemoveChild(xField.ParentNode);
				}
				rdl.SetCustomProperty("DisplayColumns", xmlDisplayColumns.OuterXml.Replace("</DisplayColumn>", "</DisplayColumn>" + ControlChars.CrLf));
			}
			catch(Exception ex)
			{
				ctlReportButtons.ErrorText = ex.Message;
			}
		}

		protected void RemoveInvalidFilters()
		{
			Hashtable hashMODULES = new Hashtable();
			string sMODULE_TABLE = Sql.ToString(Application["Modules." + lstMODULE.SelectedValue + ".TableName"]);
			hashMODULES.Add(sMODULE_TABLE, lstMODULE.SelectedValue);

			XmlDocument xmlRelationships = rdl.GetCustomProperty("Relationships");
			DataView vwModuleColumnSource = new DataView(XmlUtil.CreateDataTable(xmlRelationships.DocumentElement, "Relationship", new string[] { "MODULE_NAME", "MODULE_ALIAS", "DISPLAY_NAME", "RELATIONSHIP_TYPE" }));
			vwModuleColumnSource.RowFilter = "RELATIONSHIP_TYPE = 'one-to-many'";
			foreach ( DataRowView row in vwModuleColumnSource )
			{
				hashMODULES.Add(Sql.ToString(row["MODULE_ALIAS"]), Sql.ToString(row["MODULE_NAME"]));
			}
			// 07/13/2006 Paul.  Related may not exist, so not forget to check. 
			if ( lstRELATED.SelectedValue.IndexOf(' ') >= 0 )
			{
				string sRELATED       = lstRELATED.SelectedValue.Split(' ')[0];
				string sRELATED_ALIAS = lstRELATED.SelectedValue.Split(' ')[1];
				hashMODULES.Add(sRELATED_ALIAS, sRELATED);
			}

			XmlDocument xmlFilters = rdl.GetCustomProperty("Filters");
			try
			{
				ArrayList arrDeleted = new ArrayList();
				XmlNodeList nlFilters = xmlFilters.DocumentElement.SelectNodes("Filter");
				foreach ( XmlNode xFilter in nlFilters )
				{
					// 07/13/2006 Paul.  The filter stores the module and the alias.  We need to verify the alias. 
					string sDATA_FIELD = XmlUtil.SelectSingleNode(xFilter, "DATA_FIELD");
					if ( sDATA_FIELD.IndexOf('.') >= 0 )
					{
						string sMODULE_ALIAS = sDATA_FIELD.Split('.')[0];
						if ( !hashMODULES.ContainsKey(sMODULE_ALIAS) )
						{
							arrDeleted.Add(xFilter);
						}
					}
					else
					{
						// Delete filter if not formatted properly.  It must include the table alias. 
						arrDeleted.Add(xFilter);
					}
				}
				foreach ( XmlNode xFilter in arrDeleted )
				{
					xmlFilters.DocumentElement.RemoveChild(xFilter);
				}
				rdl.SetCustomProperty("Filters", xmlFilters.OuterXml);
				
				dgFilters.DataSource = ReportFilters();
				dgFilters.DataBind();
			}
			catch(Exception ex)
			{
				ctlReportButtons.ErrorText = ex.Message;
			}
		}

		protected void FiltersUpdate(string sID, string sMODULE_NAME, string sDATA_FIELD, string sDATA_TYPE, string sOPERATOR, string[] arrSEARCH_TEXT)
		{
			XmlDocument xmlFilters = rdl.GetCustomProperty("Filters");
			try
			{
				XmlNode xFilter = xmlFilters.DocumentElement.SelectSingleNode("Filter[ID=\'" + sID + "\']");
				if ( xFilter == null || Sql.IsEmptyString(sID) )
				{
					xFilter = xmlFilters.CreateElement("Filter");
					xmlFilters.DocumentElement.AppendChild(xFilter);
					XmlUtil.SetSingleNode(xmlFilters, xFilter, "ID", Guid.NewGuid().ToString());
				}
				else
				{
					// 06/12/2006 Paul.  The easiest way to remove the old text values is to delete them all. 
					xFilter.RemoveAll();
					XmlUtil.SetSingleNode(xmlFilters, xFilter, "ID", sID);
				}
				XmlUtil.SetSingleNode(xmlFilters, xFilter, "MODULE_NAME", sMODULE_NAME  );
				XmlUtil.SetSingleNode(xmlFilters, xFilter, "DATA_FIELD" , sDATA_FIELD   );
				XmlUtil.SetSingleNode(xmlFilters, xFilter, "DATA_TYPE"  , sDATA_TYPE    );
				XmlUtil.SetSingleNode(xmlFilters, xFilter, "OPERATOR"   , sOPERATOR     );
				XmlUtil.SetSingleNode(xmlFilters, xFilter, "SEARCH_TEXT", String.Join(", ", arrSEARCH_TEXT));
				foreach ( string sSEARCH_TEXT in arrSEARCH_TEXT )
				{
					XmlNode xSearchText = xmlFilters.CreateElement("SEARCH_TEXT_VALUES");
					xFilter.AppendChild(xSearchText);
					xSearchText.InnerText = sSEARCH_TEXT;
				}
				
				rdl.SetCustomProperty("Filters", xmlFilters.OuterXml);
				
				dgFilters.DataSource = ReportFilters();
				dgFilters.DataBind();
			}
			catch(Exception ex)
			{
				ctlReportButtons.ErrorText = ex.Message;
			}
		}

		protected void FiltersDelete(string sID)
		{
			dgFilters.EditItemIndex = -1;
			XmlDocument xmlFilters = rdl.GetCustomProperty("Filters");
			XmlNode xFilter = xmlFilters.DocumentElement.SelectSingleNode("Filter[ID=\'" + sID + "\']");
			if ( xFilter != null )
			{
				xFilter.ParentNode.RemoveChild(xFilter);
				rdl.SetCustomProperty("Filters", xmlFilters.OuterXml);
			}
			dgFilters.DataSource = ReportFilters();
			dgFilters.DataBind();
		}

		protected void DisplayColumnsUpdate()
		{
			// 06/15/2006 Paul.  There is no need to load the existing Fields data as we are going to completely replace it. 
			//string sFields = rdl.GetCustomProperty("DisplayColumns");
			try
			{
				XmlDocument xmlDisplayColumns = new XmlDocument();
				xmlDisplayColumns.AppendChild(xmlDisplayColumns.CreateElement("DisplayColumns"));

				DataTable dtDisplayColumns = ctlDisplayColumnsChooser.LeftValuesTable;
				if ( dtDisplayColumns != null )
				{
					foreach ( DataRow row in dtDisplayColumns.Rows )
					{
						// 07/15/2006 Paul.  Store  both the header and the field. 
						// The previous method of relying upon the RDL Header notes has a greater potential for errors. 
						XmlNode xDisplayColumn = xmlDisplayColumns.CreateElement("DisplayColumn");
						xmlDisplayColumns.DocumentElement.AppendChild(xDisplayColumn);

						XmlNode xLabel = xmlDisplayColumns.CreateElement("Label");
						XmlNode xField = xmlDisplayColumns.CreateElement("Field");
						xDisplayColumn.AppendChild(xLabel);
						xDisplayColumn.AppendChild(xField);
						xLabel.InnerText = Sql.ToString(row["text" ]);
						xField.InnerText = Sql.ToString(row["value"]);
					}
				}
				rdl.SetCustomProperty("DisplayColumns", xmlDisplayColumns.OuterXml.Replace("</DisplayColumn>", "</DisplayColumn>" + ControlChars.CrLf));
			}
			catch(Exception ex)
			{
				ctlReportButtons.ErrorText = ex.Message;
			}
		}

		#endregion

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
				return;

			reqNAME.DataBind();
			lblDisplayColumnsRequired.DataBind();
			lblDisplayColumnsRequired.Visible = false;

			rdl = new RdlDocument();
			lblMODULE              .Visible = bDebug;
			lblRELATED             .Visible = bDebug;
			lblMODULE_COLUMN_SOURCE.Visible = bDebug;
			lblFILTER_COLUMN_SOURCE.Visible = bDebug;
			lblFILTER_COLUMN       .Visible = bDebug;
			lblFILTER_OPERATOR_TYPE.Visible = bDebug;
			lblFILTER_OPERATOR     .Visible = bDebug;
			lblFILTER_ID           .Visible = bDebug;
			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				
				bRun = Sql.ToBoolean(Request["Run"]);
				string sRdl = Sql.ToString(Session["rdl"]);
				// 07/09/2006 Paul.  The ReportViewer has a bug that prevents it from reloading a previous RDL.
				// The only solution is to create a new ReportViewer object. 
				if ( bRun && sRdl.Length > 0 )
				{
					// 07/13/2006 Paul.  We don't store the SHOW_QUERY value in the RDL, so we must retrieve it from the session. 
					chkSHOW_QUERY.Checked = Sql.ToBoolean(Session["Reports.SHOW_QUERY"]);
					// 07/09/2006 Paul.  Clear the Run parameter on the command line. 
					RegisterClientScriptBlock("frmRedirect", "<script type=\"text/javascript\">document.forms[0].action='edit.aspx?ID=" + gID.ToString() + "';</script>");
					// 07/09/2006 Paul.  Clear the session variable as soon as we are done loading it. 
					//Session.Remove("rdl");
					rdl.LoadRdl(sRdl);
					
					txtNAME.Text              = rdl.SelectNodeAttribute(String.Empty, "Name");
					txtACTIVE_TAB.Value       = "1";
					radREPORT_TYPE_TABULAR.Checked = true;
					txtASSIGNED_USER_ID.Value = rdl.GetCustomPropertyValue("AssignedUserID");
					txtASSIGNED_TO.Text       = rdl.SelectNodeValue("Author");
					
					SetReportType(rdl.GetCustomPropertyValue("ReportType"));
					ctlModuleHeader.Title = rdl.SelectNodeAttribute(String.Empty, "Name");
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
					ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;
					ViewState["ASSIGNED_USER_ID"     ] = txtASSIGNED_USER_ID.Value;
					
					try
					{
						lstMODULE.DataSource = SplendidCache.ReportingModules();
						lstMODULE.DataBind();
						lstMODULE.SelectedValue = rdl.GetCustomPropertyValue("Module");
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					}
					lstRELATED_Bind();
					try
					{
						lstRELATED.SelectedValue = rdl.GetCustomPropertyValue("Related");
					}
					catch
					{
					}
					// 07/26/2007 Paul.  The column sources need to be updated after the related has changed. 
					lstFILTER_COLUMN_SOURCE_Bind();
					//ctlDisplayColumnsChooser_Bind();  // 07/26/2007 Paul.  Already called inside lstFILTER_COLUMN_SOURCE_Bind. 
					BuildReportSQL();
					// 07/13/2006 Paul.  The DisplayColumns List must be bound after the ReportSQL is built. 
					lstLeftListBox_Bind();

					dgFilters.DataSource = ReportFilters();
					dgFilters.DataBind();

					ctlReportView.RunReport(rdl.OuterXml);
				}
				else if ( !IsPostBack )
				{
					txtNAME.Text              = "untitled";
					txtACTIVE_TAB.Value       = "1";
					radREPORT_TYPE_TABULAR.Checked = true;
					txtASSIGNED_TO.Text       = Security.USER_NAME;
					txtASSIGNED_USER_ID.Value = Security.USER_ID.ToString();
					ViewState["ASSIGNED_USER_ID"] = txtASSIGNED_USER_ID.Value;
					lstMODULE.DataSource = SplendidCache.ReportingModules();
					lstMODULE.DataBind();
					lblMODULE.Text = lstMODULE.SelectedValue;
					
					// 07/13/2006 Paul.  We don't store the SHOW_QUERY value in the RDL, so we must retrieve it from the session. 
					chkSHOW_QUERY.Checked = Sql.ToBoolean(Session["Reports.SHOW_QUERY"]);
					
					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL;
							sSQL = "select *             " + ControlChars.CrLf
							     + "  from vwREPORTS_Edit" + ControlChars.CrLf
							     + " where ID = @ID      " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								if ( !Sql.IsEmptyGuid(gDuplicateID) )
								{
									Sql.AddParameter(cmd, "@ID", gDuplicateID);
									gID = Guid.Empty;
								}
								else
								{
									Sql.AddParameter(cmd, "@ID", gID);
								}
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										ctlModuleHeader.Title = Sql.ToString(rdr["NAME"]);
										SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
										ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;

										txtNAME.Text              = Sql.ToString(rdr["NAME"            ]);
										txtASSIGNED_USER_ID.Value = Sql.ToString(rdr["ASSIGNED_USER_ID"]);
										txtASSIGNED_TO.Text       = Sql.ToString(rdr["ASSIGNED_TO"     ]);
										ViewState["ASSIGNED_USER_ID"] = txtASSIGNED_USER_ID.Value;

										string sXML = Sql.ToString(rdr["RDL"]);
										try
										{
											if ( !Sql.IsEmptyString(sXML) )
											{
												rdl.LoadRdl(Sql.ToString(rdr["RDL"]));
												
												SetReportType(rdl.GetCustomPropertyValue("ReportType"));
												// 07/09/2006 Paul.  Update Assigned values as they may have changed externally. 
												rdl.SetCustomProperty("AssignedUserID", txtASSIGNED_USER_ID.Value);
												rdl.SetSingleNode("Author", txtASSIGNED_TO.Text);
												lstMODULE.SelectedValue = rdl.GetCustomPropertyValue("Module");
												lblMODULE.Text = lstMODULE.SelectedValue;
											}
										}
										catch
										{
										}
									}
									else
									{
										// 11/25/2006 Paul.  If item is not visible, then don't allow save 
										ctlReportButtons.DisableAll();
										ctlReportButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
									}
								}
							}
						}
					}
					// 05/27/2006 Paul.  This is a catch-all statement to create a new report if all else fails. 
					if ( rdl.DocumentElement == null )
					{
						rdl = new RdlDocument(txtNAME.Text, txtASSIGNED_TO.Text);
						rdl.SetCustomProperty("Module"        , lstMODULE.SelectedValue  );
						rdl.SetCustomProperty("Related"       , lstRELATED.SelectedValue );
						rdl.SetCustomProperty("ReportType"    , GetReportType()          );
						rdl.SetCustomProperty("AssignedUserID", txtASSIGNED_USER_ID.Value);
					}
					lstRELATED_Bind();
					lblRELATED.Text = lstRELATED.SelectedValue;
					try
					{
						lstRELATED.SelectedValue = rdl.GetCustomPropertyValue("Related");
					}
					catch
					{
					}
					// 07/26/2007 Paul.  The column sources need to be updated after the related has changed. 
					lstFILTER_COLUMN_SOURCE_Bind();
					BuildReportSQL();
					// 07/13/2006 Paul.  The DisplayColumns List must be bound after the ReportSQL is built. 
					lstLeftListBox_Bind();

					dgFilters.DataSource = ReportFilters();
					dgFilters.DataBind();
				}
				else
				{
					// 07/13/2006 Paul.  Save the SHOW_QUERY flag in the Session so that it will be available across redirects. 
					Session["Reports.SHOW_QUERY"] = chkSHOW_QUERY.Checked;

					sRdl = Sql.ToString(ViewState["rdl"]);
					rdl.LoadRdl(sRdl);
					rdl.SetSingleNodeAttribute(rdl.DocumentElement, "Name", txtNAME.Text);
					if ( Sql.ToString(ViewState["ASSIGNED_USER_ID"]) != txtASSIGNED_USER_ID.Value )
					{
						txtASSIGNED_TO.Text = Crm.Users.USER_NAME(Sql.ToGuid(txtASSIGNED_USER_ID.Value));
						ViewState["ASSIGNED_USER_ID"] = txtASSIGNED_USER_ID.Value;
					}
					rdl.SetCustomProperty("ReportType"    , GetReportType()          );
					rdl.SetCustomProperty("AssignedUserID", txtASSIGNED_USER_ID.Value);
					rdl.SetSingleNode("Author", txtASSIGNED_TO.Text);
					// 07/15/2006 Paul.  The DisplayColumns custom node will not be the primary location of the column data. 
					// Always just push to the RDL Headers instead of trying to read from them. 
					DisplayColumnsUpdate();
					XmlDocument xmlDisplayColumns = rdl.GetCustomProperty("DisplayColumns");
					DataTable dtDisplayColumns = XmlUtil.CreateDataTable(xmlDisplayColumns.DocumentElement, "DisplayColumn", new string[] { "Label", "Field"});
					rdl.UpdateDataTable(dtDisplayColumns);
					xmlDisplayColumns = null;

					ctlDisplayColumnsChooser_Bind();
					BuildReportSQL();
					// 07/13/2006 Paul.  The DisplayColumns List must be bound after the ReportSQL is built. 
					lstLeftListBox_Bind();

					dgFilters.DataSource = ReportFilters();
					dgFilters.DataBind();

					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					ctlModuleHeader.Title = Sql.ToString(ViewState["ctlModuleHeader.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
				}
#if DEBUG
				//RegisterClientScriptBlock("ReportSQL", "<script type=\"text/javascript\">sDebugSQL += '" + Sql.EscapeJavaScript("\r" + sReportSQL) + "';</script>");
#endif
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlReportButtons.ErrorText = ex.Message;
			}
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			if ( chkSHOW_QUERY.Checked )
			{
				litREPORT_QUERY.Text = "<br /><table border=\"1\" cellpadding=\"3\" cellspacing=\"0\" width=\"100%\" bgcolor=\"LightGrey\"><tr><td>";
				litREPORT_QUERY.Text += "<pre><b>" + (bRun ? ctlReportView.ReportSQL : sReportSQL) + "</b></pre>";
				litREPORT_QUERY.Text += "</td></tr></table><br />";
			}
			ViewState["rdl"] = rdl.OuterXml;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);
			ctlReportButtons.Command = new CommandEventHandler(Page_Command);
			m_sMODULE = "Reports";
			SetMenu(m_sMODULE);
		}
		#endregion
	}
}
