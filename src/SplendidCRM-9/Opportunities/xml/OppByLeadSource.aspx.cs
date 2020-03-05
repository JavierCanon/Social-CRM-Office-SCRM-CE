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
	/// Summary description for OppByLeadSource.
	/// </summary>
	public class OppByLeadSource : SplendidPage
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			XmlDocument xml = new XmlDocument();
			try
			{
				// 09/15/2005 Paul.  Values will always be in the query string. 
				string[] arrASSIGNED_USER_ID = Request.QueryString.GetValues("ASSIGNED_USER_ID");
				// 09/15/2005 Paul.  Values will always be in the query string. 
				string[] arrLEAD_SOURCE = Request.QueryString.GetValues("LEAD_SOURCE");

				xml.LoadXml(SplendidCache.XmlFile(Server.MapPath(Session["themeURL"] + "PieChart.xml")));
				XmlNode nodeRoot        = xml.SelectSingleNode("graphData");
				XmlNode nodePie         = xml.CreateElement("pie"      );
				XmlNode nodeGraphInfo   = xml.CreateElement("graphInfo");
				XmlNode nodeChartColors = nodeRoot.SelectSingleNode("chartColors");

				nodeRoot.InsertBefore(nodeGraphInfo  , nodeChartColors);
				nodeRoot.InsertBefore(nodePie        , nodeGraphInfo  );
				
				XmlUtil.SetSingleNodeAttribute(xml, nodePie, "defaultAltText", L10n.Term("Dashboard.LBL_ROLLOVER_WEDGE_DETAILS"));
				
				Hashtable hashLEAD_SOURCE = new Hashtable();
				DbProviderFactory dbf = DbProviderFactories.GetFactory();
				using ( IDbConnection con = dbf.CreateConnection() )
				{
					con.Open();
					string sSQL;
					sSQL = "select LEAD_SOURCE                                   " + ControlChars.CrLf
					     + "     , LIST_ORDER                                    " + ControlChars.CrLf
					     + "     , sum(AMOUNT_USDOLLAR/1000) as TOTAL            " + ControlChars.CrLf
					     + "     , count(*)                  as OPPORTUNITY_COUNT" + ControlChars.CrLf
					     + "  from vwOPPORTUNITIES_ByLeadSource                  " + ControlChars.CrLf;
					using ( IDbCommand cmd = con.CreateCommand() )
					{
						cmd.CommandText = sSQL;
						Security.Filter(cmd, "Opportunities", "list");
						// 09/14/2005 Paul.  Use append because it supports arrays using the IN clause. 
						Sql.AppendGuids    (cmd, arrASSIGNED_USER_ID, "ASSIGNED_USER_ID");
						Sql.AppendParameter(cmd, arrLEAD_SOURCE     , "LEAD_SOURCE"     );
#if false
						if ( arrLEAD_SOURCE != null )
							nodeGraphInfo.InnerText = "LEAD_SOURCE = " + String.Join(", ", arrLEAD_SOURCE);
#endif
						cmd.CommandText += ""
						     + " group by LEAD_SOURCE                                " + ControlChars.CrLf
						     + "        , LIST_ORDER                                 " + ControlChars.CrLf
						     + " order by LIST_ORDER                                 " + ControlChars.CrLf;
						using ( IDataReader rdr = cmd.ExecuteReader() )
						{
							double dMAX_TOTAL      = 0;
							double dPIPELINE_TOTAL = 0;
							while ( rdr.Read() )
							{
								string  sLEAD_SOURCE       = Sql.ToString (rdr["LEAD_SOURCE"      ]);
								double  dTOTAL             = Sql.ToDouble (rdr["TOTAL"            ]);
								int     nOPPORTUNITY_COUNT = Sql.ToInteger(rdr["OPPORTUNITY_COUNT"]);
								
								dPIPELINE_TOTAL += dTOTAL;
								if ( dTOTAL > dMAX_TOTAL )
									dMAX_TOTAL = dTOTAL;
								// 05/27/2007 Paul.  LBL_NONE is --None--, so create a new term LBL_NONE_VALUE.
								string sLEAD_SOURCE_TERM = String.Empty;
								if ( sLEAD_SOURCE == String.Empty )
									sLEAD_SOURCE_TERM = L10n.Term(".LBL_NONE_VALUE");
								else
									sLEAD_SOURCE_TERM = Sql.ToString(L10n.Term(".lead_source_dom.", sLEAD_SOURCE));

								XmlNode nodeWedge = xml.CreateElement("bar");
								nodePie.AppendChild(nodeWedge);
								XmlUtil.SetSingleNodeAttribute(xml, nodeWedge, "title"    , sLEAD_SOURCE_TERM);
								XmlUtil.SetSingleNodeAttribute(xml, nodeWedge, "value"    , dTOTAL.ToString("0"));
								XmlUtil.SetSingleNodeAttribute(xml, nodeWedge, "color"    , SplendidDefaults.generate_graphcolor(sLEAD_SOURCE, hashLEAD_SOURCE.Count));
								XmlUtil.SetSingleNodeAttribute(xml, nodeWedge, "labelText", Strings.FormatCurrency(dTOTAL, 0, TriState.UseDefault, TriState.UseDefault, TriState.UseDefault));
								XmlUtil.SetSingleNodeAttribute(xml, nodeWedge, "url"      , Sql.ToString(Application["rootURL"]) + "Opportunities/default.aspx?LEAD_SOURCE=" + Server.UrlEncode(sLEAD_SOURCE));
								XmlUtil.SetSingleNodeAttribute(xml, nodeWedge, "altText"  , nOPPORTUNITY_COUNT.ToString() + " " + L10n.Term("Dashboard.LBL_OPPS_IN_LEAD_SOURCE") + " " + sLEAD_SOURCE_TERM);
								hashLEAD_SOURCE.Add(sLEAD_SOURCE, sLEAD_SOURCE);
							}
							XmlUtil.SetSingleNodeAttribute(xml, nodeRoot , "title"   , L10n.Term("Dashboard.LBL_TOTAL_PIPELINE") + Strings.FormatCurrency(dPIPELINE_TOTAL, 0, TriState.UseDefault, TriState.UseDefault, TriState.UseDefault) + L10n.Term("Dashboard.LBL_OPP_THOUSANDS"));
							XmlUtil.SetSingleNodeAttribute(xml, nodeRoot , "subtitle", L10n.Term("Dashboard.LBL_OPP_SIZE"  ) + " " + Strings.FormatCurrency(1.0, 0, TriState.UseDefault, TriState.UseDefault, TriState.UseDefault) + L10n.Term("Dashboard.LBL_OPP_THOUSANDS"));
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
