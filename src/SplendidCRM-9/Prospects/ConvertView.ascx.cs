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

namespace SplendidCRM.Prospects
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
		protected HtmlTable       tblAddress                      ;
		protected HtmlTable       tblDescription                  ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				// 01/16/2006 Paul.  Enable validator before validating page. 
				this.ValidateEditViewFields(m_sMODULE + ".ConvertView"       );
				this.ValidateEditViewFields(m_sMODULE + ".ConvertAddress"    );
				this.ValidateEditViewFields(m_sMODULE + ".ConvertDescription");
				if ( Page.IsValid )
				{
					Guid gLEAD_ID = Guid.Empty;
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
								// 04/24/2006 Paul.  Upgrade to SugarCRM 4.2 Schema. 
								// 12/29/2007 Paul.  TEAM_ID is now in the stored procedure. 
								SqlProcs.spLEADS_ConvertProspect
									( ref gLEAD_ID
									, gID                                   // 01/31/2006 Paul.  Update the Prospect with this lead. 
									, new DynamicControl(this, "ASSIGNED_USER_ID"          ).ID
									, new DynamicControl(this, "SALUTATION"                ).SelectedValue
									, new DynamicControl(this, "FIRST_NAME"                ).Text
									, new DynamicControl(this, "LAST_NAME"                 ).Text
									, new DynamicControl(this, "TITLE"                     ).Text
									, new DynamicControl(this, "REFERED_BY"                ).Text
									, new DynamicControl(this, "LEAD_SOURCE"               ).SelectedValue
									, new DynamicControl(this, "LEAD_SOURCE_DESCRIPTION"   ).Text
									, new DynamicControl(this, "STATUS"                    ).SelectedValue
									, new DynamicControl(this, "STATUS_DESCRIPTION"        ).Text
									, new DynamicControl(this, "DEPARTMENT"                ).Text
									, Guid.Empty  // 06/24/2005. REPORTS_TO_ID is not used in version 3.0. 
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
									, new DynamicControl(this, "ALT_ADDRESS_STREET"        ).Text
									, new DynamicControl(this, "ALT_ADDRESS_CITY"          ).Text
									, new DynamicControl(this, "ALT_ADDRESS_STATE"         ).Text
									, new DynamicControl(this, "ALT_ADDRESS_POSTALCODE"    ).Text
									, new DynamicControl(this, "ALT_ADDRESS_COUNTRY"       ).Text
									, new DynamicControl(this, "DESCRIPTION"               ).Text
									, new DynamicControl(this, "ACCOUNT_NAME"              ).Text
									, new DynamicControl(this, "CAMPAIGN_ID"               ).ID
									, new DynamicControl(this, "TEAM_ID"                   ).ID
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
					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *                  " + ControlChars.CrLf
							     + "  from vwPROSPECTS_Convert" + ControlChars.CrLf
							     + " where ID = @ID           " + ControlChars.CrLf;
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
										ctlModuleHeader.Title = L10n.Term("Prospects.LBL_CONVERTPROSPECT");
										SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
										
										this.AppendEditViewFields(m_sMODULE + ".ConvertView"       , tblMain       , rdr);
										this.AppendEditViewFields(m_sMODULE + ".ConvertAddress"    , tblAddress    , rdr);
										this.AppendEditViewFields(m_sMODULE + ".ConvertDescription", tblDescription, rdr);
									}
								}
							}
						}
					}
					else
					{
						this.AppendEditViewFields(m_sMODULE + ".ConvertView"       , tblMain       , null);
						this.AppendEditViewFields(m_sMODULE + ".ConvertAddress"    , tblAddress    , null);
						this.AppendEditViewFields(m_sMODULE + ".ConvertDescription", tblDescription, null);
					}
				}
				else
				{
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					ctlModuleHeader.Title = L10n.Term("Prospects.LBL_CONVERTPROSPECT");
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
			m_sMODULE = "Prospects";
			if ( IsPostBack )
			{
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields(m_sMODULE + ".ConvertView"       , tblMain       , null);
				this.AppendEditViewFields(m_sMODULE + ".ConvertAddress"    , tblAddress    , null);
				this.AppendEditViewFields(m_sMODULE + ".ConvertDescription", tblDescription, null);
			}
		}
		#endregion
	}
}
