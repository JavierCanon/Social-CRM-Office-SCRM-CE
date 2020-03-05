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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Diagnostics;
//using Microsoft.VisualBasic;
using SplendidCRM._controls;

namespace SplendidCRM.Administration.DynamicLayout
{
	/// <summary>
	/// Summary description for DynamicLayoutView.
	/// </summary>
	public class DynamicLayoutView : SplendidControl
	{
		protected ModuleHeader            ctlModuleHeader ;
		protected _controls.LayoutButtons ctlLayoutButtons;
		protected _controls.SearchBasic   ctlSearch       ;
		protected SplendidCRM._controls.ListHeader ctlListHeader;

		private   NewRecord               ctlNewRecord    ;
		protected HtmlTable               tblMain         ;
		// 11/30/2006 Paul.  Not sure why, but we are having a problem with loading the viewstate. 
		// Store the fields in a hidden variable so that it is accessible inside Page_Init. 
		protected HtmlInputHidden         txtFieldState   ;
		// 11/30/2006 Paul.  Make the dtFields member so that it only needs to be loaded once. 
		protected DataTable               dtFields = null;

		protected void SaveFieldState()
		{
			using ( DataSet ds = new DataSet() )
			{
				ds.Tables.Add(dtFields);
				using ( MemoryStream mem = new MemoryStream() )
				{
					XmlTextWriter xw = new XmlTextWriter(mem, System.Text.Encoding.UTF8);
					ds.WriteXml(xw, System.Data.XmlWriteMode.WriteSchema);
					xw.Flush();
					txtFieldState.Value = Convert.ToBase64String(mem.ToArray());
				}
				ds.Tables.Remove(dtFields);
			}
		}

		protected void LoadFieldState()
		{
			dtFields = null;
			// 11/30/2006 Paul.  Pull the field state directly from the request so that this will work even before viewstate has been restored. 
			string sFieldState = Sql.ToString(Request.Form[txtFieldState.Name]);
			if ( !Sql.IsEmptyString(sFieldState) )
			{
				using ( DataSet ds = new DataSet() )
				{
					byte[] by = Convert.FromBase64String(sFieldState);
					using ( MemoryStream mem = new MemoryStream(by) )
					{
						XmlTextReader xr = new XmlTextReader(mem);
						ds.ReadXml(xr, System.Data.XmlReadMode.ReadSchema);
					}
					dtFields = ds.Tables[0];
					ds.Tables.Remove(dtFields);
				}
			}
		}

		// 01/09/2006 Paul.  Instead of creating an abstract class, just create virtual members.
		// VisualStudio 2003 gave errors when trying to load a class that was based on an abstract class. 
		#region Virtual Functions
		protected virtual string LayoutTableName()
		{
			return String.Empty;
		}

		protected virtual string LayoutIndexName()
		{
			return String.Empty;
		}

		protected virtual string LayoutTypeName()
		{
			return String.Empty;
		}

		protected virtual string LayoutUpdateProcedure()
		{
			return String.Empty;
		}
		
		protected virtual string LayoutDeleteProcedure()
		{
			return String.Empty;
		}

		protected virtual void LayoutView_Bind()
		{
		}

		protected virtual void GetLayoutFields(string sNAME)
		{
		}

		protected virtual void GetModuleName(string sNAME, ref string sMODULE_NAME, ref string sVIEW_NAME)
		{
		}

		protected virtual void ClearCache(string sNAME)
		{
		}
		#endregion

		#region Dynamic Table Management
		public int DynamicTableNewFieldIndex()
		{
			int nFieldIndex = 0;
			DataView vwFields = new DataView(dtFields);
			vwFields.RowFilter = "DELETED = 0"     ;
			vwFields.Sort      = LayoutIndexName() + " desc";
			foreach(DataRowView row in vwFields)
			{
				// 01/08/2006 Paul.  Only count records that are not deleted. 
				if ( Sql.ToInteger(row["DELETED"]) == 0 )
				{
					nFieldIndex = Sql.ToInteger(row[LayoutIndexName()]) + 1;
				}
				break;
			}
			return nFieldIndex;
		}

		public void DynamicTableDelete(int nFieldIndex)
		{
			bool bDecrementIndex = false;
			foreach(DataRow row in dtFields.Rows)
			{
				// 01/08/2006 Paul.  Only modify records that are not deleted. 
				if ( Sql.ToInteger(row["DELETED"]) == 0 )
				{
					if ( Sql.ToInteger(row[LayoutIndexName()]) == nFieldIndex )
					{
						row["DELETED"] = 1;
						bDecrementIndex = true;
					}
					else if ( bDecrementIndex )
					{
						row[LayoutIndexName()] = Sql.ToInteger(row[LayoutIndexName()]) - 1;
					}
				}
			}
		}

		public void DynamicTableInsert(int nFieldIndex)
		{
			// 01/08/2006 Paul.  Insert just makes space by shifting the indexes up. 
			DataView vwFields = new DataView(dtFields);
			vwFields.RowFilter = "DELETED = 0"     ;
			vwFields.Sort      = LayoutIndexName() + " desc";
			foreach(DataRowView row in vwFields)
			{
				// 01/08/2006 Paul.  Only modify records that are not deleted. 
				if ( Sql.ToInteger(row[LayoutIndexName()]) >= nFieldIndex )
				{
					row[LayoutIndexName()] = Sql.ToInteger(row[LayoutIndexName()]) + 1;
				}
			}
		}

		public void DynamicTableMoveUp(int nFieldIndex, int nRowMinimum)
		{
			//01/07/2006 Paul.  Move up means to decrement. 
			if ( nFieldIndex > nRowMinimum )
			{
				// 01/08/2006 Paul.  Only modify records that are not deleted. 
				foreach(DataRow row in dtFields.Rows)
				{
					if ( Sql.ToInteger(row["DELETED"]) == 0 )
					{
						if ( Sql.ToInteger(row[LayoutIndexName()]) == nFieldIndex )
						{
							row[LayoutIndexName()] = nFieldIndex - 1;
						}
						else if ( Sql.ToInteger(row[LayoutIndexName()]) == nFieldIndex-1 )
						{
							row[LayoutIndexName()] = nFieldIndex;
						}
					}
				}
			}
		}

		public void DynamicTableMoveDown(int nFieldIndex)
		{
			//01/07/2006 Paul.  Move down means to increment. 
			if ( nFieldIndex < dtFields.Rows.Count )
			{
				// 01/08/2006 Paul.  Only modify records that are not deleted. 
				foreach(DataRow row in dtFields.Rows)
				{
					if ( Sql.ToInteger(row["DELETED"]) == 0 )
					{
						if ( Sql.ToInteger(row[LayoutIndexName()]) == nFieldIndex )
						{
							row[LayoutIndexName()] = nFieldIndex + 1;
						}
						else if ( Sql.ToInteger(row[LayoutIndexName()]) == nFieldIndex+1 )
						{
							row[LayoutIndexName()] = nFieldIndex;
						}
					}
				}
			}
		}
		#endregion

		protected virtual void Page_Command(Object sender, CommandEventArgs e)
		{
			try
			{
				//ctlLayoutButtons.ErrorText = e.CommandName + ": " + e.CommandArgument.ToString();
				if ( e.CommandName == "Layout.Delete" )
				{
					int nFieldIndex = Sql.ToInteger(e.CommandArgument);
					DynamicTableDelete(nFieldIndex);
					SaveFieldState();
					LayoutView_Bind();
					if ( ctlNewRecord != null )
						ctlNewRecord.Clear();
				}
				else if ( e.CommandName == "Layout.MoveUp" )
				{
					int nFieldIndex = Sql.ToInteger(e.CommandArgument);
					int nRowMinimum = Sql.ToInteger(ViewState["ROW_MINIMUM"]);
					DynamicTableMoveUp(nFieldIndex, nRowMinimum);
					SaveFieldState();
					LayoutView_Bind();
					if ( ctlNewRecord != null )
						ctlNewRecord.Clear();
				}
				else if ( e.CommandName == "Layout.MoveDown" )
				{
					int nFieldIndex = Sql.ToInteger(e.CommandArgument);
					DynamicTableMoveDown(nFieldIndex);
					SaveFieldState();
					LayoutView_Bind();
					if ( ctlNewRecord != null )
						ctlNewRecord.Clear();
				}
				else if ( e.CommandName == "Layout.Edit" )
				{
				}
				else if ( e.CommandName == "NewRecord.Save" )
				{
				}
				else if ( e.CommandName == "NewRecord.Cancel" )
				{
					if ( ctlNewRecord != null )
						ctlNewRecord.Clear();
				}
				else if ( e.CommandName == "Layout.Insert" )
				{
					if ( ctlNewRecord != null )
					{
						ctlNewRecord.Clear();
						int nFieldIndex = Sql.ToInteger(e.CommandArgument);
						ctlNewRecord.FIELD_ID    = Guid.NewGuid();
						ctlNewRecord.FIELD_INDEX = nFieldIndex;
						ctlNewRecord.Visible = true;
					}
				}
				else if ( e.CommandName == "New" )
				{
					if ( ctlNewRecord != null )
					{
						ctlNewRecord.Clear();
						int nFieldIndex = Sql.ToInteger(e.CommandArgument);
						ctlNewRecord.FIELD_ID    = Guid.NewGuid();
						ctlNewRecord.Visible = true;
					}
				}
				else if ( e.CommandName == "Defaults" )
				{
				}
				else if ( e.CommandName == "Save" )
				{
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						using ( IDbTransaction trn = con.BeginTransaction() )
						{
							try
							{
								IDbCommand cmdUpdate = SqlProcs.Factory(con, LayoutUpdateProcedure());
								cmdUpdate.Transaction = trn;
								// 10/10/2006 Paul.  Use IDbDataParameter to be consistent. 
								foreach(IDbDataParameter par in cmdUpdate.Parameters)
								{
									par.Value = DBNull.Value;
								}
								IDbDataParameter parMODIFIED_USER_ID = Sql.FindParameter(cmdUpdate, "@MODIFIED_USER_ID");
								if ( parMODIFIED_USER_ID != null )
									parMODIFIED_USER_ID.Value = Security.USER_ID;
						
								DataView vwFields = new DataView(dtFields);
								vwFields.RowFilter = "DELETED = 0";
								foreach(DataRowView row in vwFields)
								{
									if ( row.Row.RowState == DataRowState.Modified || row.Row.RowState == DataRowState.Added )
									{
										// 10/10/2006 Paul.  Use IDbDataParameter to be consistent. 
										foreach(IDbDataParameter par in cmdUpdate.Parameters)
										{
											string sFieldName = Sql.ExtractDbName(cmdUpdate, par.ParameterName);
											if ( dtFields.Columns.Contains(sFieldName) && (sFieldName != "MODIFIED_USER_ID") )
											{
												// 01/09/2006 Paul.  Make sure to use ToDBString to convert empty stings to NULL. 
												switch ( par.DbType )
												{
													case DbType.Guid    :  par.Value = Sql.ToGuid      (row[sFieldName]);  break;
													case DbType.Int16   :  par.Value = Sql.ToDBInteger (row[sFieldName]);  break;
													case DbType.Int32   :  par.Value = Sql.ToDBInteger (row[sFieldName]);  break;
													case DbType.Int64   :  par.Value = Sql.ToDBInteger (row[sFieldName]);  break;
													case DbType.Double  :  par.Value = Sql.ToDBFloat   (row[sFieldName]);  break;
													case DbType.Decimal :  par.Value = Sql.ToDBDecimal (row[sFieldName]);  break;
													case DbType.Byte    :  par.Value = Sql.ToDBBoolean (row[sFieldName]);  break;
													case DbType.DateTime:  par.Value = Sql.ToDBDateTime(row[sFieldName]);  break;
													default             :  par.Value = Sql.ToDBString  (row[sFieldName]);  break;
												}
											}
										}
										cmdUpdate.ExecuteNonQuery();
									}
								}
						
								IDbCommand cmdDelete = SqlProcs.Factory(con, LayoutDeleteProcedure());
								cmdDelete.Transaction = trn;
								IDbDataParameter parID = Sql.FindParameter(cmdDelete, "@ID");
								parMODIFIED_USER_ID = Sql.FindParameter(cmdDelete, "@MODIFIED_USER_ID");
								if ( parMODIFIED_USER_ID != null )
									parMODIFIED_USER_ID.Value = Security.USER_ID;
						
								vwFields.RowFilter = "DELETED = 1";
								foreach(DataRowView row in vwFields)
								{
									parID.Value = Sql.ToDBGuid(row["ID"]);
									cmdDelete.ExecuteNonQuery();
								}
								trn.Commit();
								// 01/09/2006 Paul.  Make sure to clear the cache so that the changes will take effect immediately. 
								ClearCache(Sql.ToString(ViewState["LAYOUT_VIEW_NAME"]));
								Response.Redirect("default.aspx");
							}
							catch(Exception ex)
							{
								trn.Rollback();
								throw(new Exception("Failed to update, transaction aborted; " + ex.Message, ex));
							}
						}
					}
				}
				else if ( e.CommandName == "Cancel" )
				{
					Response.Redirect("default.aspx");
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlLayoutButtons.ErrorText = ex.Message;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				// 01/08/2006 Paul.  The viewstate is no longer disabled, so we can go back to using ctlSearch.NAME.
				string sNAME = ctlSearch.NAME;  //Sql.ToString(Request[ctlSearch.ListUniqueID]);
				ctlSearch     .Visible = Sql.IsEmptyString(sNAME);
				ctlLayoutButtons.Visible = !ctlSearch.Visible;
				// 09/08/2007 Paul.  Add a list header so we will know what list we are working on. 
				if ( ctlListHeader != null )
				{
					ctlListHeader.Visible = !ctlSearch.Visible;
					ctlListHeader.Title   = sNAME;
				}

				if ( !Sql.IsEmptyString(sNAME) && sNAME != Sql.ToString(ViewState["LAYOUT_VIEW_NAME"]) )
				{
					// 01/08/2006 Paul.  We are having a problem with the ViewState not loading properly.
					// This problem only seems to occur when the NewRecord is visible and we try and load a different view.
					// The solution seems to be to hide the Search dialog so that the user must Cancel out of editing the current view.
					// This works very well to clear the ViewState because we GET the next page instead of POST to it. 
					
					SetPageTitle(sNAME);
					Page.DataBind();
					tblMain.EnableViewState = false;

					string sMODULE_NAME = String.Empty;
					string sVIEW_NAME   = String.Empty;
					GetModuleName(sNAME, ref sMODULE_NAME, ref sVIEW_NAME);
					GetLayoutFields(sNAME);
					LayoutView_Bind();

					ViewState["MODULE_NAME"     ] = sMODULE_NAME;
					ViewState["VIEW_NAME"       ] = sVIEW_NAME  ;
					ViewState["LAYOUT_VIEW_NAME"] = sNAME       ;
					SaveFieldState();
					if ( dtFields.Rows.Count > 0 )
					{
						ViewState["ROW_MINIMUM"] = dtFields.Rows[0][LayoutIndexName()];
					}
					else
					{
						ViewState["ROW_MINIMUM"] = 0;
					}
				}
				// 02/08/2007 Paul.  The NewRecord control is now in the MasterPage. 
				ContentPlaceHolder plcSidebar = Page.Master.FindControl("cntSidebar") as ContentPlaceHolder;
				if ( plcSidebar != null )
				{
					ctlNewRecord = plcSidebar.FindControl("ctlNewRecord") as NewRecord;
					if ( ctlNewRecord != null )
					{
						if ( Sql.IsEmptyString(sNAME) )
						{
							ctlNewRecord.Clear();
						}
						else
						{
							ctlNewRecord.MODULE_NAME = Sql.ToString(ViewState["MODULE_NAME"]);
							ctlNewRecord.VIEW_NAME   = Sql.ToString(ViewState["VIEW_NAME"  ]);
						}
					}
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlLayoutButtons.ErrorText = ex.Message;
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
			if ( IsPostBack )
			{
				// 11/30/2006 Paul.  We were having a problem with viewstate. Make sure to load the fields inside Page_Init. 
				LoadFieldState();
				LayoutView_Bind();
			}
		}
		#endregion
	}
}
