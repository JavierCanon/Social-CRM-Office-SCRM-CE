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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Users
{
	/// <summary>
	/// Summary description for DetailView.
	/// </summary>
	public class DetailView : SplendidControl
	{
		protected _controls.ModuleHeader ctlModuleHeader;
		protected Administration.ACLRoles.AccessView ctlAccessView;
		// 03/08/2007 Paul.  We need to pass the MyAccount flag to the Roles and Teams control. 
		protected Users.Roles ctlRoles;
		protected Users.Teams ctlTeams;

		// main
		protected Label     lblError                        ;
		protected Guid      gID                             ;
		protected HtmlTable tblMain                         ;
		protected HtmlTable tblMailOptions                  ;

		protected Label     txtNAME                         ;
		protected Label     txtUSER_NAME                    ;
		protected Label     txtSTATUS                       ;
		// user_settings
		protected CheckBox  chkIS_ADMIN                     ;
		protected CheckBox  chkPORTAL_ONLY                  ;
		protected CheckBox  chkRECEIVE_NOTIFICATIONS        ;
		protected Label     txtLANGUAGE                     ;
		protected Label     txtDATEFORMAT                   ;
		protected Label     txtTIMEFORMAT                   ;
		protected Label     txtTIMEZONE                     ;
		// 08/05/2006 Paul.  Remove stub of unsupported code. Show Gridline is not supported at this time. 
		//protected CheckBox  chkGRIDLINE                     ;
		protected Label     txtCURRENCY                     ;
		protected Label     txtGROUP_SEPARATOR              ;
		protected Label     txtDECIMAL_SEPARATOR            ;
		// 08/05/2006 Paul.  Remove stub of unsupported code. Reminder is not supported at this time. 
		//protected CheckBox  chkREMINDER                     ;
		//protected Label     txtREMINDER_TIME                ;

		// freebusy
		// 08/05/2006 Paul.  Remove stub of unsupported code. Calendar Publish Key is not supported at this time. 
		//protected Label     txtCALENDAR_PUBLISH_KEY         ;
		//protected Label     txtCALENDAR_PUBLISH_URL         ;
		//protected Label     txtCALENDAR_SEARCH_URL          ;

		protected Button    btnDuplicate                    ;
		protected bool      bMyAccount                      ;

		protected LinkButton      btnReset                        ;
		protected Button          btnChangePassword               ;
		protected HtmlInputHidden txtOLD_PASSWORD                 ;
		protected HtmlInputHidden txtNEW_PASSWORD                 ;
		protected HtmlInputHidden txtCONFIRM_PASSWORD             ;

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
			try
			{
				if ( e.CommandName == "Edit" )
				{
					if ( bMyAccount )
						Response.Redirect("EditMyAccount.aspx");
					else
						Response.Redirect("edit.aspx?ID=" + gID.ToString());
				}
				else if ( e.CommandName == "Duplicate" )
				{
					Response.Redirect("edit.aspx?DuplicateID=" + gID.ToString());
				}
				else if ( e.CommandName == "Delete" )
				{
					SqlProcs.spUSERS_Delete(gID);
					Response.Redirect("default.aspx");
				}
				else if ( e.CommandName == "Cancel" )
				{
					Response.Redirect("default.aspx");
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
			SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
			// 06/04/2006 Paul.  Visibility is already controlled by the ASPX page, but it is probably a good idea to skip the load. 
			// 07/11/2006 Paul.  Users must be able to view and edit their own settings. 
			this.Visible = bMyAccount || SplendidCRM.Security.IS_ADMIN;  //(SplendidCRM.Security.GetUserAccess(m_sMODULE, "view") >= 0);
			if ( !this.Visible )
				return;

			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				if ( bMyAccount )
				{
					// 11/19/2005 Paul.  SugarCRM 3.5.0 allows administrator to duplicate itself. 
					btnDuplicate.Visible = Security.IS_ADMIN;
					gID = Security.USER_ID;
				}
				ctlAccessView.USER_ID = gID;

				// 12/06/2005 Paul.  The password button is only visible if not windows authentication or Admin.
				// The reason to allow the admin to change a password is so that the admin can prepare to turn off windows authentication. 
				btnChangePassword.Visible = !Security.IsWindowsAuthentication() || Security.IS_ADMIN;
				btnReset         .Visible = Security.IS_ADMIN;
				if ( !Sql.IsEmptyString(txtNEW_PASSWORD.Value) )
				{
					bool bValidOldPassword = false;
					if ( !Security.IS_ADMIN )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							// 07/17/2006 Paul.  The USER_HASH has been removed from the main vwUSERS view to prevent its use in reports. 
							sSQL = "select *                     " + ControlChars.CrLf
							     + "  from vwUSERS_Login         " + ControlChars.CrLf
							     + " where ID        = @ID       " + ControlChars.CrLf
							     + "   and USER_HASH = @USER_HASH" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								Sql.AddParameter(cmd, "@ID", gID);
								Sql.AddParameter(cmd, "@USER_HASH", Security.HashPassword(txtOLD_PASSWORD.Value));
								con.Open();
								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										bValidOldPassword = true;
									}
								}
							}
						}
						if ( !bValidOldPassword )
						{
							lblError.Text = L10n.Term("Users.ERR_PASSWORD_INCORRECT_OLD");
						}
					}
					if ( bValidOldPassword || Security.IS_ADMIN )
					{
						if ( txtNEW_PASSWORD.Value == txtCONFIRM_PASSWORD.Value )
						{
							SqlProcs.spUSERS_PasswordUpdate(gID, Security.HashPassword(txtNEW_PASSWORD.Value));
							if ( bMyAccount )
								Response.Redirect("MyAccount.aspx");
							else
								Response.Redirect("view.aspx?ID=" + gID.ToString());
						}
						else
						{
							lblError.Text = L10n.Term("Users.ERR_REENTER_PASSWORDS") ;
						}
					}
				}
				if ( !IsPostBack )
				{
					// 05/09/2006 Paul.  We need to always initialize the separators, just in case the user is new. 
					txtGROUP_SEPARATOR.Text   = SplendidDefaults.GroupSeparator();
					txtDECIMAL_SEPARATOR.Text = SplendidDefaults.DecimalSeparator();
					if ( !Sql.IsEmptyGuid(gID) )
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
								Sql.AddParameter(cmd, "@ID", gID);
								con.Open();

								if ( bDebug )
									RegisterClientScriptBlock("SQLCode", Sql.ClientScriptBlock(cmd));

								using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
								{
									if ( rdr.Read() )
									{
										ctlModuleHeader.Title = Sql.ToString(rdr["FULL_NAME"]) + " (" + Sql.ToString(rdr["USER_NAME"]) + ")";
										SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
										Utils.UpdateTracker(Page, m_sMODULE, gID, ctlModuleHeader.Title);
										
										// main
										txtNAME                 .Text = Sql.ToString(rdr["FULL_NAME"]);
										txtUSER_NAME            .Text = Sql.ToString(rdr["USER_NAME"]);
										txtSTATUS               .Text    = Sql.ToString(L10n.Term(".user_status_dom."    , rdr["STATUS"         ]));
										// user_settings
										chkIS_ADMIN             .Checked = Sql.ToBoolean(rdr["IS_ADMIN"             ]);
										chkPORTAL_ONLY          .Checked = Sql.ToBoolean(rdr["PORTAL_ONLY"          ]);
										chkRECEIVE_NOTIFICATIONS.Checked = Sql.ToBoolean(rdr["RECEIVE_NOTIFICATIONS"]);

										this.AppendDetailViewFields(m_sMODULE + ".DetailView" , tblMain       , rdr);
										// 08/05/2006 Paul.  MailOptions are populated manually. 
										this.AppendDetailViewFields(m_sMODULE + ".MailOptions", tblMailOptions, null);
										// 01/20/2008 Paul.  The mail options panel is manually populated. 
										new DynamicControl(this, "EMAIL1").Text = Sql.ToString (rdr["EMAIL1"]);
										new DynamicControl(this, "EMAIL2").Text = Sql.ToString (rdr["EMAIL2"]);
										
										string sUSER_PREFERENCES = Sql.ToString(rdr["USER_PREFERENCES"]);
										if ( !Sql.IsEmptyString(sUSER_PREFERENCES) )
										{
											XmlDocument xml = SplendidInit.InitUserPreferences(sUSER_PREFERENCES);
											try
											{
												// user_settings
												txtLANGUAGE.Text = L10N.NormalizeCulture(XmlUtil.SelectSingleNode(xml, "culture"));
												try
												{
													DataView vwLanguages = new DataView(SplendidCache.Languages());
													vwLanguages.RowFilter = "NAME = '" + txtLANGUAGE.Text + "'";
													if ( vwLanguages.Count > 0 )
														txtLANGUAGE.Text = Sql.ToString(vwLanguages[0]["NATIVE_NAME"]);
												}
												catch(Exception ex)
												{
													SplendidError.SystemWarning(new StackTrace(true).GetFrame(0), ex);
												}
												txtDATEFORMAT           .Text    =               XmlUtil.SelectSingleNode(xml, "dateformat"           );
												txtTIMEFORMAT           .Text    =               XmlUtil.SelectSingleNode(xml, "timeformat"           );
												// 08/05/2006 Paul.  Remove stub of unsupported code. Show Gridline is not supported at this time. 
												//chkGRIDLINE             .Checked = Sql.ToBoolean(XmlUtil.SelectSingleNode(xml, "gridline"             ));
												// mail_options
												new DynamicControl(this, "MAIL_FROMNAME"    ).Text    =               XmlUtil.SelectSingleNode(xml, "mail_fromname"        );
												new DynamicControl(this, "MAIL_FROMADDRESS" ).Text    =               XmlUtil.SelectSingleNode(xml, "mail_fromaddress"     );
												new DynamicControl(this, "MAIL_SENDTYPE"    ).Text    =               XmlUtil.SelectSingleNode(xml, "mail_sendtype"        );
												new DynamicControl(this, "MAIL_SMTPSERVER"  ).Text    =               XmlUtil.SelectSingleNode(xml, "mail_smtpserver"      );
												new DynamicControl(this, "MAIL_SMTPPORT"    ).Text    =               XmlUtil.SelectSingleNode(xml, "mail_smtpport"        );
												new DynamicControl(this, "MAIL_SMTPAUTH_REQ").Checked = Sql.ToBoolean(XmlUtil.SelectSingleNode(xml, "mail_smtpauth_req"    ));
												new DynamicControl(this, "MAIL_SMTPUSER"    ).Text    =               XmlUtil.SelectSingleNode(xml, "mail_smtpuser"        );
												// freebusy
												// 08/05/2006 Paul.  Remove stub of unsupported code. Calendar Publish Key is not supported at this time. 
												//txtCALENDAR_PUBLISH_KEY .Text    =               XmlUtil.SelectSingleNode(xml, "calendar_publish_key" );
												//txtCALENDAR_PUBLISH_URL .Text    =               XmlUtil.SelectSingleNode(xml, "calendar_publish_url" );
												//txtCALENDAR_SEARCH_URL  .Text    =               XmlUtil.SelectSingleNode(xml, "calendar_search_url"  );
												
												// 05/09/2006 Paul.  Initialize the numeric separators. 
												txtGROUP_SEPARATOR      .Text    =               XmlUtil.SelectSingleNode(xml, "num_grp_sep"          );
												txtDECIMAL_SEPARATOR    .Text    =               XmlUtil.SelectSingleNode(xml, "dec_sep"              );
												// 05/09/2006 Paul.  Check for empty strings as the user may have legacy data. 
												if ( Sql.IsEmptyString(txtGROUP_SEPARATOR.Text) )
													txtGROUP_SEPARATOR.Text   = SplendidDefaults.GroupSeparator();
												if ( Sql.IsEmptyString(txtDECIMAL_SEPARATOR.Text) )
													txtDECIMAL_SEPARATOR.Text = SplendidDefaults.DecimalSeparator();

												string sTIMEZONE = XmlUtil.SelectSingleNode(xml, "timezone");
												DataView vwTimezones = new DataView(SplendidCache.Timezones());
												vwTimezones.RowFilter    = "ID = '" + sTIMEZONE + "'";
												if ( vwTimezones.Count > 0 )
													txtTIMEZONE.Text = Sql.ToString(vwTimezones[0]["NAME"]);

												string sCURRENCY = XmlUtil.SelectSingleNode(xml, "currency_id");
												DataView vwCurrencies = new DataView(SplendidCache.Currencies());
												vwCurrencies.RowFilter    = "ID = '" + sCURRENCY + "'";
												if ( vwCurrencies.Count > 0 )
													txtCURRENCY.Text = Sql.ToString(vwCurrencies[0]["NAME_SYMBOL"]);
												// 08/05/2006 Paul.  Remove stub of unsupported code. Reminder is not supported at this time. 
												/*
												try
												{
													int nREMINDER_TIME = Sql.ToInteger(XmlUtil.SelectSingleNode(xml, "reminder_time"));
													if ( nREMINDER_TIME > 0 )
													{
														txtREMINDER_TIME.Text = L10n.Term(".reminder_time_options." + nREMINDER_TIME.ToString());
														chkREMINDER.Checked = true;
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
										//txtDESCRIPTION.Text = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String("YToxODp7czo4OiJncmlkbGluZSI7czozOiJvZmYiO3M6ODoibWF4X3RhYnMiO3M6MjoiMTIiO3M6MTI6ImRpc3BsYXlfdGFicyI7YToxNTp7aTowO3M6NDoiSG9tZSI7aToxO3M6NzoiaUZyYW1lcyI7aToyO3M6ODoiQ2FsZW5kYXIiO2k6MztzOjEwOiJBY3Rpdml0aWVzIjtpOjQ7czo4OiJBY2NvdW50cyI7aTo1O3M6NToiTGVhZHMiO2k6NjtzOjEzOiJPcHBvcnR1bml0aWVzIjtpOjc7czo1OiJDYXNlcyI7aTo4O3M6NDoiQnVncyI7aTo5O3M6OToiRG9jdW1lbnRzIjtpOjEwO3M6NjoiRW1haWxzIjtpOjExO3M6OToiQ2FtcGFpZ25zIjtpOjEyO3M6NzoiUHJvamVjdCI7aToxMztzOjU6IkZlZWRzIjtpOjE0O3M6OToiRGFzaGJvYXJkIjt9czoxMzoicmVtaW5kZXJfdGltZSI7czozOiI5MDAiO3M6NToidGltZWYiO3M6MzoiSDppIjtzOjg6ImN1cnJlbmN5IjtzOjM6Ii05OSI7czo1OiJkYXRlZiI7czo1OiJZLW0tZCI7czo1OiJ0aW1leiI7czoxOiIwIjtzOjEzOiJtYWlsX2Zyb21uYW1lIjtzOjQ6IlBhdWwiO3M6MTY6Im1haWxfZnJvbWFkZHJlc3MiO3M6MTM6InBhdWxAcm9ueS5jb20iO3M6MTM6Im1haWxfc2VuZHR5cGUiO3M6NDoiU01UUCI7czoxNToibWFpbF9zbXRwc2VydmVyIjtzOjM6Im5zMSI7czoxMzoibWFpbF9zbXRwcG9ydCI7czoyOiIyMyI7czoxMzoibWFpbF9zbXRwdXNlciI7czo4OiJwYXVscm9ueSI7czoxMzoibWFpbF9zbXRwcGFzcyI7czo3OiJwb2NrZXQxIjtzOjE3OiJtYWlsX3NtdHBhdXRoX3JlcSI7czowOiIiO3M6MTY6Im1haWxfcG9wYXV0aF9yZXEiO3M6MDoiIjtzOjIwOiJjYWxlbmRhcl9wdWJsaXNoX2tleSI7czoxMToicHVibGlzaCBoZXkiO30="));
									}
								}
							}
						}
					}
				}
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				lblError.Text = ex.Message;
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
			m_sMODULE = "Users";
			SetMenu(m_sMODULE);

			// 03/08/2007 Paul.  We need to disable the buttons unless the user is an administrator. 
			ctlRoles.MyAccount = bMyAccount;
			ctlTeams.MyAccount = bMyAccount;
		}
		#endregion
	}
}

