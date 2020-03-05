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
using System.Data.Common;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Emails
{
	/// <summary>
	/// Summary description for PopupEmailAddresses.
	/// </summary>
	public class PopupEmailAddresses : SplendidPage
	{
		protected SplendidCRM._controls.SearchView   ctlSearchView  ;

		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected Label         lblError       ;

		protected void Page_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if ( e.CommandName == "Search" )
				{
					// 10/13/2005 Paul.  Make sure to clear the page index prior to applying search. 
					grdMain.CurrentPageIndex = 0;
					grdMain.ApplySort();
					grdMain.DataBind();
				}
				// 12/14/2007 Paul.  We need to capture the sort event from the SearchView. 
				else if ( e.CommandName == "SortGrid" )
				{
					grdMain.SetSortFields(e.CommandArgument as string[]);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term("Contacts.LBL_LIST_FORM_TITLE"));
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					try
					{
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dtCombined = new DataTable() )
							{
								// 12/19/2006 Paul.  As much as we would like to combine the threee separate queries into 
								// a single query using a union, we cannot because the Security.Filter rules must be applied separately. 
								// We simply combine three DataTables as quickly and efficiently as possible. 
								cmd.CommandText = "select *                           " + ControlChars.CrLf
								                + "     , N'Contacts'  as ADDRESS_TYPE" + ControlChars.CrLf
								                + "  from vwCONTACTS_EmailList        " + ControlChars.CrLf;
								Security.Filter(cmd, "Contacts", "list");
								ctlSearchView.SqlSearchClause(cmd);
								if ( bDebug )
									Page.ClientScript.RegisterClientScriptBlock(System.Type.GetType("System.String"), "vwCONTACTS_EmailList", Sql.ClientScriptBlock(cmd));
								da.Fill(dtCombined);
								
								cmd.Parameters.Clear();
								cmd.CommandText = "select *                           " + ControlChars.CrLf
								                + "     , N'Leads'     as ADDRESS_TYPE" + ControlChars.CrLf
								                + "  from vwLEADS_EmailList           " + ControlChars.CrLf;
								Security.Filter(cmd, "Leads", "list");
								ctlSearchView.SqlSearchClause(cmd);
								if ( bDebug )
									Page.ClientScript.RegisterClientScriptBlock(System.Type.GetType("System.String"), "vwLEADS_EmailList", Sql.ClientScriptBlock(cmd));
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									foreach ( DataRow row in dt.Rows)
									{
										DataRow rowNew = dtCombined.NewRow();
										//rowNew.ItemArray = row.ItemArray;
										// 12/19/2006 Paul.  Using the ItemArray would certainly be faster,
										// but someone may accidentally modify one of the columns of the three views, 
										// so we shall be safe and check each column before setting its value. 
										foreach ( DataColumn col in dt.Columns )
										{
											if ( dtCombined.Columns.Contains(col.ColumnName) )
											{
												rowNew[col.ColumnName] = row[col.ColumnName];
											}
										}
										dtCombined.Rows.Add(rowNew);
									}
								}
								
								cmd.Parameters.Clear();
								cmd.CommandText = "select *                           " + ControlChars.CrLf
								                + "     , N'Prospects' as ADDRESS_TYPE" + ControlChars.CrLf
								                + "  from vwPROSPECTS_EmailList       " + ControlChars.CrLf;
								Security.Filter(cmd, "Prospects", "list");
								ctlSearchView.SqlSearchClause(cmd);
								if ( bDebug )
									Page.ClientScript.RegisterClientScriptBlock(System.Type.GetType("System.String"), "vwPROSPECTS_EmailList", Sql.ClientScriptBlock(cmd));
								using ( DataTable dt = new DataTable() )
								{
									da.Fill(dt);
									foreach ( DataRow row in dt.Rows)
									{
										DataRow rowNew = dtCombined.NewRow();
										//rowNew.ItemArray = row.ItemArray;
										// 12/19/2006 Paul.  Using the ItemArray would certainly be faster,
										// but someone may accidentally modify one of the columns of the three views, 
										// so we shall be safe and check each column before setting its value. 
										foreach ( DataColumn col in dt.Columns )
										{
											if ( dtCombined.Columns.Contains(col.ColumnName) )
											{
												rowNew[col.ColumnName] = row[col.ColumnName];
											}
										}
										dtCombined.Rows.Add(rowNew);
									}
								}
								
								vwMain = dtCombined.DefaultView;
								grdMain.DataSource = vwMain ;
								if ( !IsPostBack )
								{
									// 12/14/2007 Paul.  Only set the default sort if it is not already set.  It may have been set by SearchView. 
									if ( String.IsNullOrEmpty(grdMain.SortColumn) )
									{
										grdMain.SortColumn = "NAME";
										grdMain.SortOrder  = "asc" ;
									}
									grdMain.ApplySort();
									grdMain.DataBind();
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						lblError.Text = ex.Message;
					}
				}
			}
			if ( !IsPostBack )
			{
				Page.DataBind();
			}
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
			ctlSearchView.Command = new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
