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
//using Microsoft.VisualBasic;

namespace SplendidCRM.Bugs
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.ModuleHeader ctlModuleHeader;
		protected _controls.EditButtons  ctlEditButtons ;

		protected Guid            gID                          ;
		protected HtmlTable       tblMain                      ;

		public static void LoadFile(Guid gID, Stream stm, IDbTransaction trn)
		{
			const int BUFFER_LENGTH = 4*1024;
			byte[] binFILE_POINTER = new byte[16];
			// 01/20/2006 Paul.  Must include in transaction
			SqlProcs.spBUGS_ATTACHMENT_InitPointer(gID, ref binFILE_POINTER, trn);
			using ( BinaryReader reader = new BinaryReader(stm) )
			{
				int nFILE_OFFSET = 0 ;
				byte[] binBYTES = reader.ReadBytes(BUFFER_LENGTH);
				while ( binBYTES.Length > 0 )
				{
					// 08/14/2005 Paul.  gID is used by Oracle, binFILE_POINTER is used by SQL Server. 
					SqlProcs.spBUGS_ATTACHMENT_WriteOffset(gID, binFILE_POINTER, nFILE_OFFSET, binBYTES, trn);
					nFILE_OFFSET += binBYTES.Length;
					binBYTES = reader.ReadBytes(BUFFER_LENGTH);
				}
			}
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			// 08/21/2005 Paul.  Redirect to parent if that is where the note was originated. 
			Guid   gPARENT_ID   = Sql.ToGuid(Request["PARENT_ID"]);
			string sMODULE      = String.Empty;
			string sPARENT_TYPE = String.Empty;
			string sPARENT_NAME = String.Empty;
			try
			{
				SqlProcs.spPARENT_Get(ref gPARENT_ID, ref sMODULE, ref sPARENT_TYPE, ref sPARENT_NAME);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				// The only possible error is a connection failure, so just ignore all errors. 
				gPARENT_ID = Guid.Empty;
			}
			if ( e.CommandName == "Save" )
			{
				// 01/16/2006 Paul.  Enable validator before validating page. 
				this.ValidateEditViewFields(m_sMODULE + ".EditView");
				if ( Page.IsValid )
				{
					string sCUSTOM_MODULE = "BUGS";
					DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sCUSTOM_MODULE);
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
						DataRow   rowCurrent = null;
						DataTable dtCurrent  = new DataTable();
						if ( !Sql.IsEmptyGuid(gID) )
						{
							string sSQL ;
							sSQL = "select *          " + ControlChars.CrLf
							     + "  from vwBUGS_Edit" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Security.Filter(cmd, m_sMODULE, "edit");
								Sql.AppendParameter(cmd, gID, "ID", false);
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									da.Fill(dtCurrent);
									if ( dtCurrent.Rows.Count > 0 )
									{
										rowCurrent = dtCurrent.Rows[0];
									}
									else
									{
										// 11/19/2007 Paul.  If the record is not found, clear the ID so that the record cannot be updated.
										// It is possible that the record exists, but that ACL rules prevent it from being selected. 
										gID = Guid.Empty;
									}
								}
							}
						}

						using ( IDbTransaction trn = con.BeginTransaction() )
						{
							try
							{
								//die("ERROR: uploaded file was too big: max filesize: {$sugar_config['upload_maxsize']}");
								HtmlInputFile fileATTACHMENT = FindControl("ATTACHMENT") as HtmlInputFile;
								HttpPostedFile pstATTACHMENT  = null;
								if ( fileATTACHMENT != null )
									pstATTACHMENT = fileATTACHMENT.PostedFile;
								if ( pstATTACHMENT != null )
								{
									long lFileSize      = pstATTACHMENT.ContentLength;
									long lUploadMaxSize = Sql.ToLong(Application["CONFIG.upload_maxsize"]);
									if ( (lUploadMaxSize > 0) && (lFileSize > lUploadMaxSize) )
									{
										throw(new Exception("ERROR: uploaded file was too big: max filesize: " + lUploadMaxSize.ToString()));
									}
								}
								// 07/04/2007 Paul.  The Releases list references the fields as IDs, but we need to use them as text values in the detail and grid views. 
								// This also fixes a bug whereby the FOUND_IN_RELEASE field was feed data from a field call RELEASE, which did not exist. 
								// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
								// 12/29/2007 Paul.  TEAM_ID is now in the stored procedure. 
								SqlProcs.spBUGS_Update
									( ref gID
									, new DynamicControl(this, rowCurrent, "ASSIGNED_USER_ID"   ).ID
									, new DynamicControl(this, rowCurrent, "NAME"               ).Text
									, new DynamicControl(this, rowCurrent, "STATUS"             ).SelectedValue
									, new DynamicControl(this, rowCurrent, "PRIORITY"           ).SelectedValue
									, new DynamicControl(this, rowCurrent, "DESCRIPTION"        ).Text
									, new DynamicControl(this, rowCurrent, "RESOLUTION"         ).SelectedValue
									, new DynamicControl(this, rowCurrent, "FOUND_IN_RELEASE_ID").SelectedValue
									, new DynamicControl(this, rowCurrent, "TYPE"               ).SelectedValue
									, new DynamicControl(this, rowCurrent, "FIXED_IN_RELEASE_ID").SelectedValue
									, new DynamicControl(this, rowCurrent, "WORK_LOG"           ).Text
									, new DynamicControl(this, rowCurrent, "SOURCE"             ).Text
									, new DynamicControl(this, rowCurrent, "PRODUCT_CATEGORY"   ).SelectedValue
									, sMODULE
									, gPARENT_ID
									, new DynamicControl(this, rowCurrent, "TEAM_ID"         ).ID
									, trn
									);
								if ( pstATTACHMENT != null )
								{
									// 08/20/2005 Paul.  File may not have been provided. 
									if ( pstATTACHMENT.FileName.Length > 0 )
									{
										string sFILENAME       = Path.GetFileName (pstATTACHMENT.FileName);
										string sFILE_EXT       = Path.GetExtension(sFILENAME);
										string sFILE_MIME_TYPE = pstATTACHMENT.ContentType;
									
										Guid gAttachmentID = Guid.Empty;
										// 01/20/2006 Paul.  Must include in transaction
										SqlProcs.spBUG_ATTACHMENTS_Insert(ref gAttachmentID, gID, pstATTACHMENT.FileName, sFILENAME, sFILE_EXT, sFILE_MIME_TYPE, trn);
										LoadFile(gAttachmentID, pstATTACHMENT.InputStream, trn);
									}
								}
								SplendidDynamic.UpdateCustomFields(this, trn, gID, sCUSTOM_MODULE, dtCustomFields);
								trn.Commit();
							}
							catch(Exception ex)
							{
								trn.Rollback();
								SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
								ctlEditButtons.ErrorText = ex.Message;
								return;
							}
						}
					}
					if ( !Sql.IsEmptyGuid(gPARENT_ID) )
						Response.Redirect("~/" + sMODULE + "/view.aspx?ID=" + gPARENT_ID.ToString());
					else
						Response.Redirect("view.aspx?ID=" + gID.ToString());
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				if ( !Sql.IsEmptyGuid(gPARENT_ID) )
					Response.Redirect("~/" + sMODULE + "/view.aspx?ID=" + gPARENT_ID.ToString());
				else if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = (SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *          " + ControlChars.CrLf
							     + "  from vwBUGS_Edit" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
								Security.Filter(cmd, m_sMODULE, "edit");
								if ( !Sql.IsEmptyGuid(gDuplicateID) )
								{
									Sql.AppendParameter(cmd, gDuplicateID, "ID", false);
									gID = Guid.Empty;
								}
								else
								{
									Sql.AppendParameter(cmd, gID, "ID", false);
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
										Utils.UpdateTracker(Page, m_sMODULE, gID, ctlModuleHeader.Title);
										ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;
										
										this.AppendEditViewFields(m_sMODULE + ".EditView", tblMain, rdr);
									}
									else
									{
										// 11/25/2006 Paul.  If item is not visible, then don't allow save 
										ctlEditButtons.DisableAll();
										ctlEditButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
									}
								}
							}
						}
					}
					else
					{
						this.AppendEditViewFields(m_sMODULE + ".EditView", tblMain, null);
						try
						{
							// 08/31/2005 Paul.  Default value is Medium. 
							new DynamicControl(this, "PRIORITY").SelectedValue = "Medium";
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
						}
					}
				}
				else
				{
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					ctlModuleHeader.Title = Sql.ToString(ViewState["ctlModuleHeader.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlEditButtons.ErrorText = ex.Message;
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			ctlEditButtons.Command = new CommandEventHandler(Page_Command);
			m_sMODULE = "Bugs";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields(m_sMODULE + ".EditView", tblMain, null);
			}
		}
		#endregion
	}
}
