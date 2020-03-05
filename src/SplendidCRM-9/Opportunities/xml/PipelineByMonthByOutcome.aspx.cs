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
using System.Xml;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Diagnostics;
//using Microsoft.VisualBasic;

namespace SplendidCRM.Opportunities.xml
{
	/// <summary>
	/// Summary description for PipelineByMonthByOutcome.
	/// </summary>
	public class PipelineByMonthByOutcome : SplendidPage
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			XmlDocument xml = new XmlDocument();
			try
			{
				// 09/15/2005 Paul.  Values will always be in the query string. 
				int      nCHART_LENGTH = Sql.ToInteger (Request.QueryString["CHART_LENGTH"]);
				int      nYEAR         = Sql.ToInteger (Request.QueryString["YEAR"]);
				nYEAR = Math.Max(1900, nYEAR);
				nYEAR = Math.Min(2100, nYEAR);
				DateTime dtDATE_START  = new DateTime(nYEAR,  1,  1);
				DateTime dtDATE_END    = new DateTime(nYEAR, 12, 31);
				// 09/15/2005 Paul.  Values will always be in the query string. 
				string[] arrASSIGNED_USER_ID = Request.QueryString.GetValues("ASSIGNED_USER_ID");

				xml.LoadXml(SplendidCache.XmlFile(Server.MapPath(Session["themeURL"] + "BarChart.xml")));
				XmlNode nodeRoot        = xml.SelectSingleNode("graphData");
				XmlNode nodeXData       = xml.CreateElement("xData"      );
				XmlNode nodeYData       = xml.CreateElement("yData"      );
				XmlNode nodeColorLegend = xml.CreateElement("colorLegend");
				XmlNode nodeGraphInfo   = xml.CreateElement("graphInfo"  );
				XmlNode nodeChartColors = nodeRoot.SelectSingleNode("chartColors");

				nodeRoot.InsertBefore(nodeGraphInfo  , nodeChartColors);
				nodeRoot.InsertBefore(nodeColorLegend, nodeGraphInfo  );
				nodeRoot.InsertBefore(nodeYData      , nodeColorLegend);
				nodeRoot.InsertBefore(nodeXData      , nodeYData      );

				XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "length", "20");
				XmlUtil.SetSingleNodeAttribute(xml, nodeYData, "min", "0");
				XmlUtil.SetSingleNodeAttribute(xml, nodeYData, "max", "0");
				if ( nCHART_LENGTH < 4 )
					nCHART_LENGTH = 4;
				else if ( nCHART_LENGTH > 10 )
					nCHART_LENGTH = 10;
				XmlUtil.SetSingleNodeAttribute(xml, nodeYData, "length", nCHART_LENGTH.ToString());
				XmlUtil.SetSingleNodeAttribute(xml, nodeYData, "prefix", Sql.ToString(Session["USER_SETTINGS/CURRENCY_SYMBOL"]));
				XmlUtil.SetSingleNodeAttribute(xml, nodeYData, "suffix", "");
				XmlUtil.SetSingleNodeAttribute(xml, nodeYData, "defaultAltText", L10n.Term("Dashboard.LBL_ROLLOVER_DETAILS"));
				
				nodeGraphInfo.InnerText = L10n.Term("Dashboard.LBL_DATE_RANGE") + " " + Sql.ToDateString(T10n.FromServerTime(dtDATE_START)) + " " + L10n.Term("Dashboard.LBL_DATE_RANGE_TO") + Sql.ToDateString(T10n.FromServerTime(dtDATE_END)) + "<BR/>"
				                        + L10n.Term("Dashboard.LBL_OPP_SIZE"  ) + " " + Strings.FormatCurrency(1, 0, TriState.UseDefault, TriState.UseDefault, TriState.UseDefault) + L10n.Term("Dashboard.LBL_OPP_THOUSANDS");
				
				Hashtable hashOUTCOME = new Hashtable();
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					// 09/19/2005 Paul.  Prepopulate the outcome. 
					string[] arrOUTCOME = new string[] { "Closed Lost", "Closed Won", "Other" };
					foreach(string sOUTCOME in arrOUTCOME)
					{
						if ( !hashOUTCOME.ContainsKey(sOUTCOME) )
						{
							XmlNode nodeMapping = xml.CreateElement("mapping");
							nodeColorLegend.AppendChild(nodeMapping);
							XmlUtil.SetSingleNodeAttribute(xml, nodeMapping, "id"   , sOUTCOME);
							if ( sOUTCOME == "Other" )
								XmlUtil.SetSingleNodeAttribute(xml, nodeMapping, "name" , L10n.Term("Dashboard.LBL_LEAD_SOURCE_OTHER"));
							else
								XmlUtil.SetSingleNodeAttribute(xml, nodeMapping, "name" , Sql.ToString(L10n.Term(".sales_stage_dom.", sOUTCOME)));
							XmlUtil.SetSingleNodeAttribute(xml, nodeMapping, "color", SplendidDefaults.generate_graphcolor(sOUTCOME, hashOUTCOME.Count));
							hashOUTCOME.Add(sOUTCOME, sOUTCOME);
						}
					}
					sSQL = "select SALES_STAGE                                   " + ControlChars.CrLf
					     + "     , MONTH_CLOSED                                  " + ControlChars.CrLf
					     + "     , sum(AMOUNT_USDOLLAR/1000) as TOTAL            " + ControlChars.CrLf
					     + "     , count(*)                  as OPPORTUNITY_COUNT" + ControlChars.CrLf
					     + "  from vwOPPORTUNITIES_PipelineMonth                 " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "Opportunities", "list");
						cmd.CommandText += "   and DATE_CLOSED >= @DATE_START" + ControlChars.CrLf;
						cmd.CommandText += "   and DATE_CLOSED <= @DATE_END  " + ControlChars.CrLf;
						// 09/14/2005 Paul.  Use add because <= and >= are not supported. 
						Sql.AddParameter   (cmd, "@DATE_START"      , dtDATE_START      );
						Sql.AddParameter   (cmd, "@DATE_END"        , dtDATE_END        );
						// 09/14/2005 Paul.  Use append because it supports arrays using the IN clause. 
						Sql.AppendGuids    (cmd, arrASSIGNED_USER_ID, "ASSIGNED_USER_ID");
						
						cmd.CommandText += ""
						     + " group by SALES_STAGE                                " + ControlChars.CrLf
						     + "        , MONTH_CLOSED                               " + ControlChars.CrLf
						     + " order by MONTH_CLOSED, SALES_STAGE desc             " + ControlChars.CrLf;
						using ( IDataReader rdr = cmd.ExecuteReader() )
						{
							double dMAX_TOTAL      = 0;
							double dPIPELINE_TOTAL = 0;
							string sMONTHYEAR_FORMAT = m_sDATEFORMAT;
							// 09/21/2005 Paul.  Remove day from format. 
							sMONTHYEAR_FORMAT = sMONTHYEAR_FORMAT.Replace("dd", "");
							sMONTHYEAR_FORMAT = sMONTHYEAR_FORMAT.Replace("--", "-");
							sMONTHYEAR_FORMAT = sMONTHYEAR_FORMAT.Replace("//", "/");
							sMONTHYEAR_FORMAT = sMONTHYEAR_FORMAT.Replace("  ", " ");
							while ( rdr.Read() )
							{
								int      nMONTH_CLOSED      = Sql.ToInteger(rdr["MONTH_CLOSED"     ]);
								string   sSALES_STAGE       = Sql.ToString (rdr["SALES_STAGE"      ]);
								double   dTOTAL             = Sql.ToDouble (rdr["TOTAL"            ]);
								int      nOPPORTUNITY_COUNT = Sql.ToInteger(rdr["OPPORTUNITY_COUNT"]);
								DateTime dtMONTH_CLOSED     = new DateTime(nYEAR, nMONTH_CLOSED, 1);
								string   sMONTH_CLOSED      = dtMONTH_CLOSED.ToString(sMONTHYEAR_FORMAT);
								
								dPIPELINE_TOTAL += dTOTAL;
								if ( dTOTAL > dMAX_TOTAL )
									dMAX_TOTAL = dTOTAL;
								XmlNode nodeRow = nodeXData.SelectSingleNode("dataRow[@title=\'" + L10n.Term(sMONTH_CLOSED).Replace("'", "\'") +"\']");
								if ( nodeRow == null )
								{
									nodeRow = xml.CreateElement("dataRow");
									nodeXData.AppendChild(nodeRow);
									XmlUtil.SetSingleNodeAttribute(xml, nodeRow, "title"   , sMONTH_CLOSED);
									XmlUtil.SetSingleNodeAttribute(xml, nodeRow, "endLabel", dTOTAL.ToString("0")   );
								}
								else
								{
									if ( nodeRow.Attributes.GetNamedItem("endLabel") != null )
									{
										double dEND_LABEL = Sql.ToDouble(nodeRow.Attributes.GetNamedItem("endLabel").Value);
										dEND_LABEL += dTOTAL;
										if ( dEND_LABEL > dMAX_TOTAL )
											dMAX_TOTAL = dEND_LABEL;
										XmlUtil.SetSingleNodeAttribute(xml, nodeRow, "endLabel", dEND_LABEL.ToString("0")   );
									}
								}
								
								XmlNode nodeBar = xml.CreateElement("bar");
								nodeRow.AppendChild(nodeBar);
								XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "id"       , sSALES_STAGE);
								XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "totalSize", dTOTAL.ToString("0"));
								XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "altText"  , sMONTH_CLOSED + ": " + nOPPORTUNITY_COUNT.ToString() + " " + L10n.Term("Dashboard.LBL_OPPS_WORTH") + " " + dTOTAL.ToString("0") + L10n.Term("Dashboard.LBL_OPP_THOUSANDS") + " " + L10n.Term("Dashboard.LBL_OPPS_OUTCOME") + " " + Sql.ToString(L10n.Term(".sales_stage_dom.", sSALES_STAGE)) );
								XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "url"      , Sql.ToString(Application["rootURL"]) + "Opportunities/default.aspx?DATE_CLOSED=" + Server.UrlEncode(Sql.ToDateString(T10n.FromServerTime(dtMONTH_CLOSED))) + "&SALES_STAGE=" + Server.UrlEncode(sSALES_STAGE) );
							}
							int    nNumLength   = Math.Floor(dMAX_TOTAL).ToString("0").Length - 1;
							double dWhole       = Math.Pow(10, nNumLength);
							double dDecimal     = 1 / dWhole;
							double dMAX_ROUNDED = Math.Ceiling(dMAX_TOTAL * dDecimal) * dWhole;
							
							XmlUtil.SetSingleNodeAttribute(xml, nodeYData, "max", dMAX_ROUNDED.ToString("0"));
							XmlUtil.SetSingleNodeAttribute(xml, nodeRoot , "title", L10n.Term("Dashboard.LBL_TOTAL_PIPELINE") + Strings.FormatCurrency(dPIPELINE_TOTAL, 0, TriState.UseDefault, TriState.UseDefault, TriState.UseDefault) + L10n.Term("Dashboard.LBL_OPP_THOUSANDS"));
						}
					}
				}
				Response.ContentType = "text/xml";
				Response.Write(xml.OuterXml);
			}
			catch(Exception ex)
			{
				SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
				Response.Write(ex.Message);
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
		}
		#endregion
	}
}
