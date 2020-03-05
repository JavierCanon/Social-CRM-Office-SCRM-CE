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
using System.Collections;
using System.Web.Services;
using System.ComponentModel;
using SplendidCRM;

namespace SplendidCRM.Invoices
{
	public class Invoice
	{
		public Guid    ID                 ;
		public string  NAME               ;
		public Decimal AMOUNT_DUE         ;
		public Decimal AMOUNT_DUE_USDOLLAR;

		public Invoice()
		{
			ID                  = Guid.Empty  ;
			NAME                = String.Empty;
			AMOUNT_DUE          = Decimal.Zero;
			AMOUNT_DUE_USDOLLAR = Decimal.Zero;
		}
	}

	/// <summary>
	/// Summary description for AutoComplete
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.Web.Script.Services.ScriptService]
	[ToolboxItem(false)]
	public class AutoComplete : System.Web.Services.WebService
	{
		// 03/30/2007 Paul.  Enable sessions so that we can require authentication to access the data. 
		[WebMethod(EnableSession=true)]
		public Invoice GetInvoiceByName(Guid gCURRENCY_ID, string sNAME)
		{
			Invoice item = new Invoice();
			//try
			{
				if ( Security.USER_ID == Guid.Empty )
					throw(new Exception("Authentication required"));

				SplendidCRM.DbProviderFactory dbf = SplendidCRM.DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					// 04/13/2007 Paul.  Use LIKE clause so that the user can abbreviate Names. 
					sSQL = "select *         " + ControlChars.CrLf
					     + "  from vwINVOICES" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "Invoices", "list");
						Sql.AppendParameter(cmd, sNAME, Sql.SqlFilterMode.StartsWith, "NAME");
						using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
						{
							if ( rdr.Read() )
							{
								item.ID                  = Sql.ToGuid   (rdr["ID"                 ]);
								item.NAME                = Sql.ToString (rdr["NAME"               ]);
								item.AMOUNT_DUE          = Sql.ToDecimal(rdr["AMOUNT_DUE"         ]);
								item.AMOUNT_DUE_USDOLLAR = Sql.ToDecimal(rdr["AMOUNT_DUE_USDOLLAR"]);
								// 03/31/2007 Paul.  The price of the product may not be in the same currency as the order form. 
								// Make sure to convert to the specified currency. 
								if ( gCURRENCY_ID != Sql.ToGuid(rdr["CURRENCY_ID"]) )
								{
									Currency C10n = Currency.CreateCurrency(gCURRENCY_ID);
									item.AMOUNT_DUE = C10n.ToCurrency(item.AMOUNT_DUE_USDOLLAR);
								}
							}
						}
					}
				}
				if ( Sql.IsEmptyGuid(item.ID) )
					throw(new Exception("Item not found"));
			}
			//catch
			{
				// 02/04/2007 Paul.  Don't catch the exception.  
				// It is a web service, so the exception will be handled properly by the AJAX framework. 
			}
			return item;
		}

		// 03/30/2007 Paul.  Enable sessions so that we can require authentication to access the data. 
		// 03/29/2007 Paul.  In order for AutoComplete to work, the parameter names must be "prefixText" and "count". 
		[WebMethod(EnableSession=true)]
		public string[] InvoiceNameList(string prefixText, int count)
		{
			string[] arrItems = new string[0];
			try
			{
				if ( Security.USER_ID == Guid.Empty )
					throw(new Exception("Authentication required"));

				SplendidCRM.DbProviderFactory dbf = SplendidCRM.DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					string sSQL;
					sSQL = "select NAME      " + ControlChars.CrLf
					     + "  from vwINVOICES" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "Invoices", "list");
						Sql.AppendParameter(cmd, prefixText, Sql.SqlFilterMode.StartsWith, "NAME");
						sSQL += " order by NAME" + ControlChars.CrLf;
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(0, count, dt);
								arrItems = new string[dt.Rows.Count];
								for ( int i=0; i < dt.Rows.Count; i++ )
									arrItems[i] = Sql.ToString(dt.Rows[i]["NAME"]);
							}
						}
					}
				}
			}
			catch
			{
			}
			return arrItems;
		}
	}
}

