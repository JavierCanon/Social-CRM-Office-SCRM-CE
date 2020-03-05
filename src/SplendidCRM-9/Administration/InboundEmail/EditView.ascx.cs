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
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Administration.InboundEmail
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
		protected HtmlTable       tblOptions                   ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				try
				{
					// 01/16/2006 Paul.  Enable validator before validating page. 
					this.ValidateEditViewFields(m_sMODULE + ".EditView"   );
					this.ValidateEditViewFields(m_sMODULE + ".EditOptions");

					if ( Page.IsValid )
					{
						DropDownList SERVICE = FindControl("SERVICE") as DropDownList;
						if ( SERVICE != null )
						{
							if ( SERVICE.SelectedValue == "imap" )
							{
								ctlEditButtons.ErrorText += "POP3 is the only supported service at this time. ";
								return;
							}
						}
						DropDownList MAILBOX_TYPE = FindControl("MAILBOX_TYPE") as DropDownList;
						if ( MAILBOX_TYPE != null )
						{
							if ( MAILBOX_TYPE.SelectedValue != "bounce" )
							{
								ctlEditButtons.ErrorText += "Bounce handling is the only supported action at this time. ";
								return;
							}
						}
					}
					if ( Page.IsValid )
					{
						// 01/08/2008 Paul.  If the encryption key does not exist, then we must create it and we must save it back to the database. 
						// 01/08/2008 Paul.  SugarCRM uses blowfish for the inbound email encryption, but we will not since .NET 2.0 does not support blowfish natively. 
						Guid gINBOUND_EMAIL_KEY = Sql.ToGuid(Application["CONFIG.InboundEmailKey"]);
						if ( Sql.IsEmptyGuid(gINBOUND_EMAIL_KEY) )
						{
							gINBOUND_EMAIL_KEY = Guid.NewGuid();
							SqlProcs.spCONFIG_Update("mail", "InboundEmailKey", gINBOUND_EMAIL_KEY.ToString());
							Application["CONFIG.InboundEmailKey"] = gINBOUND_EMAIL_KEY;
						}
						Guid gINBOUND_EMAIL_IV = Sql.ToGuid(Application["CONFIG.InboundEmailIV"]);
						if ( Sql.IsEmptyGuid(gINBOUND_EMAIL_IV) )
						{
							gINBOUND_EMAIL_IV = Guid.NewGuid();
							SqlProcs.spCONFIG_Update("mail", "InboundEmailIV", gINBOUND_EMAIL_IV.ToString());
							Application["CONFIG.InboundEmailIV"] = gINBOUND_EMAIL_IV;
						}

						string sCUSTOM_MODULE = "INBOUND_EMAIL";
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
								sSQL = "select *                    " + ControlChars.CrLf
								     + "  from vwINBOUND_EMAILS_Edit" + ControlChars.CrLf
								     + " where ID = @ID             " + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@ID", gID);
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
									string sEMAIL_PASSWORD = new DynamicControl(this, "EMAIL_PASSWORD").Text;
									if ( sEMAIL_PASSWORD == "**********" )
									{
										if ( rowCurrent != null )
											sEMAIL_PASSWORD = Sql.ToString(rowCurrent["EMAIL_PASSWORD"]);
										else
											sEMAIL_PASSWORD = "";
									}
									else
									{
										string sENCRYPTED_EMAIL_PASSWORD = Security.EncryptPassword(sEMAIL_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV);
										if ( Security.DecryptPassword(sENCRYPTED_EMAIL_PASSWORD, gINBOUND_EMAIL_KEY, gINBOUND_EMAIL_IV) != sEMAIL_PASSWORD )
											throw(new Exception("Decryption failed"));
										sEMAIL_PASSWORD = sENCRYPTED_EMAIL_PASSWORD;
									}
									SqlProcs.spINBOUND_EMAILS_Update
										( ref gID
										, new DynamicControl(this, rowCurrent, "NAME"          ).Text
										, new DynamicControl(this, rowCurrent, "STATUS"        ).SelectedValue
										, new DynamicControl(this, rowCurrent, "SERVER_URL"    ).Text
										, new DynamicControl(this, rowCurrent, "EMAIL_USER"    ).Text
										, sEMAIL_PASSWORD
										, Sql.ToInteger(new DynamicControl(this, rowCurrent, "PORT").Text)
										, new DynamicControl(this, rowCurrent, "MAILBOX_SSL"   ).Checked
										, new DynamicControl(this, rowCurrent, "SERVICE"       ).SelectedValue
										, "INBOX"
										, new DynamicControl(this, rowCurrent, "MARK_READ"     ).Checked
										, new DynamicControl(this, rowCurrent, "ONLY_SINCE"    ).Checked
										, new DynamicControl(this, rowCurrent, "MAILBOX_TYPE"  ).SelectedValue
										, new DynamicControl(this, rowCurrent, "TEMPLATE_ID"   ).ID
										, new DynamicControl(this, rowCurrent, "GROUP_ID"      ).ID
										, new DynamicControl(this, rowCurrent, "FROM_NAME"     ).Text
										, new DynamicControl(this, rowCurrent, "FROM_ADDR"     ).Text
										, new DynamicControl(this, rowCurrent, "FILTER_DOMAIN" ).Text
										, trn
										);
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
						SplendidCache.ClearEmailGroups();
						SplendidCache.ClearInboundEmails();
						Response.Redirect("view.aspx?ID=" + gID.ToString());
					}
				}
				catch(Exception ex)
				{
					SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					ctlEditButtons.ErrorText = ex.Message;
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			this.Visible = SplendidCRM.Security.IS_ADMIN;
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
							sSQL = "select *                    " + ControlChars.CrLf
							     + "  from vwINBOUND_EMAILS_Edit" + ControlChars.CrLf
							     + " where ID = @ID             " + ControlChars.CrLf;
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

										this.AppendEditViewFields(m_sMODULE + ".EditView"   , tblMain   , rdr);
										this.AppendEditViewFields(m_sMODULE + ".EditOptions", tblOptions, rdr);
										// 01/08/2008 Paul.  Don't display the password. 
										// 01/08/2008 Paul.  Browsers don't display passwords. 
										if ( !Sql.IsEmptyString(rdr["EMAIL_PASSWORD"]) )
										{
											TextBox txtEMAIL_PASSWORD = FindControl("EMAIL_PASSWORD") as TextBox;
											if ( txtEMAIL_PASSWORD != null )
											{
												//txtEMAIL_PASSWORD.Text = "**********";
												txtEMAIL_PASSWORD.Attributes.Add("value", "**********");
											}
										}
									}
								}
							}
						}
					}
					else
					{
						this.AppendEditViewFields(m_sMODULE + ".EditView"   , tblMain   , null);
						this.AppendEditViewFields(m_sMODULE + ".EditOptions", tblOptions, null);
					}
					DropDownList GROUP_ID = FindControl("GROUP_ID") as DropDownList;
					if ( GROUP_ID != null )
					{
						GROUP_ID.Items.Insert(0, new ListItem(L10n.Term("InboundEmail.LBL_CREATE_NEW_GROUP"), ""));
					}
					DropDownList MAILBOX_TYPE = FindControl("MAILBOX_TYPE") as DropDownList;
					if ( MAILBOX_TYPE != null )
					{
						try
						{
							MAILBOX_TYPE.SelectedValue = "bounce";
						}
						catch
						{
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
			// CODEGEN: This Task is required by the ASP.NET Web Form Designer.
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
			m_sMODULE = "InboundEmail";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields(m_sMODULE + ".EditView"   , tblMain   , null);
				this.AppendEditViewFields(m_sMODULE + ".EditOptions", tblOptions, null);
			}
		}
		#endregion
	}
}
