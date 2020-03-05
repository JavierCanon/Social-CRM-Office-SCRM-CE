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

namespace SplendidCRM.Contacts
{
	/// <summary>
	///		Summary description for ListView.
	/// </summary>
	public class ListView : SplendidControl
	{
		protected _controls.ExportHeader ctlExportHeader;
		protected _controls.SearchView   ctlSearchView  ;

		protected DataView      vwMain         ;
		protected SplendidGrid  grdMain        ;
		protected Label         lblError       ;
		protected MassUpdate    ctlMassUpdate  ;

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
				else if ( e.CommandName == "MassUpdate" )
				{
					string[] arrID = Request.Form.GetValues("chkMain");
					if ( arrID != null )
					{
						// 10/26/2007 Paul.  Use a stack to run the update in blocks of under 200 IDs. 
						//string sIDs = Utils.ValidateIDs(arrID);
						System.Collections.Stack stk = Utils.FilterByACL_Stack(m_sMODULE, "edit", arrID, "CONTACTS");
						if ( stk.Count > 0 )
						{
							DbProviderFactory dbf = DbProviderFactories.GetFactory();
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								using ( IDbTransaction trn = con.BeginTransaction() )
								{
									try
									{
										while ( stk.Count > 0 )
										{
											string sIDs = Utils.BuildMassIDs(stk);
											// 09/11/2007 Paul.  Mass update of teams is now available. 
											SqlProcs.spCONTACTS_MassUpdate(sIDs, ctlMassUpdate.ASSIGNED_USER_ID, ctlMassUpdate.ACCOUNT_ID, ctlMassUpdate.LEAD_SOURCE, ctlMassUpdate.REPORTS_TO_ID, ctlMassUpdate.TEAM_ID, trn);
										}
										trn.Commit();
									}
									catch(Exception ex)
									{
										trn.Rollback();
										throw(new Exception(ex.Message, ex.InnerException));
									}
								}
							}
							Response.Redirect("default.aspx");
						}
					}
				}
				else if ( e.CommandName == "MassDelete" )
				{
					string[] arrID = Request.Form.GetValues("chkMain");
					if ( arrID != null )
					{
						// 10/26/2007 Paul.  Use a stack to run the update in blocks of under 200 IDs. 
						//string sIDs = Utils.ValidateIDs(arrID);
						System.Collections.Stack stk = Utils.FilterByACL_Stack(m_sMODULE, "delete", arrID, "CONTACTS");
						if ( stk.Count > 0 )
						{
							DbProviderFactory dbf = DbProviderFactories.GetFactory();
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								con.Open();
								using ( IDbTransaction trn = con.BeginTransaction() )
								{
									try
									{
										while ( stk.Count > 0 )
										{
											string sIDs = Utils.BuildMassIDs(stk);
											SqlProcs.spCONTACTS_MassDelete(sIDs, trn);
										}
										trn.Commit();
									}
									catch(Exception ex)
									{
										trn.Rollback();
										throw(new Exception(ex.Message, ex.InnerException));
									}
								}
							}
							Response.Redirect("default.aspx");
						}
					}
				}
				else if ( e.CommandName == "Sync" )
				{
					string[] arrID = Request.Form.GetValues("chkMain");
					if ( arrID != null )
					{
						string sIDs = Utils.ValidateIDs(arrID);
						if ( !Sql.IsEmptyString(sIDs) )
						{
							SqlProcs.spCONTACTS_MassSync(sIDs);
							Response.Redirect("default.aspx");
						}
					}
				}
				else if ( e.CommandName == "Unsync" )
				{
					string[] arrID = Request.Form.GetValues("chkMain");
					if ( arrID != null )
					{
						string sIDs = Utils.ValidateIDs(arrID);
						if ( !Sql.IsEmptyString(sIDs) )
						{
							SqlProcs.spCONTACTS_MassUnsync(sIDs);
							Response.Redirect("default.aspx");
						}
					}
				}
				else if ( e.CommandName == "Export" )
				{
					// 11/03/2006 Paul.  Apply ACL rules to Export. 
					int nACLACCESS = SplendidCRM.Security.GetUserAccess(m_sMODULE, "export");
					if ( nACLACCESS  >= 0 )
					{
						if ( nACLACCESS == ACL_ACCESS.OWNER )
							vwMain.RowFilter = "ASSIGNED_USER_ID = '" + Security.USER_ID.ToString() + "'";
						string[] arrID = Request.Form.GetValues("chkMain");
						SplendidExport.Export(vwMain, m_sMODULE, ctlExportHeader.ExportFormat, ctlExportHeader.ExportRange, grdMain.CurrentPageIndex, grdMain.PageSize, arrID);
					}
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
			SetPageTitle(L10n.Term(m_sMODULE + ".LBL_LIST_FORM_TITLE"));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "list") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				if ( this.IsMobile && grdMain.Columns.Count > 0 )
					grdMain.Columns[0].Visible = false;
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select *              " + ControlChars.CrLf
					     + "  from vwCONTACTS_List" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
						Security.Filter(cmd, m_sMODULE, "list");
						ctlSearchView.SqlSearchClause(cmd);

						if ( bDebug )
							RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(dt);
								vwMain = dt.DefaultView;
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
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
			}
			if ( !IsPostBack )
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This Contact is required by the ASP.NET Web Form Designer.
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
			// 11/26/2005 Paul.  Add fields early so that sort events will get called. 
			m_sMODULE = "Contacts";
			SetMenu(m_sMODULE);
			this.AppendGridColumns(grdMain, m_sMODULE + ".ListView");
			ctlSearchView.Command = new CommandEventHandler(Page_Command);
			ctlExportHeader.Command = new CommandEventHandler(Page_Command);
			ctlMassUpdate.Command = new CommandEventHandler(Page_Command);
			// 05/02/2006 Paul.  Hide the MassUpdate control if the user cannot make changes. 
			if ( Security.GetUserAccess(m_sMODULE, "delete") < 0 && Security.GetUserAccess(m_sMODULE, "edit") < 0 )
				ctlMassUpdate.Visible = false;

		}
		#endregion
	}
}
