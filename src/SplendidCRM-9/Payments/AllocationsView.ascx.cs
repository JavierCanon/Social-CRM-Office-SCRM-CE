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
	///		Summary description for AllocationsView.
	/// </summary>
	public class AllocationsView : SplendidControl
	{
		protected DataTable       dtLineItems           ;
		protected GridView        grdMain               ;
		protected Label           lblLineItemError      ;
		protected DropDownList    CURRENCY_ID           ;
		protected TextBox         ALLOCATED             ;
		protected HiddenField     ALLOCATED_USDOLLAR    ;
		protected HiddenField     EXCHANGE_RATE         ;

		public DataTable LineItems
		{
			get { return dtLineItems; }
		}

		#region Line Item Editing
		protected void grdMain_RowCreated(object sender, GridViewRowEventArgs e)
		{
			if ( e.Row.RowType == DataControlRowType.DataRow )
			{
			}
		}

		protected void grdMain_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if ( e.Row.RowType == DataControlRowType.DataRow )
			{
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
				if ( aCurrentRows.Length == 0 || !Sql.IsEmptyGuid(aCurrentRows[aCurrentRows.Length-1]["INVOICE_ID"]) )
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
				TextBox     txtINVOICE_NAME    = gr.FindControl("INVOICE_NAME"   ) as TextBox     ;
				HiddenField txtINVOICE_ID      = gr.FindControl("INVOICE_ID"     ) as HiddenField ;
				TextBox     txtAMOUNT          = gr.FindControl("AMOUNT"         ) as TextBox     ;
				HiddenField txtAMOUNT_USDOLLAR = gr.FindControl("AMOUNT_USDOLLAR") as HiddenField ;

				DataRow row = dtLineItems.Rows[e.RowIndex];
				// 03/31/2007 Paul.  The US Dollar values are computed from the price and are only used when changing currencies. 
				if ( txtINVOICE_NAME != null ) row["INVOICE_NAME"] =               txtINVOICE_NAME.Text;
				if ( txtINVOICE_ID   != null ) row["INVOICE_ID"  ] = Sql.ToGuid   (txtINVOICE_ID  .Value);
				if ( txtAMOUNT       != null ) row["AMOUNT"      ] = Sql.ToDecimal(txtAMOUNT      .Text);
				
				row["AMOUNT_USDOLLAR"] = C10n.FromCurrency(Sql.ToDecimal(txtAMOUNT.Text));

				DataRow[] aCurrentRows = dtLineItems.Select(String.Empty, String.Empty, DataViewRowState.CurrentRows);
				// 03/30/2007 Paul.  Always allow editing of the last empty row. Add blank row if necessary. 
				if ( aCurrentRows.Length == 0 || !Sql.IsEmptyString(aCurrentRows[aCurrentRows.Length-1]["INVOICE_NAME"]) )
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

		public void CURRENCY_ID_Changed(object sender, System.EventArgs e)
		{
			// 03/31/2007 Paul.  When the currency changes, use the default exchange rate. 
			Guid gCURRENCY_ID = Sql.ToGuid(CURRENCY_ID.SelectedValue);
			SetC10n(gCURRENCY_ID);
			EXCHANGE_RATE.Value = C10n.CONVERSION_RATE.ToString();

			foreach ( DataRow row in dtLineItems.Rows )
			{
				if ( row.RowState != DataRowState.Deleted )
				{
					row["AMOUNT"] = C10n.ToCurrency(Sql.ToDecimal(row["AMOUNT_USDOLLAR"]));
				}
			}
			grdMain.DataBind();

			UpdateTotals();
		}

		private void UpdateTotals()
		{
			Double dALLOCATED = 0.0;

			foreach ( DataRow row in dtLineItems.Rows )
			{
				if ( row.RowState != DataRowState.Deleted )
				{
					Double dAMOUNT = Sql.ToDouble (row["AMOUNT"]);
					dALLOCATED += dAMOUNT;
				}
			}
			ALLOCATED.Text           = Convert.ToDecimal(dALLOCATED).ToString("c");
			ALLOCATED_USDOLLAR.Value = C10n.FromCurrency(Convert.ToDecimal(dALLOCATED)).ToString("0.000");
		}
		#endregion

		public void LoadLineItems(Guid gID, Guid gDuplicateID, IDbConnection con, IDataReader rdr)
		{
			if ( !IsPostBack )
			{
				GetCurrencyControl();
				try
				{
					// 03/15/2007 Paul.  Set the default currency to the current user currency. 
					CURRENCY_ID.SelectedValue = C10n.ID.ToString();
					EXCHANGE_RATE.Value = C10n.CONVERSION_RATE.ToString();
				}
				catch
				{
				}
				foreach ( DataControlField col in grdMain.Columns )
				{
					if ( !Sql.IsEmptyString(col.HeaderText) )
					{
						col.HeaderText = L10n.Term(col.HeaderText);
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

				if ( (!Sql.IsEmptyGuid(gID) || !Sql.IsEmptyGuid(gDuplicateID)) && (con != null) && (rdr != null) )
				{
					try
					{
						CURRENCY_ID.SelectedValue = Sql.ToString(rdr["CURRENCY_ID"]);
						// 05/26/2007 Paul.  Make sure to update the exchange rate. 
						EXCHANGE_RATE.Value = C10n.CONVERSION_RATE.ToString();
					}
					catch
					{
					}
					// 03/31/2007 Paul.  The exchange rate might be an old value. 
					float fEXCHANGE_RATE = Sql.ToFloat(rdr["EXCHANGE_RATE"]);
					if ( fEXCHANGE_RATE == 0.0f )
						fEXCHANGE_RATE = 1.0f;
					EXCHANGE_RATE.Value = fEXCHANGE_RATE.ToString();
					if ( CURRENCY_ID.Items.Count > 0 )
					{
						// 03/31/2007 Paul.  Replace the user currency with the form currency, but use the old exchange rate. 
						Guid gCURRENCY_ID = Sql.ToGuid(CURRENCY_ID.SelectedValue);
						SetC10n(gCURRENCY_ID, fEXCHANGE_RATE);
						EXCHANGE_RATE.Value = C10n.CONVERSION_RATE.ToString();
					}
					// 05/26/2007 Paul.  ALLOCATED field is not returned, just TOTAL_ALLOCATED_USDOLLAR. 
					// Don't convert TOTAL_ALLOCATED_USDOLLAR in the hidden variable. 
					ALLOCATED.Text           = C10n.ToCurrency(Sql.ToDecimal(rdr["TOTAL_ALLOCATED_USDOLLAR"])).ToString("c");
					ALLOCATED_USDOLLAR.Value = Sql.ToDecimal(rdr["TOTAL_ALLOCATED_USDOLLAR"]).ToString("0.00");
					rdr.Close();

					string sSQL;
					sSQL = "select *                  " + ControlChars.CrLf
					     + "  from vwPAYMENTS_INVOICES" + ControlChars.CrLf
					     + " where 1 = 1              " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Sql.AppendParameter(cmd, gID, "PAYMENT_ID", false);
						cmd.CommandText += " order by DATE_MODIFIED asc" + ControlChars.CrLf;

						if ( bDebug )
							RegisterClientScriptBlock("vwPAYMENTS_INVOICES", Sql.ClientScriptBlock(cmd));

						DbProviderFactory dbf = DbProviderFactories.GetFactory();
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							dtLineItems = new DataTable();
							da.Fill(dtLineItems);
							
							// 04/01/2007 Paul.  If we are duplicating a quote, then we must create new IDs for the line items. 
							// Otherwise, edits to the line items will change the old quote. 
							if ( !Sql.IsEmptyGuid(gDuplicateID) )
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
					DataColumn colID              = new DataColumn("ID"             , Type.GetType("System.Guid"   ));
					DataColumn colINVOICE_NAME    = new DataColumn("INVOICE_NAME"   , Type.GetType("System.String" ));
					DataColumn colINVOICE_ID      = new DataColumn("INVOICE_ID"     , Type.GetType("System.Guid"   ));
					DataColumn colAMOUNT          = new DataColumn("AMOUNT"         , Type.GetType("System.Decimal"));
					DataColumn colAMOUNT_USDOLLAR = new DataColumn("AMOUNT_USDOLLAR", Type.GetType("System.Decimal"));
					dtLineItems.Columns.Add(colID             );
					dtLineItems.Columns.Add(colINVOICE_NAME   );
					dtLineItems.Columns.Add(colINVOICE_ID     );
					dtLineItems.Columns.Add(colAMOUNT         );
					dtLineItems.Columns.Add(colAMOUNT_USDOLLAR);
					// 03/27/2007 Paul.  Always add blank line to allow quick editing. 
					DataRow rowNew = null;

					DbProviderFactory dbf = DbProviderFactories.GetFactory();
					using ( IDbConnection conInvoice = dbf.CreateConnection() )
					{
						conInvoice.Open();
						string sSQL ;
						sSQL = "select *              " + ControlChars.CrLf
						     + "  from vwINVOICES_Edit" + ControlChars.CrLf
						     + " where 1 = 1          " + ControlChars.CrLf;
						using ( IDbCommand cmd = conInvoice.CreateCommand() )
						{
							cmd.CommandText = sSQL;
							Guid gPARENT_ID = Sql.ToGuid(Request["PARENT_ID"]);
							Sql.AppendParameter(cmd, gPARENT_ID, "ID", false);

							if ( bDebug )
								RegisterClientScriptBlock("vwINVOICES_Edit", Sql.ClientScriptBlock(cmd));

							using ( IDataReader rdrInvoice = cmd.ExecuteReader(CommandBehavior.SingleRow) )
							{
								if ( rdrInvoice.Read() )
								{
									rowNew = dtLineItems.NewRow();
									rowNew["INVOICE_NAME"   ] = Sql.ToString (rdrInvoice["NAME"               ]);
									rowNew["INVOICE_ID"     ] = Sql.ToGuid   (rdrInvoice["ID"                 ]);
									rowNew["AMOUNT"         ] = Sql.ToDecimal(rdrInvoice["AMOUNT_DUE"         ]);
									rowNew["AMOUNT_USDOLLAR"] = Sql.ToDecimal(rdrInvoice["AMOUNT_DUE_USDOLLAR"]);
									dtLineItems.Rows.Add(rowNew);
								}
							}
						}
					}
					rowNew = dtLineItems.NewRow();
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

		protected DropDownList GetCurrencyControl()
		{
			try
			{
				if ( CURRENCY_ID == null )
					CURRENCY_ID = Parent.FindControl("tblMain").FindControl("CURRENCY_ID") as DropDownList;
			}
			catch
			{
			}
			return CURRENCY_ID;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			GetCurrencyControl();
			if ( IsPostBack )
			{
				try
				{
					if ( CURRENCY_ID.Items.Count > 0 )
					{
						// 03/31/2007 Paul.  Replace the user currency with the form currency, but use the old exchange rate. 
						Guid gCURRENCY_ID = Sql.ToGuid(CURRENCY_ID.SelectedValue);
						SetC10n(gCURRENCY_ID, Sql.ToFloat(EXCHANGE_RATE.Value));
						EXCHANGE_RATE.Value = C10n.CONVERSION_RATE.ToString();
					}
				}
				catch
				{
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
				ServiceReference svc = new ServiceReference("~/Invoices/AutoComplete.asmx");
				ScriptReference  scr = new ScriptReference ("~/Invoices/AutoComplete.js"  );
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
