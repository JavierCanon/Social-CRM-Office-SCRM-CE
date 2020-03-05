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
 * Portions created by SplendidCRM Software are Copyright (C) 2007 SplendidCRM Software, Inc. All Rights Reserved.
 * Contributor(s): ______________________________________.
 *********************************************************************************************************************/
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Globalization;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Payments
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.ModuleHeader ctlModuleHeader   ;
		protected _controls.EditButtons  ctlEditButtons    ;
		protected AllocationsView        ctlAllocationsView;

		protected Guid            gID                   ;
		protected HtmlTable       tblMain               ;
		
		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			if ( e.CommandName == "Save" )
			{
				this.ValidateEditViewFields(m_sMODULE + ".EditView");
				if ( Page.IsValid )
				{
					string sCUSTOM_MODULE = "PAYMENTS";
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
							sSQL = "select *              " + ControlChars.CrLf
							     + "  from vwPAYMENTS_Edit" + ControlChars.CrLf;
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
								// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
								float fEXCHANGE_RATE = new DynamicControl(ctlAllocationsView, rowCurrent, "EXCHANGE_RATE").FloatValue;
								// 12/29/2007 Paul.  TEAM_ID is now in the stored procedure. 
								SqlProcs.spPAYMENTS_Update
									( ref gID
									, new DynamicControl(this, rowCurrent, "ASSIGNED_USER_ID"  ).ID
									, new DynamicControl(this, rowCurrent, "ACCOUNT_ID"        ).ID
									, new DynamicControl(this, rowCurrent, "PAYMENT_DATE"      ).DateValue
									, new DynamicControl(this, rowCurrent, "PAYMENT_TYPE"      ).SelectedValue
									, new DynamicControl(this, rowCurrent, "CUSTOMER_REFERENCE").Text
									// 05/26/2007 Paul.  Exchange rate is stored in the AllocationsView. 
									, fEXCHANGE_RATE
									, new DynamicControl(this, rowCurrent, "CURRENCY_ID"       ).ID
									, new DynamicControl(this, rowCurrent, "AMOUNT"            ).DecimalValue
									, new DynamicControl(this, rowCurrent, "DESCRIPTION"       ).Text
									, new DynamicControl(this, rowCurrent, "TEAM_ID"           ).ID
									, trn
									);
								SplendidDynamic.UpdateCustomFields(this, trn, gID, sCUSTOM_MODULE, dtCustomFields);
								
								DataTable dtLineItems = ctlAllocationsView.LineItems;
								// 03/27/2007 Paul.  Delete records before performing inserts or updates.
								foreach ( DataRow row in dtLineItems.Rows )
								{
									if ( row.RowState == DataRowState.Deleted )
									{
										// 05/26/2007 Paul.  In order to access values from deleted row, use DataRowVersion.Original, 
										// otherwise accessing the data will throw an exception "Deleted row information cannot be accessed through the row."
										Guid gITEM_ID = Sql.ToGuid(row["ID", DataRowVersion.Original]);
										if ( !Sql.IsEmptyGuid(gITEM_ID) )
											SqlProcs.spINVOICES_PAYMENTS_Delete(gITEM_ID, trn);
									}
								}
								foreach ( DataRow row in dtLineItems.Rows )
								{
									if ( row.RowState != DataRowState.Deleted )
									{
										Guid    gITEM_ID    = Sql.ToGuid   (row["ID"             ]);
										Guid    gINVOICE_ID = Sql.ToGuid   (row["INVOICE_ID"     ]);
										Decimal dAMOUNT     = Sql.ToDecimal(row["AMOUNT"         ]);

										// 03/27/2007 Paul.  Only add if product is defined.  This will exclude the blank row. 
										if ( !Sql.IsEmptyGuid(gINVOICE_ID) )
										{
											SqlProcs.spINVOICES_PAYMENTS_Update
												( ref gITEM_ID
												, gINVOICE_ID 
												, gID         
												, dAMOUNT     
												, trn
												);
										}
									}
								}
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

		protected void CURRENCY_ID_Changed(object sender, System.EventArgs e)
		{
			ctlAllocationsView.CURRENCY_ID_Changed(sender, e);
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
					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *              " + ControlChars.CrLf
							     + "  from vwPAYMENTS_Edit" + ControlChars.CrLf;
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
										ctlModuleHeader.Title = Sql.ToString(rdr["PAYMENT_NUM"]);
										SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);
										Utils.UpdateTracker(Page, m_sMODULE, gID, ctlModuleHeader.Title);
										ViewState["ctlModuleHeader.Title"] = ctlModuleHeader.Title;
										
										this.AppendEditViewFields(m_sMODULE + ".EditView", tblMain, rdr);
										DropDownList CURRENCY_ID = tblMain.FindControl("CURRENCY_ID") as DropDownList;
										if ( CURRENCY_ID != null )
										{
											CURRENCY_ID.AutoPostBack = true;
											CURRENCY_ID.SelectedIndexChanged += new EventHandler(CURRENCY_ID_Changed);
										}
										ctlAllocationsView.LoadLineItems(gID, gDuplicateID, con, rdr);
									}
									else
									{
										ctlAllocationsView.LoadLineItems(gID, gDuplicateID, con, null);
										
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
						DropDownList CURRENCY_ID = tblMain.FindControl("CURRENCY_ID") as DropDownList;
						if ( CURRENCY_ID != null )
						{
							CURRENCY_ID.AutoPostBack = true;
							CURRENCY_ID.SelectedIndexChanged += new EventHandler(CURRENCY_ID_Changed);
						}
						ctlAllocationsView.LoadLineItems(gID, gDuplicateID, null, null);

						new DynamicControl(this, "PAYMENT_DATE").DateValue = DateTime.Today;

						// 05/28/2007 Paul.  Prepopulate the Account. 
						Guid gPARENT_ID = Sql.ToGuid(Request["PARENT_ID"]);
						Guid gINVOICE_ID = gPARENT_ID;
						if ( !Sql.IsEmptyGuid(gPARENT_ID) )
						{
							string sMODULE      = String.Empty;
							string sPARENT_TYPE = String.Empty;
							string sPARENT_NAME = String.Empty;
							SqlProcs.spPARENT_Get(ref gPARENT_ID, ref sMODULE, ref sPARENT_TYPE, ref sPARENT_NAME);
							if ( !Sql.IsEmptyGuid(gPARENT_ID) && sMODULE == "Accounts" )
							{
								new DynamicControl(this, "ACCOUNT_ID"  ).ID   = gPARENT_ID;
								new DynamicControl(this, "ACCOUNT_NAME").Text = sPARENT_NAME;
							}
							else if ( !Sql.IsEmptyGuid(gINVOICE_ID) )
							{
								// 05/282/007 Paul.  spPARENT_Get will not return Invoices, so get the hard way.
								DbProviderFactory dbf = DbProviderFactories.GetFactory();
								using ( IDbConnection con = dbf.CreateConnection() )
								{
									string sSQL ;
									sSQL = "select *              " + ControlChars.CrLf
									     + "  from vwINVOICES_Edit" + ControlChars.CrLf;
									using ( IDbCommand cmd = con.CreateCommand() )
									{
										cmd.CommandText = sSQL;
										Security.Filter(cmd, "Invoices", "edit");
										Sql.AppendParameter(cmd, gINVOICE_ID, "ID", false);
										con.Open();

										if ( bDebug )
											RegisterClientScriptBlock("vwINVOICES_Edit", Sql.ClientScriptBlock(cmd));

										using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
										{
											if ( rdr.Read() )
											{
												new DynamicControl(this, "ACCOUNT_ID"  ).ID   = Sql.ToGuid  (rdr["BILLING_ACCOUNT_ID"  ]);
												new DynamicControl(this, "ACCOUNT_NAME").Text = Sql.ToString(rdr["BILLING_ACCOUNT_NAME"]);
											}
										}
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
			ctlEditButtons.Command = new CommandEventHandler(Page_Command);
			m_sMODULE = "Payments";
			SetMenu("Invoices");
			if ( IsPostBack )
			{
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields(m_sMODULE + ".EditView", tblMain, null);
				DropDownList CURRENCY_ID = tblMain.FindControl("CURRENCY_ID") as DropDownList;
				if ( CURRENCY_ID != null )
				{
					CURRENCY_ID.AutoPostBack = true;
					CURRENCY_ID.SelectedIndexChanged += new EventHandler(CURRENCY_ID_Changed);
				}
			}
		}
		#endregion
	}
}
