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

namespace SplendidCRM.Campaigns.xml
{
	/// <summary>
	/// Summary description for ReturnOnInvestment.
	/// </summary>
	public class ReturnOnInvestment : SplendidPage
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			XmlDocument xml = new XmlDocument();
			try
			{
				Guid gID = Sql.ToGuid(Request["ID"]);
				xml.LoadXml(SplendidCache.XmlFile(Server.MapPath(Session["themeURL"] + "BarChart.xml")));
				XmlNode nodeRoot        = xml.SelectSingleNode("graphData");
				XmlNode nodeXData       = xml.CreateElement("xData"      );
				XmlNode nodeYData       = xml.CreateElement("yData"      );
				XmlNode nodeColorLegend = xml.CreateElement("colorLegend");
				XmlNode nodeGraphInfo   = xml.CreateElement("graphInfo"  );
				XmlNode nodeChartColors = nodeRoot.SelectSingleNode("chartColors");

				nodeRoot.InsertBefore(nodeGraphInfo  , nodeChartColors);
				nodeRoot.InsertBefore(nodeColorLegend, nodeGraphInfo  );
				nodeRoot.InsertBefore(nodeXData      , nodeColorLegend);
				nodeRoot.InsertBefore(nodeYData      , nodeXData      );

				XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "min"   , "0" );
				XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "max"   , "80");
				XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "length", "10");
				XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "prefix", ""  );
				XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "suffix", ""  );

				XmlUtil.SetSingleNodeAttribute(xml, nodeYData, "min"   , "0" );
				XmlUtil.SetSingleNodeAttribute(xml, nodeYData, "max"   , "10");
				XmlUtil.SetSingleNodeAttribute(xml, nodeYData, "length", "10");
				XmlUtil.SetSingleNodeAttribute(xml, nodeYData, "defaultAltText", L10n.Term("Campaigns.LBL_ROLLOVER_VIEW"));
				
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					DataTable dtLegend = SplendidCache.List("roi_type_dom");
					XmlUtil.SetSingleNodeAttribute(xml, nodeColorLegend, "status", "on");
					for ( int i = 0; i < dtLegend.Rows.Count; i++ )
					{
						DataRow row = dtLegend.Rows[i];
						XmlNode nodeMapping = xml.CreateElement("mapping");
						nodeColorLegend.AppendChild(nodeMapping);
						XmlUtil.SetSingleNodeAttribute(xml, nodeMapping, "id"   , Sql.ToString(row["NAME"        ]));
						XmlUtil.SetSingleNodeAttribute(xml, nodeMapping, "name" , Sql.ToString(row["DISPLAY_NAME"]));
						XmlUtil.SetSingleNodeAttribute(xml, nodeMapping, "color", SplendidDefaults.generate_graphcolor(String.Empty, i));
					}
					
					sSQL = "select *              " + ControlChars.CrLf
					     + "  from vwCAMPAIGNS_Roi" + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "Campaigns", "view");
						Sql.AppendParameter(cmd, gID, "ID", false);
						using ( IDataReader rdr = cmd.ExecuteReader() )
						{
							if ( rdr.Read() )
							{
								double dBUDGET           = 0.0;
								double dEXPECTED_REVENUE = 0.0;
								double dINVESTMENT       = 0.0;
								double dREVENUE          = 0.0;
								Hashtable hashTOTALS = new Hashtable();
								try
								{
									dBUDGET           = Sql.ToDouble(rdr["BUDGET"          ]);
									dEXPECTED_REVENUE = Sql.ToDouble(rdr["EXPECTED_REVENUE"]);
									dINVESTMENT       = Sql.ToDouble(rdr["ACTUAL_COST"     ]);
									dREVENUE          = Sql.ToDouble(rdr["REVENUE"         ]);
									hashTOTALS.Add("Budget"          , dBUDGET          );
									hashTOTALS.Add("Expected_Revenue", dEXPECTED_REVENUE);
									hashTOTALS.Add("Investment"      , dINVESTMENT      );
									hashTOTALS.Add("Revenue"         , dREVENUE         );
								}
								catch(Exception ex)
								{
									SplendidError.SystemError(new StackTrace(true).GetFrame(0), ex);
								}
								foreach ( DataRow row in dtLegend.Rows )
								{
									string sNAME         = Sql.ToString(row["NAME"        ]);
									string sDISPLAY_NAME = Sql.ToString(row["DISPLAY_NAME"]);
									XmlNode nodeRow = xml.CreateElement("dataRow");
									nodeYData.AppendChild(nodeRow);
									XmlUtil.SetSingleNodeAttribute(xml, nodeRow, "title"   , sDISPLAY_NAME);
									XmlUtil.SetSingleNodeAttribute(xml, nodeRow, "endLabel", sDISPLAY_NAME.Substring(0, 1));
									
									XmlNode nodeBar = xml.CreateElement("bar");
									nodeRow.AppendChild(nodeBar);
									double dTOTAL = Sql.ToDouble(hashTOTALS[sNAME]);
									XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "id"       , sNAME);
									XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "totalSize", dTOTAL.ToString("0"));
									XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "altText"  , dTOTAL.ToString("0"));
									XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "url"      , "#" + sNAME);
								}
								
								double dMAX = 0.0;
								dMAX = Math.Max(dMAX, dREVENUE         );
								dMAX = Math.Max(dMAX, dINVESTMENT      );
								dMAX = Math.Max(dMAX, dBUDGET          );
								dMAX = Math.Max(dMAX, dEXPECTED_REVENUE);
								dMAX = dMAX * 1.2;  // Increase by 20%. 
								if ( dMAX <= 0.0 )
									dMAX = 80.0;
								double dMAX_ROUNDED = Math.Ceiling(dMAX);
								XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "max", dMAX_ROUNDED.ToString("0"));
							}
							XmlUtil.SetSingleNodeAttribute(xml, nodeRoot , "title", L10n.Term("Campaigns.LBL_CAMPAIGN_RETURN_ON_INVESTMENT") + "                                                                                                            ");
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
