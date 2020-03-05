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

namespace SplendidCRM.Invoices
{
	/// <summary>
	///		Summary description for EditView.
	/// </summary>
	public class EditView : SplendidControl
	{
		protected _controls.ModuleHeader      ctlModuleHeader;
		protected _controls.EditButtons       ctlEditButtons ;
		protected _controls.EditLineItemsView ctlEditLineItemsView;

		protected Guid            gID                   ;
		protected HtmlTable       tblMain               ;
		protected HtmlTable       tblAddress            ;
		protected HtmlTable       tblSummary            ;
		protected HtmlTable       tblDescription        ;

		protected void Page_Command(Object sender, CommandEventArgs e)
		{
			Guid   gORDER_ID    = Sql.ToGuid(Request["ORDER_ID"]);
			Guid   gQUOTE_ID    = Sql.ToGuid(Request["QUOTE_ID"]);
			// 06/08/2006 Paul.  Redirect to parent if that is where the note was originated. 
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
				this.ValidateEditViewFields(m_sMODULE + ".EditView"       );
				this.ValidateEditViewFields(m_sMODULE + ".EditAddress"    );
				this.ValidateEditViewFields(m_sMODULE + ".EditDescription");
				if ( Page.IsValid )
				{
					string sCUSTOM_MODULE = "INVOICES";
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
							     + "  from vwINVOICES_Edit" + ControlChars.CrLf;
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
								// 01/05/2008 Paul.  Update the totals before saving. 
								ctlEditLineItemsView.UpdateTotals();
								// 11/18/2007 Paul.  Use the current values for any that are not defined in the edit view. 
								// 12/29/2007 Paul.  TEAM_ID is now in the stored procedure. 
								SqlProcs.spINVOICES_Update
									( ref gID
									, new DynamicControl(this, rowCurrent, "ASSIGNED_USER_ID"                ).ID
									, new DynamicControl(this, rowCurrent, "NAME"                            ).Text
									, new DynamicControl(this, rowCurrent, "QUOTE_ID"                        ).ID
									, new DynamicControl(this, rowCurrent, "ORDER_ID"                        ).ID
									, new DynamicControl(this, rowCurrent, "OPPORTUNITY_ID"                  ).ID
									, new DynamicControl(this, rowCurrent, "PAYMENT_TERMS"                   ).SelectedValue
									, new DynamicControl(this, rowCurrent, "INVOICE_STAGE"                   ).SelectedValue
									, new DynamicControl(this, rowCurrent, "PURCHASE_ORDER_NUM"              ).Text
									, new DynamicControl(this, rowCurrent, "DUE_DATE"                        ).DateValue
									, new DynamicControl(ctlEditLineItemsView, rowCurrent, "EXCHANGE_RATE"   ).FloatValue
									, new DynamicControl(ctlEditLineItemsView, rowCurrent, "CURRENCY_ID"     ).ID
									, new DynamicControl(ctlEditLineItemsView, rowCurrent, "TAXRATE_ID"      ).ID
									, new DynamicControl(ctlEditLineItemsView, rowCurrent, "SHIPPER_ID"      ).ID
									, new DynamicControl(ctlEditLineItemsView, rowCurrent, "SUBTOTAL"        ).DecimalValue
									, new DynamicControl(ctlEditLineItemsView, rowCurrent, "DISCOUNT"        ).DecimalValue
									, new DynamicControl(ctlEditLineItemsView, rowCurrent, "SHIPPING"        ).DecimalValue
									, new DynamicControl(ctlEditLineItemsView, rowCurrent, "TAX"             ).DecimalValue
									, new DynamicControl(ctlEditLineItemsView, rowCurrent, "TOTAL"           ).DecimalValue
									, new DynamicControl(ctlEditLineItemsView, rowCurrent, "AMOUNT_DUE"      ).DecimalValue
									, new DynamicControl(this, rowCurrent, "BILLING_ACCOUNT_ID"              ).ID
									, new DynamicControl(this, rowCurrent, "BILLING_CONTACT_ID"              ).ID
									, new DynamicControl(this, rowCurrent, "BILLING_ADDRESS_STREET"          ).Text
									, new DynamicControl(this, rowCurrent, "BILLING_ADDRESS_CITY"            ).Text
									, new DynamicControl(this, rowCurrent, "BILLING_ADDRESS_STATE"           ).Text
									, new DynamicControl(this, rowCurrent, "BILLING_ADDRESS_POSTALCODE"      ).Text
									, new DynamicControl(this, rowCurrent, "BILLING_ADDRESS_COUNTRY"         ).Text
									, new DynamicControl(this, rowCurrent, "SHIPPING_ACCOUNT_ID"             ).ID
									, new DynamicControl(this, rowCurrent, "SHIPPING_CONTACT_ID"             ).ID
									, new DynamicControl(this, rowCurrent, "SHIPPING_ADDRESS_STREET"         ).Text
									, new DynamicControl(this, rowCurrent, "SHIPPING_ADDRESS_CITY"           ).Text
									, new DynamicControl(this, rowCurrent, "SHIPPING_ADDRESS_STATE"          ).Text
									, new DynamicControl(this, rowCurrent, "SHIPPING_ADDRESS_POSTALCODE"     ).Text
									, new DynamicControl(this, rowCurrent, "SHIPPING_ADDRESS_COUNTRY"        ).Text
									, new DynamicControl(this, rowCurrent, "DESCRIPTION"                     ).Text
									, new DynamicControl(this, rowCurrent, "TEAM_ID"         ).ID
									, trn
									);
								SplendidDynamic.UpdateCustomFields(this, trn, gID, sCUSTOM_MODULE, dtCustomFields);
								
								DataTable dtLineItems = ctlEditLineItemsView.LineItems;
								// 03/27/2007 Paul.  Delete records before performing inserts or updates. 
								foreach ( DataRow row in dtLineItems.Rows )
								{
									if ( row.RowState == DataRowState.Deleted )
									{
										// 05/26/2007 Paul.  In order to access values from deleted row, use DataRowVersion.Original, 
										// otherwise accessing the data will throw an exception "Deleted row information cannot be accessed through the row."
										Guid gITEM_ID = Sql.ToGuid(row["ID", DataRowVersion.Original]);
										if ( !Sql.IsEmptyGuid(gITEM_ID) )
											SqlProcs.spINVOICES_LINE_ITEMS_Delete(gITEM_ID, trn);
									}
								}
								// 12/28/2007 Paul.  Renumber the position. 
								int nPOSITION = 1;
								foreach ( DataRow row in dtLineItems.Rows )
								{
									if ( row.RowState != DataRowState.Deleted )
									{
										Guid    gITEM_ID             = Sql.ToGuid   (row["ID"                 ]);
										Guid    gLINE_GROUP_ID       = Sql.ToGuid   (row["LINE_GROUP_ID"      ]);
										string  sLINE_ITEM_TYPE      = Sql.ToString (row["LINE_ITEM_TYPE"     ]);
										//int     nPOSITION            = Sql.ToInteger(row["POSITION"           ]);
										string  sNAME                = Sql.ToString (row["NAME"               ]);
										string  sMFT_PART_NUM        = Sql.ToString (row["MFT_PART_NUM"       ]);
										string  sVENDOR_PART_NUM     = Sql.ToString (row["VENDOR_PART_NUM"    ]);
										Guid    gPRODUCT_TEMPLATE_ID = Sql.ToGuid   (row["PRODUCT_TEMPLATE_ID"]);
										string  sTAX_CLASS           = Sql.ToString (row["TAX_CLASS"          ]);
										int     nQUANTITY            = Sql.ToInteger(row["QUANTITY"           ]);
										Decimal dCOST_PRICE          = Sql.ToDecimal(row["COST_PRICE"         ]);
										Decimal dLIST_PRICE          = Sql.ToDecimal(row["LIST_PRICE"         ]);
										Decimal dUNIT_PRICE          = Sql.ToDecimal(row["UNIT_PRICE"         ]);
										string  sDESCRIPTION         = Sql.ToString (row["DESCRIPTION"        ]);

										// 03/27/2007 Paul.  Only add if product is defined.  This will exclude the blank row. 
										// 08/11/2007 Paul.  Allow an item to be manually added.  Require either a product ID or a name. 
										if ( !Sql.IsEmptyGuid(gPRODUCT_TEMPLATE_ID) || !Sql.IsEmptyString(sNAME) )
										{
											SqlProcs.spINVOICES_LINE_ITEMS_Update
												( ref gITEM_ID        
												, gID                 
												, gLINE_GROUP_ID      
												, sLINE_ITEM_TYPE     
												, nPOSITION           
												, sNAME               
												, sMFT_PART_NUM       
												, sVENDOR_PART_NUM    
												, gPRODUCT_TEMPLATE_ID
												, sTAX_CLASS          
												, nQUANTITY           
												, dCOST_PRICE         
												, dLIST_PRICE         
												, dUNIT_PRICE         
												, sDESCRIPTION        
												, trn
												);
											nPOSITION++;
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
				else if ( !Sql.IsEmptyGuid(gORDER_ID) )
					Response.Redirect("~/Orders/view.aspx?ID=" + gORDER_ID.ToString());
				else if ( !Sql.IsEmptyGuid(gQUOTE_ID) )
					Response.Redirect("~/Quotes/view.aspx?ID=" + gQUOTE_ID.ToString());
				else if ( Sql.IsEmptyGuid(gID) )
					Response.Redirect("default.aspx");
				else
					Response.Redirect("view.aspx?ID=" + gID.ToString());
			}
		}

		private void UpdateAccount(Guid gACCOUNT_ID, bool bUpdateBilling, bool bUpdateShipping)
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL ;
				sSQL = "select *         " + ControlChars.CrLf
				     + "  from vwACCOUNTS" + ControlChars.CrLf
				     + " where ID = @ID  " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", gACCOUNT_ID);
					con.Open();
					using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
					{
						if ( rdr.Read() )
						{
							if ( bUpdateBilling )
							{
								new DynamicControl(this, "BILLING_ACCOUNT_ID"         ).ID   = Sql.ToGuid  (rdr["ID"                         ]);
								new DynamicControl(this, "BILLING_ACCOUNT_NAME"       ).Text = Sql.ToString(rdr["NAME"                       ]);
								new DynamicControl(this, "BILLING_ADDRESS_STREET"     ).Text = Sql.ToString(rdr["BILLING_ADDRESS_STREET"     ]);
								new DynamicControl(this, "BILLING_ADDRESS_CITY"       ).Text = Sql.ToString(rdr["BILLING_ADDRESS_CITY"       ]);
								new DynamicControl(this, "BILLING_ADDRESS_STATE"      ).Text = Sql.ToString(rdr["BILLING_ADDRESS_STATE"      ]);
								new DynamicControl(this, "BILLING_ADDRESS_POSTALCODE" ).Text = Sql.ToString(rdr["BILLING_ADDRESS_POSTALCODE" ]);
								new DynamicControl(this, "BILLING_ADDRESS_COUNTRY"    ).Text = Sql.ToString(rdr["BILLING_ADDRESS_COUNTRY"    ]);
							}
							if ( bUpdateShipping )
							{
								new DynamicControl(this, "SHIPPING_ACCOUNT_ID"        ).ID   = Sql.ToGuid  (rdr["ID"                         ]);
								new DynamicControl(this, "SHIPPING_ACCOUNT_NAME"      ).Text = Sql.ToString(rdr["NAME"                       ]);
								new DynamicControl(this, "SHIPPING_ADDRESS_STREET"    ).Text = Sql.ToString(rdr["SHIPPING_ADDRESS_STREET"    ]);
								new DynamicControl(this, "SHIPPING_ADDRESS_CITY"      ).Text = Sql.ToString(rdr["SHIPPING_ADDRESS_CITY"      ]);
								new DynamicControl(this, "SHIPPING_ADDRESS_STATE"     ).Text = Sql.ToString(rdr["SHIPPING_ADDRESS_STATE"     ]);
								new DynamicControl(this, "SHIPPING_ADDRESS_POSTALCODE").Text = Sql.ToString(rdr["SHIPPING_ADDRESS_POSTALCODE"]);
								new DynamicControl(this, "SHIPPING_ADDRESS_COUNTRY"   ).Text = Sql.ToString(rdr["SHIPPING_ADDRESS_COUNTRY"   ]);
							}
						}
					}
				}
			}
		}

		private void UpdateContact(Guid gCONTACT_ID, bool bUpdateBilling, bool bUpdateShipping)
		{
			DbProviderFactory dbf = DbProviderFactories.GetFactory();
			using ( IDbConnection con = dbf.CreateConnection() )
			{
				string sSQL ;
				sSQL = "select *         " + ControlChars.CrLf
				     + "  from vwCONTACTS" + ControlChars.CrLf
				     + " where ID = @ID  " + ControlChars.CrLf;
				using ( IDbCommand cmd = con.CreateCommand() )
				{
					cmd.CommandText = sSQL;
					Sql.AddParameter(cmd, "@ID", gCONTACT_ID);
					con.Open();
					using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
					{
						if ( rdr.Read() )
						{
							if ( bUpdateBilling )
							{
								new DynamicControl(this, "BILLING_CONTACT_ID"   ).ID   = Sql.ToGuid  (rdr["ID"  ]);
								new DynamicControl(this, "BILLING_CONTACT_NAME" ).Text = Sql.ToString(rdr["NAME"]);
							}
							if ( bUpdateShipping )
							{
								new DynamicControl(this, "SHIPPING_CONTACT_ID"  ).ID   = Sql.ToGuid  (rdr["ID"  ]);
								new DynamicControl(this, "SHIPPING_CONTACT_NAME").Text = Sql.ToString(rdr["NAME"]);
							}
							// 05/28/2007 Paul.  The ACCOUNT_ID needs to be retrieved from contact record. 
							Guid gACCOUNT_ID = Sql.ToGuid(rdr["ACCOUNT_ID"]);
							if ( !Sql.IsEmptyGuid(gACCOUNT_ID) )
							{
								UpdateAccount(gACCOUNT_ID, bUpdateBilling, bUpdateShipping);
							}
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
				// 06/09/2006 Paul.  Remove data binding in the user controls.  Binding is required, but only do so in the ASPX pages. 
				//Page.DataBind();
				gID = Sql.ToGuid(Request["ID"]);
				if ( !IsPostBack )
				{
					string sLOAD_MODULE     = "Invoices"  ;
					string sLOAD_MODULE_KEY = "INVOICE_ID";
					Guid gQUOTE_ID    = Sql.ToGuid(Request["QUOTE_ID"]);
					Guid gORDER_ID    = Sql.ToGuid(Request["ORDER_ID"]);
					Guid gDuplicateID = Sql.ToGuid(Request["DuplicateID"]);
					if ( !Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID) || !Sql.IsEmptyGuid(gQUOTE_ID) || !Sql.IsEmptyGuid(gORDER_ID) )
					{
						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( IDbConnection con = dbf.CreateConnection() )
						{
							string sSQL ;
							sSQL = "select *              " + ControlChars.CrLf
							     + "  from vwINVOICES_Edit" + ControlChars.CrLf;
							using ( IDbCommand cmd = con.CreateCommand() )
							{
								cmd.CommandText = sSQL;
								if ( !Sql.IsEmptyGuid(gQUOTE_ID) )
								{
									// 04/28/2007 Paul.  Load the data from the QUOTES record. 
									sLOAD_MODULE     = "Quotes"  ;
									sLOAD_MODULE_KEY = "QUOTE_ID";
									sSQL = "select *                        " + ControlChars.CrLf
									     + "  from vwQUOTES_ConvertToInvoice" + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									// 04/28/2007 Paul.  Filter by the module we are loading. 
									Security.Filter(cmd, sLOAD_MODULE, "edit");
									Sql.AppendParameter(cmd, gQUOTE_ID, "ID", false);
								}
								else if ( !Sql.IsEmptyGuid(gORDER_ID) )
								{
									// 04/28/2007 Paul.  Load the data from the ORDERS record. 
									sLOAD_MODULE     = "Orders"  ;
									sLOAD_MODULE_KEY = "ORDER_ID";
									sSQL = "select *                        " + ControlChars.CrLf
									     + "  from vwORDERS_ConvertToInvoice" + ControlChars.CrLf;
									cmd.CommandText = sSQL;
									// 04/28/2007 Paul.  Filter by the module we are loading. 
									Security.Filter(cmd, sLOAD_MODULE, "edit");
									Sql.AppendParameter(cmd, gORDER_ID, "ID", false);
								}
								else
								{
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
										ViewState["BILLING_ACCOUNT_ID" ] = Sql.ToGuid(rdr["BILLING_ACCOUNT_ID" ]);
										ViewState["SHIPPING_ACCOUNT_ID"] = Sql.ToGuid(rdr["SHIPPING_ACCOUNT_ID"]);

										new DynamicControl(this, "QUOTE_ID").ID = Sql.ToGuid(rdr["QUOTE_ID"]);
										new DynamicControl(this, "ORDER_ID").ID = Sql.ToGuid(rdr["ORDER_ID"]);
										
										this.AppendEditViewFields(m_sMODULE + ".EditView"       , tblMain       , rdr);
										this.AppendEditViewFields(m_sMODULE + ".EditAddress"    , tblAddress    , rdr);
										this.AppendEditViewFields(m_sMODULE + ".EditDescription", tblDescription, rdr);
										
										if ( !Sql.IsEmptyGuid(gQUOTE_ID) )
										{
											new DynamicControl(this, "QUOTE_ID"  ).ID   = gQUOTE_ID;
											new DynamicControl(this, "QUOTE_NAME").Text = Sql.ToString(rdr["NAME"]);
											ctlEditLineItemsView.LoadLineItems(gQUOTE_ID, Guid.Empty, con, rdr, sLOAD_MODULE, sLOAD_MODULE_KEY);
										}
										else if ( !Sql.IsEmptyGuid(gORDER_ID) )
										{
											new DynamicControl(this, "ORDER_ID"  ).ID   = gORDER_ID;
											new DynamicControl(this, "ORDER_NAME").Text = Sql.ToString(rdr["NAME"]);
											ctlEditLineItemsView.LoadLineItems(gORDER_ID, Guid.Empty, con, rdr, sLOAD_MODULE, sLOAD_MODULE_KEY);
										}
										else
										{
											ctlEditLineItemsView.LoadLineItems(gID, gDuplicateID, con, rdr, sLOAD_MODULE, sLOAD_MODULE_KEY);
										}
									}
									else
									{
										ctlEditLineItemsView.LoadLineItems(gID, gDuplicateID, con, null, String.Empty, String.Empty);
										
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
						this.AppendEditViewFields(m_sMODULE + ".EditView"       , tblMain       , null);
						this.AppendEditViewFields(m_sMODULE + ".EditAddress"    , tblAddress    , null);
						this.AppendEditViewFields(m_sMODULE + ".EditDescription", tblDescription, null);
						// 06/08/2006 Paul.  Prepopulate the Account. 
						Guid gPARENT_ID = Sql.ToGuid(Request["PARENT_ID"]);
						if ( !Sql.IsEmptyGuid(gPARENT_ID) )
						{
							string sMODULE      = String.Empty;
							string sPARENT_TYPE = String.Empty;
							string sPARENT_NAME = String.Empty;
							SqlProcs.spPARENT_Get(ref gPARENT_ID, ref sMODULE, ref sPARENT_TYPE, ref sPARENT_NAME);
							if ( !Sql.IsEmptyGuid(gPARENT_ID) && sMODULE == "Accounts" )
							{
								UpdateAccount(gPARENT_ID, true, true);
							}
							if ( !Sql.IsEmptyGuid(gPARENT_ID) && sMODULE == "Contacts" )
							{
								UpdateContact(gPARENT_ID, true, true);
							}
							else if ( !Sql.IsEmptyGuid(gPARENT_ID) && sMODULE == "Opportunities" )
							{
								new DynamicControl(this, "OPPORTUNITY_ID"   ).ID   = gPARENT_ID;
								new DynamicControl(this, "OPPORTUNITY_NAME" ).Text = sPARENT_NAME;
							}
						}
						ctlEditLineItemsView.LoadLineItems(gID, gDuplicateID, null, null, String.Empty, String.Empty);
					}
				}
				else
				{
					// 12/02/2005 Paul.  When validation fails, the header title does not retain its value.  Update manually. 
					ctlModuleHeader.Title = Sql.ToString(ViewState["ctlModuleHeader.Title"]);
					SetPageTitle(L10n.Term(".moduleList." + m_sMODULE) + " - " + ctlModuleHeader.Title);

					DynamicControl ctlBILLING_ACCOUNT_ID  = new DynamicControl(this, "BILLING_ACCOUNT_ID" );
					DynamicControl ctlSHIPPING_ACCOUNT_ID = new DynamicControl(this, "SHIPPING_ACCOUNT_ID");
					if ( Sql.ToGuid(ViewState["BILLING_ACCOUNT_ID" ]) != ctlBILLING_ACCOUNT_ID.ID )
					{
						UpdateAccount(ctlBILLING_ACCOUNT_ID.ID, true, true);
						ViewState["BILLING_ACCOUNT_ID" ] = ctlBILLING_ACCOUNT_ID.ID;
						ViewState["SHIPPING_ACCOUNT_ID"] = ctlBILLING_ACCOUNT_ID.ID;
					}
					if ( Sql.ToGuid(ViewState["SHIPPING_ACCOUNT_ID"]) != ctlSHIPPING_ACCOUNT_ID.ID )
					{
						UpdateAccount(ctlSHIPPING_ACCOUNT_ID.ID, false, true);
						ViewState["SHIPPING_ACCOUNT_ID"] = ctlSHIPPING_ACCOUNT_ID.ID;
					}
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
			m_sMODULE = "Invoices";
			SetMenu(m_sMODULE);
			if ( IsPostBack )
			{
				// 12/02/2005 Paul.  Need to add the edit fields in order for events to fire. 
				this.AppendEditViewFields(m_sMODULE + ".EditView"       , tblMain       , null);
				this.AppendEditViewFields(m_sMODULE + ".EditAddress"    , tblAddress    , null);
				this.AppendEditViewFields(m_sMODULE + ".EditDescription", tblDescription, null);
			}
		}
		#endregion
	}
}
