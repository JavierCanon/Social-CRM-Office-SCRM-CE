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
using FredCK.FCKeditorV2;

namespace SplendidCRM.EmailTemplates
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.ModuleHeader ctlModuleHeader;
		protected _controls.EditButtons  ctlEditButtons ;

		protected Guid            gID                          ;
		protected Guid            gCAMPAIGN_ID                 ;
		protected TextBox         txtNAME                      ;
		protected TextBox         txtDESCRIPTION               ;
		protected DropDownList    lstVariableModule            ;
		protected DropDownList    lstVariableName              ;
		protected TextBox         txtVariableText              ;
		protected TextBox         txtSUBJECT                   ;
		protected FCKeditor       txtBODY                      ;
		protected HtmlInputHidden TEAM_ID                      ;
		protected TextBox         TEAM_NAME                    ;
		protected CheckBox        chkREAD_ONLY                 ;
		protected DropDownList    lstTrackerName               ;
		protected TextBox         txtTrackerText               ;
		protected Repeater        ctlAttachments               ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				if ( Page.IsValid )
				{
					// 11/22/2006 Paul.  Fix name of custom module. 
					string sCUSTOM_MODULE = "EMAIL_TEMPLATES";
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
							sSQL = "select *                     " + ControlChars.CrLf
							     + "  from vwEMAIL_TEMPLATES_Edit" + ControlChars.CrLf;
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
								// 12/19/2006 Paul.  Add READ_ONLY field. 
								// 12/29/2007 Paul.  TEAM_ID is now in the stored procedure. 
								SqlProcs.spEMAIL_TEMPLATES_Update
									( ref gID
									, false  // 11/17/2005 Paul.  The PUBLISH flag is no longer used in SugarCRM 3.5.0B
									, chkREAD_ONLY.Checked
									, txtNAME.Text
									, txtDESCRIPTION.Text
									, txtSUBJECT.Text
									, String.Empty   // BODY
									, txtBODY.Value  // BODY_HTML
									, Sql.ToGuid(TEAM_ID.Value)
									, trn
									);
								// 12/21/2007 Paul.  There can be a maximum of 10 attachments, not including attachments that were previously saved. 
								for ( int i=0; i < 10; i++ )
								{
									HtmlInputFile fileATTACHMENT = FindControl("email_attachment" + i.ToString()) as HtmlInputFile;
									if ( fileATTACHMENT != null )
									{
										HttpPostedFile pstATTACHMENT = fileATTACHMENT.PostedFile;
										if ( pstATTACHMENT != null )
										{
											long lFileSize      = pstATTACHMENT.ContentLength;
											long lUploadMaxSize = Sql.ToLong(Application["CONFIG.upload_maxsize"]);
											if ( (lUploadMaxSize > 0) && (lFileSize > lUploadMaxSize) )
											{
												throw(new Exception("ERROR: uploaded file was too big: max filesize: " + lUploadMaxSize.ToString()));
											}
											// 08/20/2005 Paul.  File may not have been provided. 
											if ( pstATTACHMENT.FileName.Length > 0 )
											{
												string sFILENAME       = Path.GetFileName (pstATTACHMENT.FileName);
												string sFILE_EXT       = Path.GetExtension(sFILENAME);
												string sFILE_MIME_TYPE = pstATTACHMENT.ContentType;
											
												Guid gNOTE_ID = Guid.Empty;
												// 12/29/2007 Paul.  TEAM_ID is now in the stored procedure. 
												SqlProcs.spNOTES_Update
													( ref gNOTE_ID
													, L10n.Term("EmailTemplates.LBL_EMAIL_ATTACHMENT") + ": " + sFILENAME
													, "EmailTemplates"   // Parent Type
													, gID        // Parent ID
													, Guid.Empty
													, String.Empty
													, Sql.ToGuid(TEAM_ID.Value)
													, trn
													);

												Guid gNOTE_ATTACHMENT_ID = Guid.Empty;
												// 01/20/2006 Paul.  Must include in transaction
												SqlProcs.spNOTE_ATTACHMENTS_Insert(ref gNOTE_ATTACHMENT_ID, gNOTE_ID, pstATTACHMENT.FileName, sFILENAME, sFILE_EXT, sFILE_MIME_TYPE, trn);
												Notes.EditView.LoadFile(gNOTE_ATTACHMENT_ID, pstATTACHMENT.InputStream, trn);
											}
										}
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
					if ( Request.AppRelativeCurrentExecutionFilePath == "~/EmailTemplates/PopupEdit.aspx" )
						Response.Redirect("Popup.aspx?CAMPAIGN_ID=" + gCAMPAIGN_ID.ToString() + "&ID=" + gID.ToString());
					else
						Response.Redirect("view.aspx?ID=" + gID.ToString());
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				if ( Request.AppRelativeCurrentExecutionFilePath == "~/EmailTemplates/PopupEdit.aspx" )
					Response.Redirect("Popup.aspx?CAMPAIGN_ID=" + gCAMPAIGN_ID.ToString());
				else if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
		}

		protected void lstVariableModule_Changed(Object sender, EventArgs e)
		{
			lstVariableName.Items.Clear();
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL ;
				sSQL = "select *                       " + ControlChars.CrLf
				     + "  from vwSqlColumns            " + ControlChars.CrLf
				     + " where ObjectName = @ObjectName" + ControlChars.CrLf
				     + " order by colid                " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ObjectName", "vw" + lstVariableModule.SelectedValue.ToUpper());
					con.Open();
					using ( IDataReader rdr = cmd.ExecuteReader() )
					{
						while ( rdr.Read() )
						{
							string sValue  = Sql.ToString(rdr["ColumnName"]);
							string sText   = L10n.Term(lstVariableModule.SelectedValue + ".LBL_" + sValue.ToUpper());
							string sModule = lstVariableModule.SelectedValue;
							if ( sModule == "Contacts" )
								sModule = "contact";
							else if ( sModule == "Accounts" )
								sModule = "account";
							sText = sText.Replace(":", "");
							lstVariableName.Items.Add(new ListItem(sText, sModule + "_" + sValue.ToLower()));
						}
					}
				}
			}
			txtVariableText.Text = "$" + lstVariableName.Items[0].Value;
		}

		private void lstTrackerName_Bind()
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL ;
				sSQL = "select *                         " + ControlChars.CrLf
				     + "  from vwCAMPAIGNS_CAMPAIGN_TRKRS" + ControlChars.CrLf
				     + " where CAMPAIGN_ID = @CAMPAIGN_ID" + ControlChars.CrLf
				     + " order by TRACKER_NAME asc       " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@CAMPAIGN_ID", gCAMPAIGN_ID);
		
					if ( bDebug )
						RegisterClientScriptBlock("vwCAMPAIGNS_CAMPAIGN_TRKRS", Sql.ClientScriptBlock(cmd));
		
					con.Open();
					using ( IDataReader rdr = cmd.ExecuteReader() )
					{
						while ( rdr.Read() )
						{
							string sValue  = Sql.ToString(rdr["TRACKER_NAME"]);
							string sText   = Sql.ToString(rdr["TRACKER_NAME"]) + " : " + Sql.ToString(rdr["TRACKER_URL"]);
							lstTrackerName.Items.Add(new ListItem(sText, sValue));
						}
					}
				}
			}
			if ( lstTrackerName.Items.Count > 0 )
				txtTrackerText.Text = "{" + lstTrackerName.Items[0].Value + "}";
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
				gCAMPAIGN_ID = Sql.ToGuid(Request["CAMPAIGN_ID"]);
				if ( !IsPostBack )
				{
					lstVariableModule.Items.Add(new ListItem(L10n.Term(".LBL_ACCOUNT"                         ), "Accounts"));
					lstVariableModule.Items.Add(new ListItem(L10n.Term("EmailTemplates.LBL_CONTACT_AND_OTHERS"), "Contacts"));
					lstVariableModule_Changed(null, null);

					if ( !Sql.IsEmptyGuid(gCAMPAIGN_ID) )
					{
						lstTrackerName_Bind();
					}

					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *                     " + ControlChars.CrLf
							     + "  from vwEMAIL_TEMPLATES_Edit" + ControlChars.CrLf;
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

										// 03/04/2006 Paul.  Name was not being set. 
										txtNAME       .Text  = Sql.ToString(rdr["NAME"       ]);
										txtDESCRIPTION.Text  = Sql.ToString(rdr["DESCRIPTION"]);
										txtSUBJECT    .Text  = Sql.ToString(rdr["SUBJECT"    ]);
										// 04/21/2006 Paul.  Change BODY to BODY_HTML. 
										txtBODY       .Value = Sql.ToString(rdr["BODY_HTML"  ]);
										// 12/19/2006 Paul.  Add READ_ONLY field. 
										chkREAD_ONLY.Checked = Sql.ToBoolean(rdr["READ_ONLY"]);
										// 12/21/2006 Paul.  Add Team data. 
										TEAM_ID       .Value = Sql.ToString(rdr["TEAM_ID"    ]);
										TEAM_NAME     .Text  = Sql.ToString(rdr["TEAM_NAME"  ]);
									}
									else
									{
										// 11/25/2006 Paul.  If item is not visible, then don't allow save 
										ctlEditButtons.DisableAll();
										ctlEditButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
									}
								}
							}
							sSQL = "select *                                     " + ControlChars.CrLf
							     + "  from vwEMAIL_TEMPLATES_Attachments         " + ControlChars.CrLf
							     + " where EMAIL_TEMPLATE_ID = @EMAIL_TEMPLATE_ID" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@EMAIL_TEMPLATE_ID", gID);

								if ( bDebug )
									RegisterClientScriptBlock("vwEMAIL_TEMPLATES_Attachments", Sql.ClientScriptBlock(cmd));

								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										ctlAttachments.DataSource = dt.DefaultView;
										ctlAttachments.DataBind();
									}
								}
							}
						}
					}
					else
					{
						// 12/21/2006 Paul.  The team name should always default to the current user's private team. 
						TEAM_NAME.Text  = Security.TEAM_NAME;
						TEAM_ID  .Value = Security.TEAM_ID.ToString();
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
			// CODEGEN: This Meeting is required by the ASP.NET Web Form Designer.
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
			m_sMODULE = "EmailTemplates";
			SetMenu(m_sMODULE);
		}
		#endregion
	}
}
