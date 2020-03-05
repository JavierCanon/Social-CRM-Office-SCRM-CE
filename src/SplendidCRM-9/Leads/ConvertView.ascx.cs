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
using SplendidCRM._controls;

namespace SplendidCRM.Leads
{
	/// <summary>
	///		Summary description for ConvertView.
	/// </summary>
	public class ConvertView : SplendidControl
	{
		protected SplendidCRM._controls.ModuleHeader ctlModuleHeader;
		protected SplendidCRM._controls.EditButtons  ctlEditButtons ;

		protected Guid            gID                             ;
		protected HtmlTable       tblMain                         ;
		protected Label           lblDATEFORMAT                   ;

		protected TextBox                txtCONTACT_NOTES_NAME                ;
		protected RequiredFieldValidator reqCONTACT_NOTES_NAME                ;
		protected TextBox                txtCONTACT_NOTES_NAME_DESCRIPTION    ;


		protected CheckBox               chkCreateAccount                     ;
		protected HtmlInputHidden        txtSELECT_ACCOUNT_ID                 ;
		protected RequiredFieldValidatorForHiddenInputs reqSELECT_ACCOUNT_ID  ;
		protected TextBox                txtSELECT_ACCOUNT_NAME               ;
		protected TextBox                txtACCOUNT_NAME                      ;
		protected RequiredFieldValidator reqACCOUNT_NAME                      ;
		protected TextBox                txtACCOUNT_DESCRIPTION               ;
		protected TextBox                txtACCOUNT_PHONE_WORK                ;
		protected TextBox                txtACCOUNT_WEBSITE                   ;
		protected TextBox                txtACCOUNT_NOTES_NAME                ;
		protected RequiredFieldValidator reqACCOUNT_NOTES_NAME                ;
		protected TextBox                txtACCOUNT_NOTES_NAME_DESCRIPTION    ;

		protected CheckBox               chkCreateOpportunity                 ;
		protected TextBox                txtOPPORTUNITY_NAME                  ;
		protected RequiredFieldValidator reqOPPORTUNITY_NAME                  ;
		protected TextBox                txtOPPORTUNITY_DESCRIPTION           ;
		protected DatePicker             ctlOPPORTUNITY_DATE_CLOSED           ;
		protected DropDownList           lstOPPORTUNITY_SALES_STAGE           ;
		protected TextBox                txtOPPORTUNITY_AMOUNT                ;
		protected RequiredFieldValidator reqOPPORTUNITY_AMOUNT                ;
		protected TextBox                txtOPPORTUNITY_NOTES_NAME            ;
		protected TextBox                txtOPPORTUNITY_NOTES_NAME_DESCRIPTION;
		protected RequiredFieldValidator reqOPPORTUNITY_NOTES_NAME            ;

		protected CheckBox               chkCreateAppointment                 ;
		protected RadioButton            radScheduleCall                      ;
		protected RadioButton            radScheduleMeeting                   ;
		protected TextBox                txtAPPOINTMENT_NAME                  ;
		protected TextBox                txtAPPOINTMENT_DESCRIPTION           ;
		protected RequiredFieldValidator reqAPPOINTMENT_NAME                  ;
		protected DatePicker             ctlAPPOINTMENT_DATE_START            ;
		protected TextBox                txtAPPOINTMENT_TIME_START            ;
		protected RequiredFieldValidator reqAPPOINTMENT_TIME_START            ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				// 01/31/2006 Paul.  Enable validator before validating page. 
				this.ValidateEditViewFields(m_sMODULE + ".ConvertView");
				if ( Page.IsValid )
				{
					// 02/27/2006 Paul.  Fix condition on notes.  Enable only if text exists. 
					txtCONTACT_NOTES_NAME_DESCRIPTION.Text = txtCONTACT_NOTES_NAME_DESCRIPTION.Text.Trim();
					reqCONTACT_NOTES_NAME    .Enabled = !Sql.IsEmptyString(txtCONTACT_NOTES_NAME_DESCRIPTION.Text    );
					reqCONTACT_NOTES_NAME    .Validate();

					txtACCOUNT_NOTES_NAME_DESCRIPTION.Text = txtACCOUNT_NOTES_NAME_DESCRIPTION.Text.Trim();
					reqACCOUNT_NOTES_NAME    .Enabled = !Sql.IsEmptyString(txtACCOUNT_NOTES_NAME_DESCRIPTION.Text    );
					reqACCOUNT_NOTES_NAME    .Validate();
					
					txtOPPORTUNITY_NOTES_NAME_DESCRIPTION.Text = txtOPPORTUNITY_NOTES_NAME_DESCRIPTION.Text.Trim();
					reqOPPORTUNITY_NOTES_NAME.Enabled = !Sql.IsEmptyString(txtOPPORTUNITY_NOTES_NAME_DESCRIPTION.Text);
					reqOPPORTUNITY_NOTES_NAME.Validate();

					// 01/31/2006 Paul.  SelectAccount is required if not creating an account but creating an opportunity. 
					reqSELECT_ACCOUNT_ID     .Enabled = !chkCreateAccount.Checked && chkCreateOpportunity.Checked;
					reqSELECT_ACCOUNT_ID     .Validate();
					reqACCOUNT_NAME          .Enabled = chkCreateAccount.Checked ;
					reqACCOUNT_NAME          .Validate();
					reqOPPORTUNITY_NAME      .Enabled = chkCreateOpportunity.Checked;
					reqOPPORTUNITY_NAME      .Validate();
					reqOPPORTUNITY_AMOUNT    .Enabled = chkCreateOpportunity.Checked;
					reqOPPORTUNITY_AMOUNT    .Validate();
					reqAPPOINTMENT_NAME      .Enabled = chkCreateAppointment.Checked;
					reqAPPOINTMENT_NAME      .Validate();
					reqAPPOINTMENT_TIME_START.Enabled = chkCreateAppointment.Checked;
					reqAPPOINTMENT_TIME_START.Validate();
					if ( chkCreateAppointment.Checked )
						ctlAPPOINTMENT_DATE_START.Validate();
				}
				if ( Page.IsValid )
				{
					string sCUSTOM_MODULE = "LEADS";
					DataTable dtCustomFields = SplendidCache.FieldsMetaData_Validated(sCUSTOM_MODULE);
					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection con = dbf.CreateConnection() )
					{
						con.Open();
						using ( IDbTransaction trn = con.BeginTransaction() )
						{
							try
							{
								Guid gCONTACT_ID     = Guid.Empty;
								Guid gACCOUNT_ID     = Guid.Empty;
								Guid gOPPORTUNITY_ID = Guid.Empty;
								Guid gAPPOINTMENT_ID = Guid.Empty;
								
								// 01/31/2006 Paul.  Create the contact first so that it can be used as the parent of the related records. 
								// We would normally create the related records second, but then it will become a pain to update the Contact ACCOUNT_ID field. 
								SqlProcs.spCONTACTS_New
									( ref gCONTACT_ID
									, new DynamicControl(this, "FIRST_NAME"                ).Text
									, new DynamicControl(this, "LAST_NAME"                 ).Text
									, new DynamicControl(this, "PHONE_WORK"                ).Text
									, new DynamicControl(this, "EMAIL1"                    ).Text
									, trn
									);
								
								if ( chkCreateAccount.Checked )
								{
									SqlProcs.spACCOUNTS_Update
										( ref gACCOUNT_ID
										, Security.USER_ID
										, txtACCOUNT_NAME.Text
										, String.Empty
										, Guid.Empty
										, String.Empty
										, String.Empty
										, Sql.ToString(ViewState["PHONE_FAX"                  ])
										, Sql.ToString(ViewState["BILLING_ADDRESS_STREET"     ])
										, Sql.ToString(ViewState["BILLING_ADDRESS_CITY"       ])
										, Sql.ToString(ViewState["BILLING_ADDRESS_STATE"      ])
										, Sql.ToString(ViewState["BILLING_ADDRESS_POSTALCODE" ])
										, Sql.ToString(ViewState["BILLING_ADDRESS_COUNTRY"    ])
										, txtACCOUNT_DESCRIPTION.Text
										, String.Empty
										, txtACCOUNT_PHONE_WORK.Text
										, Sql.ToString(ViewState["PHONE_OTHER"                ])
										, Sql.ToString(ViewState["EMAIL1"                     ])
										, Sql.ToString(ViewState["EMAIL2"                     ])
										, txtACCOUNT_WEBSITE.Text
										, String.Empty
										, String.Empty
										, String.Empty
										, String.Empty
										, Sql.ToString(ViewState["SHIPPING_ADDRESS_STREET"    ])
										, Sql.ToString(ViewState["SHIPPING_ADDRESS_CITY"      ])
										, Sql.ToString(ViewState["SHIPPING_ADDRESS_STATE"     ])
										, Sql.ToString(ViewState["SHIPPING_ADDRESS_POSTALCODE"])
										, Sql.ToString(ViewState["SHIPPING_ADDRESS_COUNTRY"   ])
										, new DynamicControl(this, "TEAM_ID"         ).ID
										, trn
										);
									
									if ( !Sql.IsEmptyString(txtACCOUNT_NOTES_NAME.Text) )
									{
										Guid gNOTE_ID = Guid.Empty;
										SqlProcs.spNOTES_Update
											( ref gNOTE_ID
											, txtACCOUNT_NOTES_NAME.Text
											, "Accounts"
											, gACCOUNT_ID
											, Guid.Empty
											, txtACCOUNT_NOTES_NAME_DESCRIPTION.Text
											, new DynamicControl(this, "TEAM_ID"         ).ID
											, trn
											);
									}
								}
								else
								{
									gACCOUNT_ID = Sql.ToGuid(txtSELECT_ACCOUNT_ID.Value);
								}
								if ( chkCreateOpportunity.Checked )
								{
									SqlProcs.spOPPORTUNITIES_Update
										( ref gOPPORTUNITY_ID
										, Security.USER_ID
										, gACCOUNT_ID
										, txtOPPORTUNITY_NAME.Text
										, String.Empty
										, new DynamicControl(this, "LEAD_SOURCE"     ).SelectedValue
										, Sql.ToDecimal(txtOPPORTUNITY_AMOUNT.Text)
										, Guid.Empty
										, T10n.ToServerTime(ctlOPPORTUNITY_DATE_CLOSED.Value)
										, String.Empty
										, lstOPPORTUNITY_SALES_STAGE.SelectedValue
										, (float) 0.0
										, txtOPPORTUNITY_DESCRIPTION.Text
										, String.Empty
										, Guid.Empty
										, String.Empty  // 11/02/2006 Paul.  ACCOUNT_NAME is only used for import. 
										, new DynamicControl(this, "TEAM_ID"         ).ID
										, trn
										);
									if ( !Sql.IsEmptyString(txtOPPORTUNITY_NOTES_NAME.Text) )
									{
										Guid gNOTE_ID = Guid.Empty;
										SqlProcs.spNOTES_Update
											( ref gNOTE_ID
											, txtOPPORTUNITY_NOTES_NAME.Text
											, "Opportunities"
											, gOPPORTUNITY_ID
											, Guid.Empty
											, txtOPPORTUNITY_NOTES_NAME_DESCRIPTION.Text
											, new DynamicControl(this, "TEAM_ID"         ).ID
											, trn
											);
									}
									// 03/04/2006 Paul.  Must be included in the transaction, otherwise entire operation will fail with a timeout message. 
									SqlProcs.spOPPORTUNITIES_CONTACTS_Update(gOPPORTUNITY_ID, gCONTACT_ID, String.Empty, trn);
								}
								if ( chkCreateAppointment.Checked )
								{
									DateTime dtDATE_START = T10n.ToServerTime(Sql.ToDateTime(ctlAPPOINTMENT_DATE_START.DateText + " " + txtAPPOINTMENT_TIME_START.Text));
									if ( radScheduleCall.Checked )
									{
										SqlProcs.spCALLS_Update
											( ref gAPPOINTMENT_ID
											, Security.USER_ID
											, txtAPPOINTMENT_NAME.Text
											, 1
											, 0
											, dtDATE_START
											, "Accounts"
											, Guid.Empty
											, "Planned"
											, "Outbound"
											, -1
											, txtAPPOINTMENT_DESCRIPTION.Text
											, gCONTACT_ID.ToString()         // 01/31/2006 Paul.  This is were we relate this call to the contact. 
											, new DynamicControl(this, "TEAM_ID"         ).ID
											, trn
											);
									}
									else
									{
										SqlProcs.spMEETINGS_Update
											( ref gAPPOINTMENT_ID
											, Security.USER_ID
											, txtAPPOINTMENT_NAME.Text
											, String.Empty
											, 1
											, 0
											, dtDATE_START
											, "Planned"
											, "Accounts"
											, Guid.Empty
											, -1
											, txtAPPOINTMENT_DESCRIPTION.Text
											, gCONTACT_ID.ToString()         // 01/31/2006 Paul.  This is were we relate this meeting to the contact. 
											, new DynamicControl(this, "TEAM_ID"         ).ID
											, trn
											);
									}
								}
								SqlProcs.spCONTACTS_ConvertLead
									( ref gCONTACT_ID
									, gID                                   // 01/31/2006 Paul.  Update the Lead with this contact. 
									, Security.USER_ID
									, new DynamicControl(this, "SALUTATION"                ).SelectedValue
									, new DynamicControl(this, "FIRST_NAME"                ).Text
									, new DynamicControl(this, "LAST_NAME"                 ).Text
									, gACCOUNT_ID
									, new DynamicControl(this, "LEAD_SOURCE"               ).SelectedValue
									, new DynamicControl(this, "TITLE"                     ).Text
									, new DynamicControl(this, "DEPARTMENT"                ).Text
									, new DynamicControl(this, "DO_NOT_CALL"               ).Checked
									, new DynamicControl(this, "PHONE_HOME"                ).Text
									, new DynamicControl(this, "PHONE_MOBILE"              ).Text
									, new DynamicControl(this, "PHONE_WORK"                ).Text
									, new DynamicControl(this, "PHONE_OTHER"               ).Text
									, new DynamicControl(this, "PHONE_FAX"                 ).Text
									, new DynamicControl(this, "EMAIL1"                    ).Text
									, new DynamicControl(this, "EMAIL2"                    ).Text
									, new DynamicControl(this, "EMAIL_OPT_OUT"             ).Checked
									, new DynamicControl(this, "INVALID_EMAIL"             ).Checked
									, new DynamicControl(this, "PRIMARY_ADDRESS_STREET"    ).Text
									, new DynamicControl(this, "PRIMARY_ADDRESS_CITY"      ).Text
									, new DynamicControl(this, "PRIMARY_ADDRESS_STATE"     ).Text
									, new DynamicControl(this, "PRIMARY_ADDRESS_POSTALCODE").Text
									, new DynamicControl(this, "PRIMARY_ADDRESS_COUNTRY"   ).Text
									, Sql.ToString(ViewState["ALT_ADDRESS_STREET"    ])
									, Sql.ToString(ViewState["ALT_ADDRESS_CITY"      ])
									, Sql.ToString(ViewState["ALT_ADDRESS_STATE"     ])
									, Sql.ToString(ViewState["ALT_ADDRESS_POSTALCODE"])
									, Sql.ToString(ViewState["ALT_ADDRESS_COUNTRY"   ])
									, new DynamicControl(this, "DESCRIPTION"               ).Text
									, gOPPORTUNITY_ID
									, txtOPPORTUNITY_NAME.Text
									, txtOPPORTUNITY_AMOUNT.Text
									, Sql.ToGuid(ViewState["CAMPAIGN_ID"   ])  // 09/10/2007 Paul.  Add Campaign tracking. 
									, new DynamicControl(this, "TEAM_ID"         ).ID
									, trn
									);
								if ( !Sql.IsEmptyString(txtCONTACT_NOTES_NAME.Text) )
								{
									Guid gNOTE_ID = Guid.Empty;
									SqlProcs.spNOTES_Update
										( ref gNOTE_ID
										, txtCONTACT_NOTES_NAME.Text
										, String.Empty
										, Guid.Empty
										, gCONTACT_ID
										, txtCONTACT_NOTES_NAME_DESCRIPTION.Text
										, new DynamicControl(this, "TEAM_ID"         ).ID
										, trn
										);
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
					Response.Redirect("view.aspx?ID=" + gID.ToString());
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
			try
			{
				gID = Sql.ToGuid(Request["ID"]);
				SetPageTitle(L10n.Term(".moduleList." + m_sMODULE));
				if ( !IsPostBack )
				{
					lblDATEFORMAT.Text = "(" + Session["USER_SETTINGS/DATEFORMAT"] + ")";
					lstOPPORTUNITY_SALES_STAGE.DataSource = SplendidCache.List("sales_stage_dom");
					lstOPPORTUNITY_SALES_STAGE.DataBind();

					chkCreateAccount    .Attributes.Add("onclick", "return ToggleCreateAccount();");
					chkCreateOpportunity.Attributes.Add("onclick", "return toggleDisplay('divCreateOpportunity');");
					chkCreateAppointment.Attributes.Add("onclick", "return toggleDisplay('divCreateAppointment');");

					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *              " + ControlChars.CrLf
							     + "  from vwLEADS_Convert" + ControlChars.CrLf
							     + " where ID = @ID       " + ControlChars.CrLf;
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
										ctlModuleHeader.Title = L10n.Term("Leads.LBL_CONVERTLEAD");
										SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
										
										txtACCOUNT_NAME          .Text  = Sql.ToString(rdr["ACCOUNT_NAME"]);
										txtACCOUNT_PHONE_WORK    .Text  = Sql.ToString(rdr["PHONE_WORK"  ]);
										// 01/31/2006 Paul.  Default start date and time is now. 
										ctlAPPOINTMENT_DATE_START.Value = T10n.FromServerTime(DateTime.Now);
										txtAPPOINTMENT_TIME_START.Text  = T10n.FromServerTime(DateTime.Now).ToShortTimeString();
										
										this.AppendEditViewFields(m_sMODULE + ".ConvertView", tblMain, rdr);
										// 01/31/2006 Paul.  Save all data to be used later. 
										for ( int i=0; i < rdr.FieldCount; i++ )
										{
											ViewState[rdr.GetName(i)] = rdr.GetValue(i);
										}
									}
								}
							}
						}
					}
					else
					{
						this.AppendEditViewFields(m_sMODULE + ".ConvertView", tblMain, null);
					}
				}
				else
				{
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					ctlModuleHeader.Title = L10n.Term("Leads.LBL_CONVERTLEAD");
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
			m_sMODULE = "Leads";
			if ( IsPostBack )
			{
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields(m_sMODULE + ".ConvertView", tblMain, null);
			}
		}
		#endregion
	}
}
