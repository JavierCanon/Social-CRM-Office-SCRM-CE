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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Net;
using System.Net.Mail;
using System.Collections;
using System.Diagnostics;
//using Microsoft.VisualBasic;
using FredCK.FCKeditorV2;

namespace SplendidCRM.Emails
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected SplendidCRM._controls.ModuleHeader ctlModuleHeader;
		protected SplendidCRM._controls.EditButtons  ctlEditButtons ;
		protected Emails._controls.EmailButtons      ctlEmailButtons;

		protected string          sEMAIL_TYPE                  ;
		protected Guid            gID                          ;
		protected HtmlInputHidden TEAM_ID                      ;
		protected TextBox         TEAM_NAME                    ;
		protected HtmlInputHidden ASSIGNED_USER_ID             ;
		protected TextBox         ASSIGNED_TO                  ;
		protected DropDownList    lstEMAIL_TEMPLATE            ;
		protected DropDownList    lstPARENT_TYPE               ;
		protected TextBox         txtPARENT_NAME               ;
		protected HtmlInputHidden txtPARENT_ID                 ;
		protected TableRow        trFROM                       ;
		protected TableRow        trNOTE_SEMICOLON             ;
		protected HtmlInputHidden txtFROM_NAME                 ;  // 11/20/2005.  Not used by SugarCRM 3.5.1.
		protected TextBox         txtFROM_ADDR                 ;
		protected TextBox         txtTO_ADDRS                  ;
		protected TextBox         txtCC_ADDRS                  ;
		protected TextBox         txtBCC_ADDRS                 ;
		protected HtmlInputHidden txtTO_ADDRS_IDS              ;
		protected HtmlInputHidden txtTO_ADDRS_NAMES            ;
		protected HtmlInputHidden txtTO_ADDRS_EMAILS           ;
		protected HtmlInputHidden txtCC_ADDRS_IDS              ;
		protected HtmlInputHidden txtCC_ADDRS_NAMES            ;
		protected HtmlInputHidden txtCC_ADDRS_EMAILS           ;
		protected HtmlInputHidden txtBCC_ADDRS_IDS             ;
		protected HtmlInputHidden txtBCC_ADDRS_NAMES           ;
		protected HtmlInputHidden txtBCC_ADDRS_EMAILS          ;
		protected TextBox         txtNAME                      ;
		protected FCKeditor       txtDESCRIPTION               ;
		// 04/16/2006 Paul.  The subject is not required. 
		//protected Label           lblNAME_REQUIRED             ;
		//protected RequiredFieldValidator reqNAME               ;

		protected TableRow               trDATE_START          ;
		protected HtmlGenericControl     spnTEMPLATE_LABEL     ;
		protected SplendidCRM._controls.DateTimeEdit ctlDATE_START         ;
		protected Repeater               ctlAttachments        ;
		protected Repeater               ctlTemplateAttachments;

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
			if ( e.CommandName == "Save" || e.CommandName == "Send" )
			{
				if ( ctlDATE_START.Visible )
					ctlDATE_START.Validate();
				if ( Page.IsValid )
				{
					string sCUSTOM_MODULE = "EMAILS";
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
							sSQL = "select *            " + ControlChars.CrLf
							     + "  from vwEMAILS_Edit" + ControlChars.CrLf;
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
								//txtDESCRIPTION     .Text  = txtDESCRIPTION     .Text .Trim();
								txtFROM_NAME       .Value = txtFROM_NAME       .Value.Trim();
								txtFROM_ADDR       .Text  = txtFROM_ADDR       .Text .Trim();
								txtTO_ADDRS        .Text  = txtTO_ADDRS        .Text .Trim();
								txtCC_ADDRS        .Text  = txtCC_ADDRS        .Text .Trim();
								txtBCC_ADDRS       .Text  = txtBCC_ADDRS       .Text .Trim();
								txtTO_ADDRS_IDS    .Value = txtTO_ADDRS_IDS    .Value.Trim();
								txtTO_ADDRS_NAMES  .Value = txtTO_ADDRS_NAMES  .Value.Trim();
								txtTO_ADDRS_EMAILS .Value = txtTO_ADDRS_EMAILS .Value.Trim();
								txtCC_ADDRS_IDS    .Value = txtCC_ADDRS_IDS    .Value.Trim();
								txtCC_ADDRS_NAMES  .Value = txtCC_ADDRS_NAMES  .Value.Trim();
								txtCC_ADDRS_EMAILS .Value = txtCC_ADDRS_EMAILS .Value.Trim();
								txtBCC_ADDRS_IDS   .Value = txtBCC_ADDRS_IDS   .Value.Trim();
								txtBCC_ADDRS_NAMES .Value = txtBCC_ADDRS_NAMES .Value.Trim();
								txtBCC_ADDRS_EMAILS.Value = txtBCC_ADDRS_EMAILS.Value.Trim();
								if ( e.CommandName == "Send" )
								{
									// 01/21/2006 Paul.  Mark an email as ready-to-send.   Type becomes "out" and Status stays at "draft". 
									if ( sEMAIL_TYPE == "draft" )
										sEMAIL_TYPE = "out";
									// 01/21/2006 Paul.  Address error only when sending. 
									if ( txtTO_ADDRS.Text.Length == 0 && txtCC_ADDRS.Text.Length == 0 && txtBCC_ADDRS.Text.Length == 0 )
										throw(new Exception(L10n.Term("Emails.ERR_NOT_ADDRESSED")));
								}
								// 11/20/2005 Paul.  SugarCRM 3.5.1 lets bad data flow through.  We clear the hidden values if the visible values are empty. 
								// There still is the issue of the data getting out of sync if the user manually edits the visible values. 
								if ( txtTO_ADDRS.Text.Length == 0 )
								{
									txtTO_ADDRS_IDS    .Value = String.Empty;
									txtTO_ADDRS_NAMES  .Value = String.Empty;
									txtTO_ADDRS_EMAILS .Value = String.Empty;
								}
								if ( txtCC_ADDRS.Text.Length == 0 )
								{
									txtCC_ADDRS_IDS    .Value = String.Empty;
									txtCC_ADDRS_NAMES  .Value = String.Empty;
									txtCC_ADDRS_EMAILS .Value = String.Empty;
								}
								if ( txtBCC_ADDRS.Text.Length == 0 )
								{
									txtBCC_ADDRS_IDS   .Value = String.Empty;
									txtBCC_ADDRS_NAMES .Value = String.Empty;
									txtBCC_ADDRS_EMAILS.Value = String.Empty;
								}
								
								// 04/24/2006 Paul.  Upgrade to SugarCRM 4.2 Schema. 
								// 06/01/2006 Paul.  MESSAGE_ID is now a text string. 
								// 12/29/2007 Paul.  TEAM_ID is now in the stored procedure. 
								SqlProcs.spEMAILS_Update
									( ref gID
									, Sql.ToGuid(ASSIGNED_USER_ID.Value)
									, txtNAME.Text
									, T10n.ToServerTime(ctlDATE_START.Value)
									, lstPARENT_TYPE.SelectedValue
									, Sql.ToGuid(txtPARENT_ID.Value)
									// 04/16/2006 Paul.  Since the Plug-in saves body in DESCRIPTION, we need to continue to use it as the primary source of data. 
									, txtDESCRIPTION     .Value  // DESCRIPTION
									, txtDESCRIPTION     .Value  // DESCRIPTION_HTML
									// 07/03/2007 Paul.  From Address & From Name were switched. 
									, txtFROM_ADDR       .Text
									, txtFROM_NAME       .Value
									, txtTO_ADDRS        .Text
									, txtCC_ADDRS        .Text
									, txtBCC_ADDRS       .Text
									, txtTO_ADDRS_IDS    .Value
									, txtTO_ADDRS_NAMES  .Value
									, txtTO_ADDRS_EMAILS .Value
									, txtCC_ADDRS_IDS    .Value
									, txtCC_ADDRS_NAMES  .Value
									, txtCC_ADDRS_EMAILS .Value
									, txtBCC_ADDRS_IDS   .Value
									, txtBCC_ADDRS_NAMES .Value
									, txtBCC_ADDRS_EMAILS.Value
									, sEMAIL_TYPE
									, new DynamicControl(this, "MESSAGE_ID"   ).Text
									, new DynamicControl(this, "REPLY_TO_NAME").Text
									, new DynamicControl(this, "REPLY_TO_ADDR").Text
									, new DynamicControl(this, "INTENT"       ).Text
									, new DynamicControl(this, "MAILBOX_ID"   ).ID
									, Sql.ToGuid(TEAM_ID.Value)
									, trn
									);
								
								// 01/21/2006 Paul.  There can be a maximum of 10 attachments, not including attachments that were previously saved. 
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
													, L10n.Term("Emails.LBL_EMAIL_ATTACHMENT") + ": " + sFILENAME
													, "Emails"   // Parent Type
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
								// 12/21/2007 Paul.  The NOTES table is used as a relationship table between emails and attachments. 
								// When applying an Email Template to an Email, we copy the NOTES records. 
								DataTable dtTemplateAttachments = ViewState["TemplateAttachments"] as DataTable;
								if ( dtTemplateAttachments != null )
								{
									foreach ( DataRow row in dtTemplateAttachments.Rows )
									{
										if ( row.RowState != DataRowState.Deleted )
										{
											Guid gNOTE_ID = Guid.Empty;
											Guid gCOPY_ID = Sql.ToGuid(row["ID"]);
											SqlProcs.spNOTES_Copy(ref gNOTE_ID, gCOPY_ID, "Emails", gID, trn);
										}
									}
								}
								//SplendidDynamic.UpdateCustomFields(this, trn, gID, sCUSTOM_MODULE, dtCustomFields);
								trn.Commit();
								// 01/21/2006 Paul.  In case the SendMail function fails, we want to make sure to reuse the GUID. 
								ViewState["ID"] = gID;
							}
							catch(Exception ex)
							{
								trn.Rollback();
								SplendidError.SystemError(new StackTrace(true).GetFrame(0),  Utils.ExpandException(ex));
								ctlEditButtons.ErrorText = Utils.ExpandException(ex);
								ctlEmailButtons.ErrorText = ctlEditButtons.ErrorText;
								return;
							}
							int nEmailsSent = 0;
							try
							{
								if ( e.CommandName == "Send" )
								{
									string sFromName    = Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/MAIL_FROMNAME"   ]);
									string sFromAddress = Sql.ToString(HttpContext.Current.Session["USER_SETTINGS/MAIL_FROMADDRESS"]);
									
									// 12/20/2007 Paul.  SendEmail was moved to EmailUtils.
									EmailUtils.SendEmail(gID, sFromName, sFromAddress, ref nEmailsSent);
									SqlProcs.spEMAILS_UpdateStatus(gID, "sent");
								}
							}
							catch(Exception ex)
							{
								if ( e.CommandName == "Send" )
								{
									if ( nEmailsSent > 0 )
									{
										SqlProcs.spEMAILS_UpdateStatus(gID, "partial");
									}
								}
								SplendidError.SystemError(new StackTrace(true).GetFrame(0), Utils.ExpandException(ex));
								ctlEditButtons.ErrorText = Utils.ExpandException(ex);
								ctlEmailButtons.ErrorText = ctlEditButtons.ErrorText;
								return;
							}
						}
					}
					if ( !Sql.IsEmptyGuid(gPARENT_ID) )
						Response.Redirect("~/" + sMODULE + "/view.aspx?ID=" + gPARENT_ID.ToString());
					else if ( sEMAIL_TYPE == "draft" )
						Response.Redirect("default.aspx");
					else
						Response.Redirect("view.aspx?ID=" + gID.ToString());
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				if ( !Sql.IsEmptyGuid(gPARENT_ID) )
					Response.Redirect("~/" + sMODULE + "/view.aspx?ID=" + gPARENT_ID.ToString());
				// 09/07/2006 Paul.  If in draft mode, redirect to list.  Viewing a draft will re-direct you to edit mode.
				else if ( Sql.IsEmptyGuid(gID) || Sql.ToString(ViewState["TYPE"]) == "draft" )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
		}

		protected void lstEMAIL_TEMPLATE_Changed(Object sender, EventArgs e)
		{
			// 12/19/2006 Paul.  A customer wanted the ability to prevent users from changing a template. 
			if ( lstEMAIL_TEMPLATE.SelectedValue == String.Empty )
			{
				txtNAME.ReadOnly = false;
				txtDESCRIPTION.ToolbarSet = "SplendidCRM";
				return;
			}

			Guid gEMAIL_TEMPLATE_ID = Sql.ToGuid(lstEMAIL_TEMPLATE.SelectedValue);
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				con.Open();
				string sSQL ;
				sSQL = "select *                     " + ControlChars.CrLf
				     + "  from vwEMAIL_TEMPLATES_Edit" + ControlChars.CrLf
				     + " where ID = @ID              " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", gEMAIL_TEMPLATE_ID);
					using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
					{
						if ( rdr.Read() )
						{
							// 03/05/2007 Michael.  We should use the Subject of the template, not the name.
							txtNAME.Text         = Sql.ToString(rdr["SUBJECT"]);
							// 11/13/2006 Paul.  We switched to BODY_HTML a while back when FCKeditor was first implemented. 
							txtDESCRIPTION.Value = Sql.ToString(rdr["BODY_HTML"]);

							// 12/19/2006 Paul.  Apply READ_ONLY rules. 
							bool bREAD_ONLY = Sql.ToBoolean(rdr["READ_ONLY"]);
							txtNAME.ReadOnly = bREAD_ONLY;
							// 12/19/2006 Paul.  Had to create an empty toolbar in ~/FCKeditor/fckconfig.js
							txtDESCRIPTION.ToolbarSet = bREAD_ONLY ? "None" : "SplendidCRM";
							if ( bREAD_ONLY )
							{
								// 12/19/2006 Paul.  We have to disable the editor in client-side code. 
								RegisterClientScriptBlock("FCKeditor_OnComplete", "<script type=\"text/javascript\">function FCKeditor_OnComplete( editorInstance ){var oEditor = editorInstance; oEditor.EditorDocument.body.disabled=true;}</script>");
							}
						}
					}
				}
				sSQL = "select *                                     " + ControlChars.CrLf
				     + "  from vwEMAIL_TEMPLATES_Attachments         " + ControlChars.CrLf
				     + " where EMAIL_TEMPLATE_ID = @EMAIL_TEMPLATE_ID" + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@EMAIL_TEMPLATE_ID", gEMAIL_TEMPLATE_ID);
					using ( DbDataAdapter da = dbf.CreateDataAdapter() )
					{
						((IDbDataAdapter)da).SelectCommand = cmd;
						using ( DataTable dt = new DataTable() )
						{
							da.Fill(dt);
							ctlTemplateAttachments.DataSource = dt.DefaultView;
							ctlTemplateAttachments.DataBind();
							ViewState["TemplateAttachments"] = dt;
						}
					}
				}
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
				// 01/21/2006 Paul.  If there is an error sending the email, we want to make sure to reuse the ID,
				// otherwise multiple emails get created as the user tries to resend. 
				gID = Sql.ToGuid(ViewState["ID"]);
				if ( Sql.IsEmptyGuid(gID) )
					gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					sEMAIL_TYPE = Sql.ToString(Request["TYPE"]).ToLower();
					if ( sEMAIL_TYPE != "archived" )
						sEMAIL_TYPE = "draft";
					ctlEditButtons .Visible  = !PrintView && (sEMAIL_TYPE != "draft");
					ctlEmailButtons.Visible  = !PrintView && (sEMAIL_TYPE == "draft");
					
					if ( Sql.IsEmptyGuid(gID) )
					{
						ctlModuleHeader.EnableModuleLabel = false;
						if ( sEMAIL_TYPE == "archived" )
							ctlModuleHeader.Title = L10n.Term("Emails.LBL_ARCHIVED_MODULE_NAME") + ":";
						else
							ctlModuleHeader.Title = L10n.Term("Emails.LBL_COMPOSE_MODULE_NAME") + ":";
						// 04/16/2006 Paul.  The subject is not required. 
						//lblNAME_REQUIRED .Visible = (sEMAIL_TYPE == "archived");
						//reqNAME.Enabled          =  lblNAME_REQUIRED.Visible;
						ctlDATE_START    .Visible = (sEMAIL_TYPE == "archived");
						trDATE_START     .Visible =  ctlDATE_START.Visible;
						spnTEMPLATE_LABEL.Visible = (sEMAIL_TYPE == "draft"   );
						lstEMAIL_TEMPLATE.Visible = spnTEMPLATE_LABEL.Visible;
						trNOTE_SEMICOLON .Visible = (sEMAIL_TYPE == "draft"   );
						// 07/03/2007 Paul.  Always show the From field.  This was causing the invalid email address error. 
						trFROM           .Visible = true;
						ViewState["TYPE"] = sEMAIL_TYPE;
						ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;
					}

					lstPARENT_TYPE     .DataSource = SplendidCache.List("record_type_display");
					lstPARENT_TYPE     .DataBind();
					if ( lstEMAIL_TEMPLATE.Visible )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *                     " + ControlChars.CrLf
							     + "  from vwEMAIL_TEMPLATES_List" + ControlChars.CrLf
							     + " order by NAME               " + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								using ( DbDataAdapter da = dbf.CreateDataAdapter() )
								{
									((IDbDataAdapter)da).SelectCommand = cmd;
									using ( DataTable dt = new DataTable() )
									{
										da.Fill(dt);
										lstEMAIL_TEMPLATE.DataSource = dt.DefaultView;
										lstEMAIL_TEMPLATE.DataBind();
										lstEMAIL_TEMPLATE.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
									}
								}
							}
						}
					}
					// 06/30/2007 Paul.  Add support for Forwrad and Reply. 
					string sRequestType = Sql.ToString(Request["type"]).ToLower();
					// 07/29/2005 Paul.  SugarCRM 3.0 does not allow the NONE option. 
					//lstPARENT_TYPE     .Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *            " + ControlChars.CrLf
							     + "  from vwEMAILS_Edit" + ControlChars.CrLf;
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
										ctlModuleHeader.Title += Sql.ToString(rdr["NAME"]);
										SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
										Utils.UpdateTracker(Page, m_sMODULE, gID, ctlModuleHeader.Title);
										ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;
										ViewState["ID"] = gID;

										txtNAME            .Text  = Sql.ToString(rdr["NAME"            ]);
										ctlDATE_START      .Value = T10n.FromServerTime(rdr["DATE_START"]);
										txtPARENT_ID       .Value = Sql.ToString(rdr["PARENT_ID"       ]);
										txtPARENT_NAME     .Text  = Sql.ToString(rdr["PARENT_NAME"     ]);
										txtFROM_NAME       .Value = Sql.ToString(rdr["FROM_NAME"       ]);
										txtFROM_ADDR       .Text  = Sql.ToString(rdr["FROM_ADDR"       ]);

										txtTO_ADDRS        .Text  = Sql.ToString(rdr["TO_ADDRS"        ]);
										txtCC_ADDRS        .Text  = Sql.ToString(rdr["CC_ADDRS"        ]);
										txtBCC_ADDRS       .Text  = Sql.ToString(rdr["BCC_ADDRS"       ]);
										txtTO_ADDRS_IDS    .Value = Sql.ToString(rdr["TO_ADDRS_IDS"    ]);
										txtTO_ADDRS_NAMES  .Value = Sql.ToString(rdr["TO_ADDRS_NAMES"  ]);
										txtTO_ADDRS_EMAILS .Value = Sql.ToString(rdr["TO_ADDRS_EMAILS" ]);
										txtCC_ADDRS_IDS    .Value = Sql.ToString(rdr["CC_ADDRS_IDS"    ]);
										txtCC_ADDRS_NAMES  .Value = Sql.ToString(rdr["CC_ADDRS_NAMES"  ]);
										txtCC_ADDRS_EMAILS .Value = Sql.ToString(rdr["CC_ADDRS_EMAILS" ]);
										txtBCC_ADDRS_IDS   .Value = Sql.ToString(rdr["BCC_ADDRS_IDS"   ]);
										txtBCC_ADDRS_NAMES .Value = Sql.ToString(rdr["BCC_ADDRS_NAMES" ]);
										txtBCC_ADDRS_EMAILS.Value = Sql.ToString(rdr["BCC_ADDRS_EMAILS"]);

										// 04/16/2006 Paul.  Since the Plug-in saves body in DESCRIPTION, we need to continue to use it as the primary source of data. 
										txtDESCRIPTION     .Value = Sql.ToString(rdr["DESCRIPTION"]);
										try
										{
											lstPARENT_TYPE.SelectedValue = Sql.ToString(rdr["PARENT_TYPE"]);
										}
										catch(Exception ex)
										{
											SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
										}
										// 12/21/2006 Paul.  Add Team data. 
										TEAM_ID         .Value = Sql.ToString(rdr["TEAM_ID"         ]);
										TEAM_NAME       .Text  = Sql.ToString(rdr["TEAM_NAME"       ]);
										// 12/21/2006 Paul.  Change Assigned To to a Change button. 
										ASSIGNED_TO     .Text  = Sql.ToString(rdr["ASSIGNED_TO"     ]);
										ASSIGNED_USER_ID.Value = Sql.ToString(rdr["ASSIGNED_USER_ID"]);

										// 11/17/2005 Paul.  Archived emails allow editing of the Date & Time Sent. 
										sEMAIL_TYPE = Sql.ToString(rdr["TYPE"]).ToLower();
										// 06/30/2007 Paul.  A forward or reply is just like a draft. 
										if ( sRequestType == "forward" || sRequestType == "reply" || sRequestType == "replyall" )
										{
											string sFrom = txtFROM_NAME .Value;
											if ( txtFROM_ADDR.Text.Length > 0 )
												sFrom += " [" + txtFROM_ADDR.Text + "]";
											// 06/30/2007 Paul.  We are going to use an HR tag as the delimiter. 
											string sReplyDelimiter = String.Empty;  //"> ";
											StringBuilder sbReplyHeader = new StringBuilder();
											//sbReplyHeader.Append(                  L10n.Term("Emails.LBL_FORWARD_HEADER") + "<br /><br />\r\n");
											sbReplyHeader.Append("<br />\r\n");
											sbReplyHeader.Append("<br />\r\n");
											sbReplyHeader.Append("<hr />\r\n");
											sbReplyHeader.Append(sReplyDelimiter + "<b>" + L10n.Term("Emails.LBL_FROM"     ) + "</b> " + sFrom.Trim()                   + "<br />\r\n");
											sbReplyHeader.Append(sReplyDelimiter + "<b>" + L10n.Term("Emails.LBL_DATE_SENT") + "</b> " + ctlDATE_START.Value.ToString() + "<br />\r\n");
											sbReplyHeader.Append(sReplyDelimiter + "<b>" + L10n.Term("Emails.LBL_TO"       ) + "</b> " + txtTO_ADDRS  .Text             + "<br />\r\n");
											sbReplyHeader.Append(sReplyDelimiter + "<b>" + L10n.Term("Emails.LBL_SUBJECT"  ) + "</b> " + txtNAME      .Text             + "<br />\r\n");
											sbReplyHeader.Append(sReplyDelimiter + "<br />\r\n");
											txtDESCRIPTION.Value = sbReplyHeader.ToString() + txtDESCRIPTION.Value;

											sEMAIL_TYPE = "draft";
											ASSIGNED_TO     .Text  = Security.USER_NAME;
											ASSIGNED_USER_ID.Value = Security.USER_ID.ToString();
											if ( sRequestType == "forward" )
											{
												txtTO_ADDRS        .Text  = String.Empty;
												txtTO_ADDRS_IDS    .Value = String.Empty;
												txtTO_ADDRS_NAMES  .Value = String.Empty;
												txtTO_ADDRS_EMAILS .Value = String.Empty;

												txtCC_ADDRS        .Text  = String.Empty;
												txtCC_ADDRS_IDS    .Value = String.Empty;
												txtCC_ADDRS_NAMES  .Value = String.Empty;
												txtCC_ADDRS_EMAILS .Value = String.Empty;
											}
											else if ( sRequestType == "reply" )
											{

												txtTO_ADDRS        .Text += txtFROM_ADDR.Text;
												//txtTO_ADDRS_IDS    .Value = String.Empty;
												txtTO_ADDRS_NAMES  .Value += txtFROM_NAME.Value;
												txtTO_ADDRS_EMAILS .Value += txtFROM_ADDR.Text ;

												txtCC_ADDRS        .Text  = String.Empty;
												txtCC_ADDRS_IDS    .Value = String.Empty;
												txtCC_ADDRS_NAMES  .Value = String.Empty;
												txtCC_ADDRS_EMAILS .Value = String.Empty;
											}
											else if ( sRequestType == "replyall" )
											{
												txtTO_ADDRS        .Text += txtFROM_ADDR.Text;
												//txtTO_ADDRS_IDS    .Value = String.Empty;
												txtTO_ADDRS_NAMES  .Value += txtFROM_NAME.Value;
												txtTO_ADDRS_EMAILS .Value += txtFROM_ADDR.Text ;
											}
											ctlDATE_START      .Value = DateTime.MinValue;
											txtFROM_NAME       .Value = Sql.ToString(Session["USER_SETTINGS/MAIL_FROMNAME"   ]);
											txtFROM_ADDR       .Text  = Sql.ToString(Session["USER_SETTINGS/MAIL_FROMADDRESS"]);
											txtBCC_ADDRS       .Text  = String.Empty;
											txtBCC_ADDRS_IDS   .Value = String.Empty;
											txtBCC_ADDRS_NAMES .Value = String.Empty;
											txtBCC_ADDRS_EMAILS.Value = String.Empty;
										}
										switch ( sEMAIL_TYPE )
										{
											case "archived":
												ctlModuleHeader.Title = L10n.Term("Emails.LBL_ARCHIVED_MODULE_NAME") + ":" + txtNAME.Text;
												break;
											case "out":
												ctlModuleHeader.Title = L10n.Term("Emails.LBL_LIST_FORM_SENT_TITLE") + ":" + txtNAME.Text;
												break;
											default:
												sEMAIL_TYPE = "draft";
												ctlModuleHeader.Title = L10n.Term("Emails.LBL_COMPOSE_MODULE_NAME" ) + ":" + txtNAME.Text;
												break;
										}
										// 12/20/2006 Paul.  Editing is not allowed for sent emails. 
										// 01/13/2008 Paul.  Editing is not allowed for campaign emails. 
										if ( sEMAIL_TYPE == "out" || sEMAIL_TYPE == "sent" || sEMAIL_TYPE == "campaign" )
										{
											// 01/21/2006 Paul.  Editing is not allowed for sent emails. 
											Response.Redirect("view.aspx?ID=" + gID.ToString());
											return;
										}
										else if ( sEMAIL_TYPE == "inbound" )
										{
											// 01/13/2008 Paul.  Editing is not allowed for inbound emails, and they have their own viewer. 
											Response.Redirect("inbound.aspx?ID=" + gID.ToString());
											return;
										}
										// 04/16/2006 Paul.  The subject is not required. 
										//lblNAME_REQUIRED .Visible = (sEMAIL_TYPE == "archived");
										//reqNAME.Enabled = lblNAME_REQUIRED.Visible;
										ctlDATE_START    .Visible = (sEMAIL_TYPE == "archived");
										trDATE_START     .Visible =  ctlDATE_START.Visible;
										spnTEMPLATE_LABEL.Visible = (sEMAIL_TYPE == "draft"   );
										lstEMAIL_TEMPLATE.Visible = spnTEMPLATE_LABEL.Visible;
										trNOTE_SEMICOLON .Visible = (sEMAIL_TYPE == "draft"   );
										trFROM           .Visible = !trNOTE_SEMICOLON.Visible;
										ctlModuleHeader.EnableModuleLabel = false;

										ctlEditButtons .Visible  = !PrintView && (sEMAIL_TYPE != "draft");
										ctlEmailButtons.Visible  = !PrintView && (sEMAIL_TYPE == "draft");
										ViewState["TYPE"] = sEMAIL_TYPE;
									}
									else
									{
										// 11/25/2006 Paul.  If item is not visible, then don't allow save 
										ctlEditButtons.DisableAll();
										ctlEditButtons.ErrorText = L10n.Term("ACL.LBL_NO_ACCESS");
									}
								}
							}
							sSQL = "select *                   " + ControlChars.CrLf
							     + "  from vwEMAILS_Attachments" + ControlChars.CrLf
							     + " where EMAIL_ID = @EMAIL_ID" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@EMAIL_ID", gID);

								if ( bDebug )
									RegisterClientScriptBlock("vwEMAILS_Attachments", Sql.ClientScriptBlock(cmd));

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
						Guid gPARENT_ID = Sql.ToGuid(Request["PARENT_ID"]);
						if ( !Sql.IsEmptyGuid(gPARENT_ID) )
						{
							string sMODULE      = String.Empty;
							string sPARENT_TYPE = String.Empty;
							string sPARENT_NAME = String.Empty;
							SqlProcs.spPARENT_Get(ref gPARENT_ID, ref sMODULE, ref sPARENT_TYPE, ref sPARENT_NAME);
							if ( !Sql.IsEmptyGuid(gPARENT_ID) )
							{
								txtPARENT_ID  .Value = gPARENT_ID.ToString();
								txtPARENT_NAME.Text  = sPARENT_NAME;
								try
								{
									lstPARENT_TYPE.SelectedValue = sPARENT_TYPE;
								}
								catch(Exception ex)
								{
									SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
								}
								if ( sPARENT_TYPE == "Cases" )
								{
									string sMacro = Crm.Config.inbound_email_case_subject_macro();
									// 01/13/2008 Paul.  SugarCRM uses the Case Number, but we will use the GUID. 
									txtNAME.Text = sMacro.Replace("%1", gPARENT_ID.ToString());
								}
								// 08/05/2006 Paul.  When an email is composed from a Lead, automatically set the To address. 
								DbProviderFactory dbf = DbProviderFactories.GetFactory();
								using ( IDbConnection con = dbf.CreateConnection() )
								{
									string sSQL ;
									sSQL = "select EMAIL1      " + ControlChars.CrLf
									     + "  from vwLEADS_Edit" + ControlChars.CrLf
									     + " where ID = @ID    " + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										Sql.AddParameter(cmd, "@ID", gPARENT_ID);
										con.Open();

										if ( bDebug )
											RegisterClientScriptBlock("vwLEADS_Edit", Sql.ClientScriptBlock(cmd));

										using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
										{
											if ( rdr.Read() )
											{
												txtTO_ADDRS.Text = Sql.ToString(rdr["EMAIL1"]);
											}
										}
									}
								}
							}
						}
						// 12/21/2006 Paul.  The team name should always default to the current user's private team. 
						TEAM_NAME       .Text  = Security.TEAM_NAME;
						TEAM_ID         .Value = Security.TEAM_ID.ToString();
						// 12/21/2006 Paul.  Change Assigned To to a Change button. 
						ASSIGNED_TO     .Text  = Security.USER_NAME;
						ASSIGNED_USER_ID.Value = Security.USER_ID.ToString();
						txtFROM_NAME    .Value = Sql.ToString(Session["USER_SETTINGS/MAIL_FROMNAME"   ]);
						txtFROM_ADDR    .Text  = Sql.ToString(Session["USER_SETTINGS/MAIL_FROMADDRESS"]);
					}
				}
				else
				{
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					ctlModuleHeader.Title = Sql.ToString(ViewState["ctlModuleHeader.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
					sEMAIL_TYPE = Sql.ToString(ViewState["TYPE"]);
				}
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				ctlEditButtons.ErrorText = ex.Message;
				ctlEmailButtons.ErrorText = ex.Message;
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
			ctlEmailButtons.Command = new CommandEventHandler(Page_Command);
			m_sMODULE = "Emails";
			// 02/13/2007 Paul.  Emails should highlight the Activities menu. 
			// 05/26/2007 Paul.  We are display the emails tab, so we must highlight the tab. 
			SetMenu(m_sMODULE);
		}
		#endregion
	}
}
