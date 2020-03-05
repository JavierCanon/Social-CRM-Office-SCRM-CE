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
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.SessionState;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM
{
	public class CreateItemTemplateTranslated : ITemplate
	{
		protected string sDATA_FIELD ;
		
		public CreateItemTemplateTranslated(string sDATA_FIELD)
		{
			this.sDATA_FIELD  = sDATA_FIELD ;
		}
		public void InstantiateIn(Control objContainer)
		{
			Literal lit = new Literal();
			lit.DataBinding += new EventHandler(OnDataBinding);
			objContainer.Controls.Add(lit);
		}
		private void OnDataBinding(object sender, EventArgs e)
		{
			Literal lbl = (Literal)sender;
			DataGridItem objContainer = (DataGridItem) lbl.NamingContainer;
			DataRowView row = objContainer.DataItem as DataRowView;
			if ( row != null )
			{
				// 04/30/2006 Paul.  Use the Context to store pointers to the localization objects.
				// This is so that we don't need to require that the page inherits from SplendidPage. 
				L10N L10n = HttpContext.Current.Items["L10n"] as L10N;
				if ( L10n == null )
				{
					// 04/26/2006 Paul.  We want to have the AccessView on the SystemCheck page. 
					L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				}
				if ( row[sDATA_FIELD] != DBNull.Value )
				{
					lbl.Text = L10n.Term(Sql.ToString(row[sDATA_FIELD]));
				}
			}
			else
			{
				lbl.Text = sDATA_FIELD;
			}
		}
	}

	public class CreateItemTemplateLiteral : ITemplate
	{
		protected string sDATA_FIELD ;
		protected string sDATA_FORMAT;
		
		public CreateItemTemplateLiteral(string sDATA_FIELD, string sDATA_FORMAT)
		{
			this.sDATA_FIELD  = sDATA_FIELD ;
			this.sDATA_FORMAT = sDATA_FORMAT;
		}
		public void InstantiateIn(Control objContainer)
		{
			Literal lit = new Literal();
			lit.DataBinding += new EventHandler(OnDataBinding);
			objContainer.Controls.Add(lit);
		}
		private void OnDataBinding(object sender, EventArgs e)
		{
			Literal lbl = (Literal)sender;
			DataGridItem objContainer = (DataGridItem) lbl.NamingContainer;
			DataRowView row = objContainer.DataItem as DataRowView;
			if ( row != null )
			{
				if ( row[sDATA_FIELD] != DBNull.Value )
				{
					switch ( sDATA_FORMAT )
					{
						case "DateTime":
						{
							// 03/30/2007 Paul.  T10n should never be NULL. 
							TimeZone T10n = HttpContext.Current.Items["T10n"] as TimeZone;
							if ( T10n != null )
								lbl.Text = Sql.ToString(T10n.FromServerTime(row[sDATA_FIELD]));
							break;
						}
						case "Date":
						{
							// 03/30/2007 Paul.  T10n should never be NULL. 
							TimeZone T10n = HttpContext.Current.Items["T10n"] as TimeZone;
							if ( T10n != null )
								lbl.Text = Sql.ToDateString(T10n.FromServerTime(row[sDATA_FIELD]));
							break;
						}
						case "Currency":
						{
							// 03/30/2007 Paul.  Move init of C10n to minimize use. 
							// 05/09/2006 Paul.  Convert the currency values before displaying. 
							// The UI culture should already be set to format the currency. 
							Currency C10n = HttpContext.Current.Items["C10n"] as Currency;
							Decimal d = C10n.ToCurrency(Convert.ToDecimal(row[sDATA_FIELD]));
							lbl.Text = d.ToString("c");
							break;
						}
						default:
							lbl.Text = Sql.ToString(row[sDATA_FIELD]);
							break;
					}
				}
			}
			else
			{
				lbl.Text = sDATA_FIELD;
			}
		}
	}

	public class CreateItemTemplateLiteralList : ITemplate
	{
		protected string sDATA_FIELD ;
		protected string sLIST_NAME  ;
		
		public CreateItemTemplateLiteralList(string sDATA_FIELD, string sLIST_NAME)
		{
			this.sDATA_FIELD  = sDATA_FIELD ;
			this.sLIST_NAME   = sLIST_NAME  ;
		}
		public void InstantiateIn(Control objContainer)
		{
			Literal lit = new Literal();
			lit.DataBinding += new EventHandler(OnDataBinding);
			objContainer.Controls.Add(lit);
		}
		private void OnDataBinding(object sender, EventArgs e)
		{
			Literal lbl = (Literal)sender;
			DataGridItem objContainer = (DataGridItem) lbl.NamingContainer;
			DataRowView row = objContainer.DataItem as DataRowView;
			if ( row != null )
			{
				// 04/30/2006 Paul.  Use the Context to store pointers to the localization objects.
				// This is so that we don't need to require that the page inherits from SplendidPage. 
				L10N L10n = HttpContext.Current.Items["L10n"] as L10N;
				if ( L10n == null )
				{
					// 04/26/2006 Paul.  We want to have the AccessView on the SystemCheck page. 
					L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				}
				if ( L10n != null )
				{
					if ( row[sDATA_FIELD] != DBNull.Value )
					{
						string sList = sLIST_NAME;
						// 01/18/2007 Paul.  If AssignedUser list, then use the cached value to find the value. 
						if ( sLIST_NAME == "AssignedUser" )
						{
							lbl.Text = SplendidCache.AssignedUser(Sql.ToGuid(row[sDATA_FIELD]));
						}
						// 12/05/2005 Paul.  The activity status needs to be dynamically converted to the correct list. 
						else if ( sLIST_NAME == "activity_status" )
						{
							string sACTIVITY_TYPE = String.Empty;
							try
							{
								sACTIVITY_TYPE = Sql.ToString(row["ACTIVITY_TYPE"]);
								switch ( sACTIVITY_TYPE )
								{
									case "Tasks"   :  sList = "task_status_dom"   ;  break;
									case "Meetings":  sList = "meeting_status_dom";  break;
									case "Calls"   :
										// 07/15/2006 Paul.  Call status is translated externally. 
										lbl.Text = Sql.ToString(row[sDATA_FIELD]);
										return;
										//sList = "call_status_dom"   ;  break;
									case "Notes"   :
										// 07/15/2006 Paul.  Note Status is not normally as it does not have a status. 
										lbl.Text = L10n.Term(".activity_dom.Note");
										return;
									// 06/15/2006 Paul.  This list name for email_status does not follow the standard. 
									case "Emails"  :  sList = "dom_email_status"  ;  break;
									// 04/21/2006 Paul.  If the activity does not have a status (such as a Note), then use activity_dom. 
									default        :  sList = "activity_dom"      ;  break;
								}
							}
							catch(Exception ex)
							{
								SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
							}
							lbl.Text = Sql.ToString(L10n.Term("." + sList + ".", row[sDATA_FIELD]));
						}
						else
						{
							lbl.Text = Sql.ToString(L10n.Term("." + sList + ".", row[sDATA_FIELD]));
						}
					}
				}
				else
				{
					lbl.Text = Sql.ToString(row[sDATA_FIELD]);
				}
			}
			else
			{
				lbl.Text = sDATA_FIELD;
			}
		}
	}

	public class CreateItemTemplateHyperLink : ITemplate
	{
		protected string sDATA_FIELD;
		protected string sURL_FIELD ;
		protected string sURL_FORMAT;
		protected string sURL_TARGET;
		protected string sCSSCLASS  ;
		protected string sURL_MODULE;
		protected string sURL_ASSIGNED_FIELD;
		
		public CreateItemTemplateHyperLink(string sDATA_FIELD, string sURL_FIELD, string sURL_FORMAT, string sURL_TARGET, string sCSSCLASS, string sURL_MODULE, string sURL_ASSIGNED_FIELD)
		{
			this.sDATA_FIELD = sDATA_FIELD;
			this.sURL_FIELD  = sURL_FIELD ;
			this.sURL_FORMAT = sURL_FORMAT;
			this.sURL_TARGET = sURL_TARGET;
			this.sCSSCLASS   = sCSSCLASS  ;
			this.sURL_MODULE = sURL_MODULE;
			this.sURL_ASSIGNED_FIELD = sURL_ASSIGNED_FIELD;
		}
		public void InstantiateIn(Control objContainer)
		{
			HyperLink lnk = new HyperLink();
			lnk.Target   = sURL_TARGET;
			lnk.CssClass = sCSSCLASS  ;
			lnk.DataBinding += new EventHandler(OnDataBinding);
			objContainer.Controls.Add(lnk);
		}
		private void OnDataBinding(object sender, EventArgs e)
		{
			HyperLink lnk = (HyperLink)sender;
			DataGridItem objContainer = (DataGridItem) lnk.NamingContainer;
			DataRowView row = objContainer.DataItem as DataRowView;
			if ( row != null )
			{
				try
				{
					// 04/27/2006 Paul.  We need the module in order to determine if access is allowed. 
					Guid gASSIGNED_USER_ID = Guid.Empty;
					string sMODULE_NAME = sURL_MODULE;
					if ( row.DataView.Table.Columns.Contains(sURL_ASSIGNED_FIELD) )
					{
						gASSIGNED_USER_ID = Sql.ToGuid(row[sURL_ASSIGNED_FIELD]);
					}
					if ( row.DataView.Table.Columns.Contains(sDATA_FIELD) )
					{
						// 01/02/2008 Paul.  It is very important that we allow the text field to be set even when the URL_FIELD is NULL. Otherwise, nothing would be displayed.  
						// When displaying Products, we allow the display of order line items, but they are not linkable. 
						if ( row[sDATA_FIELD] != DBNull.Value )
						{
							lnk.Text = Sql.ToString(row[sDATA_FIELD]);
							
							if ( row[sURL_FIELD] != DBNull.Value )
							{
								bool bAllowed = false;
								// 04/27/2006 Paul.  Only provide the URL if access is allowed.
								// 08/28/2006 Paul.  The URL_FIELD might not always be a GUID.  iFrame uses a URL in this field. 
								// 08/28/2006 Paul.  MODULE_NAME is not always available.  In those cases, assume access is allowed. 
								string sURL_FIELD_VALUE = Sql.ToString(row[sURL_FIELD]);
								int nACLACCESS = ACL_ACCESS.ALL;
								if ( !Sql.IsEmptyString(sMODULE_NAME) )
									nACLACCESS = Security.GetUserAccess(sMODULE_NAME, "view");
								// 05/02/2006 Paul.  Admin has full access. 
								if ( Security.IS_ADMIN )
									bAllowed = true;
								else if ( nACLACCESS == ACL_ACCESS.OWNER )
								{
									// 05/02/2006 Paul.  Owner can only view if USER_ID matches the assigned user id. 
									// 05/02/2006 Paul.  This role also prevents the user from seeing unassigned items.  
									// This may or may not be a good thing. 
									if ( gASSIGNED_USER_ID == Security.USER_ID || Security.IS_ADMIN )
										bAllowed = true;
								}
								// 05/02/2006 Paul.  Allow access if the item is not assigned to anyone. 
								else if ( nACLACCESS >= 0 || Sql.IsEmptyGuid(gASSIGNED_USER_ID) )
									bAllowed = true;
								if ( bAllowed )
									lnk.NavigateUrl = String.Format(sURL_FORMAT, sURL_FIELD_VALUE.ToString());
							}
						}
					}
					else
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), sDATA_FIELD + " column does not exist in recordset.");
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
		}
	}

	// 07/26/2007 Paul.  PopupViews have special requirements.  They need an OnClick action that takes more than one parameter. 
	public class CreateItemTemplateHyperLinkOnClick : ITemplate
	{
		protected string sDATA_FIELD;
		protected string sURL_FIELD ;
		protected string sURL_FORMAT;
		protected string sURL_TARGET;
		protected string sCSSCLASS  ;
		protected string sURL_MODULE;
		protected string sURL_ASSIGNED_FIELD;
		
		public CreateItemTemplateHyperLinkOnClick(string sDATA_FIELD, string sURL_FIELD, string sURL_FORMAT, string sURL_TARGET, string sCSSCLASS, string sURL_MODULE, string sURL_ASSIGNED_FIELD)
		{
			this.sDATA_FIELD = sDATA_FIELD;
			this.sURL_FIELD  = sURL_FIELD ;
			this.sURL_FORMAT = sURL_FORMAT;
			this.sURL_TARGET = sURL_TARGET;
			this.sCSSCLASS   = sCSSCLASS  ;
			this.sURL_MODULE = sURL_MODULE;
			this.sURL_ASSIGNED_FIELD = sURL_ASSIGNED_FIELD;
		}
		public void InstantiateIn(Control objContainer)
		{
			HyperLink lnk = new HyperLink();
			lnk.Target   = sURL_TARGET;
			lnk.CssClass = sCSSCLASS  ;
			lnk.DataBinding += new EventHandler(OnDataBinding);
			objContainer.Controls.Add(lnk);
		}
		private void OnDataBinding(object sender, EventArgs e)
		{
			HyperLink lnk = (HyperLink)sender;
			DataGridItem objContainer = (DataGridItem) lnk.NamingContainer;
			DataRowView row = objContainer.DataItem as DataRowView;
			if ( row != null )
			{
				try
				{
					// 04/27/2006 Paul.  We need the module in order to determine if access is allowed. 
					Guid gASSIGNED_USER_ID = Guid.Empty;
					string sMODULE_NAME = sURL_MODULE;
					if ( row.DataView.Table.Columns.Contains(sURL_ASSIGNED_FIELD) )
					{
						gASSIGNED_USER_ID = Sql.ToGuid(row[sURL_ASSIGNED_FIELD]);
					}
					if ( row.DataView.Table.Columns.Contains(sDATA_FIELD) )
					{
						if ( row[sDATA_FIELD] != DBNull.Value )
						{
							lnk.Text = Sql.ToString(row[sDATA_FIELD]);
							
							bool bAllowed = false;
							string[] arrURL_FIELD = sURL_FIELD.Split(' ');
							object[] objURL_FIELD = new object[arrURL_FIELD.Length];
							for ( int i=0 ; i < arrURL_FIELD.Length; i++ )
							{
								if ( !Sql.IsEmptyString(arrURL_FIELD[i]) )
								{
									// 07/26/2007 Paul.  Make sure to escape the javascript string. 
									if ( row[arrURL_FIELD[i]] != DBNull.Value )
										objURL_FIELD[i] = Sql.EscapeJavaScript(Sql.ToString(row[arrURL_FIELD[i]]));
									else
										objURL_FIELD[i] = String.Empty;
								}
							}

							int nACLACCESS = ACL_ACCESS.ALL;
							if ( !Sql.IsEmptyString(sMODULE_NAME) )
								nACLACCESS = Security.GetUserAccess(sMODULE_NAME, "view");
							// 05/02/2006 Paul.  Admin has full access. 
							if ( Security.IS_ADMIN )
								bAllowed = true;
							else if ( nACLACCESS == ACL_ACCESS.OWNER )
							{
								// 05/02/2006 Paul.  Owner can only view if USER_ID matches the assigned user id. 
								// 05/02/2006 Paul.  This role also prevents the user from seeing unassigned items.  
								// This may or may not be a good thing. 
								if ( gASSIGNED_USER_ID == Security.USER_ID || Security.IS_ADMIN )
									bAllowed = true;
							}
							// 05/02/2006 Paul.  Allow access if the item is not assigned to anyone. 
							else if ( nACLACCESS >= 0 || Sql.IsEmptyGuid(gASSIGNED_USER_ID) )
								bAllowed = true;
							if ( bAllowed )
							{
								lnk.NavigateUrl = "#";
								lnk.Attributes.Add("onclick", String.Format(sURL_FORMAT, objURL_FIELD));
							}
						}
					}
					else
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), sDATA_FIELD + " column does not exist in recordset.");
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
		}
	}

	public class CreateItemTemplateImage : ITemplate
	{
		protected string sDATA_FIELD;
		protected string sCSSCLASS  ;
		
		public CreateItemTemplateImage(string sDATA_FIELD, string sCSSCLASS)
		{
			this.sDATA_FIELD = sDATA_FIELD;
			this.sCSSCLASS   = sCSSCLASS  ;
		}
		public void InstantiateIn(Control objContainer)
		{
			Image img = new Image();
			img.CssClass = sCSSCLASS  ;
			img.DataBinding += new EventHandler(OnDataBinding);
			objContainer.Controls.Add(img);
		}
		private void OnDataBinding(object sender, EventArgs e)
		{
			Image img = (Image)sender;
			DataGridItem objContainer = (DataGridItem) img.NamingContainer;
			DataRowView row = objContainer.DataItem as DataRowView;
			if ( row != null )
			{
				try
				{
					if ( row.DataView.Table.Columns.Contains(sDATA_FIELD) )
					{
						if ( row[sDATA_FIELD] != DBNull.Value && row[sDATA_FIELD] != DBNull.Value )
						{
							img.ImageUrl = "~/Images/Image.aspx?ID=" + Sql.ToString(row[sDATA_FIELD]);
						}
						else
						{
							// 04/13/2006 Paul.  Don't show the image control if there is no data to show. 
							img.Visible = false;
						}
					}
					else
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), sDATA_FIELD + " column does not exist in recordset.");
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				}
			}
		}
	}

	/// <summary>
	/// Summary description for SplendidGrid.
	/// </summary>
	public class SplendidGrid : System.Web.UI.WebControls.DataGrid
	{
		protected bool bTranslated = false;

		public SplendidGrid()
		{
			// http://www2.msdnaa.net/content/?ID=1267
			ItemCreated      += new DataGridItemEventHandler       (OnItemCreated     );
			PageIndexChanged += new DataGridPageChangedEventHandler(OnPageIndexChanged);
			SortCommand      += new DataGridSortCommandEventHandler(OnSort            );
		}

		// 11/12/2005 Paul.  Not sure why, but Unified Search/Project List is not translating. 
		public void L10nTranslate()
		{
			if ( !bTranslated )
			{
				// 04/30/2006 Paul.  Use the Context to store pointers to the localization objects.
				// This is so that we don't need to require that the page inherits from SplendidPage. 
				L10N L10n = HttpContext.Current.Items["L10n"] as L10N;
				if ( L10n == null )
				{
					// 04/26/2006 Paul.  We want to have the AccessView on the SystemCheck page. 
					L10n = new L10N(Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/CULTURE"]));
				}
				PagerStyle.PrevPageText = L10n.Term(PagerStyle.PrevPageText);
				PagerStyle.NextPageText = L10n.Term(PagerStyle.NextPageText);
				foreach(DataGridColumn col in Columns)
				{
					col.HeaderText = L10n.Term(col.HeaderText);
				}
				bTranslated = true;
			}
		}

		protected void OnItemCreated(object sender, DataGridItemEventArgs e)
		{
			// 08/21/2006 Lawrence Zamorano.  Add the record count to the pager control. 
			// 08/21/2006 Paul.  Enhance to include page range. 
			if ( e.Item.ItemType == ListItemType.Pager )
			{
				TableCell pgr = e.Item.Controls[0] as TableCell; 
				DataView vw = this.DataSource as DataView;
				if ( vw != null && vw.Count > 0 )
				{
					// 08/21/2006 Paul.  Grab references to the Prev and Next controls while we know their indexes. 
					// 08/21/2006 Paul.  The previous and next controls will either be a LinkButton if active, or a Label if inactive. 
					LinkButton lnkPrev = pgr.Controls[0] as LinkButton;
					LinkButton lnkNext = pgr.Controls[2] as LinkButton;
					Label      lblPrev = pgr.Controls[0] as Label;
					Label      lblNext = pgr.Controls[2] as Label;
					
					L10N L10n = HttpContext.Current.Items["L10n"] as L10N;
					string sOf = L10n.Term(".LBL_LIST_OF");
					int nPageStart = this.CurrentPageIndex * this.PageSize + 1;
					int nPageEnd   = Math.Min((this.CurrentPageIndex+1) * this.PageSize, vw.Count);
					LiteralControl litPageRange = new LiteralControl();
					litPageRange.Text = String.Format("&nbsp; <span class=\"pageNumbers\">({0} - {1} {2} {3})</span> ", nPageStart, nPageEnd, sOf, vw.Count);
					pgr.Controls.AddAt(1, litPageRange);

					string sThemeURL = Sql.ToString(HttpContext.Current.Session["themeURL"]);
					if ( lblPrev != null )
					{
						lblPrev.Text = "<img src=\"" + sThemeURL + "images/previous_off.gif" + "\" border=\"0\" height=\"10\" width=\"6\" />&nbsp;" + lblPrev.Text;
					}
					if ( lnkPrev != null )
					{
						lnkPrev.Text = "<img src=\"" + sThemeURL + "images/previous.gif" + "\" border=\"0\" height=\"10\" width=\"6\" />&nbsp;" + lnkPrev.Text;
						//LinkButton lnkStart = new LinkButton();
						//lnkStart.CommandArgument = "1";
						//lnkStart.CommandName = "Page";
						//lnkStart.Text = "<img src=\"" + sThemeURL + "images/start.gif" + "\" border=\"0\" height=\"10\" width=\"11\" />&nbsp;" + L10n.Term(".LNK_LIST_START") + "&nbsp;";
						//pgr.Controls.AddAt(0, lnkStart);
					}
					if ( lblNext != null )
					{
						lblNext.Text = lblNext.Text + "&nbsp;<img src=\"" + sThemeURL + "images/next_off.gif" + "\" border=\"0\" height=\"10\" width=\"6\" />";
					}
					if ( lnkNext != null )
					{
						lnkNext.Text = lnkNext.Text + "&nbsp;<img src=\"" + sThemeURL + "images/next.gif" + "\" border=\"0\" height=\"10\" width=\"6\" />";
						//LinkButton lnkEnd = new LinkButton();
						//lnkEnd.CommandArgument = this.PageCount.ToString();
						//lnkEnd.CommandName = "Page";
						//lnkEnd.Text = "&nbsp;" + L10n.Term(".LNK_LIST_END") + "&nbsp;<img src=\"" + sThemeURL + "images/end.gif" + "\" border=\"0\" height=\"10\" width=\"11\" />";
						//pgr.Controls.Add(lnkEnd);
					}
				}
			}
			else if ( e.Item.ItemType == ListItemType.Header )
			{
				// 06/09/2006 Paul.  Move the translation to overridden DataBind. 
				//L10nTranslate();
				// 11/21/2005 Paul.  The header cells should never wrap, the background image was not designed to wrap. 
				foreach(TableCell cell in e.Item.Cells)
				{
					cell.Wrap = false;
				}
				HttpSessionState Session = HttpContext.Current.Session;
				string sLastSortColumn = (string)ViewState["LastSortColumn"];
				string sLastSortOrder  = (string)ViewState["LastSortOrder" ];
				// 08/28/2006 Paul.  We need to watch for overflow.  This has occurred when a grid was created with no columns. 
				for(int i = 0 ; i < e.Item.Controls.Count && i < this.Columns.Count ; i++ )
				{
					// 11/13/2005 Paul.  If sorting is not enabled, this code will cause the header text to disappear. 
					if ( this.AllowSorting && !Sql.IsEmptyString(Columns[i].SortExpression) )
					{
						Image img = new Image();
						img.Width  =  8;
						img.Height = 10;
						if ( Columns[i].SortExpression == sLastSortColumn )
						{
							if ( sLastSortOrder == "asc" )
								img.ImageUrl = Session["themeURL"] + "images/arrow_down.gif";
							else
								img.ImageUrl = Session["themeURL"] + "images/arrow_up.gif";
						}
						else
						{
							img.ImageUrl = Session["themeURL"] + "images/arrow.gif";
						}
						Literal lit = new Literal();
						lit.Text = "&nbsp;";
						e.Item.Cells[i].Controls.Add(lit);
						e.Item.Cells[i].Controls.Add(img);
					}
				}
			}
			else if ( e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem )
			{
				// 09/05/2005 Paul.  Reducing viewstate data in a table can be done at the row level. 
				// This will provide a major performance benefit while not loosing the ability to sort a grid. 
				// http://authors.aspalliance.com/jimross/Articles/DatagridDietPartTwo.aspx
				// 10/13/2005 Paul.  Can't disable the content otherwise the data is not retained during certain postback operations. 
				//e.Item.EnableViewState = false;
			}
		}

		protected void OnPageIndexChanged(Object sender, DataGridPageChangedEventArgs e)
		{
			// Set CurrentPageIndex to the page the user clicked.
			CurrentPageIndex = e.NewPageIndex;
			ApplySort();
			DataBind();
		}

		protected void OnSort(object sender, DataGridSortCommandEventArgs e)
		{
			string sNewSortColumn  = e.SortExpression.ToString();
			string sNewSortOrder   = "asc"; // default
			string sLastSortColumn = (string)ViewState["LastSortColumn"];
			string sLastSortOrder  = (string)ViewState["LastSortOrder" ];
			if ( sNewSortColumn.Equals(sLastSortColumn) && sLastSortOrder.Equals("asc") )
			{
				sNewSortOrder= "desc";
			}
			ViewState["LastSortColumn"] = sNewSortColumn;
			ViewState["LastSortOrder" ] = sNewSortOrder;

			ApplySort();
			EditItemIndex     = -1;
			CurrentPageIndex  = 0; // goto first page
			DataBind();
		}

		// 06/09/2006 Paul.  Now that we have removed all the data binding code in controls, 
		// we are back to having a problem with the translations.  
		public override void DataBind()
		{
			L10nTranslate();
			base.DataBind();
			// 10/04/2006 Paul.  Use the config value to set the page size. 
			// We need to do this inside DataBind as AllowPaging is typically updated in the ASCX, and only applies when bound. 
			if ( this.AllowPaging )
			{
				int nPageSize = Sql.ToInteger(Page.Application["CONFIG.list_max_entries_per_page"]);
				if ( nPageSize > 0 )
				{
					this.PageSize = nPageSize;
				}
			}
		}

		public void ApplySort()
		{
			string sLastSortColumn = (string)ViewState["LastSortColumn"];
			string sLastSortOrder  = (string)ViewState["LastSortOrder" ];
			DataView vw = (DataView) DataSource ;
			if ( vw != null && !Sql.IsEmptyString(sLastSortColumn) )
				vw.Sort = sLastSortColumn + " " + sLastSortOrder;
			// 11/12/2005 Paul.  Not sure why, but Unified Search/Project List is not translating. 
			// 06/09/2006 Paul.  Now that we have overridden DataBind, there is no need to translate here. 
			//L10nTranslate();
		}

		// 12/14/2007 Paul.  We need an easy way to capture the sort event from the SearchView. 
		public void SetSortFields(string[] arrSort)
		{
			if ( arrSort != null )
			{
				if ( arrSort.Length == 2 )
				{
					if ( !Sql.IsEmptyString(arrSort[0]) && !Sql.IsEmptyString(arrSort[1]) )
					{
						SortColumn = arrSort[0];
						SortOrder  = arrSort[1];
					}
				}
			}
			else
			{
				// 12/17/2007 Paul.  Clear the sort when NULL provided. 
				ViewState.Remove("LastSortColumn");
				ViewState.Remove("LastSortOrder" );
			}
		}

		public string SortColumn
		{
			get
			{
				return Sql.ToString(ViewState["LastSortColumn"]);
			}
			set
			{
				ViewState["LastSortColumn"] = value;
			}
		}

		public string SortOrder
		{
			get
			{
				return Sql.ToString(ViewState["LastSortOrder"]);
			}
			set
			{
				ViewState["LastSortOrder"] = value;
			}
		}

		// 11/19/2007 Paul.  Restore old name to reduce upgrade pains. 
		public void DynamicColumns(string sGRID_NAME)
		{
			SplendidDynamic.AppendGridColumns(sGRID_NAME, this);
		}

		public void AppendGridColumns(string sGRID_NAME)
		{
			SplendidDynamic.AppendGridColumns(sGRID_NAME, this);
		}
	}
}

