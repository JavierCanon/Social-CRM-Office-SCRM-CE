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

namespace SplendidCRM._controls
{
	/// <summary>
	///		Summary description for EditLineItemsView.
	/// </summary>
	public class EditLineItemsView : SplendidControl
	{
		protected DataTable       dtLineItems           ;
		protected GridView        grdMain               ;
		protected DropDownList    CURRENCY_ID           ;
		protected DropDownList    TAXRATE_ID            ;
		protected DropDownList    SHIPPER_ID            ;
		protected CheckBox        SHOW_LINE_NUMS        ;
		protected CheckBox        CALC_GRAND_TOTAL      ;
		protected Label           lblLineItemError      ;
		protected TextBox         SUBTOTAL              ;
		protected TextBox         DISCOUNT              ;
		protected TextBox         SHIPPING              ;
		protected TextBox         TAX                   ;
		protected TextBox         TOTAL                 ;
		protected HiddenField     DISCOUNT_USDOLLAR     ;
		protected HiddenField     SHIPPING_USDOLLAR     ;
		// 07/07/2007 Paul.  Make the Exchange Rate a user-editable field. 
		protected TextBox         EXCHANGE_RATE         ;
		protected string          m_sMODULE_KEY         ;
		protected bool            m_bShowCostPrice      = true;
		protected bool            m_bShowListPrice      = true;

		public DataTable LineItems
		{
			get { return dtLineItems; }
		}

		public string MODULE
		{
			get { return m_sMODULE; }
			set { m_sMODULE = value.Replace(" ", ""); }
		}

		public string MODULE_KEY
		{
			get { return m_sMODULE_KEY; }
			set { m_sMODULE_KEY = value.Replace(" ", ""); }
		}

		public bool ShowCostPrice
		{
			get { return m_bShowCostPrice; }
			set { m_bShowCostPrice = value; }
		}

		public bool ShowListPrice
		{
			get { return m_bShowListPrice; }
			set { m_bShowListPrice = value; }
		}

		#region Line Item Editing
		protected void grdMain_RowCreated(object sender, GridViewRowEventArgs e)
		{
			if ( e.Row.RowType == DataControlRowType.DataRow )
			{
				DropDownList lstTAX_CLASS = e.Row.FindControl("TAX_CLASS") as DropDownList;
				if ( lstTAX_CLASS != null )
				{
					lstTAX_CLASS.DataSource = SplendidCache.List("tax_class_dom");
				}
			}
		}

		protected void grdMain_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if ( e.Row.RowType == DataControlRowType.DataRow )
			{
				DropDownList lstTAX_CLASS = e.Row.FindControl("TAX_CLASS") as DropDownList;
				if ( lstTAX_CLASS != null )
				{
					try
					{
						lstTAX_CLASS.SelectedValue = DataBinder.Eval(e.Row.DataItem, "TAX_CLASS").ToString() ;
					}
					catch
					{
					}
				}
			}
		}

		protected void grdMain_RowEditing(object sender, GridViewEditEventArgs e)
		{
			grdMain.EditIndex = e.NewEditIndex;
			if ( dtLineItems != null )
			{
				grdMain.DataSource = dtLineItems;
				grdMain.DataBind();
			}
		}

		protected void grdMain_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			if ( dtLineItems != null )
			{
				//dtLineItems.Rows.RemoveAt(e.RowIndex);
				//dtLineItems.Rows[e.RowIndex].Delete();
				// 08/07/2007 fhsakai.  There might already be deleted rows, so make sure to first obtain the current rows. 
				DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				aCurrentRows[e.RowIndex].Delete();
				
				aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				// 02/04/2007 Paul.  Always allow editing of the last empty row. Add blank row if necessary. 
				// 08/11/2007 Paul.  Allow an item to be manually added.  Require either a product ID or a name. 
				if ( aCurrentRows.Length == 0 || !Sql.IsEmptyString(aCurrentRows[aCurrentRows.Length-1]["NAME"]) || !Sql.IsEmptyGuid(aCurrentRows[aCurrentRows.Length-1]["PRODUCT_TEMPLATE_ID"]) )
				{
					DataRow rowNew = dtLineItems.NewRow();
					dtLineItems.Rows.Add(rowNew);
					aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				}
				UpdateTotals();

				ViewState["LineItems"] = dtLineItems;
				grdMain.DataSource = dtLineItems;
				// 03/15/2007 Paul.  Make sure to use the last row of the current set, not the total rows of the table.  Some rows may be deleted. 
				grdMain.EditIndex = aCurrentRows.Length - 1;
				grdMain.DataBind();
				UpdateTotals();
			}
		}

		protected void grdMain_RowUpdating(object sender, GridViewUpdateEventArgs e)
		{
			if ( dtLineItems != null )
			{
				GridViewRow gr = grdMain.Rows[e.RowIndex];
				HiddenField     txtLINE_ITEM_TYPE      = gr.FindControl("LINE_ITEM_TYPE"     ) as HiddenField ;
				TextBox         txtNAME                = gr.FindControl("NAME"               ) as TextBox     ;
				TextBox         txtMFT_PART_NUM        = gr.FindControl("MFT_PART_NUM"       ) as TextBox     ;
				HiddenField     txtVENDOR_PART_NUM     = gr.FindControl("VENDOR_PART_NUM"    ) as HiddenField ;
				HiddenField     txtPRODUCT_TEMPLATE_ID = gr.FindControl("PRODUCT_TEMPLATE_ID") as HiddenField ;
				DropDownList    lstTAX_CLASS           = gr.FindControl("TAX_CLASS"          ) as DropDownList;
				TextBox         txtQUANTITY            = gr.FindControl("QUANTITY"           ) as TextBox     ;
				TextBox         txtCOST_PRICE          = gr.FindControl("COST_PRICE"         ) as TextBox     ;
				HiddenField     txtCOST_USDOLLAR       = gr.FindControl("COST_USDOLLAR"      ) as HiddenField ;
				TextBox         txtLIST_PRICE          = gr.FindControl("LIST_PRICE"         ) as TextBox     ;
				HiddenField     txtLIST_USDOLLAR       = gr.FindControl("LIST_USDOLLAR"      ) as HiddenField ;
				TextBox         txtUNIT_PRICE          = gr.FindControl("UNIT_PRICE"         ) as TextBox     ;
				HiddenField     txtUNIT_USDOLLAR       = gr.FindControl("UNIT_USDOLLAR"      ) as HiddenField ;
				TextBox         txtEXTENDED_PRICE      = gr.FindControl("EXTENDED_PRICE"     ) as TextBox     ;
				HiddenField     txtEXTENDED_USDOLLAR   = gr.FindControl("EXTENDED_USDOLLAR"  ) as HiddenField ;
				TextBox         txtDESCRIPTION         = gr.FindControl("DESCRIPTION"        ) as TextBox     ;

				//DataRow row = dtLineItems.Rows[e.RowIndex];
				// 12/07/2007 garf.  If there are deleted rows in the set, then the index will be wrong.  Make sure to use the current rowset. 
				DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				DataRow row = aCurrentRows[e.RowIndex];
				// 03/30/2007 Paul.  The text controls are empty.  Use the Request object to read the data. 
				if ( txtLINE_ITEM_TYPE != null ) row["LINE_ITEM_TYPE"] = txtLINE_ITEM_TYPE.Value;
				if ( txtDESCRIPTION    != null ) row["DESCRIPTION"   ] = txtDESCRIPTION   .Text;
				if ( Sql.ToString(row["LINE_ITEM_TYPE"]) == "Comment" )
				{
					row["NAME"               ] = DBNull.Value;
					row["MFT_PART_NUM"       ] = DBNull.Value;
					row["VENDOR_PART_NUM"    ] = DBNull.Value;
					row["PRODUCT_TEMPLATE_ID"] = DBNull.Value;
					row["TAX_CLASS"          ] = DBNull.Value;
					row["QUANTITY"           ] = DBNull.Value;
					row["COST_PRICE"         ] = DBNull.Value;
					row["COST_USDOLLAR"      ] = DBNull.Value;
					row["LIST_PRICE"         ] = DBNull.Value;
					row["LIST_USDOLLAR"      ] = DBNull.Value;
					row["UNIT_PRICE"         ] = DBNull.Value;
					row["UNIT_USDOLLAR"      ] = DBNull.Value;
					row["EXTENDED_PRICE"     ] = DBNull.Value;
					row["EXTENDED_USDOLLAR"  ] = DBNull.Value;
				}
				else
				{
					// 03/31/2007 Paul.  The US Dollar values are computed from the price and are only used when changing currencies. 
					if ( txtNAME                != null ) row["NAME"               ] =               txtNAME               .Text;
					if ( txtMFT_PART_NUM        != null ) row["MFT_PART_NUM"       ] =               txtMFT_PART_NUM       .Text;
					if ( txtVENDOR_PART_NUM     != null ) row["VENDOR_PART_NUM"    ] =               txtVENDOR_PART_NUM    .Value;
					if ( txtPRODUCT_TEMPLATE_ID != null ) row["PRODUCT_TEMPLATE_ID"] = Sql.ToGuid   (txtPRODUCT_TEMPLATE_ID.Value);
					if ( lstTAX_CLASS           != null ) row["TAX_CLASS"          ] =               lstTAX_CLASS          .SelectedValue;
					if ( txtQUANTITY            != null ) row["QUANTITY"           ] = Sql.ToInteger(txtQUANTITY           .Text);
					if ( txtCOST_PRICE          != null ) row["COST_PRICE"         ] = Sql.ToDecimal(txtCOST_PRICE         .Text);
					if ( txtLIST_PRICE          != null ) row["LIST_PRICE"         ] = Sql.ToDecimal(txtLIST_PRICE         .Text);
					if ( txtUNIT_PRICE          != null ) row["UNIT_PRICE"         ] = Sql.ToDecimal(txtUNIT_PRICE         .Text);
					
					row["COST_USDOLLAR"      ] = C10n.FromCurrency(Sql.ToDecimal(txtCOST_PRICE    .Text));
					row["LIST_USDOLLAR"      ] = C10n.FromCurrency(Sql.ToDecimal(txtLIST_PRICE    .Text));
					row["UNIT_USDOLLAR"      ] = C10n.FromCurrency(Sql.ToDecimal(txtUNIT_PRICE    .Text));
					row["EXTENDED_PRICE"     ] = Sql.ToInteger(row["QUANTITY"]) * Sql.ToDecimal(row["UNIT_PRICE"]);
					row["EXTENDED_USDOLLAR"  ] = C10n.FromCurrency(Sql.ToDecimal(row["EXTENDED_PRICE"]));
				}

				// 12/07/2007 Paul.  aCurrentRows is defined above. 
				//DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				// 03/30/2007 Paul.  Always allow editing of the last empty row. Add blank row if necessary. 
				// 08/11/2007 Paul.  Allow an item to be manually added.  Require either a product ID or a name. 
				if ( aCurrentRows.Length == 0 || !Sql.IsEmptyString(aCurrentRows[aCurrentRows.Length-1]["NAME"]) || !Sql.IsEmptyGuid(aCurrentRows[aCurrentRows.Length-1]["PRODUCT_TEMPLATE_ID"]) )
				{
					DataRow rowNew = dtLineItems.NewRow();
					dtLineItems.Rows.Add(rowNew);
					aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				}

				ViewState["LineItems"] = dtLineItems;
				grdMain.DataSource = dtLineItems;
				// 03/30/2007 Paul.  Make sure to use the last row of the current set, not the total rows of the table.  Some rows may be deleted. 
				grdMain.EditIndex = aCurrentRows.Length - 1;
				grdMain.DataBind();
				UpdateTotals();
			}
		}

		protected void grdMain_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
		{
			grdMain.EditIndex = -1;
			if ( dtLineItems != null )
			{
				DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				grdMain.DataSource = dtLineItems;
				// 03/15/2007 Paul.  Make sure to use the last row of the current set, not the total rows of the table.  Some rows may be deleted. 
				grdMain.EditIndex = aCurrentRows.Length - 1;
				grdMain.DataBind();
				UpdateTotals();
			}
		}

		protected void CURRENCY_ID_Changed(object sender, System.EventArgs e)
		{
			// 03/31/2007 Paul.  When the currency changes, use the default exchange rate. 
			Guid gCURRENCY_ID = Sql.ToGuid(CURRENCY_ID.SelectedValue);
			SetC10n(gCURRENCY_ID);
			EXCHANGE_RATE.Text = C10n.CONVERSION_RATE.ToString();

			DISCOUNT.Text = C10n.ToCurrency(Sql.ToDecimal(DISCOUNT_USDOLLAR.Value)).ToString("0.00");
			SHIPPING.Text = C10n.ToCurrency(Sql.ToDecimal(SHIPPING_USDOLLAR.Value)).ToString("0.00");
			foreach ( DataRow row in dtLineItems.Rows )
			{
				if ( row.RowState != DataRowState.Deleted )
				{
					row["COST_PRICE"    ] = C10n.ToCurrency(Sql.ToDecimal(row["COST_USDOLLAR"    ]));
					row["LIST_PRICE"    ] = C10n.ToCurrency(Sql.ToDecimal(row["LIST_USDOLLAR"    ]));
					row["UNIT_PRICE"    ] = C10n.ToCurrency(Sql.ToDecimal(row["UNIT_USDOLLAR"    ]));
					row["EXTENDED_PRICE"] = C10n.ToCurrency(Sql.ToDecimal(row["EXTENDED_USDOLLAR"]));
				}
			}
			grdMain.DataBind();

			UpdateTotals();
		}

		protected void TAXRATE_ID_Changed(object sender, System.EventArgs e)
		{
			UpdateTotals();
		}

		// 01/05/2007 Paul.  We need to update the totals before saving. 
		public void UpdateTotals()
		{
			Double dSUBTOTAL = 0.0;
			Double dDISCOUNT = new DynamicControl(this, "DISCOUNT").FloatValue;
			Double dSHIPPING = new DynamicControl(this, "SHIPPING").FloatValue;
			Double dTAX      = 0.0;
			Double dTOTAL    = 0.0;
			double dTAX_RATE = 0.0;

			DataTable dtTAX_RATE = SplendidCache.TaxRates();
			if ( !Sql.IsEmptyGuid(TAXRATE_ID.SelectedValue) )
			{
				DataRow[] row = dtTAX_RATE.Select("ID = '" + TAXRATE_ID.SelectedValue + "'");
				if ( row.Length == 1 )
				{
					dTAX_RATE = Sql.ToDouble(row[0]["VALUE"]) / 100;
				}
			}
			foreach ( DataRow row in dtLineItems.Rows )
			{
				if ( row.RowState != DataRowState.Deleted )
				{
					// 08/11/2007 Paul.  Allow an item to be manually added.  Require either a product ID or a name. 
					if ( !Sql.IsEmptyString(row["NAME"]) || !Sql.IsEmptyGuid(row["PRODUCT_TEMPLATE_ID"]) )
					{
						string  sLINE_ITEM_TYPE = Sql.ToString (row["LINE_ITEM_TYPE"]);
						string  sTAX_CLASS      = Sql.ToString (row["TAX_CLASS"     ]);
						int     nQUANTITY       = Sql.ToInteger(row["QUANTITY"      ]);
						Double  dUNIT_PRICE     = Sql.ToDouble (row["UNIT_PRICE"    ]);
						if ( sLINE_ITEM_TYPE != "Comment" )
						{
							dSUBTOTAL += dUNIT_PRICE * nQUANTITY;
							if ( sTAX_CLASS == "Taxable" )
								dTAX += dUNIT_PRICE * nQUANTITY * dTAX_RATE;
						}
					}
				}
			}
			dTOTAL = dSUBTOTAL - dDISCOUNT + dTAX + dSHIPPING;
			SUBTOTAL.Text = Convert.ToDecimal(dSUBTOTAL).ToString("c");
			DISCOUNT.Text = Convert.ToDecimal(dDISCOUNT).ToString("0.00");
			SHIPPING.Text = Convert.ToDecimal(dSHIPPING).ToString("0.00");
			TAX     .Text = Convert.ToDecimal(dTAX     ).ToString("c");
			TOTAL   .Text = Convert.ToDecimal(dTOTAL   ).ToString("c");
			// 03/31/2007 Paul.  We are using UNIT_PRICE, so the value will need to be converted to USD before stored in hidden fields. 
			DISCOUNT_USDOLLAR.Value = C10n.FromCurrency(Convert.ToDecimal(dDISCOUNT)).ToString("0.000");
			SHIPPING_USDOLLAR.Value = C10n.FromCurrency(Convert.ToDecimal(dSHIPPING)).ToString("0.000");
		}
		#endregion

		public void LoadLineItems(Guid gID, Guid gDuplicateID, IDbConnection con, IDataReader rdr, string sLOAD_MODULE, string sLOAD_MODULE_KEY)
		{
			if ( !IsPostBack )
			{
				CURRENCY_ID.DataSource = SplendidCache.Currencies();
				CURRENCY_ID.DataBind();
				TAXRATE_ID .DataSource = SplendidCache.TaxRates();
				TAXRATE_ID .DataBind();
				TAXRATE_ID .Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
				SHIPPER_ID .DataSource = SplendidCache.Shippers();
				SHIPPER_ID .DataBind();
				SHIPPER_ID.Items.Insert(0, new ListItem(L10n.Term(".LBL_NONE"), ""));
				try
				{
					// 03/15/2007 Paul.  Set the default currency to the current user currency. 
					CURRENCY_ID.SelectedValue = C10n.ID.ToString();
					EXCHANGE_RATE.Text = C10n.CONVERSION_RATE.ToString();
				}
				catch
				{
				}
				foreach ( DataControlField col in grdMain.Columns )
				{
					if ( !Sql.IsEmptyString(col.HeaderText) )
					{
						if ( col.HeaderText == ".LBL_LIST_ITEM_COST_PRICE" )
							col.Visible = m_bShowCostPrice;
						if ( col.HeaderText == ".LBL_LIST_ITEM_LIST_PRICE" )
							col.Visible = m_bShowListPrice;
						col.HeaderText = L10n.Term(m_sMODULE + col.HeaderText);
					}
					CommandField cf = col as CommandField;
					if ( cf != null )
					{
						cf.EditText   = L10n.Term(cf.EditText  );
						cf.DeleteText = L10n.Term(cf.DeleteText);
						cf.UpdateText = L10n.Term(cf.UpdateText);
						cf.CancelText = L10n.Term(cf.CancelText);
					}
				}
				if ( Sql.IsEmptyString(m_sMODULE) )
					throw(new Exception("EditLineItemsView: MODULE is undefined."));
				//if ( Sql.IsEmptyString(m_sMODULE_KEY) )
				//	throw(new Exception("EditLineItemsView: MODULE_KEY is undefined."));
				
				if ( (!Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID)) && (con != null) && (rdr != null) )
				{
					new DynamicControl(this, "SHOW_LINE_NUMS"  ).Checked = Sql.ToBoolean(rdr["SHOW_LINE_NUMS"  ]);
					new DynamicControl(this, "CALC_GRAND_TOTAL").Checked = Sql.ToBoolean(rdr["CALC_GRAND_TOTAL"]);
					try
					{
						new DynamicControl(this, "CURRENCY_ID").SelectedValue = Sql.ToString(rdr["CURRENCY_ID"]);
					}
					catch
					{
					}
					try
					{
						new DynamicControl(this, "TAXRATE_ID").SelectedValue = Sql.ToString(rdr["TAXRATE_ID"]);
					}
					catch
					{
					}
					try
					{
						new DynamicControl(this, "SHIPPER_ID" ).SelectedValue = Sql.ToString(rdr["SHIPPER_ID"]);
					}
					catch
					{
					}
					// 03/31/2007 Paul.  The exchange rate might be an old value. 
					float fEXCHANGE_RATE = Sql.ToFloat(rdr["EXCHANGE_RATE"]);
					EXCHANGE_RATE.Text = fEXCHANGE_RATE.ToString();
					if ( CURRENCY_ID.Items.Count > 0 )
					{
						// 03/31/2007 Paul.  Replace the user currency with the form currency, but use the old exchange rate. 
						Guid gCURRENCY_ID = Sql.ToGuid(CURRENCY_ID.SelectedValue);
						SetC10n(gCURRENCY_ID, fEXCHANGE_RATE);
					}
					// 07/07/2007 Paul.  We should either display the previous values or convert from USD.  
					//SUBTOTAL         .Text  = C10n.ToCurrency(Sql.ToDecimal(rdr["SUBTOTAL_USDOLLAR"])).ToString("c");
					//DISCOUNT         .Text  = C10n.ToCurrency(Sql.ToDecimal(rdr["DISCOUNT_USDOLLAR"])).ToString("0.00");
					//SHIPPING         .Text  = C10n.ToCurrency(Sql.ToDecimal(rdr["SHIPPING_USDOLLAR"])).ToString("0.00");
					//TAX              .Text  = C10n.ToCurrency(Sql.ToDecimal(rdr["TAX_USDOLLAR"     ])).ToString("c");
					//TOTAL            .Text  = C10n.ToCurrency(Sql.ToDecimal(rdr["TOTAL_USDOLLAR"   ])).ToString("c");
					// 07/07/2007 Paul.  Lets show the un-converted value as this may help us find bugs. 
					SUBTOTAL         .Text  =Sql.ToDecimal(rdr["SUBTOTAL"         ]).ToString("c");
					DISCOUNT         .Text  =Sql.ToDecimal(rdr["DISCOUNT"         ]).ToString("0.00");
					SHIPPING         .Text  =Sql.ToDecimal(rdr["SHIPPING"         ]).ToString("0.00");
					TAX              .Text  =Sql.ToDecimal(rdr["TAX"              ]).ToString("c");
					TOTAL            .Text  =Sql.ToDecimal(rdr["TOTAL"            ]).ToString("c");

					// 05/26/2007 Paul.  Stored USDOLLAR values should not be converted to local currency. 
					DISCOUNT_USDOLLAR.Value = Sql.ToDecimal(rdr["DISCOUNT_USDOLLAR"]).ToString("0.00");
					SHIPPING_USDOLLAR.Value = Sql.ToDecimal(rdr["SHIPPING_USDOLLAR"]).ToString("0.00");
					rdr.Close();

					string sSQL;
					string sLINE_ITEMS_VIEW = "vw" + sLOAD_MODULE.ToUpper() + "_LINE_ITEMS";
					sSQL = "select *                  " + ControlChars.CrLf
					     + "  from " + sLINE_ITEMS_VIEW + ControlChars.CrLf
					     + " where 1 = 1              " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						if ( !Sql.IsEmptyGuid(gDuplicateID) )
						{
							Sql.AppendParameter(cmd, gDuplicateID, sLOAD_MODULE_KEY, false);
						}
						else
						{
							Sql.AppendParameter(cmd, gID, sLOAD_MODULE_KEY, false);
						}
						cmd.CommandText += " order by POSITION asc" + ControlChars.CrLf;

						if ( bDebug )
							RegisterClientScriptBlock(sLINE_ITEMS_VIEW, Sql.ClientScriptBlock(cmd));

						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							dtLineItems = new DataTable();
							da.Fill(dtLineItems);
							
							// 04/01/2007 Paul.  If we are duplicating a quote, then we must create new IDs for the line items. 
							// Otherwise, edits to the line items will change the old quote. 
							// 04/29/2007 Paul.  If we are converting from one module to another, then the loading module will not match the current module. 
							if ( !Sql.IsEmptyGuid(gDuplicateID) || m_sMODULE != sLOAD_MODULE )
							{
								foreach ( DataRow row in dtLineItems.Rows )
								{
									row["ID"] = Guid.NewGuid();
								}
							}
							// 03/27/2007 Paul.  Always add blank line to allow quick editing. 
							DataRow rowNew = dtLineItems.NewRow();
							dtLineItems.Rows.Add(rowNew);

							ViewState["LineItems"] = dtLineItems;
							grdMain.DataSource = dtLineItems;
							// 03/27/2007 Paul.  Start with last line enabled for editing. 
							grdMain.EditIndex = dtLineItems.Rows.Count - 1;
							grdMain.DataBind();
						}
					}
				}
				else
				{
					dtLineItems = new DataTable();
					DataColumn colID                  = new DataColumn("ID"                 , Type.GetType("System.Guid"   ));
					DataColumn colLINE_GROUP_ID       = new DataColumn("LINE_GROUP_ID"      , Type.GetType("System.Guid"   ));
					DataColumn colLINE_ITEM_TYPE      = new DataColumn("LINE_ITEM_TYPE"     , Type.GetType("System.String" ));
					DataColumn colPOSITION            = new DataColumn("POSITION"           , Type.GetType("System.Int32"  ));
					DataColumn colNAME                = new DataColumn("NAME"               , Type.GetType("System.String" ));
					DataColumn colMFT_PART_NUM        = new DataColumn("MFT_PART_NUM"       , Type.GetType("System.String" ));
					DataColumn colVENDOR_PART_NUM     = new DataColumn("VENDOR_PART_NUM"    , Type.GetType("System.String" ));
					DataColumn colPRODUCT_TEMPLATE_ID = new DataColumn("PRODUCT_TEMPLATE_ID", Type.GetType("System.Guid"   ));
					DataColumn colTAX_CLASS           = new DataColumn("TAX_CLASS"          , Type.GetType("System.String" ));
					DataColumn colQUANTITY            = new DataColumn("QUANTITY"           , Type.GetType("System.Int32"  ));
					DataColumn colCOST_PRICE          = new DataColumn("COST_PRICE"         , Type.GetType("System.Decimal"));
					DataColumn colCOST_USDOLLAR       = new DataColumn("COST_USDOLLAR"      , Type.GetType("System.Decimal"));
					DataColumn colLIST_PRICE          = new DataColumn("LIST_PRICE"         , Type.GetType("System.Decimal"));
					DataColumn colLIST_USDOLLAR       = new DataColumn("LIST_USDOLLAR"      , Type.GetType("System.Decimal"));
					DataColumn colUNIT_PRICE          = new DataColumn("UNIT_PRICE"         , Type.GetType("System.Decimal"));
					DataColumn colUNIT_USDOLLAR       = new DataColumn("UNIT_USDOLLAR"      , Type.GetType("System.Decimal"));
					DataColumn colEXTENDED_PRICE      = new DataColumn("EXTENDED_PRICE"     , Type.GetType("System.Decimal"));
					DataColumn colEXTENDED_USDOLLAR   = new DataColumn("EXTENDED_USDOLLAR"  , Type.GetType("System.Decimal"));
					DataColumn colDESCRIPTION         = new DataColumn("DESCRIPTION"        , Type.GetType("System.String" ));
					dtLineItems.Columns.Add(colID                 );
					dtLineItems.Columns.Add(colLINE_GROUP_ID      );
					dtLineItems.Columns.Add(colLINE_ITEM_TYPE     );
					dtLineItems.Columns.Add(colPOSITION           );
					dtLineItems.Columns.Add(colNAME               );
					dtLineItems.Columns.Add(colMFT_PART_NUM       );
					dtLineItems.Columns.Add(colVENDOR_PART_NUM    );
					dtLineItems.Columns.Add(colPRODUCT_TEMPLATE_ID);
					dtLineItems.Columns.Add(colTAX_CLASS          );
					dtLineItems.Columns.Add(colQUANTITY           );
					dtLineItems.Columns.Add(colCOST_PRICE         );
					dtLineItems.Columns.Add(colCOST_USDOLLAR      );
					dtLineItems.Columns.Add(colLIST_PRICE         );
					dtLineItems.Columns.Add(colLIST_USDOLLAR      );
					dtLineItems.Columns.Add(colUNIT_PRICE         );
					dtLineItems.Columns.Add(colUNIT_USDOLLAR      );
					dtLineItems.Columns.Add(colEXTENDED_PRICE     );
					dtLineItems.Columns.Add(colEXTENDED_USDOLLAR  );
					dtLineItems.Columns.Add(colDESCRIPTION        );
					// 03/27/2007 Paul.  Always add blank line to allow quick editing. 
					DataRow rowNew = dtLineItems.NewRow();
					dtLineItems.Rows.Add(rowNew);

					ViewState["LineItems"] = dtLineItems;
					grdMain.DataSource = dtLineItems;
					// 02/03/2007 Paul.  Start with last line enabled for editing. 
					grdMain.EditIndex = dtLineItems.Rows.Count - 1;
					grdMain.DataBind();

					UpdateTotals();
				}
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ( IsPostBack )
			{
				if ( CURRENCY_ID.Items.Count > 0 )
				{
					// 03/31/2007 Paul.  Replace the user currency with the form currency, but use the old exchange rate. 
					Guid gCURRENCY_ID = Sql.ToGuid(CURRENCY_ID.SelectedValue);
					SetC10n(gCURRENCY_ID, Sql.ToFloat(EXCHANGE_RATE.Text));
				}

				dtLineItems = ViewState["LineItems"] as DataTable;
				grdMain.DataSource = dtLineItems;
				// 03/31/2007 Paul.  Don't bind the grid, otherwise edits will be lost. 
				//grdMain.DataBind();
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
			ScriptManager mgrAjax = Page.Master.FindControl("mgrAjax") as ScriptManager;
			if ( mgrAjax != null )
			{
				ServiceReference svc = new ServiceReference("~/Products/ProductCatalog/AutoComplete.asmx");
				ScriptReference  scr = new ScriptReference ("~/Products/ProductCatalog/AutoComplete.js"  );
				mgrAjax.Services.Add(svc);
				mgrAjax.Scripts .Add(scr);
			}
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
