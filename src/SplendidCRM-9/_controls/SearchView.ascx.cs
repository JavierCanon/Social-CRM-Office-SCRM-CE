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
using System;
using System.Xml;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for SearchView.
	/// </summary>
	public class SearchView : SplendidControl
	{
		protected string        sGridID                = "grdMain";
		protected Label         lblError               ;
		protected DataTable     dtFields               ;
		protected string        sSEARCH_VIEW           ;
		protected string        sSearchMode            ;
		protected bool          bRegisterEnterKeyPress = true;
		protected int           nAdvanced              = 0;
		protected bool          bShowSearchTabs        = true;
		protected bool          bShowSearchViews       = true;
		protected bool          bIsPopupSearch         = false;

		protected HtmlTable     tblSearch              ;
		protected HyperLink     lnkBasicSearch         ;
		protected HyperLink     lnkAdvancedSearch      ;
		protected Button        btnSearch              ;
		protected Button        btnClear               ;
		protected Panel         pnlSearchPanel         ;
		protected Panel         pnlSavedSearchPanel    ;
		protected Image         imgBasicSearch         ;
		protected Image         imgAdvancedSearch      ;
		protected DropDownList  lstColumns             ;
		protected DropDownList  lstSavedSearches       ;
		protected RadioButton   radSavedSearchDESC     ;
		protected RadioButton   radSavedSearchASC      ;
		protected TextBox       txtSavedSearchName     ;
		protected Label         lblSavedNameRequired   ;
		protected Button        btnSavedSearchUpdate   ;
		protected Button        btnSavedSearchDelete   ;
		protected Label         lblCurrentSearch       ;
		protected Label         lblCurrentXML          ;

		public CommandEventHandler Command ;

		public string GridID
		{
			get { return sGridID; }
			set { sGridID = value; }
		}

		public string Module
		{
			get { return m_sMODULE; }
			set { m_sMODULE = value; }
		}

		public string SearchMode
		{
			get { return sSearchMode; }
			set { sSearchMode = value; }
		}

		public bool RegisterEnterKeyPress
		{
			get { return bRegisterEnterKeyPress; }
			set { bRegisterEnterKeyPress = value; }
		}

		public bool ShowSearchTabs
		{
			get { return bShowSearchTabs; }
			set { bShowSearchTabs = value; }
		}

		public bool ShowSearchViews
		{
			get { return bShowSearchViews; }
			set { bShowSearchViews = value; }
		}

		public bool IsPopupSearch
		{
			get { return bIsPopupSearch; }
			set { bIsPopupSearch = value; }
		}

		public bool SavedSearchesChanged()
		{
			return lstSavedSearches.SelectedValue != Sql.ToString(ViewState["SavedSearches_PreviousValue"]);
		}

		protected void SaveDefaultView()
		{
			try
			{
				string sXML = GenerateSavedSearch(true);

				Guid gID = Guid.Empty;
				// 12/17/2007 Paul.  The default view must include the SearchModule in the name so that it does not get confused. 
				DataView vwSavedSearch = new DataView(SplendidCache.SavedSearch(sSEARCH_VIEW));
				vwSavedSearch.RowFilter = "NAME is null";
				if ( vwSavedSearch.Count > 0 )
					gID = Sql.ToGuid(vwSavedSearch[0]["ID"]);
				SqlProcs.spSAVED_SEARCH_Update(ref gID, Security.USER_ID, String.Empty, sSEARCH_VIEW, sXML, String.Empty);
				// 12/09/2007 Paul.  If the default already exists, then just update its contents, saving us from having to clear the cache. 
				if ( vwSavedSearch.Count == 0 )
					SplendidCache.ClearSavedSearch(sSEARCH_VIEW);
				else
					vwSavedSearch[0]["CONTENTS"] = sXML;
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		public void ApplySavedSearch()
		{
			// 12/08/2007 Paul.  Clear before we apply, just in case some fields were added since the view was saved. 
			ClearForm();
			if ( !IsPostBack )
			{
				// 12/09/2007 Paul.  The last search will be saved with no name. 
				// 12/17/2007 Paul.  The default view must include the SearchModule in the name so that it does not get confused. 
				DataView vwSavedSearches = new DataView(SplendidCache.SavedSearch(sSEARCH_VIEW));
				vwSavedSearches.RowFilter = "NAME is null";
				if ( vwSavedSearches.Count > 0 )
				{
					string sXML = Sql.ToString(vwSavedSearches[0]["CONTENTS"]);
					ApplySavedSearch(sXML);
				}
			}
			else
			{
				if ( !Sql.IsEmptyString(lstSavedSearches.SelectedValue) )
				{
					DataView vwSavedSearches = new DataView(SplendidCache.SavedSearch(m_sMODULE));
					vwSavedSearches.RowFilter = "ID = '" + lstSavedSearches.SelectedValue + "'";
					if ( vwSavedSearches.Count > 0 )
					{
						string sXML = Sql.ToString(vwSavedSearches[0]["CONTENTS"]);
						ApplySavedSearch(sXML);
					}
				}
			}
		}

		public void ApplySavedSearch(string sXML)
		{
			try
			{
				if ( !Sql.IsEmptyString(sXML) )
				{
					XmlDocument xml = new XmlDocument();
					xml.LoadXml(sXML);

					string sSortColumn = XmlUtil.SelectSingleNode(xml, "SortColumn");
					string sSortOrder  = XmlUtil.SelectSingleNode(xml, "SortOrder" );
					if ( !Sql.IsEmptyString(sSortColumn) && ! Sql.IsEmptyString(sSortOrder) && Command != null )
					{
						// 12/14/2007 Paul.  The ViewState in the search control is different than the view state on the page or the grid control. 
						// We need to send a command to set the sort fields.
						string[] arrSort = new string[] { sSortColumn, sSortOrder };
						CommandEventArgs eSortGrid = new CommandEventArgs("SortGrid", arrSort);
						Command(this, eSortGrid);
					}
					string sDefaultSearch = XmlUtil.SelectSingleNode(xml, "DefaultSearch");
					if ( !Sql.IsEmptyString(sDefaultSearch) )
					{
						try
						{
							lstSavedSearches.SelectedValue = sDefaultSearch;
						}
						catch
						{
						}
					}

					XmlNodeList nlSearchFields = xml.DocumentElement.SelectNodes("SearchFields/Field");
					foreach ( XmlNode xField in nlSearchFields )
					{
						string sDATA_FIELD = XmlUtil.GetNamedItem(xField, "Name");
						string sFIELD_TYPE = XmlUtil.GetNamedItem(xField, "Type");
						if ( !Sql.IsEmptyString(sDATA_FIELD) )
						{
							DynamicControl ctl = new DynamicControl(this, sDATA_FIELD);
							if ( ctl != null )
							{
								if ( sFIELD_TYPE == "ListBox" )
								{
									ListControl lst = FindControl(sDATA_FIELD) as ListControl;
									if ( lst != null )
									{
										if ( lst is ListBox )
										{
											XmlNodeList nlValues = xField.SelectNodes("Value");
											foreach ( XmlNode xValue in nlValues )
											{
												foreach ( ListItem item in lst.Items )
												{
													if ( item.Value == xValue.InnerText )
														item.Selected = true;
												}
											}
										}
										else if ( lst is DropDownList )
										{
											// 12/13/2007 Paul.  DropDownLists must be handled separately to ensure that only one item is selected. 
											try
											{
												lst.SelectedValue = xField.InnerText;
											}
											catch
											{
											}
										}
									}
								}
								else if ( sFIELD_TYPE == "DatePicker" )
								{
									DatePicker ctlDate = FindControl(sDATA_FIELD) as DatePicker;
									if ( ctlDate != null )
									{
										if ( !Sql.IsEmptyString(xField.InnerText) )
										{
											ctlDate.DateText = xField.InnerText;
										}
									}
								}
								else if ( sFIELD_TYPE == "DateRange" )
								{
									XmlNode xStart = xField.SelectSingleNode("After");
									if ( xStart != null )
									{
										DatePicker ctlDateStart = FindControl(sDATA_FIELD + "_AFTER") as DatePicker;
										if ( ctlDateStart != null )
										{
											if ( !Sql.IsEmptyString(xStart.InnerText) )
											{
												ctlDateStart.DateText = xStart.InnerText;
											}
										}
									}
									XmlNode xEnd = xField.SelectSingleNode("Before");
									if ( xEnd != null )
									{
										DatePicker ctlDateEnd = FindControl(sDATA_FIELD + "_BEFORE") as DatePicker;
										if ( ctlDateEnd != null )
										{
											if ( !Sql.IsEmptyString(xEnd.InnerText) )
											{
												ctlDateEnd.DateText = xEnd.InnerText;
											}
										}
									}
								}
								else if ( sFIELD_TYPE == "CheckBox" )
								{
									ctl.Checked = Sql.ToBoolean(xField.InnerText);
								}
								else if ( sFIELD_TYPE == "TextBox" )
								{
									ctl.Text = xField.InnerText;
								}
								else if ( sFIELD_TYPE == "ChangeButton" )
								{
									ctl.Text = xField.InnerText;
								}
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
			}
		}

		protected string GenerateSavedSearch(bool bDefaultSearch)
		{
			XmlDocument xml = new XmlDocument();
			xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
			XmlNode xSavedSearch = xml.CreateElement("SavedSearch");
			xml.AppendChild(xSavedSearch);
			if ( dtFields != null )
			{
				if ( bDefaultSearch )
				{
					// 12/14/2007 Paul.  Although it might be better to use events to get the sort field, 
					// that could cause an endless loop as this control sets the sort field and retrieves the sort field. 
					SplendidGrid grdMain = Parent.FindControl(sGridID) as SplendidGrid;
					if ( grdMain != null )
					{
						if ( !String.IsNullOrEmpty(grdMain.SortColumn) )
						{
							XmlNode xSortColumn = xml.CreateElement("SortColumn");
							xSavedSearch.AppendChild(xSortColumn);
							xSortColumn.InnerText = grdMain.SortColumn;
						}
						if ( !String.IsNullOrEmpty(grdMain.SortOrder) )
						{
							XmlNode xSortOrder = xml.CreateElement("SortOrder");
							xSavedSearch.AppendChild(xSortOrder);
							xSortOrder.InnerText =grdMain.SortOrder;
						}
					}
					if ( !Sql.IsEmptyString(lstSavedSearches.SelectedValue) )
					{
						XmlNode xDefaultSearch = xml.CreateElement("DefaultSearch");
						xSavedSearch.AppendChild(xDefaultSearch);
						xDefaultSearch.InnerText = lstSavedSearches.SelectedValue;
					}
				}
				else
				{
					if ( !Sql.IsEmptyString(lstColumns.SelectedValue) )
					{
						XmlNode xSortColumn = xml.CreateElement("SortColumn");
						xSavedSearch.AppendChild(xSortColumn);
						xSortColumn.InnerText = lstColumns.SelectedValue;
					}
					if ( radSavedSearchASC.Checked || radSavedSearchDESC.Checked )
					{
						XmlNode xSortOrder = xml.CreateElement("SortOrder");
						xSavedSearch.AppendChild(xSortOrder);
						if ( radSavedSearchASC.Checked )
							xSortOrder.InnerText ="asc";
						else if ( radSavedSearchDESC.Checked )
							xSortOrder.InnerText ="desc";
					}
				}

				XmlNode xSearchFields = xml.CreateElement("SearchFields");
				xSavedSearch.AppendChild(xSearchFields);
				foreach(DataRowView row in dtFields.DefaultView)
				{
					string sFIELD_TYPE        = Sql.ToString (row["FIELD_TYPE"       ]);
					string sDATA_FIELD        = Sql.ToString (row["DATA_FIELD"       ]);
					string sDISPLAY_FIELD     = Sql.ToString (row["DISPLAY_FIELD"    ]);
					int    nFORMAT_MAX_LENGTH = Sql.ToInteger(row["FORMAT_MAX_LENGTH"]);
					int    nFORMAT_ROWS       = Sql.ToInteger(row["FORMAT_ROWS"      ]);

					// 12/07/2007 Paul.  Create the field but don't append it unless it is used. 
					// This is how we will distinguish from unspecified field. 
					XmlNode xField = xml.CreateElement("Field");
					XmlUtil.SetSingleNodeAttribute(xml, xField, "Name", sDATA_FIELD);
					XmlUtil.SetSingleNodeAttribute(xml, xField, "Type", sFIELD_TYPE);
					DynamicControl ctl = new DynamicControl(this, sDATA_FIELD);
					if ( ctl != null )
					{
						if ( sFIELD_TYPE == "ListBox" )
						{
							ListControl lst = FindControl(sDATA_FIELD) as ListControl;
							if ( lst != null )
							{
								if ( lst is ListBox )
								{
									int nSelected = 0;
									foreach(ListItem item in lst.Items)
									{
										if ( item.Selected )
											nSelected++;
									}
									if ( nSelected > 0 )
									{
										xSearchFields.AppendChild(xField);
										foreach(ListItem item in lst.Items)
										{
											if ( item.Selected )
											{
												XmlNode xValue = xml.CreateElement("Value");
												xField.AppendChild(xValue);
												xValue.InnerText = item.Value;
											}
										}
									}
								}
								else if ( lst is DropDownList )
								{
									// 12/13/2007 Paul.  DropDownLists must be handled separately to ensure that only one item is selected. 
									xField.InnerText = lst.SelectedValue;
								}
							}
						}
						else if ( sFIELD_TYPE == "DatePicker" )
						{
							DatePicker ctlDate = FindControl(sDATA_FIELD) as DatePicker;
							if ( ctlDate != null )
							{
								if ( !Sql.IsEmptyString(ctlDate.DateText) )
								{
									xSearchFields.AppendChild(xField);
									xField.InnerText = ctlDate.DateText;
								}
							}
						}
						else if ( sFIELD_TYPE == "DateRange" )
						{
							DatePicker ctlDateStart = FindControl(sDATA_FIELD + "_AFTER") as DatePicker;
							if ( ctlDateStart != null )
							{
								if ( !Sql.IsEmptyString(ctlDateStart.DateText) )
								{
									xSearchFields.AppendChild(xField);
									XmlUtil.SetSingleNode(xml, xField, "After", ctlDateStart.DateText);
								}
							}
							DatePicker ctlDateEnd = FindControl(sDATA_FIELD + "_BEFORE") as DatePicker;
							if ( ctlDateEnd != null )
							{
								if ( !Sql.IsEmptyString(ctlDateEnd.DateText) )
								{
									xSearchFields.AppendChild(xField);
									XmlUtil.SetSingleNode(xml, xField, "Before", ctlDateEnd.DateText);
								}
							}
						}
						else if ( sFIELD_TYPE == "CheckBox" )
						{
							if ( ctl.Checked )
							{
								xSearchFields.AppendChild(xField);
								xField.InnerText = "true";
							}
						}
						else if ( sFIELD_TYPE == "TextBox" )
						{
							ctl.Text = ctl.Text.Trim();
							if ( !Sql.IsEmptyString(ctl.Text) )
							{
								xSearchFields.AppendChild(xField);
								xField.InnerText = ctl.Text;
							}
						}
						else if ( sFIELD_TYPE == "ChangeButton" )
						{
							ctl.Text = ctl.Text.Trim();
							if ( !Sql.IsEmptyString(ctl.Text) )
							{
								xSearchFields.AppendChild(xField);
								xField.InnerText = ctl.Text;
							}
							// 12/08/2007 Paul.  Save the display field as a separate XML node. 
							// Treat it as a text box and it will get populated just like any other search field. 
							DynamicControl ctlDISPLAY_FIELD = new DynamicControl(this, sDISPLAY_FIELD);
							if ( ctlDISPLAY_FIELD != null )
							{
								ctlDISPLAY_FIELD.Text = ctlDISPLAY_FIELD.Text.Trim();
								xField = xml.CreateElement("Field");
								XmlUtil.SetSingleNodeAttribute(xml, xField, "Name", sDISPLAY_FIELD);
								XmlUtil.SetSingleNodeAttribute(xml, xField, "Type", "TextBox");
								if ( !Sql.IsEmptyString(ctlDISPLAY_FIELD.Text) )
								{
									xSearchFields.AppendChild(xField);
									xField.InnerText = ctlDISPLAY_FIELD.Text;
								}
							}
						}
					}
				}
			}
			return xml.OuterXml;
		}

		protected void lstSavedSearches_Changed(object sender, System.EventArgs e)
		{
			if ( Sql.IsEmptyString(lstSavedSearches.SelectedValue) )
			{
				btnSavedSearchUpdate.Enabled = false;
				btnSavedSearchDelete.Enabled = false;
				lblCurrentSearch.Text = String.Empty;
				lblCurrentXML.Text = String.Empty;
			}
			else
			{
				btnSavedSearchUpdate.Enabled = true;
				btnSavedSearchDelete.Enabled = true;
				lblCurrentSearch.Text = "\"" + lstSavedSearches.SelectedItem.Text + "\"";
				lblCurrentXML.Text = String.Empty;
#if DEBUG
				DataView vwSavedSearches = new DataView(SplendidCache.SavedSearch(m_sMODULE));
				vwSavedSearches.RowFilter = "ID = '" + lstSavedSearches.SelectedValue + "'";
				if ( vwSavedSearches.Count > 0 )
				{
					string sXML = Sql.ToString(vwSavedSearches[0]["CONTENTS"]);
					lblCurrentXML.Text = Server.HtmlEncode(sXML);
				}
#endif
			}
			if ( Command != null )
			{
				// 12/08/2007 Paul.  We need to make sure the table is rebound after the view change event. 
				CommandEventArgs eSearch = new CommandEventArgs("Search", null);
				Command(sender, eSearch);
			}
		}

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "AdvancedSearch" )
			{
				Response.Redirect(Page.AppRelativeVirtualPath + "?Advanced=1");
			}
			else if ( e.CommandName == "BasicSearch" )
			{
				Response.Redirect(Page.AppRelativeVirtualPath + "?Advanced=0");
			}
			else if ( e.CommandName == "Clear" )
			{
				// 12/17/2007 Paul.  Clear the sort order as well. 
				CommandEventArgs eSortGrid = new CommandEventArgs("SortGrid", null);
				Command(this, eSortGrid);
				// 12/04/2007 Paul.  Clearing the form is not needed as the redirect will do the same. 
				// However, when we start to save the last search view, that is when the clear will be useful. 
				ClearForm();
				// 12/09/2007 Paul.  We have to save the cleared form, otherwise the default view will restore settings. 
				SaveDefaultView();
				Server.Transfer(Page.AppRelativeVirtualPath + "?Advanced=" + nAdvanced.ToString());
			}
			else if ( e.CommandName == "SavedSearch.Save" )
			{
				txtSavedSearchName.Text = txtSavedSearchName.Text.Trim();
				if ( Sql.IsEmptyString(txtSavedSearchName.Text) )
				{
					lblSavedNameRequired.Visible =true;
				}
				else
				{
					try
					{
						string sXML = GenerateSavedSearch(false);

						Guid gID = Guid.Empty;
						SqlProcs.spSAVED_SEARCH_Update(ref gID, Security.USER_ID, txtSavedSearchName.Text, m_sMODULE, sXML, String.Empty);

						// 12/14/2007 Paul.  The sort may have changed, so send an update event. 
						if ( !Sql.IsEmptyString(lstColumns.SelectedValue) && (radSavedSearchASC.Checked || radSavedSearchDESC.Checked) )
						{
							string[] arrSort = new string[] { lstColumns.SelectedValue, (radSavedSearchASC.Checked ? "asc" : "desc") };
							CommandEventArgs eSortGrid = new CommandEventArgs("SortGrid", arrSort);
							Command(this, eSortGrid);
						}
						RefreshSavedSearches(gID);
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						lblError.Text = ex.Message;
					}
				}
			}
			else if ( e.CommandName == "SavedSearch.Update" )
			{
				try
				{
					string sXML = GenerateSavedSearch(false);

					Guid gID = Sql.ToGuid(lstSavedSearches.SelectedItem.Value);
					SqlProcs.spSAVED_SEARCH_Update(ref gID, Security.USER_ID, txtSavedSearchName.Text, m_sMODULE, sXML, String.Empty);

					// 12/14/2007 Paul.  The sort may have changed, so send an update event. 
					if ( !Sql.IsEmptyString(lstColumns.SelectedValue) && (radSavedSearchASC.Checked || radSavedSearchDESC.Checked) )
					{
						string[] arrSort = new string[] { lstColumns.SelectedValue, (radSavedSearchASC.Checked ? "asc" : "desc") };
						CommandEventArgs eSortGrid = new CommandEventArgs("SortGrid", arrSort);
						Command(this, eSortGrid);
					}
					RefreshSavedSearches(gID);
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					lblError.Text = ex.Message;
				}
			}
			else if ( e.CommandName == "SavedSearch.Delete" )
			{
				try
				{
					Guid gID = Sql.ToGuid(lstSavedSearches.SelectedItem.Value);
					SqlProcs.spSAVED_SEARCH_Delete(gID);
					RefreshSavedSearches(Guid.Empty);
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					lblError.Text = ex.Message;
				}
			}
			else if ( Command != null )
				Command(this, e) ;
		}

		public void RefreshSavedSearches(Guid gID)
		{
			txtSavedSearchName.Text = String.Empty;
			lstColumns.SelectedIndex = 0;
			radSavedSearchASC.Checked = true;
			SplendidCache.ClearSavedSearch(m_sMODULE);
			
			DataView vwSavedSearch = new DataView(SplendidCache.SavedSearch(m_sMODULE));
			vwSavedSearch.RowFilter = "NAME is not null";
			lstSavedSearches.DataSource = vwSavedSearch;
			lstSavedSearches.DataBind();
			lstSavedSearches.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
			if ( Sql.IsEmptyGuid(gID) )
			{
				lstSavedSearches.SelectedIndex = 0;
			}
			else
			{
				lstSavedSearches.SelectedValue = gID.ToString();
			}
			lstSavedSearches_Changed(lstSavedSearches, null);
		}

		public void InitializeDynamicView()
		{
			// 12/04/2007 Paul.  We need to be able to initialize the view manually as the OnInit event occurs before we have had a chance to set the mode. 
			sSEARCH_VIEW = m_sMODULE + "." + sSearchMode + (this.IsMobile ? ".Mobile" : "");
			if ( !String.IsNullOrEmpty(m_sMODULE) && !String.IsNullOrEmpty(sSearchMode) )
			{
				dtFields = SplendidCache.EditViewFields(sSEARCH_VIEW);
				tblSearch.Rows.Clear();
				// 01/24/2008 Paul.  AppendEditViewFields was recently modified to append .Mobile to the name, so make sure it is not appended twice. 
				this.AppendEditViewFields(m_sMODULE + "." + sSearchMode, tblSearch, null);
				if ( dtFields.Rows.Count > 0 )
				{
					string sVIEW_NAME = Sql.ToString(dtFields.Rows[0]["VIEW_NAME"]);
					DataTable dtColumns = SplendidCache.SearchColumns(sVIEW_NAME).Copy();
					foreach(DataRow row in dtColumns.Rows)
					{
						// 07/04/2006 Paul.  Some columns have global terms. 
						row["DISPLAY_NAME"] = Utils.TableColumnName(L10n, m_sMODULE, Sql.ToString(row["DISPLAY_NAME"]));
					}
					
					DataView vwColumns = new DataView(dtColumns);
					vwColumns.Sort = "DISPLAY_NAME";
					lstColumns.DataSource = vwColumns;
					lstColumns.DataBind();
					lstColumns.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
				}
			}
		}

		public virtual void ClearForm()
		{
			if ( dtFields != null )
			{
				foreach(DataRowView row in dtFields.DefaultView)
				{
					string sFIELD_TYPE        = Sql.ToString (row["FIELD_TYPE"       ]);
					string sDATA_FIELD        = Sql.ToString (row["DATA_FIELD"       ]);
					string sDISPLAY_FIELD     = Sql.ToString (row["DISPLAY_FIELD"    ]);
					int    nFORMAT_MAX_LENGTH = Sql.ToInteger(row["FORMAT_MAX_LENGTH"]);
					DynamicControl ctl = new DynamicControl(this, sDATA_FIELD);
					if ( ctl != null )
					{
						if ( sFIELD_TYPE == "ListBox" )
						{
							ListControl lst = FindControl(sDATA_FIELD) as ListControl;
							if ( lst != null )
							{
								if ( lst is ListBox )
								{
									// 12/12/2007 Paul.  ClearSelection is the correct way to reset a ListBox. 
									lst.ClearSelection();
								}
								else if ( lst is DropDownList )
								{
									// 12/13/2007 Paul.  Clear a drop-down by selecting the top item. 
									lst.ClearSelection();
									lst.SelectedIndex = 0;
								}
							}
						}
						else if ( sFIELD_TYPE == "DatePicker" )
						{
							DatePicker ctlDate = FindControl(sDATA_FIELD) as DatePicker;
							if ( ctlDate != null )
							{
								ctlDate.DateText = String.Empty;
							}
						}
						else if ( sFIELD_TYPE == "DateRange" )
						{
							DatePicker ctlDateStart = FindControl(sDATA_FIELD + "_AFTER") as DatePicker;
							if ( ctlDateStart != null )
							{
								ctlDateStart.DateText = String.Empty;
							}
							DatePicker ctlDateEnd = FindControl(sDATA_FIELD + "_BEFORE") as DatePicker;
							if ( ctlDateEnd != null )
							{
								ctlDateEnd.DateText = String.Empty;
							}
						}
						else if ( sFIELD_TYPE == "CheckBox" )
						{
							ctl.Checked = false;
						}
						else if ( sFIELD_TYPE == "TextBox" )
						{
							ctl.Text = String.Empty;
						}
						else if ( sFIELD_TYPE == "ChangeButton" )
						{
							ctl.Text = String.Empty;
							DynamicControl ctlDISPLAY_FIELD = new DynamicControl(this, sDISPLAY_FIELD);
							if ( ctlDISPLAY_FIELD != null )
								ctlDISPLAY_FIELD.Text = String.Empty;
						}
					}
				}
			}
		}

		public virtual void SqlSearchClause(IDbCommand cmd)
		{
			if ( dtFields == null )
				InitializeDynamicView();
			if ( dtFields != null )
			{
				// 12/28/2007 Paul.  Disable the auto-save in a popup. 
				if ( !bIsPopupSearch && bShowSearchViews )
				{
					// 12/08/2007 Paul.  By apply the saved search here, we can automatically apply across old code. 
					if ( SavedSearchesChanged() || !IsPostBack )
						ApplySavedSearch();
				}

				foreach(DataRowView row in dtFields.DefaultView)
				{
					string sFIELD_TYPE        = Sql.ToString (row["FIELD_TYPE"       ]);
					string sDATA_FIELD        = Sql.ToString (row["DATA_FIELD"       ]);
					int    nFORMAT_MAX_LENGTH = Sql.ToInteger(row["FORMAT_MAX_LENGTH"]);
					int    nFORMAT_ROWS       = Sql.ToInteger(row["FORMAT_ROWS"      ]);
					DynamicControl ctl = new DynamicControl(this, sDATA_FIELD);
					if ( ctl != null )
					{
						if ( sFIELD_TYPE == "ListBox" )
						{
							ListControl lst = FindControl(sDATA_FIELD) as ListControl;
							if ( lst != null )
							{
								int nSelected = 0;
								foreach(ListItem item in lst.Items)
								{
									if ( item.Selected )
										nSelected++;
								}
								// 01/10/2008 Paul.  When drawing the search dialog the first time (not postback), 
								// we need to assume the the first item is selected if it is DropDownList. 
								if ( !IsPostBack && lst is DropDownList && lst.Items.Count > 0 && nSelected == 0 )
								{
									lst.SelectedIndex = 0;
									nSelected = 1;
								}
								// 12/03/2007 Paul.  If the NONE item is selected, then search for value of NULL. 
								if ( nSelected == 1 && String.IsNullOrEmpty(lst.SelectedValue) )
								{
									if ( sDATA_FIELD.IndexOf(' ') > 0 )
									{
										cmd.CommandText += "   and (1 = 0";
										foreach ( string sField in sDATA_FIELD.Split(' ') )
											cmd.CommandText += " or " + sField + " is null";
										cmd.CommandText += ")";
									}
									else
										cmd.CommandText += "   and " + sDATA_FIELD + " is null" + ControlChars.CrLf;
								}
								else if ( nSelected > 0 )
								{
									if ( sDATA_FIELD.IndexOf(' ') > 0 )
									{
										cmd.CommandText += "   and (1 = 0" + ControlChars.CrLf;
										foreach ( string sField in sDATA_FIELD.Split(' ') )
										{
											cmd.CommandText += "        or (1 = 1";
											Sql.AppendParameter(cmd, lst, sField);
											cmd.CommandText += "           )" + ControlChars.CrLf;
										}
										cmd.CommandText += "       )" + ControlChars.CrLf;
									}
									else
										Sql.AppendParameter(cmd,lst, sDATA_FIELD);
								}
							}
						}
						else if ( sFIELD_TYPE == "DatePicker" )
						{
							DatePicker ctlDate = FindControl(sDATA_FIELD) as DatePicker;
							if ( ctlDate != null )
							{
								if ( !Sql.IsEmptyString(ctlDate.DateText) )
								{
									Sql.AppendParameter(cmd, ctlDate.Value, sDATA_FIELD);
								}
							}
						}
						else if ( sFIELD_TYPE == "DateRange" )
						{
							DatePicker ctlDateStart = FindControl(sDATA_FIELD + "_AFTER") as DatePicker;
							DatePicker ctlDateEnd   = FindControl(sDATA_FIELD + "_BEFORE") as DatePicker;
							DateTime dtDateStart = DateTime.MinValue;
							DateTime dtDateEnd   = DateTime.MinValue;
							if ( ctlDateStart != null )
							{
								if ( !Sql.IsEmptyString(ctlDateStart.DateText) )
								{
									dtDateStart = ctlDateStart.Value;
								}
							}
							if ( ctlDateEnd != null )
							{
								if ( !Sql.IsEmptyString(ctlDateEnd.DateText) )
								{
									dtDateEnd = ctlDateEnd.Value;
								}
							}
							if ( dtDateStart != DateTime.MinValue ||dtDateEnd != DateTime.MinValue )
								Sql.AppendParameter(cmd, dtDateStart, dtDateEnd, sDATA_FIELD);
						}
						else if ( sFIELD_TYPE == "CheckBox" )
						{
							// 12/02/2007 Paul.  Only search for checked fields if they are checked. 
							if ( ctl.Checked )
							{
								// 12/02/2007 Paul.  Unassigned checkbox has a special meaning. 
								if ( sDATA_FIELD == "UNASSIGNED_ONLY" )
								{
									// 10/04/2006 Paul.  Add flag to show only records that are not assigned. 
									cmd.CommandText += "   and ASSIGNED_USER_ID is null" + ControlChars.CrLf;
								}
								else if ( sDATA_FIELD == "CURRENT_USER_ONLY" )
								{
									Sql.AppendParameter(cmd, Security.USER_ID, "ASSIGNED_USER_ID", false);
								}
								else
								{
									Sql.AppendParameter(cmd, ctl.Checked, sDATA_FIELD);
								}
							}
						}
						else if ( sFIELD_TYPE == "TextBox" )
						{
							if ( sDATA_FIELD.IndexOf(' ') > 0 )
								Sql.AppendParameter(cmd, ctl.Text, nFORMAT_MAX_LENGTH, Sql.SqlFilterMode.StartsWith, sDATA_FIELD.Split(' '));
							else
								Sql.AppendParameter(cmd, ctl.Text, nFORMAT_MAX_LENGTH, Sql.SqlFilterMode.StartsWith, sDATA_FIELD);
						}
						else if ( sFIELD_TYPE == "ChangeButton" )
						{
							if ( nFORMAT_MAX_LENGTH == 0 && sDATA_FIELD.EndsWith("_ID") )
							{
								if ( !Sql.IsEmptyGuid(ctl.ID) )
									Sql.AppendParameter(cmd, ctl.ID, sDATA_FIELD);
							}
							else
								Sql.AppendParameter(cmd, ctl.Text, nFORMAT_MAX_LENGTH, Sql.SqlFilterMode.StartsWith, sDATA_FIELD);
						}
					}
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( !IsPostBack )
			{
				// 12/08/2007 Paul.  We may need to initialize inside SqlSearchClause. 
				if ( dtFields == null )
					InitializeDynamicView();
				DataView vwSavedSearch = new DataView(SplendidCache.SavedSearch(m_sMODULE));
				vwSavedSearch.RowFilter = "NAME is not null";
				lstSavedSearches.DataSource = vwSavedSearch;
				lstSavedSearches.DataBind();
				lstSavedSearches.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
			}
			else
			{
				// 12/03/2007 Paul.  We've stopped using the unassigned checkbox.  Instead, use a NONE row. 
				ListBox  lstASSIGNED_USER_ID = FindControl("ASSIGNED_USER_ID") as ListBox;
				CheckBox chkUNASSIGNED_ONLY  = FindControl("UNASSIGNED_ONLY" ) as CheckBox;
				if ( lstASSIGNED_USER_ID != null && chkUNASSIGNED_ONLY != null )
					lstASSIGNED_USER_ID.Enabled = !chkUNASSIGNED_ONLY.Checked;
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			// 12/08/2007 Paul.  We need a way to detect the listbox change inside the Page_Load event. 
			ViewState["SavedSearches_PreviousValue"] = lstSavedSearches.SelectedValue;

			if ( IsPostBack )
			{
				// 12/28/2007 Paul.  Disable the auto-save in a popup. 
				if ( !bIsPopupSearch && bShowSearchViews )
					SaveDefaultView();
			}
			base.OnPreRender(e);
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
			nAdvanced = Sql.ToInteger(Request["Advanced"]);
			// 12/17/2007 Paul.  Allow the SearchMode default to be specified in SearchView definition. 
			// Campaigns.SearchPreview is one such area. 
			if ( Sql.IsEmptyString(sSearchMode) )
			{
				if ( bIsPopupSearch )
					sSearchMode = "SearchPopup";  // SearchBasic.Mobile is automatic. 
				else if ( nAdvanced == 0 )
					sSearchMode = "SearchBasic";  // SearchBasic.Mobile is automatic. 
				else
					sSearchMode = "SearchAdvanced";
			}
			if ( bShowSearchTabs )
			{
				lnkBasicSearch   .CssClass = (nAdvanced == 0) ? "current" : "";
				lnkAdvancedSearch.CssClass = (nAdvanced == 1) ? "current" : "";
				lnkBasicSearch   .NavigateUrl = Page.AppRelativeVirtualPath + "?Advanced=0";
				lnkAdvancedSearch.NavigateUrl = Page.AppRelativeVirtualPath + "?Advanced=1";

			}
			if ( IsPostBack )
			{
				// 12/02/2007 Paul.  AppendEditViewFields should be called inside Page_Load when not a postback, 
				// and in InitializeComponent when it is a postback. If done wrong, 
				// the page will bind after the list is populated, causing the list to populate again. 
				InitializeDynamicView();
			}
		}
		#endregion
	}
}
