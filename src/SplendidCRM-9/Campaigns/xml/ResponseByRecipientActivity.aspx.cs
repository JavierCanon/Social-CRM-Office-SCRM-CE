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
	/// Summary description for ResponseByRecipientActivity.
	/// </summary>
	public class ResponseByRecipientActivity : SplendidPage
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

				XmlUtil.SetSingleNodeAttribute(xml, nodeYData, "defaultAltText", L10n.Term("Campaigns.LBL_ROLLOVER_VIEW"));
				XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "min"   , "0" );
				XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "max"   , "100");
				XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "length", "10");
				XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "prefix", ""  );
				XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "suffix", ""  );

				Hashtable hashTARGET = new Hashtable();
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					// 12/25/2007 Paul.  Prepopulate the activity type rows so that empty rows will appear.  The SQL query will not return empty rows. 
					DataTable dtActivityTypes = SplendidCache.List("campainglog_activity_type_dom").Copy();
					DataRow rowActivityTypeNone = dtActivityTypes.NewRow();
					dtActivityTypes.Rows.InsertAt(rowActivityTypeNone, 0);
					rowActivityTypeNone["NAME"        ] = "";
					rowActivityTypeNone["DISPLAY_NAME"] = L10n.Term("Campaigns.NTC_NO_LEGENDS");
					foreach ( DataRow row in dtActivityTypes.Rows )
					{
						XmlNode nodeRow = xml.CreateElement("dataRow");
						nodeYData.AppendChild(nodeRow);
						XmlUtil.SetSingleNodeAttribute(xml, nodeRow, "title"   , Sql.ToString(row["DISPLAY_NAME"]));
						XmlUtil.SetSingleNodeAttribute(xml, nodeRow, "endLabel", "0");
					}

					// 12/25/2007 Paul.  Prepopulate the targets. 
					DataTable dtLegend = SplendidCache.List("campainglog_target_type_dom");
					XmlUtil.SetSingleNodeAttribute(xml, nodeColorLegend, "status", "on");
					for ( int i = 0; i < dtLegend.Rows.Count; i++ )
					{
						DataRow row = dtLegend.Rows[i];
						string sTARGET = Sql.ToString(row["NAME"]);
						if ( !hashTARGET.ContainsKey(sTARGET) )
						{
							XmlNode nodeMapping = xml.CreateElement("mapping");
							nodeColorLegend.AppendChild(nodeMapping);
							XmlUtil.SetSingleNodeAttribute(xml, nodeMapping, "id"   , Sql.ToString(row["NAME"        ]));
							XmlUtil.SetSingleNodeAttribute(xml, nodeMapping, "name" , Sql.ToString(row["DISPLAY_NAME"]));
							XmlUtil.SetSingleNodeAttribute(xml, nodeMapping, "color", SplendidDefaults.generate_graphcolor(String.Empty, hashTARGET.Count));
							hashTARGET.Add(sTARGET, sTARGET);
						}
					}

					sSQL = "select ACTIVITY_TYPE                         " + ControlChars.CrLf
					     + "     , TARGET_TYPE                           " + ControlChars.CrLf
					     + "     , LIST_ORDER                            " + ControlChars.CrLf
					     + "     , count(*)                  as HIT_COUNT" + ControlChars.CrLf
					     + "  from vwCAMPAIGNS_Activity                  " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "Campaigns", "view");
						Sql.AppendParameter(cmd, gID, "ID", false);
						cmd.CommandText += ""
						     + " group by ACTIVITY_TYPE                      " + ControlChars.CrLf
						     + "        , LIST_ORDER                         " + ControlChars.CrLf
						     + "        , TARGET_TYPE                        " + ControlChars.CrLf
						     + " order by LIST_ORDER                         " + ControlChars.CrLf
						     + "        , TARGET_TYPE                        " + ControlChars.CrLf;
						using ( IDataReader rdr = cmd.ExecuteReader() )
						{
							int nMAX_COUNT = 0;
							while ( rdr.Read() )
							{
								string  sACTIVITY_TYPE = Sql.ToString (rdr["ACTIVITY_TYPE"]);
								string  sTARGET_TYPE   = Sql.ToString (rdr["TARGET_TYPE"  ]);
								int     nHIT_COUNT     = Sql.ToInteger(rdr["HIT_COUNT"    ]);
								
								if ( nHIT_COUNT > nMAX_COUNT )
									nMAX_COUNT = nHIT_COUNT;
								string sACTIVITY_TYPE_TERM = String.Empty;
								if ( sACTIVITY_TYPE == String.Empty )
									sACTIVITY_TYPE_TERM = L10n.Term("Campaigns.NTC_NO_LEGENDS");
								else
									sACTIVITY_TYPE_TERM = Sql.ToString(L10n.Term(".campainglog_activity_type_dom.", sACTIVITY_TYPE));
								
								int nEND_LABEL = nHIT_COUNT;
								
								XmlNode nodeRow = nodeYData.SelectSingleNode("dataRow[@title=\'" + sACTIVITY_TYPE_TERM.Replace("'", "\'") +"\']");
								if ( nodeRow == null )
								{
									nodeRow = xml.CreateElement("dataRow");
									nodeYData.AppendChild(nodeRow);
									XmlUtil.SetSingleNodeAttribute(xml, nodeRow, "title"   , sACTIVITY_TYPE_TERM);
									XmlUtil.SetSingleNodeAttribute(xml, nodeRow, "endLabel", nEND_LABEL.ToString());
								}
								else
								{
									if ( nodeRow.Attributes.GetNamedItem("endLabel") != null )
									{
										nEND_LABEL = Sql.ToInteger(nodeRow.Attributes.GetNamedItem("endLabel").Value);
										nEND_LABEL += nHIT_COUNT;
										if ( nEND_LABEL > nMAX_COUNT )
											nMAX_COUNT = nEND_LABEL;
										XmlUtil.SetSingleNodeAttribute(xml, nodeRow, "endLabel", nEND_LABEL.ToString());
									}
								}
								XmlNode nodeBar = xml.CreateElement("bar");
								nodeRow.AppendChild(nodeBar);
								
								XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "id"       , sTARGET_TYPE);
								XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "totalSize", nHIT_COUNT.ToString());
								
								if ( sACTIVITY_TYPE == "targeted" )
									XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "altText"  , L10n.Term("Campaigns.LBL_TARGETED") + nHIT_COUNT.ToString() + ", " + L10n.Term("Campaigns.LBL_TOTAL_TARGETED") + " " + nEND_LABEL.ToString() + ".");
								else
									XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "altText"  , nHIT_COUNT.ToString() + " " + Sql.ToString(L10n.Term(".campainglog_target_type_dom.", sTARGET_TYPE)) );
								XmlUtil.SetSingleNodeAttribute(xml, nodeBar, "url"      , "#ACTIVITY_TYPE=" + Server.UrlEncode(sACTIVITY_TYPE) + "&TARGET_TYPE=" + Server.UrlEncode(sTARGET_TYPE) );
							}
							if ( nMAX_COUNT < 10 )
								nMAX_COUNT = 10;
							XmlUtil.SetSingleNodeAttribute(xml, nodeXData, "max", nMAX_COUNT.ToString());
							XmlUtil.SetSingleNodeAttribute(xml, nodeRoot , "title", L10n.Term("Campaigns.LBL_CAMPAIGN_RESPONSE_BY_RECIPIENT_ACTIVITY"));
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
