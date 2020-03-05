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
using System.Xml;
using System.Globalization;
using System.Threading;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Users
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.ModuleHeader ctlModuleHeader;
		protected _controls.EditButtons  ctlEditButtons ;

		private const string sEMPTY_PASSWORD = "******";
		protected Guid            gID                             ;
		protected HtmlTable       tblMain                         ;
		protected HtmlTable       tblAddress                      ;
		protected HtmlTable       tblMailOptions                  ;

		protected TextBox         txtFIRST_NAME                   ;
		protected TextBox         txtLAST_NAME                    ;
		protected TextBox         txtUSER_NAME                    ;
		protected DropDownList    lstSTATUS                       ;
		// user_settings
		protected CheckBox        chkIS_ADMIN                     ;
		protected CheckBox        chkPORTAL_ONLY                  ;
		protected CheckBox        chkRECEIVE_NOTIFICATIONS        ;
		protected DropDownList    lstTHEME                        ;
		protected DropDownList    lstLANGUAGE                     ;
		protected DropDownList    lstDATE_FORMAT                  ;
		protected DropDownList    lstTIME_FORMAT                  ;
		protected DropDownList    lstTIMEZONE                     ;
		protected CheckBox        chkGRIDLINE                     ;
		protected DropDownList    lstCURRENCY                     ;
		protected TextBox         txtGROUP_SEPARATOR              ;
		protected TextBox         txtDECIMAL_SEPARATOR            ;

		// 08/05/2006 Paul.  Remove stub of unsupported code. Reminder is not supported at this time. 
		//protected CheckBox        chkSHOULD_REMIND                ;
		//protected DropDownList    lstREMINDER_TIME                ;
		// freebusy
		// 08/05/2006 Paul.  Remove stub of unsupported code. Calendar Publish Key is not supported at this time. 
		//protected TextBox         txtCALENDAR_PUBLISH_KEY         ;
		//protected TextBox         txtCALENDAR_PUBLISH_URL         ;
		//protected TextBox         txtCALENDAR_SEARCH_URL          ;

		protected bool            bMyAccount                      ;
		protected RequiredFieldValidator reqLAST_NAME;
		protected RequiredFieldValidator reqUSER_NAME;

		public bool MyAccount
		{
			get
			{
				return bMyAccount;
			}
			set
			{
				bMyAccount = value;
			}
		}

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
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
				this.ValidateEditViewFields(m_sMODULE + ".EditView"   );
				this.ValidateEditViewFields(m_sMODULE + ".EditAddress");
				if ( Page.IsValid )
				{
					string sUSER_PREFERENCES = String.Empty;
					XmlDocument xml = new XmlDocument();
					try
					{
						try
						{
							sUSER_PREFERENCES = Sql.ToString(ViewState["USER_PREFERENCES"]);
							xml.LoadXml(sUSER_PREFERENCES);
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
							xml.AppendChild(xml.CreateProcessingInstruction("xml" , "version=\"1.0\" encoding=\"UTF-8\""));
							xml.AppendChild(xml.CreateElement("USER_PREFERENCE"));
						}
						// user_settings
						XmlUtil.SetSingleNode(xml, "gridline"            , chkGRIDLINE.Checked ? "true" : "false");
						XmlUtil.SetSingleNode(xml, "culture"             , lstLANGUAGE.SelectedValue             );
						XmlUtil.SetSingleNode(xml, "theme"               , lstTHEME.SelectedValue                );
						XmlUtil.SetSingleNode(xml, "dateformat"          , lstDATE_FORMAT.SelectedValue          );
						XmlUtil.SetSingleNode(xml, "timeformat"          , lstTIME_FORMAT.SelectedValue          );
						XmlUtil.SetSingleNode(xml, "timezone"            , lstTIMEZONE.SelectedValue             );
						XmlUtil.SetSingleNode(xml, "currency_id"         , lstCURRENCY.SelectedValue             );
						XmlUtil.SetSingleNode(xml, "num_grp_sep"         , txtGROUP_SEPARATOR.Text               );
						XmlUtil.SetSingleNode(xml, "dec_sep"             , txtDECIMAL_SEPARATOR.Text             );
						// 08/05/2006 Paul.  Remove stub of unsupported code. Reminder is not supported at this time. 
						//XmlUtil.SetSingleNode(xml, "reminder_time"       , chkSHOULD_REMIND.Checked ? lstREMINDER_TIME.SelectedValue : "0" );
						// mail_options
						
						string sMAIL_SMTPPASS = Sql.ToString(ViewState["mail_smtppass"]);
						// 08/06/2005 Paul.  Password might be our empty value. 
						TextBox txtMAIL_SMTPPASS = FindControl("MAIL_SMTPPASS") as TextBox;
						if ( txtMAIL_SMTPPASS != null )
						{
							// 08/05/2006 Paul.  Allow the password to be cleared. 
							if ( txtMAIL_SMTPPASS.Text != sEMPTY_PASSWORD )
								sMAIL_SMTPPASS = txtMAIL_SMTPPASS.Text;
						}
						
						XmlUtil.SetSingleNode(xml, "mail_fromname"       , new DynamicControl(this, "MAIL_FROMNAME"    ).Text   );
						XmlUtil.SetSingleNode(xml, "mail_fromaddress"    , new DynamicControl(this, "MAIL_FROMADDRESS" ).Text   );
						XmlUtil.SetSingleNode(xml, "mail_smtpserver"     , new DynamicControl(this, "MAIL_SMTPSERVER"  ).Text   );
						XmlUtil.SetSingleNode(xml, "mail_smtpport"       , new DynamicControl(this, "MAIL_SMTPPORT"    ).Text   );
						XmlUtil.SetSingleNode(xml, "mail_sendtype"       , new DynamicControl(this, "MAIL_SENDTYPE"    ).Text   );
						XmlUtil.SetSingleNode(xml, "mail_smtpauth_req"   , new DynamicControl(this, "MAIL_SMTPAUTH_REQ").Checked ? "true" : "false");
						XmlUtil.SetSingleNode(xml, "mail_smtpuser"       , new DynamicControl(this, "MAIL_SMTPUSER"    ).Text   );
						XmlUtil.SetSingleNode(xml, "mail_smtppass"       , sMAIL_SMTPPASS);
						
						// freebusy
						// 08/05/2006 Paul.  Remove stub of unsupported code. Calendar Publish Key is not supported at this time. 
						//XmlUtil.SetSingleNode(xml, "calendar_publish_key", txtCALENDAR_PUBLISH_KEY .Text         );
						//XmlUtil.SetSingleNode(xml, "calendar_publish_url", txtCALENDAR_PUBLISH_URL .Text         );
						//XmlUtil.SetSingleNode(xml, "calendar_search_url" , txtCALENDAR_SEARCH_URL  .Text         );
					}
					catch(Exception ex)
					{
						SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
					}
					if ( Sql.ToBoolean(Application["CONFIG.XML_UserPreferences"]) )
						sUSER_PREFERENCES = xml.OuterXml;
					else
						sUSER_PREFERENCES = XmlUtil.ConvertToPHP(xml.DocumentElement);
					
					// 12/06/2005 Paul.  Need to prevent duplicate users. 
					string sUSER_NAME = txtUSER_NAME.Text.Trim();
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					try
					{
						// 11/10/2006 Paul.  If the decimal and group separate match, then .NET will not be able to parse decimals. 
						// The exception "Input string was not in a correct format." is thrown. 
						if ( txtGROUP_SEPARATOR.Text == txtDECIMAL_SEPARATOR.Text )
							throw(new Exception(L10n.Term("Users.LBL_INVALID_DECIMAL")));
						if ( !Sql.IsEmptyString(sUSER_NAME) )
						{
							using ( IDbConnection con = dbf.CreateConnection() )
							{
								string sSQL ;
								sSQL = "select USER_NAME             " + ControlChars.CrLf
								     + "  from vwUSERS               " + ControlChars.CrLf
								     + " where USER_NAME = @USER_NAME" + ControlChars.CrLf;
								using ( IDbCommand cmd = con.CreateCommand() )
								{
									cmd.CommandText = sSQL;
									Sql.AddParameter(cmd, "@USER_NAME", sUSER_NAME);
									if ( !Sql.IsEmptyGuid(gID) )
									{
										// 12/06/2005 Paul.  Only include the ID if it is not null as we cannot compare NULL to anything. 
										cmd.CommandText += "   and ID <> @ID" + ControlChars.CrLf;
										Sql.AddParameter(cmd, "@ID", gID);
									}
									con.Open();
									using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
									{
										if ( rdr.Read() )
										{
											string sMESSAGE = String.Empty;
											sMESSAGE = String.Format(L10n.Term("Users.ERR_USER_NAME_EXISTS_1") + "{0}" + L10n.Term("Users.ERR_USER_NAME_EXISTS_2"), sUSER_NAME);
											throw(new Exception(sMESSAGE));
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

					string sCUSTOM_MODULE = "USERS";
					DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sCUSTOM_MODULE);
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
						DataRow   rowCurrent = null;
						DataTable dtCurrent  = new DataTable();
						if ( !Sql.IsEmptyGuid(gID) )
						{
							string sSQL ;
							sSQL = "select *           " + ControlChars.CrLf
							     + "  from vwUSERS_Edit" + ControlChars.CrLf;
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
								bool bNewUser = Sql.IsEmptyGuid(gID);
								// 04/24/2006 Paul.  Upgrade to SugarCRM 4.2 Schema. 
								// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
								SqlProcs.spUSERS_Update
									( ref gID
									, sUSER_NAME
									, txtFIRST_NAME.Text
									, txtLAST_NAME .Text
									, new DynamicControl(this, rowCurrent, "REPORTS_TO_ID"     ).ID
									, (Security.IS_ADMIN ? chkIS_ADMIN.Checked : Sql.ToBoolean(ViewState["IS_ADMIN"]) )
									, chkRECEIVE_NOTIFICATIONS.Checked
									, new DynamicControl(this, rowCurrent, "DESCRIPTION"       ).Text
									, new DynamicControl(this, rowCurrent, "TITLE"             ).Text
									, new DynamicControl(this, rowCurrent, "DEPARTMENT"        ).Text
									, new DynamicControl(this, rowCurrent, "PHONE_HOME"        ).Text
									, new DynamicControl(this, rowCurrent, "PHONE_MOBILE"      ).Text
									, new DynamicControl(this, rowCurrent, "PHONE_WORK"        ).Text
									, new DynamicControl(this, rowCurrent, "PHONE_OTHER"       ).Text
									, new DynamicControl(this, rowCurrent, "PHONE_FAX"         ).Text
									, new DynamicControl(this, rowCurrent, "EMAIL1"            ).Text
									, new DynamicControl(this, rowCurrent, "EMAIL2"            ).Text
									, lstSTATUS.SelectedValue
									, new DynamicControl(this, rowCurrent, "ADDRESS_STREET"    ).Text
									, new DynamicControl(this, rowCurrent, "ADDRESS_CITY"      ).Text
									, new DynamicControl(this, rowCurrent, "ADDRESS_STATE"     ).Text
									, new DynamicControl(this, rowCurrent, "ADDRESS_POSTALCODE").Text
									, new DynamicControl(this, rowCurrent, "ADDRESS_COUNTRY"   ).Text
									, sUSER_PREFERENCES
									, chkPORTAL_ONLY.Checked
									, new DynamicControl(this, rowCurrent, "EMPLOYEE_STATUS"   ).SelectedValue
									, new DynamicControl(this, rowCurrent, "MESSENGER_ID"      ).Text
									, new DynamicControl(this, rowCurrent, "MESSENGER_TYPE"    ).SelectedValue
									, sMODULE
									, gPARENT_ID
									, new DynamicControl(this, rowCurrent, "IS_GROUP"          ).Checked
									, trn
									);
								SplendidDynamic.UpdateCustomFields(this, trn, gID, sCUSTOM_MODULE, dtCustomFields);
								trn.Commit();
								// 09/09/2006 Paul.  Refresh cached user information. 
								if ( bNewUser )
									SplendidCache.ClearUsers();
								// 08/27/2005 Paul. Reload session with user preferences. 
								// 08/30/2005 Paul. Only reload preferences the user is editing his own profile. 
								// We want to allow an administrator to update other user profiles. 
								if ( Security.USER_ID == gID )
									SplendidInit.LoadUserPreferences(gID, lstTHEME.SelectedValue, lstLANGUAGE.SelectedValue);
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
					else if ( bMyAccount )
						Response.Redirect("MyAccount.aspx");
					else
						Response.Redirect("view.aspx?ID=" + gID.ToString());
				}
			}
			else if ( e.CommandName == "Cancel" )
			{
				if ( !Sql.IsEmptyGuid(gPARENT_ID) )
					Response.Redirect("~/" + sMODULE + "/view.aspx?ID=" + gPARENT_ID.ToString());
				else if ( bMyAccount )
					Response.Redirect("MyAccount.aspx");
				else if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
		}

		protected void lstLANGUAGE_Changed(Object sender, EventArgs e)
		{
			if ( lstLANGUAGE.SelectedValue.Length > 0 )
			{
				CultureInfo oldCulture   = Thread.CurrentThread.CurrentCulture   ;
				CultureInfo oldUICulture = Thread.CurrentThread.CurrentUICulture ;
				Thread.CurrentThread.CurrentCulture   = CultureInfo.CreateSpecificCulture(lstLANGUAGE.SelectedValue);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(lstLANGUAGE.SelectedValue);

				DateTime dtNow = T10n.FromServerTime(DateTime.Now);
				DateTimeFormatInfo oDateInfo   = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
				NumberFormatInfo   oNumberInfo = Thread.CurrentThread.CurrentCulture.NumberFormat  ;
				String[] aDateTimePatterns = oDateInfo.GetAllDateTimePatterns();

				lstDATE_FORMAT.Items.Clear();
				lstTIME_FORMAT.Items.Clear();
				foreach ( string sPattern in aDateTimePatterns )
				{
					// 11/12/2005 Paul.  Only allow patterns that have a full year. 
					if ( sPattern.IndexOf("yyyy") >= 0 && sPattern.IndexOf("dd") >= 0 && sPattern.IndexOf("mm") <  0 )
						lstDATE_FORMAT.Items.Add(new ListItem(sPattern + "   " + dtNow.ToString(sPattern), sPattern));
					if ( sPattern.IndexOf("yy") <  0 && sPattern.IndexOf("mm") >= 0 )
						lstTIME_FORMAT.Items.Add(new ListItem(sPattern + "   " + dtNow.ToString(sPattern), sPattern));
				}
				Thread.CurrentThread.CurrentCulture = oldCulture  ;
				Thread.CurrentThread.CurrentCulture = oldUICulture;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			// 07/11/2006 Paul.  Users must be able to view and edit their own settings. 
			this.Visible = bMyAccount || SplendidCRM.Security.IS_ADMIN;  //(SplendidCRM.Security.GetUserAccess(m_sMODULE, "edit") >= 0);
			if ( !this.Visible )
				return;

			reqUSER_NAME.DataBind();
			reqLAST_NAME.DataBind();
			try
			{
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
				gID = Sql.ToGuid(Request["ID"]);
				if ( bMyAccount )
				{
					gID = Security.USER_ID;
				}
				// 07/12/2006 Paul.  Status can only be edited by an administrator. 
				lstSTATUS.Enabled = false;
				// 12/06/2005 Paul.  A user can only edit his own user name if Windows Authentication is off. 
				if ( Security.IS_ADMIN )
				{
					// 12/06/2005 Paul.  An administrator can always edit the user name.  This is to allow him to pre-add any NTLM users. 
					txtUSER_NAME.Enabled = true;
					lstSTATUS.Enabled = true;
				}
				else if ( gID == Security.USER_ID )
				{
					// 12/06/2005 Paul.  If editing yourself, then you can only edit if not NTLM. 
					// txtUSER_NAME.Enabled = !Security.IsWindowsAuthentication();
					// 11/26/2006 Paul.  A user cannot edit their own user name. This is a job for the admin. 
					txtUSER_NAME.Enabled = false;
				}
				else
				{
					// 12/06/2005 Paul.  If not an administrator and not editing yourself, then the name cannot be edited. 
					txtUSER_NAME.Enabled = false;
				}

				if ( !IsPostBack )
				{
					// 'date_formats' => array('Y-m-d'=>'2006-12-23', 'm-d-Y'=>'12-23-2006', 'Y/m/d'=>'2006/12/23', 'm/d/Y'=>'12/23/2006')
					// 'time_formats' => array('H:i'=>'23:00', 'h:ia'=>'11:00pm', 'h:iA'=>'11:00PM', 'H.i'=>'23.00', 'h.ia'=>'11.00pm', 'h.iA'=>'11.00PM' )
					lstSTATUS         .DataSource = SplendidCache.List("user_status_dom");
					lstSTATUS         .DataBind();
					// 08/05/2006 Paul.  Remove stub of unsupported code. Reminder is not supported at this time. 
					//lstREMINDER_TIME  .DataSource = SplendidCache.List("reminder_time_dom");
					//lstREMINDER_TIME  .DataBind();
					lstTIMEZONE       .DataSource = SplendidCache.TimezonesListbox();
					lstTIMEZONE       .DataBind();
					lstCURRENCY       .DataSource = SplendidCache.Currencies();
					lstCURRENCY       .DataBind();
					// 05/09/2006 Paul.  We need to always initialize the separators, just in case the user is new. 
					txtGROUP_SEPARATOR.Text   = SplendidDefaults.GroupSeparator();
					txtDECIMAL_SEPARATOR.Text = SplendidDefaults.DecimalSeparator();

					lstLANGUAGE.DataSource = SplendidCache.Languages();
					lstLANGUAGE.DataBind();
					lstLANGUAGE_Changed(null, null);
					lstTHEME.DataSource = SplendidCache.Themes();
					lstTHEME.DataBind();

					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *           " + ControlChars.CrLf
							     + "  from vwUSERS_Edit" + ControlChars.CrLf
							     + " where ID = @ID    " + ControlChars.CrLf;
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
										ctlModuleHeader.Title = Sql.ToString(rdr["FULL_NAME"]);
										SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title + " (" + Sql.ToString(rdr["USER_NAME"]) + ")");
										Utils.UpdateTracker(Page, m_sMODULE, gID, ctlModuleHeader.Title);
										ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;
										
										this.AppendEditViewFields(m_sMODULE + ".EditView"       , tblMain       , rdr);
										this.AppendEditViewFields(m_sMODULE + ".EditAddress"    , tblAddress    , rdr);
										// 08/05/2006 Paul.  Use the dynamic grid to create the fields, but populate manually. 
										this.AppendEditViewFields(m_sMODULE + ".EditMailOptions", tblMailOptions, null);
										// 01/20/2008 Paul.  The mail options panel is manually populated. 
										new DynamicControl(this, "EMAIL1").Text = Sql.ToString (rdr["EMAIL1"]);
										new DynamicControl(this, "EMAIL2").Text = Sql.ToString (rdr["EMAIL2"]);

										// main
										txtUSER_NAME            .Text    = Sql.ToString (rdr["USER_NAME"            ]);
										txtFIRST_NAME           .Text    = Sql.ToString (rdr["FIRST_NAME"           ]);
										txtLAST_NAME            .Text    = Sql.ToString (rdr["LAST_NAME"            ]);
										// user_settings
										chkIS_ADMIN             .Checked = Sql.ToBoolean(rdr["IS_ADMIN"             ]);
										chkPORTAL_ONLY          .Checked = Sql.ToBoolean(rdr["PORTAL_ONLY"          ]);
										chkRECEIVE_NOTIFICATIONS.Checked = Sql.ToBoolean(rdr["RECEIVE_NOTIFICATIONS"]);
										// 12/04/2005 Paul.  Only allow the admin flag to be changed if the current user is an admin. 
										chkIS_ADMIN.Enabled = Security.IS_ADMIN;
										// 12/04/2005 Paul.  Save admin flag in ViewState to prevent hacking. 
										ViewState["IS_ADMIN"] = Sql.ToBoolean(rdr["IS_ADMIN"]);

										try
										{
											lstSTATUS.SelectedValue = Sql.ToString (rdr["STATUS"               ]);
										}
										catch(Exception ex)
										{
											SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
										}
										
										string sUSER_PREFERENCES = Sql.ToString(rdr["USER_PREFERENCES"]);
										if ( !Sql.IsEmptyString(sUSER_PREFERENCES) )
										{
											XmlDocument xml = SplendidInit.InitUserPreferences(sUSER_PREFERENCES);
											try
											{
												ViewState["USER_PREFERENCES"] = xml.OuterXml;
												// user_settings
												chkGRIDLINE.Checked = Sql.ToBoolean(XmlUtil.SelectSingleNode(xml, "gridline"));
												try
												{
													lstLANGUAGE.SelectedValue = L10N.NormalizeCulture(XmlUtil.SelectSingleNode(xml, "culture"));
													lstLANGUAGE_Changed(null, null);
												}
												catch(Exception ex)
												{
													SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
												}
												try
												{
													lstLANGUAGE.SelectedValue = XmlUtil.SelectSingleNode(xml, "theme");
												}
												catch(Exception ex)
												{
													SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
												}
												try
												{
													lstDATE_FORMAT.SelectedValue = XmlUtil.SelectSingleNode(xml, "dateformat");
												}
												catch(Exception ex)
												{
													SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
												}
												try
												{
													lstTIME_FORMAT.SelectedValue = XmlUtil.SelectSingleNode(xml, "timeformat");
												}
												catch(Exception ex)
												{
													SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
												}
												try
												{
													lstTIMEZONE.SelectedValue = XmlUtil.SelectSingleNode(xml, "timezone");
												}
												catch(Exception ex)
												{
													SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
												}
												try
												{
													lstCURRENCY.SelectedValue = XmlUtil.SelectSingleNode(xml, "currency_id");
												}
												catch(Exception ex)
												{
													SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
												}
												
												// mail_options
												new DynamicControl(this, "MAIL_FROMNAME"    ).Text    =               XmlUtil.SelectSingleNode(xml, "mail_fromname"        );
												new DynamicControl(this, "MAIL_FROMADDRESS" ).Text    =               XmlUtil.SelectSingleNode(xml, "mail_fromaddress"     );
												new DynamicControl(this, "MAIL_SENDTYPE"    ).Text    =               XmlUtil.SelectSingleNode(xml, "mail_sendtype"        );
												new DynamicControl(this, "MAIL_SMTPSERVER"  ).Text    =               XmlUtil.SelectSingleNode(xml, "mail_smtpserver"      );
												new DynamicControl(this, "MAIL_SMTPPORT"    ).Text    =               XmlUtil.SelectSingleNode(xml, "mail_smtpport"        );
												new DynamicControl(this, "MAIL_SMTPAUTH_REQ").Checked = Sql.ToBoolean(XmlUtil.SelectSingleNode(xml, "mail_smtpauth_req"    ));
												new DynamicControl(this, "MAIL_SMTPUSER"    ).Text    =               XmlUtil.SelectSingleNode(xml, "mail_smtpuser"        );
												new DynamicControl(this, "MAIL_SMTPPASS"    ).Text    =               XmlUtil.SelectSingleNode(xml, "mail_smtppass"        );
												
												ViewState["mail_smtppass"] = XmlUtil.SelectSingleNode(xml, "mail_smtppass");
												// 08/06/2005 Paul.  Never return password to user. 
												TextBox txtMAIL_SMTPPASS = FindControl("MAIL_SMTPPASS") as TextBox;
												if ( txtMAIL_SMTPPASS != null )
												{
													if ( !Sql.IsEmptyString(txtMAIL_SMTPPASS.Text) )
														txtMAIL_SMTPPASS.Text = sEMPTY_PASSWORD;
												}
												
												// 05/09/2006 Paul.  Initialize the numeric separators. 
												txtGROUP_SEPARATOR      .Text    =                XmlUtil.SelectSingleNode(xml, "num_grp_sep"         );
												txtDECIMAL_SEPARATOR    .Text    =                XmlUtil.SelectSingleNode(xml, "dec_sep"             );
												// 05/09/2006 Paul.  Check for empty strings as the user may have legacy data. 
												if ( Sql.IsEmptyString(txtGROUP_SEPARATOR.Text) )
													txtGROUP_SEPARATOR.Text   = SplendidDefaults.GroupSeparator();
												if ( Sql.IsEmptyString(txtDECIMAL_SEPARATOR.Text) )
													txtDECIMAL_SEPARATOR.Text = SplendidDefaults.DecimalSeparator();
												
												// freebusy
												// 08/05/2006 Paul.  Remove stub of unsupported code. Calendar Publish Key is not supported at this time. 
												//txtCALENDAR_PUBLISH_KEY .Text    =               XmlUtil.SelectSingleNode(xml, "calendar_publish_key" );
												//txtCALENDAR_PUBLISH_URL .Text    =               XmlUtil.SelectSingleNode(xml, "calendar_publish_url" );
												//txtCALENDAR_SEARCH_URL  .Text    =               XmlUtil.SelectSingleNode(xml, "calendar_search_url"  );
												// 08/05/2006 Paul.  Remove stub of unsupported code. Reminder is not supported at this time. 
												/*
												try
												{
													int nREMINDER_TIME = Sql.ToInteger(XmlUtil.SelectSingleNode(xml, "reminder_time"));
													if ( nREMINDER_TIME > 0 )
													{
														lstREMINDER_TIME.SelectedValue = nREMINDER_TIME.ToString();
														chkSHOULD_REMIND.Checked = true;
													}
												}
												catch(Exception ex)
												{
													SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
												}
												*/
											}
											catch(Exception ex)
											{
												SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
											}
										}
									}
								}
							}
						}
					}
					else
					{
						this.AppendEditViewFields(m_sMODULE + ".EditView"       , tblMain       , null);
						this.AppendEditViewFields(m_sMODULE + ".EditAddress"    , tblAddress    , null);
						this.AppendEditViewFields(m_sMODULE + ".EditMailOptions", tblMailOptions, null);

						try
						{
							lstTHEME.SelectedValue = SplendidDefaults.Theme();
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
						}
						try
						{
							string sDefaultLanguage = Sql.ToString(Request.ServerVariables["HTTP_ACCEPT_LANGUAGE"]);
							if ( Sql.IsEmptyString(sDefaultLanguage) )
								sDefaultLanguage = "en-US";
							lstLANGUAGE.SelectedValue = sDefaultLanguage;
						}
						catch(Exception ex)
						{
							SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
						}
						lstLANGUAGE_Changed(null, null);
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
			m_sMODULE = "Users";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields(m_sMODULE + ".EditView"       , tblMain       , null);
				this.AppendEditViewFields(m_sMODULE + ".EditAddress"    , tblAddress    , null);
				this.AppendEditViewFields(m_sMODULE + ".EditMailOptions", tblMailOptions, null);
			}
		}
		#endregion
	}
}
