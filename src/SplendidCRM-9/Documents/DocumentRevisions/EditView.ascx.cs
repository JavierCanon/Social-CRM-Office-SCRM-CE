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

namespace SplendidCRM.DocumentRevisions
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.ModuleHeader ctlModuleHeader;
		protected _controls.EditButtons  ctlEditButtons ;

		protected Guid            gID                          ;
		protected HyperLink       lnkFILENAME                  ;
		protected Label           txtCURRENT_REVISION          ;
		protected HtmlInputFile   fileCONTENT                  ;
		protected TextBox         txtREVISION                  ;
		protected TextBox         txtCHANGE_LOG                ;
		protected RequiredFieldValidator reqFILENAME     ;
		protected RequiredFieldValidator reqREVISION     ;

		public static void LoadFile(Guid gID, Stream stm, IDbTransaction trn)
		{
			const int BUFFER_LENGTH = 4*1024;
			byte[] binFILE_POINTER = new byte[16];
			// 01/20/2006 Paul.  Must include in transaction
			SqlProcs.spDOCUMENTS_CONTENT_InitPointer(gID, ref binFILE_POINTER, trn);
			using ( BinaryReader reader = new BinaryReader(stm) )
			{
				int nFILE_OFFSET = 0 ;
				byte[] binBYTES = reader.ReadBytes(BUFFER_LENGTH);
				while ( binBYTES.Length > 0 )
				{
					// 08/14/2005 Paul.  gID is used by Oracle, binFILE_POINTER is used by SQL Server. 
					// 01/20/2006 Paul.  Must include in transaction
					SqlProcs.spDOCUMENTS_CONTENT_WriteOffset(gID, binFILE_POINTER, nFILE_OFFSET, binBYTES, trn);
					nFILE_OFFSET += binBYTES.Length;
					binBYTES = reader.ReadBytes(BUFFER_LENGTH);
				}
			}
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				if ( Page.IsValid )
				{
					try
					{
						HttpPostedFile pstCONTENT  = fileCONTENT.PostedFile;
						//die("ERROR: uploaded file was too big: max filesize: {$sugar_config['upload_maxsize']}");
						if ( pstCONTENT != null )
						{
							long lFileSize      = pstCONTENT.ContentLength;
							long lUploadMaxSize = Sql.ToLong(Application["CONFIG.upload_maxsize"]);
							if ( (lUploadMaxSize > 0) && (lFileSize > lUploadMaxSize) )
							{
								throw(new Exception("ERROR: uploaded file was too big, max filesize: " + lUploadMaxSize.ToString()));
							}
						}
						if ( pstCONTENT != null )
						{
							// 08/20/2005 Paul.  File may not have been provided. 
							if ( pstCONTENT.FileName.Length > 0 )
							{
								string sFILENAME       = Path.GetFileName (pstCONTENT.FileName);
								string sFILE_EXT       = Path.GetExtension(sFILENAME);
								string sFILE_MIME_TYPE = pstCONTENT.ContentType;
								
								// 01/20/2006 Paul.  Use a transaction as multiple operations occur. 
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
										sSQL = "select *               " + ControlChars.CrLf
										     + "  from vwDOCUMENTS_Edit" + ControlChars.CrLf;
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
											Guid gRevisionID = Guid.Empty;
											SqlProcs.spDOCUMENT_REVISIONS_Insert
												( ref gRevisionID
												, gID
												, txtREVISION.Text
												, txtCHANGE_LOG.Text
												, sFILENAME
												, sFILE_EXT
												, sFILE_MIME_TYPE
												, trn
												);
											LoadFile(gRevisionID, pstCONTENT.InputStream, trn);
											trn.Commit();
										}
										catch(Exception ex)
										{
											trn.Rollback();
											throw(new Exception(ex.Message));
										}
									}
								}
							}
						}
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
						ctlEditButtons.ErrorText = ex.Message;
						return;
					}
					Response.Redirect("~/Documents/view.aspx?ID=" + gID.ToString());
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("~/Documents/Default.aspx");
				else
					Response.Redirect("~/Documents/view.aspx?ID=" + gID.ToString());
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
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					if ( !Sql.IsEmptyGuid(gID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *               " + ControlChars.CrLf
							     + "  from vwDOCUMENTS_Edit" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								// 11/24/2006 Paul.  Use new Security.Filter() function to apply Team and ACL security rules.
								Security.Filter(cmd, m_sMODULE, "edit");
								Sql.AppendParameter(cmd, gID, "ID", false);
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										ctlModuleHeader.Title = Sql.ToString(rdr["DOCUMENT_NAME"]);
										SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
										Utils.UpdateTracker(Page, m_sMODULE, gID, ctlModuleHeader.Title);
										ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;

										lnkFILENAME.Text        = Sql.ToString(rdr["DOCUMENT_NAME"]);
										lnkFILENAME.NavigateUrl = "~/Documents/Document.aspx?ID=" + Sql.ToString(rdr["DOCUMENT_REVISION_ID"]);
										txtCURRENT_REVISION.Text = Sql.ToString(rdr["REVISION"     ]);
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
			m_sMODULE = "Documents";
			SetMenu(m_sMODULE);
			ctlEditButtons.Command = new CommandEventHandler(Page_Command);
		}
		#endregion
	}
}
