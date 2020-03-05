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

namespace SplendidCRM.Products.ProductCatalog
{
	public class LineItem
	{
		public Guid    ID                 ;
		public string  NAME               ;
		public string  MFT_PART_NUM       ;
		public string  VENDOR_PART_NUM    ;
		public string  TAX_CLASS          ;
		public Decimal COST_PRICE         ;
		public Decimal COST_USDOLLAR      ;
		public Decimal LIST_PRICE         ;
		public Decimal LIST_USDOLLAR      ;
		public Decimal UNIT_PRICE         ;
		public Decimal UNIT_USDOLLAR      ;

		public LineItem()
		{
			ID                  = Guid.Empty  ;
			NAME                = String.Empty;
			MFT_PART_NUM        = String.Empty;
			VENDOR_PART_NUM     = String.Empty;
			TAX_CLASS           = String.Empty;
			COST_PRICE          = Decimal.Zero;
			COST_USDOLLAR       = Decimal.Zero;
			LIST_PRICE          = Decimal.Zero;
			LIST_USDOLLAR       = Decimal.Zero;
			UNIT_PRICE          = Decimal.Zero;
			UNIT_USDOLLAR       = Decimal.Zero;
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
		public LineItem GetItemDetailsByNumber(Guid gCURRENCY_ID, string sMFT_PART_NUM)
		{
			LineItem item = new LineItem();
			//try
			{
				if ( Security.USER_ID == Guid.Empty )
					throw(new Exception("Authentication required"));

				SplendidCRM.DbProviderFactory dbf = SplendidCRM.DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					// 02/04/2007 Paul.  Use LIKE clause so that the user can abbreviate unique part numbers. 
					sSQL = "select *                " + ControlChars.CrLf
					     + "  from vwPRODUCT_CATALOG" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "ProductTemplates", "list");
						Sql.AppendParameter(cmd, sMFT_PART_NUM, Sql.SqlFilterMode.StartsWith, "MFT_PART_NUM");
						// 07/02/2007 Paul.  Sort is important so that the first match is selected. 
						sSQL += " order by MFT_PART_NUM" + ControlChars.CrLf;
						using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
						{
							if ( rdr.Read() )
							{
								item.ID                  = Sql.ToGuid   (rdr["ID"                 ]);
								item.NAME                = Sql.ToString (rdr["NAME"               ]);
								item.MFT_PART_NUM        = Sql.ToString (rdr["MFT_PART_NUM"       ]);
								item.VENDOR_PART_NUM     = Sql.ToString (rdr["VENDOR_PART_NUM"    ]);
								item.TAX_CLASS           = Sql.ToString (rdr["TAX_CLASS"          ]);
								item.COST_PRICE          = Sql.ToDecimal(rdr["COST_PRICE"         ]);
								item.COST_USDOLLAR       = Sql.ToDecimal(rdr["COST_USDOLLAR"      ]);
								item.LIST_PRICE          = Sql.ToDecimal(rdr["LIST_PRICE"         ]);
								item.LIST_USDOLLAR       = Sql.ToDecimal(rdr["LIST_USDOLLAR"      ]);
								item.UNIT_PRICE          = Sql.ToDecimal(rdr["UNIT_PRICE"         ]);
								item.UNIT_USDOLLAR       = Sql.ToDecimal(rdr["UNIT_USDOLLAR"      ]);
								// 03/31/2007 Paul.  The price of the product may not be in the same currency as the order form. 
								// Make sure to convert to the specified currency. 
								if ( gCURRENCY_ID != Sql.ToGuid(rdr["CURRENCY_ID"]) )
								{
									Currency C10n = Currency.CreateCurrency(gCURRENCY_ID);
									item.COST_PRICE          = C10n.ToCurrency(item.COST_USDOLLAR);
									item.LIST_PRICE          = C10n.ToCurrency(item.LIST_USDOLLAR);
									item.UNIT_PRICE          = C10n.ToCurrency(item.UNIT_USDOLLAR);
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
		[WebMethod(EnableSession=true)]
		public LineItem GetItemDetailsByName(Guid gCURRENCY_ID, string sNAME)
		{
			LineItem item = new LineItem();
			//try
			{
				if ( Security.USER_ID == Guid.Empty )
					throw(new Exception("Authentication required"));

				SplendidCRM.DbProviderFactory dbf = SplendidCRM.DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					// 02/04/2007 Paul.  Use LIKE clause so that the user can abbreviate unique part numbers. 
					sSQL = "select *                " + ControlChars.CrLf
					     + "  from vwPRODUCT_CATALOG" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "ProductTemplates", "list");
						Sql.AppendParameter(cmd, sNAME, Sql.SqlFilterMode.StartsWith, "NAME");
						// 07/02/2007 Paul.  Sort is important so that the first match is selected. 
						sSQL += " order by NAME" + ControlChars.CrLf;
						using ( IDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow) )
						{
							if ( rdr.Read() )
							{
								item.ID                  = Sql.ToGuid   (rdr["ID"                 ]);
								item.NAME                = Sql.ToString (rdr["NAME"               ]);
								item.MFT_PART_NUM        = Sql.ToString (rdr["MFT_PART_NUM"       ]);
								item.VENDOR_PART_NUM     = Sql.ToString (rdr["VENDOR_PART_NUM"    ]);
								item.TAX_CLASS           = Sql.ToString (rdr["TAX_CLASS"          ]);
								item.COST_PRICE          = Sql.ToDecimal(rdr["COST_PRICE"         ]);
								item.COST_USDOLLAR       = Sql.ToDecimal(rdr["COST_USDOLLAR"      ]);
								item.LIST_PRICE          = Sql.ToDecimal(rdr["LIST_PRICE"         ]);
								item.LIST_USDOLLAR       = Sql.ToDecimal(rdr["LIST_USDOLLAR"      ]);
								item.UNIT_PRICE          = Sql.ToDecimal(rdr["UNIT_PRICE"         ]);
								item.UNIT_USDOLLAR       = Sql.ToDecimal(rdr["UNIT_USDOLLAR"      ]);
								// 03/31/2007 Paul.  The price of the product may not be in the same currency as the order form. 
								// Make sure to convert to the specified currency. 
								if ( gCURRENCY_ID != Sql.ToGuid(rdr["CURRENCY_ID"]) )
								{
									Currency C10n = Currency.CreateCurrency(gCURRENCY_ID);
									item.COST_PRICE          = C10n.ToCurrency(item.COST_USDOLLAR);
									item.LIST_PRICE          = C10n.ToCurrency(item.LIST_USDOLLAR);
									item.UNIT_PRICE          = C10n.ToCurrency(item.UNIT_USDOLLAR);
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
		public string[] ItemNumberList(string prefixText, int count)
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
					// 03/29/2007 Paul.  Use LIKE clause so that the user can abbreviate unique part numbers. 
					sSQL = "select MFT_PART_NUM     " + ControlChars.CrLf
					     + "  from vwPRODUCT_CATALOG" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "ProductTemplates", "list");
						Sql.AppendParameter(cmd, prefixText, Sql.SqlFilterMode.StartsWith, "MFT_PART_NUM");
						sSQL += " order by MFT_PART_NUM" + ControlChars.CrLf;
						using ( DbDataAdapter da = dbf.CreateDataAdapter() )
						{
							((IDbDataAdapter)da).SelectCommand = cmd;
							using ( DataTable dt = new DataTable() )
							{
								da.Fill(0, count, dt);
								arrItems = new string[dt.Rows.Count];
								for ( int i=0; i < dt.Rows.Count; i++ )
									arrItems[i] = Sql.ToString(dt.Rows[i]["MFT_PART_NUM"]);
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

		// 03/30/2007 Paul.  Enable sessions so that we can require authentication to access the data. 
		// 03/29/2007 Paul.  In order for AutoComplete to work, the parameter names must be "prefixText" and "count". 
		[WebMethod(EnableSession=true)]
		public string[] ItemNameList(string prefixText, int count)
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
					sSQL = "select NAME             " + ControlChars.CrLf
					     + "  from vwPRODUCT_CATALOG" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "ProductTemplates", "list");
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

